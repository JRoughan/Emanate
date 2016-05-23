using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using Emanate.Core.Configuration;
using Emanate.Core.Output;
using Emanate.Extensibility;
using Serilog;

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


        public override async Task<InitializationResult> Initialize()
        {
            globalConfig = await configurationCaretaker.Load();

            var outputDevices = new List<IOutputDevice>(globalConfig.OutputDevices);

            foreach (var outputModule in globalConfig.OutputModules)
            {
                var moduleConfig = globalConfig.OutputConfigurations.SingleOrDefault(c => c.Key == outputModule.Key);
                if (moduleConfig == null)
                {
                    moduleConfig = (IOutputConfiguration)outputModule.GenerateDefaultConfig();
                    globalConfig.OutputConfigurations.Add(moduleConfig);
                }

                var profileManager = componentContext.ResolveKeyed<ProfileManager>(moduleConfig.Key);
                await profileManager.SetTarget(moduleConfig);

                var deviceManager = componentContext.ResolveKeyed<DeviceManager>(moduleConfig.Key);
                deviceManager.SetTarget(moduleConfig);

                var moduleViewModel = new ModuleViewModel(moduleConfig.Name, profileManager, deviceManager, null);
                Modules.Add(moduleViewModel);

                var unconfiguredDevices = moduleConfig.OutputDevices.Where(d => !outputDevices.Any(od => od.Id == d.Id));
                globalConfig.OutputDevices.AddRange(unconfiguredDevices);

                moduleConfig.OutputDeviceAdded += AddOutputDevice;
                moduleConfig.OutputDeviceRemoved += RemoveOutputDevice;
            }

            foreach (var inputModule in globalConfig.InputModules)
            {
                var moduleConfig = globalConfig.InputConfigurations.SingleOrDefault(c => c.Key == inputModule.Key);
                if (moduleConfig == null)
                {
                    moduleConfig = (IInputConfiguration)inputModule.GenerateDefaultConfig();
                    globalConfig.InputConfigurations.Add(moduleConfig);
                }

                var profileManager = componentContext.ResolveKeyed<ProfileManager>(moduleConfig.Key);
                await profileManager.SetTarget(moduleConfig);

                var deviceManager = componentContext.ResolveKeyed<DeviceManager>(moduleConfig.Key);
                deviceManager.SetTarget(moduleConfig);

                var inputSelector = componentContext.ResolveKeyed<InputSelector>(moduleConfig.Key);

                var moduleConfigInfo = new ModuleViewModel(moduleConfig.Name, profileManager, deviceManager, inputSelector);
                Modules.Add(moduleConfigInfo);
            }

            foreach (var outputDevice in globalConfig.OutputDevices)
            {
                var moduleConfiguration =
                    globalConfig.OutputConfigurations.SingleOrDefault(
                        c => c.Key.Equals(outputDevice.Type, StringComparison.OrdinalIgnoreCase));
                AddActiveDevice(moduleConfiguration, outputDevice);
            }
            
            return InitializationResult.Succeeded;
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
            var outputDeviceInfo = new DeviceViewModel(outputDevice, moduleConfiguration);

            //// HACK: Force an input for a new device without any. Ugly!
            //if (!outputDevice.Inputs.Any())
            //{
            //    var inputSelector = componentContext.ResolveKeyed<InputSelector>("vso");
            //    inputSelector.Device = globalConfig.InputDevices.Single(); // Break if more than one to encourage me to handle the scenario
            //    outputDeviceInfo.InputSelector = inputSelector;
            //}

            //foreach (var inputGroup in outputDevice.Inputs.GroupBy(i => i.Source))
            //{
            //    var inputSelector = componentContext.ResolveKeyed<InputSelector>(inputGroup.Key);
            //    inputSelector.SelectInputs(inputGroup);
            //    outputDeviceInfo.InputSelector = inputSelector;
            //}

            ActiveDevices.Add(outputDeviceInfo);
        }

        public ObservableCollection<ModuleViewModel> Modules { get; } = new ObservableCollection<ModuleViewModel>();

        public ObservableCollection<DeviceViewModel> ActiveDevices { get; } = new ObservableCollection<DeviceViewModel>();

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
            globalConfig.Mappings.Clear();
            // TODO: The respective GUIs should take care of keeping this in sync
            foreach (var deviceInfo in ActiveDevices)
            {
                var mapping = new Mapping();
                mapping.OutputId = deviceInfo.OutputDevice.Id;
                foreach (var selectedInput in deviceInfo.InputSelector.GetSelectedInputs())
                {
                    mapping.Inputs.Add(selectedInput);
                };
                globalConfig.Mappings.Add(mapping);
            }

            try
            {
                ConfigurationCaretaker.Save(globalConfig);
            }
            catch (Exception ex)
            {
                var errorMessage = "Cannot save config file: " + ex.Message;
                Log.Error(errorMessage);
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
