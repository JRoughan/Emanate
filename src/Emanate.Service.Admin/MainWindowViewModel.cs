using System;
using System.Collections.ObjectModel;
using System.Linq;
using Autofac;
using Emanate.Core.Configuration;

namespace Emanate.Service.Admin
{
    public class MainWindowViewModel : ViewModel
    {
        private readonly IComponentContext componentContext;
        private readonly ConfigurationCaretaker configurationCaretaker;
        private GlobalConfig globalConfig;

        public MainWindowViewModel(IComponentContext componentContext, ConfigurationCaretaker configurationCaretaker)
        {
            //addDeviceCommand = new DelegateCommand(AddDevice);

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

            if (globalConfig == null)
                globalConfig = GenerateDefaultConfig();

            foreach (var moduleConfig in globalConfig.ModuleConfigurations)
            {
                var configurationEditor = componentContext.ResolveKeyed<ConfigurationEditor>(moduleConfig.Key);
                configurationEditor.SetTarget(moduleConfig);

                var deviceManager = componentContext.ResolveKeyed<DeviceManager>(moduleConfig.Key);
                deviceManager.SetTarget(moduleConfig);

                var moduleConfigInfo = new ConfigurationInfo(moduleConfig.Name, configurationEditor, deviceManager);
                Configurations.Add(moduleConfigInfo);
            }

            foreach (var outputDevice in globalConfig.OutputDevices)
            {
                var moduleConfiguration = globalConfig.ModuleConfigurations.Single(c => c.Key.Equals(outputDevice.Type, StringComparison.OrdinalIgnoreCase));

                var outputDeviceInfo = new OutputDeviceInfo(outputDevice.Name, outputDevice, moduleConfiguration);
                foreach (var inputGroup in outputDevice.Inputs.GroupBy(i => i.Source))
                {
                    var inputSelector = componentContext.ResolveKeyed<InputSelector>(inputGroup.Key);
                    inputSelector.SelectInputs(inputGroup);
                    outputDeviceInfo.InputSelector = inputSelector;
                }

                ActiveDevices.Add(outputDeviceInfo);
            }
        }

        private GlobalConfig GenerateDefaultConfig()
        {
            return new GlobalConfig();
        }

        private readonly ObservableCollection<ConfigurationInfo> configurations = new ObservableCollection<ConfigurationInfo>();
        public ObservableCollection<ConfigurationInfo> Configurations
        {
            get { return configurations; }
        }

        private readonly ObservableCollection<OutputDeviceInfo> activeDevices = new ObservableCollection<OutputDeviceInfo>();
        public ObservableCollection<OutputDeviceInfo> ActiveDevices
        {
            get { return activeDevices; }
        }

        //private readonly DelegateCommand addDeviceCommand;
        //public DelegateCommand AddDeviceCommand { get { return addDeviceCommand; } }

        //private void AddDevice()
        //{
        //    var addDeviceViewModel = new AddDeviceViewModel(configurations);
        //    var addProfileView = new AddDeviceView { DataContext = addDeviceViewModel };
        //    addProfileView.Owner = Application.Current.MainWindow;
        //    addProfileView.ShowDialog();
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
            // TODO: The respective Guis should take care of keeping this in sync
            foreach (var deviceInfo in ActiveDevices)
            {
                var device = deviceInfo.OutputDevice;
                device.Inputs.Clear();
                device.Inputs.AddRange(deviceInfo.InputSelector.GetSelectedInputs());
            }

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
