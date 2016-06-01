using System.Threading.Tasks;
using System.Windows.Controls;
using Emanate.Core.Configuration;
using Emanate.Extensibility.Composition;

namespace Emanate.Extensibility
{
    public class ProfileManager : UserControl
    {
        public virtual Task SetTarget(IConfiguration moduleConfiguration, IMediator mediator) { return Task.CompletedTask; }
    }
}