#nullable enable

using System.Globalization;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public sealed class LeechingEdgeEffect : IMagicSpellEffectTemplate, IMagicSpellEffectPreparedSelection
{
	private readonly Dictionary<IPerceivable, Selection> _selections = new(ReferenceEqualityComparer.Instance);
	private sealed record Selection(ICharacter Caster, IPerceivable Recipient, int Grade, string Definition,
		Func<bool> Check) : IMagicSpellEffectPreparedSelectionRawToken
	{ public bool IsCurrent => Check(); }
	public IMagicSpellEffectPreparedSelectionToken CapturePreparedSelection(ICharacter caster, IPerceivable recipient)
	{
		if (_selections.TryGetValue(recipient, out var existing))
		{
			if (!TryConfirmPreparedSelection(caster, recipient, out var error)) throw new InvalidOperationException(error);
			return existing;
		}
		if (recipient is not IGameItem item || item.GetItemType<IMeleeWeapon>() is not { } weapon ||
			!ReferenceEquals(weapon.Parent, item) || item.Deleted || item.Destroyed ||
			Spell is not MagicSpell { InvocationGrade: >= 1 and <= 7 } native || caster.Body is not { } body ||
			!ReferenceEquals(body.Actor, caster) || !ReferenceEquals(caster.Gameworld, Gameworld) || !ReferenceEquals(item.Gameworld, Gameworld))
			throw new InvalidOperationException("Leeching Edge requires an existing native melee weapon in this world and a selected grade.");
		var identity = caster.Identity; var instance = caster.InstanceId; var id = caster.Id; var itemId = item.Id;
		var state = caster.State; var grade = native.InvocationGrade.Value;
		var definition = SaveToXml().ToString(SaveOptions.DisableFormatting);
		var selection = new Selection(caster, item, grade, definition, () =>
			ReferenceEquals(caster.Gameworld, Gameworld) && ReferenceEquals(item.Gameworld, Gameworld) &&
			ReferenceEquals(caster.Body, body) && ReferenceEquals(body.Actor, caster) && ReferenceEquals(caster.Identity, identity) &&
			caster.InstanceId == instance && caster.Id == id && caster.State == state && item.Id == itemId && !item.Deleted && !item.Destroyed &&
			ReferenceEquals(item.GetItemType<IMeleeWeapon>(), weapon) && ReferenceEquals(weapon.Parent, item) &&
			native.InvocationGrade == grade && SaveToXml().ToString(SaveOptions.DisableFormatting) == definition);
		_selections.Add(recipient, selection); return selection;
	}
	public bool TryReusePreparedSelection(IMagicSpellEffectPreparedSelectionToken selection, ICharacter caster,
		IPerceivable recipient, out string? error)
	{
		error = "The prepared melee weapon, caster body or configuration changed.";
		if (selection is not Selection token || !ReferenceEquals(token.Caster, caster) || !ReferenceEquals(token.Recipient, recipient) ||
			!token.IsCurrent || Spell is not MagicSpell native || native.InvocationGrade != token.Grade ||
			SaveToXml().ToString(SaveOptions.DisableFormatting) != token.Definition) return false;
		_selections[recipient] = token; error = null; return true;
	}
	public bool TryConfirmPreparedSelection(ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = "The prepared melee weapon, caster body or configuration changed.";
		return _selections.TryGetValue(recipient, out var token) && TryReusePreparedSelection(token, caster, recipient, out error);
	}
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("leechingedge", (xml, spell) => new LeechingEdgeEffect(xml, spell));
		SpellEffectFactory.RegisterBuilderFactory("leechingedge", (_, spell) =>
			(new LeechingEdgeEffect(new XElement("Effect", new XAttribute("type", "leechingedge"),
				new XElement("FractionPerGrade", 0.1), new XElement("MaximumFraction", 0.5)), spell), ""),
			"Heals the current melee wielder from measured weapon harm", HelpText, false, true,
			SpellTriggerFactory.MagicTriggerTypes.Where(x => SpellTriggerFactory.BuilderInfoForType(x).TargetTypes is "item" or "items").ToArray());
	}
	public LeechingEdgeEffect(XElement xml, IMagicSpell spell)
	{
		Spell = spell;
		FractionPerGrade = (double)(xml.Element("FractionPerGrade") ?? throw new FormatException("Missing transfer fraction."));
		MaximumFraction = (double)(xml.Element("MaximumFraction") ?? throw new FormatException("Missing transfer cap."));
		if (!ValidFraction(FractionPerGrade) || !ValidFraction(MaximumFraction)) throw new FormatException("Transfer fractions must be finite and in (0, 1].");
	}
	private static bool ValidFraction(double value) => double.IsFinite(value) && value is > 0 and <= 1;
	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public double FractionPerGrade { get; private set; }
	public double MaximumFraction { get; private set; }
	public bool IsInstantaneous => false;
	public bool RequiresTarget => true;
	public const string HelpText = "Options: fraction <fraction per external grade>, cap <maximum fraction>. Values must be in (0,1]. The native spell duration controls expiry. Only actual melee weapon damage heals the current wielder; misses, item damage, friendly wounds, blocked and absorbed harm give no credit.";
	public XElement SaveToXml() => new("Effect", new XAttribute("type", "leechingedge"),
		new XElement("FractionPerGrade", FractionPerGrade), new XElement("MaximumFraction", MaximumFraction));
	public IMagicSpellEffectTemplate Clone() => new LeechingEdgeEffect(SaveToXml(), Spell);
	public bool IsCompatibleWithTrigger(IMagicTrigger trigger) => trigger.TargetTypes is "item" or "items";
	public string Show(ICharacter actor) => SpellEffectPresentation.Describe(actor, "Leeching Edge",
		("Share per grade", FractionPerGrade.ToString("P0", actor).ColourValue()),
		("Maximum share", MaximumFraction.ToString("P0", actor).ColourValue()));
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var field = command.PopSpeech().ToLowerInvariant();
		if (field is not ("fraction" or "cap") || !double.TryParse(command.SafeRemainingArgument, NumberStyles.Float, actor, out var value) || !ValidFraction(value))
		{ actor.OutputHandler.Send(HelpText); return false; }
		if (field == "fraction") FractionPerGrade = value; else MaximumFraction = value;
		Spell.Changed = true;
		actor.OutputHandler.Send(Show(actor));
		return true;
	}
	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (target is not null && _selections.ContainsKey(target) && !TryConfirmPreparedSelection(caster, target, out var error))
			throw new InvalidOperationException(error);
		if (target is not IGameItem item || item.GetItemType<IMeleeWeapon>() is null ||
			Spell is not MagicSpell { InvocationGrade: >= 1 and <= 7 } native) return null;
		return new SpellLeechingEdgeEffect(item, parent, Math.Min(FractionPerGrade * native.InvocationGrade.Value, MaximumFraction));
	}
}
