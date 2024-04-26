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
using MudSharp.Body.Traits.Subtypes;

namespace MudSharp.NPC.Templates;

public class SimpleCharacterTemplate : ICharacterTemplate
{
	#region ICharacterTemplate Members

	public List<IAccent> SelectedAccents { get; init; }

	public List<ITrait> SelectedAttributes { get; init; }

	public MudDate SelectedBirthday { get; init; }

	public List<Tuple<ICharacteristicDefinition, ICharacteristicValue>> SelectedCharacteristics { get; init; }

	public ICulture SelectedCulture { get; init; }

	public List<IEntityDescriptionPattern> SelectedEntityDescriptionPatterns { get; set; }

	public IEthnicity SelectedEthnicity { get; init; }

	public string SelectedFullDesc { get; set; }

	public Gender SelectedGender { get; init; }

	public double SelectedHeight { get; init; }

	public IPersonalName SelectedName { get; init; }

	public IRace SelectedRace { get; init; }

	public string SelectedSdesc { get; set; }

	public List<Tuple<ITraitDefinition, double>> SkillValues { get; init; }

	public List<ITraitDefinition> SelectedSkills
	{
		get { return SkillValues.Select(x => x.Item1).ToList(); }
	}

	public double SelectedWeight { get; init; }

	public ICell SelectedStartingLocation { get; set; }

	public List<IChargenRole> SelectedRoles { get; init; }

	public List<ICharacterMerit> SelectedMerits { get; init; }

	public List<IKnowledge> SelectedKnowledges { get; init; }

	public Alignment Handedness { get; init; }

	public List<IBodypart> MissingBodyparts { get; init; }

	public List<(IDisfigurementTemplate Disfigurement, IBodypart Bodypart)> SelectedDisfigurements { get; init; }

	public List<IGameItemProto> SelectedProstheses { get; init; }

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

	#region IHaveTraits Members
	public long Id => 0L;
	public string Name => SelectedName.GetName(NameStyle.FullName);
	public string FrameworkItemType => "CharacterTemplate";
	public IEnumerable<ITrait> Traits => SelectedAttributes.Concat(SkillValues.Select(x => new TemporaryTrait { Definition = x.Item1, Owner = this, Value = x.Item2 }));
	public double TraitValue(ITraitDefinition trait, TraitBonusContext context = TraitBonusContext.None)
	{
		var value = GetTrait(trait);
		if (value is null)
		{
			return 0.0;
		}

		return value.Value;
	}

	public double TraitRawValue(ITraitDefinition trait) => TraitValue(trait);
	public double TraitMaxValue(ITraitDefinition trait)
	{
		if (trait is ISkillDefinition sd)
		{
			return sd.Cap.Evaluate(this);
		}

		return trait.MaxValue;
	}

	public double TraitMaxValue(ITrait trait)
	{
		if (trait.Definition is ISkillDefinition sd)
		{
			return sd.Cap.Evaluate(this);
		}

		return trait.MaxValue;
	}

	public bool HasTrait(ITraitDefinition trait) => Traits.Any(x => x.Definition == trait);

	public ITrait GetTrait(ITraitDefinition definition)
	{
		if (definition is ISkillDefinition)
		{
			if (!SelectedSkills.Contains(definition))
			{
				return null;
			}

			return new TemporaryTrait { Definition = definition, Owner = this, Value = SkillValues.First(x => x.Item1 == definition).Item2 };
		}

		return SelectedAttributes.FirstOrDefault(x => x.Definition == definition);
	}
	public string GetTraitDecorated(ITraitDefinition trait) => trait.Decorator.Decorate(GetTrait(trait));
	public IEnumerable<ITrait> TraitsOfType(TraitType type) => Traits.Where(x => x.Definition.TraitType == type);

	public bool AddTrait(ITraitDefinition trait, double value)
	{
		if (trait is ISkillDefinition)
		{
			if (SelectedSkills.Contains(trait))
			{
				return false;
			}

			SelectedSkills.Add(trait);
			SkillValues.Add(Tuple.Create(trait, value));
			return true;
		}
		if (SelectedAttributes.Any(x => x.Definition == trait))
		{
			return false;
		}

		SelectedAttributes.Add(new TemporaryTrait { Definition = trait, Owner = this, Value = value });
		return true;
	}

	public bool RemoveTrait(ITraitDefinition trait)
	{
		if (trait is ISkillDefinition)
		{
			if (!SelectedSkills.Contains(trait))
			{
				return false;
			}

			SelectedSkills.Remove(trait);
			SkillValues.RemoveAll(x => x.Item1 == trait);
			return true;
		}

		if (!SelectedAttributes.Any(x => x.Definition == trait))
		{
			return false;
		}

		SelectedAttributes.RemoveAll(x => x.Definition == trait);
		return true;
	}

	public bool SetTraitValue(ITraitDefinition trait, double value)
	{
		if (trait is ISkillDefinition)
		{
			if (!SelectedSkills.Contains(trait))
			{
				SelectedSkills.Add(trait);
			}

			SkillValues.RemoveAll(x => x.Item1 == trait);
			SkillValues.Add(Tuple.Create(trait, value));
			return true;
		}

		SelectedAttributes.RemoveAll(x => x.Definition == trait);
		SelectedAttributes.Add(new TemporaryTrait { Definition = trait, Owner = this, Value = value });
		return true;
	}
	#endregion
}