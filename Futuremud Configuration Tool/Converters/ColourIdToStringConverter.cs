using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using MudSharp.Database;

namespace Futuremud_Configuration_Tool.Converters {
    public class ColourIdToStringConverter : IValueConverter {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value == DependencyProperty.UnsetValue || value == null) {
                return DependencyProperty.UnsetValue;
            }

            using (new FMDB()) {
                return FMDB.Context.Colours.Find(int.Parse((string)value));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value == DependencyProperty.UnsetValue || value == null) {
                return null;
            }
            return ((FME.Colour)value).Id;
        }

        #endregion
    }
}
