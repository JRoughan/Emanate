using System;
using System.Windows;
using System.Windows.Controls;

namespace Emanate.TeamCity.Admin.Configuration
{
    public partial class InputConfigurationControl
    {
        private TeamCityDeviceViewModel viewModel;

        public InputConfigurationControl()
        {
            DataContextChanged += InputConfigurationControl_DataContextChanged;
            InitializeComponent();
        }

        private void InputConfigurationControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            viewModel = (TeamCityDeviceViewModel)e.NewValue;
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
