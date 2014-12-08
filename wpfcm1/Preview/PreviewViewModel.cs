using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace wpfcm1.Preview
{
    public interface IPreview { }

    [Export(typeof(IPreview)), PartCreationPolicy(CreationPolicy.Shared)]
    public class PreviewViewModel : PropertyChangedBase
    {
        public PreviewViewModel()
        {
            
        }
    }
}
