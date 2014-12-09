using Caliburn.Micro;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using wpfcm1.DataAccess;
using wpfcm1.Events;
using wpfcm1.FolderTypes;
using wpfcm1.Preview;

namespace wpfcm1.FolderGroups
{
    public class FolderGroupViewModel : Conductor<IScreen>.Collection.OneActive, IDisposable, IHandle<PdfPreviewMessage>
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

        public void ActivateTabItem(int idx)
        {
            ActivateItem(FolderVMs[idx]);
        }

        protected override void OnActivate()
        {
            //_events.PublishOnUIThread(new ViewModelActivatedMessage(GetType().Name));
            if (ActiveItem == null)
                ActivateTabItem(0);
            else
                ActivateItem(ActiveItem);
        }

        protected override void OnDeactivate(bool close)
        {
            DeactivateItem(ActiveItem, false);
        }

        public void Dispose()
        {
            foreach (var folder in FolderVMs)
            {
                if (folder is InboxFolderViewModel || folder is GeneratedFolderViewModel)
                    folder.Dispose();
            }
        }

        public void Handle(PdfPreviewMessage message)
        {
            var v = GetView();
            if (v != null)
            {
                var myView = v as FolderGroupView;
                var pdfBrowser = myView.FindChild<WebBrowser>("PdfBrowser");
                if (pdfBrowser.IsVisible) pdfBrowser.Navigate(message.DocumentPath);
            }
        }
    }
}
