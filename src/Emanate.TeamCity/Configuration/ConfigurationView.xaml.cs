using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Emanate.Core.Configuration;

namespace Emanate.TeamCity.Configuration
{
    public partial class ConfigurationView
    {
        private TeamCityConfigurationViewModel viewModel;

        public ConfigurationView()
        {
            InitializeComponent();
        }

        public override async Task SetTarget(IOutputConfiguration moduleConfiguration)
        {
            viewModel = new TeamCityConfigurationViewModel(moduleConfiguration as TeamCityConfiguration);
            await viewModel.Initialize();
            PasswordInput.Password = viewModel.Password;
            DataContext = viewModel;
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
