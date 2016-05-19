using System.Threading.Tasks;
using Emanate.Core.Configuration;

namespace Emanate.Delcom.Admin.Configuration
{
    public partial class ConfigurationView
    {
        private DelcomConfigurationViewModel viewModel;

        public ConfigurationView()
        {
            Loaded += SelectInitialProfile;
            InitializeComponent();
        }

        void SelectInitialProfile(object sender, System.EventArgs e)
        {
            if (ProfileSelector.HasItems && ProfileSelector.SelectedIndex < 0)
                ProfileSelector.SelectedIndex = 0;
        }

        public override async Task SetTarget(IConfiguration moduleConfiguration)
        {
            var config = moduleConfiguration as DelcomConfiguration;
            viewModel = new DelcomConfigurationViewModel(config);
            await viewModel.Initialize();
            DataContext = viewModel;
        }
    }
}
