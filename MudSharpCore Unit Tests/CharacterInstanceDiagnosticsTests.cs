#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
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
		identity.SetupGet(x => x.Instances).Returns(new[] { instance.Object });
		instance.SetupGet(x => x.IsPrimaryInstance).Returns(true);
		instance.SetupGet(x => x.IsEmbodied).Returns(true);
		instance.SetupGet(x => x.Body).Returns(body.Object);
		instance.SetupGet(x => x.State).Returns(CharacterState.Awake);

		var diagnostics = CharacterInstanceDiagnostics.AuditPrimaryInstance(character.Object);

		Assert.AreEqual(0, diagnostics.Count);
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
}
