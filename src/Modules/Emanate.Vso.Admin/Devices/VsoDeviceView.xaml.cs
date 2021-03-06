﻿using System;
using System.Windows;
using System.Windows.Controls;

namespace Emanate.Vso.Admin.Devices
{
    public partial class VsoDeviceView
    {
        private VsoDeviceViewModel viewModel;

        public VsoDeviceView()
        {
            DataContextChanged += InputConfigurationControl_DataContextChanged;
            InitializeComponent();
        }

        private void InputConfigurationControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            viewModel = (VsoDeviceViewModel)e.NewValue;
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
    }
}
