using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Emanate.Core;

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

            target.DeviceName = device.Name;
            target.SetDevice(device);
        }

        private static readonly DependencyPropertyKey DeviceNamePropertyKey = DependencyProperty.RegisterReadOnly("DeviceName", typeof(string), typeof(InputSelector), new PropertyMetadata(null));
        public string DeviceName
        {
            get { return (string)GetValue(DeviceNamePropertyKey.DependencyProperty); }
            private set { SetValue(DeviceNamePropertyKey, value); }
        }


        protected abstract void SetDevice(IDevice device);

        public virtual void SelectInputs(IEnumerable<string> inputs) { }
        public virtual IEnumerable<string> GetSelectedInputs() { yield break; }
    }
}
