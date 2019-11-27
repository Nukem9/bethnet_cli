using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace bethnet_cli
{
    partial class API
    {
        public class Beam
        {
            private readonly API m_API;

            public Beam(API BethNetAPI)
            {
                m_API = BethNetAPI;
            }

            //
            // Module: Beam
            // POST /beam/accounts/login/USERNAMEHERE
            // X-BNET-Agent
            // X-Client-API-key
            // x-src-fp
            // X-Platform
            // {"password": "PASSWORDHERE", "language": "en"}
            // {"access_token":"REDACTED1","account":{"admin":false,"admin_read_only":false,"id":"REDACTED2","mfa_enabled":false,"sms_enabled_number":null,"username":"USERNAMEHERE"}}
            //
            public class AccountsLoginPost
            {
                public string password { get; set; }
                public string language { get; set; }
            }

            public class AccountsLoginResponse
            {
                public class Account
                {
                    public bool admin { get; set; }
                    public bool admin_read_only { get; set; }
                    public string id { get; set; }
                    public bool mfa_enabled { get; set; }
                    public object sms_enabled_number { get; set; }
                    public string username { get; set; }
                }

                public string access_token { get; set; }
                public Account account { get; set; }
            }

            public AccountsLoginResponse AccountsLogin(string Username, string Password)
            {
                var input = new AccountsLoginPost
                {
                    password = Password,
                    language = BETHNETAgentLanguage,
                };

                var headers = new List<Tuple<string, string>>()
                {
                    new Tuple<string, string>("X-Client-API-key", BETHNETClientApiKey),
                    new Tuple<string, string>("x-src-fp", m_API.m_RandomFingerprintKey),
                    new Tuple<string, string>("X-Platform", BETHNETAgentPlatform),
                };

                var jsonData = m_API.FetchPage($"/beam/accounts/login/{Username}", JsonConvert.SerializeObject(input), headers);
                return JsonConvert.DeserializeObject<AccountsLoginResponse>(jsonData);
            }
        }
    }
}
