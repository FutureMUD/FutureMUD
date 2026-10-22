using MudSharp.Body.CommunicationStrategies;
using MudSharp.Communication.Language;
using MudSharp.Form.Audio;

#nullable enable
namespace MudSharp.Magic.Casting;

public sealed partial class MagicCastingService
{
	private sealed record FormulaMatch(MagicSpell Spell, int Grade, bool Overreach, string Targets, string? Via,
		MagicCastingMode Mode = MagicCastingMode.Manifest, long? CapabilityId = null);
	private sealed record FormulaCandidate(MagicSpell Spell, ControlledSpellIncantation Profile, IMagicCastingCapability? Capability);

	public MagicCastingResult? CastFormula(ICharacter actor, string formula, string method = "Say", long? schoolId = null,
		MagicCastingSpeech? speech = null)
	{
		try
		{
			var match = MatchFormula(actor, formula, schoolId, out var explicitFormula, out var reason);
			if (match is null) return speech is not null && !explicitFormula ? null : new(MagicCastingStatus.Refused, reason);
			var routes = _world.MagicCapabilities.OfType<IMagicCastingCapability>().Where(x => x.CastingPolicy is not null &&
				(schoolId is null || x.School.Id == schoolId) && actor.Capabilities.Any(c => c.Id == x.Id) &&
				(match.CapabilityId is null || x.Id == match.CapabilityId) &&
				x.CastingPolicy.Admissions.Any(a => a.SpellId == match.Spell.Id) &&
				(match.Via is null || x.Name.EqualTo(match.Via) || x.Id.ToString() == match.Via)).ToArray();
			if (routes.Length != 1) return new(MagicCastingStatus.Refused, routes.Length == 0
				? "No current explicitly admitted route matches that formula." : "Multiple routes admit that formula. Select via <capability>; energy does not select a route.");
			return Cast(new(actor, routes[0].Id, match.Spell.Id, match.Grade, match.Overreach, match.Targets,
				match.Mode, Method: method, OriginId: speech?.OriginId, Speech: speech));
		}
		catch (Exception ex) { return new(MagicCastingStatus.Refused, $"Formula casting refused: {ex.Message}"); }
	}

	private FormulaMatch? MatchFormula(ICharacter actor, string text, long? schoolId, out bool explicitFormula, out string reason)
	{
		reason = "Formula casting is unavailable without a complete authored five-category formula or POWER/spell alias, followed by [overreach] on <target> [via <capability>].";
		explicitFormula = false;
		if (text.Length > 4096) { explicitFormula = true; reason = "The formula exceeds 4096 characters."; return null; }
		var languageId = actor.CurrentLanguage?.Id;
		var spells = _world.MagicSpells.OfType<MagicSpell>().ToArray();
		var candidates = spells.Where(x => x.GradeProfile?.Incantation is not null)
			.Select(x => new FormulaCandidate(x, x.GradeProfile!.Incantation!, null)).ToList();
		foreach (var capability in actor.Capabilities.OfType<IMagicCastingCapability>().Where(x =>
			x.CastingPolicy is { Enabled: true, Patron.Incantations: not null } && (schoolId is null || x.School.Id == schoolId)))
			foreach (var spell in spells.Where(x => x.GradeProfile is { Incantation: null } && capability.CastingPolicy!.Admissions.Any(a => a.SpellId == x.Id)))
				if (PatronMagicService.Incantation(capability, spell) is { } profile) candidates.Add(new(spell, profile, capability));
		candidates = candidates.Where(x => x.Profile.LanguageId == languageId).ToList();
		var args = new StringStack(text);
		var area = args.PeekSpeech().EqualTo("area");
		if (area) args.PopSpeech();
		List<string> words = [];
		while (!args.IsFinished && !args.PeekSpeech().EqualTo("on")) words.Add(args.PopSpeech());
		explicitFormula = words.Any(x => ArmageddonPowerWords.Grade(x) is not null) ||
			words.Count > 0 && candidates.Any(x => x.Profile.CategoryWords.Concat(x.Profile.Aliases).Any(w => w.EqualTo(words[0])));
		explicitFormula |= area;
		if (args.IsFinished) return null;
		args.PopSpeech();
		var overreach = words.Count > 0 && words[^1].EqualTo("overreach");
		if (overreach) words.RemoveAt(words.Count - 1);
		var powers = words.Select(ArmageddonPowerWords.Grade).Where(x => x.HasValue).ToArray();
		if (powers.Length != 1 || words.Count is not (2 or 5)) return null;
		var grade = powers[0]!.Value;
		var others = words.Where(x => ArmageddonPowerWords.Grade(x) is null).ToArray();
		List<string> targets = []; string? via = null;
		while (!args.IsFinished)
		{
			var token = args.PopSpeech();
			if (token.EqualTo("via"))
			{
				// Native speech removes quote wrappers. The entire final remainder still names one exact capability.
				via = args.SafeRemainingArgument;
				if (!string.IsNullOrWhiteSpace(via)) break;
				reason = "The final via argument must name one capability."; return null;
			}
			if (token.EqualToAny("area", "quiet", "practice", "overreach", "on"))
			{ reason = "That formula mode combination is not explicitly enabled."; return null; }
			targets.Add(token.Contains(' ') ? token.DoubleQuotes() : token);
		}
		if (targets.Count == 0) { reason = "Specify on <target>; use self for a self-targeted spell."; return null; }
		var selectedVia = via;
		var vocabulary = candidates.Where(x => words.Count == 2 ? x.Profile.Aliases.Any(a => a.EqualTo(others[0])) :
			others.Distinct(StringComparer.OrdinalIgnoreCase).Count() == 4 && x.Profile.CategoryWords.All(w => others.Any(o => o.EqualTo(w)))).ToArray();
		var selectedFallbacks = selectedVia is null ? [] : vocabulary.Where(x => x.Capability is { } c &&
			(c.Name.EqualTo(selectedVia) || c.Id.ToString() == selectedVia)).ToArray();
		// Explicit route vocabulary may share words with a canonical spell outside that route.
		// Retain the existing canonical-only ambiguity rule when no route fallback is selected.
		var matches = vocabulary.Where(x => x.Capability is { } c
			? selectedVia is null || c.Name.EqualTo(selectedVia) || c.Id.ToString() == selectedVia
			: selectedFallbacks.Length == 0 || selectedFallbacks.Any(f => f.Capability!.CastingPolicy!.Admissions.Any(a => a.SpellId == x.Spell.Id))).ToArray();
		if (matches.Length != 1) { reason = matches.Length == 0 ? "No authored formula matches those words in your selected language." : "That formula is ambiguous in the selected language."; return null; }
		return new(matches[0].Spell, grade, overreach, string.Join(" ", targets), via, area ? MagicCastingMode.Area : MagicCastingMode.Manifest, matches[0].Capability?.Id);
	}

	private MagicCastingDelivery? ResolveDelivery(MagicCastingIntent intent, MagicSpell spell, IMagicCastingCapability capability)
	{
		if (intent.Mode == MagicCastingMode.Practice)
		{
			if (!intent.Method.EqualTo("Say") || intent.Speech is not null)
				throw new InvalidOperationException("That speech/practice combination is not explicitly enabled.");
			return null;
		}
		if (PatronMagicService.Incantation(capability, spell) is not { } profile)
		{
			if (!intent.Method.EqualTo("Say") || intent.Speech is not null) throw new InvalidOperationException("Formula and quiet casting are unavailable until this spell has an authored incantation policy.");
			return null;
		}
		var methods = intent.Mode == MagicCastingMode.Area ? spell.GradeProfile.Area!.Deliveries : profile.Deliveries;
		if (RouteDelivery(capability, intent.Mode, methods, intent.Method) is not { } policy)
			throw new InvalidOperationException("That native speech method is not explicitly enabled for this spell.");
		if (intent.Actor.CurrentLanguage?.Id != profile.LanguageId || !intent.Actor.Languages.Any(x => x.Id == profile.LanguageId))
			throw new InvalidOperationException("Select and know this spell's bound native spoken language before casting.");
		var message = $"{(intent.Mode == MagicCastingMode.Area ? "area " : "")}{ArmageddonPowerWords.ForGrade(intent.Grade)} {string.Join(" ", profile.CategoryWords)}{(intent.Overreach ? " overreach" : "")} on {intent.Targets} via {capability.Name.DoubleQuotes()}";
		if (intent.Speech is { } speech)
		{
			if (intent.OriginId != speech.OriginId || speech.LanguageId != profile.LanguageId || speech.Volume != policy.Volume ||
				!speech.Method.EqualTo(policy.Method)) throw new InvalidOperationException("The originating utterance does not match the native method, volume or language.");
			var parsed = MatchFormula(intent.Actor, speech.FormulaText ?? speech.Message, null, out _, out var reason);
			if (parsed is null || parsed.Mode != intent.Mode || parsed.Spell.Id != intent.SpellId || parsed.Grade != intent.Grade || parsed.Overreach != intent.Overreach ||
				parsed.CapabilityId is { } parsedCapability && parsedCapability != capability.Id ||
				parsed.Targets != intent.Targets || parsed.Via is not null && !parsed.Via.EqualTo(capability.Name) && parsed.Via != capability.Id.ToString())
				throw new InvalidOperationException("The originating utterance does not match this casting intent: " + reason);
			message = speech.Message;
		}
		return new(policy.Method, policy.Volume, profile.LanguageId, policy.EnergyMultiplier, policy.DifficultySteps, message, intent.Speech is not null);
	}

	private static string? SpeechEligibility(ICharacter actor, AudioVolume volume)
	{
		var body = actor.Body;
		if (body.Communications?.CanVocalise(body, volume) != true ||
			body.CombinedEffectsOfType<ISilencedEffect>().Any(x => x.Applies()) ||
			body.Communications is HumanoidCommunicationStrategy humanoid && humanoid.IsGagged(body))
			return "You cannot speak the required words at that native volume in your current body.";
		return null;
	}

	private static void EmitIncantation(ICharacter actor, Guid operationId, MagicCastingDelivery? delivery)
	{
		if (delivery is null || delivery.UsesOriginalSpeech) return;
		using var origin = MagicSpeechContext.Generated(actor, operationId, delivery.Method, delivery.Incantation);
		switch (delivery.Method.ToLowerInvariant())
		{
			case "say": actor.Body.Say(null, delivery.Incantation); break;
			case "whisper": actor.Body.Whisper(null, delivery.Incantation); break;
			case "talk": actor.Body.Talk(null, delivery.Incantation); break;
			case "loudsay": actor.Body.LoudSay(null, delivery.Incantation); break;
			case "yell": actor.Body.Yell(null, delivery.Incantation); break;
			case "shout": actor.Body.Shout(null, delivery.Incantation); break;
			case "sing": actor.Body.Sing(null, delivery.Incantation); break;
			default: throw new InvalidOperationException("Unsupported native speech method.");
		}
		if (!origin.Emitted) throw new InvalidOperationException("The native communication method did not emit usable incantation words.");
	}
}
