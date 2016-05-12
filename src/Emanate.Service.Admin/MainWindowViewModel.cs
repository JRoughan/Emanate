using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Autofac;
using Emanate.Core.Configuration;
using Emanate.Core.Output;
using Emanate.Extensibility;

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

            var outputDevices = new List<IOutputDevice>(globalConfig.OutputDevices);

            foreach (var moduleConfig in globalConfig.OututConfigurations)
            {
                var configurationEditor = componentContext.ResolveKeyed<ConfigurationEditor>(moduleConfig.Key);
                configurationEditor.SetTarget(moduleConfig);

                var deviceManager = componentContext.ResolveKeyed<DeviceManager>(moduleConfig.Key);
                deviceManager.SetTarget(moduleConfig);

                var moduleConfigInfo = new ConfigurationInfo(moduleConfig.Name, configurationEditor, deviceManager);
                Configurations.Add(moduleConfigInfo);

                var unconfiguredDevices = moduleConfig.OutputDevices.Where(d => !outputDevices.Any(od => od.Key == d.Key));
                globalConfig.OutputDevices.AddRange(unconfiguredDevices);

                moduleConfig.OutputDeviceAdded += AddOutputDevice;
                moduleConfig.OutputDeviceRemoved += RemoveOutputDevice;
            }

            foreach (var outputDevice in globalConfig.OutputDevices)
            {
                var moduleConfiguration = globalConfig.OututConfigurations.SingleOrDefault(c => c.Key.Equals(outputDevice.Type, StringComparison.OrdinalIgnoreCase));
                AddActiveDevice(moduleConfiguration, outputDevice);
            }
        }

        private void AddOutputDevice(object sender, OutputDeviceEventArgs e)
        {
            AddActiveDevice(e.ModuleConfiguration, e.OutputDevice);
        }

        private void RemoveOutputDevice(object sender, OutputDeviceEventArgs e)
        {
            var deviceToRemove = ActiveDevices.SingleOrDefault(d => d.Name == e.OutputDevice.Name);
            if (deviceToRemove != null)
                ActiveDevices.Remove(deviceToRemove);
        }

        private void AddActiveDevice(IOutputConfiguration moduleConfiguration, IOutputDevice outputDevice)
        {
            var outputDeviceInfo = new OutputDeviceInfo(outputDevice.Name, outputDevice, moduleConfiguration);

            // HACK: Force an input for a new device without any. Ugly!
            if (!outputDevice.Inputs.Any())
            {
                var inputSelector = componentContext.ResolveKeyed<InputSelector>("vso");
                outputDeviceInfo.InputSelector = inputSelector;
            }

            foreach (var inputGroup in outputDevice.Inputs.GroupBy(i => i.Source))
            {
                var inputSelector = componentContext.ResolveKeyed<InputSelector>(inputGroup.Key);
                inputSelector.SelectInputs(inputGroup);
                outputDeviceInfo.InputSelector = inputSelector;
            }

            ActiveDevices.Add(outputDeviceInfo);
        }

        public ObservableCollection<ConfigurationInfo> Configurations { get; } = new ObservableCollection<ConfigurationInfo>();

        public ObservableCollection<OutputDeviceInfo> ActiveDevices { get; } = new ObservableCollection<OutputDeviceInfo>();

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
            // TODO: The respective GUIs should take care of keeping this in sync
            foreach (var deviceInfo in ActiveDevices)
            {
                var device = deviceInfo.OutputDevice;
                device.Inputs.Clear();
                device.Inputs.AddRange(deviceInfo.InputSelector.GetSelectedInputs());
            }

            try
            {
                ConfigurationCaretaker.Save(globalConfig);
            }
            catch (Exception ex)
            {
                var errorMessage = "Cannot save config file: " + ex.Message;
                Trace.TraceError(errorMessage);
                MessageBox.Show(errorMessage);
            }
        }

        private bool CanFindServiceConfiguration()
        {
            return true; // serviceIsInstalled;
        }

        private readonly DelegateCommand cancelCommand;
        public DelegateCommand CancelCommand { get { return cancelCommand; } }

        private void OnCloseRequested()
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
