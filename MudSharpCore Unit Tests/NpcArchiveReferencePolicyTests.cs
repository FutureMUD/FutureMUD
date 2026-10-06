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
	[DataRow("<Effect><Target>107</Target></Effect>")]
	[DataRow("<Effect owner='107' />")]
	[DataRow("<Definition><OriginalBody>209</OriginalBody></Definition>")]
	[DataRow("{\"canonical\":107}")]
	[DataRow("{\"canonical\":\"\\u0031\\u0030\\u0037\"}")]
	[DataRow("{\"canonical\":1.07e2}")]
	[DataRow("{malformed")]
	[DataRow("[107]")]
	[DataRow("107,303,404")]
	[DataRow("<invalid")]
	public void Reference_IdentityBodyOrUncertainty_Holds(string value) =>
		Assert.IsTrue(NpcArchiveReferencePolicy.HasReferenceOrUncertainty(value, 107, 209));

	[DataTestMethod]
	[DataRow("")]
	[DataRow("<Effects />")]
	[DataRow("<Definition><Bodypart>9</Bodypart><Grade>3</Grade></Definition>")]
	[DataRow("1107 2090 10.7 2.09")]
	[DataRow("<Definition><A>10</A><B>7</B></Definition>")]
	public void Reference_NoMatchingTokens_IsClear(string value) =>
		Assert.IsFalse(NpcArchiveReferencePolicy.HasReferenceOrUncertainty(value, 107, 209));

	[TestMethod]
	public void Reference_InstanceAndWoundIds_AreRetained() =>
		Assert.IsTrue(NpcArchiveReferencePolicy.HasReferenceOrUncertainty("<Reference><Wound>303</Wound></Reference>", 107, 209, 303));

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
