using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Emanate.Service.Admin
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly PluginConfigurationLoader pluginConfigurationLoader;

        public MainWindowViewModel()
        {
            pluginConfigurationLoader = new PluginConfigurationLoader();
            ConfigurationInfos = new ObservableCollection<ConfigurationInfo>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Initialize()
        {
            foreach (var plugin in pluginConfigurationLoader.Load())
            {
                ConfigurationInfos.Add(plugin);
            }
        }

        private ObservableCollection<ConfigurationInfo> configurationInfos;
        public ObservableCollection<ConfigurationInfo> ConfigurationInfos
        {
            get { return configurationInfos; }
            private set
            {
                configurationInfos = value;
                OnPropertyChanged("ConfigurationInfos");
            }
        }
    }
}
