#nullable enable

using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public sealed class CompellingRegardEffect : IMagicSpellEffectTemplate, IMagicSpellEffectPreparedSelection, IMagicSpellEffectOperation
{
	private readonly XElement _definition;
	private readonly Dictionary<IPerceivable, Selection> _selections = new(ReferenceEqualityComparer.Instance);
	public CompellingRegardEffect(XElement xml, IMagicSpell spell) { _definition = new(xml); Spell = spell; }
	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public bool IsInstantaneous => false;
	public bool RequiresTarget => true;
	private long Undead => (long?)_definition.Element("UndeadMerit") ?? 0;
	private bool AllowPcs => (bool?)_definition.Element("AllowPcs") ?? true;
	public string? DefinitionError => (int?)_definition.Attribute("version") != 1 || Gameworld.Merits.Get(Undead) is null ? "Bind the exact undead merit." : null;
	public XElement SaveToXml() => new(_definition);
	public IMagicSpellEffectTemplate Clone() => new CompellingRegardEffect(SaveToXml(), Spell);
	public bool IsCompatibleWithTrigger(IMagicTrigger trigger) => trigger is SpellTriggers.CastingTriggerCharacter;
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("compellingregard", (xml, spell) => new CompellingRegardEffect(xml, spell));
		SpellEffectFactory.RegisterBuilderFactory("compellingregard", (_, spell) => (new CompellingRegardEffect(new XElement("Effect", new XAttribute("type", "compellingregard"),
			new XAttribute("version", 1), new XElement("UndeadMerit", 0), new XElement("AllowPcs", true)), spell), ""),
			"Persistent resisted PC/NPC influence with exact IC order admission", "pcs; undead <merit>", false, true, ["character"]);
	}
	private bool Allowed(ICharacter caster, ICharacter target) => !ReferenceEquals(caster, target) && !target.IsAdministrator() &&
		(AllowPcs || !target.IsPlayerCharacter) && !target.AffectedBy<IIgnoreForceEffect>() && !target.Merits.Any(x => x.Id == Undead) &&
		!target.Effects.OfType<SpellCompellingRegardEffect>().Any(x => x.IsActive) && MagicInterdictionHelper.GetInterdiction(caster, target, Spell.School, false) is null;
	private sealed record Selection(ICharacter Caster, ICharacter Target, int Grade, SpellTravelSnapshot Snapshot, Func<bool> Raw) : IMagicSpellEffectPreparedSelectionRawToken
	{ public bool Attempted { get; set; } public bool IsCurrent => !Attempted && Snapshot.AtCapturedPositions && Raw(); }
	public IMagicSpellEffectPreparedSelectionToken CapturePreparedSelection(ICharacter caster, IPerceivable recipient)
	{
		if (_selections.TryGetValue(recipient, out var prior)) return prior;
		if (DefinitionError is not null || recipient is not ICharacter target || Spell is not MagicSpell { InvocationGrade: >= 1 and <= 7 } spell)
			throw new InvalidOperationException("Compelling Regard needs a configured physical character target.");
		var snapshot = new SpellTravelSnapshot(Gameworld, [caster, target], [caster.Location, target.Location], requireSolo: false);
		var definition = SaveToXml().ToString(SaveOptions.DisableFormatting); var merits = target.Merits.ToArray(); var pcs = target.IsPlayerCharacter;
		var existing = target.Effects.OfType<SpellCompellingRegardEffect>().ToArray();
		var token = new Selection(caster, target, spell.InvocationGrade.Value, snapshot, () => definition == SaveToXml().ToString(SaveOptions.DisableFormatting) &&
			target.IsPlayerCharacter == pcs && target.Merits.SequenceEqual(merits, ReferenceEqualityComparer.Instance) && target.Effects.OfType<SpellCompellingRegardEffect>().SequenceEqual(existing, ReferenceEqualityComparer.Instance));
		if (!token.IsCurrent || !Allowed(caster, target) || !token.IsCurrent) throw new InvalidOperationException("The target refuses this influence.");
		_selections.Add(recipient, token); return token;
	}
	public bool TryReusePreparedSelection(IMagicSpellEffectPreparedSelectionToken selection, ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = "The exact influence target or policy changed.";
		if (selection is not Selection token || !ReferenceEquals(caster, token.Caster) || !ReferenceEquals(recipient, token.Target) || Spell is not MagicSpell spell || spell.InvocationGrade != token.Grade ||
			!token.IsCurrent || !Allowed(caster, token.Target) || !token.IsCurrent) return false;
		_selections[recipient] = token; error = null; return true;
	}
	public bool TryConfirmPreparedSelection(ICharacter caster, IPerceivable recipient, out string? error)
	{ error = "Influence was not prepared."; return _selections.TryGetValue(recipient, out var token) && TryReusePreparedSelection(token, caster, recipient, out error); }
	public MagicEffectOperation Apply(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] parameters)
	{
		if (target is null || !TryConfirmPreparedSelection(caster, target, out _)) return new(MagicEffectOperationStatus.Rejected, null);
		var selection = _selections[target]; selection.Attempted = true;
		return new(MagicEffectOperationStatus.Applied, new SpellCompellingRegardEffect(selection.Target, parent, caster, AllowPcs, Undead, RuntimeClock.UtcNow.AddSeconds(30 * selection.Grade)));
	}
	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] parameters) => Apply(caster, target, outcome, power, parent, parameters).Effect;
	public string Show(ICharacter actor) => $"Compelling Regard for 30 seconds per grade; PCs allowed: {AllowPcs.ToColouredString()}. Exact IC orders use compel.";
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var option = command.PopForSwitch();
		if (option == "pcs" && command.IsFinished) _definition.SetElementValue("AllowPcs", !AllowPcs);
		else if (option == "undead" && Gameworld.Merits.GetByIdOrName(command.SafeRemainingArgument) is { } merit) _definition.SetElementValue("UndeadMerit", merit.Id);
		else return false;
		Spell.Changed = true; actor.OutputHandler.Send("The influence eligibility policy has been updated."); return true;
	}
}
