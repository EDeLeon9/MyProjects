using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

namespace SingleInstanceManager
{
    /// <summary>
    /// Para usarse debe crearse el constructor en la clase App, así:
    /// public App()
    /// {
    ///     SingleInstanceApplicationManager.Validate("NombrePrograma");
    /// }
    /// </summary>
    public static class SingleInstanceApplicationManager
    {
        //Las constantes y struct son valores tomados de los manuales de Microsoft
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMAXIMIZED = 3;
        private const uint WPF_RESTORETOMAXIMIZED = 0x0002;
        private struct WINDOWPLACEMENT
        {
            public uint length;
            public uint flags;
            public uint showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
            public System.Drawing.Rectangle rcDevice;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]  //
        private static extern bool SetForegroundWindow(IntPtr hWnd);  // DllImport() obliga a declarar static extern
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        public static Mutex mutex;

        public static void Validate(string appUniqueString)
        {
            bool createdNew;
            mutex = new Mutex(true, appUniqueString + "_Mutex", out createdNew);
            if (!createdNew)
            {
                Process current = Process.GetCurrentProcess();
                foreach (Process process in Process.GetProcessesByName(current.ProcessName))
                {
                    if (process.Id != current.Id)
                    {
                        WINDOWPLACEMENT windowPlacement = new WINDOWPLACEMENT();
                        windowPlacement.length = Convert.ToUInt32(Marshal.SizeOf(windowPlacement));
                        GetWindowPlacement(process.MainWindowHandle, ref windowPlacement);

                        if (windowPlacement.flags == WPF_RESTORETOMAXIMIZED)
                            ShowWindow(process.MainWindowHandle, SW_SHOWMAXIMIZED);
                        else
                            ShowWindow(process.MainWindowHandle, SW_SHOWNORMAL);

                        SetForegroundWindow(process.MainWindowHandle);
                        break;
                    }
                }
                Application.Current.Shutdown();
            }
        }
    }
}
