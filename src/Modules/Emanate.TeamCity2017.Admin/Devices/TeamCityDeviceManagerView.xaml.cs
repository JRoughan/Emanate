using Emanate.Core.Configuration;

namespace Emanate.TeamCity2017.Admin.Devices
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
            var config = moduleConfiguration as TeamCity2017Configuration;
            viewModel = new TeamCityDeviceManagerViewModel(config);
            await viewModel.Initialize();
            DataContext = viewModel;
        }
    }
}
