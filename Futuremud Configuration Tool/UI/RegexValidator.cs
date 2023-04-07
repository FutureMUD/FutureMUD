using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace Futuremud_Configuration_Tool.UI {
    /*
    
    In order to use this validator in a UI Element include the following:
     
    <Binding Path="Example" UpdateSourceTrigger="PropertyChanged">
                        <Binding.ValidationRules>
                            <ui:RegexValidator 
                                Regex = "[a-zA-Z]{3,3}[0-9]{5,5}[a-zA-Z]{1,1}" 
                                Message="You must enter a valid asset number."/>
                        </Binding.ValidationRules>
                    </Binding>
    
    */

    [ValueConversion(typeof(ReadOnlyObservableCollection<ValidationError>), typeof(string))]
    public class ValidationErrorsToStringConverter : MarkupExtension, IValueConverter {
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return new ValidationErrorsToStringConverter();
        }

        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture) {
            ReadOnlyObservableCollection<ValidationError> errors =
                value as ReadOnlyObservableCollection<ValidationError>;

            if (errors == null) {
                return string.Empty;
            }

            return string.Join("\n", (from e in errors
                                      select e.ErrorContent as string).ToArray());
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture) {
            throw new NotImplementedException();
        }

        public ValidationErrorsToStringConverter() {
        }
    }


    public class RegexTypeConverter : TypeConverter {

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
            if (sourceType == typeof(string)) {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
            if (value is string) {
                return new Regex(value as string);
            }
            return base.ConvertFrom(context, culture, value);
        }
    }

    public class RegexValidator : ValidationRule {
        public string Regex {
            get {
                return ActualRegex.ToString();
            }
            set {
                ActualRegex = new Regex(value);
            }
        }
        protected Regex ActualRegex { get; set; }
        public string Message { get; set; }

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo) {
            if (ActualRegex.IsMatch((string)value))
                return ValidationResult.ValidResult;
            else
                return new ValidationResult(false, Message);
        }

        public RegexValidator(string regex, string message) {
            Regex = regex;
            Message = message;
        }

        public RegexValidator() {
        }
    }
}
