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
        public DialogAboutViewModel()
        {
            DisplayName = "About & Legal";
        }


        public void OnClose()
        {
            TryClose(true);
        }

    }
}
