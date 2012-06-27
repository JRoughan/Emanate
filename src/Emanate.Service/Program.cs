using System;

namespace Emanate.Service
{
    static class Program
    {
        static void Main()
        {
            if (Environment.UserInteractive)
            {
                var serviceRunner = new ServiceRunner();
                serviceRunner.RunAsConsole();
            }
            else
            {
                var serviceRunner = new ServiceRunner();
                serviceRunner.RunAsService();
            }
        }
    }
}
