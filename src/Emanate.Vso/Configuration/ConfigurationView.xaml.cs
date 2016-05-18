using System;
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

        public override async Task SetTarget(IConfiguration moduleConfiguration)
        {
            viewModel = new VsoConfigurationViewModel(moduleConfiguration as VsoConfiguration);
            await viewModel.Initialize();
            DataContext = viewModel;
        }
    }
}
