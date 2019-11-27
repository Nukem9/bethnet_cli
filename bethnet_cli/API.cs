using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace bethnet_cli
{
    partial class API
    {
        //
        // Global SDK state and variables
        //
        public static readonly string BETHNETAgentPlatform = "WINDOWS";
        public static readonly string BETHNETAgentProduct = "FALLOUT4";
        public static readonly string BETHNETAgentVersion = $"{BETHNETAgentProduct};;BDK;1.0013.99999.1;{BETHNETAgentPlatform}";
        public static readonly string BETHNETAgentLanguage = "en";

        public static readonly string BETHNETAgentURL = "https://api.bethesda.net";
        public static readonly string BETHNETClientApiKey = "FeBqmQA8wxd94RtqymKwzmtcQcaA5KHOpDkQBSegx4WePeluZTCIm5scoeKTbmGl";
        public static readonly string BETHNETClientId = "95578d65-45bf-4a03-b7f7-a43d29b9467d";

        //
        // Internal SDK state
        //
        private string m_RandomFingerprintKey;
        private bool m_Authorized;

        private Status m_Status;
        private Beam m_Beam;
        private CDPUser m_CDPUser;
        private Session m_Session;
        private Ugc m_Ugc;

        private Beam.AccountsLoginResponse m_AuthBeam;
        private CDPUser.AuthResponse m_AuthCDPUser;
        private Session.LoginResponse m_AuthSession;
        private string m_AuthSessionLoginToken;

        public API()
        {
            byte[] keyBytes = new byte[20];

            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
                rng.GetBytes(keyBytes);

            m_RandomFingerprintKey = string.Concat(Array.ConvertAll(keyBytes, x => x.ToString("X2")));
            m_Authorized = false;

            m_Status = new Status(this);
            m_Beam = new Beam(this);
            m_CDPUser = new CDPUser(this);
            m_Session = new Session(this);
            m_Ugc = new Ugc(this);
        }

        public bool AuthorizeUser(string Username, string Password)
        {
            m_Authorized = true;

            // These are in a specific order
            m_AuthBeam = m_Beam.AccountsLogin(Username, Password);
            m_AuthCDPUser = m_CDPUser.Auth();
            m_AuthSession = m_Session.Login(Username, Password).Get();
            m_AuthSessionLoginToken = m_Session.GetLoginToken().Get();

            return true;
        }

        public Status StatusAPI
        {
            get
            {
                if (!m_Authorized)
                    return null;

                return m_Status;
            }
        }

        public Beam BeamAPI
        {
            get
            {
                if (!m_Authorized)
                    return null;

                return m_Beam;
            }
        }

        public CDPUser CDPUserAPI
        {
            get
            {
                if (!m_Authorized)
                    return null;

                return m_CDPUser;
            }
        }

        public Session SessionAPI
        {
            get
            {
                if (!m_Authorized)
                    return null;

                return m_Session;
            }
        }

        public Ugc UgcAPI
        {
            get
            {
                if (!m_Authorized)
                    return null;

                return m_Ugc;
            }
        }

        public class ResponsePlatform<T>
        {
            public string message { get; set; }
            public int code { get; set; }
            public T response { get; set; }
        }

        public class Response<T>
        {
            public ResponsePlatform<T> platform { get; set; }

            public T Get()
            {
                return platform.response;
            }
        }

        public Response<T> FetchAPI<T>(string PageUrl, string Post = null, List<Tuple<string, string>> Headers = null) where T : class
        {
            var json = JsonConvert.DeserializeObject<Response<T>>(FetchPage(PageUrl, Post, Headers));

            if (json.platform == null || json.platform.code != 2000)
                throw new Exception();

            return json;
        }

        public string FetchPage(string PageUrl, string Post = null, List<Tuple<string, string>> Headers = null)
        {
            using (var response = CreatePageRequestRaw(PageUrl, Post, Headers).GetResponse())
            using (var readStream = new StreamReader(response.GetResponseStream(), Encoding.ASCII))
            {
                // Directly read page data as ASCII string
                return readStream.ReadToEnd();
            }
        }

        public WebRequest CreatePageRequestRaw(string PageUrl, string Post = null, List<Tuple<string, string>> Headers = null)
        {
            // Throw a formatting error if the PageUrl is malformed
            if (string.IsNullOrEmpty(PageUrl) || PageUrl[0] != '/')
                throw new ArgumentException("PageUrl is either null or malformed");

            if (!m_Authorized)
                throw new UnauthorizedAccessException("Attempting to make requests without being logged in");

            // Build user-agent, cookie, and default headers
            var request = WebRequest.Create(BETHNETAgentURL + PageUrl);
            var httpRequest = (HttpWebRequest)request;

            httpRequest.Timeout = 600000;
            httpRequest.UserAgent = "bnet";
            httpRequest.Accept = "application/json";
            request.Headers.Add("X-BNET-Agent", BETHNETAgentVersion);

            // Optional headers since none of the requests are consistent
            if (Headers != null)
            {
                foreach (var header in Headers)
                    request.Headers.Add(header.Item1, header.Item2);
            }

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

            return request;
        }
    }
}