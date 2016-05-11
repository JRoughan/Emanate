using System.Linq;
using System.Reflection;
using Emanate.Core.Configuration;
using Nancy;

namespace Emanate.Service.Api
{
    public class OutputsModule : NancyModule
    {
        public OutputsModule(GlobalConfig config)
        {
            Get["/outputs"] = _ =>
            {
                return config.OututConfigurations.Select(o => new {o.Key, o.Name});
            };
        }
    }
}
