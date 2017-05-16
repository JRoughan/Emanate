using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emanate.Core;

namespace Emanate.TeamCity.Admin.Inputs
{
    public partial class TeamCityInputSelectorView
    {
        private TeamCityInputSelectorViewModel viewModel;

        public TeamCityInputSelectorView()
        {
            InitializeComponent();
        }

        protected override async Task SetDeviceInternal(IDevice device)
        {
            viewModel = new TeamCityInputSelectorViewModel((TeamCityDevice)device);
            await viewModel.Initialize();
            DataContext = viewModel;
        }

        public override void SelectInputs(IEnumerable<string> inputs)
        {
            viewModel.SelectInputs(inputs);
        }

        public override IEnumerable<string> GetSelectedInputs()
        {
            return viewModel?.GetSelectedInputs() ?? Enumerable.Empty<string>();
        }
    }
}
