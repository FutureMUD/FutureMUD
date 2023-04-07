using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using MudSharp.Database;

namespace Futuremud_Configuration_Tool.Converters {
    class TraitImproverToIDConverter : IValueConverter {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value == null) {
                return null;
            }

            using (new FMDB()) {
                return FMDB.Context.Improvers.Find((int)value);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value == null) {
                return null;
            }

            return ((FME.Improver)value).Id;
        }

        #endregion
    }
}
