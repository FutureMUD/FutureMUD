using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Community;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;

namespace MudSharp.Economy.Banking;

public class BankAccountType : SaveableItem, IBankAccountType
{
	public BankAccountType(Models.BankAccountType dbitem, IBank bank)
	{
		_id = dbitem.Id;
		_name = dbitem.Name;
		Bank = bank;
		Gameworld = bank.Gameworld;
		Gameworld.Add(this);
		CustomerDescription = dbitem.CustomerDescription;
		MaximumOverdrawAmount = dbitem.MaximumOverdrawAmount;
		WithdrawalFleeFlat = dbitem.WithdrawalFleeFlat;
		WithdrawalFleeRate = dbitem.WithdrawalFleeRate;
		DepositFeeFlat = dbitem.DepositFeeFlat;
		DepositFeeRate = dbitem.DepositFeeRate;
		TransferFeeFlat = dbitem.TransferFeeFlat;
		TransferFeeOtherBankFlat = dbitem.TransferFeeOtherBankFlat;
		TransferFeeOtherBankRate = dbitem.TransferFeeOtherBankRate;
		TransferFeeRate = dbitem.TransferFeeRate;
		DailyFee = dbitem.DailyFee;
		DailyInterestRate = dbitem.DailyInterestRate;
		OverdrawFeeFlat = dbitem.OverdrawFeeFlat;
		OverdrawFeeRate = dbitem.OverdrawFeeRate;
		DailyOverdrawnFee = dbitem.DailyOverdrawnFee;
		DailyOverdrawnInterestRate = dbitem.DailyOverdrawnInterestRate;
		CanCloseAccountProg = Gameworld.FutureProgs.Get(dbitem.CanCloseAccountProgId ?? 0);
		CanOpenAccountProgCharacter = Gameworld.FutureProgs.Get(dbitem.CanOpenAccountProgCharacterId ?? 0);
		CanOpenAccountProgClan = Gameworld.FutureProgs.Get(dbitem.CanOpenAccountProgClanId ?? 0);
		CanOpenAccountProgShop = Gameworld.FutureProgs.Get(dbitem.CanOpenAccountProgShopId ?? 0);
		NumberOfPermittedPaymentItems = dbitem.NumberOfPermittedPaymentItems;
		PaymentItemPrototype = dbitem.PaymentItemPrototypeId;
	}

	public BankAccountType(IBank bank, string name)
	{
		Bank = bank;
		Gameworld = bank.Gameworld;
		_name = name;
		CustomerDescription = name;
		using (new FMDB())
		{
			var dbitem = new Models.BankAccountType()
			{
				BankId = Bank.Id,
				Name = name,
				CustomerDescription = CustomerDescription,
				MaximumOverdrawAmount = MaximumOverdrawAmount,
				WithdrawalFleeFlat = WithdrawalFleeFlat,
				WithdrawalFleeRate = WithdrawalFleeRate,
				DepositFeeFlat = DepositFeeFlat,
				DepositFeeRate = DepositFeeRate,
				TransferFeeFlat = TransferFeeFlat,
				TransferFeeOtherBankFlat = TransferFeeOtherBankFlat,
				TransferFeeOtherBankRate = TransferFeeOtherBankRate,
				TransferFeeRate = TransferFeeRate,
				DailyFee = DailyFee,
				DailyInterestRate = DailyInterestRate,
				OverdrawFeeFlat = OverdrawFeeFlat,
				OverdrawFeeRate = OverdrawFeeRate,
				DailyOverdrawnFee = DailyOverdrawnFee,
				DailyOverdrawnInterestRate = DailyOverdrawnInterestRate,
				CanCloseAccountProgId = CanCloseAccountProg?.Id,
				CanOpenAccountProgCharacterId = CanOpenAccountProgCharacter?.Id,
				CanOpenAccountProgClanId = CanOpenAccountProgClan?.Id,
				CanOpenAccountProgShopId = CanOpenAccountProgShop?.Id
			};
			FMDB.Context.BankAccountTypes.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}

		Gameworld.Add(this);
	}

	public override string FrameworkItemType => "BankAccountType";

	public override void Save()
	{
		var dbitem = FMDB.Context.BankAccountTypes.Find(Id);
		dbitem.Name = Name;
		dbitem.CustomerDescription = CustomerDescription;
		dbitem.MaximumOverdrawAmount = MaximumOverdrawAmount;
		dbitem.WithdrawalFleeFlat = WithdrawalFleeFlat;
		dbitem.WithdrawalFleeRate = WithdrawalFleeRate;
		dbitem.DepositFeeFlat = DepositFeeFlat;
		dbitem.DepositFeeRate = DepositFeeRate;
		dbitem.TransferFeeFlat = TransferFeeFlat;
		dbitem.TransferFeeOtherBankFlat = TransferFeeOtherBankFlat;
		dbitem.TransferFeeOtherBankRate = TransferFeeOtherBankRate;
		dbitem.TransferFeeRate = TransferFeeRate;
		dbitem.DailyFee = DailyFee;
		dbitem.DailyInterestRate = DailyInterestRate;
		dbitem.OverdrawFeeFlat = OverdrawFeeFlat;
		dbitem.OverdrawFeeRate = OverdrawFeeRate;
		dbitem.DailyOverdrawnFee = DailyOverdrawnFee;
		dbitem.DailyOverdrawnInterestRate = DailyOverdrawnInterestRate;
		dbitem.CanCloseAccountProgId = CanCloseAccountProg?.Id;
		dbitem.CanOpenAccountProgCharacterId = CanOpenAccountProgCharacter?.Id;
		dbitem.CanOpenAccountProgClanId = CanOpenAccountProgClan?.Id;
		dbitem.CanOpenAccountProgShopId = CanOpenAccountProgShop?.Id;
		dbitem.PaymentItemPrototypeId = PaymentItemPrototype;
		dbitem.NumberOfPermittedPaymentItems = NumberOfPermittedPaymentItems;
		Changed = false;
	}

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "description":
			case "desc":
				return BuildingCommandDescription(actor, command);
			case "dailyfee":
				return BuildingCommandDailyFee(actor, command);
			case "interest":
				return BuildingCommandInterest(actor, command, false);
			case "interestannual":
			case "annualinterest":
				return BuildingCommandInterest(actor, command, true);
			case "maxoverdraw":
			case "maximumoverdraw":
			case "maxoverdraft":
			case "maximumoverdraft":
				return BuildingCommandMaximumOverdraft(actor, command);
			case "overdrawnfee":
				return BuildingCommandOverdrawnFee(actor, command);
			case "overdrawninterest":
				return BuildingCommandOverdrawnInterest(actor, command, false);
			case "overdrawninterestannual":
			case "annualoverdrawninterest":
				return BuildingCommandOverdrawnInterest(actor, command, true);
			case "withdrawfee":
			case "withdrawalfee":
				return BuildingCommandWithdrawalFee(actor, command);
			case "withdrawalrate":
			case "withdrawrate":
				return BuildingCommandWithdrawalRate(actor, command);
			case "transferfee":
				return BuildingCommandTransferFee(actor, command);
			case "transferrate":
				return BuildingCommandTransferRate(actor, command);
			case "otherbankrate":
				return BuildingCommandTransferOtherBankFee(actor, command);
			case "otherbankfee":
				return BuildingCommandTransferOtherBankRate(actor, command);
			case "depositfee":
				return BuildingCommandDepositFee(actor, command);
			case "depositrate":
				return BuildingCommandDepositRate(actor, command);
			case "canopen":
			case "canopenprog":
			case "canprog":
			case "openprog":
				return BuildingCommandCanOpenProg(actor, command);
			case "closeprog":
			case "cancloseprog":
				return BuildingCommandCanCloseProg(actor, command);
			case "paymentitem":
				return BuildingCommandPaymentItem(actor, command);
			case "numberitems":
				return BuildingCommandNumberOfPaymentItems(actor, command);
			default:
				actor.OutputHandler.Send(@"You can use the following options with this command:

	#3name <name>#0 - changes the name of the account type
	#3desc <desc>#0 - changes the description of the account type
	#3canopen [character|shop|clan] <prog>#0 - sets the can open prog for the appropriate type*
	#3canclose <prog>#0 - sets the can close prog
	#3maxoverdraw <amount>#0 - sets the maximum overdraw amount
	#3interest <%>#0 - sets the daily interest rate (collected daily, compounded monthly)
	#3interestannual <%>#0 - sets the effective annual interest rate (converts to daily internally)
	#3overdrawninterest <%>#0 - sets the interest rate on overdrawn balance
	#3overdrawninterestannual <%>#0 - same as above, uses an annual % to convert back to daily
	#3dailyfee <amount>#0 - a daily fee for having the account
	#3overdrawnfee <amount>#0 - a daily fee for having an overdrawn account
	#3withdrawalfee <amount>#0 - a flat fee for withdrawing money
	#3withdrawalrate <%>#0 - a percentage of transaction fee for withdrawing money
	#3depositfee <amount>#0 - a flat fee for depositing money
	#3depositrate <%>#0 - a percentage of transaction fee for depositing money
	#3transferfee <amount>#0 - a flat fee for transferring money to another account
	#3transferrate <%>#0 - a percentage of transaction fee for transferring money
	#3otherbankfee <amount>#0 - a flat fee for transferring money to another bank
	#3otherbankrate <%>#0 - a percentage of transaction fee to transferring money to another bank
	#3paymentitem <proto>#0 - sets an item as a payment item (e.g. chequebook or bank card)
	#3paymentitem none#0 - clears a payment item
	#3numberitems <##>#0 - sets the number of payment items that can be issued simultaneously

#1* Note - character|shop|clan argument only required if supplied prog is ambiguous#0".SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandCanCloseProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog do you want to specify for a can close account prog?");
			return false;
		}

		var prog = Gameworld.FutureProgs.GetByIdOrName(command.SafeRemainingArgument);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (!prog.ReturnType.CompatibleWith(FutureProgVariableTypes.Boolean))
		{
			actor.OutputHandler.Send(
				$"The prog must return a boolean value, whereas {prog.MXPClickableFunctionName()} returns {prog.ReturnType.Describe().ColourName()}.");
			return false;
		}

		if (!prog.MatchesParameters(new[] { FutureProgVariableTypes.Character }))
		{
			actor.OutputHandler.Send(
				$"The prog must accept a single character parameter, whereas {prog.MXPClickableFunctionName()} does not.");
			return false;
		}

		CanCloseAccountProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This bank account type will now use the {prog.MXPClickableFunctionName()} prog to determine if someone can close an account.");
		return true;
	}

	private bool BuildingCommandCanOpenProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog do you want to specify for a can open account prog?");
			return false;
		}

		var arg1 = command.PopSpeech();
		var arg2 = command.SafeRemainingArgument;
		IFutureProg prog;
		if (!string.IsNullOrEmpty(arg2))
		{
			switch (arg1.ToLowerInvariant())
			{
				case "char":
				case "character":
				case "person":
				case "pc":
				case "shop":
				case "clan":
					break;
				default:
					actor.OutputHandler.Send(
						$"You must specify either {new[] { "character", "shop", "clan" }.Select(x => x.ColourCommand()).ListToString(conjunction: "or ")} as your first argument to the canopen command.");
					return false;
			}

			prog = Gameworld.FutureProgs.GetByIdOrName(arg2);
		}
		else
		{
			prog = Gameworld.FutureProgs.GetByIdOrName(arg1);
		}

		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (!prog.ReturnType.CompatibleWith(FutureProgVariableTypes.Boolean))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that returns a boolean value, whereas {prog.MXPClickableFunctionName()} does not.");
			return false;
		}

		if ((prog.AcceptsAnyParameters || prog.MatchesParameters(new[] { FutureProgVariableTypes.CollectionItem }) ||
		     prog.MatchesParameters(new[] { FutureProgVariableTypes.ReferenceType })) && string.IsNullOrEmpty(arg2))
		{
			actor.OutputHandler.Send(
				$"The prog {prog.MXPClickableFunctionName()} is ambiguous as to whether it applies to characters, shops or clans, so you must specify.");
			return false;
		}

		if (!prog.MatchesParameters(new[] { FutureProgVariableTypes.Character }) &&
		    !prog.MatchesParameters(new[] { FutureProgVariableTypes.Clan }) &&
		    !prog.MatchesParameters(new[] { FutureProgVariableTypes.Shop }))
		{
			actor.OutputHandler.Send(
				$"The prog {prog.MXPClickableFunctionName()} does not take a character, shop or clan as an argument.");
			return false;
		}

		var whatFor = "";
		if (!string.IsNullOrEmpty(arg2))
		{
			switch (arg1.ToLowerInvariant())
			{
				case "char":
				case "character":
				case "person":
				case "pc":
					CanOpenAccountProgCharacter = prog;
					whatFor = "characters";
					break;
				case "shop":
					CanOpenAccountProgShop = prog;
					whatFor = "shops";
					break;
				case "clan":
					CanOpenAccountProgClan = prog;
					whatFor = "clans";
					break;
			}
		}
		else
		{
			if (prog.MatchesParameters(new[] { FutureProgVariableTypes.Character }))
			{
				CanOpenAccountProgCharacter = prog;
				whatFor = "characters";
			}
			else if (prog.MatchesParameters(new[] { FutureProgVariableTypes.Shop }))
			{
				CanOpenAccountProgShop = prog;
				whatFor = "shops";
			}
			else
			{
				CanOpenAccountProgClan = prog;
				whatFor = "clans";
			}
		}

		Changed = true;
		actor.OutputHandler.Send(
			$"The prog that determines whether {whatFor.ColourCommand()} can open new accounts is now {prog.MXPClickableFunctionName()}.");
		return true;
	}

	private bool BuildingCommandDepositRate(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !command.SafeRemainingArgument.TryParsePercentageDecimal(out var value))
		{
			actor.OutputHandler.Send("You must enter a valid percentage value.");
			return false;
		}

		DepositFeeRate = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This bank account type will now charge {value.ToString("P4", actor).ColourValue()} of the transaction value as a fee for deposits.");
		return true;
	}

	private bool BuildingCommandDepositFee(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter a valid amount of currency.");
			return false;
		}

		if (!Currency.TryGetBaseCurrency(command.SafeRemainingArgument, out var amount))
		{
			actor.OutputHandler.Send(
				$"The supplied argument ({command.SafeRemainingArgument.ColourCommand()}) is not a valid amount of {Currency.Name.ColourValue()}.");
			return false;
		}

		DepositFeeFlat = amount;
		Changed = true;
		actor.OutputHandler.Send(
			$"This bank account type will now charge a flat fee of {Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} per deposit transaction.");
		return true;
	}

	private bool BuildingCommandTransferOtherBankRate(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !command.SafeRemainingArgument.TryParsePercentageDecimal(out var value))
		{
			actor.OutputHandler.Send("You must enter a valid percentage value.");
			return false;
		}

		TransferFeeOtherBankRate = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This bank account type will now charge {value.ToString("P4", actor).ColourValue()} of the transaction value as a fee for transfers to other banks.");
		return true;
	}

	private bool BuildingCommandTransferOtherBankFee(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter a valid amount of currency.");
			return false;
		}

		if (!Currency.TryGetBaseCurrency(command.SafeRemainingArgument, out var amount))
		{
			actor.OutputHandler.Send(
				$"The supplied argument ({command.SafeRemainingArgument.ColourCommand()}) is not a valid amount of {Currency.Name.ColourValue()}.");
			return false;
		}

		TransferFeeOtherBankFlat = amount;
		Changed = true;
		actor.OutputHandler.Send(
			$"This bank account type will now charge a flat fee of {Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} per transfer to other bank transaction.");
		return true;
	}

	private bool BuildingCommandTransferRate(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !command.SafeRemainingArgument.TryParsePercentageDecimal(out var value))
		{
			actor.OutputHandler.Send("You must enter a valid percentage value.");
			return false;
		}

		TransferFeeRate = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This bank account type will now charge {value.ToString("P4", actor).ColourValue()} of the transaction value as a fee for transfers.");
		return true;
	}

	private bool BuildingCommandTransferFee(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter a valid amount of currency.");
			return false;
		}

		if (!Currency.TryGetBaseCurrency(command.SafeRemainingArgument, out var amount))
		{
			actor.OutputHandler.Send(
				$"The supplied argument ({command.SafeRemainingArgument.ColourCommand()}) is not a valid amount of {Currency.Name.ColourValue()}.");
			return false;
		}

		TransferFeeFlat = amount;
		Changed = true;
		actor.OutputHandler.Send(
			$"This bank account type will now charge a flat fee of {Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} per transfer transaction.");
		return true;
	}

	private bool BuildingCommandWithdrawalRate(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !command.SafeRemainingArgument.TryParsePercentageDecimal(out var value))
		{
			actor.OutputHandler.Send("You must enter a valid percentage value.");
			return false;
		}

		WithdrawalFleeRate = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This bank account type will now charge {value.ToString("P4", actor).ColourValue()} of the transaction value as a fee for withdrawals.");
		return true;
	}

	private bool BuildingCommandWithdrawalFee(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter a valid amount of currency.");
			return false;
		}

		if (!Currency.TryGetBaseCurrency(command.SafeRemainingArgument, out var amount))
		{
			actor.OutputHandler.Send(
				$"The supplied argument ({command.SafeRemainingArgument.ColourCommand()}) is not a valid amount of {Currency.Name.ColourValue()}.");
			return false;
		}

		DepositFeeFlat = amount;
		Changed = true;
		actor.OutputHandler.Send(
			$"This bank account type will now charge a flat fee of {Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} per withdrawal transaction.");
		return true;
	}

	private bool BuildingCommandOverdrawnInterest(ICharacter actor, StringStack command, bool annual)
	{
		if (command.IsFinished || !command.SafeRemainingArgument.TryParsePercentageDecimal(out var value))
		{
			actor.OutputHandler.Send("You must specify an interest rate as a percentage.");
			return false;
		}

		var days = Bank.EconomicZone.FinancialPeriodReferenceCalendar.Months.Sum(x => x.NormalDays);
		var months = Bank.EconomicZone.FinancialPeriodReferenceCalendar.Months.Count;
		var avgMonthDays = days / months;

		if (annual)
		{
			var daily = ((decimal)Math.Pow((double)(1.0M + value), 1.0 / months) - 1.0M) / avgMonthDays;
			DailyOverdrawnInterestRate = daily;
			Changed = true;
			actor.OutputHandler.Send(
				$"The approximate annual interest rate when overdrawn will now be {value.ToString("P4", actor).ColourValue()}\nThis is equivalent to {daily.ToString("P4", actor).ColourValue()} accrued daily compounded monthly.");
			return true;
		}

		DailyOverdrawnInterestRate = value;
		var yearly = (decimal)Math.Pow((double)(value * avgMonthDays), months);
		actor.OutputHandler.Send(
			$"The daily accural (monthly compounded) interest rate for this account type when overdrawn is now {value.ToString("P4", actor).ColourValue()}.\nThis is equivalent to {yearly.ToString("P4", actor).ColourValue()} yearly.");
		return true;
	}

	private bool BuildingCommandOverdrawnFee(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter a valid amount of currency.");
			return false;
		}

		if (!Currency.TryGetBaseCurrency(command.SafeRemainingArgument, out var amount))
		{
			actor.OutputHandler.Send(
				$"The supplied argument ({command.SafeRemainingArgument.ColourCommand()}) is not a valid amount of {Currency.Name.ColourValue()}.");
			return false;
		}

		DailyOverdrawnFee = amount;
		Changed = true;
		actor.OutputHandler.Send(
			$"This bank account type will now charge a flat fee of {Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} per day when the account if overdrawn.");
		return true;
	}

	private bool BuildingCommandMaximumOverdraft(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter a valid amount of currency.");
			return false;
		}

		if (!Currency.TryGetBaseCurrency(command.SafeRemainingArgument, out var amount))
		{
			actor.OutputHandler.Send(
				$"The supplied argument ({command.SafeRemainingArgument.ColourCommand()}) is not a valid amount of {Currency.Name.ColourValue()}.");
			return false;
		}

		MaximumOverdrawAmount = amount;
		Changed = true;
		actor.OutputHandler.Send(
			$"This bank account type will now have a maximum overdraw of {Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandInterest(ICharacter actor, StringStack command, bool annual)
	{
		if (command.IsFinished || !command.SafeRemainingArgument.TryParsePercentageDecimal(out var value))
		{
			actor.OutputHandler.Send("You must specify an interest rate as a percentage.");
			return false;
		}

		var days = Bank.EconomicZone.FinancialPeriodReferenceCalendar.Months.Sum(x => x.NormalDays);
		var months = Bank.EconomicZone.FinancialPeriodReferenceCalendar.Months.Count;
		var avgMonthDays = days / months;

		if (annual)
		{
			var daily = ((decimal)Math.Pow((double)(1.0M + value), 1.0 / months) - 1.0M) / avgMonthDays;
			DailyInterestRate = daily;
			Changed = true;
			actor.OutputHandler.Send(
				$"The approximate annual interest rate will now be {value.ToString("P4", actor).ColourValue()}\nThis is equivalent to {daily.ToString("P4", actor).ColourValue()} accrued daily compounded monthly.");
			return true;
		}

		DailyInterestRate = value;
		var yearly = (decimal)Math.Pow((double)(value * avgMonthDays), months);
		actor.OutputHandler.Send(
			$"The daily accural (monthly compounded) interest rate for this account type is now {value.ToString("P4", actor).ColourValue()}.\nThis is equivalent to {yearly.ToString("P4", actor).ColourValue()} yearly.");
		return true;
	}

	private bool BuildingCommandDailyFee(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter a valid amount of currency.");
			return false;
		}

		if (!Currency.TryGetBaseCurrency(command.SafeRemainingArgument, out var amount))
		{
			actor.OutputHandler.Send(
				$"The supplied argument ({command.SafeRemainingArgument.ColourCommand()}) is not a valid amount of {Currency.Name.ColourValue()}.");
			return false;
		}

		DailyFee = amount;
		Changed = true;
		actor.OutputHandler.Send(
			$"This bank account type will now charge a flat fee of {Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} per day.");
		return true;
	}

	private bool BuildingCommandDescription(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What customer description do you want to give to this account type?");
			return false;
		}

		CustomerDescription = command.SafeRemainingArgument.ProperSentences();
		Changed = true;
		actor.OutputHandler.Send(
			$"This bank account type will now have a description of {CustomerDescription.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandPaymentItem(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must either specify an item prototype or use {"none".ColourCommand()} to clear.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			PaymentItemPrototype = null;
			Changed = true;
			actor.OutputHandler.Send($"This bank account no longer issues a payment item.");
			return true;
		}

		if (!long.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send("You must enter the id number of the item prototype.");
			return false;
		}

		var item = Gameworld.ItemProtos.Get(value);
		if (item is null)
		{
			actor.OutputHandler.Send("There is no such item prototype.");
			return false;
		}

		if (item.Status != RevisionStatus.Current)
		{
			actor.OutputHandler.Send("Only approved items can be used.");
			return false;
		}

		PaymentItemPrototype = item.Id;
		Changed = true;
		actor.OutputHandler.Send(
			$"This bank account type will now use item #{value.ToString("N0", actor)} ({item.ShortDescription.Colour(item.CustomColour ?? Telnet.Green)}) as its payment item.");
		return true;
	}

	private bool BuildingCommandNumberOfPaymentItems(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.SafeRemainingArgument, out var value) || value < 0)
		{
			actor.OutputHandler.Send(
				"You must either enter a number of payment items that can be issued at one time, or else specify 0 to disable them.");
			return false;
		}

		NumberOfPermittedPaymentItems = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"Holders of this account will now be able to request {NumberOfPermittedPaymentItems.ToString("N0", actor).ColourValue()} payment items simultaneously.");
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to this account type?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Bank.BankAccountTypes.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				$"The {Bank.Name.ColourName()} bank already has an account type with that name. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the {Name.ColourName()} bank account type to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Bank Account Type #{Id.ToString("N0", actor)} - {Name.ColourName()}");
		sb.AppendLine($"Customer Description: {CustomerDescription.ColourCommand()}");
		sb.AppendLine(
			$"Can Close Prog: {CanCloseAccountProg?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine(
			$"Can Open Prog (Character): {CanOpenAccountProgCharacter?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine(
			$"Can Open Prog (Shop): {CanOpenAccountProgShop?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine(
			$"Can Open Prog (Clan): {CanOpenAccountProgClan?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine(
			$"Maximum Overdraw: {Currency.Describe(MaximumOverdrawAmount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		var daysInYear = Bank.EconomicZone.FinancialPeriodReferenceCalendar.Months.Sum(x => x.NormalDays);
		sb.AppendLine(
			$"Daily Interest Rate: {DailyInterestRate.ToString("P4", actor).ColourValue()} (~{(Math.Pow(1.0 + (double)DailyInterestRate, daysInYear) - 1.0).ToString("P4", actor).ColourValue()} annual)");
		sb.AppendLine(
			$"Overdrawn Interest Rate: {DailyOverdrawnInterestRate.ToString("P4", actor).ColourValue()} (~{(Math.Pow(1.0 + (double)DailyOverdrawnInterestRate, daysInYear) - 1.0).ToString("P4", actor).ColourValue()} annual)");
		sb.AppendLine(
			$"Daily Fee: {Currency.Describe(DailyFee, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		sb.AppendLine(
			$"Overdrawn Daily Fee: {Currency.Describe(DailyOverdrawnFee, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		sb.AppendLine(
			$"Overdraw Transaction Fee: {Currency.Describe(OverdrawFeeFlat, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} and {OverdrawFeeRate.ToString("P4", actor).ColourValue()} transaction value");
		sb.AppendLine(
			$"Withdraw Fee: {Currency.Describe(WithdrawalFleeFlat, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} and {WithdrawalFleeRate.ToString("P4", actor).ColourValue()} transaction value");
		sb.AppendLine(
			$"Deposit Fee: {Currency.Describe(DepositFeeFlat, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} and {DepositFeeRate.ToString("P4", actor).ColourValue()} transaction value");
		sb.AppendLine(
			$"Transfer Fee: {Currency.Describe(TransferFeeFlat, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} and {TransferFeeRate.ToString("P4", actor).ColourValue()} transaction value");
		sb.AppendLine(
			$"Transfer (Other Bank) Fee: {Currency.Describe(TransferFeeOtherBankFlat, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} and {TransferFeeOtherBankRate.ToString("P4", actor).ColourValue()} transaction value");

		sb.AppendLine($"Maximum Payment Items: {NumberOfPermittedPaymentItems.ToString("N0", actor).ColourValue()}");
		sb.AppendLine(
			$"Payment Item Proto: {Gameworld.ItemProtos.Get(PaymentItemPrototype ?? 0L)?.EditHeader() ?? "None".ColourError()}");

		return sb.ToString();
	}

	public (bool Truth, string Reason) CanOpenAccount(ICharacter actor)
	{
		if (CanOpenAccountProgCharacter?.Execute<bool?>(actor) != true)
		{
			return (false, "You are not allowed to open that account type.");
		}

		return (true, string.Empty);
	}

	public (bool Truth, string Reason) CanOpenAccount(IClan clan)
	{
		if (CanOpenAccountProgClan?.Execute<bool?>(clan) != true)
		{
			return (false, "Your clan is not allowed to open that account type.");
		}

		return (true, string.Empty);
	}

	public (bool Truth, string Reason) CanOpenAccount(IShop shop)
	{
		if (CanOpenAccountProgShop?.Execute<bool?>(shop) != true)
		{
			return (false, "Your shop is not allowed to open that account type.");
		}

		return (true, string.Empty);
	}

	public (bool Truth, string Reason) CanCloseAccount(ICharacter actor, IBankAccount account)
	{
		if (account.CurrentBalance != 0.0M)
		{
			return (false, "You can only close a bank account with a balance of zero.");
		}

		if (CanCloseAccountProg?.Execute<bool?>(actor) != true)
		{
			return (false, "You cannot close that account type. See your bank manager.");
		}

		return (true, string.Empty);
	}

	public IBankAccount OpenAccount(ICharacter actor)
	{
		using (new FMDB())
		{
			var newAccount = new Models.BankAccount
			{
				BankId = Bank.Id,
				Name = Name.CollapseString(),
				BankAccountTypeId = Id,
				AccountCreationDate = Bank.EconomicZone.ZoneForTimePurposes
				                          .DateTime(Bank.EconomicZone.FinancialPeriodReferenceCalendar)
				                          .GetDateTimeString(),
				AccountNumber = Bank.BankAccounts.Select(x => x.AccountNumber).DefaultIfEmpty(0).Max() + 1,
				AccountStatus = (int)BankAccountStatus.Active,
				CurrentBalance = 0.0M,
				CurrentMonthFees = 0.0M,
				CurrentMonthInterest = 0.0M,
				AccountOwnerCharacterId = actor.Id
			};
			FMDB.Context.BankAccounts.Add(newAccount);
			FMDB.Context.SaveChanges();
			return new BankAccount(newAccount, Bank);
		}
	}

	public IBankAccount OpenAccount(IClan clan)
	{
		using (new FMDB())
		{
			var newAccount = new Models.BankAccount
			{
				BankId = Bank.Id,
				Name = Name.CollapseString(),
				BankAccountTypeId = Id,
				AccountCreationDate = Bank.EconomicZone.ZoneForTimePurposes
				                          .DateTime(Bank.EconomicZone.FinancialPeriodReferenceCalendar)
				                          .GetDateTimeString(),
				AccountNumber = Bank.BankAccounts.Select(x => x.AccountNumber).DefaultIfEmpty(0).Max() + 1,
				AccountStatus = (int)BankAccountStatus.Active,
				CurrentBalance = 0.0M,
				CurrentMonthFees = 0.0M,
				CurrentMonthInterest = 0.0M,
				AccountOwnerClanId = clan.Id
			};
			FMDB.Context.BankAccounts.Add(newAccount);
			FMDB.Context.SaveChanges();
			return new BankAccount(newAccount, Bank);
		}
	}

	public IBankAccount OpenAccount(IShop shop)
	{
		using (new FMDB())
		{
			var newAccount = new Models.BankAccount
			{
				BankId = Bank.Id,
				Name = Name.CollapseString(),
				BankAccountTypeId = Id,
				AccountCreationDate = Bank.EconomicZone.ZoneForTimePurposes
				                          .DateTime(Bank.EconomicZone.FinancialPeriodReferenceCalendar)
				                          .GetDateTimeString(),
				AccountNumber = Bank.BankAccounts.Select(x => x.AccountNumber).DefaultIfEmpty(0).Max() + 1,
				AccountStatus = (int)BankAccountStatus.Active,
				CurrentBalance = 0.0M,
				CurrentMonthFees = 0.0M,
				CurrentMonthInterest = 0.0M,
				AccountOwnerShopId = shop.Id
			};
			FMDB.Context.BankAccounts.Add(newAccount);
			FMDB.Context.SaveChanges();
			return new BankAccount(newAccount, Bank);
		}
	}

	public IBank Bank { get; set; }
	public ICurrency Currency => Bank.PrimaryCurrency;

	public string ShowToCustomer(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"{Name.ColourName()} with {Bank.Name.ColourName()}");
		sb.AppendLine();
		sb.AppendLine(CustomerDescription.Wrap(actor.InnerLineFormatLength, "\t"));
		sb.AppendLine();
		var days = Bank.EconomicZone.FinancialPeriodReferenceCalendar.Months.Sum(x => x.NormalDays);
		sb.AppendLine(
			$"Interest Rate: {(Math.Pow((double)(1.0M + DailyInterestRate), days) - 1.0).ToString("P4", actor).ColourValue()} per annum");
		if (DailyFee == 0.0M)
		{
			sb.AppendLine($"Daily Fee: {"None".ColourValue()}");
		}
		else
		{
			sb.AppendLine(
				$"Daily Fee: {Currency.Describe(DailyFee, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		}

		if (MaximumOverdrawAmount > 0.0M)
		{
			sb.AppendLine(
				$"Maximum Overdraw Amount: {Currency.Describe(MaximumOverdrawAmount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
			if (OverdrawFeeFlat != 0.0M || OverdrawFeeRate != 0.0M)
			{
				if (OverdrawFeeFlat == 0.0M)
				{
					sb.AppendLine($"Overdraw Fee: {OverdrawFeeRate.ToString("P4", actor).ColourValue()}");
				}
				else if (OverdrawFeeRate == 0.0M)
				{
					sb.AppendLine(
						$"Overdraw Fee: {Currency.Describe(OverdrawFeeFlat, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
				}
				else
				{
					sb.AppendLine(
						$"Overdraw Fee: {Currency.Describe(OverdrawFeeFlat, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} + {OverdrawFeeRate.ToString("P4", actor).ColourValue()}");
				}
			}
			else
			{
				sb.AppendLine($"Overdraw Fee: {"None".ColourValue()}");
			}

			if (DailyOverdrawnFee != 0.0M || DailyOverdrawnInterestRate != 0.0M)
			{
				if (DailyOverdrawnFee == 0.0M)
				{
					sb.AppendLine(
						$"Daily Overdrawn Fee: {DailyOverdrawnInterestRate.ToString("P4", actor).ColourValue()}");
				}
				else if (DailyOverdrawnInterestRate == 0.0M)
				{
					sb.AppendLine(
						$"Daily Overdrawn Fee: {Currency.Describe(DailyOverdrawnFee, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
				}
				else
				{
					sb.AppendLine(
						$"Daily Overdrawn Fee: {Currency.Describe(DailyOverdrawnFee, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} + {DailyOverdrawnInterestRate.ToString("P4", actor).ColourValue()}");
				}
			}
			else
			{
				sb.AppendLine($"Daily Overdrawn Fee: {"None".ColourValue()}");
			}
		}


		if (WithdrawalFleeFlat != 0.0M || WithdrawalFleeRate != 0.0M)
		{
			if (WithdrawalFleeFlat == 0.0M)
			{
				sb.AppendLine($"Withdrawal Fee: {WithdrawalFleeRate.ToString("P4", actor).ColourValue()}");
			}
			else if (WithdrawalFleeRate == 0.0M)
			{
				sb.AppendLine(
					$"Withdrawal Fee: {Currency.Describe(WithdrawalFleeFlat, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
			}
			else
			{
				sb.AppendLine(
					$"Withdrawal Fee: {Currency.Describe(WithdrawalFleeFlat, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} + {WithdrawalFleeRate.ToString("P4", actor).ColourValue()}");
			}
		}
		else
		{
			sb.AppendLine($"Withdrawal Fee: {"None".ColourValue()}");
		}

		if (DepositFeeFlat != 0.0M || DepositFeeRate != 0.0M)
		{
			if (DepositFeeFlat == 0.0M)
			{
				sb.AppendLine($"Deposit Fee: {DepositFeeRate.ToString("P4", actor).ColourValue()}");
			}
			else if (DepositFeeRate == 0.0M)
			{
				sb.AppendLine(
					$"Deposit Fee: {Currency.Describe(DepositFeeFlat, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
			}
			else
			{
				sb.AppendLine(
					$"Deposit Fee: {Currency.Describe(DepositFeeFlat, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} + {DepositFeeRate.ToString("P4", actor).ColourValue()}");
			}
		}
		else
		{
			sb.AppendLine($"Deposit Fee: {"None".ColourValue()}");
		}

		if (TransferFeeFlat != 0.0M || TransferFeeRate != 0.0M)
		{
			if (TransferFeeFlat == 0.0M)
			{
				sb.AppendLine($"Transfer Fee: {TransferFeeRate.ToString("P4", actor).ColourValue()}");
			}
			else if (TransferFeeRate == 0.0M)
			{
				sb.AppendLine(
					$"Transfer Fee: {Currency.Describe(TransferFeeFlat, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
			}
			else
			{
				sb.AppendLine(
					$"Transfer Fee: {Currency.Describe(TransferFeeFlat, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} + {TransferFeeRate.ToString("P4", actor).ColourValue()}");
			}
		}
		else
		{
			sb.AppendLine($"Transfer Fee: {"None".ColourValue()}");
		}

		if (TransferFeeOtherBankFlat != 0.0M || TransferFeeOtherBankRate != 0.0M)
		{
			if (TransferFeeOtherBankFlat == 0.0M)
			{
				sb.AppendLine(
					$"Transfer (Other Bank) Fee: {TransferFeeOtherBankRate.ToString("P4", actor).ColourValue()}");
			}
			else if (TransferFeeOtherBankRate == 0.0M)
			{
				sb.AppendLine(
					$"Transfer (Other Bank) Fee: {Currency.Describe(TransferFeeOtherBankFlat, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
			}
			else
			{
				sb.AppendLine(
					$"Transfer (Other Bank) Fee: {Currency.Describe(TransferFeeOtherBankFlat, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} + {TransferFeeOtherBankRate.ToString("P4", actor).ColourValue()}");
			}
		}
		else
		{
			sb.AppendLine($"Transfer (Other Bank) Fee: {"None".ColourValue()}");
		}

		var proto = Gameworld.ItemProtos.Get(PaymentItemPrototype ?? 0);
		if (proto is not null && proto.Status == RevisionStatus.Current && NumberOfPermittedPaymentItems > 0)
		{
			sb.AppendLine(
				$"Payment Items Available: {proto.ShortDescription.Colour(proto.CustomColour ?? Telnet.Green)} (x{NumberOfPermittedPaymentItems.ToString("N0", actor)})");
		}

		return sb.ToString();
	}

	public string CustomerDescription { get; set; }

	public void DoTransactionFeesWithdrawal(IBankAccount account, decimal amount)
	{
		var fee = amount * WithdrawalFleeRate + WithdrawalFleeFlat;
		if (fee != 0.0M)
		{
			account.ChargeFee(fee, BankTransactionType.Withdrawal, "Withdrawal fee");
		}
	}

	public void DoOverdrawFees(IBankAccount account, decimal amount)
	{
		var fee = amount * OverdrawFeeRate + OverdrawFeeFlat;
		if (fee != 0.0M)
		{
			account.ChargeFee(fee, BankTransactionType.OverdraftFee, "Overdrawn Transaction fee");
		}
	}

	public void DoTransactionFeesDeposit(IBankAccount account, decimal amount)
	{
		var fee = amount * DepositFeeRate + DepositFeeFlat;
		if (fee != 0.0M)
		{
			account.ChargeFee(fee, BankTransactionType.Deposit, "Deposit fee");
		}
	}

	public void DoTransactionFeesDepositFromTransfer(IBankAccount account, decimal amount)
	{
		// Nothing for now
	}

	public void DoTransactionFeesTransfer(IBankAccount account, decimal amount)
	{
		var fee = amount * TransferFeeRate + TransferFeeFlat;
		if (fee != 0.0M)
		{
			account.ChargeFee(fee, BankTransactionType.Transfer, "Transfer fee");
		}
	}

	public void DoTransactionFeesTransferOtherBank(IBankAccount account, decimal amount)
	{
		var fee = amount * TransferFeeOtherBankRate + TransferFeeOtherBankFlat;
		if (fee != 0.0M)
		{
			account.ChargeFee(fee, BankTransactionType.InterBankTransfer, "Cross-Bank Transfer fee");
		}
	}

	public void DoDailyAccountFees(IBankAccount account)
	{
		if (account.CurrentBalance < 0.0M)
		{
			var overdrawnfee = account.CurrentBalance * DailyOverdrawnInterestRate + DailyOverdrawnFee;
			if (overdrawnfee != 0.0M)
			{
				account.DoDailyFee(overdrawnfee);
			}

			return;
		}

		if (DailyFee != 0.0M)
		{
			account.DoDailyFee(DailyFee);
		}

		var interest = DailyInterestRate * account.CurrentBalance;
		if (interest != 0.0M)
		{
			account.DepositInterest(interest);
		}
	}

	public decimal MaximumOverdrawAmount { get; set; }
	public decimal WithdrawalFleeFlat { get; set; }
	public decimal WithdrawalFleeRate { get; set; }
	public decimal DepositFeeFlat { get; set; }
	public decimal DepositFeeRate { get; set; }
	public decimal TransferFeeFlat { get; set; }
	public decimal TransferFeeRate { get; set; }
	public decimal TransferFeeOtherBankFlat { get; set; }
	public decimal TransferFeeOtherBankRate { get; set; }
	public decimal DailyFee { get; set; }
	public decimal DailyInterestRate { get; set; }
	public decimal OverdrawFeeFlat { get; set; }
	public decimal OverdrawFeeRate { get; set; }
	public decimal DailyOverdrawnFee { get; set; }
	public decimal DailyOverdrawnInterestRate { get; set; }

	public IFutureProg CanOpenAccountProgCharacter { get; set; }
	public IFutureProg CanOpenAccountProgClan { get; set; }
	public IFutureProg CanOpenAccountProgShop { get; set; }
	public IFutureProg CanCloseAccountProg { get; set; }

	public int NumberOfPermittedPaymentItems { get; set; }
	public long? PaymentItemPrototype { get; set; }

	#region FutureProgs

	public FutureProgVariableTypes Type => FutureProgVariableTypes.BankAccountType;
	public object GetObject => this;

	public IFutureProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "id":
				return new NumberVariable(Id);
			case "name":
				return new TextVariable(Name);
			case "bank":
				return Bank;
			default:
				throw new ApplicationException($"Invalid property {property} requested in BankAccount.GetProperty");
		}
	}

	private static IReadOnlyDictionary<string, FutureProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, FutureProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", FutureProgVariableTypes.Number },
			{ "name", FutureProgVariableTypes.Text },
			{ "bank", FutureProgVariableTypes.Bank }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "The ID of the bank account" },
			{ "name", "The name of the bank account" },
			{ "bank", "The bank this account is with" }
		};
	}

	public new static void RegisterFutureProgCompiler()
	{
		FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.BankAccountType,
			DotReferenceHandler(), DotReferenceHelp());
	}

	#endregion
}