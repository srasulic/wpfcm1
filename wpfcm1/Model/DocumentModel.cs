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
            if (Regex.IsMatch(fi.Name, @"[0-9]{9}_[0-9]{9}_.+_[0-9]{8}.+"))
            {
                string[] nameParts = fi.Name.Split('_');
                namePib1 = nameParts[0];
                namePib2 = nameParts[1];
                nameDocNum = nameParts[2];
                string datum = Regex.Match(nameParts[3], @"[0-9]{8}").ToString();
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
            set { _hasSecondSigniture = value; NotifyOfPropertyChange(() => HasSecondSigniture);  NotifyOfPropertyChange(() => IsAckedAndSigned); }
        }
        
        public virtual string this[string columnName]
        {
            //TODO: ovo sad nije bitno, sredi kasnije (error check)
            get { throw new System.NotImplementedException(); }
        }


        private String _sigSignerName;
        public String sigSignerName
        {
            get { return _sigSignerName; }
            set { _sigSignerName = value; NotifyOfPropertyChange(() => sigSignerName); }
        }


        private DateTime _sigTS;
        public DateTime sigTS
        {
            get { return _sigTS; }
            set { _sigTS = value; NotifyOfPropertyChange(() => sigTS); }
        }


        private DateTime _sigDateSigned;
        public DateTime sigDateSigned
        {
            get { return _sigDateSigned; }
            set { _sigDateSigned = value; NotifyOfPropertyChange(() => sigDateSigned); }
        }



        private String _sigOrg;
        public String sigOrg
        {
            get { return _sigOrg; }
            set { _sigOrg = value; NotifyOfPropertyChange(() => sigOrg); }
        }


        private String _sigReason;
        public String sigReason
        {
            get { return _sigReason; }
            set { _sigReason = value; NotifyOfPropertyChange(() => sigReason); }
        }

        private String _sigAdditionalInfo;
        public String sigAdditionalInfo
        {
            get { return _sigAdditionalInfo; }
            set { _sigAdditionalInfo = value; NotifyOfPropertyChange(() => sigAdditionalInfo); }
        }

        public bool ShouldSerializesigAdditionalInfo ()
        {
            return false;
        }

        private String _sigSignerName2;
        public String sigSignerName2
        {
            get { return _sigSignerName2; }
            set { _sigSignerName2 = value; NotifyOfPropertyChange(() => sigSignerName2); }
        }


        private DateTime _sigTS2;
        public DateTime sigTS2
        {
            get { return _sigTS2; }
            set { _sigTS2 = value; NotifyOfPropertyChange(() => sigTS2); }
        }

        private DateTime _sigDateSigned2;
        public DateTime sigDateSigned2
        {
            get { return _sigDateSigned2;  }
            set { _sigDateSigned2 = value; NotifyOfPropertyChange(() => sigDateSigned2); }
        }


        private String _sigOrg2;
        public String sigOrg2
        {
            get { return _sigOrg2; }
            set { _sigOrg2 = value; NotifyOfPropertyChange(() => sigOrg2); }
        }


        private String _sigReason2;
        public String sigReason2
        {
            get { return _sigReason2; }
            set { _sigReason2 = value; NotifyOfPropertyChange(() => sigReason2); }
        }


        private bool _isValidated;
        public bool isValidated
        {
            get { return _isValidated; }
            set { _isValidated = value; NotifyOfPropertyChange(() => isValidated); }
        }

        private bool _isApprovedForSigning;
        public bool isApprovedForSigning
        {
            get { return _isApprovedForSigning; }
            set 
            { 
                _isApprovedForSigning = value;
                NotifyOfPropertyChange(() => isApprovedForSigning);
                NotifyOfPropertyChange(() => IsAcked_NotSigned_Approved_Proccesed);
                NotifyOfPropertyChange(() => IsAcked_NotSigned_NotApproved_Proccesed);
                NotifyOfPropertyChange(() => IsAcked_NotSigned_Approved);
                NotifyOfPropertyChange(() => IsAcked_NotSigned_NotApproved); 
            }
        }


        private String _workflowStatus;
        public String workflowStatus
        {
            get { return _workflowStatus; }
            set { _workflowStatus = value; NotifyOfPropertyChange(() => workflowStatus); }
        }


        //check sign
        public bool ShouldSerializeIsAcked_NotSigned_Approved_Proccesed()
        {
            return false;
        }

        public bool IsAcked_NotSigned_Approved_Proccesed
        {
            get { return IsAcknowledged && !HasSecondSigniture && isApprovedForSigning && Processed; }
            set { }
        }

        public bool ShouldSerializeIsAcked_NotSigned_Approved()
        {
            return false;
        }

        public bool IsAcked_NotSigned_Approved
        {
            get { return IsAcknowledged && !HasSecondSigniture && isApprovedForSigning; }
            set { }
        }

        //remove sign
        public bool ShouldSerializeIsAcked_NotSigned_NotApproved_Proccesed()
        {
            return false;
        }

        public bool IsAcked_NotSigned_NotApproved_Proccesed
        {
            get { return IsAcknowledged && !HasSecondSigniture && !isApprovedForSigning && Processed; }
            set { }
        }

        public bool ShouldSerializeIsAcked_NotSigned_NotApproved()
        {
            return false;
        }

        public bool IsAcked_NotSigned_NotApproved
        {
            get { return IsAcknowledged && !HasSecondSigniture && !isApprovedForSigning ; }
            set { }
        }

        //lock sign
        public bool ShouldSerializeIsAckedAndSigned()
        {
            return false;
        }

        public bool IsAckedAndSigned
        {
            get { return IsAcknowledged && HasSecondSigniture; }
            set { }
        }

        [XmlIgnore]
        public string Error { get; private set; }
    }
}
