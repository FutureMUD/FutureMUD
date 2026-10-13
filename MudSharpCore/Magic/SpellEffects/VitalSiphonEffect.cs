#nullable enable

using System.Globalization;
using MudSharp.Health;
using MudSharp.NPC.AI;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

/// <summary>One linked native injury/healing operation. Payment and resistance remain in MagicSpell.</summary>
public sealed class VitalSiphonEffect : IMagicSpellEffectTemplate, IMagicSpellEffectOperation
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("vitalsiphon", (xml, spell) => new VitalSiphonEffect(xml, spell));
		SpellEffectFactory.RegisterBuilderFactory("vitalsiphon", (command, spell) =>
			(new VitalSiphonEffect(new XElement("Effect", new XAttribute("type", "vitalsiphon"),
				new XElement("DamageType", (int)DamageType.Arcane), new XElement("DamagePerGrade", 5),
				new XElement("HealingCapPerGrade", 5)), spell), ""),
			"Transfers measured native damage into bounded caster healing", HelpText, true, true,
			SpellTriggerFactory.MagicTriggerTypes.Where(x => SpellTriggerFactory.BuilderInfoForType(x).TargetTypes is "character" or "characters").ToArray());
	}

	public VitalSiphonEffect(XElement xml, IMagicSpell spell)
	{
		Spell = spell;
		DamageType = (DamageType)(int)(xml.Element("DamageType") ?? throw new FormatException("Choose a native siphon damage type."));
		DamagePerGrade = (double)(xml.Element("DamagePerGrade") ?? throw new FormatException("Missing siphon damage budget."));
		HealingCapPerGrade = (double)(xml.Element("HealingCapPerGrade") ?? throw new FormatException("Missing siphon healing cap."));
		if (!Enum.IsDefined(DamageType) || !ValidAmount(DamagePerGrade) || !ValidAmount(HealingCapPerGrade))
			throw new FormatException("Siphon needs a defined native damage type and finite positive per-grade budgets at most 1000000.");
	}

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public DamageType DamageType { get; private set; }
	public double DamagePerGrade { get; private set; }
	public double HealingCapPerGrade { get; private set; }
	public bool IsInstantaneous => true;
	public bool RequiresTarget => true;
	public const string HelpText = "Options: type <native damage type>, damage <amount per external grade>, healingcap <amount per external grade>. Requires configured external grades. Healing uses only measured delivered damage and existing Mend-eligible wounds. Arcane is the editable authored builder default.";
	private static bool ValidAmount(double amount) => double.IsFinite(amount) && amount is > 0 and <= 1000000;
	public XElement SaveToXml() => new("Effect", new XAttribute("type", "vitalsiphon"),
		new XElement("DamageType", (int)DamageType), new XElement("DamagePerGrade", DamagePerGrade),
		new XElement("HealingCapPerGrade", HealingCapPerGrade));
	public IMagicSpellEffectTemplate Clone() => new VitalSiphonEffect(SaveToXml(), Spell);
	public bool IsCompatibleWithTrigger(IMagicTrigger trigger) => trigger.TargetTypes is "character" or "characters";
	public string Show(ICharacter actor) => SpellEffectPresentation.Describe(actor, "Vital Siphon",
		("Native Damage", DamageType.DescribeEnum().ColourName()),
		("Damage per grade", DamagePerGrade.ToString("N2", actor).ColourValue()),
		("Healing cap per grade", HealingCapPerGrade.ToString("N2", actor).ColourValue()));
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var field = command.PopSpeech().ToLowerInvariant();
		var text = command.SafeRemainingArgument;
		if (field == "type" && text.TryParseEnum<DamageType>(out var type) && Enum.IsDefined(type)) DamageType = type;
		else if (field is "damage" or "healingcap" && double.TryParse(text, NumberStyles.Float, actor, out var value) && ValidAmount(value))
		{
			if (field == "damage") DamagePerGrade = value;
			else HealingCapPerGrade = value;
		}
		else { actor.OutputHandler.Send(HelpText); return false; }
		Spell.Changed = true;
		actor.OutputHandler.Send(Show(actor));
		return true;
	}

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters) =>
		Apply(caster, target, outcome, power, parent, additionalParameters).Effect;

	public MagicEffectOperation Apply(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (target is not ICharacter victim || ReferenceEquals(caster, victim) ||
			Spell is not MagicSpell { InvocationGrade: >= 1 and <= 7 } native || caster.Body is null || victim.Body is null)
			return new(MagicEffectOperationStatus.Rejected, null);
		using var receipt = new VitalTransferReceipt(caster, victim);
		var body = victim.Body;
		var part = body.RandomBodypart;
		if (part is null || !receipt.IsCurrent || !CommandExecutionScope.TryContinue())
			return new(MagicEffectOperationStatus.Rejected, null);
		var amount = Casting.SpellAreaDamageScope.ScaleDamage(caster, victim, Spell, DamagePerGrade * native.InvocationGrade.Value);
		if (!double.IsFinite(amount) || amount <= 0) return new(MagicEffectOperationStatus.NoChange, null);
		body.SufferDamage(new Damage
		{
			ActorOrigin = caster, TargetBody = body, Bodypart = part, DamageType = DamageType,
			DamageAmount = amount, PainAmount = amount, StunAmount = amount, VitalTransfer = receipt
		}).ToArray();
		receipt.Settle(1, HealingCapPerGrade * native.InvocationGrade.Value);
		return new(receipt.DeliveredDamage > 0 ? MagicEffectOperationStatus.Applied : MagicEffectOperationStatus.NoChange, null);
	}
}
