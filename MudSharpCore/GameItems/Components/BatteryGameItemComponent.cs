using System;
using System.Xml.Linq;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class BatteryGameItemComponent : GameItemComponent, IBattery
{
	protected BatteryGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (BatteryGameItemComponentProto)newProto;
		TotalWattHours = _prototype.BaseWattHours + (int)Parent.Quality * _prototype.WattHoursPerQuality;
	}

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("TotalWattHours", TotalWattHours),
			new XElement("WattHoursRemaining", WattHoursRemaining)
		).ToString();
	}

	#endregion

	#region IBattery Implementation

	public string BatteryType => _prototype.BatteryType;
	private double _wattHoursRemaining;

	public double WattHoursRemaining
	{
		get => _wattHoursRemaining;
		set
		{
			if (value != 0 && _wattHoursRemaining != value)
			{
				_wattHoursRemaining = Math.Max(0.0, value);
				Changed = true;
			}
		}
	}

	public double TotalWattHours { get; set; }
	public bool Rechargable => _prototype.Rechargable;

	#endregion

	#region Constructors

	public BatteryGameItemComponent(BatteryGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
		TotalWattHours = _prototype.BaseWattHours + (int)parent.Quality * _prototype.WattHoursPerQuality;
		WattHoursRemaining = TotalWattHours;
	}

	public BatteryGameItemComponent(MudSharp.Models.GameItemComponent component, BatteryGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public BatteryGameItemComponent(BatteryGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(
		rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		TotalWattHours = _prototype.BaseWattHours + (int)Parent.Quality * _prototype.WattHoursPerQuality;
		WattHoursRemaining = rhs.WattHoursRemaining;
	}

	protected void LoadFromXml(XElement root)
	{
		TotalWattHours = double.Parse(root.Element("TotalWattHours").Value);
		WattHoursRemaining = double.Parse(root.Element("WattHoursRemaining").Value);
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new BatteryGameItemComponent(this, newParent, temporary);
	}

	#endregion
}