#nullable enable

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Implementations;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;

namespace MudSharp_Unit_Tests;

[TestClass]
public class WearProfileSecurityTests
{
	[TestMethod]
	public void CanWear_MandatoryBodypartsMissing_RejectsProfile()
	{
		var body = TestObjectFactory.CreateUninitialized<Body>();
		var profile = MissingProfile();
		var (item, wearable) = Item(profile.Object);

		Assert.IsFalse(body.CanWear(item.Object, profile.Object));
		wearable.Verify(x => x.CanWear(body, profile.Object), Times.Never);
	}

	[TestMethod]
	public void CanWear_AllProfilesMissingBodyparts_ReturnsFalse()
	{
		var body = TestObjectFactory.CreateUninitialized<Body>();
		var (item, _) = Item(MissingProfile().Object, MissingProfile().Object);

		Assert.IsFalse(body.CanWear(item.Object));
		Assert.IsNull(body.WhichProfile(item.Object));
	}

	[TestMethod]
	public void WhichProfile_DefaultMissingBodyparts_SelectsCompatibleAlternative()
	{
		var body = TestObjectFactory.CreateUninitialized<Body>();
		var compatible = new Mock<IWearProfile>();
		compatible.Setup(x => x.Profile(body)).Returns(new Dictionary<IWear, IWearlocProfile>());
		var (item, _) = Item(MissingProfile().Object, compatible.Object);

		Assert.AreSame(compatible.Object, body.WhichProfile(item.Object));
		Assert.IsTrue(body.CanWear(item.Object, compatible.Object));
	}

	private static Mock<IWearProfile> MissingProfile()
	{
		var profile = new Mock<IWearProfile>();
		profile.Setup(x => x.Profile(It.IsAny<IBody>())).Returns((Dictionary<IWear, IWearlocProfile>)null!);
		return profile;
	}

	private static (Mock<IGameItem> Item, Mock<IWearable> Wearable) Item(params IWearProfile[] profiles)
	{
		var wearable = new Mock<IWearable>();
		wearable.SetupGet(x => x.Profiles).Returns(profiles);
		wearable.SetupGet(x => x.DefaultProfile).Returns(profiles[0]);
		wearable.Setup(x => x.CanWear(It.IsAny<IBody>())).Returns(true);
		wearable.Setup(x => x.CanWear(It.IsAny<IBody>(), It.IsAny<IWearProfile>())).Returns(true);
		var item = new Mock<IGameItem>();
		item.Setup(x => x.GetItemType<IWearable>()).Returns(wearable.Object);
		return (item, wearable);
	}
}
