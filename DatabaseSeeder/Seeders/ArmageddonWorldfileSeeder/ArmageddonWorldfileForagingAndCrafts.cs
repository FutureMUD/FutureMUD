#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using CultureInfo = System.Globalization.CultureInfo;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private void InstallForagingAndCrafts()
	{
		// An explicit empty room profile prevents unrelated stock terrain forage leaking into the imported world.
		var empty = new ForagableProfile { Id = Next<ForagableProfile>(), RevisionNumber = 0,
			Name = "Armageddon worldfile: no authored forage", EditableItem = Approval() };
		_db.ForagableProfiles.Add(empty);
		_emptyForageProfile = empty.Id;
		foreach (var room in _rooms.Values) room.ForagableProfileId = empty.Id;
		foreach (var group in _source.Forage.GroupBy(x => x.Sector, StringComparer.OrdinalIgnoreCase))
		{
			var sector = group.Key.ToLowerInvariant() switch
			{
				"hills" => 4, "mountains" => 5,
				_ => throw new InvalidOperationException($"Unqualified forage sector {group.Key}.")
			};
			var profile = new ForagableProfile { Id = Next<ForagableProfile>(), RevisionNumber = 0,
				Name = "Armageddon " + group.Key, EditableItem = Approval() };
			_db.ForagableProfiles.Add(profile);
			_forageProfiles.Add(sector, profile.Id);
			foreach (var category in group.Select(x => x.Category.ToLowerInvariant()).Distinct())
			{
				profile.ForagableProfilesMaximumYields.Add(new() { ForageType = category, Yield = 1.0 });
				profile.ForagableProfilesHourlyYieldGains.Add(new() { ForageType = category, Yield = 0.0 });
			}
			foreach (var source in group)
			{
				if (!_items.TryGetValue(source.Vnum, out var item) || source.Shares <= 0)
					throw new InvalidOperationException($"Unqualified forage output at {source.Source}.");
				var minimum = (source.Rarity & 12) != 0 ? Outcome.MajorPass :
					(source.Rarity & 2) != 0 ? Outcome.Pass : Outcome.MinorPass;
				var forage = new Foragable
				{
					Id = Next<Foragable>(), RevisionNumber = 0, EditableItem = Approval(),
					Name = Key(source.Vnum) + " " + group.Key + " " + source.Category,
					ForagableTypes = source.Category.ToLowerInvariant(), ItemProtoId = item.Id,
					ForageDifficulty = (int)Difficulty.Normal, MinimumOutcome = (int)minimum,
					MaximumOutcome = (int)Outcome.MajorPass, RelativeChance = source.Shares,
					YieldConsumptionPercentage = 0.0, QuantityDiceExpression = "1"
				};
				_db.Foragables.Add(forage);
				profile.ForagableProfilesForagables.Add(new() { ForagableId = forage.Id });
				Record("forage." + forage.Id, nameof(Foragable), forage.Id, 0, source);
			}
			foreach (var sourceRoom in _source.Rooms.Where(x => x.Sector == sector && (x.Flags & (1 << 18)) == 0))
				_rooms[sourceRoom.Vnum].ForagableProfileId = profile.Id;
		}
		_counts["forage results"] = _source.Forage.Length;
		Adapt("foraging checks", 0, "Source shares and sector/category membership are preserved. Native Normal forage checks and success bands replace the source percentile rarity roll and its fallback distribution: common=MinorPass+, uncommon=Pass+, rare/ultra-rare=MajorPass. All imported results consume zero yield points.");
		foreach (var source in _source.Crafts) InstallCraft(source);
		_counts.TryAdd("crafts", 0);
		Flush();
	}

	private void InstallCraft(JsonElement source)
	{
		string Text(string key) => source.TryGetProperty(key, out var value) && value.ValueKind == JsonValueKind.String ? value.GetString() ?? "" : "";
		int Number(string key, int fallback = 0) => int.TryParse(Text(key), NumberStyles.Integer, CultureInfo.InvariantCulture, out var value) ? value : fallback;
		int[] Vnums(string key) => Text(key).Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)
			.Select(x => int.Parse(x, CultureInfo.InvariantCulture)).ToArray();
		var name = Text("NAME");
		var inputs = Vnums("FROM");
		var outputs = Vnums("INTO_SUCCESS");
		var failures = Vnums("INTO_FAIL");
		var missing = inputs.Concat(outputs).Concat(failures).Where(x => !_items.ContainsKey(x)).Distinct().ToArray();
		var skillName = Text("SKILL");
		var skill = _db.TraitDefinitions.FirstOrDefault(x => x.Type == 0 && x.Name == skillName);
		if (inputs.Length == 0 || outputs.Length == 0 || missing.Length > 0 || skill is null ||
			Number("REQUIRE_FIRE") != 0 || !Text("TOOL_TYPE").Equals("none", StringComparison.OrdinalIgnoreCase))
		{
			var reason = $"Recipe '{name}' is retained without an active craft: missing vnums [{string.Join(",", missing)}], " +
				$"installed skill={skill is not null}, nonempty input/output={inputs.Length > 0 && outputs.Length > 0}, fire/tool restriction={Text("REQUIRE_FIRE")}/{Text("TOOL_TYPE")}.";
			Adapt("unusable craft", 0, reason);
			Record("craft.unusable." + name, "SourceCraft", null, null, new { Source = source, Reason = reason });
			return;
		}
		var minimum = Number("PERC_MIN");
		var tribe = Number("ONLY_TRIBE");
		var permission = $"return gettrait(@actor, totrait({skill.Id})) >= {minimum}";
		if (tribe > 0)
		{
			if (!_clans.TryGetValue(tribe, out var clan)) throw new InvalidOperationException($"Craft {name} refers to missing clan {tribe}.");
			permission += $" and isclanmember(@actor, toclan({clan.Id}))";
		}
		var gate = Prog("ArmageddonWorldfileCraft" + (_counts.GetValueOrDefault("crafts") + 1), permission,
			ProgVariableTypes.Boolean, ("actor", ProgVariableTypes.Character));
		var craft = new Craft
		{
			Id = Next<Craft>(), RevisionNumber = 0, EditableItem = Approval(), Name = name,
			Blurb = Text("DESC"), Category = "Armageddon", ActionDescription = "crafting " + name,
			ActiveCraftItemSdesc = "an unfinished " + name, Interruptable = true,
			AppearInCraftsListProgId = gate.Id, CanUseProgId = gate.Id, CheckTraitId = skill.Id,
			CheckDifficulty = (int)Difficulty.Normal, FailPhase = 1, FailThreshold = (int)Outcome.MinorFail,
			QualityFormula = "5", IsPracticalCheck = true, CheckQualityWeighting = 1.0,
			InputQualityWeighting = 1.0, ToolQualityWeighting = 1.0
		};
		craft.CraftPhases.Add(new() { PhaseNumber = 1, PhaseLengthInSeconds = Math.Max(1.0, (Number("DELAY_MIN", 1) + Number("DELAY_MAX", 1)) / 2.0),
			Echo = "$0 craft|crafts $p1.", FailEcho = "$0 fail|fails to finish crafting.", ExertionLevel = 1, StaminaUsage = 0.0 });
		foreach (var input in inputs.GroupBy(x => x))
			craft.CraftInputs.Add(new() { Id = Next<CraftInput>(), InputType = "SimpleItem", InputQualityWeight = 1.0,
				OriginalAdditionTime = DateTime.UtcNow, Definition = new XElement("Definition",
					new XElement("TargetItemId", _items[input.Key].Id), new XElement("Quantity", input.Count())).ToString() });
		foreach (var fail in new[] { false, true })
		foreach (var output in (fail ? failures : outputs).GroupBy(x => x))
			craft.CraftProducts.Add(new() { Id = Next<CraftProduct>(), ProductType = "SimpleProduct", IsFailProduct = fail,
				OriginalAdditionTime = DateTime.UtcNow, Definition = new XElement("Definition", new XElement("ProductProducedId", _items[output.Key].Id),
					new XElement("Quantity", output.Count()), new XElement("Skin", 0)).ToString() });
		_db.Crafts.Add(craft);
		Record("craft." + craft.Id, nameof(Craft), craft.Id, 0, source);
		_counts["crafts"] = _counts.GetValueOrDefault("crafts") + 1;
		Adapt("craft check", (int)craft.Id, "The source skill threshold is retained as a permission; native Normal crafting success and a deterministic mean delay replace the source percentile modifier and random delay. Literal source echoes remain in provenance; the active craft has one simple native phase.");
	}
}
