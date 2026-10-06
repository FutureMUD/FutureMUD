#nullable enable
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using Moq;
using DatabaseSeeder.Seeders;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonMagicInstallerTests
{
	[TestMethod]
	public void NativeRegistrySerializedLightInstallsWithSupportedPersistedIdentity()
	{
		var (db, plan) = Fixture(); using (db)
		{
			var light = db.GameItemComponentProtos.Include(x => x.EditableItem).Single(x => x.Id == 3);
			Assert.AreEqual("Prog Light", light.Type);
			Assert.IsInstanceOfType(new GameItemComponentManager().GetProto(light, new Mock<IFuturemud>().Object), typeof(ProgLightGameItemComponentProto));
			Installed(Run(db, plan));
			Assert.AreEqual(21, db.SeederManagedRecords.Count(x => x.Seeder == ArmageddonMagicInstaller.Package));
		}
	}

	[TestMethod]
	public void UnsupportedLightHelpLabelBlocksBeforeAnyPackageWrites()
	{
		var (db, plan) = Fixture(); using (db)
		{
			var light = db.GameItemComponentProtos.Single(x => x.Id == 3);
			light.Type = "ProgLight"; db.SaveChanges();
			Assert.IsNull(new GameItemComponentManager().GetProto(light, new Mock<IFuturemud>().Object));
			var result = Run(db, plan);
			Assert.AreEqual(ArmageddonInstallStatus.Blocked, result.Status);
			StringAssert.Contains(string.Join(" ", result.Messages), "Prog Light");
			Assert.AreEqual(0, db.SeederManagedRecords.Count());
			Assert.AreEqual(0, db.MagicSpells.Count());
			Assert.AreEqual(3, db.GameItemComponentProtos.Count());
		}
	}
}
