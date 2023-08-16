using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using MudSharp.NPC;
using MudSharp.RPG.Dreams;
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
		DefaultCommandHelp = BuilderModule.NPCSpawnerHelp
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

	public static EditableItemHelper MagicSchoolHelper { get; }
	public static EditableItemHelper MagicCapabilityHelper { get; }
	public static EditableItemHelper MagicResourceHelper { get; }

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
	//public static EditableItemHelper MagicPowerHelper { get; }

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