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
    /// Interaction logic for LanguageViewer.xaml
    /// </summary>
    public partial class LanguageViewer : UserControl {
        BindingList<Language> LanguageList;
        FMDB Database = new FMDB();
        readonly FuturemudDatabaseContext Context = FMDB.Context;

        public LanguageViewer() {
            InitializeComponent();
        }

        public void Initialise() {
            LanguageList = new BindingList<Language>(Context.Languages.ToList());
            LanguageListBox.ItemsSource = LanguageList;

            var oldSelectedItem = LinkedTraitComboBox.SelectedItem;
            LinkedTraitComboBox.ItemsSource = Context.TraitDefinitions.Where(x => x.Type != 1).ToList();
            LinkedTraitComboBox.SelectedItem = oldSelectedItem;

            oldSelectedItem = DifficultyModelComboBox.SelectedItem;
            DifficultyModelComboBox.ItemsSource = Context.LanguageDifficultyModels.ToList();
            DifficultyModelComboBox.SelectedItem = oldSelectedItem;
            LanguageViewGrid.DataContext = LanguageListBox.SelectedItem;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Initialise();
        }

        private void LanguageListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            LanguageViewGrid.DataContext = LanguageListBox.SelectedItem;
        }

        private void Button_Click_SaveAll(object sender, RoutedEventArgs e) {
            Context.SaveChanges();
            Initialise();
        }

        private void Button_Click_LoadAll(object sender, RoutedEventArgs e) {
            Initialise();
        }

        private void Button_Click_Add(object sender, RoutedEventArgs e) {
            var newItem = Context.Languages.Create();
            LanguageList.Add(newItem);
            FMDB.Context.Languages.Add(newItem);
            LanguageListBox.SelectedItem = newItem;
        }

        private void Button_Click_Delete(object sender, RoutedEventArgs e) {
            Context.Languages.Remove((FME.Language)LanguageListBox.SelectedItem);
            LanguageList.Remove((FME.Language)LanguageListBox.SelectedItem);
        }
    }
}
