#nullable enable

using System;
using System.Linq;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders.Utilities;

internal static class WeatherResidueSeeder
{
	/// <summary>Uses native solid/residue metadata; never replaces an existing builder material.</summary>
	internal static Material RequireDust(FuturemudDatabaseContext db)
	{
		var existing = db.Materials.FirstOrDefault(x => x.Name == "dust");
		if (existing is not null) return existing;
		var sand = db.Materials.FirstOrDefault(x => x.Name == "sand")
			?? throw new InvalidOperationException("Weather dust requires the core sand material.");
		var dust = (Material)db.Entry(sand).CurrentValues.ToObject();
		dust.Id = 0;
		dust.Name = "dust";
		dust.MaterialDescription = "fine dust";
		dust.ResidueSdesc = "(dusty)";
		dust.ResidueDesc = "It is coated in a layer of fine dust.";
		db.Materials.Add(dust);
		db.SaveChanges();
		return dust;
	}
}
