#nullable enable
using MudSharp.Community;

namespace MudSharp.Economy.Lending;

internal static partial class LoanService
{
	// A sale retires the property-owner row. Its proceeds belong to the seller's enduring identity.
	internal static string PropertyProceeds(ICharacter actor, StringStack command) => Settle(actor, () =>
	{
		var zone = actor.Gameworld.EconomicZones.FirstOrDefault(x => x.ConveyancingRooms.Contains(actor.Location));
		if (zone is null) return "Visit the conveyancer for the sale currency.";
		IFrameworkItem payee = actor;
		if (command.PeekSpeech().EqualTo("clan"))
		{
			command.PopSpeech();
			var name = command.PopSpeech();
			var membership = actor.ClanMemberships.FirstOrDefault(x =>
				x.NetPrivileges.HasFlag(ClanPrivilegeType.CanManageBankAccounts) &&
				(x.Clan.Name.EqualTo(name) || x.Clan.Id.ToString() == name));
			if (membership is null) return "Select a clan whose finances you manage.";
			payee = membership.Clan;
		}
		var balance = VirtualCashLedger.Balance(payee, zone.Currency);
		if (command.IsFinished) return $"Your available proceeds are {Money(zone.Currency, balance)}. Use PROPERTY PROCEEDS [CLAN <name>] WITHDRAW <amount>.";
		if (!command.PopSpeech().EqualTo("withdraw") || !zone.Currency.TryGetBaseCurrency(command.SafeRemainingArgument, out var amount) ||
		    amount <= 0 || amount > balance) return $"Enter a positive withdrawal up to {Money(zone.Currency, balance)}.";
		var coins = zone.Currency.FindCoinsForAmount(amount, out var exact);
		if (!exact) return "Choose an amount that can be paid in exact coins.";
		var reason = $"Proceeds withdrawn at the conveyancer {Guid.NewGuid():N}";
		_settlement!.ProceedsReceipt = reason;
		if (!VirtualCashLedger.Debit(payee, zone.Currency, amount, actor, actor, "PropertyProceeds",
			    reason, null, zone.FinancialPeriodReferenceCalendar.CurrentDateTime, out var error))
			return error;
		GiveCash(actor, zone.Currency, coins);
		return $"You collect {Money(zone.Currency, amount)}.";
	}, guard: true);
}
