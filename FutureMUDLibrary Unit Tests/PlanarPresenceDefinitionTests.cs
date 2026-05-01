#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Framework;
using MudSharp.Planes;
using MudSharp.Construction;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PlanarPresenceDefinitionTests
{
	[TestMethod]
	public void FromXml_NullXml_ResolvesToDefaultMaterialPresence()
	{
		var plane = new Mock<IPlane>();
		plane.Setup(x => x.Id).Returns(42);

		var gameworld = new Mock<IFuturemud>();
		gameworld.Setup(x => x.DefaultPlane).Returns(plane.Object);

		var definition = PlanarPresenceDefinition.FromXml((string)null!, gameworld.Object);

		CollectionAssert.AreEquivalent(new long[] { 42 }, definition.PresencePlaneIds.ToArray());
		CollectionAssert.AreEquivalent(new long[] { 42 }, definition.VisibleToPlaneIds.ToArray());
		CollectionAssert.AreEquivalent(new long[] { 42 }, definition.PerceivesPlaneIds.ToArray());
		Assert.IsFalse(definition.SuspendsPhysicalContact);
		Assert.IsTrue(definition.PropagatesInventory);
		Assert.IsTrue(definition.InteractionPlaneIds(PlanarInteractionKind.Physical).SequenceEqual(new long[] { 42 }));
	}

	[TestMethod]
	public void DefaultMaterial_SaveToXml_RecordsAllInteractionKinds()
	{
		var definition = PlanarPresenceDefinition.DefaultMaterial(7);
		var xml = definition.SaveToXml();

		foreach (PlanarInteractionKind kind in System.Enum.GetValues<PlanarInteractionKind>())
		{
			Assert.IsTrue(
				xml.Descendants("Interaction")
				   .Any(x => x.Attribute("kind")?.Value == kind.ToString() && x.Element("Plane")?.Attribute("id")?.Value == "7"),
				$"Missing interaction kind {kind}.");
		}
	}

	[TestMethod]
	public void FromXml_InvalidBooleanFlags_UsesSafeDefaults()
	{
		var plane = new Mock<IPlane>();
		plane.Setup(x => x.Id).Returns(42);

		var gameworld = new Mock<IFuturemud>();
		gameworld.Setup(x => x.DefaultPlane).Returns(plane.Object);

		var definition = PlanarPresenceDefinition.FromXml(
			"<PlanarData><Presence default=\"true\" /><Flags suspendsPhysicalContact=\"maybe\" propagatesInventory=\"maybe\" /></PlanarData>",
			gameworld.Object);

		Assert.IsFalse(definition.SuspendsPhysicalContact);
		Assert.IsTrue(definition.PropagatesInventory);
	}

	[TestMethod]
	public void NonCorporealVisibleToDefault_CanBeSeenAndSpokenToButNotTouched()
	{
		var defaultPlane = new Mock<IPlane>();
		defaultPlane.Setup(x => x.Id).Returns(1);

		var gameworld = new Mock<IFuturemud>();
		gameworld.Setup(x => x.DefaultPlane).Returns(defaultPlane.Object);

		var etherealPlane = new Mock<IPlane>();
		etherealPlane.Setup(x => x.Id).Returns(2);
		etherealPlane.Setup(x => x.Gameworld).Returns(gameworld.Object);

		var material = new PlanarPresence(PlanarPresenceDefinition.DefaultMaterial(1));
		var spirit = new PlanarPresence(PlanarPresenceDefinition.NonCorporeal(etherealPlane.Object, true));

		Assert.IsTrue(material.CanPerceive(spirit));
		Assert.IsTrue(material.CanInteract(spirit, PlanarInteractionKind.Speak));
		Assert.IsFalse(material.CanInteract(spirit, PlanarInteractionKind.Physical));
		Assert.IsFalse(material.CanInteract(spirit, PlanarInteractionKind.Combat));
		Assert.IsFalse(material.CanInteract(spirit, PlanarInteractionKind.Medical));
	}

	[TestMethod]
	public void ManifestedPresence_AllowsPhysicalInteractionsOnSamePlane()
	{
		var plane = new Mock<IPlane>();
		plane.Setup(x => x.Id).Returns(7);

		var material = new PlanarPresence(PlanarPresenceDefinition.DefaultMaterial(7));
		var manifested = new PlanarPresence(PlanarPresenceDefinition.Manifested(plane.Object));

		Assert.IsTrue(material.CanPerceive(manifested));
		Assert.IsTrue(material.CanInteract(manifested, PlanarInteractionKind.Physical));
		Assert.IsTrue(material.CanInteract(manifested, PlanarInteractionKind.Inventory));
		Assert.IsTrue(material.CanInteract(manifested, PlanarInteractionKind.Combat));
	}

	[TestMethod]
	public void DefaultMaterial_MultiplePlanes_AllowsFullPresenceOnAllPlanes()
	{
		var definition = PlanarPresenceDefinition.DefaultMaterial(new long[] { 1, 2 });

		CollectionAssert.AreEquivalent(new long[] { 1, 2 }, definition.PresencePlaneIds.ToArray());
		CollectionAssert.AreEquivalent(new long[] { 1, 2 }, definition.VisibleToPlaneIds.ToArray());
		CollectionAssert.AreEquivalent(new long[] { 1, 2 }, definition.PerceivesPlaneIds.ToArray());
		CollectionAssert.AreEquivalent(new long[] { 1, 2 }, definition.InteractionPlaneIds(PlanarInteractionKind.Physical).ToArray());
	}

	[TestMethod]
	public void AdditivePerceivesPlanes_MergedWithBase_CanSeeTargetsOnRemotePlane()
	{
		var astral = new Mock<IPlane>();
		astral.Setup(x => x.Id).Returns(2);
		var materialWithAstralSight = PlanarPresenceDefinition.DefaultMaterial(1)
		                                                      .Merge(PlanarPresenceDefinition.PerceivesPlanes(new[] { astral.Object }));
		var astralTarget = PlanarPresenceDefinition.DefaultMaterial(2);

		Assert.IsTrue(new PlanarPresence(materialWithAstralSight).CanPerceive(new PlanarPresence(astralTarget)));
	}

	[TestMethod]
	public void AdditiveVisibleToPlanes_MergedWithRemoteTarget_CanBeSeenFromAddedPlane()
	{
		var prime = new Mock<IPlane>();
		prime.Setup(x => x.Id).Returns(1);
		var primeViewer = new PlanarPresence(PlanarPresenceDefinition.DefaultMaterial(1));
		var astralVisibleToPrime = PlanarPresenceDefinition.DefaultMaterial(2)
		                                                  .Merge(PlanarPresenceDefinition.VisibleToPlanes(new[] { prime.Object }));

		Assert.IsTrue(primeViewer.CanPerceive(new PlanarPresence(astralVisibleToPrime)));
	}

	[TestMethod]
	public void CanPerceivePlanar_LocationTarget_IgnoresPlanarState()
	{
		var voyeur = new Mock<IPerceiver>();
		var location = new Mock<ILocation>();

		Assert.IsTrue(voyeur.Object.CanPerceivePlanar(location.Object));
	}

	[TestMethod]
	public void RemoteObservationTagFor_RemotePlaneSight_ReturnsConfiguredPlaneTag()
	{
		var (gameworld, prime, astral) = BuildPlanarGameworld();
		var voyeur = BuildPerceiver(gameworld.Object, PlanarPresenceDefinition.DefaultMaterial(1)
		                                                                      .Merge(PlanarPresenceDefinition.PerceivesPlanes(new[] { astral.Object })));
		var target = BuildPerceivable(gameworld.Object, PlanarPresenceDefinition.DefaultMaterial(2));
		voyeur.Setup(x => x.IsSelf(target.Object)).Returns(false);

		Assert.AreEqual("(Astral Plane)", voyeur.Object.RemoteObservationTagFor(target.Object, false));
	}

	[TestMethod]
	public void RemoteObservationTagFor_TargetVisibleToCurrentPlane_DoesNotShowRemoteTag()
	{
		var (gameworld, prime, astral) = BuildPlanarGameworld();
		var voyeur = BuildPerceiver(gameworld.Object, PlanarPresenceDefinition.DefaultMaterial(1)
		                                                                      .Merge(PlanarPresenceDefinition.PerceivesPlanes(new[] { astral.Object })));
		var targetDefinition = PlanarPresenceDefinition.DefaultMaterial(2)
		                                               .Merge(PlanarPresenceDefinition.VisibleToPlanes(new[] { prime.Object }));
		var target = BuildPerceivable(gameworld.Object, targetDefinition);
		voyeur.Setup(x => x.IsSelf(target.Object)).Returns(false);

		Assert.AreEqual(string.Empty, voyeur.Object.RemoteObservationTagFor(target.Object, false));
	}

	private static (Mock<IFuturemud> Gameworld, Mock<IPlane> Prime, Mock<IPlane> Astral) BuildPlanarGameworld()
	{
		var gameworld = new Mock<IFuturemud>();
		var prime = BuildPlane(gameworld.Object, 1, "Prime Material", 0, string.Empty);
		var astral = BuildPlane(gameworld.Object, 2, "Astral Plane", 10, "({0})");
		var planes = new Mock<IUneditableAll<IPlane>>();
		var planeList = new List<IPlane> { prime.Object, astral.Object };
		planes.Setup(x => x.GetEnumerator()).Returns(() => planeList.GetEnumerator());
		planes.As<IEnumerable>().Setup(x => x.GetEnumerator()).Returns(() => planeList.GetEnumerator());
		planes.Setup(x => x.Get(1)).Returns(prime.Object);
		planes.Setup(x => x.Get(2)).Returns(astral.Object);
		gameworld.Setup(x => x.DefaultPlane).Returns(prime.Object);
		gameworld.Setup(x => x.Planes).Returns(planes.Object);
		return (gameworld, prime, astral);
	}

	private static Mock<IPlane> BuildPlane(IFuturemud gameworld, long id, string name, int order, string tag)
	{
		var plane = new Mock<IPlane>();
		plane.Setup(x => x.Id).Returns(id);
		plane.Setup(x => x.Name).Returns(name);
		plane.Setup(x => x.DisplayOrder).Returns(order);
		plane.Setup(x => x.RemoteObservationTag).Returns(tag);
		plane.Setup(x => x.Gameworld).Returns(gameworld);
		return plane;
	}

	private static Mock<IPerceiver> BuildPerceiver(IFuturemud gameworld, PlanarPresenceDefinition definition)
	{
		var perceiver = new Mock<IPerceiver>();
		perceiver.Setup(x => x.Gameworld).Returns(gameworld);
		perceiver.Setup(x => x.EffectsOfType<IPlanarOverlayEffect>(It.IsAny<System.Predicate<IPlanarOverlayEffect>>()))
		         .Returns(Enumerable.Empty<IPlanarOverlayEffect>());
		perceiver.As<IHavePlanarPresence>()
		         .Setup(x => x.BasePlanarPresence)
		         .Returns(definition);
		return perceiver;
	}

	private static Mock<IPerceivable> BuildPerceivable(IFuturemud gameworld, PlanarPresenceDefinition definition)
	{
		var perceivable = new Mock<IPerceivable>();
		perceivable.Setup(x => x.Gameworld).Returns(gameworld);
		perceivable.Setup(x => x.EffectsOfType<IPlanarOverlayEffect>(It.IsAny<System.Predicate<IPlanarOverlayEffect>>()))
		           .Returns(Enumerable.Empty<IPlanarOverlayEffect>());
		perceivable.As<IHavePlanarPresence>()
		           .Setup(x => x.BasePlanarPresence)
		           .Returns(definition);
		return perceivable;
	}
}
