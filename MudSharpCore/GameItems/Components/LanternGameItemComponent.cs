using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Form.Material;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Construction;
using MudSharp.Form.Shape;

namespace MudSharp.GameItems.Components;

public class LanternGameItemComponent : GameItemComponent, ILiquidContainer, ILockable, ILightable, IProduceLight
{
	protected LanternGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (LanternGameItemComponentProto)newProto;
	}

	#region Constructors
	public LanternGameItemComponent(LanternGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
		IsOpen = false;
		_lit = false;
	}

	public LanternGameItemComponent(Models.GameItemComponent component, LanternGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public LanternGameItemComponent(LanternGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		LiquidMixture = rhs.LiquidMixture?.Clone();
		_lit = rhs.Lit;
		IsOpen = rhs.IsOpen;
	}

	protected void LoadFromXml(XElement root)
	{
		var attr = root.Attribute("Open");
		if (attr != null)
		{
			_isOpen = bool.Parse(attr.Value);
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

		if (root.Element("Mix") != null)
		{
			LiquidMixture = new LiquidMixture(root.Element("Mix"), Gameworld);
		}

		_lit = bool.Parse(root.Element("Lit").Value);
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new LanternGameItemComponent(this, newParent, temporary);
	}
	#endregion

	#region Saving
	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XAttribute("Open", IsOpen.ToString().ToLowerInvariant()),
			new XElement("Lit", Lit),
				LiquidMixture?.SaveToXml() ?? new XElement("NoLiquid"),
				new XElement("Locks", from thelock in Locks select new XElement("Lock", thelock.Parent.Id))
			).ToString();
	}
	#endregion

	public override void FinaliseLoad()
	{
		foreach (var item in Locks)
		{
			item.Parent.FinaliseLoadTimeTasks();
		}
	}

	public override double ComponentWeight
	{
		get { return Locks.Sum(x => x.Parent.Weight) + LiquidMixture?.TotalWeight ?? 0.0; }
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
			foreach (var thelock in Locks)
			{
				newItemLockable.InstallLock(thelock);
			}
		}
		else
		{
			foreach (var thelock in Locks)
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
		base.Quit();
		foreach (var item in Locks)
		{
			item.Quit();
		}

		if (Lit)
		{
			Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat -= HeartbeatManager_FiveSecondHeartbeat;
		}
	}

	/// <inheritdoc />
	public override void Login()
	{
		base.Login();
		foreach (var item in Locks)
		{
			item.Login();
		}
		if (Lit)
		{
			Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat += HeartbeatManager_FiveSecondHeartbeat;
		}
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return (type == DescriptionType.Contents && IsOpen) || type == DescriptionType.Short ||
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
				return $"{description}{(Lit ? " (lit)".FluentColour(Telnet.Red, colour) : "")}";
			case DescriptionType.Full:
				var sb = new StringBuilder();
				sb.Append(description);
				sb.AppendLine();
				sb.AppendLine($"It is{(Lit ? "" : " not")} lit.".Colour(Telnet.Yellow));
				sb.AppendLine($"It is able to be opened and closed, and is currently {(IsOpen ? "open" : "closed")}."
					.Colour(Telnet.Yellow));
				if (Locks.Any())
				{
					sb.AppendLine("It has the following locks:");
					foreach (var thelock in Locks)
					{
						sb.AppendLineFormat("\t{0}", thelock.Parent.HowSeen(voyeur));
					}
				}

				return sb.ToString();
			case DescriptionType.Evaluate:
				return
					$"{description}\nIt can hold {Gameworld.UnitManager.DescribeMostSignificantExact(LiquidCapacity, Framework.Units.UnitType.FluidVolume, voyeur).ColourValue()} of liquid.";
		}

		throw new NotSupportedException("Invalid Decorate type in LanternGameItemComponent.Decorate");
	}

	public override int DecorationPriority => int.MaxValue;

	public override bool PreventsMerging(IGameItemComponent component)
	{
		return true;
	}

	#region Implementation of IOpenable

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
		return !IsOpen && Locks.All(x => !x.IsLocked) &&
		       Parent.EffectsOfType<IOverrideLockEffect>().All(x => !x.Applies(opener?.Actor));
	}

	public WhyCannotOpenReason WhyCannotOpen(IBody opener)
	{
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
		return IsOpen;
	}

	public WhyCannotCloseReason WhyCannotClose(IBody closer)
	{
		return !IsOpen ? WhyCannotCloseReason.AlreadyClosed : WhyCannotCloseReason.Unknown;
	}

	public void Close()
	{
		IsOpen = false;
		OnClose?.Invoke(this);
	}

	public event OpenableEvent OnOpen;
	public event OpenableEvent OnClose;

	#endregion

	#region Implementation of ILiquidContainer

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

	public bool CanBeEmptiedWhenInRoom => true;

	public double LiquidVolume => LiquidMixture?.TotalVolume ?? 0.0;

	public double LiquidCapacity => _prototype.FuelCapacity;

	#endregion

	#region Implementation of ILockable

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

	#region Implementation of ILightable

	private bool _lit;

	/// <inheritdoc />
	public bool Lit
	{
		get => _lit;
		set
		{
			_lit = value;
			Changed = true;
		}
	}

	public virtual bool CanLight(ICharacter lightee, IPerceivable ignitionSource)
	{
		// TODO ignition sources
		if (!(Parent.Location?.CanGetAccess(Parent, lightee) ?? true))
		{
			return false;
		}

		return !Lit && LiquidMixture is not null && LiquidMixture.CountsAs(_prototype.LiquidFuel).Truth;
	}

	public virtual string WhyCannotLight(ICharacter lightee, IPerceivable ignitionSource)
	{
		if (!(Parent.Location?.CanGetAccess(Parent, lightee) ?? true))
		{
			return Parent.Location.WhyCannotGetAccess(Parent, lightee);
		}

		if (Lit)
		{
			return $"You cannot light {Parent.HowSeen(lightee)} because it is already lit.";
		}

		if (LiquidMixture is null)
		{
			return $"You cannot light {Parent.HowSeen(lightee)} because it does not have any fuel in it.";
		}

		if (!LiquidMixture.CountsAs(_prototype.LiquidFuel).Truth)
		{
			return $"You cannot light {Parent.HowSeen(lightee)} because it is full of the wrong sort of fuel.";
		}

		throw new NotSupportedException("Invalid reason in LanternGameItemComponent.WhyCannotLight");
	}

	public bool Light(ICharacter lightee, IPerceivable ignitionSource, IEmote playerEmote)
	{
		if (!CanLight(lightee, ignitionSource))
		{
			lightee.Send(WhyCannotLight(lightee, ignitionSource));
			return false;
		}

		lightee.OutputHandler.Handle(
			new MixedEmoteOutput(new Emote(_prototype.LightEmote, lightee, lightee, Parent, ignitionSource)).Append(
				playerEmote));
		Lit = true;
		Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat += HeartbeatManager_FiveSecondHeartbeat;
		return true;
	}

	private void HeartbeatManager_FiveSecondHeartbeat()
	{
		var volumeBefore = LiquidMixture?.TotalVolume ?? 0.0;
		LiquidMixture?.RemoveLiquidVolume(_prototype.FuelPerSecond * 5.0);
		if (LiquidMixture is null || LiquidMixture.TotalVolume <= 0.0)
		{
			LiquidMixture = null;
			Changed = true;
			Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat -= HeartbeatManager_FiveSecondHeartbeat;
			Parent.Handle(new EmoteOutput(new Emote(_prototype.FuelExpendedEcho, Parent, Parent)));
			_lit = false;
			return;
		}

		if ((volumeBefore / _prototype.FuelCapacity) > 0.1 &&
		    (LiquidMixture.TotalVolume / _prototype.FuelCapacity) <= 0.1)
		{
			Parent.Handle(new EmoteOutput(new Emote(_prototype.TenPercentFuelEcho, Parent, Parent)));
		}
	}

	public bool CanExtinguish(ICharacter lightee)
	{
		if (!(Parent.Location?.CanGetAccess(Parent, lightee) ?? true))
		{
			return false;
		}

		return Lit;
	}

	public string WhyCannotExtinguish(ICharacter lightee)
	{
		if (!(Parent.Location?.CanGetAccess(Parent, lightee) ?? true))
		{
			return Parent.Location.WhyCannotGetAccess(Parent, lightee);
		}

		if (!Lit)
		{
			return $"You cannot extinguish {Parent.HowSeen(lightee)} because it is not lit.";
		}

		throw new NotSupportedException("Invalid reason in LanternGameItemComponent.WhyCannotExtinguish");
	}

	public bool Extinguish(ICharacter lightee, IEmote playerEmote)
	{
		if (!CanExtinguish(lightee))
		{
			lightee.Send(WhyCannotExtinguish(lightee));
			return false;
		}

		lightee.OutputHandler.Handle(
			new MixedEmoteOutput(new Emote(_prototype.ExtinguishEmote, lightee, lightee, Parent)).Append(playerEmote));
		Lit = false;
		Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat -= HeartbeatManager_FiveSecondHeartbeat;
		return true;
	}

	#endregion

	#region Implementation of IProduceLight

	/// <inheritdoc />
	public double CurrentIllumination => Lit ? _prototype.IlluminationProvided : 0.0;

	#endregion
}