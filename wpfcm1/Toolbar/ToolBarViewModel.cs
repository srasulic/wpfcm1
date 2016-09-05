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
            Buttons.Add(new ToolBarButtonViewModel(events, "appbar_arrow_down_up", "Pošalji i primi dokumente", new MessageSync()));
            Buttons.Add(new ToolBarButtonViewModel(events, "appbar_page_search", "Pregled dokumenta", new MessageTogglePreview()));
           // Buttons.Add(new ToolBarButtonViewModel(events, "appbar_list_gear", "Obrada - Izvlačenje podataka", new MessageExtractData()));
            //Buttons.Add(new ToolBarButtonViewModel(events, "appbar_list_gear", "Obrada - Validacija potpisa", new MessageValidate()));
            Buttons.Add(new ToolBarButtonViewModel(events, "appbar_list_check", "Obrada - Izvlačenje podataka", new MessageExtractData()));
            Buttons.Add(new ToolBarButtonViewModel(events, "appbar_list_check", "Obrada - Validacija potpisa", new MessageValidate()));
            Buttons.Add(new ToolBarButtonViewModel(events, "appbar_reply_email", "Potvrda prijema dokumenta (pošalji povratnicu)", new MessageAck()));
            Buttons.Add(new ToolBarButtonViewModel(events, "appbar_draw_pen_reflection", "Potpisivanje (potpiši čekirane dokumente iz liste)", new MessageSign()));
            Buttons.Add(new ToolBarButtonViewModel(events, "appbar_page_delete", "Odbacivanje dokumenta (izbaci iz obrade čekirane dokumente iz liste)", new MessageReject()));

            Buttons.Add(new ToolBarButtonViewModel(events, "appbar_page_check", "Obrađeno (označi kao obrađene sve čekirane dokumente iz liste)", new MessageProcessed()));
            Buttons.Add(new ToolBarButtonViewModel(events, "appbar_office_excel", "Izvezi listu dokumenata u Excel", new MessageXls()));

            _activeButtons = new ButtonVisibilityManager(this, events);
        }

        public BindableCollection<ToolBarButtonViewModel> Buttons { get; set; }
    }
}
