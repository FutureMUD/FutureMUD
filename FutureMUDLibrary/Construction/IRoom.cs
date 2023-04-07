using MudSharp.Celestial;
using System;
using System.Collections.Generic;

namespace MudSharp.Construction {
    public interface IRoom : ILocation {
        IZone Zone { get; }
        IShard Shard { get; }
        IEnumerable<ICell> Cells { get; }
        IEnumerable<IArea> Areas { get; }
        TimeOfDay CurrentTimeOfDay { get; }

        long P { get; }
        int X { get; set; }
        int Y { get; set; }
        int Z { get; set; }
        void Destroy(ICell cell);
        void Register(ICell cell);
        /// <summary>
        /// Orders the complete destruction of the room and any cells within it, placing any remaining contents in the fallback cell
        /// </summary>
        /// <param name="fallbackCell">The cell to place any contents and people remaining in the room</param>
        void DestroyRoom(ICell fallbackCell);
        Action DestroyRoomWithDatabaseAction(ICell fallbackCell);
        void SetNewZone(IZone zone);
        void AddArea(IArea area);
        void RemoveArea(IArea area);
    }
}