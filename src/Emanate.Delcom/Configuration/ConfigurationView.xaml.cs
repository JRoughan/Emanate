namespace Emanate.Delcom.Configuration
{
    public partial class ConfigurationView
    {
        private DelcomConfigurationViewModel viewModel;

        public ConfigurationView()
        {
            InitializeComponent();
        }

        public override void SetTarget(Core.Configuration.IModuleConfiguration moduleConfiguration)
        {
            var config = moduleConfiguration as DelcomConfiguration;
            viewModel = new DelcomConfigurationViewModel(config);
            viewModel.Initialize();
        }
    }
}
