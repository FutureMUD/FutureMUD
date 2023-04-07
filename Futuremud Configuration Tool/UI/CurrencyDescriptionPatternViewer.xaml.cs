using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using MudSharp.FutureProg;
using MudSharp.RPG.Economy;

namespace Futuremud_Configuration_Tool.UI {
    /// <summary>
    /// Interaction logic for CurrencyDescriptionPatternViewer.xaml
    /// </summary>
    public partial class CurrencyDescriptionPatternViewer : UserControl {
        public CurrencyDescriptionPatternViewer() {
            InitializeComponent();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
            if (e.NewValue != null) {
                FutureProgComboBox.ItemsSource = new BindingList<object>(new object[] { Converters.FutureProgNullableConverter.DummyProg }.Concat(MudSharp.Database.FMDB.Context.FutureProgs.Where(x => x.ReturnType == (long)FutureProgVariableTypes.Boolean && x.FutureProgs_Parameters.Count == 1 && x.FutureProgs_Parameters.FirstOrDefault().ParameterType == (long)FutureProgVariableTypes.Number)).ToList());
                ElementListBox.ItemsSource = ((FME.CurrencyDescriptionPattern)DataContext).CurrencyDescriptionPatternElements;
                ElementListBox.SelectedIndex = 0;
            }
            else {
                ElementListBox.ItemsSource = null;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e) {
            TypeComboBox.Items.Add((int)CurrencyDescriptionPatternType.Casual);
            TypeComboBox.Items.Add((int)CurrencyDescriptionPatternType.Short);
            TypeComboBox.Items.Add((int)CurrencyDescriptionPatternType.ShortDecimal);
            TypeComboBox.Items.Add((int)CurrencyDescriptionPatternType.Long);
        }

        private void Button_Click_Add(object sender, RoutedEventArgs e) {
            var newPattern = MudSharp.Database.FMDB.Context.CurrencyDescriptionPatternElements.Create();
            ((FME.CurrencyDescriptionPattern)DataContext).CurrencyDescriptionPatternElements.Add(newPattern);
            ElementListBox.ItemsSource = ((FME.CurrencyDescriptionPattern)DataContext).CurrencyDescriptionPatternElements;
        }

        private void Button_Click_Delete(object sender, RoutedEventArgs e) {
            MudSharp.Database.FMDB.Context.CurrencyDescriptionPatternElements.Remove((FME.CurrencyDescriptionPatternElement)ElementListBox.SelectedItem);
            ((FME.CurrencyDescriptionPattern)DataContext).CurrencyDescriptionPatternElements.Remove((FME.CurrencyDescriptionPatternElement)ElementListBox.SelectedItem);
            ElementListBox.ItemsSource = ((FME.CurrencyDescriptionPattern)DataContext).CurrencyDescriptionPatternElements;
        }
    }
}
