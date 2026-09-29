#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Body;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

/// <summary>Source-owned predator additions. Stable named attacks and race links are reconciled; custom clones are untouched.</summary>
internal static class PredatorCombatSeederHelper
{
	private const int PositionClimbingId = 15;
	internal static readonly string[] Ambushers = ["Leopard", "Panther", "Jaguar", "Tiger", "Crocodile", "Alligator", "Caiman", "Bunyip", "Yacumama"];
	internal static readonly string[] Droppers = ["Eagle", "Griffin", "Garuda", "Giant Eagle", "Wyvern", "Fell Beast"];
	internal static readonly string[] Extractors = ["Leopard", "Panther", "Crocodile", "Alligator", "Caiman", "Bunyip", "Yacumama"];

	public static void Ensure(FuturemudDatabaseContext context)
	{
		OrganicCorpseMaterialSeederHelper.EnsureStockModels(context);
		AnimalSeeder.EnsureStockAirBreathing(context);
		EnsureSerpentAttacks(context);
		var names = Ambushers.Concat(Droppers).Concat(["Giant Spider", "Giant Scorpion", "Giant Centipede"]).Distinct().ToArray();
		foreach (var race in context.Races.Include(x => x.DefaultCombatSetting).Where(x => names.Contains(x.Name)).ToList())
		{
			var links = context.RacesWeaponAttacks.Include(x => x.WeaponAttack).Include(x => x.Bodypart)
				.Where(x => x.RaceId == race.Id && !x.WeaponAttack.Name.StartsWith("Wildlife - ")).ToList();
			var source = links.Where(x => x.WeaponAttack.MoveType == (int)BuiltInCombatMoveType.NaturalWeaponAttack)
				.OrderByDescending(x => x.Bodypart.Name.Contains("claw") || x.Bodypart.Name.Contains("talon"))
				.ThenByDescending(x => x.WeaponAttack.Weighting).FirstOrDefault();
			if (source is null) continue;
			var anatomy = links.Where(x => x.WeaponAttackId == source.WeaponAttackId).ToList();
			if (Ambushers.Contains(race.Name))
			{
				var attack = Upsert(context, source.WeaponAttack, $"Wildlife - Ambush {source.WeaponAttack.Name}", BuiltInCombatMoveType.AmbushAttack,
					new XElement("Data", new XElement("Sources", new[] { RoomLayer.GroundLevel, RoomLayer.InTrees, RoomLayer.HighInTrees, RoomLayer.Underwater }
						.Select(x => new XElement("Layer", x))), new XElement("Destinations", new XElement("Layer", RoomLayer.GroundLevel), new XElement("Layer", RoomLayer.Underwater)),
						new XElement("Seize", true), new XElement("Resist", Difficulty.Normal)).ToString(), 6, 1.5);
				Link(context, race, anatomy, attack);
			}
			if (Droppers.Contains(race.Name) || Extractors.Contains(race.Name))
			{
				var seize = Upsert(context, source.WeaponAttack, $"Wildlife - Seize {source.WeaponAttack.Name}", BuiltInCombatMoveType.InitiateGrapple, "", 4, 1);
				Link(context, race, anatomy, seize);
				foreach (var limb in new[] { LimbType.Arm, LimbType.Leg, LimbType.Head, LimbType.Wing, LimbType.Appendage, LimbType.Torso })
				{
					var hold = Upsert(context, source.WeaponAttack, $"Wildlife - Hold {limb} {source.WeaponAttack.Name}", BuiltInCombatMoveType.ExtendGrapple,
						((int)limb).ToString(), 4, 1);
					Link(context, race, anatomy, hold);
				}
				var haul = Upsert(context, source.WeaponAttack, $"Wildlife - Carry {source.WeaponAttack.Name}", BuiltInCombatMoveType.ForcedMovementUnarmed,
					new XElement("Data", new XElement("Resist", (int)Difficulty.Normal), new XElement("Types", ForcedMovementTypes.All),
						new XElement("Verbs", ForcedMovementVerbs.Pull), new XElement("Range", ForcedMovementRange.Grapple)).ToString(), 5, 1.2);
				Link(context, race, anatomy, haul);
			}
			if (Droppers.Contains(race.Name) && (race.DefaultCombatSetting is null || race.DefaultCombatSetting.Name == "Beast Dropper"))
				race.DefaultCombatSetting = CombatStrategySeederHelper.EnsureCombatStrategy(context, "Beast Dropper");
			if (Extractors.Contains(race.Name) && race.Name is not "Leopard" and not "Panther" &&
			    (race.DefaultCombatSetting is null || race.DefaultCombatSetting.Name == "Beast Drowner" ||
			     race.Name == "Yacumama" && race.DefaultCombatSetting.Name == "Beast Clincher"))
				race.DefaultCombatSetting = CombatStrategySeederHelper.EnsureCombatStrategy(context, "Beast Drowner");
			if (race.Name is "Leopard" or "Panther")
			{
				race.CanClimb = true;
				if (race.DefaultCombatSetting is null || race.DefaultCombatSetting.Name == "Beast Skirmisher")
					race.DefaultCombatSetting = CombatStrategySeederHelper.EnsureCombatStrategy(context, "Beast Brawler");
				foreach (var group in links.Where(x => x.WeaponAttack.MoveType is
				             (int)BuiltInCombatMoveType.NaturalWeaponAttack or (int)BuiltInCombatMoveType.ClinchUnarmedAttack)
				             .GroupBy(x => x.WeaponAttackId))
				{
					var original = group.First().WeaponAttack;
					var climbing = Upsert(context, original, $"Wildlife - Climbing {original.Name}",
						(BuiltInCombatMoveType)original.MoveType, original.AdditionalInfo, original.StaminaCost, original.BaseDelay);
					// Only supplement the missing posture; retain the original ground attack weighting.
					climbing.RequiredPositionStateIds = PositionClimbingId.ToString();
					Link(context, race, group, climbing);
				}
			}
			if (race.Name is "Giant Spider" or "Giant Scorpion" or "Giant Centipede") EnsureGiantVenom(context, race, links);
			if (race.Name == "Giant Spider" && (race.DefaultCombatSetting is null || race.DefaultCombatSetting.Name == "Beast Skirmisher"))
				race.DefaultCombatSetting = CombatStrategySeederHelper.EnsureCombatStrategy(context, "Beast Clincher");
		}
		EnsureAmbushMessage(context);
		EnsureVenomMessages(context);
		const string resistClinch = " And $1 $1|manage|manages to avoid the clinch";
		if (!context.CombatMessages.Any(x => x.Type == (int)BuiltInCombatMoveType.ResistClinch && x.Message == resistClinch))
			context.CombatMessages.Add(new CombatMessage { Type = (int)BuiltInCombatMoveType.ResistClinch,
				Message = resistClinch, FailureMessage = " And $1 $1|are|is unable to stop the clinch",
				Chance = 1, Priority = -100 });
		context.SaveChanges();
	}

	private static void EnsureSerpentAttacks(FuturemudDatabaseContext context)
	{
		var names = AnimalSeeder.SerpentRaceNames.ToArray();
		foreach (var race in context.Races.Where(x => names.Contains(x.Name)).ToList())
		{
			var links = context.RacesWeaponAttacks.Include(x => x.WeaponAttack).Include(x => x.Bodypart)
				.Where(x => x.RaceId == race.Id && !x.WeaponAttack.Name.StartsWith("Wildlife - ")).ToList();
			foreach (var group in links.Where(x => x.WeaponAttack.MoveType is
			             (int)BuiltInCombatMoveType.NaturalWeaponAttack or (int)BuiltInCombatMoveType.ClinchUnarmedAttack or
			             (int)BuiltInCombatMoveType.EnvenomingAttack or (int)BuiltInCombatMoveType.EnvenomingAttackClinch)
			             .GroupBy(x => x.WeaponAttackId))
			{
				var source = group.First().WeaponAttack;
				var types = source.MoveType is (int)BuiltInCombatMoveType.EnvenomingAttack or (int)BuiltInCombatMoveType.EnvenomingAttackClinch
					? new[] { BuiltInCombatMoveType.EnvenomingAttack, BuiltInCombatMoveType.EnvenomingAttackClinch }
					: new[] { BuiltInCombatMoveType.NaturalWeaponAttack, BuiltInCombatMoveType.ClinchUnarmedAttack };
				foreach (var type in types)
				{
					var name = $"Wildlife - Serpent {(type is BuiltInCombatMoveType.EnvenomingAttackClinch or BuiltInCombatMoveType.ClinchUnarmedAttack ? "Clinch " : "")}{source.Name}";
					var attack = Upsert(context, source, name, type, source.AdditionalInfo, source.StaminaCost, source.BaseDelay);
					attack.RequiredPositionStateIds = string.Join(" ", (source.RequiredPositionStateIds ?? "")
						.Split(' ', StringSplitOptions.RemoveEmptyEntries).Append("6").Distinct());
					Link(context, race, group, attack);
				}
			}
		}
	}

	private static WeaponAttack Upsert(FuturemudDatabaseContext context, WeaponAttack source, string name,
		BuiltInCombatMoveType type, string data, double stamina, double delay)
	{
		var existing = context.WeaponAttacks.Local.FirstOrDefault(x => x.Name == name) ?? context.WeaponAttacks.FirstOrDefault(x => x.Name == name);
		var definition = (WeaponAttack)context.Entry(source).CurrentValues.ToObject();
		definition.Id = existing?.Id ?? 0;
		definition.Name = name;
		definition.MoveType = (int)type;
		if (type is BuiltInCombatMoveType.AmbushAttack or BuiltInCombatMoveType.InitiateGrapple or
		    BuiltInCombatMoveType.ExtendGrapple or BuiltInCombatMoveType.ForcedMovementUnarmed)
		{
			// A hunter waiting or holding prey in trees is climbing, even while stationary.
			definition.RequiredPositionStateIds = string.Join(" ", (source.RequiredPositionStateIds ?? "")
				.Split(' ', StringSplitOptions.RemoveEmptyEntries).Append(PositionClimbingId.ToString()).Distinct());
		}
		definition.AdditionalInfo = data;
		definition.WeaponTypeId = null;
		definition.FutureProgId = null;
		definition.OnUseProgId = null;
		definition.MaximumTargets = 1;
		definition.StaminaCost = stamina;
		definition.BaseDelay = delay;
		definition.BaseAttackerDifficulty = (int)Difficulty.Normal;
		definition.BaseDodgeDifficulty = (int)Difficulty.Normal;
		definition.BaseParryDifficulty = (int)Difficulty.Normal;
		definition.BaseBlockDifficulty = (int)Difficulty.Normal;
		definition.Intentions = (long)(CombatMoveIntentions.Attack | CombatMoveIntentions.Wound);
		if (existing is null) { existing = definition; context.WeaponAttacks.Add(existing); }
		else context.Entry(existing).CurrentValues.SetValues(definition);
		context.SaveChanges();
		return existing;
	}

	private static void Link(FuturemudDatabaseContext context, Race race, IEnumerable<RacesWeaponAttacks> anatomy, WeaponAttack attack)
	{
		foreach (var source in anatomy)
		{
			var link = context.RacesWeaponAttacks.Local.FirstOrDefault(x => x.RaceId == race.Id && x.WeaponAttackId == attack.Id && x.BodypartId == source.BodypartId) ??
			           context.RacesWeaponAttacks.FirstOrDefault(x => x.RaceId == race.Id && x.WeaponAttackId == attack.Id && x.BodypartId == source.BodypartId);
			if (link is null) context.RacesWeaponAttacks.Add(new RacesWeaponAttacks
			{ Race = race, RaceId = race.Id, WeaponAttack = attack, WeaponAttackId = attack.Id, Bodypart = source.Bodypart, BodypartId = source.BodypartId, Quality = source.Quality });
			else link.Quality = source.Quality;
		}
	}

	private static void EnsureGiantVenom(FuturemudDatabaseContext context, Race race, List<RacesWeaponAttacks> links)
	{
		// Ordinary centipedes have only a mandible attack. The mythic variant uses the
		// existing mixed arthropod venom as a configurable stock default.
		var donorName = race.Name == "Giant Centipede" ? "Spider" : race.Name.Replace("Giant ", "");
		var donor = context.RacesWeaponAttacks.Include(x => x.WeaponAttack)
			.FirstOrDefault(x => x.Race.Name == donorName && (x.WeaponAttack.MoveType == (int)BuiltInCombatMoveType.EnvenomingAttack || x.WeaponAttack.MoveType == (int)BuiltInCombatMoveType.EnvenomingAttackClinch));
		if (donor is null) return;
		var venomParts = links.Where(x => race.Name == "Giant Scorpion" ? x.Bodypart.Name.Contains("sting") :
			x.Bodypart.Name.Contains("fang") || x.Bodypart.Name.Contains("mandib"))
			.DistinctBy(x => x.BodypartId).ToList();
		if (venomParts.Count == 0) return;
		foreach (var type in new[] { BuiltInCombatMoveType.EnvenomingAttack, BuiltInCombatMoveType.EnvenomingAttackClinch })
		{
			var attack = Upsert(context, donor.WeaponAttack, $"Wildlife - {race.Name} {(type == BuiltInCombatMoveType.EnvenomingAttack ? "Venom" : "Clinch Venom")}",
				type, donor.WeaponAttack.AdditionalInfo, 3, 1.0);
			attack.BodypartShapeId = venomParts[0].Bodypart.BodypartShapeId;
			Link(context, race, venomParts, attack);
		}
	}

	private static void EnsureAmbushMessage(FuturemudDatabaseContext context)
	{
		const string message = "@ spring|springs at $1 and strike|strikes with &0's {0}";
		var row = context.CombatMessages.FirstOrDefault(x => x.Type == (int)BuiltInCombatMoveType.AmbushAttack && x.Message == message);
		if (row is null)
		{
			context.CombatMessages.Add(new CombatMessage { Type = (int)BuiltInCombatMoveType.AmbushAttack, Message = message,
				FailureMessage = message, Chance = 1, Priority = 0 });
		}
	}

	private static void EnsureVenomMessages(FuturemudDatabaseContext context)
	{
		// Source attacks often have attack-specific messages that are not linked to new clones.
		// A low-priority general message leaves those authored messages in control when applicable.
		const string message = "@ strike|strikes at $1 with &0's {0}";
		foreach (var type in new[] { BuiltInCombatMoveType.EnvenomingAttack, BuiltInCombatMoveType.EnvenomingAttackClinch })
		{
			if (context.CombatMessages.Any(x => x.Type == (int)type && x.Message == message)) continue;
			context.CombatMessages.Add(new CombatMessage { Type = (int)type, Message = message,
				FailureMessage = message, Chance = 1, Priority = -100 });
		}
	}
}
