using Caliburn.Micro;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using wpfcm1.Events;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace wpfcm1.Toolbar
{
    public class ButtonVisibilityManager : IHandle<MessageViewModelActivated>
    {
        private readonly ToolBarViewModel _toolBar;

        private readonly Dictionary<string, BitArray> WorkspaceToButtonVisibility = new Dictionary<string, BitArray>()
        {
            // ( Reject - Sign - Ack - Validate - ExtractData - TogglePreview - Sync - ShowHome ) 
            // ( - - - PickCert - ArchiveNBGP - Web Inf - Xls - Arch )
            {"HomeViewModel",               new BitArray(new byte[]{Convert.ToByte("00000011", 2), Convert.ToByte("00010000", 2)})},
            {"GeneratedFolderViewModel",    new BitArray(new byte[]{Convert.ToByte("11001111", 2), 0x2})},
            {"InboxFolderViewModel",        new BitArray(new byte[]{Convert.ToByte("01010111", 2), 0x2})},
            {"OutboxFolderViewModel",       new BitArray(new byte[]{Convert.ToByte("10000111", 2), 0x2})},
            {"FolderViewModel",             new BitArray(new byte[]{Convert.ToByte("00000111", 2), Convert.ToByte("00001000", 2)})} //ArchiveNBGP
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

        public Task HandleAsync(MessageViewModelActivated message, CancellationToken cancellationToken)
        {
            Handle(message);
            return Task.CompletedTask;
        }

        private void SetButtonsvisibility(BitArray flags)
        {
            for (var i = 0; i < _toolBar.Buttons.Count; i++)
            {
                _toolBar.Buttons.ElementAt(i).ButtonVisibility = flags.Get(i);
            }
        }
    }
}
