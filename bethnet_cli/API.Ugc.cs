using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace bethnet_cli
{
    partial class API
    {
        public class Ugc
        {
            private readonly API m_API;

            public Ugc(API BethNetAPI)
            {
                m_API = BethNetAPI;
            }

            //
            // Module: Ugc Content
            // POST /mods/ugc-content/add-subscription
            // X-BNET-Agent
            // x-src-fp
            // X-Access-Token
            // { "content_id": "100001892" }
            // {"platform": {"message": "success", "code": 2000}}
            //
            public class ContentAddSubscriptionInput
            {
                public int content_id { get; set; }
            }

            public Response<object> ContentAddSubscription(int ContentId)
            {
                var input = new ContentAddSubscriptionInput
                {
                    content_id = ContentId,
                };

                var headers = new List<Tuple<string, string>>()
                {
                    new Tuple<string, string>("x-src-fp", m_API.m_RandomFingerprintKey),
                    new Tuple<string, string>("X-Access-Token", m_API.m_AuthBeam.access_token),
                };

                return m_API.FetchAPI<object>("/mods/ugc-content/add-subscription", JsonConvert.SerializeObject(input), headers);
            }

            //
            // Module: Ugc Content
            // POST /mods/ugc-content/unsubscribe
            // X-BNET-Agent
            // x-src-fp
            // X-Access-Token
            // { "content_id": "100001892" }
            // {"platform": {"message": "success", "code": 2000}}
            //
            public class ContentUnsubscribeInput
            {
                public int content_id { get; set; }
            }

            public Response<object> ContentUnsubscribe(int ContentId)
            {
                var input = new ContentUnsubscribeInput
                {
                    content_id = ContentId,
                };

                var headers = new List<Tuple<string, string>>()
                {
                    new Tuple<string, string>("x-src-fp", m_API.m_RandomFingerprintKey),
                    new Tuple<string, string>("X-Access-Token", m_API.m_AuthBeam.access_token),
                };

                return m_API.FetchAPI<object>("/mods/ugc-content/unsubscribe", JsonConvert.SerializeObject(input), headers);
            }

            //
            // Module: Ugc Static
            // GET /mods/ugc-static/list/categories?product=FALLOUT4;cc_mod=true
            // X-BNET-Agent
            // {"platform": {"message": "success", "code": 2000, "response": {"categories": ["Weapons", "Apparel", "World", "Characters", "Creatures", "Gameplay", "Skins"]}}}
            //
            public class StaticListCategoriesResponse
            {
                public List<string> categories { get; set; }
            }

            public Response<StaticListCategoriesResponse> StaticListCategories(string Product = "FALLOUT4", bool CCMod = true)
            {
                return m_API.FetchAPI<StaticListCategoriesResponse>($"/mods/ugc-static/list/categories?product={Product};cc_mod={CCMod}");
            }

            //
            // Module: Ugc Workshop
            // GET /mods/ugc-workshop/blacklist?platform=WINDOWS;product=FALLOUT4
            // X-BNET-Agent
            // x-src-fp
            // {"platform": {"message": "success", "code": 2000, "response": {"content": [], "total_results_count": 0}}}
            //
            public class WorkshopBlacklistResponse
            {
                public List<object> content { get; set; }
                public int total_results_count { get; set; }
            }

            public Response<WorkshopBlacklistResponse> WorkshopBlacklist(string Platform = "WINDOWS", string Product = "FALLOUT4")
            {
                return m_API.FetchAPI<WorkshopBlacklistResponse>($"/mods/ugc-workshop/blacklist?platform={Platform};product={Product}");
            }

            //
            // Module: Ugc Workshop
            // GET /mods/ugc-workshop/list?page=1;sort=alpha;order=asc;number_results=500;platform=WINDOWS;product=FALLOUT4;category=%5B%22Creatures%22%5D;cc_mod=true
            // X-BNET-Agent
            // x-src-fp
            // X-Access-Token
            // X-BNET-Language
            // <<<Response not documented because of length>>>
            //
            public class WorkshopListResponse
            {
                public class Price
                {
                    public int currency_id { get; set; }
                    public string currency_type { get; set; }
                    public bool sale { get; set; }
                    public string currency_name { get; set; }
                    public int price_id { get; set; }
                    public int amount { get; set; }
                    public int original_amount { get; set; }
                    public string currency_external_id { get; set; }
                }

                public class Content
                {
                    public double rating { get; set; }
                    public int version { get; set; }
                    public int depot_size { get; set; }
                    public bool is_subscribed { get; set; }
                    public int preview_file_size { get; set; }
                    public string preview_file_url { get; set; }
                    public bool is_following { get; set; }
                    public bool wip { get; set; }
                    public bool is_published { get; set; }
                    public int user_rating { get; set; }
                    public List<string> platform { get; set; }
                    public int state { get; set; }
                    public int rating_count { get; set; }
                    public int cdp_branch_id { get; set; }
                    public string type { get; set; }
                    public string username { get; set; }
                    public string product { get; set; }
                    public DateTime updated { get; set; }
                    public bool cc_mod { get; set; }
                    public bool bundle { get; set; }
                    public List<Price> prices { get; set; }
                    public bool is_public { get; set; }
                    public string name { get; set; }
                    public int catalog_item_id { get; set; }
                    public bool is_auto_moderated { get; set; }
                    public string content_id { get; set; }
                    public int cdp_product_id { get; set; }
                }

                public List<string> product { get; set; }
                public int total_results_count { get; set; }
                public List<Content> content { get; set; }
                public List<string> platform { get; set; }
                public List<string> categories { get; set; }
                public List<bool> wip { get; set; }
                public int page_results_count { get; set; }
                public int page { get; set; }
            }

            public Response<WorkshopListResponse> WorkshopList(int Page = 1, string Sort = "alpha", string Order = "asc", int MaxResults = 500, string Platform = "WINDOWS", string Product = "FALLOUT4", string Category = "%5B%22Creatures%22%5D", bool CCMod = true)
            {
                var headers = new List<Tuple<string, string>>()
                {
                    new Tuple<string, string>("x-src-fp", m_API.m_RandomFingerprintKey),
                    new Tuple<string, string>("X-Access-Token", m_API.m_AuthBeam.access_token),
                    new Tuple<string, string>("X-BNET-Language", BETHNETAgentLanguage),
                };

                return m_API.FetchAPI<WorkshopListResponse>($"/mods/ugc-workshop/list?page={Page};sort={Sort};order={Order};number_results={MaxResults};platform={Platform};product={Product};category={Category};cc_mod={CCMod}", null, headers);
            }

            //
            // Module: Ugc Workshop
            // GET /mods/ugc-workshop/content/get?content_id=100000435
            // X-BNET-Agent
            // x-src-fp
            // X-Access-Token
            // X-BNET-Language
            // <<<Response not documented because of length>>>
            //
            public class WorkshopContentGetResponse
            {
                public class Content
                {
                    public class RequiredMod
                    {
                        public string username { get; set; }
                        public double rating { get; set; }
                        public int preview_file_size { get; set; }
                        public string avatar_image_url { get; set; }
                        public string name { get; set; }
                        public int rating_count { get; set; }
                        public int depot_size { get; set; }
                        public bool is_subscribed { get; set; }
                        public string preview_file_url { get; set; }
                        public int version { get; set; }
                        public bool is_available { get; set; }
                        public int cdp_branch_id { get; set; }
                        public bool is_following { get; set; }
                        public string author_id { get; set; }
                        public string content_id { get; set; }
                        public int cdp_product_id { get; set; }
                    }

                    public class WINDOWS
                    {
                        public int depot_size { get; set; }
                        public int state { get; set; }
                        public int version { get; set; }
                        public int cdp_branch_id { get; set; }
                        public string version_name { get; set; }
                        public int cdp_product_id { get; set; }
                    }

                    public class PlatformData
                    {
                        public WINDOWS WINDOWS { get; set; }
                    }

                    public class ReleaseNote
                    {
                        public string updated { get; set; }
                        public string note_id { get; set; }
                        public string description { get; set; }
                        public string platform { get; set; }
                        public int version { get; set; }
                        public string published { get; set; }
                        public string version_name { get; set; }
                    }

                    public object rating_count_details { get; set; }
                    public double rating { get; set; }
                    public int rating_count { get; set; }
                    public int depot_size { get; set; }
                    public bool is_subscribed { get; set; }
                    public DateTime mark_public_date { get; set; }
                    public DateTime updated { get; set; }
                    public string preview_file_url { get; set; }
                    public bool is_following { get; set; }
                    public bool wip { get; set; }
                    public List<RequiredMod> required_mods { get; set; }
                    public int view_count { get; set; }
                    public bool is_blacklisted { get; set; }
                    public string avatar_image_url { get; set; }
                    public PlatformData platform_data { get; set; }
                    public object media { get; set; }
                    public int user_rating { get; set; }
                    public List<string> platform { get; set; }
                    public int version { get; set; }
                    public int cdp_branch_id { get; set; }
                    public int follower_count { get; set; }
                    public bool is_in_moderation { get; set; }
                    public IList<ReleaseNote> release_notes { get; set; }
                    public string username { get; set; }
                    public string product { get; set; }
                    public string description { get; set; }
                    public List<object> tags { get; set; }
                    public int entitlement_id { get; set; }
                    public int cdp_product_id { get; set; }
                    public bool cc_mod { get; set; }
                    public bool bundle { get; set; }
                    public bool reported { get; set; }
                    public List<object> required_dlc { get; set; }
                    public bool can_mark_as_public { get; set; }
                    public bool is_public { get; set; }
                    public bool is_author { get; set; }
                    public string slug { get; set; }
                    public List<string> categories { get; set; }
                    public string name { get; set; }
                    public int download_count_all { get; set; }
                    public bool is_deleted { get; set; }
                    public bool user_can_moderate_comments { get; set; }
                    public string type { get; set; }
                    public int like_count { get; set; }
                    public bool commenting_enabled { get; set; }
                    public int download_count_week { get; set; }
                    public bool is_auto_moderated { get; set; }
                    public DateTime published { get; set; }
                    public int follow_entitlement_id { get; set; }
                    public string author_id { get; set; }
                    public string content_id { get; set; }
                    public int preview_file_size { get; set; }
                    public bool editable { get; set; }
                    public bool is_published { get; set; }
                }

                public Content content { get; set; }
            }

            public Response<WorkshopContentGetResponse> WorkshopContentGet(int ContentId)
            {
                var headers = new List<Tuple<string, string>>()
                {
                    new Tuple<string, string>("x-src-fp", m_API.m_RandomFingerprintKey),
                    new Tuple<string, string>("X-Access-Token", m_API.m_AuthBeam.access_token),
                    new Tuple<string, string>("X-BNET-Language", BETHNETAgentLanguage),
                };

                return m_API.FetchAPI<WorkshopContentGetResponse>($"/mods/ugc-workshop/content/get?content_id={ContentId}", null, headers);
            }
        }
    }
}
