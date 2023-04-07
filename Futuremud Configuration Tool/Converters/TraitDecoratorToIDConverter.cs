using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using MudSharp.Database;

namespace Futuremud_Configuration_Tool.Converters {
    public class TraitDecoratorToIDConverter : IValueConverter {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            using (new FMDB()) {
                return FMDB.Context.TraitDecorators.Find((int)value);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            return ((FME.TraitDecorator)value).Id;
        }

        #endregion
    }
}
