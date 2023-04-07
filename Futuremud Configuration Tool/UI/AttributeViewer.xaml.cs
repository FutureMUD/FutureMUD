using System;
using System.Collections.Generic;
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
using FME;
using System.ComponentModel;

namespace Futuremud_Configuration_Tool.UI {
    /// <summary>
    /// Interaction logic for TraitViewer.xaml
    /// </summary>
    public partial class AttributeViewer : UserControl {

        BindingList<TraitDefinition> TraitList;
        FMDB Database = new FMDB();
        readonly FuturemudDatabaseContext Context = FMDB.Context;

        public AttributeViewer() {
            InitializeComponent();
        }

        public void Initialise() {
            TraitList = new BindingList<TraitDefinition>(Context.TraitDefinitions.Where(x => x.Type == 1).ToList());
            AttributeListBox.ItemsSource = TraitList;

            var oldSelectedItem = AttributeViewerDecoratorComboBox.SelectedItem;
            AttributeViewerDecoratorComboBox.ItemsSource = Context.TraitDecorators.ToList();
            AttributeViewerDecoratorComboBox.SelectedItem = oldSelectedItem;

            oldSelectedItem = AttributeViewerImproverComboBox.SelectedItem;
            AttributeViewerImproverComboBox.ItemsSource = Context.Improvers.ToList();
            AttributeViewerImproverComboBox.SelectedItem = oldSelectedItem;
            AttributeViewGrid.DataContext = AttributeListBox.SelectedItem;
        }

        private void Button_Click_LoadAll(object sender, RoutedEventArgs e) {
            Initialise();
        }

        private void AttributeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            AttributeViewGrid.DataContext = AttributeListBox.SelectedItem;
        }

        private void Button_Click_SaveAll(object sender, RoutedEventArgs e) {
            Context.SaveChanges();
            TraitList = new BindingList<TraitDefinition>(Context.TraitDefinitions.Where(x => x.Type == 1).ToList());
            AttributeListBox.ItemsSource = TraitList;
        }

        private void Button_Click_CancelAll(object sender, RoutedEventArgs e) {
            Initialise();
        }

        private void Button_Click_Add(object sender, RoutedEventArgs e) {
            var newItem = Context.TraitDefinitions.Create();
            newItem.Type = 1;
            TraitList.Add(newItem);
            FMDB.Context.TraitDefinitions.Add(newItem);
            AttributeListBox.SelectedItem = newItem;
        }

        private void Button_Click_Delete(object sender, RoutedEventArgs e) {
            Context.TraitDefinitions.Remove((FME.TraitDefinition)AttributeListBox.SelectedItem);
            TraitList.Remove((FME.TraitDefinition)AttributeListBox.SelectedItem);
        }
    }
}
