using MudSharp.Celestial;
using MudSharp.Climate;
using MudSharp.Framework;
using MudSharp.FutureProg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Construction
{
    /// <summary>
    /// An area is an alternative hierarchy for room grouping - an area has a number of rooms in it, but doesn't necessarily just belong to one zone
    /// </summary>
    public interface IArea : ILocation, IProgVariable
    {
        IEnumerable<IRoom> Rooms { get; }
        IEnumerable<ICell> Cells { get; }
        IEnumerable<IZone> Zones { get; }
        IWeatherController Weather { get; }
        TimeOfDay CurrentTimeOfDay { get; }
    }

    public interface IEditableArea : IArea
    {
        void Add(IRoom room);
        void Remove(IRoom room);
        new IWeatherController Weather { get; set; }
        void SetName(string name);
    }
}
