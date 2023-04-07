using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using MudSharp.Framework;

namespace Futuremud_Configuration_Tool.Converters {
    public class DisplayProperSaveLowerStringConverter : IValueConverter {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            return value.ToString().TitleCase();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            return value.ToString().ToLowerInvariant();
        }

        #endregion
    }
}
