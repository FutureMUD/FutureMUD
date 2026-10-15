#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework.Scheduling;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.Lifecycle;
using MudSharp.RPG.Merits;

namespace MudSharp_Unit_Tests;

public partial class MagicEngineV4Tests
{
	[DataTestMethod]
	[DataRow(1)] [DataRow(2)] [DataRow(3)] [DataRow(4)] [DataRow(5)] [DataRow(6)] [DataRow(7)]
	public void StormLocus_AllGradesClaimBeforeDistinctNativeInjuryAndExpireWithoutTickTraining(int grade)
	{
		var f = new StormFixture(); using var clock = RuntimeClock.Push(f.Clock);
		Assert.AreEqual(MagicCastingStatus.Succeeded, f.Cast(grade).Status);
		var child = f.Child; Assert.AreEqual(f.Clock.Now.AddSeconds(30 * grade), child.ExpiryUtc);
		Assert.AreEqual(0, f.Damage.Count); var samples = f.Pair.F.Samples;
		f.Clock.Now = f.Clock.Now.AddSeconds(10); child.Pulse(); child.Pulse();
		CollectionAssert.AreEqual(new[] { 2.0 * grade, 2.0 * grade }, f.Damage);
		Assert.AreEqual(1, f.Claims); Assert.AreEqual(samples, f.Pair.F.Samples);
		f.Clock.Now = child.ExpiryUtc!.Value; child.Pulse(); child.Pulse();
		Assert.AreEqual(2, f.Damage.Count); Assert.AreEqual(SpellLifecycleState.Completed, f.Life!.State);
	}

	[TestMethod]
	public void StormLocus_UncertainPulseClaimHoldsWithoutDamageOrReplay()
	{
		var f = new StormFixture(); using var clock = RuntimeClock.Push(f.Clock); f.Cast(1); var child = f.Child;
		f.ThrowAfterClaim = true; f.Clock.Now = f.Clock.Now.AddSeconds(10); child.Pulse(); child.Pulse();
		Assert.AreEqual(0, f.Damage.Count); Assert.AreEqual(1, f.Claims);
		StringAssert.StartsWith(f.Life!.Diagnostic, SpellRoomHazardJournal.HeldPrefix);
	}

	[TestMethod]
	public void StormLocus_ReloadSkipsMissedPulseThenContinuesAtDurableCursor()
	{
		var f = new StormFixture(); using var clock = RuntimeClock.Push(f.Clock); f.Cast(2); var child = f.Child;
		SpellStormLocusEffect.InitialiseEffectType();
		var loaded = (SpellStormLocusEffect)Effect.LoadEffect(child.SaveToXml(new Dictionary<IEffect, TimeSpan>()), f.Pair.Source.Object);
		var parent = new MagicSpellParent(f.Pair.Source.Object, f.Pair.F.Spells.Single(), f.Pair.F.Actor.Object);
		loaded.ParentEffect = parent; parent.AddSpellEffect(loaded); f.Effects.Clear(); f.Effects.Add(parent); f.Effects.Add(loaded);
		f.Clock.Now = f.Clock.Now.AddSeconds(25); loaded.Login(); loaded.Pulse();
		Assert.AreEqual(0, f.Damage.Count); Assert.AreEqual(1, f.Claims);
		f.Clock.Now = f.Life!.RoomHazardNextPulseUtc!.Value; loaded.Pulse();
		Assert.AreEqual(2, f.Damage.Count); Assert.AreEqual(2, f.Claims);
	}

	[TestMethod]
	public void StormLocus_AbsentCreatorConsumesPulseWithoutLazyActorLookupOrSubstitution()
	{
		var f = new StormFixture(); using var clock = RuntimeClock.Push(f.Clock); f.Cast(1); var child = f.Child;
		f.Pair.F.World.SetupGet(x => x.Actors).Returns(MagicCastingFixture.Collection(() => new[] { f.Pair.Target.Object }));
		f.Clock.Now = f.Clock.Now.AddSeconds(10); child.Pulse(); Assert.AreEqual(0, f.Damage.Count); Assert.AreEqual(1, f.Claims);
	}

	[TestMethod]
	public void StormLocus_JournalReaderRejectsMissingCursorAndAnyOwnershipClaim()
	{
		var now = new DateTime(2026, 10, 15, 0, 0, 0, DateTimeKind.Utc);
		var anchor = new SpellReturnAnchor(1, 2, 3, 4, 5, 6, MudSharp.Construction.RoomLayer.GroundLevel);
		var row = new MudSharp.Models.MagicSpellLifecycle { Id = Guid.NewGuid(), CreatorId = 1, SpellId = 7, Grade = 1,
			Family = SpellRoomHazardJournal.Family, Mode = (int)SpellLifecycleMode.TemporaryCleanup, CreatedUtc = now, UpdatedUtc = now,
			DeadlineUtc = now.AddSeconds(30), Provenance = anchor.Save(), Version = 1 };
		Assert.ThrowsException<InvalidOperationException>(() => SpellOwnedLifecycleStore.Read(row));
		row.RoomHazardNextPulseUtc = now.AddSeconds(10); Assert.AreEqual(0, SpellOwnedLifecycleStore.Read(row).Entities.Count);
		row.Entities.Add(new() { EntityId = 4, Kind = (int)SpellOwnedEntityKind.Room, Role = (int)SpellOwnedEntityRole.CreatedEntity });
		Assert.ThrowsException<InvalidOperationException>(() => SpellOwnedLifecycleStore.Read(row));
	}

	private sealed class StormFixture
	{
		public ExchangeFixture Pair { get; } = new();
		public ReturnClock Clock { get; } = new();
		public List<IEffect> Effects { get; } = [];
		public List<double> Damage { get; } = [];
		public SpellOwnedLifecycle? Life { get; private set; }
		public int Claims { get; private set; }
		public bool ThrowAfterClaim { get; set; }
		public SpellStormLocusEffect Child => Effects.OfType<SpellStormLocusEffect>().Single();
		public StormFixture()
		{
			var f = Pair.F; Pair.TargetLocation = Pair.Source.Object; Pair.DestinationMembers.Clear(); Pair.SourceMembers.Add(Pair.Target.Object);
			Mock.Get(Pair.Source.Object.CurrentOverlay).SetupGet(x => x.Id).Returns(311);
			Mock.Get(Pair.Source.Object.CurrentOverlay.Terrain).SetupGet(x => x.Id).Returns(312);
			var planes = new[] { 1L, 2L, 3L }.Select(id => Mock.Of<MudSharp.Planes.IPlane>(x => x.Id == id)).ToArray();
			f.World.SetupGet(x => x.DefaultPlane).Returns(planes[0]); f.World.SetupGet(x => x.Planes).Returns(MagicCastingFixture.Collection(() => planes));
			f.World.SetupGet(x => x.Merits).Returns(MagicCastingFixture.Collection<IMerit>(() => new[] { Mock.Of<IMerit>(x => x.Id == 9) }));
			var journal = new Mock<ISpellRoomHazardJournal>(); f.World.SetupGet(x => x.SpellRoomHazards).Returns(journal.Object);
			journal.Setup(x => x.Create(It.IsAny<SpellLifecycleOrigin>())).Returns<SpellLifecycleOrigin>(origin =>
				Life = new(origin, [], SpellLifecycleState.Active, null, null, null, origin.CreatedUtc, 1, "") { RoomHazardNextPulseUtc = origin.CreatedUtc.AddSeconds(10) });
			journal.Setup(x => x.Find(It.IsAny<Guid>())).Returns(() => Life);
			journal.Setup(x => x.ClaimPulse(It.IsAny<Guid>(), It.IsAny<long>(), It.IsAny<DateTime>())).Returns<Guid, long, DateTime>((id, version, now) =>
			{
				Assert.AreEqual(Life!.Version, version); Assert.IsTrue(now >= Life.RoomHazardNextPulseUtc); Claims++;
				Life = Life with { Version = version + 1, UpdatedUtc = now, RoomHazardLastPulseUtc = now,
					RoomHazardNextPulseUtc = now.AddSeconds(10) };
				if (ThrowAfterClaim) throw new InvalidOperationException("Committed pulse acknowledgement lost."); return Life;
			});
			journal.Setup(x => x.Hold(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<string>())).Callback<Guid, DateTime, string>((_, now, error) =>
				Life = Life! with { Diagnostic = SpellRoomHazardJournal.HeldPrefix + error, UpdatedUtc = now });
			journal.Setup(x => x.Retire(It.IsAny<Guid>(), It.IsAny<SpellRetirementReason>(), It.IsAny<DateTime>())).Callback<Guid, SpellRetirementReason, DateTime>((_, _, now) =>
				Life = Life! with { State = SpellLifecycleState.Completed, UpdatedUtc = now });
			Pair.Source.SetupGet(x => x.Effects).Returns(Effects);
			void Add(IEffect effect) { Effects.Add(effect); effect.InitialEffect(); }
			Pair.Source.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(Add);
			Pair.Source.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>())).Callback<IEffect, TimeSpan>((effect, _) => Add(effect));
			Pair.Source.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>())).Callback<IEffect, bool>((effect, fire) => { Effects.Remove(effect); if (fire) effect.RemovalEffect(); });
			var part = Mock.Of<IBodypart>(x => x.Id == 800);
			foreach (var body in new[] { f.Body, Pair.TargetBody })
			{
				body.SetupGet(x => x.Bodyparts).Returns([part]); body.SetupGet(x => x.RandomBodypart).Returns(part); body.SetupGet(x => x.WornItems).Returns([]); body.SetupGet(x => x.Prosthetics).Returns([]);
				body.Setup(x => x.PassiveSufferDamage(It.IsAny<IDamage>())).Returns<IDamage>(packet =>
				{
					Assert.AreEqual(Clock.Now, Life!.RoomHazardLastPulseUtc, "Commit must precede native harm.");
					packet.MutationGuard!.Validate(body.Object, packet); Assert.AreEqual(DamageType.Electrical, packet.DamageType); Damage.Add(packet.DamageAmount); return [];
				});
			}
			var content = ArmageddonStormLocusSpellContent.Create(2, 3, 9).Content;
			var row = content.SpellRow(1, 1, 1); row.Id = 1; row.EffectDurationExpressionId = 2; row.Definition = content.BuildDefinition(10, 1, 0).ToString();
			f.Spells.Clear(); f.Spells.Add(new MagicSpell(row, f.World.Object) { EffectDurationExpression = new TraitExpression("30*grade", f.World.Object) });
		}
		public MagicCastingResult Cast(int grade) => Pair.F.Service.Cast(new(Pair.F.Actor.Object, Pair.F.Earth.Id, 1, grade, false, "here", OriginId: Guid.NewGuid()));
	}
}
