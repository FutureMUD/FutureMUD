using System.Collections.Generic;
using MudSharp.Framework;

namespace MudSharp.GameItems.Interfaces {
    public interface IChair : IGameItemComponent {
        ITable Table { get; }
        IEnumerable<IPerceivable> Occupants { get; }

        /// <summary>
        ///     How many slots for chairs this chair uses up when placed at a table - rough measure of size
        /// </summary>
        int ChairSlotsUsed { get; }

        /// <summary>
        ///     How many normal sized occupants can use this chair at one time. Usually 1.
        /// </summary>
        int OccupantCapacity { get; }

        void SetTable(ITable table, bool nosave = false);
    }
}