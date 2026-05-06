#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Form.Shape;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PositionStateTests
{
	[TestMethod]
	public void CompareTo_DynamicFallbackStates_DoNotRecurse()
	{
		dynamic floatingInWater = PositionFloatingInWater.Instance;
		dynamic riding = PositionRiding.Instance;
		dynamic hanging = PositionHanging.Instance;
		dynamic undefined = PositionUndefined.Instance;

		Assert.AreEqual(PositionHeightComparison.Equivalent, PositionStanding.Instance.CompareTo(floatingInWater));
		Assert.AreEqual(PositionHeightComparison.Higher, PositionStanding.Instance.CompareTo(riding));
		Assert.AreEqual(PositionHeightComparison.Undefined, PositionStanding.Instance.CompareTo(hanging));
		Assert.AreEqual(PositionHeightComparison.Undefined, PositionStanding.Instance.CompareTo(undefined));
	}

	[TestMethod]
	public void HangingDescribe_BeforeTarget_UsesSpatialPreposition()
	{
		var voyeur = new Mock<IPerceiver>();
		var target = GetTarget("a ceiling hook");

		Assert.AreEqual(
			"hanging before a ceiling hook",
			PositionHanging.Instance.Describe(voyeur.Object, target.Object, PositionModifier.Before, null));
	}

	[TestMethod]
	public void FloatingInWaterDescribe_NoHere_DoesNotLeaveTrailingSpace()
	{
		var voyeur = new Mock<IPerceiver>();

		Assert.AreEqual(
			"floating",
			PositionFloatingInWater.Instance.Describe(voyeur.Object, null, PositionModifier.None, null, false));
	}

	private static Mock<IPerceivable> GetTarget(string description)
	{
		var target = new Mock<IPerceivable>();
		target
			.Setup(x => x.HowSeen(
				It.IsAny<IPerceiver>(),
				It.IsAny<bool>(),
				It.IsAny<DescriptionType>(),
				It.IsAny<bool>(),
				It.IsAny<PerceiveIgnoreFlags>()))
			.Returns(description);
		return target;
	}
}
