using System.Collections.Generic;
using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Climate;
using MudSharp.FutureProg;
using MudSharp.TimeAndDate.Time;
using MudSharp.Work.Foraging;

namespace MudSharp.Construction {
    public interface IZone : ILocation, IFutureProgVariable {
        IShard Shard { get; }
        ICell DefaultCell { get; }
        IEnumerable<IRoom> Rooms { get; }
        IEnumerable<ICell> Cells { get; }

        GeographicCoordinate Geography { get; }
        double AmbientLightPollution { get; }
        long P { get; }
        double CurrentLightLevel { get; }
        void RecalculateLightLevel();
        string DescribeSky { get; }

        IEditableZone GetEditableZone { get; }
        IForagableProfile ForagableProfile { get; }
        void Register(IRoom room);
        void Unregister(IRoom room);
        void CalculateCoordinates();
        IWeatherController Weather { get; }
        TimeOfDay CurrentTimeOfDay { get; }
        string ShowToBuilder(ICharacter builder);
        void InitialiseCelestials();
        void DeregisterCelestials();
    }

    public interface IEditableZone : IZone {
	    new double AmbientLightPollution { get; set; }
        new GeographicCoordinate Geography { get; set; }
        Dictionary<IClock, IMudTimeZone> TimeZones { get; }
        new IForagableProfile ForagableProfile { get; set; }
        void SetName(string name);
        new IWeatherController Weather { get; set; }
    }
}