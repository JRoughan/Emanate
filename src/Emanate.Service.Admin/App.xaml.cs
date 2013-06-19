using System.Windows;
using System.Windows.Controls;
using Autofac;
using Emanate.Core;

namespace Emanate.Service.Admin
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var container = CreateContainer();
            var mainWindowViewModel = container.Resolve<MainWindowViewModel>();
            MainWindow = container.Resolve<MainWindow>(new TypedParameter(typeof(MainWindowViewModel), mainWindowViewModel));
            MainWindow.Show();
        }

        private IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();
            var loader = new ModuleLoader();
            loader.LoadModules(builder);

            builder.RegisterType<MainWindow>();
            builder.RegisterType<MainWindowViewModel>();
            builder.RegisterType<PluginConfigurationStorer>();

            return builder.Build();
        }
    }
}
