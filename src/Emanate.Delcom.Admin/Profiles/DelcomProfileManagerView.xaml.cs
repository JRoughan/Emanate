using System.Threading.Tasks;
using Emanate.Core.Configuration;

namespace Emanate.Delcom.Admin.Profiles
{
    public partial class DelcomProfileManagerView
    {
        private DelcomProfileManagerViewModel viewModel;

        public DelcomProfileManagerView()
        {
            Loaded += SelectInitialProfile;
            InitializeComponent();
        }

        void SelectInitialProfile(object sender, System.EventArgs e)
        {
            if (ProfileSelector.HasItems && ProfileSelector.SelectedIndex < 0)
                ProfileSelector.SelectedIndex = 0;
        }

        public override async Task SetTarget(IConfiguration moduleConfiguration)
        {
            var config = moduleConfiguration as DelcomConfiguration;
            viewModel = new DelcomProfileManagerViewModel(config);
            await viewModel.Initialize();
            DataContext = viewModel;
        }
    }
}
