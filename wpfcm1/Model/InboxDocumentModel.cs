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

        private bool? _isValid;
        public bool? IsValid
        {
            get { return _isValid; }
            set { _isValid = value; NotifyOfPropertyChange(() => IsValid); }
        }

        private bool _isAcknowledged;
        public bool IsAcknowledged
        {
            get { return _isAcknowledged; }
            set { _isAcknowledged = value; NotifyOfPropertyChange(() => IsAcknowledged); }
        }
    }
}
