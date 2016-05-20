using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Emanate.Core;
using Emanate.Core.Output;

namespace Emanate.Vso.Admin.Inputs
{
    public partial class VsoInputSelectorView
    {
        private VsoInputSelectorViewModel viewModel;

        public VsoInputSelectorView()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty DeviceProperty = DependencyProperty.Register("Device", typeof(IDevice), typeof(VsoInputSelectorView), new PropertyMetadata(null, DeviceChanged));

        public IDevice Device
        {
            get { return (IDevice)GetValue(DeviceProperty); }
            set { SetValue(DeviceProperty, value); }
        }

        private static async void DeviceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (VsoInputSelectorView)d;
            var device = (VsoDevice)e.NewValue;

            target.viewModel = new VsoInputSelectorViewModel(device);
            await target.viewModel.Initialize();
            target.DataContext = target.viewModel;
        }

        public override void SelectInputs(IEnumerable<InputInfo> inputs)
        {
            viewModel.SelectInputs(inputs);
        }

        public override IEnumerable<InputInfo> GetSelectedInputs()
        {
            return viewModel?.GetSelectedInputs() ?? Enumerable.Empty<InputInfo>();
        }
    }
}
