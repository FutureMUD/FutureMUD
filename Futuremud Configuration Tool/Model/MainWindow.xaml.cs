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

namespace Futuremud_Configuration_Tool.Model {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private DisplayControl _displayControl;

        public MainWindow(Initialisation.InitialisationContext context) {
            InitializeComponent();
            _displayControl = new DisplayControl(context);
            SubcontrolGrid.Children.Add(_displayControl);
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e) {
            System.Diagnostics.Process.Start("http://www.futuremud.com");
        }

        private void MenuItem_Click_Colours(object sender, RoutedEventArgs e) {
            _displayControl.SetSubcontrol(new Subcontrols.ColourViewer());
        }

        private void MenuItem_Click_StaticStrings(object sender, RoutedEventArgs e) {
            _displayControl.SetSubcontrol(new Subcontrols.StaticStringViewer());
        }
    }
}
