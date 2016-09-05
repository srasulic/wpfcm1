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
            {FolderManager.InvoicesInboundConfirmedToDoFolder, SignatureLocation.UpperRight},
            {FolderManager.IosOutboundErpIfaceFolder, SignatureLocation.UpperLeft},
            {FolderManager.IosInboundInboxFolder, SignatureLocation.UpperRight},
            {FolderManager.KpOutboundErpIfaceFolder, SignatureLocation.UpperLeft},
            {FolderManager.KpInboundInboxFolder, SignatureLocation.UpperRight},
            {FolderManager.KpInboundConfirmedToDoFolder, SignatureLocation.UpperRight},
            {FolderManager.OtherOutboundErpIfaceFolder, SignatureLocation.UpperLeft},
            {FolderManager.OtherInboundInboxFolder, SignatureLocation.UpperRight}

        };

        public static Dictionary<SignatureLocation, string> SignatureName = new Dictionary<SignatureLocation, string>()
        {
            {SignatureLocation.UpperLeft, "Potpis1"},
            {SignatureLocation.UpperRight, "Potpis2"}
        };
    }

    public static class SigningTransferRules
    {
        public static Dictionary<string, string> LocalMap = new Dictionary<string, string>()
        {
            {FolderManager.InvoicesOutboundErpIfaceFolder, FolderManager.InvoicesOutboundOutboxFolder},
            {FolderManager.InvoicesOutboundConfirmedOutToDoFolder, FolderManager.InvoicesOutboundOutboxFolder},
            {FolderManager.InvoicesInboundInboxFolder, FolderManager.InvoicesInboundOutboxFolder},
            {FolderManager.InvoicesInboundConfirmedToDoFolder, FolderManager.InvoicesInboundOutboxFolder},
            {FolderManager.IosOutboundErpIfaceFolder, FolderManager.IosOutboundOutboxFolder},
            {FolderManager.IosInboundInboxFolder, FolderManager.IosInboundOutboxFolder},
            {FolderManager.KpOutboundErpIfaceFolder, FolderManager.KpOutboundOutboxFolder},
            {FolderManager.KpOutboundConfirmedOutToDoFolder, FolderManager.KpOutboundOutboxFolder},
            {FolderManager.KpInboundInboxFolder, FolderManager.KpInboundOutboxFolder},
            {FolderManager.KpInboundConfirmedToDoFolder, FolderManager.KpInboundOutboxFolder},
            {FolderManager.OtherOutboundErpIfaceFolder, FolderManager.OtherOutboundOutboxFolder},
            {FolderManager.OtherInboundInboxFolder, FolderManager.OtherInboundOutboxFolder}
        };

        public static Dictionary<string, string> ProcessedMap= new Dictionary<string, string>()
        {
            {FolderManager.InvoicesOutboundErpIfaceFolder, FolderManager.InvoicesOutboundErpProcFolder},
            {FolderManager.IosOutboundErpIfaceFolder, FolderManager.IosOutboundErpProcFolder},
            {FolderManager.KpOutboundErpIfaceFolder, FolderManager.KpOutboundErpProcFolder},
            {FolderManager.OtherOutboundErpIfaceFolder, FolderManager.OtherOutboundErpProcFolder},
            {FolderManager.InvoicesOutboundOutboxFolder, FolderManager.InvoicesOutboundErpProcFolder},
            {FolderManager.IosOutboundOutboxFolder, FolderManager.IosOutboundErpProcFolder},
            {FolderManager.KpOutboundOutboxFolder, FolderManager.KpOutboundErpProcFolder},
            {FolderManager.OtherOutboundOutboxFolder, FolderManager.OtherOutboundErpProcFolder}
        };

        public enum FinalAction
        {
            Acknowledge,
            Store,
            SecondSignatureMark
        };

        public static Dictionary<string, FinalAction> OnFinished = new Dictionary<string, FinalAction>()
        {
            {FolderManager.InvoicesOutboundErpIfaceFolder, FinalAction.Store},
            {FolderManager.InvoicesInboundInboxFolder, FinalAction.SecondSignatureMark},
            {FolderManager.InvoicesInboundConfirmedToDoFolder, FinalAction.SecondSignatureMark},
            {FolderManager.IosOutboundErpIfaceFolder, FinalAction.Store},
            {FolderManager.IosInboundInboxFolder, FinalAction.SecondSignatureMark},
            {FolderManager.KpOutboundErpIfaceFolder, FinalAction.Store},
            {FolderManager.KpInboundInboxFolder, FinalAction.SecondSignatureMark},
            {FolderManager.KpInboundConfirmedToDoFolder, FinalAction.SecondSignatureMark},
            {FolderManager.OtherOutboundErpIfaceFolder, FinalAction.Store},
            {FolderManager.OtherInboundInboxFolder, FinalAction.SecondSignatureMark}
        };
    }

    public static class FtpTransferRules
    {
        public enum TransferAction { Upload, Download, Sync };

        public static Dictionary<string, string> FtpMap = new Dictionary<string, string>()
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
            {FolderManager.IosInboundConfirmedFolder, "edokument/ios/inbound/confirmed/"},
            {FolderManager.KpOutboundOutboxFolder, "edokument/kp/outbound/outbox/"},
            {FolderManager.KpOutboundPendFolder, "edokument/kp/outbound/pend/"},
            {FolderManager.KpOutboundConfirmedFolder, "edokument/kp/outbound/confirmed/"},
            {FolderManager.KpInboundInboxFolder, "edokument/kp/inbound/inbox/"},
            {FolderManager.KpInboundOutboxFolder, "edokument/kp/inbound/outbox/"},
            {FolderManager.KpInboundConfirmedFolder, "edokument/kp/inbound/confirmed/"},
            {FolderManager.OtherOutboundOutboxFolder, "edokument/ostali/outbound/outbox/"},
            {FolderManager.OtherOutboundPendFolder, "edokument/ostali/outbound/pend/"},
            {FolderManager.OtherOutboundConfirmedFolder, "edokument/ostali/outbound/confirmed/"},
            {FolderManager.OtherInboundInboxFolder, "edokument/ostali/inbound/inbox/"},
            {FolderManager.OtherInboundOutboxFolder, "edokument/ostali/inbound/outbox/"},
            {FolderManager.OtherInboundConfirmedFolder, "edokument/ostali/inbound/confirmed/"}
        };

        public static Dictionary<string, TransferAction> Action = new Dictionary<string, TransferAction>()
        {
            {FolderManager.InvoicesOutboundOutboxFolder, TransferAction.Upload},
            {FolderManager.InvoicesOutboundPendFolder, TransferAction.Sync},
            {FolderManager.InvoicesOutboundConfirmedFolder, TransferAction.Download},
            {FolderManager.InvoicesInboundInboxFolder, TransferAction.Sync},
            {FolderManager.InvoicesInboundOutboxFolder, TransferAction.Upload},
            {FolderManager.InvoicesInboundConfirmedFolder, TransferAction.Download},
            {FolderManager.IosOutboundOutboxFolder, TransferAction.Upload},
            {FolderManager.IosOutboundPendFolder, TransferAction.Sync},
            {FolderManager.IosOutboundConfirmedFolder, TransferAction.Download},
            {FolderManager.IosInboundInboxFolder, TransferAction.Sync},
            {FolderManager.IosInboundOutboxFolder, TransferAction.Upload},
            {FolderManager.IosInboundConfirmedFolder, TransferAction.Download},
            {FolderManager.KpOutboundOutboxFolder, TransferAction.Upload},
            {FolderManager.KpOutboundPendFolder, TransferAction.Sync},
            {FolderManager.KpOutboundConfirmedFolder, TransferAction.Download},
            {FolderManager.KpInboundInboxFolder, TransferAction.Sync},
            {FolderManager.KpInboundOutboxFolder, TransferAction.Upload},
            {FolderManager.KpInboundConfirmedFolder, TransferAction.Download},
            {FolderManager.OtherOutboundOutboxFolder, TransferAction.Upload},
            {FolderManager.OtherOutboundPendFolder, TransferAction.Sync},
            {FolderManager.OtherOutboundConfirmedFolder, TransferAction.Download},
            {FolderManager.OtherInboundInboxFolder, TransferAction.Sync},
            {FolderManager.OtherInboundOutboxFolder, TransferAction.Upload},
            {FolderManager.OtherInboundConfirmedFolder, TransferAction.Download}
        };

        public static Dictionary<string, string> LocalMap = new Dictionary<string, string>()
        {
            {FolderManager.InvoicesOutboundOutboxFolder, FolderManager.InvoicesOutboundSentFolder},
            {FolderManager.InvoicesInboundOutboxFolder, FolderManager.InvoicesInboundSentFolder},
            {FolderManager.IosOutboundOutboxFolder, FolderManager.IosOutboundSentFolder},
            {FolderManager.IosInboundOutboxFolder, FolderManager.IosInboundSentFolder},
            {FolderManager.KpOutboundOutboxFolder, FolderManager.KpOutboundSentFolder},
            {FolderManager.KpInboundOutboxFolder, FolderManager.KpInboundSentFolder},
            {FolderManager.OtherOutboundOutboxFolder, FolderManager.OtherOutboundSentFolder},
            {FolderManager.OtherInboundOutboxFolder, FolderManager.OtherInboundSentFolder}
        };
    }
}
