using System.IO;

namespace wpfcm1.Model
{
    public class ConfirmedToDoDocumentModel : DocumentModel
    {
        public ConfirmedToDoDocumentModel()
        {
        }

        public ConfirmedToDoDocumentModel(FileInfo fi)
            : base(fi)
        {
        }

        private bool _isRejected;
        public bool IsRejected
        {
            get { return _isRejected; }
            set { _isRejected = value;  NotifyOfPropertyChange(() => IsRejected); }
        }

    }
}
