using Autofac;
using Nancy.Bootstrappers.Autofac;

namespace Emanate.Service.Api
{
    public class Bootstrapper : AutofacNancyBootstrapper
    {
        private static IContainer _container;
        public static void SetContainer(IContainer container)
        {
            _container = container;
        }

        protected override ILifetimeScope GetApplicationContainer()
        {
            return _container;
        }
    }
}