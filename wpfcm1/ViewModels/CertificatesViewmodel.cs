using Caliburn.Micro;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using wpfcm1.DataAccess;
using wpfcm1.Model;

namespace wpfcm1.ViewModels
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
            Certificates = new ObservableCollection<CertificateItem>(certificateRepositiory.CertificateItems);
        }

        public ObservableCollection<CertificateItem> Certificates { get; private set; }

        public void OnSelection(CertificateItem certificate)
        {
            _events.PublishOnUIThread(certificate);
        }
    }
}
