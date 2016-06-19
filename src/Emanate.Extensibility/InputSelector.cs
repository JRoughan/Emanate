using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Emanate.Core;

namespace Emanate.Extensibility
{
    public abstract class InputSelector : UserControl
    {
        private static readonly DependencyPropertyKey DevicePropertyKey = DependencyProperty.RegisterReadOnly("Device", typeof(IDevice), typeof(InputSelector), new PropertyMetadata(null));
        public IDevice Device
        {
            get { return (IDevice)GetValue(DevicePropertyKey.DependencyProperty); }
        }

        private static readonly DependencyPropertyKey DeviceNamePropertyKey = DependencyProperty.RegisterReadOnly("DeviceName", typeof(string), typeof(InputSelector), new PropertyMetadata(null));
        public string DeviceName
        {
            get { return (string)GetValue(DeviceNamePropertyKey.DependencyProperty); }
        }

        public async Task SetDevice(IDevice device)
        {
            SetValue(DevicePropertyKey, device);
            SetValue(DeviceNamePropertyKey, device.Name);

            await SetDeviceInternal(device);
        }

        protected abstract Task SetDeviceInternal(IDevice device);
        public virtual void SelectInputs(IEnumerable<string> inputs) { }
        public virtual IEnumerable<string> GetSelectedInputs() { yield break; }
    }
}
