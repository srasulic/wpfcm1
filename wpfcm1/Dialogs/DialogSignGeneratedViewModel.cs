using Caliburn.Micro;
using System.Collections.Generic;
using wpfcm1.Model;

namespace wpfcm1.Dialogs
{
    public class DialogSignGeneratedViewModel : Screen
    {
        public DialogSignGeneratedViewModel(CertificateModel certificate, IList<GeneratedDocumentModel> documents)
        {
            DisplayName = "";
        }

        public void OnClose()
        {
            TryClose(true);
        }

        public void OnCancel()
        {
            TryClose(false);
        }
    }
}
