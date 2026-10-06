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
public class SpellLifecycleModelTests
{
	[TestMethod]
	public void Model_Provenance_DoesNotCascadeWithCanonicalIdentityOrPhysicalEntities()
	{
		using var context = new FuturemudDatabaseContextFactory().CreateDbContext([]);
		var model = context.GetService<IDesignTimeModel>().Model;
		var lifecycle = model.FindEntityType(typeof(MagicSpellLifecycle))!;
		Assert.AreEqual(0, lifecycle.GetForeignKeys().Count());
		Assert.IsTrue(lifecycle.FindProperty(nameof(MagicSpellLifecycle.Version))!.IsConcurrencyToken);
		Assert.AreEqual(2048, lifecycle.FindProperty(nameof(MagicSpellLifecycle.Provenance))!.GetMaxLength());
		foreach (var name in new[] { nameof(MagicSpellLifecycle.RemainsRemovalRequestedUtc),
			nameof(MagicSpellLifecycle.RemainsNotificationAttemptedUtc), nameof(MagicSpellLifecycle.RemainsNotificationCompletedUtc) })
		{
			var property = lifecycle.FindProperty(name)!;
			Assert.IsTrue(property.IsNullable); Assert.AreEqual("datetime(6)", property.GetColumnType());
		}
		Assert.IsTrue(lifecycle.GetIndexes().Any(x => x.Properties.Select(p => p.Name)
			.SequenceEqual([nameof(MagicSpellLifecycle.State), nameof(MagicSpellLifecycle.DeadlineUtc)])));
		var entity = model.FindEntityType(typeof(MagicSpellOwnedEntity))!;
		CollectionAssert.AreEqual(new[] { nameof(MagicSpellOwnedEntity.Kind), nameof(MagicSpellOwnedEntity.EntityId) },
			entity.FindPrimaryKey()!.Properties.Select(x => x.Name).ToArray());
		Assert.AreEqual(DeleteBehavior.Restrict, entity.GetForeignKeys().Single().DeleteBehavior);
	}
}
