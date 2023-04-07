using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using MudSharp.FutureProg;

namespace Futuremud_Configuration_Tool.Converters {
    public class FutureProgVariableTypeToStringConverter : IValueConverter {
        public readonly static IEnumerable<FutureProgVariableTypes> AllTypes;

        static FutureProgVariableTypeToStringConverter() {
            var types = ((IEnumerable<FutureProgVariableTypes>)Enum.GetValues(typeof(FutureProgVariableTypes))).Where(x => x != FutureProgVariableTypes.Collection && x != FutureProgVariableTypes.CollectionItem && x != FutureProgVariableTypes.Error);
            AllTypes = types.Concat(types.Select(x => x | FutureProgVariableTypes.Collection));
        }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value == DependencyProperty.UnsetValue || value == null || !(value is long)) {
                return "Unknown";
            }
            var asEnum = (FutureProgVariableTypes)((long)value);
            return asEnum.Describe();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            return (long)AllTypes.FirstOrDefault(x => x.Describe() == value.ToString());
        }

        #endregion
    }
}
