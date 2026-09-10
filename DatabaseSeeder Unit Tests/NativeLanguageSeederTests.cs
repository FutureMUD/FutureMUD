using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseSeeder.Seeders.CultureToolkit;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AccentRole = MudSharp.Communication.Language.AccentRole;
using MudSharp.Database;
using MudSharp.Models;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public class NativeLanguageSeederTests
{
	[TestMethod]
	public void ClearForeignSourceAssociationsAreSeededAndBuilderOverridesSurvive()
	{
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
		var english = new Language { Name = "English" };
		var french = new Language { Name = "French" };
		var accent = new Accent { Name = "French", Group = "european", Language = english };
		context.AddRange(english, french, accent);
		context.SaveChanges();
		var conflicts = new List<string>();
		CultureStockAccentRoles.ApplyLegacy(context, accent, false, conflicts);
		context.SaveChanges();
		Assert.AreEqual((int)AccentRole.Foreign, accent.Role);
		Assert.AreEqual(french.Id, accent.AssociatedLanguages.Single().Id);
		Assert.AreEqual(0, conflicts.Count);
		accent.Role = (int)AccentRole.Native;
		accent.AssociatedLanguages.Clear();
		context.SaveChanges();
		for (var i = 0; i < 2; i++) CultureStockAccentRoles.ApplyLegacy(context, accent, false, conflicts);
		Assert.AreEqual((int)AccentRole.Native, accent.Role);
		Assert.AreEqual(0, accent.AssociatedLanguages.Count);
		Assert.IsTrue(conflicts.Any(x => x.Contains("builder edit")));
		Assert.AreEqual(1, context.SeederManagedRecords.Local.Count);
	}

	[TestMethod]
	public void GenericLearnersAndAmbiguousSourceRegionsDoNotInventLanguageBindings()
	{
		Assert.AreEqual(AccentRole.Fallback, CultureStockAccentRoles.Role("English", "Foreign", "Foreign"));
		Assert.AreEqual(AccentRole.Native, CultureStockAccentRoles.Role("English", "Welsh", "british"));
		Assert.AreEqual(AccentRole.Foreign, CultureStockAccentRoles.Role("English", "southeast asian", "asian"));
		Assert.AreEqual(0, CultureStockAccentRoles.Associations("English", "southeast asian").Count);
		Assert.AreEqual("Arabic", CultureStockAccentRoles.Associations("English", "middle-eastern").Single());
	}
}
