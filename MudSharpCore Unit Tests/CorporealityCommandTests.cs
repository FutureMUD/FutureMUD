#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Construction;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.Planes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CorporealityCommandTests
{
	[TestMethod]
	public void TryParseCorporealityArguments_CommaSeparatedPlanesAndDuration_ReturnsPlanesAndDuration()
	{
		var prime = BuildPlane(1, "Prime Material");
		var astral = BuildPlane(2, "Astral Plane");
		var gameworld = BuildGameworld(prime.Object, astral.Object);
		var actor = new Mock<ICharacter>();
		actor.Setup(x => x.Gameworld).Returns(gameworld.Object);
		var ss = new StringStack("Prime,Astral 00:10:00 visible");

		var result = StaffModule.TryParseCorporealityArguments(actor.Object, ss, true,
			out var planes, out var duration, out var visible, out var error);

		Assert.IsTrue(result, error);
		CollectionAssert.AreEquivalent(new long[] { 1, 2 }, planes.Select(x => x.Id).ToArray());
		Assert.AreEqual(TimeSpan.FromMinutes(10), duration);
		Assert.IsTrue(visible);
	}

	[TestMethod]
	public void EchoVisibilityChanges_BecomesVisible_SendsFadeIntoView()
	{
		var output = new Mock<IOutputHandler>();
		var observer = new Mock<ICharacter>();
		var target = new Mock<IPerceivable>();
		var location = new Mock<ICell>();
		location.Setup(x => x.Characters).Returns(new[] { observer.Object });
		target.Setup(x => x.Location).Returns(location.Object);
		target.Setup(x => x.HowSeen(observer.Object, true, DescriptionType.Short, true, PerceiveIgnoreFlags.None))
		      .Returns("an astral figure");
		observer.Setup(x => x.IsSelf(target.Object)).Returns(false);
		observer.Setup(x => x.CanSee(target.Object, PerceiveIgnoreFlags.None)).Returns(true);
		observer.SetupGet(x => x.OutputHandler).Returns(output.Object);

		PlanarVisibilityEchoHelper.EchoVisibilityChanges(target.Object, new Dictionary<IPerceiver, string>());

		output.Verify(x => x.Send("an astral figure fades into view.", true, false), Times.Once);
	}

	[TestMethod]
	public void EchoVisibilityChanges_BecomesHidden_SendsFadeFromView()
	{
		var output = new Mock<IOutputHandler>();
		var observer = new Mock<ICharacter>();
		var target = new Mock<IPerceivable>();
		observer.Setup(x => x.CanSee(target.Object, PerceiveIgnoreFlags.None)).Returns(false);
		observer.SetupGet(x => x.OutputHandler).Returns(output.Object);
		var before = new Dictionary<IPerceiver, string>
		{
			{ observer.Object, "an astral figure" }
		};

		PlanarVisibilityEchoHelper.EchoVisibilityChanges(target.Object, before);

		output.Verify(x => x.Send("an astral figure fades from view.", true, false), Times.Once);
	}

	private static Mock<IPlane> BuildPlane(long id, string name)
	{
		var plane = new Mock<IPlane>();
		plane.Setup(x => x.Id).Returns(id);
		plane.Setup(x => x.Name).Returns(name);
		return plane;
	}

	private static Mock<IFuturemud> BuildGameworld(params IPlane[] planeList)
	{
		var gameworld = new Mock<IFuturemud>();
		var planes = new Mock<IUneditableAll<IPlane>>();
		planes.Setup(x => x.GetEnumerator()).Returns(() => ((IEnumerable<IPlane>)planeList).GetEnumerator());
		planes.As<IEnumerable>().Setup(x => x.GetEnumerator()).Returns(() => planeList.GetEnumerator());
		planes.Setup(x => x.GetByIdOrName(It.IsAny<string>(), It.IsAny<bool>()))
		      .Returns<string, bool>((text, _) => planeList.FirstOrDefault(x =>
			      x.Id.ToString("N0").Equals(text, StringComparison.OrdinalIgnoreCase) ||
			      x.Name.Equals(text, StringComparison.OrdinalIgnoreCase) ||
			      x.Name.Split(' ', StringSplitOptions.RemoveEmptyEntries)
			       .Any(y => y.Equals(text, StringComparison.OrdinalIgnoreCase))));
		gameworld.Setup(x => x.DefaultPlane).Returns(planeList[0]);
		gameworld.Setup(x => x.Planes).Returns(planes.Object);
		return gameworld;
	}
}
