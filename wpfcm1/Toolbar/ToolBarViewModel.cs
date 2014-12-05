using Caliburn.Micro;
using System.ComponentModel.Composition;
using wpfcm1.Events;

namespace wpfcm1.Toolbar
{
    [Export(typeof(ToolBarViewModel))]
    public class ToolBarViewModel : PropertyChangedBase
    {
        private ButtonVisibilityManager _activeButtons;

        [ImportingConstructor]
        public ToolBarViewModel(IEventAggregator events)
        {
            Buttons = new BindableCollection<ToolBarButtonViewModel>();
            Buttons.Add(new ToolBarButtonViewModel(events, "appbar_home_empty", new MessageShowHome()));
            Buttons.Add(new ToolBarButtonViewModel(events, "appbar_refresh", new MessageSync()));
            Buttons.Add(new ToolBarButtonViewModel(events, "appbar_page_search", new MessagePreview()));
            Buttons.Add(new ToolBarButtonViewModel(events, "appbar_page_edit", new MessageSign()));
            Buttons.Add(new ToolBarButtonViewModel(events, "appbar_page_upload", new MessageExtractData()));
            Buttons.Add(new ToolBarButtonViewModel(events, "appbar_page_delete", new MessageReject()));
            Buttons.Add(new ToolBarButtonViewModel(events, "appbar_page_check", new MessageValidate()));
            Buttons.Add(new ToolBarButtonViewModel(events, "appbar_page_arrow", new MessageAck()));

            _activeButtons = new ButtonVisibilityManager(this, events);
        }

        public BindableCollection<ToolBarButtonViewModel> Buttons { get; set; }
    }
}
