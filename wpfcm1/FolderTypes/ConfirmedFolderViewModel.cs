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

namespace wpfcm1.FolderTypes
{
    public class ConfirmedFolderViewModel : FolderViewModel, IHandle<MessageXls>
    {
        private readonly IWindowManager _windowManager;
        private CertificateModel _certificate;
        

        public ConfirmedFolderViewModel(string path, string name, IEventAggregator events, IWindowManager winMgr) : base(path, name, events)
        {
            _windowManager = winMgr;
        }

        protected override void InitDocuments()
        {
            Documents = new BindableCollection<DocumentModel>(
                Directory.EnumerateFiles(FolderPath)
                .Where(f => Extensions.Contains(Path.GetExtension(f)))
                .Select(f => new ConfirmedDocumentModel(new FileInfo(f))));

            InitWatcher(FolderPath);

            if (Documents.Count == 0) return;
            foreach (var document in Documents)
            {
                if (!Regex.IsMatch(document.DocumentPath, @".+_s.pdf$", RegexOptions.IgnoreCase))
                {
                    continue;
                }
                if (Regex.IsMatch(document.DocumentPath, @".+_s_s.pdf$", RegexOptions.IgnoreCase))
                {
                    document.HasSecondSigniture = true;
                    document.IsSignedAgain = false;
                    document.Processed = true;
                    continue;
                }
                var found = Documents.FirstOrDefault(d => d.DocumentPath == Regex.Replace(document.DocumentPath, @"_s.pdf", @"_s_s.pdf", RegexOptions.IgnoreCase));
                if ( found == null ) {
                    document.HasSecondSigniture = false;
                    document.IsSignedAgain = false;
                    document.Processed = false;
                } else {
                    document.HasSecondSigniture = false;
                    document.IsSignedAgain = true;
                    document.Processed = true;
                }
            }

            for (int i = Documents.Count - 1; i >= 0; i-- )
            {
                if (Documents[i].IsSignedAgain) Documents.RemoveAt(i);
            }

            //var states = Deserialize();
            //foreach (var state in states)
            //{
            //    var found = Documents.FirstOrDefault(d => d.DocumentPath == state.DocumentPath);
            //    if (found == null) continue;
            //    var old = found as DocumentModel;
            //    old.Processed = state.Processed;
            //    old.IsSignedAgain = state.IsSignedAgain;
            //}
        }
    

        protected override void AddFile(string filePath)
        {
            Documents.Add(new DocumentModel(new FileInfo(filePath)));
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
            var dg = v.FindName("Documents") as DataGrid;
            dg.CommitEdit(DataGridEditingUnit.Row, true);
        }

        public void Handle(CertificateModel message)
        {
            _certificate = message;
        }

        public void Handle(MessageXls message)
        {
            if (!IsActive) return;
            XlsExport();

        }

        //public override void Dispose()
        //{
        //    Serialize();
        //}

        //private void Serialize()
        //{
        //    var filePath = Path.Combine(FolderPath, "state.xml");
        //    var file = File.Create(filePath);
        //    List<ConfirmedDocumentModel> items = Documents.Cast<ConfirmedDocumentModel>().ToList();
        //    var xs = new XmlSerializer(typeof(List<ConfirmedDocumentModel>));
        //    using (Stream s = file)
        //        xs.Serialize(s, items);
        //}

        //private List<ConfirmedDocumentModel> Deserialize()
        //{
        //    var oldList = new List<ConfirmedDocumentModel>();
        //    var xs = new XmlSerializer(typeof(List<ConfirmedDocumentModel>));
        //    var file = Path.Combine(FolderPath, "state.xml");
        //    if (!File.Exists(file)) return oldList;
        //    try
        //    {
        //        using (Stream s = File.OpenRead(file))
        //            oldList = (List<ConfirmedDocumentModel>)xs.Deserialize(s);
        //    }
        //    catch
        //    {

        //    }
        //    return oldList;
        //}

        //public override void OnCheck(object e)
        //{
        //    var ec = e as ActionExecutionContext;
        //    var cb = ec.Source as CheckBox;

        //    var view = ec.View as ConfirmedFolderView;
        //    var dg = view.Documents;
        //    var items = dg.SelectedItems;
        //    foreach (var item in items)
        //    {
        //        var doc = item as DocumentModel;
        //        doc.IsChecked = cb.IsChecked.GetValueOrDefault();
        //    }
        //}
    }
}
