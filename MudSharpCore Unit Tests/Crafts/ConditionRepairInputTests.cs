#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.RPG.Checks;
using MudSharp.Work.Crafts;
using MudSharp.Work.Crafts.Inputs;
using System.Xml.Linq;

namespace MudSharp_Unit_Tests.Crafts;

[TestClass]
public class ConditionRepairInputTests
{
	[TestMethod]
	[DataRow(Outcome.MajorFail, 0.60, 0.60, true)]
	[DataRow(Outcome.Fail, 0.60, 0.85, false)]
	[DataRow(Outcome.Pass, 0.90, 1.00, false)]
	public void LegacyProgress_ResumesAndReturnsOriginalHarness(Outcome outcome, double initial, double expected, bool failed)
	{
		var job = new RepairJob(initial);
		job.Progress.CheckOutcome = outcome;
		var saved = job.Progress.SaveDefinition();
		saved.Element("HasFailed")!.Remove();
		saved.Descendants("RepairApplied").Remove();
		var resumed = new ProgressItem(job.Parent.Object);
		resumed.LoadDefinition(saved);

		Assert.AreEqual(failed, resumed.HasFailed);
		Assert.AreEqual(initial, job.Harness.Object.Condition);
		Assert.IsTrue(resumed.DoNextPhase(job.Effect.Object).Finished);
		Assert.AreEqual(expected, job.Harness.Object.Condition, 0.00001);
		job.Room.Verify(x => x.Insert(job.Harness.Object, It.IsAny<bool>()), Times.Once);
		job.Harness.Verify(x => x.Login(), Times.Once);
	}

	[TestMethod]
	public void ReleasingCancelledWork_ReturnsOriginalHarnessAtExistingCondition()
	{
		var job = new RepairJob(0.55);
		job.Progress.ReleaseItems(job.Room.Object, RoomLayer.GroundLevel);
		Assert.AreEqual(0.55, job.Harness.Object.Condition);
		job.Room.Verify(x => x.Insert(job.Harness.Object, It.IsAny<bool>()), Times.Once);
		job.Harness.Verify(x => x.Login(), Times.Once);
	}

	[TestMethod]
	public void ReloadedCompletionReceipt_DoesNotApplyRepairTwice()
	{
		var job = new RepairJob(0.35);
		var data = job.Progress.ConsumedInputs[job.Input].Data;
		job.Input.CompleteInput(job.Harness.Object, data);
		var restored = job.Input.LoadDataFromXml(data.SaveToXml(), job.World.Object);
		job.Input.CompleteInput(job.Harness.Object, restored);
		Assert.AreEqual(0.60, job.Harness.Object.Condition, 0.00001);
	}

	private sealed class RepairJob
	{
		public Mock<IFuturemud> World { get; } = new() { DefaultValue = DefaultValue.Mock };
		public Mock<IRoom> Room { get; } = new();
		public Mock<IGameItem> Harness { get; } = new();
		public Mock<IGameItem> Parent { get; } = new();
		public Mock<IActiveCraftEffect> Effect { get; } = new();
		public ConditionRepairInput Input { get; }
		public ProgressItem Progress { get; }

		public RepairJob(double condition)
		{
			var craft = new Mock<ICraft>();
			craft.SetupGet(x => x.Id).Returns(42);
			craft.SetupGet(x => x.RevisionNumber).Returns(6);
			craft.SetupGet(x => x.FailThreshold).Returns(Outcome.MajorFail);
			craft.SetupGet(x => x.LastPhase).Returns(2);
			craft.Setup(x => x.HandleCraftPhase(It.IsAny<ICharacter>(), Effect.Object,
				It.IsAny<IActiveCraftGameItemComponent>(), 2)).Returns(true);
			World.Setup(x => x.Crafts.Get(42, 6)).Returns(craft.Object);
			Parent.SetupGet(x => x.Gameworld).Returns(World.Object);
			Parent.SetupGet(x => x.Location).Returns(Room.Object);
			Effect.SetupGet(x => x.CharacterOwner).Returns(Mock.Of<ICharacter>(x => x.Location == Room.Object));
			Harness.SetupGet(x => x.Id).Returns(27);
			Harness.SetupProperty(x => x.Condition, condition);
			World.Setup(x => x.TryGetItem(27, false)).Returns(Harness.Object);

			Input = (ConditionRepairInput)CraftInputFactory.Factory.LoadInput(new MudSharp.Models.CraftInput
			{
				Id = 1, InputType = "ConditionRepair",
				Definition = "<Definition><TargetTagId>4</TargetTagId><RepairAmount>0.25</RepairAmount></Definition>"
			}, craft.Object, World.Object);
			craft.SetupGet(x => x.Inputs).Returns([Input]);
			Progress = new ProgressItem(Parent.Object) { Craft = craft.Object, Phase = 2 };
			Progress.ConsumedInputs[Input] = (Harness.Object, Input.ReserveInput(Harness.Object));
		}
	}

	private sealed class ProgressItem(IGameItem parent) : ActiveCraftGameItemComponent((ActiveCraftGameItemComponentProto)null!, parent, true)
	{
		public XElement SaveDefinition() => XElement.Parse(SaveToXml());
		public void LoadDefinition(XElement xml) => LoadFromXml(xml);
	}
}
