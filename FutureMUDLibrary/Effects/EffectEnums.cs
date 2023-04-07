using System;

namespace MudSharp.Effects {
    public enum EffectMergeResponse {
        /// <summary>
        ///     This effect is not affected in any way by the new effect
        /// </summary>
        Irrelevant,

        /// <summary>
        ///     The new effect should be cancelled and the existing effect should stand
        /// </summary>
        CancelNew,

        /// <summary>
        ///     The new effect should stand and the existing effect should be cancelled
        /// </summary>
        CancelExisting,

        /// <summary>
        ///     The new effect should be cancelled, and the existing effect's duration should be increased
        /// </summary>
        AddDuration
    }

    public enum EffectMergePriority {
        Least,
        Low,
        Medium,
        High,
        Highest
    }

    // Todo - this should perhaps belong somewhere else
    [Flags]
    public enum PerceptionTypes : long {
        None = 0,
        VisualRadio = 0x01,
        VisualMicrowave = 0x02,
        VisualInfrared = 0x04,
        VisualLight = 0x80,
        VisualUltraviolet = 0x10,
        VisualXRay = 0x20,
        VisualGammaRay = 0x40,
        VisualUltrasonic = 0x80,
        VisualMagical = 0x100,
        VisualPsychic = 0x200,
        VisualDivine = 0x400,
        Audible = 0x800,
        Smell = 0x1000,
        Taste = 0x2000,
        Touch = 0x4000,
        SenseMundane = 0x8000,
        SenseMagical = 0x10000,
        SensePsychic = 0x20000,
        SenseDivine = 0x40000,
        AudibleLowFreq = 0x80000,
        AudibleHighFreq = 0x100000,

        #region Special Values

        DirectVisual =
            VisualRadio | VisualMicrowave | VisualLight | VisualInfrared | VisualUltrasonic | VisualUltraviolet,

        MundaneVisual =
            VisualLight | VisualMicrowave | VisualInfrared | VisualUltraviolet | VisualRadio | VisualXRay |
            VisualGammaRay | VisualUltrasonic,

        AllVisual = MundaneVisual | VisualMagical | VisualPsychic | VisualDivine,

        AllSense = SenseMundane | SenseMagical | SensePsychic | SenseDivine,

        All = long.MaxValue

        #endregion
    }
}