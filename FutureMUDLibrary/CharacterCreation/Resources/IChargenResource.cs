using System;
using MudSharp.Models;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.CharacterCreation.Resources {
    public interface IChargenResource : IHaveMultipleNames {
        /// <summary>
        ///     Pluralised version of Item.Name
        /// </summary>
        string PluralName { get; }

        /// <summary>
        ///     A short alias for this type, e.g. RPP
        /// </summary>
        string Alias { get; }

        /// <summary>
        ///     Represents the minimum amount of time passed between manual awards of this resource able to be done without having
        ///     the permission to circumvent this
        /// </summary>
        TimeSpan MinimumTimeBetweenAwards { get; }

        /// <summary>
        ///     The maximum number of this resource awarded each time the award command is used
        /// </summary>
        double MaximumNumberAwardedPerAward { get; }

        /// <summary>
        ///     Permission level required to be able to award this resource
        /// </summary>
        PermissionLevel PermissionLevelRequiredToAward { get; }

        /// <summary>
        ///     Permission level required to circumvent the MinimumTimeBetweenAwards property restriction
        /// </summary>
        PermissionLevel PermissionLevelRequiredToCircumventMinimumTime { get; }

        /// <summary>
        ///     Controls whether this ChargenResource total should be displayed in the SCORE command
        /// </summary>
        bool ShowToPlayerInScore { get; }

        /// <summary>
        ///     If not empty, contains text to send as an echo to the player when awarded.
        /// </summary>
        string TextDisplayedToPlayerOnAward { get; }

        /// <summary>
        ///     If not empty, contains text to send as an echo to the player when deducted.
        /// </summary>
        string TextDisplayedToPlayerOnDeduct { get; }

        /// <summary>
        ///     Does any save-time action required by this resource, e.g. regenerating over time
        /// </summary>
        /// <param name="character">The character to test</param>
        /// <param name="oldMinutes"></param>
        /// <param name="newMinutes"></param>
        void UpdateOnSave(ICharacter character, int oldMinutes, int newMinutes);

        /// <summary>
        ///     Returns the maximum amount of this resource a character may have. A result of -1 indicates there is no maximum.
        /// </summary>
        /// <param name="account">The account to test</param>
        /// <returns>The maximum amount of this resource the character can have, or -1 if there is no maximum.</returns>
        int GetMaximum(IAccount account);

        void PerformPostLoadUpdate(ChargenResource resource, IFuturemud gameworld);
        bool DisplayChangesOnLogin { get; }
    }
}