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
using Futuremud_Configuration_Tool.Model;

namespace Futuremud_Configuration_Tool.Subcontrols {
    /// <summary>
    /// Interaction logic for WearProfileViewer.xaml
    /// </summary>
    public partial class WearProfileViewer : FuturemudBaseControl {
        public WearProfileViewer() {
            InitializeComponent();
        }

        public BindingList<FME.BodypartProto> Bodyparts { get; set; }

        public override string FilterLabelText {
            get { throw new NotImplementedException(); }
        }

        public override ICollection<object> Dataset {
            get { throw new NotImplementedException(); }
        }

        public override ICollection<object> FilteredDataset(IEnumerable<object> set, object filter) {
            throw new NotImplementedException();
        }

        public override IEnumerable<object> FilterOptions {
            get { throw new NotImplementedException(); }
        }

        public override object AddNew() {
            throw new NotImplementedException();
        }

        public override void DeleteObject(object target) {
            throw new NotImplementedException();
        }

        public override object CopyObject(object target) {
            throw new NotImplementedException();
        }

        public override void SelectionChanged(SelectionChangedEventArgs args) {
            throw new NotImplementedException();
        }

        public override DataTemplate SelectionListboxTemplate {
            get { throw new NotImplementedException(); }
        }

        public override DataTemplate ComboboxTemplate {
            get { throw new NotImplementedException(); }
        }
    }
}
