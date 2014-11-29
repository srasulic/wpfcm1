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

        public DocumentItem(FileInfo fi)
        {
            DocumentInfo = fi;
        }

        [XmlIgnore] public FileInfo DocumentInfo { get; set; }

        public string DocumentPath { get { return DocumentInfo.FullName; } }

        private bool _needsProcessing;
        public bool NeedsProcessing
        {
            get { return _needsProcessing; }
            set { _needsProcessing = value; NotifyOfPropertyChange(() => NeedsProcessing); }
        }

        private bool _isChecked;
        [XmlIgnore]
        public bool IsChecked
        {
            get { return _isChecked; }
            set { _isChecked = value; NotifyOfPropertyChange(() => IsChecked); }
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
