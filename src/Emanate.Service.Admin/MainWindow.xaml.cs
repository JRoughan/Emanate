using System;
using System.Windows;

namespace Emanate.Service.Admin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly MainWindowViewModel viewModel;

        public MainWindow()
        {
            DataContext = viewModel = new MainWindowViewModel();
            Initialized += MainWindowInitialized;
            InitializeComponent();
        }

        void MainWindowInitialized(object sender, EventArgs e)
        {
            viewModel.Initialize();
        }
    }
}
