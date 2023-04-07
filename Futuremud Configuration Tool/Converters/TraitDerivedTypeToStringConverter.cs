using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Futuremud_Configuration_Tool.Converters {
    class TraitDerivedTypeToStringConverter : IValueConverter {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value == DependencyProperty.UnsetValue || value == null || String.IsNullOrEmpty(value.ToString())) {
                return "Not Derived";
            }

            switch ((int)value) {
                case 0:
                    return "Not Derived";
                case 1:
                    return "Attribute";
                case 2:
                    return "Skill";
                default:
                    return "Unknown";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value == null) {
                return 0;
            }

            switch (value.ToString()) {
                case "Not Derived":
                    return 0;
                case "Skill":
                    return 2;
                case "Attribute":
                    return 1;
                default:
                    return 0;
            }
        }

        #endregion
    }
}
