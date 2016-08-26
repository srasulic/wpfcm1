using Caliburn.Micro;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using wpfcm1.Events;

namespace wpfcm1.Toolbar
{
    public class ButtonVisibilityManager : IHandle<MessageViewModelActivated>
    {
        private readonly ToolBarViewModel _toolBar;

        private readonly Dictionary<string, BitArray> WorkspaceToButtonVisibility = new Dictionary<string, BitArray>()
        {
            {"HomeViewModel", new BitArray(new byte[]{0x3, 0x0})},
            {"GeneratedFolderViewModel", new BitArray(new byte[]{0xCF, 0x1})},
            {"InboxFolderViewModel", new BitArray(new byte[]{0x77, 0x1})},
            {"PendFolderViewModel", new BitArray(new byte[]{0x7, 0x1})},
            {"OutboxFolderViewModel", new BitArray(new byte[]{0x87, 0x1})},
            {"ConfirmedFolderViewModel", new BitArray(new byte[]{0x07, 0x1})},
            {"ConfirmedToDoFolderViewModel", new BitArray(new byte[]{0xC7, 0x1})},
            {"FolderViewModel", new BitArray(new byte[]{0x7, 0x0})},
        }; 

        public ButtonVisibilityManager(ToolBarViewModel toolbar, IEventAggregator events)
        {
            _toolBar = toolbar;
            events.Subscribe(this);
        }

        public void Handle(MessageViewModelActivated message)
        {
            Debug.Assert(WorkspaceToButtonVisibility.ContainsKey(message.Name));
            SetButtonsvisibility(WorkspaceToButtonVisibility[message.Name]);
        }

        private void SetButtonsvisibility(BitArray flags)
        {
            for (var i = 0; i < _toolBar.Buttons.Count; i++)
                _toolBar.Buttons.ElementAt(i).ButtonVisibility = flags.Get(i);
        }
    }
}
