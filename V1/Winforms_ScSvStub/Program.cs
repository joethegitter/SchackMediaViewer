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

        #region Configure

        // Use the following variables to control the name and location of 
        // the application to be launched by the stub. These will be combined
        // at the end of the program as TARGET. Note the role of the variables
        // DebugDefined and UseDevPath (defined in #region "Translate Pound
        // Defines to Variables").

        // Arg[0] is the fully qualified path and filename of this stub,
        // so we can steal it's path.
        public static string TARGET_PATH =
            Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
        public static string TARGET_BASE = "SchackMediaViewer";
        public static string TARGET_EXT = ".exe";
        public static string TARGET = "<this is constructed later>";

        // set fAlwaysDescribeKeys to true if you want to *always* see a
        // debugging summary of the modifier key states at stub launch.
        public static bool fAlwaysDescribeKeys = false;

        // set fShowArgsAlways to true if you want to *always* see a debugging 
        // summary of the  incoming and outgoing args, overriding any other
        // factors (key state results, etc).
        public static bool fShowArgsAlways = true;

        #endregion Configure

        #region Data

        // Filename elements, command line args and keystate results (certain 
        // keys were held down when the stub was launched) that our stub 
        // understands to mean - and will pass on as an argument to our app - 
        // "pop up a debug output window some minimum number of seconds after 
        // you start up". Ordinarily a user action (keypress, etc) would be 
        // used to open that window; however, this option allows us to get the 
        // window opened in circumstances where no user input is available 
        // (like when we draw our preview in the control panel).
        public const string FILE_DBGWIN = ".popdbgwin";
        public const string POPDBGWIN = @"/popdbgwin";
        public static bool modifierKeyState_DoPopDBGWin = false;

        // Filename elements, command line args and keystate results (certain 
        // keys were held down when the stub was launched) that our stub 
        // understands to mean - and will pass on as an argument to our app - 
        // "start generating debug output and remember it until we show a 
        // debug output window". Ordinarily we only start generating debug 
        // output after we open a debug output window, which means we can't
        // display any debug output generated during startup.
        public const string FILE_STARTBUFFER = ".startbuffer";
        public const string STARTBUFFER = @"/startbuffer";
        public static bool modifierKeyState_DoStartBuffer = false;

        // Keystate result that tells the stub to show a message box before
        // launching our app. This message box contains the arguments fed to
        // the stub by windows, and the args the stub will feed to our app at
        // launch. The message box also allows us to simply read the data and
        // then cancel the app launch (debugging assistance).
        public static bool modifierKeyState_DoShowArgs = false;

        // Keystate result that tells the stub to ignore any previous TARGET_PATH
        // configuration and only launch the application from stub directory.
        public static bool modifierKeyState_ForceNormalLaunch = false;

        // Command line args that the stub will issue to our application:
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

        #endregion Data


        /// <summary>
        /// The main entry point for the stub.
        /// </summary>
        [STAThread]
        public static void Main(string[] mainArgs)
        {
            // capture state of modifier keys at launch
            Keys mKeys = Control.ModifierKeys;
            bool fAbortLaunch = false;

            string FinalArgs = "";
            string optionalArgs = "";

            // Interpret any Modifier Keys held down at startup
            InterpretModifierKeys(mKeys, ref fAbortLaunch);
            if (fAbortLaunch) Application.Exit();

            // Build the non-screen-saver-specific args
            BuildOptionalArgs(ref optionalArgs);

            // Translate Windows-provided screen saver args into our app args
            TranslateScreenSaverArgs(mainArgs, ref FinalArgs);

            // Combine the primary and optional args
            FinalArgs += optionalArgs;

            // Build the TARGET. Override TARGET_PATH if necessary
            if (DebugDefined && UseDevPath &&
                !modifierKeyState_ForceNormalLaunch)
            {
                TARGET_PATH = @"C:\Users\LocallyMe\Source\Repos\" +
                    @"SchackMediaViewer\V1\SchackMediaViewer\bin\Debug";
            }
            TARGET = Path.Combine(TARGET_PATH, TARGET_BASE + TARGET_EXT);

            // Optionally show message box with incoming and outgoing args
            if (modifierKeyState_DoShowArgs || fShowArgsAlways)
            {
                DialogResult dr = MessageBox.Show("Incoming cmdLine: " + 
                    System.Environment.CommandLine + Environment.NewLine +
                    Environment.NewLine +
                    "Outgoing cmdLine: " + TARGET + " " + FinalArgs + 
                    Environment.NewLine + Environment.NewLine +
                    "Click OK to launch, Cancel to abort." + 
                    Environment.NewLine + Environment.NewLine,
                    Application.ProductName, MessageBoxButtons.OKCancel, 
                    MessageBoxIcon.Information);

                // if user clicks Cancel, don't launch the exe
                if (dr == DialogResult.Cancel)
                {
                    Application.Exit();
                }
            }

            // Is the application where we think it is? If not bail.
            if (!File.Exists(TARGET))  // 
            {
                DialogResult dr = MessageBox.Show("File not found: " + 
                    TARGET, Application.ProductName, MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Application.Exit();
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
            if (FinalArgs.Contains(M_CP_CONFIGURE))
            {
                proc = System.Diagnostics.Process.Start(TARGET, FinalArgs);
                proc.WaitForExit();  // don't let stub die until app dies
                return;
            }
            else  // in all other cases, fire and forget
            {
                proc = System.Diagnostics.Process.Start(TARGET, FinalArgs);
                return;
            }
        }

        /// <summary>
        /// Gets modifier key states and sets corresponding flags.
        /// </summary>
        private static void InterpretModifierKeys(Keys mKeys, ref bool fAbortLaunch)
        {
            // Shift Key means pop up the debug window
            if ((mKeys & Keys.Shift) == Keys.Shift)
            {
                modifierKeyState_DoPopDBGWin = true;
            }

            // Control Key means start the debug buffer
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                modifierKeyState_DoStartBuffer = true;
            }

            // Alt Key means show the args message box
            if ((Control.ModifierKeys & Keys.Alt) == Keys.Alt)
            {
                modifierKeyState_DoShowArgs = false;
            }

            // set fAlwaysDescribeKeys to true to force this debug dialog up
            if (fAlwaysDescribeKeys)
            {
                string msg = "Keys States: " + Environment.NewLine +
                    Environment.NewLine + 
                    "Shift = " + modifierKeyState_DoPopDBGWin  +
                    Environment.NewLine +
                    "Control = " + modifierKeyState_DoStartBuffer +
                    Environment.NewLine +
                    "Alt = " + modifierKeyState_DoShowArgs +
                    Environment.NewLine +
                    "Caps Lock = " + Control.IsKeyLocked(Keys.CapsLock) +
                    Environment.NewLine +
                    "Scroll Lock = " + Control.IsKeyLocked(Keys.Scroll);
                DialogResult dr = MessageBox.Show(msg, 
                    Application.ProductName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

            }

            // If DEBUG was defined, see if we need to force normal launch
            if (DebugDefined)
            {
                if (Control.IsKeyLocked(Keys.Scroll) && 
                    Control.IsKeyLocked(Keys.CapsLock))
                {
                    string msg = "Debug Build: Caps Lock and Scroll " +
                        "Lock are both locked. Force non-debug launch?" +
                        Environment.NewLine + Environment.NewLine +
                        "New path to exe will be: " + TARGET_PATH;
                    DialogResult dr = MessageBox.Show(msg, 
                        Application.ProductName, 
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Warning);

                    if (dr == DialogResult.Cancel)
                    {
                        fAbortLaunch = true;
                    }
                    else if (dr == DialogResult.Yes)
                    {
                        modifierKeyState_ForceNormalLaunch = true;
                    }
                    else
                    {
                        modifierKeyState_ForceNormalLaunch = false;
                    }
                }
            }
        }

        /// <summary>
        /// Builds non-screen-related arguments passed to target application 
        /// </summary>
        private static void BuildOptionalArgs(ref string optionalArgs)
        {
            // RARE, but first priority: user can modify stub filename to get 
            // certain behaviors. Check if the filename has been changed, and
            // force post arguments.
            if (Environment.GetCommandLineArgs()[0].ToLowerInvariant().
                Contains(FILE_DBGWIN.ToLowerInvariant()))
            {
                optionalArgs += " " + POPDBGWIN;
            }

            // only one of the two filename-based optionalArgs is allowed, and 
            // POPDBGWIN takes precedence. So if optionalArgs is still empty...
            if (optionalArgs == "")
            {
                if (Environment.GetCommandLineArgs()[0].ToLowerInvariant().
                    Contains(FILE_STARTBUFFER.ToLowerInvariant()))
                {
                    optionalArgs += " " + STARTBUFFER;
                }
            }

            // ONLY if the filename was not modified, check to see if there's 
            // a keystate result which impacts the postargs:
            if (optionalArgs == "")
            {
                if (modifierKeyState_DoPopDBGWin)
                    optionalArgs += " " + POPDBGWIN;
                if (modifierKeyState_DoStartBuffer)
                    optionalArgs += " " + STARTBUFFER;
            }
        }

        /// <summary>
        /// Builds screen-saver related arguments passed to target application
        /// </summary>
        private static void TranslateScreenSaverArgs(string[] mainArgs, ref string FinalArgs)
        {
            string mode = "";
            bool fHasWindowHandle = false;
            string windowHandle = "";

            // Examine incoming args and build outgoing args.
            // When Windows launches a screen saver, it sends specific 
            // command line args. Those will only ever be the following
            // (EB = the behavior that Windows expects):
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
                // can only be /P windowHandle (note space)
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
                Application.Exit();
            }

            // Finish outgoing command line
            FinalArgs = FROMSTUB + " " + mode;
            if (fHasWindowHandle)
            {
                FinalArgs = FinalArgs + " -" + windowHandle;
            }
        }

        #region Translate Pound Defines to Variables

        #if DEBUG
                public static bool DebugDefined = true;
            #if LAUNCH_APP_FROM_DEV_PATH
                    public static bool UseDevPath = true;
            #else
                                    public static bool UseDevPath = true;
            #endif
        #else
            public static bool DebugDefined = false;
        #endif

        #endregion Translate Pound Defines to Variables


    }
}
