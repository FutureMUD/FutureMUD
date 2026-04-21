#nullable enable

using MudSharp.Body;
using MudSharp.GameItems;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class MythicalAnimalSeeder
{
	private CombatBalanceProfile _combatBalanceProfile = CombatBalanceProfile.Stock;

	private bool UsesCombatRebalance => _combatBalanceProfile == CombatBalanceProfile.CombatRebalance;

	private double ResolveMythicalHealthMultiplier(MythicalRaceTemplate template)
	{
		if (!UsesCombatRebalance)
		{
			return template.BodypartHealthMultiplier;
		}

		double sizeFactor = template.Size switch
		{
			SizeCategory.Tiny => 0.45,
			SizeCategory.VerySmall => 0.65,
			SizeCategory.Small => 0.85,
			SizeCategory.Normal => 1.0,
			SizeCategory.Large => 1.35,
			SizeCategory.VeryLarge => 1.75,
			SizeCategory.Huge => 2.15,
			SizeCategory.Enormous => 2.65,
			SizeCategory.Gigantic => 3.1,
			SizeCategory.Titanic => 3.6,
			_ => template.BodypartHealthMultiplier
		};
		double constitutionFactor = 1.0 + (template.AttributeProfile.ConstitutionBonus * 0.04);
		return Math.Round(sizeFactor * constitutionFactor, 2);
	}

	private void RefreshExistingMythicalCombatBalance()
	{
		HashSet<long> refreshedBodies = new();
		foreach (MythicalRaceTemplate template in Templates.Values)
		{
			Race? race = _context.Races.FirstOrDefault(x => x.Name == template.Name);
			if (race is null || !UsesCombatRebalance || !refreshedBodies.Add(race.BaseBodyId))
			{
				continue;
			}

			BodyProto? body = _context.BodyProtos.Find(race.BaseBodyId);
			if (body is null)
			{
				continue;
			}

			RefreshMythicalBodyparts(template, body);
		}

		ApplyDefaultCombatSettingsToSeededRaces();
		_context.SaveChanges();
	}

	private void RefreshMythicalBodyparts(MythicalRaceTemplate template, BodyProto body)
	{
		foreach (BodypartProto bodypart in _context.BodypartProtos.Where(x => x.BodyId == body.Id && x.IsOrgan != 1).ToList())
		{
			BodypartProto? reference = FindReferenceBodypart(template, body, bodypart.Name);
			if (reference is null)
			{
				continue;
			}

			bodypart.MaxLife = reference.MaxLife;
			bodypart.RelativeHitChance = reference.RelativeHitChance;
			bodypart.ArmourType = reference.ArmourType;
			bodypart.SeveredThreshold = reference.SeveredThreshold;
			bodypart.SeverFormula = reference.SeverFormula;
		}
	}

	private BodypartProto? FindReferenceBodypart(MythicalRaceTemplate template, BodyProto body, string alias)
	{
		foreach (BodyProto source in GetReferenceBodies(template, body)
			         .Where(x => x is not null)
			         .DistinctBy(x => x.Id))
		{
			BodypartProto? reference = _context.BodypartProtos.FirstOrDefault(x => x.BodyId == source.Id && x.Name == alias);
			if (reference is not null)
			{
				return reference;
			}
		}

		return null;
	}

	private IEnumerable<BodyProto> GetReferenceBodies(MythicalRaceTemplate template, BodyProto body)
	{
		if (body.CountsAsId is long countsAsId && _context.BodyProtos.Find(countsAsId) is { } countsAsBody)
		{
			yield return countsAsBody;
		}

		switch (template.BodyKey)
		{
			case "Organic Humanoid":
			case "Horned Humanoid":
			case "Winged Humanoid":
			case "Naga":
			case "Mermaid":
			case "Centaur":
				yield return _organicHumanoidBody;
				yield return _quadrupedBody;
				yield return _ungulateBody;
				yield return _toedQuadrupedBody;
				yield return _avianBody;
				yield return _vermiformBody;
				yield return _serpentineBody;
				yield return _piscineBody;
				break;
			case "Insectoid":
				yield return _insectoidBody;
				yield return _beetleBody;
				yield return _centipedeBody;
				break;
			case "Beetle":
				yield return _beetleBody;
				yield return _insectoidBody;
				yield return _centipedeBody;
				break;
			case "Centipede":
				yield return _centipedeBody;
				yield return _insectoidBody;
				yield return _beetleBody;
				break;
			case "Eastern Dragon":
			case "Griffin":
			case "Hippogriff":
			case "Manticore":
			case "Hippocamp":
				yield return _quadrupedBody;
				yield return _toedQuadrupedBody;
				yield return _ungulateBody;
				yield return _avianBody;
				yield return _piscineBody;
				yield return _scorpionBody;
				break;
			case "Wyvern":
				yield return _avianBody;
				yield return _quadrupedBody;
				yield return _toedQuadrupedBody;
				yield return _ungulateBody;
				break;
			default:
				yield return _organicHumanoidBody;
				yield return _quadrupedBody;
				yield return _toedQuadrupedBody;
				yield return _ungulateBody;
				yield return _avianBody;
				yield return _insectoidBody;
				yield return _beetleBody;
				yield return _centipedeBody;
				yield return _vermiformBody;
				yield return _serpentineBody;
				yield return _piscineBody;
				yield return _scorpionBody;
				break;
		}
	}
}
