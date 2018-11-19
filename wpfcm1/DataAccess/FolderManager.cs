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
        public static readonly string InvoicesOutboundConfirmedFolder        = Path.Combine(Folders.Default.RootFolder, Folders.Default.InvoicesSubfolder, Folders.Default.OutboundConfirmedFolder);
        public static readonly string InvoicesOutboundConfirmedOutToDoFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.InvoicesSubfolder, Folders.Default.OutboundConfirmedFolder);
        public static readonly string InvoicesInboundInboxFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.InvoicesSubfolder, Folders.Default.InboundInboxFolder);
        public static readonly string InvoicesInboundOutboxFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.InvoicesSubfolder, Folders.Default.InboundOutboxFolder);
        public static readonly string InvoicesInboundSentFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.InvoicesSubfolder, Folders.Default.InboundSentFolder);
        public static readonly string InvoicesInboundConfirmedFolder     = Path.Combine(Folders.Default.RootFolder, Folders.Default.InvoicesSubfolder, Folders.Default.InboundConfirmedFolder);
        public static readonly string InvoicesInboundConfirmedToDoFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.InvoicesSubfolder, Folders.Default.InboundConfirmedFolder);
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
        public static readonly string KpOutboundConfirmedOutToDoFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.KpSubfolder, Folders.Default.OutboundConfirmedFolder);
        public static readonly string KpInboundInboxFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.KpSubfolder, Folders.Default.InboundInboxFolder);
        public static readonly string KpInboundOutboxFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.KpSubfolder, Folders.Default.InboundOutboxFolder);
        public static readonly string KpInboundSentFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.KpSubfolder, Folders.Default.InboundSentFolder);
        public static readonly string KpInboundConfirmedFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.KpSubfolder, Folders.Default.InboundConfirmedFolder);
        public static readonly string KpInboundConfirmedToDoFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.KpSubfolder, Folders.Default.InboundConfirmedFolder);
        //folderi za KP
        public static readonly string PovratiOutboundErpIfaceFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.PovratiSubfolder, Folders.Default.OutboundErpIfaceFolder);
        public static readonly string PovratiOutboundErpProcFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.PovratiSubfolder, Folders.Default.OutboundErpProcFolder);
        public static readonly string PovratiOutboundOutboxFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.PovratiSubfolder, Folders.Default.OutboundOutboxFolder);
        public static readonly string PovratiOutboundSentFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.PovratiSubfolder, Folders.Default.OutboundSentFolder);
        public static readonly string PovratiOutboundPendFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.PovratiSubfolder, Folders.Default.OutboundPendFolder);
        public static readonly string PovratiOutboundConfirmedFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.PovratiSubfolder, Folders.Default.OutboundConfirmedFolder);
        public static readonly string PovratiOutboundConfirmedOutToDoFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.PovratiSubfolder, Folders.Default.OutboundConfirmedFolder);
        public static readonly string PovratiInboundInboxFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.PovratiSubfolder, Folders.Default.InboundInboxFolder);
        public static readonly string PovratiInboundOutboxFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.PovratiSubfolder, Folders.Default.InboundOutboxFolder);
        public static readonly string PovratiInboundSentFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.PovratiSubfolder, Folders.Default.InboundSentFolder);
        public static readonly string PovratiInboundConfirmedFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.PovratiSubfolder, Folders.Default.InboundConfirmedFolder);
        public static readonly string PovratiInboundConfirmedToDoFolder = Path.Combine(Folders.Default.RootFolder, Folders.Default.PovratiSubfolder, Folders.Default.InboundConfirmedFolder);
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
        public static Dictionary<string, string> PovratiOutboundFolders { get; set; }
        public static Dictionary<string, string> PovratiInboundFolders { get; set; }
        public static Dictionary<string, string> OtherOutboundFolders { get; set; }
        public static Dictionary<string, string> OtherInboundFolders { get; set; }

        

        public static Dictionary<string, string> FolderNameMap = new Dictionary<string, string>
        {
            
            {"InvoicesOutboundErpIfaceFolder",      "Priprema"},
            {"InvoicesOutboundErpProcFolder",       "Processed"},
            {"InvoicesOutboundOutboxFolder",        "Za slanje"},
            {"InvoicesOutboundSentFolder",          "Poslato"},
            {"InvoicesOutboundPendFolder",          "Na čekanju"},
            {"InvoicesOutboundConfirmedFolder",     "Arh - uručeni"},
            {"InvoicesOutboundConfirmedOutToDoFolder", "ToDo"},

            {"InvoicesInboundInboxFolder",          "Prijem - obrada"},
            {"InvoicesInboundOutboxFolder",         "Za slanje"},
            {"InvoicesInboundSentFolder",           "Poslato"},
            {"InvoicesInboundConfirmedFolder",      "Arhiva"},
            {"InvoicesInboundConfirmedToDoFolder",  "Primljeni - nepotpisani"},
            
            {"IosOutboundErpIfaceFolder",           "Priprema"},
            {"IosOutboundErpProcFolder",            "Processed"},
            {"IosOutboundOutboxFolder",             "Za slanje"},
            {"IosOutboundSentFolder",               "Poslato"},
            {"IosOutboundPendFolder",               "Na čekanju"},
            {"IosOutboundConfirmedFolder",          "Arh - uručeni"},
            
            {"IosInboundInboxFolder",               "Prijem - obrada"},                
            {"IosInboundOutboxFolder",              "Za slanje"},
            {"IosInboundSentFolder",                "Poslato"},
            {"IosInboundConfirmedFolder",           "Arhiva"},
            
            {"KpOutboundErpIfaceFolder",            "Priprema"},
            {"KpOutboundErpProcFolder",             "Processed"},
            {"KpOutboundOutboxFolder",              "Za slanje"},
            {"KpOutboundSentFolder",                "Poslato"},
            {"KpOutboundPendFolder",                "Na čekanju"},
            {"KpOutboundConfirmedFolder",           "Arh - uručeni"},
            {"KpOutboundConfirmedOutToDoFolder",    "ToDo"},
            
            {"KpInboundInboxFolder",                "Prijem - obrada"},
            {"KpInboundOutboxFolder",               "Za slanje"},
            {"KpInboundSentFolder",                 "Poslato"},
            {"KpInboundConfirmedFolder",            "Arhiva"},
            {"KpInboundConfirmedToDoFolder",        "Primljeni - nepotpisani"},

            {"PovratiOutboundErpIfaceFolder",            "Priprema"},
            {"PovratiOutboundErpProcFolder",             "Processed"},
            {"PovratiOutboundOutboxFolder",              "Za slanje"},
            {"PovratiOutboundSentFolder",                "Poslato"},
            {"PovratiOutboundPendFolder",                "Na čekanju"},
            {"PovratiOutboundConfirmedFolder",           "Arh - uručeni"},
            {"PovratiOutboundConfirmedOutToDoFolder",    "ToDo"},

            {"PovratiInboundInboxFolder",                "Prijem - obrada"},
            {"PovratiInboundOutboxFolder",               "Za slanje"},
            {"PovratiInboundSentFolder",                 "Poslato"},
            {"PovratiInboundConfirmedFolder",            "Arhiva"},
            {"PovratiInboundConfirmedToDoFolder",        "Primljeni - nepotpisani"},


            {"OtherOutboundErpIfaceFolder",         "Priprema"},
            {"OtherOutboundErpProcFolder",          "Processed"},
            {"OtherOutboundOutboxFolder",           "Za slanje"},
            {"OtherOutboundSentFolder",             "Poslato"},
            {"OtherOutboundPendFolder",             "Na čekanju"},
            {"OtherOutboundConfirmedFolder",        "Arh - uručeni"},
            
            {"OtherInboundInboxFolder",             "Prijem - obrada"},
            {"OtherInboundOutboxFolder",            "Za slanje"},
            {"OtherInboundSentFolder",              "Poslato"},
            {"OtherInboundConfirmedFolder",         "Arhiva"}
        };

        public static readonly Dictionary<string, Type> FolderTypeMap = new Dictionary<string, Type>()
        {
            {"InvoicesOutboundErpIfaceFolder", typeof(GeneratedDocumentModel)},
            {"InvoicesOutboundOutboxFolder", typeof(OutboxDocumentModel)},
            {"InvoicesOutboundSentFolder", typeof(DocumentModel)},
            {"InvoicesOutboundPendFolder", typeof(PendDocumentModel)},
            {"InvoicesOutboundConfirmedFolder", typeof(ConfirmedDocumentModel)},
            {"InvoicesOutboundConfirmedOutToDoFolder", typeof(ConfirmedOutToDoDocumentModel)},
            {"InvoicesInboundInboxFolder", typeof(InboxDocumentModel)},
            {"InvoicesInboundOutboxFolder", typeof(OutboxDocumentModel)},
            {"InvoicesInboundSentFolder", typeof(DocumentModel)},
            {"InvoicesInboundConfirmedFolder", typeof(ConfirmedDocumentModel)},
            {"InvoicesInboundConfirmedToDoFolder", typeof(ConfirmedToDoDocumentModel)},
            {"IosOutboundErpIfaceFolder", typeof(GeneratedDocumentModel)},
            {"IosOutboundOutboxFolder", typeof(OutboxDocumentModel)},
            {"IosOutboundSentFolder", typeof(DocumentModel)},
            {"IosOutboundPendFolder", typeof(PendDocumentModel)},
            {"IosOutboundConfirmedFolder", typeof(ConfirmedDocumentModel)},
            {"IosInboundInboxFolder", typeof(InboxDocumentModel)},
            {"IosInboundOutboxFolder", typeof(OutboxDocumentModel)},
            {"IosInboundSentFolder", typeof(DocumentModel)},
            {"IosInboundConfirmedFolder", typeof(ConfirmedDocumentModel)},
            {"KpOutboundErpIfaceFolder", typeof(GeneratedDocumentModel)},
            {"KpOutboundOutboxFolder", typeof(OutboxDocumentModel)},
            {"KpOutboundSentFolder", typeof(DocumentModel)},
            {"KpOutboundPendFolder", typeof(PendDocumentModel)},
            {"KpOutboundConfirmedFolder", typeof(ConfirmedDocumentModel)},
            {"KpOutboundConfirmedOutToDoFolder", typeof(ConfirmedOutToDoDocumentModel)},
            {"KpInboundInboxFolder", typeof(InboxDocumentModel)},
            {"KpInboundOutboxFolder", typeof(OutboxDocumentModel)},
            {"KpInboundSentFolder", typeof(DocumentModel)},
            {"KpInboundConfirmedFolder", typeof(ConfirmedDocumentModel)},
            {"KpInboundConfirmedToDoFolder", typeof(ConfirmedToDoDocumentModel)},

            {"PovratiOutboundErpIfaceFolder", typeof(GeneratedDocumentModel)},
            {"PovratiOutboundOutboxFolder", typeof(OutboxDocumentModel)},
            {"PovratiOutboundSentFolder", typeof(DocumentModel)},
            {"PovratiOutboundPendFolder", typeof(PendDocumentModel)},
            {"PovratiOutboundConfirmedFolder", typeof(ConfirmedDocumentModel)},
            {"PovratiOutboundConfirmedOutToDoFolder", typeof(ConfirmedOutToDoDocumentModel)},
            {"PovratiInboundInboxFolder", typeof(InboxDocumentModel)},
            {"PovratiInboundOutboxFolder", typeof(OutboxDocumentModel)},
            {"PovratiInboundSentFolder", typeof(DocumentModel)},
            {"PovratiInboundConfirmedFolder", typeof(ConfirmedDocumentModel)},
            {"PovratiInboundConfirmedToDoFolder", typeof(ConfirmedToDoDocumentModel)},

            { "OtherOutboundErpIfaceFolder", typeof(GeneratedDocumentModel)},
            {"OtherOutboundOutboxFolder", typeof(OutboxDocumentModel)},
            {"OtherOutboundSentFolder", typeof(DocumentModel)},
            {"OtherOutboundPendFolder", typeof(PendDocumentModel)},
            {"OtherOutboundConfirmedFolder", typeof(ConfirmedDocumentModel)},
            {"OtherInboundInboxFolder", typeof(InboxDocumentModel)},
            {"OtherInboundOutboxFolder", typeof(OutboxDocumentModel)},
            {"OtherInboundSentFolder", typeof(DocumentModel)},
            {"OtherInboundConfirmedFolder", typeof(ConfirmedDocumentModel)}
        };

        static FolderManager()
        {
            InvoicesOutboundFolders = new Dictionary<string, string>
            {
                {"InvoicesOutboundErpIfaceFolder", InvoicesOutboundErpIfaceFolder},
                {"InvoicesOutboundOutboxFolder", InvoicesOutboundOutboxFolder},
                {"InvoicesOutboundSentFolder", InvoicesOutboundSentFolder},
                {"InvoicesOutboundPendFolder", InvoicesOutboundPendFolder},
                {"InvoicesOutboundConfirmedFolder", InvoicesOutboundConfirmedFolder},
                {"InvoicesOutboundConfirmedOutToDoFolder", InvoicesOutboundConfirmedFolder}
            };
            CheckFolders(InvoicesOutboundFolders);
            if (!Directory.Exists(InvoicesOutboundErpProcFolder)) Directory.CreateDirectory(InvoicesOutboundErpProcFolder);

            InvoicesInboundFolders = new Dictionary<string, string>
            {
                {"InvoicesInboundInboxFolder", InvoicesInboundInboxFolder},
                {"InvoicesInboundConfirmedToDoFolder", InvoicesInboundConfirmedFolder},                  
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
                {"KpOutboundConfirmedFolder", KpOutboundConfirmedFolder},
                {"KpOutboundConfirmedOutToDoFolder", KpOutboundConfirmedFolder}
            };
            CheckFolders(KpOutboundFolders);
            if (!Directory.Exists(KpOutboundErpProcFolder)) Directory.CreateDirectory(KpOutboundErpProcFolder);

            KpInboundFolders = new Dictionary<string, string>
            {
                {"KpInboundInboxFolder", KpInboundInboxFolder},
                {"KpInboundConfirmedToDoFolder", KpInboundConfirmedFolder},
                {"KpInboundOutboxFolder", KpInboundOutboxFolder},
                {"KpInboundSentFolder", KpInboundSentFolder},
                {"KpInboundConfirmedFolder", KpInboundConfirmedFolder}
            };
            CheckFolders(KpInboundFolders);



            PovratiOutboundFolders = new Dictionary<string, string>
            {
                {"PovratiOutboundErpIfaceFolder", PovratiOutboundErpIfaceFolder},
                {"PovratiOutboundOutboxFolder", PovratiOutboundOutboxFolder},
                {"PovratiOutboundSentFolder", PovratiOutboundSentFolder},
                {"PovratiOutboundPendFolder", PovratiOutboundPendFolder},
                {"PovratiOutboundConfirmedFolder", PovratiOutboundConfirmedFolder},
                {"PovratiOutboundConfirmedOutToDoFolder", PovratiOutboundConfirmedFolder}
            };
            CheckFolders(PovratiOutboundFolders);
            if (!Directory.Exists(PovratiOutboundErpProcFolder)) Directory.CreateDirectory(PovratiOutboundErpProcFolder);

            PovratiInboundFolders = new Dictionary<string, string>
            {
                {"PovratiInboundInboxFolder", PovratiInboundInboxFolder},
                {"PovratiInboundConfirmedToDoFolder", PovratiInboundConfirmedFolder},
                {"PovratiInboundOutboxFolder", PovratiInboundOutboxFolder},
                {"PovratiInboundSentFolder", PovratiInboundSentFolder},
                {"PovratiInboundConfirmedFolder", PovratiInboundConfirmedFolder}
            };
            CheckFolders(PovratiInboundFolders);


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
