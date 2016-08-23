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
                .Where(f => Extensions.Contains(Path.GetExtension(f)))
                .Select(f => new GeneratedDocumentModel(new FileInfo(f))));

            InitWatcher(FolderPath);

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
        }

        protected override void AddFile(string filePath)
        {
            Documents.Add(new GeneratedDocumentModel(new FileInfo(filePath)));
        }

        protected override void OnActivate()
        {
            _events.PublishOnUIThread(new MessageViewModelActivated(GetType().Name));
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            //TODO: hack: checkbox checkmark moze da se izgubi prilikom promene taba, ako promena nije komitovana
            var v = GetView() as UserControl;
            var dg = v.FindName("Documents") as DataGrid;
            dg.CommitEdit(DataGridEditingUnit.Row, true);
        }

        public void Handle(CertificateModel message)
        {
            _certificate = message;
        }

        public void Handle(MessageXls message)
        {
            if (!IsActive) return;
            try
            {
                var documents = Documents.Cast<GeneratedDocumentModel>();
                _expList = "\"Mark\",\"Pib primalac\",\"Fajl\",\"KB\",\"Br Dok\"\r\n";
                foreach (var document in documents)
                {
                    string[] fileNameParts = document.DocumentPath.Split('\\');
                    _expList = string.Concat(_expList, "\"", document.IsChecked.ToString(), "\",\"", document.Pib, "\",\"", fileNameParts.Last() , "\",\"", document.LengthKB, "\",\"", document.InvoiceNo, "\"\r\n");
                }

                // proveriti sadrzaj clipboarda pre ovoga
//                string clip = Clipboard.GetText();
//                clip = clip.Replace("\t", @""",""");
//                clip = clip.Replace("\r\n", "\"\r\n\"");
//                clip = string.Concat("\"", clip);
//                clip = clip.Remove(clip.Length - 1);
                string filename = string.Concat(Guid.NewGuid().ToString(), @".csv");
                filename = string.Concat(Path.GetTempPath(), filename);
                try
                {
                    System.Text.Encoding utf16 = System.Text.Encoding.GetEncoding(1254);
 //                   byte[] output = utf16.GetBytes(clip);
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

        public void Handle(MessageSign message)
        {
            if (IsActive)
            {
                var certificateOk = _certificate != null && _certificate.IsQualified;
                if (!certificateOk) return;
                var validDocuments = GetDocumentsForSigning();
                if (!validDocuments.Any()) return;

                //TODO: ovo mora drugacije
                _events.PublishOnUIThread(new MessageShowPdf(PreviewViewModel.Empty));
                var result = _windowManager.ShowDialog(new DialogSignViewModel(_certificate, this));
            }
        }

        public async void Handle(MessageExtractData message)
        {
            var documents = Documents.Where(d => !d.Processed).Cast<GeneratedDocumentModel>();
            var pib = User.Default.PIB;
            if (string.IsNullOrEmpty(pib))
                throw new ApplicationException("PIB korisnika nije unet!");

            foreach (var document in documents)
            {
                var matchResults = await PdfHelpers.ExtractTextAsync(document.DocumentPath);
                document.Pib = Regex.Match(matchResults.Item1, @"[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]").Value;

                document.InvoiceNo = Regex.Match(matchResults.Item2, @"F[0-9][0-9][0-9][0-9][0-9][0-9][0-9]").Value;

//                if (string.IsNullOrEmpty(document.InvoiceNo))
//                    document.InvoiceNo = Regex.Match(matchResults.Item2, @"[0-9][0-9]-[0-9][0-9][0-9][0-9][0-9][0-9][0-9]").Value;
                if (string.IsNullOrEmpty(document.InvoiceNo))
                    document.InvoiceNo = Regex.Match(matchResults.Item2, @"[0-9]{1,3}-[0-9]+").Value;
                if (string.IsNullOrEmpty(document.InvoiceNo))
                    document.InvoiceNo = Regex.Match(matchResults.Item2, @"DPTR[0-9]{1,3}-[0-9]+").Value;
                if (string.IsNullOrEmpty(document.InvoiceNo))
                    document.InvoiceNo = Regex.Match(matchResults.Item2, @"KO-[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]").Value;
                if (string.IsNullOrEmpty(document.InvoiceNo))
                    document.InvoiceNo = Regex.Match(matchResults.Item2, @"PPDV-[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]").Value;
 
               

                Regex regexAllowedCharacters = new Regex(@"[^0-9a-zA-Z]");
                document.InvoiceNo = regexAllowedCharacters.Replace(document.InvoiceNo, @"-");

                document.Processed = true;
            }
        }

        public void Handle(MessageReject message)
        {
            var checkedDocuments = Documents.Where(d => d.IsChecked);
            var destinationDir = SigningTransferRules.ProcessedMap[FolderPath];
            foreach (var document in checkedDocuments)
            {
                var sourceFilePath = document.DocumentPath;
                var fileName = Path.GetFileName(sourceFilePath);
                var destinationFileName = string.Format("X_{0}_{1}", DateTime.UtcNow.ToString("yyyyMMddHHmmssfff"), fileName);
                var destinationFilePath = Path.Combine(destinationDir, destinationFileName);
                File.Move(sourceFilePath, destinationFilePath);
            }
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
            var dg = view.Documents;
            var items = dg.SelectedItems;
            foreach (var item in items)
            {
                var doc = item as DocumentModel;
                doc.IsChecked = cb.IsChecked.GetValueOrDefault();
            }
        }
    }
}
