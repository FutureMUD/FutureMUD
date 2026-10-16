#nullable enable

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.Magic.Casting;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ChargedMagicDeviceRechargeTests
{
	[DataTestMethod]
	[DataRow("wrong-route", "does not explicitly admit")]
	[DataRow("unacquired", "not acquired")]
	[DataRow("uncontrolled", "explicit overreach")]
	[DataRow("stale-profile", "reconciliation")]
	public void Recharge_InvalidCurrentRoute_PreservesExistingBankAndRefusesBeforePayment(string condition, string message)
	{
		var fixture = new ChargedMagicDeviceTests.Fixture();
		fixture.Charge(2);
		var acquired = fixture.F.Store.Acquisition(100, 2)!;
		switch (condition)
		{
			case "unacquired": fixture.F.Store.Acquired.Remove((100, 2)); break;
			case "uncontrolled": fixture.F.Store.Write(acquired: acquired with { ControlledGrade = 1 }); break;
			case "stale-profile": fixture.F.Store.Write(acquired: acquired with { ProfileVersion = acquired.ProfileVersion + 1 }); break;
		}
		var bank = fixture.Persisted;
		var balances = fixture.F.Balances.ToArray();
		var receipts = fixture.F.Store.Operations.Count;
		var result = fixture.Begin(capability: condition == "wrong-route" ? 2 : 1);
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
		StringAssert.Contains(result.Message, message);
		Assert.AreEqual(bank, fixture.Persisted);
		CollectionAssert.AreEqual(balances, fixture.F.Balances.ToArray());
		Assert.AreEqual(receipts, fixture.F.Store.Operations.Count);
		Assert.IsNull(fixture.Device.Reservation);
	}

	[TestMethod]
	public void Recharge_AlternateAdmittedCapability_PaysItsReserveAndPreservesOriginalPotencyAndGuids()
	{
		var fixture = new ChargedMagicDeviceTests.Fixture("<Effect type='damage'><DamageType>0</DamageType><DamageExpression>variable+grade+mastery+outcome</DamageExpression></Effect>");
		fixture.Charge(2);
		var originalCharges = fixture.Device.Export().Elements("Charge").Select(x => x.Value).ToArray();
		var fingerprint = fixture.Device.Snapshot!.PotencyFingerprint;
		var acquisition = fixture.F.Store.Acquisition(100, 2);
		var alternate = fixture.F.NewCapability(90, 1, 12, false);
		Assert.IsTrue(alternate.BuildingCommand(fixture.F.Actor.Object, new StringStack("casting entry add 2")));
		fixture.F.ActiveCapabilities.Clear();
		fixture.F.ActiveCapabilities.Add(alternate);
		fixture.F.Skills[1] += 20;
		var originalReserve = fixture.F.Balances[fixture.F.Resources[1]];
		var alternateReserve = fixture.F.Balances[fixture.F.Resources[2]];
		var start = fixture.Begin(capability: alternate.Id);
		Assert.AreEqual(MagicCastingStatus.Started, start.Status, start.Message);
		fixture.F.Now += TimeSpan.FromSeconds(60);
		var complete = fixture.F.Service.CompleteDeviceProduction(fixture.F.Actor.Object, start.OperationId!.Value);
		Assert.AreEqual(MagicCastingStatus.Succeeded, complete.Status, complete.Message);
		var charges = fixture.Device.Export().Elements("Charge").Select(x => x.Value).ToArray();
		CollectionAssert.AreEqual(originalCharges, charges.Take(originalCharges.Length).ToArray());
		Assert.AreEqual(3, charges.Length);
		Assert.AreEqual(3, charges.Distinct().Count());
		Assert.AreEqual(fingerprint, fixture.Device.Snapshot.PotencyFingerprint);
		Assert.AreEqual(1L, fixture.Device.ProducerCapability);
		Assert.AreEqual(originalReserve, fixture.F.Balances[fixture.F.Resources[1]]);
		Assert.AreEqual(alternateReserve - 10, fixture.F.Balances[fixture.F.Resources[2]]);
		Assert.AreEqual(acquisition, fixture.F.Store.Acquisition(100, 2));
		Assert.AreEqual(0, fixture.F.SkillUses);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void Recharge_PaidInterruptionOrRestart_PreservesOldChargesWithoutRefundOrReplay(bool restartDuringWork)
	{
		var fixture = new ChargedMagicDeviceTests.Fixture();
		fixture.Charge(2);
		var originalCharges = fixture.Device.Export().Elements("Charge").Select(x => x.Value).ToArray();
		var fingerprint = fixture.Device.Snapshot!.PotencyFingerprint;
		var balance = fixture.F.Balances[fixture.F.Resources[1]];
		var started = fixture.Begin();
		Assert.AreEqual(MagicCastingStatus.Started, started.Status, started.Message);
		Assert.AreEqual(balance - 10, fixture.F.Balances[fixture.F.Resources[1]]);
		if (!restartDuringWork)
			Assert.AreEqual(MagicCastingStatus.Failed, fixture.F.Service.CancelDeviceProduction(fixture.F.Actor.Object,
				started.OperationId!.Value).Status);
		fixture.Restart();
		fixture.F.Now += TimeSpan.FromSeconds(120);
		Assert.AreEqual(MagicCastingStatus.Refused, fixture.F.Service.CompleteDeviceProduction(fixture.F.Actor.Object,
			started.OperationId!.Value).Status);
		Assert.AreEqual(balance - 10, fixture.F.Balances[fixture.F.Resources[1]]);
		CollectionAssert.AreEqual(originalCharges, fixture.Device.Export().Elements("Charge").Select(x => x.Value).ToArray());
		Assert.AreEqual(fingerprint, fixture.Device.Snapshot!.PotencyFingerprint);
		if (restartDuringWork)
		{
			Assert.AreEqual(MagicCastingStatus.Refused, fixture.Begin().Status);
			Assert.AreEqual(MagicCastingStatus.Refused,
				fixture.F.Service.ActivateDevice(fixture.F.Actor.Object, fixture.Item.Object, "self").Status);
		}
	}
}
