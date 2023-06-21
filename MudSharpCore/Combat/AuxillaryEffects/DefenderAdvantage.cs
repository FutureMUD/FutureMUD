using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.Combat.AuxillaryEffects;
#nullable enable
internal class DefenderAdvantage : IAuxillaryEffect
{
	public required IFuturemud Gameworld { get; set; }
	public required double DefenseBonusPerDegree { get; set; }
	public required double OffenseBonusPerDegree { get; set; }
	public required ITraitDefinition DefenseTrait { get; set; }
	public required Difficulty DefenseDifficulty { get; set; }
	public required bool AllowNegatives { get; set; }
	public required bool AllowPositives { get; set; }

	[SetsRequiredMembers]
	public DefenderAdvantage(XElement root, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		DefenseBonusPerDegree = double.Parse(root.Attribute("defensebonusperdegree")!.Value);
		OffenseBonusPerDegree = double.Parse(root.Attribute("offensebonusperdegree")!.Value);
		AllowNegatives = bool.Parse(root.Attribute("allownegatives")!.Value);
		AllowPositives = bool.Parse(root.Attribute("allowpositives")!.Value);
		DefenseTrait = gameworld.Traits.Get(long.Parse(root.Attribute("defensetrait")!.Value)) ?? throw new ApplicationException($"Missing DefenseTrait for AttackerAdvantage: {root.Attribute("defensetrait")!.Value}");
		DefenseDifficulty = (Difficulty)(int.Parse(root.Attribute("defensedifficulty")!.Value));
	}

	public XElement Save()
	{
		return new XElement("Effect",
			new XAttribute("type", "defenderadvantage"),
			new XAttribute("defensebonusperdegree", DefenseBonusPerDegree),
			new XAttribute("offensebonusperdegree", OffenseBonusPerDegree),
			new XAttribute("defensetrait", DefenseTrait.Id),
			new XAttribute("defensedifficulty", DefenseDifficulty),
			new XAttribute("allownegatives", AllowNegatives),
			new XAttribute("allowpositives", AllowPositives)
		);
	}
	public string DescribeForShow(ICharacter actor)
	{
		return $"Defender Advantage | vs {DefenseTrait.Name.ColourValue()}@{DefenseDifficulty.DescribeColoured()} | Off: [{(AllowNegatives ? OffenseBonusPerDegree.InvertSign() : 0.0).ToBonusString(actor)}/{(AllowPositives ? OffenseBonusPerDegree : 0.0).ToBonusString(actor)}] Def: [{(AllowNegatives ? DefenseBonusPerDegree.InvertSign() : 0.0).ToBonusString(actor)}/{(AllowPositives ? DefenseBonusPerDegree : 0.0).ToBonusString(actor)}]";
	}

	public void ApplyEffect(ICharacter attacker, IPerceiver? target, CheckOutcome outcome)
	{
		if (target is not ICharacter tch)
		{
			return;
		}

		var defenseCheck = Gameworld.GetCheck(CheckType.CombatMoveCheck);
		var defenderOutcome = defenseCheck.Check(tch, DefenseDifficulty, DefenseTrait, attacker);
		var opposed = new OpposedOutcome(outcome, defenderOutcome);
		switch (opposed.Outcome)
		{
			case OpposedOutcomeDirection.Proponent:
				if (!AllowPositives)
				{
					return;
				}
				tch.OffensiveAdvantage += OffenseBonusPerDegree * (int)opposed.Degree;
				tch.DefensiveAdvantage += DefenseBonusPerDegree * (int)opposed.Degree;
				break;
			case OpposedOutcomeDirection.Opponent:
				if (!AllowNegatives)
				{
					return;
				}
				tch.OffensiveAdvantage += OffenseBonusPerDegree * -1.0 * (int)opposed.Degree;
				tch.DefensiveAdvantage += DefenseBonusPerDegree * -1.0 * (int)opposed.Degree;
				break;
			case OpposedOutcomeDirection.Stalemate:
				return;
		}
	}
}
