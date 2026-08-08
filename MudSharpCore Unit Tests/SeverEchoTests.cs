#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.Implementations;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SeverEchoTests
{
	[TestMethod]
	public void SeverEchoText_LocalisedPart_NamesActualPart()
	{
		var eye = CreatePart("Right Eye", "right eye", true, false);
		var headRoot = CreatePart("Neck", "neck", true, true);
		var limb = CreateLimb("Head", headRoot.Object);

		Assert.AreEqual("$0's right eye is severed!", Body.SeverEchoText(eye.Object, limb.Object));
	}

	[TestMethod]
	public void SeverEchoText_LimbPart_NamesContainingLimb()
	{
		var elbow = CreatePart("Right Elbow", "right elbow", true, true);
		var armRoot = CreatePart("Right Upper Arm", "right upper arm", true, true);
		var limb = CreateLimb("Right Arm", armRoot.Object);

		Assert.AreEqual("$0's right arm is severed at the right elbow!", Body.SeverEchoText(elbow.Object, limb.Object));
	}

	[TestMethod]
	public void SeverEchoText_NeckPart_NamesContainingHead()
	{
		var neck = CreatePart("Neck", "neck", true, true);
		var limb = CreateLimb("Head", neck.Object);

		Assert.AreEqual("$0's head is severed at the neck!", Body.SeverEchoText(neck.Object, limb.Object));
	}

	private static Mock<IBodypart> CreatePart(string name, string description, bool significant,
		bool useLimbSeverDescription)
	{
		var part = new Mock<IBodypart>();
		part.SetupGet(x => x.Name).Returns(name);
		part.SetupGet(x => x.Significant).Returns(significant);
		part.SetupGet(x => x.UseLimbSeverDescription).Returns(useLimbSeverDescription);
		part.Setup(x => x.FullDescription(It.IsAny<bool>(), It.IsAny<PermissionLevel>())).Returns(description);
		return part;
	}

	private static Mock<ILimb> CreateLimb(string name, IBodypart root)
	{
		var limb = new Mock<ILimb>();
		limb.SetupGet(x => x.Name).Returns(name);
		limb.SetupGet(x => x.RootBodypart).Returns(root);
		return limb;
	}
}
