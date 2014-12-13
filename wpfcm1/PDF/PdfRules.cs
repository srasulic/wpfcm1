using System.Collections.Generic;
using wpfcm1.DataAccess;

namespace wpfcm1.PDF
{
    public static class SignatureRules
    {
        public enum SignatureLocation { UpperLeft, UpperRight };

        public static Dictionary<string, SignatureLocation> Map = new Dictionary<string, SignatureLocation>()
        {
            {FolderManager.InvoicesOutboundErpIfaceFolder, SignatureLocation.UpperLeft},
            {FolderManager.InvoicesInboundInboxFolder, SignatureLocation.UpperRight}
        };

        public static Dictionary<SignatureLocation, string> SignatureName = new Dictionary<SignatureLocation, string>()
        {
            {SignatureLocation.UpperLeft, "Potpis1"},
            {SignatureLocation.UpperRight, "Potpis2"}
        };
    }

    public static class SignedTransferRules
    {
        public static Dictionary<string, string> Map = new Dictionary<string, string>()
        {
            {FolderManager.InvoicesOutboundErpIfaceFolder, FolderManager.InvoicesOutboundOutboxFolder},
            {FolderManager.InvoicesInboundInboxFolder, FolderManager.InvoicesInboundOutboxFolder}
        };
    }

    public static class ProcessedTransferRules
    {
        public static Dictionary<string, string> Map = new Dictionary<string, string>()
        {
            {FolderManager.InvoicesOutboundErpIfaceFolder, FolderManager.InvoicesOutboundErpProcFolder}
        };
    }

    public static class FtpSucessTransferRules
    {
        public static Dictionary<string, string> Map = new Dictionary<string, string>()
        {
            {FolderManager.InvoicesOutboundOutboxFolder, FolderManager.InvoicesOutboundSentFolder},
            {FolderManager.InvoicesInboundOutboxFolder, FolderManager.InvoicesInboundSentFolder}
        };
    }

    public static class FtpTransferRules
    {
        public enum TransferAction { Upload, Sync };

        public static Dictionary<string, string> Map = new Dictionary<string, string>()
        {
            {FolderManager.InvoicesOutboundOutboxFolder, "edokument/faktura/outbound/outbox/"},
            {FolderManager.InvoicesOutboundPendFolder, "edokument/faktura/outbound/pend/"},
            {FolderManager.InvoicesOutboundConfirmedFolder, "edokument/faktura/outbound/confirmed/"},
            {FolderManager.InvoicesInboundInboxFolder, "edokument/faktura/inbound/inbox/"},
            {FolderManager.InvoicesInboundOutboxFolder, "edokument/faktura/inbound/outbox/"},
            {FolderManager.InvoicesInboundConfirmedFolder, "edokument/faktura/inbound/confirmed/"}
        };

        public static Dictionary<string, TransferAction> Action = new Dictionary<string, TransferAction>()
        {
            {FolderManager.InvoicesOutboundOutboxFolder, TransferAction.Upload},
            {FolderManager.InvoicesOutboundPendFolder, TransferAction.Sync},
            {FolderManager.InvoicesOutboundConfirmedFolder, TransferAction.Sync},
            {FolderManager.InvoicesInboundInboxFolder, TransferAction.Sync},
            {FolderManager.InvoicesInboundOutboxFolder, TransferAction.Upload},
            {FolderManager.InvoicesInboundConfirmedFolder, TransferAction.Sync}
        };
    }
}
