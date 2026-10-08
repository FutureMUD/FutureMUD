#nullable enable
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.Work.Projects;
using MudSharp.Work.Projects.ConcreteTypes;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ProjectQueuePreferenceTests
{
	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void Constructor_Exactly100Characters_PreservesPreference(bool startEntry)
	{
		var preference = new string('x', 100);
		Assert.AreEqual(preference, CreateEntry(startEntry, preference).LabourPreference);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void Constructor_101Characters_RejectsPreference(bool startEntry)
	{
		Assert.ThrowsException<ArgumentException>(() => CreateEntry(startEntry, new string('x', 101)));
	}

	[DataTestMethod]
	[DataRow(0)]
	[DataRow(100)]
	public void ModelConstructor_NullOr100Characters_PreservesPreference(int length)
	{
		var preference = length == 0 ? null : new string('x', length);
		var entry = new ProjectLabourQueueEntry(new MudSharp.Models.ProjectLabourQueue
		{
			LabourPreference = preference
		}, new Mock<IFuturemud>().Object, new Mock<ICharacter>().Object);

		Assert.AreEqual(preference, entry.LabourPreference);
	}

	[TestMethod]
	public void ModelConstructor_101Characters_RejectsPreference()
	{
		Assert.ThrowsException<ArgumentException>(() => new ProjectLabourQueueEntry(
			new MudSharp.Models.ProjectLabourQueue { LabourPreference = new string('x', 101) },
			new Mock<IFuturemud>().Object, new Mock<ICharacter>().Object));
	}

	[TestMethod]
	public void Constructor_ImplicitLabourName_RejectsOversizedPreference()
	{
		var labour = new Mock<IProjectLabourRequirement>();
		labour.SetupGet(x => x.Name).Returns(new string('x', 101));
		Assert.ThrowsException<ArgumentException>(() => new ProjectLabourQueueEntry(
			new Mock<ICharacter>().Object, new Mock<IActiveProject>().Object, labour.Object, 1));
	}

	[TestMethod]
	public void SetLabourPreference_101Characters_PreservesPreferenceAndLabourLink()
	{
		var labour = new Mock<IProjectLabourRequirement>();
		labour.SetupGet(x => x.Id).Returns(20L);
		labour.SetupGet(x => x.Name).Returns("Masonry");
		var entry = new ProjectLabourQueueEntry(new Mock<ICharacter>().Object,
			new Mock<IActiveProject>().Object, labour.Object, 1);

		Assert.ThrowsException<ArgumentException>(() => entry.SetLabourPreference(new string('x', 101)));
		Assert.AreEqual("Masonry", entry.LabourPreference);
		Assert.AreEqual(20L, entry.LabourId);
	}

	[TestMethod]
	public void SetLabourPreference_100CharactersAndAutomatic_ArePreserved()
	{
		var entry = CreateEntry(false, "Masonry");
		var preference = new string('x', 100);
		entry.SetLabourPreference(preference);
		Assert.AreEqual(preference, entry.LabourPreference);
		entry.SetLabourPreference(null);
		Assert.IsNull(entry.LabourPreference);
	}

	[DataTestMethod]
	[DataRow(100, "", true)]
	[DataRow(101, "", false)]
	[DataRow(100, " for 2", true)]
	[DataRow(101, " for 2", false)]
	public void QueueOptions_AddAndStartParser_EnforcesLengthBeforeAcceptingModes(int length, string mode, bool expected)
	{
		var output = new Mock<IOutputHandler>();
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
		object?[] args = [actor.Object, new StringStack(new string('x', length) + mode), false, null,
			ProjectLabourQueueCompletionMode.JoinOnce, 0.0];
		var accepted = (bool)CraftMethod("TryParseQueueOptions").Invoke(null, args)!;

		Assert.AreEqual(expected, accepted);
		if (expected)
		{
			Assert.AreEqual(new string('x', length), args[3]);
		}
		else
		{
			output.Verify(x => x.Send(It.Is<string>(s => s.Contains("100")), true, false), Times.Once);
		}
	}

	[DataTestMethod]
	[DataRow(100, true)]
	[DataRow(101, false)]
	public void ProjectQueueLabour_EditCommand_ValidatesBeforeMutation(int length, bool expected)
	{
		var preference = new string('x', length);
		var output = new Mock<IOutputHandler>();
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
		actor.Setup(x => x.SetProjectLabourQueueLabour(1, preference)).Returns(true);

		CraftMethod("ProjectQueueLabour").Invoke(null, [actor.Object, new StringStack("1 " + preference)]);

		actor.Verify(x => x.SetProjectLabourQueueLabour(1, preference), expected ? Times.Once() : Times.Never());
		if (!expected)
		{
			output.Verify(x => x.Send(It.Is<string>(s => s.Contains("100")), true, false), Times.Once);
		}
	}

	[DataTestMethod]
	[DataRow(100, true)]
	[DataRow(101, false)]
	public void ProjectQueueAdd_Command_ValidatesBeforeMutation(int length, bool expected)
	{
		var preference = new string('x', length);
		var output = new Mock<IOutputHandler>();
		var actor = new Mock<ICharacter>();
		var project = new Mock<IPersonalProject>();
		project.SetupGet(x => x.Id).Returns(10L);
		project.SetupGet(x => x.Name).Returns("Wall");
		var room = new Mock<IRoom>();
		room.SetupGet(x => x.LocalProjects).Returns(Array.Empty<ILocalProject>());
		actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
		actor.SetupGet(x => x.Location).Returns(room.Object);
		actor.SetupGet(x => x.PersonalProjects).Returns([project.Object]);
		actor.SetupGet(x => x.CurrentProject).Returns((project.Object, null!));
		actor.Setup(x => x.QueueProjectLabour(project.Object, preference, ProjectLabourQueueCompletionMode.JoinOnce, 0.0))
			.Returns(new Mock<IProjectLabourQueueEntry>().Object);
		var method = typeof(MudSharp.Character.Character).Assembly.GetType("MudSharp.Commands.Modules.CraftModule")!
			.GetMethod("ProjectQueueAdd", BindingFlags.Static | BindingFlags.NonPublic, null,
				[typeof(ICharacter), typeof(StringStack), typeof(bool)], null)!;

		method.Invoke(null, [actor.Object, new StringStack("Wall " + preference), true]);

		actor.Verify(x => x.QueueProjectLabour(project.Object, preference, ProjectLabourQueueCompletionMode.JoinOnce, 0.0),
			expected ? Times.Once() : Times.Never());
		if (!expected)
		{
			output.Verify(x => x.Send(It.Is<string>(s => s.Contains("100")), true, false), Times.Once);
		}
	}

	[TestMethod]
	public void SetProjectLabourQueueLabour_101Characters_LeavesQueueAndDirtyStateUnchanged()
	{
		var character = TestObjectFactory.CreateUninitialized<MudSharp.Character.Character>();
		var entry = CreateEntry(false, "Masonry");
		typeof(MudSharp.Character.Character).GetField("_projectLabourQueue", BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(character, new List<ProjectLabourQueueEntry> { entry });

		Assert.IsFalse(character.SetProjectLabourQueueLabour(1, new string('x', 101)));
		Assert.AreEqual("Masonry", entry.LabourPreference);
		Assert.IsFalse(character.Changed);
	}

	private static ProjectLabourQueueEntry CreateEntry(bool startEntry, string? preference)
	{
		var character = new Mock<ICharacter>().Object;
		return startEntry
			? new ProjectLabourQueueEntry(character, new Mock<IProject>().Object, preference,
				ProjectLabourQueueCompletionMode.JoinOnce, 0.0, 1)
			: new ProjectLabourQueueEntry(character, new Mock<IActiveProject>().Object, null, 1, preference);
	}

	private static MethodInfo CraftMethod(string name)
	{
		return typeof(MudSharp.Character.Character).Assembly.GetType("MudSharp.Commands.Modules.CraftModule")!
			.GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic)!;
	}
}
