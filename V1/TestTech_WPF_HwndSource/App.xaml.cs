using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using System.Windows.Interop;
using JoeKCo.Utilities;

namespace TestTech_WPF_HwndSource
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
            IntPtr hWnd = GetIntPtrFromArg(mainArgs);
            ShowPreviewFromHwndSource(hWnd);


            Log("OnStartup(): exiting.");
        }

        void ShowPreviewFromHwndSource(IntPtr hWnd)
        {
            Log("ShowPreviewFromHwndSource(): Entered.");
            Log("   ShowPreviewFromHwndSource(): cpl hWnd passed to us = " + hWnd);

            if (NativeMethods.IsWindow(hWnd))
            {
                // Get the rect of the desired parent
                int error = 0;
                System.Drawing.Rectangle ParentRect = new System.Drawing.Rectangle();
                NativeMethods.SetLastErrorEx(0, 0);
                Log("  ShowPreviewFromHwndSource(): Let's Get the ClientRect of that puppy:");
                Log("  ShowPreviewFromHwndSource(): Calling GetClientRect(" + hWnd + ")...");
                bool fSuccess = NativeMethods.GetClientRect(hWnd, ref ParentRect);
                error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                Log("  ShowPreviewFromHwndSource(): GetClientRect() returned bool = " + fSuccess + ", rect = " + ParentRect.ToString());
                Log("      GetLastError() returned: " + error.ToString());
                Log(" ");

                // Create the HwndSource which will host our Window
                Log("  ShowPreviewFromHwndSource(): Let's build a new HwndSource (src), and attach it to the cpl window:");
                HwndSourceParameters parameters = new HwndSourceParameters();
                parameters.WindowStyle = NativeMethods.WindowStyles.WS_CHILD | NativeMethods.WindowStyles.WS_VISIBLE;
                parameters.SetPosition(0, 0);  // in theory, our child will use values relative to parents position
                parameters.SetSize(ParentRect.Width, ParentRect.Height);
                parameters.ParentWindow = hWnd;
                HwndSource src = new HwndSource(parameters);

                // Let's see what Windows thinks
                Log("  ShowPreviewFromHwndSource(): Attached it. Let's see what Windows thinks:");
                Log("  ShowPreviewFromHwndSource(): Calling GetParent(src.Handle)...");
                NativeMethods.SetLastErrorEx(0, 0);
                IntPtr handle = IntPtr.Zero;
                handle = NativeMethods.GetParent(src.Handle);
                error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                Log("  ShowPreviewFromHwndSource(): GetParent() returned: " + handle.ToString());
                Log("      GetLastError() returned: " + error.ToString());
                Log(" ");

                // Create the user control
                Log("  ShowPreviewFromHwndSource(): Creating the user control (uc)");
                theUserControl uc = new theUserControl();

                Log("  ShowPreviewFromHwndSource(): setting src.RootVisual to user control...");
                src.RootVisual = uc;
                
                uc.Visibility = Visibility.Visible;

                // Let's find out what Windows thinks
                Log("  ShowPreviewFromHwndSource(): Set it. Let's see what Windows thinks the HwndSource for uc is:");
                HwndSource hs = (HwndSource)HwndSource.FromVisual(uc);
                Log("  ShowPreviewFromHwndSource(): HwndSource.FromVisual(uc) is: " + HwndSource.FromVisual(uc));
                Log("  ShowPreviewFromHwndSource(): Let's see what Windows thinks the parent hWnd of uc is:");
                Log("  ShowPreviewFromHwndSource(): Calling GetParent((HwndSource)HwndSource.FromVisual(uc).handle)...");
                NativeMethods.SetLastErrorEx(0, 0);
                IntPtr ucHandle = IntPtr.Zero;
                handle = NativeMethods.GetParent(hs.Handle);
                error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                Log("  ShowPreviewFromHwndSource(): GetParent() returned: " + handle.ToString());
                Log("      GetLastError() returned: " + error.ToString());
                Log(" ");

                

                // Let's find out what Windows thinks
                Log("  ShowPreviewFromHwndSource(): Is the src window visible?");
                Log("  ShowPreviewFromHwndSource(): Calling IsWindowVisible(src.Handle)...");
                NativeMethods.SetLastErrorEx(0, 0);
                bool fVisible = NativeMethods.IsWindowVisible(src.Handle);
                error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                Log("  ShowPreviewFromHwndSource(): IsWindowVisible() returned: " + fVisible.ToString());
                Log("      GetLastError() returned: " + error.ToString());
                Log(" ");



                // Let's hook into src's message pump
                Log("  ShowPreviewFromHwndSource(): Let's hook into src's message pump");




                //HwndSource.FromVisual(uc);

                //NativeMethods.SetLastErrorEx(0, 0);
                //Log("  ShowPreviewFromHwndSource(): Calling GetClientRect(" + hWnd + ")...");
                //fSuccess = NativeMethods.GetClientRect(hWnd, ref ParentRect);
                //Log("  ShowPreviewFromHwndSource(): GetClientRect() returned bool = " + fSuccess + ", rect = " + ParentRect.ToString());
                //Log("      GetLastError() returned: " + error.ToString());
                //Log(" ");






            }
            else
            {
                Log("  ShowPreviewFromHwndSource(): Invalid hWnd passed: " + hWnd.ToString());
                throw new ArgumentException("Invalid hWnd passed to ShowPreviewFromHwndSource(): " + hWnd.ToString());
            }

            Log("ShowPreviewFromHwndSource(): Exited.");
        }   


        private IntPtr GetIntPtrFromArg(string[] mainArgs)
        {
            Log("GetIntPtrFromArg(): entered.");

            IntPtr hWnd = IntPtr.Zero;

            if ((mainArgs.Length > 0) && mainArgs[0].ToLowerInvariant().StartsWith("-"))
            {
                string subArg = mainArgs[0].ToLowerInvariant();
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
                        Log(@"  * GetIntPtrFromArg(): invalid window handle passed: " + longCandidate);
                        throw new ArgumentException(@"CommandLine: invalid window handle passed: " + longCandidate +
                            Environment.NewLine + Environment.CommandLine);
                    }
                }
                else   // null or empty
                {
                    Log(@"  * GetIntPtrFromArg(): null or empty window handle passed.");
                    throw new ArgumentException(@"CommandLine: null or empty window handle passed." +
                        Environment.NewLine + Environment.CommandLine);
                }
            }
            Log("GetIntPtrFromArg(): exiting.");
            return hWnd;
        }

        void Log(string msg)
        {
            System.Diagnostics.Debugger.Log(0, "", msg + System.Environment.NewLine);
        }
    }
}
