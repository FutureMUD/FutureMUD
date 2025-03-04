using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Body.Needs;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.Character;
using MudSharp.Construction;

namespace MudSharp.GameItems.Components;

public class SyringeGameItemComponent : GameItemComponent, ILiquidContainer, IInject
{
	protected SyringeGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	public bool CanBeEmptiedWhenInRoom => true;

	private LiquidMixture _liquidMixture;

	public LiquidMixture LiquidMixture
	{
		get => _liquidMixture;
		set
		{
			_liquidMixture = value;
			Changed = true;
		}
	}

	private void AdjustLiquidQuantity(double amount, ICharacter who, string action)
	{
		if (LiquidMixture == null)
		{
			return;
		}

		LiquidMixture.AddLiquidVolume(amount);
		if (LiquidMixture.IsEmpty)
		{
			LiquidMixture = null;
		}

		Changed = true;
	}

	public void AddLiquidQuantity(double amount, ICharacter who, string action)
	{
		if (LiquidMixture == null)
		{
			return;
		}

		if (LiquidMixture.TotalVolume + amount > LiquidCapacity)
		{
			amount = LiquidCapacity - LiquidMixture.TotalVolume;
		}

		if (LiquidMixture.TotalVolume + amount < 0)
		{
			amount = -1 * LiquidMixture.TotalVolume;
		}

		AdjustLiquidQuantity(amount, who, action);
	}

	public void ReduceLiquidQuantity(double amount, ICharacter who, string action)
	{
		if (LiquidMixture == null)
		{
			return;
		}

		if (LiquidMixture.TotalVolume - amount > LiquidCapacity)
		{
			amount = (LiquidCapacity - LiquidMixture.TotalVolume) * -1;
		}

		if (LiquidMixture.TotalVolume - amount < 0)
		{
			amount = LiquidMixture.TotalVolume;
		}

		AdjustLiquidQuantity(amount * -1, who, action);
	}

	public void MergeLiquid(LiquidMixture otherMixture, ICharacter who, string action)
	{
		if (otherMixture == null)
		{
			return;
		}

		if (LiquidMixture == null)
		{
			LiquidMixture = otherMixture;
		}
		else
		{
			LiquidMixture.AddLiquid(otherMixture);
		}

		if (LiquidMixture.IsEmpty)
		{
			LiquidMixture = null;
		}

		Changed = true;
	}

	public LiquidMixture RemoveLiquidAmount(double amount, ICharacter who, string action)
	{
		if (LiquidMixture == null)
		{
			return null;
		}

		var newMixture = LiquidMixture.RemoveLiquidVolume(amount);
		Changed = true;
		if (LiquidMixture.IsEmpty)
		{
			LiquidMixture = null;
		}

		return newMixture;
	}

	public double LiquidVolume => LiquidMixture?.TotalVolume ?? 0.0;

	public double LiquidCapacity => _prototype.LiquidCapacity;
	public override int DecorationPriority => -1;

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return (IsOpen || _prototype.Transparent) &&
		       (type == DescriptionType.Contents || type == DescriptionType.Short);
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		switch (type)
		{
			case DescriptionType.Contents:
				if (LiquidMixture?.IsEmpty == false)
				{
					return string.Format(voyeur, "\n\nIt is approximately {0:P0} full of {1}.",
						Math.Round(20 * LiquidMixture.TotalVolume / LiquidCapacity) / 20,
						LiquidMixture.ColouredLiquidLongDescription);
				}

				return description + "\n\nIt is currently empty.";
			case DescriptionType.Short:
				if (LiquidMixture?.IsEmpty == false && !flags.HasFlag(PerceiveIgnoreFlags.IgnoreLiquidsAndFlags))
				{
					return $"{description} filled with {LiquidMixture.ColouredLiquidDescription}";
				}

				return description;
		}

		return description;
	}

	public override double ComponentWeight
		=> LiquidMixture?.TotalWeight ?? 0.0;

	public override double ComponentBuoyancy(double fluidDensity)
	{
		return LiquidMixture?.Instances.Sum(x => (fluidDensity - x.Liquid.Density) * x.Amount * x.Liquid.Density) ??
		       0.0;
	}

	public override bool HandleDieOrMorph(IGameItem newItem, ICell location)
	{
		if (LiquidMixture == null)
		{
			return false;
		}

		var newItemLiquid = newItem?.GetItemType<ILiquidContainer>();
		if (newItemLiquid != null)
		{
			newItemLiquid.LiquidMixture = LiquidMixture;
			newItemLiquid.Changed = true;
		}

		return false;
	}

	public override bool SwapInPlace(IGameItem existingItem, IGameItem newItem)
	{
		return false;
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (SyringeGameItemComponentProto)newProto;
		if (LiquidMixture != null && LiquidMixture.TotalVolume > LiquidCapacity)
		{
			LiquidMixture.SetLiquidVolume(LiquidCapacity);
		}
	}

	#region IOpenable Members

	public bool IsOpen => true;

	public bool CanOpen(IBody opener)
	{
		return false;
	}

	public WhyCannotOpenReason WhyCannotOpen(IBody opener)
	{
		return WhyCannotOpenReason.NotOpenable;
	}

	public void Open()
	{
		// Do nothing
	}

	public bool CanClose(IBody closer)
	{
		return false;
	}

	public WhyCannotCloseReason WhyCannotClose(IBody closer)
	{
		return WhyCannotCloseReason.NotOpenable;
	}

	public void Close()
	{
		// Do nothing
	}

	public event OpenableEvent OnOpen;
	public event OpenableEvent OnClose;

	#endregion

	#region Constructors

	public SyringeGameItemComponent(SyringeGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public SyringeGameItemComponent(MudSharp.Models.GameItemComponent component,
		SyringeGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	private void LoadFromXml(XElement root)
	{
		// Legacy
		var attr = root.Attribute("Liquid");
		if (attr != null)
		{
			var liquid = Gameworld.Liquids.Get(long.Parse(attr.Value));
			if (liquid != null)
			{
				LiquidMixture = new LiquidMixture(new[]
				{
					new LiquidInstance
					{
						Liquid = liquid,
						Amount = double.Parse(root.Attribute("LiquidQuantity").Value)
					}
				}, Gameworld);
			}
		}
		// Non-Legacy
		else
		{
			if (root.Element("Mix") != null)
			{
				LiquidMixture = new LiquidMixture(root.Element("Mix"), Gameworld);
			}
		}
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new SyringeGameItemComponent(this, newParent, temporary);
	}

	protected override string SaveToXml()
	{
		return
			new XElement("Definition", LiquidMixture?.SaveToXml() ?? new XElement("NoLiquid"))
				.ToString();
	}

	public WhyCannotInject CanInject(IBody target, IBodypart part)
	{
		if (LiquidMixture?.IsEmpty != false)
		{
			return WhyCannotInject.CannotInjectEmpty;
		}

		return target.WornItemsProfilesFor(part).Any(x => x.Item2.PreventsRemoval && !x.Item2.NoArmour)
			? WhyCannotInject.CannotInjectNoAccessToPart
			: WhyCannotInject.CanInject;
	}

	public void Inject(IBody target, IBodypart part, ICharacter injector)
	{
		Inject(target, part, LiquidMixture.TotalVolume, injector);
	}

	public void Inject(IBody target, IBodypart part, double amount, ICharacter injector)
	{
		if (amount == 0.0)
		{
			amount = LiquidMixture.TotalVolume;
		}

		target.HealthStrategy.InjectedLiquid(target.Actor, RemoveLiquidAmount(amount, injector, "inject"));
	}

	public SyringeGameItemComponent(SyringeGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		if (rhs.LiquidMixture != null)
		{
			LiquidMixture = new LiquidMixture(rhs.LiquidMixture);
		}
	}

	#endregion
}