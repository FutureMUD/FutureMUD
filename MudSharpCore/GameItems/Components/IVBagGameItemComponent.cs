using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.Form.Material;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Shape;
using MudSharp.Construction;

namespace MudSharp.GameItems.Components;

public class IVBagGameItemComponent : GameItemComponent, ILiquidContainer, ISwitchable, IConnectable
{
	public enum IVMode
	{
		Neutral,
		Drip,
		Drain
	}

	protected IVBagGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	#region Constructors

	public IVBagGameItemComponent(IVBagGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(
		parent, proto, temporary)
	{
		_prototype = proto;
		if (_prototype.Closable)
		{
			IsOpen = false;
		}
	}

	public IVBagGameItemComponent(MudSharp.Models.GameItemComponent component, IVBagGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public IVBagGameItemComponent(IVBagGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs,
		newParent, temporary)
	{
		_prototype = rhs._prototype;
		_isOpen = rhs._isOpen;
		if (rhs.LiquidMixture != null)
		{
			_liquidMixture = new LiquidMixture(rhs.LiquidMixture);
		}
	}

	protected void LoadFromXml(XElement root)
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
				        .Select(x => Gameworld.TryGetItem(long.Parse(x.Value), true))
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

		var element = root.Element("ConnectedItems");
		if (element != null)
		{
			foreach (var item in element.Elements("Item"))
			{
				if (item.Attribute("independent")?.Value == "false")
				{
					_pendingDependentLoadTimeConnections.Add(Tuple.Create(long.Parse(item.Attribute("id").Value),
						new ConnectorType(
							item.Attribute("connectiontype").Value)));
				}
				else
				{
					_pendingLoadTimeConnections.Add(Tuple.Create(long.Parse(item.Attribute("id").Value),
						new ConnectorType(
							item.Attribute("connectiontype").Value)));
				}
			}
		}
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new IVBagGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition", new XAttribute("Open", IsOpen.ToString().ToLowerInvariant()),
				LiquidMixture?.SaveToXml() ?? new XElement("NoLiquid"),
				new XElement(
					"Locks", from thelock in Locks select new XElement("Lock", thelock.Parent.Id)),
				new XElement("ConnectedItems",
					from item in ConnectedItems
					select
						new XElement("Item", new XAttribute("id", item.Item2.Parent.Id),
							new XAttribute("connectiontype", item.Item1),
							new XAttribute("independent", item.Item2.Independent)))
			)
			.ToString();
	}

	#endregion

	public IVMode Mode { get; protected set; }
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

	public override void Delete()
	{
		EndHeartbeat();
		base.Delete();
		foreach (var item in Locks.ToList())
		{
			_locks.Remove(item);
			item.Parent.Delete();
		}
	}

	public override void Quit()
	{
		EndHeartbeat();
		foreach (var item in Locks)
		{
			item.Quit();
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
			case DescriptionType.Full:
				var sb = new StringBuilder();
				sb.AppendLine(description);
				if (Locks.Any())
				{
					sb.AppendLine("It has the following locks:");
					foreach (var thelock in Locks)
					{
						sb.AppendLineFormat("\t{0}", thelock.Parent.HowSeen(voyeur));
					}

					sb.AppendLine();
				}

				sb.AppendLine(
					$"It connects to {(_prototype.Connections.Count == 1 ? "a connector" : "connectors")} of type {_prototype.Connections.Select(x => $"{x.ConnectionType.Colour(Telnet.Green)} ({Gendering.Get(x.Gender).GenderClass(true)})").ListToString()}.");
				if (ConnectedItems.Any())
				{
					sb.AppendLine();
				}

				foreach (var item in ConnectedItems)
				{
					sb.AppendLine(
						$"It is currently connected to {item.Item2.Parent.HowSeen(voyeur)} by a {item.Item1.ConnectionType.Colour(Telnet.Green)} connection.");
				}

				if (LiquidMixture?.IsEmpty == false)
				{
					sb.AppendLine(string.Format(voyeur, "It is approximately {0:P0} full of {1}.",
						Math.Round(20 * LiquidMixture.TotalVolume / LiquidCapacity) / 20,
						LiquidMixture.ColouredLiquidDescription));
				}

				return sb.ToString();
			case DescriptionType.Evaluate:
				return
					$"{description}\nIt can hold {Gameworld.UnitManager.DescribeMostSignificantExact(LiquidCapacity, Framework.Units.UnitType.FluidVolume, voyeur).ColourValue()} of liquid.";
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
				if (location == null)
				{
					thelock.Parent.Delete();
					continue;
				}

				location.Insert(thelock.Parent);
				thelock.Parent.ContainedIn = null;
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
		_prototype = (IVBagGameItemComponentProto)newProto;
		if (LiquidMixture != null && LiquidMixture.TotalVolume > LiquidCapacity)
		{
			LiquidMixture.SetLiquidVolume(LiquidCapacity);
		}

		if (_prototype.Closable && IsOpen)
		{
			IsOpen = false;
		}
	}

	public override void FinaliseLoad()
	{
		foreach (var item in _pendingLoadTimeConnections.ToList())
		{
			var gitem = Gameworld.Items.Get(item.Item1);
			if (gitem == null)
			{
				continue;
			}

			var connectables = gitem.GetItemTypes<IConnectable>().ToList();
			if (!connectables.Any())
			{
				continue;
			}

			if (gitem.Location != Parent.Location)
			{
				continue;
			}

			foreach (var connectable in connectables)
			{
				if (!connectable.CanConnect(null, this))
				{
					continue;
				}

				Connect(null, connectable);
				break;
			}
		}

		_pendingLoadTimeConnections.Clear();

		foreach (var item in _pendingDependentLoadTimeConnections.ToList())
		{
			var gitem = Gameworld.Items.Get(item.Item1);
			if (gitem == null)
			{
				gitem = Gameworld.TryGetItem(item.Item1, true);
				if (gitem == null)
				{
					continue;
				}

				gitem.FinaliseLoadTimeTasks();
			}

			var connectables = gitem.GetItemTypes<IConnectable>().ToList();
			if (!connectables.Any())
			{
				continue;
			}

			foreach (var connectable in connectables)
			{
				if (connectable == null)
				{
					continue;
				}

				Connect(null, connectable);
				break;
			}
		}

		_pendingDependentLoadTimeConnections.Clear();
		Changed = true;
	}

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

	#region Implementation of ISwitchable

	public IEnumerable<string> SwitchSettings { get; } = new[] { "drip", "drain", "neutral" };

	public bool CanSwitch(ICharacter actor, string setting)
	{
		ICannula cannula;
		if (_connectedItems
		    .FirstOrDefault(x => x.Item1.Gender == Gender.Male && x.Item1.ConnectionType.EqualTo("cannula"))
		    ?.Item2 is IDrip drip)
		{
			cannula = drip.ConnectedItems
			              .FirstOrDefault(x =>
				              x.Item1.Gender == Gender.Male && x.Item1.ConnectionType.EqualTo("cannula"))
			              ?.Item2 as ICannula;
		}
		else
		{
			cannula = _connectedItems
			          .FirstOrDefault(x => x.Item1.Gender == Gender.Male && x.Item1.ConnectionType.EqualTo("cannula"))
			          ?.Item2 as ICannula;
		}

		var tch = cannula?.InstalledBody?.Actor;
		if (tch == null)
		{
			return false;
		}

		if (!setting.In("drip", "drain", "neutral"))
		{
			return false;
		}

		return true;
	}

	public string WhyCannotSwitch(ICharacter actor, string setting)
	{
		ICannula cannula;
		if (_connectedItems
		    .FirstOrDefault(x => x.Item1.Gender == Gender.Male && x.Item1.ConnectionType.EqualTo("cannula"))
		    ?.Item2 is IDrip drip)
		{
			cannula = drip.ConnectedItems
			              .FirstOrDefault(x =>
				              x.Item1.Gender == Gender.Male && x.Item1.ConnectionType.EqualTo("cannula"))
			              ?.Item2 as ICannula;
		}
		else
		{
			cannula = _connectedItems
			          .FirstOrDefault(x => x.Item1.Gender == Gender.Male && x.Item1.ConnectionType.EqualTo("cannula"))
			          ?.Item2 as ICannula;
		}

		var tch = cannula?.InstalledBody?.Actor;
		if (tch == null)
		{
			return "IV Bags can only be switched into positions when connected to a patient.";
		}

		if (!setting.In("drip", "drain", "neutral"))
		{
			return "The valid options for switching this item are drip, drain and neutral.";
		}

		throw new ApplicationException("Unknown WhyCannotSwitch reason in IVBagGameItemComponent");
	}

	public bool Switch(ICharacter actor, string setting)
	{
		if (!CanSwitch(actor, setting))
		{
			actor.Send(WhyCannotSwitch(actor, setting));
			return false;
		}

		switch (setting.ToLowerInvariant())
		{
			case "drip":
				SetDrip(actor);
				break;
			case "drain":
				SetDrain(actor);
				break;
			case "neutral":
				SetNeutral(actor);
				break;
		}

		return true;
	}

	private void SetNeutral(ICharacter actor)
	{
		Mode = IVMode.Neutral;
		Changed = true;
		_initialMessageSent = false;
		actor.OutputHandler.Handle(new EmoteOutput(new Emote(
			"@ adjust|adjusts $0 until it is in a position that will neither drip feed nor drain from its attached patient.",
			actor, Parent)));
	}

	private void SetDrain(ICharacter actor)
	{
		Mode = IVMode.Drain;
		Changed = true;
		_initialMessageSent = false;
		actor.OutputHandler.Handle(new EmoteOutput(new Emote(
			"@ adjust|adjusts $0 until it is in a position that will drain fluids from its attached patient.", actor,
			Parent)));
	}

	private void SetDrip(ICharacter actor)
	{
		Mode = IVMode.Drip;
		Changed = true;
		_initialMessageSent = false;
		actor.OutputHandler.Handle(new EmoteOutput(new Emote(
			"@ adjust|adjusts $0 until it is in a position that will drip feed its contents into its attached patient.",
			actor, Parent)));
	}

	private bool _initialMessageSent;

	private void StartHeartbeat()
	{
		Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat -= HeartbeatManagerOnFuzzyFiveSecondHeartbeat;
		Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat += HeartbeatManagerOnFuzzyFiveSecondHeartbeat;
		_initialMessageSent = false;
	}

	private void HeartbeatManagerOnFuzzyFiveSecondHeartbeat()
	{
		if (Mode == IVMode.Neutral)
		{
			return;
		}

		var drip = _connectedItems
		           .FirstOrDefault(x => x.Item1.Gender == Gender.Male && x.Item1.ConnectionType.EqualTo("cannula"))
		           ?.Item2 as IDrip;
		ICannula cannula;
		if (drip != null)
		{
			cannula = drip.ConnectedItems
			              .FirstOrDefault(x =>
				              x.Item1.Gender == Gender.Male && x.Item1.ConnectionType.EqualTo("cannula"))
			              ?.Item2 as ICannula;
		}
		else
		{
			cannula = _connectedItems
			          .FirstOrDefault(x => x.Item1.Gender == Gender.Male && x.Item1.ConnectionType.EqualTo("cannula"))
			          ?.Item2 as ICannula;
		}

		var tch = cannula?.InstalledBody?.Actor;
		if (tch == null)
		{
			return;
		}

		if (!_initialMessageSent)
		{
			if (Mode == IVMode.Drain)
			{
				if ((LiquidMixture?.TotalVolume ?? 0.0) <= LiquidCapacity &&
				    tch.Body.BloodLiquid != null &&
				    (LiquidMixture == null || LiquidMixture.CanMerge(tch.Body.BloodLiquid)) &&
				    tch.Body.CurrentBloodVolumeLitres > 0.0)
				{
					tch.OutputHandler.Handle(new EmoteOutput(new Emote(
						$"{tch.Body.BloodLiquid.MaterialDescription.Colour(tch.Body.BloodLiquid.DisplayColour)} begins to flow from @ via $0 into $1.",
						tch, cannula.Parent, Parent)));
					_initialMessageSent = true;
				}
			}
			else if (Mode == IVMode.Drip)
			{
				if ((LiquidMixture?.TotalVolume ?? 0) > 0.0 && tch.Body.BloodLiquid != null &&
				    LiquidMixture.CanMerge(tch.Body.BloodLiquid))
				{
					tch.OutputHandler.Handle(new EmoteOutput(new Emote(
						$"{tch.Body.BloodLiquid.MaterialDescription.Colour(tch.Body.BloodLiquid.DisplayColour)} begins to flow into @ via $0 from $1.",
						tch, cannula.Parent, Parent)));
					_initialMessageSent = true;
				}
			}
		}

		if (Mode == IVMode.Drain)
		{
			var rate = drip?.RatePerMinute / 12 ??
			           tch.Body.CurrentBloodVolumeLitres / Gameworld.UnitManager.BaseFluidToLitres * 0.001;
			if (rate > LiquidCapacity - (LiquidMixture?.TotalVolume ?? 0))
			{
				rate = LiquidCapacity - (LiquidMixture?.TotalVolume ?? 0);
			}

			if (rate <= 0.0)
			{
				return;
			}

			MergeLiquid(new LiquidMixture(new BloodLiquidInstance(tch, rate), Gameworld), null, "ivdrain");
			tch.Body.CurrentBloodVolumeLitres -= rate;
		}
		else
		{
			var rate = drip?.RatePerMinute / 12 ?? (LiquidMixture?.TotalVolume ?? 0.0) * 0.01;
			if (rate <= 0.0 || (LiquidMixture?.TotalVolume ?? 0.0) <= 0.0)
			{
				return;
			}

			if (tch.Body.CurrentBloodVolumeLitres >= tch.Body.TotalBloodVolumeLitres)
			{
				rate *= 0.1;
			}

			if (rate > LiquidMixture.TotalVolume)
			{
				rate = LiquidMixture.TotalVolume;
			}

			tch.Body.HealthStrategy.InjectedLiquid(tch, RemoveLiquidAmount(rate, null, "ivdrip"));
		}
	}

	private void EndHeartbeat()
	{
		Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat -= HeartbeatManagerOnFuzzyFiveSecondHeartbeat;
		_initialMessageSent = false;
	}

	#endregion

	#region Implementation of IConnectable

	private readonly List<Tuple<ConnectorType, IConnectable>> _connectedItems =
		new();

	private readonly List<Tuple<long, ConnectorType>> _pendingLoadTimeConnections =
		new();

	private readonly List<Tuple<long, ConnectorType>> _pendingDependentLoadTimeConnections =
		new();

	public override bool AffectsLocationOnDestruction => true;

	public override bool PreventsMovement()
	{
		return (Parent.InInventoryOf != null && ConnectedItems.Any(x => x.Item2.Independent)) ||
		       ConnectedItems.Any(x => x.Item2.Independent && x.Item2.Parent.InInventoryOf != Parent.InInventoryOf);
	}

	public override string WhyPreventsMovement(ICharacter mover)
	{
		var preventingItems =
			ConnectedItems.Where(
				              x =>
					              x.Item2.Independent &&
					              (x.Item2.Parent.InInventoryOf == null ||
					               x.Item2.Parent.InInventoryOf != Parent.InInventoryOf))
			              .ToList();
		return
			$"{Parent.HowSeen(mover)} is still connected to {preventingItems.Select(x => x.Item2.Parent.HowSeen(mover)).ListToString()}.";
	}

	public override void ForceMove()
	{
		var preventingItems =
			ConnectedItems.Where(
				              x =>
					              x.Item2.Independent &&
					              (x.Item2.Parent.InInventoryOf == null ||
					               x.Item2.Parent.InInventoryOf != Parent.InInventoryOf))
			              .ToList();
		foreach (var item in preventingItems)
		{
			RawDisconnect(item.Item2, true);
		}
	}

	public override bool Take(IGameItem item)
	{
		if (ConnectedItems.Any(x => x.Item2.Parent == item))
		{
			RawDisconnect(item.GetItemType<IConnectable>(), true);
		}

		return false;
	}

	public IEnumerable<ConnectorType> Connections => _prototype.Connections;
	public IEnumerable<Tuple<ConnectorType, IConnectable>> ConnectedItems => _connectedItems;

	public IEnumerable<ConnectorType> FreeConnections
	{
		get
		{
			var rvar = new List<ConnectorType>(Connections);
			foreach (var item in ConnectedItems)
			{
				rvar.Remove(item.Item1);
			}

			return rvar;
		}
	}

	public bool Independent => true;

	public bool CanBeConnectedTo(IConnectable other)
	{
		return true; // TODO
	}

	public bool CanConnect(ICharacter actor, IConnectable other)
	{
		if (!FreeConnections.Any())
		{
			return false;
		}

		if (!other.FreeConnections.Any())
		{
			return false;
		}

		return other.FreeConnections.Any(x => _prototype.Connections.Any(x.CompatibleWith)) &&
		       other.CanBeConnectedTo(this);
	}

	public void Connect(ICharacter actor, IConnectable other)
	{
		var connection = FreeConnections.FirstOrDefault(x => other.FreeConnections.Any(y => y.CompatibleWith(x)));
		if (connection == null)
		{
			return;
		}

		RawConnect(other, connection);
		other.RawConnect(this, other.FreeConnections.First(x => x.CompatibleWith(connection)));
		Changed = true;
	}

	public void RawConnect(IConnectable other, ConnectorType type)
	{
		_connectedItems.Add(Tuple.Create(type, other));
		_pendingLoadTimeConnections.RemoveAll(x => x.Item1 == other.Parent.Id && x.Item2.CompatibleWith(type));
		Parent.ConnectedItem(other, type);
		if (other is ICannula || other is IDrip)
		{
			StartHeartbeat();
			Mode = IVMode.Neutral;
			Changed = true;
		}

		Changed = true;
	}

	public string WhyCannotConnect(ICharacter actor, IConnectable other)
	{
		if (!FreeConnections.Any())
		{
			return
				$"You cannot connect {Parent.HowSeen(actor)} to {other.Parent.HowSeen(actor)} as the former has no free connection points.";
		}

		if (!other.FreeConnections.Any())
		{
			return
				$"You cannot connect {Parent.HowSeen(actor)} to {other.Parent.HowSeen(actor)} as the latter has no free connection points.";
		}

		if (!other.FreeConnections.Any(x => _prototype.Connections.Any(x.CompatibleWith)))
		{
			return
				$"You cannot connect {Parent.HowSeen(actor)} to {other.Parent.HowSeen(actor)} as none of the free connection points are compatible.";
		}

		return !other.CanBeConnectedTo(this)
			? $"You cannot connect {Parent.HowSeen(actor)} to {other.Parent.HowSeen(actor)} as that item cannot be connected to."
			: $"You cannot connect {Parent.HowSeen(actor)} to {other.Parent.HowSeen(actor)} for an unknown reason.";
	}

	public bool CanDisconnect(ICharacter actor, IConnectable other)
	{
		return _connectedItems.Any(x => x.Item2 == other);
	}

	public void Disconnect(ICharacter actor, IConnectable other)
	{
		RawDisconnect(other, true);
	}

	public void RawDisconnect(IConnectable other, bool handleEvents)
	{
		if (handleEvents)
		{
			other.RawDisconnect(this, false);
			foreach (var connection in _connectedItems.Where(x => x.Item2 == other).ToList())
			{
				Parent.DisconnectedItem(other, connection.Item1);
				other.Parent.DisconnectedItem(this, connection.Item1);
				if (connection.Item2 is ICannula || other is IDrip)
				{
					EndHeartbeat();
					Mode = IVMode.Neutral;
					Changed = true;
				}
			}
		}

		_connectedItems.RemoveAll(x => x.Item2 == other);
		Changed = true;
	}

	public string WhyCannotDisconnect(ICharacter actor, IConnectable other)
	{
		return _connectedItems.All(x => x.Item2 != other)
			? $"You cannot disconnect {Parent.HowSeen(actor)} from {other.Parent.HowSeen(actor)} because they are not connected!"
			: $"You cannot disconnect {Parent.HowSeen(actor)} from {other.Parent.HowSeen(actor)} for an unknown reason";
	}

	public bool CanBeDisconnectedFrom(IConnectable other)
	{
		return true; // TODO - reasons why this might be false
	}

	#endregion
}