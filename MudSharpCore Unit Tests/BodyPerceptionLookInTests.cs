#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Implementations;
using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

namespace MudSharp_Unit_Tests;

[TestClass]
public class BodyPerceptionLookInTests
{
	[DataTestMethod]
	[DataRow(true, true, false)]
	[DataRow(true, false, false)]
	[DataRow(false, true, false)]
	[DataRow(false, false, true)]
	public void LookInText_OpenAndTransparencyCombinations_OnlyClosedOpaqueRefuses(
		bool isOpen, bool transparent, bool expectedRefusal)
	{
		var actor = new Mock<ICharacter>();
		var body = TestObjectFactory.CreateUninitialized<Body>();
		body.Actor = actor.Object;

		var openable = new Mock<IOpenable>();
		openable.SetupGet(x => x.IsOpen).Returns(isOpen);
		var container = new Mock<IContainer>();
		container.SetupGet(x => x.Transparent).Returns(transparent);
		var item = new Mock<IGameItem>();
		item.Setup(x => x.IsItemType<IContainer>()).Returns(true);
		item.Setup(x => x.GetItemType<IOpenable>()).Returns(openable.Object);
		item.Setup(x => x.GetItemType<IContainer>()).Returns(container.Object);
		item.Setup(x => x.HowSeen(actor.Object, true, DescriptionType.Short, true,
			PerceiveIgnoreFlags.None)).Returns("A test container");
		item.Setup(x => x.HowSeen(actor.Object, true, DescriptionType.Contents, true,
			PerceiveIgnoreFlags.None)).Returns("The test contents are visible.");

		var result = body.LookInText(item.Object);

		if (expectedRefusal)
		{
			StringAssert.Contains(result, "must be opened before you can look in it");
			StringAssert.DoesNotMatch(result, new System.Text.RegularExpressions.Regex("contents are visible"));
			return;
		}

		StringAssert.Contains(result, "The test contents are visible.");
		StringAssert.DoesNotMatch(result,
			new System.Text.RegularExpressions.Regex("must be opened before you can look in it"));
	}

	[TestMethod]
	public void LookInText_ClosedSheathWithoutContainerTransparency_StillRefuses()
	{
		var actor = new Mock<ICharacter>();
		var body = TestObjectFactory.CreateUninitialized<Body>();
		body.Actor = actor.Object;

		var openable = new Mock<IOpenable>();
		openable.SetupGet(x => x.IsOpen).Returns(false);
		var item = new Mock<IGameItem>();
		item.Setup(x => x.IsItemType<ISheath>()).Returns(true);
		item.Setup(x => x.GetItemType<IOpenable>()).Returns(openable.Object);
		item.Setup(x => x.HowSeen(actor.Object, true, DescriptionType.Short, true,
			PerceiveIgnoreFlags.None)).Returns("A test sheath");

		var result = body.LookInText(item.Object);

		StringAssert.Contains(result, "must be opened before you can look in it");
	}
}
