using System.Collections.Generic;
using MudSharp.Celestial;
using MudSharp.FutureProg;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.Construction {
    public interface IShard : ILocation, IProgVariable {
        double MinimumTerrestrialLux { get; }
        IEditableShard GetEditableShard { get; }
        double SphericalRadiusMetres { get; }
        IRoom DetermineRoomByCoordinates(int x, int y, int z);
        IRoom DetermineRoomByDirection(IRoom fromRoom, CardinalDirection direction);
        void Register(IRoom room);
        void Unregister(IRoom room);
        void Register(IZone zone);
        void Unregister(IZone zone);
        string DescribeSky(double skyBrightness);
        IEnumerable<IZone> Zones { get; }
        IEnumerable<IRoom> Rooms { get; }
        IEnumerable<ICell> Cells { get; }
    }

    public interface IEditableShard : IShard {
        new double MinimumTerrestrialLux { get; set; }
        new List<IClock> Clocks { get; }
        new List<ICalendar> Calendars { get; }
        new List<ICelestialObject> Celestials { get; }
        ISkyDescriptionTemplate SkyDescriptionTemplate { get; set; }
        new double SphericalRadiusMetres { get; set; }
        void SetName(string name);
    }
}