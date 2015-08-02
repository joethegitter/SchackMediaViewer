using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace WinForm_Launcher
{
    public class NativeMethods
    {
        // Set Last Error
        [DllImport("user32.dll")]
        internal static extern void SetLastErrorEx(uint dwErrCode, uint dwType);

        #region MessageBox

        // Import the Win32 MessageBox API, so we can call it
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern MessageBoxResult MessageBox(IntPtr hWnd, String text, String caption, MessageBoxOptions options);

        ///<summary>
        /// Flags that define appearance and behaviour of a standard message box displayed by a call to the MessageBox function.
        /// </summary>    
        [Flags]
        public enum MessageBoxOptions : uint
        {
            OkOnly = 0x000000,
            OkCancel = 0x000001,
            AbortRetryIgnore = 0x000002,
            YesNoCancel = 0x000003,
            YesNo = 0x000004,
            RetryCancel = 0x000005,
            CancelTryContinue = 0x000006,
            IconHand = 0x000010,
            IconQuestion = 0x000020,
            IconExclamation = 0x000030,
            IconAsterisk = 0x000040,
            UserIcon = 0x000080,
            IconWarning = IconExclamation,
            IconError = IconHand,
            IconInformation = IconAsterisk,
            IconStop = IconHand,
            DefButton1 = 0x000000,
            DefButton2 = 0x000100,
            DefButton3 = 0x000200,
            DefButton4 = 0x000300,
            ApplicationModal = 0x000000,
            SystemModal = 0x001000,
            TaskModal = 0x002000,
            Help = 0x004000,
            NoFocus = 0x008000,
            SetForeground = 0x010000,
            DefaultDesktopOnly = 0x020000,
            Topmost = 0x040000,
            Right = 0x080000,
            RTLReading = 0x100000
        }

        /// <summary>
        /// Represents possible values returned by the MessageBox function.
        /// </summary>
        public enum MessageBoxResult : uint
        {
            Ok = 1,
            Cancel,
            Abort,
            Retry,
            Ignore,
            Yes,
            No,
            Close,
            Help,
            TryAgain,
            Continue,
            Timeout = 32000
        }

        #endregion MessageBox

        #region Window Management

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
        internal static extern IntPtr GetAncestor(IntPtr hWnd, GetAncestorFlags flags);

        internal enum GetAncestorFlags
        {
            /// <summary>
            /// Retrieves the parent window. This does not include the owner, as it does with the GetParent function.
            /// </summary>
            GetParent = 1,
            /// <summary>
            /// Retrieves the root window by walking the chain of parent windows.
            /// </summary>
            GetRoot = 2,
            /// <summary>
            /// Retrieves the owned root window by walking the chain of parent and owner windows returned by GetParent.
            /// </summary>
            GetRootOwner = 3
        }

        #endregion Window Management
    }
}
