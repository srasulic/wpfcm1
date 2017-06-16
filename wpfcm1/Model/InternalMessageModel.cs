using Caliburn.Micro;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System;
using wpfcm1.PDF;

namespace wpfcm1.Model
{
    public class InternalMessageModel : PropertyChangedBase, IDataErrorInfo
    {
        public InternalMessageModel()
        {
        }

        public InternalMessageModel(DocumentModel document)
        {
            Processed = document.Processed;
            isRejected = document.isRejected;
            isApprovedForProcessing = document.isApprovedForProcessing;
            // saljemo poruku samo ako je vrednost true
            if (document.archiveReady)
            {
                archiveReady = document.archiveReady;
            }
            DocumentInfo = document.DocumentInfo;
            DocumentPath = document.DocumentPath;

            MessageFileName = Path.GetFileName(document.DocumentPath) + ".xml";
        }

        [XmlIgnore]
        public FileInfo DocumentInfo { get; set; }
        public string DocumentPath { get; set; }
        public string MessageFileName { get; set; }


        public virtual string this[string columnName]
        {
            //TODO: ovo sad nije bitno, sredi kasnije (error check)
            get { throw new System.NotImplementedException(); }
        }

        // 1.1. koristi se za promene statusa u listama za vreme obrade dokumenta
        private bool? _processed;
        public bool? Processed
        {
            get { return _processed; }
            set { _processed = value; }
        }
        

        // 4.1. interna poruka - odobren ili ne
        private bool? _isApprovedForProcessing;
        public bool? isApprovedForProcessing
        {
            get { return _isApprovedForProcessing; }
            set { _isApprovedForProcessing = value; }
        }

        // 4.3. Za Confirmed, pre svega outbound - oznaka da je dokument spreman za arhiviranje 
        private bool? _archiveReady;
        public bool? archiveReady
        {
            get { return _archiveReady; }
            set { _archiveReady = value; }
        }

        // 4.4. odbbacen - nije za dalje procesiranje
        private bool? _isRejected;
        public bool? isRejected
        {
            get { return _isRejected; }
            set { _isRejected = value; }
        }

 
        [XmlIgnore]
        public string Error { get; private set; }
    }
}
