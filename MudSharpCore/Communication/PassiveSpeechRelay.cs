#nullable enable

using MudSharp.Body;
using MudSharp.Communication.Language;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Form.Audio;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.PerceptionEngine.Outputs;

namespace MudSharp.Communication;

/// <summary>Opt-in, checked spoken-language awareness through one authored exit.</summary>
public static class PassiveSpeechRelay
{
	public static void Relay(IBody speaker, SpokenLanguageInfo speech)
		=> Relay(speaker, speech, speaker.Location);

	public static void Relay(IBody speaker, SpokenLanguageInfo speech, IRoom origin)
	{
		if (origin is null || speaker.Location != origin || speech.Volume == AudioVolume.Silent) return;
		var seen = new HashSet<ICharacter>(ReferenceEqualityComparer.Instance);
		foreach (var exit in origin.ExitsFor(speaker.Actor).Where(x => x.Destination != origin)
			.OrderBy(x => x.Exit.Door?.IsOpen == false).ToArray())
		foreach (var listener in exit.Destination.Characters.ToArray())
		{
			if (!seen.Add(listener)) continue;
			TryRelay(speaker, listener, exit, speech);
		}
	}

	internal static bool TryRelay(IBody speaker, ICharacter listener, IRoomExit exit, SpokenLanguageInfo speech)
	{
		bool Eligible() => exit.Origin != exit.Destination && speaker.Location == exit.Origin && listener.Location == exit.Destination &&
			listener.RoomLayer == speaker.RoomLayer && listener != speaker.Actor &&
			listener.State.IsConscious() && !listener.State.HasFlag(CharacterState.Sleeping) && listener.Combat is null &&
			listener.CombinedEffectsOfType<IPassiveListeningEffect>()?.Any(x => x.Applies()) == true;
		if (!Eligible() || speech.Volume == AudioVolume.Silent) return false;
		var received = (AudioVolume)Math.Max((int)AudioVolume.Faint, (int)speech.Volume - 1);
		var obstacle = exit.Exit.Door?.IsOpen == false ? 1 : 0;
		if (!AudioPerception.CanHear(listener, speaker.Actor, received, Proximity.VeryDistant, obstacle) || !Eligible()) return false;
		var output = new LanguageOutput(new Emote($"You hear a voice {exit.InboundDirectionSuffix} say", speaker.Actor),
			new SpokenLanguageInfo(speech, received, speaker.Actor), null, flags: OutputFlags.PurelyAudible);
		listener.OutputHandler.Send(output);
		// NPC output handlers deliberately have no terminal and return false from Send.
		// Hearing/event eligibility is independent of whether terminal output was delivered.
		if (!Eligible()) return false;
		listener.HandleEvent(EventType.CharacterSpeaksNearbyWitness, speaker.Actor, speech.Target, listener,
			received, speech.Language, speech.Accent, speech.RawText);
		return true;
	}
}
