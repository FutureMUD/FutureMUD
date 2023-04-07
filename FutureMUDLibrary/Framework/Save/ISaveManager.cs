using System;

namespace MudSharp.Framework.Save {
    public interface ISaveManager {
        /// <summary>
        ///     Indicates that an item is due to be saved and adds it to the queue
        /// </summary>
        /// <param name="item">The item to be saved</param>
        void Add(ISaveable item);

        /// <summary>
        ///     Calls upon all items to be saved and flush the queue
        /// </summary>
        void Flush();

        /// <summary>
        ///     Requests that the save manager abort the queued saving of the item in question
        /// </summary>
        /// <param name="item">The item to not be saved</param>
        void Abort(ISaveable item);

        void AbortLazyLoad(ILazyLoadDuringIdleTime item);

        void DirectInitialise(ILateInitialisingItem item);

        void AddInitialisation(ILateInitialisingItem item);

        void AddLazyLoad(ILazyLoadDuringIdleTime item);

        string DebugInfo(IFuturemud gameworld);

        bool IsQueued(ISaveable saveable);

        bool Flushing { get; }

        void FlushLazyLoad(TimeSpan maximumTime);

        bool MudBootingMode { get; set; }
    }
}