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
using MudSharp.Database;
using MudSharp.RPG.Economy;

namespace Futuremud_Configuration_Tool.UI {
    /// <summary>
    /// Interaction logic for CurrencyDescriptionPatternElementViewer.xaml
    /// </summary>
    public partial class CurrencyDescriptionPatternElementViewer : UserControl {
        public CurrencyDescriptionPatternElementViewer() {
            InitializeComponent();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
            if (e.NewValue != null) {
                var newValue = (FME.CurrencyDescriptionPatternElement)e.NewValue;
                CurrencyDivisionComboBox.ItemsSource = FMDB.Context.CurrencyDescriptionPatterns.First(x => x.CurrencyDescriptionPatternElements.Any(y => y.Id == newValue.Id)).Currency.CurrencyDivisions;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e) {
            foreach (int value in Enum.GetValues(typeof(RoundingMode))) {
                RoundingModeComboBox.Items.Add(new Converters.RoundingTypeToStringConverter().Convert(value, typeof(string), null, null));
            }
        }

        private void Button_Click_Add(object sender, RoutedEventArgs e) {
            var newValue = FMDB.Context.CurrencyDescriptionPatternElementSpecialValues.Create();
            FMDB.Context.CurrencyDescriptionPatternElementSpecialValues.Add(newValue);
            ((FME.CurrencyDescriptionPatternElement)DataContext).CurrencyDescriptionPatternElementSpecialValues.Add(newValue);
        }

        private void Button_Click_Delete(object sender, RoutedEventArgs e) {
            var deleting = (FME.CurrencyDescriptionPatternElementSpecialValue)SpecialValueListBox.SelectedItem;
            FMDB.Context.CurrencyDescriptionPatternElementSpecialValues.Remove(deleting);
            ((FME.CurrencyDescriptionPatternElement)DataContext).CurrencyDescriptionPatternElementSpecialValues.Remove(deleting);
        }
    }
}
