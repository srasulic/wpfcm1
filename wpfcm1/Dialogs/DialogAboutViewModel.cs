using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Caliburn.Micro;
using wpfcm1.Settings;

namespace wpfcm1.Dialogs
{
    public class DialogAboutViewModel : Screen
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private string aboutText = "";
        public DialogAboutViewModel()
        {
            DisplayName = "About & Legal";
            //appVersion = AppBootstrapper.appVersion;
            aboutText = AppBootstrapper.appVersion + " \r\n " +
                " \r\n " +
                "Opšti uslovi: \r\n " +
                "-------------- \r\n " +
                User.Default.ApiURL + "/download/Opsti_uslovi_eDokument.pdf \r\n " +
                "\r\n" +
                "Licenciranje: \r\n ------------- \r\n" +
                "eDokument PoliSign se licencira prema pravilima GNU Affero General Public \r\n" +
                "License (AGPL) (https://www.gnu.org/licenses/agpl.txt). eDokumentPolSign se \r\n" +
                "može slobodno i besplatno koristiti za potpisivanje dokumenata. \r\n" +
                "eDokument PoliSign se može koristiti kao klijent za razmenu dokumenata \r\n" +
                "putem eDokument Kliring Servera. Usluga transporta dokumenata i \r\n" +
                "održavanje eDokument Kliring servera su komercijalne usluge preduzeća \r\n" +
                "Aserta doo i njihovo korišćenje se zasebno ugovara (kontakt: office@aserta.rs). \r\n" +
                "\r\n" +
                "--\r\n" +
                "Podrška: e-dokument@aserta.rs \r\n" +
                "Autori: \r\n" +
                "          Sergej Petrovski \r\n" +
                "          Srđan Rasulić \r\n" +
                "    Aserta doo \r\n" +
                "        http://www.aserta.rs \r\n" +
                "        https://polisign.net \r\n" +
                "        https://edokument.aserta.rs \r\n";
        }

        public void ShowLog()
        {
            Process p = new Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.FileName = "fakture.log.txt";
            p.Start();
        }

        public string AboutText { get => aboutText; set => aboutText = value; }

        public void AeroHelp()
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "Aeroadmin.exe",

                    WorkingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                };

                Process process = new Process
                {
                    StartInfo = startInfo
                };

                process.Start();
                // process.WaitForExit();
            }
            catch (Exception ex)
            {
                Log.Error("Error while running Aeroadmin.exe ...", ex);
            }
        }

        public async Task OnClose()
        {
            await TryCloseAsync(true);
        }
    }
}
