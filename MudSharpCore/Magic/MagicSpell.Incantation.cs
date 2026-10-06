using System.Globalization;
using MudSharp.Form.Audio;
using MudSharp.Magic.Vancian;

#nullable enable
namespace MudSharp.Magic;

public partial class MagicSpell
{
	private static ControlledSpellIncantation LoadIncantation(XElement root)
	{
		if ((int?)root.Attribute("schema") != 1) throw new FormatException("Unsupported incantation schema.");
		return new((long)root.Attribute("language")!, Required(root, "reach"), Required(root, "element"),
			Required(root, "sphere"), Required(root, "mood"),
			Array.AsReadOnly(root.Elements("Alias").Select(x => x.Value).ToArray()),
			Array.AsReadOnly(root.Elements("Method").Select(x => new ControlledSpellDelivery(Required(x, "name"),
				(AudioVolume)(int)x.Attribute("volume")!, (double)x.Attribute("energy")!, (int)x.Attribute("difficulty")!)).ToArray()),
			Required(root, "provenance"));

		static string Required(XElement element, string name) => (string?)element.Attribute(name) ?? throw new FormatException($"Incantation requires {element.Name}/@{name}.");
	}

	private XElement? SaveIncantation() => GradeProfile?.Incantation is not { } p ? null :
		new("Incantation", new XAttribute("schema", 1), new XAttribute("language", p.LanguageId),
			new XAttribute("reach", p.Reach), new XAttribute("element", p.Element), new XAttribute("sphere", p.Sphere),
			new XAttribute("mood", p.Mood), new XAttribute("provenance", p.VocabularyProvenance),
			p.Aliases.Select(x => new XElement("Alias", x)), p.Deliveries.Select(x => new XElement("Method",
				new XAttribute("name", x.Method), new XAttribute("volume", (int)x.Volume),
				new XAttribute("energy", x.EnergyMultiplier), new XAttribute("difficulty", x.DifficultySteps))));

	private IReadOnlyList<string> IncantationConfigurationErrors(ControlledSpellIncantation? p)
	{
		if (p is null) return [];
		List<string> errors = [];
		if (!SpellTargetCapture.SupportsCompleteSpecification(Trigger)) errors.Add("Incantation: this native trigger has no complete target adapter.");
		var words = p.CategoryWords.Concat(p.Aliases).ToArray();
		if (Gameworld.Languages.Get(p.LanguageId) is null) errors.Add("Incantation: missing native spoken language.");
		if (string.IsNullOrWhiteSpace(p.VocabularyProvenance) || p.VocabularyProvenance.Length > 256)
			errors.Add("Incantation: label the four category words' provenance (up to 256 characters).");
		if (words.Any(x => x.Length is < 1 or > 64 || x.Any(c => !char.IsAsciiLetterOrDigit(c) && c != '-' && c != '_') ||
			ArmageddonPowerWords.Grade(x) is not null || x.EqualToAny("on", "via", "overreach", "area", "quiet", "practice")) ||
			words.Distinct(StringComparer.OrdinalIgnoreCase).Count() != words.Length || p.Aliases.Count > 32)
			errors.Add("Incantation: use distinct single ASCII word tokens, excluding POWER and command keywords; at most 32 aliases.");
		if (p.Deliveries.Count == 0 || p.Deliveries.Select(x => x.Method).Distinct(StringComparer.OrdinalIgnoreCase).Count() != p.Deliveries.Count)
			errors.Add("Incantation: author at least one unique native speech method.");
		foreach (var d in p.Deliveries)
			if (!MagicCastingMethods.TryVolume(d.Method, out var volume) || volume != d.Volume ||
				!double.IsFinite(d.EnergyMultiplier) || d.EnergyMultiplier <= 0 || d.DifficultySteps is < -10 or > 10)
				errors.Add($"Incantation: invalid {d.Method} native volume, positive energy multiplier or difficulty steps (-10..10).");
		return errors.AsReadOnly();
	}

	private void AppendIncantationShow(StringBuilder sb, ICharacter actor)
	{
		if (GradeProfile?.Incantation is not { } p) { sb.AppendLine("  Incantation: not authored; formula and quiet casting unavailable."); return; }
		sb.AppendLine($"  Incantation language: {Gameworld.Languages.Get(p.LanguageId)?.Name ?? p.LanguageId.ToString()}; category vocabulary: {p.VocabularyProvenance}");
		sb.AppendLine($"  Historical POWER (external grades 1..7): {string.Join(" ", ArmageddonPowerWords.All)}");
		sb.AppendLine($"  Reach / element / sphere / mood: {string.Join(" / ", p.CategoryWords)} (five categories in any order); aliases: {string.Join(", ", p.Aliases)}");
		foreach (var d in p.Deliveries) sb.AppendLine($"  Native {d.Method}, {d.Volume.Describe()}: energy x{d.EnergyMultiplier.ToString("N2", actor)}, difficulty {d.DifficultySteps:+0;-0;0}");
	}

	private bool BuildingCommandIncantation(ICharacter actor, StringStack command, ControlledSpellProfile profile)
	{
		var action = command.PopSpeech().ToLowerInvariant();
		var p = profile.Incantation;
		if (action == "off") p = null;
		else if (action == "fixture")
		{
			var language = Gameworld.Languages.GetByIdOrName(command.PopSpeech()) ?? throw new FormatException("Specify a native spoken language.");
			var reach = command.PopSpeech(); var element = command.PopSpeech(); var sphere = command.PopSpeech(); var mood = command.PopSpeech();
			var alias = command.PopSpeech();
			p = new(language.Id, reach, element, sphere, mood, Array.AsReadOnly(new[] { alias }),
				Array.AsReadOnly(new[] { new ControlledSpellDelivery("Say", AudioVolume.Decent, 1, 0), new ControlledSpellDelivery("Whisper", AudioVolume.Quiet, 2, 1) }));
		}
		else
		{
			if (p is null) throw new FormatException("Author grades incantation fixture <language> <reach> <element> <sphere> <mood> <alias> first. The four category words are labelled FutureMUD aliases; POWER is fixed.");
			switch (action)
			{
				case "language": p = p with { LanguageId = (Gameworld.Languages.GetByIdOrName(command.PopSpeech()) ?? throw new FormatException("No such spoken language.")).Id }; break;
				case "word":
					var category = command.PopSpeech().ToLowerInvariant(); var word = command.PopSpeech();
					p = category switch { "reach" => p with { Reach = word }, "element" => p with { Element = word },
						"sphere" => p with { Sphere = word }, "mood" => p with { Mood = word }, _ => throw new FormatException("Use reach, element, sphere or mood; POWER is fixed.") }; break;
				case "alias":
					var operation = command.PopSpeech(); var token = command.PopSpeech();
					p = p with { Aliases = Array.AsReadOnly(operation.ToLowerInvariant() switch {
						"add" => p.Aliases.Append(token).ToArray(), "remove" => p.Aliases.Where(x => !x.EqualTo(token)).ToArray(),
						_ => throw new FormatException("Use alias add/remove <single word>.") }) }; break;
				case "method":
					var method = MagicCastingMethods.CanonicalName(command.PopSpeech()) ?? throw new FormatException("Use Say, Whisper, Talk, LoudSay, Yell, Shout or Sing.");
					var deliveries = p.Deliveries.Where(x => !x.Method.EqualTo(method)).ToList();
					if (command.PeekSpeech().EqualTo("off")) command.PopSpeech();
					else
					{
						MagicCastingMethods.TryVolume(method, out var volume);
						deliveries.Add(new(method, volume, double.Parse(command.PopSpeech(), CultureInfo.InvariantCulture), int.Parse(command.PopSpeech(), CultureInfo.InvariantCulture)));
					}
					p = p with { Deliveries = deliveries.AsReadOnly() }; break;
				case "provenance": p = p with { VocabularyProvenance = command.SafeRemainingArgument }; command = new StringStack(""); break;
				default: throw new FormatException("Use incantation fixture/off, language, word <category> <token>, alias add/remove, method <native name> <energy multiplier> <difficulty steps> or off, or provenance <source label>.");
			}
		}
		if (!command.IsFinished) throw new FormatException("Unexpected trailing incantation input.");
		if (IncantationConfigurationErrors(p).FirstOrDefault() is { } error) throw new FormatException(error);
		GradeProfile = profile with { Incantation = p }; _unreadableGradeProfile = null; _gradeLoadError = null; Changed = true;
		actor.OutputHandler.Send("Incantation policy updated. Normal/whisper fixture modifiers are configurable tuning; area and practice combinations require a separate policy.");
		return true;
	}
}
