using System.Collections.Generic;
using System.IO;
using System.Linq;
using wpfcm1.Settings;

namespace wpfcm1.DataAccess
{
    public class FolderManager
    {
        static readonly string OutboundErpIfaceFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.InvoicesSubfolder, Folders.Default.OutboundErpIfaceFolder);
        static readonly string OutboundErpProcFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.InvoicesSubfolder, Folders.Default.OutboundErpProcFolder);
        static readonly string OutboundOutboxFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.InvoicesSubfolder, Folders.Default.OutboundOutboxFolder);
        static readonly string OutboundSentFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.InvoicesSubfolder, Folders.Default.OutboundSentFolder);
        static readonly string OutboundPendFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.InvoicesSubfolder, Folders.Default.OutboundPendFolder);
        static readonly string OutboundConfirmedFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.InvoicesSubfolder, Folders.Default.OutboundConfirmedFolder);
        static readonly string InboundInboxFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.InvoicesSubfolder, Folders.Default.InboundInboxFolder);
        static readonly string InboundOutboxFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.InvoicesSubfolder, Folders.Default.InboundOutboxFolder);
        static readonly string InboundSentFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.InvoicesSubfolder, Folders.Default.InboundSentFolder);
        static readonly string InboundConfirmedFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.InvoicesSubfolder, Folders.Default.InboundConfirmedFolder);

        public static Dictionary<string, string> InvoicesOutboundFolders { get; private set; } 
        public static Dictionary<string, string> InvoicesInboundFolders { get; private set; }

        public static Dictionary<string, string> FolderNameMap = new Dictionary<string, string>
        {
            {"OutboundErpIfaceFolder", "Generated"},
            {"OutboundErpProcFolder", "Processed"},
            {"OutboundOutboxFolder", "Outbox"},
            {"OutboundSentFolder", "Sent"},
            {"OutboundPendFolder", "Pend"},
            {"OutboundConfirmedFolder", "Confirmed"},
            {"InboundInboxFolder", "Inbox"},
            {"InboundOutboxFolder", "Outbox"},
            {"InboundSentFolder", "Sent"},
            {"InboundConfirmedFolder", "Confirmed"}
        };

        static FolderManager()
        {
            InvoicesOutboundFolders = new Dictionary<string, string>
            {
                {"OutboundErpIfaceFolder", OutboundErpIfaceFolder},
                {"OutboundErpProcFolder", OutboundErpProcFolder},
                {"OutboundOutboxFolder", OutboundOutboxFolder},
                {"OutboundSentFolder", OutboundSentFolder},
                {"OutboundPendFolder", OutboundPendFolder},
                {"OutboundConfirmedFolder", OutboundConfirmedFolder}
            };
            CheckFolders(InvoicesOutboundFolders);
            InvoicesInboundFolders = new Dictionary<string, string>
            {
                {"InboundInboxFolder", InboundInboxFolder},
                {"InboundOutboxFolder", InboundOutboxFolder},
                {"InboundSentFolder", InboundSentFolder},
                {"InboundConfirmedFolder", InboundConfirmedFolder}
            };
            CheckFolders(InvoicesInboundFolders);

        }

        private static void CheckFolders(Dictionary<string,string> folders)
        {
            foreach (var folder in folders.Where(folder => !Directory.Exists(folder.Value)))
            {
                Directory.CreateDirectory(folder.Value);
            }
        }
    }
}
