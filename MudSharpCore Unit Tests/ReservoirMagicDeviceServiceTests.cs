#nullable enable

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ReservoirMagicDeviceServiceTests
{
	private static (ChargedMagicDeviceTests.Fixture Fixture, ChargedMagicDeviceGameItemComponent Device) Create()
	{
		var fixture = new ChargedMagicDeviceTests.Fixture();
		var model = VancianItemTests.Prototype("ChargedMagicDevice");
		model.Definition = MagicDeviceDefinition.Serialize(MagicDeviceKind.Staff, MagicDeviceRole.Charged,
			MagicDeviceEligibility.Anyone, 1, 60, 0, 0, 0, 0, Difficulty.Normal, Outcome.Pass, [2],
			new XElement("Plan", new XElement("Phase")), MagicDeviceMode.EnergyReservoir, 2, 10, 50, 20, true,
			MagicDeviceDelivery.OtherCharacters, new Dictionary<int, int> { [1] = 5 });
		var proto = new ChargedMagicDeviceGameItemComponentProto(model, fixture.F.World.Object);
		var device = new ChargedMagicDeviceGameItemComponent(proto, fixture.Item.Object, true);
		fixture.Item.Setup(x => x.GetItemType<IChargedMagicDevice>()).Returns(device);
		fixture.F.Service.DevicePersistence = _ => { };
		fixture.F.ActiveCapabilities.Clear();
		return (fixture, device);
	}

	[TestMethod]
	public void Donation_TransfersManaOneForOneWithoutCastingEntitlement()
	{
		var (f, device) = Create();
		var result = f.F.Service.DonateReservoir(f.F.Actor.Object, f.Item.Object, 13);
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(33, device.Energy);
		Assert.AreEqual(87.0, f.F.Balances[f.F.Resources[0]]);
		Assert.IsNull(device.ReservoirReservation);
		Assert.AreEqual(0, f.F.Rolls);
		Assert.AreEqual(0, f.F.SkillUses);
	}

	[TestMethod]
	[DataRow(0)]
	[DataRow(-1)]
	[DataRow(31)]
	public void InvalidOrOverflowDonation_PaysNothing(int amount)
	{
		var (f, device) = Create();
		var result = f.F.Service.DonateReservoir(f.F.Actor.Object, f.Item.Object, amount);
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
		Assert.AreEqual(20, device.Energy);
		Assert.AreEqual(100.0, f.F.Balances[f.F.Resources[0]]);
		Assert.AreEqual(0, f.F.Store.Operations.Count);
	}

	[TestMethod]
	public void InsufficientDonation_PaysNothing()
	{
		var (f, device) = Create();
		f.F.Balances[f.F.Resources[0]] = 2;
		var result = f.F.Service.DonateReservoir(f.F.Actor.Object, f.Item.Object, 3);
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
		Assert.AreEqual(20, device.Energy);
		Assert.AreEqual(2.0, f.F.Balances[f.F.Resources[0]]);
		Assert.AreEqual(0, f.F.Store.Operations.Count);
	}

	[TestMethod]
	[DataRow("8")]
	[DataRow("nothing")]
	[DataRow("mon")]
	public void UnsupportedPower_PaysNothing(string word)
	{
		var (f, device) = Create();
		var result = f.F.Service.ActivateReservoir(f.F.Actor.Object, f.Item.Object, word);
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
		Assert.AreEqual(20, device.Energy);
		Assert.AreEqual(0, f.F.Store.Operations.Count);
	}

	[TestMethod]
	public void WornDevice_AllowsDonationWithoutHoldingIt()
	{
		var (f, device) = Create();
		f.Held.Clear();
		f.F.Body.SetupGet(x => x.DirectWornItems).Returns([f.Item.Object]);
		var result = f.F.Service.DonateReservoir(f.F.Actor.Object, f.Item.Object, 3);
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(23, device.Energy);
	}

	[TestMethod]
	public void ResourceCapCallbackDropsDevice_RefusesBeforePayment()
	{
		var (f, device) = Create();
		Mock.Get(f.F.Resources[0]).Setup(x => x.ResourceCap(It.IsAny<MudSharp.Magic.IHaveMagicResource>()))
			.Returns(() => { f.Held.Clear(); return 100.0; });
		var result = f.F.Service.DonateReservoir(f.F.Actor.Object, f.Item.Object, 3);
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
		Assert.AreEqual(20, device.Energy);
		Assert.AreEqual(100.0, f.F.Balances[f.F.Resources[0]]);
		Assert.AreEqual(0, f.F.Store.Operations.Count);
	}

	[TestMethod]
	public void DonationFailureAfterPayment_QuarantinesItemRatherThanReplays()
	{
		var (f, device) = Create();
		f.F.Checkpoint = stage => { if (stage == "DeviceEnergyAdded") throw new InvalidOperationException("Injected persistence boundary"); };
		var result = f.F.Service.DonateReservoir(f.F.Actor.Object, f.Item.Object, 3);
		Assert.AreEqual(MagicCastingStatus.NeedsReview, result.Status, result.Message);
		Assert.AreEqual(23, device.Energy);
		Assert.AreEqual(97.0, f.F.Balances[f.F.Resources[0]]);
		f.F.Checkpoint = null;
		Assert.AreEqual(MagicCastingStatus.Refused, f.F.Service.DonateReservoir(f.F.Actor.Object, f.Item.Object, 3).Status);
		Assert.AreEqual(23, device.Energy);
		Assert.AreEqual(97.0, f.F.Balances[f.F.Resources[0]]);
	}
}
