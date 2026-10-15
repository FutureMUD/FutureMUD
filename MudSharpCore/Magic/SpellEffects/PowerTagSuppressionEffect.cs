#nullable enable

using MudSharp.Character;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public sealed class PowerTagSuppressionEffect : IMagicSpellEffectTemplate, IMagicSpellEffectPreparedSelection, IMagicSpellEffectOperation
{
	private readonly XElement _definition;
	private readonly Dictionary<IPerceivable, Token> _selections = new(ReferenceEqualityComparer.Instance);
	public PowerTagSuppressionEffect(XElement xml, IMagicSpell spell) { _definition = new(xml); Spell = spell; }
	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public bool IsInstantaneous => false;
	public bool RequiresTarget => true;
	public XElement SaveToXml() => new(_definition);
	public IMagicSpellEffectTemplate Clone() => new PowerTagSuppressionEffect(SaveToXml(), Spell);
	public bool IsCompatibleWithTrigger(IMagicTrigger trigger) => trigger is SpellTriggers.CastingTriggerCharacter;
	private long UndeadMerit => (long?)_definition.Element("UndeadMerit") ?? 0;
	private string[] Tags => _definition.Element("PowerTags")?.Elements("Tag").Select(x => x.Value).ToArray() ?? [];
	private int SecondsPerGrade => (int?)_definition.Element("SecondsPerGrade") ?? 0;
	private bool SeverContacts => (bool?)_definition.Element("SeverMatchingMindContacts") ?? false;
	public string? DefinitionError => (int?)_definition.Attribute("version") != 1 || Tags.Length is < 1 or > 64 ||
		Tags.Any(x => !Powers.MagicPowerBase.ValidPowerTag(x) || x != x.ToLowerInvariant()) || Tags.Distinct(StringComparer.Ordinal).Count() != Tags.Length ||
		SecondsPerGrade is < 1 or > 86400 || UndeadMerit < 0 || UndeadMerit > 0 && Gameworld.Merits.Get(UndeadMerit) is null ?
		"Configure bounded distinct native power tags, lifetime and an optional exact undead merit." : null;
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("powertagsuppression", (xml, spell) => new PowerTagSuppressionEffect(xml, spell));
		SpellEffectFactory.RegisterBuilderFactory("powertagsuppression", (_, spell) => (new PowerTagSuppressionEffect(new XElement("Effect", new XAttribute("type", "powertagsuppression"),
			new XAttribute("version", 1), new XElement("UndeadMerit", 0), new XElement("PowerTags", new XElement("Tag", "psionic")),
			new XElement("SecondsPerGrade", 30), new XElement("SeverMatchingMindContacts", false)), spell), ""),
			"Resistible activation suppression for explicit native power tags", "tags <names>; undead <merit>; lifetime <seconds per grade>; contacts", false, true, ["character"]);
	}
	private bool Allowed(ICharacter caster, ICharacter target) => !target.IsAdministrator() && !target.Merits.Any(x => UndeadMerit > 0 && x.Id == UndeadMerit) &&
		!target.Effects.OfType<IMagicPowerTagSuppression>().Any(x => x.IsActiveFor(target) && x.SuppressedPowerTags.Intersect(Tags, StringComparer.Ordinal).Any()) &&
		MagicInterdictionHelper.GetInterdiction(caster, target, Spell.School, false) is null;
	private sealed record Token(ICharacter Caster, ICharacter Target, int Grade, SpellTravelSnapshot Snapshot, Func<bool> Raw) : IMagicSpellEffectPreparedSelectionRawToken
	{ public bool Attempted { get; set; } public bool IsCurrent => !Attempted && Snapshot.AtCapturedPositions && Raw(); }
	public IMagicSpellEffectPreparedSelectionToken CapturePreparedSelection(ICharacter caster, IPerceivable recipient)
	{
		if (_selections.TryGetValue(recipient, out var prior)) return prior;
		if (DefinitionError is not null || recipient is not ICharacter target || Spell is not MagicSpell { InvocationGrade: >= 1 and <= 7 } spell)
			throw new InvalidOperationException("Power-tag suppression needs one exact physical target and explicit native tags.");
		var snapshot = new SpellTravelSnapshot(Gameworld, [caster, target], [caster.Location, target.Location], requireSolo: false);
		var definition = SaveToXml().ToString(SaveOptions.DisableFormatting); var merits = target.Merits.ToArray();
		var suppressions = target.Effects.OfType<IMagicPowerTagSuppression>().ToArray();
		var token = new Token(caster, target, spell.InvocationGrade.Value, snapshot, () => definition == SaveToXml().ToString(SaveOptions.DisableFormatting) &&
			target.Merits.SequenceEqual(merits, ReferenceEqualityComparer.Instance) && target.Effects.OfType<IMagicPowerTagSuppression>().SequenceEqual(suppressions, ReferenceEqualityComparer.Instance));
		if (!token.IsCurrent || !Allowed(caster, target) || !token.IsCurrent) throw new InvalidOperationException("The exact physical target refuses power-tag suppression.");
		_selections.Add(recipient, token); return token;
	}
	public bool TryReusePreparedSelection(IMagicSpellEffectPreparedSelectionToken selection, ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = "The prepared physical target or explicit tag policy changed.";
		if (selection is not Token token || !ReferenceEquals(token.Caster, caster) || !ReferenceEquals(token.Target, recipient) ||
			Spell is not MagicSpell spell || spell.InvocationGrade != token.Grade || !token.IsCurrent || !Allowed(caster, token.Target) || !token.IsCurrent) return false;
		_selections[recipient] = token; error = null; return true;
	}
	public bool TryConfirmPreparedSelection(ICharacter caster, IPerceivable recipient, out string? error)
	{ error = "The physical target was not prepared."; return _selections.TryGetValue(recipient, out var token) && TryReusePreparedSelection(token, caster, recipient, out error); }
	public MagicEffectOperation Apply(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] parameters)
	{
		if (target is null || !TryConfirmPreparedSelection(caster, target, out _)) return new(MagicEffectOperationStatus.Rejected, null);
		var token = _selections[target]; token.Attempted = true;
		return new(MagicEffectOperationStatus.Applied, new SpellPowerTagSuppressionEffect(token.Target, parent, Tags, RuntimeClock.UtcNow.AddSeconds(SecondsPerGrade * token.Grade), SeverContacts));
	}
	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] parameters) => Apply(caster, target, outcome, power, parent, parameters).Effect;
	public string Show(ICharacter actor) => "Suppress activation of native powers tagged " + string.Join(", ", Tags) + $" for {SecondsPerGrade.ToString("N0", actor)} seconds per grade.";
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var option = command.PopForSwitch();
		if (option == "tags")
		{
			var tags = new List<string>();
			while (!command.IsFinished)
			{
				var tag = command.PopSpeech().Trim().ToLowerInvariant();
				if (!Powers.MagicPowerBase.ValidPowerTag(tag) || tags.Contains(tag) || tags.Count >= 64) return false;
				tags.Add(tag);
			}
			if (tags.Count == 0) return false;
			_definition.Element("PowerTags")!.ReplaceWith(new XElement("PowerTags", tags.Select(x => new XElement("Tag", x))));
		}
		else if (option == "lifetime")
		{
			if (!int.TryParse(command.PopSpeech(), out var seconds) || seconds is < 1 or > 86400 || !command.IsFinished) return false;
			_definition.SetElementValue("SecondsPerGrade", seconds);
		}
		else if (option == "contacts" && command.IsFinished) _definition.SetElementValue("SeverMatchingMindContacts", !SeverContacts);
		else if (option == "undead" && Gameworld.Merits.GetByIdOrName(command.SafeRemainingArgument) is { } merit) _definition.SetElementValue("UndeadMerit", merit.Id);
		else return false;
		Spell.Changed = true; actor.OutputHandler.Send("The native power-tag suppression policy has been updated."); return true;
	}
}
