#nullable enable

using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Magic;
using MudSharp.Magic.Emotions;
using MudSharp.Magic.Lifecycle;
using MudSharp.Magic.SpellEffects;

namespace MudSharp_Unit_Tests;

[TestClass]
public class S4ExistingFactoryGradeBoundaryTests
{
	private sealed class ItemEffect(XElement root, IMagicSpell spell) : CreateItemEffect(root, spell);
	[DataTestMethod]
	[DataRow(1)] [DataRow(2)] [DataRow(3)] [DataRow(4)] [DataRow(5)] [DataRow(6)] [DataRow(7)]
	public void CanonicalEmotions_ExternalGrade_UsesExactBonusAndSixtySecondsPerGrade(int grade)
	{
		foreach (var content in S4ExistingFactoryContentTests.Contents().Where(x => x.Key is "arm.spell.roused_fury" or "arm.spell.still_anger"))
		{
			var profile = EmotionalStockProfile.Read(content.BuildDefinition(1, 2, 0).Descendants("SourceProfile").Single());
			Assert.IsTrue(profile.CanonicalGradePolicy);
			var result = profile.ResolveLifetime(grade, 123, SpellPower.ExtremelyWeak, (_, _) => throw new AssertFailedException("Canonical grade must not draw a source endurance random roll."));
			Assert.AreEqual(TimeSpan.FromSeconds(60 * grade), result.Duration);
			Assert.AreEqual(grade, result.State.SourceGrade); Assert.AreEqual(SpellPower.ExtremelyWeak, result.State.Power);
			Assert.AreEqual(profile.Kind == EmotionalSpellKind.Fury ? grade : 0, result.State.EndurancePoints);
			Assert.IsTrue(XNode.DeepEquals(profile.SaveToXml(), EmotionalStockProfile.Read(profile.SaveToXml()).SaveToXml()));
		}
	}
	[DataTestMethod]
	[DataRow("unknown policy")] [DataRow("wrong seconds")] [DataRow("unknown namespace")] [DataRow("wrong units")]
	public void CanonicalEmotions_MalformedPolicy_Refuses(string mutation)
	{
		var xml = ArmageddonRousedFuryStock.Content(new(16, 17)).BuildDefinition(1, 2, 0).Descendants("SourceProfile").Single();
		switch (mutation)
		{
			case "unknown policy": xml.SetAttributeValue("gradepolicy", "Guessed"); break;
			case "wrong seconds": xml.Element("Lifetime")!.SetAttributeValue("seconds", 600); break;
			case "unknown namespace": xml.SetAttributeValue(XName.Get("gradepolicy", "urn:foreign"), "Canonical"); break;
			case "wrong units": xml.SetAttributeValue("units", 2); break;
		}
		Assert.ThrowsException<ArgumentException>(() => EmotionalStockProfile.Read(xml));
	}
	[DataTestMethod]
	[DataRow(1)] [DataRow(2)] [DataRow(3)] [DataRow(4)] [DataRow(5)] [DataRow(6)] [DataRow(7)]
	public void CanonicalWeaponOutput_RequestedGrade_SelectsExactNativePrototype(int grade)
	{
		var world = new Mock<IFuturemud>();
		var prototypes = S4ExistingFactoryContentTests.Items().Values.Select(id => Mock.Of<IGameItemProto>(x => x.Id == id)).ToArray();
		var catalogue = new Mock<IUneditableRevisableAll<IGameItemProto>>();
		catalogue.Setup(x => x.Get(It.IsAny<long>())).Returns((long id) => prototypes.SingleOrDefault(x => x.Id == id)!);
		world.SetupGet(x => x.ItemProtos).Returns(catalogue.Object);
		var spell = Mock.Of<IMagicSpell>(x => x.Gameworld == world.Object);
		foreach (var content in S4ExistingFactoryContentTests.Contents().Where(x => x.Key is "arm.spell.sand_knife" or "arm.spell.storm_spear"))
		{
			var effect = new ItemEffect(content.BuildDefinition(1, 2, 0).Element("Effects")!.Element("Effect")!, spell);
			var selected = (IGameItemProto)typeof(CreateItemEffect).GetMethod("PreparePrototype", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(effect, [grade])!;
			Assert.AreEqual(100L + grade, selected.Id); Assert.AreEqual(1, effect.GradeOutputs[grade].Length);
		}
	}
	[DataTestMethod]
	[DataRow(1)] [DataRow(7)]
	public void WalkingShadow_ResolvedGradeRangeAndColdXml_KeepOriginalBoundaries(int grade)
	{
		var policy = SpellProjectionAnchor.ReadPolicy(ArmageddonProjectionStock.WalkingShadowContent(4).BuildDefinition(1, 2, 0).Descendants("Policy").Single());
		var selected = policy.ResolveForGrade(grade); Assert.AreEqual(10 * grade, selected.MaximumRoomDistance);
		Assert.AreEqual(grade, selected.RangeGrade); Assert.IsNull(selected.Error(grade));
		var anchor = new SpellProjectionAnchor(1, 2, 3, 0, null, selected);
		Assert.AreEqual(anchor, SpellProjectionAnchor.Load(anchor.Save().ToString()));
		var world = new Mock<IFuturemud>(); var actor = new Mock<ICharacter>();
		var rooms = Enumerable.Range(1, 73).Select(i => { var room = new Mock<IRoom>(); room.SetupGet(x => x.Id).Returns(i); return room; }).ToArray();
		for (var index = 0; index < rooms.Length - 1; index++)
		{
			var exit = new Mock<IRoomExit>(); exit.SetupGet(x => x.Destination).Returns(rooms[index + 1].Object);
			rooms[index].Setup(x => x.ExitsFor(actor.Object, true)).Returns([exit.Object]);
		}
		rooms[^1].Setup(x => x.ExitsFor(actor.Object, true)).Returns([]);
		var all = new All<IRoom>(); foreach (var room in rooms) all.Add(room.Object);
		world.SetupGet(x => x.Rooms).Returns(all); actor.SetupGet(x => x.Gameworld).Returns(world.Object);
		actor.SetupGet(x => x.IsEmbodied).Returns(true); actor.SetupGet(x => x.IsControllable).Returns(true);
		actor.SetupGet(x => x.Location).Returns(rooms[0].Object);
		var boundary = new SpellProjectionBoundary(actor.Object, anchor with { RoomId = 1 });
		Assert.IsNull(boundary.TravelError(new(rooms[10 * grade].Object, RoomLayer.GroundLevel, null), true));
		Assert.IsNotNull(boundary.TravelError(new(rooms[10 * grade + 1].Object, RoomLayer.GroundLevel, null), true));
		Assert.IsNotNull(boundary.TravelError(new(rooms[1].Object, RoomLayer.GroundLevel, null), false));
		Assert.IsNotNull((selected with { RoomDistancePerGrade = 0, RangeGrade = 0, MaximumRoomDistance = 70 }).Error(grade));
	}
	[TestMethod]
	public void ProjectionClaims_ChangedSavedGradeOrOriginalDeadline_RefuseColdOwnership()
	{
		var now = new DateTime(2026, 10, 12, 0, 0, 0, DateTimeKind.Utc);
		var config = new SpellProjectionConfiguration(SpellProjectionKind.WalkingShadow, 4, 0, 60, 10, 0, RoomDistancePerGrade: 10).ResolveForGrade(7);
		var anchor = new SpellProjectionAnchor(1, 2, 3, 0, null, config);
		var origin = new SpellLifecycleOrigin(Guid.NewGuid(), 1, 7, 2, SpellProjectionAnchor.Family, SpellLifecycleMode.TemporaryCleanup, now, now.AddSeconds(420), anchor.Save().ToString());
		var life = new SpellOwnedLifecycle(origin, [new(SpellOwnedEntityKind.Body, 5), new(SpellOwnedEntityKind.CharacterInstance, 6)], SpellLifecycleState.Retiring,
			SpellRetirementReason.Reboot, null, null, now, 1, "");
		Assert.IsTrue(SpellOwnedProjectionService.HasClaims(life));
		Assert.IsFalse(SpellOwnedProjectionService.HasClaims(life with { Origin = origin with { Grade = 6 } }));
		Assert.IsFalse(SpellOwnedProjectionService.HasClaims(life with { Origin = origin with { DeadlineUtc = now.AddSeconds(421) } }));
		var changed = anchor.Save(); changed.Element("Policy")!.Element("RequestedGradeRange")!.SetAttributeValue("grade", 6);
		Assert.ThrowsException<FormatException>(() => SpellProjectionAnchor.Load(changed.ToString()));
		changed = anchor.Save(); changed.Element("Policy")!.Element("RequestedGradeRange")!.SetAttributeValue("perGrade", 11);
		Assert.ThrowsException<FormatException>(() => SpellProjectionAnchor.Load(changed.ToString()));
		changed = anchor.Save(); changed.Element("Policy")!.Add(new XElement(XName.Get("RequestedGradeRange", "urn:foreign")));
		Assert.ThrowsException<FormatException>(() => SpellProjectionAnchor.Load(changed.ToString()));
	}
}
