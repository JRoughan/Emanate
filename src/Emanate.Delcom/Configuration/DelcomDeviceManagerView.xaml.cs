using Emanate.Core.Configuration;

namespace Emanate.Delcom.Configuration
{
    public partial class DelcomDeviceManagerView
    {
        private DelcomDeviceManagerViewModel viewModel;

        public DelcomDeviceManagerView()
        {
            InitializeComponent();
        }

        public override async void SetTarget(IConfiguration moduleConfiguration)
        {
            var config = moduleConfiguration as DelcomConfiguration;
            viewModel = new DelcomDeviceManagerViewModel(config);
            await viewModel.Initialize();
            DataContext = viewModel;
        }
    }
}
