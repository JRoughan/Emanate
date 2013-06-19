using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using Emanate.Core;
using Emanate.TeamCity;

namespace Emanate.Service.Admin
{
    class MainWindowViewModel : ViewModel
    {
        private readonly PluginConfigurationStorer pluginConfigurationStorer;

        public MainWindowViewModel()
        {
            saveCommand = new DelegateCommand(SaveAndExit, CanFindServiceConfiguration);
            applyCommand = new DelegateCommand(SaveConfiguration, CanFindServiceConfiguration);
            cancelCommand = new DelegateCommand(OnCloseRequested);

            pluginConfigurationStorer = new PluginConfigurationStorer();
            InputSelector = new InputSelector();
            //ConfigurationInfos = new ObservableCollection<ConfigurationInfo>();
        }

        public event EventHandler CloseRequested;


        public override void Initialize()
        {
            //foreach (var plugin in pluginConfigurationStorer.Load())
            //{
            //    ConfigurationInfos.Add(plugin);
            //}
        }

        private UserControl inputSelector;
        public UserControl InputSelector
        {
            get { return inputSelector; }
            private set
            {
                inputSelector = value;
                OnPropertyChanged("InputSelector");
            }
        }

        //private ObservableCollection<ConfigurationInfo> configurationInfos;
        //public ObservableCollection<ConfigurationInfo> ConfigurationInfos
        //{
        //    get { return configurationInfos; }
        //    private set
        //    {
        //        configurationInfos = value;
        //        OnPropertyChanged("ConfigurationInfos");
        //    }
        //}

        
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
            //pluginConfigurationStorer.Save(configurationInfos);
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
