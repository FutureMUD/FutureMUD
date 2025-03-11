using MudSharp.GameItems;

namespace MudSharp.Form.Material {
    public interface IGas : IFluid {
        IGas CountsAsGas { get; }
        ItemQuality CountsAsQuality { get; }
        ILiquid LiquidForm { get; }
        double CondensationTemperature { get; }
        bool GasCountAs(IGas otherLiquid);
        IGas Clone(string newName);
    }
}