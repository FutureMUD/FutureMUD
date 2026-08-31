#nullable enable

using MudSharp.Computers;
using MudSharp.Construction;
using MudSharp.Form.Audio;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;

namespace MudSharp.GameItems.Components;

internal static class MediaAudioPresentation
{
	private const string NoiseType = "media playback";
	private const string DistantPlaybackEcho = "You hear audio from an electronic media device {0}.";
	private const string FeedbackEcho = "@ emit|emits a piercing burst of electronic feedback.";
	private const string FeedbackNoiseText = "A piercing burst of electronic feedback sounds here.";

	public static MediaPacket ApplyOutputVolume(MediaPacket packet, AudioVolume outputVolume)
	{
		return MediaComponentUtilities.ApplyOutputVolume(packet, outputVolume);
	}

	public static void EmitPlaybackNoise(IGameItem device, MediaPacket packet)
	{
		if (!packet.Capabilities.HasFlag(MediaCapabilities.Audio))
		{
			return;
		}

		var volume = MediaComponentUtilities.GetAudioVolume(packet);
		foreach (var location in device.TrueLocations.Distinct())
		{
			if (volume >= AudioVolume.Loud)
			{
				location.HandleAudioEcho(DistantPlaybackEcho, volume, device, device.RoomLayer, true, NoiseType);
				continue;
			}

			NoiseEmission.RaiseEvent(location, device, volume, NoiseType, DistantPlaybackEcho);
		}
	}

	public static void EmitFeedback(IGameItem device)
	{
		var output = new AudioOutput(new Emote(FeedbackEcho, device, device), AudioVolume.VeryLoud,
			flags: OutputFlags.PurelyAudible | OutputFlags.IgnoreWatchers);
		device.Handle(output, OutputRange.Local);
		foreach (var location in device.TrueLocations.Distinct())
		{
			NoiseEmission.RaiseEvent(location, device, AudioVolume.VeryLoud, "electronic feedback",
				FeedbackNoiseText);
		}
	}
}
