#nullable enable
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Work.Projects;
using MudSharp.Work.Projects.ConcreteTypes;
using System;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ProjectLabourQueueEntryTests
{
	[TestMethod]
	public void StatusForEvaluatesCandidatePhysicalInstance()
	{
		var owner = new Mock<ICharacter>();
		var secondary = new Mock<ICharacter>();
		var labour = new Mock<IProjectLabourRequirement>();
		var phase = new Mock<IProjectPhase>();
		var project = new Mock<IActiveProject>();

		labour.SetupGet(x => x.Id).Returns(20L);
		labour.SetupGet(x => x.Name).Returns("Masonry");
		labour.SetupGet(x => x.MaximumSimultaneousWorkers).Returns(1);
		labour.Setup(x => x.CharacterIsQualified(owner.Object)).Returns(false);
		labour.Setup(x => x.CharacterIsQualified(secondary.Object)).Returns(true);

		phase.SetupGet(x => x.LabourRequirements)
		     .Returns(new[] { labour.Object });

		project.SetupGet(x => x.Id).Returns(10L);
		project.SetupGet(x => x.Name).Returns("Build a Wall");
		project.SetupGet(x => x.CurrentPhase).Returns(phase.Object);
		project.SetupGet(x => x.ActiveLabour)
		       .Returns(Array.Empty<(ICharacter Character, IProjectLabourRequirement Labour)>());
		project.Setup(x => x.CanJoinLabour(secondary.Object, labour.Object)).Returns(true);

		var entry = new ProjectLabourQueueEntry(owner.Object, project.Object, labour.Object, 1);

		Assert.AreEqual(ProjectLabourQueueStatus.WaitingForQualification, entry.Status);
		Assert.AreEqual(ProjectLabourQueueStatus.Ready, entry.StatusFor(secondary.Object));
	}

	[TestMethod]
	public void DurationEntryTracksOnlyItsOwnCycleState()
	{
		var owner = new Mock<ICharacter>();
		var labour = new Mock<IProjectLabourRequirement>();
		var phase = new Mock<IProjectPhase>();
		var project = new Mock<IActiveProject>();
		var definition = new Mock<IProject>();

		owner.SetupGet(x => x.InstanceId).Returns(44L);
		labour.SetupGet(x => x.Id).Returns(20L);
		labour.SetupGet(x => x.Name).Returns("Masonry");
		phase.SetupGet(x => x.LabourRequirements).Returns(new[] { labour.Object });
		project.SetupGet(x => x.Id).Returns(10L);
		project.SetupGet(x => x.CurrentPhase).Returns(phase.Object);
		definition.SetupGet(x => x.Id).Returns(5L);
		project.SetupGet(x => x.ProjectDefinition).Returns(definition.Object);

		var entry = new ProjectLabourQueueEntry(owner.Object, project.Object, labour.Object, 1,
			completionMode: ProjectLabourQueueCompletionMode.Duration, targetHours: 2.0);

		entry.Claim(owner.Object, labour.Object);
		entry.AddFundedHours(0.75);
		Assert.AreEqual(0.75, entry.ElapsedHours, 0.0001);
		Assert.IsFalse(entry.DurationReached);

		entry.AddFundedHours(1.25);
		Assert.IsTrue(entry.DurationReached);
		entry.ResetForNextCycle();
		Assert.AreEqual(0.0, entry.ElapsedHours, 0.0001);
		Assert.IsNull(entry.ClaimingCharacterInstanceId);
	}

	[TestMethod]
	public void StartEntryRetainsDefinitionIdWithoutCreatingALiveProject()
	{
		var owner = new Mock<ICharacter>();
		var definition = new Mock<IProject>();
		definition.SetupGet(x => x.Id).Returns(99L);

		var entry = new ProjectLabourQueueEntry(owner.Object, definition.Object, "Masonry",
			ProjectLabourQueueCompletionMode.ProjectCompletion, 0.0, 1);

		Assert.AreEqual(ProjectLabourQueueEntryType.StartProject, entry.EntryType);
		Assert.AreEqual(99L, entry.ProjectDefinitionId);
		Assert.IsNull(entry.Project);
		Assert.AreEqual("Masonry", entry.LabourPreference);
	}

	[TestMethod]
	public void ResetForNextCycle_StartEntryRetainsItsLiveProjectLink()
	{
		var owner = new Mock<ICharacter>();
		var gameworld = new Mock<IFuturemud>();
		var activeProjects = new All<IActiveProject>();
		var project = new Mock<IActiveProject>();

		project.SetupGet(x => x.Id).Returns(10L);
		project.SetupGet(x => x.Name).Returns("Build a Wall");
		activeProjects.Add(project.Object);
		gameworld.SetupGet(x => x.ActiveProjects).Returns(activeProjects);
		owner.SetupGet(x => x.Gameworld).Returns(gameworld.Object);

		var entry = new ProjectLabourQueueEntry(new MudSharp.Models.ProjectLabourQueue
		{
			EntryType = (int)ProjectLabourQueueEntryType.StartProject,
			ProjectId = 99L,
			ActiveProjectId = project.Object.Id,
			CompletionMode = (int)ProjectLabourQueueCompletionMode.Duration
		}, gameworld.Object, owner.Object);

		entry.ResetForNextCycle();

		Assert.AreSame(project.Object, entry.Project);
		Assert.AreEqual(project.Object.Id, entry.ProjectId);
	}

	[TestMethod]
	public void SetCompletionMode_ClaimedEntryPreservesPhysicalInstanceClaim()
	{
		var owner = new Mock<ICharacter>();
		var labour = new Mock<IProjectLabourRequirement>();
		var project = new Mock<IActiveProject>();

		owner.SetupGet(x => x.InstanceId).Returns(44L);
		labour.SetupGet(x => x.Id).Returns(20L);
		labour.SetupGet(x => x.Name).Returns("Masonry");
		project.SetupGet(x => x.Id).Returns(10L);

		var entry = new ProjectLabourQueueEntry(owner.Object, project.Object, labour.Object, 1,
			completionMode: ProjectLabourQueueCompletionMode.Duration, targetHours: 2.0);
		entry.Claim(owner.Object, labour.Object);

		entry.SetCompletionMode(ProjectLabourQueueCompletionMode.ProjectCompletion, 0.0);

		Assert.AreEqual(owner.Object.InstanceId, entry.ClaimingCharacterInstanceId);
	}

	[TestMethod]
	public void StatusFor_FundingPreflightUsesTheConfiguredTickDuration()
	{
		var owner = new Mock<ICharacter>();
		var gameworld = new Mock<IFuturemud>();
		var labour = new Mock<IProjectLabourRequirement>();
		var phase = new Mock<IProjectPhase>();
		var project = new Mock<IActiveProject>();

		owner.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		gameworld.Setup(x => x.GetStaticDouble("ProjectProgressMultiplier")).Returns(0.25);
		labour.SetupGet(x => x.Id).Returns(20L);
		labour.SetupGet(x => x.Name).Returns("Masonry");
		labour.Setup(x => x.CharacterIsQualified(owner.Object)).Returns(true);
		phase.SetupGet(x => x.LabourRequirements).Returns([labour.Object]);
		project.SetupGet(x => x.Id).Returns(10L);
		project.SetupGet(x => x.CurrentPhase).Returns(phase.Object);
		project.Setup(x => x.CanJoinLabour(owner.Object, labour.Object)).Returns(true);
		project.Setup(x => x.LabourPaymentRateFor(labour.Object)).Returns(10.0M);
		project.Setup(x => x.CanPayLabourContribution(labour.Object, 0.25, out It.Ref<string>.IsAny)).Returns(true);

		var entry = new ProjectLabourQueueEntry(owner.Object, project.Object, labour.Object, 1);

		Assert.AreEqual(ProjectLabourQueueStatus.Ready, entry.StatusFor(owner.Object));
		project.Verify(x => x.CanPayLabourContribution(labour.Object, 0.25, out It.Ref<string>.IsAny), Times.Once);
	}

	[TestMethod]
	public void TryActivate_PostInitiationJoinFailureMarksTheLinkedStartEntryForPersistence()
	{
		var owner = new Mock<ICharacter>();
		var gameworld = new Mock<IFuturemud>();
		var projects = new Mock<IUneditableRevisableAll<IProject>>();
		var definition = new Mock<IProject>();
		var launchedProject = new Mock<IActiveProject>();
		var labour = new Mock<IProjectLabourRequirement>();
		var phase = new Mock<IProjectPhase>();

		owner.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		owner.SetupProperty(x => x.Changed);
		gameworld.SetupGet(x => x.Projects).Returns(projects.Object);
		projects.Setup(x => x.GetAll(99L)).Returns([definition.Object]);
		definition.SetupGet(x => x.Id).Returns(99L);
		definition.SetupGet(x => x.Status).Returns(RevisionStatus.Current);
		definition.SetupGet(x => x.Phases).Returns([phase.Object]);
		definition.Setup(x => x.AppearInProjectList(owner.Object)).Returns(true);
		definition.Setup(x => x.CanInitiateProject(owner.Object)).Returns(true);
		definition.Setup(x => x.InitiateProject(owner.Object)).Returns(launchedProject.Object);
		labour.SetupGet(x => x.Id).Returns(20L);
		labour.SetupGet(x => x.Name).Returns("Masonry");
		labour.Setup(x => x.CharacterIsQualified(owner.Object)).Returns(true);
		phase.SetupGet(x => x.LabourRequirements).Returns([labour.Object]);
		launchedProject.SetupGet(x => x.Id).Returns(10L);
		launchedProject.SetupGet(x => x.CurrentPhase).Returns(phase.Object);
		launchedProject.Setup(x => x.CanJoinLabour(owner.Object, labour.Object)).Returns(true);

		var entry = new ProjectLabourQueueEntry(owner.Object, definition.Object, "Masonry",
			ProjectLabourQueueCompletionMode.ProjectCompletion, 0.0, 1);

		Assert.IsFalse(entry.TryActivate(owner.Object, out _, out _, out _));
		Assert.IsTrue(owner.Object.Changed);
		Assert.AreSame(launchedProject.Object, entry.Project);
	}
}
