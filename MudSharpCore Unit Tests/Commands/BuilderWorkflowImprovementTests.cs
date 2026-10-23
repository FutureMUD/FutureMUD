#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Commands;
using MudSharp.Commands.Helpers;
using MudSharp.Commands.Modules;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.TimeAndDate.Time;
using MudSharp.Work.Agriculture;

namespace MudSharp_Unit_Tests.Commands;

[TestClass]
[DoNotParallelize]
public class BuilderWorkflowImprovementTests
{
	[TestMethod]
	public void Capabilities_UnsupportedOperationsRefuseBeforeReadingOrMutatingWorld()
	{
		var world = BuilderWorkflowRegressionTests.World();
		var actor = BuilderWorkflowRegressionTests.Actor(world.Object);
		Assert.IsFalse(EditableItemHelper.BankHelper.Capabilities.CanClone);
		Assert.IsFalse(EditableItemHelper.BankHelper.Capabilities.Delete);
		Assert.IsFalse(EditableItemHelper.PropertyHelper.Capabilities.CanClone);
		BaseBuilderModule.GenericBuildingCommand(actor.Object, new StringStack("clone 1 New"), EditableItemHelper.BankHelper);
		BaseBuilderModule.GenericBuildingCommand(actor.Object, new StringStack("delete 1"), EditableItemHelper.BankHelper);
		world.VerifyGet(x => x.Banks, Times.Never);
		actor.Verify(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()), Times.Never);
	}

	[TestMethod]
	public void Validation_SubmissionFailureIsReportedWithoutStatusOrWorldChanges()
	{
		var world = BuilderWorkflowRegressionTests.World();
		var revision = new Mock<IEditableRevisableItem>();
		revision.Setup(x => x.CanSubmit()).Returns(false);
		revision.Setup(x => x.WhyCannotSubmit()).Returns("Missing required material");
		var results = BuilderPreflight.Validate(revision.Object, world.Object);
		Assert.AreEqual("Missing required material", results.Single(x => x.IsError).Message);
		revision.Verify(x => x.ChangeStatus(It.IsAny<RevisionStatus>(), It.IsAny<string>(), It.IsAny<IAccount>()), Times.Never);
	}

	[TestMethod]
	public void Validation_CorruptSchoolCycleTerminatesAndPointsToDependency()
	{
		var a = new Mock<IMagicSchool>(); var b = new Mock<IMagicSchool>();
		a.SetupGet(x => x.Id).Returns(1); b.SetupGet(x => x.Id).Returns(2);
		a.SetupGet(x => x.ParentSchool).Returns(b.Object); b.SetupGet(x => x.ParentSchool).Returns(a.Object);
		var result = BuilderPreflight.Validate(a.Object, BuilderWorkflowRegressionTests.World().Object);
		Assert.AreEqual("magic school show 1", result.Single(x => x.IsError).InspectCommand);
	}

	[TestMethod]
	public void Validation_CropRejectsMissingMaterialsAndNonFiniteWeights()
	{
		var world = BuilderWorkflowRegressionTests.World();
		world.SetupGet(x => x.Materials).Returns(new All<MudSharp.Form.Material.ISolid>());
		var crop = new Mock<IAgricultureCropDefinition>();
		crop.SetupGet(x => x.BaseGrowthDays).Returns(10); crop.SetupGet(x => x.HarvestWindowDays).Returns(5);
		crop.SetupGet(x => x.YieldOutputs).Returns(new[] { new AgricultureCommodityYield("missing", double.NaN) });
		crop.SetupGet(x => x.SeedRequirements).Returns(Array.Empty<AgricultureCommodityYield>());
		var result = BuilderPreflight.Validate(crop.Object, world.Object);
		Assert.AreEqual(2, result.Count(x => x.IsError));
		Assert.IsTrue(result.Any(x => x.InspectCommand == "material list"));
	}

	[TestMethod]
	public void RevisionDiff_ReportsChangedRemovedAndAddedFieldsWithoutUnchangedNoise()
	{
		var before = new Dictionary<string, string> { ["Name"] = "Old", ["Text"] = "line one\nline two", ["Removed"] = "value" };
		var after = new Dictionary<string, string> { ["Name"] = "New", ["Text"] = "line one\nline two", ["Added"] = "value" };
		var changes = BuilderRevisionDiff.Compare(before, after);
		Assert.AreEqual(3, changes.Count);
		Assert.IsFalse(changes.Any(x => x.Field == "Text"));
		Assert.AreEqual("(absent)", changes.Single(x => x.Field == "Removed").After);
		Assert.AreEqual("(absent)", changes.Single(x => x.Field == "Added").Before);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void RevisionDiff_UsesCurrentRevisionAndRespectsPrivateBaselines(bool mayView)
	{
		var world = BuilderWorkflowRegressionTests.World(); var actor = BuilderWorkflowRegressionTests.Actor(world.Object);
		actor.Setup(x => x.IsAdministrator(It.IsAny<PermissionLevel>())).Returns(false);
		var current = Mock.Of<IGameItemSkin>(x => x.Id == 1 && x.RevisionNumber == 2 && x.Status == RevisionStatus.Current && x.Name == "Current" && x.FullDescription == "Secret baseline" && x.IsPublic == mayView && x.BuilderAccountID == 2);
		var rejected = Mock.Of<IGameItemSkin>(x => x.Id == 1 && x.RevisionNumber == 3 && x.Status == RevisionStatus.Rejected && x.Name == "Rejected" && x.FullDescription == "Rejected text" && x.IsPublic);
		var candidate = Mock.Of<IGameItemSkin>(x => x.Id == 1 && x.RevisionNumber == 4 && x.Status == RevisionStatus.UnderDesign && x.Name == "Candidate" && x.FullDescription == "New text" && x.BuilderAccountID == 1);
		Mock.Get(candidate).Setup(x => x.IsAssociatedBuilder(actor.Object)).Returns(true);
		var skins = new RevisableAll<IGameItemSkin>(); skins.Add(current); skins.Add(rejected); skins.Add(candidate);
		world.SetupGet(x => x.ItemSkins).Returns(skins);
		var output = Capture(actor);
		BaseBuilderModule.GenericRevisableBuildingCommand(actor.Object, new StringStack("diff 1 4"), EditableRevisableItemHelper.ItemSkinHelper);
		Assert.AreEqual(mayView, output.Any(x => x.Contains("Secret baseline")));
		Assert.IsFalse(output.Any(x => x.Contains("Rejected text")));
	}

	[DataTestMethod]
	[DataRow("normal", "Harbour Time")]
	[DataRow("intervening", "Someone Else")]
	[DataRow("collision", "New Alias")]
	[DataRow("removed", "New Alias")]
	public void NameRecovery_RevalidatesCurrentNameIdentityAndCollisions(string scenario, string expected)
	{
		var fixture = TimezoneFixture();
		BaseBuilderModule.GenericBuildingCommand(fixture.Actor.Object, new StringStack("open \"Harbour Time\""), EditableItemHelper.TimezoneHelper);
		BaseBuilderModule.GenericBuildingCommand(fixture.Actor.Object, new StringStack("set alias New Alias"), EditableItemHelper.TimezoneHelper);
		Assert.AreEqual("New Alias", fixture.Zone.Name);
		switch (scenario)
		{
			case "intervening": fixture.Zone.Alias = "Someone Else"; break;
			case "collision": fixture.Zones.Add(new MudTimeZone(2, 0, 0, "Other", "Harbour Time", fixture.Clock)); break;
			case "removed": fixture.Zones.Remove(fixture.Zone); break;
		}
		BaseBuilderModule.GenericBuildingCommand(fixture.Actor.Object, new StringStack("undo"), EditableItemHelper.TimezoneHelper);
		Assert.AreEqual(expected, fixture.Zone.Name);
	}

	[TestMethod]
	public void EditorOverview_ListsIndependentSelectionsAndClosingDoesNotRollback()
	{
		var fixture = TimezoneFixture();
		var output = Capture(fixture.Actor);
		BaseBuilderModule.GenericBuildingCommand(fixture.Actor.Object, new StringStack("open \"Harbour Time\""), EditableItemHelper.TimezoneHelper);
		fixture.Effects.Add(new BuilderEditingEffect<IGameItemSkin>(fixture.Actor.Object) { EditingItem = Mock.Of<IGameItemSkin>(x => x.Id == 9 && x.Name == "Draft Skin" && x.FrameworkItemType == "Skin" && x.Status == RevisionStatus.UnderDesign) });
		fixture.Zone.OffsetHours = 4;
		BuilderModule.Instance.Commands.TCommands["buildereditors"].Execute(fixture.Actor.Object, "buildereditors", permissionLevel: PermissionLevel.Admin);
		StringAssert.Contains(string.Join("\n", output), "Draft Skin");
		StringAssert.Contains(string.Join("\n", output), "Harbour Time");
		BaseBuilderModule.GenericBuildingCommand(fixture.Actor.Object, new StringStack("close"), EditableItemHelper.TimezoneHelper);
		Assert.AreEqual(4, fixture.Zone.OffsetHours);
		Assert.AreEqual(1, fixture.Effects.OfType<IBuilderEditingEffect>().Count());
	}

	[DataTestMethod]
	[DataRow("my skin", false)]
	[DataRow("my \"quoted\" skin", false)]
	[DataRow("my skin", true)]
	public void RevisionNameRecovery_RestoresExactTextButCannotChangeNewlyApprovedContent(string original, bool approved)
	{
		var fixture = TimezoneFixture();
		var world = Mock.Get(fixture.Actor.Object.Gameworld);
		var prototypes = new RevisableAll<IGameItemProto>();
		prototypes.Add(Mock.Of<IGameItemProto>(x => x.Id == 1 && x.PermitPlayerSkins && x.Status == RevisionStatus.Current));
		world.SetupGet(x => x.ItemProtos).Returns(prototypes);
		var progs = new All<IFutureProg>(); progs.Add(Mock.Of<IFutureProg>(x => x.Id == 1)); world.SetupGet(x => x.FutureProgs).Returns(progs);
		var row = new MudSharp.Models.GameItemSkin { Id = 42, Name = original, ItemProtoId = 1, CanUseSkinProgId = 1, EditableItem = new MudSharp.Models.EditableItem { BuilderAccountId = 1, RevisionStatus = (int)RevisionStatus.UnderDesign } };
		var skin = new GameItemSkin(row, world.Object);
		var skins = new RevisableAll<IGameItemSkin>(); skins.Add(skin); world.SetupGet(x => x.ItemSkins).Returns(skins);
		fixture.Effects.Add(new BuilderEditingEffect<IGameItemSkin>(fixture.Actor.Object) { EditingItem = skin });
		fixture.Actor.Setup(x => x.EffectsOfType<BuilderEditingEffect<IGameItemSkin>>()).Returns(() => fixture.Effects.OfType<BuilderEditingEffect<IGameItemSkin>>());
		BaseBuilderModule.GenericRevisableBuildingCommand(fixture.Actor.Object, new StringStack("set name Replacement"), EditableRevisableItemHelper.ItemSkinHelper);
		Assert.AreEqual("Replacement", skin.Name);
		if (approved) skin.ChangeStatus(RevisionStatus.Current, "Approved", fixture.Actor.Object.Account);
		BaseBuilderModule.GenericRevisableBuildingCommand(fixture.Actor.Object, new StringStack("undo"), EditableRevisableItemHelper.ItemSkinHelper);
		Assert.AreEqual(approved ? "Replacement" : original, skin.Name);
	}

	[DataTestMethod]
	[DataRow("craft validate 1")]
	[DataRow("craft undo")]
	public void PlayerCraftCommand_DiagnosticsDoNotBypassAdministratorGuard(string input)
	{
		var world = BuilderWorkflowRegressionTests.World(); var actor = BuilderWorkflowRegressionTests.Actor(world.Object);
		CraftModule.Instance.Commands.TCommands["craft"].Execute(actor.Object, input, permissionLevel: PermissionLevel.Any);
		world.VerifyGet(x => x.Crafts, Times.Never);
		actor.Verify(x => x.EffectsOfType<BuilderNameUndo>(), Times.Never);
	}

	internal static (Mock<ICharacter> Actor, MudTimeZone Zone, All<IMudTimeZone> Zones, IClock Clock, List<IEffect> Effects) TimezoneFixture()
	{
		var world = BuilderWorkflowRegressionTests.World(); var actor = BuilderWorkflowRegressionTests.Actor(world.Object);
		var clock = new Mock<IClock>(); clock.SetupGet(x => x.Id).Returns(1); clock.SetupGet(x => x.Gameworld).Returns(world.Object);
		clock.SetupGet(x => x.MinutesPerHour).Returns(60);
		var zones = new All<IMudTimeZone>(); clock.SetupGet(x => x.Timezones).Returns(zones);
		var clocks = new All<IClock>(); clocks.Add(clock.Object); world.SetupGet(x => x.Clocks).Returns(clocks);
		var zone = new MudTimeZone(1, 0, 0, "Old description", "Harbour Time", clock.Object); zones.Add(zone);
		var effects = new List<IEffect>();
		actor.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(effects.Add);
		actor.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>())).Callback<IEffect, TimeSpan>((effect, _) => effects.Add(effect));
		actor.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>())).Callback<IEffect, bool>((effect, _) => effects.Remove(effect));
		actor.Setup(x => x.CombinedEffectsOfType<IBuilderEditingEffect>()).Returns(() => effects.OfType<IBuilderEditingEffect>());
		actor.Setup(x => x.CombinedEffectsOfType<BuilderEditingEffect<IMudTimeZone>>()).Returns(() => effects.OfType<BuilderEditingEffect<IMudTimeZone>>());
		actor.Setup(x => x.EffectsOfType<BuilderNameUndo>()).Returns(() => effects.OfType<BuilderNameUndo>());
		actor.Setup(x => x.RemoveAllEffects<BuilderEditingEffect<IMudTimeZone>>(It.IsAny<Predicate<BuilderEditingEffect<IMudTimeZone>>>(), It.IsAny<bool>()))
			.Returns((Predicate<BuilderEditingEffect<IMudTimeZone>>? predicate, bool _) => effects.RemoveAll(x => x is BuilderEditingEffect<IMudTimeZone> editing && (predicate?.Invoke(editing) ?? true)) > 0);
		actor.Setup(x => x.RemoveAllEffects<BuilderNameUndo>(It.IsAny<Predicate<BuilderNameUndo>>(), It.IsAny<bool>()))
			.Returns((Predicate<BuilderNameUndo>? predicate, bool _) => effects.RemoveAll(x => x is BuilderNameUndo undo && (predicate?.Invoke(undo) ?? true)) > 0);
		return (actor, zone, zones, clock.Object, effects);
	}

	internal static List<string> Capture(Mock<ICharacter> actor)
	{
		var result = new List<string>();
		Mock.Get(actor.Object.OutputHandler).Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
			.Callback<string, bool, bool>((text, _, _) => result.Add(text)).Returns(true);
		return result;
	}
}
