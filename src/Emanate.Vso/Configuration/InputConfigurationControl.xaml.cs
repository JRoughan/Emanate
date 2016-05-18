using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Emanate.Extensibility;
using Serilog;

namespace Emanate.Vso.Configuration
{
    public partial class InputConfigurationControl
    {
        private VsoDeviceInfo viewModel;

        public InputConfigurationControl()
        {
            DataContextChanged += InputConfigurationControl_DataContextChanged;
            InitializeComponent();
            TestConnectionCommand = new DelegateCommand(TestConnection, CanTestConnection);
        }

        private async void InputConfigurationControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            viewModel = (VsoDeviceInfo)e.NewValue;
            //await viewModel.Initialize();
            PasswordInput.Password = viewModel.Password;
            //DataContext = viewModel;
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

        private bool isEditable;
        public bool IsEditable
        {
            get { return isEditable; }
            set { isEditable = value; } // OnPropertyChanged(); }
        }

        private bool? isTestSuccessful;
        public bool? IsTestSuccessful
        {
            get { return isTestSuccessful; }
            set { isTestSuccessful = value; } // OnPropertyChanged(); }
        }

        public ICommand TestConnectionCommand { get; set; }

        private bool isTesting;
        private bool CanTestConnection()
        {
            return !isTesting;
        }

        private async void TestConnection()
        {
            Log.Information("=> VsoConfigurationViewModel.TestConnection");
            isTesting = true;
            IsEditable = false;
            IsTestSuccessful = null;
            var connection = new VsoConnection(viewModel);
            try
            {
                var projects = await connection.GetProjects();
                IsTestSuccessful = projects != null;
            }
            catch (Exception)
            {
                IsTestSuccessful = false;
            }
            finally
            {
                isTesting = false;
                IsEditable = true;
                Log.Information("VSO connection test " + (IsTestSuccessful.HasValue && IsTestSuccessful.Value ? "succeeded" : "failed"));
            }
        }
    }
}
