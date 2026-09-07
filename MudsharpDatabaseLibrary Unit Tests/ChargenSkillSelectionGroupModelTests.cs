#nullable enable

using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;

namespace MudsharpDatabaseLibrary_Unit_Tests;

[TestClass]
public class ChargenSkillSelectionGroupModelTests
{
	[TestMethod]
	public void SG10_GroupModel_EnforcesStableIdentityAndMembershipKeys()
	{
		using var context = new FuturemudDatabaseContextFactory().CreateDbContext([]);
		var group = context.Model.FindEntityType(typeof(ChargenSkillSelectionGroup))!;
		Assert.IsTrue(group.GetIndexes().Any(x => x.IsUnique && x.Properties.SingleOrDefault()?.Name == "StableKey"));
		var member = context.Model.FindEntityType(typeof(ChargenSkillSelectionGroupMember))!;
		CollectionAssert.AreEqual(new[] { "GroupId", "TraitDefinitionId" }, member.FindPrimaryKey()!.Properties.Select(x => x.Name).ToArray());
		Assert.IsTrue(member.GetForeignKeys().All(x => x.DeleteBehavior == DeleteBehavior.Restrict));
		Assert.IsFalse(new ChargenSkillSelectionGroup().Enabled);
	}
}
