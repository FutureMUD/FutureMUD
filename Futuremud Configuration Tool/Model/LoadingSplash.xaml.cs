using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
    /// Interaction logic for LoadingSplash.xaml
    /// </summary>
    public partial class LoadingSplash : Window {
        private Initialisation.InitialisationContext _context = new Initialisation.InitialisationContext();

        public LoadingSplash() {
            InitializeComponent();
        }

        public class ConsoleTextBoxBinding : INotifyPropertyChanged {
            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            private void NotifyPropertyChanged(string property) {
                if (PropertyChanged != null) {
                    PropertyChanged(this, new PropertyChangedEventArgs(property));
                }
            }

            private string _text = "";
            public string Text {
                get { return _text; }
                set { _text = value; NotifyPropertyChanged("Text"); }
            }

            #endregion
        }

        public ConsoleTextBoxBinding BindingObject {
            get;
            set;
        }

        public class TextBoxStreamWriter : TextWriter {
            LoadingSplash _parent;
            public TextBoxStreamWriter(LoadingSplash parent) {
                _parent = parent;
            }

            public override void Write(string value) {
                _parent.BindingObject.Text += value;
            }

            public override void WriteLine(string value) {
                _parent.BindingObject.Text += value + "\r\n";
            }

            public override Encoding Encoding {
                get { return System.Text.Encoding.Unicode; }
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e) {
            BindingObject = new ConsoleTextBoxBinding();
            DataContext = BindingObject;
            Console.SetOut(new TextBoxStreamWriter(this));
            await _context.Initialise();
            LaunchButton.IsEnabled = true;
        }

        private void LaunchButton_Click(object sender, RoutedEventArgs e) {
            var window = new MainWindow(_context);
            window.Visibility = System.Windows.Visibility.Visible;
            window.Focus();
            this.Close();
        }

        private void ConsoleOutputTextBox_TextChanged(object sender, TextChangedEventArgs e) {
            ConsoleOutputTextBox.ScrollToEnd();
        }
    }
}
