using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.RPG.Law;

public partial class LegalAuthority
{
	#region Building

	public string BuildingHelpText => @"The valid options for editing legal authorities are as follows:

	#3name <name>#0 - renames this legal authority
	#3currency <currency>#0 - changes the currency that fines and such will be issued in
	#3know#0 - toggles whether players know the crimes they have committed
	#3zone <zone>#0 - toggles a zone as in or out of the enforcement area of this legal authority
	#3class add <name>#0 - adds a new legal class
	#3class delete <name>#0 - deletes a legal class
	#3class <which> ...#0 - sets properties of a legal class
	#3enforcement add <name>#0 - adds a new enforcer authority
	#3enforcement delete <name>#0 - deletes an enforcement authority
	#3enforcement <which> ...#0 - sets the properties of an enforcement authority
	#3inflate <multiplier>#0 - changes all fines by th specified multiplier
	#3law add <name> <type>#0 - adds a new law of the specified type
	#3law delete <name>#0 - deletes a law
	#3law <which> ...#0 - sets properties of a law
	#3patrol add <name>#0 - creates a new patrol template
	#3patrol delete <name>#0 - deletes a patrol template
	#3patrol <which> ...#0 - sets properties of a patrol template
	#3prepare here|<room>#0 - sets the patrol preparation room (usually an armoury)
	#3marshalling here|<room>#0 - sets the patrol marshalling room (where patrols launch)
	#3stow here|<room>#0 - sets the stowing location for enforcers not on duty
	#3prison here|<room>#0 - sets the prison administration location for this authority
	#3release here|<room>#0 - sets the prison release location for this authority
	#3belongings here|<room>#0 - sets the prison belongings stowage location for this authority
	#3cell here|<room>#0 - toggles a location as a holding cell for this authority
	#3imprisonedprog <prog>#0 - sets the on-imprisoned prog (when convicted and sent to jail)
	#3imprisonedprog none#0 - clears the on-imprisoned prog
	#3heldprog <prog>#0 - sets the on-held prog (when arrested and held in a cell)
	#3heldprog none#0 - clears the on-held prog
	#3releasedprog <prog>#0 - sets the on-released prog
	#3releasedprog none#0 - clears the on-released prog
	#3jailentry here|<room>#0 - sets the entry to the custodial jail
	#3jail here|<room>#0 - toggles a location as a part of the custodial jail
	#3court here|<room>#0 - sets the courtroom location for this authority
	#3bankaccount <code>:<accn>#0 - sets the bank account for fines paid
	#3autoconvict#0 - toggles automatic application of convictions
	#3autoconvicttime <timespan>#0 - sets the delay before applying auto conviction
	#3discord <channelid>|none#0 - sets or clears the discord announce channel
	#3bail <prog>#0 - sets the prog which determines the bail amount for a crime";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "currency":
				return BuildingCommandCurrency(actor, command);
			case "inflate":
				return BuildingCommandInflate(actor, command);
			case "zone":
				return BuildingCommandZone(actor, command);
			case "class":
				return BuildingCommandClass(actor, command);
			case "enforcement":
				return BuildingCommandEnforcement(actor, command);
			case "law":
				return BuildingCommandLaw(actor, command);
			case "patrol":
				return BuildingCommandPatrol(actor, command);
			case "laws":
				return BuildingCommandLaws(actor, command);
			case "prepare":
				return BuildingCommandPrepareLocation(actor, command);
			case "marshal":
			case "marshalling":
			case "marshall":
				return BuildingCommandMarshalLocation(actor, command);
			case "stow":
			case "stowing":
				return BuildingCommandEnforcerStowingLocation(actor, command);
			case "prison":
				return BuildingCommandPrisonLocation(actor, command);
			case "release":
				return BuildingCommandPrisonReleaseLocation(actor, command);
			case "belong":
			case "belongings":
				return BuildingCommandPrisonBelongingsLocation(actor, command);
			case "cell":
			case "prisoncell":
			case "holding":
			case "holdingcell":
				return BuildingCommandCellLocation(actor, command);
			case "know":
				return BuildingCommandPlayersKnowCrimes(actor, command);
			case "imprisonedprog":
				return BuildingCommandImprisonedProg(actor, command);
			case "releasedprog":
				return BuildingCommandReleasedProg(actor, command);
			case "heldprog":
			case "arrestedprog":
			case "arrestprog":
				return BuildingCommandHeldProg(actor, command);
			case "gaolentry":
			case "jailentry":
				return BuildingCommandJailEntry(actor, command);
			case "jail":
			case "gaol":
				return BuildingCommandJail(actor, command);
			case "court":
			case "courtroom":
				return BuildingCommandCourtroom(actor, command);
			case "bankaccount":
			case "bank":
			case "account":
				return BuildingCommandBankAccount(actor, command);
			case "autoconvict":
				return BuildingCommandAutoConvict(actor, command);
			case "autoconvicttime":
				return BuildingCommandAutoConvictTime(actor, command);
			case "discord":
				return BuildingCommandDiscordChannel(actor, command);
			case "bail":
			case "bailprog":
				return BuildingCommandBailProg(actor, command);
		}

		actor.OutputHandler.Send(BuildingHelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandBailProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog do you set for calculating bail?");
			return false;
		}

		var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Number,
			new[]
			{
				new[] { FutureProgVariableTypes.Character },
				new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Crime }
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		BailCalculationProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This legal authority will now use the {BailCalculationProg.MXPClickableFunctionNameWithId()} prog for determining bail.");
		return true;
	}

	private bool BuildingCommandHeldProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which prog do you want to execute when a prisoner is arrested and thrown in a cell? You can also specify 'none' to have none.");
			return false;
		}

		if (command.PeekSpeech().EqualToAny("none", "clear", "remove"))
		{
			OnPrisonerHeld = null;
			Changed = true;
			actor.OutputHandler.Send($"The legal authority will no longer execute a prog when a prisoner is arrested.");
			return true;
		}

		var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Void, new[] { FutureProgVariableTypes.Character }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		OnPrisonerHeld = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This legal authority will now execute the {OnPrisonerReleased.MXPClickableFunctionNameWithId()} prog when a prisoner is arrested.");
		return true;
	}

	private bool BuildingCommandDiscordChannel(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either specify a discord channel ID or the keyword 'none'.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			DiscordChannelId = null;
			Changed = true;
			actor.OutputHandler.Send(
				"This legal authority will no longer broadcast enforcement actions to any discord channel.");
			return true;
		}

		if (!ulong.TryParse(command.SafeRemainingArgument, out var discordChannelId))
		{
			actor.OutputHandler.Send("That is not a valid discord channel ID.");
			return false;
		}

		DiscordChannelId = discordChannelId;
		Changed = true;
		actor.OutputHandler.Send(
			$"This enforcement authority will now broadcast enforcement actions to the discord channel {discordChannelId.ToString("F0", actor).ColourName()}.");
		return true;
	}

	private bool BuildingCommandAutoConvictTime(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"How long should this legal authority wait for a PC enforcer before automatically convicting criminals held in custody?");
			return false;
		}

		if (!TimeSpanParserUtil.TimeSpanParser.TryParse(command.SafeRemainingArgument, TimeSpanParserUtil.Units.Hours,
			    TimeSpanParserUtil.Units.Days, out var ts))
		{
			actor.OutputHandler.Send("That is not a valid timespan.");
			return false;
		}

		AutomaticConvictionTime = ts;
		Changed = true;
		actor.OutputHandler.Send(
			$"This legal authority will now automatically convict held criminals after {ts.Describe(actor).ColourValue()} in custody.");
		return true;
	}

	private bool BuildingCommandAutoConvict(ICharacter actor, StringStack command)
	{
		AutomaticallyConvict = !AutomaticallyConvict;
		Changed = true;
		actor.OutputHandler.Send(
			$"This legal authority will {(AutomaticallyConvict ? "now" : "no longer")} automatically convict imprisoned criminals.");
		return true;
	}

	private bool BuildingCommandBankAccount(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must specify a bank account in the form CODE:ACCN for fines to be paid into.");
			return false;
		}

		var (account, error) = Economy.Banking.Bank.FindBankAccount(command.SafeRemainingArgument, null, actor);
		if (account == null)
		{
			actor.OutputHandler.Send(error);
			return false;
		}

		BankAccount = account;
		Changed = true;
		actor.OutputHandler.Send(
			$"This legal authority will now pay its fine revenue into the bank account {BankAccount.AccountReference.ColourValue()}.");
		return true;
	}

	private bool BuildingCommandJail(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which location do you want to toggle as a part of the jail for custodial sentences for this legal authority?");
			return false;
		}

		ICell location;
		if (command.PeekSpeech().EqualTo("here"))
		{
			location = actor.Location;
		}
		else
		{
			if (!long.TryParse(command.SafeRemainingArgument, out var value))
			{
				actor.OutputHandler.Send(
					$"You must either use the keyword {"here".ColourCommand()} or specify the ID# of a room.");
				return false;
			}

			location = Gameworld.Cells.Get(value);
			if (location == null)
			{
				actor.OutputHandler.Send("There is no such location.");
				return false;
			}
		}

		if (JailLocations.Contains(location))
		{
			_jailLocations.Remove(location);
			actor.OutputHandler.Send(
				$"The {Name.ColourName()} legal authority will no longer use the location {location.HowSeen(actor)} as a jail location for custodial sentences.");
		}
		else
		{
			_jailLocations.Add(location);
			actor.OutputHandler.Send(
				$"The {Name.ColourName()} legal authority will now use the location {location.HowSeen(actor)} as a jail location for custodial sentences.");
		}

		Changed = true;
		return true;
	}

	private bool BuildingCommandJailEntry(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which location do you want to set as the entry to the jail for this legal authority's area? This location must have an exit to a location that is not part of your jail.");
			return false;
		}

		if (command.PeekSpeech().EqualTo("here"))
		{
			JailLocation = actor.Location;
			Changed = true;
			actor.OutputHandler.Send(
				$"You set your current location as the entry to the jail for the {Name.ColourName()} legal authority.");
			return true;
		}

		if (!long.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send(
				$"You must either use the keyword {"here".ColourCommand()} or specify the ID# of a room.");
			return false;
		}

		var location = Gameworld.Cells.Get(value);
		if (location == null)
		{
			actor.OutputHandler.Send("There is no such location.");
			return false;
		}

		JailLocation = location;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {Name.ColourName()} legal authority will now use the location {location.HowSeen(actor)} as its entry to the jail.");
		return true;
	}

	private bool BuildingCommandCourtroom(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which location do you want to set as the entry to the courtroom for this legal authority's area?");
			return false;
		}

		if (command.PeekSpeech().EqualTo("here"))
		{
			CourtLocation = actor.Location;
			Changed = true;
			actor.OutputHandler.Send(
				$"You set your current location as the entry to the courtroom for the {Name.ColourName()} legal authority.");
			return true;
		}

		if (!long.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send(
				$"You must either use the keyword {"here".ColourCommand()} or specify the ID# of a room.");
			return false;
		}

		var location = Gameworld.Cells.Get(value);
		if (location == null)
		{
			actor.OutputHandler.Send("There is no such location.");
			return false;
		}

		CourtLocation = location;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {Name.ColourName()} legal authority will now use the location {location.HowSeen(actor)} as its courtroom.");
		return true;
	}

	private bool BuildingCommandLaws(ICharacter actor, StringStack command)
	{
		var sb = new StringBuilder();
		sb.AppendLine("Laws on the books:");
		sb.AppendLine(StringUtilities.GetTextTable(
			from law in Laws
			select new List<string>
			{
				law.Name,
				law.CrimeType.DescribeEnum(),
				law.OffenderClasses.Select(x => x.Name).ListToString(),
				law.VictimClasses.Select(x => x.Name).ListToString(),
				law.EnforcementStrategy.DescribeEnum(true)
			},
			new List<string>
			{
				"Name",
				"Crime",
				"Offenders",
				"Victims",
				"Enforcement"
			},
			actor.LineFormatLength,
			unicodeTable: actor.Account.UseUnicode
		));
		actor.OutputHandler.Send(sb.ToString());
		return true;
	}

	private bool BuildingCommandReleasedProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which prog do you want to execute when a prisoner is released? You can also specify 'none' to have none.");
			return false;
		}

		if (command.PeekSpeech().EqualToAny("none", "clear", "remove"))
		{
			OnPrisonerReleased = null;
			Changed = true;
			actor.OutputHandler.Send($"The legal authority will no longer execute a prog when a prisoner is released.");
			return true;
		}

		var prog = Gameworld.FutureProgs.GetByIdOrName(command.SafeRemainingArgument);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (!prog.MatchesParameters(new List<FutureProgVariableTypes> { FutureProgVariableTypes.Character }))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that accepts a single character as a parameter, whereas {prog.MXPClickableFunctionName()} does not.");
			return false;
		}

		OnPrisonerReleased = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This legal authority will now execute the {OnPrisonerReleased.MXPClickableFunctionNameWithId()} prog when a prisoner is released.");
		return true;
	}

	private bool BuildingCommandImprisonedProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which prog do you want to execute when a prisoner is imprisoned? You can also specify 'none' to have none.");
			return false;
		}

		if (command.PeekSpeech().EqualToAny("none", "clear", "remove"))
		{
			OnPrisonerImprisoned = null;
			Changed = true;
			actor.OutputHandler.Send(
				$"The legal authority will no longer execute a prog when a prisoner is imprisoned.");
			return true;
		}

		var prog = Gameworld.FutureProgs.GetByIdOrName(command.SafeRemainingArgument);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (!prog.MatchesParameters(new List<FutureProgVariableTypes> { FutureProgVariableTypes.Character }))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that accepts a single character as a parameter, whereas {prog.MXPClickableFunctionName()} does not.");
			return false;
		}

		OnPrisonerImprisoned = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This legal authority will now execute the {OnPrisonerReleased.MXPClickableFunctionNameWithId()} prog when a prisoner is imprisoned.");
		return true;
	}

	private bool BuildingCommandPatrol(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"You must either specify the ADD or DELETE keywords, or the name or ID of a patrol route to edit further.");
			return false;
		}

		if (command.PeekSpeech().EqualToAny("add", "new", "create"))
		{
			command.PopSpeech();
			if (command.IsFinished)
			{
				actor.OutputHandler.Send("What name do you want to give to your new patrol route?");
				return false;
			}

			var name = command.SafeRemainingArgument.TitleCase();
			if (_patrolRoutes.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					"There is already a patrol route for this legal authority with that name. Patrol route names must be unique.");
				return false;
			}

			var newRoute = new PatrolRoute(this, name);
			_patrolRoutes.Add(newRoute);
			actor.OutputHandler.Send(
				$"You create a new patrol route called {name.ColourName()} with ID #{newRoute.Id.ToString("N0", actor)}.");
			return true;
		}

		if (command.PopSpeech().EqualToAny("delete", "del", "remove", "rem"))
		{
			command.PopSpeech();
			if (command.IsFinished)
			{
				actor.OutputHandler.Send("Which patrol route do you want to delete?");
				return false;
			}

			var route = long.TryParse(command.PopSpeech(), out var value)
				? _patrolRoutes.FirstOrDefault(x => x.Id == value)
				: _patrolRoutes.FirstOrDefault(x => x.Name.EqualTo(command.Last)) ??
				  _patrolRoutes.FirstOrDefault(x =>
					  x.Name.StartsWith(command.Last, StringComparison.InvariantCultureIgnoreCase));
			if (route == null)
			{
				actor.OutputHandler.Send("There is no such patrol route.");
				return false;
			}

			actor.OutputHandler.Send(
				$"Are you sure you want to delete patrol route #{route.Id.ToString("N0", actor)} ({route.Name.ColourName()})? This action is permanent and cannot be undone.\n\t{Accept.StandardAcceptPhrasing}");
			actor.AddEffect(new Accept(actor, new GenericProposal
			{
				AcceptAction = text =>
				{
					_patrolRoutes.Remove(route);
					route.Delete();
					actor.OutputHandler.Send($"You delete the {route.Name.ColourName()} patrol route.");
				},
				RejectAction = text =>
				{
					actor.OutputHandler.Send(
						$"You decide not to delete the {route.Name.ColourName()} patrol route.");
				},
				ExpireAction = () =>
				{
					actor.OutputHandler.Send(
						$"You decide not to delete the {route.Name.ColourName()} patrol route.");
				},
				Keywords = new List<string> { "delete", "patrol", "route" },
				DescriptionString = $"Deleting the patrol route named {route.Name.ColourName()}."
			}), TimeSpan.FromSeconds(120));
		}

		var which = long.TryParse(command.Last, out var id)
			? _patrolRoutes.FirstOrDefault(x => x.Id == id)
			: _patrolRoutes.FirstOrDefault(x => x.Name.EqualTo(command.Last)) ??
			  _patrolRoutes.FirstOrDefault(x =>
				  x.Name.StartsWith(command.Last, StringComparison.InvariantCultureIgnoreCase));
		if (which == null)
		{
			actor.OutputHandler.Send("There is no such patrol route as that.");
			return false;
		}

		return which.BuildingCommand(actor, command);
	}

	private bool BuildingCommandInflate(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What percentage increase do you want to apply to all fines? e.g. {0.1.ToString("P0", actor).ColourValue()}");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(out var value))
		{
			actor.OutputHandler.Send(
				$"You must enter a valid percentage to increase all fines by, for example, {0.1.ToString("P0", actor).ColourValue()}.");
			return false;
		}

		var dValue = Convert.ToDecimal(value);
		foreach (var law in Laws)
		{
			law.ApplyInflation(dValue);
		}

		actor.OutputHandler.Send(
			$"You have successfully applied an inflation of {value.ToString("P2", actor).ColourValue()} to all fines.");
		return true;
	}

	private bool BuildingCommandCurrency(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which currency should this legal authority use for payment of fines and bonds?");
			return false;
		}

		var currency = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.Currencies.Get(value)
			: Gameworld.Currencies.GetByName(command.Last);
		if (currency == null)
		{
			actor.OutputHandler.Send("There is no such currency.");
			return false;
		}

		if (Currency == currency)
		{
			actor.OutputHandler.Send(
				$"The currency for this legal authority is already set to {currency.Name.Colour(Telnet.Cyan)}.");
			return false;
		}

		var sb = new StringBuilder();
		sb.AppendLine(
			$"You change the currency for this legal authority to {currency.Name.Colour(Telnet.Cyan)}.\nNote: Due to the currency change, you may find fines for laws are now completely wrong as the base values of these were not updated. If you know the conversion between these two currencies, you can bulk update the prices with the LEGAL SET INFLATE building subcommand for this authority.");
		Currency = currency;
		Changed = true;
		actor.OutputHandler.Send(sb.ToString().Wrap(actor.InnerLineFormatLength));
		return true;
	}

	private bool BuildingCommandLaw(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You can either specify the {"new".ColourCommand()} or {"delete".ColourCommand()} keywords, or specify an ID to edit an existing law.");
			return false;
		}

		var cmd = command.PopSpeech();
		if (cmd.EqualToAny("new", "create", "add"))
		{
			if (command.IsFinished)
			{
				actor.OutputHandler.Send("What name do you want to give to this new law?");
				return false;
			}

			var newName = command.PopSpeech().TitleCase();
			if (_laws.Any(x => x.Name.EqualTo(newName)))
			{
				actor.OutputHandler.Send("There is already a law with that name. Law names must be unique.");
				return false;
			}

			if (command.IsFinished)
			{
				actor.OutputHandler.Send(
					$"What crime type do you want your new law to apply to? Valid types are: {Enum.GetValues<CrimeTypes>().Select(x => x.DescribeEnum().ColourValue()).OrderBy(x => x).ListToCommaSeparatedValues(", ")}");
				return false;
			}

			if (!command.SafeRemainingArgument.TryParseEnum<CrimeTypes>(out var crime))
			{
				actor.OutputHandler.Send(
					$"That is not a valid crime type. Valid types are: {Enum.GetValues<CrimeTypes>().Select(x => x.DescribeEnum().ColourValue()).OrderBy(x => x).ListToCommaSeparatedValues(", ")}");
				return false;
			}

			var newLaw = new Law(newName, this, crime);
			AddLaw(newLaw);
			actor.OutputHandler.Send(
				$"You create a new crime called {newName.Colour(Telnet.Cyan)} (ID #{newLaw.Id.ToString("N0", actor)}) for the {crime.DescribeEnum().ColourValue()} crime type.");
			return true;
		}

		if (cmd.EqualToAny("delete", "del", "remove", "remove"))
		{
			if (command.IsFinished)
			{
				actor.OutputHandler.Send("Which law do you want to delete?");
				return false;
			}

			var law = long.TryParse(command.PopSpeech(), out var lvalue)
				? _laws.FirstOrDefault(x => x.Id == lvalue)
				: _laws.FirstOrDefault(x => x.Name.EqualTo(command.Last)) ?? _laws.FirstOrDefault(x =>
					x.Name.StartsWith(command.Last, StringComparison.InvariantCultureIgnoreCase));
			if (law == null)
			{
				actor.OutputHandler.Send("There is no such law.");
				return false;
			}

			actor.OutputHandler.Send(
				$"Are you sure you want to delete the {law.Name.Colour(Telnet.Green)} law? This action is permanent, and will delete all associated crimes.");
			actor.AddEffect(new Accept(actor, new GenericProposal
			{
				AcceptAction = text =>
				{
					actor.OutputHandler.Send($"You delete the {law.Name.Colour(Telnet.Green)} law.");
					RemoveLaw(law);
					law.Delete();
				},
				RejectAction = text =>
				{
					actor.OutputHandler.Send($"You decide not to delete the {law.Name.Colour(Telnet.Green)} law.");
				},
				ExpireAction = () =>
				{
					actor.OutputHandler.Send($"You decide not to delete the {law.Name.Colour(Telnet.Green)} law.");
				},
				DescriptionString = $"Deleting the {law.Name.Colour(Telnet.Green)} law.",
				Keywords = new List<string> { "delete", "enforcement", "law" }
			}), TimeSpan.FromSeconds(120));
			return true;
		}

		var editingLaw = long.TryParse(cmd, out var value)
			? _laws.FirstOrDefault(x => x.Id == value)
			: _laws.FirstOrDefault(x => x.Name.EqualTo(cmd)) ?? _laws.FirstOrDefault(x =>
				x.Name.StartsWith(cmd, StringComparison.InvariantCultureIgnoreCase));
		if (editingLaw == null)
		{
			actor.OutputHandler.Send("This legal authority has no such law.");
			return false;
		}

		return editingLaw.BuildingCommand(actor, command);
	}

	private bool BuildingCommandEnforcement(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You can either specify the {"new".ColourCommand()} or {"delete".ColourCommand()} keywords, or specify an ID to edit an existing enforcement authority.");
			return false;
		}

		var cmd = command.PopSpeech();
		if (cmd.EqualToAny("new", "create", "add"))
		{
			if (command.IsFinished)
			{
				actor.OutputHandler.Send("What name do you want to give to the new enforcement authority?");
				return false;
			}

			var newName = command.SafeRemainingArgument.TitleCase();
			if (_enforcementAuthorities.Any(x => x.Name.EqualTo(newName)))
			{
				actor.OutputHandler.Send(
					"There is already an enforcement authority with that name. Enforcement authorities must have unique names.");
				return false;
			}

			var newAuthority = new EnforcementAuthority(newName, this);
			_enforcementAuthorities.Add(newAuthority);
			actor.OutputHandler.Send(
				$"You create a new enforcement authority called {newName.Colour(Telnet.Cyan)} with ID #{newAuthority.Id.ToString("N0", actor)}.");
			Changed = true;
			return true;
		}

		if (cmd.EqualToAny("delete", "del", "remove", "remove"))
		{
			if (command.IsFinished)
			{
				actor.OutputHandler.Send("Which enforcement authority do you want to delete?");
				return false;
			}

			var enforcement = long.TryParse(command.PopSpeech(), out var lvalue)
				? _enforcementAuthorities.FirstOrDefault(x => x.Id == lvalue)
				: _enforcementAuthorities.FirstOrDefault(x => x.Name.EqualTo(command.Last)) ??
				  _enforcementAuthorities.FirstOrDefault(x =>
					  x.Name.StartsWith(command.Last, StringComparison.InvariantCultureIgnoreCase));
			if (enforcement == null)
			{
				actor.OutputHandler.Send("There is no such enforcement authority.");
				return false;
			}

			actor.OutputHandler.Send(
				$"Are you sure you want to delete the {enforcement.Name.Colour(Telnet.Green)} enforcement authority? This action is permanent, and will delete all associated laws and crimes.");
			actor.AddEffect(new Accept(actor, new GenericProposal
			{
				AcceptAction = text =>
				{
					actor.OutputHandler.Send(
						$"You delete the {enforcement.Name.Colour(Telnet.Green)} enforcement authority.");
					_enforcementAuthorities.Remove(enforcement);
					enforcement.Delete();
					// TODO - anything to do with stopping existing enforcers?
				},
				RejectAction = text =>
				{
					actor.OutputHandler.Send(
						$"You decide not to delete the {enforcement.Name.Colour(Telnet.Green)} enforcement authority.");
				},
				ExpireAction = () =>
				{
					actor.OutputHandler.Send(
						$"You decide not to delete the {enforcement.Name.Colour(Telnet.Green)} enforcement authority.");
				},
				DescriptionString = $"Deleting the {enforcement.Name.Colour(Telnet.Green)} enforcement authority.",
				Keywords = new List<string> { "delete", "enforcement", "authority", "enforcementauthority" }
			}), TimeSpan.FromSeconds(120));
			return true;
		}

		var editingenforcement = long.TryParse(cmd, out var value)
			? _enforcementAuthorities.FirstOrDefault(x => x.Id == value)
			: _enforcementAuthorities.FirstOrDefault(x => x.Name.EqualTo(cmd)) ??
			  _enforcementAuthorities.FirstOrDefault(x =>
				  x.Name.StartsWith(cmd, StringComparison.InvariantCultureIgnoreCase));
		if (editingenforcement == null)
		{
			actor.OutputHandler.Send("This legal authority has no such enforcement authority.");
			return false;
		}

		return editingenforcement.BuildingCommand(actor, command);
	}

	private bool BuildingCommandClass(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You can either specify the {"new".ColourCommand()} or {"delete".ColourCommand()} keywords, or specify an ID to edit an existing legal class.");
			return false;
		}

		var cmd = command.PopSpeech();
		if (cmd.EqualToAny("new", "create", "add"))
		{
			if (command.IsFinished)
			{
				actor.OutputHandler.Send("What name do you want to give to the new legal class?");
				return false;
			}

			var newName = command.SafeRemainingArgument.TitleCase();
			if (_legalClasses.Any(x => x.Name.EqualTo(newName)))
			{
				actor.OutputHandler.Send(
					"There is already a legal class with that name. Legal classes must have unique names.");
				return false;
			}

			if (command.IsFinished)
			{
				actor.OutputHandler.Send("Which prog do you want to use to determine membership in this legal class?");
				return false;
			}

			var prog = long.TryParse(command.PopSpeech(), out var progid)
				? Gameworld.FutureProgs.Get(progid)
				: Gameworld.FutureProgs.GetByName(command.Last);
			if (prog == null)
			{
				actor.OutputHandler.Send("There is no such prog.");
				return false;
			}

			if (!prog.ReturnType.CompatibleWith(FutureProgVariableTypes.Boolean))
			{
				actor.OutputHandler.Send("You must supply a prog that returns a boolean.");
				return false;
			}

			if (!prog.MatchesParameters(new[] { FutureProgVariableTypes.Character }))
			{
				actor.OutputHandler.Send("You must supply a prog that accepts a single character parameter.");
				return false;
			}

			var newClass = new LegalClass(newName, this, prog);
			_legalClasses.Add(newClass);
			actor.OutputHandler.Send(
				$"You create a new legal class called {newClass.Name.Colour(Telnet.Cyan)}, with ID #{newClass.Id.ToString("N0", actor)} that uses the {prog.MXPClickableFunctionNameWithId()} prog to determine membership.");
			Changed = true;
			return true;
		}

		if (cmd.EqualToAny("delete", "del", "remove", "remove"))
		{
			if (command.IsFinished)
			{
				actor.OutputHandler.Send("Which legal class do you want to delete?");
				return false;
			}

			var legalClass = long.TryParse(command.PopSpeech(), out var lvalue)
				? _legalClasses.FirstOrDefault(x => x.Id == lvalue)
				: _legalClasses.FirstOrDefault(x => x.Name.EqualTo(command.Last)) ?? _legalClasses.FirstOrDefault(x =>
					x.Name.StartsWith(command.Last, StringComparison.InvariantCultureIgnoreCase));
			if (legalClass == null)
			{
				actor.OutputHandler.Send("There is no such legal class.");
				return false;
			}

			actor.OutputHandler.Send(
				$"Are you sure you want to delete the {legalClass.Name.Colour(Telnet.Green)} legal class? This action is permanent, and will delete all associated laws and crimes.");
			actor.AddEffect(new Accept(actor, new GenericProposal
			{
				AcceptAction = text =>
				{
					actor.OutputHandler.Send($"You delete the {legalClass.Name.Colour(Telnet.Green)} legal class.");
					_legalClasses.Remove(legalClass);
					legalClass.Delete();
					foreach (var law in _laws.ToList())
					{
						law.RemoveAllReferencesTo(legalClass);
					}

					foreach (var enforcement in _enforcementAuthorities)
					{
						enforcement.RemoveAllReferencesTo(legalClass);
					}
				},
				RejectAction = text =>
				{
					actor.OutputHandler.Send(
						$"You decide not to delete the {legalClass.Name.Colour(Telnet.Green)} legal class.");
				},
				ExpireAction = () =>
				{
					actor.OutputHandler.Send(
						$"You decide not to delete the {legalClass.Name.Colour(Telnet.Green)} legal class.");
				},
				DescriptionString = $"Deleting the {legalClass.Name.Colour(Telnet.Green)} legal class.",
				Keywords = new List<string> { "delete", "legal", "class", "legalclass" }
			}), TimeSpan.FromSeconds(120));
			return true;
		}

		var legal = long.TryParse(cmd, out var value)
			? _legalClasses.FirstOrDefault(x => x.Id == value)
			: _legalClasses.FirstOrDefault(x => x.Name.EqualTo(cmd)) ?? _legalClasses.FirstOrDefault(x =>
				x.Name.StartsWith(cmd, StringComparison.InvariantCultureIgnoreCase));
		if (legal == null)
		{
			actor.OutputHandler.Send("This legal authority has no such legal class.");
			return false;
		}

		return legal.BuildingCommand(actor, command);
	}

	private bool BuildingCommandZone(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which zone do you want to add or remove from this legal authority's enforcement area?");
			return false;
		}

		var zone = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.Zones.Get(value)
			: Gameworld.Zones.GetByName(command.Last);
		if (zone == null)
		{
			actor.OutputHandler.Send("There is no such zone.");
			return false;
		}

		if (_enforcementZones.Contains(zone))
		{
			_enforcementZones.Remove(zone);
			actor.OutputHandler.Send(
				$"This legal authority will no longer enforce its laws in the {zone.Name.Colour(Telnet.Cyan)} zone.");
			Changed = true;
			return true;
		}

		_enforcementZones.Add(zone);
		actor.OutputHandler.Send(
			$"This legal authority will now enforce its laws in the {zone.Name.Colour(Telnet.Cyan)} zone.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What did you want to rename this legal authority?");
			return false;
		}

		var name = command.PopSpeech().TitleCase();
		if (Gameworld.LegalAuthorities.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				"There is already a legal authority so-named. Legal authority names must be unique.");
			return false;
		}

		_name = name;
		Changed = true;
		return true;
	}

	private bool BuildingCommandPrepareLocation(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which location do you want to set as the preparing location for enforcer patrols in this legal authority's area? This would typically be an armoury or somewhere they could find their equipment.");
			return false;
		}

		if (command.PeekSpeech().EqualTo("here"))
		{
			PreparingLocation = actor.Location;
			Changed = true;
			actor.OutputHandler.Send(
				$"You set your current location as the enforcer preparing location for patrols in the {Name.ColourName()} legal authority.");
			return true;
		}

		if (!long.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send(
				$"You must either use the keyword {"here".ColourCommand()} or specify the ID# of a room.");
			return false;
		}

		var location = Gameworld.Cells.Get(value);
		if (location == null)
		{
			actor.OutputHandler.Send("There is no such location.");
			return false;
		}

		PreparingLocation = location;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {Name.ColourName()} legal authority will now use the location {location.HowSeen(actor)} as its preparation location for enforcer patrols.");
		return true;
	}

	private bool BuildingCommandMarshalLocation(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which location do you want to set as the marshal location for enforcer patrols in this legal authority's area? This is where patrols launch from once ready.");
			return false;
		}

		if (command.PeekSpeech().EqualTo("here"))
		{
			MarshallingLocation = actor.Location;
			Changed = true;
			actor.OutputHandler.Send(
				$"You set your current location as the enforcer marshal location for patrols in the {Name.ColourName()} legal authority.");
			return true;
		}

		if (!long.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send(
				$"You must either use the keyword {"here".ColourCommand()} or specify the ID# of a room.");
			return false;
		}

		var location = Gameworld.Cells.Get(value);
		if (location == null)
		{
			actor.OutputHandler.Send("There is no such location.");
			return false;
		}

		MarshallingLocation = location;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {Name.ColourName()} legal authority will now use the location {location.HowSeen(actor)} as its marshal location for enforcer patrols.");
		return true;
	}

	private bool BuildingCommandEnforcerStowingLocation(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which location do you want to set as the stowing location for enforcers in this legal authority's area? This is where enforcers will go when they're not on a patrol.");
			return false;
		}

		if (command.PeekSpeech().EqualTo("here"))
		{
			EnforcerStowingLocation = actor.Location;
			Changed = true;
			actor.OutputHandler.Send(
				$"You set your current location as the enforcer stowing location for enforcers in the {Name.ColourName()} legal authority.");
			return true;
		}

		if (!long.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send(
				$"You must either use the keyword {"here".ColourCommand()} or specify the ID# of a room.");
			return false;
		}

		var location = Gameworld.Cells.Get(value);
		if (location == null)
		{
			actor.OutputHandler.Send("There is no such location.");
			return false;
		}

		EnforcerStowingLocation = location;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {Name.ColourName()} legal authority will now use the location {location.HowSeen(actor)} as its stowing location for enforcers.");
		return true;
	}

	private bool BuildingCommandPrisonLocation(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which location do you want to set as the prison location for this legal authority? This is where prisoners will be dragged to before being thrown in a cell.");
			return false;
		}

		if (command.PeekSpeech().EqualTo("here"))
		{
			PrisonLocation = actor.Location;
			Changed = true;
			actor.OutputHandler.Send(
				$"You set your current location as the prison location for patrols in the {Name.ColourName()} legal authority.");
			return true;
		}

		if (!long.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send(
				$"You must either use the keyword {"here".ColourCommand()} or specify the ID# of a room.");
			return false;
		}

		var location = Gameworld.Cells.Get(value);
		if (location == null)
		{
			actor.OutputHandler.Send("There is no such location.");
			return false;
		}

		PrisonLocation = location;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {Name.ColourName()} legal authority will now use the location {location.HowSeen(actor)} as its prison location.");
		return true;
	}

	private bool BuildingCommandPrisonBelongingsLocation(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which location do you want to set as the prison belongings storage location for this legal authority? This is where bundles of belongings for prisoners will be stored.");
			return false;
		}

		if (command.PeekSpeech().EqualTo("here"))
		{
			PrisonerBelongingsStorageLocation = actor.Location;
			Changed = true;
			actor.OutputHandler.Send(
				$"You set your current location as the prison belongings storage location for patrols in the {Name.ColourName()} legal authority.");
			return true;
		}

		if (!long.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send(
				$"You must either use the keyword {"here".ColourCommand()} or specify the ID# of a room.");
			return false;
		}

		var location = Gameworld.Cells.Get(value);
		if (location == null)
		{
			actor.OutputHandler.Send("There is no such location.");
			return false;
		}

		PrisonerBelongingsStorageLocation = location;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {Name.ColourName()} legal authority will now use the location {location.HowSeen(actor)} as its prison belongings storage location.");
		return true;
	}

	private bool BuildingCommandPrisonReleaseLocation(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which location do you want to set as the prison release location for this legal authority? This is where prisoners will be released from their cells.");
			return false;
		}

		if (command.PeekSpeech().EqualTo("here"))
		{
			PrisonReleaseLocation = actor.Location;
			Changed = true;
			actor.OutputHandler.Send(
				$"You set your current location as the prison release location for patrols in the {Name.ColourName()} legal authority.");
			return true;
		}

		if (!long.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send(
				$"You must either use the keyword {"here".ColourCommand()} or specify the ID# of a room.");
			return false;
		}

		var location = Gameworld.Cells.Get(value);
		if (location == null)
		{
			actor.OutputHandler.Send("There is no such location.");
			return false;
		}

		PrisonReleaseLocation = location;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {Name.ColourName()} legal authority will now use the location {location.HowSeen(actor)} as its prison release location.");
		return true;
	}

	private bool BuildingCommandCellLocation(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which location do you want to toggle as a holding cell for this legal authority?");
			return false;
		}

		ICell location;
		if (command.PeekSpeech().EqualTo("here"))
		{
			location = actor.Location;
		}
		else
		{
			if (!long.TryParse(command.SafeRemainingArgument, out var value))
			{
				actor.OutputHandler.Send(
					$"You must either use the keyword {"here".ColourCommand()} or specify the ID# of a room.");
				return false;
			}

			location = Gameworld.Cells.Get(value);
			if (location == null)
			{
				actor.OutputHandler.Send("There is no such location.");
				return false;
			}
		}

		if (CellLocations.Contains(location))
		{
			_cellLocations.Remove(location);
			actor.OutputHandler.Send(
				$"The {Name.ColourName()} legal authority will no longer use the location {location.HowSeen(actor)} as a holding cell.");
		}
		else
		{
			_cellLocations.Add(location);
			actor.OutputHandler.Send(
				$"The {Name.ColourName()} legal authority will now use the location {location.HowSeen(actor)} as a holding cell.");
		}

		Changed = true;
		return true;
	}

	private bool BuildingCommandPlayersKnowCrimes(ICharacter actor, StringStack command)
	{
		PlayersKnowTheirCrimes = !PlayersKnowTheirCrimes;
		Changed = true;
		actor.OutputHandler.Send(
			$"Players will {(PlayersKnowTheirCrimes ? "now" : "no longer")} know what crimes they have committed.");
		return true;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Legal Authority #{Id.ToString("N0", actor)} - {Name.Colour(Telnet.Cyan)}");
		sb.AppendLine();
		sb.AppendLine($"Legal Classes:");
		if (_legalClasses.Any())
		{
			foreach (var legal in _legalClasses)
			{
				sb.AppendLine($"\t#{legal.Id.ToString("N0", actor)}) {legal.Name.ColourName()}");
			}
		}
		else
		{
			sb.AppendLine("\tNone.");
		}

		sb.AppendLine();
		sb.AppendLine("Enforcement Authorities:");
		if (_enforcementAuthorities.Any())
		{
			foreach (var authority in _enforcementAuthorities)
			{
				sb.AppendLine($"\t#{authority.Id.ToString("N0", actor)}) {authority.Name.ColourName()}");
			}
		}
		else
		{
			sb.AppendLine("\tNone.");
		}

		sb.AppendLine();
		sb.AppendLine($"Enforcement Zones:");
		if (_enforcementZones.Any())
		{
			foreach (var zone in _enforcementZones)
			{
				sb.AppendLine($"\t#{zone.Id.ToString("N0", actor)}) {zone.Name.ColourName()}");
			}
		}
		else
		{
			sb.AppendLine("\tNone.");
		}

		sb.AppendLine();
		sb.AppendLine("Patrol Routes:");
		if (_patrolRoutes.Any())
		{
			foreach (var route in _patrolRoutes)
			{
				sb.AppendLine(
					$"\t#{route.Id.ToString("N0", actor)}) {route.Name.ColourName()}{(Patrols.Any(x => x.PatrolRoute == route) ? " [active]".Colour(Telnet.BoldBlue) : "")}"); // TODO - show if active
			}
		}
		else
		{
			sb.AppendLine("\tNone.");
		}

		sb.AppendLine();

		sb.AppendLine($"Laws In Force: {_laws.Count.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Known Crimes: {_knownCrimes.Count.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Unknown Crimes: {_unknownCrimes.Count.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Stale Crimes: {_staleCrimes.Count.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Resolved Crimes: {_resolvedCrimes.Count.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Players Know Their Crimes: {PlayersKnowTheirCrimes.ToColouredString()}");
		sb.AppendLine($"Auto Convict: {AutomaticallyConvict.ToColouredString()}");
		sb.AppendLine($"Auto Convict Timeframe: {AutomaticConvictionTime.Describe(actor).ColourValue()}");
		sb.AppendLine(
			$"Bail Calculation Prog: {BailCalculationProg?.MXPClickableFunctionName() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine($"Bank Account: {BankAccount?.AccountReference.ColourValue() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine(
			$"Discord Channel: {DiscordChannelId?.ToString("N", actor).ColourValue() ?? "None".ColourError()}");
		sb.AppendLine();
		sb.AppendLine($"On Arrest Prog: {OnPrisonerHeld?.MXPClickableFunctionName() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine(
			$"On Imprison Prog: {OnPrisonerImprisoned?.MXPClickableFunctionName() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine(
			$"On Release Prog: {OnPrisonerReleased?.MXPClickableFunctionName() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine();
		sb.AppendLine(
			$"Enforcer Stowing Location: {EnforcerStowingLocation?.GetFriendlyReference(actor) ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine(
			$"Patrol Preparation Location: {PreparingLocation?.GetFriendlyReference(actor) ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine(
			$"Patrol Marshalling Location: {MarshallingLocation?.GetFriendlyReference(actor) ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine($"Prison Location: {PrisonLocation?.GetFriendlyReference(actor) ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine(
			$"Prison Release Location: {PrisonReleaseLocation?.GetFriendlyReference(actor) ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine(
			$"Prison Belongings Location: {PrisonerBelongingsStorageLocation?.GetFriendlyReference(actor) ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine($"Courtroom: {CourtLocation?.GetFriendlyReference(actor) ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine($"Jail Entry: {JailLocation?.GetFriendlyReference(actor) ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine();
		sb.AppendLine($"Holding Cells:");
		foreach (var cell in _cellLocations)
		{
			sb.AppendLine($"\t{cell.GetFriendlyReference(actor)}");
		}

		sb.AppendLine();
		sb.AppendLine($"Jail Locations:");
		foreach (var cell in _jailLocations)
		{
			sb.AppendLine($"\t{cell.GetFriendlyReference(actor)}");
		}

		return sb.ToString();
	}

	#endregion
}