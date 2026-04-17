#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Computers;
using MudSharp.Commands.Trees;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Editor;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg;
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
	private const string ElectricalHelpText = @"The #3electrical#0 command is used to inspect, install, route, and configure signal-driven electrical systems.

You can use the following syntax:
	#3electrical <item>#0 - shows the signal sources and configurable electrical components on the item
	#3electrical install <host> <module> [<bay>]#0 - installs a loose automation module item into an automation bay
	#3electrical remove <host> <bay>#0 - removes a mounted automation module from a host item
	#3electrical route <cable> <source> <exit> [<housing>]#0 - routes a loose signal cable one room hop away and optionally places it in an automation housing or junction
	#3electrical unroute <cable>#0 - clears a routed cable segment
	#3electrical <item> bind <component> <source> [<endpoint>]#0 - rewires a configurable sink to a local source component
	#3electrical <item> clear <component>#0 - clears any live rewiring on a configurable sink, or clears a routed cable on a cable item
	#3electrical <item> threshold <component> <value>#0 - changes the component's activation threshold
	#3electrical <item> mode <component> above|below#0 - changes whether the sink activates above or below the threshold

Component and source identifiers use the parent item's normal keywords, or #6item@component#0 when you need to name a specific component on a specific item.
Duplicate nearby items can be disambiguated with the normal numeric item targeting syntax, e.g. #62.sensor#0.
Routed cables are one-room segments. Longer runs are built by chaining another cable in the next room.";

	private const string ProgrammingHelpText = @"The #3programming#0 command is used to inspect and work with computer functions, computer programs, and installed microcontrollers.

You can use the following syntax:
	#3programming list [functions|programs]#0 - lists your workspace computer executables
	#3programming new function|program <name>#0 - creates a new workspace executable and begins editing it
	#3programming edit <which>#0 - begins editing a workspace executable
	#3programming close#0 - stops editing a workspace executable
	#3programming show [<which>]#0 - shows a workspace executable
	#3programming set name <name>#0 - renames the currently edited workspace executable
	#3programming set return <type>#0 - changes the return type of the currently edited workspace executable
	#3programming set source [<text>]#0 - directly replaces the source, or opens an editor if no text is supplied
	#3programming parameter add <name> <type>#0 - adds a parameter to the currently edited workspace executable
	#3programming parameter remove <name>#0 - removes a parameter from the currently edited workspace executable
	#3programming parameter swap <first> <second>#0 - swaps parameter order on the currently edited workspace executable
	#3programming compile [<which>]#0 - compiles a workspace executable
	#3programming execute <which> [<parameters>]#0 - executes a workspace executable
	#3programming processes#0 - shows your recent computer-program processes
	#3programming kill <process>#0 - kills one of your running or sleeping computer-program processes
	#3programming help <topic>#0 - shows programming help filtered to the computer-safe language subset
	#3programming item <item>#0 - shows all programmable microcontrollers on the item
	#3programming item <item> logic [<component>]#0 - opens an editor to replace the controller logic
	#3programming item <item> logic [<component>] <text>#0 - directly replaces the controller logic
	#3programming item <item> input add [<component>] <variable> <source> [<endpoint>]#0 - binds an input variable to a local signal source
	#3programming item <item> input remove [<component>] <variable>#0 - removes an input variable

The old short form of #3programming <item>#0 still works for item-targeted microcontroller programming whenever the first word is not one of the reserved workspace verbs.
Mounted microcontrollers remain separate items, so you can target them with syntax like #6host@module#0 when they are installed behind an open access panel.";

	private static readonly HashSet<string> ReservedProgrammingWorkspaceVerbs = new(StringComparer.InvariantCultureIgnoreCase)
	{
		"help",
		"list",
		"show",
		"new",
		"edit",
		"close",
		"delete",
		"set",
		"parameter",
		"compile",
		"execute",
		"processes",
		"kill"
	};

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

		var firstToken = ss.PopSpeech();
		switch (firstToken.ToLowerInvariant())
		{
			case "install":
				ElectricalInstall(actor, ss);
				return;
			case "remove":
			case "uninstall":
				ElectricalRemove(actor, ss);
				return;
			case "route":
				ElectricalRoute(actor, ss);
				return;
			case "unroute":
			case "deroute":
				ElectricalUnroute(actor, ss);
				return;
		}

		var item = actor.TargetItem(firstToken);
		if (item is null)
		{
			actor.Send("You do not see anything like that to work on.");
			return;
		}

		var manipulation = CanManipulateElectronicsTarget(actor, item);
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

		var firstToken = ss.PopSpeech();
		if (firstToken.EqualTo("item"))
		{
			ProgrammingItem(actor, ss);
			return;
		}

		if (ReservedProgrammingWorkspaceVerbs.Contains(firstToken))
		{
			ProgrammingWorkspace(actor, firstToken, ss);
			return;
		}

		ProgrammingItem(actor,
			new StringStack(ss.IsFinished ? firstToken : $"{firstToken} {ss.RemainingArgument}"));
	}

	private static void ProgrammingWorkspace(ICharacter actor, string verb, StringStack ss)
	{
		switch (verb.ToLowerInvariant())
		{
			case "help":
				ProgrammingWorkspaceHelp(actor, ss);
				return;
			case "list":
				ProgrammingWorkspaceList(actor, ss);
				return;
			case "show":
				ProgrammingWorkspaceShow(actor, ss);
				return;
			case "new":
				ProgrammingWorkspaceNew(actor, ss);
				return;
			case "edit":
				ProgrammingWorkspaceEdit(actor, ss);
				return;
			case "close":
				ProgrammingWorkspaceClose(actor);
				return;
			case "delete":
				ProgrammingWorkspaceDelete(actor, ss);
				return;
			case "set":
				ProgrammingWorkspaceSet(actor, ss);
				return;
			case "parameter":
				ProgrammingWorkspaceParameter(actor, ss);
				return;
			case "compile":
				ProgrammingWorkspaceCompile(actor, ss);
				return;
			case "execute":
				ProgrammingWorkspaceExecute(actor, ss);
				return;
			case "processes":
				ProgrammingWorkspaceProcesses(actor);
				return;
			case "kill":
				ProgrammingWorkspaceKill(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(ProgrammingHelpText.SubstituteANSIColour());
				return;
		}
	}

	private static void ProgrammingItem(ICharacter actor, StringStack ss)
	{
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

		var manipulation = CanManipulateElectronicsTarget(actor, item);
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

	private static void ElectricalInstall(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Which automation host item do you want to install a module into?");
			return;
		}

		var hostItem = actor.TargetItem(ss.PopSpeech());
		if (hostItem is null)
		{
			actor.Send("You do not see any such automation host here.");
			return;
		}

		var hostManipulation = CanManipulateElectronicsTarget(actor, hostItem);
		if (!hostManipulation.Truth)
		{
			actor.Send(hostManipulation.Message);
			return;
		}

		var host = ResolveAutomationMountHost(actor, hostItem);
		if (host is null)
		{
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("Which loose automation module item do you want to install?");
			return;
		}

		var moduleItem = actor.TargetItem(ss.PopSpeech());
		if (moduleItem is null)
		{
			actor.Send("You do not see any such automation module here.");
			return;
		}

		var moduleManipulation = CanManipulateElectronicsTarget(actor, moduleItem);
		if (!moduleManipulation.Truth)
		{
			actor.Send(moduleManipulation.Message);
			return;
		}

		var module = moduleItem.GetItemType<IAutomationMountable>();
		if (module is null)
		{
			actor.Send($"{moduleItem.HowSeen(actor, true)} is not an installable automation module.");
			return;
		}

		var bayName = ss.IsFinished
			? ResolveDefaultCompatibleMountBay(actor, host, module, hostItem)
			: ss.SafeRemainingArgument.Trim();
		if (string.IsNullOrWhiteSpace(bayName))
		{
			return;
		}

		if (!TryAcquireSpecificItemPlan(actor, moduleItem, out var modulePlan))
		{
			return;
		}

		StartElectricalAction(
			actor,
			hostItem,
			"ElectricalInstallActionDurationSeconds",
			CheckType.InstallElectricalComponentCheck,
			"ElectricalInstallTraitName",
			"ElectricalToolTagName",
			"installing $1 into $0",
			"ElectricalInstallActionBeginEmote",
			"ElectricalInstallActionContinueEmote",
			"ElectricalInstallActionCancelEmote",
			"ElectricalInstallActionSuccessEmote",
			"ElectricalInstallActionFailureEmote",
			outcome =>
			{
				if (!host.InstallModule(actor, module, bayName, out var error))
				{
					actor.Send(error);
					return false;
				}

				actor.Send(
					$"You install {moduleItem.HowSeen(actor, true).ColourName()} into the {bayName.ColourCommand()} bay on {hostItem.HowSeen(actor, true).ColourName()}.");
				return true;
			},
			[modulePlan],
			_ => [moduleItem],
			() => EnumerateShockRiskItems(hostItem, moduleItem));
	}

	private static void ElectricalRemove(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Which automation host item do you want to remove a module from?");
			return;
		}

		var hostItem = actor.TargetItem(ss.PopSpeech());
		if (hostItem is null)
		{
			actor.Send("You do not see any such automation host here.");
			return;
		}

		var hostManipulation = CanManipulateElectronicsTarget(actor, hostItem);
		if (!hostManipulation.Truth)
		{
			actor.Send(hostManipulation.Message);
			return;
		}

		var host = ResolveAutomationMountHost(actor, hostItem);
		if (host is null)
		{
			return;
		}

		var bayName = ss.IsFinished ? ResolveDefaultOccupiedMountBay(actor, host, hostItem) : ss.SafeRemainingArgument.Trim();
		if (string.IsNullOrWhiteSpace(bayName))
		{
			return;
		}

		StartElectricalAction(
			actor,
			hostItem,
			"ElectricalInstallActionDurationSeconds",
			CheckType.InstallElectricalComponentCheck,
			"ElectricalInstallTraitName",
			"ElectricalToolTagName",
			"removing a module from $0",
			"ElectricalInstallActionBeginEmote",
			"ElectricalInstallActionContinueEmote",
			"ElectricalInstallActionCancelEmote",
			"ElectricalInstallActionSuccessEmote",
			"ElectricalInstallActionFailureEmote",
			outcome =>
			{
				if (!host.RemoveModule(actor, bayName, out var moduleItem, out var error))
				{
					actor.Send(error);
					return false;
				}

				actor.Send(
					$"You remove {moduleItem!.HowSeen(actor, true).ColourName()} from the {bayName.ColourCommand()} bay on {hostItem.HowSeen(actor, true).ColourName()}.");
				return true;
			},
			null,
			null,
			() => EnumerateShockRiskItems(hostItem));
	}

	private static void ElectricalRoute(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Which loose signal cable item do you want to route?");
			return;
		}

		var cableItem = actor.TargetItem(ss.PopSpeech());
		if (cableItem is null)
		{
			actor.Send("You do not see any such signal cable here.");
			return;
		}

		var cableManipulation = CanManipulateElectronicsTarget(actor, cableItem);
		if (!cableManipulation.Truth)
		{
			actor.Send(cableManipulation.Message);
			return;
		}

		var cable = ResolveSignalCableSegment(actor, cableItem);
		if (cable is null)
		{
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("Which signal source should this cable mirror?");
			return;
		}

		var source = ResolveActorLocalSignalSource(actor, ss.PopSpeech());
		if (source is null)
		{
			return;
		}

		if (!CanServiceElectronicsTarget(actor, source.Parent, out var sourceError))
		{
			actor.Send(sourceError);
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("Through which exit should the cable be routed?");
			return;
		}

		var exit = actor.Location.GetExitKeyword(ss.PopSpeech(), actor);
		if (exit is null)
		{
			actor.Send("You do not see any such exit to route the cable through.");
			return;
		}

		var destinationCell = exit.Destination;
		IGameItem? housingItem = null;
		IAutomationHousing? destinationHousing = null;
		IContainer? destinationContainer = null;
		if (!ss.IsFinished)
		{
			var housingIdentifier = ss.SafeRemainingArgument;
			var (target, path) = actor.TargetDistantItem(housingIdentifier, exit, 1, true, false);
			if (target is null || target.TrueLocations.All(x => x != destinationCell) || path.All(x => x != exit))
			{
				actor.Send("You do not see any such adjacent-room housing or junction there.");
				return;
			}

			housingItem = target;
			destinationHousing = ResolveAutomationHousing(actor, housingItem);
			if (destinationHousing is null)
			{
				return;
			}

			if (!destinationHousing.CanAccessHousing(actor, out var housingError))
			{
				actor.Send(housingError);
				return;
			}

			if (!destinationHousing.CanConcealItem(cableItem, out housingError))
			{
				actor.Send(housingError);
				return;
			}

			destinationContainer = housingItem.GetItemType<IContainer>();
			if (destinationContainer is null)
			{
				actor.Send($"{housingItem.HowSeen(actor, true)} is not configured with a service cavity.");
				return;
			}

			if (!destinationContainer.CanPut(cableItem))
			{
				actor.Send(destinationContainer.WhyCannotPut(cableItem) switch
				{
					WhyCannotPutReason.ContainerClosed => $"{housingItem.HowSeen(actor, true)} is closed.",
					WhyCannotPutReason.ContainerFull => $"{housingItem.HowSeen(actor, true)} is too full to accept that cable.",
					WhyCannotPutReason.ContainerFullButCouldAcceptLesserQuantity => $"{housingItem.HowSeen(actor, true)} does not have enough room for that cable.",
					WhyCannotPutReason.ItemTooLarge => $"{cableItem.HowSeen(actor, true)} is too large to fit into {housingItem.HowSeen(actor, true)}.",
					WhyCannotPutReason.NotCorrectItemType => $"{housingItem.HowSeen(actor, true)} is not configured to accept that cable.",
					WhyCannotPutReason.CantPutContainerInItself => "You cannot put an item inside itself.",
					_ => $"{housingItem.HowSeen(actor, true)} cannot accept that cable."
				});
				return;
			}
		}

		if (!TryAcquireSpecificItemPlan(actor, cableItem, out var cablePlan))
		{
			return;
		}

		StartElectricalAction(
			actor,
			cableItem,
			"ElectricalInstallActionDurationSeconds",
			CheckType.InstallElectricalComponentCheck,
			"ElectricalInstallTraitName",
			"ElectricalToolTagName",
			"routing $0",
			"ElectricalInstallActionBeginEmote",
			"ElectricalInstallActionContinueEmote",
			"ElectricalInstallActionCancelEmote",
			"ElectricalInstallActionSuccessEmote",
			"ElectricalInstallActionFailureEmote",
			outcome =>
			{
				if (!RelocateLooseItem(cableItem, destinationCell, destinationContainer, out var error))
				{
					actor.Send(error);
					return false;
				}

				if (!cable.ConfigureRoute(source, exit.Exit.Id, out error))
				{
					actor.Send(error);
					return false;
				}

				if (housingItem is null)
				{
					actor.Send(
						$"You route {cableItem.HowSeen(actor, true).ColourName()} through {exit.OutboundDirectionDescription.ColourCommand()} so it now mirrors {DescribeComponent(actor, source).ColourName()} one room away.");
				}
				else
				{
					actor.Send(
						$"You route {cableItem.HowSeen(actor, true).ColourName()} through {exit.OutboundDirectionDescription.ColourCommand()} into {housingItem.HowSeen(actor, true).ColourName()}, where it now mirrors {DescribeComponent(actor, source).ColourName()}.");
				}

				return true;
			},
			[cablePlan],
			_ => [cableItem],
			() => EnumerateShockRiskItems(cableItem, source.Parent, housingItem));
	}

	private static void ElectricalUnroute(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Which signal cable item do you want to unroute?");
			return;
		}

		var cableItem = actor.TargetItem(ss.PopSpeech());
		if (cableItem is null)
		{
			actor.Send("You do not see any such signal cable here.");
			return;
		}

		var manipulation = CanManipulateElectronicsTarget(actor, cableItem);
		if (!manipulation.Truth)
		{
			actor.Send(manipulation.Message);
			return;
		}

		var cable = ResolveSignalCableSegment(actor, cableItem);
		if (cable is null)
		{
			return;
		}

		if (!cable.IsRouted)
		{
			actor.Send($"{cableItem.HowSeen(actor, true)} is not currently routed to anything.");
			return;
		}

		StartElectricalAction(
			actor,
			cableItem,
			"ElectricalConfigureActionDurationSeconds",
			CheckType.ConfigureElectricalComponentCheck,
			"ElectricalConfigureTraitName",
			"ElectricalToolTagName",
			"disconnecting $0",
			"ElectricalConfigureActionBeginEmote",
			"ElectricalConfigureActionContinueEmote",
			"ElectricalConfigureActionCancelEmote",
			"ElectricalConfigureActionSuccessEmote",
			"ElectricalConfigureActionFailureEmote",
			outcome =>
			{
				cable.ClearRoute();
				actor.Send($"You disconnect and clear the routing on {cableItem.HowSeen(actor, true).ColourName()}.");
				return true;
			},
			null,
			null,
			() => EnumerateShockRiskItems(cableItem));
	}

	private static void ElectricalBind(ICharacter actor, IGameItem item, StringStack ss)
	{
		if (!CanServiceElectronicsTarget(actor, item, out var serviceError))
		{
			actor.Send(serviceError);
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("Which configurable electrical component do you want to rewire?");
			return;
		}

		var sink = ResolveComponentOnItem<IRuntimeConfigurableSignalSinkComponent>(actor, item, ss.PopSpeech(),
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

		var source = ResolveNearbySignalSource(actor, item, ss.PopSpeech(), "signal source component");
		if (source is null)
		{
			return;
		}

		if (!CanServiceElectronicsTarget(actor, source.Parent, out var sourceError))
		{
			actor.Send(sourceError);
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
			},
			null,
			null,
			() => EnumerateShockRiskItems(item, source.Parent));
	}

	private static void ElectricalClear(ICharacter actor, IGameItem item, StringStack ss)
	{
		if (!CanServiceElectronicsTarget(actor, item, out var serviceError))
		{
			actor.Send(serviceError);
			return;
		}

		if (ss.IsFinished)
		{
			if (TryResolveSingleComponentOnItem<ISignalCableSegment>(item, out var singleCable))
			{
				StartElectricalAction(
					actor,
					item,
					"ElectricalConfigureActionDurationSeconds",
					CheckType.ConfigureElectricalComponentCheck,
					"ElectricalConfigureTraitName",
					"ElectricalToolTagName",
					"disconnecting $0",
					"ElectricalConfigureActionBeginEmote",
					"ElectricalConfigureActionContinueEmote",
					"ElectricalConfigureActionCancelEmote",
					"ElectricalConfigureActionSuccessEmote",
					"ElectricalConfigureActionFailureEmote",
					outcome =>
					{
						singleCable.ClearRoute();
						actor.Send($"You clear the routing on {item.HowSeen(actor, true).ColourName()}.");
						return true;
					},
					null,
					null,
					() => EnumerateShockRiskItems(item));
				return;
			}

			actor.Send("Which configurable electrical component do you want to clear?");
			return;
		}

		var identifier = ss.PopSpeech();
		var sink = TryResolveComponentOnItem<IRuntimeConfigurableSignalSinkComponent>(item, identifier);
		if (sink is not null)
		{
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
				},
				null,
				null,
				() => EnumerateShockRiskItems(item));
			return;
		}

		var cable = TryResolveComponentOnItem<ISignalCableSegment>(item, identifier);
		if (cable is not null)
		{
			StartElectricalAction(
				actor,
				item,
				"ElectricalConfigureActionDurationSeconds",
				CheckType.ConfigureElectricalComponentCheck,
				"ElectricalConfigureTraitName",
				"ElectricalToolTagName",
				"disconnecting $0",
				"ElectricalConfigureActionBeginEmote",
				"ElectricalConfigureActionContinueEmote",
				"ElectricalConfigureActionCancelEmote",
				"ElectricalConfigureActionSuccessEmote",
				"ElectricalConfigureActionFailureEmote",
				outcome =>
				{
					cable.ClearRoute();
					actor.Send($"You clear the routing on {DescribeComponent(actor, cable).ColourName()}.");
					return true;
				},
				null,
				null,
				() => EnumerateShockRiskItems(item));
			return;
		}

		actor.Send(
			$"You must specify one of the following configurable electrical components on {item.HowSeen(actor, true)}:\n{DescribeAvailableElectricalTargets(actor, item)}");
	}

	private static void ElectricalThreshold(ICharacter actor, IGameItem item, StringStack ss)
	{
		if (!CanServiceElectronicsTarget(actor, item, out var serviceError))
		{
			actor.Send(serviceError);
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("Which configurable electrical component do you want to retune?");
			return;
		}

		var sink = ResolveComponentOnItem<IRuntimeConfigurableSignalSinkComponent>(actor, item, ss.PopSpeech(),
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
			},
			null,
			null,
			() => EnumerateShockRiskItems(item));
	}

	private static void ElectricalMode(ICharacter actor, IGameItem item, StringStack ss)
	{
		if (!CanServiceElectronicsTarget(actor, item, out var serviceError))
		{
			actor.Send(serviceError);
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("Which configurable electrical component do you want to reconfigure?");
			return;
		}

		var sink = ResolveComponentOnItem<IRuntimeConfigurableSignalSinkComponent>(actor, item, ss.PopSpeech(),
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
			},
			null,
			null,
			() => EnumerateShockRiskItems(item));
	}

	private static void ProgrammingWorkspaceHelp(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(ProgrammingHelpText.SubstituteANSIColour());
			return;
		}

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "types":
				actor.OutputHandler.Send(
					ComputerHelpFormatter.GetTypesText(
						actor.Gameworld.ComputerHelpService.GetAvailableTypes(FutureProgCompilationContext.ComputerProgram),
						true),
					nopage: true);
				return;
			case "type":
				ProgrammingWorkspaceHelpType(actor, ss);
				return;
			case "statements":
				actor.OutputHandler.Send(
					ComputerHelpFormatter.GetStatementsText(
						actor.Gameworld.ComputerHelpService.GetStatementHelp(FutureProgCompilationContext.ComputerProgram),
						GetProgrammingStatementAvailability,
						actor.LineFormatLength,
						actor.Account.UseUnicode,
						true),
					nopage: true);
				return;
			case "statement":
				ProgrammingWorkspaceHelpStatement(actor, ss);
				return;
			case "functions":
				ProgrammingWorkspaceHelpFunctions(actor, ss);
				return;
			case "function":
				ProgrammingWorkspaceHelpFunction(actor, ss);
				return;
			case "collections":
				actor.OutputHandler.Send(
					ComputerHelpFormatter.GetCollectionsText(
						GetProgrammingCollectionHelpInfos(actor),
						actor.LineFormatLength,
						actor.Account.UseUnicode,
						true),
					nopage: true);
				return;
			case "collection":
				ProgrammingWorkspaceHelpCollection(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(ProgrammingHelpText.SubstituteANSIColour());
				return;
		}
	}

	private static void ProgrammingWorkspaceHelpType(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("What type do you want to see property help for?");
			return;
		}

		var type = MudSharp.FutureProg.FutureProg.GetTypeByName(ss.SafeRemainingArgument);
		if (type == ProgVariableTypes.Error ||
		    !ComputerCompilationRestrictions.IsTypeAllowedInContext(type, FutureProgCompilationContext.ComputerProgram))
		{
			actor.Send("That is not a programming-safe type.");
			return;
		}

		var text = ProgModule.GetProgTypeHelpText(type, actor.LineFormatLength, actor.Account.UseUnicode, true);
		actor.OutputHandler.Send(text ?? $"The type {type.Describe().ColourName()} does not have any help.", nopage: true);
	}

	private static void ProgrammingWorkspaceHelpStatement(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Which statement do you want to see help for?");
			return;
		}

		var statement = ss.SafeRemainingArgument;
		var help = actor.Gameworld.ComputerHelpService.GetStatementHelp(statement, FutureProgCompilationContext.ComputerProgram);
		if (help is null)
		{
			actor.Send("There is no such programming-safe statement.");
			return;
		}

		actor.OutputHandler.Send(
			ComputerHelpFormatter.GetStatementText(
				statement,
				help.Value,
				GetProgrammingStatementAvailability(statement),
				actor.LineFormatLength,
				true));
	}

	private static void ProgrammingWorkspaceHelpFunctions(ICharacter actor, StringStack ss)
	{
		var infos = GetProgrammingFunctionHelpInfos(actor);
		if (!ss.IsFinished)
		{
			var category = ss.SafeRemainingArgument;
			infos = infos
				.Where(x => x.Category.EqualTo(category) ||
				            x.Category.StartsWith(category, StringComparison.InvariantCultureIgnoreCase))
				.ToList();
		}

		actor.OutputHandler.Send(
			ComputerHelpFormatter.GetFunctionsText(infos, actor.LineFormatLength, actor.Account.UseUnicode, true),
			nopage: true);
	}

	private static void ProgrammingWorkspaceHelpFunction(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Which function do you want to see help for?");
			return;
		}

		var which = ss.SafeRemainingArgument;
		var functions = GetProgrammingFunctionHelpInfos(actor)
			.Where(x => x.FunctionName.EqualTo(which))
			.ToList();
		if (!functions.Any())
		{
			actor.Send("There are no such programming-safe functions.");
			return;
		}

		actor.OutputHandler.Send(
			ComputerHelpFormatter.GetFunctionText(
				functions,
				actor.LineFormatLength,
				actor.InnerLineFormatLength,
				actor.Account.UseUnicode,
				true));
	}

	private static void ProgrammingWorkspaceHelpCollection(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Which collection function do you want to see help for?");
			return;
		}

		var which = ss.SafeRemainingArgument;
		var info = GetProgrammingCollectionHelpInfos(actor).FirstOrDefault(x => x.FunctionName.EqualTo(which));
		if (info is null)
		{
			actor.Send("There is no such programming-safe collection function.");
			return;
		}

		actor.OutputHandler.Send(ComputerHelpFormatter.GetCollectionText(info, actor.LineFormatLength, true));
	}

	private static List<ComputerFunctionHelpInfo> GetProgrammingFunctionHelpInfos(ICharacter actor)
	{
		return actor.Gameworld.ComputerHelpService.GetFunctionHelp(FutureProgCompilationContext.ComputerFunction)
			.Concat(actor.Gameworld.ComputerHelpService.GetFunctionHelp(FutureProgCompilationContext.ComputerProgram))
			.GroupBy(x =>
				$"{x.FunctionName}|{x.ReturnType.ToStorageString()}|{string.Join(",", x.Parameters.Select(y => y.ToStorageString()))}")
			.Select(group =>
			{
				var first = group.First();
				return new ComputerFunctionHelpInfo
				{
					FunctionName = first.FunctionName,
					Parameters = first.Parameters.ToList(),
					ParameterNames = first.ParameterNames.ToList(),
					ParameterHelp = first.ParameterHelp.ToList(),
					FunctionHelp = first.FunctionHelp,
					Category = first.Category,
					ReturnType = first.ReturnType,
					AllowedContexts = group.SelectMany(x => x.AllowedContexts).Distinct().ToList()
				};
			})
			.OrderBy(x => x.Category)
			.ThenBy(x => x.FunctionName)
			.ThenBy(x => x.Parameters.Count())
			.ToList();
	}

	private static List<ComputerCollectionHelpInfo> GetProgrammingCollectionHelpInfos(ICharacter actor)
	{
		return actor.Gameworld.ComputerHelpService.GetCollectionHelp(FutureProgCompilationContext.ComputerProgram)
			.GroupBy(x => $"{x.FunctionName}|{x.InnerFunctionReturnType.ToStorageString()}|{x.FunctionReturnInfo}")
			.Select(x => x.First())
			.OrderBy(x => x.FunctionName)
			.ToList();
	}

	private static string GetProgrammingStatementAvailability(string statement)
	{
		return statement.EqualTo("sleep") ? "Program" : "Both";
	}

	private static void ProgrammingWorkspaceList(ICharacter actor, StringStack ss)
	{
		var executables = actor.Gameworld.ComputerExecutionService.GetExecutables(actor).ToList();
		if (!executables.Any())
		{
			actor.Send("You do not have any workspace computer executables.");
			return;
		}

		if (!ss.IsFinished)
		{
			var filter = ss.PopSpeech().ToLowerInvariant();
			executables = filter switch
			{
				"function" or "functions" => executables.Where(x => x.ExecutableKind == ComputerExecutableKind.Function).ToList(),
				"program" or "programs" => executables.Where(x => x.ExecutableKind == ComputerExecutableKind.Program).ToList(),
				_ => executables
			};
		}

		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			executables.Select(x => new List<string>
			{
				x.Id.ToString("N0", actor),
				x.Name,
				x.ExecutableKind.DescribeEnum(),
				x.ReturnType.Describe(),
				x.CompilationStatus == ComputerCompilationStatus.Compiled ? "Compiled" : x.CompilationStatus.DescribeEnum()
			}),
			new List<string>
			{
				"Id",
				"Name",
				"Kind",
				"Return",
				"Status"
			},
			actor.LineFormatLength,
			true,
			Telnet.BoldGreen,
			1,
			actor.Account.UseUnicode), nopage: true);
	}

	private static void ProgrammingWorkspaceNew(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Do you want to create a function or a program?");
			return;
		}

		var kindText = ss.PopSpeech().ToLowerInvariant();
		var kind = kindText switch
		{
			"function" => ComputerExecutableKind.Function,
			"program" => ComputerExecutableKind.Program,
			_ => (ComputerExecutableKind?)null
		};
		if (!kind.HasValue)
		{
			actor.Send("You must specify either function or program.");
			return;
		}

		var name = ss.IsFinished ? $"Unnamed{kind.Value.DescribeEnum()}" : ss.SafeRemainingArgument.Trim();
		if (actor.Gameworld.ComputerExecutionService.GetExecutables(actor)
			    .Any(x => x.Name.EqualTo(name)))
		{
			actor.Send("You already have a workspace executable with that name.");
			return;
		}

		var executable = actor.Gameworld.ComputerExecutionService.CreateExecutable(actor, kind.Value, name);
		SetEditingComputerExecutable(actor, executable);
		actor.Send(
			$"You create the {kind.Value.DescribeEnum().ToLowerInvariant().ColourValue()} {executable.Name.ColourName()} [{executable.Id.ToString("N0", actor).ColourValue()}], which you are now editing.");
	}

	private static void ProgrammingWorkspaceEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var editing = GetEditingComputerExecutable(actor);
			if (editing is null)
			{
				actor.Send("Which workspace executable do you want to edit?");
				return;
			}

			ShowWorkspaceExecutable(actor, editing);
			return;
		}

		var executable = ResolveWorkspaceExecutable(actor, ss.SafeRemainingArgument);
		if (executable is null)
		{
			return;
		}

		SetEditingComputerExecutable(actor, executable);
		actor.Send($"You are now editing {executable.Name.ColourName()} [{executable.Id.ToString("N0", actor).ColourValue()}].");
	}

	private static void ProgrammingWorkspaceClose(ICharacter actor)
	{
		var editing = GetEditingComputerExecutable(actor);
		if (editing is null)
		{
			actor.Send("You are not currently editing any workspace executable.");
			return;
		}

		actor.RemoveAllEffects<ComputerExecutableEditingEffect>();
		actor.Send($"You are no longer editing {editing.Name.ColourName()}.");
	}

	private static void ProgrammingWorkspaceShow(ICharacter actor, StringStack ss)
	{
		var executable = ss.IsFinished ? GetEditingComputerExecutable(actor) : ResolveWorkspaceExecutable(actor, ss.SafeRemainingArgument);
		if (executable is null)
		{
			actor.Send(ss.IsFinished
				? "You are not currently editing any workspace executable."
				: "There is no such workspace executable.");
			return;
		}

		ShowWorkspaceExecutable(actor, executable);
	}

	private static void ProgrammingWorkspaceDelete(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Which workspace executable do you want to delete?");
			return;
		}

		var executable = ResolveWorkspaceExecutable(actor, ss.SafeRemainingArgument);
		if (executable is null)
		{
			return;
		}

		if (!actor.Gameworld.ComputerExecutionService.DeleteExecutable(actor, executable, out var error))
		{
			actor.Send(error);
			return;
		}

		if (GetEditingComputerExecutable(actor)?.Id == executable.Id)
		{
			actor.RemoveAllEffects<ComputerExecutableEditingEffect>();
		}

		actor.Send($"You delete the workspace executable {executable.Name.ColourName()}.");
	}

	private static void ProgrammingWorkspaceSet(ICharacter actor, StringStack ss)
	{
		var executable = GetEditingComputerExecutable(actor);
		if (executable is null)
		{
			actor.Send("You are not currently editing any workspace executable.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(ProgrammingHelpText.SubstituteANSIColour());
			return;
		}

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "name":
				ProgrammingWorkspaceSetName(actor, executable, ss);
				return;
			case "return":
			case "returns":
				ProgrammingWorkspaceSetReturn(actor, executable, ss);
				return;
			case "source":
			case "text":
				ProgrammingWorkspaceSetSource(actor, executable, ss);
				return;
			default:
				actor.OutputHandler.Send(ProgrammingHelpText.SubstituteANSIColour());
				return;
		}
	}

	private static void ProgrammingWorkspaceSetName(ICharacter actor, IComputerExecutableDefinition executable, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("What new name do you want to give to that workspace executable?");
			return;
		}

		var name = ss.SafeRemainingArgument.Trim();
		if (actor.Gameworld.ComputerExecutionService.GetExecutables(actor)
			    .Any(x => x.Id != executable.Id && x.Name.EqualTo(name)))
		{
			actor.Send("You already have another workspace executable with that name.");
			return;
		}

		((ComputerWorkspaceExecutableBase)executable).Name = name;
		actor.Gameworld.ComputerExecutionService.SaveExecutable(executable);
		AutoCompileWorkspaceExecutable(actor, executable,
			$"You rename the workspace executable to {name.ColourName()}.");
	}

	private static void ProgrammingWorkspaceSetReturn(ICharacter actor, IComputerExecutableDefinition executable,
		StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("What return type do you want to use?");
			return;
		}

		var type = MudSharp.FutureProg.FutureProg.GetTypeByName(ss.SafeRemainingArgument);
		if (type == ProgVariableTypes.Error ||
		    !ComputerCompilationRestrictions.IsTypeAllowedInContext(type, executable.CompilationContext))
		{
			actor.Send("That is not a valid programming-safe return type for that executable.");
			return;
		}

		((ComputerWorkspaceExecutableBase)executable).ReturnType = type;
		actor.Gameworld.ComputerExecutionService.SaveExecutable(executable);
		AutoCompileWorkspaceExecutable(actor, executable,
			$"You change the return type of {executable.Name.ColourName()} to {type.Describe().ColourValue()}.");
	}

	private static void ProgrammingWorkspaceSetSource(ICharacter actor, IComputerExecutableDefinition executable,
		StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Enter the replacement source code in the editor below.");
			actor.EditorMode(
				(text, handler, _) =>
				{
					((ComputerWorkspaceExecutableBase)executable).SourceCode = text;
					actor.Gameworld.ComputerExecutionService.SaveExecutable(executable);
					AutoCompileWorkspaceExecutable(actor, executable,
						$"You replace the source code for {executable.Name.ColourName()}.");
				},
				(handler, _) => handler.Send("You decide not to change the workspace source code."),
				1.0,
				recallText: executable.SourceCode,
				options: EditorOptions.PermitEmpty);
			return;
		}

		((ComputerWorkspaceExecutableBase)executable).SourceCode = ss.SafeRemainingArgument;
		actor.Gameworld.ComputerExecutionService.SaveExecutable(executable);
		AutoCompileWorkspaceExecutable(actor, executable,
			$"You replace the source code for {executable.Name.ColourName()}.");
	}

	private static void ProgrammingWorkspaceParameter(ICharacter actor, StringStack ss)
	{
		var executable = GetEditingComputerExecutable(actor);
		if (executable is null)
		{
			actor.Send("You are not currently editing any workspace executable.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("Do you want to add, remove, or swap a parameter?");
			return;
		}

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "add":
				ProgrammingWorkspaceParameterAdd(actor, executable, ss);
				return;
			case "remove":
			case "delete":
				ProgrammingWorkspaceParameterRemove(actor, executable, ss);
				return;
			case "swap":
				ProgrammingWorkspaceParameterSwap(actor, executable, ss);
				return;
			default:
				actor.Send("Do you want to add, remove, or swap a parameter?");
				return;
		}
	}

	private static void ProgrammingWorkspaceParameterAdd(ICharacter actor, IComputerExecutableDefinition executable,
		StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("What variable name do you want to add?");
			return;
		}

		var variableName = ss.PopSpeech().Trim().ToLowerInvariant();
		if (!ComputerExecutableCompiler.IsValidVariableName(variableName))
		{
			actor.Send("That is not a valid variable name.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("What parameter type should that variable use?");
			return;
		}

		var type = MudSharp.FutureProg.FutureProg.GetTypeByName(ss.SafeRemainingArgument);
		if (type == ProgVariableTypes.Error ||
		    !ComputerCompilationRestrictions.IsTypeAllowedInContext(type, executable.CompilationContext))
		{
			actor.Send("That is not a valid programming-safe parameter type for that executable.");
			return;
		}

		var parameters = executable.Parameters.ToList();
		if (parameters.Any(x => x.Name.EqualTo(variableName)))
		{
			actor.Send("That executable already has a parameter with that name.");
			return;
		}

		parameters.Add(new ComputerExecutableParameter(variableName, type));
		((ComputerWorkspaceExecutableBase)executable).Parameters = parameters;
		actor.Gameworld.ComputerExecutionService.SaveExecutable(executable);
		AutoCompileWorkspaceExecutable(actor, executable,
			$"You add the parameter {variableName.ColourCommand()} as {type.Describe().ColourValue()}.");
	}

	private static void ProgrammingWorkspaceParameterRemove(ICharacter actor, IComputerExecutableDefinition executable,
		StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Which parameter do you want to remove?");
			return;
		}

		var variableName = ss.PopSpeech().Trim();
		var parameters = executable.Parameters.ToList();
		var parameter = parameters.FirstOrDefault(x => x.Name.EqualTo(variableName));
		if (parameter == default)
		{
			actor.Send("There is no such parameter on that executable.");
			return;
		}

		parameters.Remove(parameter);
		((ComputerWorkspaceExecutableBase)executable).Parameters = parameters;
		actor.Gameworld.ComputerExecutionService.SaveExecutable(executable);
		AutoCompileWorkspaceExecutable(actor, executable,
			$"You remove the parameter {parameter.Name.ColourCommand()}.");
	}

	private static void ProgrammingWorkspaceParameterSwap(ICharacter actor, IComputerExecutableDefinition executable,
		StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Which parameter do you want to move?");
			return;
		}

		var first = ss.PopSpeech();
		if (ss.IsFinished)
		{
			actor.Send("Which parameter do you want to swap it with?");
			return;
		}

		var second = ss.PopSpeech();
		var parameters = executable.Parameters.ToList();
		var firstIndex = parameters.FindIndex(x => x.Name.EqualTo(first));
		var secondIndex = parameters.FindIndex(x => x.Name.EqualTo(second));
		if (firstIndex == -1 || secondIndex == -1)
		{
			actor.Send("You must specify two existing parameters.");
			return;
		}

		(parameters[firstIndex], parameters[secondIndex]) = (parameters[secondIndex], parameters[firstIndex]);
		((ComputerWorkspaceExecutableBase)executable).Parameters = parameters;
		actor.Gameworld.ComputerExecutionService.SaveExecutable(executable);
		AutoCompileWorkspaceExecutable(actor, executable,
			$"You swap the order of {first.ColourCommand()} and {second.ColourCommand()}.");
	}

	private static void ProgrammingWorkspaceCompile(ICharacter actor, StringStack ss)
	{
		var executable = ss.IsFinished ? GetEditingComputerExecutable(actor) : ResolveWorkspaceExecutable(actor, ss.SafeRemainingArgument);
		if (executable is null)
		{
			actor.Send(ss.IsFinished
				? "You are not currently editing any workspace executable."
				: "There is no such workspace executable.");
			return;
		}

		AutoCompileWorkspaceExecutable(actor, executable, $"You compile {executable.Name.ColourName()}.");
	}

	private static void ProgrammingWorkspaceExecute(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Which workspace executable do you want to execute?");
			return;
		}

		var executable = ResolveWorkspaceExecutable(actor, ss.PopSpeech());
		if (executable is null)
		{
			return;
		}

		List<object?> parameters = new();
		for (var i = 0; i < executable.Parameters.Count; i++)
		{
			var parameter = executable.Parameters.ElementAt(i);
			var raw = ss.PopParentheses();
			if (string.IsNullOrEmpty(raw))
			{
				raw = ss.PopSpeech();
			}

			if (string.IsNullOrEmpty(raw))
			{
				actor.Send(
					$"You must supply a value for parameter {parameter.Name.ColourCommand()} of type {parameter.Type.Describe().ColourValue()}.");
				return;
			}

			var outcome = ProgModule.GetArgument(parameter.Type, raw, i + 1, actor);
			if (!outcome.success)
			{
				return;
			}

			parameters.Add(outcome.result);
		}

		var result = actor.Gameworld.ComputerExecutionService.Execute(actor, executable, parameters);
		if (!result.Success && result.Status != ComputerProcessStatus.Completed)
		{
			actor.Send(result.ErrorMessage);
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Executing {executable.Name.ColourName()} [{executable.Id.ToString("N0", actor).ColourValue()}]...");
		if (parameters.Any())
		{
			sb.AppendLine();
			foreach (var parameter in executable.Parameters.Select((value, index) => (value, index)))
			{
				sb.AppendLine(
					$"\t{parameter.value.Type.Describe().ColourValue()} {parameter.value.Name.ColourCommand()}: {ProgModule.DescribeProgVariable(actor, parameter.value.Type, parameters[parameter.index])}");
			}
		}

		if (result.Process is not null)
		{
			sb.AppendLine();
			sb.AppendLine($"Process: {result.Process.Id.ToString("N0", actor).ColourValue()} ({result.Status.DescribeEnum().ColourName()})");
		}

		switch (result.Status)
		{
			case ComputerProcessStatus.Sleeping:
				sb.AppendLine(
					$"The program suspended until {(result.Process?.WakeTimeUtc?.ToString(actor) ?? "now").ColourValue()}.");
				break;
			case ComputerProcessStatus.Completed:
				sb.AppendLine(
					$"It returned {ProgModule.DescribeProgVariable(actor, executable.ReturnType, result.Result)}.");
				break;
			default:
				if (!string.IsNullOrEmpty(result.ErrorMessage))
				{
					sb.AppendLine(result.ErrorMessage.ColourError());
				}
				break;
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	private static void ProgrammingWorkspaceProcesses(ICharacter actor)
	{
		var processes = actor.Gameworld.ComputerExecutionService.GetProcesses(actor).ToList();
		if (!processes.Any())
		{
			actor.Send("You do not have any computer-program processes.");
			return;
		}

		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			processes.Select(x => new List<string>
			{
				x.Id.ToString("N0", actor),
				x.ProcessName,
				x.Status.DescribeEnum(),
				x.WaitType == ComputerProcessWaitType.Sleep && x.WakeTimeUtc.HasValue
					? x.WakeTimeUtc.Value.ToString(actor)
					: x.WaitType.DescribeEnum(),
				x.LastUpdatedAtUtc.ToString(actor)
			}),
			new List<string>
			{
				"Id",
				"Name",
				"Status",
				"Waiting",
				"Updated"
			},
			actor.LineFormatLength,
			true,
			Telnet.BoldGreen,
			1,
			actor.Account.UseUnicode), nopage: true);
	}

	private static void ProgrammingWorkspaceKill(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished || !long.TryParse(ss.PopSpeech(), out var processId))
		{
			actor.Send("Which process id do you want to kill?");
			return;
		}

		if (!actor.Gameworld.ComputerExecutionService.KillProcess(actor, processId, out var error))
		{
			actor.Send(error);
			return;
		}

		actor.Send($"You kill the computer-program process {processId.ToString("N0", actor).ColourValue()}.");
	}

	private static IComputerExecutableDefinition? GetEditingComputerExecutable(ICharacter actor)
	{
		return actor.CombinedEffectsOfType<ComputerExecutableEditingEffect>().FirstOrDefault()?.EditingItem;
	}

	private static void SetEditingComputerExecutable(ICharacter actor, IComputerExecutableDefinition executable)
	{
		actor.RemoveAllEffects<ComputerExecutableEditingEffect>();
		actor.AddEffect(new ComputerExecutableEditingEffect(actor) { EditingItem = executable });
	}

	private static IComputerExecutableDefinition? ResolveWorkspaceExecutable(ICharacter actor, string identifier)
	{
		var executable = actor.Gameworld.ComputerExecutionService.GetExecutable(actor, identifier);
		if (executable is not null)
		{
			return executable;
		}

		actor.Send("There is no such workspace executable.");
		return null;
	}

	private static void AutoCompileWorkspaceExecutable(ICharacter actor, IComputerExecutableDefinition executable,
		string successPrefix)
	{
		var result = actor.Gameworld.ComputerExecutionService.CompileExecutable(executable);
		if (result.Success)
		{
			actor.Send($"{successPrefix} It compiled successfully.".ColourBold(Telnet.Green));
			return;
		}

		actor.Send($"{successPrefix} It failed to compile: {result.ErrorMessage.ColourError()}");
	}

	private static void ShowWorkspaceExecutable(ICharacter actor, IComputerExecutableDefinition executable)
	{
		var sb = new StringBuilder();
		sb.AppendLine(
			$"{executable.Name.ColourName()} [{executable.Id.ToString("N0", actor).ColourValue()}] - {executable.ExecutableKind.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Return Type: {executable.ReturnType.Describe().ColourValue()}");
		sb.AppendLine($"Status: {executable.CompilationStatus.DescribeEnum().ColourName()}");
		if (!string.IsNullOrWhiteSpace(executable.CompileError))
		{
			sb.AppendLine($"Compile Error: {executable.CompileError.ColourError()}");
		}

		sb.AppendLine("Parameters:");
		if (!executable.Parameters.Any())
		{
			sb.AppendLine("\tNone");
		}
		else
		{
			foreach (var parameter in executable.Parameters)
			{
				sb.AppendLine($"\t{parameter.Name.ColourCommand()} as {parameter.Type.Describe().ColourValue()}");
			}
		}

		sb.AppendLine();
		sb.AppendLine("Source:");
		sb.AppendLine(string.IsNullOrWhiteSpace(executable.SourceCode)
			? "\t<empty>".ColourError()
			: executable.SourceCode);
		actor.OutputHandler.Send(sb.ToString(), nopage: true);
	}

	private static void ProgrammingLogic(ICharacter actor, IGameItem item, StringStack ss)
	{
		if (!CanServiceElectronicsTarget(actor, item, out var serviceError))
		{
			actor.Send(serviceError);
			return;
		}

		var controller = ResolveOptionalController(actor, item, ss, "programmable microcontroller");
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
		if (!CanServiceElectronicsTarget(actor, item, out var serviceError))
		{
			actor.Send(serviceError);
			return;
		}

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
		var controller = ResolveOptionalController(actor, item, ss, "programmable microcontroller");
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

		var source = ResolveNearbySignalSource(actor, item, ss.PopSpeech(), "signal source component");
		if (source is null)
		{
			return;
		}

		if (!CanServiceElectronicsTarget(actor, source.Parent, out var sourceError))
		{
			actor.Send(sourceError);
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
		var controller = ResolveOptionalController(actor, item, ss, "programmable microcontroller");
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
			null,
			null,
			null);
	}

	private static void StartElectricalAction(ICharacter actor, IGameItem item, string durationConfigKey,
		CheckType checkType, string traitConfigKey, string toolTagConfigKey, string actionDescription,
		string beginEmoteKey, string continueEmoteKey, string cancelEmoteKey, string successEmoteKey,
		string failureEmoteKey, Func<CheckOutcome, bool> successAction,
		IEnumerable<IInventoryPlan>? additionalInventoryPlans,
		Func<CheckOutcome, IList<IGameItem>>? successExemptItemsAction,
		Func<IEnumerable<IGameItem>>? shockRiskItemsProvider)
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
			additionalInventoryPlans,
			successExemptItemsAction,
			null,
			outcome =>
			{
				if (shockRiskItemsProvider?.Invoke().Any(IsElectricallyLive) == true)
				{
					ApplyElectricalShock(actor, item);
				}
			});
	}

	private static void StartConfiguredAction(ICharacter actor, IGameItem item, string toolTagConfigKey,
		string traitConfigKey, string durationConfigKey, CheckType checkType, Difficulty difficulty,
		string actionDescription, string beginEmoteKey, string continueEmoteKey, string cancelEmoteKey,
		string successEmoteKey, string failureEmoteKey, Func<CheckOutcome, bool> successAction,
		IEnumerable<IInventoryPlan>? additionalInventoryPlans,
		Func<CheckOutcome, IList<IGameItem>>? successExemptItemsAction,
		Action<CheckOutcome>? failureAction, Action<CheckOutcome>? abjectFailureAction)
	{
		var extraPlans = additionalInventoryPlans?.ToList() ?? [];
		if (TryExecuteConfiguredActionImmediatelyForAdministrator(actor, checkType, extraPlans, successAction,
			    successExemptItemsAction))
		{
			return;
		}

		if (!TryAcquireToolPlan(actor, toolTagConfigKey, out var plan, out var tool))
		{
			FinaliseConfiguredActionInventoryPlans(extraPlans);
			return;
		}

		var trait = ResolveTrait(actor, traitConfigKey);
		if (trait is null)
		{
			plan.FinalisePlan();
			FinaliseConfiguredActionInventoryPlans(extraPlans);
			return;
		}

		var totalDuration = TimeSpan.FromSeconds(Math.Max(3.0, actor.Gameworld.GetStaticDouble(durationConfigKey)));
		var stageDuration = TimeSpan.FromMilliseconds(totalDuration.TotalMilliseconds / 3.0);
		var inventoryPlans = new List<IInventoryPlan> { plan };
		inventoryPlans.AddRange(extraPlans);
		var effect = new ItemComponentConfigurationAction(
			actor,
			item,
			tool,
			inventoryPlans,
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
			successExemptItemsAction,
			failureAction,
			abjectFailureAction);
		actor.AddEffect(effect, stageDuration);
	}

	private static bool TryExecuteConfiguredActionImmediatelyForAdministrator(ICharacter actor, CheckType checkType,
		IEnumerable<IInventoryPlan> inventoryPlans, Func<CheckOutcome, bool> successAction,
		Func<CheckOutcome, IList<IGameItem>>? successExemptItemsAction)
	{
		if (!actor.IsAdministrator())
		{
			return false;
		}

		var outcome = CheckOutcome.SimpleOutcome(checkType, Outcome.Pass);
		IList<IGameItem>? exemptions = null;
		var completedSuccessfully = false;
		try
		{
			completedSuccessfully = successAction(outcome);
			if (completedSuccessfully)
			{
				exemptions = successExemptItemsAction?.Invoke(outcome);
			}
		}
		finally
		{
			FinaliseConfiguredActionInventoryPlans(inventoryPlans, completedSuccessfully ? exemptions : null);
		}

		return true;
	}

	private static void FinaliseConfiguredActionInventoryPlans(IEnumerable<IInventoryPlan> inventoryPlans,
		IList<IGameItem>? exemptions = null)
	{
		foreach (var inventoryPlan in inventoryPlans.Distinct())
		{
			if (exemptions?.Any() == true)
			{
				inventoryPlan.FinalisePlanWithExemptions(exemptions);
				continue;
			}

			inventoryPlan.FinalisePlan();
		}
	}

	private static void ShowElectricalStatus(ICharacter actor, IGameItem item)
	{
		var statusItems = EnumerateElectricalStatusItems(actor, item).ToList();
		var sources = statusItems.SelectMany(x => x.Components.OfType<ISignalSourceComponent>()).ToList();
		var sinks = statusItems.SelectMany(x => x.Components.OfType<IRuntimeConfigurableSignalSinkComponent>()).ToList();
		var hosts = statusItems.SelectMany(x => x.Components.OfType<IAutomationMountHost>()).ToList();
		var housings = statusItems.SelectMany(x => x.Components.OfType<IAutomationHousing>()).Distinct().ToList();
		if (!sources.Any() && !sinks.Any() && !hosts.Any() && !housings.Any())
		{
			actor.Send($"{item.HowSeen(actor, true)} has no signal-capable electrical systems.");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine($"{item.HowSeen(actor, true)} has the following electrical components:");
		if (hosts.Any())
		{
			sb.AppendLine();
			sb.AppendLine("Automation Bays:");
			foreach (var host in hosts.OrderBy(x => x.Parent.Id).ThenBy(x => x.Id))
			{
				sb.AppendLine($"\t{DescribeComponent(actor, host).ColourName()}:");
				foreach (var bay in host.Bays.OrderBy(x => x.Name))
				{
					sb.AppendLine(
						$"\t\t{bay.Name.ColourCommand()} ({bay.MountType.ColourValue()}) - {(bay.Occupied ? bay.MountedItem!.HowSeen(actor, true).ColourName() : "empty".ColourError())}");
				}
			}
		}

		if (housings.Any())
		{
			sb.AppendLine();
			sb.AppendLine("Automation Housings:");
			foreach (var housing in housings.OrderBy(x => x.Parent.Id).ThenBy(x => x.Id))
			{
				var accessText = housing.CanAccessHousing(actor, out _)
					? "open for service".ColourValue()
					: "sealed".ColourName();
				sb.AppendLine($"\t{DescribeComponent(actor, housing).ColourName()} - {accessText}");
				if (!housing.CanAccessHousing(actor, out _))
				{
					continue;
				}

				var concealed = housing.ConcealedItems.ToList();
				if (!concealed.Any())
				{
					sb.AppendLine("\t\tNo concealed automation items.");
					continue;
				}

				foreach (var concealedItem in concealed.OrderBy(x => x.Id))
				{
					sb.AppendLine($"\t\t{concealedItem.HowSeen(actor, true).ColourName()}");
				}
			}
		}

		if (sources.Any())
		{
			sb.AppendLine();
			sb.AppendLine("Sources:");
			foreach (var source in sources.OrderBy(x => x.Parent.Id).ThenBy(x => x.Id))
			{
				sb.AppendLine(
					$"\t{DescribeComponent(actor, source).ColourName()} -> {source.CurrentValue.ToString("N2", actor).ColourValue()} on {source.EndpointKey.ColourCommand()}");
			}
		}

		if (sinks.Any())
		{
			sb.AppendLine();
			sb.AppendLine("Configurable Sinks:");
			foreach (var sink in sinks.OrderBy(x => x.Parent.Id).ThenBy(x => x.Id))
			{
				sb.AppendLine(
					$"\t{DescribeComponent(actor, sink).ColourName()} <- {SignalComponentUtilities.DescribeSignalComponent(sink.CurrentBinding).ColourCommand()}, threshold {sink.ActivationThreshold.ToString("N2", actor).ColourValue()}, mode {(sink.ActiveWhenAboveThreshold ? "above/equal".ColourValue() : "below".ColourValue())}");
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

	private static TComponent? ResolveComponentOnItem<TComponent>(ICharacter actor, IGameItem item, string identifier,
		string componentTypeDescription)
		where TComponent : class, IGameItemComponent
	{
		var components = item.Components.OfType<TComponent>().ToList();
		if (!components.Any())
		{
			actor.Send($"{item.HowSeen(actor, true)} has no {componentTypeDescription}s.");
			return null;
		}

		var resolved = TryResolveComponentOnItem<TComponent>(item, identifier);
		if (resolved is not null)
		{
			return resolved;
		}

		actor.Send(
			$"You must specify one of the following {componentTypeDescription}s on {item.HowSeen(actor, true)}:\n{components.Select(x => $"\t[{x.Id.ToString("N0", actor)}] {x.Name.ColourName()}").ListToLines()}");
		return null;
	}

	private static ISignalSourceComponent? ResolveNearbySignalSource(ICharacter actor, IGameItem anchorItem, string identifier,
		string componentTypeDescription)
	{
		return ResolveComponentFromItems<ISignalSourceComponent>(actor,
			EnumerateNearbySignalItems(actor, ResolveSignalSearchAnchorItem(anchorItem)),
			identifier,
			componentTypeDescription);
	}

	private static ISignalSourceComponent? ResolveActorLocalSignalSource(ICharacter actor, string identifier)
	{
		return ResolveComponentFromItems<ISignalSourceComponent>(actor, EnumerateActorLocalSignalItems(actor), identifier,
			"signal source component");
	}

	private static TComponent? ResolveComponentFromItems<TComponent>(ICharacter actor, IEnumerable<IGameItem> items,
		string identifier, string componentTypeDescription)
		where TComponent : class, IGameItemComponent
	{
		var candidateItems = items.Distinct().ToList();
		var components = candidateItems.SelectMany(x => x.Components.OfType<TComponent>()).Distinct().ToList();
		if (!components.Any())
		{
			actor.Send($"There are no accessible {componentTypeDescription}s here.");
			return null;
		}

		if (identifier.Contains('@'))
		{
			var split = identifier.Split('@', 2, StringSplitOptions.RemoveEmptyEntries);
			if (split.Length == 2)
			{
				var itemMatch = ResolveItemReference(actor, candidateItems, split[0]);
				if (itemMatch is not null)
				{
					var directMatch = TryResolveComponentOnItem<TComponent>(itemMatch, split[1]);
					if (directMatch is not null)
					{
						return directMatch;
					}

					var attachedMatch = ResolveItemReference(actor, itemMatch.AttachedAndConnectedItems.Distinct(), split[1]);
					if (attachedMatch is not null)
					{
						var attachedComponents = attachedMatch.Components.OfType<TComponent>().ToList();
						if (attachedComponents.Count == 1)
						{
							return attachedComponents[0];
						}
					}
				}
			}
		}

		var itemMatchByKeyword = ResolveItemReference(actor, candidateItems, identifier);
		if (itemMatchByKeyword is not null)
		{
			var itemComponents = itemMatchByKeyword.Components.OfType<TComponent>().ToList();
			if (itemComponents.Count == 1)
			{
				return itemComponents[0];
			}
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
			$"You must specify one of the following {componentTypeDescription}s:\n{components.OrderBy(x => x.Parent.Id).ThenBy(x => x.Id).Select(x => $"\t{DescribeComponent(actor, x).ColourName()}").ListToLines()}");
		return null;
	}

	private static TComponent? TryResolveComponentOnItem<TComponent>(IGameItem item, string identifier)
		where TComponent : class, IGameItemComponent
	{
		var components = item.Components.OfType<TComponent>().ToList();
		if (!components.Any())
		{
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
		return prefixMatches.Count == 1 ? prefixMatches[0] : null;
	}

	private static bool TryResolveSingleComponentOnItem<TComponent>(IGameItem item, out TComponent component)
		where TComponent : class, IGameItemComponent
	{
		var components = item.Components.OfType<TComponent>().ToList();
		component = components.Count == 1 ? components[0] : null!;
		return components.Count == 1;
	}

	private static IRuntimeProgrammableMicrocontroller? ResolveOptionalController(ICharacter actor, IGameItem item,
		StringStack ss, string componentTypeDescription)
	{
		var controllers = item.Components.OfType<IRuntimeProgrammableMicrocontroller>().ToList();
		if (!controllers.Any())
		{
			actor.Send($"{item.HowSeen(actor, true)} has no {componentTypeDescription}s.");
			return null;
		}

		if (controllers.Count == 1)
		{
			if (!ss.IsFinished)
			{
				var explicitController = TryResolveComponentOnItem<IRuntimeProgrammableMicrocontroller>(item, ss.PeekSpeech());
				if (explicitController is not null)
				{
					ss.PopSpeech();
					return explicitController;
				}
			}

			return controllers[0];
		}

		if (ss.IsFinished)
		{
			actor.Send($"Which {componentTypeDescription} do you mean?");
			return null;
		}

		return ResolveComponentOnItem<IRuntimeProgrammableMicrocontroller>(actor, item, ss.PopSpeech(),
			componentTypeDescription);
	}

	private static IAutomationMountHost? ResolveAutomationMountHost(ICharacter actor, IGameItem item)
	{
		var host = item.GetItemType<IAutomationMountHost>();
		if (host is null)
		{
			actor.Send($"{item.HowSeen(actor, true)} is not configured with any automation mount bays.");
		}

		return host;
	}

	private static ISignalCableSegment? ResolveSignalCableSegment(ICharacter actor, IGameItem item)
	{
		var cable = item.GetItemType<ISignalCableSegment>();
		if (cable is null)
		{
			actor.Send($"{item.HowSeen(actor, true)} is not a signal cable segment.");
		}

		return cable;
	}

	private static IAutomationHousing? ResolveAutomationHousing(ICharacter actor, IGameItem item)
	{
		var housing = item.GetItemType<IAutomationHousing>();
		if (housing is null)
		{
			actor.Send($"{item.HowSeen(actor, true)} is not an automation housing or junction.");
		}

		return housing;
	}

	private static string? ResolveDefaultCompatibleMountBay(ICharacter actor, IAutomationMountHost host,
		IAutomationMountable module, IGameItem hostItem)
	{
		var compatible = host.Bays
			.Where(x => !x.Occupied && x.MountType.Equals(module.MountType, StringComparison.InvariantCultureIgnoreCase))
			.ToList();
		if (compatible.Count == 1)
		{
			return compatible[0].Name;
		}

		if (!compatible.Any())
		{
			actor.Send($"{hostItem.HowSeen(actor, true)} has no compatible free bays for a {module.MountType.ColourCommand()} module.");
			return null;
		}

		actor.Send(
			$"You must specify which compatible bay you want to use on {hostItem.HowSeen(actor, true)}:\n{compatible.Select(x => $"\t{x.Name.ColourCommand()}").ListToLines()}");
		return null;
	}

	private static string? ResolveDefaultOccupiedMountBay(ICharacter actor, IAutomationMountHost host, IGameItem hostItem)
	{
		var occupied = host.Bays.Where(x => x.Occupied).ToList();
		if (occupied.Count == 1)
		{
			return occupied[0].Name;
		}

		if (!occupied.Any())
		{
			actor.Send($"{hostItem.HowSeen(actor, true)} does not currently have any installed modules.");
			return null;
		}

		actor.Send(
			$"You must specify which occupied bay you want to clear on {hostItem.HowSeen(actor, true)}:\n{occupied.Select(x => $"\t{x.Name.ColourCommand()}").ListToLines()}");
		return null;
	}

	private static IEnumerable<IGameItem> EnumerateElectricalStatusItems(ICharacter actor, IGameItem item)
	{
		var rootItems = new[] { item }
			.Concat(item.AttachedAndConnectedItems)
			.Distinct()
			.ToList();
		return rootItems
			.Concat(EnumerateAccessibleAutomationHousingContents(actor, rootItems))
			.Distinct();
	}

	private static IEnumerable<IGameItem> EnumerateNearbySignalItems(ICharacter actor, IGameItem anchorItem)
	{
		var items = new List<IGameItem> { anchorItem };
		items.AddRange(anchorItem.AttachedAndConnectedItems);
		foreach (var cell in anchorItem.TrueLocations.OfType<ICell>().Distinct())
		{
			items.AddRange(cell.LayerGameItems(anchorItem.RoomLayer));
		}

		var rootItems = items
			.Distinct()
			.SelectMany(x => new[] { x }.Concat(x.AttachedAndConnectedItems))
			.Distinct()
			.ToList();
		return rootItems
			.Concat(EnumerateAccessibleAutomationHousingContents(actor, rootItems))
			.Distinct();
	}

	private static IEnumerable<IGameItem> EnumerateActorLocalSignalItems(ICharacter actor)
	{
		var items = actor.Body.ExternalItems
			.Concat(actor.Location.LayerGameItems(actor.RoomLayer))
			.Distinct()
			.ToList();
		var rootItems = items
			.SelectMany(x => new[] { x }.Concat(x.AttachedAndConnectedItems))
			.Distinct()
			.ToList();
		return rootItems
			.Concat(EnumerateAccessibleAutomationHousingContents(actor, rootItems))
			.Distinct();
	}

	private static IGameItem ResolveSignalSearchAnchorItem(IGameItem item)
	{
		return item.GetItemType<IAutomationMountable>() is IAutomationMountable { MountHost: not null } mountable
			? mountable.MountHost.Parent
			: item;
	}

	private static IEnumerable<IGameItem> EnumerateAccessibleAutomationHousingContents(ICharacter actor,
		IEnumerable<IGameItem> items)
	{
		return items
			.Distinct()
			.SelectMany(x =>
				x.GetItemType<IAutomationHousing>() is IAutomationHousing housing &&
				housing.CanAccessHousing(actor, out _)
					? housing.ConcealedItems
					: []);
	}

	private static IGameItem? ResolveItemReference(ICharacter actor, IEnumerable<IGameItem> items, string identifier)
	{
		var candidateItems = items.Distinct().ToList();
		var directTarget = actor.TargetItem(identifier);
		if (directTarget is not null && candidateItems.Any(x => ReferenceEquals(x, directTarget)))
		{
			return directTarget;
		}

		if (long.TryParse(identifier, out var itemId))
		{
			var idMatch = candidateItems.FirstOrDefault(x => x.Id == itemId);
			if (idMatch is not null)
			{
				return idMatch;
			}
		}

		return candidateItems.GetFromItemListByKeyword(identifier, actor) ??
		       candidateItems.GetFromItemListByKeywordIncludingNames(identifier, actor);
	}

	private static string DescribeAvailableElectricalTargets(ICharacter actor, IGameItem item)
	{
		var items = new[] { item }
			.Concat(item.GetItemType<IAutomationHousing>() is IAutomationHousing housing &&
			        housing.CanAccessHousing(actor, out _)
				? housing.ConcealedItems
				: [])
			.Distinct()
			.ToList();
		var components = items
			.SelectMany(x => x.Components)
			.OfType<IGameItemComponent>()
			.Where(x => x is IRuntimeConfigurableSignalSinkComponent || x is ISignalCableSegment)
			.ToList();
		return !components.Any()
			? "\tNone"
			: components.Select(x => $"\t{DescribeComponent(actor, x).ColourName()}").ListToLines();
	}

	private static (bool Truth, string Message) CanManipulateElectronicsTarget(ICharacter actor, IGameItem item)
	{
		if (item.GetItemType<IAutomationMountable>() is IAutomationMountable { MountHost: not null } mountable)
		{
			var hostManipulation = actor.CanManipulateItem(mountable.MountHost.Parent);
			if (!hostManipulation.Truth)
			{
				return hostManipulation;
			}

			if (!mountable.MountHost.CanAccessMounts(actor, out var mountError))
			{
				return (false, mountError);
			}

			return CheckContainingAutomationHousing(actor, mountable.MountHost.Parent);
		}

		var manipulation = actor.CanManipulateItem(item);
		if (!manipulation.Truth)
		{
			return manipulation;
		}

		return CheckContainingAutomationHousing(actor, item);
	}

	private static bool CanServiceElectronicsTarget(ICharacter actor, IGameItem item, out string error)
	{
		var manipulation = CanManipulateElectronicsTarget(actor, item);
		if (!manipulation.Truth)
		{
			error = manipulation.Message;
			return false;
		}

		if (item.GetItemType<IAutomationMountHost>() is IAutomationMountHost host &&
		    !host.CanAccessMounts(actor, out error))
		{
			return false;
		}

		if (item.GetItemType<IAutomationHousing>() is IAutomationHousing housing &&
		    !housing.CanAccessHousing(actor, out error))
		{
			return false;
		}

		error = string.Empty;
		return true;
	}

	private static (bool Truth, string Message) CheckContainingAutomationHousing(ICharacter actor, IGameItem item)
	{
		if (item.ContainedIn?.GetItemType<IAutomationHousing>() is not IAutomationHousing housing)
		{
			return (true, string.Empty);
		}

		var housingManipulation = actor.CanManipulateItem(housing.Parent);
		if (!housingManipulation.Truth)
		{
			return housingManipulation;
		}

		return housing.CanAccessHousing(actor, out var housingError)
			? (true, string.Empty)
			: (false, housingError);
	}

	private static bool TryAcquireSpecificItemPlan(ICharacter actor, IGameItem item, out IInventoryPlan plan)
	{
		var template = new InventoryPlanTemplate(actor.Gameworld, new[]
		{
			new InventoryPlanActionHold(actor.Gameworld, 0, 0, x => ReferenceEquals(x, item), null, 1)
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
				actor.Send($"You cannot get hold of {item.HowSeen(actor, true)} to work with it.");
				return false;
			case InventoryPlanFeasibility.NotFeasibleNotEnoughHands:
				actor.Send("You do not have enough free hands to ready that item for the work.");
				return false;
			case InventoryPlanFeasibility.NotFeasibleNotEnoughWielders:
				actor.Send("You cannot get that item into the right state for the work.");
				return false;
			default:
				actor.Send($"You cannot get hold of {item.HowSeen(actor, true)} right now.");
				return false;
		}

		var results = plan.ExecuteWholePlan().ToList();
		if (!results.Any(x => ReferenceEquals(x.PrimaryTarget, item)))
		{
			plan.FinalisePlan();
			actor.Send($"You fail to get hold of {item.HowSeen(actor, true)}.");
			return false;
		}

		return true;
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

	private static bool RelocateLooseItem(IGameItem item, ICell destinationCell, IContainer? destinationContainer, out string error)
	{
		ReleaseItemFromCurrentState(item);
		if (destinationContainer is not null)
		{
			if (!destinationContainer.CanPut(item))
			{
				error = "The destination housing cannot accept that item.";
				return false;
			}

			destinationContainer.Put(null, item, false);
			error = string.Empty;
			return true;
		}

		item.Drop(destinationCell);
		error = string.Empty;
		return true;
	}

	private static void ReleaseItemFromCurrentState(IGameItem item)
	{
		if (item.GetItemType<IHoldable>()?.HeldBy != null)
		{
			item.GetItemType<IHoldable>()!.HeldBy.Take(item);
			return;
		}

		if (item.ContainedIn != null)
		{
			item.ContainedIn.Take(item);
			return;
		}

		item.Location?.Extract(item);
	}

	private static IEnumerable<IGameItem> EnumerateShockRiskItems(params IGameItem?[] items)
	{
		return items.Where(x => x is not null).Cast<IGameItem>().Distinct();
	}

	private static bool IsElectricallyLive(IGameItem item)
	{
		if (item.GetItemTypes<IProducePower>().Any(x => x.ProducingPower))
		{
			return true;
		}

		foreach (var consumer in item.GetItemTypes<IConsumePower>())
		{
			if (consumer is IOnOff onOff)
			{
				if (onOff.SwitchedOn)
				{
					return true;
				}

				continue;
			}

			return true;
		}

		return false;
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
		return $"{component.Parent.HowSeen(actor, true)}@{component.Name}";
	}

	private static double ToolQualityBonus(IGameItem tool)
	{
		return StandardCheck.BonusesPerDifficultyLevel * ((int)tool.Quality - 5);
	}
}
