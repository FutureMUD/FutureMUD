using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Commands.Helpers;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.NPC.Templates;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class GameItemHelperTests
{
	[TestMethod]
	public void CustomSearch_CommentFilter_FiltersByBuilderNotes()
	{
		var matching = Proto("Clockwork calibration notes.");
		var other = Proto("Alchemy preparation notes.");
		List<IEditableRevisableItem> protos = [matching.Object, other.Object];

		var result = EditableRevisableItemHelper.GameItemHelper.CustomSearch(
			protos,
			"comment:calibration",
			new Mock<IFuturemud>().Object);

		CollectionAssert.AreEqual(new[] { matching.Object }, result.Cast<IGameItemProto>().ToArray());
	}

	[TestMethod]
	public void GetListTableHeaderFunc_ItemPrototype_IncludesUniqueName()
	{
		var header = EditableRevisableItemHelper.GameItemHelper.GetListTableHeaderFunc(new Mock<MudSharp.Character.ICharacter>().Object)
		                                      .ToArray();

		CollectionAssert.Contains(header, "Unique Name");
	}

	[TestMethod]
	public void CustomSearch_NpcTemplateCommentFilter_FiltersByBuilderNotes()
	{
		var matching = NpcTemplate("Clockwork calibration notes.");
		var other = NpcTemplate("Alchemy preparation notes.");
		List<IEditableRevisableItem> protos = [matching.Object, other.Object];

		var result = EditableRevisableItemHelper.NpcTemplateHelper.CustomSearch(
			protos,
			"comment:calibration",
			new Mock<IFuturemud>().Object);

		CollectionAssert.AreEqual(new[] { matching.Object }, result.Cast<INPCTemplate>().ToArray());
	}

	[TestMethod]
	public void GetListTableHeaderFunc_NpcTemplate_IncludesUniqueName()
	{
		var header = EditableRevisableItemHelper.NpcTemplateHelper.GetListTableHeaderFunc(new Mock<MudSharp.Character.ICharacter>().Object)
		                                         .ToArray();

		CollectionAssert.Contains(header, "Unique Name");
	}

	private static Mock<IGameItemProto> Proto(string builderNotes)
	{
		var proto = new Mock<IGameItemProto>();
		proto.SetupGet(x => x.BuilderNotes).Returns(builderNotes);
		return proto;
	}

	private static Mock<INPCTemplate> NpcTemplate(string builderNotes)
	{
		var template = new Mock<INPCTemplate>();
		template.SetupGet(x => x.BuilderNotes).Returns(builderNotes);
		return template;
	}
}
