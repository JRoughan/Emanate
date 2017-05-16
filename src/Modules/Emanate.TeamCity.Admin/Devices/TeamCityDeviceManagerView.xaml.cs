using Emanate.Core.Configuration;

namespace Emanate.TeamCity.Admin.Devices
{
    public partial class TeamCityDeviceManagerView
    {
        private TeamCityDeviceManagerViewModel viewModel;

        public TeamCityDeviceManagerView()
        {
            InitializeComponent();
        }

        public override async void SetTarget(IConfiguration moduleConfiguration)
        {
            var config = moduleConfiguration as TeamCityConfiguration;
            viewModel = new TeamCityDeviceManagerViewModel(config);
            await viewModel.Initialize();
            DataContext = viewModel;
        }
    }
}
