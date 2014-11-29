using Caliburn.Micro;
using System.Collections.ObjectModel;
using wpfcm1.DataAccess;
using wpfcm1.Model;

namespace wpfcm1.ViewModels
{
    public class CertificatesViewModel : PropertyChangedBase
    {
        public CertificatesViewModel()
        {
            var certificateRepositiory = new CertificateRepositiory();
            Certificates = new ObservableCollection<CertificateItem>(certificateRepositiory.CertificateItems);
        }

        public ObservableCollection<CertificateItem> Certificates { get; private set; } 
    }
}
