using Caliburn.Micro;

namespace wpfcm1.Dialogs
{
    public class DialogSignGeneratedViewModel : Screen
    {
        public DialogSignGeneratedViewModel()
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
