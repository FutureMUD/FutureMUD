using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Body.Grouping;
using MudSharp.Body.Traits.Improvement;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Combat;
using MudSharp.Commands.Modules;
using MudSharp.Commands.Trees;
using MudSharp.Community;
using MudSharp.Construction.Autobuilder;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Property;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Units;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Generators;
using MudSharp.Magic.Powers;
using MudSharp.Magic.Resources;
using MudSharp.NPC;
using MudSharp.NPC.AI;
using MudSharp.NPC.Templates;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Dreams;
using MudSharp.RPG.Hints;
using Newtonsoft.Json.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using AuctionHouse = MudSharp.Economy.Auctions.AuctionHouse;
using Bank = MudSharp.Economy.Banking.Bank;
using ChargenAdvice = MudSharp.CharacterCreation.ChargenAdvice;
using Dream = MudSharp.RPG.Dreams.Dream;
using MagicSpell = MudSharp.Magic.MagicSpell;
using NPCSpawner = MudSharp.NPC.NPCSpawner;
using Property = MudSharp.Economy.Property.Property;

namespace MudSharp.Commands.Helpers;



public partial class EditableItemHelper
{
	public static EditableItemHelper RaceHelper { get; } = new()
	{
		ItemName = "Race",
		ItemNamePlural = "Races",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IRace>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IRace>(actor) { EditingItem = (IRace)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IRace>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.Races.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.Races.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.Races.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IRace)item),
		CastToType = typeof(IRace),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your race.");
				return;
			}

			var name = input.PopSpeech().TitleCase();
			if (actor.Gameworld.Races.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send($"There is already a race called {name.ColourName()}. Names must be unique.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a body prototype for your new race.");
				return;
			}

			var body = actor.Gameworld.BodyPrototypes.GetByIdOrName(input.PopSpeech());
			if (body is null)
			{
				actor.OutputHandler.Send("There is no such body prototype.");
				return;
			}

			IRace parent = null;
			if (!input.IsFinished)
			{
				parent = actor.Gameworld.Races.GetByIdOrName(input.SafeRemainingArgument);
				if (parent is null)
				{
					actor.OutputHandler.Send("There is no such race to set as the parent race of your new race.");
					return;
				}
			}

			var race = new Race(actor.Gameworld, name, body, parent);
			actor.Gameworld.Add(race);
			actor.RemoveAllEffects<BuilderEditingEffect<IRace>>();
			actor.AddEffect(new BuilderEditingEffect<IRace>(actor) { EditingItem = race });
			var newEthnicity = new Ethnicity(actor.Gameworld, race, name);
			actor.Gameworld.Add(newEthnicity);
			actor.RemoveAllEffects<BuilderEditingEffect<IEthnicity>>();
			actor.AddEffect(new BuilderEditingEffect<IEthnicity>(actor) { EditingItem = newEthnicity });
			actor.OutputHandler.Send($"You create a new race called {name.ColourName()}{(parent is not null ? $" as a child race of {parent.Name.ColourName()}" : "")}, which you are now editing.\nAlso created a new ethnicity with the same name for that race, which you are also editing.");

		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which race do you want to clone?");
				return;
			}

			var parent = actor.Gameworld.Races.GetByIdOrName(input.PopSpeech());
			if (parent is null)
			{
				actor.OutputHandler.Send("There is no such race for you to clone.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your race.");
				return;
			}

			var name = input.SafeRemainingArgument.TitleCase();
			if (actor.Gameworld.Races.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send($"There is already a race called {name.ColourName()}. Names must be unique.");
				return;
			}

			var race = parent.Clone(name);
			actor.Gameworld.Add(race);
			actor.RemoveAllEffects<BuilderEditingEffect<IRace>>();
			actor.AddEffect(new BuilderEditingEffect<IRace>(actor) { EditingItem = race });
			var newEthnicity = new Ethnicity(actor.Gameworld, race, name);
			actor.Gameworld.Add(newEthnicity);
			actor.RemoveAllEffects<BuilderEditingEffect<IEthnicity>>();
			actor.AddEffect(new BuilderEditingEffect<IEthnicity>(actor) { EditingItem = newEthnicity });
			actor.OutputHandler.Send(
				$"You clone the race {parent.Name.ColourName()} into a new race called {name.ColourName()}, which you are now editing.\nAlso created a new ethnicity for the new race, which you are now editing.");
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Parent",
			"Breathing",
			"Swims",
			"Climbs",
			"Body",
			"# Ethnicities"
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IRace>()
														  select new List<string>
														  {
															  proto.Id.ToString("N0", character),
															  proto.Name,
															  proto.ParentRace?.Name ?? "",
															  proto.BreathingStrategy.Name.TitleCase(),
															  proto.CanSwim.ToString(),
															  proto.CanClimb.ToString(),
															  proto.BaseBody.Name,
															  character.Gameworld.Ethnicities.Count(x => proto.SameRace(x.ParentRace)).ToString("N0", character).MXPSend($"ethnicity list {proto.Name}")
														  },

		CustomSearch = (protos, keyword, gameworld) =>
		{
			if (keyword.Length > 1 && keyword[0] == '+')
			{
				keyword = keyword.Substring(1);
				return protos
					   .Cast<IRace>()
					   .Where(x =>
						   x.Name.Contains(keyword, StringComparison.InvariantCultureIgnoreCase) ||
						   x.Description.Contains(keyword))
					   .Cast<IEditableItem>()
					   .ToList();
			}

			if (keyword.Length > 1 && keyword[0] == '-')
			{
				keyword = keyword.Substring(1);
				return protos
					   .Cast<IRace>()
					   .Where(x =>
						   !x.Name.Contains(keyword, StringComparison.InvariantCultureIgnoreCase) &&
						   !x.Description.Contains(keyword))
					   .Cast<IEditableItem>()
					   .ToList();
			}

			var race = gameworld.Races.GetByIdOrName(keyword);
			if (race is not null)
			{
				return protos
					   .Cast<IRace>()
					   .Where(x => x.ParentRace?.SameRace(race) == true)
					   .Cast<IEditableItem>()
					   .ToList();
			}

			return protos;
		},
		GetEditHeader = item => $"Race #{item.Id:N0} ({item.Name})",
		DefaultCommandHelp = HeritageBuilderModule.RaceHelp
	};

	public static EditableItemHelper BodypartShapeHelper { get; } = new()
	{
		ItemName = "Bodypart Shape",
		ItemNamePlural = "Bodypart Shapes",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IBodypartShape>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IBodypartShape>(actor) { EditingItem = (IBodypartShape)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IBodypartShape>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.BodypartShapes.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.BodypartShapes.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.BodypartShapes.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IBodypartShape)item),
		CastToType = typeof(IBodypartShape),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your bodypart shape.");
				return;
			}

			var name = input.PopSpeech().TitleCase();
			if (actor.Gameworld.BodypartShapes.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already a bodypart shape called {name.ColourName()}. Names must be unique.");
				return;
			}

			var shape = new BodypartShape(actor.Gameworld, name);
			actor.Gameworld.Add(shape);
			actor.RemoveAllEffects<BuilderEditingEffect<IBodypartShape>>();
			actor.AddEffect(new BuilderEditingEffect<IBodypartShape>(actor) { EditingItem = shape });
			actor.OutputHandler.Send(
				$"You create a new bodypart shape called {name.ColourName()}, which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			actor.OutputHandler.Send("Cloning is not supported for bodypart shapes.");
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name"
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IBodypartShape>()
														  select new List<string>
														  {
															  proto.Id.ToString("N0", character),
															  proto.Name
														  },

		CustomSearch = (protos, keyword, gameworld) => protos,
		GetEditHeader = item => $"Bodypart Shape #{item.Id:N0} ({item.Name})",
		DefaultCommandHelp = BuilderModule.BodypartShapesHelp
	};

	public static EditableItemHelper BodypartGroupHelper { get; } = new()
	{
		ItemName = "Bodypart Group",
		ItemNamePlural = "Bodypart Group",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IBodypartGroupDescriber>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IBodypartGroupDescriber>(actor) { EditingItem = (IBodypartGroupDescriber)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IBodypartGroupDescriber>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.BodypartGroupDescriptionRules.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.BodypartGroupDescriptionRules.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.BodypartGroupDescriptionRules.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IBodypartGroupDescriber)item),
		CastToType = typeof(IBodypartGroupDescriber),
		EditableNewAction = (actor, input) =>
		{
			var direct = false;
			switch (input.PopForSwitch())
			{
				case "direct":
					direct = true;
					break;
				case "shape":
					break;
				default:
					actor.OutputHandler.Send($"Do you want to create a #3direct#0 or #3shape#0 bodypart describer?");
					return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which body should this describer be made for?");
				return;
			}

			var body = actor.Gameworld.BodyPrototypes.GetByIdOrName(input.PopSpeech());
			if (body is null)
			{
				actor.OutputHandler.Send($"There is no body identified by the text {input.Last.ColourCommand()}.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your bodypart shape.");
				return;
			}

			var name = input.SafeRemainingArgument.TitleCase();

			var shape = direct ? 
				(IBodypartGroupDescriber)new BodypartGroupIDDescriber(actor.Gameworld, name, body) :
				(IBodypartGroupDescriber)new BodypartGroupShapeDescriber(actor.Gameworld, name, body);
			actor.Gameworld.Add(shape);
			actor.RemoveAllEffects<BuilderEditingEffect<IBodypartGroupDescriber>>();
			actor.AddEffect(new BuilderEditingEffect<IBodypartGroupDescriber>(actor) { EditingItem = shape });
			actor.OutputHandler.Send($"You create a new bodypart group described called {name.ColourName()}, which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which bodypart group describer do you want to clone?");
				return;
			}

			var group = actor.Gameworld.BodypartGroupDescriptionRules.GetByIdOrName(input.SafeRemainingArgument);
			if (group is null)
			{
				actor.OutputHandler.Send($"There is no such bodypart group describer identified by text {input.SafeRemainingArgument.ColourCommand()}.");
				return;
			}

			var clone = group.Clone();
			actor.Gameworld.Add(clone);
			actor.RemoveAllEffects<BuilderEditingEffect<IBodypartGroupDescriber>>();
			actor.AddEffect(new BuilderEditingEffect<IBodypartGroupDescriber>(actor) { EditingItem = clone });
			actor.OutputHandler.Send($"You clone bodypart group {group.Name.ColourName()} to a duplicate group, which you are now editing.");
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Type",
			"Body",
			"Comment"
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IBodypartGroupDescriber>()
														  select new List<string>
														  {
															  proto.Id.ToString("N0", character),
															  proto.Name,
															  proto is BodypartGroupIDDescriber ? "Direct" : "Shape",
															  proto.BodyPrototype.Name,
															  proto.Comment
														  },

		CustomSearch = (protos, keyword, gameworld) =>
		{
			var body = gameworld.BodyPrototypes.GetByIdOrName(keyword);
			if (body is null)
			{
				return protos;
			}

			return protos.OfType<IBodypartGroupDescriber>().Where(x => body.CountsAs(x.BodyPrototype)).ToList<IEditableItem>();
		},
		GetEditHeader = item => $"Bodypart Group Describer #{item.Id:N0} ({item.Name})",
		DefaultCommandHelp = BuilderModule.BodypartGroupHelp
	};

	public static EditableItemHelper ShieldTypeHelper { get; } = new()
	{
		ItemName = "Shield Type",
		ItemNamePlural = "Shield Types",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IShieldType>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IShieldType>(actor) { EditingItem = (IShieldType)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IShieldType>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.ShieldTypes.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.ShieldTypes.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.ShieldTypes.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IShieldType)item),
		CastToType = typeof(IShieldType),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your shield type.");
				return;
			}

			var name = input.PopSpeech().TitleCase();
			if (actor.Gameworld.ShieldTypes.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already a shield type called {name.ColourName()}. Names must be unique.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send($"Which skill or trait should this shield use for blocking?");
				return;
			}

			var trait = actor.Gameworld.Traits.GetByIdOrName(input.PopSpeech());
			if (trait is null)
			{
				actor.OutputHandler.Send($"The text {input.Last.ColourCommand()} is not a valid skill or trait.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which armour type should this shield use?");
				return;
			}

			var armour = actor.Gameworld.ArmourTypes.GetByIdOrName(input.SafeRemainingArgument);
			if (armour is null)
			{
				actor.OutputHandler.Send($"There is no armour type identified by the text {input.SafeRemainingArgument.ColourCommand()}.");
				return;
			}

			var shield = new ShieldType(actor.Gameworld, name, trait, armour);
			actor.Gameworld.Add(shield);
			actor.RemoveAllEffects<BuilderEditingEffect<IShieldType>>();
			actor.AddEffect(new BuilderEditingEffect<IShieldType>(actor) { EditingItem = shield });
			actor.OutputHandler.Send($"You create a new shield type called {name.ColourName()}, which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which shield type would you like to clone?");
				return;
			}

			var target = actor.Gameworld.ShieldTypes.GetByIdOrName(input.PopSpeech());
			if (target is null)
			{
				actor.OutputHandler.Send("There is no such shield type to clone.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("What name would you like to give to the cloned shield type?");
				return;
			}

			var name = input.SafeRemainingArgument.TitleCase();
			if (actor.Gameworld.ShieldTypes.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already a shield type called {name.ColourName()}. Names must be unique.");
				return;
			}

			var clone = target.Clone(name);
			actor.Gameworld.Add(clone);
			actor.RemoveAllEffects<BuilderEditingEffect<IShieldType>>();
			actor.AddEffect(new BuilderEditingEffect<IShieldType>(actor) { EditingItem = clone });
			actor.OutputHandler.Send($"You create a cloned shield type from {target.Name.ColourName()} called {name.ColourName()}, which you are now editing.");
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Trait",
			"Bonus",
			"Stamina",
			"Armour"
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IShieldType>()
														  select new List<string>
														  {
															  proto.Id.ToString("N0", character),
															  proto.Name,
															  proto.BlockTrait.Name,
															  proto.BlockBonus.ToBonusString(character),
															  proto.StaminaPerBlock.ToStringN2Colour(character),
															  proto.EffectiveArmourType?.Name ?? "",
														  },

		CustomSearch = (protos, keyword, gameworld) => protos,
		GetEditHeader = item => $"Shield Type #{item.Id:N0} ({item.Name})",
		DefaultCommandHelp = CombatBuilderModule.ShieldTypeHelp
	};

	public static EditableItemHelper RangedWeaponTypeHelper { get; } = new()
	{
		ItemName = "Ranged Weapon Type",
		ItemNamePlural = "Ranged Weapon Types",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IRangedWeaponType>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IRangedWeaponType>(actor) { EditingItem = (IRangedWeaponType)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IRangedWeaponType>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.RangedWeaponTypes.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.RangedWeaponTypes.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.RangedWeaponTypes.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IRangedWeaponType)item),
		CastToType = typeof(IRangedWeaponType),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your ranged weapon type.");
				return;
			}

			var name = input.PopSpeech().TitleCase();
			if (actor.Gameworld.RangedWeaponTypes.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already a ranged weapon type called {name.ColourName()}. Names must be unique.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send($"Which ranged weapon type do you want this to be? The options are {Enum.GetValues<RangedWeaponType>().ListToColouredString()}.");
				return;
			}

			if (!input.PopSpeech().TryParseEnum<RangedWeaponType>(out var type))
			{
				actor.OutputHandler.Send($"The text {input.Last.ColourCommand()} is not a valid ranged weapon type. The options are {Enum.GetValues<RangedWeaponType>().ListToColouredString()}.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which skill do you want to use to operate that weapon?");
				return;
			}

			var skill = actor.Gameworld.Traits.GetByIdOrName(input.SafeRemainingArgument);
			if (skill is null)
			{
				actor.OutputHandler.Send($"There is no skill or trait identified by the text {input.SafeRemainingArgument.ColourCommand()}.");
				return;
			}

			var weapon = new RangedWeaponTypeDefinition(actor.Gameworld, name, type, skill);
			actor.Gameworld.Add(weapon);
			actor.RemoveAllEffects<BuilderEditingEffect<IRangedWeaponType>>();
			actor.AddEffect(new BuilderEditingEffect<IRangedWeaponType>(actor) { EditingItem = weapon });
			actor.OutputHandler.Send($"You create a new ranged weapon type called {name.ColourName()}, which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which ranged weapon type would you like to clone?");
				return;
			}

			var target = actor.Gameworld.RangedWeaponTypes.GetByIdOrName(input.PopSpeech());
			if (target is null)
			{
				actor.OutputHandler.Send("There is no such ranged weapon type to clone.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("What name would you like to give to the cloned ranged weapon type?");
				return;
			}

			var name = input.SafeRemainingArgument.TitleCase();
			if (actor.Gameworld.RangedWeaponTypes.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already a ranged weapon type called {name.ColourName()}. Names must be unique.");
				return;
			}

			var clone = target.Clone(name);
			actor.Gameworld.Add(clone);
			actor.RemoveAllEffects<BuilderEditingEffect<IRangedWeaponType>>();
			actor.AddEffect(new BuilderEditingEffect<IRangedWeaponType>(actor) { EditingItem = clone });
			actor.OutputHandler.Send($"You create a cloned ranged weapon type from {target.Name.ColourName()} called {name.ColourName()}, which you are now editing.");
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Class",
			"Type",
			"Ammunition",
			"Fire Trait",
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IRangedWeaponType>()
														  select new List<string>
														  {
															  proto.Id.ToString("N0", character),
															  proto.Name,
															  proto.Classification.Describe(),
															  proto.RangedWeaponType.DescribeEnum(),
															  proto.SpecificAmmunitionGrade,
															  proto.FireTrait.Name
														  },

		CustomSearch = (protos, keyword, gameworld) => protos,
		GetEditHeader = item => $"Ranged Weapon Type #{item.Id:N0} ({item.Name})",
		DefaultCommandHelp = CombatBuilderModule.RangedWeaponTypeHelp
	};

	public static EditableItemHelper WeaponTypeHelper { get; } = new()
	{
		ItemName = "Weapon Type",
		ItemNamePlural = "Weapon Types",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IWeaponType>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IWeaponType>(actor) { EditingItem = (IWeaponType)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IWeaponType>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.WeaponTypes.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.WeaponTypes.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.WeaponTypes.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IWeaponType)item),
		CastToType = typeof(IWeaponType),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your weapon type.");
				return;
			}

			var name = input.PopSpeech().TitleCase();
			if (actor.Gameworld.WeaponTypes.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already an weapon type called {name.ColourName()}. Names must be unique.");
				return;
			}

			var weapon = new WeaponType(actor.Gameworld, name);
			actor.Gameworld.Add(weapon);
			actor.RemoveAllEffects<BuilderEditingEffect<IWeaponType>>();
			actor.AddEffect(new BuilderEditingEffect<IWeaponType>(actor) { EditingItem = weapon });

			using (new FMDB())
			{
				var dbcomp = new Models.GameItemComponentProto
				{
					Id = actor.Gameworld.ItemComponentProtos.NextID(),
					RevisionNumber = 0,
					Name = $"Weapon_{name.CollapseString()}",
					Description = $"Turns an item into weapon type \"{name}\"",
					EditableItem = new Models.EditableItem
					{
						BuilderAccountId = actor.Account.Id,
						RevisionStatus = (int)RevisionStatus.Current,
						RevisionNumber = 0,
						BuilderDate = DateTime.UtcNow,
						BuilderComment = "System generated"
					},
					Type = "MeleeWeapon",
					Definition = $"<Definition><WeaponType>{weapon.Id}</WeaponType></Definition>"
				};
				FMDB.Context.GameItemComponentProtos.Add(dbcomp);
				FMDB.Context.SaveChanges();
				var comp = actor.Gameworld.GameItemComponentManager.GetProto(dbcomp, actor.Gameworld);
				actor.Gameworld.Add(comp);
				actor.OutputHandler.Send(
					$"You create a new weapon type called {name.ColourName()}, which you are now editing.\nAlso created a matching item component to add to items called {comp.Name.ColourName()} with Id #{comp.Id.ToString("N0", actor)}.");
			}
		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which weapon type would you like to clone?");
				return;
			}

			var target = actor.Gameworld.WeaponTypes.GetByIdOrName(input.PopSpeech());
			if (target is null)
			{
				actor.OutputHandler.Send("There is no such weapon type to clone.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("What name would you like to give to the cloned weapon type?");
				return;
			}

			var name = input.SafeRemainingArgument.TitleCase();
			if (actor.Gameworld.WeaponTypes.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already an weapon type called {name.ColourName()}. Names must be unique.");
				return;
			}

			var clone = target.Clone(name);
			actor.Gameworld.Add(clone);
			actor.RemoveAllEffects<BuilderEditingEffect<IWeaponType>>();
			actor.AddEffect(new BuilderEditingEffect<IWeaponType>(actor) { EditingItem = clone });
			using (new FMDB())
			{
				var dbcomp = new Models.GameItemComponentProto
				{
					Id = actor.Gameworld.ItemComponentProtos.NextID(),
					RevisionNumber = 0,
					Name = $"Weapon_{clone.Name.CollapseString()}",
					Description = $"Turns an item into weapon type \"{clone.Name}\"",
					EditableItem = new Models.EditableItem
					{
						BuilderAccountId = actor.Account.Id,
						RevisionStatus = (int)RevisionStatus.Current,
						RevisionNumber = 0,
						BuilderDate = DateTime.UtcNow,
						BuilderComment = "System generated"
					},
					Type = "MeleeWeapon",
					Definition = $"<Definition><WeaponType>{clone.Id}</WeaponType></Definition>"
				};
				FMDB.Context.GameItemComponentProtos.Add(dbcomp);
				FMDB.Context.SaveChanges();
				var comp = actor.Gameworld.GameItemComponentManager.GetProto(dbcomp, actor.Gameworld);
				actor.Gameworld.Add(comp);
				actor.OutputHandler.Send(
					$"You create a cloned weapon type from {target.Name.ColourName()} called {name.ColourName()}, which you are now editing.\nAlso created a matching item component to add to items called {comp.Name.ColourName()} with Id #{comp.Id.ToString("N0", actor)}.");
			}
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Class",
			"Attack",
			"Parry",
			"Parry Bonus",
			"Reach",
			"# Attacks"
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IWeaponType>()
														  select new List<string>
														  {
															  proto.Id.ToString("N0", character),
															  proto.Name,
															  proto.Classification.Describe(),
															  proto.AttackTrait?.Name ?? "None",
															  proto.ParryTrait?.Name ?? "None",
															  proto.ParryBonus.ToBonusString(),
															  proto.Reach.ToString("N0", character),
															  proto.Attacks.Count().ToString("N0", character)
														  },

		CustomSearch = (protos, keyword, gameworld) => protos,
		GetEditHeader = item => $"Weapon Type #{item.Id:N0} ({item.Name})",
		DefaultCommandHelp = CombatBuilderModule.WeaponTypeHelp
	};

	public static EditableItemHelper AmmunitionTypeHelper { get; } = new()
	{
		ItemName = "Ammunition Type",
		ItemNamePlural = "Ammunition Types",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IAmmunitionType>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IAmmunitionType>(actor) { EditingItem = (IAmmunitionType)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IAmmunitionType>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.AmmunitionTypes.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.AmmunitionTypes.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.AmmunitionTypes.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IAmmunitionType)item),
		CastToType = typeof(IAmmunitionType),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your ammunition type.");
				return;
			}

			var name = input.PopSpeech().TitleCase();
			if (actor.Gameworld.AmmunitionTypes.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already an ammunition type called {name.ColourName()}. Names must be unique.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send(
					$"Which ranged weapon type do you want this ammunition to be set for by default? The valid options are {Enum.GetValues<RangedWeaponType>().Select(x => x.Describe().ColourName()).ListToString()}.");
				return;
			}

			if (!input.SafeRemainingArgument.TryParseEnum<RangedWeaponType>(out var value))
			{
				actor.OutputHandler.Send(
					$"This is not a valid ranged weapon type. The valid values are {Enum.GetValues<RangedWeaponType>().Select(x => x.Describe().ColourName()).ListToString()}.");
				return;
			}

			var ammo = new AmmunitionType(actor.Gameworld, name, value);
			actor.Gameworld.Add(ammo);
			actor.RemoveAllEffects<BuilderEditingEffect<IAmmunitionType>>();
			actor.AddEffect(new BuilderEditingEffect<IAmmunitionType>(actor) { EditingItem = ammo });

			using (new FMDB())
			{
				var dbcomp = new Models.GameItemComponentProto
				{
					Id = actor.Gameworld.ItemComponentProtos.NextID(),
					RevisionNumber = 0,
					Name = $"Ammo_{name.CollapseString()}",
					Description = $"Turns an item into ammo type \"{name}\"",
					EditableItem = new Models.EditableItem
					{
						BuilderAccountId = actor.Account.Id,
						RevisionStatus = (int)RevisionStatus.Current,
						RevisionNumber = 0,
						BuilderDate = DateTime.UtcNow,
						BuilderComment = "System generated"
					},
					Type = "Ammunition",
					Definition = $"<Definition><AmmoType>{ammo.Id}</AmmoType></Definition>"
				};
				FMDB.Context.GameItemComponentProtos.Add(dbcomp);
				FMDB.Context.SaveChanges();
				var comp = actor.Gameworld.GameItemComponentManager.GetProto(dbcomp, actor.Gameworld);
				actor.Gameworld.Add(comp);
				actor.OutputHandler.Send(
					$"You create a new ammunition type called {name.ColourName()} for {value.Describe().ColourValue()} weapons, which you are now editing.\nAlso created a matching item component to add to items called {comp.Name.ColourName()} with Id #{comp.Id.ToString("N0", actor)}.");
			}
		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which ammunition type would you like to clone?");
				return;
			}

			var target = actor.Gameworld.AmmunitionTypes.GetByIdOrName(input.PopSpeech());
			if (target is null)
			{
				actor.OutputHandler.Send("There is no such ammunition type to clone.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("What name would you like to give to the cloned ammunition type?");
				return;
			}

			var name = input.SafeRemainingArgument.TitleCase();
			if (actor.Gameworld.AmmunitionTypes.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already an ammunition type called {name.ColourName()}. Names must be unique.");
				return;
			}

			var clone = target.Clone(name);
			actor.Gameworld.Add(clone);
			actor.RemoveAllEffects<BuilderEditingEffect<IAmmunitionType>>();
			actor.AddEffect(new BuilderEditingEffect<IAmmunitionType>(actor) { EditingItem = clone });
			using (new FMDB())
			{
				var dbcomp = new Models.GameItemComponentProto
				{
					Id = actor.Gameworld.ItemComponentProtos.NextID(),
					RevisionNumber = 0,
					Name = $"Ammo_{clone.Name.CollapseString()}",
					Description = $"Turns an item into ammo type \"{clone.Name}\"",
					EditableItem = new Models.EditableItem
					{
						BuilderAccountId = actor.Account.Id,
						RevisionStatus = (int)RevisionStatus.Current,
						RevisionNumber = 0,
						BuilderDate = DateTime.UtcNow,
						BuilderComment = "System generated"
					},
					Type = "Ammunition",
					Definition = $"<Definition><AmmoType>{clone.Id}</AmmoType></Definition>"
				};
				FMDB.Context.GameItemComponentProtos.Add(dbcomp);
				FMDB.Context.SaveChanges();
				var comp = actor.Gameworld.GameItemComponentManager.GetProto(dbcomp, actor.Gameworld);
				actor.Gameworld.Add(comp);
				actor.OutputHandler.Send(
					$"You create a cloned ammunition type from {target.Name.ColourName()} called {name.ColourName()}, which you are now editing.\nAlso created a matching item component to add to items called {comp.Name.ColourName()} with Id #{comp.Id.ToString("N0", actor)}.");
			}
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Grade",
			"Types",
			"Accuracy",
			"Damage Type",
			"Damage"
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IAmmunitionType>()
														  select new List<string>
														  {
															  proto.Id.ToString("N0", character),
															  proto.Name,
															  proto.SpecificType,
															  proto.RangedWeaponTypes.Select(x => x.Describe())
																   .ListToCommaSeparatedValues(", "),
															  proto.BaseAccuracy.ToBonusString(),
															  proto.DamageProfile.DamageType.Describe(),
															  proto.DamageProfile.DamageExpression.Formula
																   .OriginalExpression
														  },

		CustomSearch = (protos, keyword, gameworld) => protos,
		GetEditHeader = item => $"Ammunition Type #{item.Id:N0} ({item.Name})",
		DefaultCommandHelp = CombatBuilderModule.AmmunitionHelp
	};

	public static EditableItemHelper ArmourTypeHelper { get; } = new()
	{
		ItemName = "Armour Type",
		ItemNamePlural = "Armour Types",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IArmourType>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IArmourType>(actor) { EditingItem = (IArmourType)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IArmourType>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.ArmourTypes.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.ArmourTypes.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.ArmourTypes.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IArmourType)item),
		CastToType = typeof(IArmourType),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your armour type.");
				return;
			}

			var name = input.PopSpeech().TitleCase();
			if (actor.Gameworld.ArmourTypes.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already an armour type called {name.ColourName()}. Names must be unique.");
				return;
			}

			var armour = new ArmourType(actor.Gameworld, name);
			actor.Gameworld.Add(armour);
			actor.RemoveAllEffects<BuilderEditingEffect<IArmourType>>();
			actor.AddEffect(new BuilderEditingEffect<IArmourType>(actor) { EditingItem = armour });

			using (new FMDB())
			{
				var dbcomp = new Models.GameItemComponentProto
				{
					Id = actor.Gameworld.ItemComponentProtos.NextID(),
					RevisionNumber = 0,
					Name = $"Armour_{name.CollapseString()}",
					Description = $"Turns an item into armour type \"{name}\"",
					EditableItem = new Models.EditableItem
					{
						BuilderAccountId = actor.Account.Id,
						RevisionStatus = (int)RevisionStatus.Current,
						RevisionNumber = 0,
						BuilderDate = DateTime.UtcNow,
						BuilderComment = "System generated"
					},
					Type = "Armour",
					Definition = $"<Definition><ArmourType>{armour.Id}</ArmourType></Definition>"
				};
				FMDB.Context.GameItemComponentProtos.Add(dbcomp);
				FMDB.Context.SaveChanges();
				var comp = actor.Gameworld.GameItemComponentManager.GetProto(dbcomp, actor.Gameworld);
				actor.Gameworld.Add(comp);
				actor.OutputHandler.Send(
					$"You create a new armour type called {name.ColourName()}, which you are now editing.\nAlso created a matching item component to add to items called {comp.Name.ColourName()} with Id #{comp.Id.ToString("N0", actor)}.");
			}
		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which armour type would you like to clone?");
				return;
			}

			var target = actor.Gameworld.ArmourTypes.GetByIdOrName(input.PopSpeech());
			if (target is null)
			{
				actor.OutputHandler.Send("There is no such armour type to clone.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("What name would you like to give to the cloned armour type?");
				return;
			}

			var name = input.SafeRemainingArgument.TitleCase();
			if (actor.Gameworld.ArmourTypes.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already an armour type called {name.ColourName()}. Names must be unique.");
				return;
			}

			var clone = target.Clone(name);
			actor.Gameworld.Add(clone);
			actor.RemoveAllEffects<BuilderEditingEffect<IArmourType>>();
			actor.AddEffect(new BuilderEditingEffect<IArmourType>(actor) { EditingItem = clone });
			using (new FMDB())
			{
				var dbcomp = new Models.GameItemComponentProto
				{
					Id = actor.Gameworld.ItemComponentProtos.NextID(),
					RevisionNumber = 0,
					Name = $"Armour_{name.CollapseString()}",
					Description = $"Turns an item into armour type \"{name}\"",
					EditableItem = new Models.EditableItem
					{
						BuilderAccountId = actor.Account.Id,
						RevisionStatus = (int)RevisionStatus.Current,
						RevisionNumber = 0,
						BuilderDate = DateTime.UtcNow,
						BuilderComment = "System generated"
					},
					Type = "Armour",
					Definition = $"<Definition><ArmourType>{clone.Id}</ArmourType></Definition>"
				};
				FMDB.Context.GameItemComponentProtos.Add(dbcomp);
				FMDB.Context.SaveChanges();
				var comp = actor.Gameworld.GameItemComponentManager.GetProto(dbcomp, actor.Gameworld);
				actor.Gameworld.Add(comp);
				actor.OutputHandler.Send(
					$"You create a cloned armour type from {target.Name.ColourName()} called {name.ColourName()}, which you are now editing.\nAlso created a matching item component to add to items called {comp.Name.ColourName()} with Id #{comp.Id.ToString("N0", actor)}.");
			}
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Min Pen",
			"Penalty",
			"Stack Penalty"
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IArmourType>()
														  select new List<string>
														  {
															  proto.Id.ToString("N0", character),
															  proto.Name,
															  proto.MinimumPenetrationDegree.DescribeColour(),
															  proto.BaseDifficultyBonus.ToBonusString(character),
															  proto.StackedDifficultyBonus.ToBonusString(character)
														  },

		CustomSearch = (protos, keyword, gameworld) => protos,
		GetEditHeader = item => $"Armour Type #{item.Id:N0} ({item.Name})",
		DefaultCommandHelp = CombatBuilderModule.ArmourTypeHelp
	};

	public static EditableItemHelper AttributeHelper { get; } = new()
	{
		ItemName = "Attribute",
		ItemNamePlural = "Attributes",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IAttributeDefinition>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IAttributeDefinition>(actor) { EditingItem = (IAttributeDefinition)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IAttributeDefinition>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.Traits.OfType<IAttributeDefinition>().ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.Traits.OfType<IAttributeDefinition>().Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.Traits.OfType<IAttributeDefinition>().GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IAttributeDefinition)item),
		CastToType = typeof(IAttributeDefinition),
		EditableNewAction = (actor, input) =>
		{
			var simple = true;
			switch (input.PopForSwitch())
			{
				case "simple":
					break;
				case "derived":
					simple = false;
					break;
				default:
					actor.OutputHandler.Send("You must specify #3simple#0 or #3derived#0 for your attribute type.");
					return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your attribute.");
				return;
			}

			var name = input.PopSpeech().TitleCase();
			if (actor.Gameworld.Traits.OfType<IAttributeDefinition>().Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already an attribute called {name.ColourName()}. Names must be unique.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify an alias for your attribute.");
				return;
			}

			var alias = input.PopSpeech().TitleCase();
			if (actor.Gameworld.Traits.OfType<IAttributeDefinition>().Any(x => x.Alias.EqualTo(alias)))
			{
				actor.OutputHandler.Send(
					$"There is already an attribute with an alias of {alias.ColourName()}. Aliases must be unique.");
				return;
			}

			var attribute = simple ? (IAttributeDefinition)new AttributeDefinition(actor.Gameworld, name, alias) : new DerivedAttributeDefinition(actor.Gameworld, name, alias);
			actor.Gameworld.Add(attribute);
			actor.RemoveAllEffects<BuilderEditingEffect<IAttributeDefinition>>();
			actor.AddEffect(new BuilderEditingEffect<IAttributeDefinition>(actor) { EditingItem = attribute });
			actor.OutputHandler.Send($"You create a new attribute called {name.ColourName()} ({alias.ColourValue()}), which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which attribute would you like to clone?");
				return;
			}

			var target = actor.Gameworld.Traits.OfType<IAttributeDefinition>().GetByIdOrName(input.PopSpeech());
			if (target is null)
			{
				actor.OutputHandler.Send("There is no such attribute to clone.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your attribute.");
				return;
			}

			var name = input.PopSpeech().TitleCase();
			if (actor.Gameworld.Traits.OfType<IAttributeDefinition>().Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already an attribute called {name.ColourName()}. Names must be unique.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify an alias for your attribute.");
				return;
			}

			var alias = input.PopSpeech().TitleCase();
			if (actor.Gameworld.Traits.OfType<IAttributeDefinition>().Any(x => x.Alias.EqualTo(alias)))
			{
				actor.OutputHandler.Send(
					$"There is already an attribute with an alias of {alias.ColourName()}. Aliases must be unique.");
				return;
			}

			var clone = target.Clone(name, alias);
			actor.Gameworld.Add(clone);
			actor.RemoveAllEffects<BuilderEditingEffect<IAttributeDefinition>>();
			actor.AddEffect(new BuilderEditingEffect<IAttributeDefinition>(actor) { EditingItem = clone });
			actor.OutputHandler.Send($"You clone the attribute {target.Name.ColourName()} into a new attribute called {name.ColourName()} ({alias.ColourValue()}), which you are now editing.");
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Alias",
			"Group",
			"Sub",
			"Score",
			"Attributes"
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IAttributeDefinition>()
		                                                  select new List<string>
		                                                  {
			                                                  proto.Id.ToString("N0", character),
			                                                  proto.Name,
															  proto.Alias,
															  proto.Group,
															  proto.DisplayAsSubAttribute.ToColouredString(),
															  proto.ShowInScoreCommand.ToColouredString(),
															  proto.ShowInAttributeCommand.ToColouredString()
		                                                  },

		CustomSearch = (protos, keyword, gameworld) => protos,
		GetEditHeader = item => $"Attribute #{item.Id:N0} ({item.Name})",
		DefaultCommandHelp = BuilderModule.AttributeCommandHelp
	};

	public static EditableItemHelper NPCSpawnerHelper { get; } = new()
	{
		ItemName = "NPC Spawner",
		ItemNamePlural = "NPC Spawners",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<INPCSpawner>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<INPCSpawner>(actor) { EditingItem = (INPCSpawner)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<INPCSpawner>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.NPCSpawners.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.NPCSpawners.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.NPCSpawners.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((INPCSpawner)item),
		CastToType = typeof(INPCSpawner),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your NPC Spawner.");
				return;
			}

			var name = input.SafeRemainingArgument.TitleCase();
			if (actor.Gameworld.NPCSpawners.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already an NPC Spawner called {name.ColourName()}. Names must be unique.");
				return;
			}

			var spawner = new NPCSpawner(actor.Gameworld, name);
			actor.Gameworld.Add(spawner);
			actor.RemoveAllEffects<BuilderEditingEffect<INPCSpawner>>();
			actor.AddEffect(new BuilderEditingEffect<INPCSpawner>(actor) { EditingItem = spawner });
			actor.OutputHandler.Send(
				$"You create a new NPC Spawner called {name.ColourName()}, which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which NPC Spawner would you like to clone?");
				return;
			}

			var target = actor.Gameworld.NPCSpawners.GetByIdOrName(input.SafeRemainingArgument);
			if (target is not NPCSpawner npcTarget)
			{
				actor.OutputHandler.Send("There is no such NPC Spawner to clone.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("What name would you like to give to the cloned NPC Spawner?");
				return;
			}

			var name = input.SafeRemainingArgument.TitleCase();
			if (actor.Gameworld.NPCSpawners.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already an NPC Spawner called {name.ColourName()}. Names must be unique.");
				return;
			}

			var clone = new NPCSpawner(npcTarget, name);
			actor.Gameworld.Add(clone);
			actor.RemoveAllEffects<BuilderEditingEffect<INPCSpawner>>();
			actor.AddEffect(new BuilderEditingEffect<INPCSpawner>(actor) { EditingItem = clone });
			actor.OutputHandler.Send(
				$"You create a cloned NPC Spawner from {target.Name.ColourName()} called {clone.Name.ColourName()}, which you are now editing.");
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Active"
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<INPCSpawner>()
														  select new List<string>
														  {
															  proto.Id.ToString("N0", character),
															  proto.Name,
															  proto.IsActive.ToColouredString()
														  },

		CustomSearch = (protos, keyword, gameworld) => protos,
		GetEditHeader = item => $"NPC Spawner #{item.Id:N0} ({item.Name})",
		DefaultCommandHelp = NPCBuilderModule.NPCSpawnerHelp
	};

	public static EditableItemHelper AreaBuilder { get; } = new()
	{
		ItemName = "Autobuilder Area",
		ItemNamePlural = "Autobuilder Areas",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IAutobuilderArea>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IAutobuilderArea>(actor) { EditingItem = (IAutobuilderArea)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IAutobuilderArea>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.AutobuilderAreas.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.AutobuilderAreas.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.AutobuilderAreas.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IAutobuilderArea)item),
		CastToType = typeof(IAutobuilderArea),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your new area template.");
				return;
			}

			var name = input.PopSpeech().TitleCase();

			if (actor.Gameworld.AutobuilderAreas.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					"There is already an autobuilder area template with that name. Names must be unique.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send(
					$"What type of autobuilder area template would you like to create? The valid options are {AutobuilderFactory.AreaLoaderTypes.Select(x => x.ColourName()).ListToString()}.");
				return;
			}

			var item = AutobuilderFactory.LoadAreaFromBuilder(actor.Gameworld, input.SafeRemainingArgument, name);
			if (item == null)
			{
				actor.OutputHandler.Send(
					$"That is not a valid area template type. The valid options are {AutobuilderFactory.AreaLoaderTypes.Select(x => x.ColourName()).ListToString()}.");
				return;
			}

			actor.Gameworld.Add(item);
			actor.RemoveAllEffects<BuilderEditingEffect<IAutobuilderArea>>();
			actor.AddEffect(new BuilderEditingEffect<IAutobuilderArea>(actor) { EditingItem = item });
			actor.OutputHandler.Send(
				$"You create a new autobuilder area template called {name.ColourValue()}, which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which autobuilder area template do you want to clone?");
				return;
			}

			var template = actor.Gameworld.AutobuilderAreas.GetByIdOrName(input.PopSpeech());
			if (template == null)
			{
				actor.OutputHandler.Send("There is no such autobuilder area template.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your new autobuilder area template.");
				return;
			}

			var name = input.SafeRemainingArgument;
			if (actor.Gameworld.AutobuilderAreas.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already an autobuilder area template with that name. Names must be unique.");
				return;
			}

			var clone = template.Clone(name);
			actor.Gameworld.Add(clone);
			actor.RemoveAllEffects<BuilderEditingEffect<IAutobuilderArea>>();
			actor.AddEffect(new BuilderEditingEffect<IAutobuilderArea>(actor) { EditingItem = clone });
			actor.OutputHandler.Send(
				$"You clone the autobuilder area template {template.Name.ColourValue()} to a new template called {clone.Name.ColourValue()}, which you are now editing.");
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Blurb"
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IAutobuilderArea>()
														  select new List<string>
														  {
															  proto.Id.ToString("N0", character),
															  proto.Name,
															  proto.ShowCommandByLine
														  },

		CustomSearch = (protos, keyword, gameworld) => protos,

		DefaultCommandHelp = RoomBuilderModule.AutoAreaHelpText,

		GetEditHeader = item => $"Autobuilder Area Template #{item.Id:N0} ({item.Name})"
	};

	public static EditableItemHelper RoomBuilder { get; } = new()
	{
		ItemName = "Autobuilder Room",
		ItemNamePlural = "Autobuilder Rooms",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IAutobuilderRoom>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IAutobuilderRoom>(actor) { EditingItem = (IAutobuilderRoom)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IAutobuilderRoom>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.AutobuilderRooms.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.AutobuilderRooms.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.AutobuilderRooms.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IAutobuilderRoom)item),
		CastToType = typeof(IAutobuilderRoom),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your new room template.");
				return;
			}

			var name = input.PopSpeech().TitleCase();

			if (actor.Gameworld.AutobuilderRooms.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					"There is already an autobuilder room template with that name. Names must be unique.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send(
					$"What type of autobuilder room template would you like to create? The valid options are {AutobuilderFactory.RoomLoaderTypes.Select(x => x.ColourName()).ListToString()}.");
				return;
			}

			var item = AutobuilderFactory.LoadRoomFromBuilder(actor.Gameworld, input.SafeRemainingArgument, name);
			if (item == null)
			{
				actor.OutputHandler.Send(
					$"That is not a valid room template type. The valid options are {AutobuilderFactory.RoomLoaderTypes.Select(x => x.ColourName()).ListToString()}.");
				return;
			}

			actor.Gameworld.Add(item);
			actor.RemoveAllEffects<BuilderEditingEffect<IAutobuilderRoom>>();
			actor.AddEffect(new BuilderEditingEffect<IAutobuilderRoom>(actor) { EditingItem = item });
			actor.OutputHandler.Send(
				$"You create a new autobuilder room template called {name.ColourValue()}, which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which autobuilder room template do you want to clone?");
				return;
			}

			var template = actor.Gameworld.AutobuilderRooms.GetByIdOrName(input.PopSpeech());
			if (template == null)
			{
				actor.OutputHandler.Send("There is no such autobuilder room template.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your new autobuilder room template.");
				return;
			}

			var name = input.SafeRemainingArgument;
			if (actor.Gameworld.AutobuilderRooms.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already an autobuilder room template with that name. Names must be unique.");
				return;
			}

			var clone = template.Clone(name);
			actor.Gameworld.Add(clone);
			actor.RemoveAllEffects<BuilderEditingEffect<IAutobuilderRoom>>();
			actor.AddEffect(new BuilderEditingEffect<IAutobuilderRoom>(actor) { EditingItem = clone });
			actor.OutputHandler.Send(
				$"You clone the autobuilder room template {template.Name.ColourValue()} to a new template called {clone.Name.ColourValue()}, which you are now editing.");
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Blurb"
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IAutobuilderRoom>()
														  select new List<string>
														  {
															  proto.Id.ToString("N0", character),
															  proto.Name,
															  proto.ShowCommandByline
														  },

		CustomSearch = (protos, keyword, gameworld) => protos,

		DefaultCommandHelp = RoomBuilderModule.AutoRoomHelpText,

		GetEditHeader = item => $"Autobuilder Room Template #{item.Id:N0} ({item.Name})"
	};

	public static EditableItemHelper DreamHelper { get; } = new()
	{
		ItemName = "Dream",
		ItemNamePlural = "Dreams",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IDream>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IDream>(actor) { EditingItem = (IDream)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IDream>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.Dreams.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.Dreams.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.Dreams.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IDream)item),
		CastToType = typeof(IDream),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your new dream.");
				return;
			}

			var name = input.PopSpeech().TitleCase();

			if (actor.Gameworld.Dreams.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					"There is already an dream with that name. Names must be unique.");
				return;
			}

			var item = new Dream(name, actor.Gameworld);

			actor.Gameworld.Add(item);
			actor.RemoveAllEffects<BuilderEditingEffect<IDream>>();
			actor.AddEffect(new BuilderEditingEffect<IDream>(actor) { EditingItem = item });
			actor.OutputHandler.Send(
				$"You create a new dream called {name.ColourValue()}, which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which dream do you want to clone?");
				return;
			}

			var template = actor.Gameworld.Dreams.GetByIdOrName(input.PopSpeech());
			if (template == null)
			{
				actor.OutputHandler.Send("There is no such dream.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your new dream.");
				return;
			}

			var name = input.SafeRemainingArgument;
			if (actor.Gameworld.Dreams.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send($"There is already a dream with that name. Names must be unique.");
				return;
			}

			var clone = template.Clone(name);
			actor.Gameworld.Add(clone);
			actor.RemoveAllEffects<BuilderEditingEffect<IDream>>();
			actor.AddEffect(new BuilderEditingEffect<IDream>(actor) { EditingItem = clone });
			actor.OutputHandler.Send(
				$"You clone the dream {template.Name.ColourValue()} to a new dream called {clone.Name.ColourValue()}, which you are now editing.");
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"CanDream",
			"Length",
			"Once"
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IDream>()
														  select new List<string>
														  {
															  proto.Id.ToString("N0", character),
															  proto.Name,
															  proto.CanDreamProg?.MXPClickableFunctionName() ??
															  "Manual Only",
															  TimeSpan.FromSeconds(
																		  proto.DreamStages.Sum(x => x.WaitSeconds))
																	  .Describe(character),
															  proto.OnceOnly.ToColouredString()
														  },

		CustomSearch = (protos, keyword, gameworld) => protos,

		DefaultCommandHelp = BuilderModule.DreamHelpText,

		GetEditHeader = item => $"Dream #{item.Id:N0} ({item.Name})"
	};

	public static EditableItemHelper AIHelper { get; } = new()
	{
		ItemName = "AI",
		ItemNamePlural = "AIs",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IArtificialIntelligence>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IArtificialIntelligence>(actor) { EditingItem = (IArtificialIntelligence)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IArtificialIntelligence>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.AIs.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.AIs.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.AIs.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IArtificialIntelligence)item),
		CastToType = typeof(IArtificialIntelligence),
		EditableNewAction = (actor, input) =>
		{
			var newItem = ArtificialIntelligenceBase.LoadFromBuilderInput(actor, input);
			if (newItem is null)
			{
				return;
			}

			actor.Gameworld.Add(newItem);
			actor.RemoveAllEffects<BuilderEditingEffect<IArtificialIntelligence>>();
			actor.AddEffect(new BuilderEditingEffect<IArtificialIntelligence>(actor) { EditingItem = newItem });
			actor.OutputHandler.Send($"You create a new artificial intelligence of type {newItem.AIType.ColourCommand()} called {newItem.Name.ColourName()} with ID #{newItem.Id.ToString("N0", actor).ColourValue()}, which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which AI do you want to clone?");
				return;
			}

			var ai = actor.Gameworld.AIs.GetByIdOrName(input.PopSpeech());
			if (ai == null)
			{
				actor.OutputHandler.Send("There is no such AI.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your new cloned AI.");
				return;
			}

			var name = input.SafeRemainingArgument.TitleCase();
			if (actor.Gameworld.AIs.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already an AI with that name. Names must be unique.");
				return;
			}

			var newAI = ai.Clone(name);
			actor.Gameworld.Add(newAI);
			actor.RemoveAllEffects<BuilderEditingEffect<IArtificialIntelligence>>();
			actor.AddEffect(new BuilderEditingEffect<IArtificialIntelligence>(actor) { EditingItem = newAI });
			actor.OutputHandler.Send(
				$"You clone the AI {ai.Name.ColourName()} into a new AI called {newAI.Name.ColourName()} with ID #{newAI.Id.ToString("N0", actor).ColourValue()}, which you are now editing.");
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Type",
			"Templates",
			"Instances"
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IArtificialIntelligence>()
														  let templates = proto.Gameworld.NpcTemplates.GetAllApprovedOrMostRecent().Count(x => x.ArtificialIntelligences.Contains(proto))
														  let instances = proto.Gameworld.NPCs.OfType<INPC>().Count(x => x.AIs.Contains(proto))
														  select new List<string>
														  {
															  proto.Id.ToString("N0", character),
															  proto.Name,
															  proto.AIType,
															  templates.ToString("N0", character).MXPSend($"npc list *{proto.Id}", "View the specific NPC Templates"),
															  instances.ToString("N0", character).MXPSend($"ai npclist {proto.Id}", "View the specific instances")
														  },

		CustomSearch = (protos, keyword, gameworld) => protos,

		DefaultCommandHelp = NPCBuilderModule.AIHelp,

		GetEditHeader = item => $"Artificial Intelligence #{item.Id:N0} ({item.Name})"
	};

	public static EditableItemHelper ChargenResourceHelper { get; } = new()
	{
		ItemName = "Chargen Resource",
		ItemNamePlural = "Chargen Resources",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IChargenResource>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IChargenResource>(actor) { EditingItem = (IChargenResource)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IChargenResource>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.ChargenResources.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.ChargenResources.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.ChargenResources.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IChargenResource)item),
		CastToType = typeof(IChargenResource),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send($"Which type of chargen resource do you want to make? The valid choices are {ChargenResourceBase.BuilderTypesAvailable.ListToColouredString()}.");
				return;
			}

			var type = input.PopSpeech().ToLowerInvariant();
			if (!ChargenResourceBase.BuilderTypesAvailable.Contains(type))
			{
				actor.OutputHandler.Send($"The text {type.ColourCommand()} is not a valid chargen resource type. The valid choices are {ChargenResourceBase.BuilderTypesAvailable.ListToColouredString()}.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("What name do you want to give to this resource?");
				return;
			}

			var name = input.PopSpeech().TitleCase();
			if (actor.Gameworld.ChargenResources.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send($"There is already a chargen resource called {name.ColourValue()}. Names must be unique.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("What plural name do you want to give to the resource?");
				return;
			}

			var plural = input.PopSpeech().TitleCase();

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("What alias do you want to give to the resource?");
				return;
			}

			var alias = input.PopSpeech().ToLowerInvariant();
			if (actor.Gameworld.ChargenResources.Any(x => x.Alias.EqualTo(alias)))
			{
				actor.OutputHandler.Send($"There is already a chargen resource with an alias of {alias.ColourValue()}. Aliases must be unique.");
				return;
			}

			var newItem = ChargenResourceBase.LoadFromBuilderInput(actor.Gameworld, type, name, plural, alias);
			if (newItem is null)
			{
				return;
			}

			actor.Gameworld.Add(newItem);
			actor.RemoveAllEffects<BuilderEditingEffect<IChargenResource>>();
			actor.AddEffect(new BuilderEditingEffect<IChargenResource>(actor) { EditingItem = newItem });
			actor.OutputHandler.Send($"You create a new chargen resource of type {type.ColourCommand()} called {newItem.Name.ColourName()} with ID #{newItem.Id.ToString("N0", actor).ColourValue()}, which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			actor.OutputHandler.Send("Cloning is not supported for chargen resources.");
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Plural",
			"Alias",
			"Type",
			"Permission",
			"Time Between"
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IChargenResource>()
														  select new List<string>
														  {
															  proto.Id.ToString("N0", character),
															  proto.Name,
															  proto.PluralName,
															  proto.Alias,
															  proto.TypeName,
															  proto.PermissionLevelRequiredToAward.DescribeEnum(),
															  proto.MinimumTimeBetweenAwards.DescribePreciseBrief(character)
														  },

		CustomSearch = (protos, keyword, gameworld) => protos,

		DefaultCommandHelp = ChargenModule.ChargenResourceHelp,

		GetEditHeader = item => $"Chargen Resource #{item.Id:N0} ({item.Name})"
	};

	public static EditableItemHelper ImproverHelper = new()
	{
		ItemName = "Improver",
		ItemNamePlural = "Improvers",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IImprovementModel>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IImprovementModel>(actor) { EditingItem = (IImprovementModel)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IImprovementModel>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.ImprovementModels.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.ImprovementModels.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.ImprovementModels.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IImprovementModel)item),
		CastToType = typeof(IImprovementModel),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which type of improver do you want to create? The types that can be created are #3classic#0, #3branching#0, and #3theoretical#0.".SubstituteANSIColour());
				return;
			}

			var type = input.PopSpeech();

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your new improver.");
				return;
			}

			var name = input.PopSpeech().TitleCase();

			if (actor.Gameworld.ImprovementModels.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					"There is already an improver with that name. Names must be unique.");
				return;
			}

			IImprovementModel newModel = null;

			switch (type.ToLowerInvariant())
			{
				case "classic":
					newModel = new ClassicImprovement(actor.Gameworld, name);
					break;
				case "branching":
				case "branch":
					newModel = new BranchingImprover(actor.Gameworld, name);
					break;
				case "theoretical":
				case "theory":
				case "practical":
					newModel = new TheoreticalImprovementModel(actor.Gameworld, name);
					break;
				default:
					actor.OutputHandler.Send($"Unfortunately #3{type}#0 is not a valid type of improver. The types that can be created are #3classic#0, #3branching#0, and #3theoretical#0.".SubstituteANSIColour());
					return;
			}

			actor.Gameworld.Add(newModel);
			actor.RemoveAllEffects<BuilderEditingEffect<IImprovementModel>>();
			actor.AddEffect(new BuilderEditingEffect<IImprovementModel>(actor) { EditingItem = newModel });
			actor.OutputHandler.Send($"You create a new improver of type {type.ColourCommand()} called {name.ColourValue()}, which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which improver do you want to clone?");
				return;
			}

			var improver = actor.Gameworld.ImprovementModels.GetByIdOrName(input.PopSpeech());
			if (improver == null)
			{
				actor.OutputHandler.Send("There is no such improver.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your new improver.");
				return;
			}

			var name = input.SafeRemainingArgument;
			if (actor.Gameworld.ImprovementModels.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send($"There is already an improver with that name. Names must be unique.");
				return;
			}

			var clone = improver.Clone(name);
			if (clone is null)
			{
				actor.OutputHandler.Send($"You cannot clone improvers of the same type as {improver.Name.ColourName()}.");
				return;
			}

			actor.Gameworld.Add(clone);
			actor.RemoveAllEffects<BuilderEditingEffect<IImprovementModel>>();
			actor.AddEffect(new BuilderEditingEffect<IImprovementModel>(actor) { EditingItem = clone });
			actor.OutputHandler.Send(
				$"You clone the improver {improver.Name.ColourValue()} to a new improver called {clone.Name.ColourValue()}, which you are now editing.");
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Type",
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IImprovementModel>()
														  select new List<string>
														  {
															  proto.Id.ToString("N0", character),
															  proto.Name,
															  proto.ImproverType
														  },

		CustomSearch = (protos, keyword, gameworld) => protos,

		DefaultCommandHelp = BuilderModule.ImproverHelpText,

		GetEditHeader = item => $"Improver #{item.Id:N0} ({item.Name})"
	};

	public static EditableItemHelper UnitOfMeasureHelper = new()
	{
		ItemName = "Unit of Measure",
		ItemNamePlural = "Units of Measure",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IUnit>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IUnit>(actor) { EditingItem = (IUnit)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IUnit>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.UnitManager.Units.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.UnitManager.Units.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.UnitManager.Units.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.UnitManager.AddUnit((IUnit)item),
		CastToType = typeof(IUnit),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which system do you want to make your new unit for?");
				return;
			}

			var system = input.PopSpeech().TitleCase();

			if (input.IsFinished)
			{
				actor.OutputHandler.Send($"Which type of unit do you want to create? Valid options are {Enum.GetValues<UnitType>().ListToColouredString()}.");
				return;
			}

			if (!input.PopSpeech().TryParseEnum<UnitType>(out var type))
			{
				actor.OutputHandler.Send($"The text {input.Last.ColourCommand()} is not a valid unit type. Valid options are {Enum.GetValues<UnitType>().ListToColouredString()}.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send($"What multiplier should the new unit have relative to the engine base unit?");
				return;
			}

			if (!double.TryParse(input.PopSpeech(), out var multiplier))
			{
				actor.OutputHandler.Send($"The text {input.Last.ColourCommand()} is not a valid number.");
				return;
			}


			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your new unit.");
				return;
			}

			var name = input.PopSpeech().ToLowerInvariant();

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("What should the primary abbreviation of your new unit be?");
				return;
			}

			var abbreviation = input.SafeRemainingArgument;

			if (actor.Gameworld.UnitManager.Units.Any(x => x.System.EqualTo(system) && x.Type == type && x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send($"There is already a unit for the {system.ColourName()} system of type {type.DescribeEnum().ColourValue()} with the name {name.ColourCommand()}. Names must be unique.");
				return;
			}

			var unit = new Unit(actor.Gameworld, name, system, abbreviation, type, multiplier);
			actor.Gameworld.UnitManager.AddUnit(unit);
			actor.Gameworld.UnitManager.RecalculateAllUnits();
			actor.RemoveAllEffects<BuilderEditingEffect<IUnit>>();
			actor.AddEffect(new BuilderEditingEffect<IUnit>(actor) { EditingItem = unit });
			actor.OutputHandler.Send($"You create a new unit for the {system.ColourName()} system of type {type.DescribeEnum().ColourValue()} called {name.ColourValue()}, which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which unit do you want to clone?");
				return;
			}

			var unit = actor.Gameworld.UnitManager.Units.GetByIdOrName(input.PopSpeech());
			if (unit == null)
			{
				actor.OutputHandler.Send("There is no such height/weight model.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your new unit.");
				return;
			}

			var name = input.PopSpeech().ToLowerInvariant();

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("What should the primary abbreviation of your new unit be?");
				return;
			}

			var abbreviation = input.SafeRemainingArgument;
			if (actor.Gameworld.UnitManager.Units.Any(x => x.System.EqualTo(unit.System) && x.Type == unit.Type && x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send($"There is already a unit for the {unit.System.ColourName()} system of type {unit.Type.DescribeEnum().ColourValue()} with the name {name.ColourCommand()}. Names must be unique.");
				return;
			}

			var clone = unit.Clone(name, abbreviation);

			actor.Gameworld.UnitManager.AddUnit(clone);
			actor.Gameworld.UnitManager.RecalculateAllUnits();
			actor.RemoveAllEffects<BuilderEditingEffect<IUnit>>();
			actor.AddEffect(new BuilderEditingEffect<IUnit>(actor) { EditingItem = clone });
			actor.OutputHandler.Send($"You clone the unit {unit.Name.ColourValue()} to a new one called {clone.Name.ColourValue()}, which you are now editing.");
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Primary",
			"Type",
			"System",
			"Multiplier",
			"Pre-Offset",
			"Post-Offset",
			"Describer",
			"Last"
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IUnit>()
		                                                  select new List<string>
		                                                  {
			                                                  proto.Id.ToString("N0", character),
			                                                  proto.Name,
															  proto.PrimaryAbbreviation,
															  proto.Type.DescribeEnum(),
															  proto.System,
															  proto.MultiplierFromBase.ToString("N", character),
															  proto.PreMultiplierOffsetFrombase.ToString("N", character),
															  proto.PostMultiplierOffsetFrombase.ToString("N", character),
															  proto.DescriberUnit.ToColouredString(),
															  proto.LastDescriber.ToColouredString()
		                                                  },

		CustomSearch = (protos, keyword, gameworld) => protos,

		DefaultCommandHelp = BuilderModule.UnitHelpText,

		GetEditHeader = item => $"Unit #{item.Id:N0} ({item.Name})"
	};

	public static EditableItemHelper HeightWeightModelHelper = new()
	{
		ItemName = "Height/Weight Model",
		ItemNamePlural = "Height/Weight Models",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IHeightWeightModel>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IHeightWeightModel>(actor) { EditingItem = (IHeightWeightModel)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IHeightWeightModel>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.HeightWeightModels.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.HeightWeightModels.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.HeightWeightModels.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IHeightWeightModel)item),
		CastToType = typeof(IHeightWeightModel),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your new height/weight model.");
				return;
			}

			var name = input.PopSpeech().TitleCase();

			if (actor.Gameworld.HeightWeightModels.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send($"There is already a height/weight model with the name {name.ColourName()}. Names must be unique.");
				return;
			}

			var newModel = new HeightWeightModel(actor.Gameworld, name);

			actor.Gameworld.Add(newModel);
			actor.RemoveAllEffects<BuilderEditingEffect<IHeightWeightModel>>();
			actor.AddEffect(new BuilderEditingEffect<IHeightWeightModel>(actor) { EditingItem = newModel });
			actor.OutputHandler.Send($"You create a new height/weight model called {name.ColourValue()}, which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which height/weight model do you want to clone?");
				return;
			}

			var hwModel = actor.Gameworld.HeightWeightModels.GetByIdOrName(input.PopSpeech());
			if (hwModel == null)
			{
				actor.OutputHandler.Send("There is no such height/weight model.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your new height/weight model.");
				return;
			}

			var name = input.SafeRemainingArgument;
			if (actor.Gameworld.HeightWeightModels.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send($"There is already a height/weight model with the name {name.ColourName()}. Names must be unique.");
				return;
			}

			var clone = hwModel.Clone(name);
			if (clone is null)
			{
				actor.OutputHandler.Send($"You cannot clone improvers of the same type as {hwModel.Name.ColourName()}.");
				return;
			}

			actor.Gameworld.Add(clone);
			actor.RemoveAllEffects<BuilderEditingEffect<IHeightWeightModel>>();
			actor.AddEffect(new BuilderEditingEffect<IHeightWeightModel>(actor) { EditingItem = clone });
			actor.OutputHandler.Send($"You clone the height/weight model {hwModel.Name.ColourValue()} to a new one called {clone.Name.ColourValue()}, which you are now editing.");
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Mean Height",
			"Std Dev Height",
			"Mean BMI",
			"Std Dev BMI",
			"Mean Weight",
			"Std Dev Weight"
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IHeightWeightModel>()
		                                                  select new List<string>
		                                                  {
			                                                  proto.Id.ToString("N0", character),
			                                                  proto.Name,
			                                                  character.Gameworld.UnitManager.DescribeBrief(proto.MeanHeight, UnitType.Length, character),
			                                                  character.Gameworld.UnitManager.DescribeBrief(proto.StandardDeviationHeight, UnitType.Length, character),
			                                                  proto.MeanWeight is not null & proto.StandardDeviationWeight is not null ? "" : character.Gameworld.UnitManager.DescribeBrief(proto.MeanBMI, UnitType.BMI, character),
			                                                  proto.MeanWeight is not null & proto.StandardDeviationWeight is not null ? "" : character.Gameworld.UnitManager.DescribeBrief(proto.StandardDeviationBMI, UnitType.BMI, character),
			                                                  proto.MeanWeight is not null & proto.StandardDeviationWeight is not null ? character.Gameworld.UnitManager.DescribeBrief(proto.MeanWeight.Value, UnitType.Mass, character) : "",
			                                                  proto.MeanWeight is not null & proto.StandardDeviationWeight is not null ? character.Gameworld.UnitManager.DescribeBrief(proto.StandardDeviationWeight.Value, UnitType.Mass, character)  : ""
														  },

		CustomSearch = (protos, keyword, gameworld) => protos,

		DefaultCommandHelp = BuilderModule.HeightWeightModelHelp,

		GetEditHeader = item => $"Height/Weight Model #{item.Id:N0} ({item.Name})"
	};

	public string ItemName { get; private set; }
	public string ItemNamePlural { get; private set; }
	public Func<IEditableItem, string> GetEditHeader { get; private set; }
	public Action<ICharacter, IEditableItem> SetEditableItemAction { get; private set; }
	public Func<ICharacter, IEditableItem> GetEditableItemFunc { get; private set; }
	public Action<ICharacter, StringStack> EditableNewAction { get; private set; }
	public Action<ICharacter, StringStack> EditableCloneAction { get; private set; }
	public Func<ICharacter, long, IEditableItem> GetEditableItemByIdFunc { get; private set; }
	public Func<ICharacter, string, IEditableItem> GetEditableItemByIdOrNameFunc { get; private set; }
	public Func<ICharacter, IEnumerable<IEditableItem>> GetAllEditableItems { get; private set; }
	public Action<IEditableItem> AddItemToGameWorldAction { get; private set; }

	public Func<ICharacter, IEnumerable<IEditableItem>, IEnumerable<IEnumerable<string>>> GetListTableContentsFunc
	{
		get;
		private set;
	}

	public Func<ICharacter, IEnumerable<string>> GetListTableHeaderFunc { get; private set; }
	public Func<List<IEditableItem>, string, IFuturemud, List<IEditableItem>> CustomSearch { get; private set; }
	public Type CastToType { get; private set; }
	public string DefaultCommandHelp { get; private set; }
}