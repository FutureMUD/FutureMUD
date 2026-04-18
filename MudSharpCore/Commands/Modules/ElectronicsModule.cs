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
	#3programming terminal connect <terminal>#0 - connects you to a powered computer terminal
	#3programming terminal disconnect#0 - disconnects you from your current computer terminal session
	#3programming terminal status#0 - shows your current computer terminal session and selected owner
	#3programming terminal owner host#0 - selects the connected host as the current programming owner
	#3programming terminal owner <storage>#0 - selects one mounted storage device as the current programming owner
	#3type <text>#0 - types into your current terminal session, or a nearby terminal if one can be resolved automatically
	#3type <terminal> <text>#0 - types into a specific nearby terminal
	#3programming apps#0 - lists the built-in computer applications available on your connected host
	#3programming app <name>#0 - runs one built-in computer application on your connected host
	#3programming network#0 - shows shared network identity and VPN gateway status for the connected host (administrators only)
	#3programming network domain add|remove|enable|disable <domain>#0 - manages shared hosted network domains on the connected host (administrators only)
	#3programming network account add <user@domain> <password>#0 - creates a shared network account on the connected host (administrators only)
	#3programming network account enable|disable <user@domain>#0 - enables or disables a shared network account (administrators only)
	#3programming network account password <user@domain> <password>#0 - changes a shared network account password (administrators only)
	#3programming network vpn add|remove <network>#0 - changes which VPN networks this host exposes for authenticated tunnels (administrators only)
	#3programming mail#0 - shows mail service status for the connected host (administrators only)
	#3programming mail service on|off#0 - enables or disables the connected host's mail service advertisement (administrators only)
	#3programming mail domain add|remove|enable|disable <domain>#0 - manages hosted mail domains on the connected host (administrators only)
	#3programming mail account add <user@domain> <password>#0 - creates a mail account on the connected host (administrators only)
	#3programming mail account enable|disable <user@domain>#0 - enables or disables a mail account (administrators only)
	#3programming mail account password <user@domain> <password>#0 - changes a mail account password (administrators only)
	#3programming ftp#0 - shows FTP/file-transfer service status for the connected host (administrators only)
	#3programming ftp service on|off#0 - enables or disables the connected host's FTP service advertisement (administrators only)
	#3programming ftp account add <user> <password>#0 - creates an FTP account on the connected host (administrators only)
	#3programming ftp account enable|disable <user>#0 - enables or disables an FTP account (administrators only)
	#3programming ftp account password <user> <password>#0 - changes an FTP account password (administrators only)
	#3programming ftp file list#0 - lists published public FTP files on the current programming owner (administrators only)
	#3programming ftp file publish|unpublish <file>#0 - changes public FTP visibility for a file on the current programming owner (administrators only)
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
	#3programming kill <process>#0 - kills one of your running or sleeping computer-program processes on the current programming owner
	#3programming help <topic>#0 - shows programming help filtered to the computer-safe language subset
	#3programming item <item>#0 - shows all programmable microcontrollers on the item
	#3programming item <item> logic [<component>]#0 - opens an editor to replace the controller logic
	#3programming item <item> logic [<component>] <text>#0 - directly replaces the controller logic
	#3programming item <item> input add [<component>] <variable> <source> [<endpoint>]#0 - binds an input variable to a local signal source
	#3programming item <item> input remove [<component>] <variable>#0 - removes an input variable
	#3programming item <item> file [<component>]#0 - shows the backing file status for a file-driven signal generator
	#3programming item <item> file edit [<component>]#0 - opens the backing signal file in the normal multiline editor
	#3programming item <item> file write [<component>] <text>#0 - directly replaces the backing signal file contents
	#3programming item <item> file public [<component>] on|off#0 - changes whether the backing file is publicly accessible over FTP

Workspace-style authoring commands operate on your current programming owner, which is either your private workspace or the owner selected on your connected computer terminal session.
The old short form of #3programming <item>#0 still works for item-targeted microcontroller programming whenever the first word is not one of the reserved workspace verbs.
Mounted microcontrollers remain separate items, so you can target them with syntax like #6host@module#0 when they are installed behind an open access panel.";

	private const string TypeHelpText = @"The #3type#0 command is used to type text into a computer terminal.

You can use the following syntax:
	#3type <text>#0 - types into your current terminal session, or a nearby terminal if one can be resolved automatically
	#3type <terminal> <text>#0 - types into a specific nearby terminal

If you are already connected to a computer terminal, #3type#0 uses that session by default.
If you are not connected, it prefers the only nearby terminal, or a terminal on or attached to your current position target.
If more than one terminal could be used, specify one explicitly or connect first with #3programming terminal connect <terminal>#0.";

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
		"app",
		"apps",
		"network",
		"mail",
		"ftp",
		"processes",
		"kill",
		"terminal"
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

	[PlayerCommand("Type", "type")]
	[RequiredCharacterState(CharacterState.Able)]
	[NoHideCommand]
	[HelpInfo("type", TypeHelpText, AutoHelp.HelpArgOrNoArg)]
	protected static void Type(ICharacter actor, string command)
	{
		if (!actor.Gameworld.GetStaticBool("ProgrammingCommandEnabled"))
		{
			actor.Send("Computer terminal interaction is not available in this game.");
			return;
		}

		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(TypeHelpText.SubstituteANSIColour());
			return;
		}

		if (!TryResolveTerminalForTyping(actor, ss.SafeRemainingArgument.Trim(), out var terminalItem, out var terminal,
			    out var text, out var error))
		{
			actor.Send(error);
			return;
		}

		if (string.IsNullOrWhiteSpace(text))
		{
			actor.Send($"What do you want to type into {terminalItem.HowSeen(actor, true)}?");
			return;
		}

		var manipulation = CanManipulateElectronicsTarget(actor, terminalItem);
		if (!manipulation.Truth)
		{
			actor.OutputHandler.Send(manipulation.Message);
			return;
		}

		if (!TryEnsureProgrammingTerminalSession(actor, terminal, out _, out var connectedTerminal, out error))
		{
			actor.Send(error);
			return;
		}

		if (connectedTerminal)
		{
			actor.Send($"You connect to {terminalItem.HowSeen(actor, true).ColourName()}.");
		}

		if (!terminal.TryType(actor, text, out error))
		{
			actor.Send(error);
		}
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
			case "app":
				ProgrammingWorkspaceApplication(actor, ss);
				return;
			case "apps":
				ProgrammingWorkspaceApplications(actor);
				return;
			case "network":
				ProgrammingNetwork(actor, ss);
				return;
			case "mail":
				ProgrammingMail(actor, ss);
				return;
			case "ftp":
				ProgrammingFtp(actor, ss);
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
			case "terminal":
				ProgrammingTerminal(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(ProgrammingHelpText.SubstituteANSIColour());
				return;
		}
	}

	private static void ProgrammingTerminal(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			ShowProgrammingTerminalStatus(actor);
			return;
		}

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "connect":
			case "use":
				ProgrammingTerminalConnect(actor, ss);
				return;
			case "disconnect":
			case "close":
			case "exit":
				ProgrammingTerminalDisconnect(actor);
				return;
			case "status":
			case "show":
				ShowProgrammingTerminalStatus(actor);
				return;
			case "owner":
			case "select":
				ProgrammingTerminalOwner(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(ProgrammingHelpText.SubstituteANSIColour());
				return;
		}
	}

	private static void ProgrammingTerminalConnect(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Which computer terminal do you want to connect to?");
			return;
		}

		var terminalItem = actor.TargetItem(ss.SafeRemainingArgument);
		if (terminalItem is null)
		{
			actor.Send("You do not see any such computer terminal here.");
			return;
		}

		var manipulation = CanManipulateElectronicsTarget(actor, terminalItem);
		if (!manipulation.Truth)
		{
			actor.OutputHandler.Send(manipulation.Message);
			return;
		}

		if (terminalItem.Components.OfType<IComputerTerminal>().FirstOrDefault() is not ComputerTerminalGameItemComponent terminal)
		{
			actor.Send($"{terminalItem.HowSeen(actor, true)} is not a usable computer terminal.");
			return;
		}

		if (!TryEnsureProgrammingTerminalSession(actor, terminal, out var session, out _, out var error))
		{
			actor.Send(error);
			return;
		}

		actor.Send(
			$"You connect to {terminalItem.HowSeen(actor, true).ColourName()}, which is attached to {session!.Host.Name.ColourName()}. Programming commands will now use {DescribeComputerOwner(actor, session.CurrentOwner)}.");
	}

	private static void ProgrammingTerminalDisconnect(ICharacter actor)
	{
		var effect = GetCurrentProgrammingTerminalSessionEffect(actor);
		if (effect is null)
		{
			actor.Send("You are not currently connected to any computer terminal.");
			return;
		}

		var terminalDescription = DescribeTerminal(actor, effect.Session.Terminal);
		actor.RemoveEffect(effect);
		actor.Send(
			$"You disconnect from {terminalDescription}. Programming commands will now use {DescribeComputerOwner(actor, actor.Gameworld.ComputerExecutionService.GetWorkspace(actor))}.");
	}

	private static void ProgrammingTerminalOwner(ICharacter actor, StringStack ss)
	{
		var session = GetCurrentProgrammingTerminalSession(actor);
		if (session is null)
		{
			actor.Send("You are not currently connected to any computer terminal.");
			return;
		}

		if (session.Terminal is not ComputerTerminalGameItemComponent terminal)
		{
			actor.Send("That terminal session is not attached to a configurable computer terminal.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("Do you want to select the host or one of its mounted storage devices?");
			return;
		}

		IComputerExecutableOwner owner;
		var identifier = ss.SafeRemainingArgument.Trim();
		if (identifier.EqualTo("host"))
		{
			owner = session.Host;
		}
		else
		{
			var storage = ResolveTerminalStorageOwner(actor, session.Host, identifier);
			if (storage is null)
			{
				return;
			}

			owner = storage;
		}

		if (!terminal.TrySelectOwner(actor, owner, out var error))
		{
			actor.Send(error);
			return;
		}

		actor.Send(
			$"You select {DescribeComputerOwner(actor, owner)} as the current programming owner on {DescribeTerminal(actor, terminal)}.");
	}

	private static void ShowProgrammingTerminalStatus(ICharacter actor)
	{
		var session = GetCurrentProgrammingTerminalSession(actor);
		if (session is null)
		{
			actor.Send(
				$"You are not currently connected to any computer terminal. Programming commands are using {DescribeComputerOwner(actor, actor.Gameworld.ComputerExecutionService.GetWorkspace(actor))}.");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Computer Terminal: {DescribeTerminal(actor, session.Terminal)}");
		sb.AppendLine($"Connected Host: {session.Host.Name.ColourName()}");
		sb.AppendLine($"Current Owner: {DescribeComputerOwner(actor, session.CurrentOwner)}");
		sb.AppendLine($"Connected Since: {session.ConnectedAtUtc.ToString(actor).ColourValue()}");
		sb.AppendLine(
			$"Active Tunnels: {(session.ActiveRouteKeys.Any() ? ComputerNetworkRoutingUtilities.DescribeRoutes(session.ActiveRouteKeys).ColourValue() : "None".ColourError())}");
		var storage = session.Host.MountedStorage.ToList();
		sb.AppendLine("Mounted Storage:");
		if (!storage.Any())
		{
			sb.AppendLine("\tNone");
		}
		else
		{
			foreach (var item in storage.OrderBy(x => x.Name).ThenBy(x => x.OwnerStorageItemId))
			{
				sb.AppendLine(
					$"\t{DescribeComputerOwner(actor, item)}{(ReferenceEquals(item, session.CurrentOwner) ? " (selected)".ColourValue() : string.Empty)}");
			}
		}

		actor.OutputHandler.Send(sb.ToString());
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
			case "file":
			case "files":
				ProgrammingFile(actor, item, ss);
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
		var owner = GetCurrentProgrammingOwner(actor);
		var executables = actor.Gameworld.ComputerExecutionService.GetExecutables(owner).ToList();
		if (!executables.Any())
		{
			actor.Send($"There are no computer executables on {DescribeComputerOwner(actor, owner)}.");
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

		var owner = GetCurrentProgrammingOwner(actor);
		var name = ss.IsFinished ? $"Unnamed{kind.Value.DescribeEnum()}" : ss.SafeRemainingArgument.Trim();
		if (actor.Gameworld.ComputerExecutionService.GetExecutables(owner)
			    .Any(x => x.Name.EqualTo(name)))
		{
			actor.Send($"There is already an executable with that name on {DescribeComputerOwner(actor, owner)}.");
			return;
		}

		var executable = actor.Gameworld.ComputerExecutionService.CreateExecutable(owner, kind.Value, name);
		SetEditingComputerExecutable(actor, executable);
		actor.Send(
			$"You create the {kind.Value.DescribeEnum().ToLowerInvariant().ColourValue()} {executable.Name.ColourName()} [{executable.Id.ToString("N0", actor).ColourValue()}] on {DescribeComputerOwner(actor, owner)}, which you are now editing.");
	}

	private static void ProgrammingWorkspaceEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var editing = GetEditingComputerExecutable(actor);
			if (editing is null)
			{
				actor.Send($"Which executable on {DescribeComputerOwner(actor, GetCurrentProgrammingOwner(actor))} do you want to edit?");
				return;
			}

			ShowWorkspaceExecutable(actor, editing);
			return;
		}

		var executable = ResolveProgrammingExecutable(actor, ss.SafeRemainingArgument);
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
			actor.Send("You are not currently editing any computer executable.");
			return;
		}

		actor.RemoveAllEffects<ComputerExecutableEditingEffect>();
		actor.Send($"You are no longer editing {editing.Name.ColourName()}.");
	}

	private static void ProgrammingWorkspaceShow(ICharacter actor, StringStack ss)
	{
		var executable = ss.IsFinished ? GetEditingComputerExecutable(actor) : ResolveProgrammingExecutable(actor, ss.SafeRemainingArgument);
		if (executable is null)
		{
			actor.Send(ss.IsFinished
				? "You are not currently editing any computer executable."
				: $"There is no such executable on {DescribeComputerOwner(actor, GetCurrentProgrammingOwner(actor))}.");
			return;
		}

		ShowWorkspaceExecutable(actor, executable);
	}

	private static void ProgrammingWorkspaceDelete(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send($"Which executable on {DescribeComputerOwner(actor, GetCurrentProgrammingOwner(actor))} do you want to delete?");
			return;
		}

		var owner = GetCurrentProgrammingOwner(actor);
		var executable = ResolveProgrammingExecutable(actor, ss.SafeRemainingArgument, owner);
		if (executable is null)
		{
			return;
		}

		if (!actor.Gameworld.ComputerExecutionService.DeleteExecutable(owner, executable, out var error))
		{
			actor.Send(error);
			return;
		}

		if (GetEditingComputerExecutable(actor)?.Id == executable.Id)
		{
			actor.RemoveAllEffects<ComputerExecutableEditingEffect>();
		}

		actor.Send($"You delete {executable.Name.ColourName()} from {DescribeComputerOwner(actor, owner)}.");
	}

	private static void ProgrammingWorkspaceSet(ICharacter actor, StringStack ss)
	{
		var executable = GetEditingComputerExecutable(actor);
		if (executable is null)
		{
			actor.Send("You are not currently editing any computer executable.");
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
			actor.Send("What new name do you want to give to that executable?");
			return;
		}

		if (executable is not ComputerRuntimeExecutableBase runtimeExecutable)
		{
			actor.Send("That executable cannot be edited.");
			return;
		}

		var owner = ResolveExecutableOwner(actor, executable) ?? GetCurrentProgrammingOwner(actor);
		var name = ss.SafeRemainingArgument.Trim();
		if (actor.Gameworld.ComputerExecutionService.GetExecutables(owner)
			    .Any(x => x.Id != executable.Id && x.Name.EqualTo(name)))
		{
			actor.Send($"There is already another executable with that name on {DescribeComputerOwner(actor, owner)}.");
			return;
		}

		runtimeExecutable.Name = name;
		actor.Gameworld.ComputerExecutionService.SaveExecutable(executable);
		AutoCompileExecutable(actor, executable,
			$"You rename the executable to {name.ColourName()}.");
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

		if (executable is not ComputerRuntimeExecutableBase runtimeExecutable)
		{
			actor.Send("That executable cannot be edited.");
			return;
		}

		runtimeExecutable.ReturnType = type;
		actor.Gameworld.ComputerExecutionService.SaveExecutable(executable);
		AutoCompileExecutable(actor, executable,
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
					if (executable is not ComputerRuntimeExecutableBase runtimeExecutable)
					{
						handler.Send("That executable can no longer be edited.");
						return;
					}

					runtimeExecutable.SourceCode = text;
					actor.Gameworld.ComputerExecutionService.SaveExecutable(executable);
					AutoCompileExecutable(actor, executable,
						$"You replace the source code for {executable.Name.ColourName()}.");
				},
				(handler, _) => handler.Send("You decide not to change the source code."),
				1.0,
				recallText: executable.SourceCode,
				options: EditorOptions.PermitEmpty);
			return;
		}

		if (executable is not ComputerRuntimeExecutableBase runtime)
		{
			actor.Send("That executable cannot be edited.");
			return;
		}

		runtime.SourceCode = ss.SafeRemainingArgument;
		actor.Gameworld.ComputerExecutionService.SaveExecutable(executable);
		AutoCompileExecutable(actor, executable,
			$"You replace the source code for {executable.Name.ColourName()}.");
	}

	private static void ProgrammingWorkspaceParameter(ICharacter actor, StringStack ss)
	{
		var executable = GetEditingComputerExecutable(actor);
		if (executable is null)
		{
			actor.Send("You are not currently editing any computer executable.");
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
		if (executable is not ComputerRuntimeExecutableBase runtimeExecutable)
		{
			actor.Send("That executable cannot be edited.");
			return;
		}

		runtimeExecutable.Parameters = parameters;
		actor.Gameworld.ComputerExecutionService.SaveExecutable(executable);
		AutoCompileExecutable(actor, executable,
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
		if (executable is not ComputerRuntimeExecutableBase runtimeExecutable)
		{
			actor.Send("That executable cannot be edited.");
			return;
		}

		runtimeExecutable.Parameters = parameters;
		actor.Gameworld.ComputerExecutionService.SaveExecutable(executable);
		AutoCompileExecutable(actor, executable,
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
		if (executable is not ComputerRuntimeExecutableBase runtimeExecutable)
		{
			actor.Send("That executable cannot be edited.");
			return;
		}

		runtimeExecutable.Parameters = parameters;
		actor.Gameworld.ComputerExecutionService.SaveExecutable(executable);
		AutoCompileExecutable(actor, executable,
			$"You swap the order of {first.ColourCommand()} and {second.ColourCommand()}.");
	}

	private static void ProgrammingWorkspaceCompile(ICharacter actor, StringStack ss)
	{
		var executable = ss.IsFinished ? GetEditingComputerExecutable(actor) : ResolveProgrammingExecutable(actor, ss.SafeRemainingArgument);
		if (executable is null)
		{
			actor.Send(ss.IsFinished
				? "You are not currently editing any computer executable."
				: $"There is no such executable on {DescribeComputerOwner(actor, GetCurrentProgrammingOwner(actor))}.");
			return;
		}

		AutoCompileExecutable(actor, executable, $"You compile {executable.Name.ColourName()}.");
	}

	private static void ProgrammingWorkspaceApplication(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished || ss.PeekSpeech().EqualTo("list"))
		{
			if (!ss.IsFinished)
			{
				ss.PopSpeech();
			}

			ProgrammingWorkspaceApplications(actor);
			return;
		}

		var session = GetCurrentProgrammingTerminalSession(actor);
		if (session is null)
		{
			actor.Send("Built-in computer applications require a connected computer terminal session.");
			return;
		}

		var owner = GetCurrentProgrammingOwner(actor);
		var application = actor.Gameworld.ComputerExecutionService.GetBuiltInApplication(owner, ss.SafeRemainingArgument);
		if (application is null)
		{
			actor.Send(
				$"There is no such built-in application available on {session.Host.Name.ColourName()}.");
			return;
		}

		var result = actor.Gameworld.ComputerExecutionService.ExecuteBuiltInApplication(actor, owner, application, session);
		if (!result.Success)
		{
			actor.Send(result.ErrorMessage);
			return;
		}

		if (result.Status == ComputerProcessStatus.Sleeping && result.Process is not null)
		{
			actor.Send(
				$"{application.Name.ColourName()} is now running as process {result.Process.Id.ToString("N0", actor).ColourValue()} on {session.Host.Name.ColourName()}.");
		}
	}

	private static void ProgrammingWorkspaceApplications(ICharacter actor)
	{
		var session = GetCurrentProgrammingTerminalSession(actor);
		if (session is null)
		{
			actor.Send("Built-in computer applications require a connected computer terminal session.");
			return;
		}

		var applications = actor.Gameworld.ComputerExecutionService.GetBuiltInApplications(GetCurrentProgrammingOwner(actor))
			.ToList();
		if (!applications.Any())
		{
			actor.Send($"There are no built-in computer applications available on {session.Host.Name.ColourName()}.");
			return;
		}

		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			applications.Select(x => new List<string>
			{
				x.ApplicationId,
				x.Name,
				x.IsNetworkService ? "Network" : "Local",
				x.Summary
			}),
			new List<string>
			{
				"Id",
				"Name",
				"Type",
				"Summary"
			},
			actor.LineFormatLength,
			true,
			Telnet.BoldGreen,
			1,
			actor.Account.UseUnicode), nopage: true);
	}

	private static void ProgrammingNetwork(ICharacter actor, StringStack ss)
	{
		if (!TryGetProgrammingNetworkHost(actor, out var host))
		{
			return;
		}

		if (ss.IsFinished)
		{
			ShowProgrammingNetworkStatus(actor, host);
			return;
		}

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "domain":
				ProgrammingNetworkDomain(actor, host, ss);
				return;
			case "account":
				ProgrammingNetworkAccount(actor, host, ss);
				return;
			case "vpn":
			case "gateway":
				ProgrammingNetworkVpn(actor, host, ss);
				return;
			default:
				ShowProgrammingNetworkStatus(actor, host);
				return;
		}
	}

	private static bool TryGetProgrammingNetworkHost(ICharacter actor, out IComputerHost host)
	{
		host = default!;
		if (!actor.IsAdministrator())
		{
			actor.Send("Only administrators can configure computer network services.");
			return false;
		}

		var session = GetCurrentProgrammingTerminalSession(actor);
		if (session is null)
		{
			actor.Send("You must be connected to a computer terminal to configure network services.");
			return false;
		}

		host = session.Host;
		return true;
	}

	private static void ShowProgrammingNetworkStatus(ICharacter actor, IComputerHost host)
	{
		var identityService = actor.Gameworld.ComputerNetworkIdentityService;
		var domains = identityService.GetHostedDomains(host).ToList();
		var accounts = identityService.GetAccounts(host).ToList();
		var vpnNetworks = host.HostedVpnNetworkIds.OrderBy(x => x).ToList();
		var sb = new StringBuilder();
		sb.AppendLine($"Network Host: {host.Name.ColourName()}");
		sb.AppendLine($"Hosted Domains: {domains.Count.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Accounts: {accounts.Count.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"VPN Networks: {(vpnNetworks.Any() ? vpnNetworks.Select(x => x.ColourName()).ListToString() : "None".ColourError())}");
		sb.AppendLine();
		sb.AppendLine("Domains:");
		if (!domains.Any())
		{
			sb.AppendLine("\tNone");
		}
		else
		{
			sb.AppendLine(StringUtilities.GetTextTable(
				domains.Select(domain => new List<string>
				{
					domain.DomainName,
					domain.Enabled.ToColouredString()
				}),
				new List<string>
				{
					"Domain",
					"Enabled"
				},
				actor.LineFormatLength,
				true,
				Telnet.BoldGreen,
				1,
				actor.Account.UseUnicode));
		}

		sb.AppendLine();
		sb.AppendLine("Accounts:");
		if (!accounts.Any())
		{
			sb.AppendLine("\tNone");
		}
		else
		{
			sb.AppendLine(StringUtilities.GetTextTable(
				accounts.Select(account => new List<string>
				{
					account.Address,
					account.Enabled.ToColouredString()
				}),
				new List<string>
				{
					"Identity",
					"Enabled"
				},
				actor.LineFormatLength,
				true,
				Telnet.BoldGreen,
				1,
				actor.Account.UseUnicode));
		}

		sb.AppendLine();
		sb.AppendLine(
			$"Use {"programming network domain add|remove|enable|disable <domain>".ColourCommand()}, {"programming network account add|enable|disable|password ...".ColourCommand()}, and {"programming network vpn add|remove <network>".ColourCommand()} to configure this host.");
		actor.OutputHandler.Send(sb.ToString(), nopage: true);
	}

	private static void ProgrammingNetworkDomain(ICharacter actor, IComputerHost host, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Do you want to add, remove, enable or disable a domain?");
			return;
		}

		var action = ss.PopSpeech().ToLowerInvariant();
		if (ss.IsFinished)
		{
			actor.Send("Which domain do you want to work with?");
			return;
		}

		var domain = ss.SafeRemainingArgument;
		var identityService = actor.Gameworld.ComputerNetworkIdentityService;
		var success = action switch
		{
			"add" => identityService.RegisterDomain(host, domain, out var addError)
				? SendMailStatus(actor, $"The domain {domain.ColourName()} is now hosted by {host.Name.ColourName()}.")
				: SendMailError(actor, addError),
			"remove" => identityService.RemoveDomain(host, domain, out var removeError)
				? SendMailStatus(actor, $"The domain {domain.ColourName()} is no longer hosted by {host.Name.ColourName()}.")
				: SendMailError(actor, removeError),
			"enable" => identityService.SetDomainEnabled(host, domain, true, out var enableError)
				? SendMailStatus(actor, $"The domain {domain.ColourName()} is now enabled on {host.Name.ColourName()}.")
				: SendMailError(actor, enableError),
			"disable" => identityService.SetDomainEnabled(host, domain, false, out var disableError)
				? SendMailStatus(actor, $"The domain {domain.ColourName()} is now disabled on {host.Name.ColourName()}.")
				: SendMailError(actor, disableError),
			_ => false
		};

		if (!success && action is not ("add" or "remove" or "enable" or "disable"))
		{
			actor.Send("You must specify add, remove, enable or disable.");
		}
	}

	private static void ProgrammingNetworkAccount(ICharacter actor, IComputerHost host, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Do you want to add, enable, disable or change the password of an account?");
			return;
		}

		var action = ss.PopSpeech().ToLowerInvariant();
		var identityService = actor.Gameworld.ComputerNetworkIdentityService;
		switch (action)
		{
			case "add":
				if (ss.IsFinished)
				{
					actor.Send("Which account address do you want to create?");
					return;
				}

				var address = ss.PopSpeech();
				if (ss.IsFinished)
				{
					actor.Send("What password should that new account use?");
					return;
				}

				if (!identityService.CreateAccount(host, address, ss.SafeRemainingArgument, out var addError))
				{
					actor.Send(addError);
					return;
				}

				actor.Send($"Created the shared network account {address.ColourName()} on {host.Name.ColourName()}.");
				return;
			case "enable":
			case "disable":
				if (ss.IsFinished)
				{
					actor.Send("Which account address do you want to change?");
					return;
				}

				var targetAddress = ss.SafeRemainingArgument;
				if (!identityService.SetAccountEnabled(host, targetAddress, action == "enable", out var toggleError))
				{
					actor.Send(toggleError);
					return;
				}

				actor.Send(
					$"{targetAddress.ColourName()} is now {(action == "enable" ? "enabled".ColourValue() : "disabled".ColourError())} on {host.Name.ColourName()}.");
				return;
			case "password":
				if (ss.IsFinished)
				{
					actor.Send("Which account address do you want to change the password for?");
					return;
				}

				var passwordAddress = ss.PopSpeech();
				if (ss.IsFinished)
				{
					actor.Send("What new password should that account use?");
					return;
				}

				if (!identityService.SetAccountPassword(host, passwordAddress, ss.SafeRemainingArgument, out var passwordError))
				{
					actor.Send(passwordError);
					return;
				}

				actor.Send($"Updated the password for {passwordAddress.ColourName()} on {host.Name.ColourName()}.");
				return;
			default:
				actor.Send("You must specify add, enable, disable or password.");
				return;
		}
	}

	private static void ProgrammingNetworkVpn(ICharacter actor, IComputerHost host, StringStack ss)
	{
		if (ss.IsFinished || ss.PeekSpeech().EqualTo("list"))
		{
			if (!ss.IsFinished)
			{
				ss.PopSpeech();
			}

			var vpnNetworks = host.HostedVpnNetworkIds.OrderBy(x => x).ToList();
			if (!vpnNetworks.Any())
			{
				actor.Send($"{host.Name.ColourName()} is not currently exposing any VPN tunnel networks.");
				return;
			}

			actor.Send(
				$"{host.Name.ColourName()} is currently exposing the following VPN networks: {vpnNetworks.Select(x => x.ColourName()).ListToString()}.");
			return;
		}

		var action = ss.PopSpeech().ToLowerInvariant();
		if (ss.IsFinished)
		{
			actor.Send("Which VPN network do you want to work with?");
			return;
		}

		var networkId = ss.SafeRemainingArgument;
		switch (action)
		{
			case "add":
			case "enable":
				if (!host.AddHostedVpnNetwork(networkId, out var addError))
				{
					actor.Send(addError);
					return;
				}

				actor.Send($"{host.Name.ColourName()} now exposes the VPN network {networkId.ColourName()} for authenticated tunnels.");
				return;
			case "remove":
			case "disable":
				if (!host.RemoveHostedVpnNetwork(networkId, out var removeError))
				{
					actor.Send(removeError);
					return;
				}

				actor.Send($"{host.Name.ColourName()} no longer exposes the VPN network {networkId.ColourName()}.");
				return;
			default:
				actor.Send("You must specify add, remove, enable, disable or list.");
				return;
		}
	}

	private static void ProgrammingMail(ICharacter actor, StringStack ss)
	{
		if (!TryGetProgrammingMailHost(actor, out var host))
		{
			return;
		}

		if (ss.IsFinished)
		{
			ShowProgrammingMailStatus(actor, host);
			return;
		}

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "service":
				ProgrammingMailService(actor, host, ss);
				return;
			case "domain":
				ProgrammingMailDomain(actor, host, ss);
				return;
			case "account":
				ProgrammingMailAccount(actor, host, ss);
				return;
			default:
				ShowProgrammingMailStatus(actor, host);
				return;
		}
	}

	private static bool TryGetProgrammingMailHost(ICharacter actor, out IComputerHost host)
	{
		host = default!;
		if (!actor.IsAdministrator())
		{
			actor.Send("Only administrators can configure computer mail services.");
			return false;
		}

		var session = GetCurrentProgrammingTerminalSession(actor);
		if (session is null)
		{
			actor.Send("You must be connected to a computer terminal to configure mail services.");
			return false;
		}

		host = session.Host;
		return true;
	}

	private static void ShowProgrammingMailStatus(ICharacter actor, IComputerHost host)
	{
		var mailService = actor.Gameworld.ComputerMailService;
		var identityService = actor.Gameworld.ComputerNetworkIdentityService;
		var domains = identityService.GetHostedDomains(host).ToList();
		var accounts = identityService.GetAccounts(host).ToList();
		var sb = new StringBuilder();
		sb.AppendLine($"Mail Service Host: {host.Name.ColourName()}");
		sb.AppendLine($"Advertised: {mailService.IsMailServiceEnabled(host).ToColouredString()}");
		sb.AppendLine();
		sb.AppendLine("Domains:");
		if (!domains.Any())
		{
			sb.AppendLine("\tNone");
		}
		else
		{
			sb.AppendLine(StringUtilities.GetTextTable(
				domains.Select(domain => new List<string>
				{
					domain.DomainName,
					domain.Enabled.ToColouredString()
				}),
				new List<string>
				{
					"Domain",
					"Enabled"
				},
				actor.LineFormatLength,
				true,
				Telnet.BoldGreen,
				1,
				actor.Account.UseUnicode));
		}

		sb.AppendLine();
		sb.AppendLine("Accounts:");
		if (!accounts.Any())
		{
			sb.AppendLine("\tNone");
		}
		else
		{
			sb.AppendLine(StringUtilities.GetTextTable(
				accounts.Select(account => new List<string>
				{
					account.Address,
					account.Enabled.ToColouredString()
				}),
				new List<string>
				{
					"Mailbox",
					"Enabled"
				},
				actor.LineFormatLength,
				true,
				Telnet.BoldGreen,
				1,
				actor.Account.UseUnicode));
		}

		sb.AppendLine();
		sb.AppendLine($"Use {"programming mail service on|off".ColourCommand()}, {"programming mail domain add|remove|enable|disable <domain>".ColourCommand()}, and {"programming mail account add|enable|disable|password ...".ColourCommand()} to configure this host.");
		actor.OutputHandler.Send(sb.ToString(), nopage: true);
	}

	private static void ProgrammingMailService(ICharacter actor, IComputerHost host, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send($"Mail service advertisement on {host.Name.ColourName()} is currently {actor.Gameworld.ComputerMailService.IsMailServiceEnabled(host).ToColouredString()}.");
			return;
		}

		var enabled = ss.PopSpeech().ToLowerInvariant() switch
		{
			"on" or "enable" or "enabled" => true,
			"off" or "disable" or "disabled" => false,
			_ => (bool?)null
		};

		if (!enabled.HasValue)
		{
			actor.Send("You must specify either on or off.");
			return;
		}

		if (!actor.Gameworld.ComputerMailService.SetMailServiceEnabled(host, enabled.Value, out var error))
		{
			actor.Send(error);
			return;
		}

		actor.Send($"Mail service advertisement on {host.Name.ColourName()} is now {(enabled.Value ? "enabled".ColourValue() : "disabled".ColourError())}.");
	}

	private static void ProgrammingMailDomain(ICharacter actor, IComputerHost host, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Do you want to add, remove, enable or disable a domain?");
			return;
		}

		var action = ss.PopSpeech().ToLowerInvariant();
		if (ss.IsFinished)
		{
			actor.Send("Which domain do you want to work with?");
			return;
		}

		var domain = ss.SafeRemainingArgument;
		var identityService = actor.Gameworld.ComputerNetworkIdentityService;
		var success = action switch
		{
			"add" => identityService.RegisterDomain(host, domain, out var addError)
				? SendMailStatus(actor, $"The domain {domain.ColourName()} is now hosted by {host.Name.ColourName()}.")
				: SendMailError(actor, addError),
			"remove" => identityService.RemoveDomain(host, domain, out var removeError)
				? SendMailStatus(actor, $"The domain {domain.ColourName()} is no longer hosted by {host.Name.ColourName()}.")
				: SendMailError(actor, removeError),
			"enable" => identityService.SetDomainEnabled(host, domain, true, out var enableError)
				? SendMailStatus(actor, $"The domain {domain.ColourName()} is now enabled on {host.Name.ColourName()}.")
				: SendMailError(actor, enableError),
			"disable" => identityService.SetDomainEnabled(host, domain, false, out var disableError)
				? SendMailStatus(actor, $"The domain {domain.ColourName()} is now disabled on {host.Name.ColourName()}.")
				: SendMailError(actor, disableError),
			_ => false
		};

		if (!success && action is not ("add" or "remove" or "enable" or "disable"))
		{
			actor.Send("You must specify add, remove, enable or disable.");
		}
	}

	private static void ProgrammingMailAccount(ICharacter actor, IComputerHost host, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Do you want to add, enable, disable or change the password of an account?");
			return;
		}

		var action = ss.PopSpeech().ToLowerInvariant();
		var identityService = actor.Gameworld.ComputerNetworkIdentityService;
		switch (action)
		{
			case "add":
				if (ss.IsFinished)
				{
					actor.Send("Which account address do you want to create?");
					return;
				}

				var address = ss.PopSpeech();
				if (ss.IsFinished)
				{
					actor.Send("What password should that new account use?");
					return;
				}

				if (!identityService.CreateAccount(host, address, ss.SafeRemainingArgument, out var addError))
				{
					actor.Send(addError);
					return;
				}

				actor.Send($"Created the account {address.ColourName()} on {host.Name.ColourName()}.");
				return;
			case "enable":
			case "disable":
				if (ss.IsFinished)
				{
					actor.Send("Which account address do you want to change?");
					return;
				}

				var targetAddress = ss.SafeRemainingArgument;
				if (!identityService.SetAccountEnabled(host, targetAddress, action == "enable", out var toggleError))
				{
					actor.Send(toggleError);
					return;
				}

				actor.Send(
					$"{targetAddress.ColourName()} is now {(action == "enable" ? "enabled".ColourValue() : "disabled".ColourError())} on {host.Name.ColourName()}.");
				return;
			case "password":
				if (ss.IsFinished)
				{
					actor.Send("Which account address do you want to change the password for?");
					return;
				}

				var passwordAddress = ss.PopSpeech();
				if (ss.IsFinished)
				{
					actor.Send("What new password should that account use?");
					return;
				}

				if (!identityService.SetAccountPassword(host, passwordAddress, ss.SafeRemainingArgument, out var passwordError))
				{
					actor.Send(passwordError);
					return;
				}

				actor.Send($"Updated the password for {passwordAddress.ColourName()} on {host.Name.ColourName()}.");
				return;
			default:
				actor.Send("You must specify add, enable, disable or password.");
				return;
		}
	}

	private static bool SendMailStatus(ICharacter actor, string message)
	{
		actor.Send(message);
		return true;
	}

	private static bool SendMailError(ICharacter actor, string error)
	{
		actor.Send(error);
		return false;
	}

	private static void ProgrammingFtp(ICharacter actor, StringStack ss)
	{
		if (!TryGetProgrammingFtpHost(actor, out var host))
		{
			return;
		}

		if (ss.IsFinished)
		{
			ShowProgrammingFtpStatus(actor, host);
			return;
		}

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "service":
				ProgrammingFtpService(actor, host, ss);
				return;
			case "account":
				ProgrammingFtpAccount(actor, host, ss);
				return;
			case "file":
				ProgrammingFtpFile(actor, host, ss);
				return;
			default:
				ShowProgrammingFtpStatus(actor, host);
				return;
		}
	}

	private static bool TryGetProgrammingFtpHost(ICharacter actor, out IComputerHost host)
	{
		host = default!;
		if (!actor.IsAdministrator())
		{
			actor.Send("Only administrators can configure computer FTP services.");
			return false;
		}

		var session = GetCurrentProgrammingTerminalSession(actor);
		if (session is null)
		{
			actor.Send("You must be connected to a computer terminal to configure FTP services.");
			return false;
		}

		host = session.Host;
		return true;
	}

	private static void ShowProgrammingFtpStatus(ICharacter actor, IComputerHost host)
	{
		var ftpService = actor.Gameworld.ComputerFileTransferService;
		var owner = GetCurrentProgrammingOwner(actor);
		var accounts = ftpService.GetAccounts(host).ToList();
		var fileRows = ComputerFileTransferUtilities.EnumerateOwners(host)
			.Select(ownerEntry => new List<string>
			{
				ComputerFileTransferUtilities.DescribeOwner(ownerEntry),
				(ownerEntry.FileSystem?.Files.Count(x => x.PubliclyAccessible) ?? 0).ToString("N0", actor),
				ownerEntry.FileSystem?.UsedBytes.ToString("N0", actor) ?? "0",
				ownerEntry.FileSystem?.CapacityInBytes.ToString("N0", actor) ?? "0"
			})
			.ToList();
		var ownerFiles = owner.FileSystem?.Files
			.Where(x => x.PubliclyAccessible)
			.OrderBy(x => x.FileName)
			.ToList() ?? [];

		var sb = new StringBuilder();
		sb.AppendLine($"FTP Service Host: {host.Name.ColourName()}");
		sb.AppendLine($"Advertised: {ftpService.IsFtpServiceEnabled(host).ToColouredString()}");
		sb.AppendLine($"Current Programming Owner: {DescribeComputerOwner(actor, owner)}");
		sb.AppendLine();
		sb.AppendLine("Accessible File Owners:");
		sb.AppendLine(StringUtilities.GetTextTable(
			fileRows,
			new List<string>
			{
				"Owner",
				"Public Files",
				"Used",
				"Capacity"
			},
			actor.LineFormatLength,
			true,
			Telnet.BoldGreen,
			1,
			actor.Account.UseUnicode));
		sb.AppendLine();
		sb.AppendLine($"Published Files On {DescribeComputerOwner(actor, owner)}:");
		if (!ownerFiles.Any())
		{
			sb.AppendLine("\tNone");
		}
		else
		{
			sb.AppendLine(StringUtilities.GetTextTable(
				ownerFiles.Select(file => new List<string>
				{
					file.FileName,
					file.SizeInBytes.ToString("N0", actor),
					file.LastModifiedAtUtc.ToString(actor)
				}),
				new List<string>
				{
					"File",
					"Size",
					"Modified"
				},
				actor.LineFormatLength,
				true,
				Telnet.BoldGreen,
				1,
				actor.Account.UseUnicode));
		}

		sb.AppendLine();
		sb.AppendLine("Accounts:");
		if (!accounts.Any())
		{
			sb.AppendLine("\tNone");
		}
		else
		{
			sb.AppendLine(StringUtilities.GetTextTable(
				accounts.Select(account => new List<string>
				{
					account.UserName,
					account.Enabled.ToColouredString()
				}),
				new List<string>
				{
					"User",
					"Enabled"
				},
				actor.LineFormatLength,
				true,
				Telnet.BoldGreen,
				1,
				actor.Account.UseUnicode));
		}

		sb.AppendLine();
		sb.AppendLine($"Use {"programming ftp service on|off".ColourCommand()}, {"programming ftp account add|enable|disable|password ...".ColourCommand()}, and {"programming ftp file publish|unpublish <file>".ColourCommand()} to configure this host.");
		actor.OutputHandler.Send(sb.ToString(), nopage: true);
	}

	private static void ProgrammingFtpService(ICharacter actor, IComputerHost host, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send($"FTP service advertisement on {host.Name.ColourName()} is currently {actor.Gameworld.ComputerFileTransferService.IsFtpServiceEnabled(host).ToColouredString()}.");
			return;
		}

		var enabled = ss.PopSpeech().ToLowerInvariant() switch
		{
			"on" or "enable" or "enabled" => true,
			"off" or "disable" or "disabled" => false,
			_ => (bool?)null
		};
		if (!enabled.HasValue)
		{
			actor.Send("You must specify either on or off.");
			return;
		}

		if (!actor.Gameworld.ComputerFileTransferService.SetFtpServiceEnabled(host, enabled.Value, out var error))
		{
			actor.Send(error);
			return;
		}

		actor.Send($"FTP service advertisement on {host.Name.ColourName()} is now {(enabled.Value ? "enabled".ColourValue() : "disabled".ColourError())}.");
	}

	private static void ProgrammingFtpAccount(ICharacter actor, IComputerHost host, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Do you want to add, enable, disable or change the password of an FTP account?");
			return;
		}

		var action = ss.PopSpeech().ToLowerInvariant();
		var ftpService = actor.Gameworld.ComputerFileTransferService;
		switch (action)
		{
			case "add":
				if (ss.IsFinished)
				{
					actor.Send("Which FTP user do you want to create?");
					return;
				}

				var userName = ss.PopSpeech();
				if (ss.IsFinished)
				{
					actor.Send("What password should that new FTP account use?");
					return;
				}

				if (!ftpService.CreateAccount(host, userName, ss.SafeRemainingArgument, out var addError))
				{
					actor.Send(addError);
					return;
				}

				actor.Send($"Created the FTP account {userName.ColourName()} on {host.Name.ColourName()}.");
				return;
			case "enable":
			case "disable":
				if (ss.IsFinished)
				{
					actor.Send("Which FTP user do you want to change?");
					return;
				}

				var targetUserName = ss.SafeRemainingArgument;
				if (!ftpService.SetAccountEnabled(host, targetUserName, action == "enable", out var toggleError))
				{
					actor.Send(toggleError);
					return;
				}

				actor.Send(
					$"{targetUserName.ColourName()} is now {(action == "enable" ? "enabled".ColourValue() : "disabled".ColourError())} on {host.Name.ColourName()}.");
				return;
			case "password":
				if (ss.IsFinished)
				{
					actor.Send("Which FTP user do you want to change the password for?");
					return;
				}

				var passwordUserName = ss.PopSpeech();
				if (ss.IsFinished)
				{
					actor.Send("What new password should that FTP account use?");
					return;
				}

				if (!ftpService.SetAccountPassword(host, passwordUserName, ss.SafeRemainingArgument, out var passwordError))
				{
					actor.Send(passwordError);
					return;
				}

				actor.Send($"Updated the password for {passwordUserName.ColourName()} on {host.Name.ColourName()}.");
				return;
			default:
				actor.Send("You must specify add, enable, disable or password.");
				return;
		}
	}

	private static void ProgrammingFtpFile(ICharacter actor, IComputerHost host, StringStack ss)
	{
		var owner = GetCurrentProgrammingOwner(actor);
		if (owner is not (IComputerHost or IComputerStorage))
		{
			actor.Send("FTP file publishing only works on a connected host or one of its mounted storage devices.");
			return;
		}

		var fileSystem = owner.FileSystem;
		if (fileSystem is null)
		{
			actor.Send($"{DescribeComputerOwner(actor, owner)} does not expose a writable file system.");
			return;
		}

		if (ss.IsFinished || ss.PeekSpeech().EqualTo("list"))
		{
			if (!ss.IsFinished)
			{
				ss.PopSpeech();
			}

			var files = fileSystem.Files
				.Where(x => x.PubliclyAccessible)
				.OrderBy(x => x.FileName)
				.ToList();
			if (!files.Any())
			{
				actor.Send($"There are no published FTP files on {DescribeComputerOwner(actor, owner)}.");
				return;
			}

			actor.OutputHandler.Send(StringUtilities.GetTextTable(
				files.Select(file => new List<string>
				{
					file.FileName,
					file.SizeInBytes.ToString("N0", actor),
					file.LastModifiedAtUtc.ToString(actor)
				}),
				new List<string>
				{
					"File",
					"Size",
					"Modified"
				},
				actor.LineFormatLength,
				true,
				Telnet.BoldGreen,
				1,
				actor.Account.UseUnicode), nopage: true);
			return;
		}

		var action = ss.PopSpeech().ToLowerInvariant();
		if (ss.IsFinished)
		{
			actor.Send("Which file do you want to work with?");
			return;
		}

		var fileName = ss.SafeRemainingArgument;
		if (fileSystem.GetFile(fileName) is null)
		{
			actor.Send($"{DescribeComputerOwner(actor, owner)} does not have a file named {fileName.ColourName()}.");
			return;
		}

		var success = action switch
		{
			"publish" => fileSystem.SetFilePubliclyAccessible(fileName, true)
				? SendMailStatus(actor, $"{fileName.ColourName()} is now publicly accessible over FTP on {DescribeComputerOwner(actor, owner)}.")
				: SendMailError(actor, $"Unable to publish {fileName.ColourName()}."),
			"unpublish" => fileSystem.SetFilePubliclyAccessible(fileName, false)
				? SendMailStatus(actor, $"{fileName.ColourName()} is no longer publicly accessible over FTP on {DescribeComputerOwner(actor, owner)}.")
				: SendMailError(actor, $"Unable to unpublish {fileName.ColourName()}."),
			_ => false
		};
		if (!success && action is not ("publish" or "unpublish"))
		{
			actor.Send("You must specify publish, unpublish or list.");
		}
	}

	private static void ProgrammingWorkspaceExecute(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send($"Which executable on {DescribeComputerOwner(actor, GetCurrentProgrammingOwner(actor))} do you want to execute?");
			return;
		}

		var owner = GetCurrentProgrammingOwner(actor);
		var session = GetCurrentProgrammingTerminalSession(actor);
		var executable = ResolveProgrammingExecutable(actor, ss.PopSpeech(), owner);
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

		var result = actor.Gameworld.ComputerExecutionService.Execute(actor, owner, executable, parameters,
			ReferenceEquals(session?.CurrentOwner, owner) ? session : null);
		if (!result.Success && result.Status != ComputerProcessStatus.Completed)
		{
			actor.Send(result.ErrorMessage);
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Executing {executable.Name.ColourName()} [{executable.Id.ToString("N0", actor).ColourValue()}]...");
		sb.AppendLine($"Owner: {DescribeComputerOwner(actor, owner)}");
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
		var owner = GetCurrentProgrammingOwner(actor);
		var processes = actor.Gameworld.ComputerExecutionService.GetProcesses(owner).ToList();
		if (!processes.Any())
		{
			actor.Send($"There are no computer-program processes on {DescribeComputerOwner(actor, owner)}.");
			return;
		}

		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			processes.Select(x => new List<string>
			{
				x.Id.ToString("N0", actor),
				x.ProcessName,
				x.Status.DescribeEnum(),
				DescribeComputerProcessWait(actor, x),
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

	private static string DescribeComputerProcessWait(ICharacter actor, IComputerProcess process)
	{
		if (process.WaitType == ComputerProcessWaitType.Sleep && process.WakeTimeUtc.HasValue)
		{
			return process.WakeTimeUtc.Value.ToString(actor);
		}

		if (process.WaitType == ComputerProcessWaitType.UserInput &&
		    process.WaitingTerminalItemId.HasValue)
		{
			var terminal = actor.Gameworld.TryGetItem(process.WaitingTerminalItemId.Value, true);
			var waitingUser = process.WaitingCharacterId.HasValue
				? actor.Gameworld.TryGetCharacter(process.WaitingCharacterId.Value, true)
				: null;
			var terminalText = terminal?.HowSeen(actor, true) ?? $"terminal #{process.WaitingTerminalItemId.Value.ToString("N0", actor)}";
			if (waitingUser is not null)
			{
				return $"User Input ({terminalText}, {waitingUser.HowSeen(actor)})";
			}

			return $"User Input ({terminalText})";
		}

		if (process.WaitType == ComputerProcessWaitType.Signal &&
		    ComputerProcessWaitArguments.TryParseSignal(process.WaitArgument, out var signalBinding))
		{
			return $"Signal ({SignalComponentUtilities.DescribeSignalComponent(signalBinding)})";
		}

		return process.WaitType.DescribeEnum();
	}

	private static void ProgrammingWorkspaceKill(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished || !long.TryParse(ss.PopSpeech(), out var processId))
		{
			actor.Send("Which process id do you want to kill?");
			return;
		}

		var owner = GetCurrentProgrammingOwner(actor);
		if (!actor.Gameworld.ComputerExecutionService.KillProcess(owner, processId, out var error))
		{
			actor.Send(error);
			return;
		}

		actor.Send(
			$"You kill the computer-program process {processId.ToString("N0", actor).ColourValue()} on {DescribeComputerOwner(actor, owner)}.");
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

	private static ComputerTerminalSessionEffect? GetCurrentProgrammingTerminalSessionEffect(ICharacter actor)
	{
		return actor.CombinedEffectsOfType<ComputerTerminalSessionEffect>().FirstOrDefault();
	}

	private static bool TryEnsureProgrammingTerminalSession(ICharacter actor, ComputerTerminalGameItemComponent terminal,
		out IComputerTerminalSession? session, out bool connectedTerminal, out string error)
	{
		session = null;
		connectedTerminal = false;

		var existingEffect = GetCurrentProgrammingTerminalSessionEffect(actor);
		if (existingEffect is not null && !ReferenceEquals(existingEffect.Session.Terminal, terminal))
		{
			actor.RemoveEffect(existingEffect);
			existingEffect = null;
		}

		if (!terminal.TryConnectSession(actor, out session, out error))
		{
			return false;
		}

		if (existingEffect is not null && ReferenceEquals(existingEffect.Session.Terminal, terminal) &&
		    ReferenceEquals(existingEffect.Session, session))
		{
			return true;
		}

		actor.RemoveAllEffects<ComputerTerminalSessionEffect>();
		actor.AddEffect(new ComputerTerminalSessionEffect(actor) { Session = session! });
		connectedTerminal = true;
		error = string.Empty;
		return true;
	}

	private static IComputerTerminalSession? GetCurrentProgrammingTerminalSession(ICharacter actor)
	{
		return GetCurrentProgrammingTerminalSessionEffect(actor)?.Session;
	}

	private static IEnumerable<(IGameItem Item, ComputerTerminalGameItemComponent Terminal)> GetContextualComputerTerminals(
		ICharacter actor)
	{
		return actor.ContextualItems
			.Select(x => (Item: x, Terminal: x.Components.OfType<IComputerTerminal>().FirstOrDefault()))
			.Where(x => x.Terminal is ComputerTerminalGameItemComponent)
			.Select(x => (x.Item, (ComputerTerminalGameItemComponent)x.Terminal!))
			.GroupBy(x => x.Item.Id)
			.Select(x => x.First())
			.ToList();
	}

	private static IEnumerable<(IGameItem Item, ComputerTerminalGameItemComponent Terminal)>
		GetPositionTargetComputerTerminals(ICharacter actor)
	{
		if (actor.PositionTarget is not IGameItem itemTarget)
		{
			return Enumerable.Empty<(IGameItem Item, ComputerTerminalGameItemComponent Terminal)>();
		}

		var terminals = new List<(IGameItem Item, ComputerTerminalGameItemComponent Terminal)>();
		if (itemTarget.Components.OfType<IComputerTerminal>().FirstOrDefault() is ComputerTerminalGameItemComponent direct)
		{
			terminals.Add((itemTarget, direct));
		}

		terminals.AddRange(itemTarget.AttachedAndConnectedItems
			.Select(x => (Item: x, Terminal: x.Components.OfType<IComputerTerminal>().FirstOrDefault()))
			.Where(x => x.Terminal is ComputerTerminalGameItemComponent)
			.Select(x => (x.Item, (ComputerTerminalGameItemComponent)x.Terminal!)));

		return terminals
			.GroupBy(x => x.Item.Id)
			.Select(x => x.First())
			.ToList();
	}

	private static bool TryResolveTerminalForTyping(ICharacter actor, string argumentText, out IGameItem terminalItem,
		out ComputerTerminalGameItemComponent terminal, out string text, out string error)
	{
		terminalItem = null!;
		terminal = null!;
		text = string.Empty;
		error = string.Empty;

		var ss = new StringStack(argumentText);
		var firstToken = ss.PopSpeech();
		if (!string.IsNullOrWhiteSpace(firstToken) && !ss.IsFinished)
		{
			var explicitTerminalItem = actor.TargetItem(firstToken);
			if (explicitTerminalItem?.Components.OfType<IComputerTerminal>().FirstOrDefault() is
			    ComputerTerminalGameItemComponent explicitTerminal)
			{
				terminalItem = explicitTerminalItem;
				terminal = explicitTerminal;
				text = ss.SafeRemainingArgument.Trim();
				return true;
			}
		}

		var currentSession = GetCurrentProgrammingTerminalSession(actor);
		if (currentSession?.Terminal is ComputerTerminalGameItemComponent currentTerminal)
		{
			terminalItem = currentTerminal.Parent;
			terminal = currentTerminal;
			text = argumentText;
			return true;
		}

		var nearbyTerminals = GetContextualComputerTerminals(actor).ToList();
		if (nearbyTerminals.Count == 1)
		{
			(terminalItem, terminal) = nearbyTerminals[0];
			text = argumentText;
			return true;
		}

		var positionTargetTerminals = GetPositionTargetComputerTerminals(actor).ToList();
		if (positionTargetTerminals.Count == 1)
		{
			(terminalItem, terminal) = positionTargetTerminals[0];
			text = argumentText;
			return true;
		}

		if (!nearbyTerminals.Any() && !positionTargetTerminals.Any())
		{
			error = "You are not near any usable computer terminal.";
			return false;
		}

		var candidateText = nearbyTerminals
			.Concat(positionTargetTerminals)
			.GroupBy(x => x.Item.Id)
			.Select(x => x.First())
			.OrderBy(x => x.Item.HowSeen(actor))
			.Select(x => $"\t{actor.BestKeywordFor(x.Item).ColourCommand()} - {x.Item.HowSeen(actor, true).ColourName()}")
			.ToList();
		error =
			$"You are near more than one usable computer terminal. Position yourself at one, connect explicitly with {"programming terminal connect <terminal>".ColourCommand()}, or use one of:\n{candidateText.ListToString(separator: "\n", conjunction: string.Empty, twoItemJoiner: "\n")}";
		return false;
	}

	private static IComputerExecutableOwner GetCurrentProgrammingOwner(ICharacter actor)
	{
		return GetCurrentProgrammingTerminalSession(actor)?.CurrentOwner ??
		       actor.Gameworld.ComputerExecutionService.GetWorkspace(actor);
	}

	private static IComputerExecutableOwner? ResolveExecutableOwner(ICharacter actor, IComputerExecutableDefinition executable)
	{
		if (executable.OwnerCharacterId == actor.Id)
		{
			return actor.Gameworld.ComputerExecutionService.GetWorkspace(actor);
		}

		if (executable.OwnerHostItemId is > 0)
		{
			return actor.Gameworld.TryGetItem(executable.OwnerHostItemId.Value, true)?.Components.OfType<IComputerHost>()
				.FirstOrDefault();
		}

		if (executable.OwnerStorageItemId is > 0)
		{
			return actor.Gameworld.TryGetItem(executable.OwnerStorageItemId.Value, true)?.Components.OfType<IComputerStorage>()
				.FirstOrDefault();
		}

		return null;
	}

	private static IComputerStorage? ResolveTerminalStorageOwner(ICharacter actor, IComputerHost host, string identifier)
	{
		var storageItems = host.MountedStorage
			.Select(x => x as IGameItemComponent)
			.Where(x => x is not null)
			.Select(x => x!.Parent)
			.Distinct()
			.ToList();
		if (!storageItems.Any())
		{
			actor.Send($"{host.Name.ColourName()} has no mounted storage devices.");
			return null;
		}

		var item = ResolveItemReference(actor, storageItems, identifier);
		if (item?.Components.OfType<IComputerStorage>().FirstOrDefault() is { } storage)
		{
			return storage;
		}

		actor.Send(
			$"You must specify one of the following mounted storage devices on {host.Name.ColourName()}:\n{storageItems.Select(x => $"\t{x.HowSeen(actor, true).ColourName()}").ListToLines()}");
		return null;
	}

	private static string DescribeComputerOwner(ICharacter actor, IComputerExecutableOwner owner)
	{
		return owner switch
		{
			ICharacterComputerWorkspace => "your private workspace".ColourName(),
			IComputerStorage => owner.Name.ColourName(),
			IComputerHost => owner.Name.ColourName(),
			_ => owner.Name.ColourName()
		};
	}

	private static string DescribeTerminal(ICharacter actor, IComputerTerminal terminal)
	{
		return terminal is IGameItemComponent component
			? component.Parent.HowSeen(actor, true).ColourName()
			: terminal.GetType().Name.ColourName();
	}

	private static IComputerExecutableDefinition? ResolveProgrammingExecutable(ICharacter actor, string identifier,
		IComputerExecutableOwner? owner = null)
	{
		owner ??= GetCurrentProgrammingOwner(actor);
		var executable = actor.Gameworld.ComputerExecutionService.GetExecutable(owner, identifier);
		if (executable is not null)
		{
			return executable;
		}

		actor.Send($"There is no such executable on {DescribeComputerOwner(actor, owner)}.");
		return null;
	}

	private static void AutoCompileExecutable(ICharacter actor, IComputerExecutableDefinition executable,
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
		var owner = ResolveExecutableOwner(actor, executable);
		var sb = new StringBuilder();
		sb.AppendLine(
			$"{executable.Name.ColourName()} [{executable.Id.ToString("N0", actor).ColourValue()}] - {executable.ExecutableKind.DescribeEnum().ColourValue()}");
		if (owner is not null)
		{
			sb.AppendLine($"Owner: {DescribeComputerOwner(actor, owner)}");
		}
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

	private static void ProgrammingFile(ICharacter actor, IGameItem item, StringStack ss)
	{
		if (!CanServiceElectronicsTarget(actor, item, out var serviceError))
		{
			actor.Send(serviceError);
			return;
		}

		var actionKeywords = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
		{
			"show",
			"edit",
			"write",
			"public"
		};

		IFileSignalGenerator? generator;
		if (ss.IsFinished)
		{
			generator = ResolveOptionalFileSignalGenerator(actor, item, ss, "file-backed signal generator");
			if (generator is null)
			{
				return;
			}

			ShowFileSignalGenerator(actor, generator);
			return;
		}

		if (actionKeywords.Contains(ss.PeekSpeech()))
		{
			generator = ResolveOptionalFileSignalGenerator(actor, item, new StringStack(string.Empty),
				"file-backed signal generator");
			if (generator is null)
			{
				return;
			}
		}
		else
		{
			generator = ResolveOptionalFileSignalGenerator(actor, item, ss, "file-backed signal generator");
			if (generator is null)
			{
				return;
			}

			if (ss.IsFinished)
			{
				ShowFileSignalGenerator(actor, generator);
				return;
			}
		}

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "show":
				ShowFileSignalGenerator(actor, generator);
				return;
			case "edit":
				ProgrammingFileEdit(actor, item, generator);
				return;
			case "write":
				ProgrammingFileWrite(actor, item, generator, ss);
				return;
			case "public":
				ProgrammingFilePublic(actor, item, generator, ss);
				return;
			default:
				actor.OutputHandler.Send(ProgrammingHelpText.SubstituteANSIColour());
				return;
		}
	}

	private static void ProgrammingFileEdit(ICharacter actor, IGameItem item, IFileSignalGenerator generator)
	{
		var existing = generator.FileSystem?.ReadFile(generator.SignalFileName) ?? string.Empty;
		actor.Send("Enter the replacement file contents in the editor below.");
		actor.EditorMode(
			(text, handler, _) => StartProgrammingAction(
				actor,
				item,
				"programming $1",
				outcome =>
				{
					generator.FileSystem?.WriteFile(generator.SignalFileName, text);
					actor.Send(
						$"You replace the contents of {generator.SignalFileName.ColourCommand()} for {DescribeComponent(actor, generator).ColourName()}.");
					return true;
				}),
			(handler, _) => handler.Send("You decide not to change the signal file."),
			1.0,
			recallText: existing,
			options: EditorOptions.PermitEmpty);
	}

	private static void ProgrammingFileWrite(ICharacter actor, IGameItem item, IFileSignalGenerator generator,
		StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("What text should the backing signal file contain?");
			return;
		}

		var text = ss.SafeRemainingArgument;
		StartProgrammingAction(
			actor,
			item,
			"programming $1",
			outcome =>
			{
				generator.FileSystem?.WriteFile(generator.SignalFileName, text);
				actor.Send(
					$"You replace the contents of {generator.SignalFileName.ColourCommand()} for {DescribeComponent(actor, generator).ColourName()}.");
				return true;
			});
	}

	private static void ProgrammingFilePublic(ICharacter actor, IGameItem item, IFileSignalGenerator generator,
		StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Should that backing signal file be public or private?");
			return;
		}

		bool? isPublic = ss.PopSpeech().ToLowerInvariant() switch
		{
			"on" or "yes" or "true" or "public" => true,
			"off" or "no" or "false" or "private" => false,
			_ => null
		};
		if (!isPublic.HasValue)
		{
			actor.Send("You must specify either on or off.");
			return;
		}

		StartProgrammingAction(
			actor,
			item,
			"programming $1",
			outcome =>
			{
				if (generator.FileSystem?.SetFilePubliclyAccessible(generator.SignalFileName, isPublic.Value) != true)
				{
					actor.Send("That component does not currently have a backing signal file.");
					return false;
				}

				actor.Send(
					$"{generator.SignalFileName.ColourCommand()} is now {(isPublic.Value ? "public".ColourValue() : "private".ColourError())} for {DescribeComponent(actor, generator).ColourName()}.");
				return true;
			});
	}

	private static void ShowFileSignalGenerator(ICharacter actor, IFileSignalGenerator generator)
	{
		var file = generator.FileSystem?.GetFile(generator.SignalFileName);
		var sb = new StringBuilder();
		sb.AppendLine($"{DescribeComponent(actor, generator).ColourName()}");
		sb.AppendLine(
			$"\tOutput: {generator.CurrentValue.ToString("N2", actor).ColourValue()} on {generator.EndpointKey.ColourCommand()}{DescribeElectricalMachineStateSuffix(generator)}");
		sb.AppendLine($"\tFile: {generator.SignalFileName.ColourCommand()}");
		sb.AppendLine(
			$"\tStatus: {(generator.FileValueValid ? generator.FileStatus.ColourValue() : generator.FileStatus.ColourError())}");
		sb.AppendLine($"\tPublic: {(file?.PubliclyAccessible ?? false).ToColouredString()}");
		sb.AppendLine($"\tModified: {(file?.LastModifiedAtUtc.ToString(actor) ?? "never".ColourError())}");
		sb.AppendLine();
		sb.AppendLine(file?.TextContents ?? string.Empty);
		actor.OutputHandler.Send(sb.ToString(), nopage: true);
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
		var anchorItem = ResolveSignalSearchAnchorItem(item);
		var nearbyCables = EnumerateNearbySignalItems(actor, anchorItem)
			.SelectMany(x => x.Components.OfType<SignalCableSegmentGameItemComponent>())
			.Distinct()
			.OrderBy(x => x.Parent.Id)
			.ThenBy(x => x.Id)
			.ToList();
		var sources = statusItems.SelectMany(x => x.Components.OfType<ISignalSourceComponent>()).ToList();
		var sinks = statusItems.SelectMany(x => x.Components.OfType<IRuntimeConfigurableSignalSinkComponent>()).ToList();
		var controllers = statusItems.SelectMany(x => x.Components.OfType<IRuntimeProgrammableMicrocontroller>()).ToList();
		var hosts = statusItems.SelectMany(x => x.Components.OfType<IAutomationMountHost>()).ToList();
		var housings = statusItems.SelectMany(x => x.Components.OfType<IAutomationHousing>()).Distinct().ToList();
		if (!sources.Any() && !sinks.Any() && !controllers.Any() && !hosts.Any() && !housings.Any())
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
				var details = new List<string>
				{
					$"{source.CurrentValue.ToString("N2", actor).ColourValue()} on {source.EndpointKey.ColourCommand()}"
				};
				var machineState = DescribeElectricalMachineState(source);
				if (!string.IsNullOrWhiteSpace(machineState))
				{
					details.Add(machineState);
				}

				if (source is SignalCableSegmentGameItemComponent cable)
				{
					var upstreamSource = SignalComponentUtilities.FindSignalSource(cable.Parent, cable.CurrentBinding, cable);
					details.Add(
						cable.IsRouted
							? $"mirroring {DescribeSignalBinding(actor, cable.CurrentBinding).ColourCommand()} across {cable.RouteDescription.ColourCommand()} ({(upstreamSource is null ? "route broken".ColourError() : "route live".ColourValue())})"
							: "not currently routed".ColourError());
				}
				else if (source is IFileSignalGenerator fileGenerator)
				{
					var file = fileGenerator.FileSystem?.GetFile(fileGenerator.SignalFileName);
					details.Add(
						$"reading {fileGenerator.SignalFileName.ColourCommand()} ({(fileGenerator.FileValueValid ? fileGenerator.FileStatus.ColourValue() : fileGenerator.FileStatus.ColourError())}, public {(file?.PubliclyAccessible ?? false).ToColouredString()})");
				}

				sb.AppendLine($"\t{DescribeComponent(actor, source).ColourName()} -> {string.Join(", ", details)}");
			}
		}

		if (nearbyCables.Any())
		{
			sb.AppendLine();
			sb.AppendLine("Nearby Cable Routes:");
			foreach (var cable in nearbyCables)
			{
				sb.AppendLine($"\t{DescribeCableRoute(actor, cable)}");
			}
		}

		if (controllers.Any())
		{
			sb.AppendLine();
			sb.AppendLine("Programmable Controllers:");
			foreach (var controller in controllers.OrderBy(x => x.Parent.Id).ThenBy(x => x.Id))
			{
				var controllerComponent = (IGameItemComponent)controller;
				var controllerSource = (ISignalSourceComponent)controller;
				sb.AppendLine(
					$"\t{DescribeComponent(actor, controllerComponent).ColourName()} - {(controller.LogicCompiles ? "compiled".ColourValue() : controller.CompileError.ColourError())}, output {controllerSource.CurrentValue.ToString("N2", actor).ColourValue()} on {controllerSource.EndpointKey.ColourCommand()}{DescribeElectricalMachineStateSuffix(controllerComponent)}");
				if (!controller.InputBindings.Any())
				{
					sb.AppendLine("\t\tInputs: none");
					continue;
				}

				foreach (var binding in controller.InputBindings.OrderBy(x => x.VariableName))
				{
					var resolvedSource = SignalComponentUtilities.FindSignalSource(controllerComponent.Parent, binding.Binding,
						controllerComponent);
					sb.AppendLine(
						$"\t\t{binding.VariableName.ColourCommand()} <- {DescribeSignalBinding(actor, binding.Binding).ColourCommand()} = {binding.CurrentValue.ToString("N2", actor).ColourValue()} ({(resolvedSource is null ? "unresolved".ColourError() : $"resolved to {DescribeComponent(actor, resolvedSource).ColourName()}")})");
				}
			}
		}

		if (sinks.Any())
		{
			sb.AppendLine();
			sb.AppendLine("Configurable Sinks:");
			foreach (var sink in sinks.OrderBy(x => x.Parent.Id).ThenBy(x => x.Id))
			{
				var resolvedSource = SignalComponentUtilities.FindSignalSource(sink.Parent, sink.CurrentBinding,
					sink as IGameItemComponent);
				sb.AppendLine(
					$"\t{DescribeComponent(actor, sink).ColourName()} <- {DescribeSignalBinding(actor, sink.CurrentBinding).ColourCommand()}, threshold {sink.ActivationThreshold.ToString("N2", actor).ColourValue()}, mode {(sink.ActiveWhenAboveThreshold ? "above/equal".ColourValue() : "below".ColourValue())} ({(resolvedSource is null ? "unresolved".ColourError() : $"resolved to {DescribeComponent(actor, resolvedSource).ColourName()} at {resolvedSource.CurrentValue.ToString("N2", actor).ColourValue()}")})");
			}
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	private static string DescribeCableRoute(ICharacter actor, SignalCableSegmentGameItemComponent cable)
	{
		if (!cable.IsRouted)
		{
			return $"{DescribeComponent(actor, cable).ColourName()} - {"not currently routed".ColourError()}";
		}

		var upstreamSource = SignalComponentUtilities.FindSignalSource(cable.Parent, cable.CurrentBinding, cable);
		return
			$"{DescribeComponent(actor, cable).ColourName()} - mirroring {DescribeSignalBinding(actor, cable.CurrentBinding).ColourCommand()} across {cable.RouteDescription.ColourCommand()} ({(upstreamSource is null ? "route broken".ColourError() : "route live".ColourValue())}), current {cable.CurrentValue.ToString("N2", actor).ColourValue()} on {cable.EndpointKey.ColourCommand()}";
	}

	private static void ShowProgrammingStatus(ICharacter actor, IGameItem item)
	{
		var controllers = item.Components.OfType<IRuntimeProgrammableMicrocontroller>().ToList();
		var fileGenerators = item.Components.OfType<IFileSignalGenerator>().ToList();
		if (!controllers.Any() && !fileGenerators.Any())
		{
			actor.Send($"{item.HowSeen(actor, true)} has no programmable microcontrollers or file-backed signal generators.");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine($"{item.HowSeen(actor, true)} has the following programmable electronics:");
		foreach (var controller in controllers.OrderBy(x => ((IGameItemComponent)x).Id))
		{
			var component = (IGameItemComponent)controller;
			var controllerSource = (ISignalSourceComponent)controller;
			sb.AppendLine(
				$"\t{DescribeComponent(actor, component).ColourName()} - {(controller.LogicCompiles ? "compiled".ColourValue() : controller.CompileError.ColourError())}, output {controllerSource.CurrentValue.ToString("N2", actor).ColourValue()} on {controllerSource.EndpointKey.ColourCommand()}{DescribeElectricalMachineStateSuffix(component)}");
			if (!controller.InputBindings.Any())
			{
				sb.AppendLine("\t\tInputs: none");
				continue;
			}

			foreach (var binding in controller.InputBindings.OrderBy(x => x.VariableName))
			{
				var resolvedSource = SignalComponentUtilities.FindSignalSource(component.Parent, binding.Binding, component);
				sb.AppendLine(
					$"\t\t{binding.VariableName.ColourCommand()} <- {DescribeSignalBinding(actor, binding.Binding).ColourCommand()} = {binding.CurrentValue.ToString("N2", actor).ColourValue()} ({(resolvedSource is null ? "unresolved".ColourError() : $"resolved to {DescribeComponent(actor, resolvedSource).ColourName()}")})");
			}
		}

		foreach (var generator in fileGenerators.OrderBy(x => ((IGameItemComponent)x).Id))
		{
			var file = generator.FileSystem?.GetFile(generator.SignalFileName);
			sb.AppendLine(
				$"\t{DescribeComponent(actor, generator).ColourName()} - file-driven signal source, output {generator.CurrentValue.ToString("N2", actor).ColourValue()} on {generator.EndpointKey.ColourCommand()}{DescribeElectricalMachineStateSuffix(generator)}");
			sb.AppendLine(
				$"\t\t{generator.SignalFileName.ColourCommand()} - {(generator.FileValueValid ? generator.FileStatus.ColourValue() : generator.FileStatus.ColourError())}, public {(file?.PubliclyAccessible ?? false).ToColouredString()}");
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

	private static IFileSignalGenerator? ResolveOptionalFileSignalGenerator(ICharacter actor, IGameItem item,
		StringStack ss, string componentTypeDescription)
	{
		var generators = item.Components.OfType<IFileSignalGenerator>().ToList();
		if (!generators.Any())
		{
			actor.Send($"{item.HowSeen(actor, true)} has no {componentTypeDescription}s.");
			return null;
		}

		if (generators.Count == 1)
		{
			if (!ss.IsFinished)
			{
				var explicitGenerator = TryResolveComponentOnItem<IFileSignalGenerator>(item, ss.PeekSpeech());
				if (explicitGenerator is not null)
				{
					ss.PopSpeech();
					return explicitGenerator;
				}
			}

			return generators[0];
		}

		if (ss.IsFinished)
		{
			actor.Send($"Which {componentTypeDescription} do you mean?");
			return null;
		}

		return ResolveComponentOnItem<IFileSignalGenerator>(actor, item, ss.PopSpeech(), componentTypeDescription);
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
		var anchorCells = anchorItem.TrueLocations.OfType<ICell>().Distinct().ToList();
		if (!anchorCells.Any() && actor.Location is not null)
		{
			anchorCells.Add(actor.Location);
		}

		foreach (var cell in anchorCells)
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
		return SignalComponentUtilities.ResolveSignalSearchAnchorItem(item);
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

	private static string DescribeSignalBinding(ICharacter actor, LocalSignalBinding binding)
	{
		var sourceItem = binding.SourceItemId > 0
			? actor.Gameworld.TryGetItem(binding.SourceItemId, true)
			: null;
		var itemDescription = sourceItem?.HowSeen(actor, true) ??
		                      (!string.IsNullOrWhiteSpace(binding.SourceItemName)
			                      ? binding.SourceItemName
			                      : "unknown item");
		var componentDescription = !string.IsNullOrWhiteSpace(binding.SourceComponentName)
			? binding.SourceComponentName
			: "unknown component";
		return
			$"{itemDescription}@{componentDescription}:{SignalComponentUtilities.NormaliseSignalEndpointKey(binding.SourceEndpointKey)}";
	}

	private static string DescribeElectricalMachineState(IGameItemComponent component)
	{
		return component switch
		{
			PoweredMachineBaseGameItemComponent machine => $"{(machine.SwitchedOn ? "switched on".ColourValue() : "switched off".ColourError())}, {(machine.IsPowered ? "powered".ColourValue() : "not powered".ColourError())}",
			IOnOff onOff => onOff.SwitchedOn ? "switched on".ColourValue() : "switched off".ColourError(),
			_ => string.Empty
		};
	}

	private static string DescribeElectricalMachineStateSuffix(IGameItemComponent component)
	{
		var state = DescribeElectricalMachineState(component);
		return string.IsNullOrWhiteSpace(state) ? string.Empty : $", {state}";
	}

	private static double ToolQualityBonus(IGameItem tool)
	{
		return StandardCheck.BonusesPerDifficultyLevel * ((int)tool.Quality - 5);
	}
}
