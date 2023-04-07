using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.RPG.Checks;
using ExpressionEngine;
using MudSharp.Commands.Trees;

namespace MudSharp.Combat;

public class ArmourType : FrameworkItem, IArmourType, IHaveFuturemud
{
	public ArmourType(MudSharp.Models.ArmourType type, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = type.Id;
		_name = type.Name;
		MinimumPenetrationDegree = (OpposedOutcomeDegree)type.MinimumPenetrationDegree;
		BaseDifficultyBonus = type.BaseDifficultyDegrees;
		StackedDifficultyBonus = type.StackedDifficultyDegrees;
		var definition = XElement.Parse(type.Definition);
		var element = definition.Element("DissipateExpressions");
		foreach (var item in element?.Elements("Expression") ?? Enumerable.Empty<XElement>())
		{
			DissipateExpressions[(DamageType)int.Parse(item.Attribute("damagetype").Value)] =
				new Expression(item.Value);
		}

		element = definition.Element("AbsorbExpressions");
		foreach (var item in element?.Elements("Expression") ?? Enumerable.Empty<XElement>())
		{
			AbsorbExpressions[(DamageType)int.Parse(item.Attribute("damagetype").Value)] =
				new Expression(item.Value);
		}

		element = definition.Element("DissipateExpressionsPain");
		foreach (var item in element?.Elements("Expression") ?? Enumerable.Empty<XElement>())
		{
			DissipateExpressionsPain[(DamageType)int.Parse(item.Attribute("damagetype").Value)] =
				new Expression(item.Value);
		}

		element = definition.Element("AbsorbExpressionsPain");
		foreach (var item in element?.Elements("Expression") ?? Enumerable.Empty<XElement>())
		{
			AbsorbExpressionsPain[(DamageType)int.Parse(item.Attribute("damagetype").Value)] =
				new Expression(item.Value);
		}

		element = definition.Element("DissipateExpressionsStun");
		foreach (var item in element?.Elements("Expression") ?? Enumerable.Empty<XElement>())
		{
			DissipateExpressionsStun[(DamageType)int.Parse(item.Attribute("damagetype").Value)] =
				new Expression(item.Value);
		}

		element = definition.Element("AbsorbExpressionsStun");
		foreach (var item in element?.Elements("Expression") ?? Enumerable.Empty<XElement>())
		{
			AbsorbExpressionsStun[(DamageType)int.Parse(item.Attribute("damagetype").Value)] =
				new Expression(item.Value);
		}

		element = definition.Element("DamageTransformations");
		if (element != null)
		{
			foreach (var item in element.Elements("Transform"))
			{
				DamageTypeTransformations[(DamageType)int.Parse(item.Attribute("fromtype").Value)] =
					((WoundSeverity)int.Parse(item.Attribute("severity").Value), (DamageType)int.Parse(
						item.Attribute("totype").Value));
			}
		}

		foreach (var dt in Enum.GetValues(typeof(DamageType)).OfType<DamageType>().ToList())
		{
			if (!DissipateExpressions.ContainsKey(dt))
			{
				DissipateExpressions[dt] = new Expression("damage");
			}

			if (!AbsorbExpressions.ContainsKey(dt))
			{
				AbsorbExpressions[dt] = new Expression("damage");
			}

			if (!DissipateExpressionsPain.ContainsKey(dt))
			{
				DissipateExpressionsPain[dt] = new Expression("pain");
			}

			if (!AbsorbExpressionsPain.ContainsKey(dt))
			{
				AbsorbExpressionsPain[dt] = new Expression("pain");
			}

			if (!DissipateExpressionsStun.ContainsKey(dt))
			{
				DissipateExpressionsStun[dt] = new Expression("stun");
			}

			if (!AbsorbExpressionsStun.ContainsKey(dt))
			{
				AbsorbExpressionsStun[dt] = new Expression("stun");
			}
		}
	}

	public Dictionary<DamageType, Expression> DissipateExpressions { get; set; } =
		new();

	public Dictionary<DamageType, Expression> AbsorbExpressions { get; set; } =
		new();

	public Dictionary<DamageType, Expression> DissipateExpressionsPain { get; set; } =
		new();

	public Dictionary<DamageType, Expression> AbsorbExpressionsPain { get; set; } =
		new();

	public Dictionary<DamageType, Expression> DissipateExpressionsStun { get; set; } =
		new();

	public Dictionary<DamageType, Expression> AbsorbExpressionsStun { get; set; } =
		new();

	public Dictionary<DamageType, (WoundSeverity Threshold, DamageType Transform)> DamageTypeTransformations =
		new();

	public OpposedOutcomeDegree MinimumPenetrationDegree { get; set; }
	public int BaseDifficultyBonus { get; set; }
	public int StackedDifficultyBonus { get; set; }

	/// <summary>
	///     This call represents armours that do not take damage being called upon to absorb - such as spells, and natural
	///     armour. The main difference is that it only has a dissipate check, not an absorb check.
	/// </summary>
	/// <param name="damage"></param>
	/// <param name="quality"></param>
	/// <param name="material"></param>
	/// <param name="owner"></param>
	/// <param name="wounds"></param>
	/// <returns></returns>
	public (IDamage partDamage, IDamage organDamge) AbsorbDamage(IDamage damage, ItemQuality quality,
		IMaterial material, IHaveWounds owner,
		ref List<IWound> wounds)
	{
		var solid = material as ISolid;
		var strength = 0.0;
		switch (damage.DamageType)
		{
			case DamageType.Slashing:
			case DamageType.Chopping:
			case DamageType.Piercing:
			case DamageType.Ballistic:
			case DamageType.ArmourPiercing:
			case DamageType.Shearing:
			case DamageType.Claw:
			case DamageType.Bite:
			case DamageType.Shrapnel:
			case DamageType.Wrenching:
				strength = solid?.ShearYield ?? 0.0;
				break;
			case DamageType.Crushing:
			case DamageType.Shockwave:
			case DamageType.Falling:
				strength = solid?.ImpactYield ?? 0.0;
				break;
		}

		var originalValues = (damage.DamageAmount, damage.PainAmount, damage.StunAmount);

		// Dissipate - dissipation is basically damage that is ignored or written off by the armour
		var dissipateExpression = DissipateExpressions[damage.DamageType];
		dissipateExpression.Parameters["quality"] = (int)quality;
		dissipateExpression.Parameters["damage"] = damage.DamageAmount;
		dissipateExpression.Parameters["angle"] = damage.AngleOfIncidentRadians.RadiansToDegrees();
		dissipateExpression.Parameters["density"] = material?.Density ?? 1.0;
		dissipateExpression.Parameters["electrical"] = material?.ElectricalConductivity ?? 1.0;
		dissipateExpression.Parameters["thermal"] = material?.ThermalConductivity ?? 1.0;
		dissipateExpression.Parameters["organic"] = material?.Organic == true ? 1.0 : 0.0;
		dissipateExpression.Parameters["strength"] = strength;
		var newDamage = Convert.ToDouble(dissipateExpression.Evaluate());

		dissipateExpression = DissipateExpressionsStun[damage.DamageType];
		dissipateExpression.Parameters["quality"] = (int)quality;
		dissipateExpression.Parameters["stun"] = damage.StunAmount;
		dissipateExpression.Parameters["damage"] = damage.StunAmount;
		dissipateExpression.Parameters["angle"] = damage.AngleOfIncidentRadians.RadiansToDegrees();
		dissipateExpression.Parameters["density"] = material?.Density ?? 1.0;
		dissipateExpression.Parameters["electrical"] = material?.ElectricalConductivity ?? 1.0;
		dissipateExpression.Parameters["thermal"] = material?.ThermalConductivity ?? 1.0;
		dissipateExpression.Parameters["organic"] = material?.Organic == true ? 1.0 : 0.0;
		dissipateExpression.Parameters["strength"] = strength;
		var newStun = Convert.ToDouble(dissipateExpression.Evaluate());

		dissipateExpression = DissipateExpressionsPain[damage.DamageType];
		dissipateExpression.Parameters["quality"] = (int)quality;
		dissipateExpression.Parameters["pain"] = damage.PainAmount;
		dissipateExpression.Parameters["damage"] = damage.PainAmount;
		dissipateExpression.Parameters["angle"] = damage.AngleOfIncidentRadians.RadiansToDegrees();
		dissipateExpression.Parameters["density"] = material?.Density ?? 1.0;
		dissipateExpression.Parameters["electrical"] = material?.ElectricalConductivity ?? 1.0;
		dissipateExpression.Parameters["thermal"] = material?.ThermalConductivity ?? 1.0;
		dissipateExpression.Parameters["organic"] = material?.Organic == true ? 1.0 : 0.0;
		dissipateExpression.Parameters["strength"] = strength;
		var newPain = Convert.ToDouble(dissipateExpression.Evaluate());

		if (newDamage <= 0 && newStun <= 0 && newPain <= 0)
		{
			return (null, null);
		}

		var partDamage = new Damage
		{
			DamageType = damage.DamageType,
			ActorOrigin = damage.ActorOrigin,
			AngleOfIncidentRadians = damage.AngleOfIncidentRadians,
			Bodypart = damage.Bodypart,
			DamageAmount = newDamage,
			LodgableItem = damage.LodgableItem,
			PainAmount = newPain,
			PenetrationOutcome = damage.PenetrationOutcome,
			ShockAmount = damage.ShockAmount,
			StunAmount = newStun
		};

		// Absorb - Absorb is damage less that the armour passes on to layers below it
		var absorbExpression = AbsorbExpressions[damage.DamageType];
		absorbExpression.Parameters["quality"] = (int)quality;
		absorbExpression.Parameters["damage"] = partDamage.DamageAmount;
		absorbExpression.Parameters["angle"] = partDamage.AngleOfIncidentRadians.RadiansToDegrees();
		absorbExpression.Parameters["density"] = material?.Density ?? 1.0;
		absorbExpression.Parameters["electrical"] = material?.ElectricalConductivity ?? 1.0;
		absorbExpression.Parameters["thermal"] = material?.ThermalConductivity ?? 1.0;
		absorbExpression.Parameters["organic"] = material?.Organic == true ? 1.0 : 0.0;
		absorbExpression.Parameters["strength"] = strength;
		absorbExpression.Parameters["originaldamage"] = originalValues.DamageAmount;
		newDamage = Convert.ToDouble(absorbExpression.Evaluate());

		absorbExpression = AbsorbExpressionsStun[damage.DamageType];
		absorbExpression.Parameters["quality"] = (int)quality;
		absorbExpression.Parameters["stun"] = partDamage.StunAmount;
		absorbExpression.Parameters["angle"] = partDamage.AngleOfIncidentRadians.RadiansToDegrees();
		absorbExpression.Parameters["density"] = material?.Density ?? 1.0;
		absorbExpression.Parameters["electrical"] = material?.ElectricalConductivity ?? 1.0;
		absorbExpression.Parameters["thermal"] = material?.ThermalConductivity ?? 1.0;
		absorbExpression.Parameters["organic"] = material?.Organic == true ? 1.0 : 0.0;
		absorbExpression.Parameters["strength"] = strength;
		absorbExpression.Parameters["originaldamage"] = originalValues.StunAmount;
		absorbExpression.Parameters["originalstun"] = originalValues.StunAmount;
		newStun = Convert.ToDouble(absorbExpression.Evaluate());

		absorbExpression = AbsorbExpressionsPain[damage.DamageType];
		absorbExpression.Parameters["quality"] = (int)quality;
		absorbExpression.Parameters["pain"] = partDamage.PainAmount;
		absorbExpression.Parameters["angle"] = partDamage.AngleOfIncidentRadians.RadiansToDegrees();
		absorbExpression.Parameters["density"] = material?.Density ?? 1.0;
		absorbExpression.Parameters["electrical"] = material?.ElectricalConductivity ?? 1.0;
		absorbExpression.Parameters["thermal"] = material?.ThermalConductivity ?? 1.0;
		absorbExpression.Parameters["organic"] = material?.Organic == true ? 1.0 : 0.0;
		absorbExpression.Parameters["strength"] = strength;
		absorbExpression.Parameters["originaldamage"] = originalValues.PainAmount;
		absorbExpression.Parameters["originalpain"] = originalValues.PainAmount;
		newPain = Convert.ToDouble(absorbExpression.Evaluate());

		if (newDamage <= 0 && newStun <= 0 && newPain <= 0)
		{
			return (partDamage, null);
		}

		var newDamageType = damage.DamageType;
		if (DamageTypeTransformations.ContainsKey(newDamageType))
		{
			if (owner.HealthStrategy.GetSeverity(newDamage) <= DamageTypeTransformations[newDamageType].Threshold)
			{
				newDamageType = DamageTypeTransformations[newDamageType].Transform;
			}
		}

		return (partDamage, new Damage
		{
			DamageType = newDamageType,
			ActorOrigin = damage.ActorOrigin,
			AngleOfIncidentRadians = damage.AngleOfIncidentRadians,
			Bodypart = damage.Bodypart,
			DamageAmount = newDamage,
			LodgableItem = damage.LodgableItem,
			PainAmount = newPain,
			PenetrationOutcome = damage.PenetrationOutcome,
			ShockAmount = damage.ShockAmount,
			StunAmount = newStun
		});
	}

	public IDamage AbsorbDamage(IDamage damage, IArmour armour, IHaveWounds owner, ref List<IWound> wounds,
		bool passive)
	{
		// Penetrate
		var penetrateDefenseCheck = Gameworld.GetCheck(CheckType.PenetrationDefenseCheck);
		var penetrateDefenseResult = owner is ICharacter ownerAsCharacter
			? penetrateDefenseCheck.Check(ownerAsCharacter, Difficulty.Normal)
			: Outcome.NotTested;
		var penetrateOutcome = new OpposedOutcome(damage.PenetrationOutcome, penetrateDefenseResult);
#if DEBUG
		Console.WriteLine(
			$"Penetration {damage.PenetrationOutcome.Describe()} vs {penetrateDefenseResult.Describe()} - Outcome {penetrateOutcome.Outcome.Describe()} ({penetrateOutcome.Degree.Describe()}). Required degree {MinimumPenetrationDegree.Describe()}");
#endif
		if (penetrateOutcome.Outcome == OpposedOutcomeDirection.Proponent &&
		    penetrateOutcome.Degree >= MinimumPenetrationDegree)
		{
			return damage;
		}

		var solid = armour.Parent.Material as ISolid;
		var strength = 0.0;
		switch (damage.DamageType)
		{
			case DamageType.Slashing:
			case DamageType.Chopping:
			case DamageType.Piercing:
			case DamageType.Ballistic:
			case DamageType.Claw:
			case DamageType.Bite:
			case DamageType.ArmourPiercing:
			case DamageType.Shearing:
				strength = solid?.ShearYield ?? 0.0;
				break;
			case DamageType.Crushing:
			case DamageType.Shockwave:
				strength = solid?.ImpactYield ?? 0.0;
				break;
		}

		var originalValues = (damage.DamageAmount, damage.PainAmount, damage.StunAmount);

		// Dissipate - dissipation is basically damage that is ignored or written off by the armour
		var dissipateExpression = DissipateExpressions[damage.DamageType];
		dissipateExpression.Parameters["quality"] = (int)armour.Parent.Quality;
		dissipateExpression.Parameters["damage"] = damage.DamageAmount;
		dissipateExpression.Parameters["angle"] = damage.AngleOfIncidentRadians.RadiansToDegrees();
		dissipateExpression.Parameters["density"] = armour.Parent.Material?.Density ?? 1.0;
		dissipateExpression.Parameters["electrical"] = armour.Parent.Material?.ElectricalConductivity ?? 1.0;
		dissipateExpression.Parameters["thermal"] = armour.Parent.Material?.ThermalConductivity ?? 1.0;
		dissipateExpression.Parameters["organic"] = armour.Parent.Material?.Organic == true ? 1.0 : 0.0;
		dissipateExpression.Parameters["strength"] = strength;
		var newDamage = Convert.ToDouble(dissipateExpression.Evaluate());

		dissipateExpression = DissipateExpressionsStun[damage.DamageType];
		dissipateExpression.Parameters["quality"] = (int)armour.Parent.Quality;
		dissipateExpression.Parameters["stun"] = damage.StunAmount;
		dissipateExpression.Parameters["damage"] = damage.StunAmount;
		dissipateExpression.Parameters["angle"] = damage.AngleOfIncidentRadians.RadiansToDegrees();
		dissipateExpression.Parameters["density"] = armour.Parent.Material?.Density ?? 1.0;
		dissipateExpression.Parameters["electrical"] = armour.Parent.Material?.ElectricalConductivity ?? 1.0;
		dissipateExpression.Parameters["thermal"] = armour.Parent.Material?.ThermalConductivity ?? 1.0;
		dissipateExpression.Parameters["organic"] = armour.Parent.Material?.Organic == true ? 1.0 : 0.0;
		dissipateExpression.Parameters["strength"] = strength;
		var newStun = Convert.ToDouble(dissipateExpression.Evaluate());

		dissipateExpression = DissipateExpressionsPain[damage.DamageType];
		dissipateExpression.Parameters["quality"] = (int)armour.Parent.Quality;
		dissipateExpression.Parameters["pain"] = damage.PainAmount;
		dissipateExpression.Parameters["damage"] = damage.PainAmount;
		dissipateExpression.Parameters["angle"] = damage.AngleOfIncidentRadians.RadiansToDegrees();
		dissipateExpression.Parameters["density"] = armour.Parent.Material?.Density ?? 1.0;
		dissipateExpression.Parameters["electrical"] = armour.Parent.Material?.ElectricalConductivity ?? 1.0;
		dissipateExpression.Parameters["thermal"] = armour.Parent.Material?.ThermalConductivity ?? 1.0;
		dissipateExpression.Parameters["organic"] = armour.Parent.Material?.Organic == true ? 1.0 : 0.0;
		dissipateExpression.Parameters["strength"] = strength;
		var newPain = Convert.ToDouble(dissipateExpression.Evaluate());

		if (newDamage <= 0 && newStun <= 0 && newPain <= 0)
		{
			return null;
		}

		// Absorb - after suffering damage itself, the armour will remove an amount equal to its absorb expression before passing it on
		var armourDamage = new Damage
		{
			DamageType = damage.DamageType,
			ActorOrigin = damage.ActorOrigin,
			AngleOfIncidentRadians = damage.AngleOfIncidentRadians,
			Bodypart = null,
			DamageAmount = newDamage,
			LodgableItem = damage.LodgableItem,
			PainAmount = newPain,
			PenetrationOutcome = Outcome.NotTested,
			ShockAmount = damage.ShockAmount,
			StunAmount = newStun
		};
		var armourWounds = passive
			? armour.Parent.PassiveSufferDamage(armourDamage).ToList()
			: armour.Parent.SufferDamage(armourDamage).ToList();

		if (armourWounds.Any())
		{
			wounds.AddRange(armourWounds);
		}

		var absorbExpression = AbsorbExpressions[damage.DamageType];
		absorbExpression.Parameters["quality"] = (int)armour.Parent.Quality;
		absorbExpression.Parameters["damage"] = damage.DamageAmount;
		absorbExpression.Parameters["angle"] = damage.AngleOfIncidentRadians.RadiansToDegrees();
		absorbExpression.Parameters["density"] = armour.Parent.Material?.Density ?? 1.0;
		absorbExpression.Parameters["electrical"] = armour.Parent.Material?.ElectricalConductivity ?? 1.0;
		absorbExpression.Parameters["thermal"] = armour.Parent.Material?.ThermalConductivity ?? 1.0;
		absorbExpression.Parameters["organic"] = armour.Parent.Material?.Organic == true ? 1.0 : 0.0;
		absorbExpression.Parameters["strength"] = strength;
		absorbExpression.Parameters["originaldamage"] = originalValues.DamageAmount;
		newDamage = Convert.ToDouble(absorbExpression.Evaluate());

		absorbExpression = AbsorbExpressionsStun[damage.DamageType];
		absorbExpression.Parameters["quality"] = (int)armour.Parent.Quality;
		absorbExpression.Parameters["stun"] = damage.StunAmount;
		absorbExpression.Parameters["damage"] = damage.StunAmount;
		absorbExpression.Parameters["angle"] = damage.AngleOfIncidentRadians.RadiansToDegrees();
		absorbExpression.Parameters["density"] = armour.Parent.Material?.Density ?? 1.0;
		absorbExpression.Parameters["electrical"] = armour.Parent.Material?.ElectricalConductivity ?? 1.0;
		absorbExpression.Parameters["thermal"] = armour.Parent.Material?.ThermalConductivity ?? 1.0;
		absorbExpression.Parameters["organic"] = armour.Parent.Material?.Organic == true ? 1.0 : 0.0;
		absorbExpression.Parameters["strength"] = strength;
		absorbExpression.Parameters["originaldamage"] = originalValues.StunAmount;
		absorbExpression.Parameters["originalstun"] = originalValues.StunAmount;
		newStun = Convert.ToDouble(absorbExpression.Evaluate());

		absorbExpression = AbsorbExpressionsPain[damage.DamageType];
		absorbExpression.Parameters["quality"] = (int)armour.Parent.Quality;
		absorbExpression.Parameters["pain"] = damage.PainAmount;
		absorbExpression.Parameters["damage"] = damage.PainAmount;
		absorbExpression.Parameters["angle"] = damage.AngleOfIncidentRadians.RadiansToDegrees();
		absorbExpression.Parameters["density"] = armour.Parent.Material?.Density ?? 1.0;
		absorbExpression.Parameters["electrical"] = armour.Parent.Material?.ElectricalConductivity ?? 1.0;
		absorbExpression.Parameters["thermal"] = armour.Parent.Material?.ThermalConductivity ?? 1.0;
		absorbExpression.Parameters["organic"] = armour.Parent.Material?.Organic == true ? 1.0 : 0.0;
		absorbExpression.Parameters["strength"] = strength;
		absorbExpression.Parameters["originaldamage"] = originalValues.PainAmount;
		absorbExpression.Parameters["originalpain"] = originalValues.PainAmount;
		newPain = Convert.ToDouble(absorbExpression.Evaluate());

		if (newDamage <= 0 && newStun <= 0 && newPain <= 0)
		{
			return null;
		}

		var newDamageType = damage.DamageType;
		if (DamageTypeTransformations.ContainsKey(newDamageType))
		{
			if (owner.HealthStrategy.GetSeverity(newDamage) <= DamageTypeTransformations[newDamageType].Threshold)
			{
				newDamageType = DamageTypeTransformations[newDamageType].Transform;
			}
		}

		return new Damage
		{
			DamageType = newDamageType,
			ActorOrigin = damage.ActorOrigin,
			AngleOfIncidentRadians = damage.AngleOfIncidentRadians,
			Bodypart = damage.Bodypart,
			DamageAmount = newDamage,
			LodgableItem = armourWounds.Any(x => x.Lodged != null) ? null : damage.LodgableItem,
			PainAmount = newPain,
			PenetrationOutcome = damage.PenetrationOutcome,
			ShockAmount = damage.ShockAmount,
			StunAmount = newStun
		};
	}


	public (IDamage PassedOn, IDamage Absorbed) AbsorbDamageViaSpell(IDamage damage, ISolid solid, ItemQuality quality,
		IHaveWounds owner,
		bool passive)
	{
		// Penetrate
		var penetrateDefenseCheck = Gameworld.GetCheck(CheckType.PenetrationDefenseCheck);
		var penetrateDefenseResult = owner is ICharacter ownerAsCharacter
			? penetrateDefenseCheck.Check(ownerAsCharacter, Difficulty.Normal)
			: Outcome.NotTested;
		var penetrateOutcome = new OpposedOutcome(damage.PenetrationOutcome, penetrateDefenseResult);
#if DEBUG
		Console.WriteLine(
			$"Penetration {damage.PenetrationOutcome.Describe()} vs {penetrateDefenseResult.Describe()} - Outcome {penetrateOutcome.Outcome.Describe()} ({penetrateOutcome.Degree.Describe()}). Required degree {MinimumPenetrationDegree.Describe()}");
#endif
		if (penetrateOutcome.Outcome == OpposedOutcomeDirection.Proponent &&
		    penetrateOutcome.Degree >= MinimumPenetrationDegree)
		{
			return (damage, null);
		}

		var strength = 0.0;
		switch (damage.DamageType)
		{
			case DamageType.Slashing:
			case DamageType.Chopping:
			case DamageType.Piercing:
			case DamageType.Ballistic:
			case DamageType.Claw:
			case DamageType.Bite:
			case DamageType.ArmourPiercing:
			case DamageType.Shearing:
				strength = solid?.ShearYield ?? 0.0;
				break;
			case DamageType.Crushing:
			case DamageType.Shockwave:
				strength = solid?.ImpactYield ?? 0.0;
				break;
		}

		// Dissipate - dissipation is basically damage that is ignored or written off by the armour
		var dissipateExpression = DissipateExpressions[damage.DamageType];
		dissipateExpression.Parameters["quality"] = (int)quality;
		dissipateExpression.Parameters["damage"] = damage.DamageAmount;
		dissipateExpression.Parameters["angle"] = damage.AngleOfIncidentRadians.RadiansToDegrees();
		dissipateExpression.Parameters["density"] = solid?.Density ?? 1.0;
		dissipateExpression.Parameters["electrical"] = solid?.ElectricalConductivity ?? 1.0;
		dissipateExpression.Parameters["thermal"] = solid?.ThermalConductivity ?? 1.0;
		dissipateExpression.Parameters["organic"] = solid?.Organic == true ? 1.0 : 0.0;
		dissipateExpression.Parameters["strength"] = strength;
		var newDamage = Convert.ToDouble(dissipateExpression.Evaluate());

		dissipateExpression = DissipateExpressionsStun[damage.DamageType];
		dissipateExpression.Parameters["quality"] = (int)quality;
		dissipateExpression.Parameters["stun"] = damage.StunAmount;
		dissipateExpression.Parameters["angle"] = damage.AngleOfIncidentRadians.RadiansToDegrees();
		dissipateExpression.Parameters["density"] = solid?.Density ?? 1.0;
		dissipateExpression.Parameters["electrical"] = solid?.ElectricalConductivity ?? 1.0;
		dissipateExpression.Parameters["thermal"] = solid?.ThermalConductivity ?? 1.0;
		dissipateExpression.Parameters["organic"] = solid?.Organic == true ? 1.0 : 0.0;
		dissipateExpression.Parameters["strength"] = strength;
		var newStun = Convert.ToDouble(dissipateExpression.Evaluate());

		dissipateExpression = DissipateExpressionsPain[damage.DamageType];
		dissipateExpression.Parameters["quality"] = (int)quality;
		dissipateExpression.Parameters["pain"] = damage.PainAmount;
		dissipateExpression.Parameters["angle"] = damage.AngleOfIncidentRadians.RadiansToDegrees();
		dissipateExpression.Parameters["density"] = solid?.Density ?? 1.0;
		dissipateExpression.Parameters["electrical"] = solid?.ElectricalConductivity ?? 1.0;
		dissipateExpression.Parameters["thermal"] = solid?.ThermalConductivity ?? 1.0;
		dissipateExpression.Parameters["organic"] = solid?.Organic == true ? 1.0 : 0.0;
		dissipateExpression.Parameters["strength"] = strength;
		var newPain = Convert.ToDouble(dissipateExpression.Evaluate());

		if (newDamage <= 0 && newStun <= 0 && newPain <= 0)
		{
			return (null, null);
		}

		// Absorb - after suffering damage itself, the armour will remove an amount equal to its absorb expression before passing it on
		var armourDamage = new Damage
		{
			DamageType = damage.DamageType,
			ActorOrigin = damage.ActorOrigin,
			AngleOfIncidentRadians = damage.AngleOfIncidentRadians,
			Bodypart = null,
			DamageAmount = newDamage,
			LodgableItem = damage.LodgableItem,
			PainAmount = newPain,
			PenetrationOutcome = Outcome.NotTested,
			ShockAmount = damage.ShockAmount,
			StunAmount = newStun
		};

		var absorbExpression = AbsorbExpressions[damage.DamageType];
		absorbExpression.Parameters["quality"] = (int)quality;
		absorbExpression.Parameters["damage"] = damage.DamageAmount;
		absorbExpression.Parameters["angle"] = damage.AngleOfIncidentRadians.RadiansToDegrees();
		absorbExpression.Parameters["density"] = solid?.Density ?? 1.0;
		absorbExpression.Parameters["electrical"] = solid?.ElectricalConductivity ?? 1.0;
		absorbExpression.Parameters["thermal"] = solid?.ThermalConductivity ?? 1.0;
		dissipateExpression.Parameters["organic"] = solid?.Organic == true ? 1.0 : 0.0;
		dissipateExpression.Parameters["strength"] = strength;
		newDamage = Convert.ToDouble(absorbExpression.Evaluate());

		absorbExpression = AbsorbExpressionsStun[damage.DamageType];
		absorbExpression.Parameters["quality"] = (int)quality;
		absorbExpression.Parameters["stun"] = damage.StunAmount;
		absorbExpression.Parameters["angle"] = damage.AngleOfIncidentRadians.RadiansToDegrees();
		absorbExpression.Parameters["density"] = solid?.Density ?? 1.0;
		absorbExpression.Parameters["electrical"] = solid?.ElectricalConductivity ?? 1.0;
		absorbExpression.Parameters["thermal"] = solid?.ThermalConductivity ?? 1.0;
		dissipateExpression.Parameters["organic"] = solid?.Organic == true ? 1.0 : 0.0;
		dissipateExpression.Parameters["strength"] = strength;
		newStun = Convert.ToDouble(absorbExpression.Evaluate());

		absorbExpression = AbsorbExpressionsPain[damage.DamageType];
		absorbExpression.Parameters["quality"] = (int)quality;
		absorbExpression.Parameters["pain"] = damage.PainAmount;
		absorbExpression.Parameters["angle"] = damage.AngleOfIncidentRadians.RadiansToDegrees();
		absorbExpression.Parameters["density"] = solid?.Density ?? 1.0;
		absorbExpression.Parameters["electrical"] = solid?.ElectricalConductivity ?? 1.0;
		absorbExpression.Parameters["thermal"] = solid?.ThermalConductivity ?? 1.0;
		dissipateExpression.Parameters["organic"] = solid?.Organic == true ? 1.0 : 0.0;
		dissipateExpression.Parameters["strength"] = strength;
		newPain = Convert.ToDouble(absorbExpression.Evaluate());

		if (newDamage <= 0 && newStun <= 0 && newPain <= 0)
		{
			return (null, armourDamage);
		}

		var newDamageType = damage.DamageType;
		if (DamageTypeTransformations.ContainsKey(newDamageType))
		{
			if (owner.HealthStrategy.GetSeverity(newDamage) <= DamageTypeTransformations[newDamageType].Threshold)
			{
				newDamageType = DamageTypeTransformations[newDamageType].Transform;
			}
		}

		return (new Damage
		{
			DamageType = newDamageType,
			ActorOrigin = damage.ActorOrigin,
			AngleOfIncidentRadians = damage.AngleOfIncidentRadians,
			Bodypart = damage.Bodypart,
			DamageAmount = newDamage,
			LodgableItem = damage.LodgableItem,
			PainAmount = newPain,
			PenetrationOutcome = damage.PenetrationOutcome,
			ShockAmount = damage.ShockAmount,
			StunAmount = newStun
		}, armourDamage);
	}

	public string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Armour Type #{Id.ToString("N0", voyeur)} - {Name.TitleCase()}".ColourName());
		sb.AppendLine();
		sb.AppendLine($"Bonus When Worn Alone: {BaseDifficultyBonus.ToString("N0", voyeur).ColourValue()}");
		sb.AppendLine($"Bonus When Worn Stacked: {StackedDifficultyBonus.ToString("N0", voyeur).ColourValue()}");
		sb.AppendLine($"Minimum Degree for Penetration: {MinimumPenetrationDegree.Describe().ColourValue()}");
		sb.AppendLine();
		sb.AppendLine("Damage Transforms:".ColourName());
		sb.AppendLine();
		foreach (var item in DamageTypeTransformations)
		{
			sb.AppendLine(
				$"\t{item.Key.Describe().Colour(Telnet.Green)} => {item.Value.Transform.Describe().ColourValue()} when severity <= {item.Value.Threshold.Describe().ColourValue()}");
		}

		sb.AppendLine();
		sb.AppendLine("Armour Formulae".ColourName());
		sb.AppendLine();
		sb.AppendLine("Note - Armour is applied in the following sequence");
		sb.AppendLine(
			$"{"Incoming Damage".ColourName()} -> {"Dissipate".ColourName()} -> {"Damage Armour".ColourName()} -> {"Absorb".ColourName()} -> {"Pass Residual Damage Down".ColourName()}");
		sb.AppendLine();
		sb.AppendLine(@"There are also numerous variables that may appear, which can be any of the following:

	#3quality#0 - the Quality of the item 0 (terrible) - 5 (normal) - 11 (legendary)
	#3damage#0 - the damage/stun/pain of the attack
	#3angle#0 - the angle in radians that the attack hit. This is determine by the weapon attack type and how successful the defense was (even if missed). 
	#3density#0 - the relative density (specific gravity) of  the material that the armour is made out of
	#3electrical#0 - the electrical resistivity of the material in 1/ohm metres
	#3thermal#0 - the thermal resistivity of the material in watts per meter per kelvin
	#3organic#0 - 1 if the material is organic, 0 if not
	#3strength#0 - either the shear or tensile strength of the material depending on the damage type, in pascals"
			.SubstituteANSIColour());
		sb.AppendLine();
		sb.AppendLine("Dissipate Damage:".ColourName());
		sb.AppendLine();
		foreach (var item in DissipateExpressions.OrderBy(x => x.Key))
		{
			sb.AppendLine($"\t{item.Key.Describe().ColourValue()}: {item.Value.OriginalExpression.ColourCommand()}");
		}

		sb.AppendLine();
		sb.AppendLine("Absorb Damage:".ColourName());
		sb.AppendLine();
		foreach (var item in AbsorbExpressions.OrderBy(x => x.Key))
		{
			sb.AppendLine($"\t{item.Key.Describe().ColourValue()}: {item.Value.OriginalExpression.ColourCommand()}");
		}

		sb.AppendLine();
		sb.AppendLine("Dissipate Stun:".ColourName());
		sb.AppendLine();
		foreach (var item in DissipateExpressionsStun.OrderBy(x => x.Key))
		{
			sb.AppendLine($"\t{item.Key.Describe().ColourValue()}: {item.Value.OriginalExpression.ColourCommand()}");
		}

		sb.AppendLine();
		sb.AppendLine("Absorb Stun:".ColourName());
		sb.AppendLine();
		foreach (var item in AbsorbExpressionsStun.OrderBy(x => x.Key))
		{
			sb.AppendLine($"\t{item.Key.Describe().ColourValue()}: {item.Value.OriginalExpression.ColourCommand()}");
		}

		sb.AppendLine();
		sb.AppendLine("Dissipate Pain:".ColourName());
		sb.AppendLine();
		foreach (var item in DissipateExpressionsPain.OrderBy(x => x.Key))
		{
			sb.AppendLine($"\t{item.Key.Describe().ColourValue()}: {item.Value.OriginalExpression.ColourCommand()}");
		}

		sb.AppendLine();
		sb.AppendLine("Absorb Pain:".ColourName());
		sb.AppendLine();
		foreach (var item in AbsorbExpressionsPain.OrderBy(x => x.Key))
		{
			sb.AppendLine($"\t{item.Key.Describe().ColourValue()}: {item.Value.OriginalExpression.ColourCommand()}");
		}

		return sb.ToString();
	}

	#region Overrides of Item

	public override string FrameworkItemType { get; } = "ArmourType";

	#endregion

	#region Implementation of IHaveFuturemud

	public IFuturemud Gameworld { get; set; }

	#endregion
}