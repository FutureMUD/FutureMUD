using System.Collections.Generic;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory;
using MudSharp.RPG.Checks;

namespace MudSharp.Work.Butchering
{
    public enum ButcheryVerb
    {
        Butcher,
        Salvage
    }

    public interface IRaceButcheryProfile : IEditableItem
    {

        /// <summary>
        /// The verb used to interact with this race, e.g. butcher vs salvage
        /// </summary>
        ButcheryVerb Verb { get; }
        
        /// <summary>
        /// If specified, a tool tag required to be held to complete the process
        /// </summary>
        ITag RequiredToolTag { get; }

        /// <summary>
        /// The difficulty to use the SKIN verb on this
        /// </summary>
        Difficulty DifficultySkin { get; }

        /// <summary>
        /// The trait and difficulty to use the BUTCHER/SALVAGE verbs on this
        /// </summary>
        /// <param name="subcategory">If specified, the subcategory for which to fetch the difficulty</param>
        /// <returns>The trait and difficulty of the breakdown</returns>
        (ITraitDefinition Trait, Difficulty CheckDifficulty) BreakdownCheck(string subcategory);

        /// <summary>
        /// The emotes and delays between each phase of skinning
        /// </summary>
        IEnumerable<(string Emote,double Delay)> SkinEmotes { get; }

        /// <summary>
        /// The emotes and delays between each phase of breakdown
        /// </summary>
        /// <param name="subcategory">If specified, the subcategory for which to fetch the breakdown emotes</param>
        /// <returns>The emotes and delays for the phases</returns>
        IEnumerable<(string Emote, double Delay)> BreakdownEmotes(string subcategory);

        /// <summary>
        /// The products to load when salvaged
        /// </summary>
        IEnumerable<IButcheryProduct> Products { get; }

        /// <summary>
        /// A prog accepting a character and item parameter which determines whether a character can butcher this race
        /// </summary>
        IFutureProg CanButcherProg { get; }

        /// <summary>
        /// Determines whether a character can butcher the target corpse
        /// </summary>
        /// <param name="butcher">The character doing the butchering</param>
        /// <param name="targetItem">The bodypart or corpse being butchered</param>
        /// <returns>True if the butcher can butcher the item</returns>
        bool CanButcher(ICharacter butcher, IGameItem targetItem);

        /// <summary>
        /// A prog accepting a character and item parameter which determines an error message when a character cannot butcher this race
        /// </summary>
        IFutureProg WhyCannotButcherProg { get; }

        /// <summary>
        /// Retrieves an error message when a character cannot butcher a race
        /// </summary>
        /// <param name="butcher">The character doing the butchering</param>
        /// <param name="targetItem">The bodypart or corpse being butchered</param>
        /// <returns>An error message</returns>
        string WhyCannotButcher(ICharacter butcher, IGameItem targetItem);

        IInventoryPlanTemplate ToolTemplate { get; }

        IRaceButcheryProfile Clone(string newName, bool includeProducts);
    }
}
