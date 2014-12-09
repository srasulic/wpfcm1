
namespace wpfcm1.Events
{
    public class PdfPreviewMessage
    {
        public PdfPreviewMessage(string path)
        {
            DocumentPath = path;
        }

        public string DocumentPath { get; set; }
    }
}
