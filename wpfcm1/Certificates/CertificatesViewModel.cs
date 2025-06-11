using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;

namespace wpfcm1.Certificates
{
    [Export(typeof(CertificatesViewModel))]
    public class CertificatesViewModel : PropertyChangedBase
    {
        private readonly IEventAggregator _events;

        private BindableCollection<CertificateModel> _certificates;
        public BindableCollection<CertificateModel> Certificates
        {
            get => _certificates;
            set
            {
                _certificates = value;
                NotifyOfPropertyChange(() => Certificates);
            }
        }

        private CertificateModel _certificate;
        public CertificateModel SelectedCertificate
        {
            get => _certificate;
            set
            {
                _certificate = value;
                NotifyOfPropertyChange(() => SelectedCertificate);
                OnSelectedCertificate();
            }
        }


        [ImportingConstructor]
        public CertificatesViewModel(IEventAggregator events)
        {
            _events = events;
            var certificateRepositiory = new CertificateRepositiory();

            Certificates = new BindableCollection<CertificateModel>(certificateRepositiory.CertificateItems);

            if (SelectedCertificate == null && Certificates.Any())
            {
                SelectedCertificate = Certificates.First();
            }
        }

        public void RefreshCertificateList(bool pickCertificate)
        {
            Certificates.Clear();
            var certificateRepositiory = new CertificateRepositiory(pickCertificate);

            Certificates = new BindableCollection<CertificateModel>(certificateRepositiory.CertificateItems);

            if (SelectedCertificate == null && Certificates.Any())
            {
                SelectedCertificate = Certificates.First();
            }
        }

        public void OnSelectedCertificate()
        {
            if (SelectedCertificate == null) return;
            _events.PublishOnUIThreadAsync(SelectedCertificate);
        }
    }
}
