using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace bethnet_cli
{
    partial class API
    {
        public class Session
        {
            private readonly API m_API;

            public Session(API BethNetAPI)
            {
                m_API = BethNetAPI;
            }

            //
            // Module: Session
            // POST /session/login
            // X-BNET-Agent
            // x-src-fp
            // X-Client-API-key
            // X-Platform
            // { "username": "USERNAMEHERE", "password": "PASSWORDHERE", "client_id": "12345678-1234-1234-1234-123456789012", "language": "en" }
            // {"platform": {"message": "success", "code": 2000, "response": {"username": "USERNAMEHERE", "refresh_time": 1592320906, "application_id": "12345678-1234-1234-1234-123456789012", "session_type": "basic", "external_account": {}, "buid": "12345678-1234-1234-1234-123456789012", "time_to_refresh": 299, "session_token": "ACCESSTOKEN", "time_to_expire": 599, "exp": 1534321206, "master_account_id": "12345678-1234-1234-1234-123456789012"}}}
            //
            public class LoginPost
            {
                public string username { get; set; }
                public string password { get; set; }
                public string client_id { get; set; }
                public string language { get; set; }
            }

            public class LoginResponse
            {
                public class ExternalAccount
                {
                    // Unknown
                }

                public string username { get; set; }
                public int refresh_time { get; set; }
                public string application_id { get; set; }
                public string session_type { get; set; }
                public ExternalAccount external_account { get; set; }
                public string buid { get; set; }
                public int time_to_refresh { get; set; }
                public string session_token { get; set; }
                public int time_to_expire { get; set; }
                public int exp { get; set; }
                public string master_account_id { get; set; }
            }

            public Response<LoginResponse> Login(string Username, string Password)
            {
                var input = new LoginPost
                {
                    username = Username,
                    password = Password,
                    client_id = BETHNETClientId,
                    language = BETHNETAgentLanguage,
                };

                var headers = new List<Tuple<string, string>>()
                {
                    new Tuple<string, string>("x-src-fp", m_API.m_RandomFingerprintKey),
                    new Tuple<string, string>("X-Client-API-key", BETHNETClientApiKey),
                    new Tuple<string, string>("X-Platform", BETHNETAgentPlatform),
                };

                return m_API.FetchAPI<LoginResponse>("/session/login/", JsonConvert.SerializeObject(input), headers);
            }

            //
            // Module: Session
            // GET /session/get-login-token
            // X-BNET-Agent
            // x-src-fp
            // X-Session-Token
            // {"platform": {"message": "success", "code": 2000, "response": "12345678-1234-1234-1234-123456789012"}}
            //
            public Response<string> GetLoginToken()
            {
                var headers = new List<Tuple<string, string>>()
                {
                    new Tuple<string, string>("x-src-fp", m_API.m_RandomFingerprintKey),
                    new Tuple<string, string>("X-Session-Token", m_API.m_AuthSession.session_token),
                };

                return m_API.FetchAPI<string>("/session/get-login-token", null, headers);
            }
        }
    }
}