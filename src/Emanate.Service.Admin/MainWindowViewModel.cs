﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using Emanate.Core.Configuration;
using Emanate.Core.Output;
using Emanate.Extensibility;
using Emanate.Extensibility.Composition;
using Serilog;

namespace Emanate.Service.Admin
{
    public class MainWindowViewModel : ViewModel
    {
        private readonly IComponentContext componentContext;
        private readonly ConfigurationCaretaker configurationCaretaker;
        private readonly IMediator mediator;
        private GlobalConfig globalConfig;

        public MainWindowViewModel(IComponentContext componentContext, ConfigurationCaretaker configurationCaretaker, IMediator mediator)
        {
            saveCommand = new DelegateCommand(SaveAndExit, CanFindServiceConfiguration);
            applyCommand = new DelegateCommand(SaveConfiguration, CanFindServiceConfiguration);
            cancelCommand = new DelegateCommand(OnCloseRequested);

            this.componentContext = componentContext;
            this.configurationCaretaker = configurationCaretaker;
            this.mediator = mediator;
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
                    moduleConfig = componentContext.ResolveKeyed<IOutputConfiguration>(outputModule.Key);
                    globalConfig.OutputConfigurations.Add(moduleConfig);
                }

                var profileManager = componentContext.ResolveKeyed<ProfileManager>(outputModule.Key);
                await profileManager.SetTarget(moduleConfig, mediator);

                var deviceManager = componentContext.ResolveKeyed<DeviceManager>(outputModule.Key);
                deviceManager.SetTarget(moduleConfig);

                var moduleViewModel = new ModuleViewModel(outputModule.Name, profileManager, deviceManager, null);
                Modules.Add(moduleViewModel);

                var unconfiguredDevices = moduleConfig.OutputDevices.Where(d => !outputDevices.Any(od => od.Id == d.Id));
                globalConfig.OutputDevices.AddRange(unconfiguredDevices);
            }

            foreach (var inputModule in globalConfig.InputModules)
            {
                var moduleConfig = globalConfig.InputConfigurations.SingleOrDefault(c => c.Key == inputModule.Key);
                if (moduleConfig == null)
                {
                    moduleConfig = componentContext.ResolveKeyed<IInputConfiguration>(inputModule.Key);
                    globalConfig.InputConfigurations.Add(moduleConfig);
                }

                var profileManager = componentContext.ResolveKeyed<ProfileManager>(inputModule.Key);
                await profileManager.SetTarget(moduleConfig, mediator);

                var deviceManager = componentContext.ResolveKeyed<DeviceManager>(inputModule.Key);
                deviceManager.SetTarget(moduleConfig);

                var inputSelector = componentContext.ResolveKeyed<InputSelector>(inputModule.Key);

                var moduleConfigInfo = new ModuleViewModel(inputModule.Name, profileManager, deviceManager, inputSelector);
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

        private void AddActiveDevice(IOutputConfiguration moduleConfiguration, IOutputDevice outputDevice)
        {
            var outputDeviceInfo = new DeviceViewModel(outputDevice, moduleConfiguration, mediator);
            // HACK: Force an input for a new device without any. Ugly!
            var inputSelector = componentContext.ResolveKeyed<InputSelector>("vso");
            inputSelector.Device = globalConfig.InputDevices.Single(); // Break if more than one to encourage me to handle the scenario
            outputDeviceInfo.InputSelectors.Add(inputSelector);

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
