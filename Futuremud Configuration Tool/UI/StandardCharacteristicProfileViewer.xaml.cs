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
    /// Interaction logic for StandardCharacteristicProfileViewer.xaml
    /// </summary>
    public partial class StandardCharacteristicProfileViewer : UserControl {
        BindingList<FME.CharacteristicProfile> SelectionList;
        FMDB Database = new FMDB();
        readonly FuturemudDatabaseContext Context = FMDB.Context;

        public StandardCharacteristicProfileViewer() {
            InitializeComponent();
        }

        public BindingList<CharacteristicProfile> GetValues() {
            if (FilterComboBox.SelectedItem == null || FilterComboBox.SelectedItem.ToString() == "All") {
                return new BindingList<CharacteristicProfile>(Context.CharacteristicProfiles.Where(x => x.Type == "Standard").ToList());
            }

            return new BindingList<CharacteristicProfile>(Context.CharacteristicProfiles.Where(x => x.Type == "Standard").Where(x => x.CharacteristicDefinition.Name == (string)FilterComboBox.SelectedItem).ToList());
        }

        public void Initialise() {
            FilterComboBox.ItemsSource = (new string[] { "All" }).Concat(Context.CharacteristicDefinitions.Select(x => x.Name)).ToList();
            FilterComboBox.SelectedItem = FilterComboBox.Items[0];

            SelectionList = GetValues();
            SelectionListBox.ItemsSource = SelectionList;

            var oldSelectedItem = DefinitionComboBox.SelectedItem;
            DefinitionComboBox.ItemsSource = Context.CharacteristicDefinitions.ToList();
            DefinitionComboBox.SelectedItem = oldSelectedItem;

            if (SelectionListBox.SelectedItem != null)
            {
                ItemViewGrid.DataContext = new CharacteristicProfileStandard((FME.CharacteristicProfile)SelectionListBox.SelectedItem);
            //    if (ValuesListBox.ItemsSource == null)
            //    {
            //        ValuesListBox.Items.Clear();
            //    }
            //    ValuesListBox.ItemsSource = ((CharacteristicProfileStandard) ItemViewGrid.DataContext).Values;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Initialise();
        }

        private void SelectionListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (SelectionListBox.SelectedItem != null) {
                ItemViewGrid.DataContext = new CharacteristicProfileStandard((FME.CharacteristicProfile)SelectionListBox.SelectedItem);
                //    if (ValuesListBox.ItemsSource == null) {
                //        ValuesListBox.Items.Clear();
                //    }
                //    ValuesListBox.ItemsSource = ((CharacteristicProfileStandard)ItemViewGrid.DataContext).Values;
            }
        }

        private void Button_Click_SaveAll(object sender, RoutedEventArgs e)
        {
            ((CharacteristicProfileStandard)ItemViewGrid.DataContext).Save((FME.CharacteristicProfile)SelectionListBox.SelectedItem);
            FMDB.Context.SaveChanges();
            ((CharacteristicProfileStandard) ItemViewGrid.DataContext).ID =
                ((FME.CharacteristicProfile) SelectionListBox.SelectedItem).Id;
            Initialise();
        }

        private void Button_Click_LoadAll(object sender, RoutedEventArgs e) {
            Initialise();
        }

        private void Button_Click_Add(object sender, RoutedEventArgs e) {
            var newItem = Context.CharacteristicProfiles.Create();
            SelectionList.Add(newItem);
            FMDB.Context.CharacteristicProfiles.Add(newItem);
            newItem.CharacteristicDefinition = FMDB.Context.CharacteristicDefinitions.First();
            newItem.Definition = "<Definition/>";
            SelectionListBox.SelectedItem = newItem;
        }

        private void Button_Click_Delete(object sender, RoutedEventArgs e) {
            Context.CharacteristicProfiles.Remove((FME.CharacteristicProfile)SelectionListBox.SelectedItem);
            SelectionList.Remove((FME.CharacteristicProfile)SelectionListBox.SelectedItem);
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
