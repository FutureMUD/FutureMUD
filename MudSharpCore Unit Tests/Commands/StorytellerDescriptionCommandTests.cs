#nullable enable

using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Editor;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests.Commands;

[TestClass]
public class StorytellerDescriptionCommandTests
{
	[DataTestMethod]
	[DataRow("Redesc", false)]
	[DataRow("Resdesc", true)]
	public void DescriptionCommand_ConditionalPrototypeText_RecallsMatchingRawText(string command, bool shortDescription)
	{
		var fixture = new Fixture();
		fixture.Item.SetupGet(x => x.Skin).Returns((IGameItemSkin)null!);
		var prog = new Mock<IFutureProg>();
		prog.Setup(x => x.Execute<bool?>(It.IsAny<object[]>())).Returns(true);
		fixture.Item.SetupGet(x => x.Prototype.ExtraDescriptions)
			.Returns(new (IFutureProg, string?, string?, string?)[]
			{
				(prog.Object, "conditional short", "Conditional full.", null)
			});
		fixture.Invoke(command);
		Assert.AreEqual(shortDescription ? "conditional short" : "Conditional full.", fixture.Recall);
		fixture.PostAction!(fixture.Recall!, fixture.Output.Object, fixture.Arguments!);
		Assert.AreEqual(fixture.Recall, shortDescription ? fixture.Item.Object.OverrideSdesc : fixture.Item.Object.OverrideDesc);
	}

	[DataTestMethod]
	[DataRow("Redesc", false)]
	[DataRow("Resdesc", true)]
	public void DescriptionCommand_ItemTarget_SubmitChangesOnlySelectedInstanceDescription(string command, bool shortDescription)
	{
		var fixture = new Fixture();
		fixture.Invoke(command);
		Assert.IsNotNull(fixture.PostAction);
		Assert.AreEqual(shortDescription ? "skin short" : "Skin full.", fixture.Recall);
		Assert.IsNull(fixture.Item.Object.OverrideSdesc);
		Assert.IsNull(fixture.Item.Object.OverrideDesc);

		fixture.PostAction!("  custom prose  ", fixture.Output.Object, fixture.Arguments!);

		Assert.AreEqual(shortDescription ? "custom prose" : null, fixture.Item.Object.OverrideSdesc);
		Assert.AreEqual(shortDescription ? null : "custom prose", fixture.Item.Object.OverrideDesc);
		fixture.Item.VerifySet(x => x.Skin = It.IsAny<IGameItemSkin>(), Times.Never);
	}

	[DataTestMethod]
	[DataRow("Redesc")]
	[DataRow("Resdesc")]
	public void DescriptionCommand_Cancel_LeavesOverridesUnchanged(string command)
	{
		var fixture = new Fixture();
		fixture.Item.Object.OverrideSdesc = "original short";
		fixture.Item.Object.OverrideDesc = "Original full.";
		fixture.Invoke(command);
		Assert.IsNotNull(fixture.CancelAction);
		fixture.CancelAction!(fixture.Output.Object, fixture.Arguments!);
		Assert.AreEqual("original short", fixture.Item.Object.OverrideSdesc);
		Assert.AreEqual("Original full.", fixture.Item.Object.OverrideDesc);
	}

	[DataTestMethod]
	[DataRow("Redesc", false)]
	[DataRow("Resdesc", true)]
	public void DescriptionCommand_Clear_RestoresOnlySelectedFieldToNull(string command, bool shortDescription)
	{
		var fixture = new Fixture();
		fixture.Item.Object.OverrideSdesc = "original short";
		fixture.Item.Object.OverrideDesc = "Original full.";
		fixture.Invoke(command, " clear");
		Assert.AreEqual(shortDescription ? null : "original short", fixture.Item.Object.OverrideSdesc);
		Assert.AreEqual(shortDescription ? "Original full." : null, fixture.Item.Object.OverrideDesc);
		Assert.IsNull(fixture.PostAction);
	}

	[DataTestMethod]
	[DataRow("Redesc")]
	[DataRow("Resdesc")]
	public void DescriptionCommand_UnresolvedCorpse_NeverEntersItemEditor(string command)
	{
		var fixture = new Fixture();
		fixture.Item.Setup(x => x.IsItemType<ICorpse>()).Returns(true);
		fixture.Invoke(command);
		Assert.IsNull(fixture.PostAction);
		fixture.Invoke(command, " clear");
		Assert.IsNull(fixture.PostAction);
		fixture.Item.VerifySet(x => x.OverrideDesc = It.IsAny<string?>(), Times.Never);
		fixture.Item.VerifySet(x => x.OverrideSdesc = It.IsAny<string?>(), Times.Never);
	}

	[DataTestMethod]
	[DataRow("Redesc", false)]
	[DataRow("Resdesc", true)]
	public void DescriptionCommand_CharacterOrResolvedCorpse_KeepsBodyEditor(string command, bool shortDescription)
	{
		var fixture = new Fixture();
		var character = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock };
		character.SetupGet(x => x.Body.GetRawDescriptions).Returns(("old short", "Old full."));
		character.SetupGet(x => x.Race.Name).Returns("human");
		character.SetupGet(x => x.Culture.Name).Returns("test culture");
		character.SetupGet(x => x.Ethnicity.Name).Returns("test ethnicity");
		fixture.Actor.Setup(x => x.TargetActorOrCorpse("test item", PerceiveIgnoreFlags.None)).Returns(character.Object);
		fixture.Invoke(command);
		Assert.IsNotNull(fixture.PostAction);
		fixture.PostAction!("new description", fixture.Output.Object, fixture.Arguments!);
		character.Verify(x => x.Body.SetFullDescription("new description"), shortDescription ? Times.Never : Times.Once);
		character.Verify(x => x.Body.SetShortDescription("new description"), shortDescription ? Times.Once : Times.Never);
		fixture.Actor.Verify(x => x.TargetItem(It.IsAny<string>()), Times.Never);
	}

	[DataTestMethod]
	[DataRow("Redesc")]
	[DataRow("Resdesc")]
	public void DescriptionCommand_DeletedWhileEditing_DoesNotApplyOverride(string command)
	{
		var fixture = new Fixture();
		fixture.Invoke(command);
		fixture.Item.SetupGet(x => x.Deleted).Returns(true);
		fixture.PostAction!("new prose", fixture.Output.Object, fixture.Arguments!);
		Assert.IsNull(fixture.Item.Object.OverrideSdesc);
		Assert.IsNull(fixture.Item.Object.OverrideDesc);
	}

	private sealed class Fixture
	{
		public Mock<ICharacter> Actor { get; } = new() { DefaultValue = DefaultValue.Mock };
		public Mock<IGameItem> Item { get; } = new() { DefaultValue = DefaultValue.Mock };
		public Mock<IOutputHandler> Output { get; } = new();
		public Action<string, IOutputHandler, object[]>? PostAction { get; private set; }
		public Action<IOutputHandler, object[]>? CancelAction { get; private set; }
		public object[]? Arguments { get; private set; }
		public string? Recall { get; private set; }

		public Fixture()
		{
			Item.SetupProperty(x => x.OverrideSdesc);
			Item.SetupProperty(x => x.OverrideDesc);
			Item.SetupGet(x => x.Skin.ShortDescription).Returns("skin short");
			Item.SetupGet(x => x.Skin.FullDescription).Returns("Skin full.");
			Actor.Setup(x => x.TargetActorOrCorpse(It.IsAny<string>(), PerceiveIgnoreFlags.None)).Returns((ICharacter?)null);
			Actor.Setup(x => x.TargetItem("test item")).Returns(Item.Object);
			Actor.SetupGet(x => x.OutputHandler).Returns(Output.Object);
			Actor.SetupGet(x => x.InnerLineFormatLength).Returns(120);
			Actor.Setup(x => x.EditorMode(It.IsAny<Action<string, IOutputHandler, object[]>>(),
					It.IsAny<Action<IOutputHandler, object[]>>(), It.IsAny<double>(), It.IsAny<string>(),
					It.IsAny<EditorOptions>(), It.IsAny<object[]>()))
				.Callback<Action<string, IOutputHandler, object[]>, Action<IOutputHandler, object[]>, double, string, EditorOptions, object[]>(
					(post, cancel, _, recall, _, args) =>
					{
						PostAction = post;
						CancelAction = cancel;
						Recall = recall;
						Arguments = args;
					});
		}

		public void Invoke(string command, string suffix = "") => typeof(StorytellerModule)
			.GetMethod(command, BindingFlags.Static | BindingFlags.NonPublic)!
			.Invoke(null, [Actor.Object, $"{command.ToLowerInvariant()} \"test item\"{suffix}"]);
	}
}
