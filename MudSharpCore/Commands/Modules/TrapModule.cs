#nullable enable

using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Commands.Trees;
using MudSharp.Computers;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
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
	#3trap types#0 - lists the trap templates your character knows
	#3trap inspect <item|exit|here>#0 - inspects a known trap
	#3trap lay <template> on <item|exit|here> [using <item> ...]#0 - deploys a known current trap template; repeat using for tagged mechanical parts you hold or can manipulate loose in the room (held parts are preferred)
	#3trap pointout <person> <item|exit|here>#0 - shares knowledge of a trap
	#3trap disarm <item|exit|here>#0 - attempts to disarm a known trap
	#3trap recover <item|exit|here>#0 - removes a safely disarmed trap
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
			case "types":
			case "known":
			case "templates":
				ListKnownTemplates(actor);
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

	private static bool KnowsTemplate(ICharacter actor, ITrapTemplate template)
	{
		return actor.IsAdministrator() || template.KnowledgeProg?.ExecuteBool(actor) != false;
	}

	private static void ListKnownTemplates(ICharacter actor)
	{
		var templates = actor.Gameworld.TrapTemplates
			.Where(x => x.Status == RevisionStatus.Current && x.CanSubmit() && KnowsTemplate(actor, x))
			.OrderBy(x => x.Name)
			.ToList();
		if (templates.Count == 0)
		{
			actor.Send("You do not currently know how to lay any traps.");
			return;
		}

		actor.Send(StringUtilities.GetTextTable(
			from template in templates
			select new[]
			{
				template.Id.ToString("N0", actor),
				template.Name,
				template.SourceKind.DescribeEnum(),
				template.SourceKind == TrapSourceKind.Mechanical ? template.SetupTime.Describe(actor) : "n/a",
				template.SourceKind switch
				{
					TrapSourceKind.Mechanical => "trap lay",
					TrapSourceKind.Magical => "spell / prog",
					TrapSourceKind.Natural => "NPC / prog",
					_ => "special"
				},
				template.Triggers.Select(x => x.TriggerType.DescribeEnum()).ListToString()
			},
			new[] { "Id", "Trap", "Source", "Setup", "Deployment", "Triggers" }, actor, Telnet.Green));
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
				DescribeAnchor(actor, entry.Anchor, entry.Exit),
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

		var (anchor, exit, trap) = result.Value;
		if (!debug && !trap.IsKnownBy(actor))
		{
			actor.Send("You have not identified a trap there.");
			return;
		}

		var template = trap.Template;
		var sb = new StringBuilder();
		sb.AppendLine($"{DescribeAnchor(actor, anchor, exit).ColourName()} - {(template?.Name ?? "Orphaned Trap".ColourError())}");
		sb.AppendLine($"State: {trap.State.DescribeEnum().ColourValue()}  Charges: {trap.RemainingCharges.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Source: {trap.SourceKind.DescribeEnum().ColourValue()}  Template: {trap.TemplateId.ToString("N0", actor)}r{trap.TemplateRevisionNumber.ToString("N0", actor)}");
		if (template is not null)
		{
			sb.AppendLine($"Triggers: {template.Triggers.Select(x => x.TriggerType.DescribeEnum()).ListToString()}");
			sb.AppendLine($"Payloads: {template.Payloads.Select(x => x.PayloadType.DescribeEnum()).ListToString()}");
			sb.AppendLine($"Disarm: {template.DisarmPolicy.DescribeEnum()}  Lifecycle: {template.LifecyclePolicy.DescribeEnum()}");
		}
		if (trap.Components.Any())
		{
			sb.AppendLine($"Components: {trap.Components.Select(x => $"{x.Item?.HowSeen(actor) ?? $"missing item #{x.ItemId}"} ({x.Role.DescribeEnum()})").ListToString()}");
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

		if (!administrative && !KnowsTemplate(actor, template))
		{
			actor.Send("You do not know how to lay that kind of trap. Use trap types to see the traps you know.");
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

		var target = ResolveAnchor(actor, command);
		if (target is null)
		{
			actor.Send("You must specify a visible item, exit direction, or here as the trap anchor.");
			return;
		}
		var (anchor, exit, suppliedComponents) = target.Value;
		if (exit is not null && exit.Exit.Id <= 0)
		{
			actor.Send("That temporary exit cannot hold a persistent trap.");
			return;
		}
		var componentItems = suppliedComponents
			.Append(anchor as IGameItem)
			.Where(x => x is not null)
			.Cast<IGameItem>()
			.Distinct()
			.ToList();
		foreach (var item in componentItems)
		{
			var (canUse, error) = CanUsePhysicalTrapItem(actor, item);
			if (!canUse)
			{
				actor.Send(error);
				return;
			}
		}

		var bindings = MatchComponents(actor, template, componentItems);
		if (bindings is null)
		{
			return;
		}
		if (template.SourceKind == TrapSourceKind.Mechanical &&
		    template.Triggers.Any(x => x.TriggerType == TrapTriggerType.Signal) &&
		    !bindings.Any(x => x.Role.HasFlag(TrapComponentRole.Trigger) &&
		                       x.Item?.GetItemTypes<ISignalSourceComponent>().Any() == true))
		{
			actor.Send("A signal trigger requires one of the matched trigger components to expose an automation signal source.");
			return;
		}
		if (template.SourceKind == TrapSourceKind.Mechanical &&
		    template.Payloads.Any(x => x.PayloadType == TrapPayloadType.DetonateItem) &&
		    !bindings.Any(x => x.Role.HasFlag(TrapComponentRole.Payload) && x.Item?.GetItemType<IDetonatable>() is not null))
		{
			actor.Send("A detonation payload requires one of the matched payload components to be an explosive item.");
			return;
		}
		if (template.SourceKind == TrapSourceKind.Mechanical &&
		    template.Payloads.Any(x => x.PayloadType == TrapPayloadType.EmitSignal &&
		                               (!x.Parameters.TryGetValue("targetitem", out var targetText) || targetText == "0")) &&
		    !bindings.Any(x => x.Role.HasFlag(TrapComponentRole.Payload) && x.Item?.Components.OfType<ISignalSink>().Any() == true))
		{
			actor.Send("A signal payload without an explicit target item requires a matched payload component with an automation signal sink.");
			return;
		}

		if (!TrapEffect.IsValidAnchor(template, anchor))
		{
			actor.Send("Proximity traps require an item, character, or other real spatial anchor. Use a cell-entry trigger for a here trap.");
			return;
		}

		if (anchor.EffectsOfType<TrapEffect>()
		    .Any(x => x.State is not TrapState.Spent and not TrapState.Expired && SameBinding(x, exit)))
		{
			actor.Send("That anchor already has an active trap.");
			return;
		}

		void Complete(IPerceivable _)
		{
			var componentAccessFailure = componentItems
				.Select(x => CanUsePhysicalTrapItem(actor, x))
				.Where(x => !x.Truth)
				.Select(x => ((bool Truth, string Message)?)x)
				.FirstOrDefault();
			if (!AnchorStillAvailable(actor, anchor, exit) || componentAccessFailure.HasValue || anchor.EffectsOfType<TrapEffect>()
			    .Any(x => x.State is not TrapState.Spent and not TrapState.Expired && SameBinding(x, exit)))
			{
				actor.Send(string.IsNullOrEmpty(componentAccessFailure?.Message)
					? "You can no longer finish setting that trap there."
					: $"You can no longer finish setting that trap: {componentAccessFailure.Value.Message}");
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

			PlaceHeldTrapAnchor(actor, anchor);
			var trap = new TrapEffect(anchor, template, actor, exit, bindings);
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

			actor.Send($"You finish setting {template.Name.ColourName()} on {DescribeAnchor(actor, anchor, exit)}.");
		}

		if (administrative || template.SetupTime <= TimeSpan.Zero)
		{
			Complete(actor);
			return;
		}

		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ begin|begins setting a trap.", actor)));
		actor.AddEffect(new SimpleCharacterAction(actor, Complete, "setting a trap", ["general", "movement"],
			"setting a trap"), template.SetupTime);
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

		var (anchor, exit, trap) = result.Value;
		if (!trap.IsKnownBy(actor))
		{
			actor.Send("You must first identify the trap before attempting to disarm it.");
			return;
		}

		if (trap.State != TrapState.Armed)
		{
			actor.Send($"That trap is {trap.State.DescribeEnum().ToLowerInvariant()} and cannot presently be disarmed.");
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

		void Complete(IPerceivable _)
		{
			if (!anchor.EffectsOfType<TrapEffect>().Contains(trap) || trap.State != TrapState.Armed)
			{
				actor.Send("That trap is no longer available to disarm.");
				return;
			}
			var outcome = actor.Gameworld.GetCheck(CheckType.DisarmTrapCheck).Check(actor, Difficulty.Normal, actor);
			if (outcome.Outcome.IsPass())
			{
				trap.Disarm();
				actor.Send($"You finish disarming the trap on {DescribeAnchor(actor, anchor, exit)}.");
				return;
			}

			actor.Send("You make a mistake while attempting to disarm the trap.");
			if (trap.Template?.DisarmPolicy == TrapDisarmPolicy.Risky)
			{
				trap.ForceTrigger(actor);
			}
		}

		var delay = actor.IsAdministrator() ? TimeSpan.Zero : trap.Template?.DisarmTime ?? TimeSpan.FromSeconds(10);
		if (delay <= TimeSpan.Zero)
		{
			Complete(actor);
			return;
		}
		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ begin|begins carefully disarming a trap.", actor)));
		actor.AddEffect(new SimpleCharacterAction(actor, Complete, "disarming a trap", ["general", "movement"],
			"disarming a trap"), delay);
	}

	private static void RecoverTrap(ICharacter actor, StringStack command)
	{
		var result = FindTrap(actor, command.PopSpeech());
		if (result is null || result.Value.Trap.State is not (TrapState.Disarmed or TrapState.Spent))
		{
			actor.Send("There is no disarmed or spent trap there to recover.");
			return;
		}

		var (anchor, exit, trap) = result.Value;
		void Complete(IPerceivable _)
		{
			if (!anchor.EffectsOfType<TrapEffect>().Contains(trap) || trap.State is not (TrapState.Disarmed or TrapState.Spent))
			{
				actor.Send("That disarmed trap is no longer available to recover.");
				return;
			}
			var wasSpent = trap.State == TrapState.Spent;
			var recovered = trap.RecoverAndRemove(actor, wasSpent);
			actor.Send($"You dismantle the {(wasSpent ? "spent" : "disarmed")} trap from {DescribeAnchor(actor, anchor, exit)}.");
			foreach (var result in recovered)
			{
				actor.Send(result.Recovered
					? $"You recover {result.Description}."
					: $"{result.Description} breaks beyond recovery during dismantling.");
			}
		}
		var delay = actor.IsAdministrator() ? TimeSpan.Zero : trap.Template?.RecoveryTime ?? TimeSpan.FromSeconds(5);
		if (delay <= TimeSpan.Zero)
		{
			Complete(actor);
			return;
		}
		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ begin|begins recovering a disarmed trap.", actor)));
		actor.AddEffect(new SimpleCharacterAction(actor, Complete, "recovering a trap", ["general", "movement"],
			"recovering a trap"), delay);
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

		if (!result.Value.Trap.ResetAfterCooldown())
		{
			actor.Send("That trap is not currently cooling down with charges remaining.");
			return;
		}
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

	private static List<TrapComponentBinding>? MatchComponents(ICharacter actor, ITrapTemplate template,
		IReadOnlyList<IGameItem> candidates)
	{
		if (!TrapEffect.TryBindComponents(template, candidates, out var bindings))
		{
			var needs = template.ComponentRequirements
				.Select(x => $"{x.Role.DescribeEnum()} item tagged {(x.Tag?.Name ?? $"missing tag #{x.TagId}")}")
				.ListToString();
			var supplied = candidates.Any()
				? candidates.Select(x => $"{x.HowSeen(actor)} [#{x.Id.ToString("N0", actor)}]").ListToString()
				: "no physical items";
			actor.Send($"The selected components ({supplied}) do not satisfy the template's physical requirements. It needs {needs}. One item may satisfy a trigger and a payload requirement when it has both tags.");
			return null;
		}
		return bindings;
	}

	private static (bool Truth, string Message) CanUsePhysicalTrapItem(ICharacter actor, IGameItem item)
	{
		if (item.Id <= 0 || item.Deleted)
		{
			return (false, "Every physical trap component must be a persistent item.");
		}

		if (item.EffectsOfType<TrapComponentReservationEffect>().Any())
		{
			return (false, $"{item.HowSeen(actor, true)} is already installed as part of another trap.");
		}

		var heldByActor = actor.Body.ItemsInHands.Any(x => ReferenceEquals(x, item));
		var looseInRoom = item.InInventoryOf is null && item.ContainedIn is null &&
		                  ReferenceEquals(item.Location, actor.Location);
		if (!heldByActor && !looseInRoom)
		{
			return (false,
				$"You must be holding {item.HowSeen(actor, true)}, or it must be loose in your current location, to install it in a trap.");
		}

		var (canManipulate, manipulationError) = actor.CanManipulateItem(item);
		if (!canManipulate)
		{
			return (false, manipulationError);
		}

		if (heldByActor && !actor.Body.CanRemoveItem(item))
		{
			return (false, actor.Body.WhyCannotRemove(item));
		}

		return (true, string.Empty);
	}

	private static void PlaceHeldTrapAnchor(ICharacter actor, IPerceivable anchor)
	{
		if (anchor is not IGameItem item ||
		    !actor.Body.ItemsInHands.Any(x => ReferenceEquals(x, item)))
		{
			return;
		}

		actor.Body.Take(item);
		item.RoomLayer = actor.RoomLayer;
		item.InsertAtSource(actor, true);
	}

	private static (IPerceivable Anchor, ICellExit? Exit, List<IGameItem> Components)? ResolveAnchor(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			return null;
		}

		var preposition = command.PopForSwitch();
		if (preposition == "here")
		{
			return ParseSuppliedComponents(actor, command, actor.Location, null);
		}

		if (preposition != "on")
		{
			var legacy = ResolveAnchorText(actor, preposition);
			return legacy is null ? null : ParseSuppliedComponents(actor, command, legacy.Value.Anchor, legacy.Value.Exit);
		}

		var targetText = command.PopSpeech();
		var target = ResolveAnchorText(actor, targetText);
		return target is null ? null : ParseSuppliedComponents(actor, command, target.Value.Anchor, target.Value.Exit);
	}

	private static (IPerceivable Anchor, ICellExit? Exit, List<IGameItem> Components)? ParseSuppliedComponents(
		ICharacter actor, StringStack command, IPerceivable anchor, ICellExit? exit)
	{
		var components = new List<IGameItem>();
		while (!command.IsFinished)
		{
			if (command.PopForSwitch() != "using" || command.IsFinished)
			{
				return null;
			}
			var item = actor.TargetLocalOrHeldItem(command.PopSpeech());
			if (item is null)
			{
				return null;
			}
			components.Add(item);
		}
		return (anchor, exit, components);
	}

	private static (IPerceivable Anchor, ICellExit? Exit)? ResolveAnchorText(ICharacter actor, string text)
	{
		if (text.EqualTo("here"))
		{
			return (actor.Location, null);
		}

		var exit = actor.Location.GetExitKeyword(text, actor);
		if (exit is not null)
		{
			return (actor.Location, exit);
		}

		var item = actor.TargetLocalOrHeldItem(text);
		return item is null ? null : (item, null);
	}

	private static (IPerceivable Anchor, ICellExit? Exit, TrapEffect Trap)? FindTrap(ICharacter actor, string anchorText)
	{
		if (anchorText.EqualTo("here"))
		{
			var cellTrap = actor.Location.EffectsOfType<TrapEffect>().FirstOrDefault(x => !x.BoundExitId.HasValue);
			return cellTrap is null ? null : (actor.Location, null, cellTrap);
		}

		var exit = actor.Location.GetExitKeyword(anchorText, actor);
		if (exit is not null)
		{
			var exitTrap = actor.Location.EffectsOfType<TrapEffect>()
				.FirstOrDefault(x => x.BoundExitId == exit.Exit.Id && x.BoundExitOriginId == actor.Location.Id);
			if (exitTrap is not null)
			{
				return (actor.Location, exit, exitTrap);
			}

			var boundItemTrap = actor.Location.LayerGameItems(actor.RoomLayer)
				.SelectMany(x => x.EffectsOfType<TrapEffect>().Select(trap => (Item: x, Trap: trap)))
				.FirstOrDefault(x => x.Trap.BoundExitId == exit.Exit.Id &&
				                     x.Trap.BoundExitOriginId == actor.Location.Id);
			return boundItemTrap.Trap is null ? null : (boundItemTrap.Item, exit, boundItemTrap.Trap);
		}

		var item = actor.TargetItem(anchorText);
		var itemTrap = item?.EffectsOfType<TrapEffect>().FirstOrDefault();
		return itemTrap is null || item is null ? null : (item, null, itemTrap);
	}

	private static IEnumerable<(IPerceivable Anchor, ICellExit? Exit, TrapEffect Trap)> EnumerateLocalTraps(ICharacter actor)
	{
		foreach (var trap in actor.Location.EffectsOfType<TrapEffect>())
		{
			var exit = trap.BoundExitId.HasValue
				? actor.Location.ExitsFor(actor, true).FirstOrDefault(x => x.Exit.Id == trap.BoundExitId.Value)
				: null;
			yield return (actor.Location, exit, trap);
		}

		foreach (var item in actor.Location.LayerGameItems(actor.RoomLayer))
		{
			foreach (var trap in item.EffectsOfType<TrapEffect>())
			{
				yield return (item, null, trap);
			}
		}
	}

	private static bool SameBinding(TrapEffect trap, ICellExit? exit) => exit is null
		? !trap.BoundExitId.HasValue
		: trap.BoundExitId == exit.Exit.Id && trap.BoundExitOriginId == exit.Origin.Id;

	private static bool AnchorStillAvailable(ICharacter actor, IPerceivable anchor, ICellExit? exit)
	{
		if (exit is not null)
		{
			return (ReferenceEquals(actor.Location, anchor) || ReferenceEquals(anchor.Location, actor.Location)) &&
			       actor.Location.ExitsFor(actor, true)
				.Any(x => x.Exit.Id == exit.Exit.Id && x.Origin.Id == exit.Origin.Id);
		}

		return ReferenceEquals(anchor, actor.Location) || ReferenceEquals(anchor.Location, actor.Location) ||
		       anchor is IGameItem item && actor.Body.ItemsInHands.Any(x => ReferenceEquals(x, item));
	}

	private static string DescribeAnchor(ICharacter actor, IPerceivable anchor, ICellExit? exit) => exit is null
		? anchor.HowSeen(actor)
		: anchor is IGameItem
			? $"{anchor.HowSeen(actor)} at {exit.OutboundDirectionDescription} exit"
			: $"{exit.OutboundDirectionDescription} exit";
}
