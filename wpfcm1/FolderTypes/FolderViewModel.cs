using Caliburn.Micro;
using iTextSharp.text.pdf.security;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Xml.Serialization;
using wpfcm1.Events;
using wpfcm1.Model;
using wpfcm1.PDF;
using wpfcm1.Preview;

namespace wpfcm1.FolderTypes
{
    public class FolderViewModel : Screen, IDisposable
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // protected string[] Extensions = { ".pdf", ".ack" };
        protected string[] Extensions = { ".pdf" };
        protected FileSystemWatcher _watcher;
        private readonly Dispatcher _dispatcher;
        protected readonly IEventAggregator _events;
        private string _expList;

        public FolderViewModel(string path, string name, IEventAggregator events) 
        {
            FolderPath = path;
            DisplayName = name;
            _dispatcher = Dispatcher.CurrentDispatcher;
            _events = events;
            _events.Subscribe(this);

            InitDocuments();
        }

        public string FolderPath { get; private set; }
        public int Count { get { return Documents.Count; } }
        public virtual BindableCollection<DocumentModel> Documents { get; set; }
        private bool _isChanged;
        public bool IsChanged { get { return _isChanged; } set { _isChanged = value; NotifyOfPropertyChange(() => IsChanged); } }

        protected virtual void InitDocuments()
        {
            Documents = new BindableCollection<DocumentModel>(
                 Directory.EnumerateFiles(FolderPath)
                 .Where(f => Extensions.Contains(Path.GetExtension(f)))
                 .Select(f => new DocumentModel(new FileInfo(f))));
            InitWatcher(FolderPath);
        }

        public static void PsKillPdfHandlers()
        {   // Srdjan - da pustimo u pozadini neke alatke da pobiju eventualne procese koji drze sapu na PDF fajlovima
            //          Zbog brzine aplikaciji hadle.exe prosledjujemo deo naziva file handlera (bez ovoga traje 10-ak sekundi)
            //          Podrzavamo (ocekujemo) Foxit Reader ili Adobe Acrobat Reader
            try
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";

                startInfo.Arguments = @" /C for /f ""tokens=3"" %G IN ('c:\edokument\bin\handle.exe /accepteula eDokument\ -p Fox ^| findstr /i /r /c:"".*pid:.*pdf$""') DO c:\edokument\bin\pskill.exe /accepteula %G";
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();

                startInfo.Arguments = @" /C for /f ""tokens=3"" %G IN ('c:\edokument\bin\handle.exe /accepteula eDokument\ -p Acro ^| findstr /i /r /c:"".*pid:.*pdf$""') DO c:\edokument\bin\pskill.exe /accepteula %G";
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();
                
            }
            catch (Exception ex)
            {
                Log.Error("Error while running PSTOOLS...", ex);
            }
        }
        
        protected void RejectDocument ()
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

        protected void XlsExport()
        {
            try
            {
                var documents = Documents.Cast<DocumentModel>();
                _expList = "\"Mark\",\"Pib izdavalac\",\"Pib primalac\",\"Fajl\",\"KB\",\"Br Dok\"\r\n";
                foreach (var document in documents)
                {
                    string[] fileNameParts = document.DocumentPath.Split('\\');
                    string[] parts = fileNameParts.Last().Split('_');
                    _expList = string.Concat(_expList, "\"", document.IsChecked.ToString(), "\",\"", parts[0], "\",\"", parts[1], "\",\"", fileNameParts.Last(), "\",\"", document.LengthKB, "\",\"", parts[2], "\"\r\n");
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
                catch (Exception e)
                {
                    Log.Error("Error while exporting to XLS", e);
                }

                System.Diagnostics.Process.Start(filename);
            }
            catch
            {

            }
        }

        protected async Task ValidateDocSignaturesAsync()
        {
//            var documents = Documents.Where(d => !d.Processed).Cast<InboxDocumentModel>();
            var documents = Documents.Where(d => !d.isValidated || d.IsChecked).Cast<DocumentModel>();
            foreach (var document in documents)
            {
                try
                {
                    var isValid = await PdfHelpers.ValidatePdfCertificatesAsync(document.DocumentPath);
                    document.IsValid = isValid;
                    document.sigValidationInfo = @"O.K.";
                } catch (Exception e)
                {
                    document.IsValid = false;
                    document.sigValidationInfo = e.Message;
                }
                //document.isValidated = true;
                //document.Processed = true;
                // PROTOTIP - za sada obradjujemo prvi i drugi potpis na koji naidjemo. Doraditi za ostale!
                // obradjujemo prvo drugi, koji mozda ne postoji, kako bi promena properti-ja na prvom okinula osvezavanje statusa u prikazima 
                PdfPKCS7 pkcs7 = await PdfHelpers.GetPcks7Async(document.DocumentPath, 2);
                if (!(pkcs7 == null))
                {
                    document.isValidated2 = true;
                    document.sigReason2 = pkcs7.Reason;
                    document.sigTS2 = pkcs7.TimeStampDate;
                    document.sigDateSigned2 = pkcs7.SignDate;
                    document.sigSignerName2 = System.Text.RegularExpressions.Regex.Replace(CertificateInfo.GetSubjectFields(pkcs7.SigningCertificate).GetField(@"CN"), @"[0-9]", "");
                  //  document.sigOrg2 = String.Format("{0} - {1}", CertificateInfo.GetSubjectFields(pkcs7.SigningCertificate).GetField(@"O"), CertificateInfo.GetSubjectFields(pkcs7.SigningCertificate).GetField(@"OU"));
                    var docFields = CertificateInfo.GetSubjectFields(pkcs7.SigningCertificate).GetFields();
                    String organization = "";
                    foreach (var ouField in docFields.Where(f => f.Key == @"OU"))
                    {
                        foreach (var ou in ouField.Value)
                            organization = String.Format("{0}, {1}", organization, ou);
                    }
                    organization = String.Format("{0}, {1}", organization, CertificateInfo.GetSubjectFields(pkcs7.SigningCertificate).GetField(@"O"));
                    document.sigOrg2 = organization;
                }
                pkcs7 = await PdfHelpers.GetPcks7Async(document.DocumentPath, 1);
                if (!(pkcs7 == null))
                {
                    document.isValidated = true;
                    document.sigReason = pkcs7.Reason;
                    document.sigTS = pkcs7.TimeStampDate;
                    document.sigDateSigned = pkcs7.SignDate;
                    document.sigSignerName = System.Text.RegularExpressions.Regex.Replace(CertificateInfo.GetSubjectFields(pkcs7.SigningCertificate).GetField(@"CN"), @"[0-9]", "");
                    var docFields = CertificateInfo.GetSubjectFields(pkcs7.SigningCertificate).GetFields();
                    String organization = "";
                    foreach (var ouField in docFields.Where(f => f.Key == @"OU"))
                    {
                        foreach (var ou in ouField.Value)
                            organization = String.Format("{0}, {1}", organization, ou);
                    }
                    organization = String.Format("{0}, {1}", organization, CertificateInfo.GetSubjectFields(pkcs7.SigningCertificate).GetField(@"O"));
                    document.sigOrg = organization;
                }
                SetSigAdditionalInfo(document);
            }
        }

        protected void SetSigAdditionalInfo(DocumentModel document)
        {
            string strdateTS;
            string strdateDate;
            string strdateTS2;
            string strdateDate2;
            if (document.sigTS == DateTime.MinValue) strdateTS = ""; else strdateTS = document.sigTS.ToShortDateString();
            if (document.sigDateSigned == DateTime.MinValue) strdateDate = ""; else strdateDate = document.sigDateSigned.ToShortDateString();
            if (document.sigTS2 == DateTime.MinValue) strdateTS2 = ""; else strdateTS2 = document.sigTS2.ToShortDateString();
            if (document.sigDateSigned2 == DateTime.MinValue) strdateDate2 = ""; else strdateDate2 = document.sigDateSigned2.ToShortDateString();
            document.sigAdditionalInfo = String.Format(@"{11}{0}* Potpis 1:{0}Reason: {1}{0}Potpisao: {2}, {3} {0}TimeStamp datum: {4} {0}Datum potpisa: {5} {0}{0}** Potpis 2:{0}Reason: {6}{0}Potpisao: {7}, {8}{0}TimeStamp datum: {9}{0}Datum potpisa: {10} ",
                            Environment.NewLine,
                            document.sigReason, document.sigSignerName, document.sigOrg, strdateTS, strdateDate,
                            document.sigReason2, document.sigSignerName2, document.sigOrg2, strdateTS2, strdateDate2,
                            document.sigValidationInfo
                            );

        }


        public void InternalMessengerGetStates()
        {
            foreach (var document in Documents.Where(d => d.hasExternalMessage)) {
                InternalMessengerGetStates (document);
                document.hasExternalMessage = false;
            }
        }
        
        public void InternalMessengerGetStates(DocumentModel document)
        {
            var extDocState = new DocumentModel();
            var xs = new XmlSerializer(typeof(DocumentModel));
            // proverimo poruku koja je u samom folderu:
            var fileName = Path.GetFileName(document.DocumentPath);
            var file = Path.Combine(FolderPath, fileName + ".xml");
            if (File.Exists(file))
            {
                try
                {
                    using (Stream s = File.OpenRead(file))
                        extDocState = (DocumentModel)xs.Deserialize(s);

                    document.isApprovedForProcessing = extDocState.isApprovedForProcessing;
                    document.isRejected = extDocState.isRejected;
                    if (document.Processed == false) document.Processed = extDocState.Processed;

                    //document.IsAcknowledged = true; // zloupotrebili smo ga dok još nismo znali da bool može da bude bez vrednosti :)  

                }
                catch (Exception e)
                {
                    Log.Error("Error: InternalMessengerGetStates ", e);
                }
            }
            // ukoliko postoji i neka poruka koja još nije sinhronizovan asa serverom (interna obrada u aplikaciji)
            // obradicemo i nju kao najsveziju informaciju
            var destinationDir = SigningTransferRules.LocalMap[FolderPath];
            file = Path.Combine(destinationDir, fileName + ".xml");
            if (File.Exists(file))
            {
                try
                {
                    using (Stream s = File.OpenRead(file))
                        extDocState = (DocumentModel)xs.Deserialize(s);

                    document.isApprovedForProcessing = extDocState.isApprovedForProcessing;
                    document.isRejected = extDocState.isRejected;
                    if (document.Processed == false) document.Processed = extDocState.Processed;

                   // document.IsAcknowledged = true; // zloupotrebili smo ga dok još nismo znali da bool može da bude bez vrednosti :)  

                }
                catch (Exception e)
                {
                    Log.Error("Error: InternalMessengerGetStates ", e);
                }
            }
        }


        //protected void RejectToDoDocument(Type type)
        //{
        //    var checkedDocuments = Documents.Where(d => d.IsChecked).Where(d => !d.Processed);
        //    if (!checkedDocuments.Any()) return;
        //    var destinationDir = SigningTransferRules.LocalMap[FolderPath];
        //    // TODO: dodati dijalog, sada radimo bez upozorenja
        //    foreach (var document in checkedDocuments)
        //    {
        //        document.Processed = true;
        //        document.isRejected = true;
        //        var fileName = Path.GetFileName(document.DocumentPath);
        //        var destinationFilePath = Path.Combine(destinationDir, fileName + ".xml");
        //        var file = File.Create(destinationFilePath);

        //        var xs = new XmlSerializer(type);
        //        using (Stream s = file)
        //            xs.Serialize(s, document);
        //    }
        //}

        protected void SetRejected()
        {
            if (!IsActive) return;
            var checkedDocuments = Documents.Where(d => d.IsChecked).Where(d => !d.Processed);
            if (!checkedDocuments.Any()) return;
            // TODO: možda dodati dijalog, sada radimo bez upozorenja
            foreach (var document in checkedDocuments)
            {
                document.Processed = true;
                document.isRejected = true;
                SerializeMessage(document);
            }
        }


        protected void SetProcessed()
        {
            if (!IsActive) return;
            var checkedDocuments = Documents.Where(d => d.IsChecked).Where(d => !d.Processed);
            if (!checkedDocuments.Any()) return;
            // TODO: dodati dijalog, sada radimo bez upozorenja
            foreach (var document in checkedDocuments)
            {
                document.Processed = true;
                document.isApprovedForProcessing = true;
                SerializeMessage(document);
            }
        }

        protected void SerializeMessage(DocumentModel document)
        {
            try
            {
                // Nisam mogao da kastujem prosleđene klase u DocumentModel, pa pravimo ciljano poruku za razmenu:
                DocumentModel message = new DocumentModel();
                message.Processed = document.Processed;
                message.isRejected = document.isRejected;
                message.isApprovedForProcessing = document.isApprovedForProcessing;
                message.HasSecondSignature = document.HasSecondSignature;
                message.IsValid = document.IsValid;
                message.isValidated = document.isValidated;
                message.isValidated2 = document.isValidated2;

                var destinationDir = SigningTransferRules.LocalMap[FolderPath];
                var fileName = Path.GetFileName(document.DocumentPath);
                var destinationFilePath = Path.Combine(destinationDir, fileName + ".xml");
                var file = File.Create(destinationFilePath);

                var xs = new XmlSerializer(typeof(DocumentModel));
                using (Stream s = file)
                    xs.Serialize(s, message);
            }
            catch (Exception e)
            {
                Log.Error("Error: SerializeMessage ", e);
            }

        }

        //protected void SetProcessed(Type type)
        //{
        //    var checkedDocuments = Documents.Where(d => d.IsChecked).Where(d => !d.Processed);
        //    if (!checkedDocuments.Any()) return;
        //    var destinationDir = SigningTransferRules.LocalMap[FolderPath];
        //    // TODO: dodati dijalog, sada radimo bez upozorenja
        //    foreach (var document in checkedDocuments)
        //    {
        //        document.Processed = true;
        //        document.isApprovedForProcessing = true;

        //        var fileName = Path.GetFileName(document.DocumentPath);
        //        var destinationFilePath = Path.Combine(destinationDir, fileName + ".xml");
        //        var file = File.Create(destinationFilePath);

        //        var xs = new XmlSerializer(type);
        //        using (Stream s = file)
        //            xs.Serialize(s, document);
        //    }
        //}

        protected override void OnActivate()
        {
            _events.PublishOnUIThread(new MessageViewModelActivated(GetType().Name));
        }

        protected override void OnDeactivate(bool close)
        {
            _events.PublishOnUIThread(new MessageShowPdf(PreviewViewModel.Empty));
        }

        protected override void OnViewAttached(object view, object context)
        {
            Documents.Refresh(); //TODO: hack: nestajao error notification template kada se promeni tab
        }

        protected virtual void AddFile(string filePath)
        {
            if (Regex.IsMatch(filePath, @".+syncstamp$", RegexOptions.IgnoreCase))
            {
                InternalMessengerGetStates();
            }
            else
            {
                Documents.Add(new DocumentModel(new FileInfo(filePath)));
            }
        }

        public virtual void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            var message = PreviewViewModel.Empty;
            if (e.AddedItems.Count == 1)
            {
                var document = e.AddedItems[0] as DocumentModel;
                var path = document.DocumentPath;
                if (path.EndsWith(".pdf"))
                    message = path;
            }
            _events.PublishOnUIThread(new MessageShowPdf(message)); 
        }

        #region FileSystemwatcher
        protected void InitWatcher(string path)
        {
            _watcher = new FileSystemWatcher(path) { NotifyFilter = NotifyFilters.Size | NotifyFilters.FileName };
            _watcher.Changed += Watcher_Changed; 
            _watcher.Deleted += Watcher_Deleted;
            _watcher.Created += Watcher_Created;
            _watcher.Renamed += Watcher_Renamed;
            _watcher.EnableRaisingEvents = true;
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {

        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            IsChanged = true;
            var action = new System.Action(() => AddFile(e.FullPath));
            if (_dispatcher.CheckAccess())
                action();
            else
                _dispatcher.BeginInvoke(DispatcherPriority.DataBind, action);
        }

        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            IsChanged = true;
            var action = new System.Action(() =>
            {
                var docToRemove = Documents.FirstOrDefault(f => f.DocumentPath == e.FullPath);
                if (docToRemove != null) Documents.Remove(docToRemove);
            });
            if (_dispatcher.CheckAccess())
                action();
            else
                _dispatcher.BeginInvoke(DispatcherPriority.DataBind, action);
        }

        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {

        }
        #endregion

        public virtual void Dispose()
        {

        }

        public virtual void OnCheck(object e)
        {

        }
    }
}
