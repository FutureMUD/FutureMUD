using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Futuremud_Configuration_Tool.Converters {
    public class BytesToMegaBytesConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            return String.Format("{0:N2}", System.Convert.ToDouble(value) / (1024 * 1024));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            return Binding.DoNothing;
        }
    }
}
