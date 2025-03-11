using MudSharp.Framework;
using MudSharp.GameItems;
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
        bool CountsAs(IFluid other);
        ItemQuality CountAsQuality(IFluid other);
        double CountsAsMultiplier(IFluid other);
    }
}