#nullable enable
using MudSharp.Arenas;
using MudSharp.Character;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Payment;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;

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

        ICurrency currency = arenaEvent.Arena.Currency;
        OtherCashPayment payment = new(currency, bettor);
        decimal available = payment.AccessibleMoneyForPayment();
        if (available < amount)
        {
            string requiredText = currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue();
            string availableText = currency.Describe(available, CurrencyDescriptionPatternType.ShortDecimal).ColourValue();
            return (false, $"You need {requiredText} available to stake but only have {availableText} on hand.");
        }

        payment.TakePayment(amount);
        return (true, string.Empty);
    }

    public bool TryDisburse(ICharacter bettor, IArenaEvent arenaEvent, decimal amount)
    {
        if (arenaEvent is null)
        {
            return false;
        }

        return TryDisburse(bettor, arenaEvent.Arena, amount);
    }

    public bool TryDisburse(ICharacter bettor, ICombatArena arena, decimal amount)
    {
        if (bettor is null || arena is null || amount <= 0)
        {
            return false;
        }

        ICurrency currency = arena.Currency;
        Dictionary<ICoin, int> coins = currency.FindCoinsForAmount(amount, out _);
        if (!coins.Any())
        {
            return false;
        }

        IGameItem pile = CurrencyGameItemComponentProto.CreateNewCurrencyPile(currency,
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
