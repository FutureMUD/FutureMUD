#nullable enable
using System;
using System.Globalization;
using System.Linq;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private void InstallPlayerTailoringTools()
	{
		var skill = ArmageddonSkillPackage.Resolve(_db)["arm.skill.cloth_working"];
		var tag = new Tag { Id = Next<Tag>(), Name = "Armageddon Sewing Tool" };
		_db.Tags.Add(tag);
		// These descriptions explicitly identify sewing needles, rather than tattoo needles, picks or darts.
		foreach (var vnum in new[] { 21097, 46105 })
		{
			if (!_items.TryGetValue(vnum, out var item)) continue;
			item.GameItemProtosTags.Add(new GameItemProtosTags
			{
				GameItemProtoId = item.Id, GameItemProtoRevisionNumber = item.RevisionNumber,
				TagId = tag.Id, Tag = tag, GameItemProto = item
			});
		}
		foreach (var pair in new[] { ("TailoringTraitId", skill.Id), ("TailoringToolTagId", tag.Id) })
		{
			var setting = _db.StaticConfigurations.Local.FirstOrDefault(x => x.SettingName == pair.Item1) ?? _db.StaticConfigurations.Find(pair.Item1);
			if (setting is null) _db.StaticConfigurations.Add(new StaticConfiguration { SettingName = pair.Item1, Definition = pair.Item2.ToString(System.Globalization.CultureInfo.InvariantCulture) });
			else setting.Definition = pair.Item2.ToString(System.Globalization.CultureInfo.InvariantCulture);
		}
		Record("tailoring.player-tools", nameof(Tag), tag.Id, null, new { Skill = skill.Id, Tools = new[] { 21097, 46105 }, DurationSeconds = 30 });
	}
}
