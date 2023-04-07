using System.Collections.Generic;
using MudSharp.Body;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Size;

namespace MudSharp.GameItems.Interfaces {
    public interface IWearable : IGameItemComponent {
        /// <summary>
        ///     On an instance of a wearable item, this is set to however the item is currently being worn
        /// </summary>
        IWearProfile CurrentProfile { get; }

        IEnumerable<IWearProfile> Profiles { get; }

        IWearProfile DefaultProfile { get; }

        bool DisplayInventoryWhenWorn { get; }

        IBody WornBy { get; }

        bool Bulky { get; }
        double LayerWeightConsumption { get; }

        bool Waterproof { get; }

        /// <summary>
        ///     If true, overrides lack of transparency on individual wearlocs
        /// </summary>
        bool GloballyTransparent { get; }

        bool CanWear(IBody wearer, IWearProfile profile);

        WhyCannotDrapeReason WhyCannotWear(IBody wearer, IWearProfile profile);

        bool CanWear(IBody wearer);

        WhyCannotDrapeReason WhyCannotWear(IBody wearer);

        string WhyCannotWearProgText(IBody wearer);

        void UpdateWear(IBody body, IWearProfile profile);
    }
}