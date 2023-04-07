using System;
using System.Collections.Generic;

namespace MudSharp.FutureProg.Variables {
    public class TimeSpanVariable : FutureProgVariable {
        public TimeSpanVariable(TimeSpan span) {
            UnderlyingTimeSpan = span;
        }

        public TimeSpan UnderlyingTimeSpan { get; set; }

        public override FutureProgVariableTypes Type => FutureProgVariableTypes.TimeSpan;

        public override object GetObject => UnderlyingTimeSpan;

        private static IReadOnlyDictionary<string,FutureProgVariableTypes> DotReferenceHandler()
        {
            return new Dictionary<string, FutureProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
            {
                {"days", FutureProgVariableTypes.Number},
                {"hours", FutureProgVariableTypes.Number},
                {"minutes", FutureProgVariableTypes.Number},
                {"seconds", FutureProgVariableTypes.Number},
                {"milliseconds", FutureProgVariableTypes.Number},
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
            FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.TimeSpan, DotReferenceHandler(), DotReferenceHelp());
        }

        public override IFutureProgVariable GetProperty(string property) {
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