﻿using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Emanate.Core.Configuration;

namespace Emanate.Vso.Configuration
{
    public partial class ConfigurationView
    {
        private VsoConfigurationViewModel viewModel;

        public ConfigurationView()
        {
            InitializeComponent();
        }

        public override async Task SetTarget(IOutputConfiguration moduleConfiguration)
        {
            viewModel = new VsoConfigurationViewModel(moduleConfiguration as VsoConfiguration);
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
