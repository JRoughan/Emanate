using System;
using System.Collections.Generic;
using System.Web.Http;
using Autofac;
using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Delcom.Configuration;
using Emanate.TeamCity.Configuration;
using Emanate.Vso.Configuration;

namespace Emanate.Api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            // HACK to force plugins to be found as referenced assemblies
            var x = new DelcomConfiguration();
            var y = new TeamCityConfiguration();
            var z = new VsoConfiguration();

            var builder = new ContainerBuilder();
            var loader = new ModuleLoader();
            loader.LoadAdminModules(builder);

            builder.RegisterType<ConfigurationCaretaker>();

            Store.Init(builder.Build());
        }
    }

    public static class Store
    {
        public static void Init(IComponentContext componentContext)
        {
            var caretaker = componentContext.Resolve<ConfigurationCaretaker>();
            Config = caretaker.Load();
            Config.OutputGroups = new List<OutputGroup> {new OutputGroup {Id = Guid.NewGuid(), Name = "Default"}};
        }

        public static GlobalConfig Config { get; private set; }
    }
}
