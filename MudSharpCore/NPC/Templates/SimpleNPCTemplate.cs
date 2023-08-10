using System;
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
using MudSharp.CharacterCreation.Screens;
using MudSharp.Communication.Language;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Units;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Knowledge;
using MudSharp.RPG.Merits;
using MudSharp.TimeAndDate.Date;
using Accent = MudSharp.Communication.Language.Accent;
using Attribute = MudSharp.Body.Traits.Subtypes.Attribute;
using CharacteristicValue = MudSharp.Form.Characteristics.CharacteristicValue;
using PersonalName = MudSharp.Character.Name.PersonalName;
using Race = MudSharp.Character.Heritage.Race;

namespace MudSharp.NPC.Templates;

public class SimpleNPCTemplate : NPCTemplateBase
{
	public SimpleNPCTemplate(NpcTemplate template, IFuturemud gameworld)
		: base(template, gameworld)
	{
		Initialise();
		LoadFromXml(XElement.Parse(template.Definition));
	}

	public SimpleNPCTemplate(IFuturemud gameworld, IAccount originator) : base(gameworld, originator, "Simple")
	{
		Initialise();
		using (new FMDB())
		{
			var dbitem = FMDB.Context.NpcTemplates.Find(Id, RevisionNumber);
			dbitem.Definition = SaveDefinition();
			FMDB.Context.SaveChanges();
		}
	}

	public SimpleNPCTemplate(IFuturemud gameworld, IAccount originator, ICharacterTemplate template, string name) :
		base(
			gameworld, originator, "Simple")
	{
		Initialise();
		_name = string.IsNullOrEmpty(name) ? template.SelectedName.GetName(NameStyle.FullName) : name;
		SelectedGender = template.SelectedGender;
		SelectedAccents = template.SelectedAccents.ToList();
		SelectedBirthday = template.SelectedBirthday;
		SelectedAttributes = template.SelectedAttributes.ToList();
		SelectedCharacteristics = template.SelectedCharacteristics.ToList();
		SelectedCulture = template.SelectedCulture;
		SelectedEntityDescriptionPatterns = template.SelectedEntityDescriptionPatterns;
		SelectedEthnicity = template.SelectedEthnicity;
		SelectedFullDesc = template.SelectedFullDesc;
		SelectedSdesc = template.SelectedSdesc;
		SelectedHeight = template.SelectedHeight;
		SelectedWeight = template.SelectedWeight;
		SelectedName = template.SelectedName;
		SelectedMerits = template.SelectedMerits.ToList();
		SelectedRace = template.SelectedRace;
		SelectedRoles = template.SelectedRoles.ToList();
		SkillValues = template.SkillValues.ToList();
		Handedness = template.Handedness;
		var date = DateTime.UtcNow;
		using (new FMDB())
		{
			var dbitem = FMDB.Context.NpcTemplates.Find(Id, RevisionNumber);
			dbitem.EditableItem.RevisionStatus = (int)RevisionStatus.Current;
			dbitem.EditableItem.ReviewerAccountId = originator.Id;
			dbitem.EditableItem.ReviewerDate = date;
			dbitem.Definition = SaveDefinition();
			FMDB.Context.SaveChanges();
		}

		ReviewerAccountID = originator.Id;
		ReviewerDate = date;
		Status = RevisionStatus.Current;
	}

	public override INPCTemplate Clone(ICharacter builder)
	{
		return new SimpleNPCTemplate(Gameworld, builder.Account, GetCharacterTemplate(null), Name);
	}

	public override string FrameworkItemType => "SimpleNPCTemplate";

	public override string NPCTemplateType => "Simple";

	public List<Tuple<ITraitDefinition, double>> SkillValues { get; set; }

	private void Initialise()
	{
		SelectedCharacteristics = new List<Tuple<ICharacteristicDefinition, ICharacteristicValue>>();
		SelectedEntityDescriptionPatterns = new List<IEntityDescriptionPattern>();
		SelectedAccents = new List<IAccent>();
		SelectedAttributes = new List<ITrait>();
		SelectedRoles = new List<IChargenRole>();
		SelectedSdesc = "an unnamed NPC";
		SelectedFullDesc = "A newly created, undescribed NPC.";
		SkillValues = new List<Tuple<ITraitDefinition, double>>();
	}

	protected override ICharacterTemplate CharacterTemplate(ICell location)
	{
		return new SimpleCharacterTemplate
		{
			SelectedName = SelectedName,
			SelectedAccents = SelectedAccents,
			SelectedAttributes = SelectedAttributes,
			SelectedBirthday = SelectedBirthday,
			SelectedCharacteristics = SelectedCharacteristics,
			SelectedCulture = SelectedCulture,
			SelectedStartingLocation = location,
			SelectedEntityDescriptionPatterns = SelectedEntityDescriptionPatterns,
			SelectedEthnicity = SelectedEthnicity,
			SelectedFullDesc = SelectedFullDesc,
			SelectedGender = SelectedGender,
			SelectedHeight = SelectedHeight,
			SelectedRace = SelectedRace,
			SelectedSdesc = SelectedSdesc,
			SkillValues = SkillValues,
			SelectedWeight = SelectedWeight,
			SelectedRoles = SelectedRoles,
			SelectedMerits = SelectedMerits,
			SelectedKnowledges = SelectedKnowledges,
			MissingBodyparts = MissingBodyparts,
			SelectedDisfigurements = new List<(IDisfigurementTemplate Disfigurement, IBodypart Bodypart)>(),
			SelectedProstheses = new List<IGameItemProto>(),
			Gameworld = Gameworld
		};
	}

	public override string HelpText => @"You can use the following options with this command:

	#3race <race>#0 - sets the race of this NPC
	#3culture <culture>#0 - sets the culture of this NPC
	#3ethnicity <ethnicity>#0 - sets the ethnicity of this NPC
	#3gender <gender>#0 - sets the gender of this NPC
	#3birthday <date>#0 - sets this NPC's birthday
	#3birthday random <age>#0 - sets this NPC's age and assigns a random birthday
	#3name <full name>#0 - sets the full name of this NPC
	#3height <height>#0 - sets the height of this NPC
	#3weight <weight>#0 - sets the weight of this NPC
	#3hand <handedness>#0 - sets the handedness of this NPC
	#3sdesc <sdesc>#0 - sets the sdesc of this NPC
	#3desc#0 - drops you into an editor for the NPC's description
	#3pattern sdesc <#>#0 - sets the sdesc based on a particular sdesc pattern
	#3pattern sdesc random#0 - sets the sdesc based on a random sdesc pattern
	#3pattern desc <#>#0 - sets the desc based on a particular full description pattern
	#3pattern desc random#0 - sets the desc based on a random full description pattern
	#3variable <which> <value>#0 - sets a characteristic to a particular value
	#3attribute <which> <#>#0 - sets an attribute to a particular amount
	#3skill <which> <#>#0 - gives this NPC a skill at a particular value. Use 0 to remove
	#3accent <accent>#0 - toggles an NPC having a particular accent. Must have matching language skill.
	#3class <class>#0 - sets the class of this NPC (if using classes)
	#3subclass <subclass>#0 - sets the subclass of  this NPC (if using subclasses)
	#3role <which>#0 - toggles this NPC having a particular role
	#3merit <which>#0 - toggles this NPC having a particular merit
	#3knowledge <which>#0 - toggles this NPC having a particular knowledge
	#3randomise#0 - randomises the descriptions, name and characteristics of this NPC";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.Pop().ToLowerInvariant())
		{
			case "accent":
				return BuildingCommandAccent(actor, command);
			case "attribute":
				return BuildingCommandAttribute(actor, command);
			case "birthday":
				return BuildingCommandBirthday(actor, command);
			case "characteristic":
			case "char":
			case "variable":
			case "var":
				return BuildingCommandCharacteristics(actor, command);
			case "culture":
				return BuildingCommandCulture(actor, command);
			case "ethnicity":
				return BuildingCommandEthnicity(actor, command);
			case "sdesc":
				return BuildingCommandSDesc(actor, command);
			case "pattern":
				return BuildingCommandPattern(actor, command);
			case "desc":
				return BuildingCommandDesc(actor, command);
			case "gender":
				return BuildingCommandGender(actor, command);
			case "height":
				return BuildingCommandHeight(actor, command);
			case "name":
				return BuildingCommandName(actor, command);
			case "race":
				return BuildingCommandRace(actor, command);
			case "skill":
				return BuildingCommandSkill(actor, command);
			case "weight":
				return BuildingCommandWeight(actor, command);
			case "class":
				return BuildingCommandClass(actor, command);
			case "subclass":
				return BuildingCommandSubclass(actor, command);
			case "role":
				return BuildingCommandRole(actor, command);
			case "merit":
			case "flaw":
				return BuildingCommandMerit(actor, command);
			case "knowledge":
			case "know":
				return BuildingCommandKnowledge(actor, command);
			case "hand":
			case "handed":
			case "handedness":
				return BuildingCommandHand(actor, command);
			case "randomise":
			case "randomize":
				return BuildingCommandRandomise(actor, command);
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	public override string Show(ICharacter actor)
	{
		using (new FMDB())
		{
			var sb = new StringBuilder();

			sb.AppendLine(("Simple NPC #" + Id + " - " + Name).Colour(Telnet.Cyan));
			sb.AppendLine();
			sb.Append(new[]
			{
				$"Created By: {FMDB.Context.Accounts.Find(BuilderAccountID).Name.Proper().Colour(Telnet.Green)}",
				$"Reviewed By: {(ReviewerAccountID.HasValue ? FMDB.Context.Accounts.Find(ReviewerAccountID.Value).Name.Proper().Colour(Telnet.Green) : "N/A")}",
				$"Status: {Status.Describe().Colour(Telnet.Green)}"
			}.ArrangeStringsOntoLines(3, (uint)actor.LineFormatLength));

			sb.Append(new[]
			{
				$"Name: {(SelectedName != null ? SelectedName.GetName(NameStyle.FullName) : "Unnamed").Colour(Telnet.Green)}",
				SelectedName != null
					? SelectedName.GetName(NameStyle.Affectionate) != SelectedName.GetName(NameStyle.GivenOnly)
						? $"Nickname: {SelectedName.GetName(NameStyle.Affectionate).Colour(Telnet.Green)}"
						: ""
					: "",
				$"Gender: {Gendering.Get(SelectedGender).Name.Proper().Colour(Telnet.Green)}"
			}.ArrangeStringsOntoLines(3, (uint)actor.LineFormatLength));

			sb.AppendLine(
				$"Short Description: {IHaveCharacteristicsExtensions.ParseCharacteristicsAbsolute(SelectedSdesc, SelectedCharacteristics, Gendering.Get(SelectedGender), Gameworld, SelectedRace, SelectedCulture, SelectedEthnicity, SelectedBirthday?.Calendar.CurrentDate.YearsDifference(SelectedBirthday ?? SelectedCulture?.PrimaryCalendar.CurrentDate) ?? 0, SelectedHeight).Colour(Telnet.Magenta)}");
			sb.AppendLine("Full Description:");
			sb.AppendLine();
			sb.AppendLine(
				IHaveCharacteristicsExtensions.ParseCharacteristicsAbsolute(SelectedFullDesc,
					                              SelectedCharacteristics, Gendering.Get(SelectedGender), Gameworld,
					                              SelectedRace,
					                              SelectedCulture,
					                              SelectedEthnicity,
					                              SelectedBirthday?.Calendar.CurrentDate.YearsDifference(
						                              SelectedBirthday ??
						                              SelectedCulture?.PrimaryCalendar
						                                             .CurrentDate) ?? 0, SelectedHeight)
				                              .ProperSentences().Wrap(actor.InnerLineFormatLength, "\t").NoWrap());
			sb.AppendLine();
			sb.Append(new[]
			{
				$"Race: {(SelectedRace != null ? SelectedRace.Name.Colour(Telnet.Green) : "None")}",
				$"Ethnicity: {(SelectedEthnicity != null ? SelectedEthnicity.Name.Colour(Telnet.Green) : "None")}",
				$"Culture: {(SelectedCulture != null ? SelectedCulture.Name.Colour(Telnet.Green) : "None")}"
			}.ArrangeStringsOntoLines(3, (uint)actor.LineFormatLength));

			if (SelectedRoles.Any(x => x.RoleType == ChargenRoleType.Class))
			{
				sb.Append(new[]
				{
					$"Class: {SelectedRoles.FirstOrDefault(x => x.RoleType == ChargenRoleType.Class).Name.Colour(Telnet.Green)}",
					$"Subclass: {(SelectedRoles.Any(x => x.RoleType == ChargenRoleType.Subclass) ? SelectedRoles.FirstOrDefault(x => x.RoleType == ChargenRoleType.Subclass).Name.Colour(Telnet.Green) : "None".Colour(Telnet.Red))}",
					""
				}.ArrangeStringsOntoLines(3, (uint)actor.LineFormatLength));
			}

			sb.Append(new[]
			{
				$"Height: {Gameworld.UnitManager.DescribeBrief(SelectedHeight, UnitType.Length, actor).Colour(Telnet.Green)}",
				$"Weight: {Gameworld.UnitManager.DescribeBrief(SelectedWeight, UnitType.Mass, actor).Colour(Telnet.Green)}",
				$"Hand: {$"{Handedness.Describe()}-handed".Colour(Telnet.Green)}"
			}.ArrangeStringsOntoLines(3, (uint)actor.LineFormatLength));
			if (SelectedBirthday == null)
			{
				sb.AppendLine($"Birthday: {"None".Colour(Telnet.Red)}");
			}
			else
			{
				sb.AppendLine(
					$"Birthday: {SelectedBirthday.Display(CalendarDisplayMode.Long).ColourValue()} ({SelectedBirthday.Calendar.CurrentDate.YearsDifference(SelectedBirthday).ToString("N0", actor).ColourValue()} years old)");
			}

			sb.AppendLine();
			sb.AppendLine("Attributes:");
			foreach (var item in SelectedAttributes.OrderBy(x => ((IAttributeDefinition)x.Definition).DisplayOrder))
			{
				sb.AppendLine(
					$"\t{item.Definition.Name.ColourName()}: {item.Value.ToString("N").ColourValue()} ({item.Definition.Decorator.Decorate(item.Value)})");
			}

			sb.AppendLine();
			sb.AppendLine("Skills:");
			foreach (var item in SkillValues)
			{
				sb.AppendLine(
					$"\t{item.Item1.Name.ColourName()}: {item.Item2.ToString("N").ColourValue()} ({item.Item1.Decorator.Decorate(item.Item2)})");
			}

			sb.AppendLine("Languages and Accents: " +
			              SelectedSkills.SelectNotNull(
				                            x => Gameworld.Languages.FirstOrDefault(y => y.LinkedTrait == x)).Select(
				                            x =>
					                            $"{x.Name.Proper().ColourName()} ({SelectedAccents.Where(y => y.Language == x).Select(y => y.Name.Proper().ColourValue()).ListToString()})")
			                            .ListToString());

			sb.AppendLine("Characteristics:");
			sb.Append(SelectedCharacteristics.Select(x =>
				                                 $"\t{x.Item1.Name.TitleCase().ColourName()}: {x.Item2.GetValue.Colour(Telnet.Green)}")
			                                 .ArrangeStringsOntoLines((uint)actor.Account.LineFormatLength / 60,
				                                 (uint)actor.Account.LineFormatLength));
			sb.AppendLine();
			if (
				SelectedRoles.Any(
					x => x.RoleType != ChargenRoleType.Class && x.RoleType != ChargenRoleType.Subclass))
			{
				sb.AppendLine("Roles:");
				sb.AppendLine(
					SelectedRoles.Where(
						             x => x.RoleType != ChargenRoleType.Class && x.RoleType != ChargenRoleType.Subclass)
					             .Select(x => string.Format(actor, "#{0:N0}: {1}", x.Id, x.Name.TitleCase()))
					             .ArrangeStringsOntoLines(1, (uint)actor.LineFormatLength));
				sb.AppendLine();
			}

			if (SelectedMerits.Any())
			{
				sb.AppendLine("Merits and Flaws:");
				foreach (var merit in SelectedMerits)
				{
					sb.AppendLine($"\t{merit.Name}");
				}

				sb.AppendLine();
			}

			if (SelectedKnowledges.Any())
			{
				sb.AppendLine("Selected Knowledges:");
				foreach (var knowledge in SelectedKnowledges)
				{
					sb.AppendLine($"\t{knowledge.Name}");
				}

				sb.AppendLine();
			}

			if (MissingBodyparts.Any())
			{
				sb.AppendLine("Missing Bodyparts:");
				foreach (var part in MissingBodyparts)
				{
					sb.AppendLine($"\t{part.FullDescription()}");
				}
			}

			sb.AppendLine();
			sb.AppendLine($"OnLoadProg: {OnLoadProg?.MXPClickableFunctionName() ?? "None".Colour(Telnet.Red)}");

			sb.AppendLine("AI Routines:");
			sb.AppendLine();
			if (ArtificialIntelligences.Count == 0)
			{
				sb.AppendLine("\tNone");
			}
			else
			{
				foreach (var ai in ArtificialIntelligences)
				{
					sb.AppendLine($"\tAI #{ai.Id}: {ai.Name}");
				}
			}


			return sb.ToString();
		}
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
			dbnew.Type = "Simple";
			FMDB.Context.NpcTemplates.Add(dbnew);
			FMDB.Context.SaveChanges();

			return new SimpleNPCTemplate(dbnew, Gameworld);
		}
	}

	public override string EditHeader()
	{
		return $"Simple NPC Template #{Id}r{RevisionNumber} \"{Name}\"";
	}

	public override void Save()
	{
		using (new FMDB())
		{
			var dbItem = FMDB.Context.NpcTemplates.Find(Id, RevisionNumber);
			dbItem.Name = SelectedName != null
				? SelectedName.GetName(NameStyle.FullWithNickname)
				: "Unnamed NPC Template #" + Id;
			dbItem.Definition = SaveDefinition();
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

	private void LoadFromXml(XElement root)
	{
		using (new FMDB())
		{
			foreach (var item in root.Element("SelectedEntityDescriptionPatterns").Elements("Pattern"))
			{
				SelectedEntityDescriptionPatterns.Add(Gameworld.EntityDescriptionPatterns.Get(long.Parse(item.Value)));
			}

			XElement element;
			if (SelectedEntityDescriptionPatterns.All(x => x.Type != EntityDescriptionType.ShortDescription))
			{
				element = root.Element("SelectedSdesc");
				if (element != null)
				{
					SelectedSdesc = element.Value;
				}
			}
			else
			{
				SelectedSdesc =
					SelectedEntityDescriptionPatterns.First(x => x.Type == EntityDescriptionType.ShortDescription)
					                                 .Pattern;
			}

			if (SelectedEntityDescriptionPatterns.All(x => x.Type != EntityDescriptionType.FullDescription))
			{
				element = root.Element("SelectedFullDesc");
				if (element != null)
				{
					SelectedFullDesc = element.Value;
				}
			}
			else
			{
				SelectedFullDesc =
					SelectedEntityDescriptionPatterns.First(x => x.Type == EntityDescriptionType.FullDescription)
					                                 .Pattern;
			}

			element = root.Element("SelectedRace");
			if (element != null)
			{
				SelectedRace = Gameworld.Races.Get(long.Parse(element.Value));
			}

			element = root.Element("SelectedCulture");
			if (element != null)
			{
				SelectedCulture = Gameworld.Cultures.Get(long.Parse(element.Value));
			}

			element = root.Element("SelectedEthnicity");
			if (element != null)
			{
				SelectedEthnicity = Gameworld.Ethnicities.Get(long.Parse(element.Value));
			}

			element = root.Element("SelectedGender");
			if (element != null)
			{
				SelectedGender = (Gender)Convert.ToInt16(element.Value);
			}

			element = root.Element("SelectedHeight");
			if (element != null)
			{
				SelectedHeight = double.Parse(element.Value);
			}

			element = root.Element("SelectedWeight");
			if (element != null)
			{
				SelectedWeight = double.Parse(element.Value);
			}

			element = root.Element("SelectedName");
			if (element != null)
			{
				if (string.IsNullOrWhiteSpace(element.Value))
				{
					SelectedName = null;
				}
				else if (long.TryParse(element.Value, out _))
				{
					SelectedName = SelectedCulture?.NameCultureForGender(SelectedGender).GetPersonalName(Name);
					Changed = true;
				}
				else
				{
					SelectedName = new PersonalName(element.Element("Name"), Gameworld);
				}
			}

			try
			{
				SelectedBirthday = string.IsNullOrEmpty(root.Element("SelectedBirthday")?.Value)
					? null
					: SelectedCulture.PrimaryCalendar.GetDate(root.Element("SelectedBirthday").Value);
			}
			catch (MUDDateException)
			{
				// This should only happen when a name culture's calendar is changed after chargens have been created, and therefore rarely and only in some MUDs.
				foreach (var calendar in Gameworld.Calendars)
				{
					try
					{
						SelectedBirthday = calendar.GetDate(root.Element("SelectedBirthday").Value);
						break;
					}
					catch (MUDDateException)
					{
					}
				}
			}

			foreach (var item in root.Element("SelectedAttributes").Elements("Attribute"))
			{
				if (Gameworld.Traits.Get(long.Parse(item.Attribute("Id").Value)) is not AttributeDefinition definition)
				{
					continue;
				}

				SelectedAttributes.Add(new Attribute(definition, double.Parse(item.Attribute("Value").Value), true));
			}

			foreach (var item in root.Element("SelectedSkills").Elements("Skill"))
			{
				var trait = Gameworld.Traits.Get(long.Parse(item.Value));
				if (trait == null)
				{
					continue;
				}

				SkillValues.Add(Tuple.Create(trait, double.Parse(item.Attribute("Value").Value)));
			}

			foreach (var item in root.Element("SelectedAccents").Elements("Accent"))
			{
				var accent = Gameworld.Accents.Get(long.Parse(item.Value));
				if (accent == null)
				{
					continue;
				}

				SelectedAccents.Add(accent);
			}

			foreach (var item in root.Element("SelectedCharacteristics").Elements("Characteristic"))
			{
				var definition = Gameworld.Characteristics.Get(long.Parse(item.Attribute("Definition").Value));
				var value = Gameworld.CharacteristicValues.Get(long.Parse(item.Attribute("Value").Value));
				if (definition == null || value == null)
				{
					continue;
				}

				SelectedCharacteristics.Add(
					Tuple.Create(definition, value));
			}

			element = root.Element("SelectedRoles");
			if (element != null)
			{
				foreach (var item in element.Elements("Role"))
				{
					var role = Gameworld.Roles.Get(long.Parse(item.Value));
					if (role == null)
					{
						continue;
					}

					SelectedRoles.Add(role);
				}
			}

			element = root.Element("Handedness");
			if (element != null)
			{
				Handedness = (Alignment)int.Parse(element.Value);
			}
			else
			{
				Handedness = SelectedRace?.DefaultHandedness ?? Alignment.Right;
			}

			element = root.Element("Knowledges");
			if (element != null)
			{
				foreach (var item in element.Elements("Knowledge"))
				{
					var knowledge = Gameworld.Knowledges.Get(long.Parse(item.Value));
					if (knowledge == null)
					{
						continue;
					}

					SelectedKnowledges.Add(knowledge);
				}
			}

			element = root.Element("Merits");
			if (element != null)
			{
				foreach (var item in element.Elements("Merit"))
				{
					var merit = Gameworld.Merits.Get(long.Parse(item.Value));
					if (merit == null || !(merit is ICharacterMerit cmerit))
					{
						continue;
					}

					SelectedMerits.Add(cmerit);
				}
			}

			element = root.Element("MissingBodyparts");
			if (element != null)
			{
				foreach (var item in element.Elements("Bodypart"))
				{
					var bodypart = Gameworld.BodypartPrototypes.Get(long.Parse(item.Value));
					if (bodypart == null)
					{
						continue;
					}

					MissingBodyparts.Add(bodypart);
				}
			}
		}
	}

	private string SaveDefinition()
	{
		return
			new XElement("Definition", new XElement("OnLoadProg", OnLoadProg?.Id ?? 0),
				new XElement("SelectedSdesc", new XCData(SelectedSdesc ?? "")),
				new XElement("SelectedFullDesc", new XCData(SelectedFullDesc ?? "")),
				new XElement("SelectedRace", SelectedRace?.Id ?? -1),
				new XElement("SelectedCulture", SelectedCulture?.Id ?? -1),
				new XElement("SelectedEthnicity", SelectedEthnicity?.Id ?? -1),
				new XElement("SelectedName", SelectedName?.SaveToXml() ?? new XElement("Name")),
				new XElement("SelectedBirthday",
					SelectedBirthday != null ? SelectedBirthday.GetDateString() : ""),
				new XElement("SelectedGender", (int)SelectedGender),
				new XElement("SelectedHeight", SelectedHeight),
				new XElement("SelectedWeight", SelectedWeight),
				new XElement("SelectedEntityDescriptionPatterns", new object[]
				{
					from pattern in SelectedEntityDescriptionPatterns
					select new XElement("Pattern", pattern.Id)
				}), new XElement("SelectedAttributes", new object[]
				{
					from attribute in SelectedAttributes
					select
						new XElement("Attribute", new XAttribute("Id", attribute.Definition.Id),
							new XAttribute("Value", attribute.Value))
				}), new XElement("SelectedSkills", new object[]
				{
					from skill in SkillValues
					select new XElement("Skill", new XAttribute("Value", skill.Item2), skill.Item1.Id)
				}), new XElement("SelectedAccents", new object[]
				{
					from accent in SelectedAccents select new XElement("Accent", accent.Id)
				}), new XElement("SelectedCharacteristics", from characteristic in SelectedCharacteristics
				                                            select
					                                            new XElement(
						                                            "Characteristic",
						                                            new XAttribute(
							                                            "Definition",
							                                            characteristic.Item1.Id),
						                                            new XAttribute(
							                                            "Value",
							                                            characteristic.Item2.Id))),
				new XElement("SelectedRoles",
					from role in SelectedRoles select new XElement("Role", role.Id),
					new XElement("Handedness", (int)Handedness)),
				new XElement("Knowledges",
					from knowledge in SelectedKnowledges
					select new XElement("Knowledge", knowledge.Id)),
				new XElement("MissingBodyparts",
					from bodypart in MissingBodyparts
					select new XElement("Bodypart", bodypart.Id)),
				new XElement("Merits", from merit in SelectedMerits select new XElement("Merit", merit.Id))
			).ToString();
	}

	public override bool CanSubmit()
	{
		return SelectedRace != null && SelectedCulture != null && SelectedEthnicity != null &&
		       SelectedGender != Form.Shape.Gender.Indeterminate && SelectedName != null &&
		       SelectedBirthday != null &&
		       SelectedSkills.Where(x => Gameworld.Languages.Any(y => y.LinkedTrait == x))
		                     .All(x => SelectedAccents.Any(y => y.Language.LinkedTrait == x)) &&
		       !string.IsNullOrEmpty(SelectedSdesc) &&
		       SelectedRace.HandednessOptions.Contains(Handedness) &&
		       !string.IsNullOrEmpty(SelectedFullDesc);
	}

	public override string WhyCannotSubmit()
	{
		if (SelectedRace == null)
		{
			return "You must first set a race for this NPC.";
		}

		if (SelectedCulture == null)
		{
			return "You must first set a culture for this NPC.";
		}

		if (SelectedEthnicity == null)
		{
			return "You must first set an ethnicity for this NPC.";
		}

		if (SelectedGender == Form.Shape.Gender.Indeterminate)
		{
			return "You must set a gender for this NPC that is not Indeterminate.";
		}

		if (SelectedName == null)
		{
			return "You must set a name for this NPC.";
		}

		if (SelectedBirthday == null)
		{
			return "You must first give this NPC a birthday.";
		}

		if (SelectedSkills.Any(x => Gameworld.Languages.Any(y => y.LinkedTrait == x) &&
		                            SelectedAccents.All(y => y.Language.LinkedTrait != x)))
		{
			return "You have selected language skills for which you have not given this NPC any accents.";
		}

		if (string.IsNullOrWhiteSpace(SelectedSdesc))
		{
			return "You must first give this NPC a short description.";
		}

		if (string.IsNullOrWhiteSpace(SelectedFullDesc))
		{
			return "You must first give this NPC a full description.";
		}

		if (!SelectedRace.HandednessOptions.Contains(Handedness))
		{
			return "The handedness option that you picked is not a valid option for this race.";
		}

		return base.WhyCannotSubmit();
	}

	#region Building Commands

	private void CheckCharacteristicsCreation()
	{
		if (SelectedRace == null ||
		    SelectedGender == Form.Shape.Gender.Indeterminate)
		{
			return;
		}

		if (SelectedGender == Form.Shape.Gender.Indeterminate)
		{
			SelectedGender = SelectedRace.AllowedGenders.GetRandomElement();
		}

		var template = GetCharacterTemplate();

		SelectedCharacteristics =
			SelectedRace.Characteristics(SelectedGender)
			            .Select(x =>
				            Tuple.Create(x,
					            SelectedEthnicity.CharacteristicChoices[x].GetRandomCharacteristic(template)))
			            .ToList();
	}

	private bool BuildingCommandKnowledge(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which knowledge do you want to add or remove for this NPC?");
			return false;
		}

		var knowledge = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.Knowledges.Get(value)
			: Gameworld.Knowledges.GetByName(command.SafeRemainingArgument);
		if (knowledge == null)
		{
			actor.Send("There is no such knowledge.");
			return false;
		}

		if (SelectedKnowledges.Contains(knowledge))
		{
			SelectedKnowledges.Remove(knowledge);
			Changed = true;
			actor.Send($"This NPC will no longer have the {knowledge.Name.Colour(Telnet.Cyan)} knowledge.");
			return true;
		}

		SelectedKnowledges.Add(knowledge);
		Changed = true;
		actor.Send($"This NPC will now have the {knowledge.Name.Colour(Telnet.Cyan)} knowledge.");
		return true;
	}

	private bool BuildingCommandMerit(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which merit or flaw do you want to add or remove for this NPC?");
			return false;
		}

		var merit = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.Merits.Get(value)
			: Gameworld.Merits.GetByName(command.SafeRemainingArgument);
		if (merit == null)
		{
			actor.Send("There is no such merit or flaw.");
			return false;
		}

		if (!(merit is ICharacterMerit cmerit))
		{
			actor.Send($"The {merit.Name.Colour(Telnet.Cyan)} merit is not a character merit.");
			return false;
		}

		if (SelectedMerits.Contains(cmerit))
		{
			SelectedMerits.Remove(cmerit);
			Changed = true;
			actor.Send(
				$"This NPC will no longer have the {merit.Name.Colour(Telnet.Cyan)} {(merit.MeritType == MeritType.Merit ? "merit" : "flaw")}.");
			return true;
		}

		SelectedMerits.Add(cmerit);
		Changed = true;
		actor.Send(
			$"This NPC will now have the {merit.Name.Colour(Telnet.Cyan)} {(merit.MeritType == MeritType.Merit ? "merit" : "flaw")}.");
		return true;
	}

	private bool BuildingCommandHand(ICharacter actor, StringStack command)
	{
		if (SelectedRace == null)
		{
			actor.Send("You must first select a race.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.Send(
				$"What handedness do you want this NPC to have? Valid options are {SelectedRace.HandednessOptions.Select(x => x.Describe().Colour(Telnet.Green)).ListToString()}.");
			return false;
		}

		if (!Enum.TryParse(command.SafeRemainingArgument, out Alignment alignment))
		{
			actor.Send(
				$"That is not a valid selection for handedness. Valid options are {SelectedRace.HandednessOptions.Select(x => x.Describe().Colour(Telnet.Green)).ListToString()}.");
			return false;
		}

		Handedness = alignment;
		actor.Send($"This NPC is now {$"{alignment.Describe()}-handed".Colour(Telnet.Green)}.");
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

		SelectedRoles.RemoveAll(x => x.RoleType == ChargenRoleType.Class);
		SelectedRoles.Add(selectedClass);
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

		SelectedRoles.RemoveAll(x => x.RoleType == ChargenRoleType.Subclass);
		SelectedRoles.Add(selectedClass);
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

		if (SelectedRoles.Contains(selectedRole))
		{
			SelectedRoles.Remove(selectedRole);
			actor.Send("You remove the role {0} from this NPC.", selectedRole.Name.TitleCase().Colour(Telnet.Green));
		}
		else
		{
			SelectedRoles.Add(selectedRole);
			actor.Send("You add the role {0} to this NPC.", selectedRole.Name.TitleCase().Colour(Telnet.Green));
		}

		Changed = true;
		return true;
	}

	private bool BuildingCommandAccent(ICharacter actor, StringStack command)
	{
		var accent = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.Accents.Get(value)
			: Gameworld.Accents.FirstOrDefault(
				x => x.Name.StartsWith(command.Last, StringComparison.InvariantCultureIgnoreCase));

		if (accent == null)
		{
			actor.OutputHandler.Send("There is no such accent.");
			return false;
		}

		if (!SelectedSkills.Contains(accent.Language.LinkedTrait))
		{
			actor.OutputHandler.Send(
				$"You must first give this NPC the {accent.Language.Name.Proper().Colour(Telnet.Cyan)} language.");
			return false;
		}

		if (SelectedAccents.Contains(accent))
		{
			SelectedAccents.Remove(accent);
			actor.OutputHandler.Send($"This NPC will no longer have the {accent.Name.Colour(Telnet.Cyan)} accent.");
		}
		else
		{
			SelectedAccents.Add(accent);
			actor.OutputHandler.Send($"This NPC will now have the {accent.Name.Colour(Telnet.Cyan)} accent.");
		}

		Changed = true;
		return true;
	}

	private bool BuildingCommandAttribute(ICharacter actor, StringStack command)
	{
		if (SelectedRace == null)
		{
			actor.OutputHandler.Send("You must first assign a race to this NPC.");
			return false;
		}

		var trait = long.TryParse(command.PopSpeech(), out var value)
			? SelectedAttributes.Find(x => x.Definition.Id == value)
			: SelectedAttributes.Find(
				x =>
					string.Equals(((IAttributeDefinition)x.Definition).Alias, command.Last,
						StringComparison.InvariantCultureIgnoreCase));
		if (trait == null)
		{
			actor.OutputHandler.Send("There is no such attribute.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What value do you want to set the {trait.Definition.Name.Proper().Colour(Telnet.Cyan)} attribute to?");
			return false;
		}

		if (!double.TryParse(command.Pop(), out var dValue))
		{
			actor.OutputHandler.Send("You must enter a number for the value of the attribute.");
			return false;
		}

		trait.Value = dValue;
		actor.OutputHandler.Send(
			$"This NPC will now have a {trait.Definition.Name.Proper().Colour(Telnet.Cyan)} attribute of {dValue:N2}.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandBirthday(ICharacter actor, StringStack command)
	{
		if (SelectedCulture == null)
		{
			actor.OutputHandler.Send("You must first select a Culture for this NPC.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What birthday do you want to give to this NPC?");
			return false;
		}

		try
		{
			MudDate date;
			if (command.Peek().Equals("random", StringComparison.InvariantCultureIgnoreCase))
			{
				command.Pop();
				if (command.IsFinished)
				{
					actor.Send("How old do you want this NPC to be?");
					return false;
				}

				if (!int.TryParse(command.Pop(), out var age))
				{
					actor.Send("You must enter a number in years for the age of this NPC.");
					return false;
				}

				date = SelectedCulture.PrimaryCalendar.GetRandomBirthday(age);
			}
			else
			{
				date = SelectedCulture.PrimaryCalendar.GetDate(command.SafeRemainingArgument);
			}

			SelectedBirthday = date;
			actor.Send("This NPC was now born on {0}.", date.Calendar.DisplayDate(date, CalendarDisplayMode.Long));
			Changed = true;
			return true;
		}
		catch (MUDDateException e)
		{
			actor.OutputHandler.Send(e.Message);
		}

		return false;
	}

	private bool BuildingCommandRandomise(ICharacter actor, StringStack command)
	{
		if (SelectedRace is null || SelectedEthnicity is null || SelectedCulture is null ||
		    SelectedGender == Form.Shape.Gender.Indeterminate)
		{
			actor.OutputHandler.Send("You must first select a Race, Culture, Ethnicity and a Gender for this NPC.");
			return false;
		}

		var statrolls = RollRandomStats(SelectedRace.Attributes.Count(),
			SelectedRace.AttributeTotalCap, SelectedRace.IndividualAttributeCap,
			SelectedRace.DiceExpression);
		SelectedAttributes.Clear();
		var attributeOrder = SelectedRace.Attributes.Shuffle().ToList();
		for (var i = 0; i < attributeOrder.Count; i++)
		{
			SelectedAttributes.Add(TraitFactory.LoadAttribute(attributeOrder[i], null, statrolls[i]));
		}

		var category = RandomiseAgeChances.GetWeightedRandom(x => x.Weight).Age;
		var randomAge = 1;
		switch (category)
		{
			case AgeCategory.Youth:
				randomAge = RandomUtilities.Random(SelectedRace.MinimumAgeForCategory(AgeCategory.Youth),
					SelectedRace.MinimumAgeForCategory(AgeCategory.YoungAdult));
				break;
			case AgeCategory.YoungAdult:
				randomAge = RandomUtilities.Random(SelectedRace.MinimumAgeForCategory(AgeCategory.YoungAdult),
					SelectedRace.MinimumAgeForCategory(AgeCategory.Adult));
				break;
			case AgeCategory.Adult:
				randomAge = RandomUtilities.Random(SelectedRace.MinimumAgeForCategory(AgeCategory.Adult),
					SelectedRace.MinimumAgeForCategory(AgeCategory.Elder));
				break;
			case AgeCategory.Elder:
				randomAge = RandomUtilities.Random(SelectedRace.MinimumAgeForCategory(AgeCategory.Elder),
					SelectedRace.MinimumAgeForCategory(AgeCategory.Venerable));
				break;
			case AgeCategory.Venerable:
				randomAge = RandomUtilities.Random(SelectedRace.MinimumAgeForCategory(AgeCategory.Venerable),
					(int)(SelectedRace.MinimumAgeForCategory(AgeCategory.Venerable) * 1.1));
				break;
		}

		SelectedBirthday = SelectedCulture.PrimaryCalendar.GetRandomBirthday(randomAge);
		var template = GetCharacterTemplate();
		var cghp = (HeightPickerScreenStoryboard)Gameworld.ChargenStoryboard.StageScreenMap[ChargenStage.SelectHeight];
		SelectedHeight = RandomUtilities.RandomNormalOverRange(cghp.MinimumHeightProg.ExecuteDouble(template),
			cghp.MaximumHeightProg.ExecuteDouble(template));
		template = GetCharacterTemplate();
		var cgwp =
			(WeightPickerScreenStoryboard)Gameworld.ChargenStoryboard.StageScreenMap[ChargenStage.SelectWeight];
		SelectedWeight = RandomUtilities.RandomNormalOverRange(cgwp.MinimumWeightProg.ExecuteDouble(template),
			cgwp.MaximumWeightProg.ExecuteDouble(template));
		template = GetCharacterTemplate();

		SelectedName = SelectedCulture.NameCultureForGender(SelectedGender).RandomNameProfiles
		                              .Where(x => x.IsCompatibleGender(SelectedGender)).GetRandomElement()
		                              ?.GetRandomPersonalName(true);
		SelectedCharacteristics.Clear();

		var characteristicChoices = SelectedRace.Characteristics(SelectedGender);
		foreach (var choice in
		         SelectedEthnicity.CharacteristicChoices.Where(x => characteristicChoices.Contains(x.Key)))
		{
			SelectedCharacteristics.Add(Tuple.Create(choice.Key,
				choice.Value.GetRandomCharacteristic(template) ?? choice.Value.GetRandomCharacteristic()));
		}

		var sdesc = Gameworld.EntityDescriptionPatterns
		                     .Where(x => x.Type == EntityDescriptionType.ShortDescription &&
		                                 x.IsValidSelection(template))
		                     .GetWeightedRandom(x => x.RelativeWeight);
		SelectedEntityDescriptionPatterns.Add(sdesc);
		SelectedSdesc = sdesc?.Pattern ?? $"a {SelectedRace.Name.ToLowerInvariant()}";

		var fdesc = Gameworld.EntityDescriptionPatterns
		                     .Where(x => x.Type == EntityDescriptionType.FullDescription &&
		                                 x.IsValidSelection(template))
		                     .GetWeightedRandom(x => x.RelativeWeight);
		SelectedEntityDescriptionPatterns.Add(fdesc);
		SelectedFullDesc = fdesc?.Pattern ?? "You cannot tell anything special or unique about it.";

		Changed = true;
		actor.OutputHandler.Send($"You randomise this NPC's appearance and characteristics.\n\n{Show(actor)}");
		return true;
	}

	private static readonly (AgeCategory Age, double Weight)[] RandomiseAgeChances = new[]
	{
		(AgeCategory.Adult, 10.0),
		(AgeCategory.YoungAdult, 4.0),
		(AgeCategory.Elder, 3.0),
		(AgeCategory.Venerable, 1.0),
		(AgeCategory.Youth, 1.0)
	};

	private bool BuildingCommandCharacteristics(ICharacter actor, StringStack command)
	{
		if (SelectedRace == null || SelectedEthnicity == null ||
		    SelectedGender == Form.Shape.Gender.Indeterminate)
		{
			actor.OutputHandler.Send("You must first select a Race, Ethnicity and a Gender for this NPC.");
			return false;
		}

		var cmd = command.PopSpeech();
		if (string.IsNullOrEmpty(cmd))
		{
			actor.OutputHandler.Send("Which characteristic do you want to edit for this NPC?");
			return false;
		}

		var definition = SelectedCharacteristics.FirstOrDefault(x => x.Item1.Pattern.IsMatch(cmd));
		if (definition == null)
		{
			actor.OutputHandler.Send("There is no such characteristic definition.");
			return false;
		}

		cmd = command.PopSpeech();
		if (string.IsNullOrEmpty(cmd))
		{
			actor.OutputHandler.Send("Which characteristic value do you want to give this NPC?");
			return false;
		}

		var value =
			Gameworld.CharacteristicValues.FirstOrDefault(
				x =>
					definition.Item1.IsValue(x) &&
					string.Equals(x.GetValue, cmd, StringComparison.InvariantCultureIgnoreCase));
		if (value == null)
		{
			actor.OutputHandler.Send("There is no such characteristic value.");
			return false;
		}

		SelectedCharacteristics.Remove(definition);
		SelectedCharacteristics.Add(Tuple.Create(definition.Item1, value));
		actor.OutputHandler.Send(
			$"This NPC will now have {definition.Item1.Name.A_An(true, Telnet.Cyan)} of \"{value.GetValue}\"");
		Changed = true;
		return true;
	}

	private bool BuildingCommandCulture(ICharacter actor, StringStack command)
	{
		var culture = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.Cultures.Get(value)
			: Gameworld.Cultures.FirstOrDefault(
				x => string.Equals(x.Name, command.Last, StringComparison.InvariantCultureIgnoreCase));
		if (culture == null)
		{
			actor.OutputHandler.Send("That is not a valid culture.");
			return false;
		}

		SelectedCulture = culture;
		if (SelectedBirthday is null && SelectedRace is not null)
		{
			SelectedBirthday =
				SelectedCulture.PrimaryCalendar.GetRandomBirthday(
					SelectedRace.MinimumAgeForCategory(AgeCategory.Adult));
		}

		actor.OutputHandler.Send($"This NPC now belongs to the {culture.Name.Proper().Colour(Telnet.Cyan)} culture.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandEthnicity(ICharacter actor, StringStack command)
	{
		var ethnicity = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.Ethnicities.Get(value)
			: Gameworld.Ethnicities.FirstOrDefault(
				x => string.Equals(x.Name, command.Last, StringComparison.InvariantCultureIgnoreCase));
		if (ethnicity == null)
		{
			actor.OutputHandler.Send("That is not a valid ethnicity.");
			return false;
		}

		SelectedEthnicity = ethnicity;
		CheckCharacteristicsCreation();
		actor.OutputHandler.Send(
			$"This NPC now belongs to the {ethnicity.Name.Proper().Colour(Telnet.Cyan)} ethnicity.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandSDesc(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What short description do you want to give this NPC?");
			return false;
		}

		SelectedSdesc = command.SafeRemainingArgument;
		SelectedEntityDescriptionPatterns.RemoveAll(x => x.Type == EntityDescriptionType.ShortDescription);
		Changed = true;
		actor.OutputHandler.Send(
			$"You change this NPC's short description to {command.SafeRemainingArgument.Colour(Telnet.Magenta)}.");
		return true;
	}

	private bool BuildingCommandPattern(ICharacter actor, StringStack command)
	{
		var template = GetCharacterTemplate();
		IEnumerable<IEntityDescriptionPattern> patterns;
		EntityDescriptionType type;
		switch (command.Pop())
		{
			case "sdesc":
				patterns =
					Gameworld.EntityDescriptionPatterns
					         .Where(x => x.Type == EntityDescriptionType.ShortDescription &&
					                     x.IsValidSelection(template));
				type = EntityDescriptionType.ShortDescription;
				break;
			case "desc":
				patterns =
					Gameworld.EntityDescriptionPatterns.Where(x =>
						x.Type == EntityDescriptionType.FullDescription && x.IsValidSelection(template));
				type = EntityDescriptionType.FullDescription;
				break;
			default:
				actor.OutputHandler.Send(
					"Do you want to set a pattern for a short description (sdesc) or a full description (desc)?");
				return false;
		}

		IEntityDescriptionPattern pattern;
		if (command.Peek().Equals("random", StringComparison.InvariantCultureIgnoreCase))
		{
			pattern = patterns.GetRandomElement();
		}
		else
		{
			pattern = long.TryParse(command.Pop(), out var value) ? patterns.FirstOrDefault(x => x.Id == value) : null;
		}

		if (pattern == null)
		{
			if (string.Equals(command.Last, "clear", StringComparison.InvariantCultureIgnoreCase))
			{
				SelectedEntityDescriptionPatterns.RemoveAll(x => x.Type == type);
				actor.OutputHandler.Send(
					$"You clear this NPC's pattern for {(type == EntityDescriptionType.ShortDescription ? "short descriptions" : "full descriptions").Colour(Telnet.Cyan)}.");
				return true;
			}

			actor.OutputHandler.Send("That is not a valid pattern.");
			return false;
		}

		SelectedEntityDescriptionPatterns.RemoveAll(x => x.Type == type);
		SelectedEntityDescriptionPatterns.Add(pattern);
		switch (type)
		{
			case EntityDescriptionType.ShortDescription:
				SelectedSdesc = pattern.Pattern;
				actor.OutputHandler.Send(
					$"You change the short description pattern for this NPC to {pattern.Pattern}.");
				break;
			case EntityDescriptionType.FullDescription:
				SelectedFullDesc = pattern.Pattern;
				actor.OutputHandler.Send(
					$"You change the full description pattern for this NPC to {pattern.Pattern}.");
				break;
		}

		Changed = true;
		return true;
	}

	private void PostDesc(string description, IOutputHandler handler, object[] arguments)
	{
		handler.Send($"You change the full description for this NPC to:\n{description.Wrap(80, "\t")}");
		SelectedFullDesc = description;
		SelectedEntityDescriptionPatterns.RemoveAll(x => x.Type == EntityDescriptionType.FullDescription);
		Changed = true;
	}

	private void CancelPostDesc(IOutputHandler handler, object[] arguments)
	{
		handler.Send("You decline to update the full description for this NPC.");
	}

	private bool BuildingCommandDesc(ICharacter actor, StringStack command)
	{
		if (SelectedFullDesc.Length > 0)
		{
			actor.OutputHandler.Send("Replacing:\n\n" + SelectedFullDesc.Wrap(actor.InnerLineFormatLength, "\t"));
		}

		actor.OutputHandler.Send("\nEnter the description in the editor below.");
		actor.EditorMode(PostDesc, CancelPostDesc, 1.0);
		return true;
	}

	private bool BuildingCommandGender(ICharacter actor, StringStack command)
	{
		if (SelectedRace == null)
		{
			actor.OutputHandler.Send("You must first select a race for this NPC.");
			return false;
		}

		if (string.IsNullOrEmpty(command.Pop()))
		{
			actor.OutputHandler.Send("What gender do you want to give to this NPC?");
			return false;
		}

		var targetGender =
			SelectedRace.AllowedGenders.FirstOrDefault(
				x => string.Equals(Gendering.Get(x).Name, command.Last, StringComparison.InvariantCultureIgnoreCase));
		if (targetGender == Form.Shape.Gender.Indeterminate)
		{
			actor.OutputHandler.Send(
				$"That is not a valid gender for this NPC's race. Valid genders are {SelectedRace.AllowedGenders.Select(x => Gendering.Get(x).Name.Colour(Telnet.Green)).ListToString()}.");
			return false;
		}

		SelectedGender = targetGender;
		actor.Send("You set this NPC's gender to be {0}.", Gendering.Get(targetGender).Name.Colour(Telnet.Cyan));
		CheckCharacteristicsCreation();
		Changed = true;
		return true;
	}

	private bool BuildingCommandHeight(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What height do you want to give to this NPC?");
			return false;
		}

		var value = Gameworld.UnitManager.GetBaseUnits(command.SafeRemainingArgument, UnitType.Length, out var success);
		if (!success)
		{
			actor.OutputHandler.Send("That is not a valid height.");
			return false;
		}

		SelectedHeight = value;
		actor.OutputHandler.Send(
			$"You set this NPC's height to {Gameworld.UnitManager.Describe(value, UnitType.Length, actor).Colour(Telnet.Green)}.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (SelectedCulture == null)
		{
			actor.OutputHandler.Send("Before you can select a name for this NPC, you must first select a culture.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter a name to assign to this NPC.");
			return false;
		}

		var name = SelectedCulture.NameCultureForGender(SelectedGender).GetPersonalName(command.SafeRemainingArgument);
		if (name == null)
		{
			actor.OutputHandler.Send("That is not a valid name for that culture.");
			return false;
		}

		SelectedName = name;
		_name = SelectedName.GetName(NameStyle.FullWithNickname);
		actor.Send("You set this NPC's name to {0}.",
			name.GetName(NameStyle.FullName).TitleCase().Colour(Telnet.Cyan));
		Changed = true;
		return true;
	}

	private bool BuildingCommandRace(ICharacter actor, StringStack command)
	{
		var race = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.Races.Get(value)
			: Gameworld.Races.FirstOrDefault(
				x => string.Equals(x.Name, command.Last, StringComparison.InvariantCultureIgnoreCase));
		if (race == null)
		{
			actor.OutputHandler.Send("That is not a valid race.");
			return false;
		}

		SelectedRace = race;
		SelectedAttributes = race.Attributes.Select(x => TraitFactory.LoadAttribute(x, null, 10)).ToList<ITrait>();
		CheckCharacteristicsCreation();
		Handedness = SelectedRace.DefaultHandedness;
		if (SelectedBirthday is null && SelectedCulture is not null)
		{
			SelectedBirthday =
				SelectedCulture.PrimaryCalendar.GetRandomBirthday(
					SelectedRace.MinimumAgeForCategory(AgeCategory.Adult));
		}

		actor.OutputHandler.Send($"This NPC is now of the {race.Name.Proper().Colour(Telnet.Cyan)} race.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandSkill(ICharacter actor, StringStack command)
	{
		if (string.IsNullOrEmpty(command.PopSpeech()))
		{
			actor.OutputHandler.Send("What skill do you want to set on this NPC?");
			return false;
		}

		var skill =
			Gameworld.Traits.FirstOrDefault(
				x =>
					x.TraitType == TraitType.Skill &&
					x.Name.StartsWith(command.Last, StringComparison.InvariantCultureIgnoreCase));
		if (skill == null)
		{
			actor.OutputHandler.Send("There is no such skill.");
			return false;
		}

		if (!double.TryParse(command.Pop(), out var value))
		{
			actor.OutputHandler.Send("You must enter a value for this NPCs skill to be set to.");
			return false;
		}

		if (value <= 0)
		{
			SkillValues.RemoveAll(x => x.Item1 == skill);
			actor.Send("You remove the {0} skill from this NPC.", skill.Name.Proper().Colour(Telnet.Cyan));
			return true;
		}

		SkillValues.RemoveAll(x => x.Item1 == skill);
		SkillValues.Add(Tuple.Create(skill, value));
		Changed = true;
		actor.OutputHandler.Send(
			$"You set the {skill.Name.Proper().Colour(Telnet.Cyan)} skill for this NPC to be {value.ToString("N2").Colour(Telnet.Green)}.");
		return true;
	}

	private bool BuildingCommandWeight(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What weight do you want to give to this NPC?");
			return false;
		}

		var value = Gameworld.UnitManager.GetBaseUnits(command.SafeRemainingArgument, UnitType.Mass, out var success);
		if (!success)
		{
			actor.OutputHandler.Send("That is not a valid weight.");
			return false;
		}

		SelectedWeight = value;
		actor.OutputHandler.Send(
			$"You set this NPC's weight to {Gameworld.UnitManager.Describe(value, UnitType.Mass, actor).Colour(Telnet.Green)}.");
		Changed = true;
		return true;
	}

	#endregion

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

	public List<ITraitDefinition> SelectedSkills
	{
		get { return SkillValues.Select(x => x.Item1).ToList(); }
	}

	public double SelectedWeight { get; set; }

	public List<IChargenRole> SelectedRoles { get; set; }

	public List<IKnowledge> SelectedKnowledges { get; set; } = new();

	public List<ICharacterMerit> SelectedMerits { get; set; } = new();

	public Alignment Handedness { get; set; }

	public List<IBodypart> MissingBodyparts { get; set; } = new();

	public List<(IDisfigurementTemplate Disfigurement, IBodypart Bodypart)> SelectedDisfigurements { get; set; } =
		new();

	public List<IGameItemProto> SelectedProstheses { get; set; }

	#endregion
}