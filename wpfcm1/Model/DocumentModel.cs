using Caliburn.Micro;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System;
using wpfcm1.DataAccess;

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
            if (Regex.IsMatch(fi.Name, @"[0-9]{9}_[0-9]{9}_.+_[0-9]{8}.+") || Regex.IsMatch(fi.Name, @"[0-9]{13}_[0-9]{13}_.+_[0-9]{8}.+"))
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


        public virtual string this[string columnName]
        {
            //TODO: ovo sad nije bitno, sredi kasnije (error check)
            get { throw new System.NotImplementedException(); }
        }

        /// <summary>
        /// 1. statusi koji se koriste prilikom rada sa kolekcijama dokumenata i opšti atributi za prikaz u listama: 
        /// </summary>

        // 1.1. koristi se za promene statusa u listama za vreme obrade dokumenta
        private bool _processed;
        public bool Processed
        {
            get { return _processed; }
            set { _processed = value; NotifyOfPropertyChange(() => Processed); }
        }


        // 1.2 koristi se samo tokom manipulacije dokumentima u listama
        private bool _isChecked;
        [XmlIgnore]
        public bool IsChecked
        {
            get { return _isChecked; }
            set { _isChecked = value; NotifyOfPropertyChange(() => IsChecked); }
        }

        // 1.3 veličina fajla u KB
        private double _lengthKB;
        public double LengthKB
        {
            get { return _lengthKB; }
            set { _lengthKB = value; NotifyOfPropertyChange(() => LengthKB); }
        }

        // 1.4 PIB 1 iz imena fajla
        private string _namePib1;
        public string namePib1
        {
            get { return _namePib1; }
            set { _namePib1 = value; NotifyOfPropertyChange(() => namePib1); }
        }

        // 1.5 Pib 2 iz imena fajla
        private string _namePib2;
        public string namePib2
        {
            get { return _namePib2; }
            set { _namePib2 = value; NotifyOfPropertyChange(() => namePib2); }
        }

        // 1.6 broj dokumenta iz imena fajla
        private string _nameDocNum;
        public string nameDocNum
        {
            get { return _nameDocNum; }
            set { _nameDocNum = value; NotifyOfPropertyChange(() => nameDocNum); }
        }

        // 1.7 datum IZ IMENA fajla, treba nam jer se ftp-om menja modify date svakim prenosom
        private string _nameDate;
        public string nameDate
        {
            get { return _nameDate; }
            set { _nameDate = value; NotifyOfPropertyChange(() => nameDate); }
        }

        // 1.8 INBOUND INBOX - da li je poslata potvrda uručenja (ack fajl)
        private bool _isAcknowledged;
        public bool IsAcknowledged
        {
            get { return _isAcknowledged; }
            set { _isAcknowledged = value; NotifyOfPropertyChange(() => IsAcknowledged); }
        }

        // 1.9 INBOUND inbox ili confirmedToDo - status true ako se na _s dokument stavi jos jedan potpis (izgenerise se fajl _s_s)
        //     Po pravilu ovakav fajl vise ne prikazujemo u listi jer imamo njegovu novu verziju _s_s
        private bool _isSignedAgain;
        public bool IsSignedAgain
        {
            get { return _isSignedAgain; }
            set { _isSignedAgain = value; NotifyOfPropertyChange(() => IsSignedAgain); }
        }

        // 1.10 po pravilu svaki _s_s fajl ima ovaj atribut true. 
        private bool _hasSecondSignature;
        public bool HasSecondSignature
        {
            get { return _hasSecondSignature; }
            set
            {
                _hasSecondSignature = value;
                NotifyOfPropertyChange(() => HasSecondSignature);
                NotifyOfPropertyChange(() => IsValid_HasSS_IsValidated_IsNotValidated2);
            }
        }

        // 1.11 naziv primaoca izvucen po pib-u
        private string _namePib2Name;
        public string namePib2Name
        {
            get
            {
                return _namePib2Name;
            }
            set { _namePib2Name = value; NotifyOfPropertyChange(() => _namePib2Name); }
        }

        // 1.12 naziv izdavaoca izvucen po pib-u
        private string _namePib1Name;
        public string namePib1Name
        {
            get
            {
                return _namePib1Name;
            }
            set { _namePib1Name = value; NotifyOfPropertyChange(() => _namePib1Name); }
        }

        // 1.13 za Outbox - da prikažemo tipove poruka koje se šalju
        private string _tipDok;
        [XmlIgnore]
        public string tipDok
        {
            get
            {
                if (String.IsNullOrEmpty(_tipDok))
                {
                    if (Regex.IsMatch(DocumentPath, @".+.pdf.ack$", RegexOptions.IgnoreCase)) tipDok = "Potvrda prijema";
                    else if (Regex.IsMatch(DocumentPath, @".+.pdf.xml$", RegexOptions.IgnoreCase)) tipDok = "Poruka za server (promena statusa dokumenta)";
                    else if (Regex.IsMatch(DocumentPath, @".+.synchstamp$", RegexOptions.IgnoreCase)) tipDok = "Interna poruka - kraj sinhronizacije";
                    else tipDok = "Dokument"; //neka vrsta default vrednosti
                }
                return _tipDok; }
            set { _tipDok = value; NotifyOfPropertyChange(() => tipDok); }
        }

        // 1.14 WaitForServerProcessing - privremena osobina za dokumente u INBOX-u - nakon sinhronizacije treba da im se promenin neki vidljivi status, 
        //      da se zna da čekaju na serversku obradu
        private bool _waitForServerProcessing;
        public bool WaitForServerProcessing
        {
            get { return _waitForServerProcessing; }
            set { _waitForServerProcessing = value; NotifyOfPropertyChange(() => WaitForServerProcessing); }
        }

        /// <summary>
        /// 2. Polja za vrednosti izvučene iz potpisa 1 i potpisa 2 
        /// </summary>

        // 2.1.1 potpis 1 - ime potpisnika
        private String _sigSignerName;
        public String sigSignerName
        {
            get { return _sigSignerName; }
            set { _sigSignerName = value; NotifyOfPropertyChange(() => sigSignerName); }
        }

        // 2.1.2. potpis 1 - vreme iz time stampa
        private DateTime _sigTS;
        public DateTime sigTS
        {
            get { return _sigTS; }
            set { _sigTS = value; NotifyOfPropertyChange(() => sigTS); }
        }

        // 2.1.3. potpis 1 - vreme iz DateSigned polja 
        private DateTime _sigDateSigned;
        public DateTime sigDateSigned
        {
            get { return _sigDateSigned; }
            set { _sigDateSigned = value; NotifyOfPropertyChange(() => sigDateSigned); }
        }

        // 2.1.4. potpis 1 - organizacija na koju glasi sertifikat
        private String _sigOrg;
        public String sigOrg
        {
            get { return _sigOrg; }
            set { _sigOrg = value; NotifyOfPropertyChange(() => sigOrg); }
        }

        // 2.1.5. potpis 1 - reason
        private String _sigReason;
        public String sigReason
        {
            get { return _sigReason; }
            set { _sigReason = value; NotifyOfPropertyChange(() => sigReason); }
        }

        // 2.2.1. potpis 2 - ime potpisnika
        private String _sigSignerName2;
        public String sigSignerName2
        {
            get { return _sigSignerName2; }
            set { _sigSignerName2 = value; NotifyOfPropertyChange(() => sigSignerName2); }
        }

        // 2.2.2. potpis 2 - vreme iz time stampa
        private DateTime _sigTS2;
        public DateTime sigTS2
        {
            get { return _sigTS2; }
            set { _sigTS2 = value; NotifyOfPropertyChange(() => sigTS2); }
        }

        // 2.2.3. potpis 2 - vreme iz DateSigned polja 
        private DateTime _sigDateSigned2;
        public DateTime sigDateSigned2
        {
            get { return _sigDateSigned2; }
            set { _sigDateSigned2 = value; NotifyOfPropertyChange(() => sigDateSigned2); }
        }

        // 2.2.4. potpis 2 - organizacija na koju glasi sertifikat
        private String _sigOrg2;
        public String sigOrg2
        {
            get { return _sigOrg2; }
            set { _sigOrg2 = value; NotifyOfPropertyChange(() => sigOrg2); }
        }

        // 2.2.5. potpis 2 - reason
        private String _sigReason2;
        public String sigReason2
        {
            get { return _sigReason2; }
            set { _sigReason2 = value; NotifyOfPropertyChange(() => sigReason2); }
        }

        // 2.3. namenjeno da se prikaze overview o svim potpisima, npr za tooltip 
        //      setter je tu samo da uradi notify property change
        public String sigAdditionalInfo
        {
            get
            { //return _sigAdditionalInfo;

                string strdateTS;
                string strdateDate;
                string strdateTS2;
                string strdateDate2;
                if (sigTS == DateTime.MinValue) strdateTS = ""; else strdateTS = sigTS.ToShortDateString();
                if (sigDateSigned == DateTime.MinValue) strdateDate = ""; else strdateDate = sigDateSigned.ToShortDateString();
                if (sigTS2 == DateTime.MinValue) strdateTS2 = ""; else strdateTS2 = sigTS2.ToShortDateString();
                if (sigDateSigned2 == DateTime.MinValue) strdateDate2 = ""; else strdateDate2 = sigDateSigned2.ToShortDateString();
                return String.Format(@"{11}{0}* Potpis 1:{0}Reason: {1}{0}Potpisao: {2}, {3} {0}TimeStamp datum: {4} {0}Datum potpisa: {5} {0}{0}** Potpis 2:{0}Reason: {6}{0}Potpisao: {7}, {8}{0}TimeStamp datum: {9}{0}Datum potpisa: {10} ",
                                    Environment.NewLine,
                                    sigReason, sigSignerName, sigOrg, strdateTS, strdateDate,
                                    sigReason2, sigSignerName2, sigOrg2, strdateTS2, strdateDate2,
                                    sigValidationInfo
                                    );
            }
            set { if (value == "refresh") NotifyOfPropertyChange(() => sigAdditionalInfo); }
        }

        public bool ShouldSerializesigAdditionalInfo()
        {
            return false;
        }
        /// <summary>
        /// 3. Statusi prilikom procesa validacije potpisa u dokumentu :
        /// </summary>

        // 3.1. rezultat validacije svih potpisa u dokumentu
        private bool? _isValid;
        public bool? IsValid
        {
            get { return _isValid; }
            set { _isValid = value; NotifyOfPropertyChange(() => IsValid); }
        }

        // 3.2. potpis 1 - da li je vrsena validacija potpisa 1
        private bool _isValidated;
        public bool isValidated
        {
            get { return _isValidated; }
            set
            {
                _isValidated = value;
                NotifyOfPropertyChange(() => isValidated);
                NotifyOfPropertyChange(() => IsValid_HasSS_IsValidated_IsNotValidated2);
                NotifyOfPropertyChange(() => IsValid_HasSS_IsValidated2);
                NotifyOfPropertyChange(() => IsValid_HasNotSS);
            }
        }

        // 3.3. potpis 2 - da li je vrsena validacija potpisa 2
        private bool _isValidated2;
        public bool isValidated2
        {
            get { return _isValidated2; }
            set { _isValidated2 = value; NotifyOfPropertyChange(() => isValidated2); }
        }
        // 3.4. ukoliko je bilo gresaka prilikom validacije ovde treba smestiti info o njima 
        private String _sigValidationInfo;
        public String sigValidationInfo
        {
            get { return _sigValidationInfo; }
            set { _sigValidationInfo = value; NotifyOfPropertyChange(() => sigValidationInfo); }
        }


        /// <summary>
        /// 4. slede statusi za dorade za procese odobravanja
        /// </summary>
        /// 

        // 4.1. interna poruka - odobren ili ne
        private bool _isApprovedForProcessing;
        public bool isApprovedForProcessing
        {
            get { return _isApprovedForProcessing; }
            set
            {
                _isApprovedForProcessing = value;
                NotifyOfPropertyChange(() => isApprovedForProcessing);
                NotifyOfPropertyChange(() => NotSignedAgain_Approved);
            }
        }

        // 4.2. ne koristi se
        private String _workflowStatus;
        public String workflowStatus
        {
            get { return _workflowStatus; }
            set { _workflowStatus = value; NotifyOfPropertyChange(() => workflowStatus); }
        }

        // 4.3. Za Confirmed, pre svega outbound - oznaka da je dokument spreman za arhiviranje 
        private bool _archiveReady;
        public bool archiveReady
        {
            get { return _archiveReady; }
            set { _archiveReady = value; NotifyOfPropertyChange(() => archiveReady); }
        }

        // 4.4. odbbacen - nije za dalje procesiranje
        private bool _isRejected;
        public bool isRejected
        {
            get { return _isRejected; }
            set
            {
                _isRejected = value;
                NotifyOfPropertyChange(() => isRejected);
                NotifyOfPropertyChange(() => NotSignedAgain_Rejected);
            }
        }

        // 4.5. interna stavr, kada stigne xml oznaci dokument da ima eksternu poruku kako bi se kasnije osvezili podaci o njemu... 
        private bool _hasExternalMessage;
        public bool hasExternalMessage
        {
            get { return _hasExternalMessage; }
            set { _hasExternalMessage = value; NotifyOfPropertyChange(() => hasExternalMessage); }
        }

        /// <summary>
        /// 5. Ovo mozda treba izmestiti na neko pametnije emsto. Ovo su funkcije koe zavisno od kombinacije statusa vracaju true/false za potrebe prikaza... 
        /// </summary>
        /// <returns></returns>

        // 5.1. check sign
        public bool ShouldSerializeNotSignedAgain_Approved() { return false; }

        public bool NotSignedAgain_Approved { get { return !IsSignedAgain && isApprovedForProcessing; } set { } }


        // 5.2. remove sign
        public bool ShouldSerializeNotSignedAgain_Rejected() { return false; }

        public bool NotSignedAgain_Rejected { get { return !IsSignedAgain && isRejected; } set { } }

        // 5.3. 
        public bool ShouldSerializeIsValid_HasSS_IsValidated2() { return false; }

        public bool IsValid_HasSS_IsValidated2
        {
            get
            {
                if (!IsValid.HasValue) return false;
                return ((bool)IsValid && HasSecondSignature && isValidated2);
            }
            set { }
        }

        // 5.4. 
        public bool ShouldSerializeIsValid_HasNotSS() { return false; }

        public bool IsValid_HasNotSS
        {
            get
            {
                if (!IsValid.HasValue) return false;
                if ((bool)IsValid && !HasSecondSignature) return true;
                return false;
            }
            set { }
        }


        // 5.5. 
        public bool ShouldSerializeIsValid_HasSS_IsValidated_IsNotValidated2() { return false; }

        public bool IsValid_HasSS_IsValidated_IsNotValidated2
        {
            get
            {
                if (!IsValid.HasValue) return false;
                if ((bool)IsValid && HasSecondSignature && isValidated && !_isValidated2) return true;
                return false;
            }
            set { }
        }



        [XmlIgnore]
        public string Error { get; private set; }
    }
}
