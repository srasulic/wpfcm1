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
            {FolderManager.IosInboundInboxFolder, SignatureLocation.UpperRight},
            {FolderManager.OtpadOutboundErpIfaceFolder, SignatureLocation.UpperLeft},
            {FolderManager.OtpadInboundInboxFolder, SignatureLocation.UpperRight},
            {FolderManager.OtpremnicaOutboundErpIfaceFolder, SignatureLocation.UpperLeft},
            {FolderManager.OtpremnicaInboundInboxFolder, SignatureLocation.UpperRight},
            {FolderManager.KpOutboundErpIfaceFolder, SignatureLocation.UpperLeft},
            {FolderManager.KpInboundInboxFolder, SignatureLocation.UpperRight},
            {FolderManager.PovratiOutboundErpIfaceFolder, SignatureLocation.UpperLeft},
            {FolderManager.PovratiInboundInboxFolder, SignatureLocation.UpperRight},
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
            {FolderManager.InvoicesInboundInboxFolder, FolderManager.InvoicesInboundOutboxFolder},
            {FolderManager.IosOutboundErpIfaceFolder, FolderManager.IosOutboundOutboxFolder},
            {FolderManager.IosInboundInboxFolder, FolderManager.IosInboundOutboxFolder},
            {FolderManager.OtpadOutboundErpIfaceFolder, FolderManager.OtpadOutboundOutboxFolder},
            {FolderManager.OtpadInboundInboxFolder, FolderManager.OtpadInboundOutboxFolder},
            {FolderManager.OtpremnicaOutboundErpIfaceFolder, FolderManager.OtpremnicaOutboundOutboxFolder},
            {FolderManager.OtpremnicaInboundInboxFolder, FolderManager.OtpremnicaInboundOutboxFolder},
            {FolderManager.KpOutboundErpIfaceFolder, FolderManager.KpOutboundOutboxFolder},
            {FolderManager.KpInboundInboxFolder, FolderManager.KpInboundOutboxFolder},
            {FolderManager.PovratiOutboundErpIfaceFolder, FolderManager.PovratiOutboundOutboxFolder},
            {FolderManager.PovratiInboundInboxFolder, FolderManager.PovratiInboundOutboxFolder},
            {FolderManager.OtherOutboundErpIfaceFolder, FolderManager.OtherOutboundOutboxFolder},
            {FolderManager.OtherInboundInboxFolder, FolderManager.OtherInboundOutboxFolder},
        };

        public static Dictionary<string, string> ProcessedMap= new Dictionary<string, string>()
        {
            {FolderManager.InvoicesOutboundErpIfaceFolder, FolderManager.InvoicesOutboundErpProcFolder},
            {FolderManager.IosOutboundErpIfaceFolder, FolderManager.IosOutboundErpProcFolder},
            {FolderManager.OtpadOutboundErpIfaceFolder, FolderManager.OtpadOutboundErpProcFolder},
            {FolderManager.OtpremnicaOutboundErpIfaceFolder, FolderManager.OtpremnicaOutboundErpProcFolder},
            {FolderManager.KpOutboundErpIfaceFolder, FolderManager.KpOutboundErpProcFolder},
            {FolderManager.PovratiOutboundErpIfaceFolder, FolderManager.PovratiOutboundErpProcFolder},
            {FolderManager.OtherOutboundErpIfaceFolder, FolderManager.OtherOutboundErpProcFolder},            
            {FolderManager.InvoicesOutboundOutboxFolder, FolderManager.InvoicesOutboundErpProcFolder},
            {FolderManager.IosOutboundOutboxFolder, FolderManager.IosOutboundErpProcFolder},
            {FolderManager.OtpadOutboundOutboxFolder, FolderManager.OtpadOutboundErpProcFolder},
            {FolderManager.OtpremnicaOutboundOutboxFolder, FolderManager.OtpremnicaOutboundErpProcFolder},
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
            {FolderManager.IosOutboundErpIfaceFolder, FinalAction.Store},
            {FolderManager.IosInboundInboxFolder, FinalAction.SecondSignatureMark},
            {FolderManager.OtpadOutboundErpIfaceFolder, FinalAction.Store},
            {FolderManager.OtpadInboundInboxFolder, FinalAction.SecondSignatureMark},
            {FolderManager.OtpremnicaOutboundErpIfaceFolder, FinalAction.Store},
            {FolderManager.OtpremnicaInboundInboxFolder, FinalAction.SecondSignatureMark},
            {FolderManager.KpOutboundErpIfaceFolder, FinalAction.Store},
            {FolderManager.KpInboundInboxFolder, FinalAction.SecondSignatureMark},
            {FolderManager.PovratiOutboundErpIfaceFolder, FinalAction.Store},
            {FolderManager.PovratiInboundInboxFolder, FinalAction.SecondSignatureMark},
            {FolderManager.OtherOutboundErpIfaceFolder, FinalAction.Store},
            {FolderManager.OtherInboundInboxFolder, FinalAction.SecondSignatureMark}
        };
    }

    public static class FtpTransferRules
    {
        public enum TransferAction { Upload, Download, Sync, Exclude };

        public static Dictionary<string, TransferAction> Action = new Dictionary<string, TransferAction>()
        {
            //{FolderManager.InvoicesOutboundOutboxFolder, (User.Default.InvoicesOutbound ? TransferAction.Upload : TransferAction.Exclude)},
            //{FolderManager.InvoicesInboundInboxFolder, (User.Default.InvoicesInbound ? TransferAction.Sync : TransferAction.Exclude)},
            //{FolderManager.InvoicesInboundOutboxFolder, (User.Default.InvoicesInbound ? TransferAction.Upload : TransferAction.Exclude)},

            //{FolderManager.IosOutboundOutboxFolder, (User.Default.IosOutbound ? TransferAction.Upload : TransferAction.Exclude)},
            //{FolderManager.IosInboundInboxFolder, (User.Default.IosInbound ? TransferAction.Sync : TransferAction.Exclude)},
            //{FolderManager.IosInboundOutboxFolder, (User.Default.IosInbound ? TransferAction.Upload : TransferAction.Exclude)},

            //{FolderManager.OtpadOutboundOutboxFolder, (User.Default.OtpadOutbound ? TransferAction.Upload : TransferAction.Exclude)},
            //{FolderManager.OtpadInboundInboxFolder, (User.Default.OtpadInbound ? TransferAction.Sync : TransferAction.Exclude)},
            //{FolderManager.OtpadInboundOutboxFolder, (User.Default.OtpadInbound ? TransferAction.Upload : TransferAction.Exclude)},

            //{FolderManager.OtpremnicaOutboundOutboxFolder, (User.Default.OtpremnicaOutbound ? TransferAction.Upload : TransferAction.Exclude)},
            //{FolderManager.OtpremnicaInboundInboxFolder, (User.Default.OtpremnicaInbound  ? TransferAction.Sync : TransferAction.Exclude)},
            //{FolderManager.OtpremnicaInboundOutboxFolder, (User.Default.OtpremnicaInbound  ? TransferAction.Upload : TransferAction.Exclude)},

            //{FolderManager.KpOutboundOutboxFolder, (User.Default.KpOutbound ? TransferAction.Upload : TransferAction.Exclude)},
            //{FolderManager.KpInboundInboxFolder, (User.Default.KpInbound ? TransferAction.Sync : TransferAction.Exclude)},
            //{FolderManager.KpInboundOutboxFolder, (User.Default.KpInbound ? TransferAction.Upload : TransferAction.Exclude)},

            //{FolderManager.PovratiOutboundOutboxFolder, (User.Default.PovratiOutbound ? TransferAction.Upload : TransferAction.Exclude)},
            //{FolderManager.PovratiInboundInboxFolder, (User.Default.PovratiInbound ? TransferAction.Sync : TransferAction.Exclude)},
            //{FolderManager.PovratiInboundOutboxFolder, (User.Default.PovratiInbound ? TransferAction.Upload : TransferAction.Exclude)},

            //{FolderManager.OtherOutboundOutboxFolder, (User.Default.OtherOutbound ? TransferAction.Upload : TransferAction.Exclude)},
            //{FolderManager.OtherInboundInboxFolder, (User.Default.OtherInbound ? TransferAction.Sync : TransferAction.Exclude)},
            //{FolderManager.OtherInboundOutboxFolder, (User.Default.OtherInbound ? TransferAction.Upload : TransferAction.Exclude)},
        };

        public static Dictionary<string, string> LocalMap = new Dictionary<string, string>()
        {
            {FolderManager.InvoicesOutboundOutboxFolder, FolderManager.InvoicesOutboundSentFolder},
            {FolderManager.InvoicesInboundOutboxFolder, FolderManager.InvoicesInboundSentFolder},
            {FolderManager.IosOutboundOutboxFolder, FolderManager.IosOutboundSentFolder},
            {FolderManager.IosInboundOutboxFolder, FolderManager.IosInboundSentFolder},
            {FolderManager.OtpadOutboundOutboxFolder, FolderManager.OtpadOutboundSentFolder},
            {FolderManager.OtpadInboundOutboxFolder, FolderManager.OtpadInboundSentFolder},
            {FolderManager.OtpremnicaOutboundOutboxFolder, FolderManager.OtpremnicaOutboundSentFolder},
            {FolderManager.OtpremnicaInboundOutboxFolder,  FolderManager.OtpremnicaInboundSentFolder},
            {FolderManager.KpOutboundOutboxFolder, FolderManager.KpOutboundSentFolder},
            {FolderManager.KpInboundOutboxFolder, FolderManager.KpInboundSentFolder},
            {FolderManager.PovratiOutboundOutboxFolder, FolderManager.PovratiOutboundSentFolder},
            {FolderManager.PovratiInboundOutboxFolder, FolderManager.PovratiInboundSentFolder},
            {FolderManager.OtherOutboundOutboxFolder, FolderManager.OtherOutboundSentFolder},
            {FolderManager.OtherInboundOutboxFolder, FolderManager.OtherInboundSentFolder}
        };
    }
}
