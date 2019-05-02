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
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
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
                _expList = "sep=,\n\"Mark\",\"Pib primalac\",\"Fajl\",\"KB\",\"Br Dok\"\r\n";
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


        public static bool IsPibOk (String pib)
        {
            if (string.IsNullOrWhiteSpace(pib))
            {
                return false;
            }
            var regexPib = new Regex(@"\b\d{9}\b");
            var regexJib = new Regex(@"\b\d{13}\b");
            // ako nije 9 ili 13 cifara vrati false
            if (!regexPib.IsMatch(pib) && !regexJib.IsMatch(pib))
            {
                return false;
            } 
            // ako je jedan od testnih vrati true
            if (pib == "1111111111111" || pib == "2222222222222" || pib == "111111111" || pib == "222222222" || pib == "333333333")
            {
                return true;
            }

            
            if (regexPib.IsMatch(pib))  // kontrola PIBa
            {
                int ost_pret = 10;
                string cifra;
                int i_cifra, suma, ostatak, umnozak, kontCifraIzracunata, kontCifra;

                for (int i = 0; i < 8; i++)
                {
                    cifra = pib.Substring(i, 1);
                    int.TryParse(cifra, out i_cifra);
                    suma = ost_pret + i_cifra;
                    ostatak = suma % 10;
                    if (ostatak == 0) { ostatak = 10; }
                    umnozak = ostatak * 2;
                    ost_pret = umnozak % 11;
                }

                int.TryParse(pib.Substring(pib.Length - 1, 1), out kontCifra);
                kontCifraIzracunata = (11 - ost_pret) % 10;


                if (kontCifraIzracunata != kontCifra) return false;
                else
                {
                    //provera da li je PIB korisnika(iz settings-a isti kao pib primaoca iz dokumenta za slanje)
                    //                if (User.Default.PIB == pib) throw new ApplicationException("PIB pimaoca je isto kao i PIB korisnika!");
                    if (User.Default.PIB == pib)
                    {
                        Log.Error("ERR: IsPibOK - logical error - PIB pimaoca je isto kao i PIB korisnika!");
                        return false;
                    }
                    return true;
                }
            } else if (regexJib.IsMatch(pib)) {   // kontrola JIB-a
                var j = new int[pib.Length];
                for (var i = 0; i < pib.Length; i++)
                {
                    j[i] = int.Parse(pib.ElementAt(i).ToString());
                }

                //A.B.V.G.D.Đ.E.Ž.Z.I.J.K.L
                //0.1.2.3.4.5.6.7.8.9.10.11.12
                //L = 11 - (( 7*(A+E) + 6*(B+Ž) + 5*(V+Z) + 4*(G+I) + 3*(D+J) + 2*(Đ+K) ) % 11)
                var l = 11 - ((7 * (j[0] + j[6]) + 6 * (j[1] + j[7]) + 5 * (j[2] + j[8]) + 4 * (j[3] + j[9]) + 3 * (j[4] + j[10]) + 2 * (j[5] + j[11])) % 11);
                l = l >= 10 ? 0 : l;

                if (l != j[12]) return false;
                else
                {
                    //provera da li je PIB korisnika(iz settings-a isti kao pib primaoca iz dokumenta za slanje)
                    if (User.Default.PIB == pib)
                    {
                        Log.Error("ERR: IsPibOK - logical error - PIB pimaoca je isto kao i PIB korisnika!");
                        return false;
                    }
                    return true;
                }
            } 
            
            // ako je nekako kod dosao doovde
            return false;
            
        }

        public async void Handle(MessageExtractData message)
        {
            if (!IsActive) return;


            var documents = Documents.Where(d => !d.Processed || d.IsChecked).Cast<GeneratedDocumentModel>();
            var pib = User.Default.PIB;
            if (string.IsNullOrEmpty(pib))
                throw new ApplicationException("PIB korisnika nije unet!");


            // iz putanje koja je u obliku "c:\\eDokument\\Faktura\\ERP_outbound_interface" uzimamo tip dokumenta
            var tipDok = Regex.Match(FolderPath, @"edokument\\(.*)\\", RegexOptions.IgnoreCase).Groups[1].ToString();
            // sada imamo PIB i tip dokumenta - možemo da uputimo web request upit za mapiranje i za regex

            RecognitionPatternModel recPatt = new RecognitionPatternModel();
            recPatt.SetRecognitionPatterns(pib, tipDok);

            // Probamo sve setove koordinata, dok ne pronadjemo i PIB i InvoiceNo. 
            // Uvek će ostati zapamćen poslednji prepoznati PIB
            // InvoiceNo će se prepoznavati SAMO ako je u toj grani prepoznat validan PIB
            // Ovo znači da pozicija PIBa diktira i da li će se broj dokumenta prepoznavati ili ne.
            foreach (var document in documents)
            {
                // odredimo da li je portrait, landsacpe ili rotirani portrait, kako bi odredili koje će se mapiranje koristiti:
                var pageOrientationResults = await PdfHelpers.ExtractOrientationRotationAsync(document.DocumentPath);

                bool isPortrait = pageOrientationResults.Item1;
                int pageRotation = pageOrientationResults.Item2;
                RecognitionPatternModel.PageOrientation pageOrientationType;
                if (isPortrait && pageRotation == 0) {
                    pageOrientationType = RecognitionPatternModel.PageOrientation.Portrait;
                } else if (!isPortrait && pageRotation == 0)
                {
                    pageOrientationType = RecognitionPatternModel.PageOrientation.Landscape;
                } else if (isPortrait && pageRotation == 90)
                {
                    pageOrientationType = RecognitionPatternModel.PageOrientation.RotatedPortrait;
                } else
                {
                    string orj = isPortrait ? "Portrait" : "Landscape";
                    Log.Error("ERR: Neprepoznata orjentacija dokumenta. Rotacija: " + pageRotation + "Orjentacija: " + orj);
                    pageOrientationType = RecognitionPatternModel.PageOrientation.Undefined;
                }



                // probamo sa svakim setom koordinata:

                for (var i = 0; i < recPatt.MappingElementList.Count; i++)
                {
                    var pibAtt = (RecognitionPatternModel.Coordinates)recPatt.MappingElementList[i].PibAttribute;
                    var docAtt = (RecognitionPatternModel.Coordinates)recPatt.MappingElementList[i].DocNumAttribute;
                    

                    if (string.IsNullOrEmpty(document.InvoiceNo) || string.IsNullOrEmpty(document.Pib) || recPatt.MappingElementList[i].isForcedMapping)
                    {
                        // ispunjen uslov da se mapiranje primeni (prema orjentaciji dokumenta)
                        if (recPatt.MappingElementList[i].pageOrientationSpecific == RecognitionPatternModel.PageOrientation.Undefined
                            || recPatt.MappingElementList[i].pageOrientationSpecific == pageOrientationType)
                        {
                            var matchResults = await PdfHelpers.ExtractTextAsync(document.DocumentPath, pibAtt, docAtt);

<<<<<<< HEAD
=======
                            // MatchCollection matches = Regex.Matches(matchResults.Item1, @"[1-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]");
                            //MatchCollection matches = Regex.Matches(matchResults.Item1, @"4[0-9]{12}");
                            // vraca sve nizove brojeva tako da nema problema sa substringovima dugackih brojeva poput bank racuna
>>>>>>> feature_bih
                            MatchCollection matches = Regex.Matches(matchResults.Item1, @"[0-9]+");
                            foreach (Match match in matches)
                            {
                                // ako mapiranje vraća neki validan PIB, idemo na traženje broja dokumenta
                                if (IsPibOk(match.Value))
                                {
                                    document.Pib = match.Value;
                                    // nulujemo bilo kakav broj dokumenta koji je pronađen u prethodnim iteracijama (ako je mapiranjem pogođen PIB, primenjuje se novo traženje broja dokumenta
                                    // ovo je važno samo ako uvedemo mapiranje koje je obavezno (koje će se primeniti i ako se prethodnim traženjem već pronađu PIB i neki broj dok (dešava se da slab regex nađe broj pogrešnim mapiranjima)
                                    document.InvoiceNo = null;
                                    switch (recPatt.MappingElementList[i].regexToApply)
                                    {
                                        case 1:
                                            if (string.IsNullOrEmpty(document.InvoiceNo))
                                                document.InvoiceNo = Regex.Match(matchResults.Item2, recPatt.DocRegexList.Regex1, RegexOptions.Multiline).Value;
                                            break;
                                        case 2:
                                            if (string.IsNullOrEmpty(document.InvoiceNo))
                                                document.InvoiceNo = Regex.Match(matchResults.Item2, recPatt.DocRegexList.Regex2, RegexOptions.Multiline).Value;
                                            break;
                                        case 3:
                                            if (string.IsNullOrEmpty(document.InvoiceNo))
                                                document.InvoiceNo = Regex.Match(matchResults.Item2, recPatt.DocRegexList.Regex3, RegexOptions.Multiline).Value;
                                            break;
                                        case 4:
                                            if (string.IsNullOrEmpty(document.InvoiceNo))
                                                document.InvoiceNo = Regex.Match(matchResults.Item2, recPatt.DocRegexList.Regex4, RegexOptions.Multiline).Value;
                                            break;
                                        case 5:
                                            if (string.IsNullOrEmpty(document.InvoiceNo))
                                                document.InvoiceNo = Regex.Match(matchResults.Item2, recPatt.DocRegexList.Regex5, RegexOptions.Multiline).Value;
                                            break;
                                        case 6:
                                            if (string.IsNullOrEmpty(document.InvoiceNo))
                                                document.InvoiceNo = Regex.Match(matchResults.Item2, recPatt.DocRegexList.Regex1, RegexOptions.Multiline).Value;
                                            if (string.IsNullOrEmpty(document.InvoiceNo))
                                                document.InvoiceNo = Regex.Match(matchResults.Item2, recPatt.DocRegexList.Regex2, RegexOptions.Multiline).Value;
                                            if (string.IsNullOrEmpty(document.InvoiceNo))
                                                document.InvoiceNo = Regex.Match(matchResults.Item2, recPatt.DocRegexList.Regex3, RegexOptions.Multiline).Value;
                                            break;
                                        case 0:
                                            if (string.IsNullOrEmpty(document.InvoiceNo))
                                                document.InvoiceNo = Regex.Match(matchResults.Item2, recPatt.DocRegexList.Regex1, RegexOptions.Multiline).Value;
                                            if (string.IsNullOrEmpty(document.InvoiceNo))
                                                document.InvoiceNo = Regex.Match(matchResults.Item2, recPatt.DocRegexList.Regex2, RegexOptions.Multiline).Value;
                                            if (string.IsNullOrEmpty(document.InvoiceNo))
                                                document.InvoiceNo = Regex.Match(matchResults.Item2, recPatt.DocRegexList.Regex3, RegexOptions.Multiline).Value;
                                            if (string.IsNullOrEmpty(document.InvoiceNo))
                                                document.InvoiceNo = Regex.Match(matchResults.Item2, recPatt.DocRegexList.Regex4, RegexOptions.Multiline).Value;
                                            if (string.IsNullOrEmpty(document.InvoiceNo))
                                                document.InvoiceNo = Regex.Match(matchResults.Item2, recPatt.DocRegexList.Regex5, RegexOptions.Multiline).Value;
                                            break;
                                    }

                                }
                            }
                        }
                    }
                }





                // ako posle svega nije prepoznat broj, dodeli mu vrednost za Neprepoznat string
                if (string.IsNullOrEmpty(document.InvoiceNo))
                {
                    document.InvoiceNo = recPatt.DocRegexList.notRecognizedString;
                }

                Regex regexAllowedCharacters = new Regex(@"[^0-9a-zA-Z]");
                document.InvoiceNo = regexAllowedCharacters.Replace(document.InvoiceNo, @"-");

                document.Processed = true;

                //Dodajemo proveru validnosti podataka koje smo dobili:
                // (ovo se radi i na pregledu - gridu ali tamo se obrade samo prikazani pa IsValid ostane nedodeljeno ako se ne skroluje)

                document.IsValid = IsPibOk(document.Pib);

                if (string.IsNullOrWhiteSpace(document.InvoiceNo))
                {
                    document.IsValid = false;
                }

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
