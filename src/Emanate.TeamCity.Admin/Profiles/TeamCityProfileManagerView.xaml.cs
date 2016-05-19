using System.Threading.Tasks;
using Emanate.Core.Configuration;

namespace Emanate.TeamCity.Admin.Profiles
{
    public partial class TeamCityProfileManagerView
    {
        private TeamCityProfileManagerViewModel viewModel;

        public TeamCityProfileManagerView()
        {
            InitializeComponent();
        }

        public override async Task SetTarget(IConfiguration moduleConfiguration)
        {
            viewModel = new TeamCityProfileManagerViewModel();
            await viewModel.Initialize();
            DataContext = viewModel;
        }
    }
}
