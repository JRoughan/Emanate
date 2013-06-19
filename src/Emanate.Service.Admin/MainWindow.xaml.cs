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

        void MainWindowInitialized(object sender, EventArgs e)
        {
            viewModel.Initialize();

            if (DeviceSelector.Items.Count > 0)
                DeviceSelector.SelectedIndex = 0;

            if (ConfigSelector.Items.Count > 0)
                ConfigSelector.SelectedIndex = 0;
        }

        void ViewModelCloseRequested(object sender, EventArgs e)
        {
            Close();
        }
    }
}
