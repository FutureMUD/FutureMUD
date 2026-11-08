#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private static readonly string[] ComponentSpheres =
	[
		"abjuration", "alteration", "conjuration", "enchantment", "illusion", "invocation",
		"divination", "necromancy", "clerical", "nature", "teleportation", "creation"
	];
	private readonly Dictionary<(string Sphere, int Rank), Tag> _componentRanks = new();

	private void InstallPlacedSpellComponents()
	{
		// Bind definitions, not only opening placements: crafts, shops and later loads use these too.
		foreach (var (id, effective) in _effectiveItems.Where(x => x.Value.Source.ItemType == 2))
		{
			if (effective.Values[0] < 0 || effective.Values[0] >= ComponentSpheres.Length || effective.Values[1] is < 0 or > 6)
			{
				Adapt("spell component", effective.Source.Vnum, "Invalid sphere or rank; retained as an unbound definition.");
				continue;
			}
			var sphere = ComponentSpheres[effective.Values[0]];
			var rank = effective.Values[1];
			var tag = ComponentRank(sphere, rank);
			var valid = tag is not null;
			long? parent = tag?.Id;
			if (!valid)
			{
				Adapt("spell component", effective.Source.Vnum, $"Missing or modified native {sphere} rank tags; existing tags were not repaired.");
				continue;
			}
			var proto = _items.Values.Concat(_resetItemVariants.Values).First(x => x.Id == id);
			if (!proto.GameItemProtosTags.Any(x => x.TagId == parent))
				proto.GameItemProtosTags.Add(new GameItemProtosTags { TagId = parent!.Value });
			Record("spellcomponent." + id, nameof(GameItemProto), id, proto.RevisionNumber,
				new { SourceVnum = effective.Source.Vnum, Sphere = sphere, SourceRank = effective.Values[1], NativeRank = rank });
		}
	}
	private Tag? ComponentRank(string sphere, int rank)
	{
		var hierarchical = sphere is "alteration" or "divination" or "clerical" or "teleportation";
		long? parent = null;
		Tag? tag = null;
		for (var current = 0; current <= rank; current++)
		{
			if (!_componentRanks.TryGetValue((sphere, current), out tag))
			{
				var key = $"arm.material.{sphere}.rank{current}.tag";
				var id = OwnedBehaviourId(key, nameof(Tag));
				tag = id is null ? null : _db.Tags.Find(id.Value);
				if (tag is null && id is null && current >= 5)
				{
					tag = new Tag { Id = Next<Tag>(), Name = $"{sphere} material rank {current}", ParentId = hierarchical ? parent : null };
					_db.Tags.Add(tag);
					Record(key, nameof(Tag), tag.Id, null, new { Sphere = sphere, Rank = current });
				}
				if (tag is not null) _componentRanks.Add((sphere, current), tag);
			}
			if (tag is null || tag.ParentId != (hierarchical ? parent : null)) return null;
			parent = tag.Id;
		}
		return tag;
	}

}
