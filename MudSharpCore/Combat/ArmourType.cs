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
using MudSharp.Database;
using MudSharp.Framework.Save;

namespace MudSharp.Combat;

public class ArmourType : SaveableItem, IArmourType
{
	public ArmourType(IFuturemud gameworld, string name)
	{
		Gameworld = gameworld;
		_name = name;
		MinimumPenetrationDegree = OpposedOutcomeDegree.Major;
		BaseDifficultyBonus = 1;
		StackedDifficultyBonus = 0;
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

		DamageTypeTransformations[DamageType.Slashing] = (WoundSeverity.Severe, DamageType.Crushing);
		DamageTypeTransformations[DamageType.Chopping] = (WoundSeverity.Severe, DamageType.Crushing);
		DamageTypeTransformations[DamageType.Piercing] = (WoundSeverity.Moderate, DamageType.Crushing);
		DamageTypeTransformations[DamageType.Ballistic] = (WoundSeverity.Moderate, DamageType.Crushing);
		DamageTypeTransformations[DamageType.Bite] = (WoundSeverity.Severe, DamageType.Crushing);
		DamageTypeTransformations[DamageType.Claw] = (WoundSeverity.Severe, DamageType.Crushing);
		DamageTypeTransformations[DamageType.Shearing] = (WoundSeverity.Severe, DamageType.Crushing);
		DamageTypeTransformations[DamageType.Wrenching] = (WoundSeverity.Severe, DamageType.Crushing);
		DamageTypeTransformations[DamageType.BallisticArmourPiercing] = (WoundSeverity.Severe, DamageType.Crushing);
		DamageTypeTransformations[DamageType.ArmourPiercing] = (WoundSeverity.Severe, DamageType.Crushing);
		
		using (new FMDB())
		{
			var dbitem = new Models.ArmourType();
			dbitem.Name = Name;
			dbitem.MinimumPenetrationDegree = (int)MinimumPenetrationDegree;
			dbitem.BaseDifficultyDegrees = BaseDifficultyBonus;
			dbitem.StackedDifficultyDegrees = StackedDifficultyBonus;
			dbitem.Definition = SaveDefinition().ToString();
			FMDB.Context.ArmourTypes.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

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

	private ArmourType(ArmourType rhs, string newName)
	{
		Gameworld = rhs.Gameworld;
		_name = newName;
		MinimumPenetrationDegree = rhs.MinimumPenetrationDegree;
		BaseDifficultyBonus = rhs.BaseDifficultyBonus;
		StackedDifficultyBonus = rhs.StackedDifficultyBonus;

		foreach (var item in rhs.AbsorbExpressions)
		{
			AbsorbExpressions[item.Key] = new Expression(item.Value.OriginalExpression);
		}
		foreach (var item in rhs.AbsorbExpressionsPain)
		{
			AbsorbExpressionsPain[item.Key] = new Expression(item.Value.OriginalExpression);
		}
		foreach (var item in rhs.AbsorbExpressionsStun)
		{
			AbsorbExpressionsStun[item.Key] = new Expression(item.Value.OriginalExpression);
		}
		foreach (var item in rhs.DissipateExpressions)
		{
			DissipateExpressions[item.Key] = new Expression(item.Value.OriginalExpression);
		}
		foreach (var item in rhs.DissipateExpressionsPain)
		{
			DissipateExpressionsPain[item.Key] = new Expression(item.Value.OriginalExpression);
		}
		foreach (var item in rhs.DissipateExpressionsStun)
		{
			DissipateExpressionsStun[item.Key] = new Expression(item.Value.OriginalExpression);
		}

		foreach (var item in rhs.DamageTypeTransformations)
		{
			DamageTypeTransformations[item.Key] = (item.Value.Threshold, item.Value.Transform);
		}

		using (new FMDB())
		{
			var dbitem = new Models.ArmourType();
			dbitem.Name = Name;
			dbitem.MinimumPenetrationDegree = (int)MinimumPenetrationDegree;
			dbitem.BaseDifficultyDegrees = BaseDifficultyBonus;
			dbitem.StackedDifficultyDegrees = StackedDifficultyBonus;
			dbitem.Definition = SaveDefinition().ToString();
			FMDB.Context.ArmourTypes.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public IArmourType Clone(string newName)
	{
		return new ArmourType(this, newName);
	}

	/// <inheritdoc />
	public override void Save()
	{
		var dbitem = FMDB.Context.ArmourTypes.Find(Id);
		dbitem.MinimumPenetrationDegree = (int)MinimumPenetrationDegree;
		dbitem.BaseDifficultyDegrees = BaseDifficultyBonus;
		dbitem.Name = Name;
		dbitem.StackedDifficultyDegrees = StackedDifficultyBonus;
		dbitem.Definition = SaveDefinition().ToString();
		Changed = false;
	}

	protected XElement SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("DissipateExpressions",
				from item in DissipateExpressions
				select new XElement("Expression",
					new XAttribute("damagetype", (int)item.Key),
					new XCData(item.Value.OriginalExpression)
				)
			), 
			new XElement("DissipateExpressionsPain",
				from item in DissipateExpressionsPain
				select new XElement("Expression",
					new XAttribute("damagetype", (int)item.Key),
					new XCData(item.Value.OriginalExpression)
				)
			), 
			new XElement("DissipateExpressionsStun",
				from item in DissipateExpressionsStun
				select new XElement("Expression",
					new XAttribute("damagetype", (int)item.Key),
					new XCData(item.Value.OriginalExpression)
				)
			),
			new XElement("AbsorbExpressions",
				from item in AbsorbExpressions
				select new XElement("Expression",
					new XAttribute("damagetype", (int)item.Key),
					new XCData(item.Value.OriginalExpression)
				)
			),
			new XElement("AbsorbExpressionsPain",
				from item in AbsorbExpressionsPain
				select new XElement("Expression",
					new XAttribute("damagetype", (int)item.Key),
					new XCData(item.Value.OriginalExpression)
				)
			),
			new XElement("AbsorbExpressionsStun",
				from item in AbsorbExpressionsStun
				select new XElement("Expression",
					new XAttribute("damagetype", (int)item.Key),
					new XCData(item.Value.OriginalExpression)
				)
			),
			new XElement("DamageTransformations",
				from item in DamageTypeTransformations
				select new XElement("Transform",
					new XAttribute("fromtype", (int)item.Key),
					new XAttribute("totype", (int)item.Value.Transform),
					new XAttribute("severity", (int)item.Value.Threshold)
				)
			)
		);
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
	public double BaseDifficultyBonus { get; set; }
	public double StackedDifficultyBonus { get; set; }

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
	public (IDamage SufferedDamage, IDamage PassThroughDamage) AbsorbDamage(IDamage damage, ItemQuality quality,
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
			case DamageType.ArmourPiercing:
			case DamageType.Ballistic:
			case DamageType.BallisticArmourPiercing:
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
			case DamageType.ArmourPiercing:
			case DamageType.Ballistic:
			case DamageType.Claw:
			case DamageType.Bite:
			case DamageType.BallisticArmourPiercing:
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
			case DamageType.BallisticArmourPiercing:
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
		sb.AppendLine($"Armour Type #{Id.ToString("N0", voyeur)} - {Name.TitleCase()}".GetLineWithTitle(voyeur, Telnet.Orange, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Bonus When Worn Alone: {BaseDifficultyBonus.ToBonusString(voyeur)}");
		sb.AppendLine($"Bonus When Worn Stacked: {StackedDifficultyBonus.ToBonusString(voyeur)}");
		sb.AppendLine($"Minimum Degree for Penetration: {MinimumPenetrationDegree.DescribeColour()}");
		sb.AppendLine();
		sb.AppendLine("Damage Transforms".GetLineWithTitle(voyeur, Telnet.Orange, Telnet.BoldWhite));
		sb.AppendLine();
		foreach (var item in DamageTypeTransformations)
		{
			sb.AppendLine(
				$"\t{item.Key.Describe().Colour(Telnet.Green)} => {item.Value.Transform.Describe().ColourValue()} when severity <= {item.Value.Threshold.Describe().ColourValue()}");
		}

		sb.AppendLine();
		sb.AppendLine("Armour Formulae".GetLineWithTitle(voyeur, Telnet.Orange, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine("Note - Armour is applied in the following sequence");
		sb.AppendLine(
			$"{"Incoming Damage".ColourName()} -> {"Dissipate".ColourName()} -> {"Damage Armour/Bodypart".ColourName()} -> {"Absorb".ColourName()} -> {"Pass Residual Damage Down a Layer".ColourName()}");
		sb.AppendLine();
		sb.AppendLine(@"There are also numerous variables that may appear, which can be any of the following:

	#3quality#0 - the Quality of the item 0 (terrible) - 5 (normal) - 11 (legendary)
	#3damage#0 - the damage/stun/pain of the attack
	#3originaldamage#0 - the original pre-mitigation damage/stun/pain of the attack (absorb formulae only)
	#3angle#0 - the angle in radians that the attack hit. This is determine by the weapon attack type and how successful the defense was (even if missed). 
	#3density#0 - the relative density (specific gravity) of  the material that the armour is made out of
	#3electrical#0 - the electrical resistivity of the material in 1/ohm metres
	#3thermal#0 - the thermal resistivity of the material in watts per meter per kelvin
	#3organic#0 - 1 if the material is organic, 0 if not
	#3strength#0 - either the shear or tensile strength of the material depending on the damage type, in pascals"
			.SubstituteANSIColour());
		sb.AppendLine();
		sb.AppendLine("Note: damage/originaldamage can be stun/originalstun or pain/originalpain in the stun/pain formulas. They are the same value.".Colour(Telnet.Cyan));
		sb.AppendLine();
		sb.AppendLine("Dissipate Damage".GetLineWithTitle(voyeur, Telnet.Orange, Telnet.BoldWhite));
		sb.AppendLine();
		foreach (var item in DissipateExpressions.OrderBy(x => x.Key))
		{
			sb.AppendLine($"\t{item.Key.Describe().ColourValue()}: {item.Value.OriginalExpression.ColourCommand()}");
		}

		sb.AppendLine();
		sb.AppendLine("Absorb Damage".GetLineWithTitle(voyeur, Telnet.Orange, Telnet.BoldWhite));
		sb.AppendLine();
		foreach (var item in AbsorbExpressions.OrderBy(x => x.Key))
		{
			sb.AppendLine($"\t{item.Key.Describe().ColourValue()}: {item.Value.OriginalExpression.ColourCommand()}");
		}

		sb.AppendLine();
		sb.AppendLine("Dissipate Stun".GetLineWithTitle(voyeur, Telnet.Orange, Telnet.BoldWhite));
		sb.AppendLine();
		foreach (var item in DissipateExpressionsStun.OrderBy(x => x.Key))
		{
			sb.AppendLine($"\t{item.Key.Describe().ColourValue()}: {item.Value.OriginalExpression.ColourCommand()}");
		}

		sb.AppendLine();
		sb.AppendLine("Absorb Stun".GetLineWithTitle(voyeur, Telnet.Orange, Telnet.BoldWhite));
		sb.AppendLine();
		foreach (var item in AbsorbExpressionsStun.OrderBy(x => x.Key))
		{
			sb.AppendLine($"\t{item.Key.Describe().ColourValue()}: {item.Value.OriginalExpression.ColourCommand()}");
		}

		sb.AppendLine();
		sb.AppendLine("Dissipate Pain".GetLineWithTitle(voyeur, Telnet.Orange, Telnet.BoldWhite));
		sb.AppendLine();
		foreach (var item in DissipateExpressionsPain.OrderBy(x => x.Key))
		{
			sb.AppendLine($"\t{item.Key.Describe().ColourValue()}: {item.Value.OriginalExpression.ColourCommand()}");
		}

		sb.AppendLine();
		sb.AppendLine("Absorb Pain".GetLineWithTitle(voyeur, Telnet.Orange, Telnet.BoldWhite));
		sb.AppendLine();
		foreach (var item in AbsorbExpressionsPain.OrderBy(x => x.Key))
		{
			sb.AppendLine($"\t{item.Key.Describe().ColourValue()}: {item.Value.OriginalExpression.ColourCommand()}");
		}

		return sb.ToString();
	}

	public const string BuildingHelpText = @"You can use the following options with this command:

	#3name <name>#0 - renames the armour type
	#3penetration <outcome>#0 - sets the minimum outcome required for penetration
	#3difficulty <bonus>#0 - sets the base penalty for wearing this armour
	#3stacked <bonus>#0 - sets the penalty for wearing this armour when stacked
	#3transform <from> <to> <severity>#0 - sets a damage type transformation
	#3transform <type> none#0 - clears a damage type transform
	#3dissipate damage|stun|pain <damagetype> <formula>#0 - sets the dissipate damage/stun/pain formula for a damage type
	#3absorb damage|stun|pain <damagetype> <formula>#0 - sets the absorb damage/stun/pain formula for a damage type

Note, the formulas use the following parameters:

	#6damage#0 - the raw damage amount
	#6angle#0 - the angle in radians that the attack struck at
	#6density#0 - the density of the armour material in kg/m3
	#6strength#0 - the yield strength (shear or impact depending on damage type) of the armour material in Pascals
	#6electrical#0 - the electrical conductivity of the armour material in 1/ohms
	#6thermal#0 - the thermal conductivity of the armour material in W/m/DegK
	#6organic#0 - 1 if armour material is organic, 0 if not

Additionally, absorb formulas can use the following parameter:

	#6originaldamage#0 - the original damage, before dissipation step";

	public bool BuildingCommand(ICharacter actor, StringStack ss)
	{
		switch (ss.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, ss);
			case "difficulty":
				return BuildingCommandDifficulty(actor, ss);
			case "stacked":
			case "stackeddifficulty":
				return BuildingCommandStackedDifficulty(actor, ss);
			case "penetration":
				return BuildingCommandMinimumPenetrationDegree(actor, ss);
			case "absorb":
				return BuildingCommandFormula(actor, ss, false);
			case "dissipate":
				return BuildingCommandFormula(actor, ss, true);
			case "transform":
				return BuildingCommandTransform(actor, ss);
			default:
				actor.OutputHandler.Send(BuildingHelpText.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandTransform(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which damage type do you want to edit the transformation for?");
			return false;
		}

		if (!ss.PopSpeech().TryParseEnum<DamageType>(out var fromDamage))
		{
			actor.OutputHandler.Send($"The text {ss.Last.ColourCommand()} is not a valid damage type.");
			return false;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("You must either specify a second damage type to transform the first into, or #3none#0 to remove an existing transform.".SubstituteANSIColour());
			return false;
		}

		if (ss.PeekSpeech().EqualTo("none"))
		{
			DamageTypeTransformations.Remove(fromDamage);
			Changed = true;
			actor.OutputHandler.Send($"This armour type will no longer perform any transformation on {fromDamage.DescribeEnum().ColourValue()} damage.");
			return true;
		}

		if (!ss.PopSpeech().TryParseEnum<DamageType>(out var toDamage))
		{
			actor.OutputHandler.Send($"The text {ss.Last.ColourCommand()} is not a valid damage type.");
			return false;
		}

		if (fromDamage == toDamage)
		{
			actor.OutputHandler.Send("You can't transform a damage type into itself.");
			return false;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send($"What should be the maximum severity at which this transformation occurs?\nValid values are {Enum.GetValues<WoundSeverity>().ListToColouredString()}.");
			return false;
		}

		if (!ss.SafeRemainingArgument.TryParseEnum<WoundSeverity>(out var severity))
		{
			actor.OutputHandler.Send($"The text {ss.SafeRemainingArgument.ColourCommand()} is not a valid wound severity.\nValid values are {Enum.GetValues<WoundSeverity>().ListToColouredString()}.");
			return false;
		}

		DamageTypeTransformations[fromDamage] = (severity, toDamage);
		Changed = true;
		actor.OutputHandler.Send($"This armour type will now transform {fromDamage.DescribeEnum().ColourValue()} damage equal or less than {severity.DescribeEnum().ColourValue()} severity into {toDamage.DescribeEnum().ColourValue()} damage.");
		return true;
	}

	internal enum ArmourFormulaType
	{
		Damage,
		Pain,
		Stun
	}

	private bool BuildingCommandFormula(ICharacter actor, StringStack ss, bool dissipate)
	{
		ArmourFormulaType type;
		switch (ss.PopForSwitch())
		{
			case "damage":
			case "dam":
				type = ArmourFormulaType.Damage;
				break;
			case "pain":
				type = ArmourFormulaType.Pain;
				break;
			case "stun":
				type = ArmourFormulaType.Stun;
				break;
			default:
				actor.OutputHandler.Send($"You need to specify #3damage#0, #3pain#0 or #3stun#0.".SubstituteANSIColour());
				return false;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send($"Which damage type do you want to edit the formula for? The valid types are {Enum.GetValues<DamageType>().ListToColouredString(Telnet.Green)}.");
			return false;
		}

		if (!ss.PopSpeech().TryParseEnum(out DamageType dt))
		{
			actor.OutputHandler.Send($"The text {ss.Last.ColourCommand()} is not a valid damage type. The valid types are {Enum.GetValues<DamageType>().ListToColouredString(Telnet.Green)}.");
			return false;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send($@"What should the formula be set to? 

You can use the following parameters in this formula:

	#6damage#0 - the raw damage amount{(dissipate ? "" : "\n\t#6originaldamage#0 - the original damage, before dissipation step")}
	#6angle#0 - the angle in radians that the attack struck at
	#6density#0 - the density of the armour material in kg/m3
	#6strength#0 - the yield strength (shear or impact depending on damage type) of the armour material in Pascals
	#6electrical#0 - the electrical conductivity of the armour material in 1/ohms
	#6thermal#0 - the thermal conductivity of the armour material in W/m/DegK
	#6organic#0 - 1 if armour material is organic, 0 if not".SubstituteANSIColour());
			return false;
		}

		var expression = new Expression(ss.SafeRemainingArgument);
		if (expression.HasErrors())
		{
			actor.OutputHandler.Send(expression.Error);
			return false;
		}

		var sb = new StringBuilder();
		sb.Append("You update this armour's ");
		if (dissipate)
		{
			switch (type)
			{
				case ArmourFormulaType.Damage:
					DissipateExpressions[dt] = expression;
					sb.Append("dissipate damage");
					break;
				case ArmourFormulaType.Pain:
					DissipateExpressionsPain[dt] = expression;
					sb.Append("dissipate pain");
					break;
				case ArmourFormulaType.Stun:
					DissipateExpressionsStun[dt] = expression;
					sb.Append("dissipate stun");
					break;
			}
		}
		else
		{
			switch (type)
			{
				case ArmourFormulaType.Damage:
					AbsorbExpressions[dt] = expression;
					sb.Append("absorb damage");
					break;
				case ArmourFormulaType.Pain:
					AbsorbExpressionsPain[dt] = expression;
					sb.Append("absorb pain");
					break;
				case ArmourFormulaType.Stun:
					AbsorbExpressionsStun[dt] = expression;
					sb.Append("absorb stun");
					break;
			}
		}

		sb.Append(" formula to ");
		sb.Append(Telnet.Yellow);
		sb.Append(expression.OriginalExpression);
		sb.Append(Telnet.RESET);
		sb.AppendLine(".");

		foreach (var parameter in expression.Parameters)
		{
			switch (parameter.Key.ToLowerInvariant())
			{
				case "damage":
				case "density":
				case "strength":
				case "electrical":
				case "thermal":
				case "organic":
					continue;
				case "stun":
					if (type != ArmourFormulaType.Stun)
					{
						sb.AppendLine("Warning: The parameter \"stun\" is not valid and will always be zero.".ColourError());
					}

					continue;
				case "pain":
					if (type != ArmourFormulaType.Pain)
					{
						sb.AppendLine("Warning: The parameter \"pain\" is not valid and will always be zero.".ColourError());
					}

					continue;
				case "originaldamage":
					if (dissipate)
					{
						sb.AppendLine("Warning: The parameter \"originaldamage\" is not valid and will always be zero.".ColourError());
					}

					continue;
				case "originalstun":
					if (type != ArmourFormulaType.Stun || dissipate)
					{
						sb.AppendLine("Warning: The parameter \"originalstun\" is not valid and will always be zero.".ColourError());
					}

					continue;
				case "originalpain":
					if (type != ArmourFormulaType.Stun || dissipate)
					{
						sb.AppendLine("Warning: The parameter \"originalpain\" is not valid and will always be zero.".ColourError());
					}

					continue;
				default:
					sb.AppendLine($"Warning: The parameter \"{parameter.Key.ToLowerInvariant()}\" is not valid and will always be zero.".ColourError());
					continue;
			}
		}

		actor.OutputHandler.Send(sb.ToString());
		Changed = true;
		return true;
	}

	private bool BuildingCommandMinimumPenetrationDegree(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send($"What should be the minimum opposed outcome needed to penetrate and bypass this armour type? The valid values are {Enum.GetNames<OpposedOutcomeDegree>().ListToColouredString()}.");
			return false;
		}

		if (!ss.SafeRemainingArgument.TryParseEnum(out OpposedOutcomeDegree degree))
		{
			actor.OutputHandler.Send($"The text {ss.SafeRemainingArgument.ColourCommand()} is not a valid opposed outcome. The valid values are {Enum.GetNames<OpposedOutcomeDegree>().ListToColouredString()}.");
			return false;
		}

		MinimumPenetrationDegree = degree;
		Changed = true;
		actor.OutputHandler.Send($"Attackers will now need to get an opposed outcome degree of {MinimumPenetrationDegree.DescribeColour()} to penetrate and bypass this armour.");
		return true;
	}

	private bool BuildingCommandStackedDifficulty(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What should the penalty be for active checks be when this armour is stacked?");
			return false;
		}

		if (!double.TryParse(ss.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send($"The text {ss.SafeRemainingArgument.ColourCommand()} is not a valid number.");
			return false;
		}

		StackedDifficultyBonus = (-1 * value);
		Changed = true;
		actor.OutputHandler.Send($"This armour type will now impose a penalty of {(-1 * value).ToBonusString(actor)} when stacked.");
		return true;
	}

	private bool BuildingCommandDifficulty(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What should the penalty be for active checks be when this armour is worn alone?");
			return false;
		}

		if (!double.TryParse(ss.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send($"The text {ss.SafeRemainingArgument.ColourCommand()} is not a valid number.");
			return false;
		}

		BaseDifficultyBonus = (-1 * value);
		Changed = true;
		actor.OutputHandler.Send($"This armour type will now impose a penalty of {(-1 * value).ToBonusString(actor)} when worn alone.");
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to this armour type?");
			return false;
		}

		var name = ss.SafeRemainingArgument.TitleCase();
		if (Gameworld.ArmourTypes.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already an armour type called {name.ColourName()}. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the armour type {Name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	#region Overrides of Item

	public override string FrameworkItemType { get; } = "ArmourType";

	#endregion
}