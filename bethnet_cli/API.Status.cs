namespace bethnet_cli
{
    partial class API
    {
        public class Status
        {
            private readonly API m_API;

            public Status(API BethNetAPI)
            {
                m_API = BethNetAPI;
            }

            //
            // Module: Status
            // GET /status/ext-server-status?product_id=6
            // X-BNET-Agent
            // {"platform": {"message": "success", "code": 2000, "response": {"Mods": "UP", "CreationClub": "UP"}}}
            //
            public class ExtServerStatusResponse
            {
                public string Mods { get; set; }
                public string CreationClub { get; set; }
            }

            public Response<ExtServerStatusResponse> ExtServerStatus(int ProductId)
            {
                return m_API.FetchAPI<ExtServerStatusResponse>($"/status/ext-server-status?product_id={ProductId}");
            }
        }
    }
}
