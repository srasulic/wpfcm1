using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using wpfcm1.FTP;
using wpfcm1.Model;
using wpfcm1.PDF;

namespace wpfcm1.Processing
{
    public class SyncManager
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public async Task Upload(
            FtpClient ftpClient, IEnumerable<DocumentModel> documents, string sourceDir, string destinationDir,
            IProgress<string> reporter = null, CancellationToken token = default(CancellationToken))
        {
            foreach (var document in documents)
            {
                var sourceFilePath = document.DocumentPath;
                var sourceFileName = Path.GetFileName(sourceFilePath);
                var tempFileName = sourceFileName + ".tmp";

                token.ThrowIfCancellationRequested();
                if (reporter != null) reporter.Report(string.Format("Uploading: {0}", sourceFileName));
                Log.Info(string.Format("Uploading {0}", sourceFilePath));
                try {
                    await ftpClient.UploadFileAsync(sourceFilePath, destinationDir, tempFileName);

                    var destinationFtpUri = string.Format("{0}{1}{2}", ftpClient.Uri, destinationDir, tempFileName);
                    ftpClient.RenameFile(destinationFtpUri, sourceFileName);

                    //                var destinationFileName = string.Format("{0}_{1}", DateTime.UtcNow.ToString("yyyyMMddHHmmssfff"), Path.GetFileName(sourceFileName));
                    var destinationFilePath = Path.Combine(FtpTransferRules.LocalMap[sourceDir], Path.GetFileName(sourceFileName));
                    Log.Info(string.Format("Moving to {0}", destinationFilePath));

                    //  File.Move(sourceFilePath, destinationFilePath);
                    try
                    {
                        File.Move(sourceFilePath, destinationFilePath);
                    }
                    catch (IOException e)
                    {
                        var fn1 = Regex.Match(Path.GetFileName(sourceFileName), @"[0-9]{9,13}_[0-9]{9,13}_.+_[0-9]+");
                        var fn2 = Regex.Match(Path.GetFileName(sourceFileName), @"_s.+");
                        var newDestFileName = fn1 + "x" + DateTime.UtcNow.ToString("yyyyMMddHHmmssfff") + "x" + fn2;
                        var newDestPath = Path.Combine(FtpTransferRules.LocalMap[sourceDir], newDestFileName);
                        File.Move(sourceFilePath, newDestPath);
                        Log.Info(string.Format("IO exception - move to local_sent - već postoji takav fajl! Izvor: {0}, Odredtiste: {1} Izvorna greska:{2}", sourceFilePath, newDestPath, e));
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("ERR: ************************************ ");
                    Log.Error(string.Format(@"{0}{1}{2}", "Error: Error while uploading file ", " ", sourceFilePath), ex);
                    Log.Error("Warning: Upload process will proceed... ");
                    (reporter as IProgress<string>).Report("ERR: ************************************ ");
                    (reporter as IProgress<string>).Report(string.Format(@"{0}{1}{2}", "Error while uploading file ", " ", sourceFilePath));
                    (reporter as IProgress<string>).Report(ex.Message);
                    (reporter as IProgress<string>).Report("Warning: Upload process will proceed... ");
                    (reporter as IProgress<string>).Report("     ************************************ ");
                    // TODO: Šta da radimo sa ovim fajlovima / greškama???
                    //       Treba da nekako jasno korisniku signalizirati ovu situaciju
                }

                token.ThrowIfCancellationRequested();




            }
        }

        public async Task Sync(
            FtpClient ftpClient, IEnumerable<DocumentModel> documents, string sourceDir, string destinationDir, bool deleteLocal,
            IProgress<string> reporter = null, CancellationToken token = default(CancellationToken))
        {
            Log.Info(string.Format("Listing directory {0}", destinationDir));
            var localFileNames = documents.Select(di => di.DocumentInfo.Name);
            var remoteFileNames = await ftpClient.ListDirectoryAsync(destinationDir);
            // jeftinije je da imamo dva ftp upita (za nazive i za detalje) jer parsiranje naziva fajlova iz detalja koje vrati ftp server zavisi od implementacije ftp servera i njegovog OS-a
            // ovako u najgorem slučaju neće biti pouzdan filter za xml poruke te će se sve svlačiti
            // TODO: preveli smo ovo u sinhronu metodu jer je asinhrona vraćal asmao deo liste fajlova. Kroz Debugger je radila ispravno?!
            //
            var remoteFileDetails = ftpClient.ListDirectoryDetails(destinationDir);

            var local = new SortedSet<string>(localFileNames);
            var remote = new SortedSet<string>(remoteFileNames);
            //  var remote = new SortedSet<string>(remoteFileNames.Where(f => !(Regex.Match(f, "syncstamp", RegexOptions.IgnoreCase).Success)));
            var diffLocal = local.Except(remote);
            var diffRemote = remote.Except(local);
            // TODO: quick fix - svaki put cemo da skinemo i sve xml poruke sa servera
            //       da bi nam se dopremile eventualne nove poruke.
            //       Razmotriti kako filtrirati ovo tako da skinemo samo nove xml poruke
            // (za sad smo samo iskljucili da se isti ne svlače dva puta u istoj sinhronizaciji)
            var remoteXmlFiles = remote.Except(diffRemote).Where(f => (Regex.Match(f, "xml", RegexOptions.IgnoreCase).Success));
            var localDirectoryInfo = new DirectoryInfo(sourceDir);
            // poslednji datum sinhronizacije
            // TODO: opasnost: oslanjamo se na lokalno vreme radne stanice
            var syncStampFile = localDirectoryInfo.GetFiles("*.syncstamp")
                                 .OrderByDescending(f => f.LastWriteTime)
                                 .FirstOrDefault();
            
            // brisanje iz ilste za prenos: za svaki remote xml fajl ćemo proveriti datum 
            // Ako je stariji od poslednje sinhronizacije brišemo ga iz liste za prenos
            // TODO: oslanjamo se na ftp osobinu da svaki fajl dobija novi CreationDate prilikom sinhronizacije!!!
            //       Kada su u pitanju XML poruke, ovo ima smisla da ostane deo protokola, jer poruka postaje aktuelna tek kada doputuje na server
            // TODO: ako neko generiše poruke u aplikaciji i ne sinhronizuje se, poruke mogu pregaziti kasnije neke sveže informacije...
            //       Možda će server da brine o ovome kada primi poruke u outbox
            //       Možda će aplikacija da postane online i da odmah generiše xml na serveru, ili još bolje u bazi na serveru (tamo se mogu čuvati sve poruke)
            
            // ako ne postoji lokalni sync fajl, nema šta da radim ou ovoj petlji:
            if (syncStampFile != null)
            {
                foreach (var fileDetails in remoteFileDetails.Where(f => (Regex.Match(f, "xml", RegexOptions.IgnoreCase).Success)))
                {
                    string[] ftpFileInfo = fileDetails.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    var remFileName = ftpFileInfo[8];
                    var remYear = ftpFileInfo[7];
                    // ako u ovom polju dobijemo dvotačku, znači da je fajl mlađi od 6 meseci i da je u listi umesto godine vreme
                    // to menjamo tekućom godinom. Ovim možda generišemo datum u budućnosti
                    if (Regex.Match(remYear, ":").Success)
                    {
                        remYear = DateTime.Today.ToString("yyyy");
                    }
                    var remFileDate = Convert.ToDateTime(ftpFileInfo[6] + " " + ftpFileInfo[5] + " " + remYear);
                    // ako smo napravili datum u budcnosti, trebalo je da stavimo proslu godinu:
                    if (remFileDate > DateTime.Today)
                    {
                        remFileDate = remFileDate.AddYears(-1);
                    }
                    // TODO: današnje poruke će se svaki put dovući sa servera, opasnost da se _privremeno_ nekalokalna poruka zamen starijom
                    //       Još jedan razlog da počnemo da pamtimo na serveru statuse u bazi...
                    if ((syncStampFile != null) && (remFileDate < syncStampFile.CreationTime))
                    {
                        remoteXmlFiles = remoteXmlFiles.Where(f => !(f.Equals(remFileName)));
                    }

                }

            }

            IFormatProvider culture = new System.Globalization.CultureInfo("en-US", true);
            foreach (var fileName in diffRemote)
            {
                var filePath = Path.Combine(sourceDir, fileName);
                if (reporter != null) reporter.Report(string.Format("Download: {0}", fileName));
                Log.Info(string.Format("Downloading {0}", filePath));
                token.ThrowIfCancellationRequested();
                try
                {
                    await ftpClient.DownloadFileAsync(destinationDir, fileName, filePath);
                }
                catch (Exception ex)
                {
                    Log.Error("ERR: ************************************ ");
                    Log.Error(string.Format(@"{0}{1}{2}", "Error: Error while downloading file ", " ", fileName), ex);
                    Log.Error("Warning: Sync process will proceed... ");
                    (reporter as IProgress<string>).Report("ERR: ************************************ ");
                    (reporter as IProgress<string>).Report(string.Format(@"{0}{1}{2}", "Error while downloading file ", " ", fileName));
                    (reporter as IProgress<string>).Report(ex.Message);
                    (reporter as IProgress<string>).Report("Warning: Sync process will proceed... ");
                    (reporter as IProgress<string>).Report("     ************************************ ");
                    // TODO: Šta da radimo sa ovim fajlovima / greškama???
                    //       Treba da nekako jasno korisniku signalizirati ovu situaciju
                    //       (exception se može reprodukovati ako se npr. na server spusti fajl za koji user nema pravi čitanja)
                }

            }

            foreach (var fileName in remoteXmlFiles)
            {
                var filePath = Path.Combine(sourceDir, fileName);
                if (reporter != null) reporter.Report(string.Format("Download: {0}", fileName));
                Log.Info(string.Format("Downloading {0}", filePath));
                token.ThrowIfCancellationRequested();
                try
                {
                    await ftpClient.DownloadFileAsync(destinationDir, fileName, filePath);
                }
                catch (Exception ex)
                {
                    Log.Error("ERR: ************************************ ");
                    Log.Error(string.Format(@"{0}{1}{2}", "Error: Error while downloading xml file ", " ", fileName), ex);
                    Log.Error("Warning: Sync process will proceed... ");
                    (reporter as IProgress<string>).Report("ERR: ************************************ ");
                    (reporter as IProgress<string>).Report(string.Format(@"{0}{1}{2}", "Error while downloading xml file ", " ", fileName));
                    (reporter as IProgress<string>).Report(ex.Message);
                    (reporter as IProgress<string>).Report("Warning: Sync process will proceed... ");
                    (reporter as IProgress<string>).Report("     ************************************ ");
                    // TODO: Šta da radimo sa ovim fajlovima / greškama???
                    //       Treba da nekako jasno korisniku signalizirati ovu situaciju
                    //       (exception se može reprodukovati ako se npr. na server spusti fajl za koji user nema pravi čitanja)
                }
            }

            if (deleteLocal)
            {
                foreach (var fileName in diffLocal)
                {
                    var filePath = Path.Combine(sourceDir, fileName);
                    if (reporter != null) reporter.Report(string.Format("Delete: {0}", fileName));
                    Log.Info(string.Format("Deleting {0}", filePath));
                    File.Delete(filePath);
                }
            }
        }
    }
}
