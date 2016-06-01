using System.Threading.Tasks;
using Emanate.Core.Configuration;
using Emanate.Extensibility.Composition;

namespace Emanate.TeamCity.Admin.Profiles
{
    public partial class TeamCityProfileManagerView
    {
        private TeamCityProfileManagerViewModel viewModel;

        public TeamCityProfileManagerView()
        {
            InitializeComponent();
        }

        public override async Task SetTarget(IConfiguration moduleConfiguration, IMediator mediator)
        {
            viewModel = new TeamCityProfileManagerViewModel();
            await viewModel.Initialize();
            DataContext = viewModel;
        }
    }
}
