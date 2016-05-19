using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Emanate.Core.Configuration;

namespace Emanate.TeamCity.Admin.Configuration
{
    public partial class TeamCityConfigurationView
    {
        private TeamCityConfigurationViewModel viewModel;

        public TeamCityConfigurationView()
        {
            InitializeComponent();
        }

        public override async Task SetTarget(IConfiguration moduleConfiguration)
        {
            viewModel = new TeamCityConfigurationViewModel();
            await viewModel.Initialize();
            DataContext = viewModel;
        }
    }
}
