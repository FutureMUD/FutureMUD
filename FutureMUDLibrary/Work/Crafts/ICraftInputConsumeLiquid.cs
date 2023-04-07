using MudSharp.Form.Material;

namespace MudSharp.Work.Crafts
{
    /// <summary>
    /// This interface guarantees that the craft input will consume a liquid as part of its process, and that its ICraftInputData will be an ICraftInputConsumeLiquidData
    /// </summary>
    public interface ICraftInputConsumeLiquid : ICraftInput
    {
    }

    public interface ICraftInputConsumeLiquidData : ICraftInputData {
        LiquidMixture ConsumedMixture { get; }
    }
}
