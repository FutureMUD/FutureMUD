using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class ImplantLiquidContainerGameItemComponent : ImplantBaseGameItemComponent, ILiquidContainer, ILockable,
	IImplantReportStatus
{
	protected ImplantLiquidContainerGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;
	public bool CanBeEmptiedWhenInRoom => true;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (ImplantLiquidContainerGameItemComponentProto)newProto;
		if (LiquidMixture != null && LiquidMixture.TotalVolume > LiquidCapacity)
		{
			LiquidMixture.SetLiquidVolume(LiquidCapacity);
		}

		if (_prototype.Closable && IsOpen)
		{
			IsOpen = false;
		}

		base.UpdateComponentNewPrototype(newProto);
	}

	#region Constructors

	public ImplantLiquidContainerGameItemComponent(ImplantLiquidContainerGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(proto, parent, temporary)
	{
		_prototype = proto;
		if (_prototype.Closable)
		{
			IsOpen = false;
		}
	}

	public ImplantLiquidContainerGameItemComponent(MudSharp.Models.GameItemComponent component,
		ImplantLiquidContainerGameItemComponentProto proto, IGameItem parent) : base(component, proto, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public ImplantLiquidContainerGameItemComponent(ImplantLiquidContainerGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_isOpen = rhs._isOpen;
		if (rhs.LiquidMixture != null)
		{
			LiquidMixture = new LiquidMixture(rhs.LiquidMixture);
		}
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
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
				if ((item.ContainedIn != null && item.ContainedIn != Parent) || item.Location != null ||
				    item.InInventoryOf != null)
				{
					Changed = true;
					Gameworld.SystemMessage(
						$"Duplicated Item: {item.HowSeen(item, colour: false, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreLoadThings)} {item.Id.ToString("N0")}",
						true);
					continue;
				}

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
		return new ImplantLiquidContainerGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		var basevalue = SaveToXmlNoTextConversion();
		basevalue.Add(new XAttribute("Open", IsOpen.ToString().ToLowerInvariant()),
			LiquidMixture?.SaveToXml() ?? new XElement("NoLiquid"),
			new XElement("Locks", from thelock in Locks select new XElement("Lock", thelock.Parent.Id)));
		return basevalue.ToString();
	}

	#endregion

	#region ILiquidContainer Members

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

	public double LiquidVolume => LiquidMixture?.TotalVolume ?? 0.0;
	public double LiquidCapacity => _prototype.LiquidCapacity;

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
		return _prototype.Closable && IsOpen;
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

		return WhyCannotCloseReason.Unknown;
	}

	public void Close()
	{
		IsOpen = false;
		OnClose?.Invoke(this);
	}

	public event OpenableEvent OnOpen;
	public event OpenableEvent OnClose;

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

	#region GameItemComponent Overrides

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
		        (type == DescriptionType.Contents || (type == DescriptionType.Short && InstalledBody == null))) ||
		       type == DescriptionType.Full;
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
		}

		return description;
	}

	public override double ComponentWeight
	{
		get
		{
			return Locks.Sum(x => x.Parent.Weight) +
			       (LiquidMixture?.TotalWeight ?? 0.0);
		}
	}

	public override double ComponentBuoyancy(double fluidDensity)
	{
		return Locks.Sum(x => x.Parent.Buoyancy(fluidDensity)) +
			LiquidMixture?.Instances.Sum(x => (fluidDensity - x.Liquid.Density) * x.Amount * x.Liquid.Density) ?? 0.0;
	}

	public override bool Die(IGameItem newItem, ICell location)
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

	public override void FinaliseLoad()
	{
		foreach (var item in Locks)
		{
			item.Parent.FinaliseLoadTimeTasks();
		}
	}

	#endregion

	#region IImplantReportStatus

	public string ReportStatus()
	{
		if (!_powered)
		{
			return "\t* Implant is unpowered and non-functional.";
		}

		return
			$"Implant is a liquid container, designed to hold liquids.\n\t\t* {(LiquidMixture?.IsEmpty != false ? "It is currently empty" : $"It is currently filled to {(LiquidMixture.TotalVolume / LiquidCapacity).ToString("P2", InstalledBody.Actor)} capacity.")}";
	}

	#endregion
}