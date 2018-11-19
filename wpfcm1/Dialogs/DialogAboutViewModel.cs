using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using wpfcm1.Extensions;
using wpfcm1.FolderTypes;
using wpfcm1.FTP;
using wpfcm1.Model;
using wpfcm1.PDF;
using wpfcm1.Processing;
using wpfcm1.Settings;

namespace wpfcm1.Dialogs
{
    public class DialogAboutViewModel : Screen
    {
        private string aboutText = "";
        public DialogAboutViewModel()
        {
            DisplayName = "About & Legal";
            //appVersion = AppBootstrapper.appVersion;
            aboutText = "eDokument PoliSign ver " + AppBootstrapper.appVersion + " \r\n " + 
                " \r\n " +
                "Opšti uslovi: \r\n " + 
                "-------------- \r\n " +
                "https://edokument.aserta.rs/download/Opsti_uslovi_eDokument.pdf \r\n " + 
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
                "Autor:   Sergej Petrovski \r\n" + 
                "Autor:   Aserta doo \r\n" + 
                "            http://www.aserta.rs \r\n" + 
                "            https://edokument.aserta.rs \r\n" ;
        }

        public string AboutText { get => aboutText; set => aboutText = value; }

        public void OnClose()
        {
            TryClose(true);
        }

    }
}
