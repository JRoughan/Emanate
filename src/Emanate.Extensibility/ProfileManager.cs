using System.Threading.Tasks;
using System.Windows.Controls;
using Emanate.Core.Configuration;

namespace Emanate.Extensibility
{
    public class ProfileManager : UserControl
    {
        public virtual Task SetTarget(IConfiguration moduleConfiguration) { return Task.CompletedTask; }
    }
}