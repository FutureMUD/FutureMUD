#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseSeeder.Seeders.CultureToolkit;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CultureToolkitHeritageSourceTests
{
	[TestMethod]
	public void GreekRomanOverlayNeverAliasesAncientCityRomanIdentity()
	{
		using var ancient = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
		using var later = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
		ancient.Add(new Ethnicity { Name = "Roman" });
		later.Add(new Ethnicity { Name = "Roman" });
		ancient.SaveChanges();
		later.SaveChanges();
		var whole = new CultureToolkitCatalogue().Compose("renaissance");
		var pack = whole with { Ethnicities = whole.Ethnicities.Where(x => CultureToolkitCatalogue.Text(x, "key") == "ethnicity.greek-roman").ToArray() };
		var stages = new Dictionary<string, FuturemudDatabaseContext> { ["earthantiquity"] = ancient, ["earthrenaissanceeurope"] = later };
		var overlay = CultureToolkitHeritageSources.Describe(pack, stages, new Ethnicity()).Single(x => x.Overlay.HasValue);
		Assert.AreEqual("earthrenaissanceeurope", overlay.Module);
		Assert.IsFalse(overlay.Aliases.Any(x => x.Contains("earthantiquity")));
	}
}
