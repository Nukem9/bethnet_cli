using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.IO;
using System.Linq;
using System.Net;

namespace bethnet_cli
{
    class Program
    {
        static void Main(string[] Args)
        {
            if (Args.Length < 3)
            {
                Console.WriteLine("Usage: ");
                Console.WriteLine("NOTE: User must own products to access decryption keys");
                Console.WriteLine();
                Console.WriteLine("list_buildinfo_entitlements\t<user> <passwd>\t\t\t\t\t\tLists all public branches and projects from BuildInfo API");
                Console.WriteLine("list_user_entitlements\t\t<user> <passwd>\t\t\t\t\t\tLists all branches and projects available for <user> from CDP API");
                Console.WriteLine("list_project_builds\t\t<user> <passwd> <project_id> <branch_id>\t\tLists all <product> builds available for <user> from CDP API");
                Console.WriteLine("download_project\t\t<user> <passwd> <project_id> <branch_id>\t\tDownload a product and all associated depots");
                Console.WriteLine("download_project_build\t\t<user> <passwd> <project_id> <branch_id> <build_id>\tDownload a specific product build and all associated depots");
                Console.WriteLine("download_mod\t\t\t<user> <passwd> <mod_id>\t\t\t\tSubscribe to and download a bethesda.net mod");
                Console.WriteLine();
                Console.WriteLine("Examples: ");
                Console.WriteLine();
                Console.WriteLine("list_buildinfo_entitlements myusername mypassword");
                Console.WriteLine("download_project myusername mypassword 10 15856");
                Console.WriteLine("download_project_build myusername mypassword 20 100148 292198");
                Console.WriteLine("download_mod myusername mypassword 911793");
                Console.WriteLine();
                return;
            }

            string command = Args[0].ToLower();
            string username = Args[1];
            string password = Args[2];

            var testAPI = new API();
            testAPI.AuthorizeUser(username, password);

            string[] newArgs = new string[Args.Length - 3];

            for (int i = 3; i < Args.Length; i++)
                newArgs[i - 3] = Args[i];

            switch (command)
            {
                case "list_buildinfo_entitlements":
                    TestDisplayAllEntitlements(testAPI, newArgs);
                    break;

                case "list_user_entitlements":
                    TestDisplayUserEntitlements(testAPI, newArgs);
                    break;

                case "list_project_builds":
                    TestDisplayProjectBuilds(testAPI, newArgs);
                    break;

                case "download_project":
                    TestDownloadProduct(testAPI, newArgs);
                    break;

                case "download_project_build":
                    TestDownloadSpecificBuild(testAPI, newArgs);
                    break;

                case "download_mod":
                    TestDownloadBethnetMod(testAPI, newArgs);
                    break;
            }
        }

        static int AESCTRDecrypt(byte[] Key, byte[] IV, byte[] Data)
        {
            IBufferedCipher cipher = CipherUtilities.GetCipher("AES/CTR/NoPadding");
            cipher.Init(false, new ParametersWithIV(ParameterUtilities.CreateKeyParameter("AES", Key), IV));

            return cipher.DoFinal(Data, Data, 0);
        }

        static void DownloadFile(string FilePath, string DataUrl, API.CDPUser.FileList FileInfo, byte[] AESKey, byte[] AESIV)
        {
            using (Stream output = File.Open(FilePath, FileMode.Create))
            {
                foreach (var chunk in FileInfo.chunk_list)
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(DataUrl + chunk.sha);
                    WebResponse response = request.GetResponse();

                    int contentLength = int.Parse(response.Headers[HttpResponseHeader.ContentLength]);

                    if (contentLength != chunk.chunk_size)
                        throw new Exception("Invalid chunk size detected");

                    using (Stream input = response.GetResponseStream())
                    using (var memoryStream = new MemoryStream())
                    {
                        input.CopyTo(memoryStream);
                        memoryStream.Position = 0;

                        AESCTRDecrypt(AESKey, AESIV, memoryStream.GetBuffer());

                        if (chunk.uncompressed_size == chunk.chunk_size)
                            memoryStream.CopyTo(output);
                        else
                        {
                            using (var zlibStream = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream(memoryStream))
                                zlibStream.CopyTo(output);
                        }
                    }
                }
            }
        }

        private static void TestDisplayAllEntitlements(API BethNet, string[] Args)
        {
            var clientData = BuildInfo.ProjectClientData();

            Console.WriteLine($"Product [ProjectId: ProjectName] [Default Branch Id]");

            foreach (var project in clientData.projects)
            {
                try
                {
                    var projectData = BuildInfo.Project(project.product_id);

                    Console.WriteLine($"Product [{project.product_id}: {projectData.name}] [{projectData.default_branch}]");
                }
                catch (Exception)
                {
                    Console.WriteLine($"Product [{project.product_id}: <<<NO NAME PERMISSION ERROR>>>]");
                }
            }
        }

        private static void TestDisplayUserEntitlements(API BethNet, string[] Args)
        {
            var userEntitlementData = BethNet.CDPUserAPI.EntitlementInfo();

            Console.WriteLine($"Product [ProjectId: ProjectName] [BranchId: BranchName]");

            foreach (var branch in userEntitlementData.branches)
            {
                var project = userEntitlementData.projects.Where(x => x.id == branch.project).First();

                Console.WriteLine($"Product [{branch.project}: {project.name}] [{branch.id}: {branch.name}]");
            }
        }

        private static void TestDisplayProjectBuilds(API BethNet, string[] Args)
        {
            int projectId = int.Parse(Args[0]); // 7
            int branchId = int.Parse(Args[1]);  // 10

            var tree = BethNet.CDPUserAPI.ProjectBranchTree(projectId, branchId);

            Console.WriteLine($"Product [ProjectId: {tree.project}] [BranchId: {tree.id}] [CurrentBuild: {tree.build}]");

            foreach (var entry in tree.build_history)
            {
                Console.WriteLine($"Build [{entry.id}]: {entry.description}");
            }
        }

        private static void TestDownloadProduct(API BethNet, string[] Args)
        {
            int projectId = int.Parse(Args[0]); // 11
            int branchId = int.Parse(Args[1]);  // 24097

            var tree = BethNet.CDPUserAPI.ProjectBranchTree(projectId, branchId);
            var depotInfo = BethNet.CDPUserAPI.ProjectBranchDepot(projectId, branchId);

            byte[] aesKey = depotInfo.ex_info_A.ToArray();
            byte[] aesIV = depotInfo.ex_info_B.Take(16).ToArray();

            foreach (var depot in tree.depot_list)
            {
                foreach (var file in depot.file_list)
                {
                    string filePath = Path.Combine(Environment.CurrentDirectory, "downloads", file.name);
                    string dataUrl = $"{tree.storage_url}{tree.project}/{depot.properties_id}/";

                    Console.WriteLine($"Downloading file {filePath}...");
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                    DownloadFile(filePath, dataUrl, file, aesKey, aesIV);
                }
            }
        }

        private static void TestDownloadSpecificBuild(API BethNet, string[] Args)
        {
            int projectId = int.Parse(Args[0]); // 7
            int branchId = int.Parse(Args[1]);  // 10
            int buildId = int.Parse(Args[2]);   // 41112

            var depotInfo = BethNet.CDPUserAPI.ProjectBranchDepotForBuild(projectId, branchId, buildId);
            var buildTree = BethNet.CDPUserAPI.ProjectBuildTree(projectId, buildId);

            byte[] aesKey = depotInfo.ex_info_A.ToArray();
            byte[] aesIV = depotInfo.ex_info_B.Take(16).ToArray();

            foreach (var depot in buildTree.depot_list)
            {
                foreach (var file in depot.file_list)
                {
                    string filePath = Path.Combine(Environment.CurrentDirectory, "downloads", file.name);
                    string dataUrl = $"{buildTree.storage_url}{projectId}/{depot.properties_id}/";

                    Console.WriteLine($"Downloading file {filePath}...");
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                    DownloadFile(filePath, dataUrl, file, aesKey, aesIV);
                }
            }
        }

        private static void TestDownloadBethnetMod(API BethNet, string[] Args)
        {
            int modId = int.Parse(Args[0]); // 911793

            BethNet.UgcAPI.ContentAddSubscription(modId);
            var modInfo = BethNet.UgcAPI.WorkshopContentGet(modId).Get().content;

            var modTreeInfo = BethNet.CDPUserAPI.ProjectBranchTree(modInfo.cdp_product_id, modInfo.cdp_branch_id);
            var modDepotInfo = BethNet.CDPUserAPI.ProjectBranchDepot(modInfo.cdp_product_id, modInfo.cdp_branch_id);

            byte[] aesKey = modDepotInfo.ex_info_A.ToArray();
            byte[] aesIV = modDepotInfo.ex_info_B.Take(16).ToArray();

            var depot = modTreeInfo.depot_list[0];

            foreach (var file in depot.file_list)
            {
                string extractDir = Path.Combine(Environment.CurrentDirectory, "downloads\\mods");
                string filePath = Path.Combine(extractDir, file.name);
                string dataUrl = $"{modTreeInfo.storage_url}{modTreeInfo.project}/{depot.properties_id}/";

                Console.WriteLine($"Downloading file {filePath}...");
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                DownloadFile(filePath, dataUrl, file, aesKey, aesIV);

                if (file.name.ToLower().Equals("mod.ckm"))
                    CKMUtil.Extract(filePath, extractDir);
            }

            BethNet.UgcAPI.ContentUnsubscribe(modId);
        }
    }
}
