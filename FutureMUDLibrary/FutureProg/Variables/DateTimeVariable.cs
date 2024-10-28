using System;
using System.Collections.Generic;

namespace MudSharp.FutureProg.Variables {
    public class DateTimeVariable : ProgVariable {
        public DateTimeVariable(DateTime dt) {
            UnderlyingDateTime = dt;
        }

        public DateTime UnderlyingDateTime { get; set; }

        public override ProgVariableTypes Type => ProgVariableTypes.DateTime;

        public override object GetObject => UnderlyingDateTime;

        private static ProgVariableTypes DotReferenceHandler(string property) {
            switch (property.ToLowerInvariant()) {
                case "year":
                    return ProgVariableTypes.Number;
                case "month":
                    return ProgVariableTypes.Number;
                case "day":
                    return ProgVariableTypes.Number;
                case "hour":
                    return ProgVariableTypes.Number;
                case "minute":
                    return ProgVariableTypes.Number;
                case "second":
                    return ProgVariableTypes.Number;
                default:
                    return ProgVariableTypes.Error;
            }
        }

        private static IReadOnlyDictionary<string,ProgVariableTypes> DotReferenceHandler()
        {
            return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
            {
                {"year", ProgVariableTypes.Number},
                {"month", ProgVariableTypes.Number},
                {"day", ProgVariableTypes.Number},
                {"hour", ProgVariableTypes.Number},
                {"minute", ProgVariableTypes.Number},
                {"second", ProgVariableTypes.Number},
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
            ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.DateTime, DotReferenceHandler(), DotReferenceHelp());
        }

        public override IProgVariable GetProperty(string property) {
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