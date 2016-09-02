using Caliburn.Micro;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using wpfcm1.Events;
using System;

namespace wpfcm1.Toolbar
{
    public class ButtonVisibilityManager : IHandle<MessageViewModelActivated>
    {
        private readonly ToolBarViewModel _toolBar;


        private readonly Dictionary<string, BitArray> WorkspaceToButtonVisibility = new Dictionary<string, BitArray>()
        {
            {"HomeViewModel",                new BitArray(new byte[]{Convert.ToByte("00000011", 2), 0x0})},
            {"GeneratedFolderViewModel",     new BitArray(new byte[]{Convert.ToByte("11001111", 2), 0x1})},
            {"InboxFolderViewModel",         new BitArray(new byte[]{Convert.ToByte("01110111", 2), 0x1})},
            {"PendFolderViewModel",          new BitArray(new byte[]{Convert.ToByte("00000111", 2), 0x1})},
            {"OutboxFolderViewModel",        new BitArray(new byte[]{Convert.ToByte("10000111", 2), 0x1})},
            {"ConfirmedFolderViewModel",     new BitArray(new byte[]{Convert.ToByte("00000111", 2), 0x1})},
            {"ConfirmedToDoFolderViewModel", new BitArray(new byte[]{Convert.ToByte("11010111", 2), 0x1})},
            {"FolderViewModel",              new BitArray(new byte[]{Convert.ToByte("00000111", 2), 0x0})},
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
