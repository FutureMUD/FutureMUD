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

namespace Futuremud_Configuration_Tool.UI {
    /// <summary>
    /// Interaction logic for CurrencyDivisionViewer.xaml
    /// </summary>
    public partial class CurrencyDivisionViewer : UserControl {
        public CurrencyDivisionViewer() {
            InitializeComponent();
        }

        private void Button_Click_Add(object sender, RoutedEventArgs e) {
            var newPattern = MudSharp.Database.FMDB.Context.CurrencyDivisionAbbreviations.Create();
            ((FME.CurrencyDivision)DataContext).CurrencyDivisionAbbreviations.Add(newPattern);
        }

        private void Button_Click_Delete(object sender, RoutedEventArgs e) {
            MudSharp.Database.FMDB.Context.CurrencyDivisionAbbreviations.Remove((FME.CurrencyDivisionAbbreviation)PatternListBox.SelectedItem);
        }
    }
}
