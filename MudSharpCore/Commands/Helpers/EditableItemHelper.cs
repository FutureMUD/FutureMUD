using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
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
using MudSharp.Economy.Property;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Generators;
using MudSharp.Magic.Resources;
using MudSharp.NPC;
using MudSharp.NPC.AI;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Dreams;
using MudSharp.RPG.Hints;
using AuctionHouse = MudSharp.Economy.Auctions.AuctionHouse;
using Bank = MudSharp.Economy.Banking.Bank;
using ChargenAdvice = MudSharp.CharacterCreation.ChargenAdvice;
using Dream = MudSharp.RPG.Dreams.Dream;
using MagicSpell = MudSharp.Magic.MagicSpell;
using NPCSpawner = MudSharp.NPC.NPCSpawner;
using Property = MudSharp.Economy.Property.Property;

namespace MudSharp.Commands.Helpers;

public class EditableItemHelper
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
			actor.OutputHandler.Send(
				$"You create a new race called {name.ColourName()}{(parent is not null ? $" as a child race of {parent.Name.ColourName()}" : "")}, which you are now editing.");
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
			actor.OutputHandler.Send(
				$"You clone the race {parent.Name.ColourName()} into a new race called {name.ColourName()}, which you are now editing.");
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Parent",
			"Breathing",
			"Swims",
			"Climbs"
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IRace>()
		                                                  select new List<string>
		                                                  {
			                                                  proto.Id.ToString("N0", character),
			                                                  proto.Name,
			                                                  proto.ParentRace?.Name ?? "",
			                                                  proto.BreathingStrategy.Name.TitleCase(),
			                                                  proto.CanSwim.ToString(),
			                                                  proto.CanClimb.ToString()
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

	public static EditableItemHelper PropertyHelper { get; } = new()
	{
		ItemName = "Property",
		ItemNamePlural = "Properties",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IProperty>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IProperty>(actor) { EditingItem = (IProperty)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IProperty>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.Properties.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.Properties.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.Properties.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IProperty)item),
		CastToType = typeof(IProperty),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your new property.");
				return;
			}

			var name = input.PopSpeech().TitleCase();

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which economic zone should this property be tied to?");
				return;
			}

			var zone = actor.Gameworld.EconomicZones.GetByIdOrName(input.PopSpeech());
			if (zone == null)
			{
				actor.OutputHandler.Send("There is no such economic zone.");
				return;
			}

			if (actor.Gameworld.Properties.Any(x => x.EconomicZone == zone && x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already a property in the {zone.Name.ColourName()} economic zone with that name. Names must be unique.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("What default property value do you want to set for this property?");
				return;
			}

			if (!zone.Currency.TryGetBaseCurrency(input.PopSpeech(), out var value))
			{
				actor.OutputHandler.Send(
					$"The value \"{input.Last.ColourCommand()}\" is not a valid amount of {zone.Currency.Name.ColourValue()}.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send(
					"You must specify a bank account for the default owner of this property. Use the format BANKCODE:ACCOUNT#");
				return;
			}

			var bankString = input.PopSpeech();
			var (accountTarget, error) = Bank.FindBankAccount(bankString, null, actor);
			if (accountTarget == null)
			{
				actor.OutputHandler.Send(error);
				return;
			}

			var owner = accountTarget.AccountOwner;

			if (owner is not ICharacter && owner is not IClan)
			{
				actor.OutputHandler.Send(
					"Only bank accounts owned by characters and clans can be used for the default owner of a property.");
				return;
			}

			var property = new Property(name, zone, actor.Location, value, owner, accountTarget);
			actor.Gameworld.Add(property);
			actor.RemoveAllEffects<BuilderEditingEffect<IProperty>>();
			actor.AddEffect(new BuilderEditingEffect<IProperty>(actor) { EditingItem = property });
			actor.OutputHandler.Send(
				$"You create a new property named {name.ColourName()}, which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			actor.OutputHandler.Send("The clone action is not available for properties.");
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Zone"
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IProperty>()
		                                                  select new List<string>
		                                                  {
			                                                  proto.Id.ToString("N0", character),
			                                                  proto.Name,
			                                                  proto.EconomicZone.Name
		                                                  },

		CustomSearch = (protos, keyword, gameworld) => protos,
		GetEditHeader = item => $"Property #{item.Id:N0} ({item.Name})",
		DefaultCommandHelp = PropertyModule.PropertyHelpAdmins
	};

	public static EditableItemHelper AuctionHelper { get; } = new()
	{
		ItemName = "Auction House",
		ItemNamePlural = "Auction Houses",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IAuctionHouse>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IAuctionHouse>(actor) { EditingItem = (IAuctionHouse)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IAuctionHouse>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.AuctionHouses.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.AuctionHouses.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.AuctionHouses.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IAuctionHouse)item),
		CastToType = typeof(IAuctionHouse),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your new auction house.");
				return;
			}

			var name = input.PopSpeech().TitleCase();

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which economic zone should this auction house be tied to?");
				return;
			}

			var zone = actor.Gameworld.EconomicZones.GetByIdOrName(input.PopSpeech());
			if (zone == null)
			{
				actor.OutputHandler.Send("There is no such economic zone.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send(
					"You must specify a bank account into which any proceeds will be transferred. Use the format BANKCODE:ACCOUNT#.");
				return;
			}

			var bankString = input.PopSpeech();
			var (accountTarget, error) = Bank.FindBankAccount(bankString, null, actor);
			if (accountTarget == null)
			{
				actor.OutputHandler.Send(error);
				return;
			}

			if (actor.Gameworld.AuctionHouses.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already an auction house with that name. Names must be unique.");
				return;
			}

			var auctionHouse = new AuctionHouse(zone, name, actor.Location, accountTarget);
			actor.Gameworld.Add(auctionHouse);
			actor.RemoveAllEffects<BuilderEditingEffect<IAuctionHouse>>();
			actor.AddEffect(new BuilderEditingEffect<IAuctionHouse>(actor) { EditingItem = auctionHouse });
			actor.OutputHandler.Send(
				$"You create a new auction house named {name.ColourName()}, which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			actor.OutputHandler.Send("The clone action is not available for auction houses.");
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Zone"
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IAuctionHouse>()
		                                                  select new List<string>
		                                                  {
			                                                  proto.Id.ToString("N0", character),
			                                                  proto.Name,
			                                                  proto.EconomicZone.Name
		                                                  },

		CustomSearch = (protos, keyword, gameworld) => protos,
		GetEditHeader = item => $"Auction House #{item.Id:N0} ({item.Name})",
		DefaultCommandHelp = EconomyModule.AuctionHelpAdmins
	};

	public static EditableItemHelper BankHelper { get; } = new()
	{
		ItemName = "Bank",
		ItemNamePlural = "Banks",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IBank>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IBank>(actor) { EditingItem = (IBank)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IBank>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.Banks.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.Banks.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.Banks.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IBank)item),
		CastToType = typeof(IBank),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your new bank.");
				return;
			}

			var name = input.PopSpeech().TitleCase();

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("What code do you want to use for your bank in transfers?");
				return;
			}

			var code = input.PopSpeech().ToLowerInvariant();
			if (actor.Gameworld.Banks.Any(x => x.Code == code))
			{
				actor.OutputHandler.Send("There is already a bank with that code. Bank codes must be unique.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which economic zone is your bank based in?");
				return;
			}

			var zone = actor.Gameworld.EconomicZones.GetByIdOrName(input.SafeRemainingArgument);
			if (zone == null)
			{
				actor.OutputHandler.Send("There is no such economic zones.");
				return;
			}

			if (actor.Gameworld.Banks.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already a bank with that name. Names must be unique.");
				return;
			}

			var bank = new Bank(actor.Gameworld, name, code, zone);
			actor.Gameworld.Add(bank);
			actor.RemoveAllEffects<BuilderEditingEffect<IBank>>();
			actor.AddEffect(new BuilderEditingEffect<IBank>(actor) { EditingItem = bank });
			actor.OutputHandler.Send(
				$"You create a new bank named {name.ColourName()}, which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which bank do you want to clone?");
				return;
			}

			var bank = actor.Gameworld.Banks.GetByIdOrName(input.PopSpeech());
			if (bank == null)
			{
				actor.OutputHandler.Send("There is no such bank.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your new cloned bank.");
				return;
			}

			var name = input.SafeRemainingArgument;
			if (actor.Gameworld.Banks.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send($"There is already a bank with that name. Names must be unique.");
				return;
			}

			//var clone = new Bank((Economy.Banks.Bank) bank , name);
			//actor.Gameworld.Add(clone);
			//actor.RemoveAllEffects<BuilderEditingEffect<IBank>>();
			//actor.AddEffect(new BuilderEditingEffect<IBank>(actor) {EditingItem = clone});
			//actor.OutputHandler.Send($"You clone the bank {bank.Name.ColourValue()} to a new bank called {clone.Name.ColourValue()}, which you are now editing.");
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Zone",
			"# Accounts"
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IBank>()
		                                                  select new List<string>
		                                                  {
			                                                  proto.Id.ToString("N0", character),
			                                                  proto.Name,
			                                                  proto.EconomicZone.Name,
			                                                  proto.BankAccounts.Count().ToString("N0", character)
		                                                  },

		CustomSearch = (protos, keyword, gameworld) => protos,
		GetEditHeader = item => $"Bank #{item.Id:N0} ({item.Name})",
		DefaultCommandHelp = EconomyModule.BankAdminHelpText
	};

	public static EditableItemHelper MagicSpellHelper { get; } = new()
	{
		ItemName = "Magic Spell",
		ItemNamePlural = "Magic Spells",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicSpell>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IMagicSpell>(actor) { EditingItem = (IMagicSpell)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IMagicSpell>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.MagicSpells.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.MagicSpells.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.MagicSpells.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IMagicSpell)item),
		CastToType = typeof(IMagicSpell),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your new magic spell.");
				return;
			}

			var name = input.PopSpeech().TitleCase();
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a magic school for your spell to belong to.");
				return;
			}

			var school = actor.Gameworld.MagicSchools.GetByIdOrName(input.SafeRemainingArgument);
			if (school == null)
			{
				actor.OutputHandler.Send("There is no such magic school.");
				return;
			}

			if (actor.Gameworld.MagicSpells.Any(x => x.School == school && x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already a spell in the {school.Name.Colour(school.PowerListColour)} school of magic with that name. Names must be unique per school.");
				return;
			}

			var spell = new MagicSpell(name, school);
			actor.Gameworld.Add(spell);
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicSpell>>();
			actor.AddEffect(new BuilderEditingEffect<IMagicSpell>(actor) { EditingItem = spell });
			actor.OutputHandler.Send(
				$"You create a new magic spell in the {school.Name.Colour(school.PowerListColour)} school of magic named {name.ColourName()}, which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which magic spell do you want to clone?");
				return;
			}

			var spell = actor.Gameworld.MagicSpells.GetByIdOrName(input.PopSpeech());
			if (spell == null)
			{
				actor.OutputHandler.Send("There is no such magic spell.");
				return;
			}

			var spells = actor.Gameworld.MagicSpells.Where(x => x.Name.EqualTo(spell.Name)).ToList();
			if (!long.TryParse(input.Last, out _) && spells.Count > 1)
			{
				actor.OutputHandler.Send(
					$"The spell name you specified is ambiguous. For the sake of clarity, please use the ID instead from the following options:{spells.Select(x => $"#{x.Id.ToString("N0", actor)} - {x.Name.Colour(x.School.PowerListColour)} from {x.School.Name.Colour(x.School.PowerListColour)}").ListToLines(true)}");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your new cloned spell.");
				return;
			}

			var name = input.SafeRemainingArgument;
			if (actor.Gameworld.MagicSpells.Where(x => x.School == spell.School).Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already a spell in the {spell.School.Name.Colour(spell.School.PowerListColour)} school of magic with that name. Names must be unique per school.");
				return;
			}

			var clone = new MagicSpell((MagicSpell)spell, name);
			actor.Gameworld.Add(clone);
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicSpell>>();
			actor.AddEffect(new BuilderEditingEffect<IMagicSpell>(actor) { EditingItem = clone });
			actor.OutputHandler.Send(
				$"You clone the magic spell {spell.Name.Colour(spell.School.PowerListColour)} to a new spell called {clone.Name.Colour(clone.School.PowerListColour)}, which you are now editing.");
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Blurb",
			"School"
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IMagicSpell>()
		                                                  select new List<string>
		                                                  {
			                                                  proto.Id.ToString("N0", character),
			                                                  proto.Name,
			                                                  proto.Blurb,
			                                                  proto.School.Name
		                                                  },

		CustomSearch = (protos, keyword, gameworld) => protos,

		DefaultCommandHelp = @"This command is used to work with and edit magic spells.

The core syntax is as follows:

    magic spell list - shows all magic spells
    magic spell edit new <name> <school> - creates a new magic spell
    magic spell clone <old> <new> - clones an existing magic spell
    magic spell edit <which> - begins editing a magic spell
    magic spell close - closes an editing magic spell
    magic spell show <which> - shows builder information about a spell
    magic spell show - shows builder information about the currently edited spell
    magic spell edit - an alias for magic spell show (with no args)
    magic spell set <...> - edits the properties of a magic spell",

		GetEditHeader = item => $"Magic Spell #{item.Id:N0} ({item.Name})"
	};

	public static EditableItemHelper MagicSchoolHelper { get; } = new()
	{
		ItemName = "Magic School",
		ItemNamePlural = "Magic School",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicSchool>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IMagicSchool>(actor) { EditingItem = (IMagicSchool)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IMagicSchool>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.MagicSchools.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.MagicSchools.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.MagicSchools.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IMagicSchool)item),
		CastToType = typeof(IMagicSchool),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your new magic school.");
				return;
			}

			var name = input.PopSpeech().TitleCase();

			if (actor.Gameworld.MagicSchools.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send($"There is already a magic school called {name.ColourName()}. Names must be unique.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a verb used as a command to interact with that magic school.");
				return;
			}
			var verb = input.PopSpeech().ToLowerInvariant().CollapseString();

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify an adjective to describe effects from that magic school.");
				return;
			}
			var adjective = input.PopSpeech().ToLowerInvariant();

			ANSIColour colour = Telnet.Magenta;
			if (!input.IsFinished)
			{
				colour = Telnet.GetColour(input.SafeRemainingArgument);
				if (colour is null)
				{
					actor.OutputHandler.Send($"That is not a valid colour option. The options are as follows:\n\n{Telnet.GetColourOptions.Select(x => x.Colour(Telnet.GetColour(x))).ListToLines(true)}");
					return;
				}
			}

			var school = new MagicSchool(actor.Gameworld, name, verb, adjective, colour);
			actor.Gameworld.Add(school);
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicSchool>>();
			actor.AddEffect(new BuilderEditingEffect<IMagicSchool>(actor) { EditingItem = school });
			actor.OutputHandler.Send(
				$"You create a new school of magic called {school.Name.Colour(school.PowerListColour)}, which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which magic school do you want to clone?");
				return;
			}

			var school = actor.Gameworld.MagicSchools.GetByIdOrName(input.PopSpeech());
			if (school == null)
			{
				actor.OutputHandler.Send("There is no such magic school.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your new cloned school.");
				return;
			}

			var name = input.SafeRemainingArgument.TitleCase();
			if (actor.Gameworld.MagicSchools.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already a magic school with that name. Names must be unique.");
				return;
			}

			var clone = school.Clone(name);
			actor.Gameworld.Add(clone);
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicSchool>>();
			actor.AddEffect(new BuilderEditingEffect<IMagicSchool>(actor) { EditingItem = clone });
			actor.OutputHandler.Send(
				$"You clone the magic school {school.Name.Colour(school.PowerListColour)} as {clone.Name.Colour(clone.PowerListColour)}, which you are now editing.");
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Verb",
			"Adjective",
			"Parent",
			"Colour",
			"Spells",
			"Powers",
			"Capabilities"
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IMagicSchool>()
														  select new List<string>
														  {
															  proto.Id.ToString("N0", character),
															  proto.Name,
															  proto.SchoolVerb,
															  proto.SchoolAdjective,
															  proto.ParentSchool?.Name ?? "",
															  proto.PowerListColour.Name.Colour(proto.PowerListColour),
															  proto.Gameworld.MagicSpells.Count(x => x.School == proto).ToString("N0", character),
															  proto.Gameworld.MagicPowers.Count(x => x.School == proto).ToString("N0", character),
															  proto.Gameworld.MagicCapabilities.Count(x => x.School == proto).ToString("N0", character),
														  },

		CustomSearch = (protos, keyword, gameworld) => protos,

		DefaultCommandHelp = @"This command is used to work with and edit magic schools.

The core syntax is as follows:

    #3magic school list - shows all magic schools
    #3magic school edit new <name> <school>#0 - creates a new magic school
    #3magic school clone <old> <new>#0 - clones an existing magic school
    #3magic school edit <which>#0 - begins editing a magic school
    #3magic school close#0 - closes an editing magic school
    #3magic school show <which>#0 - shows builder information about a school
    #3magic school show#0 - shows builder information about the currently edited school
    #3magic school edit#0 - an alias for magic school show (with no args)
    #3magic school set name <name>#0 - renames this school
	#3magic school set parent <which>#0 - sets a parent school
	#3magic school set parent none#0 - clears a parent school
	#3magic school set adjective <which>#0 - sets the adjective used to refer to powers in this school
	#3magic school set verb <which>#0 - sets the verb (command) used for this school
	#3magic school set colour <which>#0 - sets the ANSI colour for display with this school",

		GetEditHeader = item => $"Magic School #{item.Id:N0} ({item.Name})"
	};
	public static EditableItemHelper MagicCapabilityHelper { get; } = new()
	{
		ItemName = "Magic Capability",
		ItemNamePlural = "Magic Capabilities",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicCapability>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IMagicCapability>(actor) { EditingItem = (IMagicCapability)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IMagicCapability>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.MagicCapabilities.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.MagicCapabilities.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.MagicCapabilities.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IMagicCapability)item),
		CastToType = typeof(IMagicCapability),
		EditableNewAction = (actor, input) =>
		{
			var capability = MagicCapabilityFactory.LoaderFromBuilderInput(actor.Gameworld, actor, input);
			if (capability is null)
			{
				return;
			}

			actor.Gameworld.Add(capability);
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicCapability>>();
			actor.AddEffect(new BuilderEditingEffect<IMagicCapability>(actor) { EditingItem = capability });
			actor.OutputHandler.Send($"You create a new magic capability called {capability.Name.ColourName()}, which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which magic capability do you want to clone?");
				return;
			}

			var capability = actor.Gameworld.MagicCapabilities.GetByIdOrName(input.PopSpeech());
			if (capability == null)
			{
				actor.OutputHandler.Send("There is no such magic capability.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your new cloned magic capability.");
				return;
			}

			var name = input.SafeRemainingArgument.TitleCase();
			if (actor.Gameworld.MagicCapabilities.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already a magic capability with that name. Names must be unique.");
				return;
			}

			var newCapability = capability.Clone(name);

			actor.Gameworld.Add(newCapability);
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicCapability>>();
			actor.AddEffect(new BuilderEditingEffect<IMagicCapability>(actor) { EditingItem = newCapability });
			actor.OutputHandler.Send($"You create a new magic capability called {newCapability.Name.ColourName()} as a clone of {capability.Name.ColourName()}, which you are now editing.");
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"School",
			"Power",
			"# Powers",
			"# Regens",
			"Resources"
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IMagicCapability>()
														  select new List<string>
														  {
															  proto.Id.ToString("N0", character),
															  proto.Name,
															  proto.School.Name,
															  proto.PowerLevel.ToString("N0", character),
															  proto.AllPowers.Count().ToString("N0", character),
															  proto.Regenerators.Count().ToString("N0", character),
															  proto.Regenerators.SelectMany(x => x.GeneratedResources).Distinct().Select(x => x.Name.ColourValue()).ListToCommaSeparatedValues(", ")
														  },

		CustomSearch = (protos, keyword, gameworld) => protos,

		DefaultCommandHelp = @"This command is used to work with and edit magic resources.

The core syntax is as follows:

    #3magic capability list - shows all magic capabilities
    #3magic capability edit new <type> <name>`#0 - creates a new magic capability
    #3magic capability clone <old> <new>#0 - clones an existing magic capability
    #3magic capability edit <which>#0 - begins editing a magic capability
    #3magic capability close#0 - closes an editing magic capability
    #3magic capability show <which>#0 - shows builder information about a capability
    #3magic capability show#0 - shows builder information about the currently edited capability
    #3magic capability edit#0 - an alias for magic capability show (with no args)
    #3magic capability set ...#0 - edits the properties of a magic capability. See #3magic capability set ?#0 for more info.",

		GetEditHeader = item => $"Magic Capability #{item.Id:N0} ({item.Name})"
	};

	public static EditableItemHelper MagicResourceHelper { get; } = new()
	{
		ItemName = "Magic Resource",
		ItemNamePlural = "Magic Resources",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicResource>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IMagicResource>(actor) { EditingItem = (IMagicResource)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IMagicResource>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.MagicResources.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.MagicResources.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.MagicResources.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IMagicResource)item),
		CastToType = typeof(IMagicResource),
		EditableNewAction = (actor, input) =>
		{
			var resource = BaseMagicResource.CreateResourceFromBuilderInput(actor, input);
			if (resource is null)
			{
				return;
			}

			actor.Gameworld.Add(resource);
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicResource>>();
			actor.AddEffect(new BuilderEditingEffect<IMagicResource>(actor) { EditingItem = resource });
			actor.OutputHandler.Send($"You create a new magic resource called {resource.Name.Colour(Telnet.BoldPink)}, which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which magic resource do you want to clone?");
				return;
			}

			var resource = actor.Gameworld.MagicResources.GetByIdOrName(input.PopSpeech());
			if (resource == null)
			{
				actor.OutputHandler.Send("There is no such magic resource.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your new cloned magic resource.");
				return;
			}

			var name = input.SafeRemainingArgument.TitleCase();
			if (actor.Gameworld.MagicResources.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already a magic resource with that name. Names must be unique.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a short name for your new cloned magic resource.");
				return;
			}

			var shortName = input.SafeRemainingArgument.TitleCase();
			if (actor.Gameworld.MagicResources.Any(x => x.ShortName.EqualTo(shortName)))
			{
				actor.OutputHandler.Send(
					$"There is already a magic resource with that short name. Names must be unique.");
				return;
			}

			var clone = resource.Clone(name, shortName);
			actor.Gameworld.Add(clone);
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicResource>>();
			actor.AddEffect(new BuilderEditingEffect<IMagicResource>(actor) { EditingItem = clone });
			actor.OutputHandler.Send(
				$"You clone the magic resource {resource.Name.Colour(Telnet.BoldPink)} as {clone.Name.Colour(Telnet.BoldPink)} ({clone.ShortName.Colour(Telnet.BoldPink)}), which you are now editing.");
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Short",
			"Type",
			"Prompt",
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IMagicResource>()
														  select new List<string>
														  {
															  proto.Id.ToString("N0", character),
															  proto.Name,
															  proto.ShortName,
															  proto.ResourceType.DescribeEnum(),
															  proto.ClassicPromptString(1.0)
														  },

		CustomSearch = (protos, keyword, gameworld) => protos,

		DefaultCommandHelp = @"This command is used to work with and edit magic resources.

The core syntax is as follows:

    #3magic resource list - shows all magic resources
    #3magic resource edit new <type> <name> <shortname>#0 - creates a new magic resource
    #3magic resource clone <old> <new>#0 - clones an existing magic resource
    #3magic resource edit <which>#0 - begins editing a magic resource
    #3magic resource close#0 - closes an editing magic resource
    #3magic resource show <which>#0 - shows builder information about a resource
    #3magic resource show#0 - shows builder information about the currently edited resource
    #3magic resource edit#0 - an alias for magic resource show (with no args)
    #3magic resource set ...#0 - edits the properties of a magic resource. See #3magic resource set ?#0 for more info.",

		GetEditHeader = item => $"Magic Resource #{item.Id:N0} ({item.Name})"
	};
	public static EditableItemHelper MagicRegeneratorHelper { get; } = new()
	{
		ItemName = "Magic Regenerator",
		ItemNamePlural = "Magic Regenerators",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicResourceRegenerator>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IMagicResourceRegenerator>(actor) { EditingItem = (IMagicResourceRegenerator)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IMagicResourceRegenerator>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.MagicResourceRegenerators.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.MagicResourceRegenerators.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.MagicResourceRegenerators.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IMagicResourceRegenerator)item),
		CastToType = typeof(IMagicResourceRegenerator),
		EditableNewAction = (actor, input) =>
		{
			var regenerator = BaseMagicResourceGenerator.LoadFromBuilderInput(actor, input);
			if (regenerator is null)
			{
				return;
			}

			actor.Gameworld.Add(regenerator);
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicResourceRegenerator>>();
			actor.AddEffect(new BuilderEditingEffect<IMagicResourceRegenerator>(actor) { EditingItem = regenerator });
			actor.OutputHandler.Send($"You create a new magic regenerator called {regenerator.Name.Colour(Telnet.Cyan)}, which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which magic regenerator do you want to clone?");
				return;
			}

			var regenerator = actor.Gameworld.MagicResourceRegenerators.GetByIdOrName(input.PopSpeech());
			if (regenerator == null)
			{
				actor.OutputHandler.Send("There is no such magic regenerator.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your new cloned magic regenerator.");
				return;
			}

			var name = input.SafeRemainingArgument.TitleCase();
			if (actor.Gameworld.MagicResourceRegenerators.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already a magic regenerator with that name. Names must be unique.");
				return;
			}

			var clone = regenerator.Clone(name);
			actor.Gameworld.Add(clone);
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicResourceRegenerator>>();
			actor.AddEffect(new BuilderEditingEffect<IMagicResourceRegenerator>(actor) { EditingItem = clone });
			actor.OutputHandler.Send(
				$"You clone the magic regenerator {regenerator.Name.Colour(Telnet.Cyan)} as {clone.Name.Colour(Telnet.Cyan)}, which you are now editing.");
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Type",
			"Resources",
			"# Times Used"
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IMagicResourceRegenerator>()
														  select new List<string>
														  {
															  proto.Id.ToString("N0", character),
															  proto.Name,
															  proto.RegeneratorTypeName,
															  proto.GeneratedResources.Select(x => x.Name.Colour(Telnet.BoldPink)).ListToCommaSeparatedValues(", "),
															  proto.Gameworld.MagicCapabilities.Count(x => x.Regenerators.Contains(proto)).ToString("N0", character)
														  },

		CustomSearch = (protos, keyword, gameworld) => protos,

		DefaultCommandHelp = @"This command is used to work with and edit magic regenerators.

The core syntax is as follows:

    #3magic regenerator list - shows all magic regenerators
    #3magic regenerator edit new <type> <name> <resource>#0 - creates a new magic regenerator
    #3magic regenerator clone <old> <new>#0 - clones an existing magic regenerator
    #3magic regenerator edit <which>#0 - begins editing a magic regenerator
    #3magic regenerator close#0 - closes an editing magic regenerator
    #3magic regenerator show <which>#0 - shows builder information about a regenerator
    #3magic regenerator show#0 - shows builder information about the currently edited regenerator
    #3magic regenerator edit#0 - an alias for magic regenerator show (with no args)
    #3magic regenerator set ...#0 - edits the properties of a magic regenerator. See #3magic regenerator set ?#0 for more info.",

		GetEditHeader = item => $"Magic Regenerator #{item.Id:N0} ({item.Name})"
	};

	public static EditableItemHelper MagicPowerHelper { get; } = new()
	{
		ItemName = "Magic Power",
		ItemNamePlural = "Magic Powers",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicPower>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IMagicPower>(actor) { EditingItem = (IMagicPower)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IMagicPower>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.MagicPowers.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.MagicPowers.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.MagicPowers.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IMagicPower)item),
		CastToType = typeof(IMagicPower),
		EditableNewAction = (actor, input) =>
		{
			actor.OutputHandler.Send("Not yet implemented.");
		},
		EditableCloneAction = (actor, input) =>
		{
			actor.OutputHandler.Send("Not yet implemented.");
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"School",
			"Type"
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IMagicPower>()
														  select new List<string>
														  {
															  proto.Id.ToString("N0", character),
															  proto.Name,
															  proto.School.Name,
															  proto.PowerType
														  },

		CustomSearch = (protos, keyword, gameworld) => protos,

		DefaultCommandHelp = @"This command is used to work with and edit magic regenerators.

The core syntax is as follows:

    #3magic regenerator list - shows all magic regenerators
    #3magic regenerator edit new <type> <name> <resource>#0 - creates a new magic regenerator
    #3magic regenerator clone <old> <new>#0 - clones an existing magic regenerator
    #3magic regenerator edit <which>#0 - begins editing a magic regenerator
    #3magic regenerator close#0 - closes an editing magic regenerator
    #3magic regenerator show <which>#0 - shows builder information about a regenerator
    #3magic regenerator show#0 - shows builder information about the currently edited regenerator
    #3magic regenerator edit#0 - an alias for magic regenerator show (with no args)
    #3magic regenerator set ...#0 - edits the properties of a magic regenerator. See #3magic regenerator set ?#0 for more info.",

		GetEditHeader = item => $"Magic Power #{item.Id:N0} ({item.Name})"
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
			//var capability = MagicCapabilityFactory.LoaderFromBuilderInput(actor.Gameworld, actor, input);
			//if (capability is null)
			//{
			//	return;
			//}

			//actor.Gameworld.Add(capability);
			//actor.RemoveAllEffects<BuilderEditingEffect<IMagicCapability>>();
			//actor.AddEffect(new BuilderEditingEffect<IMagicCapability>(actor) { EditingItem = capability });
			//actor.OutputHandler.Send($"You create a new magic capability called {capability.Name.ColourName()}, which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			actor.OutputHandler.Send("Cloning artificial intelligences is not currently supported.");
			return;

			//if (input.IsFinished)
			//{
			//	actor.OutputHandler.Send("Which magic capability do you want to clone?");
			//	return;
			//}

			//var capability = actor.Gameworld.MagicCapabilities.GetByIdOrName(input.PopSpeech());
			//if (capability == null)
			//{
			//	actor.OutputHandler.Send("There is no such magic capability.");
			//	return;
			//}

			//if (input.IsFinished)
			//{
			//	actor.OutputHandler.Send("You must specify a name for your new cloned magic capability.");
			//	return;
			//}

			//var name = input.SafeRemainingArgument.TitleCase();
			//if (actor.Gameworld.MagicCapabilities.Any(x => x.Name.EqualTo(name)))
			//{
			//	actor.OutputHandler.Send(
			//		$"There is already a magic capability with that name. Names must be unique.");
			//	return;
			//}

			//var newCapability = capability.Clone(name);

			//actor.Gameworld.Add(newCapability);
			//actor.RemoveAllEffects<BuilderEditingEffect<IMagicCapability>>();
			//actor.AddEffect(new BuilderEditingEffect<IMagicCapability>(actor) { EditingItem = newCapability });
			//actor.OutputHandler.Send($"You create a new magic capability called {newCapability.Name.ColourName()} as a clone of {capability.Name.ColourName()}, which you are now editing.");
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

		DefaultCommandHelp = @"This command is used to work with and edit artificial intelligences.

The core syntax is as follows:

    #3ai list - shows all AIs
    #3ai edit new <type> <name> <shortname>#0 - creates a new AI
    #3ai clone <old> <new>#0 - clones an existing AI
    #3ai edit <which>#0 - begins editing a AI
    #3ai close#0 - closes an editing AI
    #3ai show <which>#0 - shows builder information about a resource
    #3ai show#0 - shows builder information about the currently edited resource
    #3ai edit#0 - an alias for AI show (with no args)
    #3ai set ...#0 - edits the properties of a AI. See #3AI set ?#0 for more info.
	#3ai add <npc>#0 - adds an AI routine to an NPC in the gameworld
	#3ai remove <npc>#0 - removes an AI routine from an NPC in the gameworld
	#3ai instances <which>#0 - shows all NPCs who have the AI in question running

The following options are available as filters with the #3list#0 subcommand:

	#6+<keyword>#0 - only show AIs with the keyword in the name
	#6-<keyword>#0 - only show AIs without the keyword in the name",

		GetEditHeader = item => $"Artificial Intelligence #{item.Id:N0} ({item.Name})"
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