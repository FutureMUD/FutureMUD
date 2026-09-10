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

	private const string DefaultMythicalCombatRebalanceReferenceKey = "*";

	private static readonly IReadOnlyDictionary<string, string[]> MythicalCombatRebalanceReferenceBodyNames =
		new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
		{
			["Organic Humanoid"] =
			[
				"Organic Humanoid", "Quadruped Base", "Ungulate", "Toed Quadruped", "Avian", "Vermiform",
				"Serpentine", "Piscine"
			],
			["Horned Humanoid"] =
			[
				"Organic Humanoid", "Quadruped Base", "Ungulate", "Toed Quadruped", "Avian", "Vermiform",
				"Serpentine", "Piscine"
			],
			["Winged Humanoid"] =
			[
				"Organic Humanoid", "Quadruped Base", "Ungulate", "Toed Quadruped", "Avian", "Vermiform",
				"Serpentine", "Piscine"
			],
			["Naga"] =
			[
				"Organic Humanoid", "Quadruped Base", "Ungulate", "Toed Quadruped", "Avian", "Vermiform",
				"Serpentine", "Piscine"
			],
			["Mermaid"] =
			[
				"Organic Humanoid", "Quadruped Base", "Ungulate", "Toed Quadruped", "Avian", "Vermiform",
				"Serpentine", "Piscine"
			],
			["Centaur"] =
			[
				"Organic Humanoid", "Quadruped Base", "Ungulate", "Toed Quadruped", "Avian", "Vermiform",
				"Serpentine", "Piscine"
			],
			["Toed Quadruped"] = ["Toed Quadruped", "Quadruped Base", "Ungulate", "Avian", "Serpentine"],
			["Ungulate"] = ["Ungulate", "Quadruped Base", "Toed Quadruped", "Avian"],
			["Avian"] = ["Avian", "Toed Quadruped", "Quadruped Base", "Ungulate"],
			["Serpentine"] = ["Serpentine", "Vermiform"],
			["Vermiform"] = ["Vermiform", "Serpentine"],
			["Insectoid"] = ["Insectoid", "Arachnid", "Scorpion", "Beetle", "Centipede"],
			["Arachnid"] = ["Arachnid", "Scorpion", "Insectoid", "Centipede"],
			["Beetle"] = ["Beetle", "Insectoid", "Centipede"],
			["Centipede"] = ["Centipede", "Insectoid", "Beetle"],
			["Scorpion"] = ["Scorpion", "Arachnid", "Insectoid", "Centipede"],
			["Eastern Dragon"] =
				["Quadruped Base", "Toed Quadruped", "Ungulate", "Avian", "Piscine", "Scorpion"],
			["Griffin"] =
				["Quadruped Base", "Toed Quadruped", "Ungulate", "Avian", "Piscine", "Scorpion"],
			["Hippogriff"] =
				["Quadruped Base", "Toed Quadruped", "Ungulate", "Avian", "Piscine", "Scorpion"],
			["Manticore"] =
				["Quadruped Base", "Toed Quadruped", "Ungulate", "Avian", "Piscine", "Scorpion"],
			["Hippocamp"] =
				["Quadruped Base", "Toed Quadruped", "Ungulate", "Avian", "Piscine", "Scorpion"],
			["Wyvern"] = ["Avian", "Quadruped Base", "Toed Quadruped", "Ungulate"],
			[DefaultMythicalCombatRebalanceReferenceKey] =
			[
				"Organic Humanoid", "Quadruped Base", "Toed Quadruped", "Ungulate", "Avian", "Insectoid",
				"Arachnid", "Beetle", "Centipede", "Vermiform", "Serpentine", "Piscine", "Scorpion"
			]
		};

	internal static IReadOnlyDictionary<string, string[]> MythicalCombatRebalanceReferenceBodyNamesForTesting =>
		MythicalCombatRebalanceReferenceBodyNames;

	internal static IReadOnlyCollection<string> MythicalCombatRebalanceBodyKeysForTesting =>
		MythicalCombatRebalanceReferenceBodyNames.Keys
			.Where(x => !x.Equals(DefaultMythicalCombatRebalanceReferenceKey, StringComparison.OrdinalIgnoreCase))
			.ToArray();

	private double ResolveMythicalHealthMultiplier(MythicalRaceTemplate template)
	{
		return template.BodypartHealthMultiplier;
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
		foreach (BodypartProto bodypart in _context.BodypartProtos.Where(x => x.BodyId == body.Id).ToList())
		{
			if (bodypart.IsOrgan == 1)
			{
				bodypart.RelativeHitChance = 0;
				bodypart.ArmourType = null;
				continue;
			}

			if (SeederBodyUtilities.IsBoneBodypart(bodypart))
			{
				bodypart.RelativeHitChance = 0;
				continue;
			}

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

		if (!MythicalCombatRebalanceReferenceBodyNames.TryGetValue(template.BodyKey, out string[]? referenceBodyNames))
		{
			referenceBodyNames = MythicalCombatRebalanceReferenceBodyNames[DefaultMythicalCombatRebalanceReferenceKey];
		}

		foreach (string referenceBodyName in referenceBodyNames)
		{
			BodyProto? referenceBody = ResolveMythicalReferenceBody(referenceBodyName);
			if (referenceBody is not null)
			{
				yield return referenceBody;
			}
		}
	}

	private BodyProto? ResolveMythicalReferenceBody(string bodyName)
	{
		return bodyName switch
		{
			"Organic Humanoid" => _organicHumanoidBody,
			"Quadruped Base" => _quadrupedBody,
			"Ungulate" => _ungulateBody,
			"Toed Quadruped" => _toedQuadrupedBody,
			"Avian" => _avianBody,
			"Insectoid" => _insectoidBody,
			"Arachnid" => _arachnidBody,
			"Beetle" => _beetleBody,
			"Centipede" => _centipedeBody,
			"Vermiform" => _vermiformBody,
			"Serpentine" => _serpentineBody,
			"Piscine" => _piscineBody,
			"Scorpion" => _scorpionBody,
			_ => null
		};
	}
}
