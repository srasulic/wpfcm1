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
            set { _invoiceNo = value; NotifyOfPropertyChange(() => InvoiceNo); }
        }

        private bool? _isValid;
        public bool? IsValid
        {
            get { return _isValid; }
            set { _isValid = value; NotifyOfPropertyChange(() => IsValid); }
        }

        public override string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "Pib":
                        if (string.IsNullOrWhiteSpace(Pib))
                            return "Pib must have 9 numbers";
                        var regexPib = new Regex(@"\b\d{9}\b");
                        if (!regexPib.IsMatch(Pib))
                            return "Pib must have 9 numbers";
                        break;
                    case "InvoiceNo":
                        if (string.IsNullOrWhiteSpace(InvoiceNo))
                            return "InvoiceNo cannot be empty";
                        break;
                    default:
                        return base[columnName];
                }
                return null;
            }
        }
    }
}
