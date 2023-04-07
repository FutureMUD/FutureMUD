using MudSharp.Framework;
using MudSharp.Health;

namespace MudSharp.Form.Material {
    public interface IFluid : IMaterial {
        ANSIColour DisplayColour { get; }
        double Viscosity { get; }
        double SmellIntensity { get; }
        string SmellText { get; }
        string VagueSmellText { get; }
        IDrug Drug { get; }
        double DrugGramsPerUnitVolume { get; }
    }
}