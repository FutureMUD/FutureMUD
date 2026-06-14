#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CharacterInstanceIdentityComparerTests
{
	[TestMethod]
	public void IdentityId_SecondaryInstance_ReturnsOwningIdentityId()
	{
		var projection = CreateCharacter(20, 10, 200);

		Assert.AreEqual(10, CharacterInstanceIdentityComparer.IdentityId(projection.Object));
	}

	[TestMethod]
	public void IdentityId_NoIdentity_FallsBackToCharacterId()
	{
		var character = new Mock<ICharacter>();
		ICharacterIdentity identity = null!;
		character.SetupGet(x => x.Id).Returns(42);
		character.SetupGet(x => x.Identity).Returns(identity);

		Assert.AreEqual(42, CharacterInstanceIdentityComparer.IdentityId(character.Object));
	}

	[TestMethod]
	public void SameIdentity_DifferentInstancesSameIdentity_ReturnsTrue()
	{
		var primary = CreateCharacter(10, 10, 100);
		var projection = CreateCharacter(20, 10, 200);

		Assert.IsTrue(CharacterInstanceIdentityComparer.SameIdentity(primary.Object, projection.Object));
	}

	[TestMethod]
	public void SamePhysicalInstance_DifferentInstancesSameIdentity_ReturnsFalse()
	{
		var primary = CreateCharacter(10, 10, 100);
		var projection = CreateCharacter(20, 10, 200);

		Assert.IsFalse(CharacterInstanceIdentityComparer.SamePhysicalInstance(primary.Object, projection.Object));
	}

	[TestMethod]
	public void SameIdentityAndPhysicalInstance_PhaseFourToEightKinds_KeepIdentityAndInstanceDistinct()
	{
		var primary = CreateCharacter(10, 10, 100, CharacterInstanceKind.Primary);
		foreach (var kind in new[]
		         {
			         CharacterInstanceKind.Other,
			         CharacterInstanceKind.AstralProjection,
			         CharacterInstanceKind.MagicalCopy,
			         CharacterInstanceKind.PhysicalClone,
			         CharacterInstanceKind.Puppet,
			         CharacterInstanceKind.RemoteShell
		         })
		{
			var secondary = CreateCharacter(20 + (int)kind, 10, 200 + (int)kind, kind);

			Assert.IsTrue(CharacterInstanceIdentityComparer.SameIdentity(primary.Object, secondary.Object),
				$"Expected {kind} to share identity.");
			Assert.IsFalse(CharacterInstanceIdentityComparer.SamePhysicalInstance(primary.Object, secondary.Object),
				$"Expected {kind} to remain a different physical instance.");
		}
	}

	[TestMethod]
	public void SamePhysicalInstance_SameInstanceId_ReturnsTrue()
	{
		var primary = CreateCharacter(10, 10, 100);
		var sameInstanceFacade = CreateCharacter(20, 10, 100);

		Assert.IsTrue(CharacterInstanceIdentityComparer.SamePhysicalInstance(primary.Object, sameInstanceFacade.Object));
	}

	[TestMethod]
	public void SamePhysicalInstance_EmbodiedBody_ReturnsTrue()
	{
		var primary = CreateCharacter(10, 10, 100);
		var body = new Mock<IBody>();
		body.SetupGet(x => x.Actor).Returns(primary.Object);
		primary.SetupGet(x => x.Body).Returns(body.Object);

		Assert.IsTrue(CharacterInstanceIdentityComparer.SamePhysicalInstance(primary.Object, body.Object));
	}

	[TestMethod]
	public void SamePhysicalInstance_DormantOwnedBody_ReturnsFalse()
	{
		var primary = CreateCharacter(10, 10, 100);
		var activeBody = new Mock<IBody>();
		var dormantBody = new Mock<IBody>();
		activeBody.SetupGet(x => x.Actor).Returns(primary.Object);
		dormantBody.SetupGet(x => x.Actor).Returns(primary.Object);
		primary.SetupGet(x => x.Body).Returns(activeBody.Object);

		Assert.IsFalse(CharacterInstanceIdentityComparer.SamePhysicalInstance(primary.Object, dormantBody.Object));
	}

	private static Mock<ICharacter> CreateCharacter(long characterId, long identityId, long instanceId,
		CharacterInstanceKind instanceKind = CharacterInstanceKind.Other)
	{
		var identity = new Mock<ICharacterIdentity>();
		identity.SetupGet(x => x.Id).Returns(identityId);

		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Id).Returns(characterId);
		character.SetupGet(x => x.Identity).Returns(identity.Object);
		character.SetupGet(x => x.InstanceId).Returns(instanceId);
		character.SetupGet(x => x.InstanceKind).Returns(instanceKind);
		return character;
	}
}
