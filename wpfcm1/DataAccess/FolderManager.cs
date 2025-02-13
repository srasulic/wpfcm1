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
        public static readonly string InvoicesInboundInboxFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.InvoicesSubfolder, Folders.Default.InboundInboxFolder);
        public static readonly string InvoicesInboundOutboxFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.InvoicesSubfolder, Folders.Default.InboundOutboxFolder);
        public static readonly string InvoicesInboundSentFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.InvoicesSubfolder, Folders.Default.InboundSentFolder);
        //folderi za IOS
        public static readonly string IosOutboundErpIfaceFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.IosSubfolder, Folders.Default.OutboundErpIfaceFolder);
        public static readonly string IosOutboundErpProcFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.IosSubfolder, Folders.Default.OutboundErpProcFolder);
        public static readonly string IosOutboundOutboxFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.IosSubfolder, Folders.Default.OutboundOutboxFolder);
        public static readonly string IosOutboundSentFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.IosSubfolder, Folders.Default.OutboundSentFolder);
        public static readonly string IosInboundInboxFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.IosSubfolder, Folders.Default.InboundInboxFolder);
        public static readonly string IosInboundOutboxFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.IosSubfolder, Folders.Default.InboundOutboxFolder);
        public static readonly string IosInboundSentFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.IosSubfolder, Folders.Default.InboundSentFolder);
        //folderi za Kretanje otpada
        public static readonly string OtpadOutboundErpIfaceFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.OtpadSubfolder, Folders.Default.OutboundErpIfaceFolder);
        public static readonly string OtpadOutboundErpProcFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.OtpadSubfolder, Folders.Default.OutboundErpProcFolder);
        public static readonly string OtpadOutboundOutboxFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.OtpadSubfolder, Folders.Default.OutboundOutboxFolder);
        public static readonly string OtpadOutboundSentFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.OtpadSubfolder, Folders.Default.OutboundSentFolder);
        public static readonly string OtpadInboundInboxFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.OtpadSubfolder, Folders.Default.InboundInboxFolder);
        public static readonly string OtpadInboundOutboxFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.OtpadSubfolder, Folders.Default.InboundOutboxFolder);
        public static readonly string OtpadInboundSentFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.OtpadSubfolder, Folders.Default.InboundSentFolder);
        //folderi za Otpremnica
        public static readonly string OtpremnicaOutboundErpIfaceFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.OtpremnicaSubfolder, Folders.Default.OutboundErpIfaceFolder);
        public static readonly string OtpremnicaOutboundErpProcFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.OtpremnicaSubfolder, Folders.Default.OutboundErpProcFolder);
        public static readonly string OtpremnicaOutboundOutboxFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.OtpremnicaSubfolder, Folders.Default.OutboundOutboxFolder);
        public static readonly string OtpremnicaOutboundSentFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.OtpremnicaSubfolder, Folders.Default.OutboundSentFolder);
        public static readonly string OtpremnicaInboundInboxFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.OtpremnicaSubfolder, Folders.Default.InboundInboxFolder);
        public static readonly string OtpremnicaInboundOutboxFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.OtpremnicaSubfolder, Folders.Default.InboundOutboxFolder);
        public static readonly string OtpremnicaInboundSentFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.OtpremnicaSubfolder, Folders.Default.InboundSentFolder);
        //folderi za KP
        public static readonly string KpOutboundErpIfaceFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.KpSubfolder, Folders.Default.OutboundErpIfaceFolder);
        public static readonly string KpOutboundErpProcFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.KpSubfolder, Folders.Default.OutboundErpProcFolder);
        public static readonly string KpOutboundOutboxFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.KpSubfolder, Folders.Default.OutboundOutboxFolder);
        public static readonly string KpOutboundSentFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.KpSubfolder, Folders.Default.OutboundSentFolder);
        public static readonly string KpInboundInboxFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.KpSubfolder, Folders.Default.InboundInboxFolder);
        public static readonly string KpInboundOutboxFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.KpSubfolder, Folders.Default.InboundOutboxFolder);
        public static readonly string KpInboundSentFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.KpSubfolder, Folders.Default.InboundSentFolder);
        //folderi za povrati
        public static readonly string PovratiOutboundErpIfaceFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.PovratiSubfolder, Folders.Default.OutboundErpIfaceFolder);
        public static readonly string PovratiOutboundErpProcFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.PovratiSubfolder, Folders.Default.OutboundErpProcFolder);
        public static readonly string PovratiOutboundOutboxFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.PovratiSubfolder, Folders.Default.OutboundOutboxFolder);
        public static readonly string PovratiOutboundSentFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.PovratiSubfolder, Folders.Default.OutboundSentFolder);
        public static readonly string PovratiInboundInboxFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.PovratiSubfolder, Folders.Default.InboundInboxFolder);
        public static readonly string PovratiInboundOutboxFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.PovratiSubfolder, Folders.Default.InboundOutboxFolder);
        public static readonly string PovratiInboundSentFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.PovratiSubfolder, Folders.Default.InboundSentFolder);
        //folderi za ostale ulazne/izlazne
        public static readonly string OtherOutboundErpIfaceFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.OtherSubfolder, Folders.Default.OutboundErpIfaceFolder);
        public static readonly string OtherOutboundErpProcFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.OtherSubfolder, Folders.Default.OutboundErpProcFolder);
        public static readonly string OtherOutboundOutboxFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.OtherSubfolder, Folders.Default.OutboundOutboxFolder);
        public static readonly string OtherOutboundSentFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.OtherSubfolder, Folders.Default.OutboundSentFolder);
        public static readonly string OtherInboundInboxFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.OtherSubfolder, Folders.Default.InboundInboxFolder);
        public static readonly string OtherInboundOutboxFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.OtherSubfolder, Folders.Default.InboundOutboxFolder);
        public static readonly string OtherInboundSentFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.OtherSubfolder, Folders.Default.InboundSentFolder);

        //grupe foldera
        public static Dictionary<string, string> InvoicesOutboundFolders { get; set; }
        public static Dictionary<string, string> InvoicesInboundFolders { get; set; }
        public static Dictionary<string, string> IosOutboundFolders { get; set; }
        public static Dictionary<string, string> IosInboundFolders { get; set; }
        public static Dictionary<string, string> OtpadOutboundFolders { get; set; }
        public static Dictionary<string, string> OtpadInboundFolders { get; set; }
        public static Dictionary<string, string> OtpremnicaOutboundFolders { get; set; }
        public static Dictionary<string, string> OtpremnicaInboundFolders { get; set; }
        public static Dictionary<string, string> KpOutboundFolders { get; set; }
        public static Dictionary<string, string> KpInboundFolders { get; set; }
        public static Dictionary<string, string> PovratiOutboundFolders { get; set; }
        public static Dictionary<string, string> PovratiInboundFolders { get; set; }
        public static Dictionary<string, string> OtherOutboundFolders { get; set; }
        public static Dictionary<string, string> OtherInboundFolders { get; set; }

        public static Dictionary<string, string> FolderNameMap = new Dictionary<string, string>
        {

            {"InvoicesOutboundErpIfaceFolder",              "Priprema"},
            {"InvoicesOutboundErpProcFolder",               "Processed"},
            {"InvoicesOutboundOutboxFolder",                "Za slanje"},
            {"InvoicesOutboundSentFolder",                  "Poslato"},

            {"InvoicesInboundInboxFolder",                  "Prijem - obrada"},
            {"InvoicesInboundOutboxFolder",                 "Za slanje"},
            {"InvoicesInboundSentFolder",                   "Poslato"},

            {"IosOutboundErpIfaceFolder",                   "Priprema"},
            {"IosOutboundErpProcFolder",                    "Processed"},
            {"IosOutboundOutboxFolder",                     "Za slanje"},
            {"IosOutboundSentFolder",                       "Poslato"},

            {"IosInboundInboxFolder",                       "Prijem - obrada"},
            {"IosInboundOutboxFolder",                      "Za slanje"},
            {"IosInboundSentFolder",                        "Poslato"},

            {"OtpadOutboundErpIfaceFolder",                 "Priprema"},
            {"OtpadOutboundErpProcFolder",                  "Processed"},
            {"OtpadOutboundOutboxFolder",                   "Za slanje"},
            {"OtpadOutboundSentFolder",                     "Poslato"},

            {"OtpadInboundInboxFolder",                     "Prijem - obrada"},
            {"OtpadInboundOutboxFolder",                    "Za slanje"},
            {"OtpadInboundSentFolder",                      "Poslato"},

            {"OtpremnicaOutboundErpIfaceFolder",            "Priprema"},
            {"OtpremnicaOutboundErpProcFolder",             "Processed"},
            {"OtpremnicaOutboundOutboxFolder",               "Za slanje"},
            {"OtpremnicaOutboundSentFolder",                "Poslato"},

            {"OtpremnicaInboundInboxFolder",                "Prijem - obrada"},
            {"OtpremnicaInboundOutboxFolder",               "Za slanje"},
            {"OtpremnicaInboundSentFolder",                 "Poslato"},

            {"KpOutboundErpIfaceFolder",                    "Priprema"},
            {"KpOutboundErpProcFolder",                     "Processed"},
            {"KpOutboundOutboxFolder",                      "Za slanje"},
            {"KpOutboundSentFolder",                        "Poslato"},

            {"KpInboundInboxFolder",                        "Prijem - obrada"},
            {"KpInboundOutboxFolder",                       "Za slanje"},
            {"KpInboundSentFolder",                         "Poslato"},

            {"PovratiOutboundErpIfaceFolder",               "Priprema"},
            {"PovratiOutboundErpProcFolder",                "Processed"},
            {"PovratiOutboundOutboxFolder",                 "Za slanje"},
            {"PovratiOutboundSentFolder",                   "Poslato"},

            {"PovratiInboundInboxFolder",                   "Prijem - obrada"},
            {"PovratiInboundOutboxFolder",                  "Za slanje"},
            {"PovratiInboundSentFolder",                    "Poslato"},

            {"OtherOutboundErpIfaceFolder",                 "Priprema"},
            {"OtherOutboundErpProcFolder",                  "Processed"},
            {"OtherOutboundOutboxFolder",                   "Za slanje"},
            {"OtherOutboundSentFolder",                     "Poslato"},

            {"OtherInboundInboxFolder",                     "Prijem - obrada"},
            {"OtherInboundOutboxFolder",                    "Za slanje"},
            {"OtherInboundSentFolder",                      "Poslato"},
        };

        public static readonly Dictionary<string, Type> FolderTypeMap = new Dictionary<string, Type>()
        {
            {"InvoicesOutboundErpIfaceFolder",              typeof(GeneratedDocumentModel)},
            {"InvoicesOutboundOutboxFolder",                typeof(OutboxDocumentModel)},
            {"InvoicesOutboundSentFolder",                  typeof(DocumentModel)},
            {"InvoicesInboundInboxFolder",                  typeof(InboxDocumentModel)},
            {"InvoicesInboundOutboxFolder",                 typeof(OutboxDocumentModel)},
            {"InvoicesInboundSentFolder",                   typeof(DocumentModel)},

            {"IosOutboundErpIfaceFolder",                   typeof(GeneratedDocumentModel)},
            {"IosOutboundOutboxFolder",                     typeof(OutboxDocumentModel)},
            {"IosOutboundSentFolder",                       typeof(DocumentModel)},
            {"IosInboundInboxFolder",                       typeof(InboxDocumentModel)},
            {"IosInboundOutboxFolder",                      typeof(OutboxDocumentModel)},
            {"IosInboundSentFolder",                        typeof(DocumentModel)},

            {"OtpadOutboundErpIfaceFolder",                 typeof(GeneratedDocumentModel)},
            {"OtpadOutboundOutboxFolder",                   typeof(OutboxDocumentModel)},
            {"OtpadOutboundSentFolder",                     typeof(DocumentModel)},
            {"OtpadInboundInboxFolder",                     typeof(InboxDocumentModel)},
            {"OtpadInboundOutboxFolder",                    typeof(OutboxDocumentModel)},
            {"OtpadInboundSentFolder",                      typeof(DocumentModel)},

            {"OtpremnicaOutboundErpIfaceFolder",            typeof(GeneratedDocumentModel)},
            {"OtpremnicaOutboundOutboxFolder",              typeof(OutboxDocumentModel)},
            {"OtpremnicaOutboundSentFolder",                typeof(DocumentModel)},
            {"OtpremnicaInboundInboxFolder",                typeof(InboxDocumentModel)},
            {"OtpremnicaInboundOutboxFolder",               typeof(OutboxDocumentModel)},
            {"OtpremnicaInboundSentFolder",                 typeof(DocumentModel)},

            {"KpOutboundErpIfaceFolder",                    typeof(GeneratedDocumentModel)},
            {"KpOutboundOutboxFolder",                      typeof(OutboxDocumentModel)},
            {"KpOutboundSentFolder",                        typeof(DocumentModel)},
            {"KpInboundInboxFolder",                        typeof(InboxDocumentModel)},
            {"KpInboundOutboxFolder",                       typeof(OutboxDocumentModel)},
            {"KpInboundSentFolder",                         typeof(DocumentModel)},

            {"PovratiOutboundErpIfaceFolder",               typeof(GeneratedDocumentModel)},
            {"PovratiOutboundOutboxFolder",                 typeof(OutboxDocumentModel)},
            {"PovratiOutboundSentFolder",                   typeof(DocumentModel)},
            {"PovratiInboundInboxFolder",                   typeof(InboxDocumentModel)},
            {"PovratiInboundOutboxFolder",                  typeof(OutboxDocumentModel)},
            {"PovratiInboundSentFolder",                    typeof(DocumentModel)},

            { "OtherOutboundErpIfaceFolder",                typeof(GeneratedDocumentModel)},
            {"OtherOutboundOutboxFolder",                   typeof(OutboxDocumentModel)},
            {"OtherOutboundSentFolder",                     typeof(DocumentModel)},
            {"OtherInboundInboxFolder",                     typeof(InboxDocumentModel)},
            {"OtherInboundOutboxFolder",                    typeof(OutboxDocumentModel)},
            {"OtherInboundSentFolder",                      typeof(DocumentModel)},
        };

        static FolderManager()
        {
            InvoicesOutboundFolders = new Dictionary<string, string>
            {
                {"InvoicesOutboundErpIfaceFolder", InvoicesOutboundErpIfaceFolder},
                {"InvoicesOutboundOutboxFolder", InvoicesOutboundOutboxFolder},
                {"InvoicesOutboundSentFolder", InvoicesOutboundSentFolder}
            };
            CheckFolders(InvoicesOutboundFolders);
            if (!Directory.Exists(InvoicesOutboundErpProcFolder)) Directory.CreateDirectory(InvoicesOutboundErpProcFolder);

            InvoicesInboundFolders = new Dictionary<string, string>
            {
                {"InvoicesInboundInboxFolder", InvoicesInboundInboxFolder},
                {"InvoicesInboundOutboxFolder", InvoicesInboundOutboxFolder},
                {"InvoicesInboundSentFolder", InvoicesInboundSentFolder},
            };
            CheckFolders(InvoicesInboundFolders);

            IosOutboundFolders = new Dictionary<string, string>
            {
                {"IosOutboundErpIfaceFolder", IosOutboundErpIfaceFolder},
                {"IosOutboundOutboxFolder", IosOutboundOutboxFolder},
                {"IosOutboundSentFolder", IosOutboundSentFolder}
            };
            CheckFolders(IosOutboundFolders);
            if (!Directory.Exists(IosOutboundErpProcFolder)) Directory.CreateDirectory(IosOutboundErpProcFolder);

            IosInboundFolders = new Dictionary<string, string>
            {
                {"IosInboundInboxFolder", IosInboundInboxFolder},
                {"IosInboundOutboxFolder", IosInboundOutboxFolder},
                {"IosInboundSentFolder", IosInboundSentFolder},
            };
            CheckFolders(IosInboundFolders);

            OtpadOutboundFolders = new Dictionary<string, string>
            {
                {"OtpadOutboundErpIfaceFolder", OtpadOutboundErpIfaceFolder},
                {"OtpadOutboundOutboxFolder", OtpadOutboundOutboxFolder},
                {"OtpadOutboundSentFolder", OtpadOutboundSentFolder}
            };
            CheckFolders(OtpadOutboundFolders);
            if (!Directory.Exists(OtpadOutboundErpProcFolder)) Directory.CreateDirectory(OtpadOutboundErpProcFolder);

            OtpadInboundFolders = new Dictionary<string, string>
            {
                {"OtpadInboundInboxFolder", OtpadInboundInboxFolder},
                {"OtpadInboundOutboxFolder", OtpadInboundOutboxFolder},
                {"OtpadInboundSentFolder", OtpadInboundSentFolder},
            };
            CheckFolders(OtpadInboundFolders);

            OtpremnicaOutboundFolders = new Dictionary<string, string>
            {
                {"OtpremnicaOutboundErpIfaceFolder", OtpremnicaOutboundErpIfaceFolder},
                {"OtpremnicaOutboundOutboxFolder", OtpremnicaOutboundOutboxFolder},
                {"OtpremnicaOutboundSentFolder", OtpremnicaOutboundSentFolder}
            };
            CheckFolders(OtpremnicaOutboundFolders);
            if (!Directory.Exists(OtpremnicaOutboundErpProcFolder)) Directory.CreateDirectory(OtpremnicaOutboundErpProcFolder);

            OtpremnicaInboundFolders = new Dictionary<string, string>
            {
                {"OtpremnicaInboundInboxFolder", OtpremnicaInboundInboxFolder},
                {"OtpremnicaInboundOutboxFolder", OtpremnicaInboundOutboxFolder},
                {"OtpremnicaInboundSentFolder", OtpremnicaInboundSentFolder},
            };
            CheckFolders(OtpremnicaInboundFolders);

            KpOutboundFolders = new Dictionary<string, string>
            {
                {"KpOutboundErpIfaceFolder", KpOutboundErpIfaceFolder},
                {"KpOutboundOutboxFolder", KpOutboundOutboxFolder},
                {"KpOutboundSentFolder", KpOutboundSentFolder}
            };
            CheckFolders(KpOutboundFolders);
            if (!Directory.Exists(KpOutboundErpProcFolder)) Directory.CreateDirectory(KpOutboundErpProcFolder);

            KpInboundFolders = new Dictionary<string, string>
            {
                {"KpInboundInboxFolder", KpInboundInboxFolder},
                {"KpInboundOutboxFolder", KpInboundOutboxFolder},
                {"KpInboundSentFolder", KpInboundSentFolder},
            };
            CheckFolders(KpInboundFolders);

            PovratiOutboundFolders = new Dictionary<string, string>
            {
                {"PovratiOutboundErpIfaceFolder", PovratiOutboundErpIfaceFolder},
                {"PovratiOutboundOutboxFolder", PovratiOutboundOutboxFolder},
                {"PovratiOutboundSentFolder", PovratiOutboundSentFolder}
            };
            CheckFolders(PovratiOutboundFolders);
            if (!Directory.Exists(PovratiOutboundErpProcFolder)) Directory.CreateDirectory(PovratiOutboundErpProcFolder);

            PovratiInboundFolders = new Dictionary<string, string>
            {
                {"PovratiInboundInboxFolder", PovratiInboundInboxFolder},
                {"PovratiInboundOutboxFolder", PovratiInboundOutboxFolder},
                {"PovratiInboundSentFolder", PovratiInboundSentFolder},
            };
            CheckFolders(PovratiInboundFolders);

            OtherOutboundFolders = new Dictionary<string, string>
            {
                {"OtherOutboundErpIfaceFolder", OtherOutboundErpIfaceFolder},
                {"OtherOutboundOutboxFolder", OtherOutboundOutboxFolder},
                {"OtherOutboundSentFolder", OtherOutboundSentFolder}
            };
            CheckFolders(OtherOutboundFolders);
            if (!Directory.Exists(OtherOutboundErpProcFolder)) Directory.CreateDirectory(OtherOutboundErpProcFolder);

            OtherInboundFolders = new Dictionary<string, string>
            {
                {"OtherInboundInboxFolder", OtherInboundInboxFolder},
                {"OtherInboundOutboxFolder", OtherInboundOutboxFolder},
                {"OtherInboundSentFolder", OtherInboundSentFolder},
            };
            CheckFolders(OtherInboundFolders);
        }

        private static void CheckFolders(Dictionary<string, string> folders)
        {
            foreach (var folder in folders.Where(folder => !Directory.Exists(folder.Value)))
            {
                Log.Info(String.Format("Creating {0}", folder.Value));
                Directory.CreateDirectory(folder.Value);
            }
        }
    }
}
