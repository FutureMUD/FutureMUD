using System;
using System.Collections.Generic;

namespace MudSharp.FutureProg.Variables {
    public class TimeSpanVariable : ProgVariable {
        public TimeSpanVariable(TimeSpan span) {
            UnderlyingTimeSpan = span;
        }

        public TimeSpan UnderlyingTimeSpan { get; set; }

        public override ProgVariableTypes Type => ProgVariableTypes.TimeSpan;

        public override object GetObject => UnderlyingTimeSpan;

        private static IReadOnlyDictionary<string,ProgVariableTypes> DotReferenceHandler()
        {
            return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
            {
                {"days", ProgVariableTypes.Number},
                {"hours", ProgVariableTypes.Number},
                {"minutes", ProgVariableTypes.Number},
                {"seconds", ProgVariableTypes.Number},
                {"milliseconds", ProgVariableTypes.Number},
            };
        }

        private static IReadOnlyDictionary<string,string> DotReferenceHelp()
        {
            return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
            {
                {"days", ""},
                {"hours", ""},
                {"minutes", ""},
                {"seconds", ""},
                {"milliseconds", ""},
            };
        }

        public static void RegisterFutureProgCompiler() {
            ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.TimeSpan, DotReferenceHandler(), DotReferenceHelp());
        }

        public override IProgVariable GetProperty(string property) {
            switch (property.ToLowerInvariant()) {
                case "days":
                    return new NumberVariable(UnderlyingTimeSpan.Days);
                case "hours":
                    return new NumberVariable(UnderlyingTimeSpan.Hours);
                case "minutes":
                    return new NumberVariable(UnderlyingTimeSpan.Minutes);
                case "seconds":
                    return new NumberVariable(UnderlyingTimeSpan.Seconds);
                case "milliseconds":
                    return new NumberVariable(UnderlyingTimeSpan.Milliseconds);
                default:
                    throw new NotSupportedException();
            }
        }
    }
}