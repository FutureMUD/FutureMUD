using System;
using System.Collections.Generic;
using MudSharp.Framework;

namespace MudSharp.FutureProg.Variables {
    public class TextVariable : FutureProgVariable {
        public TextVariable(string theString) {
            UnderlyingString = theString;
        }

        public string UnderlyingString { get; set; }

        public override FutureProgVariableTypes Type => FutureProgVariableTypes.Text;

        public override object GetObject => UnderlyingString;

        private static IReadOnlyDictionary<string,FutureProgVariableTypes> DotReferenceHandler()
        {
            return new Dictionary<string, FutureProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
            {
                {"length", FutureProgVariableTypes.Number},
                {"upper", FutureProgVariableTypes.Text},
                {"lower", FutureProgVariableTypes.Text},
                {"proper", FutureProgVariableTypes.Text},
                {"title", FutureProgVariableTypes.Text},
            };
        }

        private static IReadOnlyDictionary<string,string> DotReferenceHelp()
        {
            return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
            {
                {"length", ""},
                {"upper", ""},
                {"lower", ""},
                {"proper", ""},
                {"title", ""},
            };
        }

        public static void RegisterFutureProgCompiler() {
            FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.Text, DotReferenceHandler(), DotReferenceHelp());
        }

        public override IFutureProgVariable GetProperty(string property) {
            switch (property.ToLowerInvariant()) {
                case "length":
                    return new NumberVariable(UnderlyingString.Length);
                case "upper":
                    return new TextVariable(UnderlyingString.ToUpperInvariant());
                case "lower":
                    return new TextVariable(UnderlyingString.ToLowerInvariant());
                case "proper":
                    return new TextVariable(UnderlyingString.ProperSentences());
                case "title":
                    return new TextVariable(UnderlyingString.TitleCase());
            }
            throw new NotSupportedException();
        }

        #region Overrides of Object

        public override string ToString() {
            return UnderlyingString;
        }

        #endregion
    }
}