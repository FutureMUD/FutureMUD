#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Framework;
using Db = MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class NpcArchiveReferencePolicyTests
{
	[DataTestMethod]
	[DataRow("<Effects />", true)]
	[DataRow(" ", true)]
	[DataRow("<Effects><Effect /></Effects>", false)]
	[DataRow("<Effects value='something' />", false)]
	[DataRow("<invalid", false)]
	[DataRow("<Definition />", false)]
	public void Effects_OnlyProvenEmptyContainer_PermitsCompaction(string value, bool expected) =>
		Assert.AreEqual(expected, NpcArchiveReferencePolicy.IsEmptyEffects(value));

	[DataTestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void CharacterConstructor_ArchivedOrMissingBody_RefusesBeforeWorldCalls(bool archived)
	{
		var world = new Mock<IFuturemud>(MockBehavior.Strict);
		Assert.ThrowsException<System.InvalidOperationException>(() => new MudSharp.Character.Character(new Db.Character
		{
			Id = 107, IsArchived = archived, BodyId = archived ? 209 : null
		}, world.Object));
		world.VerifyNoOtherCalls();
	}
}
