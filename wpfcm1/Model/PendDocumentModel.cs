using System.IO;

namespace wpfcm1.Model
{
    public class PendDocumentModel : DocumentModel
    {
        public PendDocumentModel()
        {
        }

        public PendDocumentModel(FileInfo fi) : base(fi)
        {
        }

        private bool _isAcknowledged;
        public bool IsAcknowledged
        {
            get { return _isAcknowledged; }
            set { _isAcknowledged = value; NotifyOfPropertyChange(() => IsAcknowledged); }
        }

    }
}
