#nullable enable


using MudSharp.Effects;
using MudSharp.Effects.Concrete;

namespace MudSharp.Magic;

public static class PsionicTrafficHelper
{
	private static readonly string[] BlockedCommandRoots =
	[
		"quit",
		"suicide",
		"password",
		"passwd",
		"account",
		"delete",
		"@",
		"admin",
		"staff",
		"force",
		"shutdown",
		"reboot"
	];

	public static bool IsBlockedCommandRoot(string text)
	{
		var stack = new StringStack(text);
		var root = stack.PopSpeech();
		return !string.IsNullOrWhiteSpace(root) && BlockedCommandRoots.Any(x => x.EqualTo(root));
	}

	public static bool CanReceiveInvoluntaryMentalTraffic(ICharacter target)
	{
		return !target.AffectedBy<IIgnoreForceEffect>();
	}

	public static bool CrossesDome(ICharacter source, ICharacter target)
	{
		if (source.Location is null || target.Location is null || source.Location == target.Location)
		{
			return false;
		}

		return IsWardRoom(source) || IsWardRoom(target);
	}

	public static bool CanSendPsionicTraffic(ICharacter source, ICharacter target)
	{
		return CanReceiveInvoluntaryMentalTraffic(target) && !CrossesDome(source, target) &&
		       !target.EffectsOfType<MindBarrierEffect>().Any(x => x.Applies(source));
	}

	private static bool IsWardRoom(ICharacter character)
	{
		return character.Location?.Characters
			.Any(x => x.EffectsOfType<IPsionicDomeEffect>().Any(dome => dome.AppliesTo(character))) == true;
	}

	public static string SourceDescription(ICharacter source, ICharacter observer, IMagicSchool school)
	{
		var concealment = source.EffectsOfType<IMindContactConcealmentEffect>()
		                        .FirstOrDefault(x => x.ConcealsIdentityFrom(source, observer, school));
		return concealment?.UnknownIdentityDescription.ColourCharacter() ??
		       source.HowSeen(observer, flags: PerceiveIgnoreFlags.IgnoreConsciousness);
	}

	public static void Audit(ICharacter source, ICharacter target, string action, string payload)
	{
		source.Gameworld.SystemMessage(
			$"{source.PersonalName.GetName(MudSharp.Character.Name.NameStyle.SimpleFull)} psionically {action} {target.PersonalName.GetName(MudSharp.Character.Name.NameStyle.SimpleFull)}: {payload}",
			true
		);
	}

	public static void DeliverEmotion(ICharacter source, ICharacter target, IMagicSchool school, string emotion,
		bool notifySource = true)
	{
		var cleanEmotion = emotion.Sanitise().NormaliseSpacing().Fullstop();
		if (notifySource)
		{
			source.OutputHandler.Send(
				PsionicPowerEmotes.FormatShared("EmotionSourceEcho", target.HowSeen(source, flags: PerceiveIgnoreFlags.IgnoreConsciousness), cleanEmotion.ColourCommand()));
		}

		target.OutputHandler.Send(
			PsionicPowerEmotes.FormatShared("EmotionTargetEcho", cleanEmotion.ColourCommand()));
		Audit(source, target, "projected emotion into", cleanEmotion.RawText());

		foreach (var listener in GetMentalListeners(source, target, showFeels: true))
		{
			var thinkerDescription = ListenerDescription(target, listener, school, showFeels: true);
			listener.OutputHandler.Send(PsionicPowerEmotes.FormatShared("EmotionListenerEcho", thinkerDescription, cleanEmotion));
		}
	}

	public static void DeliverThought(ICharacter source, ICharacter target, IMagicSchool school, string thought)
	{
		var cleanThought = thought.Sanitise().NormaliseSpacing().ProperSentences().Fullstop();
		source.OutputHandler.Send(
			PsionicPowerEmotes.FormatShared("ThoughtSourceEcho", target.HowSeen(source, flags: PerceiveIgnoreFlags.IgnoreConsciousness), cleanThought));
		target.OutputHandler.Send(
			PsionicPowerEmotes.FormatShared("ThoughtTargetEcho", cleanThought));
		Audit(source, target, "suggested thought to", cleanThought.RawText());

		foreach (var listener in GetMentalListeners(source, target, showThinks: true))
		{
			var thinkerDescription = ListenerDescription(target, listener, school, showThinks: true);
			listener.OutputHandler.Send(PsionicPowerEmotes.FormatShared("ThoughtListenerEcho", thinkerDescription, cleanThought));
		}
	}

	private static IEnumerable<ICharacter> GetMentalListeners(ICharacter source, ICharacter target, bool showThinks = false,
		bool showFeels = false)
	{
		return source.Gameworld.Characters
		             .Where(x => x != source && x != target)
		             // Mental perceptions do not cross an active barrier or a room ward.
		             // Builder-authored connected links remain available to their endpoints.
		             .Where(x => PassiveSensingAllowed(target, x))
		             .Where(x => x.EffectsOfType<ITelepathyEffect>()
		                          .Any(y => y.Applies(target) &&
		                                    ((!showThinks || y.ShowThinks) && (!showFeels || y.ShowFeels))));
	}

	private static bool PassiveSensingAllowed(ICharacter thinker, ICharacter listener)
	{
		if (thinker.EffectsOfType<MindBarrierEffect>().Any(x => x.Applies(listener)))
		{
			return false;
		}

		// A dome suppresses passive mental perception anywhere inside the ward and
		// prevents sensing across its boundary. This is a native deterministic mapping
		// of the source's probabilistic skill checks.
		var thinkerDome = thinker.Location?.Characters.SelectMany(x => x.EffectsOfType<IPsionicDomeEffect>())
			.FirstOrDefault(x => x.AppliesTo(thinker));
		var listenerDome = listener.Location?.Characters.SelectMany(x => x.EffectsOfType<IPsionicDomeEffect>())
			.FirstOrDefault(x => x.AppliesTo(listener));
		return thinkerDome is null && listenerDome is null;
	}

	private static string ListenerDescription(ICharacter thinker, ICharacter listener, IMagicSchool school,
		bool showThinks = false, bool showFeels = false)
	{
		var effects = listener.EffectsOfType<ITelepathyEffect>()
		                      .Where(x => x.Applies(thinker) &&
		                                  ((!showThinks || x.ShowThinks) && (!showFeels || x.ShowFeels)))
		                      .ToList();
		var concealment = thinker.EffectsOfType<IMindContactConcealmentEffect>()
		                         .FirstOrDefault(x => effects.OfType<IMagicEffect>()
		                                                    .Any(y => x.ConcealsIdentityFrom(thinker, listener, y.School)));
		if (concealment is not null)
		{
			return concealment.UnknownIdentityDescription.ColourCharacter();
		}

		return effects.Any(x => x.ShowDescription(thinker))
			? thinker.HowSeen(listener, true)
			: "Someone";
	}
}
