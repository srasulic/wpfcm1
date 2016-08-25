using Caliburn.Micro;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System;

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
            DocumentPath = fi.FullName; //za serijalizaciju
            double len = fi.Length;
            LengthKB = System.Math.Round(len / 1024);
            // ovo nece raditi za Generated fajlove jer oni jos nemaju ime po konvenciji
            if ( Regex.IsMatch(fi.Name, @"[0-9]{9}_[0-9]{9}_.+_.+" ) )
            {
                string[] nameParts = fi.Name.Split('_');
                namePib1 = nameParts[0];
                namePib2 = nameParts[1];
                nameDocNum = nameParts[2];
                string datum = Regex.Match(nameParts[3], @"[0-9]{8}").ToString();
      //          nameDate = string.Concat(datum[6], datum[7], "/", datum[4], datum[5], "/", datum[0], datum[1], datum[2], datum[3]);
                nameDate = string.Concat(datum[0], datum[1], datum[2], datum[3], "-", datum[4], datum[5], "-", datum[6], datum[7]);
            }
            else
            {
                namePib1 = "";
                namePib2 = "";
                nameDocNum = "";
                nameDate = fi.LastWriteTime.ToShortDateString();
            }
        }

        [XmlIgnore] public FileInfo DocumentInfo { get; set; }

        public string DocumentPath { get; set; }

                //TODO: zameni sa IsValid??? neka ostane da moze da prikaze npr. nove fajlove u drugim dir
        private bool _processed;
        public bool Processed
        {
            get { return _processed; }
            set { _processed = value; NotifyOfPropertyChange(() => Processed); }
        }

        private bool? _isValid;
        public bool? IsValid
        {
            get { return _isValid; }
            set { _isValid = value; NotifyOfPropertyChange(() => IsValid); }
        }

        private bool _isChecked;
        [XmlIgnore]
        public bool IsChecked
        {
            get { return _isChecked; }
            set { _isChecked = value; NotifyOfPropertyChange(() => IsChecked); }
        }

        private double _lengthKB;
        public double LengthKB
        {
            get { return _lengthKB; }
            set { _lengthKB = value; NotifyOfPropertyChange(() => LengthKB); }
        }


        private string _namePib1;
        public string namePib1
        {
            get { return _namePib1; }
            set { _namePib1 = value; NotifyOfPropertyChange(() => namePib1); }
        }


        private string _namePib2;
        public string namePib2
        {
            get { return _namePib2; }
            set { _namePib2 = value; NotifyOfPropertyChange(() => namePib2); }
        }

        private string _nameDocNum;
        public string nameDocNum
        {
            get { return _nameDocNum; }
            set { _nameDocNum = value; NotifyOfPropertyChange(() => nameDocNum); }
        }

        private string _nameDate;
        public string nameDate
        {
            get { return _nameDate; }
            set { _nameDate = value; NotifyOfPropertyChange(() => nameDate); }
        }

        private bool _isAcknowledged;
        public bool IsAcknowledged
        {
            get { return _isAcknowledged; }
            set { _isAcknowledged = value; NotifyOfPropertyChange(() => IsAcknowledged); }
        }

        private bool _isSigned;
        public bool IsSigned
        {
            get { return _isSigned; }
            set { _isSigned = value; NotifyOfPropertyChange(() => IsSigned); }
        }

        private bool _isSignedAgain;
        public bool IsSignedAgain
        {
            get { return _isSignedAgain; }
            set { _isSignedAgain = value; NotifyOfPropertyChange(() => IsSignedAgain); }
        }

        private bool _hasSecondSigniture;
        public bool HasSecondSigniture
        {
            get { return _hasSecondSigniture; }
            set { _hasSecondSigniture = value; NotifyOfPropertyChange(() => HasSecondSigniture); }
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
