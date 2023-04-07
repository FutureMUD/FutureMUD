using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using FME;

namespace Futuremud_Configuration_Tool.Converters {
    public class FutureProgNullableConverter : IValueConverter {
        public class DummyFutureProg {
            public int Id { get; set; }
            public string FunctionName { get; set; }
        }
        public static DummyFutureProg DummyProg = new DummyFutureProg { Id = 0, FunctionName = "None" };

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value == DependencyProperty.UnsetValue || value == null) {
                return DummyProg;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value is FutureProg) {
                return value;
            }
            return null;
        }

        #endregion
    }
}
