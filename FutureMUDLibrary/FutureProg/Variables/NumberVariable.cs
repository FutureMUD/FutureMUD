using System;
using System.Collections.Generic;
using System.Globalization;

namespace MudSharp.FutureProg.Variables {
    public class NumberVariable : FutureProgVariable {
        protected decimal UnderlyingNumber;

        public NumberVariable(decimal number) {
            UnderlyingNumber = number;
        }

        public NumberVariable(long number) {
            UnderlyingNumber = number;
        }

        public NumberVariable(double number) {
            UnderlyingNumber = (decimal) number;
        }

        public NumberVariable(int number) {
            UnderlyingNumber = number;
        }

        public override FutureProgVariableTypes Type => FutureProgVariableTypes.Number;

        public override object GetObject => UnderlyingNumber;

        private static IReadOnlyDictionary<string,FutureProgVariableTypes> DotReferenceHandler()
        {
            return new Dictionary<string, FutureProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
            {
            };
        }

        private static IReadOnlyDictionary<string,string> DotReferenceHelp()
        {
            return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
            {
            };
        }

        public static void RegisterFutureProgCompiler() {
            FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.Number, DotReferenceHandler(), DotReferenceHelp());
        }

        public override IFutureProgVariable GetProperty(string property) {
            throw new NotSupportedException();
        }

        #region Overrides of Object

        public override string ToString() {
            return UnderlyingNumber.ToString(CultureInfo.InvariantCulture);
        }

        #endregion
    }
}