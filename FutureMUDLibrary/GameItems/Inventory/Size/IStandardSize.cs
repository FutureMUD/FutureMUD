using MudSharp.Framework;

namespace MudSharp.GameItems.Inventory.Size {
    public interface IStandardSize : IFrameworkItem {
        /// <summary>
        ///     The ID of the Body Prototype to which this standard size applies
        /// </summary>
        int BodyID { get; }

        /// <summary>
        ///     The IWearableSize used for this template
        /// </summary>
        IWearableSize TemplateSize { get; }
    }
}