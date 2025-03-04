using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Character;
using MudSharp.Construction;

namespace MudSharp.GameItems.Components;

internal class LiquidContainerGameItemComponent : GameItemComponent, ILiquidContainer, ILockable
{
	protected LiquidContainerGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

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
		//Call prog for adjusting fluids (container, changed, final, who, action)
		_prototype.AdjustQuantityProg?.Execute(Parent, amount, LiquidMixture.TotalVolume, who, action);
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
		if (LiquidMixture == null)
		{
			LiquidMixture = otherMixture;
		}
		else
		{
			LiquidMixture.AddLiquid(otherMixture);
		}

		//Call prog for adjusting fluids (container, changed, final, who, action)
		_prototype.AdjustQuantityProg?.Execute(Parent, otherMixture.TotalVolume, LiquidMixture.TotalVolume, who,
			action);
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
		_prototype.AdjustQuantityProg?.Execute(Parent, newMixture.TotalVolume, LiquidMixture.TotalVolume, who, action);
		if (LiquidMixture.IsEmpty)
		{
			LiquidMixture = null;
		}

		return newMixture;
	}

	public bool CanBeEmptiedWhenInRoom => _prototype.CanBeEmptiedWhenInRoom;

	public double LiquidVolume => LiquidMixture?.TotalVolume ?? 0.0;

	public double LiquidCapacity => _prototype.LiquidCapacity;
	public override int DecorationPriority => -1;

	public override void Delete()
	{
		base.Delete();
		foreach (var item in Locks.ToList())
		{
			_locks.Remove(item);
			item.Parent.Delete();
		}
	}

	public override void Quit()
	{
		foreach (var item in Locks)
		{
			item.Quit();
		}
	}

	public override void Login()
	{
		foreach (var item in Locks)
		{
			item.Login();
		}
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return ((IsOpen || _prototype.Transparent) &&
		        (type == DescriptionType.Contents || type == DescriptionType.Short)) ||
		       type == DescriptionType.Full || type == DescriptionType.Evaluate;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		switch (type)
		{
			case DescriptionType.Contents:
				if (LiquidMixture?.IsEmpty == false)
				{
					if (voyeur is ICharacter ch && ch.IsAdministrator())
					{
						return
							$"It is {(LiquidMixture.TotalVolume / LiquidCapacity).ToString("P2", ch)} full of {LiquidMixture.ColouredLiquidLongDescription}";
					}
					else
					{
						return string.Format(voyeur, "\n\nIt is approximately {0:P0} full of {1}.",
							Math.Round(20 * LiquidMixture.TotalVolume / LiquidCapacity) / 20,
							LiquidMixture.ColouredLiquidLongDescription);
					}
				}

				return description + "\n\nIt is currently empty.";
			case DescriptionType.Short:
				if (LiquidMixture?.IsEmpty == false && !flags.HasFlag(PerceiveIgnoreFlags.IgnoreLiquidsAndFlags))
				{
					return $"{description} filled with {LiquidMixture.ColouredLiquidDescription}";
				}

				return description;
			case DescriptionType.Full:
				var sb = new StringBuilder();
				sb.Append(description);
				if (Locks.Any())
				{
					sb.AppendLine();
					sb.AppendLine("It has the following locks:");
					foreach (var thelock in Locks)
					{
						sb.AppendLineFormat("\t{0}", thelock.Parent.HowSeen(voyeur));
					}
				}

				return sb.ToString();
			case DescriptionType.Evaluate:
				return
					$"{description}\nIt can hold {Gameworld.UnitManager.DescribeMostSignificantExact(LiquidCapacity, Framework.Units.UnitType.FluidVolume, voyeur).ColourValue()} or {Gameworld.UnitManager.DescribeMostSignificantExact(_prototype.WeightLimit, Framework.Units.UnitType.Mass, voyeur).ColourValue()} of liquid, whichever is smaller.";
		}

		return description;
	}

	public override double ComponentWeight
	{
		get { return Locks.Sum(x => x.Parent.Weight) + (LiquidMixture?.TotalWeight ?? 0.0); }
	}

	public override double ComponentBuoyancy(double fluidDensity)
	{
		return Locks.Sum(x => x.Parent.Buoyancy(fluidDensity)) +
			LiquidMixture?.Instances.Sum(x => (fluidDensity - x.Liquid.Density) * x.Amount * x.Liquid.Density) ?? 0.0;
	}

	public override bool HandleDieOrMorph(IGameItem newItem, ICell location)
	{
		var newItemLockable = newItem?.GetItemType<ILockable>();
		if (newItemLockable != null)
		{
			foreach (var thelock in Locks.ToList())
			{
				newItemLockable.InstallLock(thelock);
			}
		}
		else
		{
			foreach (var thelock in Locks.ToList())
			{
				if (location != null)
				{
					location.Insert(thelock.Parent);
					thelock.Parent.ContainedIn = null;
				}
				else
				{
					thelock.Parent.Delete();
				}
			}
		}

		_locks.Clear();

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
		if (_locks.Any(x => x.Parent == existingItem) && newItem.IsItemType<ILock>())
		{
			_locks[_locks.IndexOf(existingItem.GetItemType<ILock>())] = newItem.GetItemType<ILock>();
			existingItem.ContainedIn = null;
			newItem.ContainedIn = Parent;
			Changed = true;
			return true;
		}

		return false;
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (LiquidContainerGameItemComponentProto)newProto;
		if (LiquidMixture != null && LiquidMixture.TotalVolume > LiquidCapacity)
		{
			LiquidMixture.SetLiquidVolume(LiquidCapacity);
		}

		if (_prototype.Closable && IsOpen)
		{
			IsOpen = false;
		}
	}

	#region Overrides of GameItemComponent

	/// <summary>
	///     This function gives the IGameItemComponent the opportunity to object to a proposed merge of two IGameItems based on
	///     a particular IGameItemComponent of the other item
	///     e.g. Coloured items with different colours, food items with different bites left, etc.
	/// </summary>
	/// <param name="component">A Component from the other IGameItem</param>
	/// <returns>True if this component objects to the merger</returns>
	public override bool PreventsMerging(IGameItemComponent component)
	{
		return LiquidMixture != null ||
		       (_prototype.OnceOnly && IsOpen)
			;
	}

	#endregion

	#region IOpenable Members

	private bool _isOpen = true;

	public bool IsOpen
	{
		get => _isOpen;
		protected set
		{
			_isOpen = value;
			Changed = true;
		}
	}

	public bool CanOpen(IBody opener)
	{
		return _prototype.Closable && !IsOpen && Locks.All(x => !x.IsLocked) &&
		       Parent.EffectsOfType<IOverrideLockEffect>().All(x => !x.Applies(opener?.Actor));
	}

	public WhyCannotOpenReason WhyCannotOpen(IBody opener)
	{
		if (!_prototype.Closable)
		{
			return WhyCannotOpenReason.NotOpenable;
		}

		if (IsOpen)
		{
			return WhyCannotOpenReason.AlreadyOpen;
		}

		return Locks.Any(x => x.IsLocked) ||
		       Parent.EffectsOfType<IOverrideLockEffect>().Any(x => x.Applies(opener?.Actor))
			? WhyCannotOpenReason.Locked
			: WhyCannotOpenReason.Unknown;
	}

	public void Open()
	{
		IsOpen = true;
		OnOpen?.Invoke(this);
	}

	public bool CanClose(IBody closer)
	{
		return _prototype.Closable && !_prototype.OnceOnly && IsOpen;
	}

	public WhyCannotCloseReason WhyCannotClose(IBody closer)
	{
		if (!_prototype.Closable)
		{
			return WhyCannotCloseReason.NotOpenable;
		}

		if (!IsOpen)
		{
			return WhyCannotCloseReason.AlreadyClosed;
		}

		return _prototype.OnceOnly ? WhyCannotCloseReason.SingleUse : WhyCannotCloseReason.Unknown;
	}

	public void Close()
	{
		IsOpen = false;
		OnClose?.Invoke(this);
	}

	public event OpenableEvent OnOpen;
	public event OpenableEvent OnClose;

	#endregion

	#region Constructors

	public LiquidContainerGameItemComponent(LiquidContainerGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
		if (_prototype.Closable)
		{
			IsOpen = false;
		}

		if (_prototype.DefaultLiquid != null)
		{
			LiquidMixture = new LiquidMixture(_prototype.DefaultLiquid, _prototype.LiquidCapacity, Gameworld);
		}
	}

	public LiquidContainerGameItemComponent(MudSharp.Models.GameItemComponent component,
		LiquidContainerGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	private void LoadFromXml(XElement root)
	{
		var attr = root.Attribute("Open");
		if (attr != null)
		{
			_isOpen = attr.Value == "true";
		}

		var lockelem = root.Element("Locks");
		if (lockelem != null)
		{
			foreach (
				var item in
				lockelem.Elements("Lock")
				        .Select(element => Gameworld.TryGetItem(long.Parse(element.Value), true))
				        .Where(item => item != null && item.IsItemType<ILock>()))
			{
				item.Get(null);
				InstallLock(item.GetItemType<ILock>());
			}
		}

		// Legacy
		attr = root.Attribute("Liquid");
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
		return new LiquidContainerGameItemComponent(this, newParent, temporary);
	}

	protected override string SaveToXml()
	{
		return
			new XElement("Definition", new XAttribute("Open", IsOpen.ToString().ToLowerInvariant()),
					LiquidMixture?.SaveToXml() ?? new XElement("NoLiquid"),
					new XElement("Locks", from thelock in Locks select new XElement("Lock", thelock.Parent.Id)))
				.ToString();
	}

	public LiquidContainerGameItemComponent(LiquidContainerGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_isOpen = rhs._isOpen;
		if (rhs.LiquidMixture != null)
		{
			LiquidMixture = new LiquidMixture(rhs.LiquidMixture);
		}
	}

	public override void FinaliseLoad()
	{
		foreach (var item in Locks)
		{
			item.Parent.FinaliseLoadTimeTasks();
		}
	}

	#endregion

	#region ILockable Members

	private readonly List<ILock> _locks = new();
	public IEnumerable<ILock> Locks => _locks;

	public bool InstallLock(ILock theLock, ICharacter actor = null)
	{
		_locks.Add(theLock);
		if (_noSave)
		{
			theLock.Parent.LoadTimeSetContainedIn(Parent);
		}
		else
		{
			theLock.Parent.ContainedIn = Parent;
		}

		Changed = true;
		return true;
	}

	public bool RemoveLock(ILock theLock)
	{
		if (_locks.Contains(theLock))
		{
			theLock.Parent.ContainedIn = null;
			_locks.Remove(theLock);
			Changed = true;
			return true;
		}

		return false;
	}

	#endregion
}