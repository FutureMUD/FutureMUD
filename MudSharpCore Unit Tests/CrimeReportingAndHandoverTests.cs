#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.AIStorytellers;
using MudSharp.RPG.Law;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CrimeReportingAndHandoverTests
{
	[TestMethod]
	public void ExplicitWitnessReport_RecordsNonautomaticLawBeforeCallbacksWithoutRevealingIdentity()
	{
		var f = new Fixture(); var authority = f.NativeAuthority();
		var law = new Mock<ILaw>(); law.SetupGet(x => x.Authority).Returns(authority);
		law.SetupGet(x => x.Gameworld).Returns(f.World.Object); law.SetupGet(x => x.CrimeType).Returns(CrimeTypes.TrafficingContraband);
		law.Setup(x => x.IsCrime(f.Target.Object, null, null, "spice")).Returns(true);
		Set(authority, "_laws", new List<ILaw> { law.Object });
		f.Receiver.Setup(x => x.HandleEvent(EventType.WitnessedCrime, It.IsAny<object[]>()))
			.Callback<EventType, object[]>((_, values) =>
			{
				var crime = (ICrime)values[3];
				Assert.IsTrue(authority.KnownCrimes.Contains(crime));
				Assert.IsFalse(authority.UnknownCrimes.Contains(crime));
				Assert.IsFalse(crime.CriminalIdentityIsKnown);
			}).Returns(false);
		var result = authority.ReportWitnessedCrime(law.Object, f.Target.Object, f.Receiver.Object, null, null, "spice");
		Assert.IsNotNull(result); Assert.IsTrue(result.IsKnownCrime); Assert.IsFalse(result.CriminalIdentityIsKnown);
		Assert.AreSame(law.Object, result.Law);
		CollectionAssert.AreEqual(new[] { f.Receiver.Object.Id }, result.WitnessIds.ToArray());
		Assert.IsTrue(result.WitnessMemories.Single().ReportDelivered);
		f.Receiver.Verify(x => x.HandleEvent(EventType.WitnessedCrime, It.IsAny<object[]>()), Times.Once);
	}

	[TestMethod]
	[DataRow("other authority")]
	[DataRow("not visible")]
	[DataRow("law refuses")]
	[DataRow("law moves target")]
	[DataRow("law detaches itself")]
	public void ExplicitWitnessReport_InvalidSceneOrLaw_DoesNotCreateCrime(string reason)
	{
		var f = new Fixture(); var authority = f.NativeAuthority(); var law = new Mock<ILaw>();
		law.SetupGet(x => x.Authority).Returns(reason == "other authority" ? Mock.Of<ILegalAuthority>() : authority);
		law.Setup(x => x.IsCrime(f.Target.Object, null, null, "spice"))
			.Callback(() =>
			{
				if (reason == "law moves target") f.Target.SetupGet(x => x.Location).Returns(Mock.Of<IRoom>());
				if (reason == "law detaches itself") Set(authority, "_laws", new List<ILaw>());
			})
			.Returns(reason != "law refuses");
		if (reason == "not visible") f.Receiver.Setup(x => x.CanSee(f.Target.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(false);
		Set(authority, "_laws", new List<ILaw> { law.Object });
		Assert.IsNull(authority.ReportWitnessedCrime(law.Object, f.Target.Object, f.Receiver.Object, null, null, "spice"));
		Assert.AreEqual(0, authority.KnownCrimes.Count()); Assert.AreEqual(0, authority.UnknownCrimes.Count());
	}

	[TestMethod]
	public void SubduedBy_RequiresActualControlledNativeGrappleAndItsReciprocalAttachment()
	{
		var f = new Fixture();
		Assert.IsFalse(CustodyHandover.IsSubduedBy(f.Target.Object, f.Holder.Object));
		var grapple = f.Grapple(controlled: false);
		Assert.IsFalse(CustodyHandover.IsSubduedBy(f.Target.Object, f.Holder.Object));
		((List<ILimb>)typeof(Grappling).GetField("_limbsUnderControl", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(grapple)!).Add(f.Limb.Object);
		Assert.IsTrue(CustodyHandover.IsSubduedBy(f.Target.Object, f.Holder.Object));
		f.HolderEffects.Clear();
		Assert.IsFalse(CustodyHandover.IsSubduedBy(f.Target.Object, f.Holder.Object));
	}

	[TestMethod]
	public void Handover_ReleasesRealGrappleBeforeNativeRemandAndLeavesUnrelatedItemsUntouched()
	{
		var f = new Fixture(); f.Grapple();
		f.Authority.Setup(x => x.IncarcerateCriminal(f.Target.Object)).Callback(() =>
		{
			Assert.AreEqual(0, f.HolderEffects.OfType<IGrappling>().Count());
			Assert.AreEqual(0, f.TargetEffects.OfType<IBeingGrappled>().Count());
		});
		Assert.IsTrue(CustodyHandover.TryHandover(f.Holder.Object, f.Target.Object, f.Receiver.Object, f.Authority.Object));
		f.Authority.Verify(x => x.IncarcerateCriminal(f.Target.Object), Times.Once);
		f.TargetBody.Verify(x => x.Take(It.IsAny<IGameItem>()), Times.Never,
			"The wrapper must delegate belongings handling rather than implement another item transfer.");
	}

	[TestMethod]
	[DataRow("release")]
	[DataRow("body replacement")]
	public void Handover_AuthorityCallbackInvalidatesPhysicalControl_RefusesBeforeCleanup(string mutation)
	{
		var f = new Fixture(); f.Grapple();
		f.Authority.Setup(x => x.GetEnforcementAuthority(f.Receiver.Object)).Callback(() =>
		{
			if (mutation == "release") f.HolderEffects.Clear();
			else f.Target.SetupGet(x => x.Body).Returns(new Mock<IBody>().Object);
		}).Returns(f.Role.Object);
		Assert.IsFalse(CustodyHandover.TryHandover(f.Holder.Object, f.Target.Object, f.Receiver.Object, f.Authority.Object));
		f.Authority.Verify(x => x.IncarcerateCriminal(It.IsAny<ICharacter>()), Times.Never);
		f.TargetBody.Verify(x => x.RemoveAllEffects<IRestraintEffect>(It.IsAny<Predicate<IRestraintEffect>>(), true), Times.Never);
	}

	[TestMethod]
	public void Handover_NoWantedCrimeOrNoControl_LeavesCustodyUnchanged()
	{
		var f = new Fixture();
		Assert.IsFalse(CustodyHandover.TryHandover(f.Holder.Object, f.Target.Object, f.Receiver.Object, f.Authority.Object));
		f.Grapple(); f.Authority.Setup(x => x.KnownCrimesForIndividual(f.Target.Object)).Returns([]);
		Assert.IsFalse(CustodyHandover.TryHandover(f.Holder.Object, f.Target.Object, f.Receiver.Object, f.Authority.Object));
		Assert.IsTrue(CustodyHandover.IsSubduedBy(f.Target.Object, f.Holder.Object));
		f.Authority.Verify(x => x.IncarcerateCriminal(It.IsAny<ICharacter>()), Times.Never);
	}

	[TestMethod]
	public void Handover_DepartureDuringGrappleCleanup_DoesNotIncarcerateReplacementScene()
	{
		var f = new Fixture(); f.Grapple();
		f.TargetBody.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), true)).Callback<IEffect, bool>((effect, _) =>
		{
			f.TargetEffects.Remove(effect);
			f.Target.SetupGet(x => x.Location).Returns(Mock.Of<IRoom>());
		});
		Assert.IsFalse(CustodyHandover.TryHandover(f.Holder.Object, f.Target.Object, f.Receiver.Object, f.Authority.Object));
		f.Authority.Verify(x => x.IncarcerateCriminal(It.IsAny<ICharacter>()), Times.Never);
	}

	[TestMethod]
	public void Handover_ReceiverAuthorityRevokedDuringCleanup_DoesNotIncarcerate()
	{
		var f = new Fixture(); f.Grapple();
		f.TargetBody.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), true)).Callback<IEffect, bool>((effect, _) =>
		{
			f.TargetEffects.Remove(effect);
			f.Authority.Setup(x => x.GetEnforcementAuthority(f.Receiver.Object)).Returns((IEnforcementAuthority)null!);
		});
		Assert.IsFalse(CustodyHandover.TryHandover(f.Holder.Object, f.Target.Object, f.Receiver.Object, f.Authority.Object));
		f.Authority.Verify(x => x.IncarcerateCriminal(It.IsAny<ICharacter>()), Times.Never);
	}

	private static void Set(object target, string field, object value) => typeof(LegalAuthority)
		.GetField(field, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(target, value);

	private sealed class Fixture
	{
		public Mock<IFuturemud> World { get; } = new();
		public Mock<ICharacter> Holder { get; } = new();
		public Mock<ICharacter> Target { get; } = new();
		public Mock<ICharacter> Receiver { get; } = new();
		public Mock<IBody> TargetBody { get; } = new();
		public Mock<ILimb> Limb { get; } = new();
		public Mock<IRoom> Room { get; } = new();
		public Mock<IZone> Zone { get; } = new();
		public Mock<ILegalAuthority> Authority { get; } = new();
		public Mock<IEnforcementAuthority> Role { get; } = new();
		public List<IEffect> HolderEffects { get; } = [];
		public List<IEffect> TargetEffects { get; } = [];

		public Fixture()
		{
			World.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
			World.SetupGet(x => x.AIStorytellers).Returns(new All<IAIStoryteller>());
			World.Setup(x => x.GetStaticInt("MinimumLimbsUnderControlForGrapple")).Returns(1);
			Room.SetupGet(x => x.Id).Returns(1); Room.SetupGet(x => x.Zone).Returns(Zone.Object);
			Room.Setup(x => x.DateTime(null)).Returns(MudDateTime.Never);
			Room.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
			var index = 1L;
			foreach (var actor in new[] { Holder, Target, Receiver })
			{
				actor.SetupGet(x => x.Id).Returns(index++);
				actor.SetupGet(x => x.Gameworld).Returns(World.Object); actor.SetupGet(x => x.Location).Returns(Room.Object);
				actor.SetupGet(x => x.Body).Returns(Mock.Of<IBody>());
				actor.SetupGet(x => x.State).Returns(CharacterState.Awake);
				actor.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
				actor.SetupGet(x => x.CharacteristicDefinitions).Returns([]);
				actor.Setup(x => x.ColocatedWith(It.IsAny<IPerceivable>())).Returns((IPerceivable other) => ReferenceEquals(actor.Object.Location, other.Location));
				actor.Setup(x => x.CanSee(It.IsAny<IPerceivable>(), It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
				actor.Setup(x => x.HowSeen(It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<DescriptionType>(), It.IsAny<bool>(), It.IsAny<PerceiveIgnoreFlags>())).Returns("a traveller");
				actor.Setup(x => x.EffectsOfType<ClinchEffect>(null)).Returns([]);
			}
			Target.SetupGet(x => x.Body).Returns(TargetBody.Object); TargetBody.SetupGet(x => x.Actor).Returns(Target.Object);
			TargetBody.SetupGet(x => x.Limbs).Returns(new[] { Limb.Object });
			TargetBody.Setup(x => x.CanUseLimb(Limb.Object)).Returns(CanUseLimbResult.CanUse);
			TargetBody.Setup(x => x.EffectsOfType<IBeingGrappled>(null)).Returns(() => TargetEffects.OfType<IBeingGrappled>());
			TargetBody.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(TargetEffects.Add);
			TargetBody.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), true)).Callback<IEffect, bool>((effect, _) => { TargetEffects.Remove(effect); effect.RemovalEffect(); });
			Target.Setup(x => x.CombinedEffectsOfType<Dragging.DragTarget>()).Returns([]);
			Holder.Setup(x => x.EffectsOfType<IGrappling>(null)).Returns(() => HolderEffects.OfType<IGrappling>());
			Holder.Setup(x => x.EffectsOfType<Dragging>(null)).Returns([]);
			Holder.Setup(x => x.RemoveAllEffects<IGrappling>(It.IsAny<Predicate<IGrappling>>(), true))
				.Callback<Predicate<IGrappling>, bool>((predicate, _) =>
				{
					foreach (var effect in HolderEffects.OfType<IGrappling>().Where(x => predicate(x)).ToArray())
					{ HolderEffects.Remove(effect); effect.RemovalEffect(); }
				}).Returns(true);
			var legalClass = Mock.Of<ILegalClass>(); Role.SetupGet(x => x.ArrestableClasses).Returns(new[] { legalClass });
			Authority.SetupGet(x => x.Gameworld).Returns(World.Object);
			Authority.SetupGet(x => x.EnforcementZones).Returns(new[] { Zone.Object });
			Authority.SetupGet(x => x.PrisonerBelongingsStorageLocation).Returns(Room.Object);
			Authority.SetupGet(x => x.RoomLocations).Returns(new[] { Room.Object });
			Authority.Setup(x => x.GetEnforcementAuthority(Receiver.Object)).Returns(Role.Object);
			Authority.Setup(x => x.GetLegalClass(Target.Object)).Returns(legalClass);
			var law = Mock.Of<ILaw>(x => x.EnforcementStrategy == EnforcementStrategy.ArrestAndDetain);
			var crime = Mock.Of<ICrime>(x => x.Law == law && x.CriminalIdentityIsKnown == true);
			Authority.Setup(x => x.KnownCrimesForIndividual(Target.Object)).Returns(new[] { crime });
		}

		public Grappling Grapple(bool controlled = true)
		{
			var effect = new Grappling(Holder.Object, Target.Object); HolderEffects.Add(effect);
			if (controlled) ((List<ILimb>)typeof(Grappling).GetField("_limbsUnderControl", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(effect)!).Add(Limb.Object);
			return effect;
		}

		public LegalAuthority NativeAuthority()
		{
			var authority = (LegalAuthority)RuntimeHelpers.GetUninitializedObject(typeof(LegalAuthority));
			typeof(LegalAuthority).GetProperty("Gameworld")!.SetValue(authority, World.Object); authority.Id = 77;
			Set(authority, "_enforcementZones", new List<IZone> { Zone.Object });
			foreach (var field in new[] { "_knownCrimes", "_unknownCrimes", "_resolvedCrimes", "_staleCrimes" })
				Set(authority, field, new HashSet<ICrime>());
			foreach (var field in new[] { "_knownCrimesLookup", "_unknownCrimesLookup", "_resolvedCrimesLookup" })
				Set(authority, field, new CollectionDictionary<long, ICrime>());
			return authority;
		}
	}
}
