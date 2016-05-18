using System.Windows.Controls;
using Emanate.Core.Configuration;

namespace Emanate.Extensibility
{
    public class DeviceManager : UserControl
    {
        public virtual void SetTarget(IConfiguration moduleConfiguration) { }
    }
}