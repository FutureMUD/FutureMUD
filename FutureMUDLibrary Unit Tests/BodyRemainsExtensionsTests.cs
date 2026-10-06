#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;

namespace MudSharp_Unit_Tests;

[TestClass]
public class BodyRemainsExtensionsTests
{
	[DataTestMethod]
	[DataRow(0, false)]
	[DataRow(1, false)]
	[DataRow(2, false)]
	[DataRow(3, false)]
	[DataRow(4, true)]
	[DataRow(5, true)]
	public void GetOriginalCharacterWithMatchingBody_RequiresResolvedFinalMatchingBody(int state, bool valid)
	{
		var corpse = new Mock<ICorpse>(); var owner = new Mock<ICharacter>(); var body = new Mock<IBody>(); var current = new Mock<IBody>();
		body.SetupGet(x => x.Id).Returns(17); current.SetupGet(x => x.Id).Returns(state == 4 ? 17 : 18);
		owner.SetupGet(x => x.Body).Returns(state == 5 ? body.Object : current.Object);
		corpse.SetupGet(x => x.RepresentsFinalCharacterDeath).Returns(state != 3);
		corpse.SetupGet(x => x.OriginalCharacter).Returns(state == 0 ? null! : owner.Object);
		corpse.SetupGet(x => x.OriginalBody).Returns(state == 1 ? null! : body.Object);
		Assert.AreEqual(valid ? owner.Object : null, corpse.Object.GetOriginalCharacterWithMatchingBody());
	}

	[TestMethod]
	public void TargetActorOrCorpseBody_UnresolvedCorpse_RefusesPhysicalBodyTarget()
	{
		var targeter = new Mock<ITarget>(); var corpse = new Mock<ICorpse>();
		targeter.Setup(x => x.TargetCorpse("corpse", PerceiveIgnoreFlags.None)).Returns(corpse.Object);
		Assert.IsNull(targeter.Object.TargetActorOrCorpseBody("corpse"));
	}
}
