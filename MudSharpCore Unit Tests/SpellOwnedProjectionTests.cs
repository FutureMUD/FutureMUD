#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.Magic.Lifecycle;
using MudSharp.Magic.SpellEffects;
using MudSharp.Planes;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SpellOwnedProjectionTests
{
	[TestMethod]
	public void Retirement_RefusesARiderOrControllerStillBoundToTheSecondary()
	{
		var (actor, _, _) = World();
		actor.SetupGet(x => x.Riders).Returns([Mock.Of<ICharacter>()]);
		Assert.IsNotNull(SpellOwnedProjectionService.RuntimeDependencyError(actor.Object));
		actor.SetupGet(x => x.Riders).Returns([]);
		actor.SetupGet(x => x.CharacterController).Returns(Mock.Of<ICharacterController>());
		Assert.IsNotNull(SpellOwnedProjectionService.RuntimeDependencyError(actor.Object));
	}

	[TestMethod]
	public void Trance_AppliesOnlyWhileTheMatchingShadowIsFocused()
	{
		var owner = new Mock<ICharacter>(); var identity = new Mock<ICharacterIdentity>();
		owner.SetupGet(x => x.Identity).Returns(identity.Object);
		identity.SetupGet(x => x.FocusedInstance).Returns(Mock.Of<ICharacterInstance>(x => x.InstanceId == 8));
		var effect = new SpellProjectionTrance(owner.Object, 8);
		Assert.IsTrue(effect.Applies());
		identity.SetupGet(x => x.FocusedInstance).Returns(Mock.Of<ICharacterInstance>(x => x.InstanceId == 9));
		Assert.IsFalse(effect.Applies());
		Assert.IsFalse(effect.Applies(owner.Object));
		Assert.IsFalse(effect.Applies(owner.Object, identity.Object));
		Assert.IsFalse(effect.Applies(owner.Object, PerceiveIgnoreFlags.None));
	}

	[TestMethod]
	public void OwnershipMetadata_RecognisesOnlyDeclaredAnchorFields_NotMatchingNumbersInText()
	{
		var references = PhysicalReferenceCodecs.Effects("<Effects><OwnedProjection Lifecycle='00000000-0000-0000-0000-000000000001' AnchorCharacterId='123' AnchorInstanceId='456'/><Narrative>789 is a body number, not a reference</Narrative><Unknown BodyId='789'/></Effects>", true).ToArray();
		Assert.AreEqual(2, references.Length);
		Assert.IsTrue(references.Any(x => x.Kind == PhysicalEntityKind.Character && x.Id == 123));
		Assert.IsTrue(references.Any(x => x.Kind == PhysicalEntityKind.CharacterInstance && x.Id == 456));
		Assert.IsFalse(references.Any(x => x.Id == 789));
	}

	[TestMethod]
	public void OwnershipClaims_RejectBorrowedOrAdditionalRows()
	{
		var now = DateTime.UtcNow;
		var anchor = new SpellProjectionAnchor(1, 2, 3, 0, null, new(SpellProjectionKind.WalkingShadow, 4, 0, 60, 2, 0));
		var origin = new SpellLifecycleOrigin(Guid.NewGuid(), 1, 1, 2, SpellProjectionAnchor.Family, SpellLifecycleMode.TemporaryCleanup, now, now.AddMinutes(1), anchor.Save().ToString());
		var life = new SpellOwnedLifecycle(origin, [new(SpellOwnedEntityKind.Body, 5), new(SpellOwnedEntityKind.CharacterInstance, 6)], SpellLifecycleState.Retiring,
			SpellRetirementReason.Reboot, null, null, now, 1, "");
		Assert.IsTrue(SpellOwnedProjectionService.HasClaims(life));
		Assert.IsFalse(SpellOwnedProjectionService.HasClaims(life with { Entities = [.. life.Entities, new(SpellOwnedEntityKind.AutonomousCharacter, 2)] }));
		Assert.IsFalse(SpellOwnedProjectionService.HasClaims(life with { Entities = [new(SpellOwnedEntityKind.Body, 5, SpellOwnedEntityRole.GeneratedPossession), life.Entities[1]] }));
	}

	[DataTestMethod]
	[DataRow(SpellProjectionKind.SandEffigy, "arm.spell.sand_effigy")]
	[DataRow(SpellProjectionKind.WalkingShadow, "arm.spell.walking_shadow")]
	public void Stocks_RetainExplicitPolicyAcrossSaveAndClone(SpellProjectionKind kind, string key)
	{
		var policy = new SpellProjectionConfiguration(kind, 21, kind == SpellProjectionKind.SandEffigy ? 22 : 0, 120, kind == SpellProjectionKind.SandEffigy ? 0 : 2, 3);
		Assert.AreEqual(key, ArmageddonProjectionStock.Key(kind));
		var root = ArmageddonProjectionStock.Definition(policy, 5, 6, 10);
		var effect = root.Descendants("Effect").Single(x => (string?)x.Attribute("type") == "createprojection");
		var native = new CreateProjectionEffect(effect, Mock.Of<IMagicSpell>());
		Assert.IsNull(native.DefinitionError);
		Assert.IsTrue(XNode.DeepEquals(effect, native.Clone().SaveToXml()));
		Assert.AreEqual(policy, SpellProjectionAnchor.ReadPolicy(effect.Element("Policy")!));
	}

	[TestMethod]
	public void InvalidPolicy_IsRejectedBeforePersistenceOrPayment()
	{
		var service = new SpellOwnedProjectionService(Mock.Of<IFuturemud>());
		Assert.IsNotNull(service.AdmissionError(Mock.Of<ICharacter>(), new(SpellProjectionKind.WalkingShadow, 0, 0, 60, 2, 0), 1));
		var xml = CreateProjectionEffect.Definition(new(SpellProjectionKind.WalkingShadow, 1, 0, 60, 2, 0));
		xml.Element("Policy")!.Add(new XElement("GuessedActorId", 999));
		Assert.IsNotNull(new CreateProjectionEffect(xml, Mock.Of<IMagicSpell>()).DefinitionError);
	}

	[TestMethod]
	public void SandEffigy_RefusesRoomLayerAndRouteMovementButAllowsItsInitialEntry()
	{
		var (actor, world, rooms) = World();
		var boundary = new SpellProjectionBoundary(actor.Object, new(11, 12, 1, 0, null, new(SpellProjectionKind.SandEffigy, 1, 20, 60, 0, 0)));
		Assert.IsNull(boundary.TravelError(new(rooms[0].Object, RoomLayer.GroundLevel, null), false));
		Assert.IsNotNull(boundary.TravelError(new(rooms[1].Object, RoomLayer.GroundLevel, null), true));
		Assert.IsNotNull(boundary.TravelError(new(rooms[0].Object, RoomLayer.InAir, null), false));
		Assert.IsNotNull(boundary.TravelError(new(rooms[0].Object, RoomLayer.GroundLevel, 1), false));
		actor.SetupGet(x => x.IsControllable).Returns(false);
		Assert.IsNotNull(boundary.TravelError(new(rooms[0].Object, RoomLayer.GroundLevel, null), false));
	}

	[TestMethod]
	public void WalkingShadow_FollowsBoundedNativeEdgesAndRejectsTeleportLayerAndRouteTravel()
	{
		var (actor, world, rooms) = World();
		var boundary = new SpellProjectionBoundary(actor.Object, new(11, 12, 1, 0, null, new(SpellProjectionKind.WalkingShadow, 2, 0, 60, 1, 0)));
		Assert.IsNull(boundary.TravelError(new(rooms[0].Object, RoomLayer.GroundLevel, null), false));
		Assert.IsNull(boundary.TravelError(new(rooms[1].Object, RoomLayer.GroundLevel, null), true));
		Assert.IsNotNull(boundary.TravelError(new(rooms[2].Object, RoomLayer.GroundLevel, null), true));
		Assert.IsNotNull(boundary.TravelError(new(rooms[1].Object, RoomLayer.GroundLevel, null), false));
		Assert.IsNotNull(boundary.TravelError(new(rooms[1].Object, RoomLayer.InAir, null), true));
		Assert.IsNotNull(boundary.TravelError(new(rooms[1].Object, RoomLayer.GroundLevel, 1), true));
	}

	[DataTestMethod]
	[DataRow(SpellProjectionKind.SandEffigy)]
	[DataRow(SpellProjectionKind.WalkingShadow)]
	public void Projection_CanObserveButCannotManipulateEvenWithNativeHands(SpellProjectionKind kind)
	{
		var (actor, world, rooms) = World();
		var boundary = new SpellProjectionBoundary(actor.Object, new(11, 12, 1, 0, null, new(kind, kind == SpellProjectionKind.SandEffigy ? 1 : 2, kind == SpellProjectionKind.SandEffigy ? 20 : 0, 60, 0, 0)));
		actor.Setup(x => x.EffectsOfType<ISpellProjectionBoundary>()).Returns([boundary]);
		var body = new Mock<IBody>(); body.SetupGet(x => x.Actor).Returns(actor.Object);
		Assert.IsFalse(body.Object.CanPerformManualAction(out var reason));
		Assert.IsTrue(reason.Contains("projected"));
		var presence = new PlanarPresence(boundary.PlanarPresenceDefinition);
		var material = new PlanarPresence(PlanarPresenceDefinition.DefaultMaterial(1));
		Assert.IsTrue(presence.CanInteract(material, PlanarInteractionKind.Observe));
		Assert.IsFalse(presence.CanInteract(material, PlanarInteractionKind.Inventory));
		Assert.IsFalse(presence.CanInteract(material, PlanarInteractionKind.Combat));
	}

	private static (Mock<ICharacter>, Mock<IFuturemud>, Mock<IRoom>[]) World()
	{
		var world = new Mock<IFuturemud>(); var actor = new Mock<ICharacter>();
		var rooms = Enumerable.Range(1, 3).Select(i => { var r = new Mock<IRoom>(); r.SetupGet(x => x.Id).Returns(i); return r; }).ToArray();
		for (var i = 0; i < 2; i++)
		{
			var exit = new Mock<IRoomExit>(); exit.SetupGet(x => x.Destination).Returns(rooms[i + 1].Object);
			rooms[i].Setup(x => x.ExitsFor(actor.Object, true)).Returns([exit.Object]);
		}
		rooms[2].Setup(x => x.ExitsFor(actor.Object, true)).Returns([]);
		var all = new All<IRoom>(); foreach (var room in rooms) all.Add(room.Object);
		world.SetupGet(x => x.Rooms).Returns(all);
		world.SetupGet(x => x.DefaultPlane).Returns(Mock.Of<IPlane>(x => x.Id == 1));
		actor.SetupGet(x => x.Gameworld).Returns(world.Object); actor.SetupGet(x => x.Location).Returns(rooms[0].Object);
		actor.SetupGet(x => x.IsEmbodied).Returns(true); actor.SetupGet(x => x.IsControllable).Returns(true);
		return (actor, world, rooms);
	}
}
