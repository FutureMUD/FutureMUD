#nullable enable

using MudSharp.Character;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Interfaces;

public enum ExplosiveSignalActivationMode
{
	Edge,
	Level
}

public interface IArmableExplosiveTrigger : IGameItemComponent
{
	bool Armed { get; }
	bool CanArm(ICharacter actor, string argument);
	string WhyCannotArm(ICharacter actor, string argument);
	bool Arm(ICharacter actor, string argument, IEmote? playerEmote = null);
	bool CanDisarm(ICharacter actor);
	string WhyCannotDisarm(ICharacter actor);
	bool Disarm(ICharacter actor, IEmote? playerEmote = null);
}

public interface IPinPullExplosiveTrigger : IGameItemComponent
{
	bool PinPulled { get; }
	bool CanPullPin(ICharacter actor);
	string WhyCannotPullPin(ICharacter actor);
	bool PullPin(ICharacter actor, IEmote? playerEmote = null);
}
