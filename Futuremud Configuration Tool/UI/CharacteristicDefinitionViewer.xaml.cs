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
using MudSharp.Form.Characteristics;
using MudSharp.Framework;

namespace Futuremud_Configuration_Tool.UI {
    /// <summary>
    /// Interaction logic for CharacteristicDefinitionViewer.xaml
    /// </summary>
    public partial class CharacteristicDefinitionViewer : UserControl {
        BindingList<FME.CharacteristicDefinition> CharacteristicDefinitionList;
        FMDB Database = new FMDB();
        readonly FuturemudDatabaseContext Context = FMDB.Context;

        public CharacteristicDefinitionViewer() {
            InitializeComponent();
        }

        public void Initialise() {
            CharacteristicDefinitionList = new BindingList<FME.CharacteristicDefinition>(Context.CharacteristicDefinitions.ToList());
            CharacteristicDefinitionListBox.ItemsSource = CharacteristicDefinitionList;

            var oldSelectedItem = TypeComboBox.SelectedItem;
            TypeComboBox.ItemsSource = ((IEnumerable<CharacteristicType>)Enum.GetValues(typeof(CharacteristicType))).Select(x => x.Describe()).ToList();
            TypeComboBox.SelectedItem = oldSelectedItem;

            oldSelectedItem = ChargenDisplayComboBox.SelectedItem;
            ChargenDisplayComboBox.ItemsSource = ((IEnumerable<CharacterGenerationDisplayType>)Enum.GetValues(typeof(CharacterGenerationDisplayType))).Select(x => x.Describe()).ToList();
            ChargenDisplayComboBox.SelectedItem = oldSelectedItem;

            var converter = new Converters.NullableCharacteristicDefinitionConverter();
            oldSelectedItem = ParentDefinitionComboBox.SelectedItem;
            ParentDefinitionComboBox.ItemsSource = (new string[] { "None" }).Concat(Context.CharacteristicDefinitions.Except((FME.CharacteristicDefinition)CharacteristicDefinitionListBox.SelectedItem).Select(x => converter.Convert(x, typeof(string), null, null).ToString())).ToList();
            ParentDefinitionComboBox.SelectedItem = oldSelectedItem;

            CharacteristicDefinitionViewGrid.DataContext = CharacteristicDefinitionListBox.SelectedItem;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Initialise();
        }

        private void CharacteristicDefinitionListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            CharacteristicDefinitionViewGrid.DataContext = CharacteristicDefinitionListBox.SelectedItem;
        }

        private void Button_Click_SaveAll(object sender, RoutedEventArgs e) {
            Context.SaveChanges();
            Initialise();
        }

        private void Button_Click_LoadAll(object sender, RoutedEventArgs e) {
            Initialise();
        }

        private void Button_Click_Add(object sender, RoutedEventArgs e) {
            var newItem = Context.CharacteristicDefinitions.Create();
            CharacteristicDefinitionList.Add(newItem);
            FMDB.Context.CharacteristicDefinitions.Add(newItem);
            CharacteristicDefinitionListBox.SelectedItem = newItem;
        }

        private void Button_Click_Delete(object sender, RoutedEventArgs e) {
            Context.CharacteristicDefinitions.Remove((FME.CharacteristicDefinition)CharacteristicDefinitionListBox.SelectedItem);
            CharacteristicDefinitionList.Remove((FME.CharacteristicDefinition)CharacteristicDefinitionListBox.SelectedItem);
        }
    }
}
