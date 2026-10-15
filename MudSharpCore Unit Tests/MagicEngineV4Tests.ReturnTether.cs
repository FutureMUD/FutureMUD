#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.Lifecycle;

namespace MudSharp_Unit_Tests;

public partial class MagicEngineV4Tests
{
	[DataTestMethod]
	[DataRow(1)] [DataRow(2)] [DataRow(3)] [DataRow(4)] [DataRow(5)] [DataRow(6)] [DataRow(7)]
	public void ReturnTether_AllGradesRecordAbsoluteDeadlineAndReturnExactlyOnce(int grade)
	{
		var f = new ReturnFixture(); using var clock = RuntimeClock.Push(f.Clock);
		var result = f.Cast(grade);
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		var child = f.Child;
		Assert.AreEqual(f.Clock.Now.AddSeconds(30 * grade), child.Binding.DeadlineUtc);
		Assert.AreEqual(f.Pair.Source.Object.Id, child.Binding.Anchor.RoomId);
		Assert.AreEqual(0, f.Pair.Moves); Assert.AreEqual(1, f.Rows.Count);
		f.WalkAway(); f.Clock.Now = child.Binding.DeadlineUtc.AddTicks(-1); child.CheckReturn(); Assert.AreEqual(0, f.Pair.Moves);
		f.Clock.Now = child.Binding.DeadlineUtc; child.CheckReturn();
		Assert.AreEqual(1, f.Pair.Moves); Assert.AreSame(f.Pair.Source.Object, f.Pair.CasterLocation);
		Assert.AreEqual(SpellLifecycleState.Completed, f.Rows[child.Binding.Id].State);
		child.CheckReturn(); f.Service.Reconcile(f.Clock.Now); Assert.AreEqual(1, f.Pair.Moves);
		Assert.IsFalse(f.Pair.CasterEffects.Contains(child)); Assert.AreEqual(0, f.Rows[child.Binding.Id].Entities.Count);
	}

	[DataTestMethod]
	[DataRow("body")] [DataRow("endpoint")] [DataRow("dispel")] [DataRow("logout")]
	public void ReturnTether_InvalidatedIdentityEndpointOrAttachmentCancelsWithoutMovement(string reason)
	{
		var f = new ReturnFixture(); using var clock = RuntimeClock.Push(f.Clock);
		Assert.AreEqual(MagicCastingStatus.Succeeded, f.Cast(1).Status);
		var child = f.Child; f.WalkAway();
		if (reason == "body") f.Pair.F.Body.SetupGet(x => x.Id).Returns(999);
		if (reason == "endpoint") Mock.Get(f.Pair.Source.Object.CurrentOverlay).SetupGet(x => x.Id).Returns(999);
		if (reason == "dispel") f.Pair.F.Actor.Object.RemoveEffect(child, true);
		if (reason == "logout") f.Pair.F.Actor.Raise(x => x.OnQuit += null!, f.Pair.F.Actor.Object);
		f.Clock.Now = child.Binding.DeadlineUtc; child.CheckReturn();
		Assert.AreEqual(0, f.Pair.Moves); Assert.AreEqual(SpellLifecycleState.Completed, f.Rows[child.Binding.Id].State);
	}

	[TestMethod]
	public void ReturnTether_UncertainClaimNeverReplaysOrMoves()
	{
		var f = new ReturnFixture(); using var clock = RuntimeClock.Push(f.Clock);
		Assert.AreEqual(MagicCastingStatus.Succeeded, f.Cast(1).Status);
		var child = f.Child; f.WalkAway(); f.ThrowAfterClaim = true; f.Clock.Now = child.Binding.DeadlineUtc;
		child.CheckReturn(); child.CheckReturn(); f.Service.Reconcile(f.Clock.Now);
		Assert.AreEqual(0, f.Pair.Moves); Assert.AreEqual(SpellLifecycleState.Retiring, f.Rows[child.Binding.Id].State);
		StringAssert.Contains(f.Rows[child.Binding.Id].Diagnostic, "no replay");
	}

	[TestMethod]
	public void ReturnTether_UncertainUncommittedClaimRemainsBlockedAfterReload()
	{
		var f = new ReturnFixture(); using var clock = RuntimeClock.Push(f.Clock);
		Assert.AreEqual(MagicCastingStatus.Succeeded, f.Cast(1).Status);
		var child = f.Child; f.WalkAway(); f.ThrowBeforeClaim = true; f.Clock.Now = child.Binding.DeadlineUtc;
		child.CheckReturn();
		Assert.AreEqual(SpellLifecycleState.Active, f.Rows[child.Binding.Id].State);
		StringAssert.StartsWith(f.Rows[child.Binding.Id].Diagnostic, SpellReturnService.BlockedDiagnosticPrefix);
		f.ThrowBeforeClaim = false;
		SpellReturnTetherEffect.InitialiseEffectType();
		var loaded = (SpellReturnTetherEffect)Effect.LoadEffect(child.SaveToXml(new Dictionary<IEffect, TimeSpan>()), f.Pair.F.Actor.Object);
		var parent = new MagicSpellParent(f.Pair.F.Actor.Object, f.Pair.F.Spells.Single(), f.Pair.F.Actor.Object);
		loaded.ParentEffect = parent; parent.AddSpellEffect(loaded); f.Pair.CasterEffects.Add(parent);
		f.Pair.CasterEffects.Add(loaded);
		loaded.CheckReturn(); f.Service.Reconcile(f.Clock.Now);
		Assert.AreEqual(0, f.Pair.Moves);
		Assert.AreEqual(SpellLifecycleState.Active, f.Rows[child.Binding.Id].State);
	}

	[TestMethod]
	public void ReturnTether_RealJournalReaderAdmitsOnlyExactUnownedReturnOrigins()
	{
		var now = new DateTime(2026, 10, 15, 10, 0, 0, DateTimeKind.Utc);
		var anchor = new SpellReturnAnchor(1, 2, 3, 4, 5, 6, RoomLayer.GroundLevel);
		var row = new MudSharp.Models.MagicSpellLifecycle
		{
			Id = Guid.NewGuid(), SpellId = 1, Grade = 1, CreatorId = 1, Family = SpellReturnAnchor.Family,
			Mode = (int)SpellLifecycleMode.TemporaryCleanup, CreatedUtc = now, DeadlineUtc = now.AddSeconds(30),
			Provenance = anchor.Save(), State = (int)SpellLifecycleState.Active, Version = 1, UpdatedUtc = now, Diagnostic = ""
		};
		SpellOwnedLifecycleStore.ValidateReturnJournal(SpellOwnedLifecycleStore.Read(row).Origin);
		row.CreatorId = 99;
		Assert.ThrowsException<InvalidOperationException>(() => SpellOwnedLifecycleStore.Read(row));
		row.CreatorId = 1; row.Family = "ordinary-owned-spell";
		Assert.ThrowsException<InvalidOperationException>(() => SpellOwnedLifecycleStore.Read(row));
		row.Family = SpellReturnAnchor.Family;
		row.Entities.Add(new() { Kind = (int)SpellOwnedEntityKind.Body, EntityId = 3, Role = (int)SpellOwnedEntityRole.CreatedEntity });
		Assert.ThrowsException<InvalidOperationException>(() => SpellOwnedLifecycleStore.Read(row));
	}

	[TestMethod]
	public void ReturnTether_UncertainNativeArrivalHoldsJournalWithoutReplay()
	{
		var f = new ReturnFixture(); using var clock = RuntimeClock.Push(f.Clock);
		Assert.AreEqual(MagicCastingStatus.Succeeded, f.Cast(1).Status);
		var child = f.Child; f.WalkAway(); f.Pair.RefuseCasterRollback = true;
		// The first native call moves, then the completion acknowledgement fails.
		f.ThrowAfterComplete = true; f.Clock.Now = child.Binding.DeadlineUtc; child.CheckReturn(); child.CheckReturn();
		Assert.AreEqual(1, f.Pair.Moves); Assert.AreSame(f.Pair.Source.Object, f.Pair.CasterLocation);
		Assert.AreEqual(SpellLifecycleState.Retiring, f.Rows[child.Binding.Id].State);
		StringAssert.Contains(f.Rows[child.Binding.Id].Diagnostic, "no replay");
	}

	[TestMethod]
	public void ReturnTether_SavedChildRetainsOriginBodyPositionAndAbsoluteDeadline()
	{
		var f = new ReturnFixture(); using var clock = RuntimeClock.Push(f.Clock);
		Assert.AreEqual(MagicCastingStatus.Succeeded, f.Cast(3).Status);
		var child = f.Child; var xml = child.SaveToXml(new Dictionary<IEffect, TimeSpan>());
		SpellReturnTetherEffect.InitialiseEffectType();
		var loaded = (SpellReturnTetherEffect)Effect.LoadEffect(xml, f.Pair.F.Actor.Object);
		Assert.AreEqual(child.Binding, loaded.Binding);
		f.Clock.Now = f.Clock.Now.AddSeconds(40);
		Assert.AreEqual(child.Binding.DeadlineUtc, loaded.ExpiryUtc); Assert.AreEqual(0, f.Pair.Moves);
	}

	[TestMethod]
	public void ReturnTether_FrozenPositionChangeBeforePaymentRefusesWithoutJournalOrPayment()
	{
		var f = new ReturnFixture(); using var clock = RuntimeClock.Push(f.Clock);
		f.Pair.F.Checkpoint = stage => { if (stage == "BeforePayment") f.WalkAway(); };
		Assert.AreEqual(MagicCastingStatus.Refused, f.Cast(2).Status);
		Assert.AreEqual(0, f.Rows.Count); Assert.AreEqual(100.0, f.Pair.F.Balances[f.Pair.F.Resources[1]]);
	}

	private sealed class ReturnClock : TimeProvider
	{
		public DateTime Now { get; set; } = new(2026, 10, 15, 10, 0, 0, DateTimeKind.Utc);
		public override DateTimeOffset GetUtcNow() => new(Now);
	}
	private sealed class ReturnFixture
	{
		public ExchangeFixture Pair { get; } = new();
		public ReturnClock Clock { get; } = new();
		public Dictionary<Guid, SpellOwnedLifecycle> Rows { get; } = [];
		public SpellReturnService Service { get; }
		public bool ThrowAfterClaim { get; set; }
		public bool ThrowBeforeClaim { get; set; }
		public bool ThrowAfterComplete { get; set; }
		public SpellReturnTetherEffect Child => Pair.CasterEffects.OfType<SpellReturnTetherEffect>().Single();
		public ReturnFixture()
		{
			var f = Pair.F;
			foreach (var (room, id) in new[] { (Pair.Source, 501L), (Pair.Destination, 502L) })
			{
				Mock.Get(room.Object.CurrentOverlay).SetupGet(x => x.Id).Returns(id);
				Mock.Get(room.Object.CurrentOverlay.Terrain).SetupGet(x => x.Id).Returns(id + 100);
			}
			var store = new Mock<ISpellOwnedLifecycleStore>();
			store.Setup(x => x.Find(It.IsAny<Guid>())).Returns<Guid>(id => Rows.GetValueOrDefault(id));
			SpellOwnedLifecycle Read(Guid id, long version)
			{ var life = Rows[id]; if (life.Version != version) throw new InvalidOperationException("Version changed."); return life; }
			store.Setup(x => x.BeginRetirement(It.IsAny<Guid>(), It.IsAny<long>(), It.IsAny<SpellRetirementReason>(), It.IsAny<DateTime>()))
				.Returns<Guid, long, SpellRetirementReason, DateTime>((id, version, reason, now) =>
				{
					if (ThrowBeforeClaim) throw new InvalidOperationException("Claim unavailable before commit.");
					var life = Read(id, version); Rows[id] = life with { State = SpellLifecycleState.Retiring, Reason = reason, UpdatedUtc = now, Version = version + 1 };
					if (ThrowAfterClaim) throw new InvalidOperationException("Lost claim acknowledgement."); return Rows[id];
				});
			store.Setup(x => x.Complete(It.IsAny<Guid>(), It.IsAny<long>(), It.IsAny<DateTime>())).Returns<Guid, long, DateTime>((id, version, now) =>
			{
				var life = Read(id, version); if (ThrowAfterComplete) throw new InvalidOperationException("Completion unavailable.");
				return Rows[id] = life with { State = SpellLifecycleState.Completed, UpdatedUtc = now, Version = version + 1 };
			});
			store.Setup(x => x.Hold(It.IsAny<Guid>(), It.IsAny<long>(), It.IsAny<string>(), It.IsAny<DateTime>())).Returns<Guid, long, string, DateTime>((id, version, diagnostic, now) =>
				Rows[id] = Read(id, version) with { Diagnostic = diagnostic, UpdatedUtc = now, Version = version + 1 });
			Service = new(f.World.Object, store.Object, origin =>
			{
				var life = new SpellOwnedLifecycle(origin, [], SpellLifecycleState.Active, null, null, null, origin.CreatedUtc, 1, ""); Rows.Add(origin.Id, life); return life;
			}, (now, limit) => Rows.Values.Where(x => x.State == SpellLifecycleState.Active && x.Origin.DeadlineUtc <= now).Take(limit).ToArray());
			f.World.SetupGet(x => x.SpellReturns).Returns(Service);
			void Add(IEffect effect) { Pair.CasterEffects.Add(effect); effect.InitialEffect(); }
			f.Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(Add);
			f.Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>())).Callback<IEffect, TimeSpan>((effect, _) => Add(effect));
			f.Actor.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>())).Callback<IEffect, bool>((effect, remove) =>
			{ Pair.CasterEffects.Remove(effect); if (remove) effect.RemovalEffect(); });
			var stock = ArmageddonReturnTetherSpellContent.Create().Content;
			var row = stock.SpellRow(1, 1, 1); row.Id = 1; row.EffectDurationExpressionId = 2; row.Definition = stock.BuildDefinition(10, 1, 0).ToString();
			f.Spells.Clear(); f.Spells.Add(new MagicSpell(row, f.World.Object) { EffectDurationExpression = new MudSharp.Body.Traits.TraitExpression("30*grade", f.World.Object) });
		}
		public MagicCastingResult Cast(int grade) => Pair.F.Service.Cast(new(Pair.F.Actor.Object, Pair.F.Earth.Id, 1, grade, false, "self", OriginId: Guid.NewGuid()));
		public void WalkAway()
		{
			Pair.CasterLocation = Pair.Destination.Object;
			Pair.SourceMembers.RemoveAll(x => ReferenceEquals(x, Pair.F.Actor.Object));
			if (!Pair.DestinationMembers.Any(x => ReferenceEquals(x, Pair.F.Actor.Object))) Pair.DestinationMembers.Add(Pair.F.Actor.Object);
		}
	}
}
