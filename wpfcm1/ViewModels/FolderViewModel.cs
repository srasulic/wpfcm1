using Caliburn.Micro;
using System.Collections.ObjectModel;
using System.Linq;
using wpfcm1.DataAccess;
using wpfcm1.Model;

namespace wpfcm1.ViewModels
{
    public class FolderViewModel : Screen
    {
        private DocumentRepository _repository;

        public FolderViewModel(string path, string name)
        {
            Path = path;
            DisplayName = name;
            _repository = new DocumentRepository(path);
            Documents = new ObservableCollection<DocumentItem>(_repository.Files.Select(fi => new DocumentItem(fi.FullName)));
        }

        public string Path { get; private set; }
        public ObservableCollection<DocumentItem> Documents { get; private set; }
        public int Count { get { return Documents.Count; } }
    }
}
