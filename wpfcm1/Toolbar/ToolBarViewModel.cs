using Caliburn.Micro;
using System.ComponentModel.Composition;

namespace wpfcm1.Toolbar
{
    [Export(typeof(ToolBarViewModel))]
    public class ToolBarViewModel : PropertyChangedBase
    {
        [ImportingConstructor]
        public ToolBarViewModel(IEventAggregator events)
        {
            Buttons = new BindableCollection<ToolBarButtonViewModel>();
            Buttons.Add(new ToolBarButtonViewModel(events, "appbar_home_empty"));
            Buttons.Add(new ToolBarButtonViewModel(events, "appbar_refresh"));
            Buttons.Add(new ToolBarButtonViewModel(events, "appbar_page_search"));
            Buttons.Add(new ToolBarButtonViewModel(events, "appbar_page_edit"));
            Buttons.Add(new ToolBarButtonViewModel(events, "appbar_page_upload"));
            Buttons.Add(new ToolBarButtonViewModel(events, "appbar_page_delete"));
            Buttons.Add(new ToolBarButtonViewModel(events, "appbar_page_check"));
            Buttons.Add(new ToolBarButtonViewModel(events, "appbar_page_arrow"));
        }

        public BindableCollection<ToolBarButtonViewModel> Buttons { get; set; }
    }
}
