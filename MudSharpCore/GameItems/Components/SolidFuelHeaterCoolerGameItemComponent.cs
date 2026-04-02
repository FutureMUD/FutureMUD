#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Components;

public class SolidFuelHeaterCoolerGameItemComponent : SwitchableThermalSourceGameItemComponent, IContainer
{
	public SolidFuelHeaterCoolerGameItemComponent(SolidFuelHeaterCoolerGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(proto, parent, temporary)
	{
		_prototype = proto;
	}

	public SolidFuelHeaterCoolerGameItemComponent(Models.GameItemComponent component,
		SolidFuelHeaterCoolerGameItemComponentProto proto, IGameItem parent) : base(component, proto, parent)
	{
		_prototype = proto;
	}

	public SolidFuelHeaterCoolerGameItemComponent(SolidFuelHeaterCoolerGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_remainingBurnSeconds = rhs._remainingBurnSeconds;
		_pendingBurningItemId = rhs._pendingBurningItemId;
	}

	private SolidFuelHeaterCoolerGameItemComponentProto _prototype;
	private readonly List<IGameItem> _contents = [];
	private double _remainingBurnSeconds;
	private long _pendingBurningItemId;
	private IGameItem? _currentFuelItem;
	public override IGameItemComponentProto Prototype => _prototype;
	protected override bool CanCurrentlyProduceHeat => _currentFuelItem is not null || _contents.Any();

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new SolidFuelHeaterCoolerGameItemComponent(this, newParent, temporary);
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (SolidFuelHeaterCoolerGameItemComponentProto)newProto;
	}

	public override void FinaliseLoad()
	{
		base.FinaliseLoad();
		foreach (var item in _contents)
		{
			item.FinaliseLoadTimeTasks();
		}

		if (_pendingBurningItemId != 0)
		{
			_currentFuelItem = _contents.FirstOrDefault(x => x.Id == _pendingBurningItemId);
			_pendingBurningItemId = 0;
		}
	}

	public override void Login()
	{
		base.Login();
		UpdateHeartbeatSubscription();
		foreach (var item in _contents)
		{
			item.Login();
		}
	}

	public override void Quit()
	{
		Gameworld.HeartbeatManager.SecondHeartbeat -= HeartbeatManagerOnSecondHeartbeat;
		foreach (var item in _contents)
		{
			item.Quit();
		}
		base.Quit();
	}

	public override void Delete()
	{
		Gameworld.HeartbeatManager.SecondHeartbeat -= HeartbeatManagerOnSecondHeartbeat;
		foreach (var item in _contents.ToList())
		{
			_contents.Remove(item);
			item.ContainedIn = null;
			item.Delete();
		}
		base.Delete();
	}

	protected override string SaveToXml()
	{
		var root = new XElement("Definition",
			from item in _contents
			select new XElement("Contained", item.Id));
		root.Add(new XElement("RemainingBurnSeconds", _remainingBurnSeconds));
		root.Add(new XElement("BurningItem", _currentFuelItem?.Id ?? 0));
		return SaveSwitchableStateToXml(root);
	}

	protected override void LoadSwitchableStateFromXmlAdditional(XElement root)
	{
		_remainingBurnSeconds = double.Parse(root.Element("RemainingBurnSeconds")?.Value ?? "0");
		_pendingBurningItemId = long.Parse(root.Element("BurningItem")?.Value ?? "0");
		foreach (var item in root.Elements("Contained")
		                         .Select(element => Gameworld.TryGetItem(long.Parse(element.Value), true))
		                         .Where(item => item is not null))
		{
			_contents.Add(item!);
			item!.Get(null);
			item.LoadTimeSetContainedIn(Parent);
		}
	}

	internal void BurnFuel(double seconds)
	{
		if (!SwitchedOn)
		{
			return;
		}

		if (_currentFuelItem is null && !PrimeFuel())
		{
			SwitchedOn = false;
			return;
		}

		_remainingBurnSeconds -= seconds;
		Changed = true;
		if (_remainingBurnSeconds > 0.0 || _currentFuelItem is null)
		{
			return;
		}

		_contents.RemoveAll(x => ReferenceEquals(x, _currentFuelItem) || x.Id == _currentFuelItem.Id);
		_currentFuelItem.Delete();
		_currentFuelItem = null;
		_remainingBurnSeconds = 0.0;
		if (!_contents.Any())
		{
			SwitchedOn = false;
		}
	}

	private bool PrimeFuel()
	{
		_currentFuelItem = _contents.FirstOrDefault();
		if (_currentFuelItem is null)
		{
			return false;
		}

		_remainingBurnSeconds = Math.Max(1.0, _currentFuelItem.Weight * _prototype.SecondsPerUnitWeight);
		Changed = true;
		return true;
	}

	private void HeartbeatManagerOnSecondHeartbeat()
	{
		BurnFuel(1.0);
	}

	private void UpdateHeartbeatSubscription()
	{
		Gameworld.HeartbeatManager.SecondHeartbeat -= HeartbeatManagerOnSecondHeartbeat;
		if (SwitchedOn)
		{
			Gameworld.HeartbeatManager.SecondHeartbeat += HeartbeatManagerOnSecondHeartbeat;
		}
	}

	protected override void HandleSwitchStateChanged(bool switchedOn)
	{
		base.HandleSwitchStateChanged(switchedOn);
		UpdateHeartbeatSubscription();
	}

	public override bool Take(IGameItem item)
	{
		if (_contents.Remove(item))
		{
			item.ContainedIn = null;
			Changed = true;
			return true;
		}

		return false;
	}

	public override bool HandleDieOrMorph(IGameItem newItem, ICell location)
	{
		var newContainer = newItem?.GetItemType<IContainer>();
		if (newContainer is not null)
		{
			foreach (var item in _contents.ToList())
			{
				if (newContainer.CanPut(item))
				{
					newContainer.Put(null, item);
				}
				else if (location is not null)
				{
					location.Insert(item);
					item.ContainedIn = null;
				}
				else
				{
					item.Delete();
				}
			}
		}
		else
		{
			foreach (var item in _contents.ToList())
			{
				if (location is not null)
				{
					location.Insert(item);
					item.ContainedIn = null;
				}
				else
				{
					item.Delete();
				}
			}
		}

		_contents.Clear();
		_currentFuelItem = null;
		_remainingBurnSeconds = 0.0;
		return false;
	}

	public IEnumerable<IGameItem> Contents => _contents;
	public string ContentsPreposition => "in";
	public bool Transparent => true;

	public bool CanPut(IGameItem item)
	{
		return item != Parent &&
		       item.IsA(_prototype.FuelTag) &&
		       _contents.Sum(x => x.Weight) + item.Weight <= _prototype.MaximumFuelWeight;
	}

	public void Put(ICharacter putter, IGameItem item, bool allowMerge = true)
	{
		_contents.Add(item);
		item.ContainedIn = Parent;
		Changed = true;
	}

	public WhyCannotPutReason WhyCannotPut(IGameItem item)
	{
		if (item == Parent)
		{
			return WhyCannotPutReason.CantPutContainerInItself;
		}

		if (!item.IsA(_prototype.FuelTag))
		{
			return WhyCannotPutReason.NotCorrectItemType;
		}

		return _contents.Sum(x => x.Weight) + item.Weight > _prototype.MaximumFuelWeight
			? WhyCannotPutReason.ContainerFull
			: WhyCannotPutReason.NotContainer;
	}

	public bool CanTake(ICharacter taker, IGameItem item, int quantity)
	{
		return _contents.Contains(item) && item != _currentFuelItem;
	}

	public IGameItem Take(ICharacter taker, IGameItem item, int quantity)
	{
		if (!CanTake(taker, item, quantity))
		{
			return null;
		}

		_contents.Remove(item);
		item.ContainedIn = null;
		Changed = true;
		return item;
	}

	public WhyCannotGetContainerReason WhyCannotTake(ICharacter taker, IGameItem item)
	{
		if (item == _currentFuelItem)
		{
			return WhyCannotGetContainerReason.UnlawfulAction;
		}

		return _contents.Contains(item)
			? WhyCannotGetContainerReason.NotContainer
			: WhyCannotGetContainerReason.NotContained;
	}

	public int CanPutAmount(IGameItem item)
	{
		return 1;
	}

	public void Empty(ICharacter emptier, IContainer intoContainer, IEmote playerEmote = null)
	{
		foreach (var item in _contents.ToList())
		{
			if (item == _currentFuelItem)
			{
				continue;
			}

			_contents.Remove(item);
			item.ContainedIn = null;
			if (intoContainer is not null && intoContainer.CanPut(item))
			{
				intoContainer.Put(emptier, item);
				continue;
			}

			(emptier?.Location ?? Parent.TrueLocations.FirstOrDefault())?.Insert(item);
		}

		Changed = true;
	}
}
