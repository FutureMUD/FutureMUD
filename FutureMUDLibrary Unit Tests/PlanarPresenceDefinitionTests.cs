#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Framework;
using MudSharp.Planes;
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
}
