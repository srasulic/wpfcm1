using Caliburn.Micro;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using wpfcm1.Events;

namespace wpfcm1.Preview
{
    //vazno je da ova klasa bude singleton, zbog stanja visibility flega
    [Export(typeof(PreviewViewModel))]
    public class PreviewViewModel : PropertyChangedBase, IHandle<MessageTogglePreview>, IHandle<MessageShowPdf>
    {
        private readonly IEventAggregator _events;
        public static readonly string Empty = "about:blank";

        [ImportingConstructor]
        public PreviewViewModel(IEventAggregator events)
        {
            _events = events;
            _events.SubscribeOnUIThread(this);
            CurrentDocument = Empty;
            CurrentDocument1 = Empty;
            iZatvaranje = 0;
        }

        private static bool _previewVisibility;
        public bool PreviewVisibility
        {
            get { return _previewVisibility; }
            set { _previewVisibility = value; NotifyOfPropertyChange(() => PreviewVisibility); }
        }

        public string CurrentDocument { get; private set; }

        // ovaj property sluzi za cuvanje informacije o prethodno otvorenom dokumentu 
        public string CurrentDocument1 { get; set; }
        // ovaj property sluzi za flegovanje poziva Handle metode 
        public int iZatvaranje { get; set; }

        public void Handle(MessageTogglePreview message)
        {
            PreviewVisibility = !PreviewVisibility;
            // u slucaju prvog otvaranja novog dokumenta, inicijalno postavljamo fleg na 0 
            if (CurrentDocument1 != CurrentDocument)
            {
                iZatvaranje = 0;
            }
            // cuvamo info o dokumentu, kojeg otvaramo 
            CurrentDocument1 = CurrentDocument; 
            if (PreviewVisibility)
            {
                // preview dokumenta je vidljiv za korisnika 
                _events.PublishOnUIThreadAsync(new MessageShowPdf(CurrentDocument));
            }
            else
            {
                // preview dokumenta nije vidljiv za korisnika 
                // ovde koristimo dva rekurzivna poziva iste ove metode, kako bismo simulirali otvaranje i odmah zatim, zatvaranje dummy dokumenta
                // ovo nam je potrebno da bismo oslobodili prethodni dokument za dalju obradu 
                if (iZatvaranje < 2)
                {
                    // string dummypdf = @"C:\eDokument\dummy.pdf";
                    string dummypdf = Settings.Folders.Default.RootFolder+@"\dummy.pdf";
                    
                    if (!File.Exists(dummypdf))
                    {
                        File.Create(dummypdf).Dispose();
                    }
                    _events.PublishOnUIThreadAsync(new MessageShowPdf(dummypdf));
                    iZatvaranje = 1;
                    // prvi Handle otvara dummy.pdf
                    Handle(message);
                    iZatvaranje = 2;
                    // drugi Handle zatvara dummy.pdf
                    Handle(message);
                    iZatvaranje = 0;
                }
            }
        }

        public void Handle(MessageShowPdf message)
        {
            //var uri = message.Uri == Empty ? Empty : string.Format("{0}#toolbar=0&navpanes=0", message.Uri);
            CurrentDocument = message.Uri;
            
        }

        public Task HandleAsync(MessageTogglePreview message, CancellationToken cancellationToken)
        {
            Handle(message);
            return Task.CompletedTask;
        }

        public Task HandleAsync(MessageShowPdf message, CancellationToken cancellationToken)
        {
            Handle(message);
            return Task.CompletedTask;
        }
    }
}
