using System.Collections.Generic;
using System.Windows.Controls;
using Emanate.Core.Configuration;
using Emanate.Core.Output;

namespace Emanate.Service.Admin
{
    public class InputSelector : UserControl
    {
        public virtual void SelectInputs(IEnumerable<InputInfo> inputs) { }
        public virtual IEnumerable<InputInfo> GetSelectedInputs() { yield break; }
    }

    public class ConfigurationEditor : UserControl
    {
        public virtual void SetTarget(IOutputConfiguration moduleConfiguration) { }
    }

    public class DeviceManager : UserControl
    {
        public virtual void SetTarget(IOutputConfiguration moduleConfiguration) { }
    }
}
