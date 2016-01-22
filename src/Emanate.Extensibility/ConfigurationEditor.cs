using System.Windows.Controls;
using Emanate.Core.Configuration;

namespace Emanate.Extensibility
{
    public class ConfigurationEditor : UserControl
    {
        public virtual void SetTarget(IOutputConfiguration moduleConfiguration) { }
    }
}