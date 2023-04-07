using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Components;

public class RepairKitGameItemComponent : GameItemComponent, IRepairKit
{
	protected RepairKitGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (RepairKitGameItemComponentProto)newProto;
	}

	#region Constructors

	public RepairKitGameItemComponent(RepairKitGameItemComponentProto proto, IGameItem parent, bool temporary = false) :
		base(parent, proto, temporary)
	{
		_prototype = proto;
		RemainingRepairPoints = _prototype.RepairPoints;
		Changed = true;
	}

	public RepairKitGameItemComponent(MudSharp.Models.GameItemComponent component,
		RepairKitGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public RepairKitGameItemComponent(RepairKitGameItemComponent rhs, IGameItem newParent, bool temporary = false) :
		base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	protected void LoadFromXml(XElement root)
	{
		RemainingRepairPoints = int.Parse(root.Element("RemainingRepairPoints").Value);
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new RepairKitGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition", new XElement("RemainingRepairPoints", RemainingRepairPoints)).ToString();
	}

	#endregion

	public int RemainingRepairPoints { get; set; }

	public (bool Success, string Reason) CanRepair(IWound wound)
	{
		if (wound.Parent is ICharacter ch)
		{
			if (_prototype.PermittedMaterials.Any() &&
			    !_prototype.PermittedMaterials.Any(x => x == ch.Body.GetEffectiveMaterial(wound.Bodypart)))
			{
				return (false, "That repair kit cannot be used with that material type.");
			}
		}
		else if (wound.Parent is IGameItem item)
		{
			if (_prototype.PermittedMaterials.Any() && !_prototype.PermittedMaterials.Any(x => x == item.Material))
			{
				return (false, "That repair kit cannot be used with that material type.");
			}

			if (_prototype.Tags.Any() && !_prototype.Tags.Any(x => item.IsA(x)))
			{
				return (false, "That repair kit cannot be used to repair that kind of item.");
			}
		}

		if (_prototype.DamageTypes.Any() && !_prototype.DamageTypes.Contains(wound.DamageType))
		{
			return (false, "That damage is not of the correct damage type for this kit to repair.");
		}

		if (_prototype.MaximumSeverity < wound.Severity)
		{
			return (false, "That damage is too severe for this repair kit to repair.");
		}

		if (RemainingRepairPoints < (int)wound.Severity)
		{
			return (false, "The repair kit does not have enough remaining supplies to repair that damage.");
		}

		if (wound.CanBeTreated(TreatmentType.Repair) == Difficulty.Impossible)
		{
			return (false, "It would be impossibly difficult to repair that wound.");
		}

		return (true, string.Empty);
	}

	public ITraitDefinition CheckTrait => _prototype.CheckTrait;
	public double CheckBonus => _prototype.CheckBonus;

	public void Repair(IWound wound, ICharacter repairer)
	{
		var check = Gameworld.GetCheck(CheckType.RepairItemCheck);
		var difficulty = wound.CanBeTreated(TreatmentType.Repair);
		var result = check.Check(repairer, difficulty, CheckTrait, externalBonus: CheckBonus);
		wound.Treat(repairer, TreatmentType.Repair, null, result, false);
		RemainingRepairPoints -= (int)wound.Severity;
		wound.Parent.EvaluateWounds();
		Changed = true;
	}

	public void Repair(IEnumerable<IWound> wounds, ICharacter repairer)
	{
		var check = Gameworld.GetCheck(CheckType.RepairItemCheck);
		var difficulty = wounds.First().CanBeTreated(TreatmentType.Repair);
		var intDiff = (int)difficulty;
		var result = check.Check(repairer, difficulty, CheckTrait, externalBonus: CheckBonus);
		var points = result.SuccessDegrees() * 5;
		foreach (var wound in wounds)
		{
			wound.Treat(repairer, TreatmentType.Repair, null, result, false);
			RemainingRepairPoints -= intDiff;
			points -= intDiff;
			if (points <= 0 || RemainingRepairPoints - intDiff < 0)
			{
				break;
			}
		}

		wounds.First().Parent.EvaluateWounds();
		Changed = true;
	}

	public IEnumerable<string> Echoes => _prototype.Echoes;
}