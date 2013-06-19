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
        }

        void ViewModelCloseRequested(object sender, EventArgs e)
        {
            Close();
        }
    }
}
