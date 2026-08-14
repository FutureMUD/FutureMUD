#nullable enable

using MudSharp.Events;
using MudSharp.Form.Audio;
using MudSharp.Framework;
using MudSharp.Character;

namespace MudSharp.Construction;

internal static class NoiseEmission
{
	public static bool RaiseEvent(
		ICell origin,
		IPerceiver source,
		AudioVolume volume,
		string noiseType,
		string audioText)
	{
		if (volume == AudioVolume.Silent)
		{
			return false;
		}

		origin.HandleEvent(
			EventType.NoiseEmitted,
			origin,
			source,
			(int)volume,
			string.IsNullOrWhiteSpace(noiseType) ? "sound" : noiseType.Trim(),
			audioText);
		return true;
	}

	public static void RaiseReceivedEvent(
		ICharacter listener,
		ICell origin,
		IPerceiver source,
		AudioVolume receivedVolume,
		Proximity proximity,
		string noiseType,
		string direction,
		string audioText)
	{
		listener.HandleEvent(
			EventType.NoiseReceived,
			listener,
			origin,
			source,
			(int)receivedVolume,
			(int)proximity,
			string.IsNullOrWhiteSpace(noiseType) ? "sound" : noiseType.Trim(),
			direction,
			audioText);
	}
}
