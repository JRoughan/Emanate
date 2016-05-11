using System.Reflection;
using Nancy;

namespace Emanate.Service.Api
{
    public class DefaultModule : NancyModule
    {
        public DefaultModule()
        {
            Get["/"] = _ => $"{Program.ServiceName} v{Assembly.GetExecutingAssembly().GetName().Version}";
        }
    }
}
