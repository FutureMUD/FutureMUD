using MudSharp.Effects;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using System;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

public class WeaponPoisonCoating : LiquidContamination, IWeaponPoisonCoatingEffect
{
	public WeaponPoisonCoating(IPerceivable owner, LiquidMixture liquid, IFutureProg applicabilityProg = null) : base(owner, liquid, applicabilityProg)
	{
	}

	protected WeaponPoisonCoating(XElement effect, IPerceivable owner) : base(effect, owner)
	{
	}

	public new static void InitialiseEffectType()
	{
		RegisterFactory("WeaponPoisonCoating", (effect, owner) => new WeaponPoisonCoating(effect, owner));
	}

	protected override double LiquidCapacityMultiplier(IGameItem ownerItem)
	{
		return Math.Max(1, ownerItem.Quantity);
	}

	protected override string SpecificEffectType { get; } = "WeaponPoisonCoating";

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Weapon poison coating with {ContaminatingLiquid.Instances.Select(x => x.Liquid.Name).Distinct().ListToString()} {Gameworld.UnitManager.Describe(ContaminatingLiquid.TotalVolume, Framework.Units.UnitType.FluidVolume, voyeur)}";
	}

	public override IEffect NewEffectOnItemMorph(IGameItem oldItem, IGameItem newItem)
	{
		if (oldItem == Owner)
		{
			return new WeaponPoisonCoating(newItem, ContaminatingLiquid.Clone(), ApplicabilityProg);
		}

		return null;
	}

	public LiquidMixture RemovePoisonVolume(double amount)
	{
		var removed = ContaminatingLiquid.RemoveLiquidVolume(amount);
		Changed = true;
		return removed;
	}
}
