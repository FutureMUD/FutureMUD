using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg.Statements.Manipulation;
using MudSharp.GameItems.Inventory;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.Movement;
using MudSharp.NPC;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.NPC.AI;

public class SelfCareAI : ArtificialIntelligenceBase
{
	internal enum RequiredSelfCare
	{
		None,
		Bind,
		Suture
	}

	internal const string SelfCareMedicalAction = "self care medical action";
	internal const string SelfCareBleedingEmoteAction = "self care bleeding emote";

	private readonly List<string> _bleedingEmotes = new();
	public IEnumerable<string> BleedingEmotes => _bleedingEmotes;

	private SelfCareAI(ArtificialIntelligence ai, IFuturemud gameworld)
		: base(ai, gameworld)
	{
		LoadFromXml(XElement.Parse(ai.Definition));
	}

	private SelfCareAI()
	{

	}

	private SelfCareAI(IFuturemud gameworld, string name) : base(gameworld, name, "SelfCare")
	{
		BindingDelayDiceExpression = "250+1d2750";
		BleedingEmoteDelayDiceExpression = "3000+1d2000";
		DatabaseInitialise();
	}

	public string BleedingEmote => BleedingEmotes.GetRandomElement();
	public string BleedingEmoteDelayDiceExpression { get; set; }
	public string BindingDelayDiceExpression { get; set; }

	/// <inheritdoc />
	public override string Show(ICharacter actor)
	{
		StringBuilder sb = new();
		sb.AppendLine($"Artificial Intelligence #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine($"Type: {AIType.ColourValue()}");
		sb.AppendLine();
		sb.AppendLine($"Bleeding Emote Delay: {BleedingEmoteDelayDiceExpression.ColourValue()} milliseconds");
		sb.AppendLine($"Self Care Command Delay: {BindingDelayDiceExpression.ColourValue()} milliseconds");
		sb.AppendLine($"Emotes:");
		sb.AppendLine();
		if (_bleedingEmotes.Count == 0)
		{
			sb.AppendLine($"\tNone");
		}
		else
		{
			foreach (string emote in _bleedingEmotes)
			{
				sb.AppendLine($"\t{emote.ColourCommand()}");
			}
		}

		return sb.ToString();
	}

	/// <inheritdoc />
	protected override string TypeHelpText => @"	#3binddelay <expression>#0 - a dice expression in milliseconds for delay before binding or suturing
	#3bleedingdelay <expression>#0 - a dice expression in milliseconds for delay emoting about bleeding
	#3addemote <emote>#0 - adds a bleeding emote to the list. $0 is the NPC.
	#3rememote <##>#0 - removes an emote from the list";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "binddelay":
				return BuildingCommandBindDelay(actor, command);
			case "bleedingdelay":
				return BuildingCommandBleedingEmoteDelay(actor, command);
			case "addemote":
				return BuildingCommandBleedingAddEmote(actor, command);
			case "deleteemote":
			case "delemote":
			case "rememote":
			case "removeemote":
				return BuildingCommandBleedingRemoveEmote(actor, command);
		}

		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandBleedingRemoveEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which emote would you like to remove?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out int index) || index < 1 || index > _bleedingEmotes.Count)
		{
			actor.OutputHandler.Send($"You must enter a valid number between {1.ToString("N0", actor).ColourValue()} and {_bleedingEmotes.Count.ToString("N0", actor).ColourValue()}.");
			return false;
		}

		Changed = true;
		actor.OutputHandler.Send($"You delete the emote {_bleedingEmotes[index - 1].ColourCommand()}.");
		_bleedingEmotes.RemoveAt(index - 1);
		return true;
	}

	private bool BuildingCommandBleedingAddEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What emote do you want to add?");
			return false;
		}

		Emote emote = new(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		_bleedingEmotes.Add(command.SafeRemainingArgument);
		Changed = true;
		actor.OutputHandler.Send($"You add the emote {command.SafeRemainingArgument.ColourCommand()} to the list of emotes.");
		return true;
	}

	private bool BuildingCommandBleedingEmoteDelay(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter a valid dice expression for milliseconds before emoting about bleeding.");
			return false;
		}

		if (!Dice.IsDiceExpression(command.SafeRemainingArgument))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid dice expression.");
			return false;
		}

		BleedingEmoteDelayDiceExpression = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now wait {BleedingEmoteDelayDiceExpression.ColourValue()} milliseconds before emoting about bleeding.");
		return true;
	}

	private bool BuildingCommandBindDelay(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter a valid dice expression for milliseconds before entering the self-care command.");
			return false;
		}

		if (!Dice.IsDiceExpression(command.SafeRemainingArgument))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid dice expression.");
			return false;
		}

		BindingDelayDiceExpression = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now wait {BindingDelayDiceExpression.ColourValue()} milliseconds before tending to its own wounds.");
		return true;
	}

	public static void RegisterLoader()
	{
		RegisterAIType("SelfCare", (ai, gameworld) => new SelfCareAI(ai, gameworld));
		RegisterAIBuilderInformation("selfcare", (gameworld, name) => new SelfCareAI(gameworld, name), new SelfCareAI().HelpText);
	}

	private void LoadFromXml(XElement root)
	{
		BindingDelayDiceExpression = root.Element("BindingDelayDiceExpression")?.Value ?? "250+1d2750";
		BleedingEmoteDelayDiceExpression = root.Element("BleedingEmoteDelayDiceExpression")?.Value ?? "3000+1d2000";
		_bleedingEmotes.AddRange(root.Elements("BleedingEmote").Select(x => x.Value));
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("BindingDelayDiceExpression", new XCData(BindingDelayDiceExpression)),
			new XElement("BleedingEmoteDelayDiceExpression", new XCData(BleedingEmoteDelayDiceExpression)),
			from emote in BleedingEmotes select new XElement("BleedingEmote", new XCData(emote))
		).ToString();
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		ICharacter ch = null;
		switch (type)
		{
			case EventType.CharacterEnterCellFinish:
			case EventType.LeaveCombat:
			case EventType.NoLongerEngagedInMelee:
			case EventType.BleedTick:
			case EventType.TenSecondTick:
				ch = (ICharacter)arguments[0];
				break;
			case EventType.NoLongerTargettedInCombat:
				ch = (ICharacter)arguments[1];
				break;
		}

		if (ch is null || ch.State.IsDead() || ch.State.IsInStatis())
		{
			return false;
		}

		switch (type)
		{
			case EventType.CharacterEnterCellFinish:
				return HandleCharacterEnterCellFinish((ICharacter)arguments[0]);
			case EventType.LeaveCombat:
				return HandleLeaveCombat((ICharacter)arguments[0]);
			case EventType.NoLongerTargettedInCombat:
				return HandleNoLongerTargetted((ICharacter)arguments[1]);
			case EventType.NoLongerEngagedInMelee:
				return HandleNoLongerEngagedInMelee((ICharacter)arguments[0]);
			case EventType.BleedTick:
				return HandleBleedTick((ICharacter)arguments[0], (double)arguments[1]);
			case EventType.TenSecondTick:
				return HandleTenSecondTick((ICharacter)arguments[0]);
		}

		return false;
	}

	public override bool HandlesEvent(params EventType[] types)
	{
		foreach (EventType type in types)
		{
			switch (type)
			{
				case EventType.CharacterEnterCellFinish:
				case EventType.LeaveCombat:
				case EventType.NoLongerTargettedInCombat:
				case EventType.NoLongerEngagedInMelee:
				case EventType.BleedTick:
				case EventType.TenSecondTick:
					return true;
			}
		}

		return false;
	}

	internal static RequiredSelfCare DetermineRequiredSelfCare(IEnumerable<IWound> wounds, bool canSuture)
	{
		var woundList = wounds.ToList();
		if (woundList.Any(x => x.BleedStatus == BleedStatus.Bleeding))
		{
			return RequiredSelfCare.Bind;
		}

		if (canSuture &&
		    woundList.Any(x => x.BleedStatus == BleedStatus.TraumaControlled &&
		                       x.CanBeTreated(TreatmentType.Close) != Difficulty.Impossible))
		{
			return RequiredSelfCare.Suture;
		}

		return RequiredSelfCare.None;
	}

	internal static bool CellHasHostileNpcs(ICell cell, ICharacter character)
	{
		return cell?.Characters.Any(x =>
			x != character &&
			x is INPC npc &&
			!npc.AffectedBy<IPauseAIEffect>() &&
			npc.AIs.Any(y => y.CountsAsAggressive)) == true;
	}

	internal static ICellExit GetSafeExitForSelfCare(ICharacter character)
	{
		if (character.Location == null)
		{
			return null;
		}

		return character.Location.ExitsFor(character, true)
		                .Where(x => !CellHasHostileNpcs(x.Destination, character))
		                .FirstOrDefault(x => character.CanMove(
			                    x,
			                    CanMoveFlags.IgnoreCancellableActionBlockers | CanMoveFlags.IgnoreSafeMovement)
		                    .Result);
	}

	private static IEnumerable<IWound> VisibleWoundsForSelfCare(ICharacter character)
	{
		return character.VisibleWounds(character, WoundExaminationType.Examination);
	}

	private static bool HasPendingDelayedAction(ICharacter character, string actionDescription)
	{
		return character.EffectsOfType<DelayedAction>().Any(x => x.ActionDescription == actionDescription);
	}

	private static bool CanSutureSelf(ICharacter character)
	{
		return character.Gameworld.SutureInventoryPlanTemplate.CreatePlan(character).PlanIsFeasible() ==
		       InventoryPlanFeasibility.Feasible;
	}

	private void MaybeScheduleBleedingEmote(ICharacter character, RequiredSelfCare careType)
	{
		if (careType != RequiredSelfCare.Bind ||
		    !BleedingEmotes.Any() ||
		    HasPendingDelayedAction(character, SelfCareBleedingEmoteAction) ||
		    Dice.Roll("1d6") != 1)
		{
			return;
		}

		character.AddEffect(
			new DelayedAction(character,
				x => { character.OutputHandler.Handle(new EmoteOutput(new Emote(BleedingEmote, character))); },
				SelfCareBleedingEmoteAction),
			TimeSpan.FromMilliseconds(Dice.Roll(BleedingEmoteDelayDiceExpression)));
	}

	private static bool TryLeaveCombatForSelfCare(ICharacter character)
	{
		if (character.Combat == null)
		{
			return false;
		}

		if (character.Combat.Friendly)
		{
			character.Combat.TruceRequested(character);
			return true;
		}

		if (character.Combat.CanFreelyLeaveCombat(character))
		{
			character.Combat.LeaveCombat(character);
			return true;
		}

		character.CombatStrategyMode = CombatStrategyMode.Flee;
		return true;
	}

	private static bool TryMoveToSafeLocationForSelfCare(ICharacter character)
	{
		if (character.Location == null || !CellHasHostileNpcs(character.Location, character))
		{
			return false;
		}

		var exit = GetSafeExitForSelfCare(character);
		return exit != null && character.Move(exit, null, true);
	}

	private bool QueueSelfCareCommand(ICharacter character, string command)
	{
		if (HasPendingDelayedAction(character, SelfCareMedicalAction))
		{
			return false;
		}

		character.AddEffect(
			new DelayedAction(character, x => { character.ExecuteCommand(command); }, SelfCareMedicalAction),
			TimeSpan.FromMilliseconds(Dice.Roll(BindingDelayDiceExpression)));
		return true;
	}

	private bool HandleSelfCare(ICharacter character)
	{
		if (character.State.IsDisabled())
		{
			return false;
		}

		var visibleWounds = VisibleWoundsForSelfCare(character).ToList();
		var careType = DetermineRequiredSelfCare(visibleWounds, false);
		if (careType == RequiredSelfCare.None)
		{
			careType = DetermineRequiredSelfCare(visibleWounds, CanSutureSelf(character));
		}

		if (careType == RequiredSelfCare.None)
		{
			return false;
		}

		MaybeScheduleBleedingEmote(character, careType);

		if (character.Combat != null)
		{
			return TryLeaveCombatForSelfCare(character);
		}

		if (character.Movement != null)
		{
			return false;
		}

		if (!character.State.IsAble())
		{
			return false;
		}

		if (TryMoveToSafeLocationForSelfCare(character))
		{
			return true;
		}

		if (character.Effects.Any(x => x.IsBlockingEffect("general")))
		{
			return false;
		}

		return careType switch
		{
			RequiredSelfCare.Bind => QueueSelfCareCommand(character, "bind self"),
			RequiredSelfCare.Suture => QueueSelfCareCommand(character, "suture self"),
			_ => false
		};
	}

	private bool HandleBleedTick(ICharacter character, double amount)
	{
		return HandleSelfCare(character);
	}

	private bool HandleCharacterEnterCellFinish(ICharacter character)
	{
		return HandleSelfCare(character);
	}

	private bool HandleNoLongerEngagedInMelee(ICharacter character)
	{
		return HandleSelfCare(character);
	}

	private bool HandleNoLongerTargetted(ICharacter character)
	{
		return HandleSelfCare(character);
	}

	private bool HandleLeaveCombat(ICharacter character)
	{
		return HandleSelfCare(character);
	}

	private bool HandleTenSecondTick(ICharacter character)
	{
		return HandleSelfCare(character);
	}
}
