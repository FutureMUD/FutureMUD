using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Commands.Helpers;
using MudSharp.Commands.Trees;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Economy.Banking;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Employment;
using MudSharp.Economy.Hospitals;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

#nullable enable

namespace MudSharp.Commands.Modules;

internal partial class EconomyModule
{
	private const string HospitalPlayerHelp = @"You can use the following options with the hospital command:

	#3hospital#0 - shows the hospital at your current location
	#3hospital services#0 - lists available medical services
	#3hospital service <##|name>#0 - shows one medical service
	#3hospital request <service> [for <target>] [cash|debt|with <payment item>]#0 - requests treatment and creates hospital task work
	#3hospital debt [person]#0 - shows medical debt or prepaid credit for yourself or a patient you can see
	#3hospital debt pay <amount> [for <target>] [cash|with <payment item>]#0 - pays down debt or creates prepaid credit
	#3hospital tasks actions [all|category|action]#0 - lists hospital task action catalogue entries
	#3hospital tasks conditions [all|category|condition]#0 - lists scheduled-rule condition catalogue entries

Hospital managers and proprietors standing in the hospital can use #3hospital help#0 to see employment, task, room, service, and debt commands.";

	private const string HospitalHelp = @"You can use the following options with the hospital command:

	#3hospital#0 - shows the hospital at your current location
	#3hospital services#0 - lists available medical services
	#3hospital service <##|name>#0 - shows one medical service
	#3hospital request <service> [for <target>] [cash|debt|with <payment item>]#0 - requests treatment and creates hospital task work
	#3hospital debt [person]#0 - shows medical debt or prepaid credit for yourself or a patient you can see
	#3hospital debt pay <amount> [for <target>] [cash|with <payment item>]#0 - pays down debt or creates prepaid credit
	#3hospital requests#0 - lists hospital service requests
	#3hospital requestshow <##>#0 - shows a hospital service request
	#3hospital operations#0 - shows room, theatre, procedure, staff, blocker, and reserved-resource status
	#3hospital open|close#0 - opens or closes the hospital
	#3hospital maxdebt <amount>#0 - sets the default debt ceiling for new patients
	#3hospital room add|remove <waiting|theatre|supply|recovery|staff> [here|<direction>|<#>]#0 - flags hospital rooms
	#3hospital service add <type> <price> <name>#0 - creates a service
	#3hospital service set <service> price|active|debt|theatre|recovery|blood|equipment|procedure|implant|implantpower|implantinterface|anesthesia|cannulation|parameters|name|desc <value>#0 - edits a service
	#3hospital bloodstock [show|set|clear] ...#0 - manages blood type target stock and donor prices
	#3hospital deposit <amount>#0 - deposits held cash into the hospital's virtual cash balance
	#3hospital withdraw <amount>#0 - withdraws cash from the hospital's virtual cash balance
	#3hospital ledger [count]#0 - reviews hospital cash ledger entries
	#3hospital cash [balance|deposit|withdraw|ledger] ...#0 - reviews or manages hospital virtual cash
	#3hospital employ|orderly|fire|manager|proprietor <target|name>#0 - manages direct contracts

Hospital employment records:

	#3hospital status#0 - shows employment status
	#3hospital contracts#0 - lists employment contracts
	#3hospital contracts delegate <##> show|grant|revoke|set ...#0 - views or changes delegated authority
	#3hospital openings#0 - lists employment openings
	#3hospital openings create <MedicalWorker|HospitalOrderly> <hourly rate> [positions]#0 - creates an NPC-facing staff opening
	#3hospital applications#0 - lists employment applications
	#3hospital applications accept|reject <##> [reason]#0 - accepts or rejects an NPC application
	#3hospital payroll#0 - lists wage payables and overdue days

Hospital employment tasks:

	#3hospital tasks#0 - lists scheduled rules and active tasks
	#3hospital tasks show <##|name>#0 - shows an active task
	#3hospital tasks diagnose#0 - explains why active employees can or cannot auto-claim tasks
	#3hospital tasks cancel <##|name> [reason]#0 - cancels a pending, assigned, in-progress, or blocked active task
	#3hospital tasks rule show <##|name>#0 - shows a scheduled rule
	#3hospital tasks create <name> <action> [then <action> ...]#0 - creates and finalises a task
	#3hospital tasks draft new|show|rename|remove|discard|finalise ...#0 - drafts and finalises active tasks
	#3hospital tasks step <action syntax>#0 - adds a catalogue action to your active-task draft
	#3hospital tasks actions [all|category|action]#0 - lists task action catalogue entries
	#3hospital tasks conditions [all|category|condition]#0 - lists scheduled-rule condition catalogue entries

Hospital communication and audit:

	#3hospital register#0 - shows employment register entries
	#3hospital employmentledger|empledger#0 - shows employment ledger entries
	#3hospital board [read <##>|write <title>]#0 - uses the staff board

Administrators can also use:
	#3hospital create <name> <economic zone> <bank account|none>#0 - creates a hospital at your current location
	#3hospital delete#0 - deletes the hospital at your current location
	#3hospital list all#0 - lists all hospitals";

	[PlayerCommand("Hospital", "hospital", "clinic", "infirmary")]
	[RequiredCharacterState(CharacterState.Conscious)]
	[NoCombatCommand]
	[NoHideCommand]
	[HelpInfo("hospital", HospitalPlayerHelp, AutoHelp.HelpArg, HospitalHelp)]
	[ConditionalHelpInfo(nameof(CanSeeHospitalManagerHelp), HospitalHelp)]
	protected static void Hospital(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		var subcommand = ss.PopSpeech().CollapseString().ToLowerInvariant();
		switch (subcommand)
		{
			case "":
			case "info":
				HospitalInfo(actor);
				return;
			case "help":
				actor.OutputHandler.Send(HospitalHelpFor(actor).SubstituteANSIColour());
				return;
			case "services":
			case "listservices":
				HospitalServices(actor);
				return;
			case "service":
				HospitalServiceCommand(actor, ss);
				return;
			case "request":
			case "procure":
			case "buy":
				HospitalRequest(actor, ss);
				return;
			case "requestshow":
			case "showrequest":
				HospitalShowRequest(actor, ss);
				return;
			case "requests":
			case "queue":
				HospitalRequests(actor, ss);
				return;
			case "operations":
			case "ops":
				HospitalOperations(actor);
				return;
			case "debt":
			case "account":
				HospitalDebt(actor, ss);
				return;
			case "cash":
			case "finance":
				HospitalCash(actor, ss);
				return;
			case "deposit":
				HospitalCashDeposit(actor, ss);
				return;
			case "withdraw":
				HospitalCashWithdraw(actor, ss);
				return;
			case "ledger":
			case "cashledger":
				HospitalCashLedger(actor, ss);
				return;
			case "bloodstock":
			case "bloodstocks":
			case "bloodpolicy":
				HospitalBloodStock(actor, ss);
				return;
			case "open":
				HospitalOpen(actor);
				return;
			case "close":
				HospitalClose(actor);
				return;
			case "maxdebt":
			case "debtlimit":
				HospitalMaxDebt(actor, ss);
				return;
			case "room":
			case "rooms":
				HospitalRoom(actor, ss);
				return;
			case "employ":
				HospitalDirectHire(actor, ss, EmploymentRole.MedicalWorker);
				return;
			case "orderly":
			case "nurse":
				HospitalDirectHire(actor, ss, EmploymentRole.HospitalOrderly);
				return;
			case "fire":
				HospitalFire(actor, ss);
				return;
			case "manager":
				HospitalDirectRoleToggle(actor, ss, EmploymentRole.Manager);
				return;
			case "proprietor":
				HospitalDirectRoleToggle(actor, ss, EmploymentRole.Proprietor);
				return;
			case "create" when actor.IsAdministrator():
				HospitalCreate(actor, ss);
				return;
			case "delete" when actor.IsAdministrator():
				HospitalDelete(actor);
				return;
			case "list" when ss.PeekSpeech().EqualTo("all") && actor.IsAdministrator():
				ss.PopSpeech();
				HospitalListAll(actor);
				return;
		}

		var hospital = CurrentHospital(actor);
		if (new EmploymentCommandService().TryExecuteShortcut(actor, hospital, "hospital", subcommand, ss))
		{
			return;
		}

		actor.OutputHandler.Send(HospitalHelpFor(actor).SubstituteANSIColour());
	}

	private static string HospitalHelpFor(ICharacter actor)
	{
		return actor.IsAdministrator() || CanSeeHospitalManagerHelp(actor) ? HospitalHelp : HospitalPlayerHelp;
	}

	private static bool CanSeeHospitalManagerHelp(ICharacter actor)
	{
		var hospital = CurrentHospital(actor);
		return EmploymentCommandService.CanViewManagerAliasHelp(actor, hospital,
			hospital?.IsManager(actor) == true || hospital?.IsProprietor(actor) == true);
	}

	private static IHospital? CurrentHospital(ICharacter actor)
	{
		var location = actor.Location;
		return location is null
			? null
			: actor.Gameworld.Hospitals.FirstOrDefault(x => x.Locations.Any(y => y.Id == location.Id));
	}

	private static bool DoHospitalCommandFindHospital(ICharacter actor, out IHospital hospital)
	{
		hospital = CurrentHospital(actor)!;
		if (hospital is not null)
		{
			return true;
		}

		actor.OutputHandler.Send("You are not currently at a hospital.");
		return false;
	}

	private static bool RequireHospitalManager(ICharacter actor, IHospital hospital)
	{
		if (actor.IsAdministrator() || hospital.IsManager(actor) || hospital.IsProprietor(actor))
		{
			return true;
		}

		actor.OutputHandler.Send("Only hospital managers, proprietors, and administrators can do that.");
		return false;
	}

	private static void HospitalInfo(ICharacter actor)
	{
		if (!DoHospitalCommandFindHospital(actor, out var hospital))
		{
			return;
		}

		actor.OutputHandler.Send(hospital.IsManager(actor) || hospital.IsProprietor(actor) || actor.IsAdministrator()
			? hospital.Show(actor)
			: hospital.ShowToNonEmployee(actor));
	}

	private static void HospitalServices(ICharacter actor)
	{
		if (!DoHospitalCommandFindHospital(actor, out var hospital))
		{
			return;
		}

		var services = hospital.ActiveServices.OrderBy(x => x.SortOrder).ThenBy(x => x.Name).ToList();
		if (!services.Any())
		{
			actor.OutputHandler.Send($"{hospital.Name.ColourName()} is not offering any services right now.");
			return;
		}

		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			services.Select(x => new List<string>
			{
				x.Id.ToString("N0", actor),
				x.Name,
				x.ServiceType.DescribeEnum(),
				HospitalServiceBilling.DescribePrice(hospital, x, actor),
				x.AllowDebt.ToColouredString(),
				HospitalServiceAvailability.Evaluate(hospital, x, actor).DescribeColoured()
			}),
			new List<string> { "Id", "Service", "Type", "Price", "Debt", "Status" },
			actor,
			Telnet.Green,
			2));
	}

	private static void HospitalServiceCommand(ICharacter actor, StringStack ss)
	{
		if (!DoHospitalCommandFindHospital(actor, out var hospital))
		{
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which hospital service do you want to show or edit?");
			return;
		}

		var action = ss.PopSpeech().CollapseString().ToLowerInvariant();
		if (action.EqualTo("add"))
		{
			HospitalServiceAdd(actor, hospital, ss);
			return;
		}

		if (action.EqualTo("set"))
		{
			HospitalServiceSet(actor, hospital, ss);
			return;
		}

		var service = hospital.ServiceByIdOrName(action);
		if (service is null)
		{
			actor.OutputHandler.Send($"There is no hospital service matching {action.ColourCommand()}.");
			return;
		}

		actor.OutputHandler.Send(service.Show(actor));
	}

	private static void HospitalServiceAdd(ICharacter actor, IHospital hospital, StringStack ss)
	{
		if (!RequireHospitalManager(actor, hospital))
		{
			return;
		}

		if (ss.IsFinished || !TryParseHospitalServiceType(ss.PopSpeech(), out var serviceType))
		{
			actor.OutputHandler.Send($"What kind of hospital service do you want to add? Valid types are {Enum.GetValues<HospitalServiceType>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return;
		}

		if (ss.IsFinished || !hospital.Currency.TryGetBaseCurrency(ss.PopSpeech(), out var price) || price < 0.0M)
		{
			actor.OutputHandler.Send($"What non-negative price should this service have in {hospital.Currency.Name.ColourName()}?");
			return;
		}

		if (HospitalServiceBilling.IsUsageBilledServiceType(serviceType))
		{
			actor.OutputHandler.Send($"{serviceType.DescribeEnum().ColourName()} is automatically available and is billed from the individual treatments performed. Edit the automatic service rather than adding another one.");
			return;
		}

		var name = ss.SafeRemainingArgument.TitleCase();
		if (string.IsNullOrWhiteSpace(name))
		{
			actor.OutputHandler.Send("What name should this service have?");
			return;
		}

		var service = new HospitalService(hospital, name, serviceType, price);
		hospital.AddService(service);
		actor.OutputHandler.Send($"You add the hospital service {service.Name.ColourName()} for {hospital.Currency.Describe(service.Price, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}.");
	}

	private static void HospitalServiceSet(ICharacter actor, IHospital hospital, StringStack ss)
	{
		if (!RequireHospitalManager(actor, hospital))
		{
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which hospital service do you want to edit?");
			return;
		}

		var service = hospital.ServiceByIdOrName(ss.PopSpeech());
		if (service is null)
		{
			actor.OutputHandler.Send("There is no such hospital service.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which property do you want to set? Options are price, active, debt, theatre, recovery, blood, equipment, procedure, implant, implantpower, implantinterface, anesthesia, cannulation, parameters, name, and desc.");
			return;
		}

		switch (ss.PopSpeech().CollapseString().ToLowerInvariant())
		{
			case "price":
				if (HospitalServiceBilling.IsUsageBilledServiceType(service.ServiceType))
				{
					actor.OutputHandler.Send($"{service.Name.ColourName()} is usage-billed from the individual treatments performed; set the prices on those component services instead.");
					return;
				}

				if (ss.IsFinished || !hospital.Currency.TryGetBaseCurrency(ss.SafeRemainingArgument, out var price) || price < 0.0M)
				{
					actor.OutputHandler.Send("What non-negative price should this service have?");
					return;
				}
				service.Price = price;
				service.Changed = true;
				actor.OutputHandler.Send($"{service.Name.ColourName()} now costs {hospital.Currency.Describe(price, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}.");
				return;
			case "active":
				service.IsActive = !service.IsActive;
				service.Changed = true;
				actor.OutputHandler.Send($"{service.Name.ColourName()} active status is now {service.IsActive.ToColouredString()}.");
				return;
			case "debt":
			case "allowdebt":
				if (HospitalServiceBilling.IsUsageBilledServiceType(service.ServiceType))
				{
					service.AllowDebt = true;
					service.Changed = true;
					actor.OutputHandler.Send($"{service.Name.ColourName()} is usage-billed and must remain debt-capable.");
					return;
				}

				service.AllowDebt = !service.AllowDebt;
				service.Changed = true;
				actor.OutputHandler.Send($"{service.Name.ColourName()} debt status is now {service.AllowDebt.ToColouredString()}.");
				return;
			case "theatre":
			case "theater":
				service.PreferOperatingTheatre = !service.PreferOperatingTheatre;
				service.Changed = true;
				actor.OutputHandler.Send($"{service.Name.ColourName()} operating-theatre preference is now {service.PreferOperatingTheatre.ToColouredString()}.");
				return;
			case "recovery":
				service.RequiresRecovery = !service.RequiresRecovery;
				service.Changed = true;
				actor.OutputHandler.Send($"{service.Name.ColourName()} recovery routing is now {service.RequiresRecovery.ToColouredString()}.");
				return;
			case "blood":
			case "bloodvolume":
				HospitalServiceSetBlood(actor, service, ss);
				return;
			case "equipment":
			case "equip":
				HospitalServiceSetEquipment(actor, service, ss);
				return;
			case "procedure":
				HospitalServiceSetProcedure(actor, service, ss);
				return;
			case "implant":
				HospitalServiceSetImplant(actor, service, ss);
				return;
			case "implantpower":
			case "powerprocedure":
			case "power":
				HospitalServiceSetImplantPowerProcedure(actor, service, ss);
				return;
			case "implantinterface":
			case "interfaceprocedure":
			case "interface":
				HospitalServiceSetImplantInterfaceProcedure(actor, service, ss);
				return;
			case "anesthesia":
			case "anaesthesia":
			case "anesthetic":
			case "anaesthetic":
				HospitalServiceSetAnesthesia(actor, service, ss);
				return;
			case "cannulation":
			case "anesthesiacannulation":
			case "anaesthesiacannulation":
			case "anesthesiadrip":
			case "anaesthesiadrip":
				HospitalServiceSetAnesthesiaCannulationProcedure(actor, service, ss);
				return;
			case "anesthesiaintensity":
			case "anaesthesiaintensity":
			case "anesthesiadose":
			case "anaesthesiadose":
				HospitalServiceSetAnesthesiaIntensity(actor, service, ss);
				return;
			case "parameters":
			case "params":
				service.ProcedureParameters = ss.SafeRemainingArgument;
				service.Changed = true;
				actor.OutputHandler.Send($"{service.Name.ColourName()} procedure parameters are now {service.ProcedureParameters.ColourCommand()}.");
				return;
			case "name":
				var name = ss.SafeRemainingArgument.TitleCase();
				if (string.IsNullOrWhiteSpace(name))
				{
					actor.OutputHandler.Send("What new name should this service have?");
					return;
				}
				service.Rename(name);
				actor.OutputHandler.Send($"The service is now named {service.Name.ColourName()}.");
				return;
			case "desc":
			case "description":
				service.Description = ss.SafeRemainingArgument;
				service.Changed = true;
				actor.OutputHandler.Send($"{service.Name.ColourName()} now has description:\n{service.Description}");
				return;
			default:
				actor.OutputHandler.Send("Unknown hospital service setting.");
				return;
		}
	}

	private static void HospitalServiceSetBlood(ICharacter actor, IHospitalService service, StringStack ss)
	{
		if (ss.IsFinished || !double.TryParse(ss.PopSpeech(), System.Globalization.NumberStyles.Any, actor, out var amount) || amount <= 0.0)
		{
			actor.OutputHandler.Send("What positive blood volume in litres should this service use?");
			return;
		}

		service.BloodVolumeLitres = amount;
		service.Changed = true;
		actor.OutputHandler.Send($"{service.Name.ColourName()} will use {amount.ToString("N2", actor).ColourValue()}L for blood donation/transfusion procedures.");
	}

	private static void HospitalServiceSetEquipment(ICharacter actor, IHospitalService service, StringStack ss)
	{
		if (ss.IsFinished || ss.PeekSpeech().EqualToAny("show", "list"))
		{
			if (!ss.IsFinished)
			{
				ss.PopSpeech();
			}

			ShowServiceEquipment(actor, service);
			return;
		}

		switch (ss.PopSpeech().CollapseString().ToLowerInvariant())
		{
			case "clear":
			case "none":
				service.ClearRequiredEquipment();
				actor.OutputHandler.Send($"{service.Name.ColourName()} no longer requires prepared equipment.");
				return;
			case "remove":
			case "delete":
				if (ss.IsFinished || !int.TryParse(ss.PopSpeech(), out var index) || index < 1 || index > service.RequiredEquipment.Count)
				{
					actor.OutputHandler.Send("Which equipment requirement number do you want to remove?");
					return;
				}

				service.RemoveRequiredEquipmentAt(index - 1);
				actor.OutputHandler.Send($"You remove equipment requirement #{index.ToString("N0", actor).ColourValue()} from {service.Name.ColourName()}.");
				return;
			case "add":
				if (ss.IsFinished || !int.TryParse(ss.PopSpeech(), out var quantity) || quantity < 1)
				{
					actor.OutputHandler.Send("How many matching items should be prepared?");
					return;
				}

				if (!EmploymentTaskAuthoringService.TryParseItemSelector(actor, ss, "hospital service equipment", out var selector, out var message) || selector is null)
				{
					actor.OutputHandler.Send(message);
					return;
				}

				service.AddRequiredEquipment(new HospitalServiceEquipmentRequirement(quantity, selector));
				actor.OutputHandler.Send($"{service.Name.ColourName()} now requires {quantity.ToString("N0", actor).ColourValue()}x {EmploymentItemSelectorResolver.Describe(selector).ColourCommand()} to be prepared.");
				return;
			default:
				actor.OutputHandler.Send("Hospital service equipment syntax is equipment show|clear|remove <#>|add <quantity> <prototype id|*item id|&tag|keyword>.");
				return;
		}
	}

	private static void ShowServiceEquipment(ICharacter actor, IHospitalService service)
	{
		if (!service.RequiredEquipment.Any())
		{
			actor.OutputHandler.Send($"{service.Name.ColourName()} has no required equipment.");
			return;
		}

		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			service.RequiredEquipment.Select((x, i) => new List<string>
			{
				(i + 1).ToString("N0", actor),
				x.Quantity.ToString("N0", actor),
				EmploymentItemSelectorResolver.Describe(x.Selector)
			}),
			new List<string> { "#", "Qty", "Selector" },
			actor,
			Telnet.Green,
			2));
	}
	private static void HospitalServiceSetProcedure(ICharacter actor, IHospitalService service, StringStack ss)
	{
		if (ss.IsFinished || ss.SafeRemainingArgument.EqualToAny("none", "clear", "remove"))
		{
			service.SurgicalProcedure = null;
			service.Changed = true;
			actor.OutputHandler.Send($"{service.Name.ColourName()} no longer has a surgical procedure configured.");
			return;
		}

		var procedure = actor.Gameworld.SurgicalProcedures.GetByIdOrName(ss.SafeRemainingArgument);
		if (procedure is null)
		{
			actor.OutputHandler.Send("There is no such surgical procedure.");
			return;
		}

		service.SurgicalProcedure = procedure;
		service.Changed = true;
		actor.OutputHandler.Send($"{service.Name.ColourName()} now uses surgical procedure {procedure.ProcedureName.ColourName()}.");
	}

	private static void HospitalServiceSetImplant(ICharacter actor, IHospitalService service, StringStack ss)
	{
		if (ss.IsFinished || ss.SafeRemainingArgument.EqualToAny("none", "clear", "remove"))
		{
			service.ImplantItemPrototype = null;
			service.Changed = true;
			actor.OutputHandler.Send($"{service.Name.ColourName()} no longer supplies an implant item.");
			return;
		}

		var proto = actor.Gameworld.ItemProtos.GetByIdOrName(ss.SafeRemainingArgument);
		if (proto is null)
		{
			actor.OutputHandler.Send("There is no such item prototype.");
			return;
		}

		service.ImplantItemPrototype = proto;
		service.Changed = true;
		actor.OutputHandler.Send($"{service.Name.ColourName()} now supplies {proto.Name.ColourName()} for implant procedures.");
	}

	private static void HospitalServiceSetImplantPowerProcedure(ICharacter actor, IHospitalService service, StringStack ss)
	{
		HospitalServiceSetImplantFollowup(actor, service, ss, SurgicalProcedureType.ConfigureImplantPower,
			"implant power", procedure => service.ImplantPowerProcedure = procedure);
	}

	private static void HospitalServiceSetImplantInterfaceProcedure(ICharacter actor, IHospitalService service, StringStack ss)
	{
		HospitalServiceSetImplantFollowup(actor, service, ss, SurgicalProcedureType.ConfigureImplantInterface,
			"implant interface", procedure => service.ImplantInterfaceProcedure = procedure);
	}

	private static void HospitalServiceSetImplantFollowup(ICharacter actor, IHospitalService service, StringStack ss,
		SurgicalProcedureType procedureType, string description, Action<ISurgicalProcedure?> setter)
	{
		if (ss.IsFinished || ss.SafeRemainingArgument.EqualToAny("none", "clear", "remove"))
		{
			setter(null);
			service.Changed = true;
			actor.OutputHandler.Send($"{service.Name.ColourName()} no longer has an {description} follow-up procedure configured.");
			return;
		}

		var procedure = actor.Gameworld.SurgicalProcedures.GetByIdOrName(ss.SafeRemainingArgument);
		if (procedure is null)
		{
			actor.OutputHandler.Send("There is no such surgical procedure.");
			return;
		}

		if (procedure.Procedure != procedureType)
		{
			actor.OutputHandler.Send($"{procedure.ProcedureName.ColourName()} is {procedure.Procedure.DescribeEnum().ColourValue()}, not {procedureType.DescribeEnum().ColourValue()}.");
			return;
		}

		setter(procedure);
		service.Changed = true;
		actor.OutputHandler.Send($"{service.Name.ColourName()} now uses {procedure.ProcedureName.ColourName()} as its {description} follow-up procedure.");
	}

	private static void HospitalServiceSetAnesthesia(ICharacter actor, IHospitalService service, StringStack ss)
	{
		if (ss.IsFinished || ss.PeekSpeech().EqualToAny("show", "list"))
		{
			actor.OutputHandler.Send(service.AnesthesiaDrug is null
				? $"{service.Name.ColourName()} has no anesthesia drug configured. Cannulation: {(service.AnesthesiaCannulationProcedure is null ? "none".ColourError() : service.AnesthesiaCannulationProcedure.ProcedureName.ColourName())}."
				: $"{service.Name.ColourName()} uses {service.AnesthesiaDrug.Name.ColourName()} at intensity {service.AnesthesiaIntensity.ToString("N2", actor).ColourValue()}. Cannulation: {(service.AnesthesiaCannulationProcedure is null ? "none".ColourError() : service.AnesthesiaCannulationProcedure.ProcedureName.ColourName())}.");
			return;
		}

		var token = ss.PopSpeech();
		if (token.EqualToAny("none", "clear", "remove"))
		{
			service.AnesthesiaDrug = null;
			service.Changed = true;
			actor.OutputHandler.Send($"{service.Name.ColourName()} no longer has an anesthesia drug configured.");
			return;
		}

		var drug = actor.Gameworld.Drugs.GetByIdOrName(token);
		if (drug is null)
		{
			actor.OutputHandler.Send("There is no such drug.");
			return;
		}

		if (!drug.DrugTypes.Contains(DrugType.Anesthesia) || !drug.DrugVectors.HasFlag(DrugVector.Injected))
		{
			actor.OutputHandler.Send($"{drug.Name.ColourName()} must have the {DrugType.Anesthesia.DescribeEnum().ColourValue()} effect and the {DrugVector.Injected.Describe().ColourValue()} vector.");
			return;
		}

		service.AnesthesiaDrug = drug;
		if (!ss.IsFinished)
		{
			if (!double.TryParse(ss.PopSpeech(), System.Globalization.NumberStyles.Any, actor, out var intensity) ||
			    intensity <= 0.0 || intensity >= 2.5)
			{
				actor.OutputHandler.Send("What anesthesia intensity should be targeted? Use a value greater than 0 and less than 2.5.");
				return;
			}

			service.AnesthesiaIntensity = intensity;
		}

		service.Changed = true;
		actor.OutputHandler.Send($"{service.Name.ColourName()} will use {drug.Name.ColourName()} for surgery anesthesia at intensity {service.AnesthesiaIntensity.ToString("N2", actor).ColourValue()}.");
	}

	private static void HospitalServiceSetAnesthesiaCannulationProcedure(ICharacter actor, IHospitalService service, StringStack ss)
	{
		if (ss.IsFinished || ss.SafeRemainingArgument.EqualToAny("none", "clear", "remove"))
		{
			service.AnesthesiaCannulationProcedure = null;
			service.Changed = true;
			actor.OutputHandler.Send($"{service.Name.ColourName()} will use direct injected anesthesia rather than a cannulation/drip setup.");
			return;
		}

		var procedure = actor.Gameworld.SurgicalProcedures.GetByIdOrName(ss.SafeRemainingArgument);
		if (procedure is null)
		{
			actor.OutputHandler.Send("There is no such surgical procedure.");
			return;
		}

		if (procedure.Procedure != SurgicalProcedureType.Cannulation)
		{
			actor.OutputHandler.Send($"{procedure.ProcedureName.ColourName()} is {procedure.Procedure.DescribeEnum().ColourValue()}, not {SurgicalProcedureType.Cannulation.DescribeEnum().ColourValue()}.");
			return;
		}

		service.AnesthesiaCannulationProcedure = procedure;
		service.Changed = true;
		actor.OutputHandler.Send($"{service.Name.ColourName()} will cannulate patients with {procedure.ProcedureName.ColourName()} before starting IV anesthesia.");
	}

	private static void HospitalServiceSetAnesthesiaIntensity(ICharacter actor, IHospitalService service, StringStack ss)
	{
		if (ss.IsFinished || !double.TryParse(ss.PopSpeech(), System.Globalization.NumberStyles.Any, actor, out var intensity) ||
		    intensity <= 0.0 || intensity >= 2.5)
		{
			actor.OutputHandler.Send("What anesthesia intensity should be targeted? Use a value greater than 0 and less than 2.5.");
			return;
		}

		service.AnesthesiaIntensity = intensity;
		service.Changed = true;
		actor.OutputHandler.Send($"{service.Name.ColourName()} will target anesthesia intensity {intensity.ToString("N2", actor).ColourValue()}.");
	}

	private static void HospitalBloodStock(ICharacter actor, StringStack ss)
	{
		if (!DoHospitalCommandFindHospital(actor, out var hospital) || !RequireHospitalManager(actor, hospital))
		{
			return;
		}

		if (ss.IsFinished || ss.PeekSpeech().EqualToAny("show", "list"))
		{
			if (!ss.IsFinished)
			{
				ss.PopSpeech();
			}

			if (ss.IsFinished)
			{
				ShowHospitalBloodStocks(actor, hospital);
				return;
			}

			if (!TryGetHospitalBloodtype(actor, ss.SafeRemainingArgument, out var bloodtype))
			{
				return;
			}

			var policy = hospital.BloodStockPolicyFor(bloodtype, false);
			actor.OutputHandler.Send(policy?.Show(actor) ??
				$"{hospital.Name.ColourName()} has no blood stock policy for {bloodtype.Name.ColourName()}. Current stock is {HospitalMedicalServiceRunner.CurrentBloodStockLitres(hospital, bloodtype).ToString("N2", actor).ColourValue()}L.");
			return;
		}

		switch (ss.PopSpeech().CollapseString().ToLowerInvariant())
		{
			case "clear":
			case "remove":
			case "delete":
				HospitalBloodStockClear(actor, hospital, ss);
				return;
			case "set":
				HospitalBloodStockSet(actor, hospital, ss);
				return;
			default:
				actor.OutputHandler.Send("Hospital bloodstock syntax is bloodstock show [bloodtype], bloodstock set <bloodtype|all> target <litres>, bloodstock set <bloodtype|all> price <amount>, or bloodstock clear <bloodtype|all>.");
				return;
		}
	}

	private static void ShowHospitalBloodStocks(ICharacter actor, IHospital hospital)
	{
		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			actor.Gameworld.Bloodtypes.OrderBy(x => x.Name).Select(x =>
			{
				var policy = hospital.BloodStockPolicyFor(x, false);
				return new List<string>
				{
					x.Name,
					HospitalMedicalServiceRunner.CurrentBloodStockLitres(hospital, x).ToString("N2", actor) + "L",
					(policy?.TargetLitres ?? 0.0).ToString("N2", actor) + "L",
					hospital.Currency.Describe(policy?.PricePerLitre ?? 0.0M, CurrencyDescriptionPatternType.ShortDecimal)
				};
			}),
			new List<string> { "Blood", "Stock", "Target", "Price/L" },
			actor,
			Telnet.Green,
			2));
	}

	private static void HospitalBloodStockSet(ICharacter actor, IHospital hospital, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which blood type policy do you want to set?");
			return;
		}

		var targetText = ss.PopSpeech();
		var bloodtypes = new List<IBloodtype>();
		if (targetText.EqualTo("all"))
		{
			bloodtypes.AddRange(actor.Gameworld.Bloodtypes);
		}
		else if (TryGetHospitalBloodtype(actor, targetText, out var bloodtype))
		{
			bloodtypes.Add(bloodtype);
		}
		else
		{
			return;
		}

		if (!bloodtypes.Any())
		{
			actor.OutputHandler.Send("There are no blood types configured.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Do you want to set target or price?");
			return;
		}

		switch (ss.PopSpeech().CollapseString().ToLowerInvariant())
		{
			case "target":
			case "stock":
			case "level":
				if (ss.IsFinished || !double.TryParse(ss.PopSpeech(), System.Globalization.NumberStyles.Any, actor, out var litres) || litres < 0.0)
				{
					actor.OutputHandler.Send("What non-negative target stock in litres should this blood type have?");
					return;
				}

				foreach (var type in bloodtypes)
				{
					hospital.BloodStockPolicyFor(type, true)!.TargetLitres = litres;
				}

				actor.OutputHandler.Send(targetText.EqualTo("all")
					? $"{hospital.Name.ColourName()} will target {litres.ToString("N2", actor).ColourValue()}L of every blood type."
					: $"{hospital.Name.ColourName()} will target {litres.ToString("N2", actor).ColourValue()}L of {bloodtypes[0].Name.ColourName()} blood.");
				return;
			case "price":
			case "pay":
			case "rate":
				if (ss.IsFinished || !hospital.Currency.TryGetBaseCurrency(ss.SafeRemainingArgument, out var price) || price < 0.0M)
				{
					actor.OutputHandler.Send("What non-negative donor price per litre should this blood type have?");
					return;
				}

				foreach (var type in bloodtypes)
				{
					hospital.BloodStockPolicyFor(type, true)!.PricePerLitre = price;
				}

				actor.OutputHandler.Send(targetText.EqualTo("all")
					? $"{hospital.Name.ColourName()} will pay {hospital.Currency.Describe(price, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} per litre for every blood type while below target."
					: $"{hospital.Name.ColourName()} will pay {hospital.Currency.Describe(price, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} per litre for {bloodtypes[0].Name.ColourName()} blood while below target.");
				return;
			default:
				actor.OutputHandler.Send("Do you want to set target or price?");
				return;
		}
	}

	private static void HospitalBloodStockClear(ICharacter actor, IHospital hospital, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which blood type policy do you want to clear?");
			return;
		}

		var targetText = ss.SafeRemainingArgument;
		if (targetText.EqualTo("all"))
		{
			var policies = hospital.BloodStockPolicies.ToList();
			foreach (var bloodPolicy in policies)
			{
				hospital.RemoveBloodStockPolicy(bloodPolicy);
			}

			actor.OutputHandler.Send($"You clear all blood stock policies at {hospital.Name.ColourName()}.");
			return;
		}

		if (!TryGetHospitalBloodtype(actor, targetText, out var bloodtype))
		{
			return;
		}

		var policy = hospital.BloodStockPolicyFor(bloodtype, false);
		if (policy is null)
		{
			actor.OutputHandler.Send($"{hospital.Name.ColourName()} has no blood stock policy for {bloodtype.Name.ColourName()}.");
			return;
		}

		hospital.RemoveBloodStockPolicy(policy);
		actor.OutputHandler.Send($"You clear the blood stock policy for {bloodtype.Name.ColourName()} at {hospital.Name.ColourName()}.");
	}

	private static bool TryGetHospitalBloodtype(ICharacter actor, string text, out IBloodtype bloodtype)
	{
		bloodtype = actor.Gameworld.Bloodtypes.GetByIdOrName(text)!;
		if (bloodtype is not null)
		{
			return true;
		}

		actor.OutputHandler.Send("There is no such blood type.");
		return false;
	}
	private static void HospitalCash(ICharacter actor, StringStack ss)
	{
		if (!DoHospitalCommandFindHospital(actor, out var hospital) || !RequireHospitalManager(actor, hospital))
		{
			return;
		}

		if (ss.IsFinished)
		{
			HospitalCashBalance(actor, hospital);
			return;
		}

		switch (ss.PopSpeech().CollapseString().ToLowerInvariant())
		{
			case "balance":
			case "bal":
			case "show":
				HospitalCashBalance(actor, hospital);
				return;
			case "deposit":
			case "dep":
				HospitalCashDeposit(actor, ss);
				return;
			case "withdraw":
			case "with":
				HospitalCashWithdraw(actor, ss);
				return;
			case "ledger":
			case "history":
				HospitalCashLedger(actor, ss);
				return;
			default:
				actor.OutputHandler.Send("Hospital cash syntax is cash [balance|deposit <amount>|withdraw <amount>|ledger [count]].");
				return;
		}
	}

	private static void HospitalCashBalance(ICharacter actor, IHospital hospital)
	{
		actor.OutputHandler.Send(
			$"{hospital.Name.ColourName()} has {hospital.Currency.Describe(hospital.CashBalance, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} in virtual cash and {hospital.Currency.Describe(hospital.AvailableFunds, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} total available funds.");
	}

	private static void HospitalCashDeposit(ICharacter actor, StringStack ss)
	{
		if (!DoHospitalCommandFindHospital(actor, out var hospital) || !RequireHospitalManager(actor, hospital))
		{
			return;
		}

		if (ss.IsFinished || !hospital.Currency.TryGetBaseCurrency(ss.SafeRemainingArgument, out var amount) || amount <= 0.0M)
		{
			actor.OutputHandler.Send($"How much {hospital.Currency.Name.ColourName()} do you want to deposit?");
			return;
		}

		if (AccessibleHospitalCash(actor, hospital.Currency) < amount)
		{
			actor.OutputHandler.Send(
				$"You do not have {hospital.Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} in accessible cash.");
			return;
		}

		TakeHospitalCash(actor, hospital, amount, "Hospital manager cash deposit");
		actor.OutputHandler.Send(
			$"You deposit {hospital.Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} into {hospital.Name.ColourName()}'s virtual cash balance.");
	}

	private static void HospitalCashWithdraw(ICharacter actor, StringStack ss)
	{
		if (!DoHospitalCommandFindHospital(actor, out var hospital) || !RequireHospitalManager(actor, hospital))
		{
			return;
		}

		if (ss.IsFinished || !hospital.Currency.TryGetBaseCurrency(ss.SafeRemainingArgument, out var amount) || amount <= 0.0M)
		{
			actor.OutputHandler.Send($"How much {hospital.Currency.Name.ColourName()} do you want to withdraw?");
			return;
		}

		if (!VirtualCashLedger.Debit(hospital, hospital.Currency, amount, actor, actor, "CashWithdrawal",
			    "Hospital manager cash withdrawal", null,
			    hospital.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime, out var error))
		{
			actor.OutputHandler.Send(error);
			return;
		}

		var cash = CurrencyGameItemComponentProto.CreateNewCurrencyPile(hospital.Currency,
			hospital.Currency.FindCoinsForAmount(amount, out _));
		if (actor.Body.CanGet(cash, 0))
		{
			actor.Body.Get(cash, silent: true);
		}
		else
		{
			cash.RoomLayer = actor.RoomLayer;
			actor.Location.Insert(cash, true);
			actor.OutputHandler.Send("You couldn't hold the money, so it is on the ground.");
		}

		actor.OutputHandler.Send(
			$"You withdraw {hospital.Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} from {hospital.Name.ColourName()}'s virtual cash balance.");
	}

	private static void HospitalCashLedger(ICharacter actor, StringStack ss)
	{
		if (!DoHospitalCommandFindHospital(actor, out var hospital) || !RequireHospitalManager(actor, hospital))
		{
			return;
		}

		var count = 25;
		if (!ss.IsFinished && (!int.TryParse(ss.SafeRemainingArgument, out count) || count <= 0))
		{
			actor.OutputHandler.Send("How many ledger entries do you want to review?");
			return;
		}

		count = VirtualCashLedger.ClampLedgerEntryCount(count);
		var entries = VirtualCashLedger.LedgerEntries(hospital, count).ToList();
		if (!entries.Any())
		{
			actor.OutputHandler.Send($"{hospital.Name.ColourName()} does not have any cash ledger entries.");
			return;
		}

		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			entries.Select(x => new List<string>
			{
				x.RealDateTime.ToString("g", actor),
				x.ActorName ?? string.Empty,
				hospital.Currency.Describe(x.Amount, CurrencyDescriptionPatternType.ShortDecimal),
				hospital.Currency.Describe(x.BalanceAfter, CurrencyDescriptionPatternType.ShortDecimal),
				$"{x.SourceKind}->{x.DestinationKind}",
				x.Reason
			}),
			new List<string> { "When", "Actor", "Amount", "Balance", "Route", "Reason" },
			actor.LineFormatLength,
			colour: Telnet.Green,
			unicodeTable: actor.Account.UseUnicode));
	}
	private sealed class HospitalPaymentSelection
	{
		public HospitalPaymentMethod Method { get; set; }
		public IBankPaymentItem? PaymentItem { get; init; }
	}

	private static void HospitalRequest(ICharacter actor, StringStack ss)
	{
		if (!DoHospitalCommandFindHospital(actor, out var hospital))
		{
			return;
		}

		if (!hospital.IsTrading)
		{
			actor.OutputHandler.Send($"{hospital.Name.ColourName()} is not presently accepting patients.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which hospital service do you want to request?");
			return;
		}

		var service = hospital.ServiceByIdOrName(ss.PopSpeech());
		if (service is null || !service.IsActive)
		{
			actor.OutputHandler.Send("There is no active hospital service like that.");
			return;
		}

		var patient = actor;
		var payment = new HospitalPaymentSelection { Method = HospitalPaymentMethod.Cash };
		if (HospitalServiceBilling.IsUsageBilledServiceType(service.ServiceType))
		{
			payment = new HospitalPaymentSelection { Method = HospitalPaymentMethod.Debt };
		}

		while (!ss.IsFinished)
		{
			var token = ss.PopSpeech();
			if (token.EqualTo("for"))
			{
				if (ss.IsFinished)
				{
					actor.OutputHandler.Send("Who is the patient?");
					return;
				}

				patient = actor.TargetActor(ss.PopSpeech());
				if (patient is null)
				{
					actor.OutputHandler.Send("You do not see that patient here.");
					return;
				}

				continue;
			}

			if (token.EqualToAny("debt", "account"))
			{
				payment = new HospitalPaymentSelection { Method = HospitalPaymentMethod.Debt };
				continue;
			}

			if (token.EqualTo("cash"))
			{
				payment = new HospitalPaymentSelection { Method = HospitalPaymentMethod.Cash };
				continue;
			}

			if (token.EqualTo("with"))
			{
				if (ss.IsFinished)
				{
					actor.OutputHandler.Send("Which payment item do you want to use?");
					return;
				}

				var item = actor.TargetItem(ss.SafeRemainingArgument);
				var paymentItem = item?.GetItemType<IBankPaymentItem>();
				if (paymentItem is null)
				{
					actor.OutputHandler.Send("You do not have any such bank payment item.");
					return;
				}

				payment = new HospitalPaymentSelection { Method = HospitalPaymentMethod.BankPaymentItem, PaymentItem = paymentItem };
				break;
			}

			if (token.EqualToAny("waive", "free"))
			{
				if (!RequireHospitalManager(actor, hospital))
				{
					return;
				}

				payment = new HospitalPaymentSelection { Method = HospitalPaymentMethod.Waived };
				continue;
			}

			actor.OutputHandler.Send($"Unknown hospital request option {token.ColourCommand()}.");
			return;
		}

		if (HospitalServiceBilling.IsUsageBilledServiceType(service.ServiceType) &&
		    payment.Method != HospitalPaymentMethod.Waived)
		{
			payment = new HospitalPaymentSelection { Method = HospitalPaymentMethod.Debt };
		}

		if (payment.Method == HospitalPaymentMethod.Debt && !service.AllowDebt)
		{
			actor.OutputHandler.Send($"{service.Name.ColourName()} cannot be charged to medical debt.");
			return;
		}

		void CreateRequest()
		{
			if (!TryCreateHospitalRequest(actor, hospital, service, patient, payment, out var message))
			{
				actor.OutputHandler.Send(message);
				return;
			}

			actor.OutputHandler.Send(message);
		}

		if (!CharacterInstanceIdentityComparer.SamePhysicalInstance(actor, patient) && !patient.IsHelpless)
		{
			patient.AddEffect(new Accept(patient, new GenericProposal(
				text =>
				{
					patient.OutputHandler.Send($"You agree to receive treatment from {hospital.Name.ColourName()}.");
					CreateRequest();
				},
				text =>
				{
					patient.OutputHandler.Send($"You decline treatment from {hospital.Name.ColourName()}.");
				},
				() =>
				{
					patient.OutputHandler.Send($"You decline treatment from {hospital.Name.ColourName()}.");
				},
				$"requesting {service.Name}", "hospital", "treatment")), TimeSpan.FromSeconds(120));
			actor.OutputHandler.Send($"You request hospital treatment for {patient.HowSeen(actor)}.");
			patient.OutputHandler.Send(Accept.StandardAcceptPhrasing);
			return;
		}

		CreateRequest();
	}

	private static bool TryCreateHospitalRequest(ICharacter requester, IHospital hospital, IHospitalService service,
		ICharacter patient, HospitalPaymentSelection payment, out string message)
	{
		var availability = HospitalServiceAvailability.Evaluate(hospital, service, requester, patient);
		if (!availability.Available)
		{
			message = $"{service.Name.ColourName()} is currently unavailable: {availability.Reason}.";
			return false;
		}

		var amountPaid = 0.0M;
		var debtCharged = 0.0M;
		if (!TryTakeHospitalPayment(requester, hospital, service, payment, patient, out amountPaid, out debtCharged, out message))
		{
			return false;
		}

		var request = new HospitalServiceRequest(hospital, service, requester, patient, payment.Method);
		request.MarkCharged(amountPaid, debtCharged);

		hospital.AddServiceRequest(request);
		var task = hospital.TaskBoard.CreateActiveTask(
			$"treat {request.PatientName}: {service.Name}",
			new EmploymentActionPlan(ServiceRequestActionSteps(hospital, request)),
			null,
			idempotencyKey: $"hospital-request:{request.Id.ToString("F0", requester)}",
			priority: 10);
		request.EmploymentTaskId = task.Id;
		request.MarkStatus(HospitalServiceRequestStatus.Queued,
			$"Queued as employment task {task.CorrelationId.ToString("D")}.");
		message = $"You request {service.Name.ColourName()} for {patient.HowSeen(requester)} at {hospital.Name.ColourName()}. Request #{request.Id.ToString("N0", requester).ColourValue()} has been queued.";
		if (HospitalServiceBilling.IsUsageBilledServiceType(service.ServiceType) && payment.Method != HospitalPaymentMethod.Waived)
		{
			message += " It will be charged to the patient's hospital debt account based on the treatments actually performed.";
		}

		return true;
	}

	private static IEnumerable<IEmploymentActionStep> ServiceRequestActionSteps(IHospital hospital, IHospitalServiceRequest request)
	{
		if (request.Service.RequiredEquipment.Any())
		{
			yield return new HospitalSupplyPreparationActionStep(hospital, request);
		}

		yield return new HospitalServiceActionStep(hospital, request);
	}
	private static bool TryTakeHospitalPayment(ICharacter actor, IHospital hospital, IHospitalService service,
		HospitalPaymentSelection payment, ICharacter patient, out decimal amountPaid, out decimal debtCharged, out string error)
	{
		amountPaid = 0.0M;
		debtCharged = 0.0M;
		error = string.Empty;
		var amount = service.Price;
		if (HospitalServiceBilling.IsUsageBilledServiceType(service.ServiceType))
		{
			if (payment.Method == HospitalPaymentMethod.Waived)
			{
				return true;
			}

			if (!service.AllowDebt)
			{
				error = $"{service.Name.ColourName()} is usage-billed and must allow medical debt.";
				return false;
			}

			var account = hospital.DebtAccountFor(patient, true)!;
			if (account.IsSuspended)
			{
				error = $"The hospital debt account for {account.PatientName.ColourName()} is suspended.";
				return false;
			}

			return true;
		}

		if (amount <= 0.0M || payment.Method == HospitalPaymentMethod.Waived)
		{
			return true;
		}

		switch (payment.Method)
		{
			case HospitalPaymentMethod.Debt:
				if (!service.AllowDebt)
				{
					error = $"{service.Name.ColourName()} cannot be charged to medical debt.";
					return false;
				}

				var account = hospital.DebtAccountFor(patient, true)!;
				if (!account.CanCharge(amount, out error))
				{
					return false;
				}

				account.Charge(amount, $"Hospital service {service.Name}");
				debtCharged = amount;
				return true;
			case HospitalPaymentMethod.Cash:
				if (AccessibleHospitalCash(actor, hospital.Currency) < amount)
				{
					error = $"You do not have {hospital.Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} in accessible cash.";
					return false;
				}

				TakeHospitalCash(actor, hospital, amount, $"Hospital service {service.Name}");
				amountPaid = amount;
				return true;
			case HospitalPaymentMethod.BankPaymentItem:
				if (payment.PaymentItem is null)
				{
					error = "Which payment item do you want to use?";
					return false;
				}

				if (payment.PaymentItem.BankAccount.Currency != hospital.Currency)
				{
					error = "That payment item is for the wrong currency.";
					return false;
				}

				if (!payment.PaymentItem.BankAccount.IsAuthorisedPaymentItem(payment.PaymentItem))
				{
					error = "That payment item is not authorised.";
					return false;
				}

				if (payment.PaymentItem.BankAccount.MaximumWithdrawal() < amount)
				{
					error = "That bank account does not have enough available funds.";
					return false;
				}

				payment.PaymentItem.BankAccount.WithdrawFromTransaction(amount, $"Hospital service {service.Name}");
				payment.PaymentItem.BankAccount.Bank.CurrencyReserves[hospital.Currency] -= amount;
				payment.PaymentItem.BankAccount.Bank.Changed = true;
				VirtualCashLedger.CreditBankOrVirtual(hospital, hospital.Currency, amount, actor, actor, "BankPaymentItem",
					$"Hospital service {service.Name}", hospital.BankAccount, hospital.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime);
				if (payment.PaymentItem.CurrentUsesRemaining > 0)
				{
					payment.PaymentItem.CurrentUsesRemaining--;
				}

				amountPaid = amount;
				return true;
		}

		error = "Unsupported hospital payment method.";
		return false;
	}

	private static bool TryTakeHospitalPaymentAmount(ICharacter actor, IHospital hospital, decimal amount,
		HospitalPaymentSelection payment, string reference, out string error)
	{
		error = string.Empty;
		if (amount <= 0.0M)
		{
			return true;
		}

		switch (payment.Method)
		{
			case HospitalPaymentMethod.Cash:
				if (AccessibleHospitalCash(actor, hospital.Currency) < amount)
				{
					error = $"You do not have {hospital.Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} in accessible cash.";
					return false;
				}

				TakeHospitalCash(actor, hospital, amount, reference);
				return true;
			case HospitalPaymentMethod.BankPaymentItem:
				if (payment.PaymentItem is null)
				{
					error = "Which payment item do you want to use?";
					return false;
				}

				if (payment.PaymentItem.BankAccount.Currency != hospital.Currency)
				{
					error = "That payment item is for the wrong currency.";
					return false;
				}

				if (!payment.PaymentItem.BankAccount.IsAuthorisedPaymentItem(payment.PaymentItem))
				{
					error = "That payment item is not authorised.";
					return false;
				}

				if (payment.PaymentItem.BankAccount.MaximumWithdrawal() < amount)
				{
					error = "That bank account does not have enough available funds.";
					return false;
				}

				payment.PaymentItem.BankAccount.WithdrawFromTransaction(amount, reference);
				payment.PaymentItem.BankAccount.Bank.CurrencyReserves[hospital.Currency] -= amount;
				payment.PaymentItem.BankAccount.Bank.Changed = true;
				VirtualCashLedger.CreditBankOrVirtual(hospital, hospital.Currency, amount, actor, actor, "BankPaymentItem",
					reference, hospital.BankAccount, hospital.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime);
				if (payment.PaymentItem.CurrentUsesRemaining > 0)
				{
					payment.PaymentItem.CurrentUsesRemaining--;
				}

				return true;
		}

		error = "You must pay hospital account balances with cash or a bank payment item.";
		return false;
	}
	private static decimal AccessibleHospitalCash(ICharacter actor, ICurrency currency)
	{
		return actor.Body.ExternalItems.RecursiveGetItems<ICurrencyPile>(true)
		            .Where(x => x.Currency == currency)
		            .Sum(x => x.TotalValue);
	}

	private static void TakeHospitalCash(ICharacter actor, IHospital hospital, decimal amount, string reference)
	{
		var targetCoins = hospital.Currency.FindCurrency(actor.Body.ExternalItems.RecursiveGetItems<ICurrencyPile>(true), amount);
		var value = targetCoins.Sum(x => x.Value.Sum(y => y.Value * y.Key.Value));
		var containers = targetCoins.SelectNotNull(x => x.Key.Parent.ContainedIn).Distinct().ToList();
		foreach (var item in targetCoins.Where(item => !item.Key.RemoveCoins(item.Value.Select(x => Tuple.Create(x.Key, x.Value)))))
		{
			item.Key.Parent.Delete();
		}

		VirtualCashLedger.Credit(hospital, hospital.Currency, amount, actor, actor, "Cash", reference,
			hospital.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime);
		var change = value - amount;
		if (change <= 0.0M)
		{
			return;
		}

		var changePile = CurrencyGameItemComponentProto.CreateNewCurrencyPile(hospital.Currency,
			hospital.Currency.FindCoinsForAmount(change, out _));
		foreach (var item in containers)
		{
			var container = item.GetItemType<IContainer>();
			if (container.CanPut(changePile))
			{
				container.Put(null, changePile);
				break;
			}
		}

		if (!changePile.Deleted && changePile.ContainedIn is null)
		{
			if (actor.Body.CanGet(changePile, 0))
			{
				actor.Body.Get(changePile);
			}
			else
			{
				actor.Location.Insert(changePile);
			}
		}
	}

	private static void HospitalDebt(ICharacter actor, StringStack ss)
	{
		if (!DoHospitalCommandFindHospital(actor, out var hospital))
		{
			return;
		}

		if (!ss.IsFinished && ss.PeekSpeech().EqualToAny("pay", "prepay", "deposit"))
		{
			ss.PopSpeech();
			HospitalDebtPay(actor, hospital, ss);
			return;
		}

		var patient = ss.IsFinished ? actor : actor.TargetActor(ss.SafeRemainingArgument);
		if (patient is null)
		{
			actor.OutputHandler.Send("You do not see that patient here.");
			return;
		}

		var account = hospital.DebtAccountFor(patient, false);
		actor.OutputHandler.Send(account?.Show(actor) ??
			$"{patient.HowSeen(actor, true)} has no debt account at {hospital.Name.ColourName()}.");
	}

	private static void HospitalDebtPay(ICharacter actor, IHospital hospital, StringStack ss)
	{
		if (ss.IsFinished || !hospital.Currency.TryGetBaseCurrency(ss.PopSpeech(), out var amount) || amount <= 0.0M)
		{
			actor.OutputHandler.Send("What positive amount do you want to pay to the hospital account?");
			return;
		}

		var patient = actor;
		var payment = new HospitalPaymentSelection { Method = HospitalPaymentMethod.Cash };
		while (!ss.IsFinished)
		{
			var token = ss.PopSpeech();
			if (token.EqualTo("for"))
			{
				if (ss.IsFinished)
				{
					actor.OutputHandler.Send("Whose hospital account do you want to pay?");
					return;
				}

				patient = actor.TargetActor(ss.PopSpeech());
				if (patient is null)
				{
					actor.OutputHandler.Send("You do not see that patient here.");
					return;
				}

				continue;
			}

			if (token.EqualTo("cash"))
			{
				payment = new HospitalPaymentSelection { Method = HospitalPaymentMethod.Cash };
				continue;
			}

			if (token.EqualTo("with"))
			{
				if (ss.IsFinished)
				{
					actor.OutputHandler.Send("Which payment item do you want to use?");
					return;
				}

				var item = actor.TargetItem(ss.SafeRemainingArgument);
				var paymentItem = item?.GetItemType<IBankPaymentItem>();
				if (paymentItem is null)
				{
					actor.OutputHandler.Send("You do not have any such bank payment item.");
					return;
				}

				payment = new HospitalPaymentSelection { Method = HospitalPaymentMethod.BankPaymentItem, PaymentItem = paymentItem };
				break;
			}

			actor.OutputHandler.Send($"Unknown hospital account payment option {token.ColourCommand()}.");
			return;
		}

		var account = hospital.DebtAccountFor(patient, true)!;
		var reference = $"Hospital account payment for {account.PatientName}";
		if (!TryTakeHospitalPaymentAmount(actor, hospital, amount, payment, reference, out var error))
		{
			actor.OutputHandler.Send(error);
			return;
		}

		account.Pay(amount, reference);
		var status = account.Balance > 0.0M
			? $"remaining balance {hospital.Currency.Describe(account.Balance, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}"
			: account.Balance < 0.0M
				? $"prepaid credit {hospital.Currency.Describe(-account.Balance, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}"
				: "the account settled".ColourValue();
		actor.OutputHandler.Send($"You pay {hospital.Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} to {hospital.Name.ColourName()} for {account.PatientName.ColourName()}, leaving {status}.");
	}

	private static void HospitalRequests(ICharacter actor, StringStack ss)
	{
		if (!DoHospitalCommandFindHospital(actor, out var hospital) || !RequireHospitalManager(actor, hospital))
		{
			return;
		}

		var requests = hospital.ServiceRequests
		                       .OrderByDescending(x => x.Status is HospitalServiceRequestStatus.Queued or HospitalServiceRequestStatus.Assigned or HospitalServiceRequestStatus.InProgress)
		                       .ThenByDescending(x => x.CreatedAt)
		                       .Take(50)
		                       .ToList();
		if (!requests.Any())
		{
			actor.OutputHandler.Send($"{hospital.Name.ColourName()} has no service requests.");
			return;
		}

		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			requests.Select(x => new List<string>
			{
				x.Id.ToString("N0", actor),
				x.PatientName,
				x.Service.Name,
				x.Status.DescribeEnum(),
				hospital.Currency.Describe(x.Price, CurrencyDescriptionPatternType.ShortDecimal),
				x.CreatedAt.ToString("g", actor)
			}),
			new List<string> { "Id", "Patient", "Service", "Status", "Price", "Created" },
			actor,
			Telnet.Green,
			2));
	}

	private static void HospitalOperations(ICharacter actor)
	{
		if (!DoHospitalCommandFindHospital(actor, out var hospital) || !RequireHospitalManager(actor, hospital))
		{
			return;
		}

		var requests = HospitalOperationalRequests(hospital).ToList();
		var activeTasks = hospital.TaskBoard.ActiveTasks
		                          .Where(x => x.Status is EmploymentTaskStatus.Pending or EmploymentTaskStatus.Assigned or
			                          EmploymentTaskStatus.InProgress or EmploymentTaskStatus.Blocked or EmploymentTaskStatus.Failed)
		                          .ToList();
		var theatres = hospital.OperatingTheatres
		                       .OrderBy(x => x.Id)
		                       .ToList();
		var rooms = HospitalOperationsRooms(hospital)
		            .Where(x => theatres.All(y => y.Id != x.Id))
		            .OrderBy(x => x.Id)
		            .ToList();

		var sb = new StringBuilder();
		sb.AppendLine($"Hospital Operations - {hospital.Name.ColourName()}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine($"Open: {hospital.IsTrading.ToColouredString()} | Active Requests: {requests.Count.ToString("N0", actor).ColourValue()} | Active Tasks: {activeTasks.Count.ToString("N0", actor).ColourValue()}");
		sb.AppendLine();
		sb.AppendLine("Operating Theatres:");
		if (!theatres.Any())
		{
			sb.AppendLine("\tNone configured.".ColourError());
		}
		else
		{
			sb.AppendLine(StringUtilities.GetTextTable(
				theatres.Select(theatre =>
				{
					var theatreRequests = requests
					                      .Where(x => x.OperatingTheatreCellId == theatre.Id)
					                      .OrderBy(x => x.Id)
					                      .ToList();
					return new List<string>
					{
						theatre.GetFriendlyReference(actor),
						DescribeHospitalTheatreState(hospital, theatre, theatreRequests),
						DescribeHospitalTheatreRequests(actor, hospital, theatreRequests),
						DescribeHospitalTheatrePatients(actor, hospital, theatre, theatreRequests),
						DescribeHospitalTheatreStaff(actor, hospital, theatre, theatreRequests),
						DescribeHospitalTheatreAction(actor, hospital, theatreRequests),
						DescribeHospitalTheatreIssues(actor, hospital, theatre, theatreRequests)
					};
				}),
				new List<string> { "Theatre", "State", "Requests", "Patients", "Staff", "Action", "Issues" },
				actor,
				Telnet.Green,
				2));
		}

		sb.AppendLine();
		sb.AppendLine("Other Hospital Rooms:");
		if (!rooms.Any())
		{
			sb.AppendLine("\tNone configured.".ColourError());
		}
		else
		{
			sb.AppendLine(StringUtilities.GetTextTable(
				rooms.Select(room => new List<string>
				{
					room.GetFriendlyReference(actor),
					DescribeHospitalRoomRoles(hospital, room),
					DescribeHospitalRoomStaff(actor, hospital, room),
					DescribeHospitalRoomPatients(actor, requests, room),
					DescribeHospitalRoomInventory(actor, room)
				}),
				new List<string> { "Room", "Roles", "Staff", "Patients/Requests", "Inventory" },
				actor,
				Telnet.Green,
				2));
		}

		sb.AppendLine();
		sb.AppendLine("Active Procedures:");
		if (!requests.Any())
		{
			sb.AppendLine("\tNone.".ColourValue());
		}
		else
		{
			sb.AppendLine(StringUtilities.GetTextTable(
				requests.OrderBy(x => x.CreatedAt)
				        .Select(request =>
				        {
					        var task = HospitalTaskForRequest(hospital, request);
					        return new List<string>
					        {
						        $"#{request.Id.ToString("N0", actor)}",
						        request.Service.Name,
						        request.Status.DescribeEnum(),
						        DescribeHospitalRequestPatient(actor, request),
						        DescribeHospitalRequestLocation(actor, hospital, request),
						        DescribeHospitalTaskEmployee(actor, task, request),
						        DescribeHospitalTaskAction(actor, task),
						        DescribeHospitalTaskIssues(request, task),
						        DescribeHospitalTaskResources(task)
					        };
				        }),
				new List<string> { "Req", "Service", "Status", "Patient", "Location", "Staff", "Action", "Issues", "Reserved/Resources" },
				actor,
				Telnet.Green,
				2));
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	private static IEnumerable<IHospitalServiceRequest> HospitalOperationalRequests(IHospital hospital)
	{
		return hospital.ServiceRequests
		               .Where(x => x.Status is not (HospitalServiceRequestStatus.Completed or
			               HospitalServiceRequestStatus.Failed or HospitalServiceRequestStatus.Cancelled or
			               HospitalServiceRequestStatus.Declined));
	}

	private static IEnumerable<ICell> HospitalOperationsRooms(IHospital hospital)
	{
		return hospital.Locations
		               .Concat(hospital.WaitingRooms)
		               .Concat(hospital.OperatingTheatres)
		               .Concat(hospital.SupplyRooms)
		               .Concat(hospital.RecoveryRooms)
		               .Concat(hospital.StaffRooms)
		               .Where(x => x is not null)
		               .DistinctBy(x => x.Id);
	}

	private static IEmploymentActiveTask? HospitalTaskForRequest(IHospital hospital, IHospitalServiceRequest request)
	{
		var tasks = hospital.TaskBoard.ActiveTasks;
		if (request.EmploymentTaskId is { } taskId)
		{
			var task = tasks.FirstOrDefault(x => x.Id == taskId);
			if (task is not null)
			{
				return task;
			}
		}

		return tasks.FirstOrDefault(x => x.ActionPlan.Steps.Any(step => step switch
		{
			HospitalServiceActionStep hospitalService =>
				hospitalService.Hospital.Id == hospital.Id && hospitalService.Request.Id == request.Id,
			HospitalSupplyPreparationActionStep hospitalSupply =>
				hospitalSupply.Hospital.Id == hospital.Id && hospitalSupply.Request.Id == request.Id,
			_ => false
		}));
	}

	private static string DescribeHospitalTheatreState(IHospital hospital, ICell theatre,
		IReadOnlyCollection<IHospitalServiceRequest> requests)
	{
		if (requests.Any())
		{
			return "Reserved".ColourValue();
		}

		return HospitalRoomCharacters(theatre).Any(x => !hospital.IsEmployee(x))
			? "Occupied".ColourCommand()
			: "Available".ColourValue();
	}

	private static string DescribeHospitalTheatreRequests(ICharacter actor, IHospital hospital,
		IReadOnlyCollection<IHospitalServiceRequest> requests)
	{
		if (!requests.Any())
		{
			return "none".ColourValue();
		}

		return requests
		       .Select(x =>
		       {
			       var task = HospitalTaskForRequest(hospital, x);
			       return $"#{x.Id.ToString("N0", actor)} {x.Service.Name} ({x.Status.DescribeEnum()}{(task is null ? string.Empty : $"/{task.Status.DescribeEnum()}")})";
		       })
		       .ListToString();
	}

	private static string DescribeHospitalTheatrePatients(ICharacter actor, IHospital hospital, ICell theatre,
		IReadOnlyCollection<IHospitalServiceRequest> requests)
	{
		var patients = requests
		               .Select(x => DescribeHospitalRequestPatient(actor, x))
		               .Where(x => !string.IsNullOrWhiteSpace(x))
		               .ToList();
		var requestPatientIds = requests
		                        .Select(x => x.Patient)
		                        .Where(x => x is not null)
		                        .Select(CharacterInstanceIdentityComparer.PhysicalInstanceKey)
		                        .ToHashSet();
		patients.AddRange(HospitalRoomCharacters(theatre)
		                  .Where(x => !hospital.IsEmployee(x))
		                  .Where(x => !requestPatientIds.Contains(CharacterInstanceIdentityComparer.PhysicalInstanceKey(x)))
		                  .Select(x => x.HowSeen(actor, colour: false)));
		return HospitalListToStringOrNone(patients.Distinct(StringComparer.InvariantCultureIgnoreCase));
	}

	private static string DescribeHospitalTheatreStaff(ICharacter actor, IHospital hospital, ICell theatre,
		IReadOnlyCollection<IHospitalServiceRequest> requests)
	{
		var staff = HospitalRoomCharacters(theatre)
		            .Where(hospital.IsEmployee)
		            .Select(x => x.HowSeen(actor, colour: false))
		            .ToList();
		staff.AddRange(requests.Select(x => DescribeHospitalTaskEmployee(actor, HospitalTaskForRequest(hospital, x), x))
		                       .Where(x => !string.IsNullOrWhiteSpace(x) && !x.EqualTo("none")));
		return HospitalListToStringOrNone(staff.Distinct(StringComparer.InvariantCultureIgnoreCase));
	}

	private static string DescribeHospitalTheatreAction(ICharacter actor, IHospital hospital,
		IReadOnlyCollection<IHospitalServiceRequest> requests)
	{
		var actions = requests
		              .Select(x => DescribeHospitalTaskAction(actor, HospitalTaskForRequest(hospital, x)))
		              .Where(x => !x.EqualTo("none"))
		              .Distinct()
		              .ToList();
		return HospitalListToStringOrNone(actions);
	}

	private static string DescribeHospitalTheatreIssues(ICharacter actor, IHospital hospital, ICell theatre,
		IReadOnlyCollection<IHospitalServiceRequest> requests)
	{
		var issues = new List<string>();
		var requestPatientIds = requests
		                        .Select(x => x.Patient)
		                        .Where(x => x is not null)
		                        .Select(CharacterInstanceIdentityComparer.PhysicalInstanceKey)
		                        .ToHashSet();
		var unrelatedOccupants = HospitalRoomCharacters(theatre)
		                         .Where(x => !hospital.IsEmployee(x))
		                         .Where(x => !requestPatientIds.Contains(CharacterInstanceIdentityComparer.PhysicalInstanceKey(x)))
		                         .Select(x => x.HowSeen(actor, colour: false))
		                         .ToList();
		if (unrelatedOccupants.Any())
		{
			issues.Add($"unrelated occupant: {unrelatedOccupants.ListToString()}");
		}

		issues.AddRange(requests
		                .Select(x => DescribeHospitalTaskIssues(x, HospitalTaskForRequest(hospital, x)))
		                .Where(x => !x.EqualTo("none")));

		return HospitalListToStringOrNone(issues.Distinct(StringComparer.InvariantCultureIgnoreCase));
	}

	private static string DescribeHospitalRoomRoles(IHospital hospital, ICell room)
	{
		var roles = new List<HospitalLocationRole>();
		if (hospital.WaitingRooms.Any(x => x.Id == room.Id))
		{
			roles.Add(HospitalLocationRole.WaitingRoom);
		}

		if (hospital.SupplyRooms.Any(x => x.Id == room.Id))
		{
			roles.Add(HospitalLocationRole.SupplyArea);
		}

		if (hospital.RecoveryRooms.Any(x => x.Id == room.Id))
		{
			roles.Add(HospitalLocationRole.RecoveryRoom);
		}

		if (hospital.StaffRooms.Any(x => x.Id == room.Id))
		{
			roles.Add(HospitalLocationRole.StaffRoom);
		}

		if (hospital.OperatingTheatres.Any(x => x.Id == room.Id))
		{
			roles.Add(HospitalLocationRole.OperatingTheatre);
		}

		return roles.Any()
			? roles.Select(x => x.DescribeEnum()).ListToString()
			: "none".ColourError();
	}

	private static string DescribeHospitalRoomStaff(ICharacter actor, IHospital hospital, ICell room)
	{
		return HospitalListToStringOrNone(HospitalRoomCharacters(room)
		                                  .Where(hospital.IsEmployee)
		                                  .Select(x => x.HowSeen(actor, colour: false))
		                                  .Distinct(StringComparer.InvariantCultureIgnoreCase));
	}

	private static string DescribeHospitalRoomPatients(ICharacter actor, IReadOnlyCollection<IHospitalServiceRequest> requests,
		ICell room)
	{
		var patients = requests
		               .Where(x => x.Patient?.Location?.Id == room.Id || x.OperatingTheatreCellId == room.Id)
		               .Select(x => $"#{x.Id.ToString("N0", actor)} {DescribeHospitalRequestPatient(actor, x)}")
		               .Distinct(StringComparer.InvariantCultureIgnoreCase)
		               .ToList();
		return HospitalListToStringOrNone(patients);
	}

	private static string DescribeHospitalRoomInventory(ICharacter actor, ICell room)
	{
		var itemCount = (room.GameItems ?? []).Count();
		return itemCount == 0
			? "none".ColourValue()
			: $"{itemCount.ToString("N0", actor)} item{(itemCount == 1 ? string.Empty : "s")}".ColourValue();
	}

	private static string DescribeHospitalRequestPatient(ICharacter actor, IHospitalServiceRequest request)
	{
		return request.Patient?.HowSeen(actor, colour: false) ?? request.PatientName;
	}

	private static string DescribeHospitalRequestLocation(ICharacter actor, IHospital hospital,
		IHospitalServiceRequest request)
	{
		if (request.OperatingTheatreCellId is { } theatreId)
		{
			var theatre = DescribeHospitalCellById(actor, hospital, theatreId);
			return request.Patient?.Location is { } patientLocation && patientLocation.Id != theatreId
				? $"{theatre} (patient at {patientLocation.GetFriendlyReference(actor)})"
				: theatre;
		}

		if (request.UsedInPlaceFallback)
		{
			return "in-place fallback".ColourCommand();
		}

		return request.Patient?.Location is { } location
			? location.GetFriendlyReference(actor)
			: "unknown".ColourError();
	}

	private static string DescribeHospitalTaskEmployee(ICharacter actor, IEmploymentActiveTask? task,
		IHospitalServiceRequest request)
	{
		if (task?.AssignedEmployee is not null)
		{
			return task.AssignedEmployee.HowSeen(actor, colour: false);
		}

		return request.AssignedEmployeeId is { } employeeId
			? $"#{employeeId.ToString("N0", actor)}"
			: "none";
	}

	private static string DescribeHospitalTaskAction(ICharacter actor, IEmploymentActiveTask? task)
	{
		if (task is null)
		{
			return "none";
		}

		var index = NextHospitalTaskStepIndex(task);
		if (index < 0 || index >= task.ActionPlan.Steps.Count || index >= task.StepStates.Count)
		{
			return task.Status.DescribeEnum();
		}

		return $"#{(index + 1).ToString("N0", actor)} {task.StepStates[index].DescribeEnum()} - {EmploymentTaskAuthoringService.DescribeStep(task.ActionPlan.Steps[index], actor)}";
	}

	private static string DescribeHospitalTaskIssues(IHospitalServiceRequest request, IEmploymentActiveTask? task)
	{
		var issues = new List<string>();
		if (request.Patient is null)
		{
			issues.Add("patient unavailable");
		}

		if (request.Service.RequiredEquipment.Any() && !request.SupplyPrepared)
		{
			issues.Add("supplies not prepared");
		}

		if (request.Service.PreferOperatingTheatre && request.Status is HospitalServiceRequestStatus.Assigned or HospitalServiceRequestStatus.InProgress &&
		    request.OperatingTheatreCellId is null)
		{
			issues.Add("no theatre reserved");
		}

		if (task is null)
		{
			issues.Add("no employment task");
		}
		else
		{
			if (task.Status == EmploymentTaskStatus.Blocked)
			{
				issues.Add(string.IsNullOrWhiteSpace(task.BlockedReason) ? "task blocked" : task.BlockedReason);
			}
			else if (task.Status == EmploymentTaskStatus.Failed)
			{
				issues.Add("task failed");
			}

			issues.AddRange(task.StepOperationalStates
			                    .Where(x => !string.IsNullOrWhiteSpace(x.FailureDiagnostic))
			                    .Select(x => x.FailureDiagnostic!));
		}

		if (request.Status == HospitalServiceRequestStatus.PendingConsent)
		{
			issues.Add("awaiting patient consent");
		}

		return HospitalListToStringOrNone(issues.Distinct(StringComparer.InvariantCultureIgnoreCase));
	}

	private static string DescribeHospitalTaskResources(IEmploymentActiveTask? task)
	{
		if (task is null)
		{
			return "none";
		}

		var resources = new List<string>();
		foreach (var state in task.StepOperationalStates.Where(x => !x.IsEmpty))
		{
			if (!string.IsNullOrWhiteSpace(state.ReservationReference))
			{
				resources.Add($"Reserved: {state.ReservationReference}");
			}

			if (!string.IsNullOrWhiteSpace(state.SelectedResources))
			{
				resources.Add($"Selection: {state.SelectedResources}");
			}

			if (!string.IsNullOrWhiteSpace(state.LoadedAssets))
			{
				resources.Add($"Loaded: {state.LoadedAssets}");
			}
		}

		return HospitalListToStringOrNone(resources.Distinct(StringComparer.InvariantCultureIgnoreCase).Take(4));
	}

	private static int NextHospitalTaskStepIndex(IEmploymentActiveTask task)
	{
		for (var i = 0; i < task.StepStates.Count; i++)
		{
			if (task.StepStates[i] is EmploymentActionStepStatus.Pending or EmploymentActionStepStatus.InProgress or
			    EmploymentActionStepStatus.Blocked or EmploymentActionStepStatus.Failed)
			{
				return i;
			}
		}

		return -1;
	}

	private static string DescribeHospitalCellById(ICharacter actor, IHospital hospital, long id)
	{
		var cell = HospitalOperationsRooms(hospital).FirstOrDefault(x => x.Id == id);
		return cell?.GetFriendlyReference(actor) ?? $"#{id.ToString("N0", actor)}";
	}

	private static IEnumerable<ICharacter> HospitalRoomCharacters(ICell room)
	{
		return room.Characters ?? [];
	}

	private static string HospitalListToStringOrNone(IEnumerable<string> values)
	{
		var text = values
		           .Where(x => !string.IsNullOrWhiteSpace(x))
		           .ToList()
		           .ListToString();
		return string.IsNullOrWhiteSpace(text) ? "none" : text;
	}

	private static void HospitalShowRequest(ICharacter actor, StringStack ss)
	{
		if (!DoHospitalCommandFindHospital(actor, out var hospital) || !RequireHospitalManager(actor, hospital))
		{
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which request do you want to show?");
			return;
		}

		var request = hospital.RequestById(ss.PopSpeech());
		actor.OutputHandler.Send(request?.Show(actor) ?? "There is no such hospital request.");
	}

	private static void HospitalOpen(ICharacter actor)
	{
		if (!DoHospitalCommandFindHospital(actor, out var hospital) || !RequireHospitalManager(actor, hospital))
		{
			return;
		}

		if (hospital.IsTrading)
		{
			actor.OutputHandler.Send($"{hospital.Name.ColourName()} is already open.");
			return;
		}

		hospital.IsTrading = true;
		hospital.Changed = true;
		actor.OutputHandler.Send($"You open {hospital.Name.ColourName()} for patients.");
	}

	private static void HospitalClose(ICharacter actor)
	{
		if (!DoHospitalCommandFindHospital(actor, out var hospital) || !RequireHospitalManager(actor, hospital))
		{
			return;
		}

		if (!hospital.IsTrading)
		{
			actor.OutputHandler.Send($"{hospital.Name.ColourName()} is already closed.");
			return;
		}

		hospital.IsTrading = false;
		hospital.Changed = true;
		actor.OutputHandler.Send($"You close {hospital.Name.ColourName()} to new patients.");
	}

	private static void HospitalMaxDebt(ICharacter actor, StringStack ss)
	{
		if (!DoHospitalCommandFindHospital(actor, out var hospital) || !RequireHospitalManager(actor, hospital))
		{
			return;
		}

		if (ss.IsFinished || !hospital.Currency.TryGetBaseCurrency(ss.SafeRemainingArgument, out var amount) || amount < 0.0M)
		{
			actor.OutputHandler.Send("What non-negative default debt limit should new patients have?");
			return;
		}

		hospital.DefaultMaximumDebt = amount;
		hospital.Changed = true;
		actor.OutputHandler.Send($"New hospital debt accounts will default to {hospital.Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}.");
	}

	private static void HospitalRoom(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Do you want to add or remove a hospital room role?");
			return;
		}

		var action = ss.PopSpeech().CollapseString().ToLowerInvariant();
		if (!action.EqualToAny("add", "remove", "delete"))
		{
			actor.OutputHandler.Send("Hospital room syntax is room add|remove <waiting|theatre|supply|recovery|staff> [here|<direction>|<#>].");
			return;
		}

		if (ss.IsFinished || !TryParseHospitalLocationRole(ss.PopSpeech(), out var role))
		{
			actor.OutputHandler.Send("Which hospital room role? Valid roles are waiting, theatre, supply, recovery, and staff.");
			return;
		}

		if (!TryResolveHospitalRoomTarget(actor, ss.SafeRemainingArgument, out var cell, out var error))
		{
			actor.OutputHandler.Send(error);
			return;
		}

		var hospital = CurrentHospital(actor);
		var hospitalError = string.Empty;
		hospital ??= FindHospitalForRoomTarget(actor, cell, out hospitalError);
		if (hospital is null)
		{
			actor.OutputHandler.Send(hospitalError);
			return;
		}

		if (!RequireHospitalManager(actor, hospital))
		{
			return;
		}

		if (action.EqualTo("add"))
		{
			hospital.AddLocation(cell, role);
			actor.OutputHandler.Send($"You flag {cell.GetFriendlyReference(actor).ColourName()} as a {role.DescribeEnum().ColourValue()} for {hospital.Name.ColourName()}.");
			return;
		}

		hospital.RemoveLocation(cell, role);
		actor.OutputHandler.Send($"You remove {cell.GetFriendlyReference(actor).ColourName()}'s {role.DescribeEnum().ColourValue()} role for {hospital.Name.ColourName()}.");
	}

	private static IHospital? FindHospitalForRoomTarget(ICharacter actor, ICell targetCell, out string error)
	{
		error = string.Empty;
		var hospitals = actor.Gameworld.Hospitals
		                     .Where(x => x.Locations.Any(y => CellsAreSameOrAdjacent(y, targetCell)))
		                     .GroupBy(x => x.Id)
		                     .Select(x => x.First())
		                     .ToList();
		if (hospitals.Count == 1)
		{
			return hospitals[0];
		}

		if (hospitals.Count > 1)
		{
			error = $"The selected room is adjacent to multiple hospitals ({hospitals.Select(x => x.Name.ColourName()).ListToString()}). Stand in one of that hospital's existing rooms or pick a less ambiguous room.";
			return null;
		}

		error = "You are not currently at a hospital, and the selected room is not adjacent to a hospital.";
		return null;
	}

	private static bool CellsAreSameOrAdjacent(ICell first, ICell second)
	{
		return first.Id == second.Id ||
		       first.Surrounds.Any(x => x.Id == second.Id) ||
		       second.Surrounds.Any(x => x.Id == first.Id);
	}

	private static bool TryResolveHospitalRoomTarget(ICharacter actor, string target, out ICell cell, out string error)
	{
		cell = null!;
		error = string.Empty;
		var location = actor.Location;
		var targetText = target.Trim();
		if (string.IsNullOrWhiteSpace(targetText) || targetText.EqualTo("here"))
		{
			if (location is null)
			{
				error = "You are nowhere.";
				return false;
			}

			cell = location;
			return true;
		}

		if (location is not null)
		{
			var directionText = targetText.ToLowerInvariant();
			if (Constants.CardinalDirectionStringToDirection.TryGetValue(directionText, out var direction) ||
			    Constants.CardinalDirectionStringToDirection.TryGetValue(directionText.CollapseString(), out direction))
			{
				var exit = location.GetExit(direction, actor);
				if (exit is null)
				{
					error = $"There is no visible exit {targetText.ColourCommand()} from here.";
					return false;
				}

				cell = exit.Destination;
				return true;
			}

			var keywordExit = location.GetExitKeyword(targetText, actor);
			if (keywordExit is not null)
			{
				cell = keywordExit.Destination;
				return true;
			}
		}

		var idOrName = targetText.TrimStart('#');
		var targetCell = actor.Gameworld.Cells.GetByIdOrName(idOrName);
		if (targetCell is null)
		{
			error = $"There is no room matching {targetText.ColourCommand()}.";
			return false;
		}

		cell = targetCell;
		return true;
	}

	private static void HospitalCreate(ICharacter actor, StringStack ss)
	{
		if (CurrentHospital(actor) is not null)
		{
			actor.OutputHandler.Send("There is already a hospital at this location.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name should this hospital have?");
			return;
		}

		var name = ss.PopSpeech().TitleCase();
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which economic zone should this hospital belong to?");
			return;
		}

		var zone = actor.Gameworld.EconomicZones.GetByIdOrName(ss.PopSpeech());
		if (zone is null)
		{
			actor.OutputHandler.Send("There is no such economic zone.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which bank account should receive hospital income, or NONE?");
			return;
		}

		IBankAccount? account = null;
		if (!ss.SafeRemainingArgument.EqualToAny("none", "clear", "remove"))
		{
			var (foundAccount, error) = MudSharp.Economy.Banking.Bank.FindBankAccount(ss.SafeRemainingArgument, null, actor);
			account = foundAccount;
			if (account is null)
			{
				actor.OutputHandler.Send(error);
				return;
			}

			if (account.Currency != zone.Currency)
			{
				actor.OutputHandler.Send($"That bank account is not in {zone.Currency.Name.ColourName()}.");
				return;
			}
		}

		var hospital = new Hospital(zone, account, name);
		hospital.AddLocation(actor.Location, HospitalLocationRole.WaitingRoom);
		actor.Gameworld.Add(hospital);
		actor.OutputHandler.Send($"You create the {hospital.Name.ColourName()} hospital at this location.");
	}

	private static void HospitalDelete(ICharacter actor)
	{
		if (!DoHospitalCommandFindHospital(actor, out var hospital))
		{
			return;
		}

		if (hospital.ActiveServiceRequests.Any())
		{
			actor.OutputHandler.Send("You cannot delete a hospital with active service requests.");
			return;
		}

		hospital.Delete();
		actor.Gameworld.Destroy(hospital);
		actor.OutputHandler.Send($"You delete the {hospital.Name.ColourName()} hospital.");
	}

	private static void HospitalListAll(ICharacter actor)
	{
		var hospitals = actor.Gameworld.Hospitals.OrderBy(x => x.Name).ToList();
		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			hospitals.Select(x => new List<string>
			{
				x.Id.ToString("N0", actor),
				x.Name,
				x.EconomicZone.Name,
				x.IsTrading.ToColouredString(),
				x.Services.Count().ToString("N0", actor),
				x.ActiveServiceRequests.Count().ToString("N0", actor)
			}),
			new List<string> { "Id", "Name", "Zone", "Open", "Services", "Active" },
			actor,
			Telnet.Green,
			2));
	}

	private static void HospitalFire(ICharacter actor, StringStack ss)
	{
		if (!DoHospitalCommandFindHospital(actor, out var hospital))
		{
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Who do you want to fire?");
			return;
		}

		var text = ss.SafeRemainingArgument;
		var target = actor.TargetActor(text);
		var service = new EmploymentCommandService();
		if (target is not null)
		{
			service.TryTerminateContractsForEmployee(actor, hospital, target, out var message);
			actor.OutputHandler.Send(message);
			return;
		}

		service.TryTerminateContractsForEmployee(actor, hospital, text, out var nameMessage);
		actor.OutputHandler.Send(nameMessage);
	}

	private static void HospitalDirectHire(ICharacter actor, StringStack ss, EmploymentRole role)
	{
		if (!DoHospitalCommandFindHospital(actor, out var hospital))
		{
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send($"Who do you want to employ as a {role.DescribeEnum()}?");
			return;
		}

		var target = actor.TargetActor(ss.SafeRemainingArgument);
		if (target is null)
		{
			actor.OutputHandler.Send("You do not see them here.");
			return;
		}

		new EmploymentCommandService().TryHireDirectContract(actor, hospital, target, role, out _, out var message);
		actor.OutputHandler.Send(message);
	}

	private static void HospitalDirectRoleToggle(ICharacter actor, StringStack ss, EmploymentRole role)
	{
		if (!DoHospitalCommandFindHospital(actor, out var hospital))
		{
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send($"Who do you want to toggle as a {role.DescribeEnum()}?");
			return;
		}

		var target = actor.TargetActor(ss.SafeRemainingArgument);
		if (target is null)
		{
			actor.OutputHandler.Send("You do not see them here.");
			return;
		}

		new EmploymentCommandService().TryToggleRoleContract(actor, hospital, target, role, out var message);
		actor.OutputHandler.Send(message);
	}

	private static bool TryParseHospitalServiceType(string text, out HospitalServiceType serviceType)
	{
		switch (text.CollapseString().ToLowerInvariant())
		{
			case "bind":
			case "binding":
				serviceType = HospitalServiceType.Binding;
				return true;
			case "clean":
			case "cleaning":
			case "woundcleaning":
				serviceType = HospitalServiceType.WoundCleaning;
				return true;
			case "close":
			case "suture":
			case "closing":
			case "woundclosing":
				serviceType = HospitalServiceType.WoundClosing;
				return true;
			case "tend":
			case "tending":
				serviceType = HospitalServiceType.WoundTending;
				return true;
			case "relocate":
			case "relocation":
			case "bonerelocation":
				serviceType = HospitalServiceType.BoneRelocation;
				return true;
			case "set":
			case "setting":
			case "bonesetting":
				serviceType = HospitalServiceType.BoneSetting;
				return true;
			case "surgery":
			case "surgical":
			case "procedure":
				serviceType = HospitalServiceType.SurgicalProcedure;
				return true;
			case "implant":
			case "implants":
				serviceType = HospitalServiceType.ImplantProcedure;
				return true;
			case "donateblood":
			case "blooddonation":
			case "donation":
				serviceType = HospitalServiceType.BloodDonation;
				return true;
			case "transfuse":
			case "transfusion":
			case "bloodtransfusion":
				serviceType = HospitalServiceType.BloodTransfusion;
				return true;
			case "stabilise":
			case "stabilize":
			case "stabilisation":
			case "stabilization":
				serviceType = HospitalServiceType.Stabilisation;
				return true;
			case "full":
			case "fulltreatment":
			case "comprehensive":
				serviceType = HospitalServiceType.FullTreatment;
				return true;
		}

		return text.TryParseEnum(out serviceType);
	}

	private static bool TryParseHospitalLocationRole(string text, out HospitalLocationRole role)
	{
		switch (text.CollapseString().ToLowerInvariant())
		{
			case "waiting":
			case "waitingroom":
			case "wait":
				role = HospitalLocationRole.WaitingRoom;
				return true;
			case "theatre":
			case "theater":
			case "operating":
			case "operatingtheatre":
			case "operatingtheater":
				role = HospitalLocationRole.OperatingTheatre;
				return true;
			case "supply":
			case "supplies":
			case "supplyroom":
				role = HospitalLocationRole.SupplyArea;
				return true;
			case "recovery":
			case "recoveryroom":
			case "recover":
				role = HospitalLocationRole.RecoveryRoom;
				return true;
			case "staff":
			case "staffroom":
			case "breakroom":
			case "break":
				role = HospitalLocationRole.StaffRoom;
				return true;
		}

		return text.TryParseEnum(out role);
	}
}
