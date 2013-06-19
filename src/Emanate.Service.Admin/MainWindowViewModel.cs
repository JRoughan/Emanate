using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using Emanate.Core;

namespace Emanate.Service.Admin
{
    public class MainWindowViewModel : ViewModel
    {
        private readonly PluginConfigurationStorer pluginConfigurationStorer;
        private Foo foo;

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
            foo = pluginConfigurationStorer.Load();
            foreach (var plugin in foo.ModuleConfigurations)
            {
                Configurations.Add(plugin);
            }
        }

        private UserControl inputSelector;
        public UserControl InputSelector
        {
            get { return inputSelector; }
            set
            {
                inputSelector = value;
                OnPropertyChanged("InputSelector");
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
            foo.ModuleConfigurations.Clear();
            foo.ModuleConfigurations.AddRange(Configurations);
            pluginConfigurationStorer.Save(foo);
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
