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
		IEnumerable<ProgVariableTypes> Parameters { get; }

		List<Tuple<ProgVariableTypes, string>> NamedParameters { get; }

		bool AcceptsAnyParameters { get; set; }

		/// <summary>
		///     The return type of this program
		/// </summary>
		ProgVariableTypes ReturnType { get; set; }

		/// <summary>
		///     The time that the program took to compile
		/// </summary>
		TimeSpan CompileTime { get; }

		bool MatchesParameters(IEnumerable<ProgVariableTypes> parameters);

		/// <summary>
		///     Executes the program and returns the result
		/// </summary>
		/// <param name="variables">The parameters to be passed in to execute the program</param>
		/// <returns>An object of the ReturnType of the program, or null in the case of an error</returns>
		object Execute(params object[] variables);
		object ExecuteWithRecursionProtection(params object[] variables);
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
		public static bool CompatibleWith(this IEnumerable<ProgVariableTypes> parameters,
			IEnumerable<ProgVariableTypes> referenceParameters) {
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

		public static string Describe(this ProgVariableTypes type) {
			var sb = new StringBuilder();
			if (type == ProgVariableTypes.Anything) {
				return "Anything";
			}

			if (type.HasFlag(ProgVariableTypes.Literal)) {
				type = type ^ ProgVariableTypes.Literal;
			}

			if (type.HasFlag(ProgVariableTypes.Collection))
			{
				sb.Append(" Collection");
				type = type ^ ProgVariableTypes.Collection;
			}
			else if (type.HasFlag(ProgVariableTypes.Dictionary))
			{
				sb.Append(" Dictionary");
				type = type ^ ProgVariableTypes.Dictionary;
			}
			else if (type.HasFlag(ProgVariableTypes.CollectionDictionary))
			{
				sb.Append(" CollectionDictionary");
				type = type ^ ProgVariableTypes.CollectionDictionary;
			}

			switch (type) {
				case ProgVariableTypes.CollectionItem:
					sb.Insert(0, "CollectionItem");
					break;
				case ProgVariableTypes.Boolean:
					sb.Insert(0, "Boolean");
					break;
				case ProgVariableTypes.Character:
					sb.Insert(0, "Character");
					break;
				case ProgVariableTypes.Error:
					sb.Insert(0, "Error");
					break;
				case ProgVariableTypes.Gender:
					sb.Insert(0, "Gender");
					break;
				case ProgVariableTypes.Item:
					sb.Insert(0, "Item");
					break;
				case ProgVariableTypes.Location:
					sb.Insert(0, "Location");
					break;
				case ProgVariableTypes.Number:
					sb.Insert(0, "Number");
					break;
				case ProgVariableTypes.Shard:
					sb.Insert(0, "Shard");
					break;
				case ProgVariableTypes.Text:
					sb.Insert(0, "Text");
					break;
				case ProgVariableTypes.Void:
					sb.Insert(0, "Void");
					break;
				case ProgVariableTypes.Zone:
					sb.Insert(0, "Zone");
					break;
				case ProgVariableTypes.Race:
					sb.Insert(0, "Race");
					break;
				case ProgVariableTypes.Culture:
					sb.Insert(0, "Culture");
					break;
				case ProgVariableTypes.Chargen:
					sb.Insert(0, "Chargen");
					break;
				case ProgVariableTypes.Trait:
					sb.Insert(0, "Trait");
					break;
				case ProgVariableTypes.Clan:
					sb.Insert(0, "Clan");
					break;
				case ProgVariableTypes.ClanAppointment:
					sb.Insert(0, "Appointment");
					break;
				case ProgVariableTypes.ClanPaygrade:
					sb.Insert(0, "Paygrade");
					break;
				case ProgVariableTypes.ClanRank:
					sb.Insert(0, "Rank");
					break;
				case ProgVariableTypes.Currency:
					sb.Insert(0, "Currency");
					break;
				case ProgVariableTypes.Exit:
					sb.Insert(0, "Exit");
					break;
				case ProgVariableTypes.Perceiver:
					sb.Insert(0, "Perceiver");
					break;
				case ProgVariableTypes.Perceivable:
					sb.Insert(0, "Perceivable");
					break;
				case ProgVariableTypes.MagicResourceHaver:
					sb.Insert(0, "MagicResourceHaver");
					break;
				case ProgVariableTypes.Toon:
					sb.Insert(0, "Toon");
					break;
				case ProgVariableTypes.Accent:
					sb.Insert(0, "Accent");
					break;
				case ProgVariableTypes.Language:
					sb.Insert(0, "Language");
					break;
				case ProgVariableTypes.DateTime:
					sb.Insert(0, "DateTime");
					break;
				case ProgVariableTypes.TimeSpan:
					sb.Insert(0, "TimeSpan");
					break;
				case ProgVariableTypes.Merit:
					sb.Insert(0, "Merit");
					break;
				case ProgVariableTypes.MudDateTime:
					sb.Insert(0, "MudDateTime");
					break;
				case ProgVariableTypes.Calendar:
					sb.Insert(0, "Calendar");
					break;
				case ProgVariableTypes.Clock:
					sb.Insert(0, "Clock");
					break;
				case ProgVariableTypes.Effect:
					sb.Insert(0, "Effect");
					break;
				case ProgVariableTypes.Knowledge:
					sb.Insert(0, "Knowledge");
					break;
				case ProgVariableTypes.Tagged:
					sb.Insert(0, "Tagged");
					break;
				case ProgVariableTypes.Shop:
					sb.Insert(0, "Shop");
					break;
				case ProgVariableTypes.Merchandise:
					sb.Insert(0, "Merchandise");
					break;
				case ProgVariableTypes.Outfit:
					sb.Insert(0, "Outfit");
					break;
				case ProgVariableTypes.OutfitItem:
					sb.Insert(0, "OutfitItem");
					break;
				case ProgVariableTypes.Project:
					sb.Insert(0, "Project");
					break;
				case ProgVariableTypes.OverlayPackage:
					sb.Insert(0, "OverlayPackage");
					break;
				case ProgVariableTypes.Terrain:
					sb.Insert(0, "Terrain");
					break;
				case ProgVariableTypes.Material:
					sb.Insert(0, "Material");
					break;
				case ProgVariableTypes.Solid:
					sb.Insert(0, "Solid");
					break;
				case ProgVariableTypes.Liquid:
					sb.Insert(0, "Liquid");
					break;
				case ProgVariableTypes.Gas:
					sb.Insert(0, "Gas");
					break;
				case ProgVariableTypes.MagicSchool:
					sb.Insert(0, "MagicSchool");
					break;
				case ProgVariableTypes.MagicCapability:
					sb.Insert(0, "MagicCapability");
					break;
				case ProgVariableTypes.MagicSpell:
					sb.Insert(0, "MagicSpell");
					break;
				case ProgVariableTypes.Bank:
					sb.Insert(0, "Bank");
					break;
				case ProgVariableTypes.BankAccount:
					sb.Insert(0, "BankAccount");
					break;
				case ProgVariableTypes.BankAccountType:
					sb.Insert(0, "BankAccountType");
					break;
				case ProgVariableTypes.Law:
					sb.Insert(0, "Law");
					break;
				case ProgVariableTypes.LegalAuthority:
					sb.Insert(0, "LegalAuthority");
					break;
				case ProgVariableTypes.Crime:
					sb.Insert(0, "Crime");
					break;
				case ProgVariableTypes.Role:
					sb.Insert(0, "Role");
					break;
				case ProgVariableTypes.Ethnicity:
					sb.Insert(0, "Ethnicity");
					break;
				case ProgVariableTypes.Drug:
					sb.Insert(0, "Drug");
					break;
				case ProgVariableTypes.WeatherEvent:
					sb.Insert(0, "WeatherEvent");
					break;
				case ProgVariableTypes.Market:
					sb.Insert(0, "Market");
					break;
				case ProgVariableTypes.MarketCategory:
					sb.Insert(0, "MarketCategory");
					break;
				case ProgVariableTypes.LiquidMixture:
					sb.Insert(0, "LiquidMixture");
					break;
				case ProgVariableTypes.ReferenceType:
					sb.Insert(0, "ReferenceType");
					break;
				case ProgVariableTypes.ValueType:
					sb.Insert(0, "ValueType");
					break;
				default:
					return "Unknown Type";
			}

			return sb.ToString();
		}
	}
}