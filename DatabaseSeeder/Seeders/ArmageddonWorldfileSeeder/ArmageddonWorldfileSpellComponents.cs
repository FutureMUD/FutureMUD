#nullable enable

using System;
using System.Linq;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private void InstallPlacedSpellComponents()
	{
		var placed = _source.Resets.Where(x => x.Opcode is "O" or "G" or "E" or "P")
			.Where(x => x.Arguments.Length > 1).Select(x => x.Arguments[1]).ToHashSet();
		foreach (var (id, effective) in _effectiveItems.Where(x => x.Value.Source.ItemType == 2 && placed.Contains(x.Value.Source.Vnum)))
		{
			// Source values are zero-based sphere and power. Only bind existing ranked native consumers.
			var sphere = effective.Values[0] switch { 1 => "alteration", 6 => "divination", 8 => "clerical", 10 => "teleportation", _ => null };
			if (sphere is null || effective.Values[1] is < 0 or > 6) continue;
			var maximum = sphere is "alteration" or "teleportation" ? 5 : 4;
			var rank = Math.Min(effective.Values[1], maximum);
			long? parent = null;
			var valid = true;
			for (var current = 0; current <= rank; current++)
			{
				var tagId = OwnedBehaviourId($"arm.material.{sphere}.rank{current}.tag", nameof(Tag));
				var tag = tagId is null ? null : _db.Tags.Find(tagId.Value);
				if (tag is null || tag.ParentId != parent)
				{
					valid = false;
					break;
				}
				parent = tag.Id;
			}
			if (!valid)
			{
				Adapt("spell component", effective.Source.Vnum, $"Missing or modified native {sphere} rank hierarchy; tags were not repaired or invented.");
				continue;
			}
			var proto = _items.Values.Concat(_resetItemVariants.Values).First(x => x.Id == id);
			if (!proto.GameItemProtosTags.Any(x => x.TagId == parent))
				proto.GameItemProtosTags.Add(new GameItemProtosTags { TagId = parent!.Value });
			Record("spellcomponent." + id, nameof(GameItemProto), id, proto.RevisionNumber,
				new { SourceVnum = effective.Source.Vnum, Sphere = sphere, SourceRank = effective.Values[1], NativeRank = rank });
		}
	}
}
