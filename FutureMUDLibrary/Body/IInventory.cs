using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Economy.Currency;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.PerceptionEngine;

namespace MudSharp.Body {
    public enum InventoryState {
        Held,
        Wielded,
        Worn,
        Dropped,
        Sheathed,
        InContainer,
        Prosthetic,
        Implanted,
        Attached,
        Consumed,
        ConsumedLiquid
    }

    public delegate void InventoryChangeEvent(InventoryState oldState, InventoryState newState, IGameItem item);

    public interface IInventory : IPerceiver {

        /// <summary>
        /// All items contained within the body, including everything from ExternalItems as well as internal implants. Basically, anything that would be left if the body itself was discombobulated and only the other items were left behind.
        /// </summary>
        IEnumerable<IGameItem> AllItems { get; }

        /// <summary>
        /// All items present that are external, and therefore targetable
        /// </summary>
        IEnumerable<IGameItem> ExternalItems { get; }

        /// <summary>
        /// Items that would appear in ExternalItems but also are exposed enough to be targeted by other actors
        /// </summary>
        IEnumerable<IGameItem> ExternalItemsForOtherActors { get; }

        /// <summary>
        /// Exposed items are those that are Outwear or otherwise not covered by anything
        /// </summary>
        IEnumerable<IGameItem> ExposedItems { get; }

        /// <summary>
        /// Outwear is the outermost layer of clothing
        /// </summary>
        IEnumerable<IGameItem> Outerwear { get; }
        /// <summary>
        /// All items that are directly worn, held or wielded by the IInventory
        /// </summary>
        IEnumerable<IGameItem> DirectItems { get; }

        /// <summary>
        /// Items that count towards carried weight
        /// </summary>
        IEnumerable<IGameItem> CarriedItems { get; }

        /// <summary>
        /// Bodyparts that have no items covering them
        /// </summary>
        IEnumerable<IBodypart> ExposedBodyparts { get; }

        /// <summary>
        /// Bodyparts who do not hide the fact that a bodyp
        /// </summary>
        IEnumerable<IBodypart> VisiblySeveredBodyparts { get; }

        /// <summary>
        /// Items that are the first-worn item for any of their slots
        /// </summary>
        IEnumerable<IGameItem> ItemsWornAgainstSkin { get; }

        /// <summary>
        ///     The singular form of the noun describing the appendages used to wield items for this body. Usually "hand". Mostly
        ///     used in displaying appropriate error messages to players.
        /// </summary>
        string WielderDescriptionSingular { get; }

        /// <summary>
        ///     The plural form of the noun describing the appendages used to wield items for this body. Usually "hands". Mostly
        ///     used in displaying appropriate error messages to players.
        /// </summary>
        string WielderDescriptionPlural { get; }

        event InventoryChangeEvent OnInventoryChange;

        IBodypart BodypartLocationOfInventoryItem(IGameItem item);

        /// <summary>
        /// Returns all items worn, held, wielded, implanted or used as a prosthetic at or downstream from the nominated bodypart
        /// </summary>
        /// <param name="part">The bodypart in question</param>
        /// <returns>A collection of items downstream from or at the part</returns>
        IEnumerable<IGameItem> AllItemsAtOrDownstreamOfPart(IBodypart part);

        CanUseBodypartResult CanUseBodypart(IBodypart part);
        CanUseLimbResult CanUseLimb(ILimb limb);

        void UpdateDescriptionWielded(IGameItem holdable);
        void UpdateDescriptionWorn(IGameItem holdable);
        void UpdateDescriptionHeld(IGameItem holdable);

        Alignment AlignmentOf(IGameItem target);
        Orientation OrientationOf(IGameItem target);

        /// <summary>
        ///     Swaps an existing item in inventory with a new one. Calling function is responsible for the fate of existingItem
        ///     once successfully swapped. Swap still must be valid.
        /// </summary>
        /// <param name="existingItem">An existing item in inventory</param>
        /// <param name="newItem">A new item not in inventory</param>
        /// <returns>True if a swap took place</returns>
        bool SwapInPlace(IGameItem existingItem, IGameItem newItem);

        void RecalculateItemHelpers();

        #region Worn Items

        /// <summary>
        ///     An IEnumerable containing all of the items currently worn, on the IInventory
        /// </summary>
        IEnumerable<IGameItem> WornItems { get; }

        IEnumerable<IGameItem> DirectWornItems { get; }

        /// <summary>
        ///     An IEnumerable of all items current worn on the IInventory sorted by display order
        /// </summary>
        IEnumerable<IGameItem> OrderedWornItems { get; }

        IEnumerable<IGameItem> WornItemsFor(IBodypart proto);

        IEnumerable<Tuple<IGameItem, IWearlocProfile>> WornItemsProfilesFor(IBodypart proto);

        IEnumerable<(IGameItem Item, IWear Wearloc, IWearlocProfile Profile)> WornItemsFullInfo { get; }

        ILookup<IWear, int> WornItemCounts { get; }

        void Restrain(IGameItem item, IWearProfile profile, ICharacter restrainer, IGameItem targetItem, IEmote emote = null, bool silent = false);

        /// <summary>
        ///     Whether or not the specified item can be worn, in any of its configurations.
        /// </summary>
        /// <param name="item">The item to test</param>
        /// <returns>True if the item can be draped</returns>
        bool CanWear(IGameItem item);

        /// <summary>
        ///     Returns an error message as to why a particular item cannot be worn.
        /// </summary>
        /// <param name="item">The item to test</param>
        /// <returns>The error message</returns>
        string WhyCannotWear(IGameItem item);

        /// <summary>
        ///     Whether or not the specified item can be worn, in the specified wear profile.
        /// </summary>
        /// <param name="item">The item to test</param>
        /// <param name="profile">The profile to test the item against</param>
        /// <returns>True if the item can be worn.</returns>
        bool CanWear(IGameItem item, IWearProfile profile);

        bool CanWear(IGameItem item, string profile);

        /// <summary>
        ///     Returns an error message as to why a particular item cannot be worn in a specified wear profile.
        /// </summary>
        /// <param name="item">The item to test</param>
        /// <param name="profile">The profile to test the item against.</param>
        /// <returns>The error message</returns>
        string WhyCannotWear(IGameItem item, IWearProfile profile);

        string WhyCannotWear(IGameItem item, string profile);

        /// <summary>
        /// Determines whether the body can be dressed by the nominated dresser, for the nominated item in such a configuration
        /// </summary>
        /// <param name="item">The item proposed to be worn</param>
        /// <param name="dresser">The person doing the dressing</param>
        /// <param name="profile">The optional specific profile to wear the item in</param>
        /// <returns>True if the person can be dressed</returns>
        bool CanDress(IGameItem item, ICharacter dresser, IWearProfile profile = null);

        string WhyCannotDress(IGameItem item, ICharacter dresser, IWearProfile profile = null);

        bool Dress(IGameItem item, ICharacter dresser, IWearProfile profile = null, IEmote emote = null);

        IWearProfile WhichProfile(IGameItem item);
        
        void RemoveItem(IGameItem item, IEmote playerEmote, bool silent = false, ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None);

        /// <summary>
        /// Removes the item with an appropriate emote for someone else taking the action. Does not subsequently "get" the item.
        /// </summary>
        /// <param name="item">The item to be removed</param>
        /// <param name="playerEmote">The playerEmote of the person REMOVING the item from this body</param>
        /// <param name="remover">The person removing the item from this body</param>
        void RemoveItem(IGameItem item, IEmote playerEmote, ICharacter remover);

        /// <summary>
        ///     Silently removes the item and does not GET it.
        /// </summary>
        /// <param name="item"></param>
        void RemoveItem(IGameItem item);

        /// <summary>
        ///     Whether or not the specified item can be removed from its current "Draped", or worn, location.
        /// </summary>
        /// <param name="item">The item to test</param>
        /// <returns>True if the item can be removed</returns>
        bool CanRemoveItem(IGameItem item, ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None);

        bool CanBeRemoved(IGameItem item, ICharacter remover);

        /// <summary>
        ///     Returns an error message as to why a particular item cannot be removed.
        /// </summary>
        /// <param name="item">The item to test</param>
        /// <returns>The error message</returns>
        string WhyCannotRemove(IGameItem item, bool ignoreFreeHands = false, ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None);

        string WhyCannotBeRemoved(IGameItem item, ICharacter remover);

        /// <summary>
        ///     Wears, the specified item in whichever profile fits.
        /// </summary>
        /// <param name="item">The item to wear</param>
        /// <param name="playerEmote"></param>
        /// <param name="silent"></param>
        /// <returns>True if successfully wore</returns>
        void Wear(IGameItem item, IEmote playerEmote = null, bool silent = false);

        /// <summary>
        ///     Wears, the specified item in the specified profile.
        /// </summary>
        /// <param name="item">The item to wear</param>
        /// <param name="profile">The profile in which to wear the item</param>
        /// <param name="playerEmote"></param>
        /// <param name="silent"></param>
        /// <returns>True if successfully wore</returns>
        void Wear(IGameItem item, IWearProfile profile, IEmote playerEmote = null, bool silent = false);

        void Wear(IGameItem item, string profile, IEmote playerEmote = null, bool silent = false);

        #endregion

        #region Wielded Items

        /// <summary>
        ///     An IEnumerable containing all of the items currently wielded
        /// </summary>
        IEnumerable<IGameItem> WieldedItems { get; }

        IEnumerable<IGameItem> WieldedItemsFor(IBodypart proto);

        bool CanDraw(IGameItem item, IWield specificHand, ItemCanWieldFlags flags = ItemCanWieldFlags.None);

        string WhyCannotDraw(IGameItem item, IWield specificHand, ItemCanWieldFlags flags = ItemCanWieldFlags.None);

        bool Draw(IGameItem item, IWield specificHand, IEmote playerEmote = null,
            OutputFlags additionalFlags = OutputFlags.Normal, bool silent = false, ItemCanWieldFlags flags = ItemCanWieldFlags.None);

        bool CanSheathe(IGameItem item, IGameItem sheath);

        string WhyCannotSheathe(IGameItem item, IGameItem sheath);

        bool Sheathe(IGameItem item, IGameItem sheath, IEmote playerEmote = null,
            OutputFlags additionalFlags = OutputFlags.Normal, bool silent = false);

        /// <summary>
        ///     Whether or not the specified item can be wielded
        /// </summary>
        /// <param name="item">The item to test</param>
        /// <returns>True if the item can be wielded</returns>
        bool CanWield(IGameItem item, ItemCanWieldFlags flags = ItemCanWieldFlags.None);

        bool CanWield(IGameItem item, IWield specificHand, ItemCanWieldFlags flags = ItemCanWieldFlags.None);

        /// <summary>
        ///     Returns an error message as to why a particular item cannot be wielded.
        /// </summary>
        /// <param name="item">The item to test</param>
        /// <param name="flags"></param>
        /// <returns>The error message</returns>
        string WhyCannotWield(IGameItem item, ItemCanWieldFlags flags = ItemCanWieldFlags.None);

        string WhyCannotWield(IGameItem item, IWield specificHand, ItemCanWieldFlags flags = ItemCanWieldFlags.None);

        /// <summary>
        ///     Whether or not the specified item can be unwielded.
        /// </summary>
        /// <param name="item">The item to test</param>
        /// <returns>True if the item can be unwielded</returns>
        bool CanUnwield(IGameItem item, bool ignoreFreeHands = false);

        /// <summary>
        ///     Returns an error message as to why a particular item cannot be wielded
        /// </summary>
        /// <param name="item">The item to test</param>
        /// <returns>The error message</returns>
        string WhyCannotUnwield(IGameItem item, bool ignoreFreeHands = false);

        /// <summary>
        ///     Wields the specified item
        /// </summary>
        /// <param name="item">The item to test</param>
        /// <param name="playerEmote"></param>
        /// <param name="silent"></param>
        /// <returns>True if the item was wielded</returns>
        bool Wield(IGameItem item, IEmote playerEmote = null, bool silent = false, ItemCanWieldFlags flags = ItemCanWieldFlags.None);

        bool Wield(IGameItem item, IWield specificHand, IEmote playerEmote = null, bool silent = false, ItemCanWieldFlags flags = ItemCanWieldFlags.None);

        /// <summary>
        ///     Unwields the specified item
        /// </summary>
        /// <param name="item">The item to test</param>
        /// <param name="playerEmote"></param>
        /// <param name="silent"></param>
        /// <returns>True if the item was unwielded</returns>
        bool Unwield(IGameItem item, IEmote playerEmote = null, bool silent = false);

        bool CanBeDisarmed(IGameItem item, ICharacter disarmer);

        IWield WieldedHand(IGameItem item);

        int WieldedHandCount(IGameItem item);

        #endregion

        #region Held Items

        /// <summary>
        ///     An IEnumerable containing all of the items current held
        /// </summary>
        IEnumerable<IGameItem> HeldItems { get; }

        IEnumerable<IGameItem> HeldOrWieldedItems { get; }
        IEnumerable<IGameItem> ItemsInHands { get; }

        IEnumerable<IGameItem> HeldItemsFor(IBodypart proto);

        IBodypart HoldOrWieldLocFor(IGameItem item);

        bool CanGet(IGameItem item, int quantity, ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None);

        bool CanGet(IGameItem item, IGameItem container, int quantity, ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None);

        bool CanGet(ICurrency currency, decimal amount, bool exact);

        bool CanGet(ICurrency currency, IGameItem container, decimal amount, bool exact);

        bool CanGetByWeight(IGameItem item, double weight, ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None);

        bool CanGetByWeight(IGameItem item, IGameItem container, double weight, ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None);

        /// <summary>
        ///     Returns an error message as to why the specified item cannot be grabbed in the specified fashion.
        /// </summary>
        /// <param name="item">The item to test</param>
        /// <returns>The error message</returns>
        string WhyCannotGet(IGameItem item, int quantity, ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None);

        string WhyCannotGet(IGameItem item, IGameItem container, int quantity, ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None);

        string WhyCannotGet(ICurrency currency, decimal amount, bool exact);

        string WhyCannotGet(ICurrency currency, IGameItem container, decimal amount, bool exact);

        bool WhyCannotGetByWeight(IGameItem item, double weight, ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None);

        bool WhyCannotGetByWeight(IGameItem item, IGameItem container, double weight, ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None);

        /// <summary>
        ///     Grabs the specified item
        /// </summary>
        /// <param name="item">The item to test</param>
        /// <param name="quantity"></param>
        /// <param name="playerEmote"></param>
        /// <param name="silent"></param>
        /// <returns>True if the grab succeeded</returns>
        void Get(IGameItem item, int quantity = 0, IEmote playerEmote = null, bool silent = false, ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None);

        void Get(IGameItem item, IGameItem containerItem, int quantity = 0, IEmote playerEmote = null, bool silent = false, ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None);

        void Get(ICurrency currency, IGameItem containerItem, decimal amount, bool exact, IEmote playerEmote = null,
            bool silent = false);

        void Get(ICurrency currency, decimal amount, bool exact, IEmote playerEmote = null, bool silent = false);

        void GetByWeight(IGameItem item, double weight, IEmote playerEmote = null, bool silent = false, ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None);

        void GetByWeight(IGameItem item, IGameItem container, double weight, IEmote playerEmote = null, bool silent = false, ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None);


        bool CanPut(IGameItem item, IGameItem container, ICharacter containerOwner, int quantity, bool allowLesserAmounts);

        bool CanPut(ICurrency currency, IGameItem container, ICharacter containerOwner, decimal amount, bool exact);

        string WhyCannotPut(IGameItem item, IGameItem container, ICharacter containerOwner, int quantity, bool allowLesserAmounts);

        string WhyCannotPut(ICurrency currency, IGameItem container, ICharacter containerOwner, decimal amount, bool exact);

        void Put(IGameItem item, IGameItem container, ICharacter containerOwner, int quantity = 0, IEmote playerEmote = null, bool silent = false, bool allowLesserAmounts = true);

        void Put(ICurrency currency, IGameItem container, ICharacter containerOwner, decimal amount, bool exact, IEmote playerEmote = null,
            bool silent = false);

        bool CanDrop(IGameItem item, int quantity);

        bool CanDrop(ICurrency currency, decimal amount, bool exact);

        /// <summary>
        ///     Returns an error message as to why the specified item cannot be dropped
        /// </summary>
        /// <param name="item">The item to test</param>
        /// <param name="quantity">The quantity of the item to test</param>
        /// <returns>The error message</returns>
        string WhyCannotDrop(IGameItem item, int quantity);

        string WhyCannotDrop(ICurrency currency, decimal amount, bool exact);

        /// <summary>
        ///     Silently removes an item from the specified inventory. Calling function assumes responsibility for the item from
        ///     there.
        /// </summary>
        /// <param name="item">The item to take</param>
        void Take(IGameItem item);

        /// <summary>
        ///     Silently removes a quantity of an item from the specified inventory. Calling function assumes all responsibility
        ///     for the item from there.
        /// </summary>
        /// <param name="item">The item to take</param>
        /// <param name="quantity">The quantity of item to take, or zero to take the whole stack</param>
        /// <returns>The item that was removed</returns>
        IGameItem Take(IGameItem item, int quantity);

        /// <summary>
        ///     Drops the specified item
        /// </summary>
        /// <param name="item">The item to drop</param>
        /// <param name="quantity"></param>
        /// <param name="newStack"></param>
        /// <param name="playerEmote"></param>
        /// <param name="silent"></param>
        /// <returns>True if the item was dropped</returns>
        void Drop(IGameItem item, int quantity = 0, bool newStack = false, IEmote playerEmote = null, bool silent = false);

        void Drop(ICurrency currency, decimal amount, bool exact, bool newStack = false, IEmote playerEmote = null,
            bool silent = false);

        bool CanGive(IGameItem item, IBody target, int quantity = 0);

        bool CanGive(ICurrency currency, IBody target, decimal amount, bool exact);

        string WhyCannotGive(IGameItem item, IBody target, int quantity = 0);

        string WhyCannotGive(ICurrency currency, IBody target, decimal amount, bool exact);

        void Give(IGameItem item, IBody target, int quantity = 0, IEmote playerEmote = null);

        void Give(ICurrency currency, IBody target, decimal amount, bool exact, IEmote playerEmote = null);

        bool CanGive(IGameItem item, ICorpse target, int quantity = 0);

        bool CanGive(ICurrency currency, ICorpse target, decimal amount, bool exact);

        string WhyCannotGive(IGameItem item, ICorpse target, int quantity = 0);

        string WhyCannotGive(ICurrency currency, ICorpse target, decimal amount, bool exact);

        void Give(IGameItem item, ICorpse target, int quantity = 0, IEmote playerEmote = null);

        void Give(ICurrency currency, ICorpse target, decimal amount, bool exact, IEmote playerEmote = null);

        IEnumerable<IGrab> FreeHands { get; }

        IEnumerable<IGrab> FunctioningFreeHands { get; }

        /// <summary>
        /// Swaps the hands in which the first and second items are held/wielded
        /// </summary>
        /// <param name="firstItem">The first item</param>
        /// <param name="secondItem">The second item</param>
        /// <returns>True if the swap took place</returns>
        bool Swap(IGameItem firstItem, IGameItem secondItem);

        #endregion

        #region Display Commands

        IEnumerable<Tuple<WearableItemCoverStatus, IGameItem>> CoverInformation(IGameItem item);

        /// <summary>
        ///     For a given group of IBodyparts, this displays the "best" fit for an amalgamated description - e.g. larm and rarm
        ///     might display as "arms"
        /// </summary>
        /// <param name="group">The IBodyparts to describe</param>
        /// <returns>The descriptions of the parts</returns>
        string DescribeBodypartGroup(IEnumerable<IBodypart> group);

        /// <summary>
        ///     For a given item, displays how it is worn. e.g. worn on the arms
        /// </summary>
        /// <param name="item">The item to test</param>
        /// <returns>The string describing how the item is worn</returns>
        string DescribeHowWorn(IGameItem item);

        #endregion
    }

    #region Extension Classes

    public static class InventoryExtensions {
        public static string GetInventoryString<T>(this T inv, IPerceiver perceiver, bool ignoreCover = false)
            where T : IInventory {
            if (!inv.HeldOrWieldedItems.Any(x => perceiver.CanSee(x)) &&
                !inv.WornItems.Any(
                    x =>
                        perceiver.CanSee(x) && x.IsItemType<IWearable>() &&
                        x.GetItemType<IWearable>().DisplayInventoryWhenWorn)) {
                return string.Format("{0} {1} completely naked, and {1} not holding or wielding anything.",
                                     perceiver.IsSelf(inv) ? "You" : inv.ApparentGender(perceiver).Subjective(true),
                                     perceiver.IsSelf(inv) ? "are" : "is");
            }

            var sb = new StringBuilder();
            var output = false;

            if (perceiver.Gameworld.GetStaticBool("HeldAndWieldedDisplayAtTop"))
            {
	            foreach (var item in inv.HeldOrWieldedItems.Where(item => perceiver.CanSee(item)))
	            {
		            sb.AppendLine(item.GetItemType<IHoldable>().CurrentInventoryDescription + item.HowSeen(perceiver));
		            output = true;
	            }

	            // Add a new line if we have any wielded/held items
	            if (output)
	            {
		            sb.AppendLine("");
	            }
			}
            
            foreach (var item in inv.OrderedWornItems) {
                var beltable = item.GetItemType<IBeltable>();
                if (beltable?.ConnectedTo != null) {
                    if (
                        inv.CoverInformation(beltable.ConnectedTo.Parent)
                            .Any(x => x.Item1 != WearableItemCoverStatus.Covered)) {
                        sb.AppendLine(
                            $"{string.Format($"<attached to {beltable.ConnectedTo.Parent.Name.ToLowerInvariant()}>", beltable.ConnectedTo.Parent.Name.ToLowerInvariant()),-35}" +
                            item.HowSeen(perceiver));
                    }

                    continue;
                }
                if (!perceiver.CanSee(item) || (!item.GetItemType<IWearable>().DisplayInventoryWhenWorn && !ignoreCover)) {
                    continue;
                }

                var cover = inv.CoverInformation(item).ToList();
                if (
                    cover.All(
                        x =>
                            (x.Item1 == WearableItemCoverStatus.Covered) &&
                            !(x.Item2.GetItemType<IWearable>()?.GloballyTransparent ?? false)) && !ignoreCover) {
                    continue;
                }

                if (
                    !cover.Any(
                        x =>
                            (x.Item1 == WearableItemCoverStatus.Covered) ||
                            (x.Item1 == WearableItemCoverStatus.TransparentlyCovered))) {
                    sb.AppendLine(item.GetItemType<IHoldable>().CurrentInventoryDescription + item.HowSeen(perceiver));
                }
                else {
                    var coveredlocs =
                        cover.Where(
                            x =>
                                (x.Item1 == WearableItemCoverStatus.Covered) ||
                                (x.Item1 == WearableItemCoverStatus.TransparentlyCovered));
                    var coveritems =
                        coveredlocs.Select(x => x.Item2)
                            .Distinct()
                            .Select(x => x.HowSeen(perceiver, colour: false))
                            .ListToString();

                    sb.AppendLineFormat("{0}{1} {2}", item.GetItemType<IHoldable>().CurrentInventoryDescription,
                        item.HowSeen(perceiver),
                        cover.All(
                            x =>
                                (x.Item1 == WearableItemCoverStatus.Covered) ||
                                (x.Item1 == WearableItemCoverStatus.TransparentlyCovered))
                            ? "(covered)".FluentTagMXP("send", $"href='look' hint='{coveritems}'")
                            : "(partially covered)".FluentTagMXP("send", $"href='look' hint='{coveritems}'"));
                }
            }

            if (!perceiver.Gameworld.GetStaticBool("HeldAndWieldedDisplayAtTop"))
			{

				// Add a new line if we have any inventory
				if (output)
				{
					sb.AppendLine("");
				}

				foreach (var item in inv.HeldOrWieldedItems.Where(item => perceiver.CanSee(item)))
	            {
		            sb.AppendLine(item.GetItemType<IHoldable>().CurrentInventoryDescription + item.HowSeen(perceiver));
		            output = true;
	            }
            }

			return sb.ToString();
        }

        public static void DisplayInventory<T>(this T inv, IPerceiver perceiver, bool ignoreCover = false)
            where T : IInventory {
            perceiver.OutputHandler.Send(inv.GetInventoryString(perceiver, ignoreCover));
        }
    }

    #region Wielding

    public enum WhyCannotWieldReason {
        NotIWieldable,
        NoFreeHands,
        FreeHandsTooDamaged,
        TooFewHands,
        TooFewUndamagedHands,
        Unknown
    }

    public enum WhyCannotUnwieldReason {
        NotWielded,
        Unknown
    }


    public static class WieldExtensionClass {
        public static WhyCannotWieldReason WhyCannotWield<T>(this IEnumerable<T> wieldlocs, IGameItem item,
            IInventory body) where T : IWield {
            var wieldable = item.GetItemType<IWieldable>();
            if (wieldable == null) {
                return WhyCannotWieldReason.NotIWieldable;
            }

            var reasons = wieldlocs.Select(x => x.CanWield(item, body)).ToList();

            if (reasons.Any(x => x == IWieldItemWieldResult.Success)) {
                return reasons.Any(x => x == IWieldItemWieldResult.TooDamaged) ? WhyCannotWieldReason.TooFewUndamagedHands : WhyCannotWieldReason.TooFewHands;
            }

            if (reasons.Any(x => x == IWieldItemWieldResult.TooDamaged)) {
                return WhyCannotWieldReason.FreeHandsTooDamaged;
            }

            return reasons.Any(
                x =>
                    (x == IWieldItemWieldResult.AlreadyWielding) ||
                    (x == IWieldItemWieldResult.GrabbingWielderHoldOtherItem)) ? WhyCannotWieldReason.NoFreeHands : WhyCannotWieldReason.Unknown;
        }
    }

    #endregion

    #region Grabbing

    public enum WhyCannotGrabReason {
        NotIHoldable,
        CannotStack,
        CouldIfNotStack,
        HandsFull,
        InventoryFull,
        HandsTooDamaged,
        NoFreeUndamagedHands,
        Unknown
    }

    public enum WhyCannotDropReason {
        Unknown
    }

    public static class GrabExtensionClass {
        public static WhyCannotGrabReason WhyCannotGrab<T>(this IEnumerable<T> holdlocs, IGameItem item,
            IInventory inventory) where T : IGrab {
            var results = holdlocs.Select(x => x.CanGrab(item, inventory)).ToList();
            if (results.All(x => x == WearlocGrabResult.FailFull)) {
                return WhyCannotGrabReason.HandsFull;
            }

            if (results.All(x => x == WearlocGrabResult.FailDamaged)) {
                return WhyCannotGrabReason.HandsTooDamaged;
            }

            if (results.All(x => (x == WearlocGrabResult.FailFull) || (x == WearlocGrabResult.FailDamaged))) {
                return WhyCannotGrabReason.NoFreeUndamagedHands;
            }

            return results.All(x => x == WearlocGrabResult.FailTooBig) ? WhyCannotGrabReason.InventoryFull : WhyCannotGrabReason.Unknown;
        }

        public static WhyCannotDropReason WhyCannotDrop<T>(this IEnumerable<T> holdlocs, IGameItem item) where T : IGrab {
            return WhyCannotDropReason.Unknown;
        }
    }

    #endregion

    #region Wearing

    public enum WhyCannotDrapeReason {
        NotIDrapeable,
        NotBodyType,
        BadFit,
        NoProfilesMatch,
        SpecificProfileNoMatch,
        ProgFailed,
        TooBulky,
        TooManyItems,
        Unknown
    }

    public enum WhyCannotRemoveReason {
        ItemIsCovered,
        ItemIsAttached,
        Unknown
    }
    
    #endregion

    #endregion
}