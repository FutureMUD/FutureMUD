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
using MudSharp.Form.Colour;

namespace Futuremud_Configuration_Tool.UI {
    /// <summary>
    /// Interaction logic for ColourViewer.xaml
    /// </summary>
    public partial class ColourViewer : UserControl {
        BindingList<FME.Colour> ColourList;
        FMDB Database = new FMDB();
        readonly FuturemudDatabaseContext Context = FMDB.Context;

        public ColourViewer() {
            InitializeComponent();
        }

        public void Initialise() {
            ColourList = new BindingList<FME.Colour>(Context.Colours.ToList());
            ColourListBox.ItemsSource = ColourList;

            var oldSelectedItem = TypeComboBox.SelectedItem;
            TypeComboBox.ItemsSource = ((IEnumerable<BasicColour>)Enum.GetValues(typeof(BasicColour))).Select(x => x.Describe()).ToList();
            TypeComboBox.SelectedItem = oldSelectedItem;

            ColourViewGrid.DataContext = ColourListBox.SelectedItem;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Initialise();
        }

        private void ColourListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            ColourViewGrid.DataContext = ColourListBox.SelectedItem;
        }

        private void Button_Click_SaveAll(object sender, RoutedEventArgs e) {
            Context.SaveChanges();
            Initialise();
        }

        private void Button_Click_LoadAll(object sender, RoutedEventArgs e) {
            Initialise();
        }

        private void Button_Click_Add(object sender, RoutedEventArgs e) {
            var newItem = Context.Colours.Create();
            ColourList.Add(newItem);
            FMDB.Context.Colours.Add(newItem);
            ColourListBox.SelectedItem = newItem;
        }

        private void Button_Click_Delete(object sender, RoutedEventArgs e) {
            Context.Colours.Remove((FME.Colour)ColourListBox.SelectedItem);
            ColourList.Remove((FME.Colour)ColourListBox.SelectedItem);
        }
    }
}
