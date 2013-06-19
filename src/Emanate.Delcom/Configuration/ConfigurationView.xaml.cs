using System.Windows;

namespace Emanate.Delcom.Configuration
{
    public partial class ConfigurationView
    {
        private DelcomConfiguration viewModel;

        public ConfigurationView()
        {
            DataContextChanged += ConfigurationView_DataContextChanged;
            InitializeComponent();
        }

        void ConfigurationView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            viewModel = e.NewValue as DelcomConfiguration;
            viewModel.Initialize();
        }
    }
}
