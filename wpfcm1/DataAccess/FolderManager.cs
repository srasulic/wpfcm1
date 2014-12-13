using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using wpfcm1.Model;
using wpfcm1.Settings;

namespace wpfcm1.DataAccess
{
    public class FolderManager
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static readonly string InvoicesOutboundErpIfaceFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.InvoicesSubfolder, Folders.Default.OutboundErpIfaceFolder);
        public static readonly string InvoicesOutboundErpProcFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.InvoicesSubfolder, Folders.Default.OutboundErpProcFolder);
        public static readonly string InvoicesOutboundOutboxFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.InvoicesSubfolder, Folders.Default.OutboundOutboxFolder);
        public static readonly string InvoicesOutboundSentFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.InvoicesSubfolder, Folders.Default.OutboundSentFolder);
        public static readonly string InvoicesOutboundPendFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.InvoicesSubfolder, Folders.Default.OutboundPendFolder);
        public static readonly string InvoicesOutboundConfirmedFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.InvoicesSubfolder, Folders.Default.OutboundConfirmedFolder);
        public static readonly string InvoicesInboundInboxFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.InvoicesSubfolder, Folders.Default.InboundInboxFolder);
        public static readonly string InvoicesInboundOutboxFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.InvoicesSubfolder, Folders.Default.InboundOutboxFolder);
        public static readonly string InvoicesInboundSentFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.InvoicesSubfolder, Folders.Default.InboundSentFolder);
        public static readonly string InvoicesInboundConfirmedFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.InvoicesSubfolder, Folders.Default.InboundConfirmedFolder);

        public static Dictionary<string, string> InvoicesOutboundFolders { get; private set; } 
        public static Dictionary<string, string> InvoicesInboundFolders { get; private set; }

        public static Dictionary<string, string> FolderNameMap = new Dictionary<string, string>
        {
            {"InvoicesOutboundErpIfaceFolder", "Generated"},
            {"InvoicesOutboundErpProcFolder", "Processed"},
            {"InvoicesOutboundOutboxFolder", "Outbox"},
            {"InvoicesOutboundSentFolder", "Sent"},
            {"InvoicesOutboundPendFolder", "Pend"},
            {"InvoicesOutboundConfirmedFolder", "Confirmed"},
            {"InvoicesInboundInboxFolder", "Inbox"},
            {"InvoicesInboundOutboxFolder", "Outbox"},
            {"InvoicesInboundSentFolder", "Sent"},
            {"InvoicesInboundConfirmedFolder", "Confirmed"}
        };

        public static readonly Dictionary<string, Type> FolderTypeMap = new Dictionary<string, Type>()
        {
            {"InvoicesOutboundErpIfaceFolder", typeof(GeneratedDocumentModel)},
            {"InvoicesOutboundOutboxFolder", typeof(DocumentModel)},
            {"InvoicesOutboundSentFolder", typeof(DocumentModel)},
            {"InvoicesOutboundPendFolder", typeof(DocumentModel)},
            {"InvoicesOutboundConfirmedFolder", typeof(DocumentModel)},
            {"InvoicesInboundInboxFolder", typeof(InboxDocumentModel)},
            {"InvoicesInboundOutboxFolder", typeof(DocumentModel)},
            {"InvoicesInboundSentFolder", typeof(DocumentModel)},
            {"InvoicesInboundConfirmedFolder", typeof(DocumentModel)}
        };

        static FolderManager()
        {
            InvoicesOutboundFolders = new Dictionary<string, string>
            {
                {"InvoicesOutboundErpIfaceFolder", InvoicesOutboundErpIfaceFolder},
                //{"OutboundErpProcFolder", OutboundErpProcFolder},
                {"InvoicesOutboundOutboxFolder", InvoicesOutboundOutboxFolder},
                {"InvoicesOutboundSentFolder", InvoicesOutboundSentFolder},
                {"InvoicesOutboundPendFolder", InvoicesOutboundPendFolder},
                {"InvoicesOutboundConfirmedFolder", InvoicesOutboundConfirmedFolder}
            };
            CheckFolders(InvoicesOutboundFolders);
            InvoicesInboundFolders = new Dictionary<string, string>
            {
                {"InvoicesInboundInboxFolder", InvoicesInboundInboxFolder},
                {"InvoicesInboundOutboxFolder", InvoicesInboundOutboxFolder},
                {"InvoicesInboundSentFolder", InvoicesInboundSentFolder},
                {"InvoicesInboundConfirmedFolder", InvoicesInboundConfirmedFolder}
            };
            CheckFolders(InvoicesInboundFolders);

            if (!Directory.Exists(InvoicesOutboundErpProcFolder)) Directory.CreateDirectory(InvoicesOutboundErpProcFolder);
        }

        private static void CheckFolders(Dictionary<string,string> folders)
        {
            foreach (var folder in folders.Where(folder => !Directory.Exists(folder.Value)))
            {
                Log.Info(string.Format("Creating {0}", folder.Value));
                Directory.CreateDirectory(folder.Value);
            }
        }
    }
}
