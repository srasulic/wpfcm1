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
            Buttons.Add(new ToolBarButtonViewModel(events, "appbar_home_empty", "Početni ekran", new MessageShowHome()));
            Buttons.Add(new ToolBarButtonViewModel(events, "appbar_refresh", "Sinhronizacija", new MessageSync()));
            Buttons.Add(new ToolBarButtonViewModel(events, "appbar_page_search", "Pregled", new MessagePreview()));
            Buttons.Add(new ToolBarButtonViewModel(events, "appbar_page_edit", "Potpisivanje", new MessageSign()));
            Buttons.Add(new ToolBarButtonViewModel(events, "appbar_page_upload", "Izvlačenje podataka", new MessageExtractData()));
            Buttons.Add(new ToolBarButtonViewModel(events, "appbar_page_delete", "Odbacivanje", new MessageReject()));
            Buttons.Add(new ToolBarButtonViewModel(events, "appbar_page_check", "Validacija potpisa", new MessageValidate()));
            Buttons.Add(new ToolBarButtonViewModel(events, "appbar_page_arrow", "Potvrda", new MessageAck()));

            _activeButtons = new ButtonVisibilityManager(this, events);
        }

        public BindableCollection<ToolBarButtonViewModel> Buttons { get; set; }
    }
}
