using MudSharp.Framework;

namespace MudSharp.Body.Traits {
    /// <summary>
    ///     An object implementing IUseTrait specifies that a particular trait be invoked when it is "Used". For example,
    ///     weapons.
    /// </summary>
    public interface IUseTrait : IFrameworkItem {
        ITraitDefinition Trait { get; }
    }
}