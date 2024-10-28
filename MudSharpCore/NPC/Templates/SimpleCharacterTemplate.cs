using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
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
using MudSharp.TimeAndDate;
using Org.BouncyCastle.Crypto.Agreement;
using MudSharp.Health;

namespace MudSharp.NPC.Templates;

public record SimpleCharacterTemplate : ICharacterTemplate
{
	public SimpleCharacterTemplate()
	{
	}

	public SimpleCharacterTemplate(XElement definition, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		SelectedAccents = new List<IAccent>(
			definition
				.Element("SelectedAccents")
				.Elements("Accent")
				.SelectNotNull(x => gameworld.Accents.Get(long.Parse(x.Value))));
		SelectedAttributes = new List<ITrait>(
				definition
					.Element("SelectedAttributes")
					.Elements("Attribute")
					.SelectNotNull( x =>
						TraitFactory.LoadAttribute(
							gameworld.Traits.Get(long.Parse(x.Attribute("id").Value)) as IAttributeDefinition, 
							null,
							double.Parse(x.Attribute("value").Value)
							)
					)
			);
		SelectedBirthday = MudDate.ParseFromText(definition.Element("SelectedBirthday").Value, Gameworld);
		SelectedCharacteristics = new(
			from item in definition.Element("SelectedCharacteristics").Elements("Characteristic")
			let tuple = (Gameworld.Characteristics.Get(long.Parse(item.Attribute("definition").Value)), Gameworld.CharacteristicValues.Get(long.Parse(item.Attribute("value").Value)))
			where tuple.Item1 is not null && tuple.Item2 is not null
			select tuple	
		);
		SelectedCulture = gameworld.Cultures.Get(long.Parse(definition.Element("SelectedCulture").Value));
		SelectedDisfigurements = new(
			from item in definition.Element("SelectedDisfigurements").Elements("Disfigurement")
			let tuple = (
				Gameworld.DisfigurementTemplates.Get(long.Parse(item.Attribute("template").Value)),
				Gameworld.BodypartPrototypes.Get(long.Parse(item.Attribute("bodypart").Value))
				)
			where tuple.Item1 is not null && tuple.Item2 is not null
			select tuple
		);
		SelectedEntityDescriptionPatterns = new(
			from item in definition.Element("SelectedEntityDescriptionPatterns").Elements("Pattern")
			let pattern = Gameworld.EntityDescriptionPatterns.Get(long.Parse(item.Value))
			where pattern is not null
			select pattern
		);
		SelectedEthnicity = Gameworld.Ethnicities.Get(long.Parse(definition.Element("SelectedEthnicity").Value));
		SelectedFullDesc = definition.Element("SelectedFullDesc").Value;
		SelectedGender = (Gender)short.Parse(definition.Element("SelectedGender").Value);
		SelectedHeight = double.Parse(definition.Element("SelectedHeight").Value);
		SelectedKnowledges = new(
			from item in definition.Element("SelectedKnowledges").Elements("Knowledge")
			let knowledge = Gameworld.Knowledges.Get(long.Parse(item.Value))
			where knowledge is not null
			select knowledge
		);
		SelectedMerits = new(
			from item in definition.Element("SelectedMerits").Elements("Merit")
			let merit = Gameworld.Merits.Get(long.Parse(item.Value)) as ICharacterMerit
			where merit is not null
			select merit
		);
		SelectedName = new PersonalName(definition.Element("SelectedName").Element("Name"), Gameworld);
		SelectedProstheses = new(
			from item in definition.Element("SelectedProstheses").Elements("Item")
			let proto = Gameworld.ItemProtos.Get(long.Parse(item.Value))
			where proto is not null
			select proto
		);

		SelectedRace = Gameworld.Races.Get(long.Parse(definition.Element("SelectedRace").Value));
		SelectedRoles = new(
			from item in definition.Element("SelectedRoles").Elements("Role")
			let role = Gameworld.Roles.Get(long.Parse(item.Value))
			where role is not null
			select role
		);
		SelectedSdesc = definition.Element("SelectedSdesc").Value;
		SelectedStartingLocation = Gameworld.Cells.Get(long.Parse(definition.Element("SelectedStartingLocation").Value));
		SelectedWeight = double.Parse(definition.Element("SelectedWeight").Value);
		SkillValues = new(
			from item in definition.Element("SkillValues").Elements("Skill")
			let tuple = (
				Gameworld.Traits.Get(long.Parse(item.Attribute("definition").Value)),
				double.Parse(item.Attribute("value").Value)
				)
			where tuple.Item1 is not null
			select tuple
		);
		Handedness = (Alignment)int.Parse(definition.Element("Handedness").Value);
		MissingBodyparts = new(
			from item in definition.Element("MissingBodyparts").Elements("Bodypart")
			let bodypart = Gameworld.BodypartPrototypes.Get(long.Parse(item.Value))
			where bodypart is not null
			select bodypart
		);
		HealthStrategy = Gameworld.HealthStrategies.Get(long.Parse(definition.Element("HealthStrategy")?.Value ?? "0"));
	}

	public XElement SaveToXml()
	{
		return new XElement("Character",
			new XElement("SelectedAccents",
				from accent in SelectedAccents
				select new XElement("Accent", accent.Id)
			),
			new XElement("SelectedAttributes",
				from attribute in SelectedAttributes
				select new XElement("Attribute",
					new XAttribute("id", attribute.Definition.Id),
					new XAttribute("value", attribute.RawValue)
				)
			),
			new XElement("SelectedBirthday",
				SelectedBirthday.GetRoundtripString()
			),
			new XElement("SelectedCharacteristics",
				from item in SelectedCharacteristics
				select new XElement("Characteristic",
					new XAttribute("definition", item.Item1.Id),
					new XAttribute("value", item.Item2.Id)
				)
			),
			new XElement("SelectedCulture", SelectedCulture.Id),
			new XElement("SelectedDisfigurements",
				from item in SelectedDisfigurements
				select new XElement("Disfigurement",
					new XAttribute("template", item.Disfigurement.Id),
					new XAttribute("bodypart", item.Bodypart.Id)
				)
			),
			new XElement("SelectedEntityDescriptionPatterns",
				from item in SelectedEntityDescriptionPatterns
				select new XElement("Pattern", item.Id)
			),
			new XElement("SelectedEthnicity", SelectedEthnicity.Id),
			new XElement("SelectedFullDesc", new XCData(SelectedFullDesc)),
			new XElement("SelectedGender", (short)SelectedGender),
			new XElement("SelectedHeight", SelectedHeight),
			new XElement("SelectedKnowledges",
				from item in SelectedKnowledges
				select new XElement("Knowledge", item.Id)
			),
			new XElement("SelectedMerits",
				from item in SelectedMerits
				select new XElement("Merit", item.Id)
			),
			new XElement("SelectedName", SelectedName.SaveToXml()),
			new XElement("SelectedProstheses",
				from item in SelectedProstheses
				select new XElement("Item", item.Id)
			),
			new XElement("SelectedRace", SelectedRace.Id),
			new XElement("SelectedRoles", 
				from item in SelectedRoles
				select new XElement("Role", item.Id)
			),
			new XElement("SelectedSdesc", new XCData(SelectedSdesc)),
			new XElement("SelectedStartingLocation", SelectedStartingLocation?.Id ?? 0),
			new XElement("SelectedWeight", SelectedWeight),
			new XElement("SkillValues", 
				from item in SkillValues
				select new XElement("Skill",
					new XAttribute("definition", item.Item1.Id),
					new XAttribute("value", item.Item2)
				)
			),
			new XElement("Handedness", (int)Handedness),
			new XElement("MissingBodyparts", 
				from item in MissingBodyparts
				select new XElement("Bodypart", item.Id)
			),
			new XElement("HealthStrategy", HealthStrategy?.Id ?? 0L)
		); // TODO
	}

	#region ICharacterTemplate Members

	public List<IAccent> SelectedAccents { get; init; }

	public List<ITrait> SelectedAttributes { get; init; }

	public MudDate SelectedBirthday { get; init; }
	public IHealthStrategy? HealthStrategy { get; init; }

	public List<(ICharacteristicDefinition, ICharacteristicValue)> SelectedCharacteristics { get; init; }

	public ICulture SelectedCulture { get; init; }

	public List<IEntityDescriptionPattern> SelectedEntityDescriptionPatterns { get; set; }

	public IEthnicity SelectedEthnicity { get; init; }

	public string SelectedFullDesc { get; set; }

	public Gender SelectedGender { get; init; }

	public double SelectedHeight { get; init; }

	public IPersonalName SelectedName { get; init; }

	public IRace SelectedRace { get; init; }

	public string SelectedSdesc { get; set; }

	public List<(ITraitDefinition, double)> SkillValues { get; init; }

	public List<ITraitDefinition> SelectedSkills
	{
		get { return SkillValues.Select(x => x.Item1).ToList(); }
	}

	public double SelectedWeight { get; init; }

	public ICell SelectedStartingLocation { get; init; }

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

	public ProgVariableTypes Type => ProgVariableTypes.Chargen;

	public IProgVariable GetProperty(string property)
	{
		IProgVariable returnVar = null;
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
				returnVar = new CollectionVariable(SelectedMerits.ToList(), ProgVariableTypes.Merit);
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
					ProgVariableTypes.Role);
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
			SkillValues.Add((trait, value));
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
			SkillValues.Add((trait, value));
			return true;
		}

		SelectedAttributes.RemoveAll(x => x.Definition == trait);
		SelectedAttributes.Add(new TemporaryTrait { Definition = trait, Owner = this, Value = value });
		return true;
	}
	#endregion
}