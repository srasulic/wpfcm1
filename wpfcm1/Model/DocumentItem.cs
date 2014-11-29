using Caliburn.Micro;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace wpfcm1.Model
{
    public class DocumentItem : PropertyChangedBase, IDataErrorInfo
    {
        public DocumentItem()
        {
        }

        public DocumentItem(string path)
        {
            DocumentInfo = new FileInfo(path);
            DocumentPath = path;
        }

        public DocumentItem(DocumentItem that)
        {
            DocumentInfo = that.DocumentInfo;
            DocumentPath = that.DocumentPath;
            NeedsProcessing = that.NeedsProcessing;
        }

        [XmlIgnore] public FileInfo DocumentInfo { get; set; }
        public string DocumentPath { get; set; }

        private bool _needsProcessing;
        public bool NeedsProcessing
        {
            get { return _needsProcessing; }
            set { _needsProcessing = value; NotifyOfPropertyChange(() => NeedsProcessing); }
        }

        public virtual string this[string columnName]
        {
            //TODO: ovo sad nije bitno, sredi kasnije (error check)
            get { throw new System.NotImplementedException(); }
        }

        [XmlIgnore]
        public string Error { get; private set; }
    }
}
