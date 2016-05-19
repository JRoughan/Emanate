using Emanate.Extensibility;
using Emanate.Vso.Configuration;

namespace Emanate.Vso.Admin
{
    public class VsoConfigurationViewModel : ViewModel
    {
        private readonly VsoConfiguration vsoConfiguration;

        public VsoConfigurationViewModel(VsoConfiguration vsoConfiguration)
        {
            this.vsoConfiguration = vsoConfiguration;
        }
    }
}