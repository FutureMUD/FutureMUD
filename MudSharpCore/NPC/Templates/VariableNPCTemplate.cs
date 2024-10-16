using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.Disfigurements;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Character.Name;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Roles;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Knowledge;
using MudSharp.RPG.Merits;
using Attribute = MudSharp.Body.Traits.Subtypes.Attribute;
using CharacteristicValue = MudSharp.Form.Characteristics.CharacteristicValue;
using Race = MudSharp.Character.Heritage.Race;
using RandomNameProfile = MudSharp.Character.Name.RandomNameProfile;

namespace MudSharp.NPC.Templates;

public class VariableNPCTemplate : NPCTemplateBase
{
	private readonly List<(Gender Value, int Weight)> _genderChances = new();

	private readonly Dictionary<Gender, IHeightWeightModel> _heightWeightModels =
		new();

	private readonly Dictionary<Gender, IRandomNameProfile> _nameProfiles =
		new();

	private readonly List<IAttributeDefinition> _priorityAttributeDefinitions = new();
	private readonly List<IChargenRole> _roles = new();
	private readonly List<VariableSkillTemplate> _skillTemplates = new();
	private int? _attributeTotal;
	private ICulture _culture;
	private IEthnicity _ethnicity;
	private IEntityDescriptionPattern _fdescPattern;
	private int _maximumAge;
	private int _minimumAge;
	private IRace _race;
	private IEntityDescriptionPattern _sdescPattern;
	private readonly List<(IKnowledge Knowledge, double Chance)> _knowledgeChances = new();
	private int _numberMerits = 0;
	private int _numberFlaws = 0;
	private int _numberQuirks = 0;
	private readonly List<ICharacterMerit> _validMerits = new();

	public VariableNPCTemplate(NpcTemplate template, IFuturemud gameworld) : base(template, gameworld)
	{
		LoadFromXml(XElement.Parse(template.Definition));
	}

	public VariableNPCTemplate(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Variable")
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.NpcTemplates.Find(Id, RevisionNumber);
			dbitem.Definition = SaveDefinition();
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
			RevisionNumber = 0;
		}
	}

	public override INPCTemplate Clone(ICharacter builder)
	{
		using (new FMDB())
		{
			var dbnew = new NpcTemplate
			{
				Id = Gameworld.NpcTemplates.NextID(),
				RevisionNumber = 0,
				Name = Name,
				Type = "Variable",
				Definition = SaveDefinition()
			};
			foreach (var item in ArtificialIntelligences)
			{
				dbnew.NpctemplatesArtificalIntelligences.Add(new NpcTemplatesArtificalIntelligences
				{ Npctemplate = dbnew, AiId = item.Id });
			}

			dbnew.EditableItem = new Models.EditableItem();
			FMDB.Context.EditableItems.Add(dbnew.EditableItem);
			dbnew.EditableItem.BuilderDate = DateTime.UtcNow;
			dbnew.EditableItem.RevisionNumber = dbnew.RevisionNumber;
			dbnew.EditableItem.BuilderAccountId = builder.Account.Id;
			dbnew.EditableItem.RevisionStatus = (int)RevisionStatus.UnderDesign;
			FMDB.Context.NpcTemplates.Add(dbnew);
			FMDB.Context.SaveChanges();

			return new VariableNPCTemplate(dbnew, Gameworld);
		}
	}

	public override string NPCTemplateType => "Variable";

	public override string ReferenceDescription(IPerceiver voyeur) => $"{Name} (#{Id.ToString("N0", voyeur)}r{RevisionNumber.ToString("N0", voyeur)})".ColourCharacter();

	public override string FrameworkItemType => "VariableNPCTemplate";

	private void LoadFromXml(XElement root)
	{
		var element = root.Element("GenderChances");
		foreach (var sub in element.Elements("GenderChance"))
		{
			_genderChances.Add((Gendering.Get(sub.Attribute("Gender").Value).Enum,
				int.Parse(sub.Attribute("Chance").Value)));
		}

		element = root.Element("NameProfiles");
		foreach (var sub in element.Elements("NameProfile"))
		{
			_nameProfiles.Add(Gendering.Get(sub.Attribute("Gender").Value).Enum,
				Gameworld.NameCultures.Get(long.Parse(sub.Attribute("Culture").Value))
				         .RandomNameProfiles.First(x => x.Id == int.Parse(sub.Attribute("Profile").Value)));
		}

		element = root.Element("Race");
		var raceID = long.Parse(element.Value);
		_race = raceID == 0 ? null : Gameworld.Races.Get(raceID);

		element = root.Element("Culture");
		var cultureID = long.Parse(element.Value);
		_culture = cultureID == 0 ? null : Gameworld.Cultures.Get(cultureID);

		element = root.Element("Ethnicity");
		var ethnicityID = long.Parse(element.Value);
		_ethnicity = ethnicityID == 0 ? null : Gameworld.Ethnicities.Get(ethnicityID);

		element = root.Element("HeightWeightModels");
		if (element != null)
		{
			foreach (var item in element.Elements("HeightWeightModel"))
			{
				_heightWeightModels[Gendering.Get(item.Attribute("Gender").Value).Enum] =
					Gameworld.HeightWeightModels.Get(long.Parse(item.Attribute("Model").Value));
			}
		}

		element = root.Element("MinimumAge");
		_minimumAge = int.Parse(element.Value);
		element = root.Element("MaximumAge");
		_maximumAge = int.Parse(element.Value);

		element = root.Element("AttributeTotal");
		var attributeTotal = int.Parse(element.Value);
		_attributeTotal = attributeTotal == 0 ? (int?)null : attributeTotal;

		element = root.Element("SDescPattern");
		var patternID = long.Parse(element.Value);
		_sdescPattern = patternID == 0 ? null : Gameworld.EntityDescriptionPatterns.Get(patternID);

		element = root.Element("FDescPattern");
		patternID = long.Parse(element.Value);
		_fdescPattern = patternID == 0 ? null : Gameworld.EntityDescriptionPatterns.Get(patternID);

		element = root.Element("PriorityAttributes");
		foreach (var sub in element.Elements("Attribute"))
		{
			_priorityAttributeDefinitions.Add((IAttributeDefinition)Gameworld.Traits.Get(long.Parse(sub.Value)));
		}

		element = root.Element("Skills");
		foreach (var sub in element.Elements("Skill"))
		{
			_skillTemplates.Add(new VariableSkillTemplate
			{
				Chance = double.Parse(sub.Attribute("Chance").Value),
				SkillMean = double.Parse(sub.Attribute("Mean").Value),
				SkillStddev = double.Parse(sub.Attribute("Stddev").Value),
				Trait = Gameworld.Traits.Get(long.Parse(sub.Attribute("Trait").Value))
			});
		}

		element = root.Element("Roles");
		if (element != null)
		{
			foreach (var item in element.Elements("Role"))
			{
				_roles.Add(Gameworld.Roles.Get(long.Parse(item.Value)));
			}
		}

		element = root.Element("Merits");
		if (element is not null)
		{
			_numberMerits = int.Parse(element.Attribute("nummerits").Value);
			_numberFlaws = int.Parse(element.Attribute("numflaws").Value);
			_numberQuirks = int.Parse(element.Attribute("numquirks").Value);
			foreach (var sub in element.Elements("Merit"))
			{
				if (Gameworld.Merits.Get(long.Parse(sub.Value)) is ICharacterMerit merit)
				{
					_validMerits.Add(merit);
				}
			}
		}

		element = root.Element("Knowledges");
		if (element is not null)
		{
			foreach (var sub in element.Elements("Knowledge"))
			{
				var knowledge = Gameworld.Knowledges.Get(long.Parse(sub.Attribute("which").Value));
				if (knowledge is null)
				{
					continue;
				}

				_knowledgeChances.Add((knowledge, double.Parse(sub.Attribute("chance").Value)));
			}
		}
	}

	private string SaveDefinition()
	{
		return
			new XElement("Definition", 
				new XElement("OnLoadProg", OnLoadProg?.Id ?? 0),
				new XElement("HealthStrategy", HealthStrategy?.Id ?? 0L),
				new XElement("GenderChances", new object[]
				{
					from item in _genderChances
					select
						new XElement("GenderChance", new XAttribute("Gender", item.Value),
							new XAttribute("Chance", item.Weight))
				}), new XElement("NameProfiles", new object[]
				{
					from item in _nameProfiles
					select
						new XElement("NameProfile", new XAttribute("Gender", item.Key),
							new XAttribute("Culture", item.Value.Culture.Id), new XAttribute("Profile", item.Value.Id))
				}), new XElement("Race", _race?.Id ?? 0),
				new XElement("Culture", _culture?.Id ?? 0),
				new XElement("Ethnicity", _ethnicity?.Id ?? 0),
				new XElement("MinimumAge", _minimumAge), new XElement("MaximumAge", _maximumAge),
				new XElement("AttributeTotal", _attributeTotal ?? 0),
				new XElement("SDescPattern", _sdescPattern?.Id ?? 0),
				new XElement("FDescPattern", _fdescPattern?.Id ?? 0),
				new XElement("HeightWeightModels", new object[]
				{
					from hwmodel in _heightWeightModels
					select
						new XElement("HeightWeightModel", new XAttribute("Model", hwmodel.Value.Id),
							new XAttribute("Gender", hwmodel.Key))
				}), 
				new XElement("PriorityAttributes", new object[]
				{
					from item in _priorityAttributeDefinitions
					select new XElement("Attribute", item.Id)
				}), 
				new XElement("Skills", new object[]
				{
					from item in _skillTemplates
					select
						new XElement("Skill", new XAttribute("Chance", item.Chance),
							new XAttribute("Mean", item.SkillMean), new XAttribute("Stddev", item.SkillStddev),
							new XAttribute("Trait", item.Trait.Id))
				}), 
				new XElement("Roles", new object[]
				{
					from item in _roles
					select new XElement("Role", item.Id)
				}),
				new XElement("Knowledges",
					from item in _knowledgeChances
					select new XElement("Knowledge",
						new XAttribute("which", item.Knowledge.Id),
						new XAttribute("chance", item.Chance)
					)
				),
				new XElement("Merits",
					new XAttribute("nummerits", _numberMerits),
					new XAttribute("numflaws", _numberFlaws),
					new XAttribute("numquirks", _numberQuirks),
					from item in _validMerits
					select new XElement("Merit", item.Id)
				)
				).ToString();
	}

	public override bool CanSubmit()
	{
		return _genderChances.Any() &&
		       _genderChances.All(
			       x => 
				       _heightWeightModels.Any(y => y.Key == x.Value) && 
			            _nameProfiles.Any(y => y.Key == x.Value)) &&
		       _race != null && 
		       _culture != null &&
		       Gameworld.Ethnicities.Any(x => _race.SameRace(x.ParentRace));
	}

	public override string WhyCannotSubmit()
	{
		var errors = new List<string>();

		if (_race == null)
		{
			errors.Add("You must select a race.");
		}

		if (_culture == null)
		{
			errors.Add("You must select a culture.");
		}

		if (_race is not null && _ethnicity is null && Gameworld.Ethnicities.All(x => !_race.SameRace(x.ParentRace)))
		{
			errors.Add("The selected race doesn't have any ethnicities.");
		}

		if (!_genderChances.Any())
		{
			errors.Add("You must set up at least one gender for this NPC.");
		}
		else
		{
			var genders = _genderChances.Where(x => _heightWeightModels.All(y => y.Key != x.Value))
			                            .Select(x => x.Value).ToList();
			if (genders.Any())
			{
				errors.Add(
					$"You are missing height/weight models for the following genders: {genders.Select(x => x.DescribeEnum()).ListToString()}");
			}

			genders = _genderChances.Where(x => _nameProfiles.All(y => y.Key != x.Value))
			                        .Select(x => x.Value).ToList();
			if (genders.Any())
			{
				errors.Add(
					$"You are missing name profiles for the following genders: {genders.Select(x => x.DescribeEnum()).ListToString()}");
			}
		}

		return errors.Select(x => x.ColourIncludingReset(Telnet.Red)).ListToLines(true);
	}

	protected override ICharacterTemplate CharacterTemplate(ICell location)
	{
		var rolledGender = _genderChances.GetWeightedRandom();
		var rolledHeightWeight = _heightWeightModels[rolledGender].GetRandomHeightWeight();
		var rolledAge = Constants.Random.Next(_minimumAge, _maximumAge + 1);
		var statrolls = RollRandomStats(_race.Attributes.Count(),
			_attributeTotal ?? _race.AttributeTotalCap, _race.IndividualAttributeCap,
			_race.DiceExpression);
		var attributeOrder =
			_race.Attributes.OrderBy(
				     x =>
					     _priorityAttributeDefinitions.Contains(x)
						     ? _priorityAttributeDefinitions.IndexOf(x)
						     : Dice.Roll("1d100+100"))
			     .ToList();
		var assignedStats = attributeOrder.Select(x => TraitFactory.LoadAttribute(x, null,
			                                  statrolls[attributeOrder.IndexOf(x)] +
			                                  Convert.ToDouble(_race.AttributeBonusProg.Execute(x, this))))
		                                  .ToList<ITrait>();
		var rolledSkills = _skillTemplates.Where(x => Constants.Random.NextDouble() <= x.Chance)
		                                  .Select(y => (y.Trait,
			                                  RandomUtilities.RandomNormal(y.SkillMean, y.SkillStddev))).ToList();
		var randomName = _nameProfiles[rolledGender].GetRandomPersonalName();
		var birthday = _culture.PrimaryCalendar.GetRandomBirthday(rolledAge);
		var accents =
			Gameworld.Languages.Where(x => rolledSkills.Any(y => y.Item1 == x.LinkedTrait))
			         .SelectMany(x => x.Accents.PickUpToRandom(1))
			         .ToList();
		var ethnicity = _ethnicity ??
		                Gameworld.Ethnicities.Where(x => _race.SameRace(x.ParentRace)).GetRandomElement();
		var knowledges = _knowledgeChances.Where(x => x.Chance >= RandomUtilities.DoubleRandom(0.0, 1.0)).Select(x => x.Knowledge).ToList();
		var merits = new List<ICharacterMerit>();
		var unselectedMerits = _validMerits.ToList();
		var i = 0;
		while (_numberMerits > i++)
		{
			var merit = unselectedMerits.Where(x => x.MeritType == MeritType.Merit).GetRandomElement();
			if (merit is null)
			{
				break;
			}

			unselectedMerits.Remove(merit);
			merits.Add(merit);
		}

		i = 0;
		while (_numberFlaws > i++)
		{
			var merit = unselectedMerits.Where(x => x.MeritType == MeritType.Flaw).GetRandomElement();
			if (merit is null)
			{
				break;
			}

			unselectedMerits.Remove(merit);
			merits.Add(merit);
		}

		i = 0;
		while (_numberQuirks > i++)
		{
			var merit = unselectedMerits.GetRandomElement();
			if (merit is null)
			{
				break;
			}

			unselectedMerits.Remove(merit);
			merits.Add(merit);
		}

		var template = new SimpleCharacterTemplate
		{
			SelectedGender = rolledGender,
			SelectedCulture = _culture,
			SelectedRace = _race,
			SelectedEthnicity = ethnicity,
			SelectedName = randomName,
			SelectedHeight = rolledHeightWeight.Item1,
			SelectedWeight = rolledHeightWeight.Item2,
			SelectedBirthday = birthday,
			SelectedCharacteristics = new(),
			SelectedAttributes = assignedStats,
			SkillValues = rolledSkills,
			SelectedAccents = accents,
			SelectedStartingLocation = location,
			SelectedRoles = _roles,
			SelectedMerits = merits,
			SelectedKnowledges = knowledges,
			MissingBodyparts = new List<IBodypart>(), // TODO
			Handedness = _race.HandednessOptions.GetRandomElement(), // TODO
			SelectedProstheses = new List<GameItems.IGameItemProto>(),
			SelectedDisfigurements = new List<(IDisfigurementTemplate Disfigurement, IBodypart Bodypart)>(),
			Gameworld = Gameworld
		};


		var sdescTemplate = _sdescPattern ?? Gameworld.EntityDescriptionPatterns.Where(
			                                              x =>
				                                              x.Type == EntityDescriptionType.ShortDescription &&
				                                              x.IsValidSelection(template))
		                                              .GetWeightedRandom(x => x.RelativeWeight);


		var fdescTemplate = _fdescPattern ?? Gameworld.EntityDescriptionPatterns.Where(
			                                              x =>
				                                              x.Type == EntityDescriptionType.FullDescription &&
				                                              x.IsValidSelection(template))
		                                              .GetWeightedRandom(x => x.RelativeWeight);

		template.SelectedFullDesc = fdescTemplate.Pattern;
		template.SelectedSdesc = sdescTemplate.Pattern;
		template.SelectedEntityDescriptionPatterns = new List<IEntityDescriptionPattern>
		{
			fdescTemplate,
			sdescTemplate
		};

		var characteristicChoices = template.SelectedRace.Characteristics(template.SelectedGender);
		foreach (var choice in ethnicity.CharacteristicChoices.Where(x => characteristicChoices.Contains(x.Key)))
		{
			template.SelectedCharacteristics.Add((choice.Key,
				choice.Value.GetRandomCharacteristic(template) ?? choice.Value.GetRandomCharacteristic()));
		}

		return template;
	}

	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine(
			$"Variable NPC #{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)} - {Name}".Colour(
				Telnet.Cyan));
		sb.AppendLine();
		using (new FMDB())
		{
			sb.Append(new[]
			{
				string.Format("{1}: {0}",
					FMDB.Context.Accounts.Find(BuilderAccountID).Name.Proper().Colour(Telnet.Green),
					"Created By".Colour(Telnet.Cyan)),
				string.Format("{1}: {0}",
					ReviewerAccountID.HasValue
						? FMDB.Context.Accounts.Find(ReviewerAccountID.Value).Name.Proper().Colour(Telnet.Green)
						: "N/A",
					"Reviewed By".Colour(Telnet.Cyan)),
				string.Format("{1}: {0}", Status.Describe().Colour(Telnet.Green),
					"Status".Colour(Telnet.Cyan))
			}.ArrangeStringsOntoLines(3, (uint)actor.Account.LineFormatLength));
		}

		sb.AppendLine();
		sb.Append(new[]
		{
			$"Race: {(_race != null ? _race.Name.Colour(Telnet.Green) : "None".Colour(Telnet.Red))}",
			$"Ethnicity: {(_ethnicity != null ? _ethnicity.Name.Colour(Telnet.Green) : "Random".Colour(Telnet.Yellow))}",
			$"Culture: {(_culture != null ? _culture.Name.Colour(Telnet.Green) : "None".Colour(Telnet.Red))}"
		}.ArrangeStringsOntoLines(3, (uint)actor.Account.LineFormatLength));

		if (_roles.Any(x => x.RoleType == ChargenRoleType.Class))
		{
			sb.Append(new[]
			{
				$"Class: {_roles.First(x => x.RoleType == ChargenRoleType.Class).Name.Colour(Telnet.Green)}",
				$"Subclass: {_roles.FirstOrDefault(x => x.RoleType == ChargenRoleType.Subclass)?.Name.Colour(Telnet.Green) ?? "None".Colour(Telnet.Red)}",
				$"Health Strategy: {HealthStrategy?.Name.ColourName() ?? "Default".ColourCommand()}"
			}.ArrangeStringsOntoLines(3));
		}

		sb.Append(new[]
		{
			$"Sdesc Pattern: {(_sdescPattern != null ? _sdescPattern.Id.ToString(actor).Colour(Telnet.Green) : "Random".Colour(Telnet.Red))}",
			$"Fdesc Pattern: {(_fdescPattern != null ? _fdescPattern.Id.ToString(actor).Colour(Telnet.Green) : "Random".Colour(Telnet.Red))}"
		}.ArrangeStringsOntoLines(2, (uint)actor.Account.LineFormatLength));

		sb.AppendLine("Gender Chances:");
		foreach (var chance in _genderChances)
		{
			sb.AppendLine(
				$"\t{Gendering.Get(chance.Value).Name.Proper().ColourName()}: {((double)chance.Weight / _genderChances.Sum(y => y.Weight)).ToString("P1", actor).Colour(Telnet.Green)}");
		}

		sb.AppendLine("Name Profiles:");
		foreach (var profile in _nameProfiles)
		{
			sb.AppendLine(
				$"\t{Gendering.Get(profile.Key).Name.Proper().ColourName()}: {profile.Value.Name.Colour(Telnet.Green)} (#{profile.Value.Id.ToString("N0", actor)})");
		}

		sb.AppendLine("Height/Weight Models:");
		foreach (var model in _heightWeightModels)
		{
			sb.AppendLine(
				$"\t{Gendering.Get(model.Key).Name.Proper().ColourName()}: {model.Value.Name.Proper().ColourValue()} (#{model.Value.Id.ToString("N0", actor)})");
		}

		sb.Append(new[]
		{
			$"Attribute Total: {(_attributeTotal.HasValue ? _attributeTotal.Value.ToString(actor).Colour(Telnet.Green) : "As Race".Colour(Telnet.Red))}",
			$"Priority Attributes: {_priorityAttributeDefinitions.Select(x => x.Name.Proper().Colour(Telnet.Green)).ListToString()}"
		}.ArrangeStringsOntoLines(2, (uint)actor.Account.LineFormatLength));

		sb.Append(new[]
		{
			$"Minimum Age: {_minimumAge.ToString(actor).Colour(Telnet.Green)}",
			$"Maximum Age: {_maximumAge.ToString(actor).Colour(Telnet.Green)}"
		}.ArrangeStringsOntoLines(2, (uint)actor.Account.LineFormatLength));
		sb.AppendLine("Skills:");
		foreach (var skill in _skillTemplates)
		{
			sb.AppendLine(string.Format(actor,
				"\t{0} - {1} Chance, Mean {2}, Standard Deviation {3} (99.7% of values between {4} and {5})",
				skill.Trait.Name.Proper().Colour(Telnet.Cyan),
				skill.Chance.ToString("P1", actor).Colour(Telnet.Green),
				skill.SkillMean.ToString("N1", actor).Colour(Telnet.Green),
				skill.SkillStddev.ToString("N1", actor).Colour(Telnet.Green),
				(skill.SkillMean - 3 * skill.SkillStddev).ToString("N1", actor).Colour(Telnet.Green),
				(skill.SkillMean + 3 * skill.SkillStddev).ToString("N1", actor).Colour(Telnet.Green)
			));
		}

		sb.AppendLine();
		if (_roles.Any(x => x.RoleType != ChargenRoleType.Class && x.RoleType != ChargenRoleType.Subclass))
		{
			sb.AppendLine("Roles:");
			sb.AppendLine(
				_roles.Where(
					      x => x.RoleType != ChargenRoleType.Class && x.RoleType != ChargenRoleType.Subclass)
				      .Select(x => string.Format(actor, "#{0:N0}: {1}", x.Id, x.Name.TitleCase()))
				      .ArrangeStringsOntoLines(1));
			sb.AppendLine();
		}

		sb.AppendLine("Merits and Flaws:");
		sb.AppendLine();
		sb.AppendLine($"# Quirks: {_numberQuirks.ToStringN0Colour(actor)}");
		sb.AppendLine($"# Merits: {_numberMerits.ToStringN0Colour(actor)}");
		sb.AppendLine($"# Flaws: {_numberFlaws.ToStringN0Colour(actor)}");
		sb.AppendLine();
		sb.AppendLine($"Valid Merits: {_validMerits.Select(x => x.Name.Colour(x.MeritType == MeritType.Merit ? Telnet.Green : Telnet.Red)).DefaultIfEmpty("None".ColourCommand()).ListToString()}");

		sb.AppendLine();
		sb.AppendLine("Knowledges:");
		sb.AppendLine();
		
		foreach (var knowledge in _knowledgeChances)
		{
			sb.AppendLine($"\t{knowledge.Knowledge.Name.ColourName()} ({knowledge.Chance.ToStringP2Colour(actor)})");
		}

		sb.AppendLine();
		sb.AppendLine($"OnLoadProg: {OnLoadProg?.MXPClickableFunctionName() ?? "None".Colour(Telnet.Red)}");

		sb.AppendLine("AI Routines:");
		sb.AppendLine();
		foreach (var ai in ArtificialIntelligences)
		{
			sb.AppendLine($"AI #{ai.Id}: {ai.Name}");
		}

		if (!CanSubmit())
		{
			sb.AppendLine();
			sb.AppendLine("Building Errors:");
			sb.AppendLine(WhyCannotSubmit().Colour(Telnet.Red));
		}

		return sb.ToString();
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		using (new FMDB())
		{
			var dbnew = new NpcTemplate
			{
				Id = Id,
				RevisionNumber = FMDB.Context.NpcTemplates.Where(x => x.Id == Id)
				                     .Select(x => x.RevisionNumber)
				                     .AsEnumerable()
				                     .DefaultIfEmpty(0)
				                     .Max() + 1,
				Name = Name,
				Type = "Variable",
				Definition = SaveDefinition()
			};
			foreach (var item in ArtificialIntelligences)
			{
				dbnew.NpctemplatesArtificalIntelligences.Add(new NpcTemplatesArtificalIntelligences
					{ Npctemplate = dbnew, AiId = item.Id });
			}

			dbnew.EditableItem = new Models.EditableItem();
			FMDB.Context.EditableItems.Add(dbnew.EditableItem);
			dbnew.EditableItem.BuilderDate = DateTime.UtcNow;
			dbnew.EditableItem.RevisionNumber = dbnew.RevisionNumber;
			dbnew.EditableItem.BuilderAccountId = initiator.Account.Id;
			dbnew.EditableItem.RevisionStatus = (int)RevisionStatus.UnderDesign;
			FMDB.Context.NpcTemplates.Add(dbnew);
			FMDB.Context.SaveChanges();

			return new VariableNPCTemplate(dbnew, Gameworld);
		}
	}

	public override string EditHeader()
	{
		return $"Variable NPC Template #{Id}r{RevisionNumber} \"{Name}\"";
	}

	public override void Save()
	{
		using (new FMDB())
		{
			var dbItem = FMDB.Context.NpcTemplates.Find(Id, RevisionNumber);
			dbItem.Definition = SaveDefinition();
			dbItem.Name = _name;
			FMDB.Context.NpctemplatesArtificalIntelligences.RemoveRange(dbItem.NpctemplatesArtificalIntelligences);
			foreach (var item in ArtificialIntelligences)
			{
				dbItem.NpctemplatesArtificalIntelligences.Add(new NpcTemplatesArtificalIntelligences
					{ Npctemplate = dbItem, AiId = item.Id });
			}

			if (_statusChanged)
			{
				Save(dbItem.EditableItem);
			}

			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}

	#region Building Commands

	private bool BuildingCommandGender(ICharacter actor, StringStack command)
	{
		if (!command.PopSpeech().TryParseEnum<Gender>(out var gender))
		{
			actor.OutputHandler.Send("That is not a valid gender.");
			return false;
		}

		if (!int.TryParse(command.Pop(), out var value))
		{
			actor.OutputHandler.Send("You must enter a number weighting for this gender.");
			return false;
		}

		_genderChances.RemoveAll(x => x.Value == gender);
		Changed = true;
		if (value == 0)
		{
			actor.OutputHandler.Send(
				$"You remove the {Gendering.Get(gender).Name.Proper()} gender as an option for this NPC.");
			return true;
		}

		_genderChances.Add((gender, value));
		actor.OutputHandler.Send(
			$"You set the {Gendering.Get(gender).Name.Proper()} gender to have {value} relative chances to be selected for this NPC.");
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name did you want to give this Variable NPC Template?");
			return false;
		}

		_name = command.SafeRemainingArgument;
		actor.OutputHandler.Send(
			$"You change the name of this Variable NPC Template to {_name.Colour(Telnet.Cyan)}.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandNameProfile(ICharacter actor, StringStack command)
	{
		if (_culture == null)
		{
			actor.OutputHandler.Send("You must first select a culture for this NPC.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which Random Name Profile do you wish to set for this NPC? See {"randomname list".FluentTagMXP("send", "href='randomname list'")} for a list of possible values.");
			return false;
		}

		var profileText = command.PopSpeech();
		var profile = long.TryParse(profileText, out var value)
			? _culture.NameCultures.SelectMany(x => x.RandomNameProfiles).Distinct().FirstOrDefault(x => x.Id == value)
			: _culture.NameCultures.SelectMany(x => x.RandomNameProfiles).Distinct().FirstOrDefault(
				x => x.Name.StartsWith(command.Last, StringComparison.InvariantCultureIgnoreCase));
		if (profile == null)
		{
			actor.OutputHandler.Send("There is no such Random Name Profile.");
			return false;
		}

		var gender = profile.Gender;
		if (!command.IsFinished)
		{
			if (!command.SafeRemainingArgument.TryParseEnum<Gender>(out gender))
			{
				actor.OutputHandler.Send(
					$"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid gender.");
				return false;
			}
		}

		if (_culture.NameCultureForGender(gender) != profile.Culture)
		{
			actor.OutputHandler.Send(
				$"The Name Profile {profile.Name.ColourName()} is designed for the Name Culture {profile.Culture.Name.ColourName()}, whereas it needs to match {_culture.NameCultureForGender(gender).Name.ColourName()}.");
			return false;
		}

		if (_genderChances.All(x => !profile.IsCompatibleGender(x.Value)))
		{
			actor.OutputHandler.Send(
				$"You must first add a chance for the {Gendering.Get(profile.Gender).Name.Proper()} gender before you can use a Random Name Profile of this gender.");
			return false;
		}

		_nameProfiles[gender] = profile;
		actor.OutputHandler.Send(
			$"You set this NPC to use the {profile.Name.Proper().Colour(Telnet.Cyan)} profile for the {Gendering.Get(gender).Name.Proper()} gender.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandRace(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which race do you wish to set for this NPC?");
			return false;
		}

		var race = actor.Gameworld.Races.GetByIdOrName(command.SafeRemainingArgument);
		if (race == null)
		{
			actor.OutputHandler.Send("There is no such race.");
			return false;
		}

		if (race == _race)
		{
			actor.OutputHandler.Send($"This NPC is already of the {_race.Name.Proper().Colour(Telnet.Cyan)} race.");
			return false;
		}

		_race = race;
		_ethnicity = null;
		_genderChances.Clear();
		foreach (var gender in _race.AllowedGenders)
		{
			switch (gender)
			{
				case Form.Shape.Gender.Neuter:
				case Form.Shape.Gender.Male:
				case Form.Shape.Gender.Female:
					_genderChances.Add((gender, 100));
					break;
				case Form.Shape.Gender.NonBinary:
					_genderChances.Add((gender, 2));
					break;
			}
		}

		_minimumAge = _race.MinimumAgeForCategory(AgeCategory.YoungAdult);
		_maximumAge = _race.MinimumAgeForCategory(AgeCategory.Venerable);
		_heightWeightModels.Clear();
		foreach (var gender in _genderChances.Select(x => x.Value))
		{
			var hwModel = race.DefaultHeightWeightModel(gender);
			if (hwModel is not null)
			{
				_heightWeightModels[gender] = hwModel;
			}
		}

		foreach (var gender in _nameProfiles.Keys.Where(x => _genderChances.All(y => y.Value != x)).ToList())
		{
			_nameProfiles.Remove(gender);
		}

		_priorityAttributeDefinitions.RemoveAll(x => !_race.Attributes.Contains(x));
		Changed = true;
		actor.Send("You set the race of this NPC to {0}.", _race.Name.Proper().Colour(Telnet.Cyan));
		return true;
	}

	private bool BuildingCommandCulture(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which culture do you wish to set for this NPC?");
			return false;
		}

		var culture = Gameworld.Cultures.GetByIdOrName(command.SafeRemainingArgument);
		if (culture == null)
		{
			actor.OutputHandler.Send("There is no such culture.");
			return false;
		}

		if (_culture == culture)
		{
			actor.OutputHandler.Send(
				$"This NPC is already of the {_culture.Name.Proper().Colour(Telnet.Cyan)} culture.");
			return false;
		}

		_culture = culture;
		_nameProfiles.Clear();
		foreach (var gender in _genderChances)
		{
			var profile =
				_culture.NameCultureForGender(gender.Value).RandomNameProfiles
				        .Where(x => x.IsReady && x.IsCompatibleGender(gender.Value)).GetRandomElement();

			if (profile != null)
			{
				_nameProfiles[gender.Value] = profile;
			}
		}

		Changed = true;
		actor.OutputHandler.Send($"You set the culture of this NPC to {_culture.Name.Proper().Colour(Telnet.Cyan)}.");
		return true;
	}

	private bool BuildingCommandEthnicity(ICharacter actor, StringStack command)
	{
		if (_race == null)
		{
			actor.OutputHandler.Send("You must first select a race for this NPC.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which ethnicity do you wish to set for this NPC?");
			return false;
		}

		var ethnicity = Gameworld.Ethnicities.GetByIdOrName(command.SafeRemainingArgument);
		if (ethnicity == null)
		{
			actor.OutputHandler.Send("There is no such ethnicity.");
			return false;
		}

		if (_ethnicity == ethnicity)
		{
			actor.OutputHandler.Send(
				$"This NPC is already of the {_ethnicity.Name.Proper().Colour(Telnet.Cyan)} ethnicity.");
			return false;
		}

		if (!_race.SameRace(ethnicity.ParentRace))
		{
			actor.OutputHandler.Send($"The {ethnicity.Name.ColourName()} ethnicity is not valid for the {_race.Name.ColourName()} race.");
			return false;
		}

		_ethnicity = ethnicity;
		Changed = true;
		actor.OutputHandler.Send(
			$"You set the ethnicity of this NPC to {_ethnicity.Name.Proper().Colour(Telnet.Cyan)}.");
		return true;
	}

	private bool BuildingCommandHeightWeightModel(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which gender do you wish to set a height-weight model for?");
			return false;
		}

		if (!command.PopSpeech().TryParseEnum<Gender>(out var gender))
		{
			actor.OutputHandler.Send("That is not a valid gender.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which Height-Weight Model do you wish to set for this NPC? See SHOW HWMODELS.");
			return false;
		}

		var hwmodel = Gameworld.HeightWeightModels.GetByIdOrName(command.SafeRemainingArgument);
		if (hwmodel == null)
		{
			actor.OutputHandler.Send("There is no such Height-Weight Model. See SHOW HWMODELS.");
			return false;
		}

		_heightWeightModels[gender] = hwmodel;
		Changed = true;
		actor.OutputHandler.Send(
			$"This NPC will now use the {hwmodel.Name.ColourName()} Height-Weight Model for the {Gendering.Get(gender).Name.ColourValue()} gender.");
		return true;
	}

	private bool BuildingCommandAge(ICharacter actor, StringStack command)
	{
		if (!int.TryParse(command.Pop(), out var minAge))
		{
			actor.OutputHandler.Send("You must enter a minimum age for this NPC.");
			return false;
		}

		if (!int.TryParse(command.Pop(), out var maxAge))
		{
			actor.OutputHandler.Send("You must enter a maximum age for this NPC.");
			return false;
		}

		if (minAge > maxAge)
		{
			var oldMin = minAge;
			minAge = maxAge;
			maxAge = oldMin;
		}

		_minimumAge = minAge;
		_maximumAge = maxAge;
		Changed = true;
		actor.OutputHandler.Send($"This NPC will now be between {_minimumAge} and {_maximumAge} years old.");
		return true;
	}

	private bool BuildingCommandAgeRange(ICharacter actor, StringStack command)
	{
		if (_race == null)
		{
			actor.OutputHandler.Send("You must first give this NPC a race.");
			return false;
		}

		if (!command.PopSpeech().TryParseEnum<AgeCategory>(out var minAgeCat))
		{
			actor.OutputHandler.Send(
				$"You must enter a valid minimum age category from the following list of values: {Enum.GetValues<AgeCategory>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<AgeCategory>(out var maxAgeCat))
		{
			actor.OutputHandler.Send(
				$"You must enter a valid maximum age category from the following list of values: {Enum.GetValues<AgeCategory>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}");
			return false;
		}

		var minAge = _race.MinimumAgeForCategory(minAgeCat);
		var maxAge = maxAgeCat == AgeCategory.Venerable
			? (int)(_race.MinimumAgeForCategory(maxAgeCat) * 1.2)
			: _race.MinimumAgeForCategory(maxAgeCat + 1);

		if (minAge > maxAge)
		{
			(minAge, maxAge) = (maxAge, minAge);
		}

		_minimumAge = minAge;
		_maximumAge = maxAge;
		Changed = true;
		actor.OutputHandler.Send($"This NPC will now be between {_minimumAge} and {_maximumAge} years old.");
		return true;
	}

	private bool BuildingCommandAttributeTotal(ICharacter actor, StringStack command)
	{
		if (!int.TryParse(command.Pop(), out var total))
		{
			actor.OutputHandler.Send(
				"You must either enter a number to be the attribute total for this NPC, or 0 to clear it and use the race's default.");
			return false;
		}

		if (total == 0)
		{
			_attributeTotal = null;
			actor.OutputHandler.Send("This NPC will now use its race's default value for attribute total.");
		}
		else
		{
			_attributeTotal = total;
			actor.Send("This NPC will now have attributes totalling {0}.", total);
		}

		Changed = true;
		return true;
	}

	private bool BuildingCommandAttribute(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which attributes did you wish to make priority attributes for this NPC?");
			return false;
		}

		if (_race == null)
		{
			actor.OutputHandler.Send("You must first select a race for this NPC.");
			return false;
		}

		var attributes =
			command.SafeRemainingArgument.Split(' ')
			       .Select(
				       x =>
					       _race.Attributes.FirstOrDefault(
						       y => y.Alias.Equals(x, StringComparison.InvariantCultureIgnoreCase))).ToList();

		if (attributes.Any(x => x == null))
		{
			actor.OutputHandler.Send(
				"You may only enter valid attributes. Enter the aliases of the attributes separated by spaces.");
			return false;
		}

		_priorityAttributeDefinitions.Clear();
		_priorityAttributeDefinitions.AddRange(attributes);
		actor.OutputHandler.Send(
			$"This NPC will now prioritise, in order, {_priorityAttributeDefinitions.Select(x => x.Name.Proper()).ListToString()}; thereafter, the order is random.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandSdesc(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which short description pattern did you wish to give this NPC?");
			return false;
		}

		if (command.Peek().Equals("clear", StringComparison.InvariantCultureIgnoreCase))
		{
			_sdescPattern = null;
			Changed = true;
			actor.OutputHandler.Send(
				"You clear the short description pattern for this NPC. It will now use a random one.");
			return true;
		}

		if (!long.TryParse(command.Pop(), out var value))
		{
			actor.OutputHandler.Send("You must enter the ID of a description pattern you'd like to use.");
			return false;
		}

		var pattern = Gameworld.EntityDescriptionPatterns.Get(value);
		if (pattern == null)
		{
			actor.OutputHandler.Send("There is no such description pattern.");
			return false;
		}

		if (pattern.Type != EntityDescriptionType.ShortDescription)
		{
			actor.OutputHandler.Send("That description pattern is not for short descriptions.");
			return false;
		}

		_sdescPattern = pattern;
		actor.OutputHandler.Send("This NPC will now use the specified short description pattern.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandFdesc(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which full description pattern did you wish to give this NPC?");
			return false;
		}

		if (command.Peek().Equals("clear", StringComparison.InvariantCultureIgnoreCase))
		{
			_fdescPattern = null;
			Changed = true;
			actor.OutputHandler.Send(
				"You clear the full description pattern for this NPC. It will now use a random one.");
			return true;
		}

		if (!long.TryParse(command.Pop(), out var value))
		{
			actor.OutputHandler.Send("You must enter the ID of a description pattern you'd like to use.");
			return false;
		}

		var pattern = Gameworld.EntityDescriptionPatterns.Get(value);
		if (pattern == null)
		{
			actor.OutputHandler.Send("There is no such description pattern.");
			return false;
		}

		if (pattern.Type != EntityDescriptionType.FullDescription)
		{
			actor.OutputHandler.Send("That description pattern is not for full descriptions.");
			return false;
		}

		_fdescPattern = pattern;
		actor.OutputHandler.Send("This NPC will now use the specified full description pattern.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandSkill(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"You must enter the name of the skill, the percentage chance for it to be selected, the mean value and a standard deviation.");
			return false;
		}

		var skillText = command.PopSpeech();
		var skill =
			Gameworld.Traits.Where(x => x.TraitType == TraitType.Skill)
			         .FirstOrDefault(x => x.Name.Equals(skillText, StringComparison.InvariantCultureIgnoreCase));
		if (skill == null)
		{
			actor.OutputHandler.Send("There is no such skill.");
			return false;
		}

		if (!command.PopSpeech().TryParsePercentage(out var chance))
		{
			actor.OutputHandler.Send("You must specify a valid percentage.");
			return false;
		}

		if (chance <= 0 && _skillTemplates.Any(x => x.Trait == skill))
		{
			_skillTemplates.RemoveAll(x => x.Trait == skill);
			Changed = true;
			actor.Send("You remove the {0} skill from this NPC.", skill.Name);
			return true;
		}

		if (!double.TryParse(command.Pop(), out var mean))
		{
			actor.OutputHandler.Send("You must enter a mean (average) value for this skill to be given to this NPC.");
			return false;
		}

		if (!double.TryParse(command.Pop(), out var stddev))
		{
			actor.OutputHandler.Send(
				$"You must enter a standard deviation value for this skill. Based on your average, a suggested value for standard deviation is {(mean * 0.05).ToString("N2", actor).ColourValue()}.");
			return false;
		}

		_skillTemplates.RemoveAll(x => x.Trait == skill);
		_skillTemplates.Add(new VariableSkillTemplate
		{
			Trait = skill,
			Chance = chance,
			SkillMean = mean,
			SkillStddev = stddev
		});

		actor.OutputHandler.Send(
			string.Format(actor,
				"You set the {0} skill to have a {1:P2} chance to be given to this NPC, with a mean (average) value of {2:N2} and a standard deviation of {3:N2}.\n99.5% of the time, this will be between {4:N2} and {5:N2}.",
				skill.Name.Proper().Colour(Telnet.Cyan), chance, mean, stddev, mean - 3 * stddev, mean + 3 * stddev
			));
		Changed = true;
		return true;
	}

	private bool BuildingCommandClass(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which class do you want this NPC to have?");
			return false;
		}

		var selectedClass = long.TryParse(command.PopSpeech(), out var value)
			? actor.Gameworld.Roles.Where(x => x.RoleType == ChargenRoleType.Class)
			       .FirstOrDefault(x => x.Id == value)
			: actor.Gameworld.Roles.Where(x => x.RoleType == ChargenRoleType.Class)
			       .FirstOrDefault(x => x.Name.Equals(command.Last, StringComparison.InvariantCultureIgnoreCase));
		if (selectedClass == null)
		{
			actor.Send("There is no such class.");
			return false;
		}

		_roles.RemoveAll(x => x.RoleType == ChargenRoleType.Class);
		_roles.Add(selectedClass);
		actor.Send("This NPC is now of class {0}.", selectedClass.Name.TitleCase().Colour(Telnet.Green));
		Changed = true;
		return true;
	}

	private bool BuildingCommandSubclass(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which subclass do you want this NPC to have?");
			return false;
		}

		var selectedClass = long.TryParse(command.PopSpeech(), out var value)
			? actor.Gameworld.Roles.Where(x => x.RoleType == ChargenRoleType.Subclass)
			       .FirstOrDefault(x => x.Id == value)
			: actor.Gameworld.Roles.Where(x => x.RoleType == ChargenRoleType.Subclass)
			       .FirstOrDefault(x => x.Name.Equals(command.Last, StringComparison.InvariantCultureIgnoreCase));
		if (selectedClass == null)
		{
			actor.Send("There is no such subclass.");
			return false;
		}

		_roles.RemoveAll(x => x.RoleType == ChargenRoleType.Subclass);
		_roles.Add(selectedClass);
		actor.Send("This NPC is now of subclass {0}.", selectedClass.Name.TitleCase().Colour(Telnet.Green));
		Changed = true;
		return true;
	}

	private bool BuildingCommandRole(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which role do you want to toggle for this NPC?");
			return false;
		}

		var selectedRole = long.TryParse(command.PopSpeech(), out var value)
			? actor.Gameworld.Roles.Where(
				       x => x.RoleType != ChargenRoleType.Class && x.RoleType != ChargenRoleType.Subclass)
			       .FirstOrDefault(x => x.Id == value)
			: actor.Gameworld.Roles.Where(
				       x => x.RoleType != ChargenRoleType.Class && x.RoleType != ChargenRoleType.Subclass)
			       .FirstOrDefault(x => x.Name.Equals(command.Last, StringComparison.InvariantCultureIgnoreCase));
		if (selectedRole == null)
		{
			actor.Send("There is no such role.");
			return false;
		}

		if (_roles.Contains(selectedRole))
		{
			_roles.Remove(selectedRole);
			actor.Send("You remove the role {0} from this NPC.", selectedRole.Name.TitleCase().Colour(Telnet.Green));
		}
		else
		{
			_roles.Add(selectedRole);
			actor.Send("You add the role {0} to this NPC.", selectedRole.Name.TitleCase().Colour(Telnet.Green));
		}

		Changed = true;
		return true;
	}

	public override string HelpText => @"You can use the following options with this command:

	#3name <name>#0 - renames the template
	#3race <race>#0 - sets the race of the NPC
	#3culture <culture>#0 - sets the culture of the NPC
	#3ethnicity <ethnicity>#0 - sets the ethnicity of the NPC
	#3gender <gender> <weight>|0#0 - sets or removes the chance of selecting a particular gender
	#3randomname <which> [<gender>]#0 - sets a random name profile for this NPC
	#3hwmodel <gender> <which>#0 - sets a height/weight model for a gender for this NPC
	#3age <min> <max>#0 - sets the minimum and maximum age
	#3agerange <minCategory> <maxCategory>#0 - sets the minimum and maximum age relative to the race's age categories
	#3attrtotal <#>#0 - sets the total number of points to distribute amongst attributes
	#3attribute <attributes separated by spaces>#0 - sets priority attributes, in order, after roll
	#3sdesc <#>#0 - sets the NPC to use a particular sdesc pattern
	#3sdesc clear#0 - sets the NPC to use a random valid sdesc pattern
	#3fdesc <#>#0 - sets the NPC to use a particular fdesc pattern
	#3fdesc clear#0 - sets the NPC ot use a random valid fdesc pattern
	#3skill <which> <%> <avg> <stddev>#0 - sets the chance/mean/stddev for a particular skill
	#3skill <which> 0%#0 - removes a skill from the list
	#3class <class>#0 - sets the class of this NPC (if using classes)
	#3subclass <subclass>#0 - sets the subclass of  this NPC (if using subclasses)
	#3role <which>#0 - toggles this NPC having a particular role
	#3knowledge <which> <% chance>#0 - sets the percentage chance of having a knowledge
	#3merit <which>#0 - toggles a merit being valid
	#3merit all#0 - adds all valid merits
	#3merit none#0 - removes all valid merits
	#3numquirks <##>#0 - sets the number of quirks (either merit or flaw) to give
	#3nummerits <##>#0 - sets the number of merits to give
	#3numflaws <##>#0 - sets the number of flaws to give
	#3onload <prog>#0 - sets a prog to be run on-load of the NPC
	#3onload none#0 - clears an onload prog
	#3ai add <which>#0 - adds an AI routine to this NPC
	#3ai remove <which>#0 - removes an AI routine from this NPC
	#3healthstrategy <which>#0 - sets an overriding health strategy
	#3healthstrategy none#0 - resets the health strategy to racial default";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "gender":
				return BuildingCommandGender(actor, command);
			case "knowledge":
				return BuildingCommandKnowledge(actor, command);
			case "merit":
				return BuildingCommandMerit(actor, command);
			case "numquirks":
				return BuildingCommandNumQuirks(actor, command);
			case "nummerits":
				return BuildingCommandNumMerits(actor, command);
			case "numflaws":
				return BuildingCommandNumFlaws(actor, command);
			case "name":
				return BuildingCommandName(actor, command);
			case "nameprofile":
			case "randomname":
			case "np":
			case "rnp":
			case "rn":
				return BuildingCommandNameProfile(actor, command);
			case "race":
				return BuildingCommandRace(actor, command);
			case "culture":
				return BuildingCommandCulture(actor, command);
			case "ethnicity":
				return BuildingCommandEthnicity(actor, command);
			case "heightweight":
			case "hwmodel":
			case "hw":
			case "heightweightmodel":
				return BuildingCommandHeightWeightModel(actor, command);
			case "age":
				return BuildingCommandAge(actor, command);
			case "agecategory":
			case "agecat":
			case "agerange":
			case "range":
				return BuildingCommandAgeRange(actor, command);
			case "attributetotal":
			case "attrtotal":
				return BuildingCommandAttributeTotal(actor, command);
			case "attribute":
				return BuildingCommandAttribute(actor, command);
			case "sdesc":
				return BuildingCommandSdesc(actor, command);
			case "fdesc":
				return BuildingCommandFdesc(actor, command);
			case "skill":
			case "skills":
				return BuildingCommandSkill(actor, command);
			case "class":
				return BuildingCommandClass(actor, command);
			case "subclass":
				return BuildingCommandSubclass(actor, command);
			case "role":
				return BuildingCommandRole(actor, command);
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandNumFlaws(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How many flaws should this Variable NPC roll?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value < 0)
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid number zero or greater.");
			return false;
		}

		_numberFlaws = value;
		Changed = true;
		actor.OutputHandler.Send($"This variable NPC will now have {value.ToStringN0Colour(actor)} {"flaw".Pluralise(value != 1)} rolled.");
		return true;
	}

	private bool BuildingCommandNumMerits(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How many merits should this Variable NPC roll?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value < 0)
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid number zero or greater.");
			return false;
		}

		_numberMerits = value;
		Changed = true;
		actor.OutputHandler.Send($"This variable NPC will now have {value.ToStringN0Colour(actor)} {"merit".Pluralise(value != 1)} rolled.");
		return true;
	}

	private bool BuildingCommandNumQuirks(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How many quirks should this Variable NPC roll?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value < 0)
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid number zero or greater.");
			return false;
		}

		_numberQuirks = value;
		Changed = true;
		actor.OutputHandler.Send($"This variable NPC will now have {value.ToStringN0Colour(actor)} {"quirk".Pluralise(value != 1)} rolled.");
		return true;
	}

	private bool BuildingCommandMerit(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which merit do you want to toggle as being a part of the valid mixture for this NPC?");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("all"))
		{
			_validMerits.Clear();
			_validMerits.AddRange(Gameworld.Merits.OfType<ICharacterMerit>());
			Changed = true;
			actor.OutputHandler.Send($"This NPC could now get any merit in the game.");
			return true;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			_validMerits.Clear();
			Changed = true;
			actor.OutputHandler.Send($"This NPC no longer has any valid merits.");
			return true;
		}

		var merit = Gameworld.Merits.OfType<ICharacterMerit>().GetByIdOrName(command.SafeRemainingArgument);
		if (merit is null)
		{
			actor.OutputHandler.Send($"There is no character merit identified by the text {command.SafeRemainingArgument.ColourCommand()}.");
			return false;
		}

		Changed = true;
		if (_validMerits.Remove(merit))
		{
			actor.OutputHandler.Send($"The merit {merit.Name.ColourValue()} is no longer a valid choice for this NPC.");
			return true;
		}

		_validMerits.Add(merit);
		actor.OutputHandler.Send($"The merit {merit.Name.ColourValue()} is now a valid choice for this NPC.");
		return true;
	}

	private bool BuildingCommandKnowledge(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which knowledge would you like to edit for this NPC Template?");
			return false;
		}

		var knowledge = Gameworld.Knowledges.GetByIdOrName(command.PopSpeech());
		if (knowledge is null)
		{
			actor.OutputHandler.Send($"There is no knowledge identified by the text {command.Last.ColourCommand()}.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the percentage chance of this NPC having that knowledge?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid percentage.");
			return false;
		}

		_knowledgeChances.RemoveAll(x => x.Knowledge == knowledge);
		Changed = true;
		if (value <= 0.0)
		{
			actor.OutputHandler.Send($"This NPC no longer has any chance to have the {knowledge.Name.ColourValue()} knowledge.");
			return true;
		}

		_knowledgeChances.Add((knowledge,value));
		actor.OutputHandler.Send($"This NPC has a {value.ToStringP2Colour(actor)} chance to have the {knowledge.Name.ColourValue()} knowledge.");
		return true;
	}

	#endregion
}