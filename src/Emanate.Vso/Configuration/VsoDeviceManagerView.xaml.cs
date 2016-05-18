﻿using System.Threading.Tasks;
using Emanate.Core.Configuration;

namespace Emanate.Vso.Configuration
{
    public partial class VsoDeviceManagerView
    {
        private VsoDeviceManagerViewModel viewModel;

        public VsoDeviceManagerView()
        {
            InitializeComponent();
        }

        public override async void SetTarget(IConfiguration moduleConfiguration)
        {
            var config = moduleConfiguration as VsoConfiguration;
            viewModel = new VsoDeviceManagerViewModel(config);
            await viewModel.Initialize();
            DataContext = viewModel;
        }
    }
}
