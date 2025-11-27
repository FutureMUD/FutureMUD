using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Form.Audio;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.PerceptionEngine.Outputs;

public class AudioOutput : EmoteOutput
{
	private static Dictionary<AudioVolume, Difficulty> _difficultyMap = new()
	{
		{ AudioVolume.Silent, Difficulty.Impossible },
		{ AudioVolume.Faint, Difficulty.VeryHard },
		{ AudioVolume.Quiet, Difficulty.Hard },
		{ AudioVolume.Decent, Difficulty.Easy },
		{ AudioVolume.Loud, Difficulty.VeryEasy },
		{ AudioVolume.VeryLoud, Difficulty.ExtremelyEasy },
		{ AudioVolume.ExtremelyLoud, Difficulty.Trivial },
		{ AudioVolume.DangerouslyLoud, Difficulty.Automatic }
	};

	private AudioVolume _volume;
	public AudioOutput(IEmote emote,
		AudioVolume volume,
		OutputVisibility visibility = OutputVisibility.Normal,
		OutputStyle style = OutputStyle.Normal,
		OutputFlags flags = OutputFlags.Normal) : 
		base(emote, visibility, style, flags)
	{
		_volume = volume;
	}


	public AudioOutput(string emote,
		AudioVolume volume,
		IPerceiver defaultSource,
		bool forceSourceInclusion = false,
		OutputVisibility visibility = OutputVisibility.Normal,
		OutputStyle style = OutputStyle.Normal,
		OutputFlags flags = OutputFlags.Normal) :
		base(emote, defaultSource, forceSourceInclusion, visibility, style, flags)
	{
		_volume = volume;
	}

	public AudioOutput(AudioOutput rhs) : base(rhs)
	{
		{
			_volume = rhs._volume;
		}
	}

	#region Overrides of EmoteOutput

	/// <inheritdoc />
	public override bool ShouldSee(IPerceiver perceiver)
	{
		if (!base.ShouldSee(perceiver))
		{
			return false;
		}

		if (perceiver is not ICharacter ch)
		{
			return true;
		}

		var check = Futuremud.Games.First().GetCheck(CheckType.GenericListenCheck);
		var result = check.Check(ch, _difficultyMap[_volume], DefaultSource);
		return result.IsPass();
	}

	#endregion
}