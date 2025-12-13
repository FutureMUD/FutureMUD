#nullable enable

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;

namespace MudSharp_Unit_Tests;

[TestClass]
public class HandToolDurabilityTests
{
	[TestMethod]
	public void CanUseTool_FullConditionAndEnoughDurability_ReturnsTrue()
	{
		var gameworld = new Mock<IFuturemud>();

		var toolTag = new Mock<ITag>();
		toolTag.Setup(x => x.IsA(It.IsAny<ITag>())).Returns(true);

		var parent = new Mock<IGameItem>();
		parent.SetupProperty(x => x.Condition, 1.0);
		parent.SetupProperty(x => x.Quality, ItemQuality.Standard);
		parent.SetupGet(x => x.Tags).Returns([toolTag.Object]);
		parent.SetupGet(x => x.Gameworld).Returns(gameworld.Object);

		var protoModel = new MudSharp.Models.GameItemComponentProto
		{
			Id = 1,
			Name = "Test Hand Tool",
			Description = string.Empty,
			Type = "HandTool",
			RevisionNumber = 0,
			EditableItem = new MudSharp.Models.EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 0,
				BuilderAccountId = 1,
				BuilderDate = DateTime.UtcNow,
				BuilderComment = string.Empty,
				ReviewerComment = string.Empty
			},
			Definition =
				"<Definition><BaseMultiplier>1.0</BaseMultiplier><MultiplierReductionPerQuality>0.0</MultiplierReductionPerQuality><ToolDurabilitySecondsExpression>60</ToolDurabilitySecondsExpression></Definition>"
		};

		var proto = new TestHandToolGameItemComponentProto(protoModel, gameworld.Object);
		var component = new HandToolGameItemComponent(proto, parent.Object, temporary: true);

		Assert.IsTrue(component.CanUseTool(toolTag.Object, TimeSpan.FromSeconds(5)));
	}

	private class TestHandToolGameItemComponentProto : HandToolGameItemComponentProto
	{
		public TestHandToolGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
			: base(proto, gameworld)
		{
		}
	}
}
