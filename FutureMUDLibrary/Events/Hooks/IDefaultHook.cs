using MudSharp.FutureProg;

namespace MudSharp.Events.Hooks {
    public interface IDefaultHook {
        IHook Hook { get; }
        IFutureProg EligibilityProg { get; }
        string PerceivableType { get; }

        /// <summary>
        ///     Determines whether or not this Default Hook applies to the specified item.
        /// </summary>
        /// <param name="item">The item to check against</param>
        /// <param name="type">The FrameworkItemType of the item.</param>
        /// <returns>True if the Default Hook should be hooked</returns>
        bool Applies(IProgVariable item, string type);

        void Delete();
    }
}