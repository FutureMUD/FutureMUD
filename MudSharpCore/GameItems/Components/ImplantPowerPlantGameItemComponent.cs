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
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Components;

public class ImplantPowerPlantGameItemComponent : GameItemComponent, IImplantPowerPlant, IImplantReportStatus,
	IImplantRespondToCommands
{
	protected ImplantPowerPlantGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	#region IImplant Implementation

	public double ImplantSpaceOccupied => _prototype.ImplantSpaceOccupied;
	public Difficulty InstallDifficulty => _prototype.InstallDifficulty;
	public double FunctionFactor => _powered ? 1.0 : 0.0;

	public bool External => _prototype.External;

	public string ExternalDescription => _prototype.ExternalDescription;

	public IBodyPrototype TargetBody => _prototype.TargetBody;

	protected IBodypart OverridenBodypart;

	public IBodypart TargetBodypart
	{
		get => OverridenBodypart ?? _prototype.TargetBodypart;
		set
		{
			OverridenBodypart = value;
			Changed = true;
		}
	}

	public IBody InstalledBody { get; set; }

	public void InstallImplant(IBody body)
	{
		InstalledBody = body;
		if (_powered && body != null)
		{
			foreach (var implant in body.Implants.Except(this).SelectNotNull(x => x.Parent.GetItemType<IConsumePower>())
			                            .ToList())
			{
				implant.OnPowerCutIn();
			}
		}
	}

	public void RemoveImplant()
	{
		if (_powered)
		{
			foreach (var implant in InstalledBody.Implants.Except(this)
			                                     .SelectNotNull(x => x.Parent.GetItemType<IConsumePower>()).ToList())
			{
				implant.OnPowerCutOut();
			}
		}

		InstalledBody = null;
		OverridenBodypart = null;
		Changed = true;
	}

	#endregion

	#region IConsumePower Implementation

	private bool _powered;

	public double PowerConsumptionInWatts => _connectedConsumers.Sum(x => x.PowerConsumptionInWatts) *
	                                         (_prototype.BaseEfficiencyMultiplier - (int)Parent.Quality *
		                                         _prototype.EfficiencyGainPerQuality);

	#endregion

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (ImplantPowerPlantGameItemComponentProto)newProto;
	}

	public void OnPowerCutIn()
	{
		_powered = true;
		foreach (var item in _connectedConsumers.ToList())
		{
			item.OnPowerCutIn();
		}
	}

	public void OnPowerCutOut()
	{
		_powered = false;
		foreach (var item in _connectedConsumers.ToList())
		{
			item.OnPowerCutOut();
		}
	}

	#region Constructors

	public ImplantPowerPlantGameItemComponent(ImplantPowerPlantGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public ImplantPowerPlantGameItemComponent(MudSharp.Models.GameItemComponent component,
		ImplantPowerPlantGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public ImplantPowerPlantGameItemComponent(ImplantPowerPlantGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	protected void LoadFromXml(XElement root)
	{
		AliasForCommands = root.Element("AliasForCommands").Value;
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ImplantPowerPlantGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition", new XElement("AliasForCommands", new XCData(AliasForCommands ?? "")))
			.ToString();
	}

	#endregion

	public override void FinaliseLoad()
	{
		_parentProducePower = Parent.Components.Except(this).OfType<IProducePower>()
		                            .First(x => x.PrimaryLoadTimePowerProducer);
	}

	public override void Quit()
	{
		_parentProducePower.EndDrawdown(this);
		base.Quit();
	}

	public override void Delete()
	{
		_parentProducePower.EndDrawdown(this);
		base.Delete();
	}

	public override void Login()
	{
		base.Login();
		_parentProducePower.BeginDrawdown(this);
	}

	#region IProducePower Members

	private IProducePower _parentProducePower;
	public bool ProducingPower => _powered;
	public double MaximumPowerInWatts => _parentProducePower?.MaximumPowerInWatts ?? 0.0;
	private readonly List<IConsumePower> _connectedConsumers = new();

	public bool CanBeginDrawDown(double wattage)
	{
		return _parentProducePower?.CanBeginDrawDown(wattage) ?? false;
	}

	public void BeginDrawdown(IConsumePower item)
	{
		if (!_connectedConsumers.Contains(item))
		{
			_connectedConsumers.Add(item);
			if (_powered)
			{
				item.OnPowerCutIn();
			}
		}
	}

	public void EndDrawdown(IConsumePower item)
	{
		_connectedConsumers.Remove(item);
		if (_powered)
		{
			item.OnPowerCutOut();
		}
	}

	public bool CanDrawdownSpike(double wattage)
	{
		return _parentProducePower.CanDrawdownSpike(wattage);
	}

	public bool DrawdownSpike(double wattage)
	{
		return _parentProducePower.DrawdownSpike(wattage);
	}

	#endregion

	#region IImplantReportStatus

	public string ReportStatus()
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Implant is a power plant, which powers other items to which it is connected.");
		sb.AppendLine(
			$"\t* Implant is powered and functioning at {FunctionFactor.ToString("P2", InstalledBody.Actor)} capacity.");
		sb.AppendLine(
			$"\t* Power source is at {(_parentProducePower?.FuelLevel ?? 0.0).ToString("P2", InstalledBody.Actor)} fuel load.");
		sb.AppendLine(
			$"\t* Consumers are drawing {$"{_connectedConsumers.Sum(x => x.PowerConsumptionInWatts).ToString("N2", InstalledBody.Actor)} watts.".ColourValue()}.");
		foreach (var consumer in _connectedConsumers.OfType<IImplant>())
		{
			var iirtc = consumer as IImplantRespondToCommands;
			sb.AppendLine(
				$"\t\t* Power connection to {consumer.Parent.HowSeen(InstalledBody.Actor)}{(iirtc != null ? $" [{iirtc.AliasForCommands.Colour(Telnet.BoldWhite)}]" : "")}");
		}

		return sb.ToString();
	}

	#endregion

	#region IImplantRespondToCommands

	private string _aliasForCommands;

	public string AliasForCommands
	{
		get => _aliasForCommands;
		set
		{
			_aliasForCommands = value;
			Changed = true;
		}
	}

	public IEnumerable<string> Commands => new[] { "on", "off" };

	public string CommandHelp =>
		$"You can issue the commands {"on".ColourCommand()} and {"off".ColourCommand()}, which switch this power plant on and off.";

	public void IssueCommand(string command, StringStack arguments)
	{
		if (command.EqualTo("on"))
		{
			DoCommandOn();
			return;
		}

		if (command.EqualTo("off"))
		{
			DoCommandOff();
			return;
		}

		InstalledBody.Actor.OutputHandler.Send("That is not a valid command for this implant.");
		return;
	}

	private void DoCommandOn()
	{
		if (_parentProducePower == null || !(_parentProducePower is IOnOff ioo))
		{
			InstalledBody.Actor.OutputHandler.Send(
				"There was a problem issuing the power-on command: the power module did not respond. It may not have support for switching on and off.");
			return;
		}

		if (ioo.SwitchedOn)
		{
			InstalledBody.Actor.OutputHandler.Send(
				"The power-on command reports that the power module is already switched on.");
			return;
		}

		ioo.SwitchedOn = true;
		InstalledBody.Actor.OutputHandler.Send("Successfully issued the power-on command.");
	}

	private void DoCommandOff()
	{
		if (_parentProducePower == null || !(_parentProducePower is IOnOff ioo))
		{
			InstalledBody.Actor.OutputHandler.Send(
				"There was a problem issuing the power-off command: the power module did not respond. It may not have support for switching on and off.");
			return;
		}

		if (!ioo.SwitchedOn)
		{
			InstalledBody.Actor.OutputHandler.Send(
				"The power-of command reports that the power module is already switched off.");
			return;
		}

		ioo.SwitchedOn = false;
		InstalledBody.Actor.OutputHandler.Send("Successfully issued the power-off command.");
	}

	#endregion
}