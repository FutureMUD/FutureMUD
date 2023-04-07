using System;

namespace MudSharp.Effects.Interfaces {
    public interface ILearningFatigueEffect : IEffectSubtype {
        DateTime BlockUntil { get; set; }
        int FatigueDegrees { get; set; }
    }
}