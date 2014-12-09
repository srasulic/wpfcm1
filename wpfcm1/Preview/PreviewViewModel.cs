using Caliburn.Micro;
using System;
using System.ComponentModel.Composition;
using wpfcm1.Events;

namespace wpfcm1.Preview
{
    [Export(typeof(PreviewViewModel))]
    public class PreviewViewModel : PropertyChangedBase, IHandle<MessagePreview>
    {
        private IEventAggregator _events;

        [ImportingConstructor]
        public PreviewViewModel(IEventAggregator events)
        {
            _events = events;
            _events.Subscribe(this);
        }

        private static bool _previewVisibility;
        private Uri _documentUri;

        public bool PreviewVisibility
        {
            get { return _previewVisibility; }
            set { _previewVisibility = value; NotifyOfPropertyChange(() => PreviewVisibility); }
        }

        public Uri DocumentUri
        {
            get { return _documentUri; }
            set { _documentUri = value; NotifyOfPropertyChange(() => DocumentUri);}
        }

        public void Handle(MessagePreview message)
        {
            PreviewVisibility = !PreviewVisibility;
            if (!PreviewVisibility)
                _events.PublishOnUIThread(new PdfPreviewMessage("about:blank")); 
        }
    }
}
