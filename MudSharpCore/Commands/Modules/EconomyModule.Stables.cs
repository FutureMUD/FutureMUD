using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Commands.Trees;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Economy.Banking;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Stables;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable enable

namespace MudSharp.Commands.Modules;

internal partial class EconomyModule
{
	private const string StableAccountListFilterHelp = @"You can use the following filters with #3stable account list#0:

	#3suspended#0 - only suspended accounts
	#3active#0 - only non-suspended accounts
	#3overlimit#0 or #3over limit#0 - only accounts beyond their credit limit
	#3owing#0 - only accounts with a negative balance
	#3prepaid#0 or #3credit#0 - only accounts with a positive balance
	#3owner <text>#0 - owner name contains the text
	#3name <text>#0 - account name contains the text
	#3user <text>#0 - authorised-user name contains the text
	#3search <text>#0 - account, owner, or authorised-user name contains the text";

	private const string StableHelp = @"You can use the following options with the stable command:

	#3stable#0 - shows information about the stable here
	#3stable quote <mount>#0 - shows the price to lodge a mount
	#3stable lodge <mount> [cash|account <account>|with <payment item>]#0 - stables a mount and gives you a ticket
	#3stable redeem <ticket> [cash|account <account>|with <payment item>]#0 - redeems a stabled mount
	#3stable accounts#0 - show accounts that you have access to at this stable
	#3stable accountstatus <account>#0 - shows your stable account
	#3stable payaccount <account> <amount> [cash|with <payment item>]#0 - pays or prepays a stable account

Stable managers can use the following additional commands:

	#3stable list [active|history]#0 - lists stabled mounts
	#3stable show <stay>#0 - shows a stable stay and ledger
	#3stable release <stay> [waive]#0 - releases a mount without a ticket
	#3stable bank <account|none>#0 - sets the stable bank account for proprietors
	#3stable deposit <amount>#0 - deposits held cash into the stable's virtual cash balance
	#3stable withdraw <amount>#0 - withdraws cash from the stable's virtual cash balance and bank fallback
	#3stable ledger [count]#0 - reviews stable cash ledger entries
	#3stable fee lodge|daily <amount|prog <prog>|none>#0 - sets stable fees for proprietors
	#3stable account list [<filters>]#0 - lists all accounts, optionally filtered
	#3stable account show <id|name>#0 - shows a particular credit account
	#3stable account create <name> <ownership> <credit limit>#0 - creates a new credit account for a customer
	#3stable account limit <id|name> <limit>#0 - sets a new credit limit for an account
	#3stable account suspend <id|name>#0 - toggles suspension of a credit account
	#3stable account authorise <id|name> <person> <limit>#0 - authorises an additional person to access a credit account
	#3stable account unauthorise <id|name> <person>#0 - removes an authorisation of an additional person on a credit account
	#3stable employ|fire|manager|proprietor <target|name>#0 - manages employees
	#3stable open|close#0 - opens or closes the stable
	#3stable set can <prog|none> [whyprog]#0 - sets access progs

Administrators can also use:
	#3stable create <name> <economic zone> <bank account|none>#0 - creates a stable at your current location
	#3stable delete#0 - deletes the stable at your current location if it has no active stays
	#3stable list all#0 - lists all stables";

	[PlayerCommand("Stable", "stable")]
	[RequiredCharacterState(CharacterState.Conscious)]
	[NoCombatCommand]
	[NoHideCommand]
	[HelpInfo("stable", StableHelp, AutoHelp.HelpArg)]
	protected static void Stable(ICharacter actor, string command)
	{
		StringStack ss = new(command.RemoveFirstWord());
		var subcommand = ss.PopSpeech().ToLowerInvariant();
		switch (subcommand)
		{
			case "":
			case "info":
				StableInfo(actor, ss);
				return;
			case "quote":
				StableQuote(actor, ss);
				return;
			case "lodge":
				StableLodge(actor, ss);
				return;
			case "redeem":
				StableRedeem(actor, ss);
				return;
			case "accountstatus":
				StableAccountStatus(actor, ss);
				return;
			case "payaccount":
				StablePayAccount(actor, ss);
				return;
			case "list":
				StableList(actor, ss);
				return;
			case "show":
				StableShowStay(actor, ss);
				return;
			case "release":
				StableRelease(actor, ss);
				return;
			case "bank":
				StableBank(actor, ss);
				return;
			case "deposit":
				StableDeposit(actor, ss);
				return;
			case "withdraw":
				StableWithdraw(actor, ss);
				return;
			case "ledger":
				StableLedger(actor, ss);
				return;
			case "fee":
				StableFee(actor, ss);
				return;
			case "account":
				StableAccount(actor, ss);
				return;
			case "accounts":
				StableAccounts(actor, ss);
				return;
			case "employ":
				StableEmploy(actor, ss);
				return;
			case "fire":
				StableFire(actor, ss);
				return;
			case "manager":
				StableManager(actor, ss);
				return;
			case "proprietor":
				StableProprietor(actor, ss);
				return;
			case "open":
				StableOpen(actor);
				return;
			case "close":
				StableClose(actor);
				return;
			case "set":
				StableSet(actor, ss);
				return;
			case "create" when actor.IsAdministrator():
				StableCreate(actor, ss);
				return;
			case "delete" when actor.IsAdministrator():
				StableDelete(actor, ss);
				return;
		}

		actor.OutputHandler.Send(StableHelp.SubstituteANSIColour());
	}

	private static bool DoStableCommandFindStable(ICharacter actor, out IStable stable)
	{
		stable = actor.Gameworld.Stables.FirstOrDefault(x => x.Location == actor.Location)!;
		if (stable is not null)
		{
			return true;
		}

		actor.OutputHandler.Send("You are not currently at a stable.");
		stable = null!;
		return false;
	}

	private static void StableInfo(ICharacter actor, StringStack ss)
	{
		if (!DoStableCommandFindStable(actor, out var stable))
		{
			return;
        }

        if (!stable.IsManager(actor))
        {
            actor.OutputHandler.Send(stable.ShowToNonEmployee(actor));
            return;
        }

        actor.OutputHandler.Send(stable.Show(actor));
	}

	private static void StableQuote(ICharacter actor, StringStack ss)
	{
		if (!DoStableCommandFindStable(actor, out var stable))
		{
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which mount do you want a stabling quote for?");
			return;
		}

		var mount = actor.TargetActor(ss.PopSpeech());
		if (mount is null)
		{
			actor.OutputHandler.Send("You do not see any such mount.");
			return;
		}

		var can = stable.CanLodge(actor, mount);
		if (!can.Truth)
		{
			actor.OutputHandler.Send(can.Reason);
			return;
		}

		actor.OutputHandler.Send(
			$"{stable.Name.TitleCase().ColourName()} would charge {stable.Currency.Describe(stable.QuoteLodgeFee(mount, actor), CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} to lodge {mount.HowSeen(actor)} and {stable.Currency.Describe(stable.QuoteDailyFee(mount, actor), CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} per day after that.");
	}

	private static void StableLodge(ICharacter actor, StringStack ss)
	{
		if (!DoStableCommandFindStable(actor, out var stable))
		{
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which mount do you want to stable?");
			return;
		}

		var mount = actor.TargetActor(ss.PopSpeech());
		if (mount is null)
		{
			actor.OutputHandler.Send("You do not see any such mount.");
			return;
		}

		var can = stable.CanLodge(actor, mount);
		if (!can.Truth)
		{
			actor.OutputHandler.Send(can.Reason);
			return;
		}

		if (!ParseStablePayment(actor, stable, ss, out var payment, out var error))
		{
			actor.OutputHandler.Send(error);
			return;
		}

		var price = stable.QuoteLodgeFee(mount, actor);
		if (!CanTakeStablePayment(actor, stable, payment, price, out error))
		{
			actor.OutputHandler.Send(error);
			return;
		}

		var stay = stable.Lodge(actor, mount);
		TakeStablePayment(actor, stable, payment, price, $"Stabling {mount.HowSeen(actor, colour: false)}", stay);
		var ticket = StableTicketGameItemComponentProto.CreateNewStableTicket(stay);
		GiveStableTicket(actor, ticket);
		actor.OutputHandler.Handle(new EmoteOutput(new Emote($"@ stable|stables $1 with {stable.Name.TitleCase().ColourName()} and receive|receives $2.", actor, actor, mount, ticket)));
		mount.Quit(true);
	}

	private static void StableRedeem(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which stable ticket do you want to redeem?");
			return;
		}

		var item = actor.TargetItem(ss.PopSpeech());
		var ticket = item?.GetItemType<IStableTicket>();
		if (ticket is null)
		{
			actor.OutputHandler.Send("You do not have any such stable ticket.");
			return;
		}

		if (ticket.StableStay is null)
		{
			actor.OutputHandler.Send("That stable ticket does not correspond to any known stable stay.");
			return;
		}

		var stable = ticket.StableStay.Stable;
		var can = stable.CanRedeem(actor, ticket);
		if (!can.Truth)
		{
			actor.OutputHandler.Send(can.Reason);
			return;
		}

		stable.AssessFees(ticket.StableStay);
		if (!ParseStablePayment(actor, stable, ss, out var payment, out var error))
		{
			actor.OutputHandler.Send(error);
			return;
		}

		var price = ticket.StableStay.AmountOwing;
		if (!CanTakeStablePayment(actor, stable, payment, price, out error))
		{
			actor.OutputHandler.Send(error);
			return;
		}

		TakeStablePayment(actor, stable, payment, price, "Stable stay redemption", ticket.StableStay);
		stable.Redeem(actor, ticket.StableStay);
		actor.OutputHandler.Send($"You redeem {item!.HowSeen(actor)} and settle {stable.Currency.Describe(price, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} in outstanding fees.");
		ticket.Parent.Delete();
	}

	private static void StableList(ICharacter actor, StringStack ss)
	{
		if (!ss.IsFinished && ss.PeekSpeech().EqualTo("all") && actor.IsAdministrator())
		{
			actor.OutputHandler.Send(StringUtilities.GetTextTable(
				from st in actor.Gameworld.Stables
				select new[]
				{
					st.Id.ToString("N0", actor),
					st.Name,
					st.EconomicZone.Name,
					st.Location.Id.ToString("N0", actor),
					st.IsTrading.ToString(actor),
					st.ActiveStays.Count().ToString("N0", actor)
				},
				new[] { "#", "Name", "Zone", "Cell", "Open", "Active" },
				actor.LineFormatLength,
				colour: Telnet.Yellow,
				unicodeTable: actor.Account.UseUnicode));
			return;
		}

		if (!DoStableCommandFindStable(actor, out var stable))
		{
			return;
		}

		if (!stable.IsManager(actor))
		{
			actor.OutputHandler.Send("You are not a manager of this stable.");
			return;
		}

		var history = !ss.IsFinished && ss.PeekSpeech().EqualTo("history");
		var stays = (history ? stable.Stays.Where(x => !x.IsActive) : stable.ActiveStays).ToList();
		foreach (var stay in stays)
		{
			stable.AssessFees(stay);
		}

		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from stay in stays
			select new[]
			{
				stay.Id.ToString("N0", actor),
				stay.Mount?.HowSeen(actor, colour: false) ?? $"#{stay.MountId:N0}",
				stay.OriginalOwnerName?.GetName(NameStyle.FullName) ?? $"#{stay.OriginalOwnerId:N0}",
				stay.Status.DescribeEnum(),
				stable.Currency.Describe(stay.AmountOwing, CurrencyDescriptionPatternType.ShortDecimal)
			},
			new[] { "#", "Mount", "Lodger", "Status", "Owing" },
			actor.LineFormatLength,
			colour: Telnet.Yellow,
			unicodeTable: actor.Account.UseUnicode));
	}

	private static void StableShowStay(ICharacter actor, StringStack ss)
	{
		if (!DoStableCommandFindStable(actor, out var stable))
		{
			return;
		}

		if (!stable.IsManager(actor))
		{
			actor.OutputHandler.Send("You are not a manager of this stable.");
			return;
		}

		if (ss.IsFinished || !long.TryParse(ss.PopSpeech(), out var id))
		{
			actor.OutputHandler.Send("Which stable stay do you want to see?");
			return;
		}

		var stay = stable.Stays.FirstOrDefault(x => x.Id == id);
		if (stay is null)
		{
			actor.OutputHandler.Send("There is no such stay at this stable.");
			return;
		}

		actor.OutputHandler.Send(stable.ShowStay(actor, stay));
	}

	private static void StableRelease(ICharacter actor, StringStack ss)
	{
		if (!DoStableCommandFindStable(actor, out var stable))
		{
			return;
		}

		if (!stable.IsManager(actor))
		{
			actor.OutputHandler.Send("You are not a manager of this stable.");
			return;
		}

		if (ss.IsFinished || !long.TryParse(ss.PopSpeech(), out var id))
		{
			actor.OutputHandler.Send("Which stable stay do you want to release?");
			return;
		}

		var stay = stable.ActiveStays.FirstOrDefault(x => x.Id == id);
		if (stay is null)
		{
			actor.OutputHandler.Send("There is no such active stay at this stable.");
			return;
		}

		var waive = !ss.IsFinished && ss.PopSpeech().EqualTo("waive");
		stable.Release(actor, stay, waive);
		actor.OutputHandler.Send(
			$"You release {stay.Mount?.HowSeen(actor) ?? $"mount #{stay.MountId:N0}"} from {stable.Name.TitleCase().ColourName()}{(waive ? " and waive all outstanding fees" : string.Empty)}.");
	}

	private static void StableBank(ICharacter actor, StringStack ss)
	{
		if (!DoStableCommandFindStable(actor, out var stable))
		{
			return;
		}

		if (!stable.IsProprietor(actor))
		{
			actor.OutputHandler.Send("You are not a proprietor of this stable.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which bank account should this stable use, or NONE?");
			return;
		}

		if (ss.PeekSpeech().EqualTo("none"))
		{
			stable.BankAccount = null;
			actor.OutputHandler.Send("This stable no longer has a bank account.");
			return;
		}

		var (account, findError) = MudSharp.Economy.Banking.Bank.FindBankAccount(ss.SafeRemainingArgument, null, actor);
		if (account is null)
		{
			actor.OutputHandler.Send(findError);
			return;
		}

		if (!actor.IsAdministrator() && !account.IsAuthorisedAccountUser(actor))
		{
			actor.OutputHandler.Send("You are not authorised to use that bank account.");
			return;
		}

		if (account.Currency != stable.Currency)
		{
			actor.OutputHandler.Send($"That bank account is not in {stable.Currency.Name.ColourName()}.");
			return;
		}

		stable.BankAccount = account;
		actor.OutputHandler.Send($"This stable will now use {account.AccountReference.ColourValue()} for receipts.");
	}

	private static void StableDeposit(ICharacter actor, StringStack ss)
	{
		if (!DoStableCommandFindStable(actor, out var stable))
		{
			return;
		}

		if (!stable.IsProprietor(actor))
		{
			actor.OutputHandler.Send("You are not a proprietor of this stable.");
			return;
		}

		if (ss.IsFinished || !stable.Currency.TryGetBaseCurrency(ss.SafeRemainingArgument, out var amount) || amount <= 0.0M)
		{
			actor.OutputHandler.Send($"How much {stable.Currency.Name.ColourName()} do you want to deposit?");
			return;
		}

		if (AccessibleStableCash(actor, stable.Currency) < amount)
		{
			actor.OutputHandler.Send(
				$"You do not have {stable.Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} in accessible cash.");
			return;
		}

		TakeStableCash(actor, stable, amount, "Manager cash deposit");
		actor.OutputHandler.Send(
			$"You deposit {stable.Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} into {stable.Name.TitleCase().ColourName()}'s virtual cash balance.");
	}

	private static void StableWithdraw(ICharacter actor, StringStack ss)
	{
		if (!DoStableCommandFindStable(actor, out var stable))
		{
			return;
		}

		if (!stable.IsProprietor(actor))
		{
			actor.OutputHandler.Send("You are not a proprietor of this stable.");
			return;
		}

		if (ss.IsFinished || !stable.Currency.TryGetBaseCurrency(ss.SafeRemainingArgument, out var amount) || amount <= 0.0M)
		{
			actor.OutputHandler.Send($"How much {stable.Currency.Name.ColourName()} do you want to withdraw?");
			return;
		}

		if (!VirtualCashLedger.Debit(stable, stable.Currency, amount, actor, actor, "CashWithdrawal",
			    "Manager cash withdrawal", stable.BankAccount,
			    stable.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime, out var error))
		{
			actor.OutputHandler.Send(error);
			return;
		}

		var cash = CurrencyGameItemComponentProto.CreateNewCurrencyPile(stable.Currency,
			stable.Currency.FindCoinsForAmount(amount, out _));
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
			$"You withdraw {stable.Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} from {stable.Name.TitleCase().ColourName()}.");
	}

	private static void StableLedger(ICharacter actor, StringStack ss)
	{
		if (!DoStableCommandFindStable(actor, out var stable))
		{
			return;
		}

		if (!stable.IsManager(actor))
		{
			actor.OutputHandler.Send("You are not a manager of this stable.");
			return;
		}

		var count = 25;
		if (!ss.IsFinished && (!int.TryParse(ss.SafeRemainingArgument, out count) || count <= 0))
		{
			actor.OutputHandler.Send("How many ledger entries do you want to review?");
			return;
		}

		var entries = VirtualCashLedger.LedgerEntries(stable, count).ToList();
		if (!entries.Any())
		{
			actor.OutputHandler.Send($"{stable.Name.TitleCase().ColourName()} does not have any cash ledger entries.");
			return;
		}

		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			entries.Select(x => new List<string>
			{
				x.RealDateTime.ToString("g", actor),
				x.ActorName ?? string.Empty,
				stable.Currency.Describe(x.Amount, CurrencyDescriptionPatternType.ShortDecimal),
				stable.Currency.Describe(x.BalanceAfter, CurrencyDescriptionPatternType.ShortDecimal),
				$"{x.SourceKind}->{x.DestinationKind}",
				x.Reason
			}),
			new List<string> { "When", "Actor", "Amount", "Balance", "Route", "Reason" },
			actor.LineFormatLength,
			colour: Telnet.Green,
			unicodeTable: actor.Account.UseUnicode));
	}

	private static void StableFee(ICharacter actor, StringStack ss)
	{
		if (!DoStableCommandFindStable(actor, out var stable))
		{
			return;
		}

		if (!stable.IsProprietor(actor))
		{
			actor.OutputHandler.Send("You are not a proprietor of this stable.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Do you want to set the LODGE fee or DAILY fee?");
			return;
		}

		var which = ss.PopSpeech().ToLowerInvariant();
		if (!which.EqualToAny("lodge", "daily"))
		{
			actor.OutputHandler.Send("You must choose either LODGE or DAILY.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Do you want to set an amount, PROG <prog>, or NONE?");
			return;
		}

		if (ss.PeekSpeech().EqualTo("none"))
		{
			if (which.EqualTo("lodge"))
			{
				stable.LodgeFee = 0.0M;
				stable.LodgeFeeProg = null;
			}
			else
			{
				stable.DailyFee = 0.0M;
				stable.DailyFeeProg = null;
			}

			actor.OutputHandler.Send($"The {which.ColourName()} fee is now zero.");
			return;
		}

		if (ss.PeekSpeech().EqualTo("prog"))
		{
			ss.PopSpeech();
			var prog = actor.Gameworld.FutureProgs.GetByIdOrName(ss.SafeRemainingArgument);
			if (prog is null)
			{
				actor.OutputHandler.Send("There is no such prog.");
				return;
			}

			if (!prog.ReturnType.CompatibleWith(ProgVariableTypes.Number) ||
			    !prog.MatchesParameters(new[] { ProgVariableTypes.Character, ProgVariableTypes.Character }))
			{
				actor.OutputHandler.Send("The fee prog must return a number and take parameters (mount, owner) as characters.");
				return;
			}

			if (which.EqualTo("lodge"))
			{
				stable.LodgeFeeProg = prog;
			}
			else
			{
				stable.DailyFeeProg = prog;
			}

			actor.OutputHandler.Send($"The {which.ColourName()} fee now uses {prog.MXPClickableFunctionNameWithId()}.");
			return;
		}

		if (!stable.Currency.TryGetBaseCurrency(ss.SafeRemainingArgument, out var amount) || amount < 0.0M)
		{
			actor.OutputHandler.Send($"That is not a valid amount of {stable.Currency.Name.ColourName()}.");
			return;
		}

		if (which.EqualTo("lodge"))
		{
			stable.LodgeFee = amount;
		}
		else
		{
			stable.DailyFee = amount;
		}

		actor.OutputHandler.Send($"The {which.ColourName()} fee is now {stable.Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}.");
	}

	private static void StableSet(ICharacter actor, StringStack ss)
	{
		if (!DoStableCommandFindStable(actor, out var stable))
		{
			return;
		}

		if (!stable.IsProprietor(actor))
		{
			actor.OutputHandler.Send("You are not a proprietor of this stable.");
			return;
		}

		if (ss.IsFinished || !ss.PopSpeech().EqualTo("can"))
		{
			actor.OutputHandler.Send("The only stable set option is CAN <prog|none> [whyprog].");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should control access, or NONE?");
			return;
		}

		if (ss.PeekSpeech().EqualTo("none"))
		{
			stable.CanStableProg = null;
			stable.WhyCannotStableProg = null;
			actor.OutputHandler.Send("This stable no longer uses access progs.");
			return;
		}

		var canProg = actor.Gameworld.FutureProgs.GetByIdOrName(ss.PopSpeech());
		if (canProg is null)
		{
			actor.OutputHandler.Send("There is no such access prog.");
			return;
		}

		if (!canProg.ReturnType.CompatibleWith(ProgVariableTypes.Boolean) ||
		    !canProg.MatchesParameters(new[] { ProgVariableTypes.Character, ProgVariableTypes.Character }))
		{
			actor.OutputHandler.Send("The access prog must return boolean and take parameters (customer, mount) as characters.");
			return;
		}

		IFutureProg? whyProg = null;
		if (!ss.IsFinished)
		{
			whyProg = actor.Gameworld.FutureProgs.GetByIdOrName(ss.SafeRemainingArgument);
			if (whyProg is null)
			{
				actor.OutputHandler.Send("There is no such failure text prog.");
				return;
			}

			if (!whyProg.ReturnType.CompatibleWith(ProgVariableTypes.Text) ||
			    !whyProg.MatchesParameters(new[] { ProgVariableTypes.Character, ProgVariableTypes.Character }))
			{
				actor.OutputHandler.Send("The failure text prog must return text and take parameters (customer, mount) as characters.");
				return;
			}
		}

		stable.CanStableProg = canProg;
		stable.WhyCannotStableProg = whyProg;
		actor.OutputHandler.Send($"This stable now uses {canProg.MXPClickableFunctionNameWithId()} for access checks.");
	}

	private static void StableOpen(ICharacter actor)
	{
		if (!DoStableCommandFindStable(actor, out var stable))
		{
			return;
		}

		if (!stable.IsManager(actor))
		{
			actor.OutputHandler.Send("You are not a manager of this stable.");
			return;
		}

		if (stable.IsTrading)
		{
			actor.OutputHandler.Send($"{stable.Name.TitleCase().ColourName()} is already open.");
			return;
		}

		stable.ToggleIsTrading();
		actor.OutputHandler.Send($"You open {stable.Name.TitleCase().ColourName()} for business.");
	}

	private static void StableClose(ICharacter actor)
	{
		if (!DoStableCommandFindStable(actor, out var stable))
		{
			return;
		}

		if (!stable.IsManager(actor))
		{
			actor.OutputHandler.Send("You are not a manager of this stable.");
			return;
		}

		if (!stable.IsTrading)
		{
			actor.OutputHandler.Send($"{stable.Name.TitleCase().ColourName()} is already closed.");
			return;
		}

		stable.ToggleIsTrading();
		actor.OutputHandler.Send($"You close {stable.Name.TitleCase().ColourName()} for business.");
	}

	private static void StableCreate(ICharacter actor, StringStack ss)
	{
		if (actor.Gameworld.Stables.Any(x => x.Location == actor.Location))
		{
			actor.OutputHandler.Send("There is already a stable at this location.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name should this stable have?");
			return;
		}

		var name = ss.PopSpeech().TitleCase();
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which economic zone should this stable belong to?");
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
			actor.OutputHandler.Send("Which bank account should receive stable fees, or NONE?");
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

		var stable = new Stable(zone, actor.Location, account, name);
		actor.Gameworld.Add(stable);
		actor.OutputHandler.Send($"You create the {stable.Name.TitleCase().ColourName()} stable at this location.");
	}

	private static void StableDelete(ICharacter actor, StringStack ss)
	{
		if (!DoStableCommandFindStable(actor, out var stable))
		{
			return;
		}

		if (stable.ActiveStays.Any())
		{
			actor.OutputHandler.Send("You cannot delete a stable with active stays.");
			return;
		}

		stable.Delete();
		actor.Gameworld.Destroy(stable);
		actor.OutputHandler.Send($"You delete the {stable.Name.TitleCase().ColourName()} stable.");
	}

	private static void StableEmploy(ICharacter actor, StringStack ss)
	{
		StableEmployeeToggle(actor, ss, "employ", (stable, target) =>
		{
			stable.AddEmployee(target);
			return $"{target.HowSeen(actor, true)} is now employed by {stable.Name.TitleCase().ColourName()}.";
		}, proprietorOnly: true);
	}

	private static void StableFire(ICharacter actor, StringStack ss)
	{
		if (!DoStableCommandFindStable(actor, out var stable))
		{
			return;
		}

		if (!stable.IsProprietor(actor))
		{
			actor.OutputHandler.Send("You are not a proprietor of this stable.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Who do you want to fire?");
			return;
		}

		var text = ss.SafeRemainingArgument;
		var employee = stable.EmployeeRecords.FirstOrDefault(x => x.Name.GetName(NameStyle.FullName).StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
		var target = actor.TargetActor(text);
		if (employee is null && target is not null)
		{
			employee = stable.EmployeeRecords.FirstOrDefault(x => x.EmployeeCharacterId == target.Id);
		}

		if (employee is null)
		{
			actor.OutputHandler.Send("There is no such employee.");
			return;
		}

		stable.RemoveEmployee(employee);
		actor.OutputHandler.Send($"{employee.Name.GetName(NameStyle.FullName).ColourName()} is no longer employed by {stable.Name.TitleCase().ColourName()}.");
	}

	private static void StableManager(ICharacter actor, StringStack ss)
	{
		StableEmployeeToggle(actor, ss, "manager", (stable, target) =>
		{
			if (!stable.IsEmployee(target))
			{
				stable.AddEmployee(target);
			}

			var newValue = !stable.IsManager(target);
			stable.SetManager(target, newValue);
			return $"{target.HowSeen(actor, true)} {(newValue ? "is now" : "is no longer")} a manager of {stable.Name.TitleCase().ColourName()}.";
		}, proprietorOnly: true);
	}

	private static void StableProprietor(ICharacter actor, StringStack ss)
	{
		StableEmployeeToggle(actor, ss, "proprietor", (stable, target) =>
		{
			if (!stable.IsEmployee(target))
			{
				stable.AddEmployee(target);
			}

			var newValue = !stable.IsProprietor(target);
			stable.SetProprietor(target, newValue);
			if (newValue && !stable.IsManager(target))
			{
				stable.SetManager(target, true);
			}

			return $"{target.HowSeen(actor, true)} {(newValue ? "is now" : "is no longer")} a proprietor of {stable.Name.TitleCase().ColourName()}.";
		}, proprietorOnly: true);
	}

	private static void StableEmployeeToggle(ICharacter actor, StringStack ss, string verb,
		Func<IStable, ICharacter, string> action, bool proprietorOnly)
	{
		if (!DoStableCommandFindStable(actor, out var stable))
		{
			return;
		}

		if (proprietorOnly && !stable.IsProprietor(actor))
		{
			actor.OutputHandler.Send("You are not a proprietor of this stable.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send($"Who do you want to {verb}?");
			return;
		}

		var target = actor.TargetActor(ss.SafeRemainingArgument);
		if (target is null)
		{
			actor.OutputHandler.Send("You do not see any such person.");
			return;
		}

		actor.OutputHandler.Send(action(stable, target));
	}

	private static void StableAccountStatus(ICharacter actor, StringStack ss)
	{
		if (!DoStableCommandFindStable(actor, out var stable))
		{
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which stable account do you want to view?");
			return;
		}

		var account = stable.AccountByName(ss.SafeRemainingArgument);
		if (account is null || (!account.AccountUsers.Any(x => x.Id == actor.Id) && !stable.IsManager(actor)))
		{
			actor.OutputHandler.Send("There is no such stable account that you are authorised to view.");
			return;
		}

		actor.OutputHandler.Send(account.Show(actor));
	}

	private static void StablePayAccount(ICharacter actor, StringStack ss)
	{
		if (!DoStableCommandFindStable(actor, out var stable))
		{
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which stable account do you want to pay?");
			return;
		}

		var account = stable.AccountByName(ss.PopSpeech());
		if (account is null || !account.AccountUsers.Any(x => x.Id == actor.Id))
		{
			actor.OutputHandler.Send("There is no such stable account that you are authorised to pay.");
			return;
		}

		if (ss.IsFinished || !stable.Currency.TryGetBaseCurrency(ss.PopSpeech(), out var amount) || amount <= 0.0M)
		{
			actor.OutputHandler.Send($"How much {stable.Currency.Name.ColourName()} do you want to pay?");
			return;
		}

		if (!ParseStableReceiptPayment(actor, stable, ss, out var payment, out var error) ||
		    !CanTakeStablePayment(actor, stable, payment, amount, out error))
		{
			actor.OutputHandler.Send(error);
			return;
		}

		TakeStablePayment(actor, stable, payment, amount, $"Payment to stable account {account.AccountName}", null);
		account.PayAccount(amount);
		actor.OutputHandler.Send($"You pay {stable.Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} to {account.AccountName.ColourName()}.");
	}

    private static void StableAccounts(ICharacter actor, StringStack ss)
	{
        if (!DoStableCommandFindStable(actor, out var stable))
        {
            return;
        }
    }


    private static void StableAccount(ICharacter actor, StringStack ss)
	{
		if (!DoStableCommandFindStable(actor, out var stable))
		{
			return;
		}

		if (!stable.IsManager(actor))
		{
			actor.OutputHandler.Send("You are not a manager of this stable.");
			return;
		}

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "create":
				StableAccountCreate(actor, stable, ss);
				return;
			case "show":
				StableAccountShow(actor, stable, ss);
				return;
			case "limit":
				StableAccountLimit(actor, stable, ss);
				return;
			case "suspend":
				StableAccountSuspend(actor, stable, ss);
				return;
			case "authorise":
			case "authorize":
				StableAccountAuthorise(actor, stable, ss);
				return;
			case "unauthorise":
			case "unauthorize":
				StableAccountUnauthorise(actor, stable, ss);
				return;
			case "list":
				StableAccountList(actor, stable, ss);
				return;
		}

		actor.OutputHandler.Send((@"You can use the following options with the account subcommand:

	#3list [<filters>]#0 - lists all accounts, optionally filtered
	#3show <id|name>#0 - shows a particular credit account
	#3create <name> <ownership> <credit limit>#0 - creates a new credit account for a customer
	#3limit <id|name> <limit>#0 - sets a new credit limit for an account
	#3suspend <id|name>#0 - toggles suspension of a credit account
	#3authorise <id|name> <person> <limit>#0 - authorises an additional person to access a credit account
	#3unauthorise <id|name> <person>#0 - removes an authorisation of an additional person on a credit account

" + StableAccountListFilterHelp).SubstituteANSIColour());
	}

	internal sealed class StableAccountListFilterSet
	{
		private readonly List<Func<IStableAccount, bool>> _predicates = new();
		private readonly List<string> _descriptions = new();

		public IReadOnlyList<string> Descriptions => _descriptions;

		public bool Matches(IStableAccount account)
		{
			return _predicates.All(predicate => predicate(account));
		}

		public IEnumerable<IStableAccount> Apply(IEnumerable<IStableAccount> accounts)
		{
			return accounts.Where(Matches);
		}

		internal void Add(string description, Func<IStableAccount, bool> predicate)
		{
			_descriptions.Add(description);
			_predicates.Add(predicate);
		}
	}

	internal static bool TryParseStableAccountListFilters(StringStack ss, out StableAccountListFilterSet filters, out string error)
	{
		filters = new StableAccountListFilterSet();
		error = string.Empty;

		while (!ss.IsFinished)
		{
			var cmd = ss.PopSpeech().ToLowerInvariant();
			switch (cmd)
			{
				case "suspended":
				case "suspend":
					filters.Add("suspended", account => account.IsSuspended);
					continue;
				case "active":
				case "unsuspended":
				case "open":
					filters.Add("active", account => !account.IsSuspended);
					continue;
				case "over":
					if (!ss.IsFinished && ss.PeekSpeech().EqualTo("limit"))
					{
						ss.PopSpeech();
					}

					filters.Add("over limit", account => account.CreditAvailable < 0.0M);
					continue;
				case "overlimit":
				case "over-limit":
				case "overbalanced":
					filters.Add("over limit", account => account.CreditAvailable < 0.0M);
					continue;
				case "owing":
				case "owed":
				case "debt":
				case "negative":
					filters.Add("owing balance", account => account.Balance < 0.0M);
					continue;
				case "prepaid":
				case "credit":
				case "positive":
					filters.Add("prepaid balance", account => account.Balance > 0.0M);
					continue;
				case "owner":
					if (!TryPopStableAccountListFilterText(ss, "owner", out var ownerText, out error))
					{
						return false;
					}

					filters.Add($"owner matching \"{ownerText}\"", account => StableAccountOwnerNameMatches(account, ownerText));
					continue;
				case "name":
				case "account":
					if (!TryPopStableAccountListFilterText(ss, "name", out var nameText, out error))
					{
						return false;
					}

					filters.Add($"account name matching \"{nameText}\"", account => StableAccountNameMatches(account, nameText));
					continue;
				case "user":
				case "authorised":
				case "authorized":
					if (!TryPopStableAccountListFilterText(ss, "user", out var userText, out error))
					{
						return false;
					}

					filters.Add($"authorised user matching \"{userText}\"", account => StableAccountUserNameMatches(account, userText));
					continue;
				case "search":
				case "match":
					if (!TryPopStableAccountListFilterText(ss, "search", out var searchText, out error))
					{
						return false;
					}

					filters.Add($"matching \"{searchText}\"", account =>
						StableAccountNameMatches(account, searchText) ||
						StableAccountOwnerNameMatches(account, searchText) ||
						StableAccountUserNameMatches(account, searchText));
					continue;
			}

			error = $"The text {cmd.ColourCommand()} is not a valid stable account list filter.\n\n{StableAccountListFilterHelp}";
			return false;
		}

		return true;
	}

	private static bool TryPopStableAccountListFilterText(StringStack ss, string filterName, out string text, out string error)
	{
		text = string.Empty;
		error = string.Empty;
		if (ss.IsFinished)
		{
			error = $"The {filterName.ColourCommand()} filter needs matching text.\n\n{StableAccountListFilterHelp}";
			return false;
		}

		text = ss.PopSpeech();
		if (!string.IsNullOrWhiteSpace(text))
		{
			return true;
		}

		error = $"The {filterName.ColourCommand()} filter needs matching text.\n\n{StableAccountListFilterHelp}";
		return false;
	}

	private static bool StableAccountNameMatches(IStableAccount account, string text)
	{
		return account.AccountName.Contains(text, StringComparison.InvariantCultureIgnoreCase);
	}

	private static bool StableAccountOwnerNameMatches(IStableAccount account, string text)
	{
		return account.AccountOwnerName.GetName(NameStyle.FullName).Contains(text, StringComparison.InvariantCultureIgnoreCase) ||
		       account.AccountOwnerName.GetName(NameStyle.SimpleFull).Contains(text, StringComparison.InvariantCultureIgnoreCase);
	}

	private static bool StableAccountUserNameMatches(IStableAccount account, string text)
	{
		return account.AccountUsers.Any(user =>
			user.PersonalName.GetName(NameStyle.FullName).Contains(text, StringComparison.InvariantCultureIgnoreCase) ||
			user.PersonalName.GetName(NameStyle.SimpleFull).Contains(text, StringComparison.InvariantCultureIgnoreCase));
	}

	private static void StableAccountList(ICharacter actor, IStable stable, StringStack ss)
	{
		if (!TryParseStableAccountListFilters(ss, out var filters, out var error))
		{
			actor.OutputHandler.Send(error.SubstituteANSIColour());
			return;
		}

		var allAccounts = stable.StableAccounts.ToList();
		var accounts = filters.Apply(allAccounts)
			.OrderBy(account => account.AccountName, StringComparer.InvariantCultureIgnoreCase)
			.ToList();

		StringBuilder sb = new();
		sb.AppendLine($"Stable accounts for {stable.Name.TitleCase().ColourName()}");
		if (filters.Descriptions.Any())
		{
			sb.AppendLine($"Filters: {filters.Descriptions.Select(x => x.ColourCommand()).ListToString()}");
		}

		sb.AppendLine($"Showing {accounts.Count.ToStringN0(actor)} of {allAccounts.Count.ToStringN0(actor)} {"account".Pluralise(allAccounts.Count != 1)}.");
		if (!accounts.Any())
		{
			sb.AppendLine("No stable accounts match those filters.");
			actor.OutputHandler.Send(sb.ToString());
			return;
		}

		sb.AppendLine(StringUtilities.GetTextTable(
			from account in accounts
			select new List<string>
			{
				account.Id.ToStringN0(actor),
				account.AccountName,
				account.AccountOwnerName.GetName(NameStyle.SimpleFull),
				account.IsSuspended.ToColouredString(),
				stable.Currency.Describe(account.Balance, CurrencyDescriptionPatternType.ShortDecimal).ColourValue(),
				stable.Currency.Describe(account.CreditLimit, CurrencyDescriptionPatternType.ShortDecimal).ColourValue(),
				stable.Currency.Describe(account.CreditAvailable, CurrencyDescriptionPatternType.ShortDecimal).ColourValue(),
				account.AccountUsers.Count().ToStringN0(actor)
			},
			new List<string>
			{
				"Id",
				"Account",
				"Owner",
				"Susp?",
				"Balance",
				"Limit",
				"Available",
				"Users"
			},
			actor.LineFormatLength,
			colour: Telnet.Yellow,
			truncatableColumnIndex: 1,
			unicodeTable: actor.Account.UseUnicode
		));
		actor.OutputHandler.Send(sb.ToString());
	}

	private static void StableAccountCreate(ICharacter actor, IStable stable, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name should the account have?");
			return;
		}

		var name = ss.PopSpeech();
		if (stable.AccountByName(name) is not null)
		{
			actor.OutputHandler.Send("There is already a stable account with that name.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Who should own this account?");
			return;
		}

		var owner = actor.TargetActor(ss.PopSpeech());
		if (owner is null)
		{
			actor.OutputHandler.Send("You do not see any such person.");
			return;
		}

		if (ss.IsFinished || !stable.Currency.TryGetBaseCurrency(ss.SafeRemainingArgument, out var limit) || limit < 0.0M)
		{
			actor.OutputHandler.Send("What credit limit should this account have?");
			return;
		}

		var account = new StableAccount(stable, name, owner, limit);
		stable.AddStableAccount(account);
		actor.OutputHandler.Send($"You create stable account {name.ColourName()} for {owner.HowSeen(actor)}.");
	}

	private static void StableAccountShow(ICharacter actor, IStable stable, StringStack ss)
	{
		var account = ss.IsFinished ? null : stable.AccountByName(ss.SafeRemainingArgument);
		if (account is null)
		{
			actor.OutputHandler.Send("Which stable account do you want to show?");
			return;
		}

		actor.OutputHandler.Send(account.Show(actor));
	}

	private static void StableAccountLimit(ICharacter actor, IStable stable, StringStack ss)
	{
		var account = ss.IsFinished ? null : stable.AccountByName(ss.PopSpeech());
		if (account is null)
		{
			actor.OutputHandler.Send("Which stable account do you want to edit?");
			return;
		}

		if (ss.IsFinished || !stable.Currency.TryGetBaseCurrency(ss.SafeRemainingArgument, out var limit) || limit < 0.0M)
		{
			actor.OutputHandler.Send("What credit limit should this account have?");
			return;
		}

		account.SetCreditLimit(limit);
		actor.OutputHandler.Send($"The credit limit for {account.AccountName.ColourName()} is now {stable.Currency.Describe(limit, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}.");
	}

	private static void StableAccountSuspend(ICharacter actor, IStable stable, StringStack ss)
	{
		var account = ss.IsFinished ? null : stable.AccountByName(ss.SafeRemainingArgument);
		if (account is null)
		{
			actor.OutputHandler.Send("Which stable account do you want to suspend or unsuspend?");
			return;
		}

		account.IsSuspended = !account.IsSuspended;
		actor.OutputHandler.Send($"{account.AccountName.ColourName()} is {(account.IsSuspended ? "now" : "no longer")} suspended.");
	}

	private static void StableAccountAuthorise(ICharacter actor, IStable stable, StringStack ss)
	{
		var account = ss.IsFinished ? null : stable.AccountByName(ss.PopSpeech());
		if (account is null)
		{
			actor.OutputHandler.Send("Which stable account do you want to authorise someone for?");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Who do you want to authorise?");
			return;
		}

		var target = actor.TargetActor(ss.PopSpeech());
		if (target is null)
		{
			actor.OutputHandler.Send("You do not see any such person.");
			return;
		}

		decimal? limit = null;
		if (!ss.IsFinished && !ss.SafeRemainingArgument.EqualTo("none"))
		{
			if (!stable.Currency.TryGetBaseCurrency(ss.SafeRemainingArgument, out var parsed) || parsed < 0.0M)
			{
				actor.OutputHandler.Send("That is not a valid spending limit.");
				return;
			}

			limit = parsed;
		}

		account.AddAuthorisation(target, limit);
		actor.OutputHandler.Send($"{target.HowSeen(actor, true)} is now authorised to use {account.AccountName.ColourName()}.");
	}

	private static void StableAccountUnauthorise(ICharacter actor, IStable stable, StringStack ss)
	{
		var account = ss.IsFinished ? null : stable.AccountByName(ss.PopSpeech());
		if (account is null)
		{
			actor.OutputHandler.Send("Which stable account do you want to unauthorise someone from?");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Who do you want to unauthorise?");
			return;
		}

		var text = ss.SafeRemainingArgument;
		var user = account.AccountUsers.FirstOrDefault(x => x.PersonalName.GetName(NameStyle.FullName).StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
		var target = actor.TargetActor(text);
		if (user is null && target is not null)
		{
			user = account.AccountUsers.FirstOrDefault(x => x.Id == target.Id);
		}

		if (user is null)
		{
			actor.OutputHandler.Send("There is no such authorised user.");
			return;
		}

		account.RemoveAuthorisation(user);
		actor.OutputHandler.Send($"{user.PersonalName.GetName(NameStyle.FullName).ColourName()} is no longer authorised for {account.AccountName.ColourName()}.");
	}

	private enum StablePaymentKind
	{
		Cash,
		StableAccount,
		BankPaymentItem
	}

	private sealed class StablePaymentSelection
	{
		public StablePaymentKind Kind { get; init; }
		public IStableAccount? Account { get; init; }
		public IBankPaymentItem? PaymentItem { get; init; }
	}

	private static bool ParseStablePayment(ICharacter actor, IStable stable, StringStack ss,
		out StablePaymentSelection payment, out string error)
	{
		if (ss.IsFinished || ss.PeekSpeech().EqualTo("cash"))
		{
			if (!ss.IsFinished)
			{
				ss.PopSpeech();
			}

			payment = new StablePaymentSelection { Kind = StablePaymentKind.Cash };
			error = string.Empty;
			return true;
		}

		if (ss.PeekSpeech().EqualTo("account"))
		{
			ss.PopSpeech();
			if (ss.IsFinished)
			{
				payment = null!;
				error = "Which stable account do you want to charge?";
				return false;
			}

			var account = stable.AccountByName(ss.SafeRemainingArgument);
			if (account is null)
			{
				payment = null!;
				error = "There is no such stable account.";
				return false;
			}

			payment = new StablePaymentSelection { Kind = StablePaymentKind.StableAccount, Account = account };
			error = string.Empty;
			return true;
		}

		return ParseStableReceiptPayment(actor, stable, ss, out payment, out error);
	}

	private static bool ParseStableReceiptPayment(ICharacter actor, IStable stable, StringStack ss,
		out StablePaymentSelection payment, out string error)
	{
		if (ss.IsFinished || ss.PeekSpeech().EqualTo("cash"))
		{
			if (!ss.IsFinished)
			{
				ss.PopSpeech();
			}

			payment = new StablePaymentSelection { Kind = StablePaymentKind.Cash };
			error = string.Empty;
			return true;
		}

		if (ss.PeekSpeech().EqualTo("with"))
		{
			ss.PopSpeech();
			if (ss.IsFinished)
			{
				payment = null!;
				error = "Which payment item do you want to use?";
				return false;
			}

			var item = actor.TargetItem(ss.SafeRemainingArgument);
			var paymentItem = item?.GetItemType<IBankPaymentItem>();
			if (paymentItem is null)
			{
				payment = null!;
				error = "You do not have any such bank payment item.";
				return false;
			}

			payment = new StablePaymentSelection { Kind = StablePaymentKind.BankPaymentItem, PaymentItem = paymentItem };
			error = string.Empty;
			return true;
		}

		payment = null!;
		error = "You must specify CASH, ACCOUNT <account>, or WITH <payment item>.";
		return false;
	}

	private static bool CanTakeStablePayment(ICharacter actor, IStable stable, StablePaymentSelection payment,
		decimal amount, out string error)
	{
		error = string.Empty;
		if (amount <= 0.0M)
		{
			return true;
		}

		switch (payment.Kind)
		{
			case StablePaymentKind.Cash:
				if (AccessibleStableCash(actor, stable.Currency) < amount)
				{
					error = $"You do not have {stable.Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} in accessible cash.";
					return false;
				}
				return true;
			case StablePaymentKind.BankPaymentItem:
				if (payment.PaymentItem!.BankAccount.Currency != stable.Currency)
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
				return true;
			case StablePaymentKind.StableAccount:
				var reason = payment.Account!.IsAuthorisedToUse(actor, amount);
				if (reason != StableAccountAuthorisationFailureReason.None)
				{
					error = reason.DescribeEnum();
					return false;
				}
				return true;
		}

		return false;
	}

	private static decimal AccessibleStableCash(ICharacter actor, ICurrency currency)
	{
		return actor.Body.ExternalItems.RecursiveGetItems<ICurrencyPile>(true)
		            .Where(x => x.Currency == currency)
		            .Sum(x => x.TotalValue);
	}

	private static void TakeStablePayment(ICharacter actor, IStable stable, StablePaymentSelection payment,
		decimal amount, string reference, IStableStay? stay)
	{
		if (amount <= 0.0M)
		{
			return;
		}

		switch (payment.Kind)
		{
			case StablePaymentKind.Cash:
				TakeStableCash(actor, stable, amount, reference);
				stay?.AddPayment(amount, actor, reference);
				return;
			case StablePaymentKind.BankPaymentItem:
				payment.PaymentItem!.BankAccount.WithdrawFromTransaction(amount, reference);
				payment.PaymentItem.BankAccount.Bank.CurrencyReserves[stable.Currency] -= amount;
				payment.PaymentItem.BankAccount.Bank.Changed = true;
				VirtualCashLedger.CreditBankOrVirtual(stable, stable.Currency, amount, actor, actor, "BankPaymentItem",
					reference, stable.BankAccount, stable.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime);
				if (payment.PaymentItem.CurrentUsesRemaining > 0)
				{
					payment.PaymentItem.CurrentUsesRemaining--;
				}
				stay?.AddPayment(amount, actor, reference);
				return;
			case StablePaymentKind.StableAccount:
				payment.Account!.ChargeAccount(amount);
				stay?.AddPayment(amount, actor, $"Charged to stable account {payment.Account.AccountName}");
				return;
		}
	}

	private static void TakeStableCash(ICharacter actor, IStable stable, decimal amount, string reference)
	{
		var targetCoins = stable.Currency.FindCurrency(actor.Body.ExternalItems.RecursiveGetItems<ICurrencyPile>(true), amount);
		var value = targetCoins.Sum(x => x.Value.Sum(y => y.Value * y.Key.Value));
		var containers = targetCoins.SelectNotNull(x => x.Key.Parent.ContainedIn).Distinct().ToList();
		foreach (var item in targetCoins.Where(item => !item.Key.RemoveCoins(item.Value.Select(x => Tuple.Create(x.Key, x.Value)))))
		{
			item.Key.Parent.Delete();
		}

		VirtualCashLedger.Credit(stable, stable.Currency, amount, actor, actor, "Cash", reference,
			stable.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime);
		var change = value - amount;
		if (change <= 0.0M)
		{
			return;
		}

		var changePile = CurrencyGameItemComponentProto.CreateNewCurrencyPile(stable.Currency,
			stable.Currency.FindCoinsForAmount(change, out _));
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

	private static void GiveStableTicket(ICharacter actor, IGameItem ticket)
	{
		if (actor.Body.CanGet(ticket, 0))
		{
			actor.Body.Get(ticket);
			return;
		}

		actor.Location.Insert(ticket);
	}
}
