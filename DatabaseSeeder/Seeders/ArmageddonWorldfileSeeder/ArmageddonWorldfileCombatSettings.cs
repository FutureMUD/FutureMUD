#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Combat;
using MudSharp.GameItems;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private readonly Dictionary<(string Role, bool Stationary, bool Humanoid), CharacterCombatSetting> _npcSettings = new();
	private readonly Dictionary<long, bool> _combatHumanoidRaces = new();
	private Dictionary<int, ArmWorldItem>? _combatItems;
	private ArmWorldItem CombatSourceItem(int vnum) => (_combatItems ??= _source.Items.ToDictionary(x => x.Vnum))[vnum];

	private bool IsHumanoidCombatRace(Race race)
	{
		if (_combatHumanoidRaces.TryGetValue(race.Id, out var cached)) return cached;
		var seen = new HashSet<long>();
		Race? current = race;
		while (current is not null && seen.Add(current.Id))
		{
			if (current.Id == _settings.NativeBindings[ArmageddonNativeBinding.HumanRace] || current.Name == "Organic Humanoid")
				return _combatHumanoidRaces[race.Id] = true;
			current = current.ParentRaceId is { } parent ? _db.Races.Single(x => x.Id == parent) : null;
		}
		return _combatHumanoidRaces[race.Id] = false;
	}

	private CharacterCombatSetting NpcCombatSetting(string role, bool stationary, bool humanoid)
	{
		var key = (role, stationary, humanoid);
		if (_npcSettings.TryGetValue(key, out var cached)) return cached;
		var coreName = role switch { "natural" => humanoid ? "Brawler (Auto)" : "Beast Brawler", "ranged" => "Musketeer (Auto)", "throw" => "Melee (Auto)", "shield" => "Shielder (Auto)", _ => "Melee (Auto)" };
		var baseline = _db.CharacterCombatSettings.SingleOrDefault(x => x.Name == coreName && x.GlobalTemplate)
			?? throw new InvalidOperationException($"Core combat must supply the selectable {coreName} template.");
		if (!stationary && role != "throw" && (humanoid || role == "natural")) return _npcSettings[key] = baseline;
		var title = role switch { "natural" => "Natural Fighter", "ranged" => "Ranged Fighter", "throw" => "Throw and Close", "shield" => "Shield Fighter", _ => "Versatile Fighter" };
		var name = (stationary ? "Stationary " : "") + title + " (Auto)";
		var existing = _npcSettings.Values.FirstOrDefault(x => x.Name == name);
		if (existing is not null) return _npcSettings[key] = existing;
		if (_db.CharacterCombatSettings.Any(x => x.Name == name))
			throw new InvalidOperationException($"Combat template {name} already exists; resolve the one-shot import collision.");
		var result = (CharacterCombatSetting)_db.Entry(baseline).CurrentValues.ToObject();
		result.Id = Next<CharacterCombatSetting>(); result.Name = name;
		result.GlobalTemplate = true; result.CharacterOwnerId = null; result.PriorityProgId = null;
		result.AvailabilityProgId = _settings.NativeBindings[ArmageddonNativeBinding.AlwaysTrueProg];
		result.Description = (stationary ? "Hold your room without pursuing targets or seeking cover. " : "") + (role switch
		{
			"natural" => "Fight with natural attacks and automated inventory and posture management.",
			"ranged" => "Load and fire ranged weapons, with ordinary melee and natural-attack fallback.",
			"throw" => "Throw a readied weapon at range, then close for melee when ranged engagement is unavailable.",
			"shield" => "Prefer armed melee and a usable shield, with natural-attack fallback.",
			_ => "Fight with available melee weapons, falling back to natural attacks when unarmed."
		});
		if (role is "ranged" or "throw")
		{
			result.PreferredRangedMode = (int)CombatStrategyMode.FireNoCover;
			result.PreferredMeleeMode = (int)CombatStrategyMode.StandardMelee;
			result.RequiredMinimumAim = role == "throw" ? 0.0 : baseline.RequiredMinimumAim;
			result.PursuitMode = (int)PursuitMode.AlwaysPursue;
		}
		if (stationary)
		{
			result.PursuitMode = (int)PursuitMode.NeverPursue;
			result.AutomaticallyMoveTowardsTarget = false;
			result.MoveToMeleeIfCannotEngageInRangedCombat = false;
			result.MovementManagement = (int)AutomaticMovementSettings.FullyAutomatic;
			result.SkirmishToOtherLocations = false;
			result.PreferredMeleeMode = (int)CombatStrategyMode.StandardMelee;
			result.PreferredRangedMode = role is "ranged" or "throw" ? (int)CombatStrategyMode.FireNoCover : (int)CombatStrategyMode.FullAdvance;
		}
		_db.CharacterCombatSettings.Add(result);
		Record("combat.setting." + name, nameof(CharacterCombatSetting), result.Id, null,
			new { result.Name, result.Description, result.GlobalTemplate, result.AvailabilityProgId, CoreTemplate = baseline.Id });
		return _npcSettings[key] = result;
	}

	private long DefaultNpcCombatSetting(ArmWorldNpc source, Race race)
	{
		var humanoid = IsHumanoidCombatRace(race);
		return NpcCombatSetting(humanoid && race.CanUseWeapons ? "melee" : "natural", (source.Flags[0] & 2) != 0, humanoid).Id;
	}

	private long OutfitNpcCombatSetting(long templateId, ArmWorldNpc source, Race race, IEnumerable<ResetOutfitItem> items)
	{
		var equipment = items.ToArray();
		var wielded = equipment.Where(x => x.Placement == OutfitTemplateItemPlacement.Wielded).ToArray();
		var launcher = wielded.Any(x => CombatSourceItem(x.SourceVnum).ItemType == 28);
		var longThrow = wielded.Any(x => SourceThrowRange(CombatSourceItem(x.SourceVnum)) > 0);
		var hasAmmunition = equipment.Any(x => CombatSourceItem(x.SourceVnum) is { ItemType: 5 } ammo &&
			(ammo.Flags[1] & ((1 << 9) | (1 << 21))) != 0);
		var armed = wielded.Any(x => x.Prototype.GameItemProtosGameItemComponentProtos.Any(y => y.GameItemComponent.Type is "MeleeWeapon" or "ThrownWeapon"));
		var role = launcher && hasAmmunition ? "ranged" : longThrow ? "throw" : armed ? "melee" : "natural";
		var setting = NpcCombatSetting(role, (source.Flags[0] & 2) != 0, IsHumanoidCombatRace(race));
		Record("combat.assignment." + templateId, "NpcCombatSettingAssignment", setting.Id, null,
			new { SourceNpc = source.Vnum, SourceGuild = source.Identity[3], SourceRace = source.Identity[6], Setting = setting.Name,
				Stationary = (source.Flags[0] & 2) != 0, Launcher = launcher, Ammunition = hasAmmunition, LongThrow = longThrow });
		return setting.Id;
	}

	private void AuditImportedCombatSettings()
	{
		foreach (var template in _npcs.Values.Concat(_npcLoadoutPlans.Select(x => x.Template)))
		{
			var definition = System.Xml.Linq.XElement.Parse(template.Definition);
			var settingId = (long?)definition.Element("DefaultCombatSetting") ?? 0;
			var setting = _db.CharacterCombatSettings.Local.SingleOrDefault(x => x.Id == settingId)
				?? _db.CharacterCombatSettings.SingleOrDefault(x => x.Id == settingId);
			if (setting is null || !setting.GlobalTemplate || string.IsNullOrWhiteSpace(setting.Name) || string.IsNullOrWhiteSpace(setting.Description))
				throw new InvalidOperationException($"Imported NPC template {template.UniqueName} has no named, selectable combat setting.");
		}
		Record("combat.settings.coverage", "NpcCombatSettingsAudit", null, null, new
		{
			BaseTemplates = _npcs.Count, OutfitVariants = _npcLoadoutPlans.Count,
			RaceDefaults = _races.Values.Select(x => new { x.Name, x.DefaultCombatSettingId, x.CanUseWeapons, x.CanAttack, x.CanDefend }).ToArray(),
			SourceClasses = _source.Npcs.GroupBy(x => x.Identity[3]).Select(x => new { SourceGuild = x.Key, Definitions = x.Count(),
				PowerPolicy = "Skills and equipment do not invent native spells or powers; no automatic magic profile without usable powers." }).ToArray()
		});
	}
}
