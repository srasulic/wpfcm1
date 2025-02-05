using System.Collections.Generic;

namespace wpfcm1.OlympusApi
{
    public class UsersMe
    {
        public string id_operater { get; set; }
    }

    public class Token
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
    }

    public class Tenant
    {
        public string tenant { get; set; }
        public string naziv { get; set; }
        public string polisign { get; set; }
    }

    public class Result
    {
        public int code { get; set; }
        public string message { get; set; }
        public string userMessage { get; set; }
    }


    public class TenantsResult
    {
        public Result result { get; set; }
        public List<Tenant> tenants { get; set; }
    }

    public class TenantInfo
    {
        public string tenant { get; set; }
        public string ib { get; set; }
        public string naziv { get; set; }
        public string ext_tenant_id { get; set; }
        public string inbox_node_id { get; set; }
        public string ts_url { get; set; }
        public string ts_username { get; set; }
        public string ts_pass { get; set; }
        public string arch_polisa { get; set; }
        public int X_SIG_SHIFT_LEVI { get; set; }
        public int Y_SIG_SHIFT_LEVI { get; set; }
        public int X_SIG_SHIFT_DESNI { get; set; }
        public int Y_SIG_SHIFT_DESNI { get; set; }
    }

    public class TipDokPristup
    {
        public string tip_dok { get; set; }
        public string smer { get; set; }
    }

    public class Profile
    {
        public TenantInfo tenant_info { get; set; }
        public List<TipDokPristup> tip_dok_pristup { get; set; }
    }

    public class ProfileResult
    {
        public Result result { get; set; }
        public Profile profile { get; set; }
    }

}
