using System.Linq;
using Emanate.Core.Configuration;
using Nancy;

namespace Emanate.Service.Api
{
    public class InputsModule : NancyModule
    {
        public InputsModule(GlobalConfig config)
        {
            Get["/inputs"] = _ =>
            {
                return config.InputModules.Select(o => new {o.Key, o.Name});
            };
        }
    }
}
