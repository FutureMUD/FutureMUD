#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.PerceptionEngine;
using MudSharp.Planes;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PlaneDisplayFormatSecurityTests
{
	[DataTestMethod]
	[DataRow("roomname")]
	[DataRow("tag")]
	public void PlaneBuilder_RejectsExpansionAndPreservesExistingFormat(string command)
	{
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
		var plane = new Plane(new MudSharp.Models.Plane
		{
			Id = 1, Name = "Astral", Alias = "astral", RoomNameFormat = "Astral {0}", RemoteObservationTag = "({0})"
		}, world.Object);
		var actor = Mock.Of<ICharacter>(x => x.OutputHandler == Mock.Of<IOutputHandler>());
		Assert.IsFalse(plane.BuildingCommand(actor, new StringStack(command + " {0}{0,9999999}")));
		Assert.AreEqual("Astral {0}", plane.RoomNameFormat);
		Assert.AreEqual("({0})", plane.RemoteObservationTag);
		Assert.IsFalse(plane.Changed);
		Assert.IsTrue(plane.BuildingCommand(actor, new StringStack(command + " Astral {0}")));
	}

	[TestMethod]
	public void RoomName_RenderingUnsafeStoredFormatFallsBackBeforeExpansion()
	{
		var world = new Mock<IFuturemud>();
		var plane = Mock.Of<IPlane>(x => x.Id == 1 && x.RoomNameFormat == "{0}{0,9999999}");
		world.SetupGet(x => x.DefaultPlane).Returns(plane);
		var viewer = Mock.Of<IPerceiver>(x => x.Gameworld == world.Object);
		var room = (Room)RuntimeHelpers.GetUninitializedObject(typeof(Room));
		var result = typeof(Room).GetMethod("RoomNameForPlane", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(room, ["Test Room", viewer, PerceiveIgnoreFlags.None]);
		Assert.AreEqual("Test Room", result);
	}
}
