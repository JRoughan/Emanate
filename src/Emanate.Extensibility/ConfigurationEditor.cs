using System.Threading.Tasks;
using System.Windows.Controls;
using Emanate.Core.Configuration;

namespace Emanate.Extensibility
{
    public class ConfigurationEditor : UserControl
    {
        public virtual Task SetTarget(IOutputConfiguration moduleConfiguration) { return Task.CompletedTask; }
    }
}