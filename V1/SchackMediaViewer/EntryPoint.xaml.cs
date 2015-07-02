using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using JoeKCo.Utilities;

namespace SchackMediaViewer
{
    /// <summary>
    /// Application Object and Entry Point
    /// </summary>
    public partial class App : Application
    {
        #region AppWide Data

        #endregion AppWide Data

        /// <summary>
        /// When the app first starts up, the OnStartup event is received by the app. This is the handler for that event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnStartup(Object sender, StartupEventArgs e)
        {
            Log("OnStartup(): entered.");

            // Scan the command line args for debug output options
            SetupLoggingOptions(e.Args);

            LaunchModality LaunchMode = LaunchModality.Undecided;
            IntPtr hWnd = IntPtr.Zero;

            GetLaunchMode(e, out LaunchMode, out hWnd);

            if (LaunchMode == LaunchModality.NOLAUNCH)
            {
                Log(" ** OnStartup() Error: LaunchMode = NOLAUNCH.");
                MessageBox.Show("Sorry, an error prevented the " + ProductName + " " + AppTypeDescription + " from running." +
                    Environment.NewLine + Environment.NewLine + "CommandLine: " + Environment.CommandLine,
                    this.ProductName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                App.Current.Shutdown(1);
            }

            // Here, set any AppWide variables specific to 
            // the returned LaunchMode, then call OpenAppropriateWindow
            OpenAppropriateWindow(LaunchMode, hWnd);

            Log("OnStartup(): exiting.");
        }

        private void Application_Activated(object sender, EventArgs e)
        {
            Log("Application_Activated(): entered.");
            Log("Application_Activated(): exiting.");

        }

        private void Application_Deactivated(object sender, EventArgs e)
        {
            Log("Application_Activated(): entered.");
            Log("Application_Activated(): exiting.");
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Log("Application_Deactivated(): entered.");
            Log("Application_Deactivated(): exiting.");
        }

        private void Application_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            Log("Application_SessionEnding(): entered.");
            Log("Application_SessionEnding(): exiting.");
        }

        public static void Log(string msg)
        {
            System.Diagnostics.Debugger.Log(0, "", msg + Environment.NewLine);
        }

    }
}
