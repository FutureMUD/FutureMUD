using MudSharp.Framework;

namespace MudSharp.GameItems.Decorators {
    /// <summary>
    ///     An IStackDecorator is designed to be used with stacking items to determine their description
    /// </summary>
    public interface IStackDecorator : IFrameworkItem {
        string Description { get; }
        string Describe(string name, string description, double quantity);
    }
}