#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonStockSpellInstallerTests
{

	[TestMethod]
	[TestCategory("Integration")]
	public void RestorativeMud_RejectsAnOverBudgetOwnedDoseBeforeInstallingStock()
	{
		using var db = Fixture(ArmageddonProgressionMode.Classic);
		var componentId = db.SeederManagedRecords.Single(x => x.StableKey == "arm.item.restorative_mud.component").LogicalId!.Value;
		var component = db.GameItemComponentProtos.Single(x => x.Id == componentId);
		var definition = XElement.Parse(component.Definition);
		definition.Element("NativeHealingDose")!.SetAttributeValue("budgetPerGrade", 11);
		component.Definition = definition.ToString(SaveOptions.DisableFormatting);
		db.SaveChanges();

		var installed = ArmageddonStockSpellInstaller.Install(ArmageddonInstallationSettingsTests.Factory(db));
		Assert.AreEqual(ArmageddonInstallStatus.Blocked, installed.Status);
		StringAssert.Contains(string.Join("\n", installed.Messages), "arm.item.restorative_mud");
	}
}
