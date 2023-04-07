using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Futuremud_Configuration_Tool.Converters {
    public class NullableStringConverter : IValueConverter {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value == DependencyProperty.UnsetValue || value == null) {
                return "";
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value == null || value.ToString() == "") {
                return null;
            }

            return value;
        }

        #endregion
    }
}
