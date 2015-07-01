using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.IO;

// BUILD NOTE - do not change the build option to "Prefer 32 bit", as Windows 
//    will not let apps compiled with this switch launch from the System32
//    directory. If you get the "This application could not be started" error,
//    this is likely why.

namespace SchackSaverStub
{
    #region Documentation

    // 1. This is a stub .scr file. Windows thinks that it is our screen saver;
    //    in reality, it is just a little stub that launches our application
    //    with the appropriate screen-saver related arguments.
    // 2. Although this stub project is a WinForms project, it contains no 
    //    forms, just the Main() method. We use WinForms project instead of
    //    Console project because the Console project will flash up a console
    //    window at startup.
    // 3. A custom build action causes this file to be copied to *.scr after
    //    a successful build. Copy it to your Windows/System32 directory to
    //    make it show up as a Screen Saver in the control panel.
    // 4. By holding down certain keys when stub launches (or by renaming the
    //    stub in specific ways before launch), the user can specify these 
    //    options:
    //    a. ALT key down:
    //       the stub should, before launching the app, present a dialog that
    //       shows the arguments it received from Windows, plus the arguments 
    //       that it intends to pass on to the application, with an OK and a
    //       Cancel button. OK launches the app, Cancel simply quits.
    //    b. CONTROL key down, or filename has ".startbuffer" anywhere in it
    //       before the extension:
    //       the stub should pass on to the app a request that the app begins
    //       recording all debug logging output to a buffer. That way, if the 
    //       user wants to review debug output later, it will be present when
    //       the user shows the built in debug output window (or when the user
    //       attaches a debugger).
    //    c. SHIFT key down, or filename has ".popdbgwin" anywhere in it
    //       before the extension:
    //       the stub should pass on to the app a request that the app opens 
    //       up the debug output window 6 seconds after it is launched. This
    //       allows the user to see debug output in situations where the 
    //       debug output windows cannot be requested directly (for example, 
    //       the app is being displayed in the control panel window, and there-
    //       for cannot receive keyboard / mouse / touch input). This option
    //       overrides the CONTROL key option as well (buffer will be started
    //       immediately).

    #endregion Documentation

    static class StubScr
    {

        #region Data

        // If DEBUG and LAUNCH_APP_FROM_DEV_PATH are both is defined, edit 
        // this PATH variable to optionally launch the debug version of 
        // your app from your development directory. Otherwise, the stub will 
        // expect to find your application in the same directory as the stub.
#if DEBUG
    #if LAUNCH_APP_FROM_DEV_PATH
            public static string PATH = @"C:\Users\LocallyMe\Documents\Visual Studio 2013\Projects\SchackSvr\SchackSvr\bin\Debug";
    #else
            public static string PATH = System.IO.Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
    #endif
#else
        // fully qualified path and filename of executing exe is in arg[0]
        public static string PATH = 
            Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
#endif

        // Filename of the application the stub will launch
        public static string TARGET_BASE = "SchackSvr";
        public static string TARGET_EXT = ".exe";
        public static string TARGET = 
            System.IO.Path.Combine(PATH,TARGET_BASE + TARGET_EXT);

        // Filename elements, command line args and keystates that tell our
        // application to pop up debugOutputWindow on a timer after launch.
        public const string FILE_DBGWIN = ".popdbgwin";
        public const string POPDBGWIN = @"/popdbgwin";
        public static bool fShiftKeyOnly = false;

        // Filename elements, command line args and keystates that tell our
        // application to immediately start storing debug output in a 
        // a buffer at launch (we normally only start when the debug window
        // is opened, so we miss startup data)
        public const string FILE_STARTBUFFER = ".startbuffer";
        public const string STARTBUFFER = @"/startbuffer";
        public static bool fControlKeyOnly = false;

        // Command line args that the stub will issue to your application:
        // 1. tells app that it was launched from the screen saver stub
        public const string FROMSTUB = @"/scr";
        // 2. tells app to open settings dlg in control panel
        public const string M_CP_CONFIGURE = @"/cp_configure";
        // 3. tells app to draw screen saver in tiny control panel preview
        public const string M_CP_MINIPREVIEW = @"/cp_minipreview";
        // 4. tells app to open settings dlg on desktop
        public const string M_DT_CONFIGURE = @"/dt_configure";  
        // 5. tells app to open the screen saver in full screen
        public const string M_SCREENSAVER = @"/screensaver";        

        // Keystate that tells us to show the args received by the stub plus
        // the launch string the stub will use to launch your application in a
        // message box, before launching our app.
        public static bool fAltKeyOnly = false;

        #endregion Data

        /// <summary>
        /// The main entry point for the stub.
        /// </summary>
        [STAThread]
        static void Main(string[] mainArgs)
        {
            string debugOutput = "";
            string scrArgs = "";
            string mode = "";
            bool fHasWindowHandle = false;
            string windowHandle = "";

            // The incoming command line to this stub will ONLY ever be 
            // the folling values (EB = Expected Behavior):
            //  /S                - EB: run screensaver in fullscreen
            //                      so we pass /screensaver to our app
            //  /P windowHandle   - EB: show little Preview in control panel
            //                      so we pass /cp_minipreview -windowHandle  
            //  no args           - EB: show Settings dlg on desktop
            //                      so we pass /dt_configure
            //  /C                - EB: show Settings dlg on desktop
            //                      so we pass /dt_configure
            //  /C:windowHandle   - EB: show Settings modal to control panel
            //                      so we pass /cp_configure -windowHandle

            // Capture the state of the Shift key and Control Key
            // and ALT keys at stub launch.
            // Note that in this implementation, we are checking to see if 
            // each key is the ONLY modifier key being pressed. Combinations 
            // of these keys will do nothing.
            if (Control.ModifierKeys == Keys.Alt) fAltKeyOnly = true;
            if (Control.ModifierKeys == Keys.Shift) fShiftKeyOnly = true;
            if (Control.ModifierKeys == Keys.Control) fControlKeyOnly = true;

            // RARE, but first priority: user can modify filename to get 
            // certain behaviors. Check if the filename has been changed, 
            // in order to force post arguments.
            string postArgs = "";
            if (Environment.GetCommandLineArgs()[0].ToLowerInvariant().
                Contains(FILE_DBGWIN.ToLowerInvariant()))
            {
                postArgs += " " + POPDBGWIN;
            }

            // only one of the two filename-based postArgs is allowed, and 
            // POPDBGWIN takes precedence. So if postArgs is still empty...
            if (postArgs == "")
            {
                if (Environment.GetCommandLineArgs()[0].ToLowerInvariant().
                    Contains(FILE_STARTBUFFER.ToLowerInvariant()))
                {
                    postArgs += " " + STARTBUFFER;
                }
            }

            // If filename was not modified, check which keys were held 
            // down at stub launch
            if (postArgs == "")
            {
                // these are exclusive
                if (fShiftKeyOnly) postArgs += " " + POPDBGWIN;
                if (fControlKeyOnly) postArgs += " " + STARTBUFFER;
            }

            // Examine incoming args and build outgoing args.
            if (mainArgs.Length < 1) // no args
            {
                mode = M_DT_CONFIGURE;
            }
            else if (mainArgs.Length < 2) // 1 arg
            {
                // can only be:
                //  /S or 
                //  /C or 
                //  /C:windowHandle  (note: single arg, no space in it)

                // these are exclusive, only one will ever be true
                if (mainArgs[0].ToLowerInvariant().
                    Trim() == @"/s") mode = M_SCREENSAVER;
                if (mainArgs[0].ToLowerInvariant().
                    Trim() == @"/c") mode = M_DT_CONFIGURE;

                if (mainArgs[0].ToLowerInvariant().Trim().StartsWith(@"/c:"))
                {
                    // get the chars after /c: for the windowHandle
                    mode = M_CP_CONFIGURE;
                    fHasWindowHandle = true;
                    windowHandle = mainArgs[0].Substring(3);
                }

            }
            else if (mainArgs.Length < 3) // 2 args
            {
                // can only be /P windowHandle
                mode = M_CP_MINIPREVIEW;
                fHasWindowHandle = true;
                windowHandle = mainArgs[1];
            }
            else
            {
                string msg = 
                    "CommandLine had more than 2 arguments, could not parse.";
                DialogResult dr = MessageBox.Show(msg,
                    Application.ProductName, MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Information);
                return;
            }

            // Finish outgoing command line
            scrArgs = FROMSTUB + " " + mode;
            if (fHasWindowHandle)
            {
                scrArgs = scrArgs + " -" + windowHandle;
            }
            scrArgs += postArgs;

            // Decide whether to put up message box showing command line args.
            // Change fShowArgsAlways to true if *always* want message box.
            bool fShowArgsAlways = false;

            if (fAltKeyOnly || fShowArgsAlways)
            {
                DialogResult dr = MessageBox.Show("Incoming cmdLine: " + 
                    System.Environment.CommandLine + Environment.NewLine +
                    Environment.NewLine +
                    "Outgoing cmdLine: " + TARGET + " " + scrArgs + 
                    Environment.NewLine + Environment.NewLine +
                    "Click OK to launch, Cancel to abort." + 
                    Environment.NewLine + Environment.NewLine + debugOutput,
                    Application.ProductName, MessageBoxButtons.OKCancel, 
                    MessageBoxIcon.Information);

                // if user clicks Cancel, don't launch the exe
                if (dr == DialogResult.Cancel)
                {
                    return;
                }
            }

            // Is the application where we think it is?
            if (!File.Exists(TARGET))  // 
            {
                DialogResult dr = MessageBox.Show("File not found: " + 
                    TARGET, Application.ProductName, MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            // In the M_CP_CONFIGURE case: don't let the stub process die 
            // until the instance of our app running the Settings dialog dies. 
            // Why?
            // 1. expected screen saver behavior is that the control panel 
            //    Preview window stops drawing while the Settings dialog is
            //    up. If you let the stub process die, Windows immediately
            //    launches the Preview instance again, causing it to run in
            //    the background while the Settings dialog is still up.
            // 2. because of 1 above, when the Settings dialog is dismissed,
            //    the Preview instance does not update with the changes made
            //    from the Settings dialog.
            System.Diagnostics.Process proc = null;
            if (mode == M_CP_CONFIGURE)
            {
                proc = System.Diagnostics.Process.Start(TARGET, scrArgs);
                proc.WaitForExit();  // don't let stub die until app dies
                return;
            }
            else  // in all other cases, fire and forget
            {
                proc = System.Diagnostics.Process.Start(TARGET, scrArgs);
                return;
            }
        }
    }
}
