#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CharacterInstanceDiagnosticsTests
{
	[TestMethod]
	public void AuditPrimaryInstance_CompatiblePrimaryInstance_ReturnsNoDiagnostics()
	{
		var location = new Mock<ICell>();
		var body = new Mock<IBody>();
		var form = new Mock<ICharacterForm>();
		var character = new Mock<ICharacter>();
		var identity = new Mock<ICharacterIdentity>();
		var instance = new Mock<ICharacterInstance>();

		character.SetupGet(x => x.Id).Returns(10);
		character.SetupGet(x => x.Identity).Returns(identity.Object);
		character.SetupGet(x => x.Body).Returns(body.Object);
		character.SetupGet(x => x.CurrentBody).Returns(body.Object);
		character.SetupGet(x => x.Location).Returns(location.Object);
		character.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		character.SetupGet(x => x.Bodies).Returns(new[] { body.Object });
		character.SetupGet(x => x.Forms).Returns(new[] { form.Object });

		body.SetupGet(x => x.Id).Returns(20);
		body.SetupGet(x => x.Actor).Returns(character.Object);
		body.SetupGet(x => x.Location).Returns(location.Object);
		body.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		form.SetupGet(x => x.Body).Returns(body.Object);
		identity.SetupGet(x => x.Id).Returns(10);
		identity.SetupGet(x => x.PrimaryInstance).Returns(instance.Object);
		identity.SetupGet(x => x.Instances).Returns(new[] { instance.Object });
		instance.SetupGet(x => x.Identity).Returns(identity.Object);
		instance.SetupGet(x => x.InstanceId).Returns(100);
		instance.SetupGet(x => x.IsPrimaryInstance).Returns(true);
		instance.SetupGet(x => x.InstanceKind).Returns(CharacterInstanceKind.Primary);
		instance.SetupGet(x => x.DeathPolicy).Returns(CharacterInstanceDeathPolicy.FinalCharacterDeath);
		instance.SetupGet(x => x.PerceptionPolicy).Returns(CharacterInstancePerceptionPolicy.OrdinaryEmbodied);
		instance.SetupGet(x => x.PersistencePolicy).Returns(CharacterInstancePersistencePolicy.Persistent);
		instance.SetupGet(x => x.IsEmbodied).Returns(true);
		instance.SetupGet(x => x.Body).Returns(body.Object);
		instance.SetupGet(x => x.Location).Returns(location.Object);
		instance.SetupGet(x => x.State).Returns(CharacterState.Awake);
		instance.SetupGet(x => x.Status).Returns(CharacterStatus.Active);

		var diagnostics = CharacterInstanceDiagnostics.AuditPrimaryInstance(character.Object);

		Assert.AreEqual(0, diagnostics.Count);
	}

	[TestMethod]
	public void AuditLoadedIdentity_DuplicatePrimaryAndEmbodiedBody_ReturnsDiagnostics()
	{
		var identity = new Mock<ICharacterIdentity>();
		identity.SetupGet(x => x.Id).Returns(10);
		var primary = BuildLoadedInstance(identity.Object, 100, 20, true);
		var duplicatePrimary = BuildLoadedInstance(identity.Object, 101, 20, true);
		identity.SetupGet(x => x.PrimaryInstance).Returns(primary.Object);
		identity.SetupGet(x => x.Instances).Returns(new[] { primary.Object, duplicatePrimary.Object });

		var diagnostics = CharacterInstanceDiagnostics.AuditLoadedIdentity(identity.Object);

		Assert.IsTrue(diagnostics.Any(x => x.Code == "duplicate-primary-instance"));
		Assert.IsTrue(diagnostics.Any(x => x.Code == "duplicate-embodied-body"));
	}

	[TestMethod]
	public void AuditLoadedIdentity_PolicyContradictions_ReturnsDiagnostics()
	{
		var identity = new Mock<ICharacterIdentity>();
		identity.SetupGet(x => x.Id).Returns(10);
		var primary = BuildLoadedInstance(identity.Object, 100, 20, true);
		primary.SetupGet(x => x.InstanceKind).Returns(CharacterInstanceKind.Other);
		var secondaryPrimaryKind = BuildLoadedInstance(identity.Object, 101, 21, false,
			CharacterInstanceKind.Primary);
		var controllablePassive = BuildLoadedInstance(identity.Object, 102, 22, false);
		controllablePassive.SetupGet(x => x.IsControllable).Returns(true);
		controllablePassive.SetupGet(x => x.ControlPolicy).Returns(CharacterInstanceControlPolicy.NotControllable);
		identity.SetupGet(x => x.PrimaryInstance).Returns(primary.Object);
		identity.SetupGet(x => x.Instances)
		        .Returns(new[] { primary.Object, secondaryPrimaryKind.Object, controllablePassive.Object });

		var diagnostics = CharacterInstanceDiagnostics.AuditLoadedIdentity(identity.Object);

		Assert.IsTrue(diagnostics.Any(x => x.Code == "primary-policy-mismatch"));
		Assert.IsTrue(diagnostics.Any(x => x.Code == "secondary-primary-flag-mismatch"));
		Assert.IsTrue(diagnostics.Any(x => x.Code == "controllable-with-not-controllable-policy"));
	}

	[TestMethod]
	public void AuditPersistedInstances_DuplicatePrimaryRows_ReturnsDiagnostic()
	{
		var diagnostics = CharacterInstanceDiagnostics.AuditPersistedInstances(new[]
		{
			new MudSharp.Models.CharacterInstance { CharacterId = 10, BodyId = 20, IsPrimary = true },
			new MudSharp.Models.CharacterInstance { CharacterId = 10, BodyId = 21, IsPrimary = true }
		});

		Assert.IsTrue(diagnostics.Any(x =>
			x.Code == "duplicate-primary-instance" &&
			x.Severity == CharacterInstanceDiagnosticSeverity.Error));
	}

	[TestMethod]
	public void AuditPersistedInstances_DuplicateLiveEmbodiedRows_ReturnsDiagnostic()
	{
		var diagnostics = CharacterInstanceDiagnostics.AuditPersistedInstances(new[]
		{
			new MudSharp.Models.CharacterInstance
			{
				CharacterId = 10,
				BodyId = 20,
				IsEmbodied = true,
				State = (int)CharacterState.Awake,
				Status = (int)CharacterStatus.Active
			},
			new MudSharp.Models.CharacterInstance
			{
				CharacterId = 11,
				BodyId = 20,
				IsEmbodied = true,
				State = (int)CharacterState.Awake,
				Status = (int)CharacterStatus.Active
			}
		});

		Assert.IsTrue(diagnostics.Any(x =>
			x.Code == "duplicate-embodied-body" &&
			x.Severity == CharacterInstanceDiagnosticSeverity.Error));
	}

	[TestMethod]
	public void AuditPersistedInstances_DuplicateDeadEmbodiedRows_IgnoresDeadRows()
	{
		var diagnostics = CharacterInstanceDiagnostics.AuditPersistedInstances(new[]
		{
			new MudSharp.Models.CharacterInstance
			{
				CharacterId = 10,
				BodyId = 20,
				IsEmbodied = true,
				State = (int)CharacterState.Dead,
				Status = (int)CharacterStatus.Deceased
			},
			new MudSharp.Models.CharacterInstance
			{
				CharacterId = 11,
				BodyId = 20,
				IsEmbodied = true,
				State = (int)CharacterState.Awake,
				Status = (int)CharacterStatus.Active
			}
		});

		Assert.IsFalse(diagnostics.Any(x => x.Code == "duplicate-embodied-body"));
	}

	[TestMethod]
	public void AuditPersistedInstances_StaleReferencesAndMalformedEffects_ReturnDiagnostics()
	{
		var diagnostics = CharacterInstanceDiagnostics.AuditPersistedInstances(new[]
		{
			PersistedInstance(id: 1, bodyId: 99, locationId: 88, effectData: "<Effects>")
		}, true, new CharacterInstanceReferenceSets(
			new HashSet<long> { 20 },
			new HashSet<long> { 30 }));

		Assert.IsTrue(diagnostics.Any(x => x.Code == "stale-body-reference"));
		Assert.IsTrue(diagnostics.Any(x => x.Code == "stale-location-reference"));
		Assert.IsTrue(diagnostics.Any(x => x.Code == "malformed-effect-data"));
	}

	[TestMethod]
	public void AuditPersistedInstances_PolicyContradictions_ReturnDiagnostics()
	{
		var primaryMismatch = PersistedInstance(id: 1, isPrimary: true, instanceKind: CharacterInstanceKind.Other,
			deathPolicy: CharacterInstanceDeathPolicy.DestroyInstanceOnly,
			persistencePolicy: CharacterInstancePersistencePolicy.DespawnOnReboot,
			isEmbodied: false);
		var secondaryPrimaryKind = PersistedInstance(id: 2, instanceKind: CharacterInstanceKind.Primary);
		var controllablePassive = PersistedInstance(id: 3, isControllable: true,
			controlPolicy: CharacterInstanceControlPolicy.NotControllable);
		var embodiedWithoutLocation = PersistedInstance(id: 4, locationId: null);

		var diagnostics = CharacterInstanceDiagnostics.AuditPersistedInstances(new[]
		{
			primaryMismatch,
			secondaryPrimaryKind,
			controllablePassive,
			embodiedWithoutLocation
		});

		Assert.IsTrue(diagnostics.Any(x => x.Code == "primary-policy-mismatch"));
		Assert.IsTrue(diagnostics.Any(x => x.Code == "secondary-primary-flag-mismatch"));
		Assert.IsTrue(diagnostics.Any(x => x.Code == "controllable-with-not-controllable-policy"));
		Assert.IsTrue(diagnostics.Any(x => x.Code == "embodied-without-location"));
	}

	[TestMethod]
	public void RenderDiagnosticsTable_IncludesSeverityCodeScopeAndSubject()
	{
		var rendered = CharacterInstanceDiagnostics.RenderDiagnosticsTable(new[]
		{
			new CharacterInstanceDiagnostic(
				CharacterInstanceDiagnosticSeverity.Error,
				CharacterInstanceStateScope.Instance,
				"test-code",
				"Test message.",
				new CharacterInstanceDiagnosticSubject(10, 20, 30, 40))
		}, 120, false);

		StringAssert.Contains(rendered, "Error");
		StringAssert.Contains(rendered, "test-code");
		StringAssert.Contains(rendered, "Instance");
		StringAssert.Contains(rendered, "C#10 I#20 B#30 L#40");
		StringAssert.Contains(rendered, "Test message.");
	}

	private static Mock<ICharacterInstance> BuildLoadedInstance(ICharacterIdentity identity, long instanceId,
		long bodyId, bool primary, CharacterInstanceKind? instanceKind = null)
	{
		var body = new Mock<IBody>();
		body.SetupGet(x => x.Id).Returns(bodyId);
		var location = new Mock<ICell>();
		location.SetupGet(x => x.Id).Returns(30 + instanceId);
		var instance = new Mock<ICharacterInstance>();
		instance.SetupGet(x => x.Identity).Returns(identity);
		instance.SetupGet(x => x.Id).Returns(identity.Id);
		instance.SetupGet(x => x.InstanceId).Returns(instanceId);
		instance.SetupGet(x => x.IsPrimaryInstance).Returns(primary);
		instance.SetupGet(x => x.InstanceKind).Returns(instanceKind ?? (primary
			? CharacterInstanceKind.Primary
			: CharacterInstanceKind.Other));
		instance.SetupGet(x => x.ControlPolicy).Returns(CharacterInstanceControlPolicy.NotControllable);
		instance.SetupGet(x => x.DeathPolicy).Returns(primary
			? CharacterInstanceDeathPolicy.FinalCharacterDeath
			: CharacterInstanceDeathPolicy.DestroyInstanceOnly);
		instance.SetupGet(x => x.PerceptionPolicy).Returns(CharacterInstancePerceptionPolicy.OrdinaryEmbodied);
		instance.SetupGet(x => x.PersistencePolicy).Returns(primary
			? CharacterInstancePersistencePolicy.Persistent
			: CharacterInstancePersistencePolicy.DespawnOnReboot);
		instance.SetupGet(x => x.IsEmbodied).Returns(true);
		instance.SetupGet(x => x.IsControllable).Returns(false);
		instance.SetupGet(x => x.Body).Returns(body.Object);
		instance.SetupGet(x => x.Location).Returns(location.Object);
		instance.SetupGet(x => x.State).Returns(CharacterState.Awake);
		instance.SetupGet(x => x.Status).Returns(CharacterStatus.Active);
		return instance;
	}

	private static MudSharp.Models.CharacterInstance PersistedInstance(long id, long characterId = 10,
		long bodyId = 20, long? locationId = 30, bool isPrimary = false,
		CharacterInstanceKind instanceKind = CharacterInstanceKind.Other,
		CharacterInstanceControlPolicy controlPolicy = CharacterInstanceControlPolicy.NotControllable,
		CharacterInstanceDeathPolicy deathPolicy = CharacterInstanceDeathPolicy.DestroyInstanceOnly,
		CharacterInstancePerceptionPolicy perceptionPolicy = CharacterInstancePerceptionPolicy.OrdinaryEmbodied,
		CharacterInstancePersistencePolicy persistencePolicy = CharacterInstancePersistencePolicy.DespawnOnReboot,
		bool isEmbodied = true,
		bool isControllable = false,
		string effectData = "<Effects/>")
	{
		return new MudSharp.Models.CharacterInstance
		{
			Id = id,
			CharacterId = characterId,
			BodyId = bodyId,
			LocationId = locationId,
			IsPrimary = isPrimary,
			InstanceKind = (int)instanceKind,
			ControlPolicy = (int)controlPolicy,
			DeathPolicy = (int)deathPolicy,
			PerceptionPolicy = (int)perceptionPolicy,
			PersistencePolicy = (int)persistencePolicy,
			IsEmbodied = isEmbodied,
			IsControllable = isControllable,
			State = (int)CharacterState.Awake,
			Status = (int)CharacterStatus.Active,
			EffectData = effectData
		};
	}
}
