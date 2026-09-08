#nullable enable

using System;
using System.Linq;
using DatabaseSeeder.Seeders.CultureToolkit;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CultureToolkitInstallerPreflightTests
{
	[TestMethod]
	public void SkippingAllContentDoesNotCreateAnInstallationMarkerOrRequireUnusedPrerequisites()
	{
		using var context = Context();
		var result = CultureToolkitInstaller.Install(context, "antiquity", false, false, false);
		Assert.AreEqual(0, result.CultureIds.Count);
		Assert.AreEqual(0, context.SeederManagedRecords.Count());
		Assert.IsFalse(context.ChangeTracker.HasChanges());
		CultureToolkitInstaller.Install(context, "medieval", false, false, false);
	}

	[TestMethod]
	public void MissingLearnedLiteracyFailsBeforeSourceEvaluationOrWrites()
	{
		using var context = Context();
		var error = Assert.ThrowsException<InvalidOperationException>(() => CultureToolkitInstaller.Install(context, "medieval", true, true, true));
		StringAssert.Contains(error.Message, "Literacy");
		Assert.IsFalse(context.ChangeTracker.HasChanges());
	}

	private static FuturemudDatabaseContext Context() => new(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
		.UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
}
