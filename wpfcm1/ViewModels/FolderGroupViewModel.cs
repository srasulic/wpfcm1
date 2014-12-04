using Caliburn.Micro;
using System.Collections.Generic;
using wpfcm1.DataAccess;

namespace wpfcm1.ViewModels
{
    public class FolderGroupViewModel : Conductor<IScreen>.Collection.OneActive
    {
        private readonly IEventAggregator _events;

        public FolderGroupViewModel(Dictionary<string,string> wsFolders, string name, IEventAggregator events)
        {
            DisplayName = name;
            _events = events;

            FolderVMs = new BindableCollection<FolderViewModel>();
            foreach (var wsFolder in wsFolders)
                switch (FolderManager.FolderTypeMap[wsFolder.Key].Name)
                {
                    case "GeneratedDocumentItem":
                        FolderVMs.Add(new GeneratedFolderViewModel(wsFolder.Value,
                            FolderManager.FolderNameMap[wsFolder.Key], _events));
                        break;
                    case "InboxDocumentItem":
                        FolderVMs.Add(new InboxFolderViewModel(wsFolder.Value, FolderManager.FolderNameMap[wsFolder.Key],
                            _events));
                        break;
                    default:
                        FolderVMs.Add(new FolderViewModel(wsFolder.Value, FolderManager.FolderNameMap[wsFolder.Key],
                            _events));
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
            ActivateTabItem(0);
        }

        protected override void OnDeactivate(bool close)
        {
            DeactivateItem(ActiveItem, false);
        }
    }
}
