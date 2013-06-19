using System;
using System.Windows;
using System.Windows.Controls;

namespace Emanate.TeamCity.Configuration
{
    public partial class ConfigurationView
    {
        private TeamCityConfiguration viewModel;

        public ConfigurationView()
        {
            DataContextChanged += ConfigurationView_DataContextChanged;
            InitializeComponent();
        }

        void ConfigurationView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            viewModel = e.NewValue as TeamCityConfiguration;
            viewModel.Initialize();

            if (PasswordInput != null)
                PasswordInput.Password = viewModel.Password;
        }

        private void PasswordInputInitialized(object sender, EventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox == null)
                return;

            if (viewModel != null)
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
