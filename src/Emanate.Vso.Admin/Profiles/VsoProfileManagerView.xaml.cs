using System.Threading.Tasks;
using Emanate.Core.Configuration;
using Emanate.Extensibility.Composition;

namespace Emanate.Vso.Admin.Profiles
{
    public partial class VsoProfileManagerView
    {
        private VsoProfileManagerViewModel viewModel;

        public VsoProfileManagerView()
        {
            InitializeComponent();
        }

        public override async Task SetTarget(IConfiguration moduleConfiguration, IMediator mediator)
        {
            viewModel = new VsoProfileManagerViewModel();
            await viewModel.Initialize();
            DataContext = viewModel;
        }
    }
}
