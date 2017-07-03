using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Xml.Serialization;
using wpfcm1.Dialogs;
using wpfcm1.Events;
using wpfcm1.Model;
using wpfcm1.PDF;
using wpfcm1.Preview;
using wpfcm1.Settings;
using System.Text.RegularExpressions;
using System.Windows;
using System.Net;
using System.Windows.Data;

namespace wpfcm1.FolderTypes
{
    public class GeneratedFolderViewModel : FolderViewModel, IHandle<CertificateModel>, IHandle<MessageSign>, IHandle<MessageExtractData>, IHandle<MessageReject>, IHandle<MessageXls>
    {
        private readonly IWindowManager _windowManager;
        private CertificateModel _certificate;
        private string _expList;

        public GeneratedFolderViewModel(string path, string name, IEventAggregator events, IWindowManager winMgr) : base(path, name, events)
        {
            _windowManager = winMgr;
        }

        protected override void InitDocuments()
        {
            Documents = new BindableCollection<DocumentModel>(
                Directory.EnumerateFiles(FolderPath)
                .Where(f => Extensions.Contains(Path.GetExtension(f).ToLower()))
                .Select(f => new GeneratedDocumentModel(new FileInfo(f))));

            InitWatcher(FolderPath);

            DocumentsCV = CollectionViewSource.GetDefaultView(Documents) as ListCollectionView;
            DocumentsCV.Filter = new Predicate<object>(FilterDocument);

            if (Documents.Count == 0) return;
            var states = Deserialize();
            foreach (var state in states)
            {
                var found = Documents.FirstOrDefault(d => d.DocumentPath == state.DocumentPath);
                if (found == null) continue;
                var old = found as GeneratedDocumentModel;
                old.IsChecked = state.IsChecked;
                old.IsValid = state.IsValid;
                old.InvoiceNo = state.InvoiceNo;
                old.Pib = state.Pib;
                old.Processed = state.Processed;
            }
            CheckForDuplicateInvNo();
        }

        protected override void AddFile(string filePath)
        {
            if (Regex.IsMatch(filePath, @".+syncstamp$", RegexOptions.IgnoreCase))
            {
                InternalMessengerGetStates();
            }
            else
            {
                var newDoc = new GeneratedDocumentModel(new FileInfo(filePath));
                Documents.Add(newDoc);
                //CheckForDuplicateInvNo(newDoc);
            }
        }

        protected override void OnActivate()
        {
            _events.PublishOnUIThread(new MessageViewModelActivated(GetType().Name));
        }

        protected void CheckForDuplicateInvNo()
        {
            //if (!IsActive) return;
            var documents = Documents.Cast<GeneratedDocumentModel>();
            foreach (GeneratedDocumentModel document in Documents)
            {
                int found = documents.Where(d => d.InvoiceNo == document.InvoiceNo).Count();
                if (found > 1) document.multipleInvoiceNo = true;
                if (found == 1 && document.multipleInvoiceNo) document.multipleInvoiceNo = false;
            }
        }

        protected void CheckForDuplicateInvNo(GeneratedDocumentModel document)
        {
            if (!IsActive) return;
            var documents = Documents.Cast<GeneratedDocumentModel>();
            int found = documents.Where(d => d.InvoiceNo == document.InvoiceNo).Count();
            if (found > 1)
            {
                //document.multipleInvoiceNo = true;
                foreach (GeneratedDocumentModel docForUpdate in documents.Where(d => d.InvoiceNo == document.InvoiceNo))
                {
                    docForUpdate.multipleInvoiceNo = true;
                }
            }
            if (found == 1 && document.multipleInvoiceNo) document.multipleInvoiceNo = false;
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            //TODO: hack: checkbox checkmark moze da se izgubi prilikom promene taba, ako promena nije komitovana
            var v = GetView() as UserControl;
            var dg = v.FindName("DocumentsCV") as DataGrid;
            dg.CommitEdit(DataGridEditingUnit.Row, true);
        }

        private new void XlsExport()
        { 
            try
            {
                var documents = Documents.Cast<GeneratedDocumentModel>();
                _expList = "\"Mark\",\"Pib primalac\",\"Fajl\",\"KB\",\"Br Dok\"\r\n";
                foreach (var document in documents)
                {
                    string[] fileNameParts = document.DocumentPath.Split('\\');
                    _expList = string.Concat(_expList, "\"", document.IsChecked.ToString(), "\",\"", document.Pib, "\",\"", fileNameParts.Last() , "\",\"", document.LengthKB, "\",\"", document.InvoiceNo, "\"\r\n");
                }

                string filename = string.Concat(Guid.NewGuid().ToString(), @".csv");
                filename = string.Concat(Path.GetTempPath(), filename);
                try
                {
                    System.Text.Encoding utf16 = System.Text.Encoding.GetEncoding(1254);
                    byte[] output = utf16.GetBytes(_expList);
                    FileStream fs = new FileStream(filename, FileMode.Create);
                    BinaryWriter bw = new BinaryWriter(fs);
                    bw.Write(output, 0, output.Length); //write the encoded file
                    bw.Flush();
                    bw.Close();
                    fs.Close();
                }
                catch
                {

                }
                                
                System.Diagnostics.Process.Start(filename);
            }
            catch
            {

            }
        }

        public void Handle(CertificateModel message)
        {
            _certificate = message;
        }

        public void Handle(MessageXls message)
        {
            if (!IsActive) return;
            XlsExport();
            
        }

        public void Handle(MessageSign message)
        {
            if (IsActive)
            {
                //PsKillPdfHandlers(); // workaround - pskill ubija sve procese koji rade nad PDF-ovima u eDokument
                var certificateOk = _certificate != null && _certificate.IsQualified;
                if (!certificateOk) return;
                var validDocuments = GetDocumentsForSigning();
                if (!validDocuments.Any()) return;

                //TODO: ovo mora drugacije
                // _events.PublishOnUIThread(new MessageShowPdf(PreviewViewModel.Empty));
                var result = _windowManager.ShowDialog(new DialogSignViewModel(_certificate, this));
            }
        }

        public async void Handle(MessageExtractData message)
        {
            if (!IsActive) return;
            // iz putanje koja je u obliku "c:\\eDokument\\Faktura\\ERP_outbound_interface" uzimamo tip dokumenta
//            var tip_dok = Regex.Match(FolderPath, @"edokument\\(.*)\\", RegexOptions.IgnoreCase).Groups[1].ToString();
//            if (tip_dok == "Faktura") return;
            // sada imamo PIB i tip dokumenta - možemo da uputimo web request upit za mapiranje i za regex

            var documents = Documents.Where(d => !d.Processed).Cast<GeneratedDocumentModel>();
            var pib = User.Default.PIB;
            if (string.IsNullOrEmpty(pib))
                throw new ApplicationException("PIB korisnika nije unet!");

/*

            var uri = "https://edokument.aserta.rs/index/api";
            uri = @"http://edokument_hu.aserta.dev/index/api?test_do_login=1&user=111111111&pass=test01";
            var request = WebRequest.Create(uri);
            request.Proxy = null;
            request.Method = "GET";

            using (WebResponse response = request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    //                    XmlTextReader reader = new XmlTextReader(stream);
                    StreamReader reader = new StreamReader(stream);
                    string responseFromServer = reader.ReadToEnd();
                    Console.WriteLine(responseFromServer);
                    reader.Close();

                }
            }

            uri = @"http://edokument_hu.aserta.dev/index/api?test_is_login=1";
            request = WebRequest.Create(uri);
            using (WebResponse response = request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    //                    XmlTextReader reader = new XmlTextReader(stream);
                    StreamReader reader = new StreamReader(stream);
                    string responseFromServer = reader.ReadToEnd();
                    Console.WriteLine(responseFromServer);
                    reader.Close();

                }
            }


*/
            foreach (var document in documents)
            {
                var matchResults = await PdfHelpers.ExtractTextAsync(document.DocumentPath);
                document.Pib = Regex.Match(matchResults.Item1, @"[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]").Value;

                document.InvoiceNo = Regex.Match(matchResults.Item2, @"F[0-9][0-9][0-9][0-9][0-9][0-9][0-9]").Value;

                // perihard:
                if (string.IsNullOrEmpty(document.InvoiceNo))
                    document.InvoiceNo = Regex.Match(matchResults.Item2, @"[0-9]{2}-[0-9]{3}-[0-9]+").Value;
                if (string.IsNullOrEmpty(document.InvoiceNo))
                    document.InvoiceNo = Regex.Match(matchResults.Item2, @"[0-9]{1,3}-[0-9]+").Value;
                if (string.IsNullOrEmpty(document.InvoiceNo))
                    document.InvoiceNo = Regex.Match(matchResults.Item2, @"DPTR[0-9]{1,3}-[0-9]+").Value;
                // farmalogist KP
                if (string.IsNullOrEmpty(document.InvoiceNo))
                    document.InvoiceNo = Regex.Match(matchResults.Item2, @"[A-Z]{2,4}-[0-9]{8,9}").Value;
               
                 //budzevina za perihard Zapisnike
                // TODO: ovo napraviti kao univerzalno rešenje - trebaće i drugima
                if ( string.IsNullOrEmpty(document.Pib) ) {
                    var matchResults_ZAP = await PdfHelpers.ExtractTextAsync_ZAP(document.DocumentPath);
                    document.Pib = Regex.Match(matchResults_ZAP.Item1, @"[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]").Value;
                    document.InvoiceNo = "Zapisnik";

                }

                Regex regexAllowedCharacters = new Regex(@"[^0-9a-zA-Z]");
                document.InvoiceNo = regexAllowedCharacters.Replace(document.InvoiceNo, @"-");

                document.Processed = true;
            }
            CheckForDuplicateInvNo();
        }

        public void Handle(MessageReject message)
        {
            if (!IsActive) return;
            PsKillPdfHandlers(); // workaround - pskill ubija sve procese koji rade nad PDF-ovima u eDokument
            RejectDocument();
        }

        public IList<DocumentModel> GetDocumentsForSigning()
        {
            var checkedDocuments = Documents.Where(d => d.IsChecked).Cast<GeneratedDocumentModel>();
            var validDocuments = checkedDocuments.Where(d => d.IsValid.GetValueOrDefault()).Cast<DocumentModel>().ToList();
            return validDocuments;
        }

        public override void Dispose()
        {
            Serialize();
        }

        private void Serialize()
        {
            var filePath = Path.Combine(FolderPath, "state.xml");
            var file = File.Create(filePath);
            List<GeneratedDocumentModel> items = Documents.Cast<GeneratedDocumentModel>().ToList();
            var xs = new XmlSerializer(typeof(List<GeneratedDocumentModel>));
            using (Stream s = file)
                xs.Serialize(s, items);
        }

        private List<GeneratedDocumentModel> Deserialize()
        {
            var oldList = new List<GeneratedDocumentModel>();
            var xs = new XmlSerializer(typeof(List<GeneratedDocumentModel>));
            var file = Path.Combine(FolderPath, "state.xml");
            if (!File.Exists(file)) return oldList;
            try
            {
                using (Stream s = File.OpenRead(file))
                    oldList = (List<GeneratedDocumentModel>) xs.Deserialize(s);
            }
            catch
            {
                
            }
            return oldList;
        }

        public override void OnCheck(object e)
        {
            var ec = e as ActionExecutionContext;
            var cb = ec.Source as CheckBox;
            
            var view = ec.View as GeneratedFolderView;
            var dg = view.DocumentsCV;
            var items = dg.SelectedItems;
            if (items.Count > 1)
            {
                foreach (var item in items)
                {
                    var doc = item as DocumentModel;
                    doc.IsChecked = cb.IsChecked.GetValueOrDefault();
                }
            }
            else
            {
                foreach (var item in DocumentsCV)
                {
                    var doc = item as DocumentModel;
                    doc.IsChecked = cb.IsChecked.GetValueOrDefault();
                }
            }
        } 
    }
}
