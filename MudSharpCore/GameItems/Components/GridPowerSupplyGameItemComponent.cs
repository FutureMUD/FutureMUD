using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Construction.Grids;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class GridPowerSupplyGameItemComponent : GameItemComponent, IProducePower, IConsumePower,
	ICanConnectToElectricalGrid
{
	protected GridPowerSupplyGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (GridPowerSupplyGameItemComponentProto)newProto;
	}

	private IElectricalGrid _grid;

	public IElectricalGrid ElectricalGrid
	{
		get => _grid;
		set
		{
			if (_grid != null)
			{
				_grid.LeaveGrid((IConsumePower)this);
			}

			_grid = value;
			if (_grid != null)
			{
				_grid.JoinGrid((IConsumePower)this);
			}

			Changed = true;
		}
	}

	#region Overrides of GameItemComponent

	public override void Login()
	{
		base.Login();
		ElectricalGrid?.RecalculateGrid();
	}

	#endregion

	#region Constructors

	public GridPowerSupplyGameItemComponent(GridPowerSupplyGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public GridPowerSupplyGameItemComponent(MudSharp.Models.GameItemComponent component,
		GridPowerSupplyGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public GridPowerSupplyGameItemComponent(GridPowerSupplyGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	protected void LoadFromXml(XElement root)
	{
		var element = root.Element("Grid");
		if (element != null)
		{
			ElectricalGrid = Gameworld.Grids.Get(long.Parse(element.Value)) as IElectricalGrid;
		}
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new GridPowerSupplyGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition", new XElement("Grid", ElectricalGrid?.Id ?? 0)).ToString();
	}

	#endregion

	#region Implementation of IProducePower

	public bool PrimaryLoadTimePowerProducer => true;
	public bool PrimaryExternalConnectionPowerProducer => false;
	public double FuelLevel => 1.0;
	private readonly List<IConsumePower> _connectedConsumers = new();
	private readonly List<IConsumePower> _powerUsers = new();
	private bool _powered;

	public void BeginDrawdown(IConsumePower item)
	{
		_connectedConsumers.Add(item);
		if (ProducingPower)
		{
			_powerUsers.Add(item);
			item.OnPowerCutIn();
		}

		ElectricalGrid?.RecalculateGrid();
	}

	public void EndDrawdown(IConsumePower item)
	{
		_connectedConsumers.Remove(item);
		if (_powerUsers.Contains(item))
		{
			item.OnPowerCutOut();
		}

		_powerUsers.Remove(item);
		ElectricalGrid?.RecalculateGrid();
	}

	public bool CanBeginDrawDown(double wattage)
	{
		return true;
	}

	public bool CanDrawdownSpike(double wattage)
	{
		return ProducingPower;
	}

	public bool DrawdownSpike(double wattage)
	{
		return ProducingPower && (ElectricalGrid?.DrawdownSpike(wattage) ?? false);
	}

	public double MaximumPowerInWatts =>
		ElectricalGrid != null ? ElectricalGrid.TotalSupply - ElectricalGrid.TotalDrawdown : 0.0;

	public bool ProducingPower => ElectricalGrid != null && _powered;

	#endregion

	#region IConsumePower Implementation

	public double PowerConsumptionInWatts => _powerUsers.Sum(x => x.PowerConsumptionInWatts);

	public void OnPowerCutIn()
	{
		_powered = true;
		foreach (var item in _connectedConsumers.Where(x => !_powerUsers.Contains(x)).ToList())
		{
			_powerUsers.Add(item);
		}

		foreach (var item in _powerUsers)
		{
			item.OnPowerCutIn();
		}
	}

	public void OnPowerCutOut()
	{
		_powered = false;
		foreach (var item in _powerUsers)
		{
			item.OnPowerCutOut();
		}

		_powerUsers.Clear();
	}

	#endregion

	#region ICanConnectToGrid Implementation

	string ICanConnectToGrid.GridType => "Electrical";

	IGrid ICanConnectToGrid.Grid
	{
		get => ElectricalGrid;
		set => ElectricalGrid = value as IElectricalGrid;
	}

	#endregion
}