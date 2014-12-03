using Caliburn.Micro;
using System.IO;
using System.Linq;
using wpfcm1.Model;

namespace wpfcm1.ViewModels
{
    public class GeneratedFolderViewModel : FolderViewModel
    {
        public GeneratedFolderViewModel(string path, string name) : base(path, name)
        {
        }

        protected override void InitDocuments()
        {
            Documents = new BindableCollection<DocumentItem>(
                 Directory.EnumerateFiles(FolderPath)
                 .Where(f => Extensions.Contains(Path.GetExtension(f)))
                 .Select(f => new GeneratedDocumentItem(new FileInfo(f))));
            InitWatcher(FolderPath);
        }

        protected override void AddFile(string filePath)
        {
            Documents.Add(new GeneratedDocumentItem(new FileInfo(filePath)));
        }

        protected override void OnActivate()
        {

        }

        protected override void OnDeactivate(bool close)
        {

        }
    }
}
