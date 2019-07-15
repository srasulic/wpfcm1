using Caliburn.Micro;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Xml.Serialization;
using wpfcm1.Dialogs;
using wpfcm1.Events;
using wpfcm1.Model;
using wpfcm1.PDF;
using wpfcm1.Preview;
using System;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace wpfcm1.FolderTypes
{
    public class PendFolderViewModel : FolderViewModel, IHandle<MessageXls>
    {
        private readonly IWindowManager _windowManager;
        new protected string[] Extensions = { ".pdf", ".ack", ".xml" };

        //public ListCollectionView DocumentsCV { get; set; }

        public PendFolderViewModel(string path, string name, IEventAggregator events, IWindowManager winMgr) : base(path, name, events)
        {
            _windowManager = winMgr;
        }

        protected override void InitDocuments()
        {
            Documents = new BindableCollection<DocumentModel>(
                Directory.EnumerateFiles(FolderPath)
                .Where(f => Extensions.Contains(Path.GetExtension(f).ToLower()))
                .Where(f => !(Regex.IsMatch(Path.GetFileName(f), @"stat.+\.xml", RegexOptions.IgnoreCase)))
                .Select(f => new PendDocumentModel(new FileInfo(f))));
            
            InitWatcher(FolderPath);

            DocumentsCV = CollectionViewSource.GetDefaultView(Documents) as ListCollectionView;
            DocumentsCV.Filter = new Predicate<object>(FilterDocument);

            if (Documents.Count == 0) return;
            var states = Deserialize();
            foreach (var state in states)
            {
                var found = Documents.FirstOrDefault(d => d.DocumentPath == state.DocumentPath);
                if (found == null) continue;
                var old = found as PendDocumentModel;
                old.IsChecked = state.IsChecked;
                old.IsValid = state.IsValid;
                old.Processed = state.Processed;

                old.namePib1Name = state.namePib1Name;
                old.namePib2Name = state.namePib2Name;
            }
        }

        //public bool FilterDocument(Object item)
        //{
        //    var doc = item as DocumentModel;
        //    if (doc != null && FilterText != null)
        //    {
        //        //if (doc.namePib2.StartsWith(FilterText))
        //        if (doc.DocumentInfo.Name.ToLower().Contains(FilterText.ToLower()) || doc.namePib2Name.ToLower().Contains(FilterText.ToLower()))
        //            return true;
        //        else
        //            return false;
        //    }
        //    return true;
        //}

        //private string _filterText;
        //public string FilterText
        //{
        //    get { return _filterText; }
        //    set
        //    {
        //        _filterText = value;
        //        if (_filterText.Length > 2) OnFilterText();
        //        if (_filterText.Length == 0) OnFilterText();
                
        //    }
        //}

        //public void OnFilterText()
        //{
        //    DocumentsCV.Refresh();
        //}

        protected override void AddFile(string filePath)
        {
            if (Regex.IsMatch(filePath, @".+syncstamp$", RegexOptions.IgnoreCase))
            {
                InternalMessengerGetStates();
            }
            // necemo u listu dodavati one koji nisu validno nazvan pdf
            else if (!Regex.IsMatch(filePath, @".+_s.pdf$", RegexOptions.IgnoreCase))
            {
                
            }
            else
            {
                Documents.Add(new PendDocumentModel(new FileInfo(filePath)));
            }
        }

        protected override void OnActivate()
        {
            _events.PublishOnUIThread(new MessageViewModelActivated(GetType().Name));
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            //TODO: hack: checkbox checkmark moze da se izgubi prilikom promene taba, ako promena nije komitovana
            var v = GetView() as UserControl;
            var dg = v.FindName("DocumentsCV") as DataGrid;
            dg.CommitEdit(DataGridEditingUnit.Row, true);
        }

        public void Handle(MessageXls message)
        {
            if (!IsActive) return;
            XlsExport();
        }

        public override void Dispose(bool disposing)
        {
            Serialize();
        }

        private void Serialize()
        {
            var filePath = Path.Combine(FolderPath, "state.xml");
            var file = File.Create(filePath);
            List<PendDocumentModel> items = Documents.Cast<PendDocumentModel>().ToList();
            var xs = new XmlSerializer(typeof(List<PendDocumentModel>));
            using (Stream s = file)
                xs.Serialize(s, items);
        }

        private List<PendDocumentModel> Deserialize()
        {
            var oldList = new List<PendDocumentModel>();
            var xs = new XmlSerializer(typeof(List<PendDocumentModel>));
            var file = Path.Combine(FolderPath, "state.xml");
            if (!File.Exists(file)) return oldList;
            try
            {
                using (Stream s = File.OpenRead(file))
                    oldList = (List<PendDocumentModel>)xs.Deserialize(s);
            }
            catch
            {
                
            }
            return oldList;
        }

/*        public override void OnCheck(object e)
        {
            var ec = e as ActionExecutionContext;
            var cb = ec.Source as CheckBox;

            var view = ec.View as PendFolderView;
            var dg = view.DocumentsCV;
            var items = dg.SelectedItems;
            foreach (var item in items)
            {
                var doc = item as DocumentModel;
                doc.IsChecked = cb.IsChecked.GetValueOrDefault();
            }
        } */
    }
}
