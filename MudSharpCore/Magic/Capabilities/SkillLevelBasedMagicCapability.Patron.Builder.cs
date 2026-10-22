#nullable enable
namespace MudSharp.Magic.Capabilities;

public partial class SkillLevelBasedMagicCapability
{
	private MagicCastingPolicy EditPatron(ICharacter actor, StringStack command, MagicCastingPolicy policy)
	{
		var field = command.PopSpeech().ToLowerInvariant();
		if (field == "remove") return policy with { Patron = null };
		var patron = policy.Patron ?? new PatronCastingPolicy(Gameworld.AlwaysFalseProg.Id, Gameworld.AlwaysFalseProg.Id, 7, true, true, true, []);
		var value = command.SafeRemainingArgument;
		long Prog()
		{
			var prog = Gameworld.FutureProgs.GetByIdOrName(value);
			if (prog is null || !prog.ReturnType.CompatibleWith(ProgVariableTypes.Boolean) || !prog.MatchesParameters([ProgVariableTypes.Character])) throw new FormatException("Choose a boolean(character) prog.");
			return prog.Id;
		}
		patron = field switch
		{
			"authority" => patron with { AuthorityProgId = Prog() }, "territory" => patron with { TerritoryProgId = Prog() },
			"grade" => patron with { FixedGrade = int.Parse(value) }, "limit" => patron with { SelectionLimit = int.Parse(value) },
			"free" => patron with { FreeWithinTerritory = Toggle(value) }, "refill" => patron with { RefillWithinTerritory = Toggle(value) },
			"shout" => patron with { RequireShout = Toggle(value) },
			"permanent" => TogglePermanent(),
			"incantation" => EditIncantation(),
			_ => throw new FormatException("Use patron authority|territory|grade|limit|free|refill|shout|permanent|incantation|remove.")
		};
		return policy with { Patron = patron };

		PatronCastingPolicy EditIncantation()
		{
			var spell = Gameworld.MagicSpells.GetByIdOrName(command.PopSpeech()) ?? throw new FormatException("Choose an admitted spell.");
			if (!policy.Admissions.Any(x => x.SpellId == spell.Id)) throw new FormatException("The spell is not admitted.");
			var entries = patron.Incantations?.ToDictionary(x => x.Key, x => x.Value) ?? new Dictionary<long, ControlledSpellIncantation>();
			var languageText = command.PopSpeech();
			if (languageText.EqualTo("none")) entries.Remove(spell.Id);
			else
			{
				var language = Gameworld.Languages.GetByIdOrName(languageText) ?? throw new FormatException("Choose a native spoken language.");
				var reach = command.PopSpeech(); var element = command.PopSpeech(); var sphere = command.PopSpeech(); var mood = command.PopSpeech();
				var energy = command.IsFinished ? 1.0 : double.Parse(command.PopSpeech(), actor);
				var difficulty = command.IsFinished ? 0 : int.Parse(command.PopSpeech());
				if (!command.IsFinished) throw new FormatException("Unexpected trailing incantation arguments.");
				entries[spell.Id] = new(language.Id, reach, element, sphere, mood, [],
					[new("Shout", MudSharp.Form.Audio.AudioVolume.ExtremelyLoud, energy, difficulty)]);
			}
			return patron with { Incantations = entries };
		}

		PatronCastingPolicy TogglePermanent()
		{
			var spell = Gameworld.MagicSpells.GetByIdOrName(value) ?? throw new FormatException("Choose an admitted spell.");
			if (!policy.Admissions.Any(x => x.SpellId == spell.Id)) throw new FormatException("The spell is not admitted.");
			return patron with { PermanentSpells = patron.PermanentSpells.Contains(spell.Id) ? patron.PermanentSpells.Where(x => x != spell.Id).ToArray() : patron.PermanentSpells.Append(spell.Id).ToArray() };
		}
	}
}
