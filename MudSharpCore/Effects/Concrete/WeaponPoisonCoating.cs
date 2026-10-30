using MudSharp.Form.Material;
using MudSharp.GameItems;

namespace MudSharp.Effects.Concrete;

public class WeaponPoisonCoating : Effect, IWeaponPoisonCoatingEffect, IEffectAddsWeight
{
	private static TimeSpan BaseEffectDuration =>
		TimeSpan.FromSeconds(Futuremud.Games.First().GetStaticDouble("LiquidContaminationEffectDuration"));

	public static TimeSpan EffectDuration(LiquidMixture mixture)
	{
		return BaseEffectDuration * mixture.RelativeEnthalpy;
	}

	private LiquidMixture _contaminatingLiquid;

	public WeaponPoisonCoating(IPerceivable owner, LiquidMixture liquid, IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
	{
		ContaminatingLiquid = liquid;
	}

	public WeaponPoisonCoating(IPerceivable owner, LiquidMixture liquid, IFutureProg applicabilityProg,
		IFutureProg deliveryProg, bool singleUseOnWound) : this(owner, liquid, applicabilityProg)
	{
		DeliveryProg = deliveryProg;
		SingleUseOnWound = singleUseOnWound;
	}

	public IFutureProg DeliveryProg { get; private set; }
	public bool SingleUseOnWound { get; private set; }

	protected WeaponPoisonCoating(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		LoadFromDb(effect.Element("Effect"));
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("WeaponPoisonCoating", (effect, owner) => new WeaponPoisonCoating(effect, owner));
	}

	public LiquidMixture ContaminatingLiquid
	{
		get => _contaminatingLiquid;
		private set
		{
			if (_contaminatingLiquid is not null)
			{
				_contaminatingLiquid.OnLiquidMixtureChanged -= ContaminatingLiquidOnLiquidMixtureChanged;
			}

			_contaminatingLiquid = value;
			if (_contaminatingLiquid is not null)
			{
				_contaminatingLiquid.OnLiquidMixtureChanged += ContaminatingLiquidOnLiquidMixtureChanged;
			}
		}
	}

	private void LoadFromDb(XElement root)
	{
		DeliveryProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("DeliveryProg")?.Value ?? "0"));
		SingleUseOnWound = bool.Parse(root.Element("SingleUseOnWound")?.Value ?? "false");
		if (root.Element("Liquid") != null)
		{
			ContaminatingLiquid = new LiquidMixture(new[]
			{
				new LiquidInstance
				{
					Liquid = Gameworld.Liquids.Get(long.Parse(root.Element("Liquid")?.Value ?? "0")),
					Amount = double.Parse(root.Element("Quantity")?.Value ?? "0")
				}
			}, Gameworld);
			return;
		}

		ContaminatingLiquid = new LiquidMixture(root.Element("Mix"), Gameworld);
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect", ContaminatingLiquid.SaveToXml(),
			new XElement("DeliveryProg", DeliveryProg?.Id ?? 0), new XElement("SingleUseOnWound", SingleUseOnWound));
	}

	protected override string SpecificEffectType { get; } = "WeaponPoisonCoating";

	public override bool SavingEffect => true;

	public override bool PreventsItemFromMerging(IGameItem effectOwnerItem, IGameItem targetItem)
	{
		return true;
	}

	public override string Describe(IPerceiver voyeur)
	{
		if (DeliveryProg is not null)
			return $"Weapon poison coating delivered by {DeliveryProg.FunctionName}, single use: {SingleUseOnWound.ToColouredString()}.";
		return
			$"Weapon poison coating with {ContaminatingLiquid.Instances.Select(x => x.Liquid.Name).Distinct().ListToString()} {Gameworld.UnitManager.Describe(ContaminatingLiquid.TotalVolume, Framework.Units.UnitType.FluidVolume, voyeur)}";
	}

	public override IEffect NewEffectOnItemMorph(IGameItem oldItem, IGameItem newItem)
	{
		return oldItem == Owner ? new WeaponPoisonCoating(newItem, ContaminatingLiquid.Clone(), ApplicabilityProg, DeliveryProg, SingleUseOnWound) : null;
	}

	public void AddLiquid(LiquidMixture liquid)
	{
		if (liquid.IsEmpty)
		{
			return;
		}

		ContaminatingLiquid.AddLiquid(liquid.Clone());
		Changed = true;
	}

	public LiquidMixture RemovePoisonVolume(double amount)
	{
		var removed = ContaminatingLiquid.RemoveLiquidVolume(amount);
		Changed = true;
		return removed;
	}

	public override void RemovalEffect()
	{
		if (_contaminatingLiquid is not null)
		{
			_contaminatingLiquid.OnLiquidMixtureChanged -= ContaminatingLiquidOnLiquidMixtureChanged;
		}
	}

	private void ContaminatingLiquidOnLiquidMixtureChanged(LiquidMixture mixture)
	{
		if (mixture.IsEmpty)
		{
			Owner.RemoveEffect(this, true);
			return;
		}

		Changed = true;
	}

	public double AddedWeight => ContaminatingLiquid?.TotalWeight ?? 0.0;
}
