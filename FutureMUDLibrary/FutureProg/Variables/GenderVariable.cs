using System;
using System.Collections.Generic;
using MudSharp.Form.Shape;

namespace MudSharp.FutureProg.Variables {
    public class GenderVariable : ProgVariable {
        protected Gender UnderlyingGender;

        public GenderVariable(Gender gender) {
            UnderlyingGender = gender;
        }

        public override ProgVariableTypes Type => ProgVariableTypes.Gender;

        public override object GetObject => UnderlyingGender;

        private static IReadOnlyDictionary<string,ProgVariableTypes> DotReferenceHandler()
        {
            return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
            {
                {"he", ProgVariableTypes.Text},
                {"him", ProgVariableTypes.Text},
                {"his", ProgVariableTypes.Text},
                {"himself", ProgVariableTypes.Text},
                {"class", ProgVariableTypes.Text }
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
                { "class", "returns male, female, neuter, indeterminate as appropriate" }
            };
        }

        public static void RegisterFutureProgCompiler() {
            ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Gender, DotReferenceHandler(), DotReferenceHelp());
        }

        public override IProgVariable GetProperty(string property) {
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
                case "class":
                    return new TextVariable(gender.GenderClass());
            }
            throw new NotSupportedException();
        }
    }
}