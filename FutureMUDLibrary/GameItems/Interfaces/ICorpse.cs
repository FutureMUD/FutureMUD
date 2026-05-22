using MudSharp.Character;
using System;

namespace MudSharp.GameItems.Interfaces
{
	public enum BodyRemainsContext
	{
		FinalCharacterDeath = 0,
		AbandonedBody = 1,
		SleeveDeath = 2,
		SpentClone = 3,
		Other = 99
	}

    public enum DecayState
    {
        Fresh,
        Recent,
        Decaying,
        Decayed,
        HeavilyDecayed,
        Skeletal
    }

	public interface IBodyRemains : IButcherable, IOverrideItemWoundBehaviour, IHaveABody
	{
		BodyRemainsContext RemainsContext { get; set; }
		bool RepresentsFinalCharacterDeath { get; }
	}

    public interface ICorpse : IBodyRemains
    {
        double DecayPoints { get; set; }

        DateTime TimeOfDeath { get; }
        bool Skinned { get; set; }
        double EatenWeight { get; set; }
        double RemainingEdibleWeight { get; }
    }
}
