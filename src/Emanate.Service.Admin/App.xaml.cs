using System.Diagnostics;
using System.Windows;
using Autofac;
using Emanate.Core;
using Emanate.Core.Configuration;
using Serilog;
using Serilog.Sinks.RollingFile;

namespace Emanate.Service.Admin
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            if (Debugger.IsAttached)
                Diagnostics.InitialiseConsole();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.ColoredConsole()
                .WriteTo.RollingFile(Paths.AdminLogFilePath)
                .CreateLogger();

            var container = CreateContainer();
            var mainWindowViewModel = container.Resolve<MainWindowViewModel>();
            MainWindow = container.Resolve<MainWindow>(new TypedParameter(typeof(MainWindowViewModel), mainWindowViewModel));
            MainWindow.Show();
        }

        private IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();
            var loader = new ModuleLoader();
            loader.LoadAdminModules(builder);

            builder.RegisterType<MainWindow>();
            builder.RegisterType<MainWindowViewModel>();
            builder.RegisterType<ConfigurationCaretaker>();

            return builder.Build();
        }
    }
}
