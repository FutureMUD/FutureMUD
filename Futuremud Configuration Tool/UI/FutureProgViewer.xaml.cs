using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Core.Objects.DataClasses;
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
    /// Interaction logic for FutureProgViewer.xaml
    /// </summary>
    public partial class FutureProgViewer : UserControl {
        BindingList<FME.FutureProg> FutureProgList;
        FMDB Database = new FMDB();
        readonly FuturemudDatabaseContext Context = FMDB.Context;
        MudSharp.Framework.Futuremud DummyFuturemud = new MudSharp.Framework.Futuremud(null);

        public FutureProgViewer() {
            InitializeComponent();
            MudSharp.FutureProg.FutureProg.Initialise();
        }

        public void Initialise() {
            FutureProgList = new BindingList<FME.FutureProg>(Context.FutureProgs.ToList());
            FutureProgListBox.ItemsSource = FutureProgList;

            FutureProgViewGrid.DataContext = FutureProgListBox.SelectedItem;
            if (FutureProgListBox.SelectedItem != null) {
                var futureprog = MudSharp.FutureProg.FutureProgFactory.CreateNew((FME.FutureProg)FutureProgListBox.SelectedItem, DummyFuturemud);
                try {
                    if (!futureprog.Compile()) {
                        CompileErrorTextBox.Text = futureprog.CompileError;
                    }
                    else {
                        CompileErrorTextBox.Text = "";
                    }
                }
                catch {
                    CompileErrorTextBox.Text = "Warning: Encountered exception when compiling";
                }

                ColourFunctionRichTextBox.Document = Converters.ANSIColourToFlowDocument.Convert(futureprog.ColourisedFunctionText);
            }
            else {
                CompileErrorTextBox.Text = "";
                ColourFunctionRichTextBox.Document = new FlowDocument();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Initialise();
        }

        private void FutureProgListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            FutureProgViewGrid.DataContext = FutureProgListBox.SelectedItem;
            if (FutureProgListBox.SelectedItem != null && ((FME.FutureProg)FutureProgListBox.SelectedItem).Id > 0) {
                var futureprog = MudSharp.FutureProg.FutureProgFactory.CreateNew((FME.FutureProg)FutureProgListBox.SelectedItem, DummyFuturemud);
                ColourFunctionRichTextBox.Document = Converters.ANSIColourToFlowDocument.Convert(futureprog.ColourisedFunctionText);
                try {
                    if (!futureprog.Compile()) {
                        CompileErrorTextBox.Text = futureprog.CompileError;
                    }
                    else {
                        CompileErrorTextBox.Text = "";
                    }
                }
                catch {
                    CompileErrorTextBox.Text = "Warning: Encountered exception when compiling";
                }
            }
            else {
                CompileErrorTextBox.Text = "";
                ColourFunctionRichTextBox.Document = new FlowDocument();
            }
        }

        private void Button_Click_SaveAll(object sender, RoutedEventArgs e) {
            Context.SaveChanges();
            Initialise();
        }

        private void Button_Click_LoadAll(object sender, RoutedEventArgs e) {
            Initialise();
        }

        private void Button_Click_Add(object sender, RoutedEventArgs e) {
            var newItem = Context.FutureProgs.Create();
            FutureProgList.Add(newItem);
            FMDB.Context.FutureProgs.Add(newItem);
            FutureProgListBox.SelectedItem = newItem;
        }

        private void Button_Click_Copy(object sender, RoutedEventArgs e) {
            var selectedItem = (FME.FutureProg)FutureProgListBox.SelectedItem;
            if (selectedItem == null) {
                MessageBox.Show("You must first select a FutureProg to copy.");
                return;
            }

            var newProg = Context.FutureProgs.Add(Context.FutureProgs.Include("FutureProgs_Parameters").AsNoTracking().FirstOrDefault(x => x.Id == selectedItem.Id));
            FMDB.Context.SaveChanges();
            FutureProgListBox.SelectedItem = newProg;
            Initialise();
        }

        private void Button_Click_Delete(object sender, RoutedEventArgs e) {
            Context.FutureProgs.Remove((FME.FutureProg)FutureProgListBox.SelectedItem);
            FutureProgList.Remove((FME.FutureProg)FutureProgListBox.SelectedItem);
        }

        private void ComboBox_Loaded(object sender, RoutedEventArgs e) {
            var combo = (ComboBox)sender;
            combo.ItemsSource = Converters.FutureProgVariableTypeToStringConverter.AllTypes.Select(x => x.Describe()).ToList();
        }

        private void Button_Click_Add_Parameter(object sender, RoutedEventArgs e) {
            var parameter = Context.FutureProgs_Parameters.Create();
            var prog = (FME.FutureProg)FutureProgViewGrid.DataContext;
            prog.FutureProgs_Parameters.Add(parameter);
            Context.FutureProgs_Parameters.Add(parameter);
        }

        private void Button_Click_Delete_Parameter(object sender, RoutedEventArgs e) {
            if (ParametersListBox.SelectedItem == null) {
                return;
            }

            Context.FutureProgs_Parameters.Remove((FutureProgs_Parameters)ParametersListBox.SelectedItem);
        }

        private void ReturnTypeComboBox_Loaded(object sender, RoutedEventArgs e) {
            var combo = (ComboBox)sender;
            combo.ItemsSource = Converters.FutureProgVariableTypeToStringConverter.AllTypes.Select(x => x.Describe()).ToList();
        }
    }
}
