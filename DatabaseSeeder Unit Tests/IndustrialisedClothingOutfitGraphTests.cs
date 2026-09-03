#nullable enable

using System.IO;
using System.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.GameItems;

namespace MudSharp_Unit_Tests;

public partial class IndustrialisedClothingReuseTests
{
	[DataTestMethod]
	[DataRow("missing-item", "exactly one current item prototype")]
	[DataRow("invalid-key", "invalid or duplicate entry key")]
	[DataRow("missing-profile", "missing or unsupported wear profile")]
	[DataRow("profile-on-inventory", "wear profile but is not worn")]
	[DataRow("missing-colour", "explicitly select every runtime colour channel")]
	[DataRow("random-colour", "must select a value")]
	[DataRow("unknown-placement", "unknown placement")]
	[DataRow("wielded", "is not wieldable")]
	[DataRow("self-container", "missing or self-referential target")]
	public void PreservedOutfitGraph_RejectsInvalidRuntimeGraphWithoutMutation(string fault, string diagnostic)
	{
		using var context = Context();
		SeedSkinPrerequisites(context);
		var catalogue = CatalogueWithTwoOutfits();
		InstallSkins(context, catalogue);
		context.ChangeTracker.Clear();
		var outfit = context.OutfitTemplates.Include(x => x.OutfitTemplateItems)
			.Single(x => x.Name == "Fixture first ensemble");
		var entry = outfit.OutfitTemplateItems.Single();
		switch (fault)
		{
			case "missing-item": entry.GameItemProtoId = 999_999; break;
			case "invalid-key": entry.TemplateKey = "invalid key"; break;
			case "missing-profile": entry.WearProfileId = 999_999; break;
			case "profile-on-inventory": entry.Placement = (int)OutfitTemplateItemPlacement.Inventory; break;
			case "missing-colour": entry.LoadArguments = ""; break;
			case "random-colour": entry.LoadArguments = "colour=:31"; break;
			case "unknown-placement": entry.Placement = 999; entry.WearProfileId = null; break;
			case "wielded": entry.Placement = (int)OutfitTemplateItemPlacement.Wielded; entry.WearProfileId = null; break;
			case "self-container":
				entry.Placement = (int)OutfitTemplateItemPlacement.Container;
				entry.WearProfileId = null;
				entry.ContainerKey = entry.TemplateKey;
				break;
		}
		context.SaveChanges();
		context.ChangeTracker.Clear();
		var before = OutfitOwnershipState(context);
		var error = Assert.ThrowsException<InvalidDataException>(() =>
			new ItemSeeder(catalogue).ValidateClothingPrerequisitesForTesting(context, "industrial"));
		StringAssert.Contains(error.Message, "Clothing/outfits.tsv:2");
		StringAssert.Contains(error.Message, diagnostic);
		Assert.IsFalse(context.ChangeTracker.HasChanges());
		Assert.AreEqual(before, OutfitOwnershipState(context));
	}
}
