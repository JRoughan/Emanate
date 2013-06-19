using System;
using System.Windows;
using System.Windows.Controls;

namespace Emanate.TeamCity.Configuration
{
    public partial class ConfigurationView
    {
        private readonly ConfigurationViewModel viewModel;

        public ConfigurationView()
        {
            Initialized += ConfigurationView_Initialized;
        }

        void ConfigurationView_Initialized(object sender, EventArgs e)
        {
            //PasswordInput.PasswordChanged += PasswordInput_PasswordChanged;
        }

        void PasswordInput_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            viewModel.Password = PasswordInput.SecurePassword.ToString();
        }

        public ConfigurationView(ConfigurationViewModel viewModel)
            :this()
        {
            DataContext = this.viewModel = viewModel;
            Initialized += ViewInitialized;
            InitializeComponent();
        }

        void ViewInitialized(object sender, EventArgs e)
        {
            viewModel.Initialize();
        }

        private void PasswordInputInitialized(object sender, EventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox == null)
                return;

            passwordBox.Password = viewModel.Password;
            passwordBox.PasswordChanged += passwordBox_PasswordChanged;
        }

        void passwordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox == null)
                return;


            viewModel.Password = passwordBox.Password;
        }
    }
}
