using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Framework;

namespace MudSharp.Celestial {
    public delegate void CelestialUpdateHandler(ICelestialObject sender);

    public enum CelestialMoveDirection {
        Ascending,
        Descending
    }

    public static class CelestialMoveDirectionExtension {
        public static string Describe(this CelestialMoveDirection direction) {
            switch (direction) {
                case CelestialMoveDirection.Ascending:
                    return "Ascending";
                case CelestialMoveDirection.Descending:
                    return "Descending";
                default:
                    return "Unknown";
            }
        }
    }

    public interface ICelestialObject : IFrameworkItem, IPerceivable {
        PerceptionTypes PerceivableTypes { get; }
        void AddMinutes(int numberOfMinutes);
        void AddMinutes();
        double CurrentCelestialDay { get; }
        double CelestialDaysPerYear { get; }
        /// <summary>
        ///     Prompts locations to update their celestial information and request any triggers
        /// </summary>
        event CelestialUpdateHandler MinuteUpdateEvent;

        /// <summary>
        ///     Returns the current elevation angle of the celestial object given the supplied parameters from the viewer
        /// </summary>
        /// <param name="latitude">Latitude in Degrees of viewer</param>
        /// <param name="longitude">Longitude in Degrees of viewer</param>
        /// <param name="elevation">Elevation in Metres of viewer</param>
        /// <returns>Elevation angle in Radians</returns>
        double CurrentElevationAngle(GeographicCoordinate geography);

        /// <summary>
        ///     Returns the current azimuth angle of the celestial object from North=0 given the supplied parameters from the
        ///     viewer
        /// </summary>
        /// <param name="latitude">Latitude in Degrees of viewer</param>
        /// <param name="longitude">Longitude in Degrees of viewer</param>
        /// <param name="elevation">ElevationAngle is result of currentElevationAngle</param>
        /// <returns>Azimuth angle in Radians - north = 0</returns>
        double CurrentAzimuthAngle(GeographicCoordinate geography, double elevationAngle);

        /// <summary>
        ///     Returns the current level of illumination from this celestial object in Lux at the specified geography. Does not
        ///     include weather effects.
        /// </summary>
        /// <param name="geography"></param>
        /// <returns></returns>
        double CurrentIllumination(GeographicCoordinate geography);

        CelestialInformation CurrentPosition(GeographicCoordinate geography);

        CelestialInformation ReturnNewCelestialInformation(ILocation location, CelestialInformation celestialStatus,
            GeographicCoordinate coordinate);

        string Describe(CelestialInformation information);

        bool CelestialAngleIsUsedToDetermineTimeOfDay { get; }
        TimeOfDay CurrentTimeOfDay(GeographicCoordinate geography);
    }
}