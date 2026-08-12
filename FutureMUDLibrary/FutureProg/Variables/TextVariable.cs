using MudSharp.Framework;
using System;
using System.Collections.Generic;

namespace MudSharp.FutureProg.Variables
{
    public class TextVariable : ProgVariable
    {
        public TextVariable(string theString)
        {
            UnderlyingString = theString;
        }

        public string UnderlyingString { get; set; }

        public override ProgVariableTypes Type => ProgVariableTypes.Text;

        public override object GetObject => UnderlyingString;

        private static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
        {
            return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
            {
                {"length", ProgVariableTypes.Number},
                {"upper", ProgVariableTypes.Text},
                {"lower", ProgVariableTypes.Text},
                {"proper", ProgVariableTypes.Text},
                {"title", ProgVariableTypes.Text},
            };
        }

        private static IReadOnlyDictionary<string, string> DotReferenceHelp()
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

        public static void RegisterFutureProgCompiler()
        {
            ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Text, DotReferenceHandler(), DotReferenceHelp());
        }

        public override IProgVariable GetProperty(string property)
        {
			switch (CanonicalProperty(property))
            {
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

		private static string CanonicalProperty(string property)
		{
			if (property is "length" or "upper" or "lower" or "proper" or "title")
			{
				return property;
			}

			if (property.Equals("length", StringComparison.OrdinalIgnoreCase)) return "length";
			if (property.Equals("upper", StringComparison.OrdinalIgnoreCase)) return "upper";
			if (property.Equals("lower", StringComparison.OrdinalIgnoreCase)) return "lower";
			if (property.Equals("proper", StringComparison.OrdinalIgnoreCase)) return "proper";
			if (property.Equals("title", StringComparison.OrdinalIgnoreCase)) return "title";
			return property;
		}

        #region Overrides of Object

        public override string ToString()
        {
            return UnderlyingString;
        }

        #endregion
    }
}
