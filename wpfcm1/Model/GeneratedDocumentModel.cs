using System.IO;
using System.Text.RegularExpressions;

namespace wpfcm1.Model
{
    public class GeneratedDocumentModel : DocumentModel
    {
        public GeneratedDocumentModel()
        {
        }

        public GeneratedDocumentModel(FileInfo fi) : base(fi)
        {
        }

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
            set {
                if (string.IsNullOrEmpty(value))
                {
                    _invoiceNo = value;
                } else
                {
                    Regex regexAllowedCharacters = new Regex(@"[^0-9a-zA-Z]");
                    _invoiceNo = regexAllowedCharacters.Replace(value, @"-");
                }
                //_invoiceNo = value;
                NotifyOfPropertyChange(() => InvoiceNo);
            }
        }

        private bool _multipleInvoiceNo;
        public bool multipleInvoiceNo
        {
            get { return _multipleInvoiceNo; }
            set { _multipleInvoiceNo = value; NotifyOfPropertyChange(() => multipleInvoiceNo); }
        }

        public override string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "Pib":
                        if (wpfcm1.FolderTypes.GeneratedFolderViewModel.IsPibOk(Pib))  // treba na lepše mesto staviti IsPibOk...
                        {
                            IsValid = true;
                        }
                        else 
                        {
                            IsValid = false;
                            return "Pib is not invalid PIB number";
                        }
                      
                        break;
                    case "InvoiceNo":
                        if (string.IsNullOrWhiteSpace(InvoiceNo))
                        {
                            IsValid = false;
                            return "InvoiceNo cannot be empty";
                        }
                        break;
                    default:
                        return base[columnName];
                }
                return null;
            }
        }
    }
}
