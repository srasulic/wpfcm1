using Caliburn.Micro;
using System.Windows;
using System.Windows.Controls;

namespace wpfcm1.Toolbar
{
    public class ToolBarButtonViewModel : PropertyChangedBase
    {
        private readonly IEventAggregator _events;

        public ToolBarButtonViewModel(IEventAggregator events, string resName)
        {
            _events = events;
            var res = Application.Current.Resources[resName];
            ButtonImg = res as Canvas;
        }

        private Canvas _buttonImg;
        public Canvas ButtonImg
        {
            get { return _buttonImg; }
            set { _buttonImg = value; NotifyOfPropertyChange(() => ButtonImg); }
        }
    }
}
