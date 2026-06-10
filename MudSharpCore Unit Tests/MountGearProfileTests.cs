#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class MountGearProfileTests
{
	[TestMethod]
	public void ProfileFor_BarebackMount_AppliesDefaultControlAndStabilityPenalties()
	{
		var mount = CreateCharacterWithGear([]);

		var profile = MountGearService.ProfileFor(mount.Object);

		Assert.IsFalse(profile.HasSaddle);
		Assert.IsFalse(profile.HasBridle);
		Assert.IsFalse(profile.HasReins);
		Assert.AreEqual(-25.0, profile.ControlBonus);
		Assert.AreEqual(-10.0, profile.StabilityBonus);
		CollectionAssert.AreEquivalent(
			new[]
			{
				"no saddle or pack saddle",
				"no bridle or equivalent control headgear",
				"no reins or equivalent control aid"
			},
			profile.Warnings.ToArray());
	}

	[TestMethod]
	public void ProfileFor_BitlessControl_DoesNotApplyMissingBitPenalty()
	{
		var gearItem = CreateRidingGearItem(1,
			RidingGearRole.Saddle |
			RidingGearRole.SaddlePad |
			RidingGearRole.Stirrups |
			RidingGearRole.Bridle |
			RidingGearRole.Reins |
			RidingGearRole.BitlessControl,
			controlBonus: 2.0,
			stabilityBonus: 3.0);
		var mount = CreateCharacterWithGear([gearItem.Object]);

		var profile = MountGearService.ProfileFor(mount.Object);

		Assert.IsTrue(profile.UsesBitlessControl);
		Assert.IsTrue(profile.HasBit);
		Assert.AreEqual(2.0, profile.ControlBonus);
		Assert.AreEqual(3.0, profile.StabilityBonus);
		Assert.AreEqual(0, profile.Warnings.Count);
	}

	[TestMethod]
	public void ProfileFor_RiderAndMountGear_AggregatesBothSources()
	{
		var saddle = CreateRidingGearItem(1,
			RidingGearRole.Saddle | RidingGearRole.SaddlePad | RidingGearRole.Stirrups,
			stabilityBonus: 1.0);
		var bridle = CreateRidingGearItem(2,
			RidingGearRole.Bridle | RidingGearRole.Reins | RidingGearRole.Bit,
			controlBonus: 1.5);
		var mount = CreateCharacterWithGear([saddle.Object]);
		var rider = CreateCharacterWithGear([bridle.Object]);

		var profile = MountGearService.ProfileFor(mount.Object, rider.Object);

		Assert.IsTrue(profile.HasSaddle);
		Assert.IsTrue(profile.HasBridle);
		Assert.IsTrue(profile.HasReins);
		Assert.IsTrue(profile.HasBit);
		Assert.AreEqual(1.5, profile.ControlBonus);
		Assert.AreEqual(1.0, profile.StabilityBonus);
		Assert.AreEqual(2, profile.GearItems.Count);
		Assert.AreEqual(0, profile.Warnings.Count);
	}

	private static Mock<ICharacter> CreateCharacterWithGear(IReadOnlyCollection<IGameItem> items)
	{
		var body = new Mock<IBody>();
		body.SetupGet(x => x.ExternalItems).Returns(items);
		body.SetupGet(x => x.ExternalItemsForOtherActors).Returns([]);

		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Body).Returns(body.Object);
		return character;
	}

	private static Mock<IGameItem> CreateRidingGearItem(long id, RidingGearRole roles, double controlBonus = 0.0,
		double stabilityBonus = 0.0)
	{
		var gear = new Mock<IRidingGear>();
		gear.SetupGet(x => x.Roles).Returns(roles);
		gear.SetupGet(x => x.ControlBonus).Returns(controlBonus);
		gear.SetupGet(x => x.StabilityBonus).Returns(stabilityBonus);

		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Id).Returns(id);
		item.Setup(x => x.GetItemTypes<IRidingGear>()).Returns([gear.Object]);
		item.Setup(x => x.IsItemType<IRidingGear>()).Returns(true);
		return item;
	}
}
