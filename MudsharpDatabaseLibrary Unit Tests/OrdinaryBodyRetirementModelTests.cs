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
public class OrdinaryBodyRetirementModelTests
{
	[TestMethod]
	public void Model_OrdinaryRetirement_ExactBodyHasOneOwnerAndCannotDeleteCanonicalIdentity()
	{
		using var context = new FuturemudDatabaseContextFactory().CreateDbContext([]);
		var model = context.GetService<IDesignTimeModel>().Model;
		var retirement = model.FindEntityType(typeof(CharacterBodyRetirement))!;
		CollectionAssert.AreEqual(new[] { nameof(CharacterBodyRetirement.BodyId) },
			retirement.FindPrimaryKey()!.Properties.Select(x => x.Name).ToArray());
		Assert.AreEqual(ValueGenerated.Never, retirement.FindProperty(nameof(CharacterBodyRetirement.BodyId))!.ValueGenerated);
		Assert.AreEqual(2, retirement.GetForeignKeys().Count());
		Assert.AreEqual(DeleteBehavior.Restrict, retirement.GetForeignKeys()
			.Single(x => x.PrincipalEntityType.ClrType == typeof(Character)).DeleteBehavior);
		Assert.AreEqual(DeleteBehavior.Cascade, retirement.GetForeignKeys()
			.Single(x => x.PrincipalEntityType.ClrType == typeof(Body)).DeleteBehavior);
		Assert.IsFalse(retirement.FindProperty(nameof(CharacterBodyRetirement.RetiredUtc))!.IsNullable);
		Assert.IsFalse(model.FindEntityType(typeof(Character))!.GetForeignKeys()
			.Any(x => x.PrincipalEntityType == retirement));
	}
}
