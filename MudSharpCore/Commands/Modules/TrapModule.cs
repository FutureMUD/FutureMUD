#nullable enable

using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Commands.Trees;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Law;
using MudSharp.Traps;

namespace MudSharp.Commands.Modules;

/// <summary>
/// Player and administrator controls for deploying and interacting with persisted trap effects.
/// Builder authoring remains on <c>traptemplate</c>; this module deliberately contains no mutable
/// template-definition logic.
/// </summary>
internal class TrapModule : Module<ICharacter>
{
	private TrapModule()
		: base("Traps")
	{
		IsNecessary = true;
	}

	public static TrapModule Instance { get; } = new();

	private const string HelpText = @"The #3trap#0 command lets you set, inspect, reveal and deal with traps. Templates are authored separately with #3traptemplate#0 by builders.

	#3trap list#0 - lists traps you know at this location
	#3trap inspect <item|here>#0 - inspects a known trap
	#3trap lay <template> [on <item>|here]#0 - deploys a current trap template
	#3trap pointout <person> <item|here>#0 - shares knowledge of a trap
	#3trap disarm <item|here>#0 - attempts to disarm a known trap
	#3trap recover <item|here>#0 - removes a safely disarmed trap
	#3trap struggle#0 - attempts to escape a trap restraint

Administrators additionally have #3trap create|show|debug|arm|trigger|reset|reveal|delete#0 for verification and world maintenance.";

	[PlayerCommand("Trap", "trap")]
	[RequiredCharacterState(CharacterState.Able)]
	[HelpInfo("trap", HelpText, AutoHelp.HelpArgOrNoArg)]
	protected static void Trap(ICharacter actor, string input)
	{
		var command = new StringStack(input.RemoveFirstWord());
		switch (command.PopForSwitch())
		{
			case "list":
				ListTraps(actor);
				return;
			case "inspect":
			case "show":
				InspectTrap(actor, command, false);
				return;
			case "lay":
			case "set":
				LayTrap(actor, command, false);
				return;
			case "pointout":
				PointOutTrap(actor, command);
				return;
			case "disarm":
				DisarmTrap(actor, command);
				return;
			case "recover":
				RecoverTrap(actor, command);
				return;
			case "struggle":
				Struggle(actor);
				return;
			case "create" when actor.IsAdministrator():
				LayTrap(actor, command, true);
				return;
			case "debug" when actor.IsAdministrator():
				InspectTrap(actor, command, true);
				return;
			case "arm" when actor.IsAdministrator():
				SetArmedState(actor, command, true);
				return;
			case "trigger" when actor.IsAdministrator():
				TriggerTrap(actor, command);
				return;
			case "reset" when actor.IsAdministrator():
				ResetTrap(actor, command);
				return;
			case "reveal" when actor.IsAdministrator():
				RevealTrap(actor, command);
				return;
			case "delete" when actor.IsAdministrator():
				DeleteTrap(actor, command);
				return;
			default:
				actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
				return;
		}
	}

	private static void ListTraps(ICharacter actor)
	{
		var traps = EnumerateLocalTraps(actor)
			.Where(x => x.Trap.IsKnownBy(actor))
			.ToList();
		if (!traps.Any())
		{
			actor.Send("You do not know of any traps here.");
			return;
		}

		actor.Send(StringUtilities.GetTextTable(
			from entry in traps
			select new[]
			{
				entry.Anchor.HowSeen(actor),
				entry.Trap.Template?.Name ?? $"orphaned template {entry.Trap.TemplateId:N0}",
				entry.Trap.State.DescribeEnum(),
				entry.Trap.RemainingCharges.ToString("N0", actor)
			},
			new[] { "Anchor", "Template", "State", "Charges" },
			actor, Telnet.Green));
	}

	private static void InspectTrap(ICharacter actor, StringStack command, bool debug)
	{
		var result = FindTrap(actor, command.PopSpeech());
		if (result is null)
		{
			actor.Send("You do not see a trap on that anchor.");
			return;
		}

		var (anchor, trap) = result.Value;
		if (!debug && !trap.IsKnownBy(actor))
		{
			actor.Send("You have not identified a trap there.");
			return;
		}

		var template = trap.Template;
		var sb = new StringBuilder();
		sb.AppendLine($"{anchor.HowSeen(actor).ColourName()} - {(template?.Name ?? "Orphaned Trap".ColourError())}");
		sb.AppendLine($"State: {trap.State.DescribeEnum().ColourValue()}  Charges: {trap.RemainingCharges.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Source: {trap.SourceKind.DescribeEnum().ColourValue()}  Template: {trap.TemplateId.ToString("N0", actor)}r{trap.TemplateRevisionNumber.ToString("N0", actor)}");
		if (template is not null)
		{
			sb.AppendLine($"Triggers: {template.Triggers.Select(x => x.TriggerType.DescribeEnum()).ListToString()}");
			sb.AppendLine($"Payloads: {template.Payloads.Select(x => x.PayloadType.DescribeEnum()).ListToString()}");
			sb.AppendLine($"Disarm: {template.DisarmPolicy.DescribeEnum()}  Lifecycle: {template.LifecyclePolicy.DescribeEnum()}");
		}

		actor.Send(sb.ToString());
	}

	private static void LayTrap(ICharacter actor, StringStack command, bool administrative)
	{
		var template = actor.Gameworld.TrapTemplates.GetByIdOrName(command.PopSpeech());
		if (template is null)
		{
			actor.Send("There is no such trap template.");
			return;
		}

		if (!administrative && template.Status != RevisionStatus.Current)
		{
			actor.Send("Only current trap template revisions can be deployed.");
			return;
		}

		if (!administrative && !template.CanSubmit())
		{
			actor.Send($"That template is not ready to deploy: {template.WhyCannotSubmit()}");
			return;
		}

		if (!administrative && template.SourceKind == TrapSourceKind.Natural)
		{
			actor.Send("Natural trap templates are deployed by natural NPCs or FutureProgs, not hand-set by characters.");
			return;
		}

		if (!administrative && template.SourceKind == TrapSourceKind.Magical)
		{
			actor.Send("Magical trap templates must be deployed by a spell or FutureProg, not hand-set by characters.");
			return;
		}

		var anchor = ResolveAnchor(actor, command);
		if (anchor is null)
		{
			actor.Send("You must specify a visible item or here as the trap anchor.");
			return;
		}

		if (!TrapEffect.IsValidAnchor(template, anchor))
		{
			actor.Send("Proximity traps require an item, character, or other real spatial anchor. Use a cell-entry trigger for a here trap.");
			return;
		}

		if (anchor.EffectsOfType<TrapEffect>()
		    .Any(x => x.State is not TrapState.Spent and not TrapState.Expired))
		{
			actor.Send("That anchor already has an active trap.");
			return;
		}

		if (!administrative)
		{
			var outcome = actor.Gameworld.GetCheck(CheckType.SetTrapCheck).Check(actor, Difficulty.Normal, actor);
			if (!outcome.Outcome.IsPass())
			{
				actor.Send("You fail to set the trap without making it usable.");
				return;
			}
		}

		var trap = new TrapEffect(anchor, template, actor);
		if (TrapEffect.HasTimedLifetime(template))
		{
			anchor.AddEffect(trap, template.Lifespan!.Value);
		}
		else
		{
			anchor.AddEffect(trap);
		}

		if (!administrative)
		{
			CrimeExtensions.CheckPossibleCrimeAllAuthorities(actor, CrimeTypes.BoobyTrapping, null, anchor as IGameItem,
				template.Name);
		}

		actor.Send($"You set {template.Name.ColourName()} on {anchor.HowSeen(actor)}.");
	}

	private static void PointOutTrap(ICharacter actor, StringStack command)
	{
		var target = actor.TargetActor(command.PopSpeech());
		if (target is null)
		{
			actor.Send("You do not see anyone by that description.");
			return;
		}

		var result = FindTrap(actor, command.PopSpeech());
		if (result is null || !result.Value.Trap.IsKnownBy(actor))
		{
			actor.Send("You have not identified a trap there.");
			return;
		}

		result.Value.Trap.MarkKnownBy(target);
		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ point|points out a trap to $0.", actor, target)));
	}

	private static void DisarmTrap(ICharacter actor, StringStack command)
	{
		var result = FindTrap(actor, command.PopSpeech());
		if (result is null)
		{
			actor.Send("You do not see a trap on that anchor.");
			return;
		}

		var (anchor, trap) = result.Value;
		if (!trap.IsKnownBy(actor))
		{
			actor.Send("You must first identify the trap before attempting to disarm it.");
			return;
		}

		if (trap.Template?.DisarmPolicy == TrapDisarmPolicy.Impossible)
		{
			actor.Send("That trap cannot be disarmed.");
			return;
		}

		if (trap.Template?.DisarmPolicy == TrapDisarmPolicy.Dispellable)
		{
			actor.Send("That trap must be removed with a suitable dispelling spell.");
			return;
		}

		var outcome = actor.Gameworld.GetCheck(CheckType.DisarmTrapCheck).Check(actor, Difficulty.Normal, actor);
		if (outcome.Outcome.IsPass())
		{
			trap.Disarm();
			actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ disarm|disarms a trap on $0.", actor, anchor)));
			return;
		}

		actor.Send("You make a mistake while attempting to disarm the trap.");
		if (trap.Template?.DisarmPolicy == TrapDisarmPolicy.Risky)
		{
			trap.ForceTrigger(actor);
		}
	}

	private static void RecoverTrap(ICharacter actor, StringStack command)
	{
		var result = FindTrap(actor, command.PopSpeech());
		if (result is null || result.Value.Trap.State != TrapState.Disarmed)
		{
			actor.Send("There is no disarmed trap there to recover.");
			return;
		}

		result.Value.Anchor.RemoveEffect(result.Value.Trap, true);
		actor.Send("You recover the disarmed trap's remaining components.");
	}

	private static void Struggle(ICharacter actor)
	{
		var restraint = actor.EffectsOfType<TrapRestraintEffect>().FirstOrDefault();
		if (restraint is null)
		{
			actor.Send("You are not currently restrained by a trap.");
			return;
		}

		var outcome = actor.Gameworld.GetCheck(CheckType.EscapeTrapCheck).Check(actor, Difficulty.Normal, actor);
		if (!outcome.Outcome.IsPass())
		{
			actor.Send("You struggle against the trap but cannot get free.");
			return;
		}

		actor.RemoveEffect(restraint, true);
		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ struggle|struggles free of a trap.", actor)));
	}

	private static void SetArmedState(ICharacter actor, StringStack command, bool armed)
	{
		var result = FindTrap(actor, command.PopSpeech());
		if (result is null)
		{
			actor.Send("You do not see a trap on that anchor.");
			return;
		}

		if (!armed)
		{
			result.Value.Trap.Disarm();
		}
		else if (!result.Value.Trap.Arm())
		{
			actor.Send("That trap cannot be armed in its current state.");
			return;
		}

		actor.Send($"That trap is now {(armed ? "armed" : "disarmed").ColourValue()}.");
	}

	private static void TriggerTrap(ICharacter actor, StringStack command)
	{
		var result = FindTrap(actor, command.PopSpeech());
		if (result is null)
		{
			actor.Send("You do not see a trap on that anchor.");
			return;
		}

		if (!result.Value.Trap.ForceTrigger(actor))
		{
			actor.Send("The trap did not trigger. It may need arming.");
			return;
		}

		actor.Send("You force the trap to trigger.");
	}

	private static void ResetTrap(ICharacter actor, StringStack command)
	{
		var result = FindTrap(actor, command.PopSpeech());
		if (result is null)
		{
			actor.Send("You do not see a trap on that anchor.");
			return;
		}

		result.Value.Trap.ResetAfterCooldown();
		actor.Send("You reset the trap cooldown.");
	}

	private static void RevealTrap(ICharacter actor, StringStack command)
	{
		var result = FindTrap(actor, command.PopSpeech());
		var target = actor.TargetActor(command.PopSpeech());
		if (result is null || target is null)
		{
			actor.Send("Use trap reveal <item|here> <character>.");
			return;
		}

		result.Value.Trap.MarkKnownBy(target);
		actor.Send($"You reveal that trap to {target.HowSeen(actor)}.");
	}

	private static void DeleteTrap(ICharacter actor, StringStack command)
	{
		var result = FindTrap(actor, command.PopSpeech());
		if (result is null)
		{
			actor.Send("You do not see a trap on that anchor.");
			return;
		}

		result.Value.Anchor.RemoveEffect(result.Value.Trap, true);
		actor.Send("You delete the trap.");
	}

	private static IPerceivable? ResolveAnchor(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			return null;
		}

		var preposition = command.PopForSwitch();
		if (preposition == "here")
		{
			return actor.Location;
		}

		if (preposition != "on")
		{
			return actor.TargetItem(preposition);
		}

		var item = command.PopSpeech();
		return item.EqualTo("here") ? actor.Location : actor.TargetItem(item);
	}

	private static (IPerceivable Anchor, TrapEffect Trap)? FindTrap(ICharacter actor, string anchorText)
	{
		if (anchorText.EqualTo("here"))
		{
			var cellTrap = actor.Location.EffectsOfType<TrapEffect>().FirstOrDefault();
			return cellTrap is null ? null : (actor.Location, cellTrap);
		}

		var item = actor.TargetItem(anchorText);
		var itemTrap = item?.EffectsOfType<TrapEffect>().FirstOrDefault();
		return itemTrap is null || item is null ? null : (item, itemTrap);
	}

	private static IEnumerable<(IPerceivable Anchor, TrapEffect Trap)> EnumerateLocalTraps(ICharacter actor)
	{
		foreach (var trap in actor.Location.EffectsOfType<TrapEffect>())
		{
			yield return (actor.Location, trap);
		}

		foreach (var item in actor.Location.LayerGameItems(actor.RoomLayer))
		{
			foreach (var trap in item.EffectsOfType<TrapEffect>())
			{
				yield return (item, trap);
			}
		}
	}
}
