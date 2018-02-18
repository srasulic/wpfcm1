using Caliburn.Micro;
using System;
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
using wpfcm1.Settings;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;


namespace wpfcm1.FolderTypes
{
    public class OutboxFolderViewModel : FolderViewModel, IHandle<MessageReject>, IHandle<MessageXls>
    {
        private readonly IWindowManager _windowManager;
//        new protected string[] Extensions = { ".pdf", ".ack", ".xml" };
        new protected string[] Extensions = { ".pdf", ".ack" };


        public OutboxFolderViewModel(string path, string name, IEventAggregator events, IWindowManager winMgr) : base(path, name, events)
        {
            _windowManager = winMgr;
        }

        protected override void InitDocuments()
        {
            Documents = new BindableCollection<DocumentModel>(
                Directory.EnumerateFiles(FolderPath)
                .Where(f => Extensions.Contains(Path.GetExtension(f).ToLower()))
                .Where(f => !( Regex.IsMatch(Path.GetFileName(f) , @"stat.+\.xml",RegexOptions.IgnoreCase) ) )
                .Select(f => new OutboxDocumentModel(new FileInfo(f))));

            InitWatcher(FolderPath);

            DocumentsCV = CollectionViewSource.GetDefaultView(Documents) as ListCollectionView;
            DocumentsCV.Filter = new Predicate<object>(FilterDocument);

            if (Documents.Count == 0) return;
            var states = Deserialize();
            foreach (var state in states)
            {
                var found = Documents.FirstOrDefault(d => d.DocumentPath == state.DocumentPath);
                if (found == null) continue;
                var old = found as OutboxDocumentModel;
                old.IsChecked = state.IsChecked;
                old.IsValid = state.IsValid;
                old.Processed = state.Processed;

                old.namePib1Name = state.namePib1Name;
                old.namePib2Name = state.namePib2Name;
            }
        }

        // nećemo prikazivati ack i xml fajlove koji su za slanje
        protected override void AddFile(string filePath)
        {
            if (Regex.IsMatch(filePath, @".+syncstamp$", RegexOptions.IgnoreCase))
            {
                // za outbox - mesto za akciju za kraj sinhronizacije
            }
            else if (Regex.IsMatch(filePath, @".+.pdf.ack$", RegexOptions.IgnoreCase))
            {
                // mesto za akciju za ack fajl
                Documents.Add(new OutboxDocumentModel(new FileInfo(filePath)));
//                var docName = filePath;
//                var found = Documents.Where(d => d.DocumentPath == docName).FirstOrDefault();
//                if (!(found == null)) found.tipDok = "Potvrda prijema";    
            }
            else if (Regex.IsMatch(filePath, @".+.pdf.xml$", RegexOptions.IgnoreCase))
            {
                // xml-ove ne prikazujemo - mesto za akciju za xml fajl
//                Documents.Add(new OutboxDocumentModel(new FileInfo(filePath)));
//                var docName = filePath;
//                var found = Documents.Where(d => d.DocumentPath == docName).FirstOrDefault();
//                if (!(found == null)) found.tipDok = "Poruka za server (promena statusa dokumenta)";
            }
            else 
            {
                Documents.Add(new OutboxDocumentModel(new FileInfo(filePath)));
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

        public void Handle(MessageReject message)
        {
            if (!IsActive) return;
            if (Regex.IsMatch(FolderPath, @".*Inbound.*Outbox.*", RegexOptions.IgnoreCase)) return;
            PsKillPdfHandlers(); // workaround - pskill ubija sve procese koji rade nad PDF-ovima u eDokument
            RejectDocument();
        }

        //public IList<DocumentModel> GetDocumentsForSigning()
        //{
        //    var checkedDocuments = Documents.Where(d => d.IsChecked).Cast<OutboxDocumentModel>();
        //    var validDocuments = checkedDocuments.Where(d => d.IsValid.GetValueOrDefault()).Cast<DocumentModel>().ToList();
        //    return validDocuments;
        //}

        public override void Dispose()
        {
            Serialize();
        }

        private void Serialize()
        {
            var filePath = Path.Combine(FolderPath, "state.xml");
            var file = File.Create(filePath);
            List<OutboxDocumentModel> items = Documents.Cast<OutboxDocumentModel>().ToList();
            var xs = new XmlSerializer(typeof(List<OutboxDocumentModel>));
            using (Stream s = file)
                xs.Serialize(s, items);
        }

        private List<OutboxDocumentModel> Deserialize()
        {
            var oldList = new List<OutboxDocumentModel>();
            var xs = new XmlSerializer(typeof(List<OutboxDocumentModel>));
            var file = Path.Combine(FolderPath, "state.xml");
            if (!File.Exists(file)) return oldList;
            try
            {
                using (Stream s = File.OpenRead(file))
                    oldList = (List<OutboxDocumentModel>)xs.Deserialize(s);
            }
            catch
            {
                
            }
            return oldList;
        }

    public override void OnCheck(object e)
        {
            var ec = e as ActionExecutionContext;
            var cb = ec.Source as CheckBox;
            
            var view = ec.View as OutboxFolderView;
            var dg = view.DocumentsCV;
            var items = dg.SelectedItems;
            if (items.Count > 1)
            {
                foreach (var item in items)
                {
                    var doc = item as DocumentModel;
                    doc.IsChecked = cb.IsChecked.GetValueOrDefault();
                }
            }
            else
            {
                foreach (var item in DocumentsCV)
                {
                    var doc = item as DocumentModel;
                    doc.IsChecked = cb.IsChecked.GetValueOrDefault();
                }

            }
        }
    }
}
