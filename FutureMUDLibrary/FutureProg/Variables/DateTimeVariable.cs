using System;
using System.Collections.Generic;

namespace MudSharp.FutureProg.Variables {
    public class DateTimeVariable : FutureProgVariable {
        public DateTimeVariable(DateTime dt) {
            UnderlyingDateTime = dt;
        }

        public DateTime UnderlyingDateTime { get; set; }

        public override FutureProgVariableTypes Type => FutureProgVariableTypes.DateTime;

        public override object GetObject => UnderlyingDateTime;

        private static FutureProgVariableTypes DotReferenceHandler(string property) {
            switch (property.ToLowerInvariant()) {
                case "year":
                    return FutureProgVariableTypes.Number;
                case "month":
                    return FutureProgVariableTypes.Number;
                case "day":
                    return FutureProgVariableTypes.Number;
                case "hour":
                    return FutureProgVariableTypes.Number;
                case "minute":
                    return FutureProgVariableTypes.Number;
                case "second":
                    return FutureProgVariableTypes.Number;
                default:
                    return FutureProgVariableTypes.Error;
            }
        }

        private static IReadOnlyDictionary<string,FutureProgVariableTypes> DotReferenceHandler()
        {
            return new Dictionary<string, FutureProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
            {
                {"year", FutureProgVariableTypes.Number},
                {"month", FutureProgVariableTypes.Number},
                {"day", FutureProgVariableTypes.Number},
                {"hour", FutureProgVariableTypes.Number},
                {"minute", FutureProgVariableTypes.Number},
                {"second", FutureProgVariableTypes.Number},
            };
        }

        private static IReadOnlyDictionary<string,string> DotReferenceHelp()
        {
            return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
            {
                {"year", ""},
                {"month", ""},
                {"day", ""},
                {"hour", ""},
                {"minute", ""},
                {"second", ""},
            };
        }

        public static void RegisterFutureProgCompiler() {
            FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.DateTime, DotReferenceHandler(), DotReferenceHelp());
        }

        public override IFutureProgVariable GetProperty(string property) {
            switch (property.ToLowerInvariant()) {
                case "year":
                    return new NumberVariable(UnderlyingDateTime.Year);
                case "month":
                    return new NumberVariable(UnderlyingDateTime.Month);
                case "day":
                    return new NumberVariable(UnderlyingDateTime.Day);
                case "hour":
                    return new NumberVariable(UnderlyingDateTime.Hour);
                case "minute":
                    return new NumberVariable(UnderlyingDateTime.Minute);
                case "second":
                    return new NumberVariable(UnderlyingDateTime.Second);
                default:
                    throw new NotSupportedException();
            }
        }
    }
}