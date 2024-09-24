using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp.FutureProg {
    public interface IFutureProg : IFrameworkItem, ISaveable {
        /// <summary>
        ///     The name of the program, used to invoke it. Must be unique for each variable space pattern.
        /// </summary>
        string FunctionName { get; set; }

        /// <summary>
        ///     An optional comment from the creator of the FutureProg about the operations or purpose of the program
        /// </summary>
        string FunctionComment { get; set; }

        /// <summary>
        ///     The original uncompiled text of the program.
        /// </summary>
        string FunctionText { get; set; }

        /// <summary>
        ///     The category to which this program belongs.
        /// </summary>
        string Category { get; set; }

        /// <summary>
        ///     The subcategory within the category to which this program belongs.
        /// </summary>
        string Subcategory { get; set; }

        /// <summary>
        ///     The original uncompiled text of the program coloured by the lexer
        /// </summary>
        string ColourisedFunctionText { get; }

        /// <summary>
        ///     If the program fails to compile, this will contain the error message.
        /// </summary>
        string CompileError { get; }

        /// <summary>
        ///     Whether or not the futureProg should be visible to those who are not administrators
        /// </summary>
        bool Public { get; set; }

        /// <summary>
        ///     An ordered collection of the parameters required to be used to invoke this program
        /// </summary>
        IEnumerable<FutureProgVariableTypes> Parameters { get; }

        List<Tuple<FutureProgVariableTypes, string>> NamedParameters { get; }

        bool AcceptsAnyParameters { get; set; }

        /// <summary>
        ///     The return type of this program
        /// </summary>
        FutureProgVariableTypes ReturnType { get; set; }

        /// <summary>
        ///     The time that the program took to compile
        /// </summary>
        TimeSpan CompileTime { get; }

        bool MatchesParameters(IEnumerable<FutureProgVariableTypes> parameters);

        /// <summary>
        ///     Executes the program and returns the result
        /// </summary>
        /// <param name="variables">The parameters to be passed in to execute the program</param>
        /// <returns>An object of the ReturnType of the program, or null in the case of an error</returns>
        object Execute(params object[] variables);
        T Execute<T>(params object[] variables);

        double ExecuteDouble(params object[] variables);
        double ExecuteDouble(double defaultIfNull, params object[] variables);
        decimal ExecuteDecimal(params object[] variables);
        decimal ExecuteDecimal(decimal defaultIfNull, params object[] variables);
        int ExecuteInt(params object[] variables);
        int ExecuteInt(int defaultIfNull, params object[] variables);
		long ExecuteLong(params object[] variables);
		long ExecuteLong(long defaultIfNull, params object[] variables);
		bool ExecuteBool(params object[] variables);
		bool ExecuteBool(bool defaultIfNull, params object[] variables);
        string ExecuteString(params object[] variables);
		IEnumerable<T> ExecuteCollection<T>(params object[] variables);
		IReadOnlyDictionary<string, T> ExecuteDictionary<T>(params object[] variables);
		IReadOnlyCollectionDictionary<string, T> ExecuteCollectionDictionary<T>(params object[] variables);

		bool Compile();

        string MXPClickableFunctionName();
        string MXPClickableFunctionNameWithId();
        void ColouriseFunctionText();
        
		FutureProgStaticType StaticType { get; set; }
    }

    public enum FutureProgStaticType
    {
        NotStatic,
        StaticByParameters,
        FullyStatic
    }

    public static class FutureProgExtensions {
        public static bool CompatibleWith(this IEnumerable<FutureProgVariableTypes> parameters,
            IEnumerable<FutureProgVariableTypes> referenceParameters) {
            if (referenceParameters.Count() != parameters.Count()) {
                return false;
            }

            for (var i = 0; i < referenceParameters.Count(); i++) {
                if (!parameters.ElementAt(i).CompatibleWith(referenceParameters.ElementAt(i))) {
                    return false;
                }
            }

            return true;
        }

        public static string DescribeParameters(this IFutureProg prog) {
            return prog.NamedParameters.Select(x => $"{x.Item1.Describe()}").ListToString(conjunction: "");
        }

        public static string Describe(this FutureProgVariableTypes type) {
            var sb = new StringBuilder();
            if (type == FutureProgVariableTypes.Anything) {
                return "Anything";
            }

            if (type.HasFlag(FutureProgVariableTypes.Literal)) {
                type = type ^ FutureProgVariableTypes.Literal;
            }

            if (type.HasFlag(FutureProgVariableTypes.Collection))
            {
	            sb.Append(" Collection");
	            type = type ^ FutureProgVariableTypes.Collection;
            }
            else if (type.HasFlag(FutureProgVariableTypes.Dictionary))
            {
	            sb.Append(" Dictionary");
	            type = type ^ FutureProgVariableTypes.Dictionary;
            }
            else if (type.HasFlag(FutureProgVariableTypes.CollectionDictionary))
            {
	            sb.Append(" CollectionDictionary");
	            type = type ^ FutureProgVariableTypes.CollectionDictionary;
            }

            switch (type) {
                case FutureProgVariableTypes.CollectionItem:
                    sb.Insert(0, "CollectionItem");
                    break;
                case FutureProgVariableTypes.Boolean:
                    sb.Insert(0, "Boolean");
                    break;
                case FutureProgVariableTypes.Character:
                    sb.Insert(0, "Character");
                    break;
                case FutureProgVariableTypes.Error:
                    sb.Insert(0, "Error");
                    break;
                case FutureProgVariableTypes.Gender:
                    sb.Insert(0, "Gender");
                    break;
                case FutureProgVariableTypes.Item:
                    sb.Insert(0, "Item");
                    break;
                case FutureProgVariableTypes.Location:
                    sb.Insert(0, "Location");
                    break;
                case FutureProgVariableTypes.Number:
                    sb.Insert(0, "Number");
                    break;
                case FutureProgVariableTypes.Shard:
                    sb.Insert(0, "Shard");
                    break;
                case FutureProgVariableTypes.Text:
                    sb.Insert(0, "Text");
                    break;
                case FutureProgVariableTypes.Void:
                    sb.Insert(0, "Void");
                    break;
                case FutureProgVariableTypes.Zone:
                    sb.Insert(0, "Zone");
                    break;
                case FutureProgVariableTypes.Race:
                    sb.Insert(0, "Race");
                    break;
                case FutureProgVariableTypes.Culture:
                    sb.Insert(0, "Culture");
                    break;
                case FutureProgVariableTypes.Chargen:
                    sb.Insert(0, "Chargen");
                    break;
                case FutureProgVariableTypes.Trait:
                    sb.Insert(0, "Trait");
                    break;
                case FutureProgVariableTypes.Clan:
                    sb.Insert(0, "Clan");
                    break;
                case FutureProgVariableTypes.ClanAppointment:
                    sb.Insert(0, "Appointment");
                    break;
                case FutureProgVariableTypes.ClanPaygrade:
                    sb.Insert(0, "Paygrade");
                    break;
                case FutureProgVariableTypes.ClanRank:
                    sb.Insert(0, "Rank");
                    break;
                case FutureProgVariableTypes.Currency:
                    sb.Insert(0, "Currency");
                    break;
                case FutureProgVariableTypes.Exit:
                    sb.Insert(0, "Exit");
                    break;
                case FutureProgVariableTypes.Perceiver:
                    sb.Insert(0, "Perceiver");
                    break;
                case FutureProgVariableTypes.Perceivable:
                    sb.Insert(0, "Perceivable");
                    break;
                case FutureProgVariableTypes.MagicResourceHaver:
                    sb.Insert(0, "MagicResourceHaver");
                    break;
                case FutureProgVariableTypes.Toon:
                    sb.Insert(0, "Toon");
                    break;
                case FutureProgVariableTypes.Accent:
                    sb.Insert(0, "Accent");
                    break;
                case FutureProgVariableTypes.Language:
                    sb.Insert(0, "Language");
                    break;
                case FutureProgVariableTypes.DateTime:
                    sb.Insert(0, "DateTime");
                    break;
                case FutureProgVariableTypes.TimeSpan:
                    sb.Insert(0, "TimeSpan");
                    break;
                case FutureProgVariableTypes.Merit:
                    sb.Insert(0, "Merit");
                    break;
                case FutureProgVariableTypes.MudDateTime:
                    sb.Insert(0, "MudDateTime");
                    break;
                case FutureProgVariableTypes.Calendar:
                    sb.Insert(0, "Calendar");
                    break;
                case FutureProgVariableTypes.Clock:
                    sb.Insert(0, "Clock");
                    break;
                case FutureProgVariableTypes.Effect:
                    sb.Insert(0, "Effect");
                    break;
                case FutureProgVariableTypes.Knowledge:
                    sb.Insert(0, "Knowledge");
                    break;
                case FutureProgVariableTypes.Tagged:
                    sb.Insert(0, "Tagged");
                    break;
                case FutureProgVariableTypes.Shop:
                    sb.Insert(0, "Shop");
                    break;
                case FutureProgVariableTypes.Merchandise:
                    sb.Insert(0, "Merchandise");
                    break;
                case FutureProgVariableTypes.Outfit:
                    sb.Insert(0, "Outfit");
                    break;
                case FutureProgVariableTypes.OutfitItem:
                    sb.Insert(0, "OutfitItem");
                    break;
                case FutureProgVariableTypes.Project:
                    sb.Insert(0, "Project");
                    break;
                case FutureProgVariableTypes.OverlayPackage:
                    sb.Insert(0, "OverlayPackage");
                    break;
                case FutureProgVariableTypes.Terrain:
                    sb.Insert(0, "Terrain");
                    break;
                case FutureProgVariableTypes.Material:
                    sb.Insert(0, "Material");
                    break;
                case FutureProgVariableTypes.Solid:
                    sb.Insert(0, "Solid");
                    break;
                case FutureProgVariableTypes.Liquid:
                    sb.Insert(0, "Liquid");
                    break;
                case FutureProgVariableTypes.Gas:
                    sb.Insert(0, "Gas");
                    break;
                case FutureProgVariableTypes.MagicSchool:
                    sb.Insert(0, "MagicSchool");
                    break;
                case FutureProgVariableTypes.MagicCapability:
                    sb.Insert(0, "MagicCapability");
                    break;
                case FutureProgVariableTypes.MagicSpell:
                    sb.Insert(0, "MagicSpell");
                    break;
                case FutureProgVariableTypes.Bank:
                    sb.Insert(0, "Bank");
                    break;
                case FutureProgVariableTypes.BankAccount:
                    sb.Insert(0, "BankAccount");
                    break;
                case FutureProgVariableTypes.BankAccountType:
                    sb.Insert(0, "BankAccountType");
                    break;
                case FutureProgVariableTypes.Law:
                    sb.Insert(0, "Law");
                    break;
                case FutureProgVariableTypes.LegalAuthority:
                    sb.Insert(0, "LegalAuthority");
                    break;
                case FutureProgVariableTypes.Crime:
                    sb.Insert(0, "Crime");
                    break;
                case FutureProgVariableTypes.Role:
	                sb.Insert(0, "Role");
                    break;
                case FutureProgVariableTypes.Ethnicity:
	                sb.Insert(0, "Ethnicity");
                    break;
                case FutureProgVariableTypes.Drug:
	                sb.Insert(0, "Drug");
                    break;
                case FutureProgVariableTypes.WeatherEvent:
	                sb.Insert(0, "WeatherEvent");
                    break;
                case FutureProgVariableTypes.Market:
	                sb.Insert(0, "Market");
	                break;
                case FutureProgVariableTypes.MarketCategory:
	                sb.Insert(0, "MarketCategory");
	                break;
				case FutureProgVariableTypes.ReferenceType:
	                sb.Insert(0, "ReferenceType");
                    break;
                case FutureProgVariableTypes.ValueType:
	                sb.Insert(0, "ValueType");
                    break;
                default:
                    return "Unknown Type";
            }

            return sb.ToString();
        }
    }
}