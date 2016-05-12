using System;

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
        }
    }
}
