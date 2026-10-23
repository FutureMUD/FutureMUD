// Selection views over the canonical compiled clothing and outfit records.
#nullable enable

using MudSharp.GameItems;

using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private static readonly DocumentedClothingItemSpec[] AntiquityOutfitSupplementalItemSpecs =
		[.. ClothingSources01Stock.AntiquityOutfitSupplementalItemSpecs().Concat(ClothingSources02Stock.AntiquityOutfitSupplementalItemSpecs()).Select(item => new DocumentedClothingItemSpec(item))];

	private static readonly DocumentedClothingItemSpec[] RenaissanceClothingItemSpecs =
		[.. ClothingSources01Stock.RenaissanceClothingItemSpecs().Concat(ClothingSources02Stock.RenaissanceClothingItemSpecs()).Select(item => new DocumentedClothingItemSpec(item))];

	private static readonly DocumentedClothingItemSpec[] EarlyModernOutfitReferencedItemSpecs =
		[.. ClothingSources01Stock.EarlyModernOutfitReferencedItemSpecs().Concat(ClothingSources02Stock.EarlyModernOutfitReferencedItemSpecs()).Select(item => new DocumentedClothingItemSpec(item))];

	private static readonly CatalogueSkinDefinition[] CatalogueSkinDefinitions = [.. ClothingSkinsStock.Items()];

	private static readonly OutfitManifestSpec[] AntiquityOutfitManifestSpecs =
		[.. Outfits01Stock.AntiquityOutfitManifestSpecs().Concat(Outfits02Stock.AntiquityOutfitManifestSpecs()).Select(ToLegacyOutfit)];

	private static readonly OutfitManifestSpec[] MedievalOutfitManifestSpecs =
		[.. Outfits01Stock.MedievalOutfitManifestSpecs().Concat(Outfits02Stock.MedievalOutfitManifestSpecs()).Select(ToLegacyOutfit)];

	private static readonly OutfitManifestSpec[] RenaissanceOutfitManifestSpecs =
		[.. Outfits01Stock.RenaissanceOutfitManifestSpecs().Concat(Outfits02Stock.RenaissanceOutfitManifestSpecs()).Select(ToLegacyOutfit)];

	private static readonly OutfitManifestSpec[] EarlyModernOutfitManifestSpecs =
		[.. Outfits01Stock.EarlyModernOutfitManifestSpecs().Concat(Outfits02Stock.EarlyModernOutfitManifestSpecs()).Select(ToLegacyOutfit)];

}
