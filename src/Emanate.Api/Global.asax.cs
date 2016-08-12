using System.Web.Http;
using Autofac;
using Emanate.Core;
using Emanate.Core.Configuration;

namespace Emanate.Api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            var builder = new ContainerBuilder();
            var loader = new ModuleLoader();
            loader.LoadAdminModules(builder);

            builder.RegisterType<ConfigurationCaretaker>();

            Store.Init(builder.Build());
        }
    }

    public static class Store
    {
        public static async void Init(IComponentContext componentContext)
        {
            var caretaker = componentContext.Resolve<ConfigurationCaretaker>();
            Config = await caretaker.Load();
        }

        public static GlobalConfig Config { get; private set; }
    }
}
