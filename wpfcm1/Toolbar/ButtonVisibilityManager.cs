using Caliburn.Micro;
using System.Diagnostics;
using wpfcm1.Events;

namespace wpfcm1.Toolbar
{
    public class ButtonVisibilityManager : IHandle<ViewModelActivatedMessage>
    {
        private ToolBarViewModel _toolBar;
        private IEventAggregator _events;

        public ButtonVisibilityManager(ToolBarViewModel toolbar, IEventAggregator events)
        {
            _toolBar = toolbar;
            _events = events;
            _events.Subscribe(this);
        }

        public void Handle(ViewModelActivatedMessage message)
        {
            Debug.WriteLine(message.Name);
            switch (message.Name)
            {
                case "HomeViewModel":
                    break;
                case "GeneratedFolderViewModel":
                    break;
                case "InboxFolderViewModel":
                    break;
                case "FolderViewModel":
                    break;
            }
        }
    }
}
