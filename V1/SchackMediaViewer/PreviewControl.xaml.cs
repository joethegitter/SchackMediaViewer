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

using JoeKCo.Utilities;

namespace SchackMediaViewer
{
    /// <summary>
    /// Interaction logic for PreviewControl.xaml
    /// </summary>
    public partial class PreviewControl : UserControl
    {
        public PreviewControl()
        {
            App.Log("PreviewControl(): entered.");
            InitializeComponent();
            App.Log("PreviewControl(): exiting.");
        }

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            App.Log("UserControl_Initialized(): entered.");
            App.Log("UserControl_Initialized(): exiting.");
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            App.Log("UserControl_Loaded(): entered.");
            App.Log("UserControl_Loaded(): exiting.");
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            App.Log("UserControl_Unloaded(): entered.");

            App.Current.Shutdown();

            App.Log("UserControl_Unloaded(): exiting.");
        }


        private void Grid_Unloaded(object sender, RoutedEventArgs e)
        {
            App.Log("Grid_Unloaded(): entered.");
            App.Log("Grid_Unloaded(): exiting.");
        }

        private void Grid_Initialized(object sender, EventArgs e)
        {
            App.Log("Grid_Initialized(): entered.");
            App.Log("Grid_Initialized(): exiting.");
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            App.Log("Grid_Loaded(): entered.");
            App.Log("Grid_Loaded(): exiting.");
        }


    }
}
