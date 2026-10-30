#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using CultureInfo = System.Globalization.CultureInfo;
using System.Linq;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	internal static string NpcResetFitArguments(OutfitTemplateItemPlacement placement, int sourceItemType) =>
		placement == OutfitTemplateItemPlacement.Worn && sourceItemType is 9 or 11
			? "wearsize=\"for self\"" : string.Empty;

	private sealed class ResetOutfitItem
	{
		public required ArmWorldReset Reset { get; init; }
		public required GameItemProto Prototype { get; set; }
		public required int SourceVnum { get; init; }
		public required int[] Values { get; init; }
		public required OutfitTemplateItemPlacement Placement { get; init; }
		public string? ContainerKey { get; set; }
		public long? WearProfileId { get; init; }
		public List<ArmWorldExtra> ExtraDescriptions { get; } = [];
		public string Key => "reset-" + Reset.Source.Line.ToString(CultureInfo.InvariantCulture);
	}
	private readonly Dictionary<int, long[]> _npcFamilyIds = new();
	private sealed record NpcLoadoutPlan(NpcTemplate Template, OutfitTemplate Outfit, long CashProg, ResetOutfitItem[] Items);
	private readonly List<NpcLoadoutPlan> _npcLoadoutPlans = [];

	internal static ArmWorldReset ResolvePopulationReset(ArmWorldReset source)
	{
		if (source.Opcode != "M" || source.Arguments.Length < 4 ||
			source.Arguments[1] is not (46704 or 46705) || source.Arguments[3] != 2011) return source;
		var arguments = source.Arguments.ToArray();
		arguments[3] = 20040;
		return source with { Arguments = arguments };
	}

	private void InstallPopulation()
	{
		var resetRows = _source.Resets.OrderBy(x => x.Zone).ThenBy(x => x.Source.Line).Select(source =>
		{
			var mapped = ResolvePopulationReset(source);
			if (!ReferenceEquals(mapped, source))
				Adapt("NPC home placement", source.Arguments[1], "Approved starting/home placement in House Kohmar's offices 20040, the coded return-home destination; source reset room 2011 is exitless. Archive/reset provenance remains unchanged.");
			return mapped;
		}).ToArray();
		var invalid = resetRows.Where(x => x.ReferenceIssues.Length > 0).ToArray();
		foreach (var reset in invalid)
		{
			Adapt("reset", reset.Source.Line,
				$"Skipped invalid {reset.Opcode} reset in zone {reset.Zone}: {string.Join("; ", reset.ReferenceIssues)}");
		}

		var validRows = resetRows.Where(x => x.ReferenceIssues.Length == 0).ToArray();
		PrepareObjectResetVariants(validRows);
		var mobileVariants = BuildNpcOutfitVariants(resetRows);
		foreach (var family in validRows.Where(x => x.Opcode == "M").GroupBy(x => x.Arguments[1]))
			_npcFamilyIds[family.Key] = family.Select(x => mobileVariants[(x.Zone, x.Source.Line)].Id)
				.Append(_npcs[family.Key].Id).Distinct().ToArray();
		InstallNativeSpawners(validRows, mobileVariants);
		foreach (var zoneGroup in resetRows.GroupBy(x => x.Zone))
		{
			var zone = _source.Zones.SingleOrDefault(x => x.Index == zoneGroup.Key);
			if (zone is null || !_zones.TryGetValue(zoneGroup.Key, out var nativeZone)) continue;
			var validCommands = zoneGroup.OrderBy(x => x.Source.Line).ToArray();
			var code = BuildZoneResetProgram(zoneGroup.Key, validCommands, mobileVariants, boot: true);
			var bootProg = Prog($"ArmageddonWorldfileZone{zoneGroup.Key}Boot", code,
				ProgVariableTypes.Boolean, ("room", ProgVariableTypes.Location));
			var sentinel = _rooms.Where(x => !_interiorTemplates.Contains(x.Key)).Select(x => x.Value).Where(x => x.ZoneId == nativeZone.Id).OrderBy(x => x.Id).FirstOrDefault();
			if (sentinel is not null)
			{
				var hook = new Hooks
				{
					Id = Next<Hooks>(), Name = $"Armageddon worldfile zone {zoneGroup.Key} boot population",
					Type = "FutureProgHook", Category = "Worldfile", TargetEventType = 155,
					Definition = new XElement("Definition", new XElement("FutureProg", bootProg.Id)).ToString()
				};
				_db.Hooks.Add(hook);
				_db.HooksPerceivables.Add(new HooksPerceivable { HookId = hook.Id, RoomId = sentinel.Id });
				Record($"population.boothook.{zoneGroup.Key}", nameof(Hooks), hook.Id, null,
					new { Zone = zoneGroup.Key, SentinelRoom = sentinel.Id, Prog = bootProg.FunctionName });
			}
			else Adapt("population boot hook", zoneGroup.Key, "No imported room was available to coordinate this zone's boot reset.");

			var lifespan = zone.Fields.ElementAtOrDefault(1);
			var resetMode = zone.Fields.ElementAtOrDefault(2);
			if (lifespan > 0 && resetMode is 1 or 2)
			{
				var calendar = _db.Calendars.Single(x => x.Id == _db.Cultures.Single(c => c.Id == _settings.NativeBindings[ArmageddonNativeBinding.Culture]).PrimaryCalendarId);
				var clock = _db.Clocks.Single(x => x.Id == calendar.FeedClockId);
				var clockXml = XElement.Parse(clock.Definition);
				var rate = (double)clockXml.Element("InGameSecondsPerRealSecond")!;
				var secondsPerMinute = (int)clockXml.Element("SecondsPerMinute")!;
				var nativeMinutes = checked((int)Math.Ceiling(lifespan * 60.0 * rate / secondsPerMinute));
				var timezone = _db.Timezones.Single(x => x.Id == clock.PrimaryTimezoneId);
				var recurringCode = BuildZoneResetProgram(zoneGroup.Key, validCommands, mobileVariants, boot: false);
				var lastReturn = recurringCode.LastIndexOf("return true", StringComparison.Ordinal);
				var body = lastReturn >= 0 ? recurringCode[..lastReturn] : recurringCode;
				var playersInZone = $"loadedcharacters().Count(x, @x.npctemplateid == 0 and not(isnull(@x.location)) and @x.location.zone.id == {nativeZone.Id})";
				if (resetMode == 1) recurringCode = $"if ({playersInZone} == 0)\n{body}\nend if\nreturn true";
				var scheduleProg = Prog($"ArmageddonWorldfileZone{zoneGroup.Key}Reset", recurringCode, ProgVariableTypes.Boolean);
				_db.ProgSchedules.Add(new ProgSchedule
				{
					Id = Next<ProgSchedule>(), Name = $"Armageddon worldfile zone {zoneGroup.Key} reset",
					IntervalType = (int)MudSharp.TimeAndDate.Intervals.IntervalType.Minutely,
					IntervalModifier = nativeMinutes, IntervalOther = 0, IntervalOtherSecondary = 0,
					IntervalFallback = 0, ReferenceDate = $"{calendar.Id}_{calendar.Date}_{clock.Id}_{timezone.Name} {clock.Hours}:{clock.Minutes}:{clock.Seconds}",
					ReferenceTime = string.Empty, FutureProgId = scheduleProg.Id
				});
			}
			else if (lifespan > 0)
				Adapt("zone reset mode", zoneGroup.Key, $"Source reset mode {resetMode} is non-recurring in the source; only boot reset progs were installed.");
		}

		foreach (var reset in resetRows.Where(x => x.ReferenceIssues.Length == 0 && x.Opcode == "A"))
			Adapt("reset A", reset.Source.Line, "Around/table placement is not persisted by native room or container item placement; the source corpus command was retained.");
		foreach (var reset in resetRows.Where(x => x.ReferenceIssues.Length == 0 && x.Opcode == "Y"))
			Adapt("reset Y", reset.Source.Line, "The source NPC extra-description table has no native Simple-template field; the source text is retained in provenance.");
		foreach (var reset in resetRows.Where(x => x.ReferenceIssues.Length == 0 && x.Opcode == "D" && x.Arguments.ElementAtOrDefault(0) != 0))
			Adapt("reset D conditional", reset.Source.Line, "Door state was installed as the room's boot state; source success-chain conditionals are not evaluated during installation.");
		foreach (var reset in resetRows.Where(x => x.ReferenceIssues.Length == 0 && x.Opcode == "X" && x.Arguments.ElementAtOrDefault(1) != 1))
			Adapt("reset X", reset.Source.Line, "Source X selector outside selector 1 is a legacy name/keyword no-op in the active source interpreter.");
		Record("population.resets", "ArmageddonWorldfileResetManifest", null, null,
			new { Rows = resetRows, InvalidRows = invalid.Length, MobileSpawnerCount = _counts.GetValueOrDefault("NPC population spawners") });
		Flush();
	}

	private Dictionary<(int Zone, int Line), NpcTemplate> BuildNpcOutfitVariants(ArmWorldReset[] resets)
	{
		var result = new Dictionary<(int Zone, int Line), NpcTemplate>();
		foreach (var zone in resets.GroupBy(x => x.Zone))
		{
			ArmWorldReset? mobile = null;
			var outfitItems = new List<ResetOutfitItem>();
			ResetOutfitItem? lastItem = null;
			var parentStack = new List<ResetOutfitItem>();
			var equippedPositions = new HashSet<int>();
			var lastSucceeded = true;
			var lastObjectIsRoomItem = false;
			void SaveMobileVariant()
			{
				if (mobile is null || !_npcs.TryGetValue(mobile.Arguments[1], out var baseTemplate)) return;
				var outfit = new OutfitTemplate
				{
					Id = Next<OutfitTemplate>(), Name = ResetOutfitName(mobile),
					Description = "Equipment and carried objects created by the source reset sequence.", Exclusivity = 0
				};
				foreach (var item in outfitItems)
				{
					outfit.OutfitTemplateItems.Add(new OutfitTemplateItem
					{
						Id = Next<OutfitTemplateItem>(), OutfitTemplateId = outfit.Id, TemplateKey = item.Key,
						GameItemProtoId = item.Prototype.Id, Placement = (int)item.Placement,
						ContainerKey = item.ContainerKey ?? string.Empty, LoadArguments = NpcResetFitArguments(item.Placement, _effectiveItems.GetValueOrDefault(item.Prototype.Id).Source?.ItemType ?? 0), WearOrder = 0,
						WearProfileId = item.WearProfileId
					});
				}
				_db.OutfitTemplates.Add(outfit);
				var clone = (NpcTemplate)_db.Entry(baseTemplate).CurrentValues.ToObject();
				clone.Id = Next<NpcTemplate>(); clone.Name = baseTemplate.Name + $" reset {mobile.Source.Line}";
				clone.UniqueName = Key(mobile.Arguments[1]) + "_Reset" + mobile.Zone.ToString(CultureInfo.InvariantCulture) + "_" + mobile.Source.Line.ToString(CultureInfo.InvariantCulture);
				clone.EditableItemId = 0; clone.EditableItem = Approval();
				var definition = XElement.Parse(baseTemplate.Definition);
				var additions = definition.Element("TemplateLoadAdditions")!;
				additions.Element("Outfits")!.Add(new XElement("Outfit", new XAttribute("template", outfit.Id), new XAttribute("name", outfit.Name)));
				var sourceOnLoadProgId = long.Parse(definition.Element("OnLoadProg")!.Value, CultureInfo.InvariantCulture);
				_npcLoadoutPlans.Add(new(clone, outfit, sourceOnLoadProgId, outfitItems.ToArray()));
				clone.Definition = definition.ToString();
				_db.NpcTemplates.Add(clone);
				result[(mobile.Zone, mobile.Source.Line)] = clone;
				Record($"npc.reset.{mobile.Zone}.{mobile.Arguments[1]}.{mobile.Source.Line}", nameof(NpcTemplate), clone.Id, clone.RevisionNumber,
					new { SourceNpcVnum = mobile.Arguments[1], SourceMLine = mobile.Source.Line,
						Items = outfitItems.Select(x => new { x.SourceVnum, x.Reset.Source.Line, x.Placement, x.ContainerKey, ProtoId = x.Prototype.Id }) });
				_counts["reset NPC outfit variants"] = _counts.GetValueOrDefault("reset NPC outfit variants") + 1;
			}

			foreach (var reset in zone.OrderBy(x => x.Source.Line))
			{
				if (reset.ReferenceIssues.Length > 0)
				{
					if (reset.Opcode == "M") { SaveMobileVariant(); mobile = null; outfitItems = []; }
					lastSucceeded = false; lastItem = null; parentStack.Clear();
					lastObjectIsRoomItem = reset.Opcode == "O";
					continue;
				}
				if (reset.Opcode == "M")
				{
					SaveMobileVariant();
					mobile = reset; outfitItems = []; parentStack = []; lastItem = null;
					equippedPositions = []; lastSucceeded = true;
					continue;
				}
				if (mobile is null) continue;
				if (reset.Opcode == "X" && reset.Arguments.ElementAtOrDefault(1) == 1 && lastItem is not null && reset.Strings.Length >= 2)
				{
					if (reset.Arguments.ElementAtOrDefault(0) != 0 && !lastSucceeded) { lastSucceeded = false; continue; }
					lastItem.ExtraDescriptions.Add(new(reset.Strings[0], reset.Strings[1]));
					lastItem.Prototype = GetResetItem(_source.Items.Single(x => x.Vnum == lastItem.SourceVnum),
						lastItem.Values, "reset-" + lastItem.Reset.Source.Line, lastItem.ExtraDescriptions, lastItem.Reset.Opcode == "E");
					lastSucceeded = true;
					continue;
				}
				if (reset.Opcode == "O")
				{
					lastItem = null;
					parentStack = [];
					lastSucceeded = false;
					lastObjectIsRoomItem = true;
					continue;
				}
				if (reset.Opcode == "P" && lastObjectIsRoomItem) continue;
				if (reset.Opcode is not ("G" or "E" or "P") || reset.Arguments.Length < 10)
				{
					if (reset.Opcode is "O" or "D") lastSucceeded = true;
					continue;
				}
				var enabled = reset.Opcode == "P" ? reset.Arguments[0] == 0 : reset.Arguments[0] == 0;
				if (!enabled && !lastSucceeded) { lastSucceeded = false; continue; }
				var itemVnum = reset.Arguments[1];
				if (!_items.ContainsKey(itemVnum)) { lastSucceeded = false; continue; }
				var sourceItem = _source.Items.Single(x => x.Vnum == itemVnum);
				var values = ResetValues(reset);
				if (values.Length != 6) { lastSucceeded = false; continue; }
				var placement = reset.Opcode switch
				{
					"E" when reset.Arguments[3] is 16 or 17 or 19 && sourceItem.ItemType is 5 or 28 => OutfitTemplateItemPlacement.Wielded,
					"E" when reset.Arguments[3] is 11 or 18 && (sourceItem.Flags[2] & (1 << 16)) != 0 => OutfitTemplateItemPlacement.AttachedToBelt,
					"E" when reset.Arguments[3] is 11 or 16 or 17 or 18 or 19 => OutfitTemplateItemPlacement.Inventory,
					"E" => OutfitTemplateItemPlacement.Worn,
					"P" => OutfitTemplateItemPlacement.Container,
					_ => OutfitTemplateItemPlacement.Inventory
				};
				var item = new ResetOutfitItem
				{
					Reset = reset, SourceVnum = itemVnum, Values = values, Placement = placement,
					Prototype = GetResetItem(sourceItem, values, "reset-" + reset.Source.Line, forceLit: reset.Opcode == "E",
						forcedWearMask: placement == OutfitTemplateItemPlacement.Worn ? ResetForcedWearMask(sourceItem, reset.Arguments[3]) : 0),
					WearProfileId = placement == OutfitTemplateItemPlacement.Worn ? SourceEquipmentProfile(sourceItem, reset.Arguments[3], resetOverride: true) : null
				};
				if (placement == OutfitTemplateItemPlacement.Container)
				{
					var depth = Math.Max(0, reset.Arguments[0]);
					var parentIndex = Math.Clamp(depth - 1, 0, Math.Max(0, parentStack.Count - 1));
					var parent = parentStack.ElementAtOrDefault(parentIndex);
					var parentCanContain = parent?.Prototype.GameItemProtosGameItemComponentProtos
						.Any(x => x.GameItemComponent.Type.Contains("Container", StringComparison.OrdinalIgnoreCase)) == true;
					if (parent is null || !parentCanContain)
					{
						Adapt("reset P", reset.Source.Line, "No native outfit container ancestor matched the source P depth; the item remains in source provenance.");
						lastSucceeded = false;
						continue;
					}
					item.ContainerKey = parent.Key;
					if (parentStack.Count > parentIndex + 1) parentStack.RemoveRange(parentIndex + 1, parentStack.Count - parentIndex - 1);
				}
				if (reset.Opcode == "G" && outfitItems.Count(x => x.SourceVnum == itemVnum && x.Placement == OutfitTemplateItemPlacement.Inventory) >= reset.Arguments[2])
				{
					lastSucceeded = false;
					continue;
				}
				if (reset.Opcode == "E" && !equippedPositions.Add(reset.Arguments[3]))
				{
					lastSucceeded = false;
					continue;
				}
				outfitItems.Add(item); lastItem = item;
				if (placement != OutfitTemplateItemPlacement.Container) parentStack.Clear();
				parentStack.Add(item);
				lastObjectIsRoomItem = false;
				lastSucceeded = true;
				if (reset.Opcode == "E")
					Adapt("reset E wear position", reset.Source.Line, $"Source equipment position {reset.Arguments[3]} maps to native {placement}; native fit selects the body-compatible wear profile.");
			}
			SaveMobileVariant();
		}
		return result;
	}

	private void FinalizeNpcLoadPrograms()
	{
		foreach (var plan in _npcLoadoutPlans)
		{
			var remainingKeys = plan.Outfit.OutfitTemplateItems.Select(x => x.TemplateKey).ToHashSet(StringComparer.Ordinal);
			var transferred = plan.Items.Where(x => !remainingKeys.Contains(x.Key)).Select(x => x.Key).ToHashSet(StringComparer.Ordinal);
			var cash = _progs.Single(x => x.Id == plan.CashProg);
			var code = BuildLoadoutItemLimitCode(cash.FunctionText, plan.Items, transferred);
			if (code is null) continue;
			var prog = Prog($"ArmageddonWorldfileNpcLimit{plan.Template.Id}", code,
				ProgVariableTypes.Boolean, ("npc", ProgVariableTypes.Character));
			var definition = XElement.Parse(plan.Template.Definition);
			definition.Element("OnLoadProg")!.Value = prog.Id.ToString(CultureInfo.InvariantCulture);
			plan.Template.Definition = definition.ToString();
		}
	}

	private string? BuildLoadoutItemLimitCode(string sourceOnLoadCode, IReadOnlyList<ResetOutfitItem> items, IReadOnlySet<string> transferred)
	{
		var limitedItems = items
			.Where(x => !transferred.Contains(x.Key) && x.Reset.Opcode != "P" && _source.Items.Single(source => source.Vnum == x.SourceVnum).Limit >= 0)
			.ToArray();
		if (limitedItems.Length == 0) return null;

		var limitedVnums = limitedItems.Select(x => x.SourceVnum).Distinct().OrderBy(x => x).ToArray();
		var builder = new System.Text.StringBuilder();
		var returnIndex = sourceOnLoadCode.LastIndexOf("return true", StringComparison.OrdinalIgnoreCase);
		builder.AppendLine(returnIndex >= 0
			? sourceOnLoadCode[..returnIndex].TrimEnd()
			: sourceOnLoadCode.TrimEnd());
		builder.AppendLine("var loadoutlimitlast as boolean");
		builder.AppendLine("loadoutlimitlast = true");
		builder.AppendLine("var loadoutpending as item collection\nloadoutpending = collectionconcat(@npc.inventory, @npc.inventory.SelectMany(i, @i.contents))");

		string FamilyFor(int vnum, string variable) =>
			ItemVariantIdExpression(vnum).Replace("@x.proto", $"@{variable}.proto", StringComparison.Ordinal);

		foreach (var vnum in limitedVnums)
		{
			var family = ItemVariantIdExpression(vnum);
			var familyForInventory = FamilyFor(vnum, "i");
			var familyForContents = FamilyFor(vnum, "c");
			builder.AppendLine($"var loadoutoutside{vnum} as number");
			builder.AppendLine($"loadoutoutside{vnum} = loadeditems().Count(x, {family}) - @npc.inventory.Count(i, {familyForInventory}) - @npc.inventory.SelectMany(i, @i.contents).Count(c, {familyForContents})");
			builder.AppendLine($"var loadoutaccepted{vnum} as number");
			builder.AppendLine($"loadoutaccepted{vnum} = 0");
			var source = _source.Items.Single(x => x.Vnum == vnum);
			Adapt("reset outfit global limit", vnum,
				$"Source item family limit {source.Limit} is checked sequentially in the NPC on-load prog after native outfit items materialise. Native outfit wear-fit processing may therefore occur before a capped item is pruned.");
		}

		for (var index = 0; index < items.Count; index++)
		{
			var item = items[index];
			if (transferred.Contains(item.Key))
			{
				builder.AppendLine("loadoutlimitlast = true");
				continue;
			}
			var itemVariable = "loadoutitem" + index.ToString(CultureInfo.InvariantCulture);
			var itemLookup = $"@loadoutpending.First(i, @i.proto == {item.Prototype.Id})";
			builder.AppendLine($"var {itemVariable} as item");
			builder.AppendLine($"{itemVariable} = {itemLookup}");
			builder.AppendLine($"if (not(isnull(@{itemVariable})))\nloadoutpending = @loadoutpending.Where(i, @i.id != @{itemVariable}.id)\nend if");

			if (item.Reset.Opcode == "P")
			{
				var parent = items.SingleOrDefault(x => x.Key == item.ContainerKey);
				if (parent is null)
				{
					builder.AppendLine($"if (not(isnull(@{itemVariable})))");
					builder.AppendLine($"\tdeleteitem(@{itemVariable})");
					builder.AppendLine("end if");
					builder.AppendLine("loadoutlimitlast = false");
					continue;
				}

				var parentVariable = "loadoutparent" + index.ToString(CultureInfo.InvariantCulture);
				var parentLookup = $"ifnull(@npc.inventory.First(i, @i.proto == {parent.Prototype.Id}), @npc.inventory.SelectMany(i, @i.contents).First(i, @i.proto == {parent.Prototype.Id}))";
				builder.AppendLine($"var {parentVariable} as item");
				builder.AppendLine($"{parentVariable} = {parentLookup}");
				builder.AppendLine($"if (@loadoutlimitlast and not(isnull(@{parentVariable})) and not(isnull(@{itemVariable})))");
				builder.AppendLine("\tloadoutlimitlast = true");
				if (limitedVnums.Contains(item.SourceVnum)) builder.AppendLine($"loadoutaccepted{item.SourceVnum} = @loadoutaccepted{item.SourceVnum} + 1");
				builder.AppendLine("else");
				builder.AppendLine($"\tif (not(isnull(@{itemVariable})))");
				builder.AppendLine($"\t\tdeleteitem(@{itemVariable})");
				builder.AppendLine("\tend if");
				builder.AppendLine("\tloadoutlimitlast = false");
				builder.AppendLine("end if");
				continue;
			}

			var conditionalGate = item.Reset.Arguments.ElementAtOrDefault(0) == 0 ? "true" : "@loadoutlimitlast";
			builder.AppendLine($"if ({conditionalGate} and not(isnull(@{itemVariable})))");
			if (item.Reset.Opcode != "P" && _source.Items.Single(source => source.Vnum == item.SourceVnum).Limit >= 0)
			{
				var limit = _source.Items.Single(source => source.Vnum == item.SourceVnum).Limit;
				builder.AppendLine($"\tif (@loadoutoutside{item.SourceVnum} + @loadoutaccepted{item.SourceVnum} >= {limit})");
				builder.AppendLine($"\t\tdeleteitem(@{itemVariable})");
				builder.AppendLine("\t\tloadoutlimitlast = false");
				builder.AppendLine("\telse");
				builder.AppendLine($"\t\tloadoutaccepted{item.SourceVnum} = @loadoutaccepted{item.SourceVnum} + 1");
				builder.AppendLine("\t\tloadoutlimitlast = true");
				builder.AppendLine("\tend if");
			}
			else
			{
				builder.AppendLine("\tloadoutlimitlast = true");
			}
			builder.AppendLine("else");
			builder.AppendLine($"\tif (not(isnull(@{itemVariable})))");
			builder.AppendLine($"\t\tdeleteitem(@{itemVariable})");
			builder.AppendLine("\tend if");
			builder.AppendLine("\tloadoutlimitlast = false");
			builder.AppendLine("end if");
		}

		builder.AppendLine("return true");
		return builder.ToString();
	}

	private void InstallNativeSpawners(ArmWorldReset[] resets, IReadOnlyDictionary<(int Zone, int Line), NpcTemplate> variants)
	{
		var allZoneIds = _db.Zones.AsNoTracking().Select(x => x.Id).ToArray();
		foreach (var reset in resets.Where(x => x.Opcode == "M").OrderBy(x => x.Zone).ThenBy(x => x.Source.Line))
		{
			if (_interiorTemplates.Contains(reset.Arguments[3]) || _source.Zones.Single(x => x.Index == reset.Zone).Fields[2] != 2 ||
			    !_rooms.TryGetValue(reset.Arguments[3], out var room)) continue;
			var source = _source.Npcs.Single(x => x.Vnum == reset.Arguments[1]);
			var cap = Math.Max(0, reset.Arguments[2]);
			if (source.Limit >= 0) cap = Math.Min(cap, source.Limit);
			if (cap == 0) continue;
			var ownTemplate = variants[(reset.Zone, reset.Source.Line)];
			var family = _npcFamilyIds[source.Vnum];
			var spawner = new NPCSpawner
			{
				Id = Next<NPCSpawner>(), Name = $"Armageddon {Key(source.Vnum)} reset {reset.Zone}:{reset.Source.Line}",
				TargetCount = cap, MinimumCount = cap, SpawnStrategy = 2, TargetTemplateId = null,
				IsActiveProgId = _settings.NativeBindings[ArmageddonNativeBinding.AlwaysTrueProg],
				Definition = new XElement("Definition", new XElement("Templates", family.Select(id => new XElement("Template",
					new XAttribute("id", id), new XAttribute("weight", id == ownTemplate.Id ? 1 : 0))))).ToString()
			};
			spawner.Rooms.Add(new NPCSpawnerRoom { NPCSpawnerId = spawner.Id, RoomId = room.Id });
			foreach (var zoneId in allZoneIds) spawner.Zones.Add(new NPCSpawnerZone { NPCSpawnerId = spawner.Id, ZoneId = zoneId });
			_db.NpcSpawners.Add(spawner);
			Record($"population.mobile.{reset.Zone}.{reset.Source.Line}", nameof(NPCSpawner), spawner.Id, null,
				new { SourceVnum = source.Vnum, Reset = reset.Source, Room = room.Id, Cap = cap, Family = family, Template = ownTemplate.Id });
			_counts["NPC population spawners"] = _counts.GetValueOrDefault("NPC population spawners") + 1;
		}
		Adapt("reset M recurrence", 0,
			"Each source reset-mode 2 M command has a native spawner at its authored room, counting the entire Vnum family globally. Native polling fills its deficit rather than creating at most one NPC per source zone reset; conditional M success chains are not replayed by spawners. Mode 1 retains scheduled empty-zone checks.");
	}

	private void PrepareObjectResetVariants(ArmWorldReset[] resets)
	{
		foreach (var zone in resets.GroupBy(x => x.Zone))
		{
			var ordered = zone.OrderBy(x => x.Source.Line).ToArray();
			for (var i = 0; i < ordered.Length; i++)
			{
				var reset = ordered[i];
				if (reset.Opcode is not ("O" or "G" or "E" or "P") || reset.Arguments.Length < (reset.Opcode == "G" ? 10 : 11)) continue;
				var extras = ordered.Skip(i + 1).TakeWhile(x => x.Opcode == "X")
					.Where(x => x.Arguments.ElementAtOrDefault(1) == 1 && x.Strings.Length >= 2)
					.Select(x => new ArmWorldExtra(x.Strings[0], x.Strings[1])).ToArray();
				var item = _source.Items.SingleOrDefault(x => x.Vnum == reset.Arguments[1]);
				if (item is null) continue;
				GetResetItem(item, ResetValues(reset), "reset-" + reset.Source.Line, extras, reset.Opcode == "E",
					reset.Opcode == "E" ? ResetForcedWearMask(item, reset.Arguments[3]) : 0);
			}
		}
	}

	internal static int[] ResetValues(ArmWorldReset reset) => reset.Arguments.Skip(reset.Opcode == "G" ? 3 : 4).Take(6).ToArray();
	internal static string ResetOutfitName(ArmWorldReset reset) =>
		$"Armageddon {Key(reset.Arguments[1])} equipment zone {reset.Zone} line {reset.Source.Line}";

	private string ItemVariantIdExpression(int sourceVnum)
	{
		var ids = new HashSet<long>();
		if (_items.TryGetValue(sourceVnum, out var baseItem)) ids.Add(baseItem.Id);
		var prefix = sourceVnum.ToString(CultureInfo.InvariantCulture) + ":";
		foreach (var variant in _resetItemVariants.Where(x => x.Key.StartsWith(prefix, StringComparison.Ordinal)).Select(x => x.Value))
			ids.Add(variant.Id);
		return string.Join(" or ", ids.OrderBy(x => x).Select(x => $"@x.proto == {x}"));
	}

	private static bool IsNativeContainer(GameItemProto proto) => proto.GameItemProtosGameItemComponentProtos
		.Any(x => x.GameItemComponent.Type.Contains("Container", StringComparison.OrdinalIgnoreCase));

	private string BuildZoneResetProgram(int zoneId, ArmWorldReset[] resets,
		IReadOnlyDictionary<(int Zone, int Line), NpcTemplate> variants, bool boot)
	{
		var builder = new System.Text.StringBuilder("var lastcmd as boolean\nlastcmd = true\nvar lastobj as item\nvar mobilenew as boolean\nmobilenew = false\n");
		var lastCommand = "lastcmd";
		var roomObjectContext = false;
		var extras = new Dictionary<int, List<ArmWorldExtra>>();
		var previousObject = -1;
		for (var i = 0; i < resets.Length; i++)
		{
			if (resets[i].Opcode is "O" or "G" or "E" or "P") previousObject = i;
			if (resets[i].Opcode != "X" || resets[i].Arguments[1] != 1 || previousObject < 0) continue;
			if (!extras.TryGetValue(previousObject, out var list)) extras[previousObject] = list = [];
			list.Add(new(resets[i].Strings[0], resets[i].Strings[1]));
		}
		for (var i = 0; i < resets.Length; i++) builder.AppendLine($"var cmd{i} as boolean\ncmd{i} = false");
		var objectLoopOpen = false;
		for (var index = 0; index < resets.Length; index++)
		{
			var reset = resets[index];
			if (objectLoopOpen && reset.Opcode is not ("P" or "X"))
			{
				builder.AppendLine("end foreach");
				objectLoopOpen = false;
			}
			var arguments = reset.Arguments;
			var command = "cmd" + index;
			if (reset.Opcode == "O" && reset.ReferenceIssues.Length == 0 && _rooms.TryGetValue(arguments[3], out var rootRoom))
			{
				builder.AppendLine($"foreach (destination{index} in instancerooms(tolocation({rootRoom.Id})))");
				objectLoopOpen = true;
			}
			builder.AppendLine($"{command} = false");
			builder.AppendLine($"if ({(arguments[0] == 0 ? "true" : "@" + lastCommand)})");
			if (reset.ReferenceIssues.Length > 0)
			{
				builder.AppendLine($"{command} = false");
				if (reset.Opcode == "M") { builder.AppendLine("mobilenew = false"); roomObjectContext = false; }
				if (reset.Opcode == "O") { builder.AppendLine("lastobj = null(\"item\")"); roomObjectContext = true; }
			}
			else if (reset.Opcode == "M" && _rooms.TryGetValue(arguments[3], out var destination))
			{
				var source = _source.Npcs.Single(x => x.Vnum == arguments[1]);
				var template = variants[(reset.Zone, reset.Source.Line)];
				var family = _npcFamilyIds[source.Vnum];
				var familyCount = "loadedcharacters().Count(x, " + string.Join(" or ", family.Select(x => "@x.npctemplateid == " + x)) + ")";
				var maximum = source.Limit >= 0 ? source.Limit : int.MaxValue;
				if (!boot) maximum = Math.Min(maximum, Math.Max(0, arguments[2]));
				var mode = _source.Zones.Single(x => x.Index == zoneId).Fields[2];
				builder.AppendLine("mobilenew = false");
				builder.AppendLine($"foreach (destination{index} in instancerooms(tolocation({destination.Id})))");
				if (boot || mode != 2 || _interiorTemplates.Contains(arguments[3]))
				{
					var slot = boot ? $" and loadedcharacters().Count(x, @x.npctemplateid == {template.Id} and @x.location == @destination{index}) == 0" : "";
					builder.AppendLine($"if ({familyCount} < {maximum}{slot})\nvar mobile{index} as character\nmobile{index} = loadnpc({template.Id}, @destination{index})\nmobilenew = not(isnull(@mobile{index}))\nend if");
				}
				builder.AppendLine($"{command} = @mobilenew\nend foreach");
				roomObjectContext = false;
			}
			else if (reset.Opcode == "O" && arguments[1] == 20610 &&
			         TryBuildTableReset(arguments[3], $"destination{index}", index, command) is { } tableReset)
			{
				builder.AppendLine(tableReset);
				roomObjectContext = true;
			}
			else if (reset.Opcode == "O" && _rooms.TryGetValue(arguments[3], out var room))
			{
				var source = _source.Items.Single(x => x.Vnum == arguments[1]);
				var prototype = GetResetItem(source, ResetValues(reset), "reset-" + reset.Source.Line, extras.GetValueOrDefault(index));
				var family = ItemVariantIdExpression(source.Vnum);
				var limit = source.Limit >= 0 ? source.Limit : int.MaxValue;
				builder.AppendLine("lastobj = null(\"item\")");
				builder.AppendLine($"if (@destination{index}.items.Count(x, {family}) < {Math.Max(0, arguments[2])} and loadeditems().Count(x, {family}) < {limit})");
				var isVehicle = _vehicleByItemProto.TryGetValue(prototype.Id, out var vehicleProto);
				var create = isVehicle
					? $"loadvehicle({vehicleProto}, @destination{index})" : $"loaditem({prototype.Id})";
				var placed = isVehicle ? $"not(isnull(@item{index}))" : $"put(@item{index}, @destination{index})";
				builder.AppendLine($"var item{index} as item\nitem{index} = {create}\n{command} = {placed}");
				builder.AppendLine($"if (@{command})\nlastobj = @item{index}\nelse\nif (not(isnull(@item{index})))\ndeleteitem(@item{index})\nend if\nend if\nend if");
				roomObjectContext = true;
			}
			else if (reset.Opcode == "P" && roomObjectContext)
			{
				var source = _source.Items.Single(x => x.Vnum == arguments[1]);
				var prototype = GetResetItem(source, ResetValues(reset), "reset-" + reset.Source.Line, extras.GetValueOrDefault(index));
				builder.AppendLine($"var parent{index} as item\nparent{index} = @lastobj\nvar ancestor{index} as item\nancestor{index} = @lastobj\nvar depth{index} as number\ndepth{index} = 0");
				builder.AppendLine($"while (not(isnull(@ancestor{index})) and not(isnull(@ancestor{index}.container)) and @depth{index} < 1000)\ndepth{index} = @depth{index} + 1\nancestor{index} = @ancestor{index}.container\nend while");
				builder.AppendLine($"depth{index} = @depth{index} + 1\nwhile (@depth{index} > {arguments[0]} and not(isnull(@parent{index})) and not(isnull(@parent{index}.container)))\ndepth{index} = @depth{index} - 1\nparent{index} = @parent{index}.container\nend while");
				// The source's P count and global-limit test is commented out. Do not invent either gate.
				builder.AppendLine($"if (not(isnull(@parent{index})) and @parent{index}.iscontainer)\nvar item{index} as item\nitem{index} = loaditem({prototype.Id})\n{command} = forceput(@item{index}, @parent{index})\nif (@{command})\nlastobj = @item{index}\nelse\ndeleteitem(@item{index})\nend if\nend if");
			}
			else if (reset.Opcode is "G" or "E" or "P")
			{
				builder.AppendLine($"{command} = @mobilenew");
				if (reset.Opcode is "G" or "E") roomObjectContext = false;
			}
			else builder.AppendLine($"{command} = @{lastCommand}");
			builder.AppendLine("end if");
			lastCommand = command;
		}
		if (objectLoopOpen) builder.AppendLine("end foreach");
		builder.AppendLine("return true");
		return builder.ToString();
	}
}
