using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using wpfcm1.DataAccess;
using wpfcm1.Model;

namespace wpfcm1.Certificates
{
    [Export(typeof(CertificatesViewModel))]
    public class CertificatesViewModel : PropertyChangedBase
    {
        private readonly IEventAggregator _events;

        [ImportingConstructor]
        public CertificatesViewModel(IEventAggregator events)
        {
            _events = events;
            var certificateRepositiory = new CertificateRepositiory();
            Certificates = new ObservableCollection<CertificateModel>(certificateRepositiory.CertificateItems);
        }

        public ObservableCollection<CertificateModel> Certificates { get; private set; }

        // 
        private bool first = true;

        public void RefreshCertificateList()
        {
            if (!first)
            {
                Certificates.Clear();
                var certificateRepositiory = new CertificateRepositiory();
                int i = 0;
                foreach(CertificateModel cert in certificateRepositiory.CertificateItems)
                {
                    Certificates.Add(certificateRepositiory.CertificateItems[i++]);
                }            
            }
            else first = false;
        }

        public void OnSelection(CertificateModel certificate)
        {
            if (certificate == null) return;
            _events.PublishOnUIThread(certificate);
        }
    }
}
