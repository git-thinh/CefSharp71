using System;
using System.Threading;
using System.Windows.Forms;

namespace English
{
    static class App
    {
        /// <summary>
        /// Helps us to ensure only one instance runs at a time.
        /// </summary>
        static Mutex mutex = new Mutex(true, "{0fbc294c-f089-4009-9b1a-ab757739483f}");

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                try
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new MainContext());
                    CefSharp.Cef.Shutdown();
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
        }
        
    }
}
