using System.Windows;
using Caliburn.Micro;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using wpfcm1.DataAccess;
using wpfcm1.Events;
using wpfcm1.FolderTypes;
using wpfcm1.Preview;

namespace wpfcm1.FolderGroups
{
    public class FolderGroupViewModel : Conductor<IScreen>.Collection.OneActive, IDisposable, IHandle<MessageShowPdf>
    {
        private readonly IEventAggregator _events;

        public FolderGroupViewModel(Dictionary<string,string> wsFolders, string name, IEventAggregator events, IWindowManager windowManager)
        {
            DisplayName = name;
            _events = events;
            _events.Subscribe(this);
            Preview = IoC.Get<PreviewViewModel>();

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

        public PreviewViewModel Preview { get; set; }
        public IObservableCollection<FolderViewModel> FolderVMs { get; private set; }

        public void ActivateTabItem(SelectionChangedEventArgs e)
        {
            var item = e.AddedItems[0] as FolderViewModel;
            if (item != null) ActivateItem(item);
        }

        protected override void OnActivate()
        {
            ActivateItem(ActiveItem ?? FolderVMs[0]);
        }

        protected override void OnDeactivate(bool close)
        {
            DeactivateItem(ActiveItem, false);
        }

        public void Dispose()
        {
            foreach (var folder in FolderVMs.Where(folder => folder is InboxFolderViewModel || folder is GeneratedFolderViewModel))
            {
                folder.Dispose();
            }
        }

        public void Handle(MessageShowPdf message)
        {
            var v = GetView();
            if (v == null) return;
            var pdfBrowser = (v as FolderGroupView).FindChild<WebBrowser>("PdfBrowser");
            if (pdfBrowser != null && pdfBrowser.Visibility == Visibility.Visible)
            {
                pdfBrowser.Navigate(message.Uri);
            }
        }
    }
}
