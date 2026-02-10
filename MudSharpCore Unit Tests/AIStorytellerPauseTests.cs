using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.RPG.AIStorytellers;
using ModelStoryteller = MudSharp.Models.AIStoryteller;

namespace MudSharp_Unit_Tests;

[TestClass]
public class AIStorytellerPauseTests
{
	[TestMethod]
	public void Pause_Unpause_TogglesPausedState()
	{
		var storyteller = new AIStoryteller(CreateModel(isPaused: false), CreateGameworld().Object);

		Assert.IsFalse(storyteller.IsPaused);
		storyteller.Pause();
		Assert.IsTrue(storyteller.IsPaused);
		storyteller.Unpause();
		Assert.IsFalse(storyteller.IsPaused);
	}

	private static Mock<IFuturemud> CreateGameworld()
	{
		var progRepo = new Mock<IUneditableAll<IFutureProg>>();
		progRepo.Setup(x => x.Get(It.IsAny<long>())).Returns((IFutureProg)null);
		var saveManager = new Mock<ISaveManager>();

		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.FutureProgs).Returns(progRepo.Object);
		gameworld.SetupGet(x => x.SaveManager).Returns(saveManager.Object);
		return gameworld;
	}

	private static ModelStoryteller CreateModel(bool isPaused)
	{
		return new ModelStoryteller
		{
			Id = 1L,
			Name = "Test Storyteller",
			Description = "Test",
			Model = "gpt-5",
			SystemPrompt = "System prompt",
			AttentionAgentPrompt = "Attention prompt",
			SurveillanceStrategyDefinition = string.Empty,
			ReasoningEffort = "2",
			CustomToolCallsDefinition = "<ToolCalls />",
			SubscribeToRoomEvents = false,
			SubscribeTo5mHeartbeat = false,
			SubscribeTo10mHeartbeat = false,
			SubscribeTo30mHeartbeat = false,
			SubscribeToHourHeartbeat = false,
			IsPaused = isPaused
		};
	}
}
