#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Commands.Modules;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests.Commands;

[TestClass]
public class RoomBuilderModuleExitLookupTests
{
	[DataTestMethod]
	[DataRow("north Normal", CardinalDirection.North, "Normal")]
	[DataRow("n Normal", CardinalDirection.North, "Normal")]
	[DataRow("nw Normal", CardinalDirection.NorthWest, "Normal")]
	[DataRow("northwest Normal", CardinalDirection.NorthWest, "Normal")]
	[DataRow("north-west Normal", CardinalDirection.NorthWest, "Normal")]
	[DataRow("north west Normal", CardinalDirection.NorthWest, "Normal")]
	public void GetCellExitForBuilderInput_CardinalAliasTargetsExactDirection(string command,
		CardinalDirection expectedDirection, string expectedRemainder)
	{
		var north = CreateExit(1, CardinalDirection.North, "North");
		var northwest = CreateExit(2, CardinalDirection.NorthWest, "North-West");
		var input = new StringStack(command);

		var result = RoomBuilderModule.GetCellExitForBuilderInput(
			[northwest.Object, north.Object],
			input,
			CreatePerceiver().Object);

		Assert.IsNotNull(result);
		Assert.AreEqual(expectedDirection, result.OutboundDirection);
		Assert.AreEqual(expectedRemainder, input.RemainingArgument);
	}

	[TestMethod]
	public void GetCellExitForBuilderInput_IdTargetsExitById()
	{
		var north = CreateExit(1, CardinalDirection.North, "North");
		var northwest = CreateExit(2, CardinalDirection.NorthWest, "North-West");
		var input = new StringStack("2 clear");

		var result = RoomBuilderModule.GetCellExitForBuilderInput(
			[north.Object, northwest.Object],
			input,
			CreatePerceiver().Object);

		Assert.AreSame(northwest.Object, result);
		Assert.AreEqual("clear", input.RemainingArgument);
	}

	[TestMethod]
	public void GetCellExitForBuilderInput_NonCardinalKeywordRequiresExactKeyword()
	{
		var gate = CreateExit(3, CardinalDirection.Unknown, "gate");
		var input = new StringStack("gate Normal");

		var result = RoomBuilderModule.GetCellExitForBuilderInput(
			[gate.Object],
			input,
			CreatePerceiver().Object);

		Assert.AreSame(gate.Object, result);
		Assert.AreEqual("Normal", input.RemainingArgument);
	}

	[TestMethod]
	public void GetCellExitForBuilderInput_NonCardinalKeywordDoesNotUsePrefixMatching()
	{
		var gate = CreateExit(3, CardinalDirection.Unknown, "gate");
		var input = new StringStack("ga Normal");

		var result = RoomBuilderModule.GetCellExitForBuilderInput(
			[gate.Object],
			input,
			CreatePerceiver().Object);

		Assert.IsNull(result);
		Assert.AreEqual("Normal", input.RemainingArgument);
	}

	private static Mock<ICellExit> CreateExit(long id, CardinalDirection direction, params string[] keywords)
	{
		var parent = new Mock<IExit>();
		parent.SetupGet(x => x.Id).Returns(id);

		var exit = new Mock<ICellExit>();
		exit.SetupGet(x => x.Exit).Returns(parent.Object);
		exit.SetupGet(x => x.OutboundDirection).Returns(direction);
		exit.SetupGet(x => x.Keywords).Returns(keywords);
		exit.Setup(x => x.GetKeywordsFor(It.IsAny<IPerceiver>())).Returns(keywords);
		exit.Setup(x => x.HasKeyword(It.IsAny<string>(), It.IsAny<IPerceiver>(), It.IsAny<bool>(),
				It.IsAny<bool>()))
		    .Returns<string, IPerceiver, bool, bool>((target, _, abbreviated, useContainsOverStartsWith) =>
			    HasKeyword(keywords, target, abbreviated, useContainsOverStartsWith));
		return exit;
	}

	private static Mock<IPerceiver> CreatePerceiver()
	{
		var perceiver = new Mock<IPerceiver>();
		perceiver.Setup(x => x.HasDubFor(It.IsAny<IKeyworded>(), It.IsAny<string>())).Returns(false);
		return perceiver;
	}

	private static bool HasKeyword(IEnumerable<string> keywords, string target, bool abbreviated,
		bool useContainsOverStartsWith)
	{
		if (!abbreviated)
		{
			return keywords.Any(x => x.Equals(target, StringComparison.InvariantCultureIgnoreCase));
		}

		return useContainsOverStartsWith
			? keywords.Any(x => x.Contains(target, StringComparison.InvariantCultureIgnoreCase))
			: keywords.Any(x => x.StartsWith(target, StringComparison.InvariantCultureIgnoreCase));
	}
}
