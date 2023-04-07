using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Futuremud_Configuration_Tool.Converters {
    public class TraitTypeToStringConverter : IValueConverter {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value == DependencyProperty.UnsetValue || String.IsNullOrEmpty(value.ToString())) {
                return "";
            }

            if (value == null) {
                return "Null";
            }

            switch ((int)value) {
                case 0:
                    return "Skill";
                case 1:
                    return "Attribute";
                case 2:
                    return "Derived";
                default:
                    return "Unknown";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value == null) {
                return null;
            }

            switch (value.ToString()) {
                case "Skill":
                    return 0;
                case "Attribute":
                    return 1;
                case "Derived":
                    return 2;
                default:
                    return null;
            }
        }

        #endregion
    }
}
