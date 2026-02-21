#nullable enable

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using MudSharp.Work.Crafts;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CraftToolUsageTests
{
	[TestMethod]
	public void HandleCraftPhase_ToolUsedInMultiplePhases_UsesToolEachPhase()
	{
		var tags = new Mock<IUneditableAll<ITag>>();
		tags.Setup(x => x.Get(It.IsAny<long>())).Returns((ITag)null!);

		var traits = new Mock<IUneditableAll<ITraitDefinition>>();
		traits.Setup(x => x.Get(It.IsAny<long>())).Returns((ITraitDefinition)null!);

		var progs = new Mock<IUneditableAll<IFutureProg>>();
		progs.Setup(x => x.Get(It.IsAny<long>())).Returns((IFutureProg)null!);

		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Tags).Returns(tags.Object);
		gameworld.SetupGet(x => x.Traits).Returns(traits.Object);
		gameworld.SetupGet(x => x.FutureProgs).Returns(progs.Object);
		gameworld.Setup(x => x.DebugMessage(It.IsAny<string>()));

		var toolItemComponent = new Mock<IToolItem>();

		var proto = new Mock<IGameItemProto>();
		proto.SetupGet(x => x.Id).Returns(1L);

		var toolItem = new Mock<IGameItem>();
		toolItem.SetupProperty(x => x.Condition, 1.0);
		toolItem.SetupGet(x => x.Prototype).Returns(proto.Object);
		toolItem.SetupGet(x => x.ContainedIn).Returns((IGameItem)null!);
		toolItem.SetupGet(x => x.InInventoryOf).Returns((IBody)null!);
		toolItem.Setup(x => x.IsA(It.IsAny<ITag>())).Returns(true);
		toolItem.Setup(x => x.GetItemType<IToolItem>()).Returns(toolItemComponent.Object);

		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.GameItems).Returns(new List<IGameItem> { toolItem.Object });
		cell.Setup(x => x.LayerGameItems(It.IsAny<RoomLayer>())).Returns(new List<IGameItem> { toolItem.Object });

		var body = new Mock<IBody>();
		body.SetupGet(x => x.WieldLocs).Returns(Array.Empty<IWield>());
		body.SetupGet(x => x.HoldLocs).Returns(Array.Empty<IGrab>());
		body.SetupGet(x => x.WornItems).Returns(Array.Empty<IGameItem>());
		body.SetupGet(x => x.WieldedItems).Returns(Array.Empty<IGameItem>());
		body.SetupGet(x => x.HeldItems).Returns(Array.Empty<IGameItem>());
		body.SetupGet(x => x.DirectItems).Returns(Array.Empty<IGameItem>());

		var outputHandler = new Mock<IOutputHandler>();
		outputHandler.Setup(x => x.Send(It.IsAny<IOutput>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);

		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Body).Returns(body.Object);
		character.SetupGet(x => x.Location).Returns(cell.Object);
		character.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		character.SetupGet(x => x.OutputHandler).Returns(outputHandler.Object);
		character.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()));
		character.Setup(x => x.AddEffect(It.IsAny<IEffect>()));
		character.Setup(x => x.RemoveAllEffects(It.IsAny<Predicate<IEffect>>(), It.IsAny<bool>()));
		character.Setup(x => x.EffectsOfType<IInventoryPlanItemEffect>(It.IsAny<Predicate<IInventoryPlanItemEffect>>()))
		         .Returns(Array.Empty<IInventoryPlanItemEffect>());
		outputHandler.SetupGet(x => x.Perceiver).Returns(character.Object);

		var craftModel = new MudSharp.Models.Craft
		{
			Id = 1,
			RevisionNumber = 0,
			Name = "Test Craft",
			Blurb = "Test",
			ActionDescription = "crafting",
			Category = "test",
			Interruptable = false,
			ToolQualityWeighting = 1.0,
			InputQualityWeighting = 1.0,
			CheckQualityWeighting = 1.0,
			FreeSkillChecks = 0,
			FailThreshold = (int)Outcome.MinorFail,
			CheckDifficulty = (int)Difficulty.Normal,
			FailPhase = 1,
			QualityFormula = "5",
			ActiveCraftItemSdesc = "a craft",
			IsPracticalCheck = true,
			EditableItem = new MudSharp.Models.EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = (int)RevisionStatus.Current,
				BuilderAccountId = 1,
				BuilderDate = DateTime.UtcNow,
				BuilderComment = string.Empty,
				ReviewerComment = string.Empty
			}
		};

		craftModel.CraftPhases.Add(new MudSharp.Models.CraftPhase
		{
			PhaseNumber = 1,
			PhaseLengthInSeconds = 5.0,
			Echo = "Phase 1",
			FailEcho = "Fail 1",
			ExertionLevel = 0,
			StaminaUsage = 0.0
		});

		craftModel.CraftPhases.Add(new MudSharp.Models.CraftPhase
		{
			PhaseNumber = 2,
			PhaseLengthInSeconds = 5.0,
			Echo = "Phase 2",
			FailEcho = "Fail 2",
			ExertionLevel = 0,
			StaminaUsage = 0.0
		});

		craftModel.CraftTools.Add(new MudSharp.Models.CraftTool
		{
			Id = 1,
			ToolType = "SimpleTool",
			Definition = "<Definition><TargetItemId>1</TargetItemId></Definition>",
			DesiredState = (int)DesiredItemState.InRoom,
			ToolQualityWeight = 1.0,
			UseToolDuration = true,
			OriginalAdditionTime = DateTime.UtcNow,
			CraftId = craftModel.Id,
			CraftRevisionNumber = craftModel.RevisionNumber
		});

		var craft = new Craft(craftModel, gameworld.Object)
		{
			QualityFormula = null
		};

		var craftComponent = new Mock<IActiveCraftGameItemComponent>();
		craftComponent.SetupProperty(x => x.HasFailed, false);
		craftComponent.SetupProperty(x => x.Phase, 1);
		craftComponent.SetupProperty(x => x.QualityCheckOutcome, Outcome.MajorPass);
		craftComponent.SetupGet(x => x.ConsumedInputs).Returns(new Dictionary<ICraftInput, (IPerceivable Input, ICraftInputData Data)>());
		craftComponent.SetupGet(x => x.ProducedProducts).Returns(new Dictionary<ICraftProduct, ICraftProductData>());
		craftComponent.SetupGet(x => x.UsedToolQualities).Returns(new Dictionary<ICraftTool, (ItemQuality Quality, double Weight)>());
		craftComponent.SetupGet(x => x.Craft).Returns(craft);

		var effect = new Mock<IActiveCraftEffect>();
		effect.SetupProperty(x => x.NextPhaseDuration);

		Assert.IsTrue(craft.HandleCraftPhase(character.Object, effect.Object, craftComponent.Object, 1));
		craftComponent.Object.Phase = 2;
		Assert.IsTrue(craft.HandleCraftPhase(character.Object, effect.Object, craftComponent.Object, 2));

		toolItemComponent.Verify(x => x.UseTool(null!, It.IsAny<TimeSpan>()), Times.Exactly(2));
	}

	[TestMethod]
	public void HandleCraftPhase_TagToolUsedInTwoPhases_ReducesToolConditionEachPhase()
	{
		var toolTag = new Mock<ITag>();
		toolTag.Setup(x => x.IsA(It.IsAny<ITag>())).Returns(true);

		var tags = new Mock<IUneditableAll<ITag>>();
		tags.Setup(x => x.Get(1L)).Returns(toolTag.Object);
		tags.Setup(x => x.Get(It.Is<long>(id => id != 1L))).Returns((ITag)null!);

		var traits = new Mock<IUneditableAll<ITraitDefinition>>();
		traits.Setup(x => x.Get(It.IsAny<long>())).Returns((ITraitDefinition)null!);

		var progs = new Mock<IUneditableAll<IFutureProg>>();
		progs.Setup(x => x.Get(It.IsAny<long>())).Returns((IFutureProg)null!);

		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Tags).Returns(tags.Object);
		gameworld.SetupGet(x => x.Traits).Returns(traits.Object);
		gameworld.SetupGet(x => x.FutureProgs).Returns(progs.Object);
		gameworld.Setup(x => x.DebugMessage(It.IsAny<string>()));

		var toolItemComponent = new Mock<IToolItem>();
		toolItemComponent.Setup(x => x.CountAsTool(toolTag.Object)).Returns(true);
		toolItemComponent.Setup(x => x.ToolTimeMultiplier(toolTag.Object)).Returns(0.8);

		var proto = new Mock<IGameItemProto>();
		proto.SetupGet(x => x.Id).Returns(1L);

		var toolItem = new Mock<IGameItem>();
		toolItem.SetupProperty(x => x.Condition, 1.0);
		toolItem.SetupGet(x => x.Prototype).Returns(proto.Object);
		toolItem.SetupGet(x => x.ContainedIn).Returns((IGameItem)null!);
		toolItem.SetupGet(x => x.InInventoryOf).Returns((IBody)null!);
		toolItem.Setup(x => x.IsA(It.IsAny<ITag>())).Returns(true);
		toolItem.Setup(x => x.GetItemType<IToolItem>()).Returns(toolItemComponent.Object);

		toolItemComponent
			.Setup(x => x.UseTool(toolTag.Object, It.IsAny<TimeSpan>()))
			.Callback<ITag, TimeSpan>((_, usage) => { toolItem.Object.Condition -= usage.TotalSeconds / 60.0; });

		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.GameItems).Returns(new List<IGameItem> { toolItem.Object });
		cell.Setup(x => x.LayerGameItems(It.IsAny<RoomLayer>())).Returns(new List<IGameItem> { toolItem.Object });

		var body = new Mock<IBody>();
		body.SetupGet(x => x.WieldLocs).Returns(Array.Empty<IWield>());
		body.SetupGet(x => x.HoldLocs).Returns(Array.Empty<IGrab>());
		body.SetupGet(x => x.WornItems).Returns(Array.Empty<IGameItem>());
		body.SetupGet(x => x.WieldedItems).Returns(Array.Empty<IGameItem>());
		body.SetupGet(x => x.HeldItems).Returns(Array.Empty<IGameItem>());
		body.SetupGet(x => x.DirectItems).Returns(Array.Empty<IGameItem>());

		var outputHandler = new Mock<IOutputHandler>();
		outputHandler.Setup(x => x.Send(It.IsAny<IOutput>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);

		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Body).Returns(body.Object);
		character.SetupGet(x => x.Location).Returns(cell.Object);
		character.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		character.SetupGet(x => x.OutputHandler).Returns(outputHandler.Object);
		character.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()));
		character.Setup(x => x.AddEffect(It.IsAny<IEffect>()));
		character.Setup(x => x.RemoveAllEffects(It.IsAny<Predicate<IEffect>>(), It.IsAny<bool>()));
		character.Setup(x => x.EffectsOfType<IInventoryPlanItemEffect>(It.IsAny<Predicate<IInventoryPlanItemEffect>>()))
		         .Returns(Array.Empty<IInventoryPlanItemEffect>());
		outputHandler.SetupGet(x => x.Perceiver).Returns(character.Object);

		var craftModel = new MudSharp.Models.Craft
		{
			Id = 1,
			RevisionNumber = 0,
			Name = "Test Craft",
			Blurb = "Test",
			ActionDescription = "crafting",
			Category = "test",
			Interruptable = false,
			ToolQualityWeighting = 1.0,
			InputQualityWeighting = 1.0,
			CheckQualityWeighting = 1.0,
			FreeSkillChecks = 0,
			FailThreshold = (int)Outcome.MinorFail,
			CheckDifficulty = (int)Difficulty.Normal,
			FailPhase = 1,
			QualityFormula = "5",
			ActiveCraftItemSdesc = "a craft",
			IsPracticalCheck = true,
			EditableItem = new MudSharp.Models.EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = (int)RevisionStatus.Current,
				BuilderAccountId = 1,
				BuilderDate = DateTime.UtcNow,
				BuilderComment = string.Empty,
				ReviewerComment = string.Empty
			}
		};

		craftModel.CraftPhases.Add(new MudSharp.Models.CraftPhase
		{
			PhaseNumber = 1,
			PhaseLengthInSeconds = 5.0,
			Echo = "Phase 1 uses $t1",
			FailEcho = "Fail 1",
			ExertionLevel = 0,
			StaminaUsage = 0.0
		});

		craftModel.CraftPhases.Add(new MudSharp.Models.CraftPhase
		{
			PhaseNumber = 2,
			PhaseLengthInSeconds = 5.0,
			Echo = "Phase 2 uses $t1",
			FailEcho = "Fail 2",
			ExertionLevel = 0,
			StaminaUsage = 0.0
		});

		craftModel.CraftTools.Add(new MudSharp.Models.CraftTool
		{
			Id = 1,
			ToolType = "TagTool",
			Definition = "<Definition><TargetItemTag>1</TargetItemTag></Definition>",
			DesiredState = (int)DesiredItemState.InRoom,
			ToolQualityWeight = 1.0,
			UseToolDuration = true,
			OriginalAdditionTime = DateTime.UtcNow,
			CraftId = craftModel.Id,
			CraftRevisionNumber = craftModel.RevisionNumber
		});

		var craft = new Craft(craftModel, gameworld.Object)
		{
			QualityFormula = null
		};

		var craftComponent = new Mock<IActiveCraftGameItemComponent>();
		craftComponent.SetupProperty(x => x.HasFailed, false);
		craftComponent.SetupProperty(x => x.Phase, 1);
		craftComponent.SetupProperty(x => x.QualityCheckOutcome, Outcome.MajorPass);
		craftComponent.SetupGet(x => x.ConsumedInputs).Returns(new Dictionary<ICraftInput, (IPerceivable Input, ICraftInputData Data)>());
		craftComponent.SetupGet(x => x.ProducedProducts).Returns(new Dictionary<ICraftProduct, ICraftProductData>());
		craftComponent.SetupGet(x => x.UsedToolQualities).Returns(new Dictionary<ICraftTool, (ItemQuality Quality, double Weight)>());
		craftComponent.SetupGet(x => x.Craft).Returns(craft);

		var effect = new Mock<IActiveCraftEffect>();
		effect.SetupProperty(x => x.NextPhaseDuration);

		Assert.IsTrue(craft.HandleCraftPhase(character.Object, effect.Object, craftComponent.Object, 1));
		Assert.AreEqual(1.0 - 4.0 / 60.0, toolItem.Object.Condition, 0.000001);

		craftComponent.Object.Phase = 2;
		Assert.IsTrue(craft.HandleCraftPhase(character.Object, effect.Object, craftComponent.Object, 2));
		Assert.AreEqual(1.0 - 8.0 / 60.0, toolItem.Object.Condition, 0.000001);

		toolItemComponent.Verify(
			x => x.UseTool(toolTag.Object, It.Is<TimeSpan>(ts => Math.Abs(ts.TotalSeconds - 4.0) < 0.001)),
			Times.Exactly(2));
	}
}
