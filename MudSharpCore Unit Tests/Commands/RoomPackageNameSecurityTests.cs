#nullable enable

using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests.Commands;

[TestClass]
public class RoomPackageNameSecurityTests
{
	[DataTestMethod]
	[DataRow("RoomPackageNew")]
	[DataRow("RoomPackageRename")]
	public void PackageCommand_OverlongName_RejectsBeforeLookupOrMutation(string method)
	{
		var actor = new Mock<ICharacter>();
		var package = new Mock<IRoomOverlayPackage>();
		var output = new Mock<IOutputHandler>();
		actor.SetupGet(x => x.CurrentOverlayPackage).Returns(package.Object);
		actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
		Invoke(method, actor.Object, new string('a', RoomOverlayPackage.MaximumNameLength + 1));
		actor.VerifyGet(x => x.Gameworld, Times.Never);
		package.Verify(x => x.SetName(It.IsAny<string>()), Times.Never);
		output.Verify(x => x.Send(It.Is<string>(s => s.Contains("cannot exceed")), true, false), Times.Once);
	}

	[TestMethod]
	public void PackageRename_NameAtDatabaseLimit_RenamesEveryRevisionWithoutTruncation()
	{
		var packages = new RevisableAll<IRoomOverlayPackage>();
		var current = new Mock<IRoomOverlayPackage>();
		var previous = new Mock<IRoomOverlayPackage>();
		current.SetupGet(x => x.Id).Returns(1);
		current.SetupGet(x => x.RevisionNumber).Returns(1);
		current.SetupGet(x => x.Name).Returns("Old");
		previous.SetupGet(x => x.Id).Returns(1);
		previous.SetupGet(x => x.RevisionNumber).Returns(0);
		previous.SetupGet(x => x.Name).Returns("Old");
		packages.Add(current.Object);
		packages.Add(previous.Object);
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.RoomOverlayPackages).Returns(packages);
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Gameworld).Returns(world.Object);
		actor.SetupGet(x => x.CurrentOverlayPackage).Returns(current.Object);
		actor.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
		var name = new string('a', RoomOverlayPackage.MaximumNameLength).TitleCase();
		Invoke("RoomPackageRename", actor.Object, name);
		current.Verify(x => x.SetName(name), Times.Once);
		previous.Verify(x => x.SetName(name), Times.Once);
	}

	private static void Invoke(string method, ICharacter actor, string name) =>
		typeof(RoomBuilderModule).GetMethod(method, BindingFlags.NonPublic | BindingFlags.Static)!
			.Invoke(null, [actor, new StringStack(name)]);
}
