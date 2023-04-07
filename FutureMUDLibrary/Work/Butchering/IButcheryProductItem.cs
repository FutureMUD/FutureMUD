using MudSharp.GameItems;

namespace MudSharp.Work.Butchering
{
    public interface IButcheryProductItem
    {
        /// <summary>
        /// The prototype to load in normal cases for this item
        /// </summary>
        IGameItemProto NormalProto { get; }

        /// <summary>
        /// The prototype to load in cases where the damage threshold is exceeded. If null, load nothing.
        /// </summary>
        IGameItemProto DamagedProto { get; }

        /// <summary>
        /// The quantity of items to load in the normal case
        /// </summary>
        int NormalQuantity { get; }

        /// <summary>
        /// The quantity of items to load in the damaged case
        /// </summary>
        int DamagedQuantity { get; }

        /// <summary>
        /// The threshold in terms of damage percentage to parts beyond which this item is considered "damaged"
        /// </summary>
        double DamagedThreshold { get; }
    }
}
