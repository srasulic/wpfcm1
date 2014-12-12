using Caliburn.Micro;
using System.ComponentModel.Composition;
using wpfcm1.Events;

namespace wpfcm1.Preview
{
    [Export(typeof(PreviewViewModel))]
    public class PreviewViewModel : PropertyChangedBase, IHandle<MessageTogglePreview>, IHandle<MessageShowPdf>
    {
        private readonly IEventAggregator _events;
        public static readonly string Empty = "about:blank";

        [ImportingConstructor]
        public PreviewViewModel(IEventAggregator events)
        {
            _events = events;
            _events.Subscribe(this);
            CurrentDocument = Empty;
        }

        private static bool _previewVisibility;
        public bool PreviewVisibility
        {
            get { return _previewVisibility; }
            set { _previewVisibility = value; NotifyOfPropertyChange(() => PreviewVisibility); }
        }

        public string CurrentDocument { get; private set; }

        public void Handle(MessageTogglePreview message)
        {
            PreviewVisibility = !PreviewVisibility;
            if (PreviewVisibility)
            {
                _events.PublishOnUIThread(new MessageShowPdf(CurrentDocument));
            }
        }

        public void Handle(MessageShowPdf message)
        {
            var uri = message.Uri == Empty ? Empty : string.Format("{0}#toolbar=0&navpanes=0", message.Uri);
            CurrentDocument = uri;
        }
    }
}
