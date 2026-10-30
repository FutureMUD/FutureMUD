#nullable enable
extern alias EngineCompiler;

using System;
using System.Linq;
using MudSharp.Database;
using MudSharp.Models;
using EngineCompiler::MudSharp.Celestial.Authored;

namespace DatabaseSeeder.Seeders;

internal static class ArmageddonWorldfileLighting
{
	internal static double AddedLight(int zone, int sector, int flags)
	{
		var bits = unchecked((uint)flags);
		if ((bits & 1u) != 0) return 0; // Explicit authored darkness remains meaningful.
		// Built rooms and urban streets in cities/settlements; never illuminate surrounding wild terrain.
		var settlement = zone is 8 or 9 or 20 or 21 or 24 or 31 or 37 or 45 or 48 or 49 or 50 or 64;
		if (sector == 1 || sector == 0 && settlement) return 75;
		return (bits & (1u << 27)) != 0 ? 25 : 0;
	}

	internal static bool AttachSky(FuturemudDatabaseContext db, long shardId)
	{
		var sky = db.Celestials.Where(x => x.CelestialType == "ScriptedSun" || x.CelestialType == "ScriptedMoon")
			.AsEnumerable().Select(x => (Row: x, Definition: AuthoredCelestialFormat.Parse(x.Definition)))
			.Where(x => x.Definition.SeederPreset is "ArmageddonSun" or "ArmageddonJihae" or "ArmageddonLirathu").ToArray();
		if (sky.Length == 0) return false;
		var calendar = CelestialSeeder.RequireArmageddonCalendar(db);
		if (sky.Length != 3 || sky.Select(x => x.Definition.SeederPreset).Distinct().Count() != 3 ||
		    sky.Any(x => x.Row.FeedClockId != calendar.FeedClockId || x.Definition.CalendarId != calendar.Id))
			throw new InvalidOperationException("The worldfile sky needs the complete Armageddon celestial package on its native calendar and clock.");
		var ids = sky.Select(x => x.Row.Id).ToArray();
		if (db.ShardsCelestials.Any(x => x.ShardId == shardId && !ids.Contains(x.CelestialId)))
			throw new InvalidOperationException("The worldfile shard already has a different sky. Resolve its celestial assignments before importing Armageddon.");
		foreach (var id in ids)
			if (!db.ShardsCelestials.Any(x => x.ShardId == shardId && x.CelestialId == id))
				db.ShardsCelestials.Add(new ShardsCelestials { ShardId = shardId, CelestialId = id });
		db.SaveChanges();
		return true;
	}
}
