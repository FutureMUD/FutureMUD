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
public class VehicleRoutePersistenceModelTests
{
	private static FuturemudDatabaseContext CreateContext()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseMySql(
				"server=localhost;port=3306;database=dbo;uid=futuremud;password=unused",
				ServerVersion.Parse("8.0.36-mysql"))
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	[TestMethod]
	public void VehicleJourney_ServiceForeignKey_UsesStableServiceIdentityOnly()
	{
		using var context = CreateContext();
		var model = context.GetService<IDesignTimeModel>().Model;
		var service = model.FindEntityType(typeof(VehicleService));
		var journey = model.FindEntityType(typeof(VehicleJourney));
		Assert.IsNotNull(service);
		Assert.IsNotNull(journey);
		Assert.AreEqual(1, service.GetKeys().Count());
		CollectionAssert.AreEqual(
			new[] { nameof(VehicleService.Id) },
			service.FindPrimaryKey()!.Properties.Select(x => x.Name).ToArray());

		var serviceForeignKey = journey.GetForeignKeys()
			.Single(x => x.PrincipalEntityType.ClrType == typeof(VehicleService));
		CollectionAssert.AreEqual(
			new[] { nameof(VehicleJourney.VehicleServiceId) },
			serviceForeignKey.Properties.Select(x => x.Name).ToArray());
		var departureIndex = journey.GetIndexes().Single(x =>
			x.GetDatabaseName() == "UX_VehicleJourneys_Service_ScheduledDeparture");
		Assert.IsTrue(departureIndex.IsUnique);
		CollectionAssert.AreEqual(
			new[] { nameof(VehicleJourney.VehicleServiceId), nameof(VehicleJourney.ScheduledDeparture) },
			departureIndex.Properties.Select(x => x.Name).ToArray());
	}

	[TestMethod]
	public void VehicleRouteStep_PinsRouteCellTopologyAtBothEndpoints()
	{
		using var context = CreateContext();
		var step = context.GetService<IDesignTimeModel>().Model.FindEntityType(typeof(VehicleRouteStep));
		Assert.IsNotNull(step);
		Assert.IsNotNull(step.FindProperty(nameof(VehicleRouteStep.PinnedTopologyVersion)));
		Assert.IsNotNull(step.FindProperty(nameof(VehicleRouteStep.DestinationTopologyVersion)));
		var constraint = step.GetCheckConstraints()
			.Single(x => x.Name == "CK_VehicleRouteSteps_Positions");
		StringAssert.Contains(constraint.Sql, "`PinnedTopologyVersion`");
		StringAssert.Contains(constraint.Sql, "`DestinationTopologyVersion`");
	}
}
