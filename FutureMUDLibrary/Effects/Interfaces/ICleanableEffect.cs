using System;
using MudSharp.Form.Material;
using MudSharp.Framework;

namespace MudSharp.Effects.Interfaces {
    public interface ICleanableEffect : IEffectSubtype {
        TimeSpan BaseCleanTime { get; }
        ITag CleaningToolTag { get; }
        ILiquid LiquidRequired { get; }
        double LiquidAmountConsumed { get; set; }
        string EmoteBeginClean { get; }
        string EmoteStopClean { get; }
        string EmoteFinishClean { get; }
        string PromptStatusLine { get; }
        /// <summary>
        /// Performs a small amount of cleaning with a liquid.
        /// </summary>
        /// <param name="liquid">A liquid being used to clean</param>
        /// <param name="amount">The amount of liquid being used</param>
        /// <returns>True if the effect is now totally cleaned</returns>
        bool CleanWithLiquid(LiquidMixture liquid, double amount);
    }
}