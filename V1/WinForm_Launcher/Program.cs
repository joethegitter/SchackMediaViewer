using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.IO;

namespace WinForm_Launcher
{
    static class Program
    {
        #region Data To Sync With Screen Saver App
        // Any changes to this data must be synced with the screen saver!

        // Command line args that the launcher will pass to the screen saver app.
        // 1. tells app that it was launched from the screen saver stub
        const string FROMSTUB = @"/scr";
        // 2. tells app to open settings dlg in control panel
        const string M_CP_CONFIGURE = @"/cp_configure";
        // 3. tells app to draw screen saver in tiny control panel preview
        const string M_CP_MINIPREVIEW = @"/cp_minipreview";
        // 4. tells app to open settings dlg on desktop
        const string M_DT_CONFIGURE = @"/dt_configure";
        // 5. tells app to open the screen saver in full screen
        const string M_SCREENSAVER = @"/screensaver";

        #endregion Data To Sync With Screen Saver App

        #region Configuration Data

        // Names of file which serve as flags
        const string FilenameFlag_DoSetScreenSaverApp = "ChangeScreenSaverApp.yes";
        const string FilenameFlag_DoDebugSettingsDlg = "OpenDebugSettingsDlg.yes";

        // The initial name of the target screen saver app is kept in
        // Properties.Settings.Settings. It cannot be changed here.

        #endregion Configuration Data

        // Application wide variables
        static Keys mKeys = Control.ModifierKeys;
        static string NL = Environment.NewLine;
        static LaunchData ourLaunchData = new LaunchData(); 
        static IntPtr hWndModalDialogParent = IntPtr.Zero;

        /// <summary>
        /// The main entry point for the launcher executable.
        /// </summary>
        [STAThread]
        static void Main(string[] Args)
        {
            Log("Main(): entered.");

            // Immediately capture the state of modifier keys when the launcher opens
            mKeys = Control.ModifierKeys;   // "Control" here refers to the app, there is no window
            Log("   Main(): Modifier Keys held down at launch were: " + mKeys.ToString());

            // WinForms boilerplate, ignore
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Initialize some app-wide ("global") variables
            ourLaunchData.PassedHwnd = IntPtr.Zero;
            ourLaunchData.LaunchMode = ScrSvrLaunchMode.Default;
            ourLaunchData.OutGoingArgs = "";

            // Parse the command line, as we'll need some of that data immediately (hWndModalDialogParent)
            bool fAbortAppLaunch = false;
            string abortReason = "<Unspecified>";
            fAbortAppLaunch = TryParseScreenSaverArgs(Args, ref ourLaunchData, ref abortReason);
            if (fAbortAppLaunch)
            {
                Log("Main(): exiting because TryParseScreenSaverArgs() returned true, abortReason = " + abortReason);
                // Todo: JOE: put up a message box here
                return;
            }

            // We may need to present modal dialogs before we launch the screen saver. These
            // need to be modal to the Control Panel or the Desktop, as we have no app window.
            // If it's Control Panel, that data is passed on the command line arg. Might as
            // well parse all of the command line args now.
            hWndModalDialogParent = GetParentHwndForModalDialogs(ourLaunchData.LaunchMode, ourLaunchData.PassedHwnd);

            // See if we need to let user change target screen saver app before launching 
            bool fChangeTargetApplication = DetectChangeTargetApplicationFlags();

            if (fChangeTargetApplication)
            {
                // note: user can abort launch during ChangeTargetApplicationAndOrAbortLaunch()
                fAbortAppLaunch = ChangeTargetApplicationAndOrAbortLaunch(true);
                if (fAbortAppLaunch)
                {
                    Log("Main(): exiting because ChangeTargetApplicationAndOrAbortLaunch() returned true.");
                    return;
                }
                // PathToLauncher may now be stale, always use Get
            }

            // See if we need to let user change debug settings
            bool fShowDebugSettingsDialog = DetectShowDebugSettingsFlags();

            if (fShowDebugSettingsDialog)
            {
                fAbortAppLaunch = false;
                fAbortAppLaunch = ShowDebugSettingsDlgAndOrAbortLaunch();
                if (fAbortAppLaunch)
                {
                    Log("Main(): exiting because ShowDebugSettingsDlgAndOrAbortLaunch() returned true.");
                    return;
                }
            }

            // At this point, Settings.Settings contains the correct target screen saver app,
            // and incoming screen saver args have been translated and placed in FinalArgs.
            // PathToLauncher may now be stale, but we won't use it again

            LaunchScreenSaver();

            Log("Main(): exiting.");

        }

        private static void LaunchScreenSaver()
        {
            Log("LaunchScreenSaver(): we would normally send this here: " + ourLaunchData.OutGoingArgs);
        }

        /// <summary>
        /// Gets the normal or alt value of location of launcher.
        /// </summary>
        /// <param name="pathToLauncher"></param>
        /// <param name="directoryScreenSaverIsLocatedIn"></param>
        /// <returns></returns>
        private static void GetPathToLauncher(ref string pathToLauncher, ref string directoryScreenSaverIsLocatedIn)
        {
            // JOE, update all other places to use this, instead of old globals

            pathToLauncher = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
            directoryScreenSaverIsLocatedIn = "";
            if (Properties.Settings.Default.UseAltScreenSaverDirectory)
            {
                directoryScreenSaverIsLocatedIn = Properties.Settings.Default.AltScreenSaverDirectory;
            }
            else
            {
                directoryScreenSaverIsLocatedIn = PathToLauncher;
            }
        }

        public class LaunchData
        {
            public string OutGoingArgs;
            public ScrSvrLaunchMode LaunchMode;
            public IntPtr PassedHwnd;
        }

        public enum ScrSvrLaunchMode
        {
            Default,        // full screen, no parent window
            Configure,      // open screen saver settings window on desktop
            CP_Configure,   // open screen saver settings window modal to control panel 
            CP_Preview      // draw screen saver image in tiny preview window in control panel
        }



        /// <summary>
        /// Translate incoming args to the args we'll pass to the screen saver app.
        /// </summary>
        /// <param name="incomingArgs"></param>
        /// <returns>Returns TRUE if something critical failed and program should abort launch.</returns>
        private static bool TryParseScreenSaverArgs(string[] incomingArgs, ref LaunchData zoopy, ref string AbortReason)
        {
            bool fAbort = false;
            string outModeStr = "";
            bool fHasWindowHandle = false;
            string outHwndStr = "";

            // Examine incoming args and build outgoing args.
            // When Windows launches a screen saver, it sends specific 
            // command line args. Those will only ever be the following
            // (EB = the behavior that Windows expects):
            //  /S                - EB: run screensaver in fullscreen
            //                      so we pass /screensaver to our app
            //  /P outHwndStr   - EB: show little Preview in control panel
            //                      so we pass /cp_minipreview -outHwndStr  
            //  no args           - EB: show Settings dlg on desktop
            //                      so we pass /dt_configure
            //  /C                - EB: show Settings dlg on desktop
            //                      so we pass /dt_configure
            //  /C:outHwndStr   - EB: show Settings modal to control panel
            //                      so we pass /cp_configure -outHwndStr

            if (incomingArgs.Length < 1) // no args
            {
                ourLaunchData.LaunchMode = ScrSvrLaunchMode.Configure;
                outModeStr = M_DT_CONFIGURE;
            }
            else if (incomingArgs.Length < 2) // 1 arg
            {
                // can only be:
                //  /S or 
                //  /C or 
                //  /C:hWnd  (note: single arg, no space in it)

                // these two are exclusive, only one will ever be true
                if (incomingArgs[0].ToLowerInvariant().Trim() == @"/s")
                {
                    outModeStr = M_SCREENSAVER;
                    ourLaunchData.LaunchMode = ScrSvrLaunchMode.Default;
                }
                if (incomingArgs[0].ToLowerInvariant().Trim() == @"/c")
                {
                    ourLaunchData.LaunchMode = ScrSvrLaunchMode.Configure;
                    outModeStr = M_DT_CONFIGURE;
                }

                if (incomingArgs[0].ToLowerInvariant().Trim().StartsWith(@"/c:"))
                {
                    ourLaunchData.LaunchMode = ScrSvrLaunchMode.CP_Configure;
                    outModeStr = M_CP_CONFIGURE;
                    // get the chars after /c: for the outHwndStr
                    outHwndStr = incomingArgs[0].Substring(3);
                    fHasWindowHandle = true;
                }
            }
            else if (incomingArgs.Length < 3) // 2 args
            {
                // can only be /P outHwndStr (note space)
                ourLaunchData.LaunchMode = ScrSvrLaunchMode.CP_Preview;
                outModeStr = M_CP_MINIPREVIEW;
                fHasWindowHandle = true;
                outHwndStr = incomingArgs[1];
            }
            else
            {
                AbortReason = "CommandLine had more than 2 arguments, could not parse.";
                fAbort = true;
            }

            // add outModeStr info to outgoingArgs
            ourLaunchData.OutGoingArgs = FROMSTUB + " " + outModeStr;

            // process window handle
            if (fHasWindowHandle)
            {
                // Validate hWnd, as modal dialogs in this launcher will use it
                bool IsValid = false;
                IntPtr hWnd = IntPtr.Zero;

                IsValid = TryGetValidatedWindowHandle(outHwndStr, ref hWnd);
                if (!IsValid)
                {
                    AbortReason = "Bad hWnd passed to screen saver launcher.";
                    fAbort = true;
                }

                // add outHwndStr as str to OutGoingArgs
                ourLaunchData.OutGoingArgs += " -" + outHwndStr;
                ourLaunchData.PassedHwnd = hWnd;
            }
            else  // incoming args did not contain hWnd
            {
                ourLaunchData.PassedHwnd = IntPtr.Zero;
            }

            return fAbort;
        }

        private static bool TryGetValidatedWindowHandle(string windowHandle, ref IntPtr hWnd)
        {
            bool IsValid = false;

            // try to translate outHwndStr string to a long
            if (!String.IsNullOrEmpty(windowHandle))
            {
                long val;
                bool worked = long.TryParse(windowHandle, out val);
                if (worked)
                {
                    hWnd = new IntPtr(val);
                }
                else  // bad parse
                {
                    Log(@"  * TryGetValidatedWindowHandle(): invalid window handle passed: " + windowHandle);
                    IsValid = false;
                }
            }
            else   // null or empty
            {
                Log(@"  * TryGetValidatedWindowHandle(): null or empty window handle passed.");
                IsValid = false;
            }

            return IsValid;
        }

        /// <summary>
        /// Allows user to launch app w/ path/name already stored in Settings, or change path/name in Settings, or cancel launch/exit launcher.
        /// </summary>
        /// <param name="hWndModalParent">hWnd of parent window for modal dialog.</param>
        /// <param name="showExplanationText">If false, Hello text won't be shown in dialog.</param>
        /// <returns></returns>
        private static bool ChangeTargetApplicationAndOrAbortLaunch(bool showExplanationText = false)
        {
            Log("ChangeTargetApplicationAndOrAbortLaunch(): entered.");

            bool fAbort = false;
            string Message = "";

            // If requested pre-pend explanation text to message
            if (showExplanationText == true)
            {
                Message = "Select a different application to launch as a screen saver?" +
                NL + NL +
                "This dialog appears if, when the screen saver launcher is selected in the control panel, the Control Key is held down or a file named " + 
                FilenameFlag_DoSetScreenSaverApp + " is found in the same directory as the launcher." +
                NL + NL;
            }

            // body
            Message += "The application currently slated to be launched as a screen saver is: " +
            NL +
            NL +
            Path.Combine(Properties.Settings.Default.AltScreenSaverDirectory, Properties.Settings.Default.ScreenSaverAppFilenameAndExtension) +
            NL +
            NL +
            "Do you wish to select a different application to be launched as a screen saver, now and in the future? " +
            NL +
            NL +
            "Press Yes to select an different application.  Press No to launch the current application.  Press Cancel to abort this launch attempt.";

            // present dialog
            DialogResult dr = MessageBox.Show(new WindowWrapper(hWndModalDialogParent), 
                Message, 
                "Change Screen Saver Application", 
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question, 
                MessageBoxDefaultButton.Button3);

            // capture any keys held down when dialog was dismissed
            Keys mKeys = Control.ModifierKeys;
            Log("ChangeTargetApplicationAndOrAbortLaunch(): user chose dlg button '" + dr.ToString() + "' with modifier keys: " + mKeys.ToString());

            if (dr == DialogResult.Cancel)
            {
                // quit the launcher
                fAbort = true;
            }
            else
            {
                if (dr == DialogResult.Yes)
                {
                    fAbort = false;
                    GetAndSetNewTargetApplication();

                    // recurse to allow change or confirmation or cancel of launch
                    ChangeTargetApplicationAndOrAbortLaunch(false);
                }
            }

            Log("ChangeTargetApplicationAndOrAbortLaunch(): exiting.");
            return fAbort;
        }

        /// <summary>
        /// Shows the File Open dialog so user can choose new app. Handles Cancel.
        /// </summary>
        /// <returns>Returns TRUE if user selected an app, FALSE if user Canceled.</returns>
        private static bool GetAndSetNewTargetApplication()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Please select the application to be used as a screen saver.";
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.DefaultExt = ".exe";
            ofd.Multiselect = false;
            ofd.ValidateNames = true;

            DialogResult dr = ofd.ShowDialog();
            if (dr == DialogResult.Cancel)
            {
                System.Diagnostics.Debugger.Log(0, "", "Filename after GetAndSetNewTargetApplication was canceled is: " + Properties.Settings.Default.ScreenSaverAppFilenameAndExtension);
                return false;
            }
            else
            {
                Log("GetAndSetNewTargetApplication(): user selected file: " + ofd.FileName);
                Log("GetAndSetNewTargetApplication(): updating Settings and calling Save(): ");

                Properties.Settings.Default.ScreenSaverAppFilenameAndExtension = ofd.FileName;
                Properties.Settings.Default.Save();

                //Log("GetAndSetNewTargetApplication(): Reloading Settings:");
                //Properties.Settings.Default.Reload();

                //Log("GetAndSetNewTargetApplication(): the saved value, once reloaded is: " + Properties.Settings.Default.ScreenSaverAppFilenameAndExtension);
                return true;

                // JOE - where is this being written on disk?
            }
        }

        /// <summary>
        /// Wrapper class so that we can return an IWin32Window given a hwnd
        /// </summary>
        public class WindowWrapper : System.Windows.Forms.IWin32Window
        {
            public WindowWrapper(IntPtr handle)
            {
                _hwnd = handle;
            }

            public IntPtr Handle
            {
                get { return _hwnd; }
            }

            private IntPtr _hwnd;
        }

        /// <summary>
        /// Gets the correct hWnd to set as owner of our modal dialog. 
        /// </summary>
        /// <param name="cwMode"></param>
        /// <param name="ScrSvrLaunchMode"></param>
        /// <returns>Returns passedHwnd if non-CP launchmode is passed, or if error occurs.</returns>
        private static IntPtr GetParentHwndForModalDialogs(ScrSvrLaunchMode launchMode, IntPtr passedHwnd)
        {
            // if passedHwnd = IntPtr.Zero, that value will be returned.
            // if launchMode != one of the CP LaunchModes, IntPtr.Zero will be returned.
            // if an error occurs with one of the CP LaunchModes, passedHwnd will be returned.
            if (passedHwnd == IntPtr.Zero)
            {
                Log("  GetParentHwndForModalDialogs(): passedHwnd was IntPtr.Zero.  Returning IntPtr.Zero.");
                return IntPtr.Zero;
            } 

            IntPtr retVal = IntPtr.Zero;
            if (launchMode == ScrSvrLaunchMode.Default) retVal = IntPtr.Zero;

            if (launchMode == ScrSvrLaunchMode.Configure) retVal = IntPtr.Zero;

            if (launchMode == ScrSvrLaunchMode.CP_Preview) retVal = passedHwnd;

            if (launchMode == ScrSvrLaunchMode.CP_Configure)
            {
                // Get the root owner window of the passed hWnd
                int error = 0;
                Log("  GetParentHwndForModalDialogs(): Getting Root Ancestor: calling GetAncestor(hWnd, GetRoot)...");
                NativeMethods.SetLastErrorEx(0, 0);
                IntPtr ancestorWindow = NativeMethods.GetAncestor(passedHwnd, NativeMethods.GetAncestorFlags.GetRoot);
                error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                Log("  GetParentHwndForModalDialogs(): GetAncestor() returned IntPtr: " + ancestorWindow.ToString());
                Log("  GetParentHwndForModalDialogs(): GetLastError() returned: " + error.ToString());

                if (error != 0)
                {
                    Log("  GetParentHwndForModalDialogs(): non-zero error value, so returning passedHwnd instead.");
                    retVal = passedHwnd;
                }
                else
                {
                    if (!NativeMethods.IsWindow(ancestorWindow))
                    {
                        Log("  GetParentHwndForModalDialogs(): IsWindow(" + ancestorWindow.ToString() + ") returned false, so returning passedHwnd instead.");
                        retVal = passedHwnd;
                    }
                    else
                    {
                        retVal = ancestorWindow;
                    }
                }

            }

            Log("  GetParentHwndForModalDialogs(): launchMode was not recognized, should not happen. Returning IntPtr.Zero.");
            return IntPtr.Zero;
        }


        private static bool ShowDebugSettingsDlgAndOrAbortLaunch()
        {
            //string Message = "The settings dialog would be here.";
            //bool fUseParentWindow = false;
            //IntPtr hWndToUse = IntPtr.Zero;

            //if (hWndModalParent == IntPtr.Zero)
            //{
            //    fUseParentWindow = false;
            //}
            //else
            //{

            //    fUseParentWindow = true;
            //}

            //// use Win32 API's to get the window handles we need for ShowModal

            //// JOE! rewrite all message box stuff to use Win32 api for message box

            //// Get the root owner window of the passed hWnd
            //int error = 0;
            //Log("  ShowSettings(): Getting Root Ancestor: calling GetAncestor(hWnd, GetRoot)...");
            //NativeMethods.SetLastErrorEx(0, 0);
            //IntPtr passedWndRoot = NativeMethods.GetAncestor(hWndModalParent, NativeMethods.GetAncestorFlags.GetRoot);
            //error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
            //Log("  ShowSettings(): GetAncestor() returned IntPtr: " + hWndModalParent.ToString());
            //Log("      GetLastError() returned: " + error.ToString());
            //Log(" ");

            //// and then show ourselves modal to that window
            //WindowWrapper ww = new WindowWrapper(passedWndRoot);
            //Log("  ShowSettings(): calling ShowDialog().");
            //settings.ShowDialog(ww);



            //if (hWndModalParent == IntPtr.Zero)
            //{
            //    MessageBox.Show("The settings dialog would be here.");
            //}
            //else
            //{
            //    MessageBox.Show(hWndModalParent, Message);
            //}
            return false;
        }

        /// <summary>
        /// Detects signs that user wants to change the target screen saver application.
        /// </summary>
        /// <returns></returns>
        private static bool DetectChangeTargetApplicationFlags()
        {
            // if there's a file in same directory as launcher with FilenameFlag_DoDebugSettingsDlg
            if (File.Exists(Path.Combine(PathToLauncher, FilenameFlag_DoSetScreenSaverApp)))
            {
                return true;
            }

            // or if the CTRL key was held down at launch
            if ((mKeys & Keys.Control) == Keys.Control)
            {
                return true;
            }

            // otherwise return false
            return false;

        }

        /// <summary>
        /// Detects signs that user wants to change debug settings for launcher.
        /// </summary>
        /// <returns></returns>
        private static bool DetectShowDebugSettingsFlags()
        {
            // if there's a file in same directory as launcher with FilenameFlag_DoDebugSettingsDlg
            if (File.Exists(Path.Combine(PathToLauncher, FilenameFlag_DoDebugSettingsDlg)))
            {
                return true;
            }

            // or if the SHIFT key was held down at launch
            if ((mKeys & Keys.Shift) == Keys.Shift)
            {
                return true;
            }

            // otherwise return false
            return false;
        }

        /// <summary>
        /// Shortcut to System.Diagnostics.Debugger.Log() method.
        /// </summary>
        /// <param name="msg"></param>
        public static void Log(string msg)
        {
            System.Diagnostics.Debugger.Log(0, "", msg + NL);
        }



    }   // class Program
}       // namespace 


