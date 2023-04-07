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
using MudSharp.Database;

namespace Futuremud_Configuration_Tool.UI {
    /// <summary>
    /// Interaction logic for StaticStringViewer.xaml
    /// </summary>
    public partial class StaticStringViewer : UserControl {
        BindingList<FME.StaticString> ColourList;
        FMDB Database = new FMDB();
        readonly FME.FuturemudDatabaseContext Context = FMDB.Context;

        public StaticStringViewer() {
            InitializeComponent();
        }

        public void Initialise() {
            ColourList = new BindingList<FME.StaticString>(Context.StaticStrings.ToList());
            SelectionListBox.ItemsSource = ColourList;

            ItemViewGrid.DataContext = SelectionListBox.SelectedItem;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Initialise();
        }

        private void SelectionListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            ItemViewGrid.DataContext = SelectionListBox.SelectedItem;
        }

        private void Button_Click_SaveAll(object sender, RoutedEventArgs e) {
            Context.SaveChanges();
            Initialise();
        }

        private void Button_Click_LoadAll(object sender, RoutedEventArgs e) {
            Initialise();
        }

        private void Button_Click_Add(object sender, RoutedEventArgs e) {
            var newItem = Context.StaticStrings.Create();
            ColourList.Add(newItem);
            FMDB.Context.StaticStrings.Add(newItem);
            SelectionListBox.SelectedItem = newItem;
        }

        private void Button_Click_Delete(object sender, RoutedEventArgs e) {
            Context.StaticStrings.Remove((FME.StaticString)SelectionListBox.SelectedItem);
            ColourList.Remove((FME.StaticString)SelectionListBox.SelectedItem);
        }
    }
}
