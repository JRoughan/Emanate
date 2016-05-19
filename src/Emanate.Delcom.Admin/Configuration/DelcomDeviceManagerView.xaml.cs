using Emanate.Core.Configuration;

namespace Emanate.Delcom.Admin.Configuration
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
            var config = (DelcomConfiguration)moduleConfiguration;
            viewModel = new DelcomDeviceManagerViewModel(config);
            await viewModel.Initialize();
            DataContext = viewModel;
        }
    }
}
