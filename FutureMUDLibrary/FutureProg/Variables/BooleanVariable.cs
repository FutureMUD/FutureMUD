using System;
using System.Collections.Generic;

namespace MudSharp.FutureProg.Variables {
    public class BooleanVariable : FutureProgVariable {
        public BooleanVariable(bool boolean) {
            UnderlyingBoolean = boolean;
        }

        public bool UnderlyingBoolean { get; set; }

        public override FutureProgVariableTypes Type => FutureProgVariableTypes.Boolean;

        public override object GetObject => UnderlyingBoolean;

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
            FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.Boolean, DotReferenceHandler(), DotReferenceHelp());
        }

        public override IFutureProgVariable GetProperty(string property) {
            throw new NotSupportedException();
        }
    }
}