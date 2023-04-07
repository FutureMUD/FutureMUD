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
using MudSharp.RPG.Checks;
using MudSharp.Framework;

namespace Futuremud_Configuration_Tool.UI {
    /// <summary>
    /// Interaction logic for AccentViewer.xaml
    /// </summary>
    public partial class AccentViewer : UserControl {
        BindingList<Accent> AccentList;
        FMDB Database = new FMDB();
        readonly FuturemudDatabaseContext Context = FMDB.Context;

        public AccentViewer() {
            InitializeComponent();
        }

        public BindingList<Accent> GetAccents() {
            if (LanguageFilterComboBox.SelectedItem == null || LanguageFilterComboBox.SelectedItem.ToString() == "All") {
                return new BindingList<Accent>(Context.Accents.ToList());
            }

            return new BindingList<Accent>(Context.Accents.Where(x => x.Language.Name == (string)LanguageFilterComboBox.SelectedItem).ToList());
        }

        public void Initialise() {
            LanguageFilterComboBox.ItemsSource = (new string[] { "All" }).Concat(Context.Languages.Select(x => x.Name)).ToList();
            LanguageFilterComboBox.SelectedItem = LanguageFilterComboBox.Items[0];

            AccentList = GetAccents();
            AccentListBox.ItemsSource = AccentList;

            var oldSelectedItem = LinkedLanguageComboBox.SelectedItem;
            LinkedLanguageComboBox.ItemsSource = Context.Languages.ToList();
            LinkedLanguageComboBox.SelectedItem = oldSelectedItem;

            oldSelectedItem = DifficultyComboBox.SelectedItem;
            DifficultyComboBox.ItemsSource = ((IEnumerable<Difficulty>)Enum.GetValues(typeof(Difficulty))).Select(x => x.Describe()).ToList();
            DifficultyComboBox.SelectedItem = oldSelectedItem;

            oldSelectedItem = GroupComboBox.SelectedItem;
            GroupComboBox.ItemsSource = Context.Accents.Select(x => x.Group).Distinct().ToList();
            GroupComboBox.SelectedItem = oldSelectedItem;

            AccentViewGrid.DataContext = AccentListBox.SelectedItem;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Initialise();
        }

        private void LanguageListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            AccentViewGrid.DataContext = AccentListBox.SelectedItem;
        }

        private void Button_Click_SaveAll(object sender, RoutedEventArgs e) {
            Context.SaveChanges();
            Initialise();
        }

        private void Button_Click_LoadAll(object sender, RoutedEventArgs e) {
            Initialise();
        }

        private void Button_Click_Add(object sender, RoutedEventArgs e) {
            var newItem = Context.Accents.Create();
            AccentList.Add(newItem);
            FMDB.Context.Accents.Add(newItem);
            AccentListBox.SelectedItem = newItem;
        }

        private void Button_Click_Delete(object sender, RoutedEventArgs e) {
            Context.Accents.Remove((FME.Accent)AccentListBox.SelectedItem);
            AccentList.Remove((FME.Accent)AccentListBox.SelectedItem);
        }

        private void LanguageFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            AccentList = GetAccents();
            AccentListBox.ItemsSource = AccentList;
        }
    }
}
