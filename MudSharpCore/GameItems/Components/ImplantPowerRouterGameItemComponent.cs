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

public class ImplantPowerRouterGameItemComponent : GameItemComponent, IImplantPowerPlant, IImplantReportStatus
{
	protected ImplantPowerRouterGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (ImplantPowerRouterGameItemComponentProto)newProto;
	}

	#region Constructors

	public ImplantPowerRouterGameItemComponent(ImplantPowerRouterGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public ImplantPowerRouterGameItemComponent(MudSharp.Models.GameItemComponent component,
		ImplantPowerRouterGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public ImplantPowerRouterGameItemComponent(ImplantPowerRouterGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		if (!temporary)
		{
			Gameworld.SaveManager.AddInitialisation(this);
		}
		else
		{
			_noSave = true;
		}
	}

	protected void LoadFromXml(XElement root)
	{
		// TODO
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ImplantPowerRouterGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition").ToString();
	}

	#endregion

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
		CheckPowerPlants();
		if (_powered)
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
	}

	#endregion

	#region IConsumePower Implementation

	private bool _powered;

	public double PowerConsumptionInWatts => _connectedConsumers.Sum(x => x.PowerConsumptionInWatts) *
	                                         (_prototype.BaseEfficiencyMultiplier - (int)Parent.Quality *
		                                         _prototype.EfficiencyGainPerQuality);

	#endregion

	public void OnPowerCutIn()
	{
		if (!_powered)
		{
			_powered = true;
			foreach (var item in _connectedConsumers.ToList())
			{
				item.OnPowerCutIn();
			}
		}
	}

	public void OnPowerCutOut()
	{
		_powered = false;
		CheckPowerPlants();
		if (!_powered)
		{
			foreach (var item in _connectedConsumers.ToList())
			{
				item.OnPowerCutOut();
			}
		}
	}

	private void CheckPowerPlants()
	{
		if (InstalledBody != null)
		{
			_inCheckPowerPlants = true;
			var installed = InstalledBody.Implants.OfType<IImplantPowerPlant>().ToList();
			foreach (var implant in installed)
			{
				if (implant is ImplantPowerRouterGameItemComponent)
					// We don't want routers connecting to other routers so that they don't create any loops
				{
					continue;
				}

				if (!_connectedPowerPlants.Contains(implant))
				{
					_connectedPowerPlants.Add(implant);
				}
			}

			foreach (var implant in _connectedPowerPlants.ToList())
			{
				if (!installed.Contains(implant))
				{
					_connectedPowerPlants.Remove(implant);
				}
			}

			if (!_powered)
			{
				foreach (var item in _connectedPowerPlants)
				{
					if (item.CanBeginDrawDown(PowerConsumptionInWatts))
					{
						item.BeginDrawdown(this);
						break;
					}
				}
			}

			_inCheckPowerPlants = false;
		}
	}

	public override void FinaliseLoad()
	{
		CheckPowerPlants();
	}

	#region IProducePower Members

	private readonly List<IImplantPowerPlant> _connectedPowerPlants = new();

	private bool _inCheckPowerPlants;
	public bool ProducingPower => _powered;
	private readonly List<IConsumePower> _connectedConsumers = new();
	public double MaximumPowerInWatts => _connectedPowerPlants.Sum(x => x.MaximumPowerInWatts);

	public bool CanBeginDrawDown(double wattage)
	{
		return _connectedPowerPlants.FirstOrDefault()?.CanBeginDrawDown(wattage) ?? false;
	}

	public void BeginDrawdown(IConsumePower item)
	{
		if (!_inCheckPowerPlants)
		{
			CheckPowerPlants();
		}

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
		CheckPowerPlants();
		_connectedConsumers.Remove(item);
	}

	public bool CanDrawdownSpike(double wattage)
	{
		CheckPowerPlants();
		return _connectedPowerPlants.Any(x => x.CanDrawdownSpike(wattage));
	}

	public bool DrawdownSpike(double wattage)
	{
		CheckPowerPlants();
		foreach (var parent in _connectedPowerPlants)
		{
			if (parent.CanDrawdownSpike(wattage))
			{
				parent.DrawdownSpike(wattage);
				return true;
			}
		}

		return false;
	}

	#endregion

	#region IImplantReportStatus

	public string ReportStatus()
	{
		CheckPowerPlants();
		if (!_powered)
		{
			return "\t* Implant is unpowered and non-functional.";
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Implant is a power router, which routes power between implants and power plants.");
		sb.AppendLine(
			$"\t* Implant is powered and functioning at {FunctionFactor.ToString("P2", InstalledBody.Actor)} capacity.");
		foreach (var pp in _connectedPowerPlants)
		{
			var iirtc = pp as IImplantRespondToCommands;
			sb.AppendLine(
				$"\t* Connected to power plant {pp.Parent.HowSeen(InstalledBody.Actor)}{(iirtc?.AliasForCommands != null ? $" [{iirtc.AliasForCommands.Colour(Telnet.BoldWhite)}]" : "")}.");
		}

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
}