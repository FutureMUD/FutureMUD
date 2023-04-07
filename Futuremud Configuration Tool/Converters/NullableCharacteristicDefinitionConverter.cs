using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using FME;
using MudSharp.Database;

namespace Futuremud_Configuration_Tool.Converters {
    public class NullableCharacteristicDefinitionConverter : IValueConverter {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value == DependencyProperty.UnsetValue || value == null) {
                return "None";
            }

            var cValue = (CharacteristicDefinition)value;
            return String.Format("ID: {0} - {1}", cValue.Id, cValue.Name);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value == null || value.ToString() == "None") {
                return null;
            }

            return FMDB.Context.CharacteristicDefinitions.Find(int.Parse(value.ToString().Split(' ')[1]));
        }

        #endregion
    }
}
