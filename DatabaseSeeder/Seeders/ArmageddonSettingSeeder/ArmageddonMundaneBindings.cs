#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using MudSharp.Database;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

/// <summary>Explicit source consumers, separate from generic engine-only scaffolds and player grants.</summary>
public static class ArmageddonMundaneBindings
{
	public const string Module = "setting-bindings";
	public static string? Formula(CheckType type, IReadOnlyDictionary<string, TraitDefinition> skills)
	{
		string Trait(string name) => name.ToLowerInvariant().Replace(' ', '_') + ":" + skills[name].Id;
		return type switch
		{
			CheckType.SearchForTracksCheck or CheckType.SearchForTracksByScentScheck => Trait("Hunt"),
			CheckType.FleeMeleeCheck or CheckType.OpposeFleeMeleeCheck or CheckType.FleeMovementUnmountedCheck or CheckType.FleeMovementMountedCheck => Trait("Flee"),
			CheckType.PursuitMovementUnmountedCheck => $"running:{skills["Run"].Id} + 0.5 * veterancy:{skills["Veterancy"].Id}",
			CheckType.PursuitMovementMountedCheck => $"0.7 * {Trait("Ride")} + 0.3 * veterancy:{skills["Veterancy"].Id}",
			CheckType.RescueCheck or CheckType.OpposeRescueCheck => Trait("Rescue"),
			CheckType.MountControlCheck or CheckType.AvoidMountFallCheck => Trait("Ride"),
			CheckType.MountedChargeCheck or CheckType.AerialMountedChargeCheck or CheckType.AquaticMountedChargeCheck => $"0.7 * {Trait("Ride")} + 0.3 * {Trait("Charge")}",
			CheckType.VehicleChargeCheck or CheckType.AquaticVehicleChargeCheck => $"0.7 * {Trait("Drive")} + 0.3 * {Trait("Charge")}",
			_ => null
		};
	}

	internal static void EnsureCheck(FuturemudDatabaseContext db, CheckType type, string formula, long templateId,
		Difficulty maximumImprovementDifficulty, ICollection<string> messages, bool rejectUnowned = false)
	{
		var key = "arm.check." + type.ToString().ToLowerInvariant();
		if (rejectUnowned && db.Checks.Any(x => x.Type == (int)type) &&
			!db.SeederManagedRecords.Any(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == key + ".binding"))
			throw new InvalidOperationException($"Unowned {type} check exists. Preserve it and resolve the binding explicitly before installing the optional adapter.");
		var expression = ArmageddonSettingOwnership.Apply(db, Module, key + ".expression",
			new TraitExpression { Name = "Armageddon " + type + " Formula", Expression = formula }, messages);
		var check = db.Checks.SingleOrDefault(x => x.Type == (int)type);
		if (check is null)
		{
			check = new Check { Type = (int)type, TraitExpressionId = expression.Id, CheckTemplateId = templateId,
				MaximumDifficultyForImprovement = (int)maximumImprovementDifficulty };
			db.Checks.Add(check); db.SaveChanges();
		}
		var current = new Dictionary<string, string>
		{
			["ExpressionId"] = JsonSerializer.Serialize(check.TraitExpressionId), ["TemplateId"] = JsonSerializer.Serialize(check.CheckTemplateId),
			["MaximumImprovementDifficulty"] = JsonSerializer.Serialize(check.MaximumDifficultyForImprovement)
		};
		var desired = new Dictionary<string, string>
		{
			["ExpressionId"] = JsonSerializer.Serialize(expression.Id), ["TemplateId"] = JsonSerializer.Serialize(templateId),
			["MaximumImprovementDifficulty"] = JsonSerializer.Serialize((int)maximumImprovementDifficulty)
		};
		var merged = ArmageddonSettingLinks.Reconcile(db, key + ".binding", nameof(Check), (long)type, current, desired, messages);
		check.TraitExpressionId = JsonSerializer.Deserialize<long>(merged["ExpressionId"]);
		check.CheckTemplateId = JsonSerializer.Deserialize<long>(merged["TemplateId"]);
		check.MaximumDifficultyForImprovement = JsonSerializer.Deserialize<int>(merged["MaximumImprovementDifficulty"]);
		db.SaveChanges();
	}

	internal static void BindDisarm(FuturemudDatabaseContext db, IReadOnlyDictionary<string, TraitDefinition> skills)
	{
		foreach (var name in new[] { "Pommel Beat", "Wrist Check", "Beat the Weapon" })
		{
			var action = db.CombatActions.Single(x => x.Name == name);
			action.TraitDefinitionId = skills["arm.skill.disarm"].Id;
			var xml = XElement.Parse(action.AdditionalInfo);
			foreach (var effect in xml.Elements("Effect").Where(x => string.Equals((string?)x.Attribute("type"), "disarm", StringComparison.OrdinalIgnoreCase)))
				effect.SetAttributeValue("defensetrait", skills["arm.skill.parry"].Id);
			action.AdditionalInfo = xml.ToString();
			ArmageddonSettingOwnership.RecordNew(db, Module, "arm.auxiliary." + name.ToLowerInvariant().Replace(' ', '_'), action,
				nameof(CombatAction.TraitDefinitionId), nameof(CombatAction.AdditionalInfo));
		}
	}
}
