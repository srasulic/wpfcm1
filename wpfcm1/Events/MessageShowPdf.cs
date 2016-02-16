
namespace wpfcm1.Events
{
    public class MessageShowPdf
    {
        public MessageShowPdf(string path)
        {
            Uri = path;
        }

        public string Uri { get; set; }
    }
}
