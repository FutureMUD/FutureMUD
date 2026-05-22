using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

namespace MudSharp_Unit_Tests;

[TestClass]
public class TargetExtensionsTests
{
	[TestMethod]
	public void TargetActorOrCorpseBody_CorpseTarget_UsesCorpseBodyNotOriginalCharactersCurrentBody()
	{
		var targeter = new Mock<ITarget>();
		var originalCharacterCurrentBody = new Mock<IBody>();
		var corpseBody = new Mock<IBody>();
		var originalCharacter = new Mock<ICharacter>();
		var corpseItem = new Mock<IGameItem>();
		var corpse = new Mock<ICorpse>();

		originalCharacter.SetupGet(x => x.Body).Returns(originalCharacterCurrentBody.Object);
		corpse.SetupGet(x => x.Parent).Returns(corpseItem.Object);
		corpse.SetupGet(x => x.Body).Returns(corpseBody.Object);
		corpse.SetupGet(x => x.OriginalCharacter).Returns(originalCharacter.Object);
		targeter.Setup(x => x.TargetActor("corpse", PerceiveIgnoreFlags.None)).Returns((ICharacter)null);
		targeter.Setup(x => x.TargetCorpse("corpse", PerceiveIgnoreFlags.None)).Returns(corpse.Object);

		var result = targeter.Object.TargetActorOrCorpseBody("corpse");

		Assert.IsNotNull(result);
		Assert.IsTrue(result.IsCorpse);
		Assert.AreSame(corpseBody.Object, result.Body);
		Assert.AreSame(corpseItem.Object, result.Perceivable);
		Assert.AreSame(originalCharacter.Object, result.Character);
	}
}
