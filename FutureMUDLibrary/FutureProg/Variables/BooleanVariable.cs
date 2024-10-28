using System;
using System.Collections.Generic;

namespace MudSharp.FutureProg.Variables {
    public class BooleanVariable : ProgVariable {
        public BooleanVariable(bool boolean) {
            UnderlyingBoolean = boolean;
        }

        public bool UnderlyingBoolean { get; set; }

        public override ProgVariableTypes Type => ProgVariableTypes.Boolean;

        public override object GetObject => UnderlyingBoolean;

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
            ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Boolean, DotReferenceHandler(), DotReferenceHelp());
        }

        public override IProgVariable GetProperty(string property) {
            throw new NotSupportedException();
        }
    }
}