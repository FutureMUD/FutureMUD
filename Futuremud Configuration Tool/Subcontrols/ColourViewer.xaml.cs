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
using MudSharp.Framework;
using MudSharp.Form.Shape;
using MudSharp.Form.Colour;

namespace Futuremud_Configuration_Tool.Subcontrols {
    /// <summary>
    /// Interaction logic for TestControl.xaml
    /// </summary>
    public partial class ColourViewer {
        public ColourViewer() {
            InitializeComponent();
            TypeComboBox.ItemsSource = ((IEnumerable<BasicColour>)Enum.GetValues(typeof(BasicColour))).Select(x => x.Describe()).ToList();
        }

        public override string FilterLabelText {
            get { return "Test"; }
        }

        public override ICollection<object> Dataset {           
            get { 
                return FMDB.Context.Colours.AsEnumerable<object>().ToList(); 
            }
        }

        public override ICollection<object> FilteredDataset(IEnumerable<object> set, object filter) {
            return 
                (filter == null || filter == UnfilteredOptionObject) ?
                set.ToList() :
                set.OfType<FME.Colour>().Where(x => ((BasicColour)x.Basic).Describe().StartsWith(filter.ToString())).ToList<object>();
        }

        public override IEnumerable<object> FilterOptions {
            get {
                var list = new List<object>();
                list.Add(UnfilteredOptionObject);
                list.AddRange(FMDB.Context.Colours.Select(x => (BasicColour)x.Basic).ToList().Select(x => x.Describe()).Distinct());
                return list;
            }
        }

        public override object AddNew() {
            var item = FMDB.Context.Colours.Create();
            FMDB.Context.Colours.Add(item);
            return item;
        }

        public override void DeleteObject(object target) {
            if (target != null && target is FME.Colour) {
                FMDB.Context.Colours.Remove((FME.Colour)target);
            }
        }

        public override object CopyObject(object target) {
            var copyColour = (FME.Colour)target;
            var item = FMDB.Context.Colours.Create();
            FMDB.Context.Colours.Add(item);
            item.Name = copyColour.Name;
            item.Fancy = copyColour.Fancy;
            item.Basic = copyColour.Basic;
            item.Blue = copyColour.Blue;
            item.Red = copyColour.Red;
            item.Green = copyColour.Green;
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
                mb.StringFormat = "{0} - {1}";
                mb.Bindings.Add(new Binding("Id"));
                mb.Bindings.Add(new Binding("Name"));
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
