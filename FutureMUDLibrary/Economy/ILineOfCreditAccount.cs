using MudSharp.Character;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character.Name;
using MudSharp.Economy.Currency;

namespace MudSharp.Economy
{
    public enum LineOfCreditAuthorisationFailureReason
    {
        None,
        NotAuthorisedAccountUser,
        AccountOverbalanced,
        UserOverbalanced,
        AccountSuspended,
    }

    public class LineOfCreditAccountUser : IHavePersonalName
    {
        public long Id { get; set; }
        public IPersonalName PersonalName { get; set; }
        public decimal? SpendingLimit { get; set; }
    }

    public interface ILineOfCreditAccount : IFrameworkItem
    {
        ICurrency Currency { get; }
        string AccountName { get; }
        LineOfCreditAuthorisationFailureReason IsAuthorisedToUse(ICharacter actor, decimal amount);

        decimal MaximumAuthorisedToUse(ICharacter actor);
        long AccountOwnerId { get; }
        bool IsAccountOwner(ICharacter actor);
        void SetAccountOwner(ICharacter actor);
        IPersonalName AccountOwnerName { get; }
        void AddAuthorisation(ICharacter actor, decimal? spendingLimit);
        void RemoveAuthorisation(LineOfCreditAccountUser actor);
        void SetLimit(LineOfCreditAccountUser user, decimal? spendingLimit);
        void SetAccountLimit(decimal spendingLimit);
        void ChargeAccount(decimal amount);
        void PayoffAccount(decimal amount);
        decimal AccountLimit { get; }
        decimal OutstandingBalance { get; }
        IEnumerable<LineOfCreditAccountUser> AccountUsers { get; }
        bool IsSuspended { get; set; }
        string Show(ICharacter actor);
    }
}
