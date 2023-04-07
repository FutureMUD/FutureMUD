using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
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
    /// Interaction logic for SkillViewer.xaml
    /// </summary>
    public partial class SkillViewer : UserControl {
        BindingList<FME.TraitDefinition> SkillList;
        FMDB Database = new FMDB();
        readonly FuturemudDatabaseContext Context = FMDB.Context;

        public SkillViewer() {
            InitializeComponent();
        }

        public void Initialise() {
            SkillList = new BindingList<TraitDefinition>(Context.TraitDefinitions.Where(x => x.Type != 1).ToList());
            SkillListBox.ItemsSource = SkillList;

            var oldSelectedItem = SkillViewerDecoratorComboBox.SelectedItem;
            SkillViewerDecoratorComboBox.ItemsSource = Context.TraitDecorators.ToList();
            SkillViewerDecoratorComboBox.SelectedItem = oldSelectedItem;

            oldSelectedItem = SkillViewerImproverComboBox.SelectedItem;
            SkillViewerImproverComboBox.ItemsSource = Context.Improvers.ToList();
            SkillViewerImproverComboBox.SelectedItem = oldSelectedItem;

            TraitTypeComboBox.Items.Add(0);
            TraitTypeComboBox.Items.Add(2);

            TraitDerivedTypeComboBox.Items.Add(0);
            TraitDerivedTypeComboBox.Items.Add(1);
            TraitDerivedTypeComboBox.Items.Add(2);

            SkillViewGrid.DataContext = SkillListBox.SelectedItem;
        }

        private void Button_Click_LoadAll(object sender, RoutedEventArgs e) {
            Initialise();
        }

        private void SkillListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            SkillViewGrid.DataContext = SkillListBox.SelectedItem;
        }

        private void Button_Click_SaveAll(object sender, RoutedEventArgs e) {
            try {
                Context.SaveChanges();
                SkillList = new BindingList<TraitDefinition>(Context.TraitDefinitions.Where(x => x.Type != 1).ToList());
                SkillListBox.ItemsSource = SkillList;
            }
            catch (DbUpdateException ex) {
                System.Windows.MessageBox.Show(ex.Message, "Database Exception");
            }
            catch (DbEntityValidationException ex) {
                var sb = new StringBuilder(ex.Message);
                sb.AppendLine();
                foreach (var error in ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors)) {
                    sb.AppendLine(String.Format("{0} - {1}", error.PropertyName, error.ErrorMessage));
                }

                System.Windows.MessageBox.Show(sb.ToString(), "Database Exception");
            }
        }

        private void Button_Click_CancelAll(object sender, RoutedEventArgs e) {
            Initialise();
        }

        private void Button_Click_Add(object sender, RoutedEventArgs e) {
            var newItem = Context.TraitDefinitions.Create();
            newItem.Type = 0;
            SkillList.Add(newItem);
            Context.TraitDefinitions.Add(newItem);
            var newExpression = Context.TraitExpressions.Create();
            newItem.TraitExpression = newExpression;
            Context.TraitExpressions.Add(newExpression);
            SkillListBox.SelectedItem = newItem;
        }

        private void Button_Click_Delete(object sender, RoutedEventArgs e) {
            Context.TraitDefinitions.Remove((FME.TraitDefinition)SkillListBox.SelectedItem);
            SkillList.Remove((FME.TraitDefinition)SkillListBox.SelectedItem);
        }
    }
}
