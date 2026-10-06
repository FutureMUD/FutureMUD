using MudSharp.Celestial;
using MudSharp.FutureProg;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using System.Collections.Generic;

namespace MudSharp.Construction
{
    public interface IShard : ILocation, IProgVariable
    {
        double MinimumTerrestrialLux { get; }
        IEditableShard GetEditableShard { get; }
        double SphericalRadiusMetres { get; }
        ICell DetermineCellByCoordinates(int x, int y, int z);
        ICell DetermineCellByDirection(ICell fromCell, CardinalDirection direction);
        void Register(ICell room);
        void Unregister(ICell room);
        void Register(IZone zone);
        void Unregister(IZone zone);
        string DescribeSky(double skyBrightness);
        IEnumerable<IZone> Zones { get; }
        new IEnumerable<ICell> Cells { get; }
    }

    public interface IEditableShard : IShard
    {
        new double MinimumTerrestrialLux { get; set; }
        new List<IClock> Clocks { get; }
        new List<ICalendar> Calendars { get; }
        new List<ICelestialObject> Celestials { get; }
        ISkyDescriptionTemplate SkyDescriptionTemplate { get; set; }
        new double SphericalRadiusMetres { get; set; }
        void SetName(string name);
    }
}
