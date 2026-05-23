using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.NPC.Templates;

#nullable enable

namespace MudSharp_Unit_Tests;

[TestClass]
public class NPCTemplateLookupExtensionsTests
{
	[TestMethod]
	public void UniqueNameLookupPrefersExactUniqueNameBeforeLegacyName()
	{
		var unique = Template(1, 0, RevisionStatus.Current, "wanderer", "guard");
		var legacy = Template(2, 0, RevisionStatus.Current, "guard", null);

		var result = new[] { legacy, unique }.GetByIdOrUniqueNameOrName("guard");

		Assert.AreSame(unique, result);
	}

	[TestMethod]
	public void UniqueNameLookupIsCaseInsensitiveAndActiveOnly()
	{
		var historical = Template(1, 0, RevisionStatus.Obsolete, "old", "template-key");
		var current = Template(2, 0, RevisionStatus.Current, "new", "Template-Key");

		var result = new[] { historical, current }.FindByUniqueName("template-key");

		Assert.AreSame(current, result);
	}

	[TestMethod]
	public void UniqueNameValidationRejectsAllNumericNames()
	{
		Assert.IsFalse(NPCTemplateLookupExtensions.IsValidUniqueName("12345"));
		Assert.IsTrue(NPCTemplateLookupExtensions.IsValidUniqueName("template-12345"));
		Assert.IsTrue(NPCTemplateLookupExtensions.IsValidUniqueName("  "));
	}

	[TestMethod]
	public void ActiveConflictIgnoresSameIdAndHistoricalRows()
	{
		var sameId = Template(1, 1, RevisionStatus.UnderDesign, "same", "shared-key");
		var obsolete = Template(2, 0, RevisionStatus.Obsolete, "old", "shared-key");
		var current = Template(3, 0, RevisionStatus.Current, "current", "shared-key");

		var templates = new[] { sameId, obsolete, current };

		Assert.IsNull(new[] { sameId, obsolete }.GetActiveUniqueNameConflict("shared-key", 1));
		Assert.IsNull(new[] { obsolete }.GetActiveUniqueNameConflict("shared-key", 99));
		Assert.AreSame(current, templates.GetActiveUniqueNameConflict("shared-key", 1));
	}

	[TestMethod]
	public void RevisableLookupUsesDifferentPriorityForEditing()
	{
		var current = Template(1, 0, RevisionStatus.Current, "guard", "guard-key");
		var underDesign = Template(1, 1, RevisionStatus.UnderDesign, "guard", "guard-key");

		var templates = new[] { current, underDesign };

		Assert.AreSame(current, templates.GetByIdOrNameRevisable("guard-key"));
		Assert.AreSame(underDesign, templates.GetByIdOrNameRevisableForEditing("guard-key"));
	}

	[TestMethod]
	public void BuilderSearchTextMatchesUniqueNameAndBuilderNotes()
	{
		var template = Template(1, 0, RevisionStatus.Current, "guard", "clockwork-key", "Used by the clockwork quest.");

		Assert.IsTrue(template.HasBuilderSearchText("clockwork"));
		Assert.IsTrue(template.HasBuilderSearchText("quest"));
		Assert.IsFalse(template.HasBuilderSearchText("alchemy"));
	}

	private static INPCTemplate Template(long id, int revision, RevisionStatus status, string name, string? uniqueName, string? builderNotes = null)
	{
		var template = new Mock<INPCTemplate>();
		template.SetupGet(x => x.Id).Returns(id);
		template.SetupGet(x => x.RevisionNumber).Returns(revision);
		template.SetupGet(x => x.Status).Returns(status);
		template.SetupGet(x => x.Name).Returns(name);
		template.SetupGet(x => x.UniqueName).Returns(uniqueName);
		template.SetupGet(x => x.BuilderNotes).Returns(builderNotes);
		return template.Object;
	}
}
