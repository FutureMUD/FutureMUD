#nullable enable

using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Health;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete.SpellEffects;

public class SpellItemEnchantmentEffect : MagicSpellEffectBase, IDescriptionAdditionEffect, ISDescAdditionEffect,
	IProduceIllumination, IMagicWeaponEnhancementEffect, IMagicArmourEnhancementEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellItemEnchantment", (effect, owner) => new SpellItemEnchantmentEffect(effect, owner));
	}

	public SpellItemEnchantmentEffect(IPerceivable owner, IMagicSpellEffectParent parent, string sdescAddendum,
		string descAddendum, ANSIColour colour, double glowLux, double attackCheckBonus, double qualityBonus,
		double damageBonus, double painBonus, double stunBonus, double armourDamageReduction, IFutureProg? prog = null)
		: base(owner, parent, prog)
	{
		SDescAddendum = sdescAddendum;
		DescAddendum = descAddendum;
		GlowAddendumColour = colour;
		ProvidedLux = glowLux;
		AttackCheckBonus = attackCheckBonus;
		QualityBonus = qualityBonus;
		DamageBonus = damageBonus;
		PainBonus = painBonus;
		StunBonus = stunBonus;
		ArmourDamageReduction = armourDamageReduction;
	}

	private SpellItemEnchantmentEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		var trueRoot = root.Element("Effect");
		SDescAddendum = trueRoot?.Element("SDescAddendum")?.Value ?? string.Empty;
		DescAddendum = trueRoot?.Element("DescAddendum")?.Value ?? string.Empty;
		GlowAddendumColour = Telnet.GetColour(trueRoot?.Element("Colour")?.Value ?? "bold magenta") ?? Telnet.BoldMagenta;
		ProvidedLux = double.Parse(trueRoot?.Element("GlowLux")?.Value ?? "0");
		AttackCheckBonus = double.Parse(trueRoot?.Element("AttackCheckBonus")?.Value ?? "0");
		QualityBonus = double.Parse(trueRoot?.Element("QualityBonus")?.Value ?? "0");
		DamageBonus = double.Parse(trueRoot?.Element("DamageBonus")?.Value ?? "0");
		PainBonus = double.Parse(trueRoot?.Element("PainBonus")?.Value ?? "0");
		StunBonus = double.Parse(trueRoot?.Element("StunBonus")?.Value ?? "0");
		ArmourDamageReduction = double.Parse(trueRoot?.Element("ArmourDamageReduction")?.Value ?? "0");
	}

	public string SDescAddendum { get; }
	public string DescAddendum { get; }
	public ANSIColour GlowAddendumColour { get; }
	public double ProvidedLux { get; }
	public double AttackCheckBonus { get; }
	public double QualityBonus { get; }
	public double DamageBonus { get; }
	public double PainBonus { get; }
	public double StunBonus { get; }
	public double ArmourDamageReduction { get; }

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("ApplicabilityProg", ApplicabilityProg?.Id ?? 0),
			new XElement("SDescAddendum", new XCData(SDescAddendum)),
			new XElement("DescAddendum", new XCData(DescAddendum)),
			new XElement("Colour", GlowAddendumColour.Name),
			new XElement("GlowLux", ProvidedLux),
			new XElement("AttackCheckBonus", AttackCheckBonus),
			new XElement("QualityBonus", QualityBonus),
			new XElement("DamageBonus", DamageBonus),
			new XElement("PainBonus", PainBonus),
			new XElement("StunBonus", StunBonus),
			new XElement("ArmourDamageReduction", ArmourDamageReduction)
		);
	}

	public string AddendumText => SDescAddendum;

	public string GetAddendumText(bool colour)
	{
		return colour ? SDescAddendum.Colour(GlowAddendumColour) : SDescAddendum;
	}

	public string GetAdditionalText(IPerceiver voyeur, bool colour)
	{
		return colour ? DescAddendum.Colour(GlowAddendumColour) : DescAddendum;
	}

	public bool PlayerSet => false;

	public bool AppliesToWeaponAttack(ICharacter attacker, IPerceivable target, IGameItem weapon)
	{
		return ReferenceEquals(Owner, weapon) &&
		       (ApplicabilityProg?.ExecuteBool(weapon, attacker, target) ?? true);
	}

	public IDamage SufferDamage(IDamage damage, ref List<IWound> wounds)
	{
		return ReduceDamage(damage);
	}

	public IDamage PassiveSufferDamage(IDamage damage, ref List<IWound> wounds)
	{
		return ReduceDamage(damage);
	}

	public void ProcessPassiveWound(IWound wound)
	{
	}

	private IDamage ReduceDamage(IDamage damage)
	{
		if (damage is null)
		{
			return null!;
		}

		if (ArmourDamageReduction <= 0.0)
		{
			return damage;
		}

		var reducedDamage = Math.Max(0.0, damage.DamageAmount - ArmourDamageReduction);
		var reducedPain = Math.Max(0.0, damage.PainAmount - ArmourDamageReduction);
		var reducedStun = Math.Max(0.0, damage.StunAmount - ArmourDamageReduction);
		if (reducedDamage <= 0.0 && reducedPain <= 0.0 && reducedStun <= 0.0)
		{
			return null!;
		}

		return new Damage(damage)
		{
			DamageAmount = reducedDamage,
			PainAmount = reducedPain,
			StunAmount = reducedStun
		};
	}

	public override string Describe(IPerceiver voyeur)
	{
		return "Magically enchanted item.";
	}

	protected override string SpecificEffectType => "SpellItemEnchantment";
}
