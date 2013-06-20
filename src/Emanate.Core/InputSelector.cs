using System.Collections.Generic;
using System.Windows.Controls;
using Emanate.Core.Configuration;
using Emanate.Core.Output;

namespace Emanate.Core
{
    public class InputSelector : UserControl
    {
        public virtual void SelectInputs(IEnumerable<InputInfo> inputs, IModuleConfiguration moduleConfiguration, string currentOutputProfile) { }
    }
}
