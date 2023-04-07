using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.GameItems;
using System.Collections.Generic;

#nullable enable

namespace MudSharp.Framework {
    public interface ITarget {
        /// <summary>
        ///     Searches for an returns the first IPerceivable it can find.
        /// </summary>
        /// <param name="keyword">What keyword to look for.</param>
        /// <returns>An IPerceivable meeting the keyword criterion, if found - otherwise null.</returns>
        IPerceivable? Target(string keyword);

        /// <summary>
        ///     Searches for and returns an ICharacter meeting the prescribed keywords.
        /// </summary>
        /// <param name="keyword">What keyword to look for.</param>
        /// <param name="ignoreFlags"></param>
        /// <returns>An ICharacter meeting the keyword criterion, if found - otherwise null.</returns>
        ICharacter? TargetActor(string keyword, PerceiveIgnoreFlags ignoreFlags = PerceiveIgnoreFlags.None);

        /// <summary>
        /// Searches for and returns an ICharacter who is also an ally that meets the prescribed keywords
        /// </summary>
        /// <param name="keyword">What keyword to look for</param>
        /// <returns>An allied ICharacter meeting the keyword criterion, if found - otherwise null.</returns>
        ICharacter? TargetAlly(string keyword);

        /// <summary>
        /// Searches for and returns an ICharacter who is also not an ally that meets the prescribed keywords
        /// </summary>
        /// <param name="keyword">What keyword to look for</param>
        /// <returns>A non-allied ICharacter meeting the keyword criterion, if found - otherwise null.</returns>
        ICharacter? TargetNonAlly(string keyword);

        /// <summary>
        ///     Searches for and returns an IBody meeting the prescribed keywords.
        /// </summary>
        /// <param name="keyword">What keyword to look for.</param>
        /// <returns>An IBody meeting the keyword criterion, if found - otherwise null.</returns>
        IBody? TargetBody(string keyword);

        /// <summary>
        ///     Searches for and returns an IPerceivable in the Location of the ITarget, excluding contents or personal items
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        IPerceivable? TargetLocal(string keyword);

        /// <summary>
        ///     Searches for and returns an IGameItem meeting the given keywords. This combines TargetLocalItem and
        ///     TargetPersonalItem.
        /// </summary>
        /// <param name="keyword">What keyword to look for.</param>
        /// <returns>An IGameItem meeting the keyword criterion, if found - otherwise null.</returns>
        IGameItem? TargetItem(string keyword);

        /// <summary>
        ///     Searches for and returns an IGameItem meeting the given keywords found in this ITarget's Location
        /// </summary>
        /// <param name="keyword">What keyword to look for.</param>
        /// <returns>An IGameItem meeting the keyword criterion, if found - otherwise null.</returns>
        IGameItem? TargetLocalItem(string keyword);

        /// <summary>
        ///     Searches for and returns an IGameItem meeting the given keywords found in this ITarget's inventory
        /// </summary>
        /// <param name="keyword">What keyword to look for.</param>
        /// <returns>An IGameItem meeting the keyword criterion, if found - otherwise null.</returns>
        IGameItem? TargetPersonalItem(string keyword);

        /// <summary>
        ///     Searches for and returns an IGameItem meeting the given keywords found either in this ITarget's Location or Held
        ///     Inventory
        /// </summary>
        /// <param name="keyword">What keyword to look for.</param>
        /// <returns>An IGameItem meeting the keyword criterion, if found - otherwise null.</returns>
        IGameItem? TargetLocalOrHeldItem(string keyword);

        /// <summary>
        ///     Searches for and returns an IGameItem meeting the given keywords found in this ITarget's hands
        /// </summary>
        /// <param name="keyword">What keyword to look for.</param>
        /// <returns>An IGameItem meeting the keyword criterion, if found - otherwise null.</returns>
        IGameItem? TargetHeldItem(string keyword);

        /// <summary>
        ///     Searches for and returns an IGameItem meeting the given keywords found in this ITarget's worn items
        /// </summary>
        /// <param name="keyword">What keyword to look for.</param>
        /// <returns>An IGameItem meeting the keyword criterion, if found - otherwise null.</returns>
        IGameItem? TargetWornItem(string keyword);

        /// <summary>
        ///     Differs from TargetWornItem in that it puts top-level worn items first in the list
        /// </summary>
        /// <param name="keyword">What keyword to look for.</param>
        /// <returns>An IGameItem meeting the keyword criterion, if found - otherwise null.</returns>
        IGameItem? TargetTopLevelWornItem(string keyword);

        ICharacter? TargetActorOrCorpse(string keyword, PerceiveIgnoreFlags ignoreFlags = PerceiveIgnoreFlags.None);

        (ICharacter? Target, IEnumerable<ICellExit> Path) TargetDistantActor(string keyword, ICellExit? initialExit, uint maximumRange, bool respectDoors,
            bool respectCorners);

        (IGameItem? Target, IEnumerable<ICellExit> Path) TargetDistantItem(string keyword, ICellExit? initialExit, uint maximumRange, bool respectDoors,
            bool respectCorners);


        string BestKeywordFor(IPerceivable target);
        string BestKeywordFor(ICharacter target);
        string BestKeywordFor(IGameItem target);
    }
}