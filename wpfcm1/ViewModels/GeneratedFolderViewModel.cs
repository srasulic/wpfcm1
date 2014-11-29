using System.Collections.ObjectModel;
using System.Linq;
using wpfcm1.Model;

namespace wpfcm1.ViewModels
{
    public class GeneratedFolderViewModel : FolderViewModel
    {
        public GeneratedFolderViewModel(string path, string name) : base(path, name)
        {
        }

        private ObservableCollection<DocumentItem> _documents;
        public new ObservableCollection<DocumentItem> Documents
        {
            get { return _documents ?? new ObservableCollection<DocumentItem>(_repository.Files.Select(fi => new GeneratedDocumentItem(fi))); }
            private set { _documents = value; NotifyOfPropertyChange(() => Documents); }
        }
    }
}
