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
            Buttons = new BindableCollection<ToolBarButtonViewModel>
            {
                //FIRST 8 BUTTONS: bits = {0.. 7}
                new ToolBarButtonViewModel(events, "appbar_home_empty", "Početni ekran", new MessageShowHome()),
                new ToolBarButtonViewModel(events, "appbar_arrow_down_up", "Sinhronizacija - pošalji i primi dokumente", new MessageSync()),
                new ToolBarButtonViewModel(events, "appbar_page_search", "Pregled dokumenta", new MessageTogglePreview()),
                new ToolBarButtonViewModel(events, "appbar_list_check", "Obrada - Izvlačenje podataka", new MessageExtractData()),
                new ToolBarButtonViewModel(events, "appbar_list_check", "Obrada - Validacija potpisa", new MessageValidate()),
                new ToolBarButtonViewModel(events, "appbar_reply_email", "Potvrda prijema dokumenta (pošalji povratnicu)", new MessageAck()),
                new ToolBarButtonViewModel(events, "appbar_draw_pen_reflection", "Potpisivanje (potpiši obeležene dokumente iz liste)", new MessageSign()),
                new ToolBarButtonViewModel(events, "appbar_page_delete", "Odbacivanje dokumenta (izbaci iz obrade obeležene dokumente iz liste)", new MessageReject()),

                //SECOND 8 BUTTONS: bits = {0.. 7}
                new ToolBarButtonViewModel(events, "appbar_office_excel", "Izvezi listu dokumenata u Excel", new MessageXls()),
                new ToolBarButtonViewModel(events, "appbar_page_check", "Izaberi sertifikat", new MessagePickCert())
            };

            _activeButtons = new ButtonVisibilityManager(this, events);
        }

        public BindableCollection<ToolBarButtonViewModel> Buttons { get; set; }
    }
}
