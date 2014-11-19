using System;
using System.Diagnostics;

namespace Emanate.Core
{
    public static class Diagnostics
    {
        public static void InitialiseConsole()
        {
            var handle = NativeMethods.GetConsoleWindow();
            if (handle == IntPtr.Zero)
                NativeMethods.AllocConsole();
            else
                NativeMethods.ShowWindow(handle, NativeMethods.SW_SHOW);

            // TODO: Look into TraceOutputOptions to remove useless info from log lines (or just override Write/WriteLine)
            var consoleTraceListener = new ConsoleTraceListener();
            Trace.Listeners.Add(consoleTraceListener);
        }
    }
}
