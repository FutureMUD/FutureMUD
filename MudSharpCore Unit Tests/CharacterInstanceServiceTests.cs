#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.NPC;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CharacterInstanceServiceTests
{
	[TestMethod]
	public void ValidateSecondarySpawnMode_Passive_AllowsPlayerCharacters()
	{
		var pc = BuildPlayerCharacter();

		var result = CharacterInstanceService.ValidateSecondarySpawnMode(pc.Object,
			SecondaryCharacterInstanceSpawnMode.Passive);

		Assert.IsTrue(result.Success);
	}

	[TestMethod]
	public void ValidateSecondarySpawnMode_PlayerFocusable_RejectsNpcs()
	{
		var npc = BuildNpc();

		var result = CharacterInstanceService.ValidateSecondarySpawnMode(npc.Object,
			SecondaryCharacterInstanceSpawnMode.PlayerFocusable);

		Assert.IsFalse(result.Success);
		StringAssert.Contains(result.Message, "player characters");
	}

	[TestMethod]
	public void ValidateSecondarySpawnMode_NpcAiControlled_AllowsNpcs()
	{
		var npc = BuildNpc();

		var result = CharacterInstanceService.ValidateSecondarySpawnMode(npc.Object,
			SecondaryCharacterInstanceSpawnMode.NpcAiControlled);

		Assert.IsTrue(result.Success);
	}

	[TestMethod]
	public void ValidateSecondarySpawnMode_NpcAiControlled_RejectsPlayerCharacters()
	{
		var pc = BuildPlayerCharacter();

		var result = CharacterInstanceService.ValidateSecondarySpawnMode(pc.Object,
			SecondaryCharacterInstanceSpawnMode.NpcAiControlled);

		Assert.IsFalse(result.Success);
		StringAssert.Contains(result.Message, "NPC identities");
	}

	private static Mock<ICharacter> BuildPlayerCharacter()
	{
		var identity = new Mock<ICharacterIdentity>();
		var pc = new Mock<ICharacter>();
		pc.SetupGet(x => x.Identity).Returns(identity.Object);
		pc.SetupGet(x => x.IsPlayerCharacter).Returns(true);
		pc.SetupGet(x => x.IsGuest).Returns(false);
		return pc;
	}

	private static Mock<INPC> BuildNpc()
	{
		var identity = new Mock<ICharacterIdentity>();
		var npc = new Mock<INPC>();
		npc.SetupGet(x => x.Identity).Returns(identity.Object);
		npc.SetupGet(x => x.IsPlayerCharacter).Returns(false);
		npc.SetupGet(x => x.IsGuest).Returns(false);
		return npc;
	}
}
