using System;
using System.Collections.Generic;
using MudSharp.Form.Shape;

namespace MudSharp.FutureProg.Variables {
    public class GenderVariable : FutureProgVariable {
        protected Gender UnderlyingGender;

        public GenderVariable(Gender gender) {
            UnderlyingGender = gender;
        }

        public override FutureProgVariableTypes Type => FutureProgVariableTypes.Gender;

        public override object GetObject => UnderlyingGender;

        private static IReadOnlyDictionary<string,FutureProgVariableTypes> DotReferenceHandler()
        {
            return new Dictionary<string, FutureProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
            {
                {"he", FutureProgVariableTypes.Text},
                {"him", FutureProgVariableTypes.Text},
                {"his", FutureProgVariableTypes.Text},
                {"himself", FutureProgVariableTypes.Text},
            };
        }

        private static IReadOnlyDictionary<string,string> DotReferenceHelp()
        {
            return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
            {
                {"he", "returns he, she, it or they as appropriate"},
                {"him", "returns him, her, it or them as appropriate"},
                {"his", "returns his, her, its or theirs as appropriate"},
                {"himself", "returns himself, herself, itself or theirself as appropriate"},
            };
        }

        public static void RegisterFutureProgCompiler() {
            FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.Gender, DotReferenceHandler(), DotReferenceHelp());
        }

        public override IFutureProgVariable GetProperty(string property) {
            var gender = Gendering.Get(UnderlyingGender);
            switch (property.ToLowerInvariant()) {
                case "he":
                    return new TextVariable(gender.Subjective());
                case "him":
                    return new TextVariable(gender.Objective());
                case "his":
                    return new TextVariable(gender.Possessive());
                case "himself":
                    return new TextVariable(gender.Reflexive());
            }
            throw new NotSupportedException();
        }
    }
}