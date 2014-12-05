using System.Collections.Generic;
using Caliburn.Micro;
using wpfcm1.DataAccess;
using wpfcm1.FolderTypes;

namespace wpfcm1.FolderGroups
{
    public class FolderGroupViewModel : Conductor<IScreen>.Collection.OneActive
    {
        private readonly IEventAggregator _events;

        public FolderGroupViewModel(Dictionary<string,string> wsFolders, string name, IEventAggregator events, IWindowManager windowManager)
        {
            DisplayName = name;
            _events = events;

            FolderVMs = new BindableCollection<FolderViewModel>();
            foreach (var wsFolder in wsFolders)
                switch (FolderManager.FolderTypeMap[wsFolder.Key].Name)
                {
                    case "GeneratedDocumentModel":
                        FolderVMs.Add(new GeneratedFolderViewModel(wsFolder.Value, FolderManager.FolderNameMap[wsFolder.Key], _events, windowManager));
                        break;
                    case "InboxDocumentModel":
                        FolderVMs.Add(new InboxFolderViewModel(wsFolder.Value, FolderManager.FolderNameMap[wsFolder.Key], _events, windowManager));
                        break;
                    default:
                        FolderVMs.Add(new FolderViewModel(wsFolder.Value, FolderManager.FolderNameMap[wsFolder.Key], _events));
                        break;
                }
        }

        public IObservableCollection<FolderViewModel> FolderVMs { get; private set; }

        public void ActivateTabItem(int idx)
        {
            ActivateItem(FolderVMs[idx]);
        }

        protected override void OnActivate()
        {
            //_events.PublishOnUIThread(new ViewModelActivatedMessage(GetType().Name));
            if (ActiveItem == null)
                ActivateTabItem(0);
            else
                ActivateItem(ActiveItem);
        }

        protected override void OnDeactivate(bool close)
        {
            DeactivateItem(ActiveItem, false);
        }
    }
}
