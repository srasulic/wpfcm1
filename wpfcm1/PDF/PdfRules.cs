using System.Collections.Generic;
using System.Net.NetworkInformation;
using wpfcm1.DataAccess;
using wpfcm1.OlympusApi;
using static iTextSharp.text.pdf.AcroFields;

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

        public static Dictionary<string, string> ProcessedMap = new Dictionary<string, string>()
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

    public static class SyncTransferRules
    {
        public enum TransferAction { Upload, Download, Sync, Exclude };

        public static Dictionary<string, bool> DocDirection = new Dictionary<string, bool> {
            {"InvoicesOutbound", false},
            {"InvoicesInbound", false},
            {"IosOutbound", false},
            {"IosInbound", false},
            {"OtpadOutbound", false},
            {"OtpadInbound", false},
            {"OtpremnicaOutbound", false},
            {"OtpremnicaInbound", false},
            {"KpOutbound", false},
            {"KpInbound", false},
            {"PovratiOutbound", false},
            {"PovratiInbound", false},
            {"OtherOutbound", false},
            {"OtherInbound", false}
        };

        public static List<(string Folder, TransferAction Action)> GetFoldersForDocType(TipDokPristup item)
        {
            if (item.tip_dok == "faktura" && item.smer == "outbound")
            {
                return new List<(string, TransferAction)>
                {
                    (FolderManager.InvoicesOutboundOutboxFolder, TransferAction.Upload),
                };
            }
            if (item.tip_dok == "faktura" && item.smer == "inbound")
            {
                return new List<(string, TransferAction)>
                {
                    (FolderManager.InvoicesInboundInboxFolder, TransferAction.Sync),
                    (FolderManager.InvoicesInboundOutboxFolder, TransferAction.Upload)
                };
            }

            if (item.tip_dok == "ios" && item.smer == "outbound")
            {
                return new List<(string, TransferAction)>
                {
                    (FolderManager.IosOutboundOutboxFolder, TransferAction.Upload),
                };
            }
            if (item.tip_dok == "ios" && item.smer == "inbound")
            {
                return new List<(string, TransferAction)>
                {
                    (FolderManager.IosInboundInboxFolder, TransferAction.Sync),
                    (FolderManager.IosInboundOutboxFolder, TransferAction.Upload)
                };
            }

            if (item.tip_dok == "otpad" && item.smer == "outbound")
            {
                return new List<(string, TransferAction)>
                {
                    (FolderManager.OtpadOutboundOutboxFolder, TransferAction.Upload),
                };
            }
            if (item.tip_dok == "otpad" && item.smer == "inbound")
            {
                return new List<(string, TransferAction)>
                {
                    (FolderManager.OtpadInboundInboxFolder, TransferAction.Sync),
                    (FolderManager.OtpadInboundOutboxFolder, TransferAction.Upload)
                };
            }

            if (item.tip_dok == "otpremnica" && item.smer == "outbound")
            {
                return new List<(string, TransferAction)>
                {
                    (FolderManager.OtpremnicaOutboundOutboxFolder, TransferAction.Upload),
                };
            }
            if (item.tip_dok == "otpremnica" && item.smer == "inbound")
            {
                return new List<(string, TransferAction)>
                {
                    (FolderManager.OtpremnicaInboundInboxFolder, TransferAction.Sync),
                    (FolderManager.OtpremnicaInboundOutboxFolder, TransferAction.Upload)
                };
            }

            if (item.tip_dok == "kp" && item.smer == "outbound")
            {
                return new List<(string, TransferAction)>
                {
                    (FolderManager.KpOutboundOutboxFolder, TransferAction.Upload),
                };
            }
            if (item.tip_dok == "kp" && item.smer == "inbound")
            {
                return new List<(string, TransferAction)>
                {
                    (FolderManager.KpInboundInboxFolder, TransferAction.Sync),
                    (FolderManager.KpInboundOutboxFolder, TransferAction.Upload)
                };
            }

            if (item.tip_dok == "povrati" && item.smer == "outbound")
            {
                return new List<(string, TransferAction)>
                {
                    (FolderManager.PovratiOutboundOutboxFolder, TransferAction.Upload),
                };
            }
            if (item.tip_dok == "povrati" && item.smer == "inbound")
            {
                return new List<(string, TransferAction)>
                {
                    (FolderManager.PovratiInboundInboxFolder, TransferAction.Sync),
                    (FolderManager.PovratiInboundOutboxFolder, TransferAction.Upload)
                };
            }

            if (item.tip_dok == "ostali" && item.smer == "outbound")
            {
                return new List<(string, TransferAction)>
                {
                    (FolderManager.OtherOutboundOutboxFolder, TransferAction.Upload),
                };
            }
            if (item.tip_dok == "ostali" && item.smer == "inbound")
            {
                return new List<(string, TransferAction)>
                {
                    (FolderManager.OtherInboundInboxFolder, TransferAction.Sync),
                    (FolderManager.OtherInboundOutboxFolder, TransferAction.Upload)
                };
            }

            return new List<(string, TransferAction)>();
        }

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
