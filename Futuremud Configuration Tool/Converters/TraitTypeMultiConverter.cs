using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Futuremud_Configuration_Tool.Converters {
    public class TraitTypeMultiConverter : IMultiValueConverter {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (values[0] == DependencyProperty.UnsetValue) {
                return "Unknown";
            }

            switch ((int)values[0]) {
                case 0:
                    return "Skill";
                case 1:
                    return "Attribute";
                case 2:
                    return (int)values[1] == 1 ? "Derived Attribute" : "Derived Skill";
                default:
                    return "Unknown";
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) {
            if (value == null) {
                return new object[] { 0, null };
            }

            switch (value.ToString()) {
                case "Skill":
                    return new object[] { 0, null };
                case "Attribute":
                    return new object[] { 1, null };
                case "Derived Attribute":
                    return new object[] { 2, 1 };
                case "Derived Skill":
                    return new object[] { 2, 2 };
                default:
                    return new object[] { 0, 0 };
            }
        }

        #endregion
    }
}
