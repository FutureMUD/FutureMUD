#nullable enable

using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RacialMeritModelTests
{
	private static FuturemudDatabaseContext Context()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseMySql("server=localhost;database=unused;uid=unused", ServerVersion.Parse("8.0.36-mysql"))
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	[TestMethod]
	public void RacialMerits_UseUniqueCompositeIdentityAndCascadeOnlyAssociationRows()
	{
		using var context = Context();
		var join = context.Model.FindEntityType(typeof(RacesMerits))!;
		Assert.AreEqual("Races_Merits", join.GetTableName());
		CollectionAssert.AreEqual(new[] { nameof(RacesMerits.RaceId), nameof(RacesMerits.MeritId) },
			join.FindPrimaryKey()!.Properties.Select(x => x.Name).ToArray());
		Assert.AreEqual(2, join.GetForeignKeys().Count());
		Assert.IsTrue(join.GetForeignKeys().All(x => x.DeleteBehavior == DeleteBehavior.Cascade && x.IsRequired));
		Assert.IsTrue(join.GetForeignKeys().Any(x => x.PrincipalEntityType.ClrType == typeof(Race)));
		Assert.IsTrue(join.GetForeignKeys().Any(x => x.PrincipalEntityType.ClrType == typeof(Merit)));
		Assert.IsFalse(join.GetForeignKeys().Any(x => x.PrincipalEntityType.ClrType == typeof(PerceiverMerit)),
			"Intrinsic race definitions must remain separate from explicit character/body merit assignments.");
	}

	[TestMethod]
	public void RacialMerits_TrackRaceAndMeritNavigationsWithoutExplicitPerceiverGrants()
	{
		using var context = Context();
		var race = new Race { Id = 101, Name = "Test Race" };
		var merit = new Merit { Id = 201, Name = "Test Merit" };
		context.Attach(race);
		context.Attach(merit);
		var link = new RacesMerits { RaceId = race.Id, MeritId = merit.Id };
		context.RacesMerits.Add(link);
		Assert.AreSame(race, link.Race);
		Assert.AreSame(merit, link.Merit);
		Assert.IsTrue(race.RacesMerits.Contains(link));
		Assert.IsTrue(merit.RacesMerits.Contains(link));
		Assert.AreEqual(0, context.ChangeTracker.Entries<PerceiverMerit>().Count());
	}
}
