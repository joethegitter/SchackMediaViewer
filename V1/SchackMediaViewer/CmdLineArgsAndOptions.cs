using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using System.Reflection;

using JoeKCo.Utilities;

namespace SchackMediaViewer
{
    /// <summary>
    /// Application Object and Entry Point
    /// </summary>
    public partial class App : Application
    {
        #region More AppWide Data
        // more App wide variables
        public bool fVSHosted = false;
        public bool fLaunchedFromStub = false;

        // command line strings and file extensions
        // SHARED: modify in stub also!
        public const string VSHOSTED = ".vshost";                   // string appended to our exe basename by VS when hosted
        public const string FROMSTUB = @"/scr";                     // tells us that our exe was launched from our .scr stub

        // command line strings for launch modes
        // SHARED: modify in stub also!
        public const string M_CP_CONFIGURE = @"/cp_configure";      // open settings dlg in control panel
        public const string M_CP_MINIPREVIEW = @"/cp_minipreview";  // open miniPreview form in control panel
        public const string M_DT_CONFIGURE = @"/dt_configure";      // open settings dlg on desktop
        public const string M_SCREENSAVER = @"/screensaver";        // open screenSaverForm

        // not shared, but derived
        public static List<string> scrArgs = new List<string>() { M_CP_CONFIGURE, M_CP_MINIPREVIEW, M_DT_CONFIGURE, M_SCREENSAVER };

        public const string M_NO_MODE = "no_mode";                  // open screenSaverForm in windowed mode


        #endregion More AppWide Data


        private void SetupLoggingOptions(string[] mainArgs)
        {
            Log("SetupLoggingOptions(): entered.");

            // do work.
            // detect and set fVSHosted here.
            // detect and set fRunFromScreenSaverStub here

            // Full name and path of our executable is conveniently stored in item zero
            // of the Environment version of the command line args.
            string[] EnvArgs = Environment.GetCommandLineArgs();

            // Determine if we are running hosted by Visual Studio.
            fVSHosted = EnvArgs[0].ToLowerInvariant().Contains(VSHOSTED.ToLowerInvariant());

            foreach (string arg in EnvArgs)
            {
                // Determine if we were run from the screen saver stub
                if (arg.ToLowerInvariant().Trim() == FROMSTUB)
                {
                    fLaunchedFromStub = true;
                }
            }


            Log("SetupLoggingOptions(): entered.");
        }

        private void GetLaunchMode(StartupEventArgs SEArgs, out LaunchModality LaunchMode, out IntPtr hWnd)
        {
            Log("GetLaunchMode(): entered.");

            string[] mainArgs = SEArgs.Args;
            bool retVal = true;
            hWnd = IntPtr.Zero;

            // The only mainArgs that this exe cares about are:
            // 1. no mode mainArgs passed = M_NO_MODE (open screensaver in last remembered non-ScreenSaverStyle window, with no Slideshow)
            // 2. /scr, which tells us that we were launched from the .scr stub,
            //    and therefore need to respect arguments with window handles
            // 3. /dt_configure (open settings dialog on desktop)
            // 4. /c (same as dt_configure)
            // 5. /cp_configure -windowHandle (open settings owned by control panel)
            // 6. /cp_minipreview -windowHandle (put minipreview form in control panel)
            // 7. /screensaver (run the default screen saver with slideshow)
            // 8. /s (same as /screensaver)
            // 9. /popdbgwin (pop up the debugoutputwindow on a timer after launch)
            // 10. any 'unofficial' mainArgs we process with SetDebugOutputAndHostOptions() and/or HandleUnofficialArgs()

            // By the time we get to this method, the command line will have been scanned once by
            // SetDebugOutputAndHostOptions(), and certain LaunchManager.Modes will have been set:
            //     1. fRunFromScreenSaverStub
            //     2. VSHOSTED

            string launchString = M_SCREENSAVER;

            // First, handle only the very rigorous "we were launched from the scr stub" case.
            // Note "/scr" was already detected, and noted in fRunFromScreenSaverStub
            int lastProcessedIndex = -1;
            if (fLaunchedFromStub)  // set for us by SetDebugOutputAndHostOptions()
            {
                // Logic:
                // A. If /scr exists, it must be first
                // B. If /scr, only one /scr arg allowed
                // C. If /scr, one /scr arg required
                // D. If /scr, validate that windowHandles parse to IntPtr 
                // E. If /scr, non-scr mainArgs not allowed except /popdbgwin or /startbuffer

                // first argument must be /scr
                if ((mainArgs.Length > 0) && (mainArgs[0].ToLowerInvariant() != @"/scr"))
                {
                    Log(@"  * GetLaunchMode(): /scr can only appear as the first argument.");
                    throw new ArgumentException(@"CommandLine: /scr can only appear as the first argument." +
                    Environment.NewLine + Environment.CommandLine);
                }
                lastProcessedIndex = 0;

                // second arg must be one of four valid /scr-related arguments
                if ((mainArgs.Length > 1) && !scrArgs.Contains(mainArgs[1].ToLowerInvariant()))
                {
                    Log(@"  * GetLaunchMode(): /scr must be followed by a valid /scr-related argument.");
                    throw new ArgumentException(@"CommandLine: /scr must be followed by a valid /scr-related argument." +
                    Environment.NewLine + Environment.CommandLine);
                }
                lastProcessedIndex = 1;

                // if second arg starts with cp_ it must be followed with a valid window handle
                if (mainArgs[1].ToLowerInvariant() == M_CP_CONFIGURE || mainArgs[1].ToLowerInvariant() == M_CP_MINIPREVIEW)
                {
                    if ((mainArgs.Length > 2) && mainArgs[2].ToLowerInvariant().StartsWith("-"))
                    {
                        string subArg = mainArgs[2].ToLowerInvariant();
                        string longCandidate = subArg.Substring(1);
                        if (!String.IsNullOrEmpty(longCandidate))
                        {
                            long val;
                            bool worked = long.TryParse(longCandidate, out val);
                            if (worked)
                            {
                                hWnd = new IntPtr(val);
                            }
                            else  // bad parse
                            {
                                Log(@"  * GetLaunchMode(): invalid window handle passed: " + longCandidate);
                                throw new ArgumentException(@"CommandLine: invalid window handle passed: " + longCandidate +
                                    Environment.NewLine + Environment.CommandLine);
                            }
                        }
                        else   // null or empty
                        {
                            Log(@"  * GetLaunchMode(): null or empty window handle passed.");
                            throw new ArgumentException(@"CommandLine: null or empty window handle passed." +
                                Environment.NewLine + Environment.CommandLine);
                        }
                    }
                    else  // missing required sub argument
                    {
                        Log(@"  * GetLaunchMode(): /cp_ argument missing required subargument.");
                        throw new ArgumentException(@"CommandLine: /cp_ argument missing required subargument." +
                            Environment.NewLine + Environment.CommandLine);
                    }
                    lastProcessedIndex = 2;
                }

                // by this point, a valid mode is in mainArgs[1] and hWnd is either IntPtr.Zero or a numerically validated hWnd.
                launchString = mainArgs[1].ToLowerInvariant();
            }
            else
            {
                launchString = M_NO_MODE;
            }

            // If not launched from stub, all commandline args are ignored.

            // Now map launchString to LaunchMode enum
            LaunchMode = LaunchModality.Undecided;

            if (launchString == M_NO_MODE) LaunchMode = LaunchModality.ScreenSaverWindowed;
            if (launchString == M_CP_CONFIGURE) LaunchMode = LaunchModality.CP_Configure;
            if (launchString == M_CP_MINIPREVIEW) LaunchMode = LaunchModality.CP_MiniPreview;
            if (launchString == M_DT_CONFIGURE) LaunchMode = LaunchModality.DT_Configure;
            if (launchString == M_SCREENSAVER) LaunchMode = LaunchModality.ScreenSaver;

            // Handle any 'unofficial' arguments here
            // HandleUnofficialArguments(mainArgs);

            Log("GetLaunchMode(): exiting, returning:" + retVal);
        }


        public enum LaunchModality
        {
            /// <summary>
            /// Functional modes.
            /// </summary>
            DT_Configure = 10, CP_Configure = 11, CP_MiniPreview = 20, ScreenSaver = 30, ScreenSaverWindowed = 40,
            /// <summary>
            /// Lauch mode has not yet been established.
            /// </summary>
            Undecided = 0,
            /// <summary>
            /// App should not launch, see NoLaunchReason.
            /// </summary>
            NOLAUNCH = -1
        }

        

        // create a public property which contains the name of the product as specified
        // in the assembly.cs info
        private string _ProductName = null;
        public string ProductName
        {
            get
            {
                if (_ProductName == null)
                {
                    // Get the Product Name from the Assembly information
                    string productName = String.Empty;
                    var list = Application.Current.GetType().Assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), true);
                    if (list != null)
                    {
                        if (list.Length > 0)
                        {
                            _ProductName = (list[0] as AssemblyProductAttribute).Product;
                        }
                    }
                }

                return _ProductName;
            }
        }

        /// <summary>
        /// Property which describes the app as an "Application" or as a "Screen Saver", depending on how it was launched.
        /// </summary>
        public string AppTypeDescription
        {
            get
            {
                if (fLaunchedFromStub)
                {
                    return "Screen Saver";
                }
                else
                {
                    return "Application";
                }
            }
        }

    }
}