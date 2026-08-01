#nullable enable

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Implementations;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.Health;
using MudSharp.Health.Wounds;

namespace MudSharp_Unit_Tests;

[TestClass]
public class MedicalRuntimeRegressionTests
{
	[TestMethod]
	public void NonFunctionalProstheticDisablesBodypart_FunctionalExactTargetRemainsUsable()
	{
		var bodypart = new Mock<IBodypart>();
		var prosthetic = new Mock<IProsthetic>();
		prosthetic.SetupGet(x => x.TargetBodypart).Returns(bodypart.Object);
		prosthetic.SetupGet(x => x.Functional).Returns(true);

		Assert.IsFalse(Body.NonFunctionalProstheticDisablesBodypart(prosthetic.Object, bodypart.Object));

		prosthetic.SetupGet(x => x.Functional).Returns(false);
		Assert.IsTrue(Body.NonFunctionalProstheticDisablesBodypart(prosthetic.Object, bodypart.Object));
	}

	[TestMethod]
	public void ProstheticTargetsBodypart_AcceptsCompatibleDerivedBodypart()
	{
		var canonical = new Mock<IBodypart>();
		var derived = new Mock<IBodypart>();
		derived.Setup(x => x.CountsAs(canonical.Object)).Returns(true);
		var prosthetic = new Mock<IProsthetic>();
		prosthetic.SetupGet(x => x.TargetBodypart).Returns(canonical.Object);

		Assert.IsTrue(Body.ProstheticTargetsBodypart(prosthetic.Object, derived.Object));
	}

	[TestMethod]
	public void CrutchMatchesLimb_UsesSideOfInventoryLocation()
	{
		var rightHand = new Mock<IBodypart>();
		rightHand.SetupGet(x => x.Alignment).Returns(Alignment.Right);
		var leftHand = new Mock<IBodypart>();
		leftHand.SetupGet(x => x.Alignment).Returns(Alignment.Left);
		var rightLegRoot = new Mock<IBodypart>();
		rightLegRoot.SetupGet(x => x.Alignment).Returns(Alignment.Right);
		var rightLeg = new Mock<ILimb>();
		rightLeg.SetupGet(x => x.RootBodypart).Returns(rightLegRoot.Object);

		Assert.IsTrue(Body.CrutchMatchesLimb(rightHand.Object, rightLeg.Object));
		Assert.IsFalse(Body.CrutchMatchesLimb(leftHand.Object, rightLeg.Object));
	}

	[TestMethod]
	public void WoundsCoveredByProfile_ReturnsFracturesUnderCoveredWearLocations()
	{
		var bone = new Mock<IBone>();
		var wearLocation = new Mock<IWear>();
		wearLocation.SetupGet(x => x.BoneInfo)
		            .Returns(new Dictionary<IBone, BodypartInternalInfo>
		            {
			            [bone.Object] = new BodypartInternalInfo(1.0, true, "arm")
		            });
		var wearProfile = new Mock<IWearProfile>();
		var body = new Mock<IBody>();
		wearProfile.Setup(x => x.Profile(body.Object))
		           .Returns(new Dictionary<IWear, IWearlocProfile>
		           {
			           [wearLocation.Object] = Mock.Of<IWearlocProfile>()
		           });
		var covered = new Mock<IImmobilisableWound>();
		covered.SetupGet(x => x.Bodypart).Returns(bone.Object);
		var uncovered = new Mock<IImmobilisableWound>();
		uncovered.SetupGet(x => x.Bodypart).Returns(Mock.Of<IBone>());
		body.SetupGet(x => x.Wounds).Returns(new IWound[] { covered.Object, uncovered.Object });

		var result = ImmobilisingGameItemComponent.WoundsCoveredByProfile(body.Object, wearProfile.Object).ToList();

		CollectionAssert.AreEqual(new[] { covered.Object }, result);
	}

	[TestMethod]
	public void FindReplacementImmobilisingItem_UsesAnotherWornSplintCoveringTheFracture()
	{
		var bone = new Mock<IBone>();
		var wearLocation = new Mock<IWear>();
		wearLocation.SetupGet(x => x.BoneInfo)
		            .Returns(new Dictionary<IBone, BodypartInternalInfo>
		            {
			            [bone.Object] = new BodypartInternalInfo(1.0, true, "arm")
		            });
		var wearProfile = new Mock<IWearProfile>();
		var body = new Mock<IBody>();
		wearProfile.Setup(x => x.Profile(body.Object))
		           .Returns(new Dictionary<IWear, IWearlocProfile>
		           {
			           [wearLocation.Object] = Mock.Of<IWearlocProfile>()
		           });
		var wound = new Mock<IImmobilisableWound>();
		wound.SetupGet(x => x.Bodypart).Returns(bone.Object);
		body.SetupGet(x => x.Wounds).Returns(new IWound[] { wound.Object });
		var remainingSplint = Mock.Of<IGameItem>();

		var result = ImmobilisingGameItemComponent.FindReplacementImmobilisingItem(
			body.Object,
			wound.Object,
			new[] { (remainingSplint, wearProfile.Object) });

		Assert.AreSame(remainingSplint, result);
	}

}
