using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp_Unit_Tests.PerceptionEngine;

[TestClass]
public class EmoteTokenTests
{
	[TestMethod]
	public void ParseFor_NonSelfTokenTargetIsViewer_RendersShortDescription()
	{
		var viewer = new Mock<IPerceiver>();
		var oldBody = new Mock<IPerceivable>();
		var newBody = new Mock<IPerceivable>();
		newBody.Setup(x => x.IsSelf(viewer.Object)).Returns(true);
		newBody
			.Setup(x => x.HowSeen(
				viewer.Object,
				false,
				DescriptionType.Short,
				true,
				It.Is<PerceiveIgnoreFlags>(flags => flags.HasFlag(PerceiveIgnoreFlags.IgnoreSelf))))
			.Returns("a silver wolf");

		var emote = new Emote("into ^1", viewer.Object, oldBody.Object, newBody.Object);

		Assert.AreEqual("into a silver wolf", emote.ParseFor(viewer.Object));
		newBody.Verify(x => x.HowSeen(
			viewer.Object,
			false,
			DescriptionType.Short,
			true,
			It.Is<PerceiveIgnoreFlags>(flags => flags.HasFlag(PerceiveIgnoreFlags.IgnoreSelf))), Times.Once);
	}

	[TestMethod]
	public void ParseFor_NonSelfPossessiveTokenTargetIsViewer_RendersPossessiveDescription()
	{
		var viewer = new Mock<IPerceiver>();
		var oldBody = new Mock<IPerceivable>();
		var newBody = new Mock<IPerceivable>();
		newBody.Setup(x => x.IsSelf(viewer.Object)).Returns(true);
		newBody
			.Setup(x => x.HowSeen(
				viewer.Object,
				false,
				DescriptionType.Possessive,
				true,
				It.Is<PerceiveIgnoreFlags>(flags => flags.HasFlag(PerceiveIgnoreFlags.IgnoreSelf))))
			.Returns("a silver wolf's");

		var emote = new Emote("beside ^1's shadow", viewer.Object, oldBody.Object, newBody.Object);

		Assert.AreEqual("beside a silver wolf's shadow", emote.ParseFor(viewer.Object));
		newBody.Verify(x => x.HowSeen(
			viewer.Object,
			false,
			DescriptionType.Possessive,
			true,
			It.Is<PerceiveIgnoreFlags>(flags => flags.HasFlag(PerceiveIgnoreFlags.IgnoreSelf))), Times.Once);
	}
}
