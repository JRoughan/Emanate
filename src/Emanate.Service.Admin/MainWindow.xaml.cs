using System;

namespace Emanate.Service.Admin
{
    public partial class MainWindow
    {
        private readonly MainWindowViewModel viewModel;

        public MainWindow() { }

        public MainWindow(MainWindowViewModel viewModel)
        {
            DataContext = this.viewModel = viewModel;
            viewModel.CloseRequested += ViewModelCloseRequested;
            Initialized += MainWindowInitialized;
            InitializeComponent();
        }

        async void MainWindowInitialized(object sender, EventArgs e)
        {
            await viewModel.Initialize();

            if (DeviceSelector.Items.Count > 0)
                DeviceSelector.SelectedIndex = 0;

            if (ModuleProfilesSelector.Items.Count > 0)
                ModuleProfilesSelector.SelectedIndex = 0;

            if (ModuleDevicesSelector.Items.Count > 0)
                ModuleDevicesSelector.SelectedIndex = 0;
        }

        void ViewModelCloseRequested(object sender, EventArgs e)
        {
            Close();
        }
    }
}
