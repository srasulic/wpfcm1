using Caliburn.Micro;
using System.Collections.ObjectModel;
using System.IO;
using wpfcm1.DataAccess;

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
            Documents = new ObservableCollection<FileInfo>(_repository.Files);
        }

        public string Path { get; private set; }
        public ObservableCollection<FileInfo> Documents { get; private set; } 
    }
}
