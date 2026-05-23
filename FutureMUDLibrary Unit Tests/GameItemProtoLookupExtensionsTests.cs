using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace MudSharp_Unit_Tests;

[TestClass]
public class GameItemProtoLookupExtensionsTests
{
	[TestMethod]
	public void UniqueNameLookupPrefersExactUniqueNameBeforeLegacyName()
	{
		var unique = Proto(1, 0, RevisionStatus.Current, "widget", "alpha");
		var legacy = Proto(2, 0, RevisionStatus.Current, "alpha", null);

		var result = new[] { legacy, unique }.GetByIdOrUniqueNameOrName("alpha");

		Assert.AreSame(unique, result);
	}

	[TestMethod]
	public void UniqueNameLookupIsCaseInsensitiveAndActiveOnly()
	{
		var historical = Proto(1, 0, RevisionStatus.Obsolete, "old", "template-key");
		var current = Proto(2, 0, RevisionStatus.Current, "new", "Template-Key");

		var result = new[] { historical, current }.FindByUniqueName("template-key");

		Assert.AreSame(current, result);
	}

	[TestMethod]
	public void UniqueNameValidationRejectsAllNumericNames()
	{
		Assert.IsFalse(GameItemProtoLookupExtensions.IsValidUniqueName("12345"));
		Assert.IsTrue(GameItemProtoLookupExtensions.IsValidUniqueName("template-12345"));
		Assert.IsTrue(GameItemProtoLookupExtensions.IsValidUniqueName("  "));
	}

	[TestMethod]
	public void ActiveConflictIgnoresSameIdAndHistoricalRows()
	{
		var sameId = Proto(1, 1, RevisionStatus.UnderDesign, "same", "shared-key");
		var obsolete = Proto(2, 0, RevisionStatus.Obsolete, "old", "shared-key");
		var current = Proto(3, 0, RevisionStatus.Current, "current", "shared-key");

		var protos = new[] { sameId, obsolete, current };

		Assert.IsNull(new[] { sameId, obsolete }.GetActiveUniqueNameConflict("shared-key", 1));
		Assert.IsNull(new[] { obsolete }.GetActiveUniqueNameConflict("shared-key", 99));
		Assert.AreSame(current, protos.GetActiveUniqueNameConflict("shared-key", 1));
	}

	[TestMethod]
	public void RevisableLookupUsesDifferentPriorityForEditing()
	{
		var current = Proto(1, 0, RevisionStatus.Current, "tool", "tool-key");
		var underDesign = Proto(1, 1, RevisionStatus.UnderDesign, "tool", "tool-key");

		var protos = new[] { current, underDesign };

		Assert.AreSame(current, protos.GetByIdOrNameRevisable("tool-key"));
		Assert.AreSame(underDesign, protos.GetByIdOrNameRevisableForEditing("tool-key"));
	}

	[TestMethod]
	public void BuilderSearchTextMatchesUniqueNameAndBuilderNotes()
	{
		var proto = Proto(1, 0, RevisionStatus.Current, "tool", "clockwork-key", "Used by the clockwork quest.");

		Assert.IsTrue(proto.HasBuilderSearchText("clockwork"));
		Assert.IsTrue(proto.HasBuilderSearchText("quest"));
		Assert.IsFalse(proto.HasBuilderSearchText("alchemy"));
	}

	private static IGameItemProto Proto(long id, int revision, RevisionStatus status, string name, string? uniqueName, string? builderNotes = null)
	{
		var proto = new Mock<IGameItemProto>();
		proto.SetupGet(x => x.Id).Returns(id);
		proto.SetupGet(x => x.RevisionNumber).Returns(revision);
		proto.SetupGet(x => x.Status).Returns(status);
		proto.SetupGet(x => x.Name).Returns(name);
		proto.SetupGet(x => x.UniqueName).Returns(uniqueName);
		proto.SetupGet(x => x.BuilderNotes).Returns(builderNotes);
		return proto.Object;
	}
}
