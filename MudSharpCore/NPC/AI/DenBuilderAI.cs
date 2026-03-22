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
using MudSharp.Work.Crafts;

namespace MudSharp.NPC.AI;

public class DenBuilderAI : PathingAIBase
{
	public ICraft? DenCraft { get; private set; }
	public IFutureProg DenSiteProg { get; private set; } = null!;
	public IFutureProg BuildEnabledProg { get; private set; } = null!;
	public IFutureProg WillDefendDenProg { get; private set; } = null!;
	public IFutureProg? AnchorItemProg { get; private set; }

	protected DenBuilderAI(ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld)
	{
	}

	private DenBuilderAI()
	{
	}

	private DenBuilderAI(IFuturemud gameworld, string name) : base(gameworld, name, "DenBuilder")
	{
		DenSiteProg = Gameworld.AlwaysTrueProg;
		BuildEnabledProg = Gameworld.AlwaysTrueProg;
		WillDefendDenProg = Gameworld.AlwaysFalseProg;
		OpenDoors = false;
		UseKeys = false;
		UseDoorguards = false;
		CloseDoorsBehind = false;
		MoveEvenIfObstructionInWay = false;
		SmashLockedDoors = false;
		DatabaseInitialise();
	}

	public override bool IsReadyToBeUsed => DenCraft is not null;

	public static void RegisterLoader()
	{
		RegisterAIType("DenBuilder", (ai, gameworld) => new DenBuilderAI(ai, gameworld));
		RegisterAIBuilderInformation("denbuilder",
			(gameworld, name) => new DenBuilderAI(gameworld, name),
			new DenBuilderAI().HelpText);
	}

	protected override void LoadFromXML(XElement root)
	{
		base.LoadFromXML(root);
		var craftId = long.Parse(root.Element("DenCraftId")?.Value ?? "0");
		DenCraft = craftId > 0 ? Gameworld.Crafts.Get(craftId) : null;
		DenSiteProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("DenSiteProg")?.Value ?? "0")) ?? Gameworld.AlwaysTrueProg;
		BuildEnabledProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("BuildEnabledProg")?.Value ?? "0")) ?? Gameworld.AlwaysTrueProg;
		WillDefendDenProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("WillDefendDenProg")?.Value ?? "0")) ?? Gameworld.AlwaysFalseProg;
		var anchorProgId = long.Parse(root.Element("AnchorItemProg")?.Value ?? "0");
		AnchorItemProg = anchorProgId > 0 ? Gameworld.FutureProgs.Get(anchorProgId) : null;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("DenCraftId", DenCraft?.Id ?? 0),
			new XElement("DenSiteProg", DenSiteProg?.Id ?? 0),
			new XElement("BuildEnabledProg", BuildEnabledProg?.Id ?? 0),
			new XElement("WillDefendDenProg", WillDefendDenProg?.Id ?? 0),
			new XElement("AnchorItemProg", AnchorItemProg?.Id ?? 0),
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
		sb.AppendLine($"Den Craft: {DenCraft?.Name.ColourName() ?? "None".ColourError()}");
		sb.AppendLine($"Den Site Prog: {DenSiteProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Build Enabled Prog: {BuildEnabledProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Defend Den Prog: {WillDefendDenProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Anchor Item Prog: {AnchorItemProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		return sb.ToString();
	}

	protected override string TypeHelpText => $@"{base.TypeHelpText}
	#3craft <craft>#0 - sets the craft used to build the den or nest
	#3site <prog>#0 - sets the prog that chooses suitable den cells
	#3enabled <prog>#0 - sets the prog that controls whether den building is active
	#3defend <prog>#0 - sets the prog that decides who to attack near the den
	#3anchor <prog>#0 - sets the prog that identifies the completed den anchor item
	#3anchor clear#0 - clears the anchor-identification prog";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "craft":
				return BuildingCommandCraft(actor, command);
			case "site":
			case "siteprog":
				return BuildingCommandSiteProg(actor, command);
			case "enabled":
			case "enabledprog":
				return BuildingCommandEnabledProg(actor, command);
			case "defend":
			case "defendprog":
				return BuildingCommandDefendProg(actor, command);
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
			actor.OutputHandler.Send("Which craft should this AI use to build its den?");
			return false;
		}

		var craft = Gameworld.Crafts.GetByIdOrName(command.SafeRemainingArgument);
		if (craft is null)
		{
			actor.OutputHandler.Send("There is no such craft.");
			return false;
		}

		DenCraft = craft;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now use the craft {craft.Name.ColourName()} to build its den.");
		return true;
	}

	private bool BuildingCommandSiteProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should decide whether a cell is suitable for a den?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean,
			new[] { ProgVariableTypes.Character, ProgVariableTypes.Location }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		DenSiteProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now use {prog.MXPClickableFunctionName()} to choose den sites.");
		return true;
	}

	private bool BuildingCommandEnabledProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should control whether den building is enabled?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
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

	private bool BuildingCommandDefendProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should decide who this AI will defend its den against?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean,
			new[] { ProgVariableTypes.Character, ProgVariableTypes.Character }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		WillDefendDenProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now use {prog.MXPClickableFunctionName()} to defend its den.");
		return true;
	}

	private bool BuildingCommandAnchorProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should identify the completed den item? Use #3clear#0 to remove it.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "remove", "delete"))
		{
			AnchorItemProg = null;
			Changed = true;
			actor.OutputHandler.Send("This AI will now use its fallback den-anchor detection.");
			return true;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean,
			new[] { ProgVariableTypes.Character, ProgVariableTypes.Item }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		AnchorItemProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now use {prog.MXPClickableFunctionName()} to identify its den anchor item.");
		return true;
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		var ch = arguments[0] as ICharacter;
		if (ch is null || ch.State.IsDead() || ch.State.IsInStatis())
		{
			return false;
		}

		if (type == EventType.MinuteTick)
		{
			EvaluateDenLifecycle(ch);
		}

		return base.HandleEvent(type, arguments);
	}

	public override bool HandlesEvent(params EventType[] types)
	{
		foreach (var type in types)
		{
			if (type == EventType.MinuteTick)
			{
				return true;
			}
		}

		return base.HandlesEvent(types);
	}

	internal static IGameItem? SelectAnchorItem(ICharacter character, IFutureProg? anchorItemProg)
	{
		return character.Location.LayerGameItems(character.RoomLayer)
			.Where(x => x.GetItemType<IActiveCraftGameItemComponent>() is null)
			.Where(x => anchorItemProg?.ExecuteBool(character, x) != false)
			.FirstOrDefault();
	}

	private void EvaluateDenLifecycle(ICharacter character)
	{
		if (!BuildEnabledProg.ExecuteBool(false, character))
		{
			return;
		}

		var home = NpcHomeBaseEffect.GetOrCreate(character);
		if (home.HomeCell is null)
		{
			if (DenSiteProg.ExecuteBool(false, character, character.Location))
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

		var activeCraftEffect = character.EffectsOfType<IActiveCraftEffect>()
			.FirstOrDefault(x => ReferenceEquals(x.Component.Craft, DenCraft));
		if (activeCraftEffect is not null)
		{
			TryDefendDen(character);
			return;
		}

		RefreshAnchorItem(character, home);
		if (home.AnchorItem is not null)
		{
			TryDefendDen(character);
			return;
		}

		if (DenCraft is null)
		{
			return;
		}

		var interruptedCraft = character.Location.LayerGameItems(character.RoomLayer)
			.SelectNotNull(x => x.GetItemType<IActiveCraftGameItemComponent>())
			.FirstOrDefault(x => ReferenceEquals(x.Craft, DenCraft));
		if (interruptedCraft is not null)
		{
			var (canResume, _) = DenCraft.CanResumeCraft(character, interruptedCraft);
			if (canResume)
			{
				DenCraft.ResumeCraft(character, interruptedCraft);
			}

			return;
		}

		var (canDoCraft, _) = DenCraft.CanDoCraft(character, null, true, true);
		if (canDoCraft)
		{
			DenCraft.BeginCraft(character);
		}
	}

	private void RefreshAnchorItem(ICharacter character, NpcHomeBaseEffect home)
	{
		if (home.AnchorItem is not null && ReferenceEquals(home.AnchorItem.Location, home.HomeCell))
		{
			return;
		}

		home.ClearAnchorItem();
		var anchor = SelectAnchorItem(character, AnchorItemProg);
		if (anchor is not null)
		{
			home.SetAnchorItem(anchor);
		}
	}

	private void TryDefendDen(ICharacter character)
	{
		if (character.Combat is not null || character.Movement is not null || !character.State.IsAble())
		{
			return;
		}

		var target = character.Location.LayerCharacters(character.RoomLayer)
			.Except(character)
			.Where(x => WillDefendDenProg.ExecuteBool(character, x))
			.Where(character.CanEngage)
			.FirstOrDefault();
		if (target is null)
		{
			return;
		}

		character.Engage(target);
	}

	protected override bool WouldMove(ICharacter ch)
	{
		if (!BuildEnabledProg.ExecuteBool(false, ch) || ch.Combat is not null)
		{
			return false;
		}

		if (ch.EffectsOfType<IActiveCraftEffect>().Any(x => ReferenceEquals(x.Component.Craft, DenCraft)))
		{
			return false;
		}

		return true;
	}

	protected override (ICell Target, IEnumerable<ICellExit>) GetPath(ICharacter ch)
	{
		var home = NpcHomeBaseEffect.GetOrCreate(ch);
		if (home.HomeCell is not null && !ReferenceEquals(home.HomeCell, ch.Location))
		{
			var homePath = ch.PathBetween(home.HomeCell, 20, GetSuitabilityFunction(ch)).ToList();
			return homePath.Any()
				? (home.HomeCell, homePath)
				: (null, Enumerable.Empty<ICellExit>());
		}

		if (home.HomeCell is not null)
		{
			return (null, Enumerable.Empty<ICellExit>());
		}

		var targetPath = ch.AcquireTargetAndPath(
			x => x is ICell cell && DenSiteProg.ExecuteBool(false, ch, cell),
			20,
			GetSuitabilityFunction(ch));
		return targetPath.Item1 is ICell denCell && targetPath.Item2.Any()
			? (denCell, targetPath.Item2)
			: (null, Enumerable.Empty<ICellExit>());
	}
}
