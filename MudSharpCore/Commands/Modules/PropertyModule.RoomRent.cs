using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Commands.Helpers;
using MudSharp.Community;
using MudSharp.Economy;
using MudSharp.Economy.Banking;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Payment;
using MudSharp.Economy.Property;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSpanParserUtil;

namespace MudSharp.Commands.Modules;

internal partial class PropertyModule
{
	public const string RoomRentHelp =
		@"The roomrent command manages short-term hotel-style room rentals inside properties.

Guest commands:

	#3roomrent list [property]#0 - lists hotel rooms available to rent
	#3roomrent rent <property> <room> <duration> [bankcode:account]#0 - rents a hotel room
	#3roomrent checkout [property] [room]#0 - ends your stay and returns your deposit balance
	#3roomrent claim [property]#0 - claims deposit refunds and held lost property
	#3roomrent pay <property> [amount] [bankcode:account]#0 - pays an outstanding hotel balance

Hotel manager commands:

	#3roomrent show <property>#0 - shows hotel setup and room state
	#3roomrent request <property>#0 - requests hotel approval from the economic zone
	#3roomrent surrender <property>#0 - surrenders hotel approval
	#3roomrent bank <property> <bankcode:account>#0 - sets the hotel bank account
	#3roomrent prog <property> <prog|none>#0 - sets who may rent rooms
	#3roomrent retention <property> <timespan>#0 - sets lost property retention
	#3roomrent taxes <property>#0 - pays outstanding hotel rental taxes
	#3roomrent ban <property> <character>#0 - bans a patron
	#3roomrent unban <property> <character>#0 - removes a patron ban
	#3roomrent room list <property>#0 - lists hotel rooms
	#3roomrent room add <property> <price/day> <deposit> <min> <max>#0 - adds the current room as a hotel room
	#3roomrent room remove <property> <room>#0 - removes a hotel room
	#3roomrent room price <property> <room> <price/day>#0 - changes room price
	#3roomrent room deposit <property> <room> <deposit>#0 - changes security deposit
	#3roomrent room length <property> <room> <min> <max>#0 - changes min/max duration
	#3roomrent room name <property> <room> <name>#0 - renames a hotel room
	#3roomrent room listed <property> <room>#0 - toggles whether a room is listed
	#3roomrent key add <property> <room> <keyname>#0 - assigns an existing property key to a hotel room
	#3roomrent key remove <property> <room> <keyname>#0 - removes a key from a hotel room
	#3roomrent furnish list <property> <room>#0 - lists furnishings
	#3roomrent furnish add <property> <room> <item> [value]#0 - marks an item as furnishing
	#3roomrent furnish remove <property> <room> <item>#0 - removes a furnishing marker
	#3roomrent lost list <property>#0 - lists lost property
	#3roomrent lost extend <property> <#> <timespan>#0 - extends lost property retention
	#3roomrent lost release <property> <#>#0 - removes a bundle from lost property

Economic zone manager commands:

	#3roomrent approve <property>#0 - approves a requested hotel license";

	[RequiredCharacterState(CharacterState.Conscious)]
	[NoCombatCommand]
	[NoHideCommand]
	[PlayerCommand("RoomRent", "roomrent")]
	[HelpInfo("roomrent", RoomRentHelp, AutoHelp.HelpArg)]
	[CustomModuleName("Economy")]
	protected static void RoomRent(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.PopForSwitch())
		{
			case "list":
				RoomRentList(actor, ss);
				return;
			case "rent":
				RoomRentRent(actor, ss);
				return;
			case "checkout":
			case "check-out":
				RoomRentCheckout(actor, ss);
				return;
			case "claim":
				RoomRentClaim(actor, ss);
				return;
			case "pay":
				RoomRentPay(actor, ss);
				return;
			case "show":
				RoomRentShow(actor, ss);
				return;
			case "request":
				RoomRentRequest(actor, ss);
				return;
			case "surrender":
				RoomRentSurrender(actor, ss);
				return;
			case "approve":
				RoomRentApprove(actor, ss);
				return;
			case "bank":
				RoomRentBank(actor, ss);
				return;
			case "prog":
				RoomRentProg(actor, ss);
				return;
			case "retention":
			case "lostretention":
				RoomRentRetention(actor, ss);
				return;
			case "tax":
			case "taxes":
				RoomRentTaxes(actor, ss);
				return;
			case "ban":
				RoomRentBan(actor, ss, true);
				return;
			case "unban":
			case "pardon":
				RoomRentBan(actor, ss, false);
				return;
			case "room":
				RoomRentRoom(actor, ss);
				return;
			case "key":
			case "keys":
				RoomRentKey(actor, ss);
				return;
			case "furnish":
			case "furnishing":
			case "furnishings":
				RoomRentFurnish(actor, ss);
				return;
			case "lost":
			case "lostproperty":
				RoomRentLost(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(RoomRentHelp.SubstituteANSIColour());
				return;
		}
	}

	private static IProperty CurrentProperty(ICharacter actor)
	{
		return actor.Gameworld.Properties.FirstOrDefault(x => x.PropertyLocations.Contains(actor.Location));
	}

	private static IProperty FindRoomRentProperty(ICharacter actor, StringStack ss, IEnumerable<IProperty> source,
		string prompt, string failure)
	{
		var properties = source.ToList();
		if (ss.IsFinished)
		{
			var current = CurrentProperty(actor);
			if (current != null && properties.Contains(current))
			{
				return current;
			}

			actor.OutputHandler.Send(prompt);
			return null;
		}

		var text = ss.PopSpeech();
		if (text.EqualTo("here"))
		{
			var current = CurrentProperty(actor);
			if (current != null && properties.Contains(current))
			{
				return current;
			}

			actor.OutputHandler.Send(failure);
			return null;
		}

		var property = properties.GetFromItemListByKeywordIncludingNames(text, actor);
		if (property == null)
		{
			actor.OutputHandler.Send(failure);
			return null;
		}

		return property;
	}

	private static IProperty FindOwnedHotelProperty(ICharacter actor, StringStack ss, string action)
	{
		return FindRoomRentProperty(actor, ss,
			actor.Gameworld.Properties.Where(x => x.IsAuthorisedOwner(actor)),
			$"Which property do you want to {action}? You can use {"here".ColourCommand()} if you are standing inside it.",
			"You do not manage such a property.");
	}

	private static IHotelRoom FindHotelRoom(ICharacter actor, IProperty property, StringStack ss, string action)
	{
		if (ss.IsFinished)
		{
			var currentRoom = property.HotelRoomForCell(actor.Location);
			if (currentRoom != null)
			{
				return currentRoom;
			}

			actor.OutputHandler.Send($"Which hotel room do you want to {action}?");
			return null;
		}

		var text = ss.PopSpeech();
		if (text.EqualTo("here"))
		{
			var currentRoom = property.HotelRoomForCell(actor.Location);
			if (currentRoom != null)
			{
				return currentRoom;
			}

			actor.OutputHandler.Send("Your current location is not a configured room for that hotel.");
			return null;
		}

		var room = property.HotelRooms.GetFromItemListByKeyword(text, actor) ??
				   property.HotelRooms.FirstOrDefault(x => x.Name.EqualTo(text));
		if (room == null)
		{
			actor.OutputHandler.Send($"The {property.Name.ColourName()} hotel does not have such a room.");
			return null;
		}

		return room;
	}

	private static ICharacter GetCharacterByIdOrName(ICharacter actor, string text)
	{
		if (long.TryParse(text, out var value))
		{
			return actor.Gameworld.TryGetCharacter(value, true);
		}

		return actor.TargetActor(text) ?? actor.Gameworld.Characters.GetByPersonalName(text);
	}

	private static bool CanManageEconomicZone(ICharacter actor, IEconomicZone zone)
	{
		return actor.IsAdministrator() ||
			   (zone.ControllingClan != null && actor.ClanMemberships.Any(x =>
				   x.Clan == zone.ControllingClan &&
				   x.NetPrivileges.HasFlag(ClanPrivilegeType.CanManageEconomicZones)));
	}

	private static string HotelRoomAvailability(IHotelRoom room)
	{
		return room.ActiveRental == null
			? "Now"
			: room.ActiveRental.EndTime.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short);
	}

	private static void RoomRentList(ICharacter actor, StringStack ss)
	{
		var properties = actor.Gameworld.Properties
							  .Where(x => x.IsApprovedHotel && x.HotelRooms.Any(y => y.Listed))
							  .ToList();
		if (!ss.IsFinished)
		{
			var property = FindRoomRentProperty(actor, ss, properties,
				"Which hotel property do you want to list rooms for?",
				"There is no such approved hotel.");
			if (property == null)
			{
				return;
			}

			properties = new List<IProperty> { property };
		}

		var rows = properties
				   .SelectMany(property => property.HotelRooms
					   .Where(room => room.Listed)
					   .Select(room => new List<string>
					   {
						   property.Name,
						   room.Name,
						   property.EconomicZone.Currency.Describe(room.PricePerDay, CurrencyDescriptionPatternType.Short),
						   property.EconomicZone.Currency.Describe(room.SecurityDeposit, CurrencyDescriptionPatternType.Short),
						   $"{room.MinimumDuration.Describe(actor)} to {room.MaximumDuration.Describe(actor)}",
						   HotelRoomAvailability(room)
					   }))
				   .ToList();

		if (!rows.Any())
		{
			actor.OutputHandler.Send("There are no hotel rooms currently available to rent.");
			return;
		}

		actor.OutputHandler.Send(StringUtilities.GetTextTable(rows,
			new List<string> { "Property", "Room", "Daily", "Deposit", "Duration", "Available" },
			actor.LineFormatLength,
			colour: Telnet.Green,
			unicodeTable: actor.Account.UseUnicode));
	}

	private static (TimeSpan Duration, string BankReference)? ParseDurationAndOptionalBank(ICharacter actor,
		StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("How long do you want to rent the room for?");
			return null;
		}

		var text = ss.SafeRemainingArgument;
		string bankReference = null;
		var lastSpace = text.LastIndexOf(' ');
		if (lastSpace > 0)
		{
			var possibleBank = text[(lastSpace + 1)..];
			if (possibleBank.Contains(':'))
			{
				bankReference = possibleBank;
				text = text[..lastSpace].Trim();
			}
		}

		if (!TimeSpanParser.TryParse(text, Units.Days, Units.Hours, out var duration))
		{
			actor.OutputHandler.Send(
				$"That was not a valid time span.\n{"Note: Years and Months are not supported, use Weeks or Days in that case".ColourCommand()}");
			return null;
		}

		return (duration, bankReference);
	}

	private static bool TryPayHotel(ICharacter actor, IProperty property, decimal amount, string bankReference,
		string reference)
	{
		if (amount <= 0.0M)
		{
			return true;
		}

		if (property.HotelBankAccount == null)
		{
			actor.OutputHandler.Send($"The {property.Name.ColourName()} hotel does not have a bank account configured.");
			return false;
		}

		if (string.IsNullOrWhiteSpace(bankReference))
		{
			var payment = new OtherCashPayment(property.EconomicZone.Currency, actor);
			if (payment.AccessibleMoneyForPayment() < amount)
			{
				actor.OutputHandler.Send(
					$"You aren't holding enough money to pay {property.EconomicZone.Currency.Describe(amount, CurrencyDescriptionPatternType.Short).ColourValue()} to {property.Name.ColourName()}.\nYou are only holding {property.EconomicZone.Currency.Describe(payment.AccessibleMoneyForPayment(), CurrencyDescriptionPatternType.Short).ColourValue()}.");
				return false;
			}

			payment.TakePayment(amount);
		}
		else
		{
			(IBankAccount account, string error) = Bank.FindBankAccount(bankReference, null, actor);
			if (account == null)
			{
				actor.OutputHandler.Send(error);
				return false;
			}

			if (!account.IsAuthorisedAccountUser(actor))
			{
				actor.OutputHandler.Send("You are not an authorised user for that bank account.");
				return false;
			}

			(bool truth, string accountError) = account.CanWithdraw(amount, true);
			if (!truth)
			{
				actor.OutputHandler.Send(accountError);
				return false;
			}

			account.WithdrawFromTransaction(amount, $"Hotel payment to {property.Name}");
			account.Bank.CurrencyReserves[property.EconomicZone.Currency] -= amount;
		}

		property.HotelBankAccount.DepositFromTransaction(amount, reference);
		property.HotelBankAccount.Bank.CurrencyReserves[property.EconomicZone.Currency] += amount;
		property.HotelBankAccount.Bank.Changed = true;
		return true;
	}

	private static void GiveItemToActor(ICharacter actor, IGameItem item, string groundMessage)
	{
		item.RoomLayer = actor.RoomLayer;
		if (actor.Body.CanGet(item, 0))
		{
			actor.Body.Get(item, silent: true);
			return;
		}

		actor.Location.Insert(item, true);
		actor.OutputHandler.Send(groundMessage);
	}

	private static bool TryPayoutHotelBalance(ICharacter actor, IProperty property)
	{
		var balance = property.HotelBalanceFor(actor);
		if (balance <= 0.0M)
		{
			return false;
		}

		if (property.HotelBankAccount == null)
		{
			actor.OutputHandler.Send(
				$"The {property.Name.ColourName()} hotel owes you {property.EconomicZone.Currency.Describe(balance, CurrencyDescriptionPatternType.Short).ColourValue()}, but it does not have a bank account configured.");
			return false;
		}

		(bool truth, string error) = property.HotelBankAccount.CanWithdraw(balance, false);
		if (!truth)
		{
			actor.OutputHandler.Send(
				$"The {property.Name.ColourName()} hotel owes you {property.EconomicZone.Currency.Describe(balance, CurrencyDescriptionPatternType.Short).ColourValue()}, but its bank account cannot currently pay that out: {error}");
			return false;
		}

		property.HotelBankAccount.WithdrawFromTransaction(balance, $"Hotel balance paid to {actor.PersonalName.GetName(NameStyle.FullName)}");
		property.HotelBankAccount.Bank.CurrencyReserves[property.EconomicZone.Currency] -= balance;
		property.HotelBankAccount.Bank.Changed = true;
		property.AdjustHotelBalance(actor, -balance);

		var cash = CurrencyGameItemComponentProto.CreateNewCurrencyPile(property.EconomicZone.Currency,
			property.EconomicZone.Currency.FindCoinsForAmount(balance, out _));
		actor.Gameworld.Add(cash);
		GiveItemToActor(actor, cash, "You couldn't hold the money, so it is on the ground.");
		actor.OutputHandler.Send(
			$"You claim {property.EconomicZone.Currency.Describe(balance, CurrencyDescriptionPatternType.Short).ColourValue()} owed to you by {property.Name.ColourName()}.");
		return true;
	}

	private static void GiveRoomKeys(ICharacter actor, IHotelRoom room)
	{
		var keys = room.Keys.ToList();
		if (!keys.Any())
		{
			return;
		}

		IGameItem givenItem;
		if (keys.Count == 1)
		{
			givenItem = keys.Single().GameItem;
		}
		else
		{
			givenItem = PileGameItemComponentProto.CreateNewBundle(keys.Select(x => x.GameItem));
			actor.Gameworld.Add(givenItem);
		}

		GiveItemToActor(actor, givenItem, $"You couldn't hold {givenItem.HowSeen(actor)}, so it is on the ground.");
	}

	private static void ReturnHeldRoomKeys(ICharacter actor, IHotelRoom room)
	{
		foreach (var key in room.Keys.Where(x => actor.Body.AllItems.Contains(x.GameItem)).ToList())
		{
			key.GameItem.ContainedIn?.Take(key.GameItem);
			key.GameItem.InInventoryOf?.Take(key.GameItem);
			key.GameItem.Location?.Extract(key.GameItem);
			key.GameItem.Quit();
			key.IsReturned = true;
		}
	}

	private static void RoomRentRent(ICharacter actor, StringStack ss)
	{
		var property = FindRoomRentProperty(actor, ss,
			actor.Gameworld.Properties.Where(x => x.IsApprovedHotel && x.HotelRooms.Any(y => y.Listed)),
			"Which hotel property do you want to rent a room from?",
			"There is no such approved hotel with listed rooms.");
		if (property == null)
		{
			return;
		}

		var room = FindHotelRoom(actor, property, ss, "rent");
		if (room == null)
		{
			return;
		}

		var durationAndBank = ParseDurationAndOptionalBank(actor, ss);
		if (durationAndBank == null)
		{
			return;
		}

		var duration = durationAndBank.Value.Duration;
		if (!property.CanRentHotelRoom(actor, room, duration, out var reason))
		{
			actor.OutputHandler.Send(reason);
			return;
		}

		var billedDays = Math.Max(1, (int)Math.Ceiling(duration.TotalDays));
		var rentalCharge = billedDays * room.PricePerDay;
		var taxCharge = property.EconomicZone.CalculateHotelTax(property, actor, rentalCharge);
		var total = rentalCharge + room.SecurityDeposit + taxCharge;
		if (!TryPayHotel(actor, property, total, durationAndBank.Value.BankReference, $"Hotel stay at {property.Name}"))
		{
			return;
		}

		property.RentHotelRoom(actor, room, duration, rentalCharge, taxCharge);
		GiveRoomKeys(actor, room);
		actor.OutputHandler.Send(
			$"You rent {room.Name.ColourName()} at {property.Name.ColourName()} for {duration.Describe(actor).ColourValue()}, paying {property.EconomicZone.Currency.Describe(rentalCharge, CurrencyDescriptionPatternType.Short).ColourValue()} rent, {property.EconomicZone.Currency.Describe(room.SecurityDeposit, CurrencyDescriptionPatternType.Short).ColourValue()} deposit and {property.EconomicZone.Currency.Describe(taxCharge, CurrencyDescriptionPatternType.Short).ColourValue()} tax.");
	}

	private static void RoomRentCheckout(ICharacter actor, StringStack ss)
	{
		IProperty property;
		IHotelRoom room;
		if (ss.IsFinished)
		{
			property = CurrentProperty(actor);
			room = property?.HotelRoomForCell(actor.Location);
			if (room?.ActiveRental?.GuestId != actor.Id)
			{
				var rentals = actor.Gameworld.Properties
								   .SelectMany(x => x.HotelRooms)
								   .Where(x => x.ActiveRental?.GuestId == actor.Id)
								   .ToList();
				if (rentals.Count == 1)
				{
					room = rentals.Single();
					property = room.Property;
				}
				else
				{
					actor.OutputHandler.Send("Which rented room do you want to check out of?");
					return;
				}
			}
		}
		else
		{
			property = FindRoomRentProperty(actor, ss,
				actor.Gameworld.Properties.Where(x => x.HotelRooms.Any(y => y.ActiveRental?.GuestId == actor.Id)),
				"Which hotel property do you want to check out of?",
				"You are not renting a room at such a hotel.");
			if (property == null)
			{
				return;
			}

			room = FindHotelRoom(actor, property, ss, "check out of");
		}

		if (room?.ActiveRental?.GuestId != actor.Id)
		{
			actor.OutputHandler.Send("You are not currently renting that room.");
			return;
		}

		ReturnHeldRoomKeys(actor, room);
		var refund = property.CompleteHotelStay(room, actor, false);
		if (refund < 0.0M)
		{
			actor.OutputHandler.Send(
				$"Your deposit did not cover all deductions. You now owe {property.Name.ColourName()} {property.EconomicZone.Currency.Describe(refund.InvertSign(), CurrencyDescriptionPatternType.Short).ColourError()}.");
			return;
		}

		if (refund == 0.0M)
		{
			actor.OutputHandler.Send("You check out, but there is no deposit balance to return.");
			return;
		}

		if (!TryPayoutHotelBalance(actor, property))
		{
			actor.OutputHandler.Send(
				$"Your deposit balance of {property.EconomicZone.Currency.Describe(refund, CurrencyDescriptionPatternType.Short).ColourValue()} remains claimable with {property.Name.ColourName()}.");
		}
	}

	private static void RoomRentClaim(ICharacter actor, StringStack ss)
	{
		var properties = actor.Gameworld.Properties
							  .Where(x => x.HotelBalanceFor(actor) > 0.0M ||
										  x.HotelLostProperties.Any(y =>
											  y.Status == HotelLostPropertyStatus.Held &&
											  (y.OwnerId == actor.Id || y.Bundle?.IsOwnedBy(actor) == true)))
							  .ToList();
		if (!ss.IsFinished)
		{
			var property = FindRoomRentProperty(actor, ss, properties,
				"Which hotel do you want to claim from?",
				"You do not have anything claimable at such a hotel.");
			if (property == null)
			{
				return;
			}

			properties = new List<IProperty> { property };
		}

		if (!properties.Any())
		{
			actor.OutputHandler.Send("You do not have any hotel balances or lost property to claim.");
			return;
		}

		var didAnything = false;
		foreach (var property in properties)
		{
			didAnything |= TryPayoutHotelBalance(actor, property);
			foreach (var lost in property.HotelLostProperties
										 .Where(x => x.Status == HotelLostPropertyStatus.Held &&
													 (x.OwnerId == actor.Id || x.Bundle?.IsOwnedBy(actor) == true))
										 .ToList())
			{
				lost.Bundle.Login();
				lost.Bundle.SetOwner(actor);
				property.ClaimHotelLostProperty(lost);
				GiveItemToActor(actor, lost.Bundle,
					$"You couldn't hold {lost.Bundle.HowSeen(actor)}, so it is on the ground.");
				actor.OutputHandler.Send(
					$"You claim {lost.Bundle.HowSeen(actor)} from {property.Name.ColourName()}'s lost property.");
				didAnything = true;
			}
		}

		if (!didAnything)
		{
			actor.OutputHandler.Send("Nothing was claimable right now.");
		}
	}

	private static void RoomRentPay(ICharacter actor, StringStack ss)
	{
		var property = FindRoomRentProperty(actor, ss,
			actor.Gameworld.Properties.Where(x => x.HotelBalanceFor(actor) < 0.0M || x.IsApprovedHotel),
			"Which hotel do you want to pay?",
			"There is no such hotel.");
		if (property == null)
		{
			return;
		}

		var owed = property.HotelBalanceFor(actor).InvertSign();
		if (owed <= 0.0M)
		{
			actor.OutputHandler.Send($"You do not owe {property.Name.ColourName()} anything.");
			return;
		}

		var amount = owed;
		string bankReference = null;
		if (!ss.IsFinished)
		{
			var amountText = ss.PopSpeech();
			if (amountText.Contains(':'))
			{
				bankReference = amountText;
			}
			else if (!property.EconomicZone.Currency.TryGetBaseCurrency(amountText, out amount))
			{
				actor.OutputHandler.Send(
					$"The text {amountText.ColourCommand()} is not a valid amount of {property.EconomicZone.Currency.Name.ColourName()}.");
				return;
			}

			if (!ss.IsFinished)
			{
				bankReference = ss.SafeRemainingArgument;
			}
		}

		if (amount <= 0.0M)
		{
			actor.OutputHandler.Send("You must pay a positive amount.");
			return;
		}

		if (!TryPayHotel(actor, property, amount, bankReference, $"Hotel debt payment to {property.Name}"))
		{
			return;
		}

		property.AdjustHotelBalance(actor, amount);
		actor.OutputHandler.Send(
			$"You pay {property.EconomicZone.Currency.Describe(amount, CurrencyDescriptionPatternType.Short).ColourValue()} to {property.Name.ColourName()}.");
	}

	private static void RoomRentShow(ICharacter actor, StringStack ss)
	{
		var property = FindRoomRentProperty(actor, ss,
			actor.Gameworld.Properties.Where(x => x.IsAuthorisedOwner(actor) || x.IsApprovedHotel),
			"Which hotel property do you want to show?",
			"There is no such hotel property that you can view.");
		if (property == null)
		{
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Hotel Setup for {property.Name.ColourName()}".GetLineWithTitleInner(actor, Telnet.FunctionYellow, Telnet.BoldWhite));
		sb.AppendLine($"License: {property.HotelLicenseStatus.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Bank: {property.HotelBankAccount?.AccountReference.ColourValue() ?? "None".ColourError()}");
		sb.AppendLine($"Can Rent Prog: {property.HotelCanRentProg?.MXPClickableFunctionNameWithId() ?? "None".ColourError()}");
		sb.AppendLine($"Lost Property Retention: {((TimeSpan)property.HotelLostPropertyRetention).Describe(actor).ColourValue()}");
		sb.AppendLine($"Outstanding Taxes: {property.EconomicZone.Currency.Describe(property.HotelOutstandingTaxes, CurrencyDescriptionPatternType.Short).ColourValue()}");
		sb.AppendLine();
		sb.AppendLine(StringUtilities.GetTextTable(
			property.HotelRooms.Select(room => new List<string>
			{
				room.Name,
				room.Cell.GetFriendlyReference(actor),
				room.Listed.ToColouredString(),
				property.EconomicZone.Currency.Describe(room.PricePerDay, CurrencyDescriptionPatternType.Short),
				property.EconomicZone.Currency.Describe(room.SecurityDeposit, CurrencyDescriptionPatternType.Short),
				$"{room.MinimumDuration.Describe(actor)} to {room.MaximumDuration.Describe(actor)}",
				HotelRoomAvailability(room)
			}),
			new List<string> { "Room", "Cell", "Listed", "Daily", "Deposit", "Duration", "Available" },
			actor.LineFormatLength,
			colour: Telnet.Green,
			unicodeTable: actor.Account.UseUnicode));
		actor.OutputHandler.Send(sb.ToString());
	}

	private static void RoomRentRequest(ICharacter actor, StringStack ss)
	{
		var property = FindOwnedHotelProperty(actor, ss, "request hotel approval for");
		if (property == null)
		{
			return;
		}

		if (property.HotelLicenseStatus == HotelLicenseStatus.Approved)
		{
			actor.OutputHandler.Send($"{property.Name.ColourName()} is already approved as a hotel.");
			return;
		}

		property.HotelLicenseStatus = HotelLicenseStatus.Requested;
		actor.OutputHandler.Send(
			$"You request hotel approval for {property.Name.ColourName()} from the {property.EconomicZone.Name.ColourName()} economic zone.");
	}

	private static void RoomRentSurrender(ICharacter actor, StringStack ss)
	{
		var property = FindOwnedHotelProperty(actor, ss, "surrender hotel approval for");
		if (property == null)
		{
			return;
		}

		if (property.HotelRooms.Any(x => x.ActiveRental is not null))
		{
			actor.OutputHandler.Send("You cannot surrender a hotel license while rooms are actively rented.");
			return;
		}

		property.HotelLicenseStatus = HotelLicenseStatus.None;
		actor.OutputHandler.Send($"{property.Name.ColourName()} is no longer licensed as a hotel.");
	}

	private static void RoomRentApprove(ICharacter actor, StringStack ss)
	{
		var property = FindRoomRentProperty(actor, ss,
			actor.Gameworld.Properties.Where(x => x.HotelLicenseStatus == HotelLicenseStatus.Requested),
			"Which requested hotel license do you want to approve?",
			"There is no such property with a requested hotel license.");
		if (property == null)
		{
			return;
		}

		if (!CanManageEconomicZone(actor, property.EconomicZone))
		{
			actor.OutputHandler.Send(
				$"You do not have permission to approve hotel licenses for the {property.EconomicZone.Name.ColourName()} economic zone.");
			return;
		}

		property.HotelLicenseStatus = HotelLicenseStatus.Approved;
		actor.OutputHandler.Send(
			$"You approve {property.Name.ColourName()} as a hotel in the {property.EconomicZone.Name.ColourName()} economic zone.");
	}

	private static void RoomRentBank(ICharacter actor, StringStack ss)
	{
		var property = FindOwnedHotelProperty(actor, ss, "set the hotel bank account for");
		if (property == null)
		{
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which bank account do you want this hotel to use?");
			return;
		}

		(IBankAccount account, string error) = Bank.FindBankAccount(ss.SafeRemainingArgument, null, actor);
		if (account == null)
		{
			actor.OutputHandler.Send(error);
			return;
		}

		if (!account.IsAuthorisedAccountUser(actor))
		{
			actor.OutputHandler.Send("You are not an authorised user for that bank account.");
			return;
		}

		property.HotelBankAccount = account;
		actor.OutputHandler.Send(
			$"You set {property.Name.ColourName()}'s hotel bank account to {account.AccountReference.ColourValue()}.");
	}

	private static void RoomRentProg(ICharacter actor, StringStack ss)
	{
		var property = FindOwnedHotelProperty(actor, ss, "set the hotel rental prog for");
		if (property == null)
		{
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should control who may rent rooms, or NONE to clear it?");
			return;
		}

		if (ss.SafeRemainingArgument.EqualToAny("none", "clear", "remove"))
		{
			property.HotelCanRentProg = null;
			actor.OutputHandler.Send($"{property.Name.ColourName()} no longer uses a prog to control room rentals.");
			return;
		}

		var prog = long.TryParse(ss.SafeRemainingArgument, out var value)
			? actor.Gameworld.FutureProgs.Get(value)
			: actor.Gameworld.FutureProgs.GetByName(ss.SafeRemainingArgument);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return;
		}

		if (!prog.ReturnType.CompatibleWith(ProgVariableTypes.Boolean))
		{
			actor.OutputHandler.Send(
				$"The prog must return boolean, while {prog.MXPClickableFunctionName()} returns {prog.ReturnType.Describe().ColourValue()}.");
			return;
		}

		if (!prog.MatchesParameters(Enumerable.Empty<ProgVariableTypes>()) &&
			!prog.MatchesParameters(new[] { ProgVariableTypes.Character }))
		{
			actor.OutputHandler.Send("The prog must accept either no parameters or a single character parameter.");
			return;
		}

		property.HotelCanRentProg = prog;
		actor.OutputHandler.Send(
			$"{property.Name.ColourName()} now uses {prog.MXPClickableFunctionNameWithId()} to control who may rent rooms.");
	}

	private static void RoomRentRetention(ICharacter actor, StringStack ss)
	{
		var property = FindOwnedHotelProperty(actor, ss, "set lost property retention for");
		if (property == null)
		{
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("How long should lost property be held before auction or liquidation?");
			return;
		}

		if (!TimeSpanParser.TryParse(ss.SafeRemainingArgument, Units.Days, Units.Hours, out var duration))
		{
			actor.OutputHandler.Send("That is not a valid time span.");
			return;
		}

		property.HotelLostPropertyRetention = duration;
		actor.OutputHandler.Send(
			$"{property.Name.ColourName()} will now hold lost property for {duration.Describe(actor).ColourValue()}.");
	}

	private static void RoomRentTaxes(ICharacter actor, StringStack ss)
	{
		var property = FindOwnedHotelProperty(actor, ss, "pay hotel taxes for");
		if (property == null)
		{
			return;
		}

		var taxes = property.HotelOutstandingTaxes;
		if (taxes <= 0.0M)
		{
			actor.OutputHandler.Send($"{property.Name.ColourName()} does not owe any hotel rental taxes.");
			return;
		}

		if (property.HotelBankAccount == null)
		{
			actor.OutputHandler.Send($"{property.Name.ColourName()} does not have a hotel bank account configured.");
			return;
		}

		(bool truth, string error) = property.HotelBankAccount.CanWithdraw(taxes, false);
		if (!truth)
		{
			actor.OutputHandler.Send(error);
			return;
		}

		property.HotelBankAccount.WithdrawFromTransaction(taxes, $"Hotel rental taxes for {property.Name}");
		property.HotelBankAccount.Bank.CurrencyReserves[property.EconomicZone.Currency] -= taxes;
		property.HotelBankAccount.Bank.Changed = true;
		property.EconomicZone.TotalRevenueHeld += taxes;
		property.HotelOutstandingTaxes = 0.0M;
		actor.OutputHandler.Send(
			$"You pay {property.EconomicZone.Currency.Describe(taxes, CurrencyDescriptionPatternType.Short).ColourValue()} in hotel rental taxes for {property.Name.ColourName()}.");
	}

	private static void RoomRentBan(ICharacter actor, StringStack ss, bool ban)
	{
		var property = FindOwnedHotelProperty(actor, ss, ban ? "ban a patron from" : "unban a patron from");
		if (property == null)
		{
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(ban ? "Who do you want to ban?" : "Who do you want to unban?");
			return;
		}

		var target = GetCharacterByIdOrName(actor, ss.SafeRemainingArgument);
		if (target == null)
		{
			actor.OutputHandler.Send("There is no such character.");
			return;
		}

		if (ban)
		{
			property.BanFromHotel(target);
			actor.OutputHandler.Send($"{target.PersonalName.GetName(NameStyle.FullName).ColourName()} is now banned from {property.Name.ColourName()}.");
			return;
		}

		property.UnbanFromHotel(target);
		actor.OutputHandler.Send($"{target.PersonalName.GetName(NameStyle.FullName).ColourName()} is no longer banned from {property.Name.ColourName()}.");
	}

	private static void RoomRentRoom(ICharacter actor, StringStack ss)
	{
		switch (ss.PopForSwitch())
		{
			case "list":
				RoomRentRoomList(actor, ss);
				return;
			case "add":
			case "create":
				RoomRentRoomAdd(actor, ss);
				return;
			case "remove":
			case "delete":
				RoomRentRoomRemove(actor, ss);
				return;
			case "price":
			case "rent":
				RoomRentRoomPrice(actor, ss);
				return;
			case "deposit":
				RoomRentRoomDeposit(actor, ss);
				return;
			case "length":
			case "duration":
				RoomRentRoomLength(actor, ss);
				return;
			case "name":
			case "rename":
				RoomRentRoomName(actor, ss);
				return;
			case "listed":
			case "listroom":
				RoomRentRoomListed(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(RoomRentHelp.SubstituteANSIColour());
				return;
		}
	}

	private static void RoomRentRoomList(ICharacter actor, StringStack ss)
	{
		var property = FindOwnedHotelProperty(actor, ss, "list rooms for");
		if (property == null)
		{
			return;
		}

		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			property.HotelRooms.Select(room => new List<string>
			{
				room.Name,
				room.Cell.GetFriendlyReference(actor),
				room.Listed.ToColouredString(),
				property.EconomicZone.Currency.Describe(room.PricePerDay, CurrencyDescriptionPatternType.Short),
				property.EconomicZone.Currency.Describe(room.SecurityDeposit, CurrencyDescriptionPatternType.Short),
				$"{room.MinimumDuration.Describe(actor)} to {room.MaximumDuration.Describe(actor)}",
				room.Keys.Select(x => x.Name).ListToString(),
				room.Furnishings.Count().ToString("N0", actor)
			}),
			new List<string> { "Room", "Cell", "Listed", "Daily", "Deposit", "Duration", "Keys", "Furnishings" },
			actor.LineFormatLength,
			colour: Telnet.Green,
			unicodeTable: actor.Account.UseUnicode));
	}

	private static void RoomRentRoomAdd(ICharacter actor, StringStack ss)
	{
		var property = FindOwnedHotelProperty(actor, ss, "add a hotel room to");
		if (property == null)
		{
			return;
		}

		if (!property.PropertyLocations.Contains(actor.Location))
		{
			actor.OutputHandler.Send($"You must be standing in a room in the {property.Name.ColourName()} property.");
			return;
		}

		if (property.HotelRoomForCell(actor.Location) != null)
		{
			actor.OutputHandler.Send("This room is already configured as a hotel room.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What should the daily room price be?");
			return;
		}

		if (!property.EconomicZone.Currency.TryGetBaseCurrency(ss.PopSpeech(), out var price))
		{
			actor.OutputHandler.Send($"That is not a valid amount of {property.EconomicZone.Currency.Name.ColourName()}.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What security deposit should this room require?");
			return;
		}

		if (!property.EconomicZone.Currency.TryGetBaseCurrency(ss.PopSpeech(), out var deposit))
		{
			actor.OutputHandler.Send($"That is not a valid amount of {property.EconomicZone.Currency.Name.ColourName()}.");
			return;
		}

		if (ss.IsFinished || !TimeSpanParser.TryParse(ss.PopSpeech(), Units.Days, Units.Hours, out var minimum))
		{
			actor.OutputHandler.Send("What is the minimum rental duration?");
			return;
		}

		if (ss.IsFinished || !TimeSpanParser.TryParse(ss.PopSpeech(), Units.Days, Units.Hours, out var maximum))
		{
			actor.OutputHandler.Send("What is the maximum rental duration?");
			return;
		}

		if (minimum <= TimeSpan.Zero || maximum < minimum)
		{
			actor.OutputHandler.Send("The minimum duration must be positive and the maximum must be at least the minimum.");
			return;
		}

		var name = ss.IsFinished ? actor.Location.GetFriendlyReference(actor) : ss.SafeRemainingArgument;
		property.AddHotelRoom(actor.Location, name, price, deposit, minimum, maximum);
		actor.OutputHandler.Send($"{name.ColourName()} is now a hotel room for {property.Name.ColourName()}.");
	}

	private static void RoomRentRoomRemove(ICharacter actor, StringStack ss)
	{
		var property = FindOwnedHotelProperty(actor, ss, "remove a hotel room from");
		if (property == null)
		{
			return;
		}

		var room = FindHotelRoom(actor, property, ss, "remove");
		if (room == null)
		{
			return;
		}

		if (room.ActiveRental is not null)
		{
			actor.OutputHandler.Send("You cannot remove a hotel room while it is actively rented.");
			return;
		}

		property.RemoveHotelRoom(room);
		actor.OutputHandler.Send($"{room.Name.ColourName()} is no longer a hotel room for {property.Name.ColourName()}.");
	}

	private static void RoomRentRoomPrice(ICharacter actor, StringStack ss)
	{
		var property = FindOwnedHotelProperty(actor, ss, "change a hotel room price for");
		if (property == null)
		{
			return;
		}

		var room = FindHotelRoom(actor, property, ss, "set a price for");
		if (room == null)
		{
			return;
		}

		if (ss.IsFinished || !property.EconomicZone.Currency.TryGetBaseCurrency(ss.SafeRemainingArgument, out var amount))
		{
			actor.OutputHandler.Send($"What should the daily price be, in {property.EconomicZone.Currency.Name.ColourName()}?");
			return;
		}

		room.PricePerDay = amount;
		actor.OutputHandler.Send(
			$"{room.Name.ColourName()} now rents for {property.EconomicZone.Currency.Describe(amount, CurrencyDescriptionPatternType.Short).ColourValue()} per day.");
	}

	private static void RoomRentRoomDeposit(ICharacter actor, StringStack ss)
	{
		var property = FindOwnedHotelProperty(actor, ss, "change a hotel room deposit for");
		if (property == null)
		{
			return;
		}

		var room = FindHotelRoom(actor, property, ss, "set a deposit for");
		if (room == null)
		{
			return;
		}

		if (ss.IsFinished || !property.EconomicZone.Currency.TryGetBaseCurrency(ss.SafeRemainingArgument, out var amount))
		{
			actor.OutputHandler.Send($"What should the security deposit be, in {property.EconomicZone.Currency.Name.ColourName()}?");
			return;
		}

		room.SecurityDeposit = amount;
		actor.OutputHandler.Send(
			$"{room.Name.ColourName()} now requires a {property.EconomicZone.Currency.Describe(amount, CurrencyDescriptionPatternType.Short).ColourValue()} security deposit.");
	}

	private static void RoomRentRoomLength(ICharacter actor, StringStack ss)
	{
		var property = FindOwnedHotelProperty(actor, ss, "change hotel room rental lengths for");
		if (property == null)
		{
			return;
		}

		var room = FindHotelRoom(actor, property, ss, "set rental lengths for");
		if (room == null)
		{
			return;
		}

		if (ss.IsFinished || !TimeSpanParser.TryParse(ss.PopSpeech(), Units.Days, Units.Hours, out var minimum))
		{
			actor.OutputHandler.Send("What is the minimum rental duration?");
			return;
		}

		if (ss.IsFinished || !TimeSpanParser.TryParse(ss.PopSpeech(), Units.Days, Units.Hours, out var maximum))
		{
			actor.OutputHandler.Send("What is the maximum rental duration?");
			return;
		}

		if (minimum <= TimeSpan.Zero || maximum < minimum)
		{
			actor.OutputHandler.Send("The minimum duration must be positive and the maximum must be at least the minimum.");
			return;
		}

		room.MinimumDuration = minimum;
		room.MaximumDuration = maximum;
		actor.OutputHandler.Send(
			$"{room.Name.ColourName()} may now be rented for between {minimum.Describe(actor).ColourValue()} and {maximum.Describe(actor).ColourValue()}.");
	}

	private static void RoomRentRoomName(ICharacter actor, StringStack ss)
	{
		var property = FindOwnedHotelProperty(actor, ss, "rename a hotel room for");
		if (property == null)
		{
			return;
		}

		var room = FindHotelRoom(actor, property, ss, "rename");
		if (room == null)
		{
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What new name should this room have?");
			return;
		}

		var name = ss.SafeRemainingArgument;
		room.Name = name;
		actor.OutputHandler.Send($"That hotel room is now named {name.ColourName()}.");
	}

	private static void RoomRentRoomListed(ICharacter actor, StringStack ss)
	{
		var property = FindOwnedHotelProperty(actor, ss, "toggle listing for");
		if (property == null)
		{
			return;
		}

		var room = FindHotelRoom(actor, property, ss, "toggle listing for");
		if (room == null)
		{
			return;
		}

		room.Listed = !room.Listed;
		actor.OutputHandler.Send($"{room.Name.ColourName()} is {(room.Listed ? "now" : "no longer")} listed for hotel rental.");
	}

	private static void RoomRentKey(ICharacter actor, StringStack ss)
	{
		bool? add = ss.PopForSwitch() switch
		{
			"add" => true,
			"remove" or "rem" or "delete" => false,
			_ => null
		};
		if (add == null)
		{
			actor.OutputHandler.Send(RoomRentHelp.SubstituteANSIColour());
			return;
		}

		var property = FindOwnedHotelProperty(actor, ss, add.Value ? "add a room key for" : "remove a room key from");
		if (property == null)
		{
			return;
		}

		var room = FindHotelRoom(actor, property, ss, add.Value ? "add a key to" : "remove a key from");
		if (room == null)
		{
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(add.Value ? "Which property key do you want to assign?" : "Which room key do you want to remove?");
			return;
		}

		var key = property.PropertyKeys.GetByNameOrAbbreviation(ss.SafeRemainingArgument);
		if (key == null)
		{
			actor.OutputHandler.Send($"That is not a key for {property.Name.ColourName()}.");
			return;
		}

		if (add.Value)
		{
			room.AddKey(key);
			actor.OutputHandler.Send($"{key.Name.ColourName()} is now a key for {room.Name.ColourName()}.");
			return;
		}

		room.RemoveKey(key);
		actor.OutputHandler.Send($"{key.Name.ColourName()} is no longer a key for {room.Name.ColourName()}.");
	}

	private static void RoomRentFurnish(ICharacter actor, StringStack ss)
	{
		switch (ss.PopForSwitch())
		{
			case "list":
				RoomRentFurnishList(actor, ss);
				return;
			case "add":
				RoomRentFurnishAdd(actor, ss);
				return;
			case "remove":
			case "rem":
			case "delete":
				RoomRentFurnishRemove(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(RoomRentHelp.SubstituteANSIColour());
				return;
		}
	}

	private static void RoomRentFurnishList(ICharacter actor, StringStack ss)
	{
		var property = FindOwnedHotelProperty(actor, ss, "list furnishings for");
		if (property == null)
		{
			return;
		}

		var room = FindHotelRoom(actor, property, ss, "list furnishings for");
		if (room == null)
		{
			return;
		}

		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			room.Furnishings.Select(furnishing => new List<string>
			{
				furnishing.Description,
				furnishing.GameItem?.HowSeen(actor) ?? "Missing",
				property.EconomicZone.Currency.Describe(furnishing.ReplacementValue, CurrencyDescriptionPatternType.Short),
				property.EconomicZone.Currency.Describe(furnishing.CurrentDepositClaim(room), CurrencyDescriptionPatternType.Short)
			}),
			new List<string> { "Description", "Item", "Replacement", "Current Claim" },
			actor.LineFormatLength,
			colour: Telnet.Green,
			unicodeTable: actor.Account.UseUnicode));
	}

	private static void RoomRentFurnishAdd(ICharacter actor, StringStack ss)
	{
		var property = FindOwnedHotelProperty(actor, ss, "add a furnishing to");
		if (property == null)
		{
			return;
		}

		var room = FindHotelRoom(actor, property, ss, "add a furnishing to");
		if (room == null)
		{
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which item do you want to mark as a furnishing?");
			return;
		}

		var itemText = ss.PopSpeech();
		var item = actor.TargetItem(itemText);
		if (item == null)
		{
			actor.OutputHandler.Send("You cannot see any such item.");
			return;
		}

		if (!item.TrueLocations.Contains(room.Cell))
		{
			actor.OutputHandler.Send($"That item is not in {room.Name.ColourName()}.");
			return;
		}

		var value = item.Prototype.CostInBaseCurrency > 0.0M
			? item.Prototype.CostInBaseCurrency / property.EconomicZone.Currency.BaseCurrencyToGlobalBaseCurrencyConversion
			: 0.0M;
		if (!ss.IsFinished && !property.EconomicZone.Currency.TryGetBaseCurrency(ss.SafeRemainingArgument, out value))
		{
			actor.OutputHandler.Send($"That is not a valid amount of {property.EconomicZone.Currency.Name.ColourName()}.");
			return;
		}

		room.AddFurnishing(item, value);
		actor.OutputHandler.Send(
			$"{item.HowSeen(actor, true)} is now a furnishing for {room.Name.ColourName()} with a replacement value of {property.EconomicZone.Currency.Describe(value, CurrencyDescriptionPatternType.Short).ColourValue()}.");
	}

	private static void RoomRentFurnishRemove(ICharacter actor, StringStack ss)
	{
		var property = FindOwnedHotelProperty(actor, ss, "remove a furnishing from");
		if (property == null)
		{
			return;
		}

		var room = FindHotelRoom(actor, property, ss, "remove a furnishing from");
		if (room == null)
		{
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which furnishing do you want to remove?");
			return;
		}

		var text = ss.SafeRemainingArgument;
		var furnishing = room.Furnishings.FirstOrDefault(x => x.GameItem?.HowSeen(actor).EqualTo(text) == true) ??
						 room.Furnishings.FirstOrDefault(x => x.Description.EqualTo(text)) ??
						 room.Furnishings.FirstOrDefault(x => x.Description.StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
		if (furnishing == null)
		{
			actor.OutputHandler.Send("That is not a furnishing for this room.");
			return;
		}

		room.RemoveFurnishing(furnishing);
		actor.OutputHandler.Send($"{furnishing.Description.ColourName()} is no longer a furnishing for {room.Name.ColourName()}.");
	}

	private static void RoomRentLost(ICharacter actor, StringStack ss)
	{
		switch (ss.PopForSwitch())
		{
			case "list":
				RoomRentLostList(actor, ss);
				return;
			case "extend":
				RoomRentLostExtend(actor, ss);
				return;
			case "release":
			case "take":
				RoomRentLostRelease(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(RoomRentHelp.SubstituteANSIColour());
				return;
		}
	}

	private static void RoomRentLostList(ICharacter actor, StringStack ss)
	{
		var property = FindOwnedHotelProperty(actor, ss, "list lost property for");
		if (property == null)
		{
			return;
		}

		var lost = property.HotelLostProperties.ToList();
		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			lost.Select((item, index) => new List<string>
			{
				(index + 1).ToString("N0", actor),
				item.Room.Name,
				item.Owner?.PersonalName.GetName(NameStyle.FullName) ?? $"#{item.OwnerId.ToString("N0", actor)}",
				item.Description,
				item.Status.DescribeEnum(),
				item.StoredUntil.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short),
				property.EconomicZone.Currency.Describe(item.ReservePrice, CurrencyDescriptionPatternType.Short)
			}),
			new List<string> { "#", "Room", "Owner", "Bundle", "Status", "Held Until", "Reserve" },
			actor.LineFormatLength,
			colour: Telnet.Green,
			unicodeTable: actor.Account.UseUnicode));
	}

	private static IHotelLostProperty FindLostProperty(ICharacter actor, IProperty property, StringStack ss)
	{
		if (ss.IsFinished || !int.TryParse(ss.PopSpeech(), out var value) || value <= 0)
		{
			actor.OutputHandler.Send("Which lost property number do you mean?");
			return null;
		}

		var lost = property.HotelLostProperties.ElementAtOrDefault(value - 1);
		if (lost == null)
		{
			actor.OutputHandler.Send("There is no such lost property record.");
			return null;
		}

		return lost;
	}

	private static void RoomRentLostExtend(ICharacter actor, StringStack ss)
	{
		var property = FindOwnedHotelProperty(actor, ss, "extend lost property for");
		if (property == null)
		{
			return;
		}

		var lost = FindLostProperty(actor, property, ss);
		if (lost == null)
		{
			return;
		}

		if (lost.Status != HotelLostPropertyStatus.Held)
		{
			actor.OutputHandler.Send("You can only extend lost property that is still being held.");
			return;
		}

		if (ss.IsFinished || !TimeSpanParser.TryParse(ss.SafeRemainingArgument, Units.Days, Units.Hours, out var duration))
		{
			actor.OutputHandler.Send("How much longer should this lost property be held?");
			return;
		}

		lost.StoredUntil += duration;
		actor.OutputHandler.Send(
			$"{lost.Description.ColourName()} will now be held until {lost.StoredUntil.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()}.");
	}

	private static void RoomRentLostRelease(ICharacter actor, StringStack ss)
	{
		var property = FindOwnedHotelProperty(actor, ss, "release lost property for");
		if (property == null)
		{
			return;
		}

		var lost = FindLostProperty(actor, property, ss);
		if (lost == null)
		{
			return;
		}

		if (lost.Status != HotelLostPropertyStatus.Held)
		{
			actor.OutputHandler.Send("You can only release lost property that is still being held.");
			return;
		}

		lost.Bundle.Login();
		property.ClaimHotelLostProperty(lost);
		GiveItemToActor(actor, lost.Bundle, $"You couldn't hold {lost.Bundle.HowSeen(actor)}, so it is on the ground.");
		actor.OutputHandler.Send($"You remove {lost.Bundle.HowSeen(actor)} from {property.Name.ColourName()}'s lost property.");
	}
}
