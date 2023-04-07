using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using MudSharp.RPG.Checks;

namespace Futuremud_Configuration_Tool.Converters {
    public class DifficultyToStringConverter : IValueConverter {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value == DependencyProperty.UnsetValue || value == null || !(value is int)) {
                return "Unknown";
            }

            return ((Difficulty)((int)value)).Describe();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            return ((IEnumerable<Difficulty>)Enum.GetValues(typeof(Difficulty))).FirstOrDefault(x => x.Describe() == value.ToString());
        }

        #endregion
    }
}
