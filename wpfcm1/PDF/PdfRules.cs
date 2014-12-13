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
            {FolderManager.InvoicesInboundInboxFolder, SignatureLocation.UpperRight},
            {FolderManager.IosOutboundErpIfaceFolder, SignatureLocation.UpperLeft},
            {FolderManager.IosInboundInboxFolder, SignatureLocation.UpperRight}
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
            {FolderManager.InvoicesInboundInboxFolder, FolderManager.InvoicesInboundOutboxFolder},
            {FolderManager.IosOutboundErpIfaceFolder, FolderManager.IosOutboundOutboxFolder},
            {FolderManager.IosInboundInboxFolder, FolderManager.IosInboundOutboxFolder}
        };
    }

    public static class ProcessedTransferRules
    {
        public static Dictionary<string, string> Map = new Dictionary<string, string>()
        {
            {FolderManager.InvoicesOutboundErpIfaceFolder, FolderManager.InvoicesOutboundErpProcFolder},
            {FolderManager.IosOutboundErpIfaceFolder, FolderManager.IosOutboundErpProcFolder}
        };
    }

    public static class FtpSucessTransferRules
    {
        public static Dictionary<string, string> Map = new Dictionary<string, string>()
        {
            {FolderManager.InvoicesOutboundOutboxFolder, FolderManager.InvoicesOutboundSentFolder},
            {FolderManager.InvoicesInboundOutboxFolder, FolderManager.InvoicesInboundSentFolder},
            {FolderManager.IosOutboundOutboxFolder, FolderManager.IosOutboundSentFolder},
            {FolderManager.IosInboundOutboxFolder, FolderManager.IosInboundSentFolder}
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
            {FolderManager.InvoicesInboundConfirmedFolder, "edokument/faktura/inbound/confirmed/"},
            {FolderManager.IosOutboundOutboxFolder, "edokument/ios/outbound/outbox/"},
            {FolderManager.IosOutboundPendFolder, "edokument/ios/outbound/pend/"},
            {FolderManager.IosOutboundConfirmedFolder, "edokument/ios/outbound/confirmed/"},
            {FolderManager.IosInboundInboxFolder, "edokument/ios/inbound/inbox/"},
            {FolderManager.IosInboundOutboxFolder, "edokument/ios/inbound/outbox/"},
            {FolderManager.IosInboundConfirmedFolder, "edokument/ios/inbound/confirmed/"}
        };

        public static Dictionary<string, TransferAction> Action = new Dictionary<string, TransferAction>()
        {
            {FolderManager.InvoicesOutboundOutboxFolder, TransferAction.Upload},
            {FolderManager.InvoicesOutboundPendFolder, TransferAction.Sync},
            {FolderManager.InvoicesOutboundConfirmedFolder, TransferAction.Sync},
            {FolderManager.InvoicesInboundInboxFolder, TransferAction.Sync},
            {FolderManager.InvoicesInboundOutboxFolder, TransferAction.Upload},
            {FolderManager.InvoicesInboundConfirmedFolder, TransferAction.Sync},
            {FolderManager.IosOutboundOutboxFolder, TransferAction.Upload},
            {FolderManager.IosOutboundPendFolder, TransferAction.Sync},
            {FolderManager.IosOutboundConfirmedFolder, TransferAction.Sync},
            {FolderManager.IosInboundInboxFolder, TransferAction.Sync},
            {FolderManager.IosInboundOutboxFolder, TransferAction.Upload},
            {FolderManager.IosInboundConfirmedFolder, TransferAction.Sync}
        };
    }
}
