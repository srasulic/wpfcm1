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

/*
        private string _pib;
        public string Pib
        {
            get { return _pib; }
            set { _pib = value; NotifyOfPropertyChange(() => Pib); }
        }

        private string _invoiceNo;
        public string InvoiceNo
        {
            get { return _invoiceNo; }
            set { _invoiceNo = value; NotifyOfPropertyChange(() => InvoiceNo); }
        }
*/
 
    }
}
