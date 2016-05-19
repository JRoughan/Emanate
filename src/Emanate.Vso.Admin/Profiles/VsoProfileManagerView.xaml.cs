using System.Threading.Tasks;
using Emanate.Core.Configuration;

namespace Emanate.Vso.Admin.Profiles
{
    public partial class VsoProfileManagerView
    {
        private VsoProfileManagerViewModel viewModel;

        public VsoProfileManagerView()
        {
            InitializeComponent();
        }

        public override async Task SetTarget(IConfiguration moduleConfiguration)
        {
            viewModel = new VsoProfileManagerViewModel();
            await viewModel.Initialize();
            DataContext = viewModel;
        }
    }
}
