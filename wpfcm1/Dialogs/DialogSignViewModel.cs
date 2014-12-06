using Caliburn.Micro;

namespace wpfcm1.Dialogs
{
    public class DialogSignViewModel : Screen
    {
        public DialogSignViewModel()
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
