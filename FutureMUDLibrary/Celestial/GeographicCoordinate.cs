using System;

namespace MudSharp.Celestial {
    public class GeographicCoordinate {
        protected double _elevation;
        protected double _latitude;

        protected double _longitude;

        protected double _radius;

        public GeographicCoordinate(double latitude, double longitude, double elevation, double radius) {
            Latitude = latitude;
            Longitude = longitude;
            Elevation = elevation;
            Radius = radius;
        }

        public double Latitude
        {
            get { return _latitude; }
            protected set { _latitude = value; }
        }

        public double Longitude
        {
            get { return _longitude; }
            protected set { _longitude = value; }
        }

        public double Elevation
        {
            get { return _elevation; }
            protected set { _elevation = value; }
        }

        public double Radius
        {
            get { return _radius; }
            protected set { _radius = value; }
        }

        /// <summary>
        ///     Determines the great-sphere distance between two points ("as the crow flies") in base length units
        /// </summary>
        /// <param name="other">The other geographical coordinate</param>
        /// <returns>The number of base length units between the two points</returns>
        public double DistanceTo(GeographicCoordinate other) {
            var a = Math.Sin(Math.Abs(Latitude - other.Latitude)/2)*Math.Sin(Math.Abs(Latitude - other.Latitude)/2) +
                    Math.Cos(Latitude)*Math.Cos(other.Latitude)*Math.Sin(Math.Abs(Longitude - other.Longitude)/2)*
                    Math.Sin(Math.Abs(Longitude - other.Longitude)/2);
            var c = 2*Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return Radius*c;
        }
    }
}