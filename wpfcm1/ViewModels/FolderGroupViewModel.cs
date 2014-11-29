using Caliburn.Micro;
using System.Collections.Generic;
using wpfcm1.DataAccess;

namespace wpfcm1.ViewModels
{
    public class FolderGroupViewModel : Conductor<IScreen>.Collection.OneActive
    {
        public FolderGroupViewModel(Dictionary<string,string> wsFolders, string name)
        {
            DisplayName = name;
            FolderVMs = new BindableCollection<FolderViewModel>();
            foreach (var wsFolder in wsFolders)
            {
                FolderVMs.Add(new FolderViewModel(wsFolder.Value, FolderManager.FolderNameMap[wsFolder.Key]));
            }
        }

        public IObservableCollection<FolderViewModel> FolderVMs { get; private set; }
    }
}
