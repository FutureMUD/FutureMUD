using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
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
using Futuremud_Configuration_Tool.Initialisation;

namespace Futuremud_Configuration_Tool.Model {
    /// <summary>
    /// Interaction logic for FuturemudBase.xaml
    /// </summary>
    public partial class DisplayControl : UserControl {
        private FuturemudBaseControl _subcontrol = null;
        private InitialisationContext _context;

        private ObservableCollection<object> _dataset;

        public DisplayControl(InitialisationContext context) {
            InitializeComponent();
            _context = context;
        }

        public void SetSubcontrol(FuturemudBaseControl control) {
            _subcontrol = control;
            ItemGrid.Children.Clear();
            ItemGrid.Children.Add(control);
            _dataset = new ObservableCollection<object>(control.Dataset);
            SelectionListBox.ItemsSource = _dataset;
            FilterLabel.Content = control.FilterLabelText;
            FilterComboBox.ItemsSource = control.FilterOptions;
            FilterComboBox.SelectedItem = control.UnfilteredOptionObject;
            SelectionListBox.SelectedItem = null;
            SelectionListBox.ItemTemplate = control.SelectionListboxTemplate;
            FilterComboBox.ItemTemplate = control.ComboboxTemplate;
        }

        public void Button_Click_Add(object sender, RoutedEventArgs e) {
            var newItem = _subcontrol.AddNew();
            _dataset.Add(newItem);
            SelectionListBox.SelectedItem = newItem;
        }

        public void Button_Click_Delete(object sender, RoutedEventArgs e) {
            var deletedItem = SelectionListBox.SelectedItem;
            _subcontrol.DeleteObject(deletedItem);
            _dataset.Remove(deletedItem);
        }

        public void Button_Click_Copy(object sender, RoutedEventArgs e) {
            var copiedItem = SelectionListBox.SelectedItem;
            var newItem = _subcontrol.CopyObject(copiedItem);
            _dataset.Add(newItem);
            SelectionListBox.SelectedItem = newItem;
        }

        public async void Button_Click_Save(object sender, RoutedEventArgs e) {
            await _context.Context.SaveChangesAsync();
        }

        public void Button_Click_Reload(object sender, RoutedEventArgs e) {
            foreach (var entry in _context.Context.ChangeTracker.Entries()) {
                switch (entry.State) {
                    case EntityState.Modified: {
                            entry.CurrentValues.SetValues(entry.OriginalValues);
                            entry.State = EntityState.Unchanged;
                            break;
                        }
                    case EntityState.Deleted: {
                            entry.State = EntityState.Unchanged;
                            break;
                        }
                    case EntityState.Added: {
                            entry.State = EntityState.Detached;
                            break;
                        }
                }
            }

            SetSubcontrol(_subcontrol);
        }

        public void Button_Click_Cancel(object sender, RoutedEventArgs e) {
            foreach (var entry in _context.Context.ChangeTracker.Entries()) {
                switch (entry.State) {
                    case EntityState.Modified: {
                            entry.CurrentValues.SetValues(entry.OriginalValues);
                            entry.State = EntityState.Unchanged;
                            break;
                        }
                    case EntityState.Deleted: {
                            entry.State = EntityState.Unchanged;
                            break;
                        }
                    case EntityState.Added: {
                            entry.State = EntityState.Detached;
                            break;
                        }
                }
            }

            SetSubcontrol(_subcontrol);
        }

        private void SelectionListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            ItemGrid.DataContext = SelectionListBox.SelectedItem;
            _subcontrol.SelectionChanged(e);
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (FilterComboBox.SelectedItem == null) {
                _dataset = new ObservableCollection<object>(_subcontrol.Dataset);
            }
            else {
                _dataset = new ObservableCollection<object>(_subcontrol.FilteredDataset(_subcontrol.Dataset, FilterComboBox.SelectedItem));
            }

            var selected = SelectionListBox.SelectedItem;
            SelectionListBox.ItemsSource = _dataset;
            if (selected != null && _dataset.Contains(selected)) {
                SelectionListBox.SelectedItem = selected;
            }
        }
    }
}
