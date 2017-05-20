using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Autofac.Features.Indexed;
using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Core.Output;
using Emanate.Extensibility;
using Emanate.Extensibility.Composition;
using Serilog;

namespace Emanate.Service.Admin
{
    public class MainWindowViewModel : ViewModel
    {
        private readonly ConfigurationCaretaker configurationCaretaker;
        private readonly IMediator mediator;
        private readonly IIndex<string, IInputConfiguration> inputConfigurations;
        private readonly IIndex<string, IOutputConfiguration> outputConfigurations;
        private readonly IIndex<string, ProfileManager> profileManagers;
        private readonly IIndex<string, DeviceManager> deviceManagers;
        private readonly IIndex<string, InputSelector> inputSelectors;
        private GlobalConfig globalConfig;

        public MainWindowViewModel(ConfigurationCaretaker configurationCaretaker,
            IMediator mediator,
            IIndex<string, IInputConfiguration> inputConfigurations,
            IIndex<string, IOutputConfiguration> outputConfigurations,
            IIndex<string, ProfileManager> profileManagers,
            IIndex<string, DeviceManager> deviceManagers,
            IIndex<string, InputSelector> inputSelectors)
        {
            this.configurationCaretaker = configurationCaretaker;
            this.mediator = mediator;
            this.inputConfigurations = inputConfigurations;
            this.outputConfigurations = outputConfigurations;
            this.profileManagers = profileManagers;
            this.deviceManagers = deviceManagers;
            this.inputSelectors = inputSelectors;

            AddInputSourceCommand = new DelegateCommand<DeviceViewModel>(AddInputSource, CanAddInputSource);

            SaveCommand = new DelegateCommand(SaveAndExit, CanFindServiceConfiguration);
            ApplyCommand = new DelegateCommand(SaveConfiguration, CanFindServiceConfiguration);
            cancelCommand = new DelegateCommand(OnCloseRequested);
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
                    moduleConfig = outputConfigurations[outputModule.Key];
                    globalConfig.OutputConfigurations.Add(moduleConfig);
                }

                var profileManager = profileManagers[outputModule.Key];
                await profileManager.SetTarget(moduleConfig, mediator);

                var deviceManager = deviceManagers[outputModule.Key];
                deviceManager.SetTarget(moduleConfig);

                var moduleViewModel = new ModuleViewModel(outputModule.Name, profileManager, deviceManager, null);
                Modules.Add(moduleViewModel);

                var unconfiguredDevices = moduleConfig.OutputDevices.Where(d => outputDevices.All(od => od.Id != d.Id));
                globalConfig.OutputDevices.AddRange(unconfiguredDevices);
            }

            foreach (var inputModule in globalConfig.InputModules)
            {
                var moduleConfig = globalConfig.InputConfigurations.SingleOrDefault(c => c.Key == inputModule.Key);
                if (moduleConfig == null)
                {
                    moduleConfig = inputConfigurations[inputModule.Key];
                    globalConfig.InputConfigurations.Add(moduleConfig);
                }

                var profileManager = profileManagers[inputModule.Key];
                await profileManager.SetTarget(moduleConfig, mediator);

                var deviceManager = deviceManagers[inputModule.Key];
                deviceManager.SetTarget(moduleConfig);
               
                var inputSelector = inputSelectors[inputModule.Key];
                
                var moduleViewModel = new ModuleViewModel(inputModule.Name, profileManager, deviceManager, inputSelector);
                Modules.Add(moduleViewModel);
            }

            foreach (var outputDevice in globalConfig.OutputDevices)
            {
                var moduleConfiguration = globalConfig.OutputConfigurations.SingleOrDefault(c => c.Key.Equals(outputDevice.Type, StringComparison.OrdinalIgnoreCase));
                AddActiveDevice(moduleConfiguration, outputDevice);
            }
            
            return InitializationResult.Succeeded;
        }

        private async void AddActiveDevice(IOutputConfiguration moduleConfiguration, IOutputDevice outputDevice)
        {
            var outputDeviceInfo = new DeviceViewModel(outputDevice, moduleConfiguration, globalConfig.InputDevices, mediator);

            var mapping = globalConfig.Mappings.SingleOrDefault(m => m.OutputDeviceId == outputDevice.Id);
            if (mapping != null)
            {
                foreach (var inputGroup in mapping.InputGroups)
                {
                    var inputDevice = globalConfig.InputDevices.Single(i => i.Id == inputGroup.InputDeviceId);
                    var inputSelector = inputSelectors[inputDevice.Key];
                    await inputSelector.SetDevice(inputDevice);
                    inputSelector.SelectInputs(inputGroup.Inputs);
                    outputDeviceInfo.InputSelectors.Add(inputSelector);
                    outputDeviceInfo.AvailableInputDevices.Remove(inputDevice);
                }
            }

            ActiveDevices.Add(outputDeviceInfo);
        }

        public DelegateCommand<DeviceViewModel> AddInputSourceCommand { get; }

        private bool CanAddInputSource(DeviceViewModel device)
        {
            return device != null && newInputDevice != null;
        }

        private IInputDevice newInputDevice;
        public IInputDevice NewInputDevice
        {
            get { return newInputDevice; }
            set { newInputDevice = value; OnPropertyChanged(); }
        }

        private async void AddInputSource(DeviceViewModel device)
        {
            var inputSelector = inputSelectors[newInputDevice.Key];
            await inputSelector.SetDevice(newInputDevice);
            var mapping = globalConfig.Mappings.Single(m => m.OutputDeviceId == device.OutputDevice.Id);
            var inputGroup = mapping.GetOrAddInputGroup(inputSelector.Device.Id);
            inputSelector.SelectInputs(inputGroup.Inputs);
            device.InputSelectors.Add(inputSelector);
            device.AvailableInputDevices.Remove(newInputDevice);
        }

        public ObservableCollection<ModuleViewModel> Modules { get; } = new ObservableCollection<ModuleViewModel>();

        public ObservableCollection<DeviceViewModel> ActiveDevices { get; } = new ObservableCollection<DeviceViewModel>();

        public DelegateCommand SaveCommand { get; }

        private void SaveAndExit()
        {
            SaveConfiguration();
            OnCloseRequested();
        }

        public DelegateCommand ApplyCommand { get; }

        private void SaveConfiguration()
        {
            globalConfig.Mappings.Clear();
            // TODO: The respective GUIs should take care of keeping this in sync
            foreach (var deviceInfo in ActiveDevices)
            {
                var mapping = new Mapping();
                mapping.OutputDeviceId = deviceInfo.OutputDevice.Id;

                foreach (var inputSelector in deviceInfo.InputSelectors)
                {
                    var inputGroup = new InputGroup();
                    inputGroup.InputDeviceId = inputSelector.Device.Id;
                    foreach (var selectedInput in inputSelector.GetSelectedInputs())
                    {
                        inputGroup.Inputs.Add(selectedInput);
                    }
                    mapping.InputGroups.Add(inputGroup);
                }
                globalConfig.Mappings.Add(mapping);
            }

            try
            {
                configurationCaretaker.Save(globalConfig);
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
