using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using wpfcm1.Events;

namespace wpfcm1.Toolbar
{
    public class ButtonVisibilityManager : IHandle<MessageViewModelActivated>
    {
        private readonly ToolBarViewModel _toolBar;

        private readonly Dictionary<string, BitArray> WorkspaceToButtonVisibility = new Dictionary<string, BitArray>()
        {
            // ( Reject - Sign - Ack - Validate - ExtractData - TogglePreview - Sync - ShowHome ) 
            // ( PickCert - Xls )
            {"HomeViewModel",               new BitArray(new byte[]{0b00000011, 0b00000010})},
            {"GeneratedFolderViewModel",    new BitArray(new byte[]{0b11001111, 0b00000001})},
            {"InboxFolderViewModel",        new BitArray(new byte[]{0b01010111, 0b00000001})},
            {"OutboxFolderViewModel",       new BitArray(new byte[]{0b10000111, 0x00000001})},
            {"FolderViewModel",             new BitArray(new byte[]{0b00000111, 0b00000000})}
        };

        public ButtonVisibilityManager(ToolBarViewModel toolbar, IEventAggregator events)
        {
            _toolBar = toolbar;
            events.SubscribeOnUIThread(this);
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
