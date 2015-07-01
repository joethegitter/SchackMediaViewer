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
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void OnStartup(Object sender, StartupEventArgs e)
        {
            Log("OnStartup(): entered.");
            string[] mainArgs = e.Args;

            

            //SetupDebugOutputOptions(mainArgs);
            //ProcessCommandLineArgsAndExecute(mainArgs);


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

        private void Application_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            Log("Application_LoadCompleted(): entered.");
            Log("Application_LoadCompleted(): exiting.");
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
