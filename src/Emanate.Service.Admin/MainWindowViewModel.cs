using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using Autofac;
using Emanate.Core;

namespace Emanate.Service.Admin
{
    public class MainWindowViewModel : ViewModel
    {
        private readonly IComponentContext componentContext;
        private readonly ConfigurationCaretaker configurationCaretaker;
        private GlobalConfig globalConfig;

        public MainWindowViewModel(IComponentContext componentContext, ConfigurationCaretaker configurationCaretaker)
        {
            saveCommand = new DelegateCommand(SaveAndExit, CanFindServiceConfiguration);
            applyCommand = new DelegateCommand(SaveConfiguration, CanFindServiceConfiguration);
            cancelCommand = new DelegateCommand(OnCloseRequested);

            this.componentContext = componentContext;
            this.configurationCaretaker = configurationCaretaker;
        }

        public event EventHandler CloseRequested;


        public override void Initialize()
        {
            globalConfig = configurationCaretaker.Load();
            foreach (var moduleConfig in globalConfig.ModuleConfigurations)
            {
                var gui = componentContext.ResolveKeyed<UserControl>(moduleConfig.Key + "-Config");
                var moduleConfigInfo = new ConfigurationInfo(moduleConfig.Name, gui);
                Configurations.Add(moduleConfigInfo);
            }

            foreach (var outputDevice in globalConfig.OutputDevices)
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
            configurationCaretaker.Save(globalConfig);
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
