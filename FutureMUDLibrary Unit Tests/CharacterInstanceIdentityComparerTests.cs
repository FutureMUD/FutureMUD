#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using System.Collections.Generic;
using System.Linq;

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
	public void FrameworkItemId_SecondaryCharacter_ReturnsOwningIdentityId()
	{
		var projection = CreateCharacter(20, 10, 200);

		Assert.AreEqual(10, CharacterInstanceIdentityComparer.FrameworkItemId(projection.Object));
	}

	[TestMethod]
	public void FrameworkItemId_NonCharacter_ReturnsFrameworkItemId()
	{
		var item = new Mock<IFrameworkItem>();
		item.SetupGet(x => x.Id).Returns(99);

		Assert.AreEqual(99, CharacterInstanceIdentityComparer.FrameworkItemId(item.Object));
	}

	[TestMethod]
	public void RecognitionKeyFor_CharacterIdentity_UsesDurableIdentity()
	{
		var character = CreateCharacter(20, 10, 200, bodyId: 30);

		var key = CharacterInstanceIdentityComparer.RecognitionKeyFor(character.Object);

		Assert.AreEqual(CharacterInstanceIdentityComparer.CharacterRecognitionTargetType, key.TargetType);
		Assert.AreEqual(10, key.TargetId);
	}

	[TestMethod]
	public void RecognitionKeyFor_CharacterPhysicalBody_UsesBodyId()
	{
		var character = CreateCharacter(20, 10, 200, bodyId: 30);

		var key = CharacterInstanceIdentityComparer.RecognitionKeyFor(character.Object,
			CharacterRecognitionScope.PhysicalBody);

		Assert.AreEqual(CharacterInstanceIdentityComparer.BodyRecognitionTargetType, key.TargetType);
		Assert.AreEqual(30, key.TargetId);
	}

	[TestMethod]
	public void RecognitionLookupKeys_Character_IncludesIdentityAndBody()
	{
		var character = CreateCharacter(20, 10, 200, bodyId: 30);

		var keys = CharacterInstanceIdentityComparer.RecognitionLookupKeys(character.Object).ToList();

		Assert.IsTrue(keys.Contains(new CharacterRecognitionKey(
			CharacterInstanceIdentityComparer.CharacterRecognitionTargetType, 10)));
		Assert.IsTrue(keys.Contains(new CharacterRecognitionKey(
			CharacterInstanceIdentityComparer.BodyRecognitionTargetType, 30)));
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
	public void SameIdentityAndPhysicalInstance_PhaseFourToNineKinds_KeepIdentityAndInstanceDistinct()
	{
		var primary = CreateCharacter(10, 10, 100, CharacterInstanceKind.Primary);
		foreach (var kind in new[]
		         {
			         CharacterInstanceKind.Other,
			         CharacterInstanceKind.AstralProjection,
			         CharacterInstanceKind.MagicalCopy,
			         CharacterInstanceKind.PhysicalClone,
			         CharacterInstanceKind.Puppet,
			         CharacterInstanceKind.PossessedBody,
			         CharacterInstanceKind.RemoteShell,
			         CharacterInstanceKind.ScriptedAi,
			         CharacterInstanceKind.PossessedCorpse,
			         CharacterInstanceKind.AnimatedCorpse
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

	[TestMethod]
	public void InstanceId_ZeroInstance_ReturnsNull()
	{
		var character = CreateCharacter(10, 10, 0);

		Assert.IsNull(CharacterInstanceIdentityComparer.InstanceId(character.Object));
	}

	[TestMethod]
	public void PhysicalInstanceKey_UsesInstanceIdWhenAvailable()
	{
		var character = CreateCharacter(20, 10, 200);

		Assert.AreEqual(200, CharacterInstanceIdentityComparer.PhysicalInstanceKey(character.Object));
	}

	[TestMethod]
	public void PhysicalInstanceKey_FallsBackToIdentityIdForLegacyActors()
	{
		var character = CreateCharacter(20, 10, 0);

		Assert.AreEqual(10, CharacterInstanceIdentityComparer.PhysicalInstanceKey(character.Object));
	}

	[TestMethod]
	public void SamePhysicalInstanceOrBody_MatchesOnlyActiveBody()
	{
		var character = CreateCharacter(20, 10, 200);
		var activeBody = new Mock<IBody>();
		var dormantBody = new Mock<IBody>();
		activeBody.SetupGet(x => x.Actor).Returns(character.Object);
		dormantBody.SetupGet(x => x.Actor).Returns(character.Object);
		character.SetupGet(x => x.Body).Returns(activeBody.Object);

		Assert.IsTrue(CharacterInstanceIdentityComparer.SamePhysicalInstanceOrBody(character.Object, activeBody.Object));
		Assert.IsFalse(CharacterInstanceIdentityComparer.SamePhysicalInstanceOrBody(character.Object, dormantBody.Object));
	}

	[TestMethod]
	public void SameIdentityOrPrimaryOwner_MatchesSameIdentityBodies()
	{
		var primary = CreateCharacter(10, 10, 100);
		var secondary = CreateCharacter(20, 10, 200);
		var secondaryBody = new Mock<IBody>();
		secondaryBody.SetupGet(x => x.Actor).Returns(secondary.Object);

		Assert.IsTrue(CharacterInstanceIdentityComparer.SameIdentityOrPrimaryOwner(primary.Object, secondary.Object));
		Assert.IsTrue(CharacterInstanceIdentityComparer.SameIdentityOrPrimaryOwner(primary.Object, secondaryBody.Object));
	}

	[TestMethod]
	public void DistinctPhysicalInstances_KeepsDifferentBodiesForSameIdentity()
	{
		var primary = CreateCharacter(10, 10, 100);
		var samePrimaryFacade = CreateCharacter(99, 10, 100);
		var secondary = CreateCharacter(20, 10, 200);

		var result = new[] { primary.Object, samePrimaryFacade.Object, secondary.Object }
		             .DistinctPhysicalInstances()
		             .ToList();

		Assert.AreEqual(2, result.Count);
		Assert.IsTrue(result.Any(x => ReferenceEquals(x, primary.Object)));
		Assert.IsTrue(result.Any(x => ReferenceEquals(x, secondary.Object)));
	}

	[TestMethod]
	public void RemovePhysicalInstance_RemovesOnlyMatchingBody()
	{
		var primary = CreateCharacter(10, 10, 100);
		var secondary = CreateCharacter(20, 10, 200);
		var actors = new List<ICharacter> { primary.Object, secondary.Object };

		var removed = actors.RemovePhysicalInstance(secondary.Object);

		Assert.AreEqual(1, removed);
		Assert.AreEqual(1, actors.Count);
		Assert.AreSame(primary.Object, actors[0]);
	}

	[TestMethod]
	public void PhysicalInstanceDictionaryComparer_AllowsSameIdentityDifferentInstances()
	{
		var primary = CreateCharacter(10, 10, 100);
		var secondary = CreateCharacter(20, 10, 200);
		var roles = new Dictionary<ICharacter, string>(CharacterPhysicalInstanceEqualityComparer.Instance)
		{
			[primary.Object] = "primary",
			[secondary.Object] = "secondary"
		};

		Assert.AreEqual(2, roles.Count);
		Assert.AreEqual("primary", roles[primary.Object]);
		Assert.AreEqual("secondary", roles[secondary.Object]);
	}

	[TestMethod]
	public void ResolvePhysicalInstance_ExplicitInstanceUsesLoadedIdentityInstance()
	{
		var identity = new Mock<ICharacterIdentity>();
		var primary = CreateCharacterInstance(10, 10, 100, identity);
		var secondary = CreateCharacterInstance(20, 10, 200, identity);
		identity.SetupGet(x => x.Instances).Returns(new[] { primary.Object, secondary.Object });
		var gameworld = new Mock<IFuturemud>();
		gameworld.Setup(x => x.TryGetCharacter(10, true)).Returns(primary.Object);

		var result = CharacterInstanceIdentityComparer.ResolvePhysicalInstance(gameworld.Object, 10, 200,
			fallbackToPrimary: false);

		Assert.AreSame(secondary.Object, result);
	}

	[TestMethod]
	public void ResolvePhysicalInstance_MissingExplicitInstanceDoesNotFallbackWhenDisabled()
	{
		var identity = new Mock<ICharacterIdentity>();
		var primary = CreateCharacterInstance(10, 10, 100, identity);
		identity.SetupGet(x => x.Instances).Returns(new[] { primary.Object });
		var gameworld = new Mock<IFuturemud>();
		gameworld.Setup(x => x.TryGetCharacter(10, true)).Returns(primary.Object);

		var result = CharacterInstanceIdentityComparer.ResolvePhysicalInstance(gameworld.Object, 10, 999,
			fallbackToPrimary: false);

		Assert.IsNull(result);
	}

	[TestMethod]
	public void ResolveActorReference_LegacyIdentityOnly_ReturnsPrimaryFallback()
	{
		var identity = new Mock<ICharacterIdentity>();
		var primary = CreateCharacterInstance(10, 10, 100, identity);
		identity.SetupGet(x => x.Instances).Returns(new[] { primary.Object });
		var gameworld = new Mock<IFuturemud>();
		gameworld.Setup(x => x.TryGetCharacter(10, true)).Returns(primary.Object);

		var result = gameworld.Object.ResolveActorReference(new CharacterActorReference(10));

		Assert.AreEqual(CharacterActorReferenceResolutionStatus.LoadedPrimaryFallback, result.Status);
		Assert.AreSame(primary.Object, result.Actor);
	}

	[TestMethod]
	public void ResolveActorReference_ExplicitInstance_ReturnsSpecificInstance()
	{
		var identity = new Mock<ICharacterIdentity>();
		var primary = CreateCharacterInstance(10, 10, 100, identity);
		var secondary = CreateCharacterInstance(20, 10, 200, identity);
		identity.SetupGet(x => x.Instances).Returns(new[] { primary.Object, secondary.Object });
		var gameworld = new Mock<IFuturemud>();
		gameworld.Setup(x => x.TryGetCharacter(10, true)).Returns(primary.Object);

		var result = gameworld.Object.ResolveActorReference(
			new CharacterActorReference(10, 200, ReferenceKind: CharacterActorReferenceKind.SpecificInstance));

		Assert.AreEqual(CharacterActorReferenceResolutionStatus.LoadedSpecificInstance, result.Status);
		Assert.AreSame(secondary.Object, result.Actor);
	}

	[TestMethod]
	public void ResolveActorReference_MissingExplicitInstance_DoesNotFallbackToPrimary()
	{
		var identity = new Mock<ICharacterIdentity>();
		var primary = CreateCharacterInstance(10, 10, 100, identity);
		identity.SetupGet(x => x.Instances).Returns(new[] { primary.Object });
		var gameworld = new Mock<IFuturemud>();
		gameworld.Setup(x => x.TryGetCharacter(10, true)).Returns(primary.Object);

		var result = gameworld.Object.ResolveActorReference(
			new CharacterActorReference(10, 999, ReferenceKind: CharacterActorReferenceKind.SpecificInstance));

		Assert.AreEqual(CharacterActorReferenceResolutionStatus.InstanceUnavailable, result.Status);
		Assert.IsNull(result.Actor);
	}

	[TestMethod]
	public void RenderStaffActorReference_IncludesIdentityInstanceBodyKindAndLocation()
	{
		var character = CreateCharacter(20, 10, 200, CharacterInstanceKind.PhysicalClone);
		var body = new Mock<IBody>();
		body.SetupGet(x => x.Id).Returns(30);
		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.Id).Returns(40);
		character.SetupGet(x => x.Body).Returns(body.Object);
		character.SetupGet(x => x.Location).Returns(cell.Object);
		character.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		character.Setup(x => x.HowSeen(character.Object, false, DescriptionType.Short, false,
				PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreSelf))
		         .Returns("a clone");

		var text = character.Object.RenderStaffActorReference();

		StringAssert.Contains(text, "identity #10");
		StringAssert.Contains(text, "instance #200");
		StringAssert.Contains(text, "body #30");
		StringAssert.Contains(text, "Physical Clone");
		StringAssert.Contains(text, "cell #40");
	}

	private static Mock<ICharacter> CreateCharacter(long characterId, long identityId, long instanceId,
		CharacterInstanceKind instanceKind = CharacterInstanceKind.Other, long? bodyId = null)
	{
		var identity = new Mock<ICharacterIdentity>();
		identity.SetupGet(x => x.Id).Returns(identityId);

		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Id).Returns(characterId);
		character.SetupGet(x => x.Identity).Returns(identity.Object);
		character.SetupGet(x => x.InstanceId).Returns(instanceId);
		character.SetupGet(x => x.InstanceKind).Returns(instanceKind);
		if (bodyId is not null)
		{
			var body = new Mock<IBody>();
			body.SetupGet(x => x.Id).Returns(bodyId.Value);
			body.SetupGet(x => x.Actor).Returns(character.Object);
			character.SetupGet(x => x.Body).Returns(body.Object);
		}

		return character;
	}

	private static Mock<ICharacterInstance> CreateCharacterInstance(long characterId, long identityId, long instanceId,
		Mock<ICharacterIdentity>? identity = null)
	{
		identity ??= new Mock<ICharacterIdentity>();
		identity.SetupGet(x => x.Id).Returns(identityId);

		var character = new Mock<ICharacterInstance>();
		character.SetupGet(x => x.Id).Returns(characterId);
		character.SetupGet(x => x.Identity).Returns(identity.Object);
		character.SetupGet(x => x.InstanceId).Returns(instanceId);
		character.SetupGet(x => x.InstanceKind).Returns(CharacterInstanceKind.Other);
		return character;
	}
}
