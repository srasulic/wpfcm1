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
        public string PibReciever
        {
            get { return _pib; }
            set { _pib = value; NotifyOfPropertyChange(() => PibReciever); }
        }

        private string _pibIssuer;
        public string PibIssuer
        {
            get { return _pibIssuer; }
            set { _pibIssuer = value; NotifyOfPropertyChange(() => PibIssuer); }
        }

        private string _invoiceNo;
        public string InvoiceNo
        {
            get { return _invoiceNo; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _invoiceNo = value;
                }
                else
                {
                    Regex regexAllowedCharacters = new Regex(@"[^0-9a-zA-Z]");
                    _invoiceNo = regexAllowedCharacters.Replace(value, @"-");
                }
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
                    case "PibReciever":
                        if (wpfcm1.FolderTypes.GeneratedFolderViewModel.IsPibOk(PibReciever))  // treba na lepše mesto staviti IsPibOk...
                        {
                            IsValid = true;
                        }
                        else
                        {
                            IsValid = false;
                            return "Pib is not invalid PIB number";
                        }
                        break;
                    case "PibIssuer":
                        if (wpfcm1.FolderTypes.GeneratedFolderViewModel.IsPibOk(PibIssuer, false))  // treba na lepše mesto staviti IsPibOk...
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
