using System;
using System.Collections.ObjectModel;
using Emanate.Core;

namespace Emanate.Service.Admin
{
    public class MainWindowViewModel : ViewModel
    {
        private readonly PluginConfigurationStorer pluginConfigurationStorer;
        private TotalConfig totalConfig;

        public MainWindowViewModel(PluginConfigurationStorer pluginConfigurationStorer)
        {
            saveCommand = new DelegateCommand(SaveAndExit, CanFindServiceConfiguration);
            applyCommand = new DelegateCommand(SaveConfiguration, CanFindServiceConfiguration);
            cancelCommand = new DelegateCommand(OnCloseRequested);

            this.pluginConfigurationStorer = pluginConfigurationStorer;
        }

        public event EventHandler CloseRequested;


        public override void Initialize()
        {
            totalConfig = pluginConfigurationStorer.Load();
            foreach (var plugin in totalConfig.ModuleConfigurations)
            {
                Configurations.Add(plugin);
            }

            foreach (var outputDevice in totalConfig.OutputDevices)
            {
                ActiveDevices.Add(outputDevice);
            }
        }

        private ObservableCollection<ConfigurationInfo> configurations = new ObservableCollection<ConfigurationInfo>();
        public ObservableCollection<ConfigurationInfo> Configurations
        {
            get { return configurations; }
            private set
            {
                configurations = value;
                OnPropertyChanged("Configurations");
            }
        }

        private ObservableCollection<OutputDeviceInfo> activeDevices = new ObservableCollection<OutputDeviceInfo>();
        public ObservableCollection<OutputDeviceInfo> ActiveDevices
        {
            get { return activeDevices; }
            private set
            {
                activeDevices = value;
                OnPropertyChanged("ActiveDevices");
            }
        }
        
        private readonly DelegateCommand saveCommand;
        public DelegateCommand SaveCommand { get { return saveCommand; } }

        private void SaveAndExit()
        {
            SaveConfiguration();
            OnCloseRequested();
        }

        private readonly DelegateCommand applyCommand;
        public DelegateCommand ApplyCommand { get { return applyCommand; } }

        private void SaveConfiguration()
        {
            totalConfig.ModuleConfigurations.Clear();
            totalConfig.ModuleConfigurations.AddRange(Configurations);
            pluginConfigurationStorer.Save(totalConfig);
        }

        private bool CanFindServiceConfiguration()
        {
            return true; // serviceIsInstalled;
        }

        private readonly DelegateCommand cancelCommand;
        public DelegateCommand CancelCommand { get { return cancelCommand; } }

        private void OnCloseRequested()
        {
            var handler = CloseRequested;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}
