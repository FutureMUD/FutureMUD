using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.SpellEffects;
using MudSharp.Magic.Vancian;
using MudSharp.RPG.Checks;
using MudSharp.FutureProg;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public class ChargedMagicDeviceTests
{
	private delegate bool TargetPolicyExecutor(out object result, object[] arguments);
	private static Mock<IFutureProg> SuccessfulTargetPolicy(long id, Action callback)
	{
		var prog = new Mock<IFutureProg>(); prog.SetupGet(x => x.Id).Returns(id);
		prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
		prog.SetupGet(x => x.CompileError).Returns("");
		prog.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);
		prog.Setup(x => x.ExecuteWithStatus(out It.Ref<object>.IsAny, It.IsAny<object[]>()))
			.Returns(new TargetPolicyExecutor((out object result, object[] arguments) =>
			{
				callback(); result = true; return true;
			}));
		return prog;
	}
	[TestMethod]
	[DataRow("DeviceConsumed", false)]
	[DataRow("DeviceConsumed", true)]
	[DataRow("DeviceFilled", false)]
	[DataRow("DeviceFilled", true)]
	public void DurableFailure_TransferredToOtherCanonicalIdentity_QuarantinesBankUntilReconciliation(string boundary, bool reload)
	{
		var f = new Fixture(); MagicCastingResult failed;
		if (boundary == "DeviceConsumed")
		{
			f.Charge(2); f.F.Checkpoint = stage => { if (stage == boundary) throw new InvalidOperationException("Owned transfer failure"); };
			failed = f.F.Service.ActivateDevice(f.F.Actor.Object, f.Item.Object, "self");
		}
		else
		{
			var started = f.Begin(count: 2); Assert.AreEqual(MagicCastingStatus.Started, started.Status);
			f.F.Checkpoint = stage => { if (stage == boundary) throw new InvalidOperationException("Owned transfer failure"); };
			f.F.Now += TimeSpan.FromSeconds(120); failed = f.F.Service.CompleteDeviceProduction(f.F.Actor.Object, started.OperationId!.Value);
		}
		Assert.AreEqual(MagicCastingStatus.NeedsReview, failed.Status); Assert.IsNull(f.Device.Reservation);
		if (reload) f.Restart(); f.F.Checkpoint = null;
		// The fixture changes its canonical identity to model transfer; the native test transfers between actual distinct bodies.
		f.F.Actor.SetupGet(x => x.Id).Returns(200); f.F.Actor.SetupGet(x => x.InstanceId).Returns(200);
		Assert.IsNull(f.F.Service.QuarantineReason(f.F.Actor.Object, spellId: 2));
		Assert.IsNotNull(f.F.Service.QuarantineReason(f.F.Actor.Object, itemIds: [400]));
		var bank = f.Device.Export().ToString(); var guid = f.Device.NextCharge; var balance = f.F.Balances[f.F.Resources[1]];
		var journal = f.F.Store.Operations.ToDictionary(x => x.Key, x => x.Value);
		Assert.AreEqual(MagicCastingStatus.Refused, f.F.Service.ActivateDevice(f.F.Actor.Object, f.Item.Object, "self").Status);
		Assert.AreEqual(bank, f.Device.Export().ToString()); Assert.AreEqual(guid, f.Device.NextCharge); Assert.AreEqual(balance, f.F.Balances[f.F.Resources[1]]);
		CollectionAssert.AreEquivalent(journal.ToArray(), f.F.Store.Operations.ToArray());
		f.F.Actor.Verify(x => x.AddEffect(It.IsAny<SpellBlindnessEffect>()), Times.Never);
		f.F.Actor.SetupGet(x => x.Id).Returns(100); f.F.Actor.SetupGet(x => x.InstanceId).Returns(100);
		Assert.IsTrue(f.F.Service.ReconcileOperation(f.F.Staff.Object, f.F.Actor.Object, failed.OperationId!.Value, "Owned explicit reconciliation").Allowed);
		f.F.Actor.SetupGet(x => x.Id).Returns(200); f.F.Actor.SetupGet(x => x.InstanceId).Returns(200);
		Assert.IsNull(f.F.Service.QuarantineReason(f.F.Actor.Object, itemIds: [400]));
		Assert.AreEqual(MagicCastingStatus.Succeeded, f.F.Service.ActivateDevice(f.F.Actor.Object, f.Item.Object, "self").Status);
	}
	[TestMethod]
	public void FinalCommitment_NewGlobalItemQuarantineAfterCallback_RefusesWithoutClaim()
	{
		var f = new Fixture(); var calls = 0;
		var prog = SuccessfulTargetPolicy(703, () =>
		{
			if (++calls == 2) f.F.Store.Write(new(Guid.NewGuid(), 999, 999, 998, 1, 2, 1, 11, "NeedsReview", "<Casting version='1'><Item id='400'/></Casting>", f.F.Now, f.F.Now));
		});
		f.F.World.SetupGet(x => x.FutureProgs).Returns(MagicCastingFixture.Collection(() => new[] { prog.Object }));
		var source = (MagicSpell)f.F.Spells.Single(x => x.Id == 2);
		// Frozen banks capture their trigger, so recreate with this legitimate source configuration.
		var trigger = new XElement("Trigger", new XAttribute("type", "character"), new XElement("MinimumPower", 0), new XElement("MaximumPower", 10), new XElement("CanTargetSelf", true), new XElement("TargetFilterProg", 703));
		source.Trigger = SpellTriggerFactory.LoadTrigger(trigger, source); f.Charge(2); var bank = f.Device.Export().ToString(); var charge = f.Device.NextCharge;
		f.F.Actor.Setup(x => x.TargetActorOrCorpse(It.IsAny<string>(), It.IsAny<PerceiveIgnoreFlags>())).Returns(f.F.Actor.Object);
		var receipts = f.F.Store.Operations.Count; var balance = f.F.Balances[f.F.Resources[1]];
		Assert.AreEqual(MagicCastingStatus.Refused, f.F.Service.ActivateDevice(f.F.Actor.Object, f.Item.Object, "self").Status);
		Assert.AreEqual(2, calls); Assert.AreEqual(receipts + 1, f.F.Store.Operations.Count);
		prog.Verify(x => x.ExecuteWithStatus(out It.Ref<object>.IsAny, It.IsAny<object[]>()), Times.Exactly(2));
		Assert.AreEqual(bank, f.Device.Export().ToString()); Assert.AreEqual(charge, f.Device.NextCharge); Assert.AreEqual(balance, f.F.Balances[f.F.Resources[1]]);
	}
	[TestMethod]
	public void FinalCapabilityGetter_TrueEntitlementAfterDroppingDevice_RefusesRawCustody()
	{
		var f = new Fixture(); f.Charge(); var calls = 0;
		f.F.Actor.SetupGet(x => x.Capabilities).Returns(() => { if (++calls == 2) f.Held.Clear(); return f.F.ActiveCapabilities; });
		var bank = f.Persisted; var balance = f.F.Balances[f.F.Resources[1]]; var receipts = f.F.Store.Operations.Count;
		var result = f.F.Service.ActivateDevice(f.F.Actor.Object, f.Item.Object, "self");
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status); Assert.AreEqual(2, calls);
		Assert.AreEqual(bank, f.Persisted); Assert.AreEqual(balance, f.F.Balances[f.F.Resources[1]]);
		Assert.AreEqual(receipts, f.F.Store.Operations.Count); Assert.IsNull(f.Device.Reservation);
	}
	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void InsufficientDevicePayment_RefusesWithoutJournalBankOrReservation(bool focus)
	{
		var f = new Fixture(); f.Charge(); f.F.Balances[f.F.Resources[1]] = 1;
		var bank = f.Persisted; var receipts = f.F.Store.Operations.Count;
		var result = focus ? f.F.Service.CastDeviceFocus(new(f.F.Actor.Object, 1, 2, 2, false, "self"), f.Item.Object) : f.Begin();
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message); Assert.AreEqual(1, f.F.Balances[f.F.Resources[1]]);
		Assert.AreEqual(bank, f.Persisted); Assert.AreEqual(receipts, f.F.Store.Operations.Count); Assert.IsNull(f.Device.Reservation);
	}
	[TestMethod]
	[DataRow("amount")]
	[DataRow("holder")]
	[DataRow("resource")]
	public void CapacityAdmission_ExactDebitOnly_NoUnrelatedReadsOrReuse(string mismatch)
	{
		var holder = new Mock<IHaveMagicResource>(); var otherHolder = new Mock<IHaveMagicResource>();
		var resource = new Mock<IMagicResource>(); var otherResource = new Mock<IMagicResource>(); var calls = 0;
		resource.Setup(x => x.ResourceCap(holder.Object)).Returns(() => { calls++; return 45; });
		var admission = new MagicResourceCapacityAdmission([new(holder.Object, resource.Object, 10, 100)]);
		using (admission.OpenScope())
		{
			Assert.IsTrue(MagicResourceCapacity.TryGetCap(resource.Object, holder.Object, out var live, out _)); Assert.AreEqual(45, live);
			Assert.ThrowsException<InvalidOperationException>(() => MagicResourceCapacityAdmission.BeginDebit(
				mismatch == "holder" ? otherHolder.Object : holder.Object, mismatch == "resource" ? otherResource.Object : resource.Object, mismatch == "amount" ? 11 : 10));
			Assert.ThrowsException<InvalidOperationException>(() => new MagicResourceCapacityAdmission([]).OpenScope());
			using (MagicResourceCapacityAdmission.BeginDebit(holder.Object, resource.Object, 10))
			{
				Assert.IsTrue(MagicResourceCapacity.TryGetCap(resource.Object, holder.Object, out var frozen, out _)); Assert.AreEqual(100, frozen);
				Assert.IsFalse(MagicResourceCapacity.TryGetCap(resource.Object, holder.Object, out _, out _));
			}
			Assert.ThrowsException<InvalidOperationException>(() => MagicResourceCapacityAdmission.BeginDebit(holder.Object, resource.Object, 10));
		}
		Assert.IsTrue(MagicResourceCapacity.TryGetCap(resource.Object, holder.Object, out var after, out _)); Assert.AreEqual(45, after);
		Assert.AreEqual(2, calls); Assert.IsNull(MagicResourceCapacityAdmission.BeginDebit(holder.Object, resource.Object, 10));
		Assert.ThrowsException<InvalidOperationException>(() => admission.OpenScope());
	}
	[TestMethod]
	public void CapacityAdmission_ExceptionAndConcurrentEscape_DoNotLeakCachedCapacity()
	{
		var holder = new Mock<IHaveMagicResource>(); var resource = new Mock<IMagicResource>();
		resource.Setup(x => x.ResourceCap(holder.Object)).Returns(42);
		try
		{
			using var batch = new MagicResourceCapacityAdmission([new(holder.Object, resource.Object, 10, 100)]).OpenScope();
			System.Threading.Tasks.Task.Run(() => Assert.ThrowsException<InvalidOperationException>(() => MagicResourceCapacityAdmission.BeginDebit(holder.Object, resource.Object, 10))).GetAwaiter().GetResult();
			using var debit = MagicResourceCapacityAdmission.BeginDebit(holder.Object, resource.Object, 10);
			throw new ApplicationException("Owned fixture failure");
		}
		catch (ApplicationException) { }
		Assert.IsNull(MagicResourceCapacityAdmission.BeginDebit(holder.Object, resource.Object, 10));
		Assert.IsTrue(MagicResourceCapacity.TryGetCap(resource.Object, holder.Object, out var cap, out _)); Assert.AreEqual(42, cap);
		using var next = new MagicResourceCapacityAdmission([]).OpenScope();
	}
	[TestMethod]
	[DataRow(false, "custody")]
	[DataRow(true, "custody")]
	[DataRow(true, "balance")]
	[DataRow(true, "configuration")]
	[DataRow(true, "body")]
	[DataRow(true, "holder")]
	[DataRow(false, "skill")]
	[DataRow(true, "skill")]
	public void FinalCapacityCallback_ValidReturnAfterMutation_RefusesBeforeJournal(bool focus, string mutation)
	{
		var f = new Fixture(); f.Charge(); var armed = false;
		f.F.Actor.SetupGet(x => x.Traits).Returns([f.F.NativeSkill.Object]);
		f.F.NativeSkill.SetupGet(x => x.RawValue).Returns(() => f.F.Skills[1]);
		f.F.Checkpoint = stage => { if (stage == "DeviceAdmissionPolicies") armed = true; };
		var resource = Mock.Get(f.F.Resources[1]);
		resource.Setup(x => x.ResourceCap(It.IsAny<IHaveMagicResource>())).Returns<IHaveMagicResource>(_ =>
		{
			if (!armed) return 100;
			armed = false;
			if (mutation == "custody") f.Held.Clear();
			if (mutation == "balance") f.F.Balances[f.F.Resources[1]] = 1;
			if (mutation == "skill") f.F.Skills[1] = 5;
			if (mutation == "configuration") f.Proto.BuildingCommand(f.F.Actor.Object, new StringStack("capacity 6"));
			if (mutation == "body") f.F.Body.SetupGet(x => x.Id).Returns(901);
			if (mutation == "holder")
			{
				var changedOwner = new Mock<ICharacterInstance>(); changedOwner.SetupGet(x => x.Id).Returns(902);
				var changedIdentity = new Mock<ICharacterIdentity>(); changedIdentity.SetupGet(x => x.PrimaryInstance).Returns(changedOwner.Object);
				f.F.Actor.SetupGet(x => x.Identity).Returns(changedIdentity.Object);
			}
			return 100;
		});
		var receipts = f.F.Store.Operations.Count; var bank = f.Persisted; var balance = f.F.Balances[f.F.Resources[1]];
		var result = focus ? f.F.Service.CastDeviceFocus(new(f.F.Actor.Object, 1, 2, 2, false, "self"), f.Item.Object) : f.Begin();
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message); Assert.IsFalse(armed);
		Assert.AreEqual(receipts, f.F.Store.Operations.Count); Assert.AreEqual(bank, f.Persisted); Assert.IsNull(f.Device.Reservation);
		Assert.AreEqual(mutation == "balance" ? 1 : balance, f.F.Balances[f.F.Resources[1]]);
	}
	internal sealed class Fixture
	{
		public MagicCastingFixture F { get; } = new();
		public Mock<IGameItem> Item { get; } = new();
		public ChargedMagicDeviceGameItemComponentProto Proto { get; }
		public ChargedMagicDeviceGameItemComponent Device { get; private set; }
		public List<IGameItem> Held { get; } = [];
		public string Persisted { get; private set; } = "";
		public Fixture(string effect = "<Effect type='blindness'/>")
		{
			F.Actor.SetupGet(x => x.Combat).Returns((MudSharp.Combat.ICombat)null!);
			F.Actor.SetupGet(x => x.Movement).Returns((MudSharp.Movement.IMovement)null!);
			var source = F.NewSpell(2, "Device Payload", effect);
			Assert.IsTrue(source.BuildingCommand(F.Actor.Object, new StringStack("grades fixture")));
			Assert.IsTrue(F.Earth.BuildingCommand(F.Actor.Object, new StringStack("casting entry add 2")));
			F.Store.Write(acquired: new(100, 2, 2, 1, F.Now, "legitimate device test grant", DateTime.UnixEpoch, 0));
			Item.SetupGet(x => x.Id).Returns(400); Item.SetupGet(x => x.Name).Returns("test wand");
			Item.SetupGet(x => x.GetObject).Returns(Item.Object); Item.SetupGet(x => x.Gameworld).Returns(F.World.Object);
			F.Actor.Setup(x => x.CanSee(It.IsAny<IPerceivable>(), It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
			F.Actor.Setup(x => x.CanManipulateItem(It.IsAny<IGameItem>())).Returns((true, ""));
			F.Body.SetupGet(x => x.HeldOrWieldedItems).Returns(() => Held);
			Held.Add(Item.Object);
			var model = VancianItemTests.Prototype("ChargedMagicDevice");
			model.Definition = "<Definition version='1'><Kind>0</Kind><Role>2</Role><Eligibility>1</Eligibility><Capacity>5</Capacity><Seconds>60</Seconds><Spell>2</Spell><Plan><Phase/></Plan></Definition>";
			Proto = new(model, F.World.Object);
			Device = new(Proto, Item.Object, true);
			Bind();
		}
		public void Bind()
		{
			Item.Setup(x => x.GetItemType<IChargedMagicDevice>()).Returns(() => Device);
			F.Service.DevicePersistence = _ => Persisted = Device.Export().ToString();
		}
		public MagicCastingResult Begin(int grade = 2, int count = 1, long capability = 1) => F.Service.BeginDeviceProduction(F.Actor.Object, Item.Object, capability, 2, grade, count);
		public void Charge(int count = 1)
		{
			var start = Begin(count: count); Assert.AreEqual(MagicCastingStatus.Started, start.Status, start.Message);
			F.Now += TimeSpan.FromSeconds(60 * count);
			var complete = F.Service.CompleteDeviceProduction(F.Actor.Object, start.OperationId!.Value);
			Assert.AreEqual(MagicCastingStatus.Succeeded, complete.Status, complete.Message);
		}
		public void Restart()
		{
			F.Restart();
			Device = new(new MudSharp.Models.GameItemComponent { Id = 10, Definition = Persisted }, Proto, Item.Object); Bind();
		}
	}

	[TestMethod]
	[DataRow(false, "custody")]
	[DataRow(false, "entitlement")]
	[DataRow(false, "configuration")]
	[DataRow(false, "body")]
	[DataRow(true, "custody")]
	[DataRow(true, "entitlement")]
	[DataRow(true, "configuration")]
	[DataRow(true, "body")]
	public void ActualUsabilityCallback_TrueAfterMutation_RefusesBeforeCommitment(bool focus, string mutation)
	{
		var f = new Fixture(); f.Charge();
		var prog = new Mock<IFutureProg>(); prog.SetupGet(x => x.Id).Returns(701);
		prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
		prog.SetupGet(x => x.Parameters).Returns(new[] { ProgVariableTypes.Character, ProgVariableTypes.Item });
		prog.Setup(x => x.Execute(It.IsAny<object[]>())).Returns<object[]>(_ =>
		{
			if (mutation == "custody") f.Held.Clear();
			if (mutation == "entitlement") f.F.ActiveCapabilities.Clear();
			if (mutation == "configuration") f.Proto.BuildingCommand(f.F.Actor.Object, new StringStack("capacity 6"));
			if (mutation == "body") f.F.Body.SetupGet(x => x.Id).Returns(900);
			return true;
		});
		f.F.World.SetupGet(x => x.FutureProgs).Returns(MagicCastingFixture.Collection(() => new[] { prog.Object }));
		Assert.IsTrue(f.Proto.BuildingCommand(f.F.Actor.Object, new StringStack("usable 701")));
		var before = f.F.Balances[f.F.Resources[1]]; var bank = f.Persisted; var receipts = f.F.Store.Operations.Count;
		var result = focus ? f.F.Service.CastDeviceFocus(new(f.F.Actor.Object, 1, 2, 2, false, "self"), f.Item.Object) : f.F.Service.ActivateDevice(f.F.Actor.Object, f.Item.Object, "self");
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
		Assert.AreEqual(before, f.F.Balances[f.F.Resources[1]]); Assert.AreEqual(bank, f.Persisted);
		Assert.AreEqual(receipts, f.F.Store.Operations.Count); Assert.IsNull(f.Device.Reservation);
		prog.Verify(x => x.Execute(It.IsAny<object[]>()), Times.AtLeastOnce);
	}

	[TestMethod]
	[DataRow(false, "custody")]
	[DataRow(false, "entitlement")]
	[DataRow(true, "custody")]
	[DataRow(true, "entitlement")]
	public void ActualFinalTargetFilter_TrueAfterMutation_RefusesWithoutPaymentBankOrJournal(bool focus, string mutation)
	{
		var f = new Fixture(); var source = (MagicSpell)f.F.Spells.Single(x => x.Id == 2);
		var calls = 0; var commitCall = focus ? 3 : 2;
		var prog = SuccessfulTargetPolicy(702, () =>
		{
			if (++calls == commitCall) { if (mutation == "custody") f.Held.Clear(); else f.F.ActiveCapabilities.Clear(); }
		});
		f.F.World.SetupGet(x => x.FutureProgs).Returns(MagicCastingFixture.Collection(() => new[] { prog.Object }));
		var trigger = new XElement("Trigger", new XAttribute("type", "character"), new XElement("MinimumPower", 0), new XElement("MaximumPower", 10), new XElement("CanTargetSelf", true), new XElement("TargetFilterProg", 702));
		source.Trigger = SpellTriggerFactory.LoadTrigger(trigger, source);
		f.F.Actor.Setup(x => x.TargetActorOrCorpse(It.IsAny<string>(), It.IsAny<PerceiveIgnoreFlags>())).Returns(f.F.Actor.Object);
		f.Charge(); var before = f.F.Balances[f.F.Resources[1]]; var bank = f.Persisted; var receipts = f.F.Store.Operations.Count;
		var result = focus ? f.F.Service.CastDeviceFocus(new(f.F.Actor.Object, 1, 2, 2, false, "self"), f.Item.Object) : f.F.Service.ActivateDevice(f.F.Actor.Object, f.Item.Object, "self");
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message); Assert.AreEqual(commitCall, calls);
		prog.Verify(x => x.ExecuteWithStatus(out It.Ref<object>.IsAny, It.IsAny<object[]>()), Times.Exactly(commitCall));
		Assert.IsTrue(mutation == "custody" ? f.Held.Count == 0 : f.F.ActiveCapabilities.Count == 0,
			"The successful final target policy must actually mutate custody or entitlement.");
		Assert.AreEqual(before, f.F.Balances[f.F.Resources[1]]); Assert.AreEqual(bank, f.Persisted);
		Assert.AreEqual(receipts, f.F.Store.Operations.Count); Assert.IsNull(f.Device.Reservation);
	}

	[TestMethod]
	public void BankMutationsAndRefusal_DoNotQueueOrdinarySaves_AndPersistenceDispatchIsOverridden()
	{
		var f = new Fixture(); f.Charge(); var token = f.Device.NextCharge!.Value;
		Assert.IsTrue(f.Device.Reserve(token)); f.Device.Release(token); Assert.IsFalse(f.Device.Changed);
		f.F.World.Verify(x => x.SaveManager.Add(f.Device), Times.Never);
		foreach (var method in new[] { "Save", "CheckPrototypeForUpdate" })
		{
			var implementation = typeof(ChargedMagicDeviceGameItemComponent).GetMethod(method)!;
			Assert.AreEqual(typeof(ChargedMagicDeviceGameItemComponent), implementation.DeclaringType);
			Assert.AreEqual(typeof(GameItemComponent).GetMethod(method)!.GetBaseDefinition(), implementation.GetBaseDefinition());
		}
	}

	[TestMethod]
	public void Production_AcquiredConfiguredRoute_PaysPerChargeWithoutManifestationOrVancianEnrolment()
	{
		var f = new Fixture(); f.Charge(2);
		Assert.AreEqual(80.0, f.F.Balances[f.F.Resources[1]]);
		Assert.AreEqual(2, f.Device.Charges); Assert.AreEqual(2, f.Device.Grade);
		Assert.AreEqual(0, f.F.Rolls); Assert.AreEqual(0, f.F.SkillUses); Assert.AreEqual(0, f.F.Samples);
		Assert.AreEqual(0, f.F.Store.Opportunities.Count);
		f.F.Actor.Verify(x => x.AddEffect(It.IsAny<SpellBlindnessEffect>()), Times.Never);
		Assert.IsFalse(f.F.ActiveCapabilities.Any(x => x is IVancianMagicCapability));
	}

	[TestMethod]
	[DataRow("custody")]
	[DataRow("configuration")]
	[DataRow("entitlement")]
	public void Focus_FinalPaymentRevalidation_RefusesCallbackChanges(string mutation)
	{
		var f = new Fixture();
		f.F.Checkpoint = boundary =>
		{
			if (boundary != "BeforePayment") return;
			if (mutation == "custody") f.Held.Clear();
			if (mutation == "configuration") f.Proto.BuildingCommand(f.F.Actor.Object, new StringStack("role charged"));
			if (mutation == "entitlement") f.F.ActiveCapabilities.Clear();
		};
		f.Bind();
		var before = f.F.Balances[f.F.Resources[1]];
		var result = f.F.Service.CastDeviceFocus(new(f.F.Actor.Object, 1, 2, 2, false, ""), f.Item.Object);
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
		Assert.AreEqual(before, f.F.Balances[f.F.Resources[1]]);
		Assert.AreEqual(0, f.F.Store.Operations.Count); Assert.AreEqual(0, f.F.SkillUses);
	}

	[TestMethod]
	public void ChargedRelease_FrozenPotency_NoPersonalPaymentNoSkillOrMasteryGain()
	{
		var f = new Fixture(); f.Charge();
		var before = f.F.Balances[f.F.Resources[1]]; var acquired = f.F.Store.Acquisition(100, 2);
		var duration = f.Device.Snapshot!.Numbers.Evaluate("duration", ((MagicSpell)f.F.Spells.Single(x => x.Id == 2)).EffectDurationExpression!,
			f.F.Actor.Object, f.F.Traits[0], TraitBonusContext.SpellDuration, []);
		f.F.Skills[1] = 999;
		Assert.AreEqual(duration, f.Device.Snapshot.Numbers.Evaluate("duration", ((MagicSpell)f.F.Spells.Single(x => x.Id == 2)).EffectDurationExpression!,
			f.F.Actor.Object, f.F.Traits[0], TraitBonusContext.SpellDuration, []));
		var result = f.F.Service.ActivateDevice(f.F.Actor.Object, f.Item.Object, "self");
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(0, f.Device.Charges); Assert.AreEqual(before, f.F.Balances[f.F.Resources[1]]);
		Assert.AreEqual(acquired, f.F.Store.Acquisition(100, 2)); Assert.AreEqual(0, f.F.Store.Opportunities.Count);
		Assert.AreEqual(0, f.F.Rolls); Assert.AreEqual(0, f.F.SkillUses); Assert.AreEqual(0, f.F.Samples);
		f.F.Actor.Verify(x => x.AddEffect(It.IsAny<SpellBlindnessEffect>()), Times.Once);
	}

	[TestMethod]
	public void StockCasterAndAnyoneProfiles_RecheckCurrentEntitlement()
	{
		var f = new Fixture(); f.Charge(); f.F.ActiveCapabilities.Clear();
		var rejected = f.F.Service.ActivateDevice(f.F.Actor.Object, f.Item.Object, "self");
		Assert.AreEqual(MagicCastingStatus.Refused, rejected.Status); Assert.AreEqual(1, f.Device.Charges);
		Assert.IsTrue(f.Proto.BuildingCommand(f.F.Actor.Object, new StringStack("eligibility anyone")));
		Assert.AreEqual(MagicCastingStatus.Succeeded, f.F.Service.ActivateDevice(f.F.Actor.Object, f.Item.Object, "self").Status);
		Assert.AreEqual(0, f.F.SkillUses); Assert.AreEqual(0, f.F.Store.Opportunities.Count);
	}

	[TestMethod]
	public void MagicTypeAndAcquiredProfiles_RefuseWrongTypeAndStaleKnowledge()
	{
		var f = new Fixture(); f.Charge();
		Assert.IsTrue(f.Proto.BuildingCommand(f.F.Actor.Object, new StringStack("capability Sorcerer")));
		Assert.IsTrue(f.Proto.BuildingCommand(f.F.Actor.Object, new StringStack("eligibility magictype")));
		f.F.ActiveCapabilities.Remove(f.F.Sorcerer);
		Assert.AreEqual(MagicCastingStatus.Refused, f.F.Service.ActivateDevice(f.F.Actor.Object, f.Item.Object, "self").Status);
		Assert.IsTrue(f.Proto.BuildingCommand(f.F.Actor.Object, new StringStack("eligibility acquiredspell")));
		f.F.Store.Acquired.Remove((100, 2));
		Assert.AreEqual(MagicCastingStatus.Refused, f.F.Service.ActivateDevice(f.F.Actor.Object, f.Item.Object, "self").Status);
		Assert.AreEqual(1, f.Device.Charges);
	}

	[TestMethod]
	public void DepletedChargedMode_Refuses_ExplicitFocusUsesOrdinaryPaidRoute()
	{
		var f = new Fixture(); f.F.Acquire(2);
		Assert.IsTrue(f.Proto.BuildingCommand(f.F.Actor.Object, new StringStack("spell add 1")));
		Assert.AreEqual(MagicCastingStatus.Refused, f.F.Service.ActivateDevice(f.F.Actor.Object, f.Item.Object, "self").Status);
		Assert.AreEqual(100.0, f.F.Balances[f.F.Resources[1]]);
		var focus = f.F.Service.CastDeviceFocus(new(f.F.Actor.Object, 1, 1, 2, false, "self"), f.Item.Object);
		Assert.AreEqual(MagicCastingStatus.Succeeded, focus.Status, focus.Message);
		Assert.AreEqual(90.0, f.F.Balances[f.F.Resources[1]]); Assert.AreEqual(1, f.F.SkillUses);
	}

	[DataTestMethod]
	[DataRow(0, 1)]
	[DataRow(3, 1)]
	[DataRow(2, 0)]
	[DataRow(2, 6)]
	public void Production_InvalidGradeOrCapacity_RefusesWithoutPayment(int grade, int count)
	{
		var f = new Fixture(); Assert.AreEqual(MagicCastingStatus.Refused, f.Begin(grade, count).Status);
		Assert.AreEqual(100.0, f.F.Balances[f.F.Resources[1]]); Assert.AreEqual(0, f.Device.Charges);
	}

	[TestMethod]
	public void Recharge_QualifiedProducerFillsOnlyMissingCapacity_RejectsWeakerNumerics()
	{
		var f = new Fixture(); f.Charge(2); var original = f.Device.Snapshot!.PotencyFingerprint;
		f.F.Skills[1]--;
		Assert.AreEqual(MagicCastingStatus.Refused, f.Begin(count: 1).Status);
		f.F.Skills[1]++;
		f.Charge(1); Assert.AreEqual(3, f.Device.Charges); Assert.AreEqual(original, f.Device.Snapshot.PotencyFingerprint);
		Assert.AreEqual(MagicCastingStatus.Refused, f.Begin(grade: 1).Status);
		Assert.AreEqual(70.0, f.F.Balances[f.F.Resources[1]]);
	}

	[TestMethod]
	public void Recharge_RawProficiencyCannotBeSubstitutedByTemporaryBonus()
	{
		var f = new Fixture(); f.Charge();
		var captured = f.F.Skills[1]; f.F.Skills[1] = 1;
		f.F.Actor.Setup(x => x.TraitValue(f.F.Traits[0], It.IsAny<TraitBonusContext>())).Returns(captured);
		Assert.AreEqual(MagicCastingStatus.Refused, f.Begin().Status);
		Assert.AreEqual(1, f.Device.Charges);
	}

	[TestMethod]
	public void Recharge_StrongerDeterministicDamage_ReusesOriginalHomogeneousPotency()
	{
		var f = new Fixture("<Effect type='damage'><DamageType>0</DamageType><DamageExpression>variable+grade+mastery+outcome</DamageExpression></Effect>");
		f.Charge(); var fingerprint = f.Device.Snapshot!.PotencyFingerprint;
		f.F.Skills[1] += 20;
		var prior = f.F.Store.Acquisition(100, 2)!; f.F.Store.Write(acquired: prior with { ControlledGrade = 3 });
		f.Charge(); Assert.AreEqual(2, f.Device.Charges); Assert.AreEqual(fingerprint, f.Device.Snapshot.PotencyFingerprint);
		var release = f.Device.Snapshot.CreateSpell(f.F.World.Object, false);
		Assert.AreEqual(48.0, ScrollSpellCompatibility.Expressions(release.SpellEffects.Single()).Single().Expression.EvaluateWith(f.F.Actor.Object, values: [("outcome", 2)]));
	}

	[TestMethod]
	public void Recharge_NonmonotonicFormula_RejectsStrongerSkillWhenItsPayloadIsWeaker()
	{
		var f = new Fixture("<Effect type='damage'><DamageType>0</DamageType><DamageExpression>100-variable+grade+outcome</DamageExpression></Effect>");
		f.Charge(); f.F.Skills[1] += 20;
		Assert.AreEqual(MagicCastingStatus.Refused, f.Begin().Status);
		Assert.AreEqual(1, f.Device.Charges);
	}

	[TestMethod]
	public void AdditionalMinimumGrade_IsItemConfiguredAndRecheckedLive()
	{
		var f = new Fixture(); f.Charge(2);
		Assert.IsTrue(f.Proto.BuildingCommand(f.F.Actor.Object, new StringStack("eligibility anyone")));
		Assert.IsTrue(f.Proto.BuildingCommand(f.F.Actor.Object, new StringStack("mingrade 3")));
		Assert.AreEqual(MagicCastingStatus.Refused, f.F.Service.ActivateDevice(f.F.Actor.Object, f.Item.Object, "self").Status);
		var prior = f.F.Store.Acquisition(100, 2)!; f.F.Store.Write(acquired: prior with { ControlledGrade = 3 });
		Assert.AreEqual(MagicCastingStatus.Succeeded, f.F.Service.ActivateDevice(f.F.Actor.Object, f.Item.Object, "self").Status);
		f.F.ActiveCapabilities.Clear(); Assert.AreEqual(MagicCastingStatus.Refused, f.F.Service.ActivateDevice(f.F.Actor.Object, f.Item.Object, "self").Status);
		Assert.AreEqual(1, f.Device.Charges);
	}

	[TestMethod]
	public void Builder_StaffDefaultsAndInvalidInputsPreservePriorValues()
	{
		var f = new Fixture(); Assert.IsTrue(f.Proto.BuildingCommand(f.F.Actor.Object, new StringStack("kind staff")));
		Assert.AreEqual(10, f.Device.Capacity);
		foreach (var text in new[] { "capacity 101", "seconds 0", "kind 99", "mingrade 8" }) Assert.IsFalse(f.Proto.BuildingCommand(f.F.Actor.Object, new StringStack(text)));
		Assert.AreEqual(10, f.Device.Capacity); Assert.AreEqual(MagicDeviceKind.Staff, f.Proto.Kind);
		Assert.AreEqual(60, f.Proto.SecondsPerCharge); Assert.AreEqual(0, f.Proto.MinimumUseGrade);
	}

	[TestMethod]
	public void Production_MissingKnowledgeRouteOrReserve_Refuses()
	{
		var f = new Fixture(); f.F.Store.Acquired.Remove((100, 2));
		Assert.AreEqual(MagicCastingStatus.Refused, f.Begin().Status);
		f.F.Store.Write(acquired: new(100, 2, 2, 1, f.F.Now, "restored", DateTime.UnixEpoch, 0));
		f.F.ActiveCapabilities.Clear(); Assert.AreEqual(MagicCastingStatus.Refused, f.Begin().Status);
		f.F.ActiveCapabilities.Add(f.F.Earth); f.F.Balances[f.F.Resources[1]] = 0;
		Assert.AreEqual(MagicCastingStatus.Refused, f.Begin().Status);
	}

	[TestMethod]
	public void InterruptedWork_PaidOnce_NoChargeOrRefund()
	{
		var f = new Fixture(); var result = f.Begin(); Assert.AreEqual(MagicCastingStatus.Started, result.Status, result.Message);
		Assert.AreEqual(90.0, f.F.Balances[f.F.Resources[1]]);
		var cancelled = f.F.Service.CancelDeviceProduction(f.F.Actor.Object, result.OperationId!.Value);
		Assert.AreEqual(MagicCastingStatus.Failed, cancelled.Status); Assert.AreEqual(0, f.Device.Charges);
		Assert.AreEqual(90.0, f.F.Balances[f.F.Resources[1]]);
		f.F.Now += TimeSpan.FromSeconds(100);
		Assert.AreEqual(MagicCastingStatus.Refused, f.F.Service.CompleteDeviceProduction(f.F.Actor.Object, result.OperationId.Value).Status);
	}

	[TestMethod]
	public void CapabilityLostAtCompletion_PaidWorkCancelsWithoutCharges()
	{
		var f = new Fixture(); var start = f.Begin(); Assert.AreEqual(MagicCastingStatus.Started, start.Status, start.Message);
		f.F.ActiveCapabilities.Clear(); f.F.Now += TimeSpan.FromSeconds(60);
		Assert.AreEqual(MagicCastingStatus.Failed, f.F.Service.CompleteDeviceProduction(f.F.Actor.Object, start.OperationId!.Value).Status);
		Assert.AreEqual(0, f.Device.Charges); Assert.AreEqual(90.0, f.F.Balances[f.F.Resources[1]]);
	}

	[DataTestMethod]
	[DataRow("DevicePaying")]
	[DataRow("DevicePaid")]
	public void ProductionFailure_RestartRefusesReplayAndRefund(string boundary)
	{
		var f = new Fixture(); f.F.Checkpoint = stage => { if (stage == boundary) throw new InvalidOperationException("injected production failure"); };
		var start = f.Begin(); Assert.AreEqual(MagicCastingStatus.NeedsReview, start.Status, start.Message);
		if (f.Persisted.Length == 0) f.F.Service.DevicePersistence!(f.Device);
		f.Restart(); f.F.Checkpoint = null;
		Assert.AreEqual(MagicCastingStatus.Refused, f.Begin().Status); Assert.AreEqual(0, f.Device.Charges);
	}

	[DataTestMethod]
	[DataRow("DeviceFilling")]
	[DataRow("DeviceFilled")]
	public void FillingFailure_UsableChargesRemainQuarantinedAfterRestart(string boundary)
	{
		var f = new Fixture(); var start = f.Begin(); Assert.AreEqual(MagicCastingStatus.Started, start.Status, start.Message);
		f.F.Checkpoint = stage => { if (stage == boundary) throw new InvalidOperationException("injected fill failure"); };
		f.F.Now += TimeSpan.FromSeconds(60);
		Assert.AreEqual(MagicCastingStatus.NeedsReview, f.F.Service.CompleteDeviceProduction(f.F.Actor.Object, start.OperationId!.Value).Status);
		f.Restart(); f.F.Checkpoint = null;
		Assert.AreEqual(MagicCastingStatus.Refused, f.F.Service.ActivateDevice(f.F.Actor.Object, f.Item.Object, "self").Status);
		Assert.AreEqual(MagicCastingStatus.Refused, f.Begin().Status);
	}

	[TestMethod]
	public void RestartDuringPaidProduction_NeverResumesOrRefills()
	{
		var f = new Fixture(); var start = f.Begin(); Assert.AreEqual(MagicCastingStatus.Started, start.Status, start.Message);
		f.Restart(); f.F.Now += TimeSpan.FromSeconds(120);
		Assert.AreEqual(MagicCastingStatus.Refused, f.F.Service.CompleteDeviceProduction(f.F.Actor.Object, start.OperationId!.Value).Status);
		Assert.AreEqual(MagicCastingStatus.Refused, f.Begin().Status); Assert.AreEqual(0, f.Device.Charges);
	}

	[DataTestMethod]
	[DataRow("DeviceConsuming")]
	[DataRow("DeviceConsumed")]
	public void ActivationFailure_ChargeTombstonePreventsReplayAfterRestart(string boundary)
	{
		var f = new Fixture(); f.Charge();
		f.F.Checkpoint = stage => { if (stage == boundary) throw new InvalidOperationException("injected activation failure"); };
		Assert.AreEqual(MagicCastingStatus.NeedsReview, f.F.Service.ActivateDevice(f.F.Actor.Object, f.Item.Object, "self").Status);
		f.Restart(); f.F.Checkpoint = null;
		Assert.AreEqual(MagicCastingStatus.Refused, f.F.Service.ActivateDevice(f.F.Actor.Object, f.Item.Object, "self").Status);
		f.F.Actor.Verify(x => x.AddEffect(It.IsAny<SpellBlindnessEffect>()), Times.Never);
	}

	[TestMethod]
	public void NormalCopy_IsEmpty_ReloadedDuplicateCannotReplayConsumedCharge()
	{
		var f = new Fixture(); f.Charge(); var saved = f.Device.Export().ToString();
		var copy = (ChargedMagicDeviceGameItemComponent)f.Device.Copy(f.Item.Object, true);
		Assert.AreEqual(0, copy.Charges); Assert.IsNull(copy.Snapshot); Assert.IsNull(copy.Reservation);
		Assert.AreEqual(MagicCastingStatus.Succeeded, f.F.Service.ActivateDevice(f.F.Actor.Object, f.Item.Object, "self").Status);
		var duplicate = new ChargedMagicDeviceGameItemComponent(new() { Id = 11, Definition = saved }, f.Proto, f.Item.Object);
		f.Item.Setup(x => x.GetItemType<IChargedMagicDevice>()).Returns(duplicate);
		Assert.AreEqual(MagicCastingStatus.Refused, f.F.Service.ActivateDevice(f.F.Actor.Object, f.Item.Object, "self").Status);
	}

	[TestMethod]
	public void InvalidTargetOrSpeechOrHeldInput_LeavesChargeIntact()
	{
		var f = new Fixture(); f.Charge();
		Assert.AreEqual(MagicCastingStatus.Refused, f.F.Service.ActivateDevice(f.F.Actor.Object, f.Item.Object, "self extra").Status);
		f.F.Body.Setup(x => x.Communications.CanVocalise(f.F.Body.Object)).Returns(false);
		Assert.AreEqual(MagicCastingStatus.Refused, f.F.Service.ActivateDevice(f.F.Actor.Object, f.Item.Object, "self").Status);
		f.F.Body.Setup(x => x.Communications.CanVocalise(f.F.Body.Object)).Returns(true); f.Held.Clear();
		Assert.AreEqual(MagicCastingStatus.Refused, f.F.Service.ActivateDevice(f.F.Actor.Object, f.Item.Object, "self").Status);
		Assert.AreEqual(1, f.Device.Charges);
	}

	[TestMethod]
	public void FrozenDamageHealAndStatusAdapters_CaptureReleaseAndXmlRoundTrip()
	{
		foreach (var xml in new[] {
			"<Effect type='damage'><DamageType>0</DamageType><DamageExpression>variable+grade+mastery+outcome</DamageExpression></Effect>",
			"<Effect type='heal'><HealWorstWoundsFirst>true</HealWorstWoundsFirst><HealOverflow>true</HealOverflow><HealingAmount>variable+grade+mastery+outcome</HealingAmount></Effect>",
			"<Effect type='blindness'/>" })
		{
			var f = new Fixture(xml); f.Charge(); f.Restart();
			Assert.IsNull(f.Device.DataError); var spell = f.Device.Snapshot!.CreateSpell(f.F.World.Object, false);
			foreach (var (_, expression, _) in ScrollSpellCompatibility.Expressions(spell.SpellEffects.Single()))
			{
				Assert.AreEqual(48.0, expression.EvaluateWith(f.F.Actor.Object, values: [("outcome", 2)]));
				f.F.Skills[1] = 999;
				Assert.AreEqual(49.0, expression.EvaluateWith(f.F.Actor.Object, values: [("outcome", 3)]));
			}
		}
	}

	[TestMethod]
	public void UnsupportedCarrier_RejectsBeforePayment_CorruptBankPreservesBytes()
	{
		var f = new Fixture("<Effect type='waterbreathing'/>");
		Assert.IsTrue(f.Proto.BuildingCommand(f.F.Actor.Object, new StringStack("spell remove 2")));
		Assert.AreEqual(MagicCastingStatus.Refused, f.Begin().Status);
		var invalid = "<ChargedMagicDevice version='99'/>";
		var disabled = new ChargedMagicDeviceGameItemComponent(new() { Id = 7, Definition = invalid }, f.Proto, f.Item.Object);
		Assert.IsNotNull(disabled.DataError); Assert.AreEqual(invalid, VancianItemTests.Serialize(disabled));
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void ChargedRelease_LiveWardOrResistance_BlocksPayloadAfterOneCharge(bool useResistance)
	{
		var f = new Fixture();
		var source = (MagicSpell)f.F.Spells.Single(x => x.Id == 2);
		Assert.IsTrue(source.BuildingCommand(f.F.Actor.Object, new StringStack("trigger new character")));
		if (useResistance) Assert.IsTrue(source.BuildingCommand(f.F.Actor.Object, new StringStack("resist 3 normal")));
		f.Charge();
		var target = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock };
		target.SetupGet(x => x.Id).Returns(101); target.SetupGet(x => x.InstanceId).Returns(101);
		target.SetupGet(x => x.Gameworld).Returns(f.F.World.Object); target.SetupGet(x => x.Location).Returns(f.F.Actor.Object.Location);
		target.SetupGet(x => x.Body.BasePlanarPresence).Returns(MudSharp.Planes.PlanarPresenceDefinition.DefaultMaterial(1));
		f.F.Actor.Setup(x => x.TargetActorOrCorpse("recipient", It.IsAny<PerceiveIgnoreFlags>())).Returns(target.Object);
		target.Setup(x => x.EffectsOfType<IMagicInterdictionEffect>(It.IsAny<Predicate<IMagicInterdictionEffect>>())).Returns([]);
		if (useResistance)
		{
			var resistance = new Mock<ICheck>();
			resistance.Setup(x => x.CheckAgainstAllDifficulties(target.Object, It.IsAny<Difficulty>(), It.IsAny<ITraitDefinition>(),
				It.IsAny<IPerceivable>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
				.Returns(() => Enum.GetValues<Difficulty>().ToDictionary(x => x, _ => CheckOutcome.SimpleOutcome(CheckType.ResistMagicSpellCheck, Outcome.MajorPass)));
			f.F.World.Setup(x => x.GetCheck(CheckType.ResistMagicSpellCheck)).Returns(resistance.Object);
		}
		else
		{
			var ward = new Mock<IMagicInterdictionEffect>(); ward.SetupGet(x => x.Coverage).Returns(MagicInterdictionCoverage.Incoming);
			ward.SetupGet(x => x.Mode).Returns(MagicInterdictionMode.Fail); ward.Setup(x => x.ShouldInterdict(f.F.Actor.Object, f.F.School)).Returns(true);
			target.Setup(x => x.EffectsOfType<IMagicInterdictionEffect>(It.IsAny<Predicate<IMagicInterdictionEffect>>())).Returns([ward.Object]);
		}
		Assert.AreEqual(MagicCastingStatus.Failed, f.F.Service.ActivateDevice(f.F.Actor.Object, f.Item.Object, "recipient").Status);
		Assert.AreEqual(0, f.Device.Charges); target.Verify(x => x.AddEffect(It.IsAny<SpellBlindnessEffect>()), Times.Never);
		Assert.AreEqual(0, f.F.SkillUses); Assert.AreEqual(0, f.F.Samples);
	}

	[TestMethod]
	public void CommittedControlFailure_SpendsChargeWithoutSpellImprovement()
	{
		var f = new Fixture(); f.Charge();
		Assert.IsTrue(f.Proto.BuildingCommand(f.F.Actor.Object, new StringStack("check 3 normal pass")));
		f.F.Check.Setup(x => x.Check(f.F.Actor.Object, It.IsAny<Difficulty>(), f.F.Traits[2], It.IsAny<IPerceivable>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
			.Returns(CheckOutcome.SimpleOutcome(CheckType.CastSpellCheck, Outcome.MajorFail));
		Assert.AreEqual(MagicCastingStatus.Failed, f.F.Service.ActivateDevice(f.F.Actor.Object, f.Item.Object, "self").Status);
		Assert.AreEqual(0, f.Device.Charges); Assert.AreEqual(0, f.F.SkillUses);
		f.F.Actor.Verify(x => x.AddEffect(It.IsAny<SpellBlindnessEffect>()), Times.Never);
	}

	[TestMethod]
	public void ChangedNumericalXml_DisablesEntireBankWithoutDiscardingEvidence()
	{
		var f = new Fixture(); f.Charge(); var xml = f.Device.Export();
		xml.Descendants("Numbers").Single().SetAttributeValue("grade", 7);
		var disabled = new ChargedMagicDeviceGameItemComponent(new() { Id = 7, Definition = xml.ToString() }, f.Proto, f.Item.Object);
		Assert.IsNotNull(disabled.DataError); Assert.AreEqual(xml.ToString(), VancianItemTests.Serialize(disabled));
	}
}
