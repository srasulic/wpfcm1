using System.IO;

namespace wpfcm1.Model
{
    public class OutboxDocumentModel : DocumentModel
    {
        public OutboxDocumentModel()
        {
        }

        public OutboxDocumentModel(FileInfo fi) : base(fi)
        {
        }

    }
}
