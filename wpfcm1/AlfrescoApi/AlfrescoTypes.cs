namespace wpfcm1.AlfrescoApi
{
    public class Error
    {
        public string errorKey { get; set; }
        public int statusCode { get; set; }
        public string briefSummary { get; set; }
        public string stackTrace { get; set; }
        public string descriptionURL { get; set; }
        public string logId { get; set; }
    }
}
