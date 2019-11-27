using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace bethnet_cli
{
    class BuildInfo
    {
        //
        // Module: BuildInfo
        // GET /ping
        // "pong"
        //
        public static string Ping()
        {
            return FetchPage("/ping");
        }

        //
        // Module: BuildInfo
        // GET /projects/client_data/.json
        // <<<Response not documented because of length>>>
        //
        public class ProjectsClientDataResponse
        {
            public class CrashReportSettings
            {
                public bool detect_hangs { get; set; }
                public int mode { get; set; }
                public string database { get; set; }
            }

            public class Project
            {
                public string id { get; set; }
                public int branch_id { get; set; }
                public int product_id { get; set; }
                public int status_id { get; set; }
            }

            public List<int> entitlement_ids { get; set; }
            public CrashReportSettings crash_report_settings { get; set; }
            public List<Project> projects { get; set; }
        }

        public static ProjectsClientDataResponse ProjectClientData()
        {
            var jsonData = FetchPage("/projects/client_data/.json");
            return JsonConvert.DeserializeObject<ProjectsClientDataResponse>(jsonData);
        }

        //
        // Module: BuildInfo
        // GET /projects/7/.json
        // <<<Response not documented because of length>>>
        //
        public class ProjectsResponse
        {
            public class DependencyList
            {
                public int id { get; set; }
                public string name { get; set; }
                public int platform { get; set; }
                public string installer_link { get; set; }
                public int architecture { get; set; }
                public string cmdline_args { get; set; }
            }

            public string icon_link { get; set; }
            public string eula_link { get; set; }
            public bool check_filter { get; set; }
            public string support_link { get; set; }
            public List<object> storage_list { get; set; }
            public string firewall_path { get; set; }
            public bool new_chunk_format { get; set; }
            public bool require_latest { get; set; }
            public string install_registry { get; set; }
            public bool require_sysreq_check { get; set; }
            public string name { get; set; }
            public string install_folder { get; set; }
            public object launchinfo_set { get; set; }
            public List<DependencyList> dependency_list { get; set; }
            public int state { get; set; }
            public int default_branch { get; set; }
            public string firewall_label { get; set; }
            public bool has_oauth_client_id { get; set; }
            public bool new_chunk_download { get; set; }
        }

        public static ProjectsResponse Project(int ProjectId)
        {
            var jsonData = FetchPage($"/projects/{ProjectId}/.json");
            return JsonConvert.DeserializeObject<ProjectsResponse>(jsonData);
        }

        //
        // Module: BuildInfo
        // GET /projects/7/branches/10/.json
        // <<<Response not documented because of length>>>
        //
        public class ProjectsBranchesResponse
        {
            public class Filediffcontainer
            {
                public int id { get; set; }
                public int from_build { get; set; }
                public int to_build { get; set; }
            }

            public class BuildHistory
            {
                public int id { get; set; }
                public string description { get; set; }
            }

            public int project { get; set; }
            public bool available { get; set; }
            public int main_launchinfo { get; set; }
            public bool preload_ondeck { get; set; }
            public string storage_url { get; set; }
            public object on_deck_build { get; set; }
            public object dlcs { get; set; }
            public int diff_type { get; set; }
            public object preload_live_time { get; set; }
            public int build { get; set; }
            public object graphics_launchinfo { get; set; }
            public string name { get; set; }
            public List<object> tools_launchinfos { get; set; }
            public bool preload { get; set; }
            public object depot_list { get; set; }
            public List<Filediffcontainer> filediffcontainers { get; set; }
            public int branch_type { get; set; }
            public List<int> launchinfo_list { get; set; }
            public List<BuildHistory> build_history { get; set; }
            public List<int> file_diff_build_list { get; set; }
        }

        public static ProjectsResponse ProjectBranche(int ProjectId, int BranchId)
        {
            var jsonData = FetchPage($"/projects/{ProjectId}/branches/{BranchId}/.json");
            return JsonConvert.DeserializeObject<ProjectsResponse>(jsonData);
        }

        private static string FetchPage(string PageUrl, string Post = null)
        {
            // Throw a formatting error if the PageUrl is malformed
            if (string.IsNullOrEmpty(PageUrl) || PageUrl[0] != '/')
                throw new ArgumentException("PageUrl is either null or malformed");

            // Build user-agent, cookie, and default headers
            var request = WebRequest.Create("https://buildinfo.cdp.bethesda.net" + PageUrl);
            var httpRequest = (HttpWebRequest)request;

            httpRequest.Timeout = 600000;
            httpRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36";
            httpRequest.Accept = "*/*";

            // Append POST data if necessary (UTF8)
            if (Post != null)
            {
                byte[] data = Encoding.UTF8.GetBytes(Post);

                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = data.Length;

                // Write to send stream
                using (Stream dataStream = request.GetRequestStream())
                    dataStream.Write(data, 0, data.Length);
            }

            using (var response = request.GetResponse())
            using (var readStream = new StreamReader(response.GetResponseStream(), Encoding.ASCII))
            {
                // Directly read page data as ASCII string
                return readStream.ReadToEnd();
            }
        }
    }
}
