using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Futuremud_Configuration_Tool.UI {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        private void Application_DispatcherUnhandledException(object sender,
                       System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {

                           new Model.ExceptionViewer("Exception Caught", e.Exception).ShowDialog();

            //MessageBox.Show(e.Exception.Message, "Exception Caught", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}
