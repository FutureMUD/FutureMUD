using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Futuremud_Configuration_Tool.UI {

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public MainWindow() {
            InitializeComponent();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e) {
            ProgramReadyImage.Source = new BitmapImage(new Uri(@"../Images/Tick_Icon.png", UriKind.Relative));
            ProgramReadyLabel.Content = "Program Ready";
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        }
    }
}