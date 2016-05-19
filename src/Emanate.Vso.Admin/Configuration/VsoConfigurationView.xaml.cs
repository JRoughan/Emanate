using System.Threading.Tasks;
using Emanate.Core.Configuration;

namespace Emanate.Vso.Admin.Configuration
{
    public partial class VsoConfigurationView
    {
        private VsoConfigurationViewModel viewModel;

        public VsoConfigurationView()
        {
            InitializeComponent();
        }

        public override async Task SetTarget(IConfiguration moduleConfiguration)
        {
            viewModel = new VsoConfigurationViewModel();
            await viewModel.Initialize();
            DataContext = viewModel;
        }
    }
}
