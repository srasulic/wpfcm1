using Caliburn.Micro;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
            _events.SubscribeOnUIThread(this);
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
                    case "OutboxDocumentModel":
                        FolderVMs.Add(new OutboxFolderViewModel(wsFolder.Value, FolderManager.FolderNameMap[wsFolder.Key], _events, windowManager));
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
            if (e.AddedItems.Count > 0)
            {
                var item = e.AddedItems[0] as FolderViewModel;
                if (item != null) ActivateItemAsync(item);
            }
        }

        protected override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            //TODO: ShowOutbound poziva ovu funkciju, ali ne moze da nasetuje IsActive za FolderVMs[0]
            await ActivateItemAsync(ActiveItem ?? FolderVMs[0]);
            await _events.PublishOnUIThreadAsync(new MessageViewModelActivated(ActiveItem.GetType().Name));
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            return DeactivateItemAsync (ActiveItem, false);
        }

        public void Dispose()
        {
            foreach (var folder in FolderVMs)
            {
                folder.Dispose();
            }
        }

        public void Handle(MessageShowPdf message)
        {
            var v = GetView();
            if (v == null) return;
            var pdfBrowser = (v as FolderGroupView).FindChild<WebBrowser>("PdfBrowser");
            if (pdfBrowser != null && pdfBrowser.IsVisible)
            {
                //TODO: srediti ovaj haos sa webbrowserom
                //browser zabaguje ako mu se proslediisti dokument
                var oldUri = pdfBrowser.Source.AbsolutePath;
                var newUri = new Uri(message.Uri).AbsolutePath;
                if (String.Compare(oldUri, newUri, StringComparison.OrdinalIgnoreCase) == 0) return;

                pdfBrowser.Navigate(new System.Uri(message.Uri));
            }
        }

        public Task HandleAsync(MessageShowPdf message, CancellationToken cancellationToken)
        {
            Handle(message);
            return Task.CompletedTask;
        }
    }
}
