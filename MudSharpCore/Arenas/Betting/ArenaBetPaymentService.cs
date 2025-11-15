#nullable enable
using System;
using System.Linq;
using MudSharp.Arenas;
using MudSharp.Character;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Payment;
using MudSharp.Framework;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.Arenas;

/// <summary>
/// Default implementation that moves physical currency between bettors and arenas.
/// </summary>
public class ArenaBetPaymentService : IArenaBetPaymentService
{
	public (bool Success, string Error) CollectStake(ICharacter bettor, IArenaEvent arenaEvent, decimal amount)
	{
		if (bettor is null)
		{
			return (false, "You must be present to place that bet.");
		}

		if (amount <= 0)
		{
			return (true, string.Empty);
		}

		var currency = arenaEvent.Arena.Currency;
		var payment = new OtherCashPayment(currency, bettor);
		var available = payment.AccessibleMoneyForPayment();
		if (available < amount)
		{
			var requiredText = currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue();
			var availableText = currency.Describe(available, CurrencyDescriptionPatternType.ShortDecimal).ColourValue();
			return (false, $"You need {requiredText} available to stake but only have {availableText} on hand.");
		}

		payment.TakePayment(amount);
		return (true, string.Empty);
	}

	public bool TryDisburse(ICharacter bettor, IArenaEvent arenaEvent, decimal amount)
	{
		if (bettor is null || amount <= 0)
		{
			return false;
		}

		var currency = arenaEvent.Arena.Currency;
		var coins = currency.FindCoinsForAmount(amount, out _);
		if (!coins.Any())
		{
			return false;
		}

		var pile = CurrencyGameItemComponentProto.CreateNewCurrencyPile(currency,
			coins.Select(x => Tuple.Create(x.Key, x.Value)));
		if (bettor.Body.CanGet(pile, 0))
		{
			bettor.Body.Get(pile, silent: true);
		}
		else
		{
			pile.RoomLayer = bettor.RoomLayer;
			bettor.Location.Insert(pile, true);
			bettor.OutputHandler.Send("You couldn't hold your payout, so it has been placed at your feet.".Colour(Telnet.Yellow));
		}

		return true;
	}
}
