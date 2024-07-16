using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body.Traits.Improvement;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.CharacterCreation;
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
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Generators;
using MudSharp.Magic.Powers;
using MudSharp.Magic.Resources;
using MudSharp.NPC;
using MudSharp.NPC.AI;
using MudSharp.PerceptionEngine;
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
	public static EditableItemHelper CharacterIntroTemplateHelper { get; } = new()
	{
		ItemName = "Character Intro Template",
		ItemNamePlural = "Character Intro Templates",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<ICharacterIntroTemplate>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<ICharacterIntroTemplate>(actor) { EditingItem = (ICharacterIntroTemplate)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<ICharacterIntroTemplate>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.CharacterIntroTemplates.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.CharacterIntroTemplates.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.CharacterIntroTemplates.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((ICharacterIntroTemplate)item),
		CastToType = typeof(ICharacterIntroTemplate),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("What name do you want to give to the character intro template?");
				return;
			}

			var name = input.SafeRemainingArgument.TitleCase();
			if (actor.Gameworld.CharacterIntroTemplates.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send($"There is already a character intro template with the name {name.ColourName()}. Names must be unique.");
				return;
			}

			var cit = new CharacterCreation.CharacterIntroTemplate(actor.Gameworld, name);
			actor.Gameworld.Add(cit);
			actor.RemoveAllEffects(x => x.IsEffectType<IBuilderEditingEffect<ICharacterIntroTemplate>>());
			actor.AddEffect(new BuilderEditingEffect<ICharacterIntroTemplate>(actor){EditingItem = cit});
			actor.OutputHandler.Send($"You create a new character intro template called {name.ColourName()}, which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which character intro template do you want to clone?");
				return;
			}

			var template = actor.Gameworld.CharacterIntroTemplates.GetByIdOrName(input.PopSpeech());
			if (template is null)
			{
				actor.OutputHandler.Send("There is no such character intro template.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("What name do you want to give to the new character intro template?");
				return;
			}

			var name = input.SafeRemainingArgument.TitleCase();
			if (actor.Gameworld.CharacterIntroTemplates.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send($"There is already a character intro template with the name {name.ColourName()}. Names must be unique.");
				return;
			}

			var cit = template.Clone(name);
			actor.Gameworld.Add(cit);
			actor.RemoveAllEffects(x => x.IsEffectType<IBuilderEditingEffect<ICharacterIntroTemplate>>());
			actor.AddEffect(new BuilderEditingEffect<ICharacterIntroTemplate>(actor) { EditingItem = cit });
			actor.OutputHandler.Send($"You create a new character intro template called {name.ColourName()} as a clone of {template.Name.ColourName()}, which you are now editing.");
		},
		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Priority",
			"Prog",
			"# Echoes",
			"Length"
		},

		GetListTableContentsFunc = (character, protos) =>
			from proto in protos.OfType<ICharacterIntroTemplate>()
			select new List<string>
			{
				proto.Id.ToString("N0", character),
				proto.Name,
				proto.ResolutionPriority.ToString("N0", character),
				proto.AppliesToCharacterProg.MXPClickableFunctionName(),
				proto.Echoes.Count.ToString("N0", character),
				TimeSpan.FromSeconds(proto.Delays.Sum(x => x.TotalSeconds)).DescribePreciseBrief(character)

			},

		CustomSearch = (protos, keyword, gameworld) => protos,
		GetEditHeader = item => $"Character Intro Template #{item.Id:N0}",
		DefaultCommandHelp = ChargenModule.IntroTemplateHelp
	};

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

	public static EditableItemHelper NewPlayerHintHelper { get; } = new() {
		ItemName = "New Player Hint",
		ItemNamePlural = "New Player Hints",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<INewPlayerHint>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<INewPlayerHint>(actor) { EditingItem = (INewPlayerHint)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<INewPlayerHint>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.NewPlayerHints.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.NewPlayerHints.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.NewPlayerHints.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((INewPlayerHint)item),
		CastToType = typeof(INewPlayerHint),
		EditableNewAction = (actor, input) =>
		{
			void NewItemPost(string text, IOutputHandler handler, object[] args)
			{
				var hint = new RPG.Hints.NewPlayerHint(actor.Gameworld, text);
				actor.Gameworld.Add(hint);
				actor.RemoveAllEffects<BuilderEditingEffect<INewPlayerHint>>();
				actor.AddEffect(new BuilderEditingEffect<INewPlayerHint>(actor) { EditingItem = hint });
				actor.OutputHandler.Send($"You create new player hint #{hint.Id.ToString("N0", actor)}, which you are now editing.");
			}

			void NewItemCancel(IOutputHandler handler, object[] args)
			{
				handler.Send("You decide not to create a new player hint.");
			}

			actor.OutputHandler.Send("Please enter the text that will be shown for this hint.");
			actor.EditorMode(NewItemPost, NewItemCancel, 1.0, null);
		},
		EditableCloneAction = null,
		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Repeatable?",
			"Filter",
			"Priority"
		},

		GetListTableContentsFunc = (character, protos) => 
			from proto in protos.OfType<INewPlayerHint>()
			orderby proto.Priority descending
			select new List<string>
			{
				proto.Id.ToString("N0", character),
				proto.CanRepeat.ToColouredString(),
				proto.FilterProg?.MXPClickableFunctionName() ?? "",
				proto.Priority.ToString("N0", character)
			},

		CustomSearch = (protos, keyword, gameworld) => protos,
		GetEditHeader = item => $"New Player Hint #{item.Id:N0}",
		DefaultCommandHelp = BuilderModule.NewPlayerHintHelp
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

	public static EditableItemHelper ChargenAdviceHelper { get; } = new()
	{
		ItemName = "Chargen Advice",
		ItemNamePlural = "Chargen Advices",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IChargenAdvice>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IChargenAdvice>(actor) { EditingItem = (IChargenAdvice)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IChargenAdvice>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.ChargenAdvices.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.ChargenAdvices.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.ChargenAdvices.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IChargenAdvice)item),
		CastToType = typeof(IChargenAdvice),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send(
					$"What stage of character creation should this advice apply to?\nOptions are: {Enum.GetValues<ChargenStage>().Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
				return;
			}

			if (!input.PopSpeech().TryParseEnum<ChargenStage>(out var stage))
			{
				actor.OutputHandler.Send(
					$"That is not a valid character creation stage.\nOptions are: {Enum.GetValues<ChargenStage>().Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a title for your chargen advice.");
				return;
			}

			var name = input.SafeRemainingArgument.TitleCase();

			actor.OutputHandler.Send("Enter the content of the advice in the editor below.\n\n");
			actor.EditorMode((text, handler, args) =>
			{
				var advice = new ChargenAdvice(actor.Gameworld, name, text, stage);
				actor.Gameworld.Add(advice);
				actor.RemoveAllEffects<BuilderEditingEffect<IChargenAdvice>>();
				actor.AddEffect(new BuilderEditingEffect<IChargenAdvice>(actor) { EditingItem = advice });
				handler.Send(
					$"You create a new character creation advice titled {name.ColourName()}, which you are now editing.");
			}, (handler, objects) => { handler.Send("You decide not to create a character creation advice."); }, 1.0);
		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which piece of character creation advice do you want to clone?");
				return;
			}

			var target = actor.Gameworld.ChargenAdvices.GetByIdOrName(input.SafeRemainingArgument);
			if (target is null)
			{
				actor.OutputHandler.Send("There is no such character creation advice to clone.");
				return;
			}

			var clone = new ChargenAdvice(target);
			actor.Gameworld.Add(clone);
			actor.RemoveAllEffects<BuilderEditingEffect<IChargenAdvice>>();
			actor.AddEffect(new BuilderEditingEffect<IChargenAdvice>(actor) { EditingItem = clone });
			actor.OutputHandler.Send(
				$"You create a cloned character creation advice titled {clone.Name.ColourName()}, which you are now editing.");
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Title",
			"Prog",
			"Races",
			"Cultures",
			"Ethnicities",
			"Roles"
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IChargenAdvice>()
														  select new List<string>
														  {
															  proto.Id.ToString("N0", character),
															  proto.AdviceTitle,
															  proto.ShouldShowAdviceProg?.MXPClickableFunctionName() ??
															  "None",
															  proto.Gameworld.Races
																   .Count(x => x.ChargenAdvices.Contains(proto))
																   .ToString("N0", character),
															  proto.Gameworld.Cultures
																   .Count(x => x.ChargenAdvices.Contains(proto))
																   .ToString("N0", character),
															  proto.Gameworld.Ethnicities
																   .Count(x => x.ChargenAdvices.Contains(proto))
																   .ToString("N0", character),
															  proto.Gameworld.Roles
																   .Count(x => x.ChargenAdvices.Contains(proto))
																   .ToString("N0", character)
														  },

		CustomSearch = (protos, keyword, gameworld) => protos,
		GetEditHeader = item => $"Chargen Advice #{item.Id:N0} ({item.Name})",
		DefaultCommandHelp = BuilderModule.ChargenAdviceHelp
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
			actor.OutputHandler.Send("Creating new artificial intelligences is not currently supported.");
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