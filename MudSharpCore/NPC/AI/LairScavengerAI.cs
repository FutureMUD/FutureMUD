#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Models;

namespace MudSharp.NPC.AI;

public class LairScavengerAI : PathingAIBase
{
	public IFutureProg WillScavengeItemProg { get; private set; } = null!;
	public IFutureProg ScavengingEnabledProg { get; private set; } = null!;
	public IFutureProg? HomeLocationProg { get; private set; }

	protected LairScavengerAI(ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld)
	{
	}

	private LairScavengerAI()
	{
	}

	private LairScavengerAI(IFuturemud gameworld, string name) : base(gameworld, name, "LairScavenger")
	{
		WillScavengeItemProg = Gameworld.AlwaysFalseProg;
		ScavengingEnabledProg = Gameworld.AlwaysTrueProg;
		OpenDoors = false;
		UseKeys = false;
		UseDoorguards = false;
		CloseDoorsBehind = false;
		MoveEvenIfObstructionInWay = false;
		SmashLockedDoors = false;
		DatabaseInitialise();
	}

	public static void RegisterLoader()
	{
		RegisterAIType("LairScavenger", (ai, gameworld) => new LairScavengerAI(ai, gameworld));
		RegisterAIBuilderInformation("lairscavenger",
			(gameworld, name) => new LairScavengerAI(gameworld, name),
			new LairScavengerAI().HelpText);
	}

	protected override void LoadFromXML(XElement root)
	{
		base.LoadFromXML(root);
		WillScavengeItemProg =
			Gameworld.FutureProgs.Get(long.Parse(root.Element("WillScavengeItemProg")?.Value ?? "0")) ??
			Gameworld.AlwaysFalseProg;
		ScavengingEnabledProg =
			Gameworld.FutureProgs.Get(long.Parse(root.Element("ScavengingEnabledProg")?.Value ?? "0")) ??
			Gameworld.AlwaysTrueProg;
		var homeProgId = long.Parse(root.Element("HomeLocationProg")?.Value ?? "0");
		HomeLocationProg = homeProgId > 0 ? Gameworld.FutureProgs.Get(homeProgId) : null;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("WillScavengeItemProg", WillScavengeItemProg?.Id ?? 0),
			new XElement("ScavengingEnabledProg", ScavengingEnabledProg?.Id ?? 0),
			new XElement("HomeLocationProg", HomeLocationProg?.Id ?? 0),
			new XElement("OpenDoors", OpenDoors),
			new XElement("UseKeys", UseKeys),
			new XElement("SmashLockedDoors", SmashLockedDoors),
			new XElement("CloseDoorsBehind", CloseDoorsBehind),
			new XElement("UseDoorguards", UseDoorguards),
			new XElement("MoveEvenIfObstructionInWay", MoveEvenIfObstructionInWay)
		).ToString();
	}

	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder(base.Show(actor));
		sb.AppendLine($"Scavenge Prog: {WillScavengeItemProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Enabled Prog: {ScavengingEnabledProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Fallback Home Prog: {HomeLocationProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		return sb.ToString();
	}

	protected override string TypeHelpText => $@"{base.TypeHelpText}
	#3will <prog>#0 - sets the prog that decides whether an item is worth scavenging
	#3enabled <prog>#0 - sets the prog that controls whether scavenging is enabled
	#3home <prog>#0 - sets a fallback home location prog for NPCs without a remembered lair
	#3home clear#0 - clears the fallback home location prog";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "will":
			case "willprog":
				return BuildingCommandWillProg(actor, command);
			case "enabled":
			case "enabledprog":
				return BuildingCommandEnabledProg(actor, command);
			case "home":
			case "homeprog":
				return BuildingCommandHomeProg(actor, command);
		}

		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandWillProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should decide whether an item is worth scavenging?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean,
			new[] { ProgVariableTypes.Character, ProgVariableTypes.Item }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		WillScavengeItemProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now use {prog.MXPClickableFunctionName()} to evaluate scavenging targets.");
		return true;
	}

	private bool BuildingCommandEnabledProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should control whether scavenging is enabled?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean,
			new[] { ProgVariableTypes.Character }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		ScavengingEnabledProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now use {prog.MXPClickableFunctionName()} to determine whether it scavenges.");
		return true;
	}

	private bool BuildingCommandHomeProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should return the fallback lair location? Use #3clear#0 to remove it.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "remove", "none", "delete"))
		{
			HomeLocationProg = null;
			Changed = true;
			actor.OutputHandler.Send("This AI will now rely solely on remembered home-base state.");
			return true;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Location,
			new[] { ProgVariableTypes.Character }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		HomeLocationProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now use {prog.MXPClickableFunctionName()} as its fallback lair source.");
		return true;
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		var ch = arguments[0] as ICharacter;
		if (ch is null || ch.State.IsDead() || ch.State.IsInStatis())
		{
			return false;
		}

		switch (type)
		{
			case EventType.CharacterEntersGame:
			case EventType.CharacterEnterCellFinish:
			case EventType.CharacterStopMovement:
			case EventType.CharacterStopMovementClosedDoor:
			case EventType.LeaveCombat:
			case EventType.MinuteTick:
			case EventType.NPCOnGameLoadFinished:
				EvaluateScavenging(ch);
				break;
		}

		return base.HandleEvent(type, arguments);
	}

	public override bool HandlesEvent(params EventType[] types)
	{
		foreach (var type in types)
		{
			switch (type)
			{
				case EventType.CharacterEntersGame:
				case EventType.CharacterEnterCellFinish:
				case EventType.CharacterStopMovement:
				case EventType.CharacterStopMovementClosedDoor:
				case EventType.LeaveCombat:
				case EventType.MinuteTick:
				case EventType.NPCOnGameLoadFinished:
					return true;
			}
		}

		return base.HandlesEvent(types);
	}

	internal static IEnumerable<IGameItem> GetScavengedItems(ICharacter character, IFutureProg willScavengeItemProg)
	{
		return character.Body.HeldOrWieldedItems
			.Where(x => willScavengeItemProg.ExecuteBool(false, character, x));
	}

	internal static IGameItem? FindVisibleScavengeItem(ICharacter character, IFutureProg willScavengeItemProg,
		IGameItem? excludedItem = null)
	{
		return character.Location.LayerGameItems(character.RoomLayer)
			.Where(x => x != excludedItem)
			.Where(x => character.CanSee(x))
			.Where(x => character.Body.CanGet(x, 0))
			.FirstOrDefault(x => willScavengeItemProg.ExecuteBool(false, character, x));
	}

	private bool IsEnabled(ICharacter character)
	{
		return ScavengingEnabledProg.ExecuteBool(true, character);
	}

	private NpcHomeBaseEffect ResolveHomeBase(ICharacter character)
	{
		var home = NpcHomeBaseEffect.GetOrCreate(character);
		if (home.HomeCell is not null || HomeLocationProg is null)
		{
			return home;
		}

		var location = HomeLocationProg.Execute<ICell?>(character);
		if (location is not null)
		{
			home.SetHomeCell(location);
		}

		return home;
	}

	private void EvaluateScavenging(ICharacter character)
	{
		if (!IsEnabled(character) ||
		    character.Movement is not null ||
		    character.Combat is not null ||
		    character.Effects.Any(x => x.IsBlockingEffect("movement")) ||
		    character.EffectsOfType<IActiveCraftEffect>().Any())
		{
			return;
		}

		var home = ResolveHomeBase(character);
		var carriedLoot = GetScavengedItems(character, WillScavengeItemProg).ToList();
		if (carriedLoot.Any())
		{
			if (home.HomeCell is null)
			{
				return;
			}

			if (!ReferenceEquals(character.Location, home.HomeCell))
			{
				CheckPathingEffect(character, true);
				return;
			}

			DepositLoot(character, home, carriedLoot);
			return;
		}

		var targetItem = FindVisibleScavengeItem(character, WillScavengeItemProg, home.AnchorItem);
		if (targetItem is not null)
		{
			character.Body.Get(targetItem, silent: true);
			if (home.HomeCell is not null && !ReferenceEquals(character.Location, home.HomeCell))
			{
				CheckPathingEffect(character, true);
			}

			return;
		}

		CheckPathingEffect(character, true);
	}

	private void DepositLoot(ICharacter character, NpcHomeBaseEffect home, IEnumerable<IGameItem> carriedLoot)
	{
		var anchorContainer = home.AnchorItem?.GetItemType<IContainer>() is not null &&
		                      ReferenceEquals(home.AnchorItem.Location, character.Location)
			? home.AnchorItem
			: null;

		foreach (var item in carriedLoot.ToList())
		{
			if (anchorContainer is not null && character.Body.CanPut(item, anchorContainer, null, 0, true))
			{
				character.Body.Put(item, anchorContainer, null, silent: true);
				continue;
			}

			if (character.Body.CanDrop(item, 0))
			{
				character.Body.Drop(item, silent: true);
			}
		}
	}

	protected override bool WouldMove(ICharacter ch)
	{
		if (!IsEnabled(ch) ||
		    ch.Combat is not null ||
		    ch.EffectsOfType<IActiveCraftEffect>().Any())
		{
			return false;
		}

		var home = ResolveHomeBase(ch);
		var carriedLoot = GetScavengedItems(ch, WillScavengeItemProg).Any();
		if (carriedLoot)
		{
			return home.HomeCell is not null && !ReferenceEquals(ch.Location, home.HomeCell);
		}

		if (FindVisibleScavengeItem(ch, WillScavengeItemProg, home.AnchorItem) is not null)
		{
			return false;
		}

		return true;
	}

	protected override (ICell Target, IEnumerable<ICellExit>) GetPath(ICharacter ch)
	{
		var home = ResolveHomeBase(ch);
		if (GetScavengedItems(ch, WillScavengeItemProg).Any())
		{
			if (home.HomeCell is null || ReferenceEquals(ch.Location, home.HomeCell))
			{
				return (null, Enumerable.Empty<ICellExit>());
			}

			var homePath = ch.PathBetween(home.HomeCell, 20, GetSuitabilityFunction(ch)).ToList();
			return homePath.Any()
				? (home.HomeCell, homePath)
				: (null, Enumerable.Empty<ICellExit>());
		}

		var target = ch.AcquireTargetAndPath(
			x => x is IGameItem item &&
			     item != home.AnchorItem &&
			     WillScavengeItemProg.ExecuteBool(false, ch, item),
			20,
			GetSuitabilityFunction(ch));
		return target.Item1?.Location is not null && target.Item2.Any()
			? (target.Item1.Location, target.Item2)
			: (null, Enumerable.Empty<ICellExit>());
	}
}
