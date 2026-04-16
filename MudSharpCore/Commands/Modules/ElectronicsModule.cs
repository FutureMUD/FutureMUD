#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Commands.Trees;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Commands.Modules;

internal class ElectronicsModule : Module<ICharacter>
{
	private const string ElectricalHelpText = @"The #3electrical#0 command is used to inspect and configure signal-driven electrical components on an item.

You can use the following syntax:
	#3electrical <item>#0 - shows the signal sources and configurable electrical components on the item
	#3electrical <item> bind <component> <source> [<endpoint>]#0 - rewires a configurable sink to a local source component
	#3electrical <item> clear <component>#0 - clears any live rewiring on a configurable sink
	#3electrical <item> threshold <component> <value>#0 - changes the component's activation threshold
	#3electrical <item> mode <component> above|below#0 - changes whether the sink activates above or below the threshold

Component and source identifiers can be either the live component #6id#0 shown in the inspection output or the component name.";

	private const string ProgrammingHelpText = @"The #3programming#0 command is used to inspect and live-program microcontrollers on an item.

You can use the following syntax:
	#3programming <item>#0 - shows all programmable microcontrollers on the item
	#3programming <item> logic <component>#0 - opens an editor to replace the controller logic
	#3programming <item> logic <component> <text>#0 - directly replaces the controller logic
	#3programming <item> input add <component> <variable> <source> [<endpoint>]#0 - binds an input variable to a local signal source
	#3programming <item> input remove <component> <variable>#0 - removes an input variable

Component and source identifiers can be either the live component #6id#0 shown in the inspection output or the component name.";

	private ElectronicsModule()
		: base("Electronics")
	{
		IsNecessary = true;
	}

	public static ElectronicsModule Instance { get; } = new();

	[PlayerCommand("Electrical", "electrical")]
	[RequiredCharacterState(CharacterState.Able)]
	[DelayBlock("general", "movement", "You must first stop {0} before you can do that.")]
	[NoHideCommand]
	[HelpInfo("electrical", ElectricalHelpText, AutoHelp.HelpArgOrNoArg)]
	protected static void Electrical(ICharacter actor, string command)
	{
		if (!actor.Gameworld.GetStaticBool("ElectricalCommandEnabled"))
		{
			actor.Send("Electrical work is not available in this game.");
			return;
		}

		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(ElectricalHelpText.SubstituteANSIColour());
			return;
		}

		var item = actor.TargetItem(ss.PopSpeech());
		if (item is null)
		{
			actor.Send("You do not see anything like that to work on.");
			return;
		}

		var manipulation = actor.CanManipulateItem(item);
		if (!manipulation.Truth)
		{
			actor.OutputHandler.Send(manipulation.Message);
			return;
		}

		if (ss.IsFinished)
		{
			ShowElectricalStatus(actor, item);
			return;
		}

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "bind":
				ElectricalBind(actor, item, ss);
				return;
			case "clear":
				ElectricalClear(actor, item, ss);
				return;
			case "threshold":
				ElectricalThreshold(actor, item, ss);
				return;
			case "mode":
				ElectricalMode(actor, item, ss);
				return;
			default:
				actor.OutputHandler.Send(ElectricalHelpText.SubstituteANSIColour());
				return;
		}
	}

	[PlayerCommand("Programming", "programming")]
	[RequiredCharacterState(CharacterState.Able)]
	[DelayBlock("general", "movement", "You must first stop {0} before you can do that.")]
	[NoHideCommand]
	[HelpInfo("programming", ProgrammingHelpText, AutoHelp.HelpArgOrNoArg)]
	protected static void Programming(ICharacter actor, string command)
	{
		if (!actor.Gameworld.GetStaticBool("ProgrammingCommandEnabled"))
		{
			actor.Send("Programming work is not available in this game.");
			return;
		}

		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(ProgrammingHelpText.SubstituteANSIColour());
			return;
		}

		var item = actor.TargetItem(ss.PopSpeech());
		if (item is null)
		{
			actor.Send("You do not see anything like that to program.");
			return;
		}

		var manipulation = actor.CanManipulateItem(item);
		if (!manipulation.Truth)
		{
			actor.OutputHandler.Send(manipulation.Message);
			return;
		}

		if (ss.IsFinished)
		{
			ShowProgrammingStatus(actor, item);
			return;
		}

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "logic":
			case "code":
				ProgrammingLogic(actor, item, ss);
				return;
			case "input":
			case "inputs":
				ProgrammingInput(actor, item, ss);
				return;
			default:
				actor.OutputHandler.Send(ProgrammingHelpText.SubstituteANSIColour());
				return;
		}
	}

	private static void ElectricalBind(ICharacter actor, IGameItem item, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Which configurable electrical component do you want to rewire?");
			return;
		}

		var sink = ResolveComponent<IRuntimeConfigurableSignalSinkComponent>(actor, item, ss.PopSpeech(),
			"configurable electrical component");
		if (sink is null)
		{
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("Which local signal source component should drive that component?");
			return;
		}

		var source = ResolveComponent<ISignalSourceComponent>(actor, item, ss.PopSpeech(), "signal source component");
		if (source is null)
		{
			return;
		}

		var endpointKey = ss.IsFinished ? source.EndpointKey : ss.PopSpeech();
		StartElectricalAction(
			actor,
			item,
			"ElectricalInstallActionDurationSeconds",
			CheckType.InstallElectricalComponentCheck,
			"ElectricalInstallTraitName",
			"ElectricalToolTagName",
			"rewiring $1",
			"ElectricalInstallActionBeginEmote",
			"ElectricalInstallActionContinueEmote",
			"ElectricalInstallActionCancelEmote",
			"ElectricalInstallActionSuccessEmote",
			"ElectricalInstallActionFailureEmote",
			outcome =>
			{
				if (!sink.ConfigureSignalBinding(source, endpointKey, out var error))
				{
					actor.Send(error);
					return false;
				}

				actor.Send(
					$"You rewire {DescribeComponent(actor, sink).ColourName()} so it now listens to {DescribeComponent(actor, source).ColourName()} on the {SignalComponentUtilities.NormaliseSignalEndpointKey(endpointKey).ColourCommand()} endpoint.");
				return true;
			});
	}

	private static void ElectricalClear(ICharacter actor, IGameItem item, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Which configurable electrical component do you want to clear?");
			return;
		}

		var sink = ResolveComponent<IRuntimeConfigurableSignalSinkComponent>(actor, item, ss.PopSpeech(),
			"configurable electrical component");
		if (sink is null)
		{
			return;
		}

		StartElectricalAction(
			actor,
			item,
			"ElectricalConfigureActionDurationSeconds",
			CheckType.ConfigureElectricalComponentCheck,
			"ElectricalConfigureTraitName",
			"ElectricalToolTagName",
			"configuring $1",
			"ElectricalConfigureActionBeginEmote",
			"ElectricalConfigureActionContinueEmote",
			"ElectricalConfigureActionCancelEmote",
			"ElectricalConfigureActionSuccessEmote",
			"ElectricalConfigureActionFailureEmote",
			outcome =>
			{
				sink.ClearSignalBinding();
				actor.Send($"You clear any live rewiring from {DescribeComponent(actor, sink).ColourName()}.");
				return true;
			});
	}

	private static void ElectricalThreshold(ICharacter actor, IGameItem item, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Which configurable electrical component do you want to retune?");
			return;
		}

		var sink = ResolveComponent<IRuntimeConfigurableSignalSinkComponent>(actor, item, ss.PopSpeech(),
			"configurable electrical component");
		if (sink is null)
		{
			return;
		}

		if (ss.IsFinished || !double.TryParse(ss.SafeRemainingArgument, out var threshold))
		{
			actor.Send("What numeric threshold should that component use?");
			return;
		}

		StartElectricalAction(
			actor,
			item,
			"ElectricalConfigureActionDurationSeconds",
			CheckType.ConfigureElectricalComponentCheck,
			"ElectricalConfigureTraitName",
			"ElectricalToolTagName",
			"configuring $1",
			"ElectricalConfigureActionBeginEmote",
			"ElectricalConfigureActionContinueEmote",
			"ElectricalConfigureActionCancelEmote",
			"ElectricalConfigureActionSuccessEmote",
			"ElectricalConfigureActionFailureEmote",
			outcome =>
			{
				if (!sink.SetActivationThreshold(threshold, out var error))
				{
					actor.Send(error);
					return false;
				}

				actor.Send(
					$"You set {DescribeComponent(actor, sink).ColourName()} to trigger at {threshold.ToString("N2", actor).ColourValue()}.");
				return true;
			});
	}

	private static void ElectricalMode(ICharacter actor, IGameItem item, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Which configurable electrical component do you want to reconfigure?");
			return;
		}

		var sink = ResolveComponent<IRuntimeConfigurableSignalSinkComponent>(actor, item, ss.PopSpeech(),
			"configurable electrical component");
		if (sink is null)
		{
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("Should that component trigger above or below the threshold?");
			return;
		}

		var mode = ss.PopSpeech().ToLowerInvariant();
		bool? activeWhenAboveThreshold = mode switch
		{
			"above" => true,
			"high" => true,
			"below" => false,
			"low" => false,
			_ => null
		};
		if (!activeWhenAboveThreshold.HasValue)
		{
			actor.Send("You must specify either above or below.");
			return;
		}

		StartElectricalAction(
			actor,
			item,
			"ElectricalConfigureActionDurationSeconds",
			CheckType.ConfigureElectricalComponentCheck,
			"ElectricalConfigureTraitName",
			"ElectricalToolTagName",
			"configuring $1",
			"ElectricalConfigureActionBeginEmote",
			"ElectricalConfigureActionContinueEmote",
			"ElectricalConfigureActionCancelEmote",
			"ElectricalConfigureActionSuccessEmote",
			"ElectricalConfigureActionFailureEmote",
			outcome =>
			{
				sink.SetActiveWhenAboveThreshold(activeWhenAboveThreshold.Value);
				actor.Send(
					$"You set {DescribeComponent(actor, sink).ColourName()} to trigger when its control signal is {(activeWhenAboveThreshold.Value ? "at or above".ColourValue() : "below".ColourValue())} the configured threshold.");
				return true;
			});
	}

	private static void ProgrammingLogic(ICharacter actor, IGameItem item, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Which programmable microcontroller do you want to update?");
			return;
		}

		var controller = ResolveComponent<IRuntimeProgrammableMicrocontroller>(actor, item, ss.PopSpeech(),
			"programmable microcontroller");
		if (controller is null)
		{
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("Enter the replacement microcontroller logic in the editor below.");
			actor.EditorMode(
				(text, handler, _) => StartProgrammingAction(
					actor,
					item,
					"programming $1",
					outcome =>
					{
						if (!controller.SetLogicText(text, out var error))
						{
							actor.Send(error);
							return false;
						}

						actor.Send(
							$"You replace the logic in {DescribeComponent(actor, controller).ColourName()}.");
						return true;
					}),
				(handler, _) => handler.Send("You decide not to change the microcontroller logic."),
				1.0,
				recallText: controller.LogicText);
			return;
		}

		var logicText = ss.SafeRemainingArgument;
		StartProgrammingAction(
			actor,
			item,
			"programming $1",
			outcome =>
			{
				if (!controller.SetLogicText(logicText, out var error))
				{
					actor.Send(error);
					return false;
				}

				actor.Send($"You replace the logic in {DescribeComponent(actor, controller).ColourName()}.");
				return true;
			});
	}

	private static void ProgrammingInput(ICharacter actor, IGameItem item, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Do you want to add or remove an input binding?");
			return;
		}

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "add":
				ProgrammingInputAdd(actor, item, ss);
				return;
			case "remove":
			case "rem":
			case "delete":
			case "del":
				ProgrammingInputRemove(actor, item, ss);
				return;
			default:
				actor.Send("Do you want to add or remove an input binding?");
				return;
		}
	}

	private static void ProgrammingInputAdd(ICharacter actor, IGameItem item, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Which programmable microcontroller do you want to change?");
			return;
		}

		var controller = ResolveComponent<IRuntimeProgrammableMicrocontroller>(actor, item, ss.PopSpeech(),
			"programmable microcontroller");
		if (controller is null)
		{
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("What variable name should that input use?");
			return;
		}

		var variableName = ss.PopSpeech();
		if (ss.IsFinished)
		{
			actor.Send("Which local signal source should feed that input?");
			return;
		}

		var source = ResolveComponent<ISignalSourceComponent>(actor, item, ss.PopSpeech(), "signal source component");
		if (source is null)
		{
			return;
		}

		var endpointKey = ss.IsFinished ? source.EndpointKey : ss.PopSpeech();
		StartProgrammingAction(
			actor,
			item,
			"programming $1",
			outcome =>
			{
				if (!controller.SetInputBinding(variableName, source, endpointKey, out var error))
				{
					actor.Send(error);
					return false;
				}

				actor.Send(
					$"You bind the {variableName.ColourCommand()} input on {DescribeComponent(actor, controller).ColourName()} to {DescribeComponent(actor, source).ColourName()} on endpoint {SignalComponentUtilities.NormaliseSignalEndpointKey(endpointKey).ColourCommand()}.");
				return true;
			});
	}

	private static void ProgrammingInputRemove(ICharacter actor, IGameItem item, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Which programmable microcontroller do you want to change?");
			return;
		}

		var controller = ResolveComponent<IRuntimeProgrammableMicrocontroller>(actor, item, ss.PopSpeech(),
			"programmable microcontroller");
		if (controller is null)
		{
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("Which input variable do you want to remove?");
			return;
		}

		var variableName = ss.PopSpeech();
		StartProgrammingAction(
			actor,
			item,
			"programming $1",
			outcome =>
			{
				if (!controller.RemoveInputBinding(variableName, out var error))
				{
					actor.Send(error);
					return false;
				}

				actor.Send(
					$"You remove the {variableName.ColourCommand()} input from {DescribeComponent(actor, controller).ColourName()}.");
				return true;
			});
	}

	private static void StartProgrammingAction(ICharacter actor, IGameItem item, string actionDescription,
		Func<CheckOutcome, bool> successAction)
	{
		StartConfiguredAction(
			actor,
			item,
			"ProgrammingToolTagName",
			"ProgrammingTraitName",
			"ProgrammingActionDurationSeconds",
			CheckType.ProgrammingComponentCheck,
			Difficulty.Hard,
			actionDescription,
			"ProgrammingActionBeginEmote",
			"ProgrammingActionContinueEmote",
			"ProgrammingActionCancelEmote",
			"ProgrammingActionSuccessEmote",
			"ProgrammingActionFailureEmote",
			successAction,
			null,
			null);
	}

	private static void StartElectricalAction(ICharacter actor, IGameItem item, string durationConfigKey,
		CheckType checkType, string traitConfigKey, string toolTagConfigKey, string actionDescription,
		string beginEmoteKey, string continueEmoteKey, string cancelEmoteKey, string successEmoteKey,
		string failureEmoteKey, Func<CheckOutcome, bool> successAction)
	{
		StartConfiguredAction(
			actor,
			item,
			toolTagConfigKey,
			traitConfigKey,
			durationConfigKey,
			checkType,
			checkType == CheckType.InstallElectricalComponentCheck ? Difficulty.Normal : Difficulty.Easy,
			actionDescription,
			beginEmoteKey,
			continueEmoteKey,
			cancelEmoteKey,
			successEmoteKey,
			failureEmoteKey,
			successAction,
			null,
			outcome => ApplyElectricalShock(actor, item));
	}

	private static void StartConfiguredAction(ICharacter actor, IGameItem item, string toolTagConfigKey,
		string traitConfigKey, string durationConfigKey, CheckType checkType, Difficulty difficulty,
		string actionDescription, string beginEmoteKey, string continueEmoteKey, string cancelEmoteKey,
		string successEmoteKey, string failureEmoteKey, Func<CheckOutcome, bool> successAction,
		Action<CheckOutcome>? failureAction, Action<CheckOutcome>? abjectFailureAction)
	{
		if (!TryAcquireToolPlan(actor, toolTagConfigKey, out var plan, out var tool))
		{
			return;
		}

		var trait = ResolveTrait(actor, traitConfigKey);
		if (trait is null)
		{
			plan.FinalisePlan();
			return;
		}

		var totalDuration = TimeSpan.FromSeconds(Math.Max(3.0, actor.Gameworld.GetStaticDouble(durationConfigKey)));
		var stageDuration = TimeSpan.FromMilliseconds(totalDuration.TotalMilliseconds / 3.0);
		var effect = new ItemComponentConfigurationAction(
			actor,
			item,
			tool,
			plan,
			actionDescription,
			actor.Gameworld.GetStaticString(beginEmoteKey),
			actor.Gameworld.GetStaticString(continueEmoteKey),
			actor.Gameworld.GetStaticString(cancelEmoteKey),
			actor.Gameworld.GetStaticString(successEmoteKey),
			actor.Gameworld.GetStaticString(failureEmoteKey),
			stageDuration,
			3,
			() => actor.Gameworld.GetCheck(checkType)
				.Check(actor, difficulty, trait, item, externalBonus: ToolQualityBonus(tool)),
			successAction,
			failureAction,
			abjectFailureAction);
		actor.AddEffect(effect, stageDuration);
	}

	private static void ShowElectricalStatus(ICharacter actor, IGameItem item)
	{
		var sources = item.Components.OfType<ISignalSourceComponent>().ToList();
		var sinks = item.Components.OfType<IRuntimeConfigurableSignalSinkComponent>().ToList();
		if (!sources.Any() && !sinks.Any())
		{
			actor.Send($"{item.HowSeen(actor, true)} has no signal-capable electrical components.");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine($"{item.HowSeen(actor, true)} has the following electrical components:");
		if (sources.Any())
		{
			sb.AppendLine();
			sb.AppendLine("Sources:");
			foreach (var source in sources.OrderBy(x => ((IGameItemComponent)x).Id))
			{
				var component = (IGameItemComponent)source;
				sb.AppendLine(
					$"\t[{component.Id.ToString("N0", actor)}] {component.Name.ColourName()} -> {source.CurrentValue.ToString("N2", actor).ColourValue()} on {source.EndpointKey.ColourCommand()}");
			}
		}

		if (sinks.Any())
		{
			sb.AppendLine();
			sb.AppendLine("Configurable Sinks:");
			foreach (var sink in sinks.OrderBy(x => ((IGameItemComponent)x).Id))
			{
				var component = (IGameItemComponent)sink;
				sb.AppendLine(
					$"\t[{component.Id.ToString("N0", actor)}] {component.Name.ColourName()} <- {SignalComponentUtilities.DescribeSignalComponent(sink.CurrentBinding).ColourCommand()}, threshold {sink.ActivationThreshold.ToString("N2", actor).ColourValue()}, mode {(sink.ActiveWhenAboveThreshold ? "above/equal".ColourValue() : "below".ColourValue())}");
			}
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	private static void ShowProgrammingStatus(ICharacter actor, IGameItem item)
	{
		var controllers = item.Components.OfType<IRuntimeProgrammableMicrocontroller>().ToList();
		if (!controllers.Any())
		{
			actor.Send($"{item.HowSeen(actor, true)} has no programmable microcontrollers.");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine($"{item.HowSeen(actor, true)} has the following programmable microcontrollers:");
		foreach (var controller in controllers.OrderBy(x => ((IGameItemComponent)x).Id))
		{
			var component = (IGameItemComponent)controller;
			sb.AppendLine(
				$"\t[{component.Id.ToString("N0", actor)}] {component.Name.ColourName()} - {(controller.LogicCompiles ? "compiled".ColourValue() : controller.CompileError.ColourError())}");
			if (!controller.InputBindings.Any())
			{
				sb.AppendLine("\t\tInputs: none");
				continue;
			}

			foreach (var binding in controller.InputBindings.OrderBy(x => x.VariableName))
			{
				sb.AppendLine(
					$"\t\t{binding.VariableName.ColourCommand()} <- {SignalComponentUtilities.DescribeSignalComponent(binding.Binding).ColourCommand()} ({binding.CurrentValue.ToString("N2", actor).ColourValue()})");
			}
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	private static TComponent? ResolveComponent<TComponent>(ICharacter actor, IGameItem item, string identifier,
		string componentTypeDescription)
		where TComponent : class, IGameItemComponent
	{
		var components = item.Components.OfType<TComponent>().ToList();
		if (!components.Any())
		{
			actor.Send($"{item.HowSeen(actor, true)} has no {componentTypeDescription}s.");
			return null;
		}

		if (long.TryParse(identifier, out var componentId))
		{
			var idMatch = components.FirstOrDefault(x => x.Id == componentId);
			if (idMatch is not null)
			{
				return idMatch;
			}
		}

		var exactMatches = components
			.Where(x => x.Name.Equals(identifier, StringComparison.InvariantCultureIgnoreCase))
			.ToList();
		if (exactMatches.Count == 1)
		{
			return exactMatches[0];
		}

		var prefixMatches = components
			.Where(x => x.Name.StartsWith(identifier, StringComparison.InvariantCultureIgnoreCase))
			.ToList();
		if (prefixMatches.Count == 1)
		{
			return prefixMatches[0];
		}

		actor.Send(
			$"You must specify one of the following {componentTypeDescription}s on {item.HowSeen(actor, true)}:\n{components.Select(x => $"\t[{x.Id.ToString("N0", actor)}] {x.Name.ColourName()}").ListToLines()}");
		return null;
	}

	private static ITraitDefinition? ResolveTrait(ICharacter actor, string traitConfigKey)
	{
		var traitText = actor.Gameworld.GetStaticConfiguration(traitConfigKey);
		if (string.IsNullOrWhiteSpace(traitText))
		{
			actor.Send($"The static configuration {traitConfigKey.ColourCommand()} is not set.");
			return null;
		}

		var trait = long.TryParse(traitText, out var traitId)
			? actor.Gameworld.Traits.Get(traitId)
			: actor.Gameworld.Traits.GetByName(traitText);
		if (trait is null)
		{
			actor.Send($"The configured trait {traitText.ColourCommand()} could not be found.");
		}

		return trait;
	}

	private static bool TryAcquireToolPlan(ICharacter actor, string toolTagConfigKey, out IInventoryPlan plan,
		out IGameItem tool)
	{
		plan = null!;
		tool = null!;

		var tagText = actor.Gameworld.GetStaticConfiguration(toolTagConfigKey);
		if (string.IsNullOrWhiteSpace(tagText))
		{
			actor.Send($"The static configuration {toolTagConfigKey.ColourCommand()} is not set.");
			return false;
		}

		var tag = long.TryParse(tagText, out var tagId)
			? actor.Gameworld.Tags.Get(tagId)
			: actor.Gameworld.Tags.GetByName(tagText);
		if (tag is null)
		{
			actor.Send($"The configured tool tag {tagText.ColourCommand()} could not be found.");
			return false;
		}

		var template = new InventoryPlanTemplate(actor.Gameworld, new[]
		{
			new InventoryPlanActionHold(actor.Gameworld, tag.Id, 0, null, null, 1)
			{
				ItemsAlreadyInPlaceOverrideFitnessScore = true
			}
		});
		plan = template.CreatePlan(actor);
		switch (plan.PlanIsFeasible())
		{
			case InventoryPlanFeasibility.Feasible:
				break;
			case InventoryPlanFeasibility.NotFeasibleMissingItems:
				actor.Send($"You need to have access to something tagged {tag.Name.ColourName()} to do that.");
				return false;
			case InventoryPlanFeasibility.NotFeasibleNotEnoughHands:
				actor.Send("You do not have enough free hands to ready the tools for that work.");
				return false;
			case InventoryPlanFeasibility.NotFeasibleNotEnoughWielders:
				actor.Send("You cannot get the necessary tools into the right state for that work.");
				return false;
			default:
				actor.Send("You cannot get your tools ready for that work right now.");
				return false;
		}

		var results = plan.ExecuteWholePlan().ToList();
		tool = results.Select(x => x.PrimaryTarget).FirstOrDefault(x => x is not null)!;
		if (tool is null)
		{
			plan.FinalisePlan();
			actor.Send("You fail to get the necessary tools ready.");
			return false;
		}

		return true;
	}

	private static void ApplyElectricalShock(ICharacter actor, IGameItem item)
	{
		actor.OutputHandler.Handle(
			new EmoteOutput(new Emote(actor.Gameworld.GetStaticString("ElectricalShockEmote"), actor, item),
				flags: OutputFlags.SuppressObscured));
		actor.Body.SufferDamage(new Damage
		{
			ActorOrigin = actor,
			ToolOrigin = item,
			DamageType = DamageType.Electrical,
			DamageAmount = actor.Gameworld.GetStaticDouble("ElectricalShockDamage"),
			Bodypart = actor.Body.RandomBodypart
		});
	}

	private static string DescribeComponent(ICharacter actor, IGameItemComponent component)
	{
		return $"[{component.Id.ToString("N0", actor)}] {component.Name}";
	}

	private static double ToolQualityBonus(IGameItem tool)
	{
		return StandardCheck.BonusesPerDifficultyLevel * ((int)tool.Quality - 5);
	}
}
