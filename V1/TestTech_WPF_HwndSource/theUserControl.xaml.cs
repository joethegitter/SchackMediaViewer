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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Interop;
//using JoeKCo.Utilities;

namespace TestTech_WPF_HwndSource
{
    /// <summary>
    /// Interaction logic for theUserControl.xaml
    /// </summary>
    public partial class theUserControl : UserControl
    {
        public theUserControl()
        {
            Log("theUserControl(): entered.");
            Log("   theUserControl(): calling InitializeComponent()...");
            InitializeComponent();
            Log("theUserControl(): exiting.");
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Log("UserControl_Loaded(): entered.");

            Log("UserControl_Loaded(): exiting.");
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Log("UserControl_IsVisibleChanged(): entered.");
            Log("UserControl_IsVisibleChanged(): exiting.");
        }

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            Log("UserControl_Initialized(): entered.");
            Log("UserControl_Initialized(): exiting.");
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Log("UserControl_SizeChanged(): entered.");
            Log("UserControl_SizeChanged(): exiting.");
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Log("UserControl_Unloaded(): entered.");
            Log("UserControl_Unloaded(): exiting.");
        }


        void Log(string msg)
        {
            System.Diagnostics.Debugger.Log(0, "", msg + System.Environment.NewLine);
        }
    }
}
