namespace Emanate.Delcom.Configuration
{
    public partial class DelcomDeviceManagerView
    {
        private DelcomDeviceManagerViewModel viewModel;

        public DelcomDeviceManagerView()
        {
            InitializeComponent();
        }

        public override async void SetTarget(Core.Configuration.IOutputConfiguration moduleConfiguration)
        {
            var config = moduleConfiguration as DelcomConfiguration;
            viewModel = new DelcomDeviceManagerViewModel(config);
            await viewModel.Initialize();
            DataContext = viewModel;
        }
    }
}
