using MudSharp.Form.Material;

namespace MudSharp.Effects.Interfaces {
    public interface ILiquidContaminationEffect : ICleanableEffect {
        LiquidMixture ContaminatingLiquid { get; }
        /// <summary>
        /// Adds liquid to the effect
        /// </summary>
        /// <param name="liquid">The liquid to add in</param>
        void AddLiquid(LiquidMixture liquid);
    }
}