using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using System.Windows.Interop;
using JoeKCo.Utilities;

namespace SchackMediaViewer
{
    /// <summary>
    /// Application Object and Entry Point
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Opens the appropriate Media Viewer window for the requested LaunchMode.
        /// </summary>
        /// <param name="LaunchMode"></param>
        /// <param name="hWnd"></param>
        private void OpenAppropriateWindow(LaunchModality LaunchMode, IntPtr hWnd)
        {
            // Based on Launch Mode, show the correct window in the correct place
            if (LaunchMode == LaunchModality.DT_Configure)
            {
                Log("   OnStartup(): LaunchMode = DT_Configure");
                ShowSettings();
            }
            else if (LaunchMode == LaunchModality.CP_Configure)
            {
                Log("   OnStartup(): LaunchMode = CP_Configure");
                ShowSettings(hWnd);
            }
            else if (LaunchMode == LaunchModality.ScreenSaver)
            {
                Log("   OnStartup(): LaunchMode = ScreenSaver");
                ShowScreenSaver(false);
            }
            else if (LaunchMode == LaunchModality.ScreenSaverWindowed)
            {
                Log("   OnStartup(): LaunchMode = ScreenSaverWindowed");
                ShowScreenSaver(true);
            }
            else if (LaunchMode == LaunchModality.CP_MiniPreview)
            {
                Log("   OnStartup(): LaunchMode = CP_MiniPreview");
                ShowPreview(hWnd);
            }
            else if (LaunchMode == LaunchModality.Undecided)
            {
                Log(" ** OnStartup() Error: LaunchMode == LaunchModality.Undecided");

#if DEBUG
                // if we are in DEBUG and there's a debugger attached, offer to break into it
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    Log(" ** OnStartup() Breaking into debugger...");

                    MessageBoxResult mbr = MessageBox.Show("Apparently we are still in LaunchMode.Undecided. Cancel to break into debugger, or OK to launch in Full Screen mode." +
                        Environment.NewLine + Environment.NewLine + "CommandLine: " + Environment.CommandLine,
                        this.ProductName, MessageBoxButton.OKCancel, MessageBoxImage.Exclamation);

                    if (mbr == MessageBoxResult.Cancel)
                    {
                        System.Diagnostics.Debugger.Break();
                    }
                }
#endif

                // In release version, we'll just quietly fail here, and launch in desktop Configure mode
                Log(" ** OnStartup(): we fell through to Mode.Undecided, calling Application.Run().");
                ShowSettings();
                Log(" ** OnStartup()(): exiting for realsies.");
            }
        }

        private void ShowSettings()
        {
        }

        private void ShowSettings(IntPtr hWnd)
        {
        }

        /// <summary>
        /// Show 
        /// </summary>
        /// <param name="InWindowedMode"></param>
        private void ShowScreenSaver(bool InWindowedMode)
        {
        }

        /// <summary>
        /// Show our Preview user control in the little window provided by the Screen Saver control panel.
        /// </summary>
        /// <param name="hWnd">The hWnd passed to us by the Screen Saver control panel.</param>
        private void ShowPreview(IntPtr hWnd)
        {
            Log("ShowPreview(): Entered.");
            Log("   ShowPreview(): cpl hWnd passed to us = " + hWnd);

            if (NativeMethods.IsWindow(hWnd))
            {
                // Get the rect of the desired parent
                int error = 0;
                System.Drawing.Rectangle ParentRect = new System.Drawing.Rectangle();
                NativeMethods.SetLastErrorEx(0, 0);
                Log("  ShowPreview(): Let's Get the ClientRect of that puppy:");
                Log("  ShowPreview(): Calling GetClientRect(" + hWnd + ")...");
                bool fSuccess = NativeMethods.GetClientRect(hWnd, ref ParentRect);
                error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                Log("  ShowPreview(): GetClientRect() returned bool = " + fSuccess + ", rect = " + ParentRect.ToString());
                Log("      GetLastError() returned: " + error.ToString());
                Log(" ");

                // Create the HwndSource which will host our Preview user control
                Log("  ShowPreview(): Let's build a new HwndSource (src), and attach it to the cpl window:");
                HwndSourceParameters parameters = new HwndSourceParameters();
                parameters.WindowStyle = NativeMethods.WindowStyles.WS_CHILD | NativeMethods.WindowStyles.WS_VISIBLE;
                parameters.SetPosition(0, 0);  // in theory, our child will use values relative to parents position
                parameters.SetSize(ParentRect.Width, ParentRect.Height);
                parameters.ParentWindow = hWnd;
                HwndSource src = new HwndSource(parameters);

#if DEBUG
                // Let's see what Windows thinks
                Log("  ShowPreview(): Attached it. Let's see what Windows thinks:");
                Log("  ShowPreview(): Calling GetParent(src.Handle)...");
                NativeMethods.SetLastErrorEx(0, 0);
                IntPtr handle = IntPtr.Zero;
                handle = NativeMethods.GetParent(src.Handle);
                error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                Log("  ShowPreview(): GetParent() returned: " + handle.ToString());
                Log("      GetLastError() returned: " + error.ToString());
                Log(" ");
#endif

                // Create the user control and attach it
                Log("  ShowPreview(): Creating the user control (Preview)");
                PreviewControl Preview = new PreviewControl();
                Log("  ShowPreview(): setting src.RootVisual to user control...");
                src.RootVisual = Preview;
                Preview.Visibility = Visibility.Visible;

#if DEBUG
                // Let's find out what Windows thinks
                Log("  ShowPreview(): Set it. Let's see what Windows thinks the HwndSource for Preview is:");
                HwndSource hs = (HwndSource)HwndSource.FromVisual(Preview);
                Log("  ShowPreview(): HwndSource.FromVisual(Preview) is: " + HwndSource.FromVisual(Preview));
                Log("  ShowPreview(): Let's see what Windows thinks the parent hWnd of Preview is:");
                Log("  ShowPreview(): Calling GetParent((HwndSource)HwndSource.FromVisual(Preview).handle)...");
                NativeMethods.SetLastErrorEx(0, 0);
                IntPtr ucHandle = IntPtr.Zero;
                handle = NativeMethods.GetParent(hs.Handle);
                error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                Log("  ShowPreview(): GetParent() returned: " + handle.ToString());
                Log("      GetLastError() returned: " + error.ToString());
                Log(" ");

                Log("  ShowPreview(): Is the src window visible?");
                Log("  ShowPreview(): Calling IsWindowVisible(src.Handle)...");
                NativeMethods.SetLastErrorEx(0, 0);
                bool fVisible = NativeMethods.IsWindowVisible(src.Handle);
                error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                Log("  ShowPreview(): IsWindowVisible() returned: " + fVisible.ToString());
                Log("      GetLastError() returned: " + error.ToString());
                Log(" ");
#endif

                // Let's hook into src's message pump
                Log("  ShowPreview(): Let's hook into src's message pump");
                // TODO: determine if we need to hook into message pump

            }
            else
            {
                Log("  ShowPreview(): Invalid hWnd passed: " + hWnd.ToString());
                throw new ArgumentException("Invalid hWnd passed to ShowPreview(): " + hWnd.ToString());
            }

            Log("ShowPreview(): Exited.");
        }

    }
}