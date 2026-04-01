#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class FuelHeaterCoolerGameItemComponent : SwitchableThermalSourceGameItemComponent, IConnectable
{
	public FuelHeaterCoolerGameItemComponent(FuelHeaterCoolerGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(proto, parent, temporary)
	{
		_prototype = proto;
	}

	public FuelHeaterCoolerGameItemComponent(Models.GameItemComponent component,
		FuelHeaterCoolerGameItemComponentProto proto, IGameItem parent) : base(component, proto, parent)
	{
		_prototype = proto;
	}

	public FuelHeaterCoolerGameItemComponent(FuelHeaterCoolerGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_pendingConnectedItemId = rhs._pendingConnectedItemId;
	}

	private FuelHeaterCoolerGameItemComponentProto _prototype;
	private long _pendingConnectedItemId;
	private readonly List<Tuple<ConnectorType, IConnectable>> _connectedItems = [];
	public override IGameItemComponentProto Prototype => _prototype;
	protected override bool CanCurrentlyProduceHeat => HasUsableFuelSource();

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new FuelHeaterCoolerGameItemComponent(this, newParent, temporary);
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (FuelHeaterCoolerGameItemComponentProto)newProto;
	}

	public override void FinaliseLoad()
	{
		base.FinaliseLoad();
		if (_pendingConnectedItemId == 0)
		{
			return;
		}

		var item = Gameworld.Items.Get(_pendingConnectedItemId) ?? Gameworld.TryGetItem(_pendingConnectedItemId, true);
		if (item is null)
		{
			return;
		}

		var connectable = item.GetItemTypes<IConnectable>().FirstOrDefault(x => x != this && CanConnect(null, x));
		if (connectable is not null)
		{
			Connect(null, connectable);
		}

		_pendingConnectedItemId = 0;
	}

	public override void Login()
	{
		base.Login();
		UpdateHeartbeatSubscription();
	}

	public override void Quit()
	{
		Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat -= HeartbeatManagerOnFuzzyFiveSecondHeartbeat;
		base.Quit();
	}

	public override void Delete()
	{
		foreach (var item in _connectedItems.Select(x => x.Item2).ToList())
		{
			RawDisconnect(item, true);
		}

		Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat -= HeartbeatManagerOnFuzzyFiveSecondHeartbeat;
		base.Delete();
	}

	protected override string SaveToXml()
	{
		var root = new XElement("Definition");
		if (_connectedItems.Any())
		{
			root.Add(new XElement("ConnectedItem", _connectedItems.First().Item2.Parent.Id));
		}

		return SaveSwitchableStateToXml(root);
	}

	protected override void LoadSwitchableStateFromXmlAdditional(XElement root)
	{
		_pendingConnectedItemId = long.Parse(root.Element("ConnectedItem")?.Value ?? "0");
	}

	internal void BurnFuel(double seconds)
	{
		if (!SwitchedOn)
		{
			return;
		}

		if (_prototype.FuelMedium == FuelHeaterCoolerFuelMedium.Liquid)
		{
			var liquid = GetConnectedLiquidContainer();
			if (liquid?.LiquidMixture?.CountsAs(_prototype.LiquidFuel!).Truth != true)
			{
				SwitchedOn = false;
				return;
			}

			liquid.RemoveLiquidAmount(_prototype.FuelPerSecond * seconds, null, "burn");
			if (liquid.LiquidMixture?.CountsAs(_prototype.LiquidFuel!).Truth != true)
			{
				SwitchedOn = false;
			}

			return;
		}

		var gasSupply = GetConnectedGasSupply();
		if (_prototype.GasFuel is null || gasSupply?.Gas?.CountsAs(_prototype.GasFuel) != true)
		{
			SwitchedOn = false;
			return;
		}

		if (!gasSupply.ConsumeGas(_prototype.FuelPerSecond * seconds))
		{
			SwitchedOn = false;
		}
	}

	private void HeartbeatManagerOnFuzzyFiveSecondHeartbeat()
	{
		BurnFuel(5.0);
	}

	private void UpdateHeartbeatSubscription()
	{
		Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat -= HeartbeatManagerOnFuzzyFiveSecondHeartbeat;
		if (SwitchedOn)
		{
			Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat += HeartbeatManagerOnFuzzyFiveSecondHeartbeat;
		}
	}

	protected override void HandleSwitchStateChanged(bool switchedOn)
	{
		base.HandleSwitchStateChanged(switchedOn);
		UpdateHeartbeatSubscription();
	}

	private bool HasUsableFuelSource()
	{
		return _prototype.FuelMedium == FuelHeaterCoolerFuelMedium.Liquid
			? _prototype.LiquidFuel is not null &&
			  GetConnectedLiquidContainer()?.LiquidMixture?.CountsAs(_prototype.LiquidFuel).Truth == true
			: _prototype.GasFuel is not null &&
			  GetConnectedGasSupply()?.Gas?.CountsAs(_prototype.GasFuel) == true;
	}

	private ILiquidContainer? GetConnectedLiquidContainer()
	{
		return _connectedItems.Select(x => x.Item2.Parent.GetItemType<ILiquidContainer>()).FirstOrDefault();
	}

	private IGasSupply? GetConnectedGasSupply()
	{
		return _connectedItems.Select(x => x.Item2.Parent.GetItemType<IGasSupply>()).FirstOrDefault();
	}

	public IEnumerable<ConnectorType> Connections => [_prototype.Connector];
	public IEnumerable<Tuple<ConnectorType, IConnectable>> ConnectedItems => _connectedItems;
	public IEnumerable<ConnectorType> FreeConnections => _connectedItems.Any() ? [] : [_prototype.Connector];
	public bool Independent => true;

	public bool CanBeConnectedTo(IConnectable other)
	{
		return true;
	}

	public bool CanConnect(ICharacter actor, IConnectable other)
	{
		if (_connectedItems.Any() || !other.FreeConnections.Any(x => x.CompatibleWith(_prototype.Connector)) || !other.CanBeConnectedTo(this))
		{
			return false;
		}

		return _prototype.FuelMedium == FuelHeaterCoolerFuelMedium.Liquid
			? other.Parent.GetItemType<ILiquidContainer>() is not null
			: other.Parent.GetItemType<IGasSupply>() is not null;
	}

	public void Connect(ICharacter actor, IConnectable other)
	{
		if (!CanConnect(actor, other))
		{
			return;
		}

		RawConnect(other, _prototype.Connector);
		other.RawConnect(this, other.FreeConnections.First(x => x.CompatibleWith(_prototype.Connector)));
		Changed = true;
	}

	public void RawConnect(IConnectable other, ConnectorType type)
	{
		if (_connectedItems.All(x => x.Item2 != other))
		{
			_connectedItems.Add(Tuple.Create(type, other));
			Changed = true;
		}
	}

	public string WhyCannotConnect(ICharacter actor, IConnectable other)
	{
		if (_connectedItems.Any())
		{
			return $"{Parent.HowSeen(actor)} is already connected to a fuel source.";
		}

		return $"That is not a compatible {_prototype.FuelMedium.DescribeEnum().ToLowerInvariant()} fuel source.";
	}

	public bool CanBeDisconnectedFrom(IConnectable other)
	{
		return _connectedItems.Any(x => x.Item2 == other);
	}

	public bool CanDisconnect(ICharacter actor, IConnectable other)
	{
		return CanBeDisconnectedFrom(other);
	}

	public void Disconnect(ICharacter actor, IConnectable other)
	{
		RawDisconnect(other, true);
	}

	public void RawDisconnect(IConnectable other, bool handleEvents)
	{
		if (_connectedItems.RemoveAll(x => x.Item2 == other) == 0)
		{
			return;
		}

		if (handleEvents)
		{
			other.RawDisconnect(this, false);
		}

		if (SwitchedOn)
		{
			SwitchedOn = false;
		}

		Changed = true;
	}

	public string WhyCannotDisconnect(ICharacter actor, IConnectable other)
	{
		return !_connectedItems.Any(x => x.Item2 == other)
			? $"{Parent.HowSeen(actor)} is not connected to that item."
			: string.Empty;
	}
}
