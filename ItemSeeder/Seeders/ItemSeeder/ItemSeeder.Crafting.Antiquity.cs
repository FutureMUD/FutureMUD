using MudSharp.Models;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
    private bool ShouldSeedAntiquityCrafts()
    {
        return _questionAnswers?.TryGetValue("eras", out var eras) == true &&
               eras.Contains("antiquity", StringComparison.InvariantCultureIgnoreCase);
    }

    private bool TryLookupReworkItem(string stableReference, out GameItemProto item)
    {
        if (_items.TryGetValue(stableReference, out item!))
        {
            return true;
        }

        item = _context!.GameItemProtos.Local
            .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.UniqueName) &&
                                 x.UniqueName.Equals(stableReference, StringComparison.OrdinalIgnoreCase)) ??
               _context.GameItemProtos
                   .AsEnumerable()
                   .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.UniqueName) &&
                                        x.UniqueName.Equals(stableReference, StringComparison.OrdinalIgnoreCase))!;
        if (item is not null)
        {
            _items[stableReference] = item;
            _items[item.ShortDescription] = item;
            return true;
        }

        if (!HellenicAntiquityClothingStableReferences.TryGetValue(stableReference, out var shortDescription) &&
            !EgyptianAntiquityClothingStableReferences.TryGetValue(stableReference, out shortDescription) &&
            !RomanAntiquityClothingStableReferences.TryGetValue(stableReference, out shortDescription) &&
            !CelticAntiquityClothingStableReferences.TryGetValue(stableReference, out shortDescription) &&
            !GermanicAntiquityClothingStableReferences.TryGetValue(stableReference, out shortDescription) &&
            !KushiteAntiquityClothingStableReferences.TryGetValue(stableReference, out shortDescription) &&
            !PunicAntiquityClothingStableReferences.TryGetValue(stableReference, out shortDescription) &&
            !PersianAntiquityClothingStableReferences.TryGetValue(stableReference, out shortDescription) &&
            !EtruscanAntiquityClothingStableReferences.TryGetValue(stableReference, out shortDescription) &&
            !AnatolianAntiquityClothingStableReferences.TryGetValue(stableReference, out shortDescription) &&
            !ScythianSarmatianAntiquityClothingStableReferences.TryGetValue(stableReference, out shortDescription) &&
            !AntiquityLeatherClothingStableReferences.TryGetValue(stableReference, out shortDescription) &&
            !AntiquityLeatherArmourStableReferences.TryGetValue(stableReference, out shortDescription) &&
            !AntiquityLeatherContainerStableReferences.TryGetValue(stableReference, out shortDescription) &&
            !AntiquityLeatherFurnishingStableReferences.TryGetValue(stableReference, out shortDescription))
        {
            item = null!;
            return false;
        }

        if (_items.TryGetValue(shortDescription, out item!))
        {
            _items[stableReference] = item;
            return true;
        }

        item = _context!.GameItemProtos.Local
            .FirstOrDefault(x => x.ShortDescription.Equals(shortDescription, StringComparison.OrdinalIgnoreCase)) ??
               _context.GameItemProtos
                   .FirstOrDefault(x => x.ShortDescription.Equals(shortDescription, StringComparison.OrdinalIgnoreCase))!;
        if (item is null)
        {
            return false;
        }

        _items[stableReference] = item;
        return true;
    }

    private GameItemProto LookupReworkItem(string stableReference)
    {
        return TryLookupReworkItem(stableReference, out var item)
            ? item
            : throw new ApplicationException($"Unknown rework item stable reference {stableReference}");
    }

    private string StableSimpleProduct(string stableReference)
    {
        var item = LookupReworkItem(stableReference);
        return $"SimpleProduct - 1x {item.ShortDescription} (#{item.Id})";
    }

    private string StableSimpleProduct(string stableReference, int quantity)
    {
        var item = LookupReworkItem(stableReference);
        return $"SimpleProduct - {quantity}x {item.ShortDescription} (#{item.Id})";
    }

    private string StableSimpleItemDescription(string stableReference)
    {
        var item = LookupReworkItem(stableReference);
        return $"1x {item.ShortDescription} (#{item.Id})";
    }

    private string StableSimpleItemInput(string stableReference)
    {
        return $"SimpleItem - {StableSimpleItemDescription(stableReference)}";
    }

    private string StableUnusedInputProduct(string stableReference, int inputIndex)
    {
        return $"UnusedInput - 100.00% of {StableSimpleItemDescription(stableReference)} ($i{inputIndex})";
    }

	private MudSharp.Models.Craft? AddAntiquityCraft(
        string name,
        string category,
        string blurb,
        string action,
        string itemDescription,
        string knowledgeName,
        string traitName,
        int? minimumTraitValue,
        Difficulty difficulty,
        IEnumerable<(int Seconds, string Echo, string FailEcho)> phases,
        IEnumerable<string> inputs,
        IEnumerable<string> tools,
        IEnumerable<string> products,
        IEnumerable<string>? failProducts = null,
        string knowledgeSubtype = "General",
		string? knowledgeDescription = null,
		string? knowledgeLongDescription = null)
	{
		var phaseList = phases.ToList();
		return AddCraft(
            name,
            category,
            blurb,
            action,
            itemDescription,
            knowledgeName,
            traitName,
            minimumTraitValue,
            difficulty,
            Outcome.MinorFail,
            5,
			Math.Min(3, phaseList.Count),
			false,
			phaseList,
            inputs,
            tools,
            products,
            failProducts ?? [],
            knowledgeSubtype: knowledgeSubtype,
            knowledgeDescription: knowledgeDescription,
            knowledgeLongDescription: knowledgeLongDescription);
    }

    private sealed record AntiquityCultureGarmentCraftSpec(
        string StableReference,
        string Name,
        string DisplayName,
        string Material,
        int Cloth,
        int Yarn,
        bool Fine,
        int Minimum,
        Difficulty Difficulty,
        bool GeneratedProduct = false,
        int SecondaryCloth = 0,
        int Hair = 0);

    private const string AntiquityLeatherKnowledge = "Ancient Hide and Leatherworking";

    private const string AntiquityLeatherKnowledgeDescription =
        "Shared ancient hideworking knowledge for scraping, dehairing, tanning, cutting leather stock, and assembling leather clothing, armour, containers, and camp fittings.";

    private sealed record AntiquityLeatherClothingCraftSpec(
        string StableReference,
        string Name,
        string DisplayName,
        int Leather,
        int Soles,
        int Straps,
        int Thongs,
        int LinenYarn,
        bool VariableProduct,
        int Minimum,
        Difficulty Difficulty,
        bool Footwear = true);

    private sealed record AntiquityLeatherArmourCraftSpec(
        string StableReference,
        string Name,
        string DisplayName,
        int Panels,
        int HardenedPanels,
        int Scales,
        int Soles,
        int Straps,
        int Thongs,
        int LinenYarn,
        int Hair,
        int Metal,
        string MetalMaterial,
        bool VariableProduct,
        int Minimum,
        Difficulty Difficulty,
        bool Footwear = false,
        bool FineProduct = false);

    private sealed record AntiquityLeatherContainerCraftSpec(
        string StableReference,
        string Name,
        string DisplayName,
        int Panels,
        int SealedPanels,
        int Straps,
        int Thongs,
        int LinenYarn,
        int Hair,
        int Beeswax,
        int Metal,
        string MetalMaterial,
        bool Liquid,
        int Minimum,
        Difficulty Difficulty,
        bool VariableProduct = false);
}
