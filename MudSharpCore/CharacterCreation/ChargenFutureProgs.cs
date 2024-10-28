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

	public ProgVariableTypes Type => ProgVariableTypes.Chargen;

	public static void RegisterFutureProgCompiler()
	{
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Chargen, DotReferenceHandler(),
			DotReferenceHelp());
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Toon, ToonDotReferenceHandler(),
			ToonDotReferenceHelp());
	}

	public IProgVariable GetProperty(string property)
	{
		IProgVariable returnVar = null;
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
				returnVar = new CollectionVariable(SelectedSkills, ProgVariableTypes.Trait);
				break;

			case "accents":
				returnVar = new CollectionVariable(SelectedAccents, ProgVariableTypes.Accent);
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
					ProgVariableTypes.Merit);
				break;
			case "roles":
				returnVar = new CollectionVariable(SelectedRoles.WhereNotNull(x => x).ToList(),
					ProgVariableTypes.Role);
				break;
			case "special":
				returnVar = new BooleanVariable(ApplicationType == ApplicationType.Special);
				break;
			case "simple":
				returnVar = new BooleanVariable(ApplicationType == ApplicationType.Simple);
				break;
		}

		return returnVar;
	}

	private static IReadOnlyDictionary<string, ProgVariableTypes> ToonDotReferenceHandler()
	{
		return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", ProgVariableTypes.Number },
			{ "name", ProgVariableTypes.Text },
			{ "simplefullname", ProgVariableTypes.Text },
			{ "fullname", ProgVariableTypes.Text },
			{ "surname", ProgVariableTypes.Text },
			{ "race", ProgVariableTypes.Race },
			{ "culture", ProgVariableTypes.Culture },
			{ "ethnicity", ProgVariableTypes.Ethnicity },
			{ "age", ProgVariableTypes.Number },
			{ "agecategory", ProgVariableTypes.Text },
			{ "height", ProgVariableTypes.Number },
			{ "weight", ProgVariableTypes.Number },
			{ "gender", ProgVariableTypes.Gender },
			{ "skills", ProgVariableTypes.Trait | ProgVariableTypes.Collection },
			{ "accents", ProgVariableTypes.Accent | ProgVariableTypes.Collection },
			{ "class", ProgVariableTypes.Text },
			{ "subclass", ProgVariableTypes.Text },
			{ "npc", ProgVariableTypes.Boolean },
			{ "guest", ProgVariableTypes.Boolean },
			{ "pc", ProgVariableTypes.Boolean },
			{ "merits", ProgVariableTypes.Merit | ProgVariableTypes.Collection },
			{ "applicablemerits", ProgVariableTypes.Merit | ProgVariableTypes.Collection },
			{ "roles", ProgVariableTypes.Role | ProgVariableTypes.Collection },
			{ "special", ProgVariableTypes.Boolean },
			{ "simple", ProgVariableTypes.Boolean }
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
			{ "special", "" },
			{ "simple", "" }
		};
	}

	private static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", ProgVariableTypes.Number },
			{ "name", ProgVariableTypes.Text },
			{ "simplefullname", ProgVariableTypes.Text },
			{ "fullname", ProgVariableTypes.Text },
			{ "surname", ProgVariableTypes.Text },
			{ "race", ProgVariableTypes.Race },
			{ "culture", ProgVariableTypes.Culture },
			{ "ethnicity", ProgVariableTypes.Ethnicity },
			{ "age", ProgVariableTypes.Number },
			{ "agecategory", ProgVariableTypes.Text },
			{ "height", ProgVariableTypes.Number },
			{ "weight", ProgVariableTypes.Number },
			{ "gender", ProgVariableTypes.Gender },
			{ "skills", ProgVariableTypes.Trait | ProgVariableTypes.Collection },
			{ "accents", ProgVariableTypes.Accent | ProgVariableTypes.Collection },
			{ "class", ProgVariableTypes.Text },
			{ "subclass", ProgVariableTypes.Text },
			{ "npc", ProgVariableTypes.Boolean },
			{ "guest", ProgVariableTypes.Boolean },
			{ "pc", ProgVariableTypes.Boolean },
			{ "merits", ProgVariableTypes.Merit | ProgVariableTypes.Collection },
			{ "applicablemerits", ProgVariableTypes.Merit | ProgVariableTypes.Collection },
			{ "roles", ProgVariableTypes.Role | ProgVariableTypes.Collection },
			{ "special", ProgVariableTypes.Boolean },
			{ "simple", ProgVariableTypes.Boolean }
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
			{ "special", "" },
			{ "simple", "" }
		};
	}

	#endregion IFutureProgVariable Members
}