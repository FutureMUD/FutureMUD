#nullable enable

using MudSharp.Form.Material;
using MudSharp.GameItems;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private sealed record EraCultureSpec(
		string Key,
		string Display,
		string TagName);

	private sealed record EraVariableColourPolicy(
		string SingleColourComponent,
		string TwoColourComponent,
		IReadOnlyCollection<string> SingleColourVariables,
		IReadOnlyCollection<string> TwoColourVariables,
		IReadOnlyCollection<string> NonColourableMaterials);

	private sealed record EraSeederConfiguration(
		string EraKey,
		string EraRootTag,
		string CultureTagRoot,
		string? StatusOrSocialRoleTagRoot,
		string StableReferencePrefix,
		IReadOnlyCollection<string> DefaultMarketTags,
		EraVariableColourPolicy DefaultVariableColourPolicy,
		IReadOnlyCollection<string> DefaultCraftKnowledgeNames,
		IReadOnlyCollection<string> DefaultCraftCategories,
		IReadOnlyDictionary<string, string> CommonMaterialStockNames,
		IReadOnlyCollection<EraOutfitSlotSpec> ClothingSlotDefinitions,
		bool CompleteOutfitCataloguesRequired,
		bool GenericBaselineWardrobeGenerationAllowed,
		bool PlayerFacingDescriptionsMayIncludeCultureNames);

	private sealed record EraItemSpec(
		string StableReference,
		string Noun,
		string ShortDescription,
		string FullDescription,
		SizeCategory Size,
		ItemQuality Quality,
		double WeightInGrams,
		decimal Cost,
		string Material,
		MaterialBehaviourType MaterialType,
		string[] Tags,
		string[] Components,
		string? BuilderNotes = null,
		string? MorphToUniqueReference = null,
		string? MorphEmote = null,
		TimeSpan? MorphTimer = null,
		string? DestroyedItemUniqueReference = null);

	private sealed record EraOutfitSlotSpec(
		string Key,
		string Display,
		bool RequiredForAllOutfits,
		string[] RequiredForRoles);

	private sealed record EraOutfitSpec(
		string OutfitReference,
		string CultureKey,
		string SexGenderPresentation,
		string SocialClassRole,
		string DisplayName,
		IReadOnlyDictionary<string, string> SlotItemStableReferences,
		IReadOnlyCollection<string> IntentionallySharedOrGenericSlots);

	private sealed record EraOutfitPieceSpec(
		string OutfitReference,
		string CultureKey,
		string SexGenderPresentation,
		string SocialClassRole,
		string SlotKey,
		string PieceName,
		string StableReference,
		bool CultureSpecificOrClusterSpecific);

	private sealed record EraCraftInputSpec(string Definition);

	private sealed record EraCraftToolSpec(string Definition);

	private sealed record EraCraftProductSpec(
		Func<GameItemProto, string> DefinitionFactory,
		IReadOnlyCollection<string> VariableMappings)
	{
		public EraCraftProductSpec(string definition) : this(_ => definition, [])
		{
		}

		public string BuildDefinition(GameItemProto item)
		{
			return DefinitionFactory(item);
		}
	}

	private sealed record EraCraftSpec(
		string Category,
		string Trait,
		IReadOnlyCollection<EraCraftInputSpec> Inputs,
		IReadOnlyCollection<EraCraftToolSpec> Tools,
		IReadOnlyCollection<EraCraftProductSpec> Products,
		Difficulty Difficulty,
		string Verb,
		string Gerund);

	private sealed record EraClothingPieceSpec(
		EraItemSpec Item,
		string OutfitReference,
		string CultureKey,
		string SexGenderPresentation,
		string SocialClassRole,
		string PieceName,
		bool CultureSpecificOrClusterSpecific,
		IReadOnlyCollection<string> SlotKeys,
		IReadOnlyCollection<string> OutfitReferences,
		string? VariableColourComponent,
		IReadOnlyCollection<string> ColourVariablesUsed,
		EraCraftSpec Craft,
		bool IntentionallySharedOrGeneric);

	private void SeedEraItemSpecs(IEnumerable<EraItemSpec> specs)
	{
		foreach (var spec in specs)
		{
			EnsureAntiquityComponentGapMaterial(spec.Material, spec.MaterialType);
			foreach (var tag in spec.Tags)
			{
				EnsureAntiquityTagPath(tag);
			}

			CreateItem(
				spec.StableReference,
				spec.Noun,
				spec.ShortDescription,
				null,
				spec.FullDescription,
				spec.Size,
				spec.Quality,
				spec.WeightInGrams,
				spec.Cost,
				false,
				false,
				spec.Material,
				spec.Tags,
				spec.Components,
				spec.MorphToUniqueReference,
				spec.MorphEmote,
				spec.MorphTimer,
				spec.DestroyedItemUniqueReference,
				spec.BuilderNotes);
		}
	}
}
