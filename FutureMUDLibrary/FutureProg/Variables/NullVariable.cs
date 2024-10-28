using System;
using System.Collections.Generic;

namespace MudSharp.FutureProg.Variables {
    public class NullVariable : ProgVariable {
        public NullVariable(ProgVariableTypes type) {
            Type = type;
        }

        public override ProgVariableTypes Type { get; }

        public override object GetObject => GetDefaultFor(Type);

        private static IReadOnlyDictionary<string,ProgVariableTypes> DotReferenceHandler()
        {
            return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
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
            ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Void, DotReferenceHandler(), DotReferenceHelp());
        }

        private static Type GetUnderlyingType(ProgVariableTypes type) {
            switch (type) {
                case ProgVariableTypes.Boolean:
                    return typeof(bool);
                case ProgVariableTypes.Number:
                    return typeof(decimal);
                case ProgVariableTypes.Text:
                    return typeof(string);
                case ProgVariableTypes.TimeSpan:
                    return typeof(TimeSpan);
                case ProgVariableTypes.DateTime:
                    return typeof(DateTime);
                default:
                    throw new NotImplementedException();
            }
        }

        private static object GetDefaultFor(ProgVariableTypes type) {
            switch (type) {
                case ProgVariableTypes.Boolean:
                    return default(bool);
                case ProgVariableTypes.Number:
                    return default(decimal);
                case ProgVariableTypes.Text:
                    return default(string);
                case ProgVariableTypes.TimeSpan:
                    return default(TimeSpan);
                case ProgVariableTypes.DateTime:
                    return default(DateTime);
                default:
                    return null;
            }
        }

        public override IProgVariable GetProperty(string property) {
            throw new NotSupportedException("Property for a null object sought in FutureProg.");
        }
    }
}