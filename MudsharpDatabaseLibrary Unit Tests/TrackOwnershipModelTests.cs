#nullable enable

using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class TrackOwnershipModelTests
{
	[TestMethod]
	public void TrackModel_SupportsExactlyOneCharacterOrVehicleOwner()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseMySql(
				"server=localhost;port=3306;database=dbo;uid=futuremud;password=unused",
				ServerVersion.Parse("8.0.36-mysql"))
			.Options;
		using var context = new FuturemudDatabaseContext(options);

		var entityType = context.GetService<IDesignTimeModel>().Model.FindEntityType(typeof(Track));
		Assert.IsNotNull(entityType);
		Assert.IsTrue(entityType.FindProperty(nameof(Track.CharacterId))!.IsNullable);
		Assert.IsTrue(entityType.FindProperty(nameof(Track.BodyPrototypeId))!.IsNullable);
		Assert.IsTrue(entityType.FindProperty(nameof(Track.VehicleId))!.IsNullable);

		var vehicleForeignKey = entityType
			.GetForeignKeys()
			.Single(x => x.PrincipalEntityType.ClrType == typeof(Vehicle));
		Assert.AreEqual(nameof(Track.VehicleId), vehicleForeignKey.Properties.Single().Name);
		Assert.AreEqual(nameof(Vehicle.Tracks), vehicleForeignKey.PrincipalToDependent?.Name);
		var characterForeignKey = entityType
			.GetForeignKeys()
			.Single(x => x.PrincipalEntityType.ClrType == typeof(Character));
		Assert.AreEqual(nameof(Track.CharacterId), characterForeignKey.Properties.Single().Name);
		var bodyPrototypeForeignKey = entityType
			.GetForeignKeys()
			.Single(x => x.PrincipalEntityType.ClrType == typeof(BodyProto));
		Assert.AreEqual(nameof(Track.BodyPrototypeId), bodyPrototypeForeignKey.Properties.Single().Name);

		var ownerConstraint = entityType
			.GetCheckConstraints()
			.Single(x => x.Name == "CK_Tracks_Owner");
		StringAssert.Contains(ownerConstraint.Sql, "`VehicleId` IS NULL");
		StringAssert.Contains(ownerConstraint.Sql, "`CharacterId` IS NOT NULL");
		StringAssert.Contains(ownerConstraint.Sql, "`BodyPrototypeId` IS NOT NULL");
		StringAssert.Contains(ownerConstraint.Sql, "`VehicleId` IS NOT NULL");
	}
}
