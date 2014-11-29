using System.Xml.Serialization;

namespace wpfcm1.Model
{
    public class InboxDocumentItem : DocumentItem
    {
        public InboxDocumentItem()
        {
        }

        public InboxDocumentItem(string path) : base(path)
        {
        }

        public InboxDocumentItem(InboxDocumentItem that) : base(that)
        {
            IsValid = that.IsValid;
        }

        public InboxDocumentItem(DocumentItem that) : base(that)
        {
        }

        private bool _isChecked;
        [XmlIgnore]
        public bool IsChecked
        {
            get { return _isChecked; }
            set { NotifyOfPropertyChange(() => IsChecked); }
        }

        private bool? _isValid;
        public bool? IsValid
        {
            get { return _isValid; }
            set { NotifyOfPropertyChange(() => IsValid); }
        }

        private bool _isAcknowledged;
        public bool IsAcknowledged
        {
            get { return _isAcknowledged; }
            set { NotifyOfPropertyChange(() => IsAcknowledged); }
        }
    }
}
