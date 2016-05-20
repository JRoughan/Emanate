using System.Collections.Generic;
using System.Windows;
using Emanate.Core;
using Emanate.Core.Output;

namespace Emanate.TeamCity.Admin.Inputs
{
    public partial class InputSelectorView
    {
        private InputSelectorViewModel viewModel;

        public InputSelectorView()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty DeviceProperty = DependencyProperty.Register("Device", typeof(IDevice), typeof(InputSelectorView), new PropertyMetadata(null, DeviceChanged));

        public IDevice Device
        {
            get { return (IDevice)GetValue(DeviceProperty); }
            set { SetValue(DeviceProperty, value); }
        }

        private static async void DeviceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (InputSelectorView)d;
            var device = (TeamCityDevice)e.NewValue;

            target.viewModel = new InputSelectorViewModel(device);
            await target.viewModel.Initialize();
            target.DataContext = target.viewModel;
        }

        public override void SelectInputs(IEnumerable<InputInfo> inputs)
        {
            viewModel.SelectInputs(inputs);
        }

        public override IEnumerable<InputInfo> GetSelectedInputs()
        {
            return viewModel.GetSelectedInputs();
        }
    }
}
