using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SchackMediaViewer
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            App.Log("SettingsWindow.ctor(): entered.");

            InitializeComponent();

            App.Log("SettingsWindow.ctor(): exiting.");
        }

        public SettingsWindow(IntPtr hWnd)
        {
            string hWndToString = "null";
            if (hWnd != null)
            {
                hWndToString = hWnd.ToString();
            }

            App.Log("SettingsWindow.ctor(" + hWndToString + "): entered.");

            InitializeComponent();

            App.Log("SettingsWindow.ctor(" + hWndToString + "): exiting.");
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            App.Log("Window_Activated(): entered.");
            App.Log("Window_Activated(): exited.");

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            App.Log("Window_Closed(): entered.");
            App.Log("Window_Closed(): exited.");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.Log("Window_Closing(): entered.");
            App.Log("Window_Closing(): exited.");
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            App.Log("Window_Deactivated(): entered.");
            App.Log("Window_Deactivated(): exited.");
        }

        private void Window_GotFocus(object sender, RoutedEventArgs e)
        {
            App.Log("Window_GotFocus(): entered.");
            App.Log("Window_GotFocus(): exited.");
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            App.Log("Window_Initialized(): entered.");
            App.Log("Window_Initialized(): exited.");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            App.Log("Window_Loaded(): entered.");
            App.Log("Window_Loaded(): exited.");
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            App.Log("Window_Unloaded(): entered.");
            App.Log("Window_Unloaded(): exited.");
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            App.Log("Window_IsVisibleChanged(): entered.");
            App.Log("Window_IsVisibleChanged(): exited.");
        }

        private void Window_LostFocus(object sender, RoutedEventArgs e)
        {
            App.Log("Window_LostFocus(): entered.");
            App.Log("Window_LostFocus(): exited.");
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            App.Log("Window_StateChanged(): entered.");
            App.Log("Window_StateChanged(): exited.");
        }

    }
}