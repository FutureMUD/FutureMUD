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
	public void SeverEchoText_PartDoesNotRemoveLimbRoot_NamesActualPart()
	{
		var eye = CreatePart("Right Eye", "right eye", true);
		var headRoot = CreatePart("Head", "head", true);
		headRoot.Setup(x => x.DownstreamOfPart(eye.Object)).Returns(false);
		var limb = CreateLimb("Head", headRoot.Object);

		Assert.AreEqual("$0's right eye is severed!", Body.SeverEchoText(eye.Object, limb.Object));
	}

	[TestMethod]
	public void SeverEchoText_PartRemovesLimbRoot_RetainsLimbAtPartForm()
	{
		var elbow = CreatePart("Elbow", "elbow", true);
		var armRoot = CreatePart("Upper Arm", "upper arm", true);
		armRoot.Setup(x => x.DownstreamOfPart(elbow.Object)).Returns(true);
		var limb = CreateLimb("Arm", armRoot.Object);

		Assert.AreEqual("$0's arm is severed at the elbow!", Body.SeverEchoText(elbow.Object, limb.Object));
	}

	private static Mock<IBodypart> CreatePart(string name, string description, bool significant)
	{
		var part = new Mock<IBodypart>();
		part.SetupGet(x => x.Name).Returns(name);
		part.SetupGet(x => x.Significant).Returns(significant);
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
