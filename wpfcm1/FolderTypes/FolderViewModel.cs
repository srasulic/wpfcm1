using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;
using Caliburn.Micro;
using iTextSharp.text.pdf.security;
using wpfcm1.DataAccess;
using wpfcm1.Events;
using wpfcm1.FolderGroups;
using wpfcm1.Model;
using wpfcm1.PDF;
using wpfcm1.Preview;

namespace wpfcm1.FolderTypes
{
    public class FolderViewModel : Screen, IDisposable, IHandle<MessageGetPibNames>
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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
            _events.SubscribeOnUIThread(this);

            InitDocuments();
        }

        public string FolderPath { get; private set; }
        public int Count { get { return Documents.Count; } }
        public virtual BindableCollection<DocumentModel> Documents { get; set; }
        private bool _isChanged;
        public bool IsChanged { get { return _isChanged; } set { _isChanged = value; NotifyOfPropertyChange(() => IsChanged); } }

        public ListCollectionView DocumentsCV { get; set; }

        protected virtual void InitDocuments()
        {
            Documents = new BindableCollection<DocumentModel>(
                 Directory.EnumerateFiles(FolderPath)
                 .Where(f => Extensions.Contains(Path.GetExtension(f).ToLower()))
                 .Select(f => new DocumentModel(new FileInfo(f))));
            InitWatcher(FolderPath);

            DocumentsCV = CollectionViewSource.GetDefaultView(Documents) as ListCollectionView;
            DocumentsCV.Filter = new Predicate<object>(FilterDocument);

            if (Documents.Count == 0) return;
            var states = Deserialize();
            foreach (var state in states)
            {
                var found = Documents.FirstOrDefault(d => d.DocumentPath == state.DocumentPath);
                if (found == null) continue;
                var old = found as DocumentModel;
                old.IsChecked = state.IsChecked;
                old.IsValid = state.IsValid;
                old.Processed = state.Processed;

                old.namePib1Name = state.namePib1Name;
                old.namePib2Name = state.namePib2Name;
            }
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

                // Legacy: Zbog starih instalacija ostavljamo mogućnost da su handle.exe i pskill.exe u c:\bin direktorijumu
                if (File.Exists(@"c:\edokument\bin\handle.exe"))
                {
                    startInfo.WorkingDirectory = @"c:\edokument\bin\";
                }
                else {
                    startInfo.WorkingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                }

                //                startInfo.Arguments = @" /C for /f ""tokens=3"" %G IN ('c:\edokument\bin\handle.exe /accepteula eDokument\ -p Fox ^| findstr /i /r /c:"".*pid:.*pdf$""') DO c:\edokument\bin\pskill.exe /accepteula %G";
                startInfo.Arguments = @" /C for /f ""tokens=3"" %G IN ('handle.exe /accepteula eDokument\ -p Fox ^| findstr /i /r /c:"".*pid:.*pdf$""') DO pskill.exe /accepteula %G";
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();

                //                startInfo.Arguments = @" /C for /f ""tokens=3"" %G IN ('c:\edokument\bin\handle.exe /accepteula eDokument\ -p Acro ^| findstr /i /r /c:"".*pid:.*pdf$""') DO c:\edokument\bin\pskill.exe /accepteula %G";
                startInfo.Arguments = @" /C for /f ""tokens=3"" %G IN ('handle.exe /accepteula eDokument\ -p Acro ^| findstr /i /r /c:"".*pid:.*pdf$""') DO pskill.exe /accepteula %G";
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
                if (Regex.IsMatch(fileName, @".*pdf.xml$", RegexOptions.IgnoreCase)) continue;
                if (Regex.IsMatch(fileName, @".*pdf.ack$", RegexOptions.IgnoreCase)) continue;
                var destinationFileName = string.Format("X_{0}_{1}", DateTime.UtcNow.ToString("yyyyMMddHHmmssfff"), fileName);
                var destinationFilePath = Path.Combine(destinationDir, destinationFileName);
                File.Move(sourceFilePath, destinationFilePath);
            }
        }

        public static string BoolToDaNeString(bool value)
        {
            return value ? "DA" : "ne";
        }

        protected void XlsExport()
        {
            try
            {
                var documents = Documents.Where(d => d.IsChecked).Cast<DocumentModel>();
                if (!documents.Any()) { documents = Documents.Cast<DocumentModel>(); }
                _expList = "sep=,\n\"Obelezen\",\"Fajl\",\"KB\",\"Pib1\",\"Izdavalac\",\"Pib2\",\"Primalac\",\"Br dok\",\"Datum\",\""
                        + "Reason (napomena prilikom potpisivanja)\",\"Ime potpisnika\",\"Organizacija\",\"Datum potpisivanja\",\"Vremenski žig\",\""
                        + "Reason 2 (napomena prilikom potpisivanja)\",\"Ime potpisnika 2\",\"Organizacija 2\",\"Datum potpisivanja 2\",\"Vremenski žig 2\",\""
                        + "Validacija - info\",\"Odobren za obradu\",\"Odbačen\",\"Odobren za arhiviranje\"\r\n";
                foreach (var document in documents)
                {
                    string[] fileNameParts = document.DocumentPath.Split('\\');
                    string[] parts = fileNameParts.Last().Split('_');
                    _expList = string.Concat(
                        _expList, "\"",
                        BoolToDaNeString(document.IsChecked), "\",\"",
                        fileNameParts.Last(), "\",\"",
                        document.LengthKB, "\",\"",

                        document.namePib1, "\",\"",
                        document.namePib1Name, "\",\"",
                        document.namePib2, "\",\"",
                        document.namePib2Name, "\",\"",
                        document.nameDocNum, "\",\"",
                        document.nameDate, "\",\"",

                        document.sigReason, "\",\"",
                        document.sigSignerName, "\",\"",
                        document.sigOrg, "\",\"",
                        document.sigDateSigned, "\",\"",
                        document.sigTS, "\",\"",
                        
                        document.sigReason2, "\",\"",
                        document.sigSignerName2, "\",\"",
                        document.sigOrg2, "\",\"",
                        document.sigDateSigned2, "\",\"",
                        document.sigTS2, "\",\"",
                        
                        document.sigValidationInfo, "\",\"",
                        BoolToDaNeString(document.isApprovedForProcessing), "\",\"",
                        BoolToDaNeString(document.isRejected), "\",\"",
                        BoolToDaNeString(document.archiveReady), "\"\r\n"
                        );
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
            //var documents = Documents.Where(d => !d.Processed).Cast<InboxDocumentModel>();
            var documents = Documents.Where(d => !d.isValidated || d.IsChecked).Cast<DocumentModel>();
            foreach (var document in documents)
            {
                try
                {
                    Tuple<bool, string> isValid = await PdfHelpers.ValidatePdfCertificatesWithInfoAsync(document.DocumentPath);
                    document.IsValid = isValid.Item1;
                    document.sigValidationInfo = isValid.Item2;
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
                    document.sigSignerName2 = Regex.Replace(CertificateInfo.GetSubjectFields(pkcs7.SigningCertificate).GetField(@"CN"), @"[0-9]", "");
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
                document.sigAdditionalInfo = "refresh";
            }
        }

        protected async Task GetPibNamesAsync(BindableCollection<DocumentModel> folderDocuments)
        {
            if (folderDocuments == null) return;

            //var documents = Documents.Where(d => !d.Processed).Cast<InboxDocumentModel>();
            //var documents = Documents.Where(d => String.IsNullOrEmpty(d.namePib1Name) || String.IsNullOrEmpty(d.namePib2Name)).Cast<DocumentModel>();
            var documents = folderDocuments.Where(d => String.IsNullOrEmpty(d.namePib1Name) || String.IsNullOrEmpty(d.namePib2Name)).Cast<DocumentModel>();
            foreach (var document in documents)
            {
                try
                {
                    if (String.IsNullOrEmpty(document.namePib1Name))
                        document.namePib1Name = await APIManager.GetCustomerNameByPIBAsync(document.namePib1);
                    if (String.IsNullOrEmpty(document.namePib2Name))
                        document.namePib2Name = await APIManager.GetCustomerNameByPIBAsync(document.namePib2);
                }
                catch (Exception e)
                {
                    Log.Error("Error GetPibNamesAsync", e);
                    throw e;

                }
            }
        }

        public void InternalMessengerGetStates()
        {
            foreach (var document in Documents.Where(d => d.hasExternalMessage))
            {
                InternalMessengerGetStates(document);
                document.hasExternalMessage = false;
            }
        }

        public void SetWaitForServerProcessing()
        {
            foreach (var document in Documents.Where(d => d.IsAcknowledged))
            {
                // nakon zavrsene sinhronizacije cemo sve dokumente koji imaju status IsAcknoledged
                // obeležiti sa WaitForServerProcessing. Ovo se koristi samo u Inboxu, ali se sme primeniti svuda.
                document.WaitForServerProcessing = true;
            }
        }

        public void InternalMessengerGetStates(DocumentModel document)
        {
            // promenjen način čitanja atributa, tako da ne zavisi od tipa poruke (ne koristimo deserialize)
            // Čitamo bilo kakav xml koji ima atribute koje ocekujemo... 
            // proverimo poruku koja je u samom folderu:
            var fileName = Path.GetFileName(document.DocumentPath);
            var file = Path.Combine(FolderPath, fileName + ".xml");
            if (File.Exists(file))
            {
                try
                {
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load(file);
                    foreach (XmlNode node in xDoc.DocumentElement.ChildNodes)
                    {
                        if (node.Name == "isApprovedForProcessing")
                        {
                            if (node.InnerText == "true") document.isApprovedForProcessing = true;
                            else if (node.InnerText == "false") document.isApprovedForProcessing = false;
                        }
                        else if (node.Name == "isRejected")
                        {
                            if (node.InnerText == "true") document.isRejected = true;
                            else if (node.InnerText == "false") document.isRejected = false;
                        }
                        else if (node.Name == "archiveReady")
                        {
                            if (node.InnerText == "true") document.archiveReady = true;
                            else if (node.InnerText == "false") document.archiveReady = false;
                        }
                        else if (node.Name == "Processed" && document.Processed == false) // ne mozemo da vratimo dokument u prethodno stanje, moze samo iz false u true
                        {
                            if (node.InnerText == "true") document.Processed = true;
                            else if (node.InnerText == "false") document.Processed = false;
                        }
                    }
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
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load(file);
                    foreach (XmlNode node in xDoc.DocumentElement.ChildNodes)
                    {
                        if (node.Name == "isApprovedForProcessing")
                        {
                            if (node.InnerText == "true") document.isApprovedForProcessing = true;
                            else if (node.InnerText == "false") document.isApprovedForProcessing = false;
                        }
                        else if (node.Name == "isRejected")
                        {
                            if (node.InnerText == "true") document.isRejected = true;
                            else if (node.InnerText == "false") document.isRejected = false;
                        }
                        else if (node.Name == "archiveReady")
                        {
                            if (node.InnerText == "true") document.archiveReady = true;
                            else if (node.InnerText == "false") document.archiveReady = false;
                        }
                        else if (node.Name == "Processed" && document.Processed == false) // ne mozemo da vratimo dokument u prethodno stanje, moze samo iz false u true
                        {
                            if (node.InnerText == "true") document.Processed = true;
                            else if (node.InnerText == "false") document.Processed = false;
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error("Error: InternalMessengerGetStates ", e);
                }
            }
        }

        public void SerializeMessage(InternalMessageModel message)
        {
            try
            {
                var destinationDir = SigningTransferRules.LocalMap[FolderPath];
                var destinationFilePath = Path.Combine(destinationDir, message.MessageFileName);

                var file = File.Create(destinationFilePath);

                var xs = new XmlSerializer(typeof(InternalMessageModel));
                using (Stream s = file)
                    xs.Serialize(s, message);
            }
            catch (Exception e)
            {
                Log.Error("Error: SerializeMessage ", e);
            }
        }

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
                var message = new InternalMessageModel(document);
                SerializeMessage(message);
            }
        }

        protected override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await HandleAsync(new MessageGetPibNames(), cancellationToken);
        }

        protected override async Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            await _events.PublishOnUIThreadAsync(new MessageShowPdf(PreviewViewModel.Empty));
        }

        protected override void OnViewReady(object view)
        {
            // IsActive == true
        }

        public async Task HandleAsync(MessageGetPibNames message, CancellationToken cancellationToken)
        {
            var pf = Parent as FolderGroupViewModel;
            var activeFolder = pf?.FolderVMs.SingleOrDefault(f => f == pf.ActiveItem);

            //if (!IsActive) return;
            await GetPibNamesAsync(activeFolder?.Documents);
        }

        protected override void OnViewLoaded(object view)
        {

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
            else if (Regex.IsMatch(filePath, @".+.pdf.xml$", RegexOptions.IgnoreCase))
            {
                // xml-ove ne prikazujemo - mesto za akciju za xml fajl
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
                // da ne bude case sensitive...
                if (path.ToLower().EndsWith(".pdf"))
                    message = path;
            }
            _events.PublishOnUIThreadAsync(new MessageShowPdf(message)); 
        }

        public bool FilterDocument(Object item)
        {
            var doc = item as DocumentModel;
            if (doc != null && FilterText != null)
            {
                //if (doc.namePib2.StartsWith(FilterText))
                if (doc.DocumentInfo.Name.ToLower().Contains(FilterText.ToLower()) || doc.namePib2Name.ToLower().Contains(FilterText.ToLower()))
                    return true;
                else
                    return false;
            }
            return true;
        }

        private string _filterText;
        public string FilterText
        {
            get { return _filterText; }
            set
            {
                _filterText = value;
                if (_filterText.Length > 2) OnFilterText();
                if (_filterText.Length == 0) OnFilterText();

            }
        }

        public void OnFilterText()
        {
            DocumentsCV.Refresh();
        }

        #region FileSystemwatcher
        protected void InitWatcher(string path)
        {
            _watcher = new FileSystemWatcher(path) { NotifyFilter = NotifyFilters.Size | NotifyFilters.FileName };
            _watcher.InternalBufferSize = 65536;
            _watcher.Changed += Watcher_Changed; 
            _watcher.Deleted += Watcher_Deleted;
            _watcher.Created += Watcher_Created;
            _watcher.Renamed += Watcher_Renamed;
            _watcher.EnableRaisingEvents = true;
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            // TODO: 
            // NIJE Log-eru možda ovde mesto... hvata sve promene, a ne samo one koje napravi sinhronizacija
            // To jeste overhed, mada funkcionalno ne smeta - samo fajl dobija mark da ima neki eksterni xml... što jeste tačno...
             
        //    Log.Info("*** Promena:");
        //    Log.Info(e.FullPath);

            if ( Regex.Match(e.FullPath, @".xml", RegexOptions.IgnoreCase).Success ) {
                var docName = Regex.Replace(e.FullPath, @".xml", "", RegexOptions.IgnoreCase);
                var found = Documents.Where(d => d.DocumentPath == docName).FirstOrDefault();
                if (!(found == null)) found.hasExternalMessage = true;
         //       Log.Info("Ažuriran status ***");
            }
            
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

        public void Dispose()
        {
            Dispose(true);
        }

        public virtual void Dispose(bool disposing)
        {
            Serialize();
        }

        private void Serialize()
        {
            var filePath = Path.Combine(FolderPath, "state.xml");
            var file = File.Create(filePath);
            List<DocumentModel> items = Documents.Cast<DocumentModel>().ToList();
            var xs = new XmlSerializer(typeof(List<DocumentModel>));
            using (Stream s = file)
                xs.Serialize(s, items);
        }

        private List<DocumentModel> Deserialize()
        {
            var oldList = new List<DocumentModel>();
            var xs = new XmlSerializer(typeof(List<DocumentModel>));
            var file = Path.Combine(FolderPath, "state.xml");
            if (!File.Exists(file)) return oldList;
            try
            {
                using (Stream s = File.OpenRead(file))
                    oldList = (List<DocumentModel>)xs.Deserialize(s);
            }
            catch
            {

            }
            return oldList;
        }

        public virtual void OnCheck(object e)
        {
            var ec = e as ActionExecutionContext;
            var cb = ec.Source as CheckBox;

            var view = ec.View as FolderView;
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

        public static void renamePdf(string oldFile, string barcodeNumber, string newPath)
        {
            if (!File.Exists(oldFile))
            {
                return;
            }
            else
            {
                string newFile = "";
                try
                {
                    newFile = newPath + @"\" + barcodeNumber + ".pdf";
                    File.Move(oldFile, newFile);
                    Log.Info(oldFile + "-->" + newFile);
                }
                catch (IOException e)
                {
                    Log.Error(string.Format("IO exception - move to ARCH! Izvor: {0}, Odredtiste: {1} Izvorna greska:{2}", oldFile, newFile, e));
                    throw e;
                }
            }
        }
    }
}
