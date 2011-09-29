using System;
using System.Windows;
using System.Windows.Controls;

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

        private void PasswordInputInitialized(object sender, EventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox == null)
                return;

            var configProperty = passwordBox.Tag as ConfigurationProperty;
            if (configProperty == null)
                return;

            passwordBox.Password = configProperty.Value as string;
            passwordBox.PasswordChanged += passwordBox_PasswordChanged;
        }

        void passwordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox == null)
                return;

            var configProperty = passwordBox.Tag as ConfigurationProperty;
            if (configProperty == null)
                return;

            configProperty.Value = passwordBox.Password;
        }
    }
}
