#nullable enable

using MudSharp.Character;
using MudSharp.Communication.Language;
using MudSharp.Computers;
using MudSharp.Form.Audio;
using MudSharp.RPG.Checks;

namespace MudSharp.PerceptionEngine.Outputs;

/// <summary>
/// Presents a typed media packet without flattening language semantics. The recipient's own language and accent
/// knowledge is evaluated at playback time, not at record time.
/// </summary>
public sealed class MediaPlaybackOutput : Output, IMediaPacketOutput
{
	private readonly IFuturemud _gameworld;
	private readonly IPerceiver _presentationSource;

	public MediaPlaybackOutput(IFuturemud gameworld, IPerceiver presentationSource, MediaPacket mediaPacket)
		: base(OutputVisibility.Normal)
	{
		_gameworld = gameworld;
		_presentationSource = presentationSource;
		MediaPacket = mediaPacket;
	}

	public MediaPacket MediaPacket { get; }

	public override string RawString => MediaPacket.Payload switch
	{
		MediaTextPayload text => text.Text,
		MediaScenePayload scene => scene.CanonicalScene,
		MediaCrimePayload => string.Empty,
		MediaLanguagePayload language => $"{language.SpeakerName} communicates in a recorded language.",
		_ => "A media playback event occurs."
	};

	public override string ParseFor(IPerceiver perceiver)
	{
		if (MediaPacket.Payload is MediaLanguagePayload language)
		{
			var output = BuildLanguageOutput(language);
			return output?.ParseFor(perceiver) ??
			       $"{language.SpeakerName.Proper()} communicates in a language that the playback device cannot reproduce.";
		}

		return RawString;
	}

	public override bool ShouldSee(IPerceiver perceiver)
	{
		if (!base.ShouldSee(perceiver))
		{
			return false;
		}

		var carriesAudio = MediaPacket.Capabilities.HasFlag(MediaCapabilities.Audio);
		var carriesVideo = MediaPacket.Capabilities.HasFlag(MediaCapabilities.Video);
		return (carriesAudio && perceiver.CanHear(_presentationSource)) ||
		       (carriesVideo && perceiver.CanSee(_presentationSource));
	}

	private LanguageOutput? BuildLanguageOutput(MediaLanguagePayload payload)
	{
		var origin = payload.SpeakerCharacterId is { } characterId
			? _gameworld.Characters.Get(characterId) as IPerceivable ?? _presentationSource
			: _presentationSource;
		var preamble = new Emote("@ play|plays back a recorded voice", _presentationSource, _presentationSource);
		if (payload.IsSigned)
		{
			var language = _gameworld.SignedLanguages.Get(payload.LanguageId);
			if (language is null)
			{
				return null;
			}

			var variety = language.Varieties.FirstOrDefault(x => x.Id == payload.AccentOrVarietyId);
			var info = new SignedLanguageInfo(language, variety, payload.RawText, (Outcome)payload.Outcome, origin,
				null, _presentationSource);
			return new LanguageOutput(preamble, info, null, flags: OutputFlags.PurelyVisual);
		}

		var spokenLanguage = _gameworld.Languages.Get(payload.LanguageId);
		var accent = _gameworld.Accents.Get(payload.AccentOrVarietyId);
		if (spokenLanguage is null || accent is null)
		{
			return null;
		}

		var volume = Enum.IsDefined(typeof(AudioVolume), payload.Volume)
			? (AudioVolume)payload.Volume
			: AudioVolume.Decent;
		var spoken = new SpokenLanguageInfo(spokenLanguage, accent, volume, payload.RawText,
			(Outcome)payload.Outcome, origin, null, _presentationSource);
		return new LanguageOutput(preamble, spoken, null, flags: OutputFlags.PurelyAudible);
	}
}
