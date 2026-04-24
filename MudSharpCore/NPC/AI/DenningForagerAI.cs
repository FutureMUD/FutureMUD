#nullable enable
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
using MudSharp.Work.Crafts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.NPC.AI;

public class DenningForagerAI : TerritorialForagerAI
{
	public ICraft? BurrowCraft { get; private set; }
	public IFutureProg BurrowSiteProg { get; private set; } = null!;
	public IFutureProg? HomeLocationProg { get; private set; }
	public IFutureProg BuildEnabledProg { get; private set; } = null!;
	public IFutureProg? AnchorItemProg { get; private set; }

	protected DenningForagerAI(ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld)
	{
	}

	private DenningForagerAI(IFuturemud gameworld, string name) : base(gameworld, name, "DenningForager")
	{
		BurrowSiteProg = Gameworld.AlwaysTrueProg;
		BuildEnabledProg = Gameworld.AlwaysTrueProg;
		DatabaseInitialise();
	}

	private DenningForagerAI()
	{
	}

	public override bool IsReadyToBeUsed => base.IsReadyToBeUsed &&
	                                        BurrowSiteProg is not null &&
	                                        BuildEnabledProg is not null;

	public static new void RegisterLoader()
	{
		RegisterAIType("DenningForager", (ai, gameworld) => new DenningForagerAI(ai, gameworld));
		RegisterAIBuilderInformation("denningforager",
			(gameworld, name) => new DenningForagerAI(gameworld, name),
			new DenningForagerAI().HelpText);
	}

	protected override void LoadFromXML(XElement root)
	{
		base.LoadFromXML(root);
		long craftId = long.Parse(root.Element("BurrowCraftId")?.Value ?? "0");
		BurrowCraft = craftId > 0 ? Gameworld.Crafts.Get(craftId) : null;
		BurrowSiteProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("BurrowSiteProg")?.Value ?? "0")) ??
		                 Gameworld.AlwaysTrueProg;
		long homeProgId = long.Parse(root.Element("HomeLocationProg")?.Value ?? "0");
		HomeLocationProg = homeProgId > 0 ? Gameworld.FutureProgs.Get(homeProgId) : null;
		BuildEnabledProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("BuildEnabledProg")?.Value ?? "0")) ??
		                   Gameworld.AlwaysTrueProg;
		long anchorProgId = long.Parse(root.Element("AnchorItemProg")?.Value ?? "0");
		AnchorItemProg = anchorProgId > 0 ? Gameworld.FutureProgs.Get(anchorProgId) : null;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("SuitableTerritoryProg", SuitableTerritoryProg?.Id ?? 0),
			new XElement("DesiredTerritorySizeProg", DesiredTerritorySizeProg?.Id ?? 0),
			new XElement("WanderChancePerMinute", WanderChancePerMinute),
			new XElement("WanderEmote", new XCData(WanderEmote)),
			new XElement("BurrowCraftId", BurrowCraft?.Id ?? 0),
			new XElement("BurrowSiteProg", BurrowSiteProg?.Id ?? 0),
			new XElement("HomeLocationProg", HomeLocationProg?.Id ?? 0),
			new XElement("BuildEnabledProg", BuildEnabledProg?.Id ?? 0),
			new XElement("AnchorItemProg", AnchorItemProg?.Id ?? 0),
			new XElement("OpenDoors", OpenDoors),
			new XElement("UseKeys", UseKeys),
			new XElement("SmashLockedDoors", SmashLockedDoors),
			new XElement("CloseDoorsBehind", CloseDoorsBehind),
			new XElement("UseDoorguards", UseDoorguards),
			new XElement("MoveEvenIfObstructionInWay", MoveEvenIfObstructionInWay),
			new XElement("WillShareTerritory", WillShareTerritory),
			new XElement("WillShareTerritoryWithOtherRaces", WillShareTerritoryWithOtherRaces)
		).ToString();
	}

	public override string Show(ICharacter actor)
	{
		StringBuilder sb = new(base.Show(actor));
		sb.AppendLine($"Burrow Craft: {BurrowCraft?.Name.ColourName() ?? "None".ColourError()}");
		sb.AppendLine($"Burrow Site Prog: {BurrowSiteProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Home Location Prog: {HomeLocationProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Build Enabled Prog: {BuildEnabledProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Anchor Item Prog: {AnchorItemProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		return sb.ToString();
	}

	protected override string TypeHelpText => $@"{base.TypeHelpText}
	#3craft <craft>#0 - sets the optional craft used to build the burrow
	#3site <prog>#0 - sets the prog that chooses suitable burrow cells
	#3home <prog>#0 - sets the fallback home location prog
	#3home clear#0 - clears the fallback home location prog
	#3enabled <prog>#0 - sets the prog that controls whether burrow building is active
	#3anchor <prog>#0 - sets the prog that identifies the completed burrow anchor item
	#3anchor clear#0 - clears the anchor-identification prog";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "craft":
			case "burrow":
			case "burrowcraft":
				return BuildingCommandCraft(actor, command);
			case "site":
			case "siteprog":
			case "burrowsite":
			case "burrowsiteprog":
				return BuildingCommandSiteProg(actor, command);
			case "home":
			case "homeprog":
			case "homebase":
			case "homebaseprog":
				return BuildingCommandHomeProg(actor, command);
			case "enabled":
			case "enabledprog":
				return BuildingCommandEnabledProg(actor, command);
			case "anchor":
			case "anchorprog":
				return BuildingCommandAnchorProg(actor, command);
		}

		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandCraft(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which craft should this AI use to build its burrow? Use #3clear#0 to remove it."
			                         .SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "remove", "delete"))
		{
			BurrowCraft = null;
			Changed = true;
			actor.OutputHandler.Send("This AI will no longer use a craft to build its burrow.");
			return true;
		}

		ICraft? craft = Gameworld.Crafts.GetByIdOrName(command.SafeRemainingArgument);
		if (craft is null)
		{
			actor.OutputHandler.Send("There is no such craft.");
			return false;
		}

		BurrowCraft = craft;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now use the craft {craft.Name.ColourName()} to build its burrow.");
		return true;
	}

	private bool BuildingCommandSiteProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should decide whether a cell is suitable for a burrow?");
			return false;
		}

		IFutureProg? prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean,
			new[] { ProgVariableTypes.Character, ProgVariableTypes.Location }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		BurrowSiteProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now use {prog.MXPClickableFunctionName()} to choose burrow sites.");
		return true;
	}

	private bool BuildingCommandHomeProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should return the fallback home location? Use #3clear#0 to remove it.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "remove", "delete"))
		{
			HomeLocationProg = null;
			Changed = true;
			actor.OutputHandler.Send("This AI will now remember home bases from burrow-site selection only.");
			return true;
		}

		IFutureProg? prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Location,
			new[] { ProgVariableTypes.Character }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		HomeLocationProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now use {prog.MXPClickableFunctionName()} as its fallback home source.");
		return true;
	}

	private bool BuildingCommandEnabledProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should control whether burrow building is enabled?");
			return false;
		}

		IFutureProg? prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean,
			new[] { ProgVariableTypes.Character }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		BuildEnabledProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now use {prog.MXPClickableFunctionName()} to determine whether it should build.");
		return true;
	}

	private bool BuildingCommandAnchorProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should identify the completed burrow item? Use #3clear#0 to remove it.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "remove", "delete"))
		{
			AnchorItemProg = null;
			Changed = true;
			actor.OutputHandler.Send("This AI will now use its fallback burrow-anchor detection.");
			return true;
		}

		IFutureProg? prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean,
			new[] { ProgVariableTypes.Character, ProgVariableTypes.Item }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		AnchorItemProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now use {prog.MXPClickableFunctionName()} to identify its burrow item.");
		return true;
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		ICharacter? ch = arguments[0] as ICharacter;
		if (ch is null || ch.State.IsDead() || ch.State.IsInStatis())
		{
			return false;
		}

		if (!ForagerAIHelpers.IsHungry(ch))
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
					EvaluateBurrowLifecycle(ch);
					break;
			}

			if (type == EventType.MinuteTick)
			{
				return false;
			}
		}

		return base.HandleEvent(type, arguments);
	}

	public override bool HandlesEvent(params EventType[] types)
	{
		foreach (EventType type in types)
		{
			switch (type)
			{
				case EventType.CharacterStopMovement:
					return true;
			}
		}

		return base.HandlesEvent(types);
	}

	private bool IsBuildEnabled(ICharacter character)
	{
		return BuildEnabledProg.ExecuteBool(true, character);
	}

	private NpcHomeBaseEffect ResolveHomeBase(ICharacter character)
	{
		NpcHomeBaseEffect home = NpcHomeBaseEffect.GetOrCreate(character);
		if (home.HomeCell is not null)
		{
			return home;
		}

		if (HomeLocationProg?.Execute<ICell?>(character) is ICell location)
		{
			home.SetHomeCell(location);
		}

		return home;
	}

	private void EvaluateBurrowLifecycle(ICharacter character)
	{
		if (character.Movement is not null ||
		    character.Combat is not null ||
		    character.Effects.Any(x => x.IsBlockingEffect("movement")) ||
		    character.EffectsOfType<IActiveCraftEffect>().Any(x => !ReferenceEquals(x.Component.Craft, BurrowCraft)))
		{
			return;
		}

		NpcHomeBaseEffect home = ResolveHomeBase(character);
		if (home.HomeCell is null)
		{
			if (BurrowSiteProg.ExecuteBool(false, character, character.Location))
			{
				home.SetHomeCell(character.Location);
			}
			else
			{
				CheckPathingEffect(character, true);
				return;
			}
		}

		if (!ReferenceEquals(home.HomeCell, character.Location))
		{
			CheckPathingEffect(character, true);
			return;
		}

		RefreshAnchorItem(character, home);
		if (home.AnchorItem is not null || BurrowCraft is null || !IsBuildEnabled(character))
		{
			return;
		}

		IActiveCraftGameItemComponent? interruptedCraft = character.Location.LayerGameItems(character.RoomLayer)
			.SelectNotNull(x => x.GetItemType<IActiveCraftGameItemComponent>())
			.FirstOrDefault(x => ReferenceEquals(x.Craft, BurrowCraft));
		if (interruptedCraft is not null)
		{
			(bool canResume, string _) = BurrowCraft.CanResumeCraft(character, interruptedCraft);
			if (canResume)
			{
				BurrowCraft.ResumeCraft(character, interruptedCraft);
			}

			return;
		}

		(bool canDoCraft, string _) = BurrowCraft.CanDoCraft(character, null, true, true);
		if (canDoCraft)
		{
			BurrowCraft.BeginCraft(character);
		}
	}

	private void RefreshAnchorItem(ICharacter character, NpcHomeBaseEffect home)
	{
		if (home.AnchorItem is not null && ReferenceEquals(home.AnchorItem.Location, home.HomeCell))
		{
			return;
		}

		home.ClearAnchorItem();
		IGameItem? anchor = DenBuilderAI.SelectAnchorItem(character, AnchorItemProg);
		if (anchor is not null)
		{
			home.SetAnchorItem(anchor);
		}
	}

	protected override bool WouldMove(ICharacter ch)
	{
		if (ForagerAIHelpers.IsHungry(ch))
		{
			return base.WouldMove(ch);
		}

		if (ch.Combat is not null ||
		    ch.EffectsOfType<IActiveCraftEffect>().Any(x => ReferenceEquals(x.Component.Craft, BurrowCraft)))
		{
			return false;
		}

		NpcHomeBaseEffect home = ResolveHomeBase(ch);
		return home.HomeCell is null || !ReferenceEquals(ch.Location, home.HomeCell);
	}

	protected override (ICell Target, IEnumerable<ICellExit>) GetPath(ICharacter ch)
	{
		if (ForagerAIHelpers.IsHungry(ch))
		{
			return base.GetPath(ch);
		}

		NpcHomeBaseEffect home = ResolveHomeBase(ch);
		if (home.HomeCell is not null && !ReferenceEquals(home.HomeCell, ch.Location))
		{
			List<ICellExit> homePath = ch.PathBetween(home.HomeCell, 20, GetSuitabilityFunction(ch)).ToList();
			return homePath.Any()
				? (home.HomeCell, homePath)
				: (null, Enumerable.Empty<ICellExit>());
		}

		if (home.HomeCell is not null)
		{
			return (null, Enumerable.Empty<ICellExit>());
		}

		Tuple<IPerceivable, IEnumerable<ICellExit>> targetPath = ch.AcquireTargetAndPath(
			x => x is ICell cell && BurrowSiteProg.ExecuteBool(false, ch, cell),
			20,
			GetSuitabilityFunction(ch));
		return targetPath.Item1 is ICell burrowCell && targetPath.Item2.Any()
			? (burrowCell, targetPath.Item2)
			: (null, Enumerable.Empty<ICellExit>());
	}
}
