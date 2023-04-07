using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using MudSharp.Form.Colour;

namespace Futuremud_Configuration_Tool.Converters {
    class BasicColourToStringConverter : IValueConverter {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value == DependencyProperty.UnsetValue || value == null || !(value is int)) {
                return "Unknown";
            }

            return ((BasicColour)((int)value)).Describe();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (int)(Enum.GetValues(typeof(BasicColour))).OfType<BasicColour>().First(x => x.Describe() == value.ToString());
        }

        #endregion
    }
}
