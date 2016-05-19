using System.Threading.Tasks;
using Emanate.Core.Configuration;
using Emanate.Vso.Configuration;

namespace Emanate.Vso.Admin.Configuration
{
    public partial class ConfigurationView
    {
        private VsoConfigurationViewModel viewModel;

        public ConfigurationView()
        {
            InitializeComponent();
        }

        public override async Task SetTarget(IConfiguration moduleConfiguration)
        {
            viewModel = new VsoConfigurationViewModel(moduleConfiguration as VsoConfiguration);
            await viewModel.Initialize();
            DataContext = viewModel;
        }
    }
}
