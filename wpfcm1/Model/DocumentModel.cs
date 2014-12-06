using Caliburn.Micro;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace wpfcm1.Model
{
    public class DocumentModel : PropertyChangedBase, IDataErrorInfo
    {
        public DocumentModel()
        {
        }

        public DocumentModel(FileInfo fi)
        {
            DocumentInfo = fi;
        }

        [XmlIgnore] public FileInfo DocumentInfo { get; set; }

        public string DocumentPath { get { return DocumentInfo.FullName; } }

        //TODO: zameni sa IsValid??? neka ostane da moze da prikaze npr. nove fajlove u drugim dir
        private bool _processed;
        public bool Processed
        {
            get { return _processed; }
            set { _processed = value; NotifyOfPropertyChange(() => Processed); }
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
