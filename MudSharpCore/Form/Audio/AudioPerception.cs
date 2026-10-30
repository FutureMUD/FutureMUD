#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Form.Audio;

/// <summary>
/// Shared native decision for whether a character notices a received sound.
/// </summary>
public static class AudioPerception
{
	public static bool CanHear(
		ICharacter listener,
		IPerceiver source,
		AudioVolume receivedVolume,
		Proximity proximity)
		=> CanHear(listener, source, receivedVolume, proximity, 0);

	/// <summary>Runs native audio checks with additional bounded difficulty for intervening obstacles.</summary>
	public static bool CanHear(ICharacter listener, IPerceiver source, AudioVolume receivedVolume,
		Proximity proximity, int additionalDifficultyStages)
	{
		ArgumentNullException.ThrowIfNull(listener);
		ArgumentNullException.ThrowIfNull(source);

		if (listener.IsSelf(source))
		{
			return true;
		}

		if (receivedVolume == AudioVolume.Silent || !listener.CanHear(source))
		{
			return false;
		}

		var difficulty = listener.Location.LocalAudioDifficulty(listener, receivedVolume, proximity);
		difficulty = difficulty.StageUp(Math.Max(0, additionalDifficultyStages));
		return listener.Gameworld.GetCheck(CheckType.GenericListenCheck)
		               .Check(listener, difficulty, source)
		               .IsPass();
	}
}
