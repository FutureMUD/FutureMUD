using System;
using MudSharp.Body;
using MudSharp.Framework;

namespace MudSharp.GameItems.Inventory.Size {
    public interface IWearableSize : IFrameworkItem {
        /// <summary>
        ///     Sets the size of an item to the parameters of a given body
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        IWearableSize SetSize(IBody body);

        /// <summary>
        ///     Sets the size of an item to the standard size specified
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        IWearableSize SetSize(IWearableSize size);

        IWearableSize Clone();
        bool CanWear(IBody body);
        Tuple<ItemVolumeFitDescription, ItemLinearFitDescription> DescribeFit(IBody body);

        double Height { get; }
        double Weight { get; }
        IBodyPrototype Body { get; }
        double TraitValue { get; }
    }
}