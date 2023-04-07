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

namespace Futuremud_Configuration_Tool.Subcontrols {
    /// <summary>
    /// Interaction logic for StaticStringViewer.xaml
    /// </summary>
    public partial class StaticStringViewer : Model.FuturemudBaseControl {
        public StaticStringViewer() {
            InitializeComponent();
        }

        public override string FilterLabelText {
            get { return "Filter"; }
        }

        public override ICollection<object> Dataset {
            get { return FMDB.Context.StaticStrings.AsEnumerable<object>().ToList(); }
        }

        public override ICollection<object> FilteredDataset(IEnumerable<object> set, object filter) {
            return set.AsEnumerable<object>().ToList();
        }

        public override IEnumerable<object> FilterOptions {
            get { return new object[] { UnfilteredOptionObject }; }
        }

        public override object AddNew() {
            var newItem = FMDB.Context.StaticStrings.Create();
            FMDB.Context.StaticStrings.Add(newItem);
            return newItem;
        }

        public override void DeleteObject(object target) {
            if (target != null && target is FME.StaticString) {
                FMDB.Context.StaticStrings.Remove((FME.StaticString)target);
            }
        }

        public override object CopyObject(object target) {
            var copyItem = (FME.StaticString)target;
            var item = FMDB.Context.StaticStrings.Create();
            FMDB.Context.StaticStrings.Add(item);
            item.Id = copyItem.Id;
            item.Text = copyItem.Text;
            return item;
        }

        public override void SelectionChanged(SelectionChangedEventArgs args) {
            // Do nothing
        }

        public override DataTemplate SelectionListboxTemplate {
            get {
                Style style = new Style(typeof(ListBox));
                var dt = new DataTemplate();
                var mb = new MultiBinding();
                mb.StringFormat = "{0}";
                mb.Bindings.Add(new Binding("Id"));
                FrameworkElementFactory textElement = new FrameworkElementFactory(typeof(TextBlock));
                textElement.SetBinding(TextBlock.TextProperty, mb);
                dt.VisualTree = textElement;
                return dt;
            }
        }

        public override DataTemplate ComboboxTemplate {
            get {
                Style style = new Style(typeof(ComboBox));
                var dt = new DataTemplate();
                FrameworkElementFactory textElement = new FrameworkElementFactory(typeof(TextBlock));
                textElement.SetBinding(TextBlock.TextProperty, new Binding());
                dt.VisualTree = textElement;
                return dt;
            }
        }
    }
}
