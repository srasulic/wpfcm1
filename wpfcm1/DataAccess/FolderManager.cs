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

        //folderi za fakture
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
        //folderi za IOS
        public static readonly string IosOutboundErpIfaceFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.IosSubfolder, Folders.Default.OutboundErpIfaceFolder);
        public static readonly string IosOutboundErpProcFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.IosSubfolder, Folders.Default.OutboundErpProcFolder);
        public static readonly string IosOutboundOutboxFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.IosSubfolder, Folders.Default.OutboundOutboxFolder);
        public static readonly string IosOutboundSentFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.IosSubfolder, Folders.Default.OutboundSentFolder);
        public static readonly string IosOutboundPendFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.IosSubfolder, Folders.Default.OutboundPendFolder);
        public static readonly string IosOutboundConfirmedFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.IosSubfolder, Folders.Default.OutboundConfirmedFolder);
        public static readonly string IosInboundInboxFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.IosSubfolder, Folders.Default.InboundInboxFolder);
        public static readonly string IosInboundOutboxFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.IosSubfolder, Folders.Default.InboundOutboxFolder);
        public static readonly string IosInboundSentFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.IosSubfolder, Folders.Default.InboundSentFolder);
        public static readonly string IosInboundConfirmedFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.IosSubfolder, Folders.Default.InboundConfirmedFolder);
        //folderi za KP
        public static readonly string KpOutboundErpIfaceFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.KpSubfolder, Folders.Default.OutboundErpIfaceFolder);
        public static readonly string KpOutboundErpProcFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.KpSubfolder, Folders.Default.OutboundErpProcFolder);
        public static readonly string KpOutboundOutboxFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.KpSubfolder, Folders.Default.OutboundOutboxFolder);
        public static readonly string KpOutboundSentFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.KpSubfolder, Folders.Default.OutboundSentFolder);
        public static readonly string KpOutboundPendFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.KpSubfolder, Folders.Default.OutboundPendFolder);
        public static readonly string KpOutboundConfirmedFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.KpSubfolder, Folders.Default.OutboundConfirmedFolder);
        public static readonly string KpInboundInboxFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.KpSubfolder, Folders.Default.InboundInboxFolder);
        public static readonly string KpInboundOutboxFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.KpSubfolder, Folders.Default.InboundOutboxFolder);
        public static readonly string KpInboundSentFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.KpSubfolder, Folders.Default.InboundSentFolder);
        public static readonly string KpInboundConfirmedFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.KpSubfolder, Folders.Default.InboundConfirmedFolder);
        //folderi za ostale ulazne/izlazne
        public static readonly string OtherOutboundErpIfaceFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.OtherSubfolder, Folders.Default.OutboundErpIfaceFolder);
        public static readonly string OtherOutboundErpProcFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.OtherSubfolder, Folders.Default.OutboundErpProcFolder);
        public static readonly string OtherOutboundOutboxFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.OtherSubfolder, Folders.Default.OutboundOutboxFolder);
        public static readonly string OtherOutboundSentFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.OtherSubfolder, Folders.Default.OutboundSentFolder);
        public static readonly string OtherOutboundPendFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.OtherSubfolder, Folders.Default.OutboundPendFolder);
        public static readonly string OtherOutboundConfirmedFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.OtherSubfolder, Folders.Default.OutboundConfirmedFolder);
        public static readonly string OtherInboundInboxFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.OtherSubfolder, Folders.Default.InboundInboxFolder);
        public static readonly string OtherInboundOutboxFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.OtherSubfolder, Folders.Default.InboundOutboxFolder);
        public static readonly string OtherInboundSentFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.OtherSubfolder, Folders.Default.InboundSentFolder);
        public static readonly string OtherInboundConfirmedFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.OtherSubfolder, Folders.Default.InboundConfirmedFolder);

        //grupe foldera
        public static Dictionary<string, string> InvoicesOutboundFolders { get; set; }
        public static Dictionary<string, string> InvoicesInboundFolders { get; set; }
        public static Dictionary<string, string> IosOutboundFolders { get; set; }
        public static Dictionary<string, string> IosInboundFolders { get; set; }
        public static Dictionary<string, string> KpOutboundFolders { get; set; }
        public static Dictionary<string, string> KpInboundFolders { get; set; }
        public static Dictionary<string, string> OtherOutboundFolders { get; set; }
        public static Dictionary<string, string> OtherInboundFolders { get; set; }

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
            {"InvoicesInboundConfirmedFolder", "Confirmed"},
            {"IosOutboundErpIfaceFolder", "Generated"},
            {"IosOutboundErpProcFolder", "Processed"},
            {"IosOutboundOutboxFolder", "Outbox"},
            {"IosOutboundSentFolder", "Sent"},
            {"IosOutboundPendFolder", "Pend"},
            {"IosOutboundConfirmedFolder", "Confirmed"},
            {"IosInboundInboxFolder", "Inbox"},
            {"IosInboundOutboxFolder", "Outbox"},
            {"IosInboundSentFolder", "Sent"},
            {"IosInboundConfirmedFolder", "Confirmed"},
            {"KpOutboundErpIfaceFolder", "Generated"},
            {"KpOutboundErpProcFolder", "Processed"},
            {"KpOutboundOutboxFolder", "Outbox"},
            {"KpOutboundSentFolder", "Sent"},
            {"KpOutboundPendFolder", "Pend"},
            {"KpOutboundConfirmedFolder", "Confirmed"},
            {"KpInboundInboxFolder", "Inbox"},
            {"KpInboundOutboxFolder", "Outbox"},
            {"KpInboundSentFolder", "Sent"},
            {"KpInboundConfirmedFolder", "Confirmed"},
            {"OtherOutboundErpIfaceFolder", "Generated"},
            {"OtherOutboundErpProcFolder", "Processed"},
            {"OtherOutboundOutboxFolder", "Outbox"},
            {"OtherOutboundSentFolder", "Sent"},
            {"OtherOutboundPendFolder", "Pend"},
            {"OtherOutboundConfirmedFolder", "Confirmed"},
            {"OtherInboundInboxFolder", "Inbox"},
            {"OtherInboundOutboxFolder", "Outbox"},
            {"OtherInboundSentFolder", "Sent"},
            {"OtherInboundConfirmedFolder", "Confirmed"}
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
            {"InvoicesInboundConfirmedFolder", typeof(DocumentModel)},
            {"IosOutboundErpIfaceFolder", typeof(GeneratedDocumentModel)},
            {"IosOutboundOutboxFolder", typeof(DocumentModel)},
            {"IosOutboundSentFolder", typeof(DocumentModel)},
            {"IosOutboundPendFolder", typeof(DocumentModel)},
            {"IosOutboundConfirmedFolder", typeof(DocumentModel)},
            {"IosInboundInboxFolder", typeof(InboxDocumentModel)},
            {"IosInboundOutboxFolder", typeof(DocumentModel)},
            {"IosInboundSentFolder", typeof(DocumentModel)},
            {"IosInboundConfirmedFolder", typeof(DocumentModel)},
            {"KpOutboundErpIfaceFolder", typeof(GeneratedDocumentModel)},
            {"KpOutboundOutboxFolder", typeof(DocumentModel)},
            {"KpOutboundSentFolder", typeof(DocumentModel)},
            {"KpOutboundPendFolder", typeof(DocumentModel)},
            {"KpOutboundConfirmedFolder", typeof(DocumentModel)},
            {"KpInboundInboxFolder", typeof(InboxDocumentModel)},
            {"KpInboundOutboxFolder", typeof(DocumentModel)},
            {"KpInboundSentFolder", typeof(DocumentModel)},
            {"KpInboundConfirmedFolder", typeof(DocumentModel)},
            {"OtherOutboundErpIfaceFolder", typeof(GeneratedDocumentModel)},
            {"OtherOutboundOutboxFolder", typeof(DocumentModel)},
            {"OtherOutboundSentFolder", typeof(DocumentModel)},
            {"OtherOutboundPendFolder", typeof(DocumentModel)},
            {"OtherOutboundConfirmedFolder", typeof(DocumentModel)},
            {"OtherInboundInboxFolder", typeof(InboxDocumentModel)},
            {"OtherInboundOutboxFolder", typeof(DocumentModel)},
            {"OtherInboundSentFolder", typeof(DocumentModel)},
            {"OtherInboundConfirmedFolder", typeof(DocumentModel)}
        };

        static FolderManager()
        {
            InvoicesOutboundFolders = new Dictionary<string, string>
            {
                {"InvoicesOutboundErpIfaceFolder", InvoicesOutboundErpIfaceFolder},
                {"InvoicesOutboundOutboxFolder", InvoicesOutboundOutboxFolder},
                {"InvoicesOutboundSentFolder", InvoicesOutboundSentFolder},
                {"InvoicesOutboundPendFolder", InvoicesOutboundPendFolder},
                {"InvoicesOutboundConfirmedFolder", InvoicesOutboundConfirmedFolder}
            };
            CheckFolders(InvoicesOutboundFolders);
            if (!Directory.Exists(InvoicesOutboundErpProcFolder)) Directory.CreateDirectory(InvoicesOutboundErpProcFolder);

            InvoicesInboundFolders = new Dictionary<string, string>
            {
                {"InvoicesInboundInboxFolder", InvoicesInboundInboxFolder},
                {"InvoicesInboundOutboxFolder", InvoicesInboundOutboxFolder},
                {"InvoicesInboundSentFolder", InvoicesInboundSentFolder},
                {"InvoicesInboundConfirmedFolder", InvoicesInboundConfirmedFolder}
            };
            CheckFolders(InvoicesInboundFolders);

            IosOutboundFolders = new Dictionary<string, string>
            {
                {"IosOutboundErpIfaceFolder", IosOutboundErpIfaceFolder},
                {"IosOutboundOutboxFolder", IosOutboundOutboxFolder},
                {"IosOutboundSentFolder", IosOutboundSentFolder},
                {"IosOutboundPendFolder", IosOutboundPendFolder},
                {"IosOutboundConfirmedFolder", IosOutboundConfirmedFolder}
            };
            CheckFolders(IosOutboundFolders);
            if (!Directory.Exists(IosOutboundErpProcFolder)) Directory.CreateDirectory(IosOutboundErpProcFolder);

            IosInboundFolders = new Dictionary<string, string>
            {
                {"IosInboundInboxFolder", IosInboundInboxFolder},
                {"IosInboundOutboxFolder", IosInboundOutboxFolder},
                {"IosInboundSentFolder", IosInboundSentFolder},
                {"IosInboundConfirmedFolder", IosInboundConfirmedFolder}
            };
            CheckFolders(IosInboundFolders);

            KpOutboundFolders = new Dictionary<string, string>
            {
                {"KpOutboundErpIfaceFolder", KpOutboundErpIfaceFolder},
                {"KpOutboundOutboxFolder", KpOutboundOutboxFolder},
                {"KpOutboundSentFolder", KpOutboundSentFolder},
                {"KpOutboundPendFolder", KpOutboundPendFolder},
                {"KpOutboundConfirmedFolder", KpOutboundConfirmedFolder}
            };
            CheckFolders(KpOutboundFolders);
            if (!Directory.Exists(KpOutboundErpProcFolder)) Directory.CreateDirectory(KpOutboundErpProcFolder);

            KpInboundFolders = new Dictionary<string, string>
            {
                {"KpInboundInboxFolder", KpInboundInboxFolder},
                {"KpInboundOutboxFolder", KpInboundOutboxFolder},
                {"KpInboundSentFolder", KpInboundSentFolder},
                {"KpInboundConfirmedFolder", KpInboundConfirmedFolder}
            };
            CheckFolders(KpInboundFolders);

            OtherOutboundFolders = new Dictionary<string, string>
            {
                {"OtherOutboundErpIfaceFolder", OtherOutboundErpIfaceFolder},
                {"OtherOutboundOutboxFolder", OtherOutboundOutboxFolder},
                {"OtherOutboundSentFolder", OtherOutboundSentFolder},
                {"OtherOutboundPendFolder", OtherOutboundPendFolder},
                {"OtherOutboundConfirmedFolder", OtherOutboundConfirmedFolder}
            };
            CheckFolders(OtherOutboundFolders);
            if (!Directory.Exists(OtherOutboundErpProcFolder)) Directory.CreateDirectory(OtherOutboundErpProcFolder);

            OtherInboundFolders = new Dictionary<string, string>
            {
                {"OtherInboundInboxFolder", OtherInboundInboxFolder},
                {"OtherInboundOutboxFolder", OtherInboundOutboxFolder},
                {"OtherInboundSentFolder", OtherInboundSentFolder},
                {"OtherInboundConfirmedFolder", OtherInboundConfirmedFolder}
            };
            CheckFolders(OtherInboundFolders);
        }

        private static void CheckFolders(Dictionary<string,string> folders)
        {
            foreach (var folder in folders.Where(folder => !Directory.Exists(folder.Value)))
            {
                Log.Info(String.Format("Creating {0}", folder.Value));
                Directory.CreateDirectory(folder.Value);
            }
        }
    }
}
