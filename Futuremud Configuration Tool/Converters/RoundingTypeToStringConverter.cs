using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using MudSharp.RPG.Economy;

namespace Futuremud_Configuration_Tool.Converters {
    public class RoundingTypeToStringConverter : IValueConverter {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value == DependencyProperty.UnsetValue || value == null || !(value is int)) {
                return "Unknown";
            }

            return Enum.GetName(typeof(RoundingMode), value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value.ToString() == "Unknown") {
                return 0;
            }

            return ((IEnumerable<RoundingMode>)Enum.GetValues(typeof(RoundingMode))).FirstOrDefault(x => Enum.GetName(typeof(RoundingMode), x) == value.ToString());
        }

        #endregion
    }
}
