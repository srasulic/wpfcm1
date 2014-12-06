using System.Collections.Generic;
using wpfcm1.DataAccess;

namespace wpfcm1.PDF
{
    public static class SignatureRules
    {
        public enum SignatureLocation { UpperLeft, UpperRight };

        public static Dictionary<string, SignatureLocation> Map = new Dictionary<string, SignatureLocation>()
        {
            {FolderManager.OutboundErpIfaceFolder, SignatureLocation.UpperLeft},
            {FolderManager.InboundInboxFolder, SignatureLocation.UpperRight}
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
            {FolderManager.OutboundErpIfaceFolder, FolderManager.OutboundOutboxFolder},
            {FolderManager.InboundInboxFolder, FolderManager.InboundOutboxFolder}
        };
    }

    public static class ProcessedTransferRules
    {
        public static Dictionary<string, string> Map = new Dictionary<string, string>()
        {
            {FolderManager.OutboundErpIfaceFolder, FolderManager.OutboundErpProcFolder}
        };
    }

    public static class FtpSucessTransferRules
    {
        public static Dictionary<string, string> TransferMap = new Dictionary<string, string>()
        {
            {FolderManager.OutboundOutboxFolder, FolderManager.OutboundSentFolder},
            {FolderManager.InboundOutboxFolder, FolderManager.InboundSentFolder}
        };
    }

    public static class FtpSyncRules
    {
        public static Dictionary<string, string> TransferMap = new Dictionary<string, string>()
        {
            {FolderManager.OutboundOutboxFolder, "edokument/faktura/outbound/outbox/"},
            {FolderManager.OutboundPendFolder, "edokument/faktura/outbound/pend/"},
            {FolderManager.OutboundConfirmedFolder, "edokument/faktura/outbound/confirmed/"},
            {FolderManager.InboundInboxFolder, "edokument/faktura/inbound/inbox/"},
            {FolderManager.InboundOutboxFolder, "edokument/faktura/inbound/outbox/"},
            {FolderManager.InboundConfirmedFolder, "edokument/faktura/inbound/confirmed/"}
        };
    }
}
