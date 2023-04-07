using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Futuremud_Configuration_Tool.Model {
    public abstract class FuturemudBaseControl : UserControl {
        public abstract string FilterLabelText { get; }
        public abstract ICollection<object> Dataset { get; }
        public abstract ICollection<object> FilteredDataset(IEnumerable<object> set, object filter);
        public abstract IEnumerable<object> FilterOptions { get; }
        public abstract object AddNew();
        public abstract void DeleteObject(object target);
        public abstract object CopyObject(object target);
        public abstract void SelectionChanged (SelectionChangedEventArgs args);
        public abstract DataTemplate SelectionListboxTemplate { get; }
        public abstract DataTemplate ComboboxTemplate { get; }

        internal class NullFilterObjectClass {
            public override string ToString() {
                return "(None)";
            }
        }

        private static Object _nullFilterObject = new NullFilterObjectClass();
        public object UnfilteredOptionObject {
            get { return _nullFilterObject; }
        }
    }
}
