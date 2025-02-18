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

    public class Payload
    {
        public string tenant { get; set; }
        public string tip_dok { get; set; }
        public string teh_naziv_fajla { get; set; }
        public string sadrzaj { get; set; }
    }

    public class Document
    {
        public string tenant { get; set; }
        public string id_dokument { get; set; }
        public string tip_dok { get; set; }
        public string tenant_izv { get; set; }
        public string tenant_odr { get; set; }
        public string br_dok { get; set; }
        public string status { get; set; }
        public string status_s { get; set; }
        public string status_ss { get; set; }
        public string status_sss { get; set; }
        public string dat_kreiranja { get; set; }
        public string dat_pripreme { get; set; }
        public string id_edok_status { get; set; }
        public string naziv { get; set; }
        public string opis { get; set; }
        public string teh_naziv { get; set; }
        public string id_fajl { get; set; }
    }

    public class DocumentsResult
    {
        public Result result { get; set; }
        public int start_index { get; set; }
        public int index_count { get; set; }
        public List<Document> collection { get; set; }
    }

    public class DownloadResult
    {
        public Result result { get; set; }
        public string content { get; set; }
    }

}
