namespace Emanate.Delcom.Configuration
{
    public partial class DelcomDeviceManagerView
    {
        private DelcomDeviceManagerViewModel viewModel;

        public DelcomDeviceManagerView()
        {
            InitializeComponent();
        }

        public override void SetTarget(Core.Configuration.IModuleConfiguration moduleConfiguration)
        {
            var config = moduleConfiguration as DelcomConfiguration;
            viewModel = new DelcomDeviceManagerViewModel(config);
            viewModel.Initialize();
            DataContext = viewModel;
        }
    }
}
