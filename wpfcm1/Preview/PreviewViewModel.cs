using Caliburn.Micro;
using System.ComponentModel.Composition;
using wpfcm1.Events;

namespace wpfcm1.Preview
{
    [Export(typeof(PreviewViewModel))]
    public class PreviewViewModel : PropertyChangedBase, IHandle<MessagePreview>
    {

        [ImportingConstructor]
        public PreviewViewModel(IEventAggregator events)
        {
            events.Subscribe(this);
        }

        private static bool _previewVisibility;
        public bool PreviewVisibility
        {
            get { return _previewVisibility; }
            set { _previewVisibility = value; NotifyOfPropertyChange(() => PreviewVisibility); }
        }

        //private double _previewWidth;
        //public double PreviewWidth
        //{
        //    get { return _previewWidth; }
        //    set { _previewWidth = value; NotifyOfPropertyChange(() => PreviewWidth); }
        //}

        public void Handle(MessagePreview message)
        {
            PreviewVisibility = !PreviewVisibility;
            //if (PreviewVisibility)
            //{
            //    PreviewGridLength = new GridLength(40, GridUnitType.Star);
            //}
            //else
            //{
            //    PreviewGridLength = GridLength.Auto;
            //}
        }
    }
}
