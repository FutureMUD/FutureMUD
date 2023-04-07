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
using MudSharp.FutureProg;

namespace Futuremud_Configuration_Tool.UI {
    /// <summary>
    /// Interaction logic for CharacteristicValueViewer.xaml
    /// </summary>
    public partial class CharacteristicValueViewer : UserControl {
        BindingList<FME.CharacteristicValue> SelectionList;
        FMDB Database = new FMDB();
        readonly FuturemudDatabaseContext Context = FMDB.Context;

        public CharacteristicValueViewer() {
            InitializeComponent();
        }

        public BindingList<CharacteristicValue> GetValues() {
            if (FilterComboBox.SelectedItem == null || FilterComboBox.SelectedItem.ToString() == "All") {
                return new BindingList<CharacteristicValue>(Context.CharacteristicValues.Where(x => x.CharacteristicDefinition.Type == (int)MudSharp.Form.Characteristics.CharacteristicType.Multiform || x.CharacteristicDefinition.Type == (int)MudSharp.Form.Characteristics.CharacteristicType.Standard).ToList());
            }

            return new BindingList<CharacteristicValue>(Context.CharacteristicValues.Where(x => x.CharacteristicDefinition.Type == (int)MudSharp.Form.Characteristics.CharacteristicType.Multiform || x.CharacteristicDefinition.Type == (int)MudSharp.Form.Characteristics.CharacteristicType.Standard).Where(x => x.CharacteristicDefinition.Name == (string)FilterComboBox.SelectedItem).ToList());
        }

        public void Initialise() {
            FilterComboBox.ItemsSource = (new string[] { "All" }).Concat(Context.CharacteristicDefinitions.Where(x => x.Type == (int)MudSharp.Form.Characteristics.CharacteristicType.Multiform || x.Type == (int)MudSharp.Form.Characteristics.CharacteristicType.Standard).Select(x => x.Name)).ToList();
            FilterComboBox.SelectedItem = FilterComboBox.Items[0];

            SelectionList = GetValues();
            SelectionListBox.ItemsSource = SelectionList;

            var oldSelectedItem = DefinitionComboBox.SelectedItem;
            DefinitionComboBox.ItemsSource = Context.CharacteristicDefinitions.Where(x => x.Type == (int)MudSharp.Form.Characteristics.CharacteristicType.Multiform || x.Type == (int)MudSharp.Form.Characteristics.CharacteristicType.Standard).ToList();
            DefinitionComboBox.SelectedItem = oldSelectedItem;

            oldSelectedItem = FutureProgComboBox.SelectedItem;
            FutureProgComboBox.ItemsSource =
                new BindingList<object>(new object[] { Converters.FutureProgNullableConverter.DummyProg }.Concat(Context.FutureProgs.ToList().Where(
                    x =>
                        x.ReturnType == (long) FutureProgVariableTypes.Boolean &&
                        x.FutureProgs_Parameters.Select(y => y.ParameterType)
                            .SequenceEqual(new[] {(long) FutureProgVariableTypes.Chargen}))).ToList());
            FutureProgComboBox.SelectedItem = oldSelectedItem;

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
            var newItem = Context.CharacteristicValues.Create();
            SelectionList.Add(newItem);
            FMDB.Context.CharacteristicValues.Add(newItem);
            SelectionListBox.SelectedItem = newItem;
        }

        private void Button_Click_Delete(object sender, RoutedEventArgs e) {
            Context.CharacteristicValues.Remove((FME.CharacteristicValue)SelectionListBox.SelectedItem);
            SelectionList.Remove((FME.CharacteristicValue)SelectionListBox.SelectedItem);
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            SelectionList = GetValues();
            SelectionListBox.ItemsSource = SelectionList;
        }
    }
}
