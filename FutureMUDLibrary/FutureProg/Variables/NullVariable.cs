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
            return type.ExactKind switch {
                ProgTypeKind.Boolean => typeof(bool),
                ProgTypeKind.Number => typeof(decimal),
                ProgTypeKind.Text => typeof(string),
                ProgTypeKind.TimeSpan => typeof(TimeSpan),
                ProgTypeKind.DateTime => typeof(DateTime),
                _ => throw new NotImplementedException()
            };
        }

        private static object GetDefaultFor(ProgVariableTypes type) {
            return type.ExactKind switch {
                ProgTypeKind.Boolean => default(bool),
                ProgTypeKind.Number => default(decimal),
                ProgTypeKind.Text => default(string),
                ProgTypeKind.TimeSpan => default(TimeSpan),
                ProgTypeKind.DateTime => default(DateTime),
                _ => null
            };
        }

        public override IProgVariable GetProperty(string property) {
            throw new NotSupportedException("Property for a null object sought in FutureProg.");
        }
    }
}
