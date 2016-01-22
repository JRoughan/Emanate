using System.Collections.Generic;
using System.Windows.Controls;
using Emanate.Core.Output;

namespace Emanate.Extensibility
{
    public class InputSelector : UserControl
    {
        public virtual void SelectInputs(IEnumerable<InputInfo> inputs) { }
        public virtual IEnumerable<InputInfo> GetSelectedInputs() { yield break; }
    }
}
