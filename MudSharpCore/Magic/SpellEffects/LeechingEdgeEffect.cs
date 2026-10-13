#nullable enable

using System.Globalization;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public sealed class LeechingEdgeEffect : IMagicSpellEffectTemplate
{
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
		if (target is not IGameItem item || item.GetItemType<IMeleeWeapon>() is null ||
			Spell is not MagicSpell { InvocationGrade: >= 1 and <= 7 } native) return null;
		return new SpellLeechingEdgeEffect(item, parent, Math.Min(FractionPerGrade * native.InvocationGrade.Value, MaximumFraction));
	}
}
