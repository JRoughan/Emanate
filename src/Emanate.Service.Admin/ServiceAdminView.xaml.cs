using System;

namespace Emanate.Service.Admin
{
    public partial class ServiceAdminView
    {
        private readonly ServiceAdminViewModel viewModel;

        public ServiceAdminView()
        {
            DataContext = viewModel = new ServiceAdminViewModel();
            Initialized += ViewInitialized;
            InitializeComponent();
        }

        async void ViewInitialized(object sender, EventArgs e)
        {
            await viewModel.Initialize();
        }
    }
}
