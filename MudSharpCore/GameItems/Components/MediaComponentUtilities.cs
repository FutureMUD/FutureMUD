#nullable enable

using MudSharp.Character;
using MudSharp.Communication.Language;
using MudSharp.Computers;
using MudSharp.Form.Audio;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Components;

internal static class MediaComponentUtilities
{
	public static bool TryParseCapabilities(string text, out MediaCapabilities capabilities)
	{
		capabilities = text.Trim().ToLowerInvariant() switch
		{
			"audio" or "a" => MediaCapabilities.Audio,
			"video" or "v" => MediaCapabilities.Video,
			"av" or "a/v" or "audio/video" or "audiovideo" => MediaCapabilities.Audio | MediaCapabilities.Video,
			_ => MediaCapabilities.None
		};
		return capabilities != MediaCapabilities.None;
	}

	public static string DescribeCapabilities(MediaCapabilities capabilities)
	{
		return capabilities switch
		{
			MediaCapabilities.Audio => "audio",
			MediaCapabilities.Video => "video",
			MediaCapabilities.Audio | MediaCapabilities.Video => "audio/video",
			_ => "no media"
		};
	}

	public static MediaLanguagePayload CreateLanguagePayload(IRecordableLanguageOutput output)
	{
		var info = output.LanguageInfo;
		var source = output.DefaultSource;
		var character = source as ICharacter;
		var speakerId = character?.Id;
		var speakerName = source?.Name ?? "an unknown speaker";
		var speakerGender = (short)(source?.ApparentGender(null).Enum ?? Gender.Indeterminate);
		var preLanguage = output.PreLanguageEmote?.RawText ?? string.Empty;
		var optional = output.OptionalEmote?.RawText ?? string.Empty;
		return info switch
		{
			SpokenLanguageInfo spoken => new MediaLanguagePayload(false, spoken.Language.Id, spoken.Accent?.Id ?? 0L,
				spoken.RawText, (int)spoken.Volume, (int)spoken.OriginOutcome, speakerId, speakerName, speakerGender,
				preLanguage, optional, (long)RecordedAudioSegment.EstimateDuration(spoken.RawText).TotalMilliseconds),
			SignedLanguageInfo signed => new MediaLanguagePayload(true, signed.Language.Id, signed.Variety?.Id ?? 0L,
				signed.RawText, 0, (int)signed.OriginOutcome, speakerId, speakerName, speakerGender, preLanguage,
				optional),
			_ => new MediaLanguagePayload(info.Form == LanguageForm.Signed, info.Language.Id, 0L, info.RawText, 0,
				(int)info.OriginOutcome, speakerId, speakerName, speakerGender, preLanguage, optional)
		};
	}

	public static bool IsAudible(IOutput output)
	{
		return output is AudioOutput || output.Flags.HasFlag(OutputFlags.PurelyAudible) ||
		       output is IRecordableLanguageOutput { LanguageInfo.Form: LanguageForm.Spoken };
	}

	public static bool IsVisual(IOutput output)
	{
		return output.Flags.HasFlag(OutputFlags.PurelyVisual) ||
		       output is IRecordableLanguageOutput { LanguageInfo.Form: LanguageForm.Signed } ||
		       output is not AudioOutput;
	}

	public static AudioVolume? GetAudioVolume(IOutput output)
	{
		return output switch
		{
			AudioOutput audio => audio.Volume,
			IRecordableLanguageOutput { LanguageInfo: SpokenLanguageInfo spoken } => spoken.Volume,
			_ => null
		};
	}

	public static AudioVolume GetAudioVolume(MediaPacket packet)
	{
		var rawVolume = packet.Payload switch
		{
			MediaLanguagePayload { IsSigned: false } language => language.Volume,
			MediaTextPayload { IsAudible: true, Volume: { } volume } => volume,
			_ => (int)AudioVolume.Decent
		};
		return Enum.IsDefined(typeof(AudioVolume), rawVolume)
			? (AudioVolume)rawVolume
			: AudioVolume.Decent;
	}

	public static AudioVolume ScaleAudioVolume(AudioVolume source, AudioVolume output)
	{
		if (source == AudioVolume.Silent || output == AudioVolume.Silent)
		{
			return AudioVolume.Silent;
		}

		var scaled = (int)source + (int)output - (int)AudioVolume.Decent;
		return (AudioVolume)Math.Clamp(scaled, (int)AudioVolume.Faint, (int)AudioVolume.DangerouslyLoud);
	}

	public static MediaPacket ApplyOutputVolume(MediaPacket packet, AudioVolume output)
	{
		if (!packet.Capabilities.HasFlag(MediaCapabilities.Audio))
		{
			return packet;
		}

		var volume = ScaleAudioVolume(GetAudioVolume(packet), output);
		var payload = packet.Payload switch
		{
			MediaLanguagePayload { IsSigned: false } language => language with { Volume = (int)volume },
			MediaTextPayload { IsAudible: true } text => text with { Volume = (int)volume },
			_ => packet.Payload
		};
		var capabilities = volume == AudioVolume.Silent
			? packet.Capabilities & ~MediaCapabilities.Audio
			: packet.Capabilities;
		return packet with { Capabilities = capabilities, Payload = payload };
	}

	public static bool IsLoudFeedbackLoop(MediaPacket packet, MediaEndpointAddress captureEndpoint)
	{
		return packet.Capabilities.HasFlag(MediaCapabilities.Audio) &&
		       GetAudioVolume(packet) >= AudioVolume.Loud &&
		       (packet.Source == captureEndpoint || packet.HasVisited(captureEndpoint));
	}
}
