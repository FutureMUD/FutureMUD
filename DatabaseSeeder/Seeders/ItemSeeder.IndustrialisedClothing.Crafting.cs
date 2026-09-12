#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Form.Material;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void ValidateIndustrialisedClothingCrafts(IndustrialisedClothingCatalogueDocument document,
		IReadOnlySet<string> selected, IndustrialisedClothingColourBindings colours)
	{
		var physicalItems = IndustrialisedCatalogue.Items.ToDictionary(x => x.StableReference, StringComparer.Ordinal);
		var traits = _context!.TraitDefinitions.AsNoTracking().ToArray();
		var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		foreach (var craft in document.Crafts)
		{
			if (craft.EraAdmissions.Any(selected.Contains)) RequireReviewed(craft.ReviewStatus, craft.Source);
			if (!names.Add(CraftLookupKey(craft.Name, craft.Category))) throw craft.Source.Error("Two clothing crafts have the same name/category.");
			var exactTraits = traits.Where(x => x.Name.Equals(craft.Trait, StringComparison.OrdinalIgnoreCase)).ToArray();
			var traitNames = TraitDefinitionLookupNames(craft.Trait).ToHashSet(StringComparer.OrdinalIgnoreCase);
			var candidates = exactTraits.Length > 0 ? exactTraits : traits.Where(x => traitNames.Contains(x.Name)).ToArray();
			if (candidates.Length != 1) throw craft.Source.Error($"Missing or ambiguous crafting trait {craft.Trait}.");
			var spec = IndustrialisedClothingCraftPlan.Compile(document, craft) with { Trait = candidates[0] };
			var inputs = document.CraftInputs.Where(x => x.CraftReference == craft.StableReference).ToDictionary(x => x.Order);
			foreach (var input in inputs.Values)
			{
				switch (input.Kind)
				{
					case ClothingInputKind.Item: ResolveItemMaterial(input.Reference, craft, input.Source); break;
					case ClothingInputKind.Commodity: RequireSolid(input.Reference, input.Source); break;
					case ClothingInputKind.Tag:
					case ClothingInputKind.CommodityTag: RequireTag(input.Reference, input.Source); break;
					case ClothingInputKind.Liquid:
						if (!_liquids.ContainsKey(input.Reference)) throw input.Source.Error($"Missing liquid {input.Reference}.");
						break;
				}
			}
			foreach (var tool in document.CraftTools.Where(x => x.CraftReference == craft.StableReference)) RequireTag(tool.Tag, tool.Source);
			foreach (var product in document.CraftProducts.Where(x => x.CraftReference == craft.StableReference))
			{
				if (product.Kind == ClothingProductKind.Commodity) RequireSolid(product.Reference, product.Source);
				if (product.Kind != ClothingProductKind.Item) continue;
				var material = ResolveItemMaterial(product.Reference, craft, product.Source);
				if (product.MaterialInputOrder is { } order)
				{
					var input = inputs[order];
					var inputMaterial = input.Kind switch
					{
						ClothingInputKind.Item => ResolveItemMaterial(input.Reference, craft, input.Source),
						ClothingInputKind.Commodity => RequireSolid(input.Reference, input.Source),
						_ => throw product.Source.Error("Material override must have an exact solid item/commodity input; a tagged or liquid source cannot prove the garment's physical/economic material.")
					};
					if (inputMaterial.Id != material.Id) throw product.Source.Error("Material override changes the garment's declared material; use the correct distinct base.");
				}
				var presentation = product.SkinReference.Length > 0 ? product.SkinReference : product.Reference;
				var bound = _clothingColourBindings[presentation];
				foreach (var (variable, selection) in IndustrialisedClothingColourPlan.CraftValues(document, product))
				{
					if (selection.InputOrder.HasValue) colours.ValidateUnrestrictedCraftInheritance(bound[variable], selection.Source);
					else if (!bound[variable].Values.ContainsKey(selection.Value)) throw selection.Source.Error("Selected craft value has no resolved characteristic binding.");
				}
			}
			_clothingCraftPlans.Add(craft.StableReference, spec);
		}

		Material RequireSolid(string name, ClothingSourceLocation source)
		{
			if (!_materials.TryGetValue(name, out var material) || material.Type != (int)MaterialType.Solid)
				throw source.Error($"Missing solid material {name}.");
			return material;
		}
		void RequireTag(string path, ClothingSourceLocation source)
		{
			if (!_tagsByFullPath.ContainsKey(path)) throw source.Error($"Missing exact parent-qualified tag {path}.");
		}
		Material ResolveItemMaterial(string reference, ClothingCraftRow craft, ClothingSourceLocation source)
		{
			if (physicalItems.TryGetValue(reference, out var item))
			{
				if (craft.EraAdmissions.Except(item.EraAdmissions, StringComparer.Ordinal).Any())
					throw source.Error($"Item {reference} is not admitted in every craft era.");
				return RequireSolid(item.Material, source);
			}
			if (!_itemsByStableReference.TryGetValue(reference, out var existing))
			{
				if (_clothingReusePlans.TryGetValue(reference, out var reuse)) return RequireSolid(reuse.Material, source);
				throw source.Error($"Unresolved craft item {reference}; provide its authoritative source/reuse dependency.");
			}
			if (!_materialNamesById.TryGetValue(existing.MaterialId, out var name)) throw source.Error($"Unresolved material on {reference}.");
			return RequireSolid(name, source);
		}
	}

	private void SeedIndustrialisedClothingCrafts(string eras)
	{
		if (IndustrialisedCatalogue.Clothing.Crafts.Count > 0 && !_clothingPreflightComplete)
			throw new InvalidOperationException("Clothing crafts cannot be applied before complete successful preflight.");
		var selected = ParseEraTokens(eras).ToHashSet(StringComparer.OrdinalIgnoreCase);
		ValidateIndustrialisedClothingCraftOwnership(IndustrialisedCatalogue.Clothing, selected);
		foreach (var craft in IndustrialisedCatalogue.Clothing.Crafts.Where(x => _manifestCaptureOnly || x.EraAdmissions.Any(selected.Contains)))
		{
			if (!_clothingCraftPlans.TryGetValue(craft.StableReference, out var spec))
				throw craft.Source.Error("Clothing craft must have a resolved preflight plan before persistence or canonical capture.");
			using var module = UseManifestModule("crafts", craft.EraAdmissions.ToArray());
			var gates = EnsureTraitGateProgs(spec.Trait!.Name, craft.MinimumTraitValue);
			AddCraft(spec with
			{
				Trait = gates.Trait, AppearProg = gates.AppearProg, CanUseProg = gates.CanUseProg, WhyCannotUseProg = gates.WhyCannotUseProg
			});
		}
	}
}
