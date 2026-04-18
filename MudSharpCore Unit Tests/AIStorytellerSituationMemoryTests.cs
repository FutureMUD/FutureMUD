using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.RPG.AIStorytellers;
using System;
using ModelMemory = MudSharp.Models.AIStorytellerCharacterMemory;
using ModelSituation = MudSharp.Models.AIStorytellerSituation;

namespace MudSharp_Unit_Tests;

[TestClass]
public class AIStorytellerSituationMemoryTests
{
    [TestMethod]
    public void Situation_UpdateAndResolve_UpdatesLifecycleState()
    {
        Mock<IFuturemud> gameworld = new();
        gameworld.SetupGet(x => x.SaveManager).Returns(new Mock<ISaveManager>().Object);
        IAIStoryteller storyteller = CreateStorytellerWithWorld(gameworld.Object);
        AIStorytellerSituation situation = new(
            new ModelSituation
            {
                Id = 1L,
                AIStorytellerId = storyteller.Id,
                Name = "Before",
                SituationText = "Before details",
                CreatedOn = new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc),
                IsResolved = false
            },
            storyteller);

        situation.UpdateSituation("After", "After details");
        situation.Resolve();

        Assert.AreEqual("After", situation.Name);
        Assert.AreEqual("After details", situation.SituationText);
        Assert.IsTrue(situation.IsResolved);
    }

    [TestMethod]
    public void Situation_SetScope_UpdatesScopeState()
    {
        Mock<IFuturemud> gameworld = new();
        gameworld.SetupGet(x => x.SaveManager).Returns(new Mock<ISaveManager>().Object);
        IAIStoryteller storyteller = CreateStorytellerWithWorld(gameworld.Object);
        AIStorytellerSituation situation = new(
            new ModelSituation
            {
                Id = 3L,
                AIStorytellerId = storyteller.Id,
                Name = "Scoped",
                SituationText = "Scoped details",
                CreatedOn = new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc),
                IsResolved = false
            },
            storyteller);

        situation.SetScope(55L, null);
        Assert.AreEqual(55L, situation.ScopeCharacterId);
        Assert.IsNull(situation.ScopeRoomId);

        situation.SetScope(null, 102L);
        Assert.IsNull(situation.ScopeCharacterId);
        Assert.AreEqual(102L, situation.ScopeRoomId);

        situation.SetScope(null, null);
        Assert.IsNull(situation.ScopeCharacterId);
        Assert.IsNull(situation.ScopeRoomId);
    }

    [TestMethod]
    public void Memory_UpdateMemory_UpdatesLifecycleState()
    {
        Mock<ICharacter> character = new();
        character.SetupGet(x => x.Id).Returns(55L);

        Mock<IFuturemud> gameworld = new();
        gameworld.SetupGet(x => x.SaveManager).Returns(new Mock<ISaveManager>().Object);
        gameworld.Setup(x => x.TryGetCharacter(55L, true)).Returns(character.Object);
        IAIStoryteller storyteller = CreateStorytellerWithWorld(gameworld.Object);

        AIStorytellerCharacterMemory memory = new(
            new ModelMemory
            {
                Id = 2L,
                AIStorytellerId = storyteller.Id,
                CharacterId = 55L,
                MemoryTitle = "Old",
                MemoryText = "Old details",
                CreatedOn = new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc)
            },
            storyteller);

        memory.UpdateMemory("New", "New details");

        Assert.AreEqual("New", memory.MemoryTitle);
        Assert.AreEqual("New details", memory.MemoryText);
    }

    private static IAIStoryteller CreateStorytellerWithWorld(IFuturemud gameworld)
    {
        Mock<IAIStoryteller> storyteller = new();
        storyteller.SetupGet(x => x.Id).Returns(99L);
        storyteller.SetupGet(x => x.Gameworld).Returns(gameworld);
        return storyteller.Object;
    }
}
