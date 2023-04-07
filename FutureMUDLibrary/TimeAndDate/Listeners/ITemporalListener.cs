using MudSharp.Framework;

namespace MudSharp.TimeAndDate.Listeners {
    public interface ITemporalListener : IFrameworkItem {
        /// <summary>
        ///     This function specifies whether this listener or its action have anything to do with the specified item - for
        ///     instance, if the Item was actually an ICharacter who was dead/logging out, this could be used
        ///     to identify ones that might need to be removed.
        /// </summary>
        /// <param name="item">The item to compare</param>
        /// <returns>True if any component of the listener or payload refers to the Item</returns>
        bool PertainsTo(object item);

        void CancelListener();
    }
}