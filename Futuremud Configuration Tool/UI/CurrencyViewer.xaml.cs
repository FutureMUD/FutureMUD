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
using FME;
using MudSharp.Database;

namespace Futuremud_Configuration_Tool.UI {
    /// <summary>
    /// Interaction logic for CurrencyViewer.xaml
    /// </summary>
    public partial class CurrencyViewer : UserControl {
        BindingList<FME.Currency> SelectionList;
        FMDB Database = new FMDB();
        readonly FuturemudDatabaseContext Context = FMDB.Context;

        public CurrencyViewer() {
            InitializeComponent();
        }

        public void Initialise() {
            SelectionList = new BindingList<Currency>(FMDB.Context.Currencies.ToList());
            SelectionListBox.ItemsSource = SelectionList;

            ItemViewGrid.DataContext = SelectionListBox.SelectedItem;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Initialise();
        }

        private void SelectionListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            ItemViewGrid.DataContext = SelectionListBox.SelectedItem;
            DivisionListBox.ItemsSource = ((FME.Currency)SelectionListBox.SelectedItem).CurrencyDivisions.ToList();
            DivisionListBox.SelectedIndex = 0;
            DescriptionPatternListBox.ItemsSource = ((FME.Currency)SelectionListBox.SelectedItem).CurrencyDescriptionPatterns.OrderBy(x => x.Type).ThenBy(x => x.Order).ToList();
            DescriptionPatternListBox.SelectedIndex = 0;
        }

        private void Button_Click_SaveAll(object sender, RoutedEventArgs e) {
            Context.SaveChanges();
            Initialise();
        }

        private void Button_Click_LoadAll(object sender, RoutedEventArgs e) {
            Initialise();
        }

        private void Button_Click_Add(object sender, RoutedEventArgs e) {
            var newItem = Context.Currencies.Create();
            SelectionList.Add(newItem);
            FMDB.Context.Currencies.Add(newItem);
            SelectionListBox.SelectedItem = newItem;
        }

        private void Button_Click_Delete(object sender, RoutedEventArgs e) {
            Context.CharacteristicValues.Remove((FME.CharacteristicValue)SelectionListBox.SelectedItem);
            SelectionList.Remove((FME.Currency)SelectionListBox.SelectedItem);
        }

        private void Button_Click_Add_Division(object sender, RoutedEventArgs e) {
            var newDivision = Context.CurrencyDivisions.Create();
            newDivision.Name = "unknown";
            newDivision.BaseUnitConversionRate = 1;
            ((FME.Currency)SelectionListBox.SelectedItem).CurrencyDivisions.Add(newDivision);
            DivisionListBox.ItemsSource = ((FME.Currency)SelectionListBox.SelectedItem).CurrencyDivisions;
        }

        private void Button_Click_Delete_Division(object sender, RoutedEventArgs e) {
            Context.CurrencyDivisions.Remove((FME.CurrencyDivision)DivisionListBox.SelectedItem);
            ((FME.Currency)SelectionListBox.SelectedItem).CurrencyDivisions.Remove((FME.CurrencyDivision)DivisionListBox.SelectedItem);
            DivisionListBox.ItemsSource = ((FME.Currency)SelectionListBox.SelectedItem).CurrencyDivisions;
        }

        private void Button_Add_Pattern(object sender, RoutedEventArgs e) {
            var newPattern = Context.CurrencyDescriptionPatterns.Create();
            ((FME.Currency)SelectionListBox.SelectedItem).CurrencyDescriptionPatterns.Add(newPattern);
            DescriptionPatternListBox.ItemsSource = ((FME.Currency)SelectionListBox.SelectedItem).CurrencyDescriptionPatterns.OrderBy(x => x.Type).ThenBy(x => x.Order);
        }

        private void Button_Delete_Pattern(object sender, RoutedEventArgs e) {
            Context.CurrencyDescriptionPatterns.Remove((FME.CurrencyDescriptionPattern)DivisionListBox.SelectedItem);
            ((FME.Currency)SelectionListBox.SelectedItem).CurrencyDescriptionPatterns.Remove((FME.CurrencyDescriptionPattern)DescriptionPatternListBox.SelectedItem);
            DescriptionPatternListBox.ItemsSource = ((FME.Currency)SelectionListBox.SelectedItem).CurrencyDescriptionPatterns.OrderBy(x => x.Type).ThenBy(x => x.Order);

        }
    }
}
