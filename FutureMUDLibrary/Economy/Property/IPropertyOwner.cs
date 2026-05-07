using MudSharp.Character;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.TimeAndDate;

#nullable enable

namespace MudSharp.Economy.Property
{
    public interface IPropertyOwner
    {
        long OwnerId { get; }
        string OwnerFrameworkItemType { get; }
        IFrameworkItem Owner { get; }
        decimal ShareOfOwnership { get; set; }
        IBankAccount RevenueAccount { get; set; }
        decimal RevenueCashBalance(ICurrency currency);
        void CreditRevenue(ICurrency currency, decimal amount, ICharacter? actor, IFrameworkItem? counterparty,
            string reason, MudDateTime? mudDateTime = null);
        string Describe(IPerceiver voyeur);
        void Delete();
    }
}
