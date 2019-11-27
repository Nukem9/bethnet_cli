using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace bethnet_cli
{
    partial class API
    {
        public class CDPUser
        {
            private readonly API m_API;

            public CDPUser(API BethNetAPI)
            {
                m_API = BethNetAPI;
            }

            #region Common structures
            public class ChunkList
            {
                public long chunk_size { get; set; }
                public long uncompressed_size { get; set; }
                public int index { get; set; }
                public string sha { get; set; }
            }

            public class FileList
            {
                public int file_id { get; set; }
                public string name { get; set; }
                public string sha { get; set; }
                public long file_size { get; set; }
                public long compressed_size { get; set; }
                public int chunk_count { get; set; }
                public bool modifiable { get; set; }
                public List<ChunkList> chunk_list { get; set; }
            }

            public class ProjectBranchDepotResponse
            {
                public int architecture { get; set; }
                public int build { get; set; }
                public long download_size { get; set; }
                public int depot_type { get; set; }
                public int platform { get; set; }
                public bool default_region { get; set; }
                public List<byte> ex_info_A { get; set; }
                public int encryption_type { get; set; }
                public string name { get; set; }
                public int language { get; set; }
                public bool default_language { get; set; }
                public int id { get; set; }
                public long size_on_disk { get; set; }
                public int compression_type { get; set; }
                public int properties_id { get; set; }
                public int region { get; set; }
                public bool is_dlc { get; set; }
                public int deployment_order { get; set; }
                public List<byte> ex_info_B { get; set; }
                public int bytes_per_chunk { get; set; }
            }
            #endregion

            //
            // Module: CDP-User
            // GET /cdp-user/version/.json
            // X-BNET-Agent
            // {"NAME": "", "TAG": "", "BRANCH": "release-1.58", "VERSION": [1, 58, 1], "MIN_CLIENT_LIB_VERSION": [], "BUILD_TIME": "Mon Nov 04 20:20:51 UTC 2019", "DIRTY": false, "COMMIT": "a7409bc28e236958dbe2607ba24ad12c2f8c097a"}
            //
            public class VersionResponse
            {
                public string NAME { get; set; }
                public string TAG { get; set; }
                public string BRANCH { get; set; }
                public List<int> VERSION { get; set; }
                public List<object> MIN_CLIENT_LIB_VERSION { get; set; }
                public string BUILD_TIME { get; set; }
                public bool DIRTY { get; set; }
                public string COMMIT { get; set; }
            }

            public VersionResponse Version()
            {
                return JsonConvert.DeserializeObject<VersionResponse>(m_API.FetchPage("/cdp-user/version/.json"));
            }

            //
            // Module: CDP-User (Beam)
            // POST /cdp-user/auth
            // X-BNET-Agent
            // x-src-fp
            // x-cdp-app
            // x-cdp-app-ver
            // x-cdp-lib-ver
            // x-cdp-platform
            // { "access_token": "REDACTED" }
            // { "entitlement_ids": [321920, 289158, 166164, 0], "beam_client_api_key": "REDACTED1", "beam_token": ["", "REDACTED2", ""], "oauth_token": "unknown" }
            //
            public class AuthPost
            {
                public string access_token { get; set; }
            }

            public class AuthResponse
            {
                public List<int> entitlement_ids { get; set; }
                public string beam_client_api_key { get; set; }
                public string session_id { get; set; }
                public string token { get; set; }
                public List<string> beam_token { get; set; }
                public string oauth_token { get; set; }
            }

            public AuthResponse Auth()
            {
                var input = new AuthPost
                {
                    access_token = m_API.m_AuthBeam.access_token,
                };

                var headers = new List<Tuple<string, string>>()
                {
                    new Tuple<string, string>("x-src-fp", m_API.m_RandomFingerprintKey),
                    new Tuple<string, string>("x-cdp-app", "UGC SDK"),
                    new Tuple<string, string>("x-cdp-app-ver", "0.9.11314/debug"),
                    new Tuple<string, string>("x-cdp-lib-ver", "0.9.11314/debug"),
                    new Tuple<string, string>("x-cdp-platform", "Win/32"),
                };

                var jsonData = m_API.FetchPage("/cdp-user/auth", JsonConvert.SerializeObject(input), headers);
                return JsonConvert.DeserializeObject<AuthResponse>(jsonData);
            }

            //
            // Module: CDP-User
            // POST /cdp-user/entitlement_info/.json
            // X-BNET-Agent
            // x-cdp-app
            // x-cdp-app-ver
            // x-cdp-lib-ver
            // x-cdp-platform
            // <<<Response not documented because of length>>>
            //
            public class EntitlementInfoPost
            {
                public List<int> entitlement_ids;
            }

            public class EntitlementInfoResponse
            {
                public class Blacklist
                {
                    public string ip { get; set; }
                    public List<Branch> branches { get; set; }
                    public List<Project> projects { get; set; }
                    public string country { get; set; }
                }

                public class Branch
                {
                    public int id { get; set; }
                    public string name { get; set; }
                    public int branch_type { get; set; }
                    public int project { get; set; }
                    public int? build { get; set; }
                    public bool preload { get; set; }
                    public bool available { get; set; }
                }

                public class Project
                {
                    public int id { get; set; }
                    public string name { get; set; }
                    public int? default_branch { get; set; }
                    public bool new_chunk_format { get; set; }
                    public bool new_chunk_download { get; set; }
                    public bool buildinfo { get; set; }
                    public bool beam_client_key { get; set; }
                    public bool has_contentmirror { get; set; }
                }

                public Blacklist blacklist { get; set; }
                public List<Branch> branches { get; set; }
                public List<Project> projects { get; set; }
            }

            public EntitlementInfoResponse EntitlementInfo()
            {
                // TODO? These don't ever seem to be used.....
                var input = new EntitlementInfoPost
                {
                    entitlement_ids = new List<int>() { 0 },
                };

                var headers = new List<Tuple<string, string>>()
                {
                    //new Tuple<string, string>("x-src-fp", m_API.m_RandomFingerprintKey),
                    new Tuple<string, string>("x-cdp-app", "UGC SDK"),
                    new Tuple<string, string>("x-cdp-app-ver", "0.9.11314/debug"),
                    new Tuple<string, string>("x-cdp-lib-ver", "0.9.11314/debug"),
                    new Tuple<string, string>("x-cdp-platform", "Win/32"),
                };

                var jsonData = m_API.FetchPage("/cdp-user/entitlement_info/.json", JsonConvert.SerializeObject(input), headers);
                return JsonConvert.DeserializeObject<EntitlementInfoResponse>(jsonData);
            }

            //
            // Module: CDP-User
            // GET /cdp-user/projects/10/branches/35548/tree/.json
            // Authorization
            // X-BNET-Agent
            // x-src-fp
            // x-cdp-app
            // x-cdp-app-ver
            // x-cdp-lib-ver
            // x-cdp-platform
            // <<<Response not documented because of length>>>
            //
            public class ProjectBranchTreeResponse
            {
                public class BuildHistory
                {
                    public int id { get; set; }
                    public string description { get; set; }
                }

                public class BuildFields
                {
                    public int id { get; set; }
                    public string name { get; set; }
                    public DateTime create_date { get; set; }
                    public string description { get; set; }
                    public int build_type { get; set; }
                    public bool locked { get; set; }
                    public string storage_key { get; set; }
                    public object storage { get; set; }
                    public bool major { get; set; }
                }

                public class DepotList
                {
                    public int platform { get; set; }
                    public int id { get; set; }
                    public long download_size { get; set; }
                    public bool is_dlc { get; set; }
                    public int language { get; set; }
                    public bool default_language { get; set; }
                    public int region { get; set; }
                    public int properties_id { get; set; }
                    public int build { get; set; }
                    public int compression_type { get; set; }
                    public int depot_type { get; set; }
                    public List<FileList> file_list { get; set; }
                    public int bytes_per_chunk { get; set; }
                    public int deployment_order { get; set; }
                    public string name { get; set; }
                    public long size_on_disk { get; set; }
                    public bool default_region { get; set; }
                    public int encryption_type { get; set; }
                    public int architecture { get; set; }
                }

                public int id { get; set; }
                public string name { get; set; }
                public int entitlement_id { get; set; }
                public int branch_type { get; set; }
                public int project { get; set; }
                public int build { get; set; }
                public object on_deck_build { get; set; }
                public bool available { get; set; }
                public bool preload { get; set; }
                public bool preload_ondeck { get; set; }
                public object preload_live_time { get; set; }
                public int diff_type { get; set; }
                public int build_history_length { get; set; }
                public List<object> tools_launchinfos { get; set; }
                public object main_launchinfo { get; set; }
                public object graphics_launchinfo { get; set; }
                public bool promote_ondeck_after_diff { get; set; }
                public string storage_url { get; set; }
                public List<object> file_diff_build_list { get; set; }
                public List<object> filediffcontainers { get; set; }
                public List<BuildHistory> build_history { get; set; }
                public BuildFields build_fields { get; set; }
                public List<DepotList> depot_list { get; set; }
            }

            public ProjectBranchTreeResponse ProjectBranchTree(int ProjectId, int BranchId)
            {
                var headers = new List<Tuple<string, string>>()
                {
                    new Tuple<string, string>("Authorization", $"Token {m_API.m_AuthCDPUser.token}"),
                    new Tuple<string, string>("x-src-fp", m_API.m_RandomFingerprintKey),
                    new Tuple<string, string>("x-cdp-app", "UGC SDK"),
                    new Tuple<string, string>("x-cdp-app-ver", "0.9.11314/debug"),
                    new Tuple<string, string>("x-cdp-lib-ver", "0.9.11314/debug"),
                    new Tuple<string, string>("x-cdp-platform", "Win/32"),
                };

                var jsonData = m_API.FetchPage($"/cdp-user/projects/{ProjectId}/branches/{BranchId}/tree/.json", null, headers);
                return JsonConvert.DeserializeObject<ProjectBranchTreeResponse>(jsonData);
            }

            //
            // Module: CDP-User
            // GET /cdp-user/projects/7/builds/392467/tree/.json
            // Authorization
            // X-BNET-Agent
            // x-src-fp
            // x-cdp-app
            // x-cdp-app-ver
            // x-cdp-lib-ver
            // x-cdp-platform
            // <<<Response not documented because of length>>>
            //
            public class ProjectBuildTreeResponse
            {
                public class DepotList
                {
                    public long size_on_disk { get; set; }
                    public bool default_region { get; set; }
                    public long download_size { get; set; }
                    public int depot_type { get; set; }
                    public int encryption_type { get; set; }
                    public int deployment_order { get; set; }
                    public int properties_id { get; set; }
                    public int region { get; set; }
                    public int language { get; set; }
                    public List<FileList> file_list { get; set; }
                    public int id { get; set; }
                    public bool is_dlc { get; set; }
                    public bool default_language { get; set; }
                    public int bytes_per_chunk { get; set; }
                    public int build { get; set; }
                    public int compression_type { get; set; }
                    public int platform { get; set; }
                    public int architecture { get; set; }
                    public string name { get; set; }
                }

                public int id { get; set; }
                public string name { get; set; }
                public DateTime create_date { get; set; }
                public string description { get; set; }
                public int build_type { get; set; }
                public bool locked { get; set; }
                public string storage_key { get; set; }
                public object storage { get; set; }
                public bool major { get; set; }
                public string storage_url { get; set; }
                public List<DepotList> depot_list { get; set; }
            }

            public ProjectBuildTreeResponse ProjectBuildTree(int ProjectId, int BuildId)
            {
                var headers = new List<Tuple<string, string>>()
                {
                    new Tuple<string, string>("Authorization", $"Token {m_API.m_AuthCDPUser.token}"),
                    new Tuple<string, string>("x-src-fp", m_API.m_RandomFingerprintKey),
                    new Tuple<string, string>("x-cdp-app", "UGC SDK"),
                    new Tuple<string, string>("x-cdp-app-ver", "0.9.11314/debug"),
                    new Tuple<string, string>("x-cdp-lib-ver", "0.9.11314/debug"),
                    new Tuple<string, string>("x-cdp-platform", "Win/32"),
                };

                var jsonData = m_API.FetchPage($"/cdp-user/projects/{ProjectId}/builds/{BuildId}/tree/.json", null, headers);
                return JsonConvert.DeserializeObject<ProjectBuildTreeResponse>(jsonData);
            }

            //
            // Module: CDP-User
            // GET /cdp-user/projects/10/branches/35548/depots/.json
            // Authorization
            // X-BNET-Agent
            // x-src-fp
            // x-cdp-app
            // x-cdp-app-ver
            // x-cdp-lib-ver
            // x-cdp-platform
            // {"450":{"architecture":0,"build":425,"download_size":153822622,"depot_type":2,"platform":0,"default_region":false,"ex_info_A":[REDACTED],"encryption_type":1,"name":"Depot_Name_100000548","language":1,"default_language":false,"id":450,"size_on_disk":154069595,"compression_type":1,"properties_id":350,"region":1,"is_dlc":false,"deployment_order":0,"ex_info_B":[REDACTED],"bytes_per_chunk":1048576}}
            //
            public ProjectBranchDepotResponse ProjectBranchDepot(int ProjectId, int BranchId)
            {
                var headers = new List<Tuple<string, string>>()
                {
                    new Tuple<string, string>("Authorization", $"Token {m_API.m_AuthCDPUser.token}"),
                    new Tuple<string, string>("x-src-fp", m_API.m_RandomFingerprintKey),
                    new Tuple<string, string>("x-cdp-app", "UGC SDK"),
                    new Tuple<string, string>("x-cdp-app-ver", "0.9.11314/debug"),
                    new Tuple<string, string>("x-cdp-lib-ver", "0.9.11314/debug"),
                    new Tuple<string, string>("x-cdp-platform", "Win/32"),
                };

                var jsonData = m_API.FetchPage($"/cdp-user/projects/{ProjectId}/branches/{BranchId}/depots/.json", null, headers);

                // Requires a fixup because the first json member is a number (impossible to represent in C#)
                var rawJson = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(jsonData);
                var json = rawJson.First.First.ToObject<ProjectBranchDepotResponse>();

                return json;
            }

            //
            // Module: CDP-User
            // GET /cdp-user/projects/7/branches/10/depots/for_build/41112/.json
            // Authorization
            // X-BNET-Agent
            // x-src-fp
            // x-cdp-app
            // x-cdp-app-ver
            // x-cdp-lib-ver
            // x-cdp-platform
            // {"450":{"architecture":0,"build":425,"download_size":153822622,"depot_type":2,"platform":0,"default_region":false,"ex_info_A":[REDACTED],"encryption_type":1,"name":"Depot_Name_100000548","language":1,"default_language":false,"id":450,"size_on_disk":154069595,"compression_type":1,"properties_id":350,"region":1,"is_dlc":false,"deployment_order":0,"ex_info_B":[REDACTED],"bytes_per_chunk":1048576}}
            //
            public ProjectBranchDepotResponse ProjectBranchDepotForBuild(int ProjectId, int BranchId, int BuildId)
            {
                var headers = new List<Tuple<string, string>>()
                {
                    new Tuple<string, string>("Authorization", $"Token {m_API.m_AuthCDPUser.token}"),
                    new Tuple<string, string>("x-src-fp", m_API.m_RandomFingerprintKey),
                    new Tuple<string, string>("x-cdp-app", "UGC SDK"),
                    new Tuple<string, string>("x-cdp-app-ver", "0.9.11314/debug"),
                    new Tuple<string, string>("x-cdp-lib-ver", "0.9.11314/debug"),
                    new Tuple<string, string>("x-cdp-platform", "Win/32"),
                };

                var jsonData = m_API.FetchPage($"/cdp-user/projects/{ProjectId}/branches/{BranchId}/depots/for_build/{BuildId}/.json", null, headers);

                // Requires a fixup because the first json member is a number (impossible to represent in C#)
                var rawJson = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(jsonData);
                var json = rawJson.First.First.ToObject<ProjectBranchDepotResponse>();

                return json;
            }
        }
    }
}