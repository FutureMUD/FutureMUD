#nullable enable

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Effects;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory.Size;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests;

[TestClass]
public class TailorCommandTests
{
	[TestMethod]
	public void Tailor_WithoutConfiguredTailoringTrait_DoesNotStartAction()
	{
		var actor = new Mock<ICharacter>();
		var body = PhysicalManipulationTestHelper.SetUpUsableHands(actor);
		var world = new Mock<IFuturemud>();
		var output = new Mock<IOutputHandler>();
		var garment = new Mock<IGameItem>();
		var wearable = new Mock<IWearable>();
		var trait = new Mock<ITraitDefinition>();
		var traits = new Mock<IUneditableAll<ITraitDefinition>>();
		var held = new List<IGameItem> { garment.Object };

		actor.SetupGet(x => x.Gameworld).Returns(world.Object);
		actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
		actor.Setup(x => x.TargetHeldItem("garment")).Returns(garment.Object);
		actor.Setup(x => x.HasTrait(trait.Object)).Returns(false);
		actor.Setup(x => x.TraitRawValue(trait.Object)).Returns(0.0);
		body.SetupGet(x => x.HeldItems).Returns(held);
		world.SetupGet(x => x.Traits).Returns(traits.Object);
		world.Setup(x => x.GetStaticLong("TailoringTraitId")).Returns(101L);
		traits.Setup(x => x.Get(101L)).Returns(trait.Object);
		trait.SetupGet(x => x.Name).Returns("Tailoring");
		garment.Setup(x => x.GetItemType<IWearable>()).Returns(wearable.Object);
		wearable.SetupGet(x => x.UsesSizing).Returns(true);
		wearable.SetupGet(x => x.WornBy).Returns((IBody)null!);

		typeof(ManipulationModule).GetMethod("Tailor", BindingFlags.Static | BindingFlags.NonPublic)!
			.Invoke(null, [actor.Object, "tailor garment"]);

		actor.Verify(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()), Times.Never);
		world.Verify(x => x.GetStaticLong("TailoringToolTagId"), Times.Never);
		output.Verify(x => x.Send(It.Is<string>(message => message.StripANSIColour().Contains("Tailoring skill")), true, false), Times.Once);
	}
}
