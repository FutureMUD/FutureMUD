using System.Collections.Generic;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.GameItems;

namespace MudSharp.Work.Butchering
{
    public interface IButcheryProduct : IEditableItem
    {
        /// <summary>
        /// The body prototype to which this product applies
        /// </summary>
        IBodyPrototype TargetBody { get; }

        /// <summary>
        /// The bodyparts that an item or corpse is required to contain to be butchered
        /// </summary>
        IEnumerable<IBodypart> RequiredBodyparts { get; }

        /// <summary>
        /// Whether this counts as a pelt, i.e. requires the SKIN verb rather than the BUTCHER/SALVAGE verb.
        /// </summary>
        bool IsPelt { get; }

        /// <summary>
        /// A prog accepting a character and an item parameter that determines whether this product can be produced
        /// </summary>
        IFutureProg CanProduceProg { get; }

        /// <summary>
        /// Determines whether a butcher can produce this product from a given item
        /// </summary>
        /// <param name="butcher">The character who is butchering</param>
        /// <param name="targetItem">The bodypart or corpse being butchered</param>
        /// <returns>True if this product applies</returns>
        bool CanProduce(ICharacter butcher, IGameItem targetItem);

        /// <summary>
        /// An optional string specifying a category to which this product belongs, i.e. if someone does SALVAGE CORPSE ELECTRONICS to only salvage electronics, electronics would be the subcategory
        /// </summary>
        string Subcategory { get; }

        /// <summary>
        /// The actual items produced when this product is invoked
        /// </summary>
        IEnumerable<IButcheryProductItem> ProductItems { get; }
    }
}
