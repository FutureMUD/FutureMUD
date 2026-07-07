#nullable enable
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
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
}
