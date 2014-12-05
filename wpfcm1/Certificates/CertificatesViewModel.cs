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

        public void OnSelection(CertificateModel certificate)
        {
            _events.PublishOnUIThread(certificate);
        }
    }
}
