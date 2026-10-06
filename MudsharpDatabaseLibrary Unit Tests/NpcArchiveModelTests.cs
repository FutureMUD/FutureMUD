#nullable enable

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class NpcArchiveModelTests
{
	[TestMethod]
	public void Model_PrimaryBody_RequiresBodyUnlessArchivedAndCannotCascadeCanonicalHistory()
	{
		using var context = new FuturemudDatabaseContextFactory().CreateDbContext([]);
		var character = context.GetService<IDesignTimeModel>().Model.FindEntityType(typeof(Character))!;
		Assert.IsTrue(character.FindProperty(nameof(Character.BodyId))!.IsNullable);
		Assert.IsTrue(character.FindProperty(nameof(Character.IsArchived))!.IsConcurrencyToken);
		Assert.AreEqual(false, character.FindProperty(nameof(Character.IsArchived))!.GetDefaultValue());
		Assert.AreEqual(DeleteBehavior.Restrict, character.GetForeignKeys().Single(x =>
			x.Properties.Single().Name == nameof(Character.BodyId)).DeleteBehavior);
		Assert.IsTrue(character.GetCheckConstraints().Single(x => x.Name == "CK_Characters_BodyOrArchive")
			.Sql.Contains("`BodyId` IS NOT NULL OR `IsArchived` = 1", StringComparison.Ordinal));
	}

	[TestMethod]
	public void Model_Archive_IsBoundedUniqueAndPreservesCanonicalIdentity()
	{
		using var context = new FuturemudDatabaseContextFactory().CreateDbContext([]);
		var archive = context.GetService<IDesignTimeModel>().Model.FindEntityType(typeof(CharacterArchive))!;
		Assert.AreEqual(DeleteBehavior.Restrict, archive.GetForeignKeys().Single().DeleteBehavior);
		Assert.AreEqual(1024, archive.FindProperty(nameof(CharacterArchive.DisplayName))!.GetMaxLength());
		Assert.AreEqual(65535, archive.FindProperty(nameof(CharacterArchive.WoundHistory))!.GetMaxLength());
		Assert.IsTrue(archive.GetIndexes().Single(x => x.Properties.Single().Name == nameof(CharacterArchive.LifecycleId)).IsUnique);
		Assert.IsFalse(archive.GetForeignKeys().Any(x => x.Properties.Any(p => p.Name == nameof(CharacterArchive.OriginalBodyId))));
	}
}
