using System;
using MudSharp.Character;

namespace MudSharp.GameItems.Interfaces {
    public enum DecayState {
        Fresh,
        Recent,
        Decaying,
        Decayed,
        HeavilyDecayed,
        Skeletal
    }

    public interface ICorpse : IButcherable, IOverrideItemWoundBehaviour, IHaveABody
    {
        double DecayPoints { get; set; }

        DateTime TimeOfDeath { get; }
        bool Skinned { get; set; }
        double EatenWeight { get; set; }
        double RemainingEdibleWeight { get; }
    }
}