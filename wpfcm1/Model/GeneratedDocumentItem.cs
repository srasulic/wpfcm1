using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace wpfcm1.Model
{
    public class GeneratedDocumentItem : DocumentItem
    {
        public GeneratedDocumentItem()
        {
        }

        public GeneratedDocumentItem(string path) : base(path)
        {
        }

        public GeneratedDocumentItem(GeneratedDocumentItem that) : base(that)
        {
            Pib = that.Pib;
            InvoiceNo = that.InvoiceNo;
        }

        public GeneratedDocumentItem(DocumentItem that) : base(that)
        {
        }

        private bool _isChecked;
        [XmlIgnore] 
        public bool IsChecked
        {
            get { return _isChecked; }
            set { _isChecked = value; NotifyOfPropertyChange(() => IsChecked); }
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
