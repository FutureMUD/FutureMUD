using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Framework;
using MudSharp.RPG.AIStorytellers;
using ModelReferenceDocument = MudSharp.Models.AIStorytellerReferenceDocument;

namespace MudSharp_Unit_Tests;

[TestClass]
public class AIStorytellerReferenceDocumentTests
{
	[TestMethod]
	public void IsVisibleTo_EmptyRestrictions_IsGlobal()
	{
		var document = new AIStorytellerReferenceDocument(
			new ModelReferenceDocument
			{
				Id = 1L,
				Name = "Primer",
				Description = "General primer",
				FolderName = "General",
				DocumentType = "Guide",
				Keywords = "primer,guide",
				DocumentContents = "General storyteller information.",
				RestrictedStorytellerIds = string.Empty
			},
			new Mock<IFuturemud>().Object
		);

		Assert.IsTrue(document.IsVisibleTo(CreateStoryteller(1L)));
		Assert.IsTrue(document.IsVisibleTo(CreateStoryteller(999L)));
	}

	[TestMethod]
	public void IsVisibleTo_PopulatedRestrictions_AllowsOnlyListedStorytellers()
	{
		var document = new AIStorytellerReferenceDocument(
			new ModelReferenceDocument
			{
				Id = 1L,
				Name = "Restricted Doc",
				Description = "Restricted",
				FolderName = "World",
				DocumentType = "Brief",
				Keywords = "restricted",
				DocumentContents = "Restricted text",
				RestrictedStorytellerIds = "5,7,11"
			},
			new Mock<IFuturemud>().Object
		);

		Assert.IsTrue(document.IsVisibleTo(CreateStoryteller(7L)));
		Assert.IsFalse(document.IsVisibleTo(CreateStoryteller(3L)));
		CollectionAssert.AreEquivalent(new List<long> { 5L, 7L, 11L }, document.RestrictedStorytellerIds.ToList());
	}

	[TestMethod]
	public void RestrictedStorytellerIds_LegacyXmlFormat_ParsesCorrectly()
	{
		var document = new AIStorytellerReferenceDocument(
			new ModelReferenceDocument
			{
				Id = 2L,
				Name = "Legacy",
				Description = "Legacy format test",
				FolderName = "World",
				DocumentType = "Brief",
				Keywords = "legacy",
				DocumentContents = "Legacy content",
				RestrictedStorytellerIds =
					"<Restrictions><Storyteller>22</Storyteller><Storyteller>44</Storyteller></Restrictions>"
			},
			new Mock<IFuturemud>().Object
		);

		CollectionAssert.AreEquivalent(new List<long> { 22L, 44L }, document.RestrictedStorytellerIds.ToList());
	}

	[TestMethod]
	public void ReturnForSearch_MultiTermSearch_MatchesAcrossFields()
	{
		var document = new AIStorytellerReferenceDocument(
			new ModelReferenceDocument
			{
				Id = 3L,
				Name = "Harbour Ward Primer",
				Description = "Faction and city-level operations",
				FolderName = "World",
				DocumentType = "Brief",
				Keywords = "harbour,faction,operations",
				DocumentContents = "This content explains district politics.",
				RestrictedStorytellerIds = string.Empty
			},
			new Mock<IFuturemud>().Object
		);

		Assert.IsTrue(document.ReturnForSearch("harbour politics"));
		Assert.IsTrue(document.ReturnForSearch(string.Empty));
		Assert.IsFalse(document.ReturnForSearch("harbour astronomy"));
	}

	private static IAIStoryteller CreateStoryteller(long id)
	{
		var storyteller = new Mock<IAIStoryteller>();
		storyteller.SetupGet(x => x.Id).Returns(id);
		return storyteller.Object;
	}
}
