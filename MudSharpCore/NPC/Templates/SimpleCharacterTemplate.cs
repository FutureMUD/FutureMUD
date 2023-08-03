using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.Disfigurements;
using MudSharp.Body.Traits;
using MudSharp.Character.Heritage;
using MudSharp.Character.Name;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Roles;
using MudSharp.Communication.Language;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Shape;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.RPG.Knowledge;
using MudSharp.RPG.Merits;
using MudSharp.TimeAndDate.Date;

namespace MudSharp.NPC.Templates;

public class SimpleCharacterTemplate : ICharacterTemplate
{
	#region ICharacterTemplate Members

	public List<IAccent> SelectedAccents { get; set; }

	public List<ITrait> SelectedAttributes { get; set; }

	public MudDate SelectedBirthday { get; set; }

	public List<Tuple<ICharacteristicDefinition, ICharacteristicValue>> SelectedCharacteristics { get; set; }

	public ICulture SelectedCulture { get; set; }

	public List<IEntityDescriptionPattern> SelectedEntityDescriptionPatterns { get; set; }

	public IEthnicity SelectedEthnicity { get; set; }

	public string SelectedFullDesc { get; set; }

	public Gender SelectedGender { get; set; }

	public double SelectedHeight { get; set; }

	public IPersonalName SelectedName { get; set; }

	public IRace SelectedRace { get; set; }

	public string SelectedSdesc { get; set; }

	public List<Tuple<ITraitDefinition, double>> SkillValues { get; set; }

	public List<ITraitDefinition> SelectedSkills
	{
		get { return SkillValues.Select(x => x.Item1).ToList(); }
	}

	public double SelectedWeight { get; set; }

	public ICell SelectedStartingLocation { get; set; }

	public List<IChargenRole> SelectedRoles { get; set; }

	public List<ICharacterMerit> SelectedMerits { get; set; }

	public List<IKnowledge> SelectedKnowledges { get; set; }

	public Alignment Handedness { get; set; }

	public List<IBodypart> MissingBodyparts { get; set; }

	public List<(IDisfigurementTemplate Disfigurement, IBodypart Bodypart)> SelectedDisfigurements { get; set; }

	public List<IGameItemProto> SelectedProstheses { get; set; }

	public IAccount Account => DummyAccount.Instance;

	public string NeedsModel => "NoNeeds";

	#endregion

	#region IFutureProgVariable

	public object GetObject => this;

	public FutureProgVariableTypes Type => FutureProgVariableTypes.Chargen;

	public IFutureProgVariable GetProperty(string property)
	{
		IFutureProgVariable returnVar = null;
		switch (property.ToLowerInvariant())
		{
			case "id":
				returnVar = new NumberVariable(0);
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

			case "age":
				returnVar =
					new NumberVariable(SelectedBirthday?.Calendar.CurrentDate.YearsDifference(SelectedBirthday) ?? 0);
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
				returnVar = new BooleanVariable(true);
				break;
			case "guest":
				returnVar = new BooleanVariable(false);
				break;
			case "pc":
				returnVar = new BooleanVariable(false);
				break;

			case "merits":
			case "applicablemerits":
				// Chargen Merits always apply
				returnVar = new CollectionVariable(SelectedMerits.ToList(), FutureProgVariableTypes.Merit);
				break;

			case "ethnicity":
				returnVar = SelectedEthnicity;
				break;

			case "agecategory":
				returnVar = new TextVariable(SelectedRace
				                             ?.AgeCategory(
					                             SelectedBirthday?.Calendar.CurrentDate.YearsDifference(
						                             SelectedBirthday) ?? 0)
				                             .DescribeEnum() ?? "None");
				break;
			case "roles":
				returnVar = new CollectionVariable(SelectedRoles.WhereNotNull(x => x).ToList(),
					FutureProgVariableTypes.Role);
				break;
			case "special":
				returnVar = new BooleanVariable(false);
				break;
			case "simple":
				return new BooleanVariable(false);
		}

		return returnVar;
	}

	#endregion

	#region Implementation of IHaveFuturemud

	public IFuturemud Gameworld { get; init; }

	#endregion
}