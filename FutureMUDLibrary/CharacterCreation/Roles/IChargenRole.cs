using System.Collections.Generic;
using MudSharp.Accounts;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.RPG.Merits;

namespace MudSharp.CharacterCreation.Roles {
    public interface IChargenRole : IFrameworkItem, ISaveable, IFutureProgVariable {
        /// <summary>
        ///     An enumerable representing the type of role in question
        /// </summary>
        ChargenRoleType RoleType { get; set; }

        /// <summary>
        ///     The account name of the person who posted this role
        /// </summary>
        string Poster { get; }

        /// <summary>
        ///     A List of account names of people who must approve any applications with this role
        /// </summary>
        List<string> RequiredApprovers { get; }

        /// <summary>
        ///     Maximum number of holders of this role who are currently alive
        /// </summary>
        int MaximumNumberAlive { get; set; }

        /// <summary>
        ///     Maximum number of holders of this role who, including dead people
        /// </summary>
        int MaximumNumberTotal { get; set; }

        /// <summary>
        ///     A summary of the role shown during character creation
        /// </summary>
        string ChargenBlurb { get; set; }

        /// <summary>
        ///     A prog returning bool that accepts a chargen as an input, determining whether this role should be shown
        /// </summary>
        IFutureProg AvailabilityProg { get; set; }

        bool ChargenAvailable(ICharacterTemplate template);

        /// <summary>
        ///     The costs per-resource for selecting this role
        /// </summary>
        Dictionary<IChargenResource, int> Costs { get; }

        /// <summary>
        /// The requirements to have a certain amount of resource (but not spend it) for this role.
        /// </summary>
        Dictionary<IChargenResource, int> Requirements { get; }

        Dictionary<ITraitDefinition, (double amount, bool giveIfMissing)> TraitAdjustments { get; }

        Dictionary<ICurrency, decimal> StartingCurrency { get; }

        IEnumerable<IMerit> AdditionalMerits { get; }

        List<IRoleClanMembership> ClanMemberships { get; }

        bool Expired { get; set; }

        PermissionLevel MinimumPermissionToApprove { get; set; }

        PermissionLevel MinimumPermissionToView { get; set; }

        int ResourceCost(IChargenResource resource);

        string Show(ICharacter actor);

        void SetName(string name);
        IEnumerable<IChargenAdvice> ChargenAdvices { get; }
        bool ToggleAdvice(IChargenAdvice advice);
    }
}