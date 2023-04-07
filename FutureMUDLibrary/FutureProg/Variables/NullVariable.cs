using System;
using System.Collections.Generic;

namespace MudSharp.FutureProg.Variables {
    public class NullVariable : FutureProgVariable {
        public NullVariable(FutureProgVariableTypes type) {
            Type = type;
        }

        public override FutureProgVariableTypes Type { get; }

        public override object GetObject => GetDefaultFor(Type);

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
            FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.Void, DotReferenceHandler(), DotReferenceHelp());
        }

        private static Type GetUnderlyingType(FutureProgVariableTypes type) {
            switch (type) {
                case FutureProgVariableTypes.Boolean:
                    return typeof(bool);
                case FutureProgVariableTypes.Number:
                    return typeof(decimal);
                case FutureProgVariableTypes.Text:
                    return typeof(string);
                case FutureProgVariableTypes.TimeSpan:
                    return typeof(TimeSpan);
                case FutureProgVariableTypes.DateTime:
                    return typeof(DateTime);
                default:
                    throw new NotImplementedException();
            }
        }

        private static object GetDefaultFor(FutureProgVariableTypes type) {
            switch (type) {
                case FutureProgVariableTypes.Boolean:
                    return default(bool);
                case FutureProgVariableTypes.Number:
                    return default(decimal);
                case FutureProgVariableTypes.Text:
                    return default(string);
                case FutureProgVariableTypes.TimeSpan:
                    return default(TimeSpan);
                case FutureProgVariableTypes.DateTime:
                    return default(DateTime);
                default:
                    return null;
            }
        }

        public override IFutureProgVariable GetProperty(string property) {
            throw new NotSupportedException("Property for a null object sought in FutureProg.");
        }
    }
}