namespace MudSharp.Celestial {
    public class CelestialInformation {
        public CelestialInformation() {
        }

        public CelestialInformation(ICelestialObject origin, double lastAzimuthAngle, double lastAscentionAngle,
            CelestialMoveDirection direction) {
            Origin = origin;
            LastAzimuthAngle = lastAzimuthAngle;
            LastAscensionAngle = lastAscentionAngle;
            Direction = direction;
        }

        public ICelestialObject Origin { get; protected set; }
        public double LastAzimuthAngle { get; protected set; }
        public double LastAscensionAngle { get; protected set; }
        public CelestialMoveDirection Direction { get; set; }

        #region Overrides of Object

        public override string ToString()
        {
	        return $"{Origin.Name} - Azimuth {LastAzimuthAngle} - Ascension {LastAscensionAngle} - Direction {Direction.Describe()}";
        }

        #endregion
    }
}