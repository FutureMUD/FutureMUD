#nullable enable

using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Health;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete.SpellEffects;

public class SpellItemEnchantmentEffect : MagicSpellEffectBase, IDescriptionAdditionEffect, ISDescAdditionEffect,
	IProduceIllumination, IMagicWeaponEnhancementEffect, IMagicArmourEnhancementEffect, IMagicProjectilePayloadEffect,
	IMagicCraftToolEnhancementEffect, IMagicPowerOrFuelEnhancementEffect, IMagicItemEventEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellItemEnchantment", (effect, owner) => new SpellItemEnchantmentEffect(effect, owner));
	}

	public SpellItemEnchantmentEffect(IPerceivable owner, IMagicSpellEffectParent parent, string sdescAddendum,
		string descAddendum, ANSIColour colour, double glowLux, double attackCheckBonus, double qualityBonus,
		double damageBonus, double painBonus, double stunBonus, double armourDamageReduction,
		double projectileQualityBonus = 0.0, double projectileDamageBonus = 0.0, double projectilePainBonus = 0.0,
		double projectileStunBonus = 0.0, double toolFitnessBonus = 0.0, double toolSpeedMultiplier = 1.0,
		double toolUsageMultiplier = 1.0, double powerProductionMultiplier = 1.0,
		double powerConsumptionMultiplier = 1.0, double fuelUseMultiplier = 1.0,
		EventType? itemEventType = null, IFutureProg? itemEventProg = null, IFutureProg? prog = null)
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
		ProjectileQualityBonus = projectileQualityBonus;
		ProjectileDamageBonus = projectileDamageBonus;
		ProjectilePainBonus = projectilePainBonus;
		ProjectileStunBonus = projectileStunBonus;
		ToolFitnessBonus = toolFitnessBonus;
		ToolSpeedMultiplier = toolSpeedMultiplier;
		ToolUsageMultiplier = toolUsageMultiplier;
		PowerProductionMultiplier = powerProductionMultiplier;
		PowerConsumptionMultiplier = powerConsumptionMultiplier;
		FuelUseMultiplier = fuelUseMultiplier;
		ItemEventType = itemEventType;
		ItemEventProg = itemEventProg;
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
		ProjectileQualityBonus = double.Parse(trueRoot?.Element("ProjectileQualityBonus")?.Value ?? "0");
		ProjectileDamageBonus = double.Parse(trueRoot?.Element("ProjectileDamageBonus")?.Value ?? "0");
		ProjectilePainBonus = double.Parse(trueRoot?.Element("ProjectilePainBonus")?.Value ?? "0");
		ProjectileStunBonus = double.Parse(trueRoot?.Element("ProjectileStunBonus")?.Value ?? "0");
		ToolFitnessBonus = double.Parse(trueRoot?.Element("ToolFitnessBonus")?.Value ?? "0");
		ToolSpeedMultiplier = double.Parse(trueRoot?.Element("ToolSpeedMultiplier")?.Value ?? "1");
		ToolUsageMultiplier = double.Parse(trueRoot?.Element("ToolUsageMultiplier")?.Value ?? "1");
		PowerProductionMultiplier = double.Parse(trueRoot?.Element("PowerProductionMultiplier")?.Value ?? "1");
		PowerConsumptionMultiplier = double.Parse(trueRoot?.Element("PowerConsumptionMultiplier")?.Value ?? "1");
		FuelUseMultiplier = double.Parse(trueRoot?.Element("FuelUseMultiplier")?.Value ?? "1");
		var eventValue = int.Parse(trueRoot?.Element("ItemEventType")?.Value ?? "-1");
		ItemEventType = eventValue < 0 ? null : (EventType)eventValue;
		ItemEventProg = Gameworld.FutureProgs.Get(long.Parse(trueRoot?.Element("ItemEventProg")?.Value ?? "0"));
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
	public double ProjectileQualityBonus { get; }
	public double ProjectileDamageBonus { get; }
	public double ProjectilePainBonus { get; }
	public double ProjectileStunBonus { get; }
	public double ToolFitnessBonus { get; }
	public double ToolSpeedMultiplier { get; }
	public double ToolUsageMultiplier { get; }
	public double PowerProductionMultiplier { get; }
	public double PowerConsumptionMultiplier { get; }
	public double FuelUseMultiplier { get; }
	public EventType? ItemEventType { get; }
	public IFutureProg? ItemEventProg { get; }

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
			new XElement("ArmourDamageReduction", ArmourDamageReduction),
			new XElement("ProjectileQualityBonus", ProjectileQualityBonus),
			new XElement("ProjectileDamageBonus", ProjectileDamageBonus),
			new XElement("ProjectilePainBonus", ProjectilePainBonus),
			new XElement("ProjectileStunBonus", ProjectileStunBonus),
			new XElement("ToolFitnessBonus", ToolFitnessBonus),
			new XElement("ToolSpeedMultiplier", ToolSpeedMultiplier),
			new XElement("ToolUsageMultiplier", ToolUsageMultiplier),
			new XElement("PowerProductionMultiplier", PowerProductionMultiplier),
			new XElement("PowerConsumptionMultiplier", PowerConsumptionMultiplier),
			new XElement("FuelUseMultiplier", FuelUseMultiplier),
			new XElement("ItemEventType", ItemEventType.HasValue ? (int)ItemEventType.Value : -1),
			new XElement("ItemEventProg", ItemEventProg?.Id ?? 0L)
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

	public bool AppliesToProjectileAttack(ICharacter attacker, IPerceiver target, IGameItem projectile)
	{
		return ReferenceEquals(Owner, projectile) &&
		       (ApplicabilityProg?.ExecuteBool(projectile, attacker, target) ?? true);
	}

	public bool AppliesToCraftTool(IGameItem tool, ITag? toolTag)
	{
		return ReferenceEquals(Owner, tool) &&
		       (ApplicabilityProg?.ExecuteBool(tool) ?? true);
	}

	public bool AppliesToPoweredItem(IGameItem item)
	{
		return ReferenceEquals(Owner, item) &&
		       (ApplicabilityProg?.ExecuteBool(item) ?? true);
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

	public bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		if (ItemEventProg is null || (ItemEventType.HasValue && ItemEventType.Value != type) || Owner is not IGameItem item)
		{
			return false;
		}

		if (ItemEventProg.MatchesParameters([ProgVariableTypes.Item, ProgVariableTypes.Text]))
		{
			ItemEventProg.Execute(item, type.DescribeEnum());
		}
		else if (ItemEventProg.MatchesParameters([ProgVariableTypes.Item]))
		{
			ItemEventProg.Execute(item);
		}

		return false;
	}

	public bool HandlesEvent(params EventType[] types)
	{
		return ItemEventProg is not null && (!ItemEventType.HasValue || types.Contains(ItemEventType.Value));
	}

	protected override string SpecificEffectType => "SpellItemEnchantment";
}
