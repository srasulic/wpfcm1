using System.IO;

namespace wpfcm1.Model
{
    public class InboxDocumentModel : DocumentModel
    {
        public InboxDocumentModel()
        {
        }

        public InboxDocumentModel(FileInfo fi) : base(fi)
        {
        }


        public bool IsAckedAndSigned
        {
            get { return IsAcknowledged & HasSecondSigniture; }
        }

    }
}
