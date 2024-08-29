using System;
using MudSharp.Accounts;
using MudSharp.Character;

namespace MudSharp.Framework.Revision {
    public enum RevisionStatus {
        /// <summary>
        ///     This item has been Revised, and is now superceded
        /// </summary>
        Revised = 0,

        /// <summary>
        ///     This item is a Pending Revision of another item, and is not yet Revised
        /// </summary>
        PendingRevision = 1,

        /// <summary>
        ///     This item is still being designed, and has not yet been submitted for review
        /// </summary>
        UnderDesign = 2,

        /// <summary>
        ///     This item is a Rejected former Pending Revision
        /// </summary>
        Rejected = 3,

        /// <summary>
        ///     This is the Current revision of this item
        /// </summary>
        Current = 4,

        /// <summary>
        ///     The item has been superceded and should no longer be used
        /// </summary>
        Obsolete = 5
    }

    public interface IEditableItem : IHaveFuturemud, IFrameworkItem
    {
        /// <summary>
        /// Executes a building command based on player input
        /// </summary>
        /// <param name="actor">The avatar of the player doing the command</param>
        /// <param name="command">The command they wish to execute</param>
        /// <returns>Returns true if the command was valid and anything was changed. If nothing was changed or the command was invalid, it returns false</returns>
        bool BuildingCommand(ICharacter actor, StringStack command);

        /// <summary>
        /// Shows a builder-specific output representing the IEditableItem
        /// </summary>
        /// <param name="actor">The avatar of the player who wants to view the IEditableItem</param>
        /// <returns>A string representing the item textually</returns>
        string Show(ICharacter actor);
    }


    public interface IEditableRevisableItem : IEditableItem, IRevisableItem, IHaveFuturemud, IKeywordedItem {
        /// <summary>
        ///     The ID of the Account who created this Revision
        /// </summary>
        long BuilderAccountID { get; }

        /// <summary>
        ///     The ID of the Account who reviewed this Revision
        /// </summary>
        long? ReviewerAccountID { get; }

        /// <summary>
        ///     A comment left by the creator of this Revision
        /// </summary>
        string BuilderComment { get; }

        /// <summary>
        ///     A comment left by the reviewer of this Revision
        /// </summary>
        string ReviewerComment { get; }

        /// <summary>
        ///     The date that this Revision was submitted for review
        /// </summary>
        DateTime BuilderDate { get; }

        /// <summary>
        ///     The date that this Revision was given its review
        /// </summary>
        DateTime? ReviewerDate { get; }

        /// <summary>
        ///     The date that this Revision became obsolete
        /// </summary>
        DateTime? ObsoleteDate { get; }

        bool ChangeStatus(RevisionStatus status, string comment, IAccount reviewer);

        string EditHeader();

        IEditableRevisableItem CreateNewRevision(ICharacter initiator);

        bool CanSubmit();
        string WhyCannotSubmit();

        /// <summary>
        /// A ReadOnly IEditableRevisableItem should not be allowed to have new revisions made to it
        /// </summary>
        bool ReadOnly { get; set; }

        bool IsAssociatedBuilder(ICharacter character);
    }

    public static class IEditableItemExtensions {
        public static string Describe(this RevisionStatus status) {
            switch (status) {
                case RevisionStatus.Current:
                    return "Current";
                case RevisionStatus.PendingRevision:
                    return "Pending Review";
                case RevisionStatus.Rejected:
                    return "Rejected";
                case RevisionStatus.Revised:
                    return "Revised";
                case RevisionStatus.UnderDesign:
                    return "Under Design";
                case RevisionStatus.Obsolete:
                    return "Obsolete";
                default:
                    return "Unknown Status";
            }
        }

        public static string DescribeColour(this RevisionStatus status) {
            switch (status) {
                case RevisionStatus.Current:
                    return "Current".Colour(Telnet.Green);
                case RevisionStatus.PendingRevision:
                    return "Pending Review".Colour(Telnet.BoldYellow);
                case RevisionStatus.Rejected:
                    return "Rejected".Colour(Telnet.Red);
                case RevisionStatus.Revised:
                    return "Revised".Colour(Telnet.Blue);
                case RevisionStatus.UnderDesign:
                    return "Under Design".Colour(Telnet.Yellow);
                case RevisionStatus.Obsolete:
                    return "Obsolete".Colour(Telnet.Blue);
                default:
                    return "Unknown Status";
            }
        }
    }
}