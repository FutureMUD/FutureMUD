using MudSharp.Framework;

namespace MudSharp.Form.Material {
    public interface ISolid : IMaterial {
        double ImpactFracture { get; }

        double ImpactYield { get; }

        double ImpactStrainAtYield { get; }

        double ShearFracture { get; }

        double ShearYield { get; }

        double ShearStrainAtYield { get; }

        double? HeatDamagePoint { get; }
        /// <summary>
        ///     Ignition temperature of the material in Kelvin
        /// </summary>
        double? IgnitionPoint { get; }

        /// <summary>
        ///     Melting point of the material in Kelvin
        /// </summary>
        double? MeltingPoint { get; }

        /// <summary>
        ///     In GPa
        /// </summary>
        double YoungsModulus { get; }
        ILiquid Solvent { get; }

        double SolventRatio { get; }

        string ResidueSdesc { get; }

        string ResidueDesc { get; }

        ANSIColour ResidueColour { get; }

        double Absorbency { get; }

        ILiquid LiquidForm { get; }

        IGas GasForm { get; }

        ISolid Clone(string newName, string newDescription);
    }
}