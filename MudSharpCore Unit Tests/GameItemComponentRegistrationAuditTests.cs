#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.GameItems;
using System;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class GameItemComponentRegistrationAuditTests
{
	[TestMethod]
	public void RegistrationAuditEntries_LiveRegistry_IsCompleteAndDeterministic()
	{
		var manager = new GameItemComponentManager();
		var entries = manager.RegistrationAuditEntries;

		Assert.AreEqual(244, entries.Count);
		Assert.AreEqual(109, entries.Count(x => x.Technology == GameItemComponentTypeTechnology.Modern));
		Assert.AreEqual(18, entries.Count(x => x.Technology == GameItemComponentTypeTechnology.Futuristic));
		Assert.AreEqual(117, entries.Count(x => x.Technology == GameItemComponentTypeTechnology.None));
		Assert.AreEqual(entries.Count,
			entries.Select(x => x.CanonicalDatabaseType).Distinct(StringComparer.OrdinalIgnoreCase).Count());
		Assert.IsTrue(entries.All(x => x.HasDatabaseLoader));
		Assert.IsTrue(entries.All(x => x.HasHelp));
		CollectionAssert.AreEqual(
			entries.Select(x => x.CanonicalDatabaseType).OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToList(),
			entries.Select(x => x.CanonicalDatabaseType).ToList());
	}

	[TestMethod]
	public void RegistrationAuditEntries_RecordsAliasesCapabilitiesAndRequirements()
	{
		var entries = new GameItemComponentManager().RegistrationAuditEntries;

		var refrigerator = entries.Single(x => x.CanonicalDatabaseType == "Refrigerator");
		Assert.AreEqual("refrigerator", refrigerator.PrimaryBuilderType);
		CollectionAssert.Contains(refrigerator.BuilderAliases.ToList(), "fridge");
		Assert.AreEqual(GameItemComponentTypeTechnology.Modern, refrigerator.Technology);

		var ammoClip = entries.Single(x => x.CanonicalDatabaseType == "AmmoClip");
		CollectionAssert.Contains(ammoClip.ExclusiveCapabilities.ToList(), "IAmmoClip");
		CollectionAssert.Contains(ammoClip.ExclusiveCapabilities.ToList(), "IContainer");

		var pinPull = entries.Single(x => x.CanonicalDatabaseType == "PinPullDetonator");
		CollectionAssert.Contains(pinPull.RequiredSiblingCapabilities.ToList(), "IDetonatable");
		Assert.IsFalse(pinPull.HasContextDependentRequirements);

		var attachment = entries.Single(x => x.CanonicalDatabaseType == "FirearmAttachment");
		Assert.IsTrue(attachment.HasContextDependentRequirements);
	}
}
