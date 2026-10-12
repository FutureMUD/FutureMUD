#nullable enable
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Body;
using MudSharp.Combat;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits;

namespace DatabaseSeeder.Seeders;

/// <summary>Independent races/ethnicities, culture, declared affinities and usable natural attack data.</summary>
public static class ArmageddonCatalogueCreatureFoundations
{
	public static ArmageddonInstallResult Install(Func<FuturemudDatabaseContext> factory,
		Action<ArmageddonInstallCheckpoint>? checkpoint = null)
	{
		using var db = factory();
		var settings = ArmageddonInstallation.Read(db) ?? throw new InvalidOperationException("Fix Armageddon settings before creature foundations.");
		var errors = ArmageddonInstallation.ValidateFoundations(db, settings).ToList();
		var blueprints = new ArmageddonStockCatalogue().Blueprints.Where(x => x.Kind == ArmageddonBlueprintKind.RaceBundle).ToArray();
		var bodies = new Dictionary<string, long>(StringComparer.Ordinal);
		foreach (var blueprint in blueprints)
		{
			var record = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == blueprint.Key + ".body");
			if (record is null || record.Module != ArmageddonCatalogueOwnership.Module || record.EntityType != nameof(BodyProto) ||
				record.Retired || record.RevisionNumber is not null || record.LogicalId is null || !db.BodyProtos.Any(x => x.Id == record.LogicalId))
				errors.Add($"Install the owned {blueprint.Key} anatomy before creature foundations.");
			else bodies.Add(blueprint.Key, record.LogicalId.Value);
		}
		if (errors.Count != 0) return new(ArmageddonInstallStatus.Blocked, errors, new Dictionary<string, long>());
		var messages = new List<string>(); var ids = new Dictionary<string, long>(StringComparer.Ordinal);
		var committing = false; var committed = false;
		try
		{
			using var transaction = db.Database.IsRelational() ? db.Database.BeginTransaction(IsolationLevel.Serializable) : null;
			var owned = new ArmageddonCatalogueOwnership(db, settings.NativeBindings[ArmageddonNativeBinding.BuilderAccount], messages);
			var source = Source.Read(db, settings.NativeBindings[ArmageddonNativeBinding.HumanRace]);
			long? population = null;
			if (source.Race.BloodModelId is { } bloodModel)
			{
				bool Compatible(long id) => db.PopulationBloodModelsBloodtypes.Any(x => x.PopulationBloodModelId == id && x.Weight > 0) &&
					!db.PopulationBloodModelsBloodtypes.Any(x => x.PopulationBloodModelId == id &&
						(x.Weight < 0 || (x.Weight > 0 && !db.BloodModelsBloodtypes.Any(b => b.BloodModelId == bloodModel && b.BloodtypeId == x.BloodtypeId))));
				population = ArmageddonCatalogueLinks.Bind(db, "arm.binding.creatures.blood_population", nameof(PopulationBloodModel), () =>
					db.Ethnicities.AsNoTracking().Where(x => x.ParentRaceId == source.Race.Id && x.PopulationBloodModelId != null)
						.OrderBy(x => x.Id).Select(x => x.PopulationBloodModelId!.Value).ToArray().First(Compatible));
				if (!Compatible(population.Value)) throw new InvalidOperationException("The retained creature blood population is incompatible with the fixed native race blood model.");
			}
			var sourceCulture = db.Cultures.AsNoTracking().Include(x => x.CulturesNameCultures)
				.Single(x => x.Id == settings.NativeBindings[ArmageddonNativeBinding.Culture]);
			var nameCultures = sourceCulture.CulturesNameCultures.OrderBy(x => x.Gender).ThenBy(x => x.NameCultureId).ToArray();
			if (!Enumerable.Range(0, 5).All(g => nameCultures.Any(x => x.Gender == g)))
				throw new InvalidOperationException("The fixed source culture needs a native name culture for each gender.");
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.PreflightComplete);
			var culture = new Culture
			{
				Id = owned.Id<Culture>("arm.culture.constructs"), Name = "Conjured Creatures",
				Description = "Artificial magical creatures bound to the installed world's language and calendar.",
				PersonWordMale = "creature", PersonWordFemale = "creature", PersonWordNeuter = "creature", PersonWordIndeterminate = "creature",
				NativeLanguageId = settings.NativeBindings[ArmageddonNativeBinding.Language],
				PrimaryCalendarId = sourceCulture.PrimaryCalendarId, SkillStartingValueProgId = sourceCulture.SkillStartingValueProgId,
				AvailabilityProgId = settings.NativeBindings[ArmageddonNativeBinding.AlwaysFalseProg]
			};
			foreach (var link in nameCultures) culture.CulturesNameCultures.Add(new() { CultureId = culture.Id, NameCultureId = link.NameCultureId, Gender = link.Gender });
			owned.Stage("arm.culture.constructs", culture, []); ids.Add("arm.culture.constructs", culture.Id);
			var attributeId = source.Attributes.Select(x => x.AttributeId).Order().First(id =>
				db.TraitDefinitions.Any(x => x.Id == id && x.Type == 1 && x.OwnerScope == 0 && x.DerivedType == 0));
			foreach (var blueprint in blueprints)
			{
				var config = blueprint.Configuration;
				var label = blueprint.Name.Replace(" stock anatomy", "", StringComparison.Ordinal);
				var race = owned.Copy(source.Race);
				race.Id = owned.Id<Race>(blueprint.Key); race.Name = "Armageddon " + label; race.Description = "A conjured " + label.ToLowerInvariant() + ".";
				race.BaseBodyId = bodies[blueprint.Key]; race.ParentRaceId = null; race.AllowedGenders = ((short)MudSharp.Form.Shape.Gender.Neuter).ToString();
				race.AvailabilityProgId = settings.NativeBindings[ArmageddonNativeBinding.AlwaysFalseProg];
				race.IndividualAttributeCap = Math.Max(24, race.IndividualAttributeCap);
				race.AttributeTotalCap = Math.Max(checked(24 * source.Attributes.Length), race.AttributeTotalCap);
				race.SizeStanding = race.SizeProne = race.SizeSitting = (int)Enum.Parse<SizeCategory>(config.GetProperty("size").GetString()!);
				// These temporary creatures are dissipation-bound at creation, not an animal butchery catalogue.
				race.RaceButcheryProfileId = null;
				foreach (var link in source.Attributes) race.RacesAttributes.Add(new() { RaceId = race.Id, AttributeId = link.AttributeId,
					IsHealthAttribute = link.IsHealthAttribute, AttributeBonus = link.AttributeBonus, DiceExpression = link.DiceExpression });
				foreach (var link in source.Gases) race.RacesBreathableGases.Add(new() { RaceId = race.Id, GasId = link.Key, Multiplier = link.Value });
				foreach (var link in source.Liquids) race.RacesBreathableLiquids.Add(new() { RaceId = race.Id, LiquidId = link.Key, Multiplier = link.Value });
				var strike = Attack(db, owned, blueprint, label, race.BaseBodyId, attributeId);
				foreach (var part in db.BodypartProtos.AsNoTracking().Where(x => x.BodyId == race.BaseBodyId &&
					(x.BodypartType == (int)BodypartTypeEnum.GrabbingWielding || x.BodypartType == (int)BodypartTypeEnum.BonyGrabbingWielding)))
					race.RacesWeaponAttacks.Add(new() { RaceId = race.Id, WeaponAttackId = strike.Id, BodypartId = part.Id, Quality = (int)ItemQuality.Standard });
				if (race.RacesWeaponAttacks.Count == 0) throw new InvalidOperationException($"{blueprint.Key}: no native grasping strike part.");
				owned.Stage(blueprint.Key, race, [blueprint.Key + ".body", "arm.culture.constructs", blueprint.Key + ".strike"]);
				ids.Add(blueprint.Key, race.Id);
				var ethnicityKey = config.GetProperty("ethnicity_key").GetString()!;
				var ethnicity = new Ethnicity { Id = owned.Id<Ethnicity>(ethnicityKey), Name = "Armageddon " + label,
					ParentRaceId = race.Id, NativeLanguageId = culture.NativeLanguageId, EthnicGroup = "Conjured Creatures", EthnicSubgroup = label,
					PopulationBloodModelId = population,
					ChargenBlurb = "A magically conjured creature; unavailable in player character creation.",
					AvailabilityProgId = settings.NativeBindings[ArmageddonNativeBinding.AlwaysFalseProg] };
				owned.Stage(ethnicityKey, ethnicity, [blueprint.Key]); ids.Add(ethnicityKey, ethnicity.Id);
				var affinityKey = blueprint.Key + ".affinity";
				var merit = new Merit { Id = owned.Id<Merit>(affinityKey), Name = "Armageddon " + label + " affinity",
					Type = "Elemental Affinity", MeritScope = (int)MeritScope.Character, MeritType = (int)MeritType.Merit,
					Definition = new XElement("Merit", new XElement("ChargenAvailableProg", settings.NativeBindings[ArmageddonNativeBinding.AlwaysFalseProg]),
						new XElement("ApplicabilityProg", settings.NativeBindings[ArmageddonNativeBinding.AlwaysTrueProg]),
						new XElement("ChargenBlurb", "Declared affinities of a conjured creature."),
						new XElement("DescriptionText", "@ have|has a declared elemental affinity"),
						new XElement("ElementalAffinities", config.GetProperty("affinity").EnumerateArray().Select(x => new XElement("Affinity", x.GetString())))).ToString() };
				owned.Stage(affinityKey, merit, []); ids.Add(affinityKey, merit.Id);
			}
			owned.Flush();
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.ContentCreated); checkpoint?.Invoke(ArmageddonInstallCheckpoint.BeforeCommit);
			committing = true; transaction?.Commit(); committed = true; checkpoint?.Invoke(ArmageddonInstallCheckpoint.AfterCommit);
			messages.Add("S3 creature foundations committed: seven native race/ethnicity bundles, one culture and explicit affinity merits. NPC templates and controllers remain to qualify.");
			return new(ArmageddonInstallStatus.Completed, messages.AsReadOnly(), ids);
		}
		catch (Exception error)
		{
			messages.Add((committed ? "Committed; confirmation failed. " : committing ? "Commit outcome unknown; reconnect before rerunning. " : "Creature foundations failed before commit; relational transaction rolled back. ") + error.Message);
			if (!committed) ids.Clear();
			return new(committed ? ArmageddonInstallStatus.CommittedConfirmationFailed : committing ? ArmageddonInstallStatus.CommitOutcomeUnknown : ArmageddonInstallStatus.Failed, messages.AsReadOnly(), ids);
		}
	}

	private static WeaponAttack Attack(FuturemudDatabaseContext db, ArmageddonCatalogueOwnership owned,
		ArmageddonStockBlueprint blueprint, string label, long bodyId, long attributeId)
	{
		var key = blueprint.Key + ".strike";
		TraitExpression Expression(string suffix, string formula) => owned.Put(key + "." + suffix, new TraitExpression
		{ Name = "Armageddon " + label + " strike " + suffix, Expression = formula }, []);
		// Every stock attribute is 10 + 2*grade. A declared native attribute reference gives
		// exactly 3*grade damage and grade pain without seven mutually overlapping race attacks.
		var damage = Expression("damage", $"max(0, 1.5 * (power:{attributeId}{{noimprove,nobranch}} - 10))");
		var pain = Expression("pain", $"max(0, 0.5 * (power:{attributeId}{{noimprove,nobranch}} - 10))");
		var stun = Expression("stun", "0");
		var part = db.BodypartProtos.AsNoTracking().Where(x => x.BodyId == bodyId &&
			(x.BodypartType == (int)BodypartTypeEnum.GrabbingWielding || x.BodypartType == (int)BodypartTypeEnum.BonyGrabbingWielding)).OrderBy(x => x.Id).First();
		var attack = owned.Put(key, new WeaponAttack { Name = "Armageddon " + label + " strike", MoveType = (int)BuiltInCombatMoveType.NaturalWeaponAttack,
			Verb = (int)MeleeWeaponVerb.Strike, DamageType = (int)DamageType.Crushing, BodypartShapeId = part.BodypartShapeId,
			DamageExpressionId = damage.Id, PainExpressionId = pain.Id, StunExpressionId = stun.Id,
			BaseAttackerDifficulty = (int)Difficulty.Normal, BaseBlockDifficulty = (int)Difficulty.Normal,
			BaseDodgeDifficulty = (int)Difficulty.Normal, BaseParryDifficulty = (int)Difficulty.Normal,
			BaseAngleOfIncidence = Math.PI * 0.5, RecoveryDifficultySuccess = (int)Difficulty.Easy, RecoveryDifficultyFailure = (int)Difficulty.Hard,
			Intentions = (long)(CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill),
			ExertionLevel = (int)ExertionLevel.Heavy, Weighting = 100, MaximumTargets = 1, StaminaCost = 5, BaseDelay = 1,
			Orientation = (int)Orientation.Centre, Alignment = (int)Alignment.Front,
			HandednessOptions = (int)AttackHandednessOptions.Any, RequiredPositionStateIds = "1 16 17 18" }, [key + ".damage", key + ".pain", key + ".stun"]);
		var message = new CombatMessage { Type = (int)BuiltInCombatMoveType.NaturalWeaponAttack, Verb = (int)MeleeWeaponVerb.Strike,
			Priority = 50, Chance = 1, Message = "@ strike|strikes at $1", FailureMessage = "@ strike|strikes at $1" };
		message.CombatMessagesWeaponAttacks.Add(new() { WeaponAttackId = attack.Id });
		owned.Put(key + ".message", message, [key]);
		return attack;
	}

	private sealed record Source(Race Race, RacesAttributes[] Attributes, Dictionary<long, double> Gases, Dictionary<long, double> Liquids)
	{
		internal static Source Read(FuturemudDatabaseContext db, long id)
		{
			var chain = new List<Race>(); var seen = new HashSet<long>();
			for (long? current = id; current is not null;)
			{
				if (!seen.Add(current.Value)) throw new InvalidOperationException("The fixed race foundation has a cyclic ParentRace chain.");
				var row = db.Races.AsNoTracking().Single(x => x.Id == current); chain.Add(row); current = row.ParentRaceId;
			}
			chain.Reverse(); var raceIds = chain.Select(x => x.Id).ToArray();
			var attributes = db.RacesAttributes.AsNoTracking().Where(x => raceIds.Contains(x.RaceId)).ToArray();
			var flattened = attributes.GroupBy(x => x.AttributeId).Select(group =>
			{
				double bonus = 0; string? dice = null; var declared = false;
				for (var index = 0; index < chain.Count; ++index)
				{
					var race = chain[index]; var local = group.SingleOrDefault(x => x.RaceId == race.Id);
					if (local is not null)
					{
						bonus = local.AttributeBonus;
						dice = string.IsNullOrWhiteSpace(local.DiceExpression) ? race.DiceExpression : local.DiceExpression;
						declared = true;
					}
					else if (declared && index > 0 && string.Equals(dice, chain[index - 1].DiceExpression, StringComparison.OrdinalIgnoreCase))
						dice = race.DiceExpression;
				}
				return new RacesAttributes { AttributeId = group.Key, IsHealthAttribute = group.Any(x => x.IsHealthAttribute),
					AttributeBonus = bonus, DiceExpression = dice };
			}).ToArray();
			if (flattened.Length == 0) throw new InvalidOperationException("The fixed native race must declare its body attributes.");
			var gases = new Dictionary<long, double>(); var liquids = new Dictionary<long, double>();
			foreach (var race in chain)
			{
				foreach (var link in db.RacesBreathableGases.AsNoTracking().Where(x => x.RaceId == race.Id)) gases[link.GasId] = link.Multiplier;
				foreach (var link in db.RacesBreathableLiquids.AsNoTracking().Where(x => x.RaceId == race.Id)) liquids[link.LiquidId] = link.Multiplier;
				foreach (var link in db.RacesRemoveBreathableGases.AsNoTracking().Where(x => x.RaceId == race.Id)) gases.Remove(link.GasId);
				foreach (var link in db.RacesRemoveBreathableLiquids.AsNoTracking().Where(x => x.RaceId == race.Id)) liquids.Remove(link.LiquidId);
			}
			if (chain.Last().NeedsToBreathe && gases.Count + liquids.Count == 0) throw new InvalidOperationException("The fixed native breathing race has no breathable fluid links.");
			return new(chain.Last(), flattened, gases, liquids);
		}
	}
}
