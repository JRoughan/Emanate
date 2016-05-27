using System.Linq;
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
                return config.OutputModules.Select(o => new {o.Key, o.Name});
            };
        }
    }
}
