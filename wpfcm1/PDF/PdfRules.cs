using System.Collections.Generic;
using wpfcm1.DataAccess;
using wpfcm1.Settings;

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
            {FolderManager.IosInboundConfirmedToDoFolder, SignatureLocation.UpperRight},

            {FolderManager.OtpadOutboundErpIfaceFolder, SignatureLocation.UpperLeft},
            {FolderManager.OtpadInboundInboxFolder, SignatureLocation.UpperRight},
            {FolderManager.OtpadInboundConfirmedToDoFolder, SignatureLocation.UpperRight},

            {FolderManager.KpOutboundErpIfaceFolder, SignatureLocation.UpperLeft},
            {FolderManager.KpInboundInboxFolder, SignatureLocation.UpperRight},
            {FolderManager.KpInboundConfirmedToDoFolder, SignatureLocation.UpperRight},
            {FolderManager.PovratiOutboundErpIfaceFolder, SignatureLocation.UpperLeft},
            {FolderManager.PovratiInboundInboxFolder, SignatureLocation.UpperRight},
            {FolderManager.PovratiInboundConfirmedToDoFolder, SignatureLocation.UpperRight},
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
            {FolderManager.InvoicesOutboundErpIfaceFolder,          FolderManager.InvoicesOutboundOutboxFolder},
            //{FolderManager.InvoicesOutboundConfirmedOutToDoFolder,  FolderManager.InvoicesOutboundOutboxFolder},
            {FolderManager.InvoicesOutboundConfirmedFolder,         FolderManager.InvoicesOutboundOutboxFolder},
            {FolderManager.InvoicesInboundInboxFolder,              FolderManager.InvoicesInboundOutboxFolder},
            //{FolderManager.InvoicesInboundConfirmedToDoFolder,      FolderManager.InvoicesInboundOutboxFolder},
            {FolderManager.InvoicesInboundConfirmedFolder,          FolderManager.InvoicesInboundOutboxFolder},

            {FolderManager.IosOutboundErpIfaceFolder, FolderManager.IosOutboundOutboxFolder},
            {FolderManager.IosOutboundConfirmedFolder, FolderManager.IosOutboundOutboxFolder},
            {FolderManager.IosInboundInboxFolder,      FolderManager.IosInboundOutboxFolder},
            {FolderManager.IosInboundConfirmedFolder,  FolderManager.IosInboundOutboxFolder},

            {FolderManager.OtpadOutboundErpIfaceFolder, FolderManager.OtpadOutboundOutboxFolder},
            {FolderManager.OtpadOutboundConfirmedFolder, FolderManager.OtpadOutboundOutboxFolder},
            {FolderManager.OtpadInboundInboxFolder,      FolderManager.OtpadInboundOutboxFolder},
            {FolderManager.OtpadInboundConfirmedFolder,  FolderManager.OtpadInboundOutboxFolder},

            {FolderManager.KpOutboundErpIfaceFolder,            FolderManager.KpOutboundOutboxFolder},
            //{FolderManager.KpOutboundConfirmedOutToDoFolder,    FolderManager.KpOutboundOutboxFolder},
            {FolderManager.KpOutboundConfirmedFolder,           FolderManager.KpOutboundOutboxFolder},
            {FolderManager.KpInboundInboxFolder,                FolderManager.KpInboundOutboxFolder},
            //{FolderManager.KpInboundConfirmedToDoFolder,        FolderManager.KpInboundOutboxFolder},
            {FolderManager.KpInboundConfirmedFolder,            FolderManager.KpInboundOutboxFolder},

            {FolderManager.PovratiOutboundErpIfaceFolder,            FolderManager.PovratiOutboundOutboxFolder},
            {FolderManager.PovratiOutboundConfirmedFolder,           FolderManager.PovratiOutboundOutboxFolder},
            {FolderManager.PovratiInboundInboxFolder,                FolderManager.PovratiInboundOutboxFolder},
            {FolderManager.PovratiInboundConfirmedFolder,            FolderManager.PovratiInboundOutboxFolder},

            {FolderManager.OtherOutboundErpIfaceFolder,   FolderManager.OtherOutboundOutboxFolder},
            {FolderManager.OtherOutboundConfirmedFolder,  FolderManager.OtherOutboundOutboxFolder},
            {FolderManager.OtherInboundInboxFolder,       FolderManager.OtherInboundOutboxFolder},
            {FolderManager.OtherInboundConfirmedFolder,   FolderManager.OtherInboundOutboxFolder}
        };

        public static Dictionary<string, string> ProcessedMap= new Dictionary<string, string>()
        {
            {FolderManager.InvoicesOutboundErpIfaceFolder, FolderManager.InvoicesOutboundErpProcFolder},
            {FolderManager.IosOutboundErpIfaceFolder, FolderManager.IosOutboundErpProcFolder},
            {FolderManager.OtpadOutboundErpIfaceFolder, FolderManager.OtpadOutboundErpProcFolder},
            {FolderManager.KpOutboundErpIfaceFolder, FolderManager.KpOutboundErpProcFolder},
            {FolderManager.PovratiOutboundErpIfaceFolder, FolderManager.PovratiOutboundErpProcFolder},
            {FolderManager.OtherOutboundErpIfaceFolder, FolderManager.OtherOutboundErpProcFolder},
            
            {FolderManager.InvoicesOutboundOutboxFolder, FolderManager.InvoicesOutboundErpProcFolder},
            {FolderManager.IosOutboundOutboxFolder, FolderManager.IosOutboundErpProcFolder},
            {FolderManager.OtpadOutboundOutboxFolder, FolderManager.OtpadOutboundErpProcFolder},
            {FolderManager.KpOutboundOutboxFolder, FolderManager.KpOutboundErpProcFolder},
            {FolderManager.PovratiOutboundOutboxFolder, FolderManager.PovratiOutboundErpProcFolder},
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
            {FolderManager.IosInboundConfirmedToDoFolder, FinalAction.SecondSignatureMark},

            {FolderManager.OtpadOutboundErpIfaceFolder, FinalAction.Store},
            {FolderManager.OtpadInboundInboxFolder, FinalAction.SecondSignatureMark},
            {FolderManager.OtpadInboundConfirmedToDoFolder, FinalAction.SecondSignatureMark},

            {FolderManager.KpOutboundErpIfaceFolder, FinalAction.Store},
            {FolderManager.KpInboundInboxFolder, FinalAction.SecondSignatureMark},
            {FolderManager.KpInboundConfirmedToDoFolder, FinalAction.SecondSignatureMark},
            {FolderManager.PovratiOutboundErpIfaceFolder, FinalAction.Store},
            {FolderManager.PovratiInboundInboxFolder, FinalAction.SecondSignatureMark},
            {FolderManager.PovratiInboundConfirmedToDoFolder, FinalAction.SecondSignatureMark},
            {FolderManager.OtherOutboundErpIfaceFolder, FinalAction.Store},
            {FolderManager.OtherInboundInboxFolder, FinalAction.SecondSignatureMark}
        };
    }

    public static class FtpTransferRules
    {
        public enum TransferAction { Upload, Download, Sync, Exclude };

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

            {FolderManager.OtpadOutboundOutboxFolder, "edokument/otpad/outbound/outbox/"},
            {FolderManager.OtpadOutboundPendFolder, "edokument/otpad/outbound/pend/"},
            {FolderManager.OtpadOutboundConfirmedFolder, "edokument/otpad/outbound/confirmed/"},
            {FolderManager.OtpadInboundInboxFolder, "edokument/otpad/inbound/inbox/"},
            {FolderManager.OtpadInboundOutboxFolder, "edokument/otpad/inbound/outbox/"},
            {FolderManager.OtpadInboundConfirmedFolder, "edokument/otpad/inbound/confirmed/"},

            {FolderManager.KpOutboundOutboxFolder, "edokument/kp/outbound/outbox/"},
            {FolderManager.KpOutboundPendFolder, "edokument/kp/outbound/pend/"},
            {FolderManager.KpOutboundConfirmedFolder, "edokument/kp/outbound/confirmed/"},
            {FolderManager.KpInboundInboxFolder, "edokument/kp/inbound/inbox/"},
            {FolderManager.KpInboundOutboxFolder, "edokument/kp/inbound/outbox/"},
            {FolderManager.KpInboundConfirmedFolder, "edokument/kp/inbound/confirmed/"},
            {FolderManager.PovratiOutboundOutboxFolder, "edokument/povrati/outbound/outbox/"},
            {FolderManager.PovratiOutboundPendFolder, "edokument/povrati/outbound/pend/"},
            {FolderManager.PovratiOutboundConfirmedFolder, "edokument/povrati/outbound/confirmed/"},
            {FolderManager.PovratiInboundInboxFolder, "edokument/povrati/inbound/inbox/"},
            {FolderManager.PovratiInboundOutboxFolder, "edokument/povrati/inbound/outbox/"},
            {FolderManager.PovratiInboundConfirmedFolder, "edokument/povrati/inbound/confirmed/"},
            {FolderManager.OtherOutboundOutboxFolder, "edokument/ostali/outbound/outbox/"},
            {FolderManager.OtherOutboundPendFolder, "edokument/ostali/outbound/pend/"},
            {FolderManager.OtherOutboundConfirmedFolder, "edokument/ostali/outbound/confirmed/"},
            {FolderManager.OtherInboundInboxFolder, "edokument/ostali/inbound/inbox/"},
            {FolderManager.OtherInboundOutboxFolder, "edokument/ostali/inbound/outbox/"},
            {FolderManager.OtherInboundConfirmedFolder, "edokument/ostali/inbound/confirmed/"}
        };

        public static Dictionary<string, TransferAction> Action = new Dictionary<string, TransferAction>()
        {
            {FolderManager.InvoicesOutboundOutboxFolder, (User.Default.InvoicesOutbound ? TransferAction.Upload : TransferAction.Exclude)},
            {FolderManager.InvoicesOutboundPendFolder, (User.Default.InvoicesOutbound ? TransferAction.Sync : TransferAction.Exclude)},
            {FolderManager.InvoicesOutboundConfirmedFolder, (User.Default.InvoicesOutbound ? TransferAction.Download : TransferAction.Exclude)},
            {FolderManager.InvoicesInboundInboxFolder, (User.Default.InvoicesInbound ? TransferAction.Sync : TransferAction.Exclude)},
            {FolderManager.InvoicesInboundOutboxFolder, (User.Default.InvoicesInbound ? TransferAction.Upload : TransferAction.Exclude)},
            {FolderManager.InvoicesInboundConfirmedFolder, (User.Default.InvoicesInbound? TransferAction.Download : TransferAction.Exclude)},

            {FolderManager.IosOutboundOutboxFolder, (User.Default.IosOutbound ? TransferAction.Upload : TransferAction.Exclude)},
            {FolderManager.IosOutboundPendFolder, (User.Default.IosOutbound ? TransferAction.Sync : TransferAction.Exclude)},
            {FolderManager.IosOutboundConfirmedFolder, (User.Default.IosOutbound ? TransferAction.Download : TransferAction.Exclude)},
            {FolderManager.IosInboundInboxFolder, (User.Default.IosInbound ? TransferAction.Sync : TransferAction.Exclude)},
            {FolderManager.IosInboundOutboxFolder, (User.Default.IosInbound ? TransferAction.Upload : TransferAction.Exclude)},
            {FolderManager.IosInboundConfirmedFolder, (User.Default.IosInbound? TransferAction.Download : TransferAction.Exclude)},

            {FolderManager.OtpadOutboundOutboxFolder, (User.Default.OtpadOutbound ? TransferAction.Upload : TransferAction.Exclude)},
            {FolderManager.OtpadOutboundPendFolder, (User.Default.OtpadOutbound ? TransferAction.Sync : TransferAction.Exclude)},
            {FolderManager.OtpadOutboundConfirmedFolder, (User.Default.OtpadOutbound ? TransferAction.Download : TransferAction.Exclude)},
            {FolderManager.OtpadInboundInboxFolder, (User.Default.OtpadInbound ? TransferAction.Sync : TransferAction.Exclude)},
            {FolderManager.OtpadInboundOutboxFolder, (User.Default.OtpadInbound ? TransferAction.Upload : TransferAction.Exclude)},
            {FolderManager.OtpadInboundConfirmedFolder, (User.Default.OtpadInbound? TransferAction.Download : TransferAction.Exclude)},

            {FolderManager.KpOutboundOutboxFolder, (User.Default.KpOutbound ? TransferAction.Upload : TransferAction.Exclude)},
            {FolderManager.KpOutboundPendFolder, (User.Default.KpOutbound ? TransferAction.Sync : TransferAction.Exclude)},
            {FolderManager.KpOutboundConfirmedFolder, (User.Default.KpOutbound ? TransferAction.Download : TransferAction.Exclude)},
            {FolderManager.KpInboundInboxFolder, (User.Default.KpInbound ? TransferAction.Sync : TransferAction.Exclude)},
            {FolderManager.KpInboundOutboxFolder, (User.Default.KpInbound ? TransferAction.Upload : TransferAction.Exclude)},
            {FolderManager.KpInboundConfirmedFolder, (User.Default.KpInbound ? TransferAction.Download : TransferAction.Exclude)},
            {FolderManager.PovratiOutboundOutboxFolder, (User.Default.PovratiOutbound ? TransferAction.Upload : TransferAction.Exclude)},
            {FolderManager.PovratiOutboundPendFolder, (User.Default.PovratiOutbound ? TransferAction.Sync : TransferAction.Exclude)},
            {FolderManager.PovratiOutboundConfirmedFolder, (User.Default.PovratiOutbound ? TransferAction.Download : TransferAction.Exclude)},
            {FolderManager.PovratiInboundInboxFolder, (User.Default.PovratiInbound ? TransferAction.Sync : TransferAction.Exclude)},
            {FolderManager.PovratiInboundOutboxFolder, (User.Default.PovratiInbound ? TransferAction.Upload : TransferAction.Exclude)},
            {FolderManager.PovratiInboundConfirmedFolder, (User.Default.PovratiInbound ? TransferAction.Download : TransferAction.Exclude)},
            {FolderManager.OtherOutboundOutboxFolder, (User.Default.OtherOutbound ? TransferAction.Upload : TransferAction.Exclude)},
            {FolderManager.OtherOutboundPendFolder, (User.Default.OtherOutbound ? TransferAction.Sync : TransferAction.Exclude)},
            {FolderManager.OtherOutboundConfirmedFolder, (User.Default.OtherOutbound ? TransferAction.Download : TransferAction.Exclude)},
            {FolderManager.OtherInboundInboxFolder, (User.Default.OtherInbound ? TransferAction.Sync : TransferAction.Exclude)},
            {FolderManager.OtherInboundOutboxFolder, (User.Default.OtherInbound ? TransferAction.Upload : TransferAction.Exclude)},
            {FolderManager.OtherInboundConfirmedFolder, (User.Default.OtherInbound ? TransferAction.Download : TransferAction.Exclude)}
        };

        public static Dictionary<string, string> LocalMap = new Dictionary<string, string>()
        {
            {FolderManager.InvoicesOutboundOutboxFolder, FolderManager.InvoicesOutboundSentFolder},
            {FolderManager.InvoicesInboundOutboxFolder, FolderManager.InvoicesInboundSentFolder},
            {FolderManager.IosOutboundOutboxFolder, FolderManager.IosOutboundSentFolder},
            {FolderManager.IosInboundOutboxFolder, FolderManager.IosInboundSentFolder},
            {FolderManager.OtpadOutboundOutboxFolder, FolderManager.OtpadOutboundSentFolder},
            {FolderManager.OtpadInboundOutboxFolder, FolderManager.OtpadInboundSentFolder},
            {FolderManager.KpOutboundOutboxFolder, FolderManager.KpOutboundSentFolder},
            {FolderManager.KpInboundOutboxFolder, FolderManager.KpInboundSentFolder},
            {FolderManager.PovratiOutboundOutboxFolder, FolderManager.PovratiOutboundSentFolder},
            {FolderManager.PovratiInboundOutboxFolder, FolderManager.PovratiInboundSentFolder},
            {FolderManager.OtherOutboundOutboxFolder, FolderManager.OtherOutboundSentFolder},
            {FolderManager.OtherInboundOutboxFolder, FolderManager.OtherInboundSentFolder}
        };
    }
}
