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
                        IsValid = true;
                        if (string.IsNullOrWhiteSpace(Pib))
                        {
                            IsValid = false;
                            return "Pib must have 9 numbers";
                        }
                        var regexPib = new Regex(@"\b\d{9}\b");
                        if (!regexPib.IsMatch(Pib))
                        {
                            IsValid = false;
                            return "Pib must have 9 numbers";
                        }

                        if (Pib != "111111111" && Pib != "222222222" && Pib != "333333333")
                        {
                            // begin kontrola PIBa:
                            int ost_pret = 10;
                            string cifra;
                            int i_cifra, suma, ostatak, umnozak, kontCifraIzracunata, kontCifra;

                            for (int i = 0; i < 8; i++)
                            {
                                cifra = Pib.Substring(i, 1);
                                int.TryParse(cifra, out i_cifra);
                                suma = ost_pret + i_cifra;
                                ostatak = suma % 10;
                                if (ostatak == 0) { ostatak = 10; }
                                umnozak = ostatak * 2;
                                ost_pret = umnozak % 11;
                            }

                            int.TryParse(Pib.Substring(Pib.Length - 1, 1), out kontCifra);
                            kontCifraIzracunata = (11 - ost_pret) % 10;


                            if (kontCifraIzracunata != kontCifra)
                            {
                                IsValid = false;
                                return "Pib - invalid control digit";
                            }
                            // end kontrola PIBa:
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
