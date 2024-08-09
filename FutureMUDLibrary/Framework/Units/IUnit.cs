using System.Collections.Generic;

namespace MudSharp.Framework.Units
{
    public enum UnitType {
        Mass = 0,
        Length = 1,
        FluidVolume = 2,
        Area = 3,
        Volume = 4,
        /// <summary>
        /// Temperature is used to display ABSOLUTE temperatures, i.e. what you would read off of a thermometer
        /// </summary>
        Temperature = 5,
        /// <summary>
        /// TemperatureDelta is used to display differences in temperatures, i.e. the heat is increased by 5 degrees.
        /// </summary>
        TemperatureDelta = 6,
        Force = 7,
        Stress = 8,
        BMI = 9
    }

    public interface IUnit
    {
        /// <summary>
        ///     The name of this unit
        /// </summary>
        string Name { get; }

        string PrimaryAbbreviation { get; }

        /// <summary>
        ///     A series of acceptable abbreviations that players may use for this unit when entering quantities
        /// </summary>
        IEnumerable<string> Abbreviations { get; }

        /// <summary>
        ///     The ratio of 1 of this unit to the base unit for this unit of measure
        /// </summary>
        double MultiplierFromBase { get; }

        /// <summary>
        ///     A flat offset applied before the multiplier
        /// </summary>
        double PreMultiplierOffsetFrombase { get; }

        /// <summary>
        ///     A flat offset applied after the multiplier
        /// </summary>
        double PostMultiplierOffsetFrombase { get; }

        /// <summary>
        ///     The fundamental physical property which this unit of measure represents
        /// </summary>
        UnitType Type { get; }

        /// <summary>
        ///     Whether or not this unit should be considered when asked to describe a quantity
        /// </summary>
        bool DescriberUnit { get; }

        bool SpaceBetween { get; }
        bool LastDescriber { get; set; }
        string System { get; set; }
        bool DefaultUnitForSystem { get; }
    }
}