#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Models;
using MudSharp.TimeAndDate.Time;
using RuntimeClock = MudSharp.TimeAndDate.Time.Clock;

namespace MudSharp_Unit_Tests;

[TestClass]
[DoNotParallelize]
public class ClockCreationSecurityTests
{
	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void ClockCreation_NewOrClonedClock_PersistsReloadableDefinitionFromFirstInsert(bool clone)
	{
		var writes = new ClockDefinitionWrites();
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString()).AddInterceptors(writes).Options);
		var contextProperty = typeof(FMDB).GetProperty(nameof(FMDB.Context), BindingFlags.Public | BindingFlags.Static)!;
		var previous = contextProperty.GetValue(null);
		contextProperty.SetValue(null, context);
		try
		{
			using var scope = new FMDB();
			var world = new Mock<IFuturemud>();
			world.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
			var created = new RuntimeClock(world.Object, "New Clock", "NC");
			var clock = clone ? created.Clone("Copy Clock", "CC") : created;
			context.SaveChanges();
			var stored = context.Clocks.Include(x => x.Timezones).AsNoTracking().Single(x => x.Id == clock.Id);
			var reloaded = new RuntimeClock(stored, world.Object);
			Assert.AreEqual(clock.Alias, reloaded.Alias);
			Assert.AreEqual("day", reloaded.SaveToXml().Element("CrudeTimeIntervals")!.Elements().Single().Attribute("text")!.Value);
			Assert.IsNotNull(reloaded.PrimaryTimezone);
			Assert.IsTrue(writes.Definitions.Count >= (clone ? 2 : 1));
			foreach (var definition in writes.Definitions)
			{
				var loaded = new RuntimeClock(XElement.Parse(definition), world.Object);
				var range = loaded.SaveToXml().Element("CrudeTimeIntervals")!.Elements().Single();
				Assert.AreEqual("0", range.Attribute("Lower")!.Value);
				Assert.AreEqual(loaded.HoursPerDay.ToString(System.Globalization.CultureInfo.InvariantCulture), range.Attribute("Upper")!.Value);
			}
		}
		finally
		{
			contextProperty.SetValue(null, previous);
		}
	}

	private sealed class ClockDefinitionWrites : SaveChangesInterceptor
	{
		public List<string> Definitions { get; } = [];
		public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
		{
			Definitions.AddRange(eventData.Context!.ChangeTracker.Entries<MudSharp.Models.Clock>()
				.Where(x => x.State is EntityState.Added or EntityState.Modified)
				.Select(x => x.Entity.Definition));
			return result;
		}
	}
}
