using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using MudSharp.Form.Characteristics;

namespace Futuremud_Configuration_Tool.Converters {
    class ChargenDisplayTypeToStringConverter : IValueConverter {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value == DependencyProperty.UnsetValue || value == null || !(value is int)) {
                return "Unknown";
            }

            return ((CharacterGenerationDisplayType)((int)value)).Describe();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            return ((IEnumerable<CharacterGenerationDisplayType>)Enum.GetValues(typeof(CharacterGenerationDisplayType))).FirstOrDefault(x => x.Describe() == value.ToString());
        }

        #endregion
    }
}
