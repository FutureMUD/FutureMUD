#nullable enable

using System.Globalization;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.FutureProg;
using MudSharp.Magic.Emotions;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public abstract partial class SourceEmotionalEffect
{
	public const string BuilderHelp = "Use intensity <number>, eligibility <prog>, lifetime <group> <seconds> <cap>, reset (discard the profile for repair); Fury: attribute <trait>, units <number>, terrain <ID> <numerator> <denominator> <bonus>, terrain remove <ID>, fallback <numerator> <denominator> <bonus>; Calm: savetrait <trait>, save <grade> <difficulty>, attackbreak on|off. Use one exclusive target effect, character/self trigger, duration expression 0 and no ordinary opposed trait.";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var option = command.PopSpeech().ToLowerInvariant();
		var candidate = new XElement(_profileXml);
		try
		{
			switch (option)
			{
				case "reset":
					if (!command.IsFinished) throw new ArgumentException("Reset takes no arguments.");
					_profileXml = EmptyProfile(Kind); ReadProfile(); Spell.Changed = true;
					actor.OutputHandler.Send(Show(actor)); return true;
				case "intensity":
				case "units" when Kind == EmotionalSpellKind.Fury:
					var number = double.Parse(command.PopSpeech(), NumberStyles.Float, actor);
					if (!double.IsFinite(number) || (option == "units" ? number <= 0 : number < 0)) throw new ArgumentException("Select a finite valid value.");
					candidate.SetAttributeValue(option, number); break;
				case "attribute" when Kind == EmotionalSpellKind.Fury:
				case "savetrait" when Kind == EmotionalSpellKind.Calm:
					var trait = Gameworld.Traits.GetByIdOrName(command.PopSpeech());
					if (trait is null || (Kind == EmotionalSpellKind.Fury && trait.TraitType != TraitType.Attribute))
						throw new ArgumentException("Select an existing trait; Fury requires an ordinary attribute.");
					candidate.SetAttributeValue("trait", trait.Id); break;
				case "eligibility":
					var prog = Gameworld.FutureProgs.GetByIdOrName(command.PopSpeech());
					if (prog is null || prog.ReturnType != ProgVariableTypes.Boolean ||
						!prog.MatchesParameters([ProgVariableTypes.Character, ProgVariableTypes.Character]) || !prog.Compile())
						throw new ArgumentException("Select a compiling boolean (target, caster) prog.");
					candidate.SetAttributeValue("eligibility", prog.Id); break;
				case "lifetime":
					var group = command.PopSpeech(); var seconds = ParseInt(command); var cap = ParseInt(command);
					if (DetectInvisibleEffect.PolicyError(new(group, seconds, cap)) is { } error) throw new ArgumentException(error);
					candidate.Element("Lifetime")?.Remove();
					candidate.Add(new XElement("Lifetime", new XAttribute("group", group), new XAttribute("seconds", seconds), new XAttribute("cap", cap))); break;
				case "fallback" when Kind == EmotionalSpellKind.Fury:
					var fallback = ParseRule(command); candidate.Element("Fallback")?.Remove(); candidate.Add(fallback); break;
				case "terrain" when Kind == EmotionalSpellKind.Fury:
					var first = command.PopSpeech(); var remove = first.EqualTo("remove");
					var id = long.Parse(remove ? command.PopSpeech() : first, CultureInfo.InvariantCulture);
					if (id <= 0 || (!remove && Gameworld.Terrains.Get(id) is null)) throw new ArgumentException("Select an existing native terrain ID.");
					var terrains = candidate.Element("Terrains") ?? new XElement("Terrains");
					if (terrains.Parent is null) candidate.Add(terrains);
					terrains.Elements("Terrain").Where(x => (long?)x.Attribute("id") == id).Remove();
					if (!remove) { var rule = ParseRule(command); rule.Name = "Terrain"; rule.SetAttributeValue("id", id); terrains.Add(rule); }
					break;
				case "save" when Kind == EmotionalSpellKind.Calm:
					var grade = ParseInt(command); EmotionalSpellPolicy.ValidateGrade(grade);
					if (!Enum.TryParse<Difficulty>(command.PopSpeech(), true, out var difficulty) || !Enum.IsDefined(difficulty))
						throw new ArgumentException("Select a native save difficulty.");
					var saves = candidate.Element("Saves") ?? new XElement("Saves");
					if (saves.Parent is null) candidate.Add(saves);
					saves.Elements("Grade").Where(x => (int?)x.Attribute("number") == grade).Remove();
					saves.Add(new XElement("Grade", new XAttribute("number", grade), new XAttribute("difficulty", (int)difficulty))); break;
				case "attackbreak" when Kind == EmotionalSpellKind.Calm:
					var value = command.PopSpeech();
					if (!value.EqualTo("on") && !value.EqualTo("off")) throw new ArgumentException("Use attackbreak on or off.");
					candidate.SetAttributeValue("attackbreak", value.EqualTo("on")); break;
				default: actor.OutputHandler.Send(BuilderHelp); return false;
			}
			if (!command.IsFinished) throw new ArgumentException("Unexpected extra arguments.");
			// Reject invalid edits to a valid profile. Incomplete builder profiles can be repaired one field at a time.
			if (Profile is not null) EmotionalStockProfile.Read(candidate);
			_profileXml = candidate; ReadProfile(); Spell.Changed = true;
			actor.OutputHandler.Send(Show(actor)); return true;
		}
		catch (Exception error) when (error is ArgumentException or FormatException or OverflowException or InvalidOperationException)
		{
			actor.OutputHandler.Send(error.Message.ColourError()); return false;
		}
	}

	private static int ParseInt(StringStack command) => int.Parse(command.PopSpeech(), CultureInfo.InvariantCulture);
	private static XElement ParseRule(StringStack command)
	{
		var rule = new EmotionalTerrainRule(ParseInt(command), ParseInt(command), ParseInt(command));
		for (var grade = 1; grade <= 7; grade++)
		{
			rule.DurationUnits(grade);
			EmotionalSpellPolicy.RollEndurance(grade, rule, (_, upper) => upper);
		}
		return new XElement("Fallback", new XAttribute("numerator", rule.DurationNumerator),
			new XAttribute("denominator", rule.DurationDenominator), new XAttribute("endurance", rule.EnduranceBonusPerGrade));
	}
}
