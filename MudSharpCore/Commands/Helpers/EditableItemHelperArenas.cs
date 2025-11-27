using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using MudSharp.Arenas;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Commands.Helpers;

public partial class EditableItemHelper
{
	public static EditableItemHelper CombatArenaHelper { get; } = new()
	{
		ItemName = "Combat Arena",
		ItemNamePlural = "Combat Arenas",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<ICombatArena>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<ICombatArena>(actor) { EditingItem = (ICombatArena)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<ICombatArena>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.CombatArenas.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.CombatArenas.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.CombatArenas.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((ICombatArena)item),
		CastToType = typeof(ICombatArena),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for the new combat arena.");
				return;
			}

			var name = input.PopSpeech().TitleCase();
			if (actor.Gameworld.CombatArenas.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send("There is already a combat arena with that name.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which economic zone will this arena belong to?");
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
				actor.OutputHandler.Send("Which currency should this arena use?");
				return;
			}

			var currency = actor.Gameworld.Currencies.GetByIdOrName(input.SafeRemainingArgument);
			if (currency == null)
			{
				actor.OutputHandler.Send("There is no such currency.");
				return;
			}

			var arena = new CombatArena(actor.Gameworld, name, zone, currency);
			actor.Gameworld.Add(arena);
			actor.RemoveAllEffects<BuilderEditingEffect<ICombatArena>>();
			actor.AddEffect(new BuilderEditingEffect<ICombatArena>(actor) { EditingItem = arena });
			actor.OutputHandler.Send(
				$"You create combat arena #{arena.Id.ToStringN0(actor)} ({arena.Name.ColourName()}), which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			actor.OutputHandler.Send("Cloning combat arenas is not currently supported.");
		},
		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Zone",
			"Currency"
		},
		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<ICombatArena>()
			select new List<string>
			{
				proto.Id.ToString("N0", character),
				proto.Name,
				proto.EconomicZone.Name,
				proto.Currency.Name
			},
		CustomSearch = (protos, keyword, gameworld) => protos,
		GetEditHeader = item => $"Combat Arena #{item.Id:N0} ({item.Name})",
		DefaultCommandHelp = @"You can use the combat arena builder with the following syntax:

	#3combatarena list#0 - lists combat arenas
	#3combatarena show <which>#0 - shows a particular arena
	#3combatarena edit <which>#0 - opens an arena for editing
	#3combatarena set <...>#0 - issues a building command to the open arena
	#3combatarena close#0 - closes the currently edited arena
	#3combatarena new <name> <zone> <currency>#0 - creates a new arena".SubstituteANSIColour()
	};

	public static EditableItemHelper CombatantClassHelper { get; } = new()
	{
		ItemName = "Combatant Class",
		ItemNamePlural = "Combatant Classes",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<ICombatantClass>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<ICombatantClass>(actor) { EditingItem = (ICombatantClass)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<ICombatantClass>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.CombatArenas.SelectMany(x => x.CombatantClasses).ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.CombatArenas.SelectMany(x => x.CombatantClasses)
			.FirstOrDefault(x => x.Id == id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.CombatArenas.SelectMany(x => x.CombatantClasses)
			.FirstOrDefault(x => x.Id.ToString("N0").EqualTo(input) || x.Name.EqualTo(input)),
		AddItemToGameWorldAction = _ => { },
		CastToType = typeof(ICombatantClass),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which arena is this combatant class for?");
				return;
			}

			var arena = actor.Gameworld.CombatArenas.GetByIdOrName(input.PopSpeech());
			if (arena == null)
			{
				actor.OutputHandler.Send("There is no such combat arena.");
				return;
			}

			if (arena is not CombatArena concreteArena)
			{
				actor.OutputHandler.Send("That arena type does not support editing combatant classes.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("What name should this combatant class have?");
				return;
			}

			var name = input.PopSpeech().TitleCase();
			if (arena.CombatantClasses.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send("That arena already has a combatant class with that name.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which eligibility prog should this class use? It must return a boolean.");
				return;
			}

			var prog = actor.Gameworld.FutureProgs.GetByIdOrName(input.SafeRemainingArgument);
			if (prog == null)
			{
				actor.OutputHandler.Send("There is no such prog.");
				return;
			}

			if (!prog.ReturnType.CompatibleWith(ProgVariableTypes.Boolean))
			{
				actor.OutputHandler.Send("That prog does not return a boolean value.");
				return;
			}

			var cls = new ArenaCombatantClass(concreteArena, name, prog);
			actor.RemoveAllEffects<BuilderEditingEffect<ICombatantClass>>();
			actor.AddEffect(new BuilderEditingEffect<ICombatantClass>(actor) { EditingItem = cls });
			actor.OutputHandler.Send(
				$"You create combatant class #{cls.Id.ToStringN0(actor)} ({cls.Name.ColourName()}), which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			actor.OutputHandler.Send("Cloning combatant classes is not currently supported.");
		},
		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Arena",
			"Eligibility Prog",
			"NPC Loader",
			"Resurrect"
		},
		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<ICombatantClass>()
			select new List<string>
			{
				proto.Id.ToString("N0", character),
				proto.Name,
				proto.Arena.Name,
				proto.EligibilityProg?.MXPClickableFunctionName() ?? "None",
				proto.AdminNpcLoaderProg?.MXPClickableFunctionName() ?? "None",
				proto.ResurrectNpcOnDeath.ToColouredString()
			},
		CustomSearch = (protos, keyword, gameworld) => protos,
		GetEditHeader = item => $"Combatant Class #{item.Id:N0} ({item.Name})",
		DefaultCommandHelp = @"You can use the combatant class builder with the following syntax:

	#3combatantclass list#0 - lists combatant classes
	#3combatantclass show <which>#0 - shows a combatant class
	#3combatantclass edit <which>#0 - opens a class for editing
	#3combatantclass set <...>#0 - issues a building command to the open class
	#3combatantclass close#0 - closes the current class
	#3combatantclass new <arena> <name> <eligibility prog>#0 - creates a new class".SubstituteANSIColour()
	};

	public static EditableItemHelper ArenaEventHelper { get; } = new()
	{
		ItemName = "Arena Event",
		ItemNamePlural = "Arena Events",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IArenaEvent>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IArenaEvent>(actor) { EditingItem = (IArenaEvent)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IArenaEvent>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.CombatArenas.SelectMany(x => x.ActiveEvents).ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.CombatArenas.SelectMany(x => x.ActiveEvents)
			.FirstOrDefault(x => x.Id == id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.CombatArenas.SelectMany(x => x.ActiveEvents)
			.FirstOrDefault(x => x.Id.ToString("N0").EqualTo(input) || x.Name.EqualTo(input)),
		AddItemToGameWorldAction = _ => { },
		CastToType = typeof(IArenaEvent),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which arena is this event for?");
				return;
			}

			var arena = actor.Gameworld.CombatArenas.GetByIdOrName(input.PopSpeech());
			if (arena == null)
			{
				actor.OutputHandler.Send("There is no such arena.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which event type should this event use?");
				return;
			}

			var typeText = input.PopSpeech();
			var eventType = arena.EventTypes.FirstOrDefault(x => x.Name.EqualTo(typeText)) ??
			                arena.EventTypes.FirstOrDefault(x => x.Id.ToString("N0").EqualTo(typeText));
			if (eventType == null)
			{
				actor.OutputHandler.Send("There is no such event type for that arena.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("When should this event be scheduled for?");
				return;
			}

			if (!DateTime.TryParse(input.SafeRemainingArgument, actor.Account.Culture, DateTimeStyles.None, out var when))
			{
				actor.OutputHandler.Send("That is not a valid date/time.");
				return;
			}

			var evt = arena.CreateEvent(eventType, when.ToUniversalTime());
			actor.RemoveAllEffects<BuilderEditingEffect<IArenaEvent>>();
			actor.AddEffect(new BuilderEditingEffect<IArenaEvent>(actor) { EditingItem = evt });
			actor.OutputHandler.Send(
				$"You create arena event #{evt.Id.ToStringN0(actor)} ({evt.Name.ColourName()}), which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			actor.OutputHandler.Send("Cloning arena events is not supported.");
		},
		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Arena",
			"Type",
			"State",
			"Scheduled"
		},
		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IArenaEvent>()
			select new List<string>
			{
				proto.Id.ToString("N0", character),
				proto.Name,
				proto.Arena.Name,
				proto.EventType.Name,
				proto.State.DescribeEnum(),
				proto.ScheduledAt.ToString("g", character)
			},
		CustomSearch = (protos, keyword, gameworld) => protos,
		GetEditHeader = item => $"Arena Event #{item.Id:N0} ({item.Name})",
		DefaultCommandHelp = @"You can use the arena event builder with the following syntax:

	#3arenaevent list#0 - lists arena events
	#3arenaevent show <which>#0 - shows an arena event
	#3arenaevent edit <which>#0 - opens an arena event for editing
	#3arenaevent edit new <arena> <eventtype> <datetime>#0 - creates a new arena event
	#3arenaevent set <...>#0 - issues a building command to the open event
	#3arenaevent close#0 - closes the current arena event".SubstituteANSIColour()
	};
}
