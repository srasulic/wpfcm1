using Caliburn.Micro;
using System.Windows;
using System.Windows.Controls;
using wpfcm1.Events;

namespace wpfcm1.Toolbar
{
    public class ToolBarButtonViewModel : PropertyChangedBase
    {
        private readonly IEventAggregator _events;
        private IButtonMessage _msg;

        public ToolBarButtonViewModel(IEventAggregator events, string resName, IButtonMessage msg=null)
        {
            _events = events;
            _msg = msg;
            var res = Application.Current.Resources[resName];
            ButtonImg = res as Canvas;
        }

        private Canvas _buttonImg;
        public Canvas ButtonImg
        {
            get { return _buttonImg; }
            set { _buttonImg = value; NotifyOfPropertyChange(() => ButtonImg); }
        }

        private bool _buttonVisibility;
        public bool ButtonVisibility
        {
            get { return _buttonVisibility; }
            set { _buttonVisibility = value; NotifyOfPropertyChange(() => ButtonVisibility); }
        }

        public void SendMessage()
        {
            if (_msg != null)
                _events.PublishOnUIThread(_msg);
        }
    }
}
