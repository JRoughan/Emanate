using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Emanate.Core;
using Emanate.Core.Output;

namespace Emanate.Extensibility
{
    public abstract class InputSelector : UserControl
    {
        public static readonly DependencyProperty DeviceProperty = DependencyProperty.Register("Device", typeof(IDevice), typeof(InputSelector), new PropertyMetadata(null, DeviceChanged));

        public IDevice Device
        {
            get { return (IDevice)GetValue(DeviceProperty); }
            set { SetValue(DeviceProperty, value); }
        }

        private static void DeviceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (InputSelector)d;
            var device = (IDevice)e.NewValue;

            target.SetDevice(device);
        }

        protected abstract void SetDevice(IDevice device);

        public virtual void SelectInputs(IEnumerable<InputInfo> inputs) { }
        public virtual IEnumerable<InputInfo> GetSelectedInputs() { yield break; }
    }
}
