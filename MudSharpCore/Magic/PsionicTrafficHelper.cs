#nullable enable

using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using System;
using System.Collections.Generic;
using System.Linq;

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
				$"You push a feeling into {target.HowSeen(source, flags: PerceiveIgnoreFlags.IgnoreConsciousness)}'s mind: {cleanEmotion.ColourCommand()}");
		}

		target.OutputHandler.Send(
			$"A feeling that is not your own settles into your mind: {cleanEmotion.ColourCommand()}");
		Audit(source, target, "projected emotion into", cleanEmotion.RawText());

		foreach (var listener in GetMentalListeners(source, target, showFeels: true))
		{
			var thinkerDescription = ListenerDescription(target, listener, school, showFeels: true);
			listener.OutputHandler.Send($"{thinkerDescription} feels {cleanEmotion}");
		}
	}

	public static void DeliverThought(ICharacter source, ICharacter target, IMagicSchool school, string thought)
	{
		var cleanThought = thought.Sanitise().NormaliseSpacing().ProperSentences().Fullstop();
		source.OutputHandler.Send(
			$"You press a thought into {target.HowSeen(source, flags: PerceiveIgnoreFlags.IgnoreConsciousness)}'s mind:\n\t\"{cleanThought}\"");
		target.OutputHandler.Send(
			$"A thought that is not your own surfaces in your mind:\n\t\"{cleanThought}\"");
		Audit(source, target, "suggested thought to", cleanThought.RawText());

		foreach (var listener in GetMentalListeners(source, target, showThinks: true))
		{
			var thinkerDescription = ListenerDescription(target, listener, school, showThinks: true);
			listener.OutputHandler.Send($"{thinkerDescription} thinks,\n\t\"{cleanThought}\"");
		}
	}

	private static IEnumerable<ICharacter> GetMentalListeners(ICharacter source, ICharacter target, bool showThinks = false,
		bool showFeels = false)
	{
		return source.Gameworld.Characters
		             .Where(x => x != source && x != target)
		             .Where(x => x.EffectsOfType<ITelepathyEffect>()
		                          .Any(y => y.Applies(target) &&
		                                    ((!showThinks || y.ShowThinks) && (!showFeels || y.ShowFeels))));
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
