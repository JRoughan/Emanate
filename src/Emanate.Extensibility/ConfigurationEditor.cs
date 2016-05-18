using System.Threading.Tasks;
using System.Windows.Controls;
using Emanate.Core.Configuration;

namespace Emanate.Extensibility
{
    public class ConfigurationEditor : UserControl
    {
        public virtual Task SetTarget(IConfiguration moduleConfiguration) { return Task.CompletedTask; }
    }
}