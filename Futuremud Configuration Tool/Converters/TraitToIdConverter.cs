using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Futuremud_Configuration_Tool.Converters {
    public class TraitToIdConverter : IValueConverter {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value == null) {
                return null;
            }
            using (new FMDB()) {
                return FMDB.Context.Traits.Find((int)value);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value == null) {
                return null;
            }
            return ((FME.Trait)value).Id;
        }

        #endregion
    }
}
