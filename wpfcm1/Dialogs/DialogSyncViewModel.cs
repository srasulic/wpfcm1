using Caliburn.Micro;

namespace wpfcm1.Dialogs
{
    public class DialogSyncViewModel : Screen
    {
        public DialogSyncViewModel()
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
