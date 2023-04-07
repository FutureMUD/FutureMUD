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
using MudSharp.Database;

namespace Futuremud_Configuration_Tool.UI {
    /// <summary>
    /// Interaction logic for TraitExpressionViewer.xaml
    /// </summary>
    public partial class TraitExpressionViewer : UserControl {
        FMDB Database = new FMDB();
        readonly FME.FuturemudDatabaseContext Context = FMDB.Context;
        readonly BindingList<FME.TraitDefinition> Traits;

        public TraitExpressionViewer() {
            InitializeComponent();
            Traits = new BindingList<FME.TraitDefinition>(Context.TraitDefinitions.ToList());
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            var parameter = Context.TraitExpressionParameters.Create();
            var expression = (FME.TraitExpression)this.DataContext;
            expression.TraitExpressionParameters.Add(parameter);
            Context.TraitExpressionParameters.Add(parameter);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e) {
            //Traits = new BindingList<FME.TraitDefinition>(Context.TraitDefinitions.ToList());
        }

        private void ComboBox_Loaded(object sender, RoutedEventArgs e) {
            var combo = (ComboBox)sender;
            combo.ItemsSource = Traits;
        }
    }
}
