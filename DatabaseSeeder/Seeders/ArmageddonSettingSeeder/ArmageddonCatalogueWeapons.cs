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
using WeaponType = MudSharp.Models.WeaponType;
using WeaponAttack = MudSharp.Models.WeaponAttack;
using CombatMessage = MudSharp.Models.CombatMessage;
using CultureInfo = System.Globalization.CultureInfo;

namespace DatabaseSeeder.Seeders;

/// <summary>Owned native weapon types, attacks and prototypes; creation/lifetime bindings are S4.</summary>
public static class ArmageddonCatalogueWeapons
{
	public static ArmageddonInstallResult Install(Func<FuturemudDatabaseContext> factory,
		Action<ArmageddonInstallCheckpoint>? checkpoint = null)
	{
		using var db = factory();
		var settings = ArmageddonInstallation.Read(db) ?? throw new InvalidOperationException("Fix Armageddon settings before catalogue weapons.");
		var errors = ArmageddonInstallation.ValidateFoundations(db, settings).ToList();
		var holdable = db.SeederManagedRecords.AsNoTracking().SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package &&
			x.Module == ArmageddonCatalogueOwnership.Module && x.StableKey == "arm.catalogue.component.holdable");
		if (holdable is null || holdable.EntityType != nameof(GameItemComponentProto) || holdable.Retired || holdable.LogicalId is null ||
			holdable.RevisionNumber is null) errors.Add("Install the S3 catalogue holdable dependency before weapons.");
		if (errors.Count != 0) return new(ArmageddonInstallStatus.Blocked, errors, new Dictionary<string, long>());
		var weapons = new ArmageddonStockCatalogue().Blueprints.Where(x => x.Kind == ArmageddonBlueprintKind.MeleeItem).ToArray();
		if (weapons.Length != 49) throw new InvalidOperationException("The approved catalogue requires 49 melee prototypes.");
		var messages = new List<string>();
		var ids = new Dictionary<string, long>(StringComparer.Ordinal);
		var committing = false;
		var committed = false;
		try
		{
			using var transaction = db.Database.IsRelational() ? db.Database.BeginTransaction(IsolationLevel.Serializable) : null;
			var owned = new ArmageddonCatalogueOwnership(db, settings.NativeBindings[ArmageddonNativeBinding.BuilderAccount], messages);
			var foundations = new Dictionary<string, WeaponType>(StringComparer.Ordinal);
			foreach (var (shape, name) in new[] { ("knife", "Knife"), ("blade", "Longsword"), ("staff", "Club"),
				("lash", "Improvised Bludgeon"), ("spear", "Short Spear") })
			{
				var id = ArmageddonCatalogueLinks.Bind(db, "arm.binding.weapon." + shape, nameof(WeaponType), () =>
				{
					var matches = db.WeaponTypes.AsNoTracking().Where(x => x.Name == name).ToArray();
					if (matches.Length != 1) throw new InvalidOperationException($"Exactly one native CombatSeeder {name} type is required for {shape} combat traits.");
					return matches[0].Id;
				});
				var foundation = db.WeaponTypes.AsNoTracking().Single(x => x.Id == id);
				if (foundation.AttackTraitId is null || foundation.ParryTraitId is null ||
					!db.TraitDefinitions.Any(x => x.Id == foundation.AttackTraitId && x.Type == 0 && (x.OwnerScope == 0 || x.OwnerScope == 1)) ||
					!db.TraitDefinitions.Any(x => x.Id == foundation.ParryTraitId && x.Type == 0 && (x.OwnerScope == 0 || x.OwnerScope == 1)))
					throw new InvalidOperationException($"The fixed {shape} combat foundation no longer has valid native attack/parry skills.");
				foundations.Add(shape, foundation);
			}
			var kilograms = db.UnitsOfMeasure.Single(x => x.Id == settings.NativeBindings[ArmageddonNativeBinding.Kilogram]).BaseMultiplier;
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.PreflightComplete);
			foreach (var blueprint in weapons)
			{
				var config = blueprint.Configuration;
				var shape = config.GetProperty("shape").GetString()!;
				var foundation = foundations[shape];
				var typeKey = blueprint.Key + ".weapon";
				var type = owned.Put(typeKey, new WeaponType
				{
					Name = "Armageddon " + blueprint.Name, Classification = (int)WeaponClassification.Lethal,
					AttackTraitId = foundation.AttackTraitId, ParryTraitId = foundation.ParryTraitId, ParryBonus = foundation.ParryBonus,
					Reach = shape is "staff" or "lash" or "spear" ? 2 : 1, StaminaPerParry = foundation.StaminaPerParry
				}, ["arm.binding.weapon." + shape]);
				TraitExpression Expression(string suffix, string field) => owned.Put(blueprint.Key + ".attack." + suffix,
					new TraitExpression { Name = "Armageddon " + blueprint.Name + " " + suffix,
						Expression = config.GetProperty(field).GetDouble().ToString(CultureInfo.InvariantCulture) }, []);
				var damage = Expression("damage", "native_attack_damage");
				var pain = Expression("pain", "native_attack_pain");
				var stun = Expression("stun", "native_attack_stun");
				var verb = shape == "spear" ? MeleeWeaponVerb.Thrust : MeleeWeaponVerb.Swing;
				var attackKey = blueprint.Key + ".attack";
				var attack = owned.Put(attackKey, new WeaponAttack
				{
					Name = "Armageddon " + blueprint.Name + " strike", WeaponTypeId = type.Id, Verb = (int)verb,
					MoveType = (int)BuiltInCombatMoveType.UseWeaponAttack,
					BaseAttackerDifficulty = (int)Difficulty.Normal, BaseBlockDifficulty = (int)Difficulty.Normal,
					BaseDodgeDifficulty = (int)Difficulty.Normal, BaseParryDifficulty = (int)Difficulty.Normal,
					BaseAngleOfIncidence = Math.PI * 0.5, RecoveryDifficultySuccess = (int)Difficulty.Easy,
					RecoveryDifficultyFailure = (int)Difficulty.Hard,
					Intentions = (long)(CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill),
					ExertionLevel = (int)ExertionLevel.Heavy, DamageType = (int)Enum.Parse<DamageType>(config.GetProperty("native_damage_type").GetString()!),
					DamageExpressionId = damage.Id, PainExpressionId = pain.Id, StunExpressionId = stun.Id,
					Weighting = 100, MaximumTargets = 1, StaminaCost = config.GetProperty("attack_stamina").GetDouble(), BaseDelay = 1,
					Orientation = (int)Orientation.Centre, Alignment = (int)Alignment.Front,
					HandednessOptions = (int)AttackHandednessOptions.Any, RequiredPositionStateIds = "1 16 17 18"
				}, [typeKey, blueprint.Key + ".attack.damage", blueprint.Key + ".attack.pain", blueprint.Key + ".attack.stun"]);
				var message = new CombatMessage { Type = (int)BuiltInCombatMoveType.UseWeaponAttack, Verb = (int)verb,
					Priority = 50, Chance = 1.0, Message = "@ strike|strikes at $1 with $2",
					FailureMessage = "@ strike|strikes at $1 with $2" };
				message.CombatMessagesWeaponAttacks.Add(new() { WeaponAttackId = attack.Id });
				owned.Put(attackKey + ".message", message, [attackKey]);
				var componentKey = blueprint.Key + ".component";
				var component = owned.Put(componentKey, new GameItemComponentProto
				{
					Id = owned.Id<GameItemComponentProto>(componentKey), Name = "Armageddon " + blueprint.Name + " melee", Type = "MeleeWeapon",
					Description = "Native conjured weapon component.",
					Definition = new XElement("Definition", new XElement("WeaponType", type.Id), new XElement("CanWieldProg", 0), new XElement("WhyCannotWieldProg", 0)).ToString(),
					EditableItem = owned.Approval()
				}, [typeKey]);
				var item = new GameItemProto
				{
					Id = owned.Id<GameItemProto>(blueprint.Key), Name = "Armageddon " + blueprint.Name,
					Keywords = "armageddon " + blueprint.Name.ToLowerInvariant() + " " + shape,
					ShortDescription = "a conjured " + shape,
					FullDescription = "This conjured " + shape + " is fashioned for use as a magical weapon.",
					MaterialId = settings.NativeBindings[shape is "staff" or "lash" ? ArmageddonNativeBinding.Wood : ArmageddonNativeBinding.Stone],
					Size = (int)(shape == "knife" ? SizeCategory.Small : SizeCategory.Normal),
					Weight = config.GetProperty("weight_kilograms").GetDouble() * kilograms, BaseItemQuality = (int)ItemQuality.Standard,
					MorphEmote = "$0 changes.", EditableItem = owned.Approval()
				};
				item.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = holdable!.LogicalId!.Value, GameItemComponentRevision = holdable.RevisionNumber!.Value });
				item.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = component.Id, GameItemComponentRevision = component.RevisionNumber });
				item = owned.Put(blueprint.Key, item, ["arm.catalogue.component.holdable", componentKey]);
				ids.Add(blueprint.Key, item.Id);
			}
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.ContentCreated);
			db.SaveChanges();
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.BeforeCommit);
			committing = true;
			transaction?.Commit();
			committed = true;
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.AfterCommit);
			messages.Add("S3 weapon batch committed: 49 native melee prototypes, owned attacks and combat messages. Existing combat skills are referenced; no mundane class skills or live items are created. Staff/lash use the native Club/Improvised combat traits. Creation and lifetime payloads remain S4 work.");
			return new(ArmageddonInstallStatus.Completed, messages.AsReadOnly(), ids);
		}
		catch (Exception error)
		{
			messages.Add((committed ? "Committed; confirmation failed. Rerun preserves owned identities. " : committing ? "Commit outcome unknown; reconnect before rerunning. " : "Weapon batch failed before commit; relational transaction rolled back. ") + error.Message);
			if (!committed) ids.Clear();
			return new(committed ? ArmageddonInstallStatus.CommittedConfirmationFailed : committing ? ArmageddonInstallStatus.CommitOutcomeUnknown : ArmageddonInstallStatus.Failed, messages.AsReadOnly(), ids);
		}
	}
}
