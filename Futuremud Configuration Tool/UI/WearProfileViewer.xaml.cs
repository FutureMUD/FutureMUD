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
    /// Interaction logic for WearProfileViewer.xaml
    /// </summary>
    public partial class WearProfileViewer : UserControl {
        BindingList<FME.WearProfile> SelectionList;
        FMDB Database = new FMDB();
        readonly FuturemudDatabaseContext Context = FMDB.Context;

        public WearProfileViewer() {
            InitializeComponent();
        }

        public BindingList<FME.WearProfile> GetValues() {
            if (FilterComboBox.SelectedItem == null || FilterComboBox.SelectedItem.ToString() == "All") {
                return new BindingList<FME.WearProfile>(Context.WearProfiles.ToList());
            }

            return new BindingList<FME.WearProfile>(Context.WearProfiles.Where(x => x.Type == (string)FilterComboBox.SelectedItem).ToList());
        }

        public void Initialise() {
            FilterComboBox.ItemsSource = (new string[] { "All" }).Concat(Context.WearProfiles.Select(x => x.Name).Distinct()).ToList();
            FilterComboBox.SelectedItem = FilterComboBox.Items[0];

            SelectionList = GetValues();
            SelectionListBox.ItemsSource = SelectionList;

            var oldSelectedItem = DefinitionComboBox.SelectedItem;
            DefinitionComboBox.ItemsSource = new[] { "Direct", "Shape" };
            DefinitionComboBox.SelectedItem = oldSelectedItem;

            if (SelectionListBox.SelectedItem != null)
            {
                ItemViewGrid.DataContext = (FME.WearProfile)SelectionListBox.SelectedItem;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Initialise();
        }

        private void SelectionListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (SelectionListBox.SelectedItem != null) {
                ItemViewGrid.DataContext = ItemViewGrid.DataContext = (FME.WearProfile)SelectionListBox.SelectedItem;
            }
        }

        private void Button_Click_SaveAll(object sender, RoutedEventArgs e)
        {
            FMDB.Context.SaveChanges();
            Initialise();
        }

        private void Button_Click_LoadAll(object sender, RoutedEventArgs e) {
            Initialise();
        }

        private void Button_Click_Add(object sender, RoutedEventArgs e) {
            var newItem = Context.WearProfiles.Create();
            SelectionList.Add(newItem);
            FMDB.Context.WearProfiles.Add(newItem);
            newItem.Type = "Direct";
            SelectionListBox.SelectedItem = newItem;
        }

        private void Button_Click_Delete(object sender, RoutedEventArgs e) {
            Context.WearProfiles.Remove((FME.WearProfile)SelectionListBox.SelectedItem);
            SelectionList.Remove((FME.WearProfile)SelectionListBox.SelectedItem);
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            SelectionList = GetValues();
            SelectionListBox.ItemsSource = SelectionList;
        }

        private void DefinitionComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (ValuesListBox.ItemsSource == null) {
            //    ValuesListBox.Items.Clear();
            //}
            //ValuesListBox.ItemsSource = ((CharacteristicProfileStandard)ItemViewGrid.DataContext).Values;
        }
    }
}
