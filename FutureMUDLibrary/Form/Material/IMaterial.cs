using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;

namespace MudSharp.Form.Material {
    public interface IMaterial : IHaveTags, IProgVariable, IEditableItem
    {
        /// <summary>
        ///     Density of the material in kg/m3
        /// </summary>
        double Density { get; }

        bool Organic { get; }

        MaterialType MaterialType { get; }

        MaterialBehaviourType BehaviourType { get; }

        /// <summary>
        ///     The Thermal Conductivity of the material in watts per meter per Kelvin
        /// </summary>
        double ThermalConductivity { get; }

        /// <summary>
        ///     The Electrical Conductivity of the material in 1 / ohm metres
        /// </summary>
        double ElectricalConductivity { get; }

        /// <summary>
        ///     The Heat Capacity of the material expressed per unit mass, in units Joules per Kilogram Kelvin
        /// </summary>
        double SpecificHeatCapacity { get; }
        
        string MaterialDescription { get; }
    }
}