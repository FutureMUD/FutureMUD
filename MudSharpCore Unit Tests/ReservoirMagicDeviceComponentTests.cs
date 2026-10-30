using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;
using MudSharp.Magic;
using MudSharp.RPG.Checks;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public class ReservoirMagicDeviceComponentTests
{
	private static ChargedMagicDeviceGameItemComponentProto ReservoirPrototype(ChargedMagicDeviceTests.Fixture fixture,
		int initial = 9, int maximum = 25)
	{
		var model = VancianItemTests.Prototype("ChargedMagicDevice");
		model.Definition = MagicDeviceDefinition.Serialize(MagicDeviceKind.Staff, MagicDeviceRole.Charged,
			MagicDeviceEligibility.Anyone, 10, 60, 0, 0, 0, 0, Difficulty.Normal, Outcome.Pass, [],
			new XElement("Plan", new XElement("Phase")), MagicDeviceMode.EnergyReservoir, 2, 1, maximum,
			initial, true, MagicDeviceDelivery.NativeArea, [new System.Collections.Generic.KeyValuePair<int, int>(1, 2)], true);
		return new(model, fixture.F.World.Object);
	}

	private static string WithChecksum(XElement root)
	{
		root.Attribute("checksum")?.Remove();
		var checksum = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(root.ToString(SaveOptions.DisableFormatting))));
		root.Add(new XAttribute("checksum", checksum));
		return root.ToString(SaveOptions.DisableFormatting);
	}

	[TestMethod]
	public void ReservoirState_RoundTripsEnergyReservationAndChecksum()
	{
		var f = new ChargedMagicDeviceTests.Fixture();
		var proto = ReservoirPrototype(f);
		var device = (ChargedMagicDeviceGameItemComponent)proto.CreateNew(f.Item.Object);
		Assert.AreEqual(9, device.Energy);
		var reservation = Guid.NewGuid();
		Assert.IsTrue(device.ReserveReservoir(reservation));
		var persisted = device.Export().ToString(SaveOptions.DisableFormatting);
		var reloaded = new ChargedMagicDeviceGameItemComponent(new MudSharp.Models.GameItemComponent { Id = 10, Definition = persisted }, proto, f.Item.Object);
		Assert.IsNull(reloaded.DataError);
		Assert.AreEqual(9, reloaded.Energy);
		Assert.AreEqual(reservation, reloaded.ReservoirReservation);
		reloaded.SpendReservoir(reservation, 2);
		Assert.AreEqual(7, reloaded.Energy);
		Assert.IsNull(reloaded.ReservoirReservation);
		var altered = XElement.Parse(reloaded.Export().ToString());
		altered.SetAttributeValue("energy", 6);
		var invalid = new ChargedMagicDeviceGameItemComponent(new MudSharp.Models.GameItemComponent { Id = 11, Definition = altered.ToString(SaveOptions.DisableFormatting) }, proto, f.Item.Object);
		Assert.IsNotNull(invalid.DataError);
	}

	[TestMethod]
	public void ReservedDonation_RequiresExactTokenAndClearsReservation()
	{
		var f = new ChargedMagicDeviceTests.Fixture();
		var device = (ChargedMagicDeviceGameItemComponent)ReservoirPrototype(f).CreateNew(f.Item.Object);
		var token = Guid.NewGuid();
		Assert.IsTrue(device.ReserveReservoir(token));
		Assert.ThrowsExactly<InvalidOperationException>(() => device.AddEnergy(Guid.NewGuid(), 3));
		Assert.AreEqual(9, device.Energy);
		Assert.AreEqual(3, device.AddEnergy(token, 3));
		Assert.AreEqual(12, device.Energy);
		Assert.IsNull(device.ReservoirReservation);
	}

	[TestMethod]
	public void NewReservoirUsesConfiguredInitialEnergy_WhileCopyStartsEmpty()
	{
		var f = new ChargedMagicDeviceTests.Fixture();
		var proto = ReservoirPrototype(f, initial: 11, maximum: 30);
		var created = (ChargedMagicDeviceGameItemComponent)proto.CreateNew(f.Item.Object);
		Assert.AreEqual(11, created.Energy);
		var copy = (ChargedMagicDeviceGameItemComponent)created.Copy(f.Item.Object);
		Assert.AreEqual(0, copy.Energy);
		Assert.AreEqual(30, copy.MaximumEnergy);
	}

	[TestMethod]
	public void LegacyStoredChargePayload_RoundTripsWithoutReservoirState()
	{
		var f = new ChargedMagicDeviceTests.Fixture();
		f.Charge(2);
		var legacyDefinition = XElement.Parse(f.Device.Export().ToString());
		legacyDefinition.Attribute("energy")?.Remove();
		var persisted = WithChecksum(legacyDefinition);
		var prototypeDefinition = XElement.Parse(f.Proto.Configuration);
		prototypeDefinition.Element("Reservoir")?.Remove();
		var prototypeModel = VancianItemTests.Prototype("ChargedMagicDevice");
		prototypeModel.Definition = prototypeDefinition.ToString(SaveOptions.DisableFormatting);
		var legacyPrototype = new ChargedMagicDeviceGameItemComponentProto(prototypeModel, f.F.World.Object);
		var reloaded = new ChargedMagicDeviceGameItemComponent(new MudSharp.Models.GameItemComponent { Id = 12, Definition = persisted }, legacyPrototype, f.Item.Object);
		Assert.IsNull(reloaded.DataError);
		Assert.AreEqual(MagicDeviceMode.StoredCharges, reloaded.Mode);
		Assert.AreEqual(2, reloaded.Charges);
		Assert.AreEqual(0, reloaded.Energy);
	}

	[TestMethod]
	public void PrototypeRevision_RefusesModeConversionWithStoredState()
	{
		var f = new ChargedMagicDeviceTests.Fixture();
		f.Charge();
		var previousPrototype = f.Device.Prototype;
		var previousState = f.Device.Export().ToString(SaveOptions.DisableFormatting);
		var next = ReservoirPrototype(f);
		StringAssert.Contains(f.Device.PrototypeUpdateError(next)!, "reinterpret");
		Assert.AreSame(previousPrototype, f.Device.Prototype);
		Assert.AreEqual(previousState, f.Device.Export().ToString(SaveOptions.DisableFormatting));
	}

	[TestMethod]
	public void PrototypeRevision_RefusesMaximumBelowPersistedEnergy()
	{
		var f = new ChargedMagicDeviceTests.Fixture();
		var current = ReservoirPrototype(f, initial: 9, maximum: 25);
		var device = (ChargedMagicDeviceGameItemComponent)current.CreateNew(f.Item.Object);
		var previousState = device.Export().ToString(SaveOptions.DisableFormatting);
		var reducedMaximum = ReservoirPrototype(f, initial: 0, maximum: 8);
		StringAssert.Contains(device.PrototypeUpdateError(reducedMaximum)!, "maximum");
		Assert.AreEqual(previousState, device.Export().ToString(SaveOptions.DisableFormatting));
	}

	[TestMethod]
	public void ReservoirRejectsMixedLegacyChargeBankPayload()
	{
		var f = new ChargedMagicDeviceTests.Fixture();
		var proto = ReservoirPrototype(f);
		var root = new XElement("ChargedMagicDevice", new XAttribute("version", 1), new XAttribute("capability", 0),
			new XAttribute("trait", 0), new XAttribute("raw", 0), new XAttribute("energy", 9),
			new XElement("Charge", Guid.NewGuid()));
		var loaded = new ChargedMagicDeviceGameItemComponent(new MudSharp.Models.GameItemComponent { Id = 13, Definition = WithChecksum(root) }, proto, f.Item.Object);
		Assert.IsNotNull(loaded.DataError);
	}

	[TestMethod]
	public void SeederReservoirStateHelperProducesLoadableInitialEnergy()
	{
		var f = new ChargedMagicDeviceTests.Fixture();
		var proto = ReservoirPrototype(f, initial: 4, maximum: 12);
		var loaded = new ChargedMagicDeviceGameItemComponent(new MudSharp.Models.GameItemComponent
			{ Id = 14, Definition = MagicDeviceDefinition.ReservoirState(4) }, proto, f.Item.Object);
		Assert.IsNull(loaded.DataError);
		Assert.AreEqual(4, loaded.Energy);
		Assert.AreEqual(12, loaded.MaximumEnergy);
	}
}
