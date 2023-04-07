using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Framework;

namespace MudSharp.GameItems.Inventory.Size
{
    public interface IWearableSizeRules
    {
        /// <summary>
        ///     The body for which these rules apply
        /// </summary>
        IBodyPrototype Body { get; }

        double MinHeightFactor { get; }
        double MaxHeightFactor { get; }
        double MinWeightFactor { get; }
        double MaxWeightFactor { get; }
        double MinTraitFactor { get; }
        double MaxTraitFactor { get; }
        ITraitDefinition WhichTrait { get; }
        bool IgnoreAttribute { get; }
        RankedRange<ItemVolumeFitDescription> WeightVolumeRatios { get; }
        RankedRange<ItemVolumeFitDescription> TraitVolumeRatios { get; }
        RankedRange<ItemLinearFitDescription> HeightLinearRatios { get; }
    }
}