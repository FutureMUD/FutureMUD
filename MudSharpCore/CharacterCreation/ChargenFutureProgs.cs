using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character.Name;
using MudSharp.CharacterCreation.Roles;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;

namespace MudSharp.CharacterCreation;

public partial class Chargen
{
	#region IFutureProgVariable Members

	public object GetObject => this;

	public FutureProgVariableTypes Type => FutureProgVariableTypes.Chargen;

	public static void RegisterFutureProgCompiler()
	{
		FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.Chargen, DotReferenceHandler(),
			DotReferenceHelp());
		FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.Toon, ToonDotReferenceHandler(),
			ToonDotReferenceHelp());
	}

	public IFutureProgVariable GetProperty(string property)
	{
		IFutureProgVariable returnVar = null;
		switch (property.ToLowerInvariant())
		{
			case "id":
				returnVar = new NumberVariable(Id);
				break;

			case "name":
				returnVar = new TextVariable(SelectedName != null ? SelectedName.GetName(NameStyle.GivenOnly) : "");
				break;

			case "simplefullname":
				returnVar = new TextVariable(SelectedName != null ? SelectedName.GetName(NameStyle.SimpleFull) : "");
				break;

			case "fullname":
				returnVar = new TextVariable(SelectedName != null ? SelectedName.GetName(NameStyle.FullName) : "");
				break;

			case "surname":
				returnVar = new TextVariable(SelectedName != null ? SelectedName.GetName(NameStyle.SurnameOnly) : "");
				break;

			case "race":
				returnVar = SelectedRace;
				break;

			case "culture":
				returnVar = SelectedCulture;
				break;

			case "ethnicity":
				returnVar = SelectedEthnicity;
				break;

			case "age":
				returnVar =
					new NumberVariable(SelectedBirthday?.Calendar.CurrentDate.YearsDifference(SelectedBirthday) ?? 0);
				break;

			case "agecategory":
				returnVar = new TextVariable(SelectedRace
				                             ?.AgeCategory(
					                             SelectedBirthday?.Calendar.CurrentDate.YearsDifference(
						                             SelectedBirthday) ?? 0)
				                             .DescribeEnum() ?? "None");
				break;

			case "height":
				returnVar = new NumberVariable(SelectedHeight);
				break;

			case "weight":
				returnVar = new NumberVariable(SelectedWeight);
				break;

			case "gender":
				returnVar = new GenderVariable(SelectedGender);
				break;

			case "skills":
				returnVar = new CollectionVariable(SelectedSkills, FutureProgVariableTypes.Trait);
				break;

			case "accents":
				returnVar = new CollectionVariable(SelectedAccents, FutureProgVariableTypes.Accent);
				break;
			case "class":
				returnVar =
					new TextVariable(
						SelectedRoles.Where(x => x.RoleType == ChargenRoleType.Class)
						             .Select(x => x.Name)
						             .DefaultIfEmpty("None")
						             .First());
				break;

			case "subclass":
				returnVar =
					new TextVariable(
						SelectedRoles.Where(x => x.RoleType == ChargenRoleType.Subclass)
						             .Select(x => x.Name)
						             .DefaultIfEmpty("None")
						             .First());
				break;

			case "npc":
				returnVar = new BooleanVariable(false);
				break;
			case "pc":
				returnVar = new BooleanVariable(true);
				break;
			case "guest":
				returnVar = new BooleanVariable(false);
				break;

			case "merits":
			case "applicablemerits":
				// Chargen Merits always apply
				returnVar = new CollectionVariable(SelectedMerits.WhereNotNull(x => x).ToList(),
					FutureProgVariableTypes.Merit);
				break;
			case "roles":
				returnVar = new CollectionVariable(SelectedRoles.WhereNotNull(x => x).ToList(),
					FutureProgVariableTypes.Role);
				break;
			case "special":
				returnVar = new BooleanVariable(IsSpecialApplication);
				break;
		}

		return returnVar;
	}

	private static IReadOnlyDictionary<string, FutureProgVariableTypes> ToonDotReferenceHandler()
	{
		return new Dictionary<string, FutureProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", FutureProgVariableTypes.Number },
			{ "name", FutureProgVariableTypes.Text },
			{ "simplefullname", FutureProgVariableTypes.Text },
			{ "fullname", FutureProgVariableTypes.Text },
			{ "surname", FutureProgVariableTypes.Text },
			{ "race", FutureProgVariableTypes.Race },
			{ "culture", FutureProgVariableTypes.Culture },
			{ "ethnicity", FutureProgVariableTypes.Ethnicity },
			{ "age", FutureProgVariableTypes.Number },
			{ "agecategory", FutureProgVariableTypes.Text },
			{ "height", FutureProgVariableTypes.Number },
			{ "weight", FutureProgVariableTypes.Number },
			{ "gender", FutureProgVariableTypes.Gender },
			{ "skills", FutureProgVariableTypes.Trait | FutureProgVariableTypes.Collection },
			{ "accents", FutureProgVariableTypes.Accent | FutureProgVariableTypes.Collection },
			{ "class", FutureProgVariableTypes.Text },
			{ "subclass", FutureProgVariableTypes.Text },
			{ "npc", FutureProgVariableTypes.Boolean },
			{ "guest", FutureProgVariableTypes.Boolean },
			{ "pc", FutureProgVariableTypes.Boolean },
			{ "merits", FutureProgVariableTypes.Merit | FutureProgVariableTypes.Collection },
			{ "applicablemerits", FutureProgVariableTypes.Merit | FutureProgVariableTypes.Collection },
			{ "roles", FutureProgVariableTypes.Role | FutureProgVariableTypes.Collection },
			{ "special", FutureProgVariableTypes.Boolean }
		};
	}

	private static IReadOnlyDictionary<string, string> ToonDotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "" },
			{ "name", "" },
			{ "simplefullname", "" },
			{ "fullname", "" },
			{ "surname", "" },
			{ "race", "" },
			{ "culture", "" },
			{ "ethnicity", "" },
			{ "age", "" },
			{ "agecategory", "" },
			{ "height", "" },
			{ "weight", "" },
			{ "gender", "" },
			{ "skills", "" },
			{ "accents", "" },
			{ "class", "" },
			{ "subclass", "" },
			{ "npc", "" },
			{ "guest", "" },
			{ "pc", "" },
			{ "merits", "" },
			{ "applicablemerits", "" },
			{ "roles", "" },
			{ "special", "" }
		};
	}

	private static IReadOnlyDictionary<string, FutureProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, FutureProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", FutureProgVariableTypes.Number },
			{ "name", FutureProgVariableTypes.Text },
			{ "simplefullname", FutureProgVariableTypes.Text },
			{ "fullname", FutureProgVariableTypes.Text },
			{ "surname", FutureProgVariableTypes.Text },
			{ "race", FutureProgVariableTypes.Race },
			{ "culture", FutureProgVariableTypes.Culture },
			{ "ethnicity", FutureProgVariableTypes.Ethnicity },
			{ "age", FutureProgVariableTypes.Number },
			{ "agecategory", FutureProgVariableTypes.Text },
			{ "height", FutureProgVariableTypes.Number },
			{ "weight", FutureProgVariableTypes.Number },
			{ "gender", FutureProgVariableTypes.Gender },
			{ "skills", FutureProgVariableTypes.Trait | FutureProgVariableTypes.Collection },
			{ "accents", FutureProgVariableTypes.Accent | FutureProgVariableTypes.Collection },
			{ "class", FutureProgVariableTypes.Text },
			{ "subclass", FutureProgVariableTypes.Text },
			{ "npc", FutureProgVariableTypes.Boolean },
			{ "guest", FutureProgVariableTypes.Boolean },
			{ "pc", FutureProgVariableTypes.Boolean },
			{ "merits", FutureProgVariableTypes.Merit | FutureProgVariableTypes.Collection },
			{ "applicablemerits", FutureProgVariableTypes.Merit | FutureProgVariableTypes.Collection },
			{ "roles", FutureProgVariableTypes.Role | FutureProgVariableTypes.Collection },
			{ "special", FutureProgVariableTypes.Boolean }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "" },
			{ "name", "" },
			{ "simplefullname", "" },
			{ "fullname", "" },
			{ "surname", "" },
			{ "race", "" },
			{ "culture", "" },
			{ "ethnicity", "" },
			{ "age", "" },
			{ "agecategory", "" },
			{ "height", "" },
			{ "weight", "" },
			{ "gender", "" },
			{ "skills", "" },
			{ "accents", "" },
			{ "class", "" },
			{ "subclass", "" },
			{ "npc", "" },
			{ "guest", "" },
			{ "pc", "" },
			{ "merits", "" },
			{ "applicablemerits", "" },
			{ "roles", "" },
			{ "special", "" }
		};
	}

	#endregion IFutureProgVariable Members
}