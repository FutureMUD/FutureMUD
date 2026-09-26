using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.GameItems;
using MudSharp.Planes;

#nullable enable

namespace MudSharp_Unit_Tests;

internal static class PhysicalManipulationTestHelper
{
	// Existing component tests isolate their own capability from scene access.
	// PhysicalManipulationTests exercises the real access and anatomy checks separately.
	public static Mock<IBody> SetUpUsableHands(Mock<ICharacter> actor, Mock<IBody>? existingBody = null)
	{
		var body = existingBody ?? new Mock<IBody>();
		body.SetupGet(x => x.Actor).Returns(actor.Object);
		body.SetupGet(x => x.HoldLocs).Returns([Mock.Of<IGrab>()]);
		body.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
		body.Setup(x => x.CanUseBodypart(It.IsAny<IBodypart>())).Returns(CanUseBodypartResult.CanUse);
		actor.SetupGet(x => x.Body).Returns(body.Object);
		actor.Setup(x => x.CanManipulateItem(It.IsAny<IGameItem>())).Returns((true, string.Empty));
		return body;
	}
}
