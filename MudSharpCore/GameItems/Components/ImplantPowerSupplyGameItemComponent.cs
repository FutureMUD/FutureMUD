using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class ImplantPowerSupplyGameItemComponent : GameItemComponent, IImplantPowerSupply
{
	protected ImplantPowerSupplyGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (ImplantPowerSupplyGameItemComponentProto)newProto;
	}

	#region Constructors

	public ImplantPowerSupplyGameItemComponent(ImplantPowerSupplyGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public ImplantPowerSupplyGameItemComponent(MudSharp.Models.GameItemComponent component,
		ImplantPowerSupplyGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public ImplantPowerSupplyGameItemComponent(ImplantPowerSupplyGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	protected void LoadFromXml(XElement root)
	{
		_powerPlantId = long.Parse(root.Element("PowerPlantId").Value);
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ImplantPowerSupplyGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("PowerPlantId", _powerPlantId)
		).ToString();
	}

	#endregion

	#region IProducePower Implementation

	public bool PrimaryLoadTimePowerProducer => true;
	public bool PrimaryExternalConnectionPowerProducer => false;
	public double FuelLevel => 1.0;

	private long _powerPlantId;
	private IImplantPowerPlant _powerPlant;

	public IImplantPowerPlant PowerPlant
	{
		get => _powerPlant;
		set
		{
			var oldPlant = _powerPlant;
			_powerPlant = value;
			foreach (var item in _connectedConsumers.ToList())
			{
				oldPlant?.EndDrawdown(item);
				_powerPlant?.BeginDrawdown(item);
			}

			_powerPlantId = value?.Id ?? 0;
			Changed = true;
		}
	}

	private readonly List<IConsumePower> _connectedConsumers = new();
	public bool ProducingPower => PowerPlant?.ProducingPower ?? false;
	public double MaximumPowerInWatts => PowerPlant?.MaximumPowerInWatts ?? 0.0;

	public bool CanBeginDrawDown(double wattage)
	{
		return PowerPlant?.CanBeginDrawDown(wattage) ?? false;
	}

	public void BeginDrawdown(IConsumePower item)
	{
		if (!_connectedConsumers.Contains(item))
		{
			_connectedConsumers.Add(item);
			PowerPlant?.BeginDrawdown(item);
		}
	}

	public void EndDrawdown(IConsumePower item)
	{
		_connectedConsumers.Remove(item);
		PowerPlant?.EndDrawdown(item);
	}

	public bool CanDrawdownSpike(double wattage)
	{
		return PowerPlant?.CanDrawdownSpike(wattage) ?? false;
	}

	public bool DrawdownSpike(double wattage)
	{
		return PowerPlant?.DrawdownSpike(wattage) ?? false;
	}

	#endregion

	#region Overrides of GameItemComponent

	public override void FinaliseLoad()
	{
		var implant = Parent.GetItemType<IImplant>();
		if (implant == null)
		{
			return;
		}

		if (_powerPlantId != 0 && implant.InstalledBody != null)
		{
			_powerPlant = implant.InstalledBody.Implants
			                     .SelectNotNull(x => x.Parent.GetItemType<IImplantPowerPlant>())
			                     .FirstOrDefault(x => x.Id == _powerPlantId);
		}

		if (_powerPlant != null)
		{
			foreach (var item in _connectedConsumers.ToList())
			{
				_powerPlant.BeginDrawdown(item);
			}
		}
	}

	#endregion
}