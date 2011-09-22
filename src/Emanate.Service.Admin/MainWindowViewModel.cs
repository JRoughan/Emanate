using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Emanate.Service.Admin
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly PluginLoader pluginLoader;

        public MainWindowViewModel()
        {
            pluginLoader = new PluginLoader();
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
            var plugins = pluginLoader.Load();
            foreach (var plugin in plugins.BuildMonitorPlugins)
            {
                InputPlugins.Add(plugin);
            }
            foreach (var plugin in plugins.OutputPlugins)
            {
                OutputPlugins.Add(plugin);
            }
        }

        private ObservableCollection<PluginType> inputPlugins = new ObservableCollection<PluginType>();
        public ObservableCollection<PluginType> InputPlugins
        {
            get { return inputPlugins; }
            set
            {
                inputPlugins = value;
                OnPropertyChanged("InputPlugins");
            }
        }

        private ObservableCollection<PluginType> outputPlugins = new ObservableCollection<PluginType>();
        public ObservableCollection<PluginType> OutputPlugins
        {
            get { return outputPlugins; }
            set
            {
                outputPlugins = value;
                OnPropertyChanged("OutputPlugins");
            }
        }
    }
}
