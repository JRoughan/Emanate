using Emanate.Extensibility;

namespace Emanate.Vso.Configuration
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