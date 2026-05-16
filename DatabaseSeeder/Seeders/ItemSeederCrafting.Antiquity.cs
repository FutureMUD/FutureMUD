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
            !ScythianSarmatianAntiquityClothingStableReferences.TryGetValue(stableReference, out shortDescription))
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

    private string StableVariableProduct(string stableReference, bool fineColour)
    {
        var item = LookupReworkItem(stableReference);
        return fineColour
            ? $"SimpleVariableProduct - 1x {item.ShortDescription} (#{item.Id}); variable Colour=$i1, Fine Colour=$i1"
            : $"SimpleVariableProduct - 1x {item.ShortDescription} (#{item.Id}); variable Colour=$i1";
    }

    private string StableSimpleProduct(string stableReference)
    {
        var item = LookupReworkItem(stableReference);
        return $"SimpleProduct - 1x {item.ShortDescription} (#{item.Id})";
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

    private void SeedAntiquityCultureGarmentCrafts(
        IReadOnlyDictionary<string, string> stableReferences,
        string knowledge,
        string knowledgeSubtype,
        string knowledgeDescription,
        IEnumerable<AntiquityCultureGarmentCraftSpec> garments)
    {
        var missingGarments = stableReferences.Keys
            .Where(x => !TryLookupReworkItem(x, out _))
            .ToList();
        if (missingGarments.Count > 0)
        {
            return;
        }

        void AddCultureGarmentCraft(string name, string blurb, string action, string itemDescription,
            int minimumTraitValue, Difficulty difficulty,
            IEnumerable<(int Seconds, string Echo, string FailEcho)> phases, IEnumerable<string> inputs,
            IEnumerable<string> tools, IEnumerable<string> products)
        {
            AddCraft(
                name,
                "Tailoring",
                blurb,
                action,
                itemDescription,
                knowledge,
                "Tailoring",
                minimumTraitValue,
                difficulty,
                Outcome.MinorFail,
                5,
                3,
                false,
                phases,
                inputs,
                tools,
                products,
                [],
                knowledgeSubtype: knowledgeSubtype,
                knowledgeDescription: knowledgeDescription,
                knowledgeLongDescription: knowledgeDescription);
        }

        var assemblyPhases = new (int Seconds, string Echo, string FailEcho)[]
        {
            (35, "$0 lay|lays out $i1 and mark|marks the garment panels, folds, or drape lines.", "$0 lay|lays out $i1, but mark|marks the garment unevenly."),
            (35, "$0 cut|cuts the cloth with $t2 and turn|turns the edges into narrow hems.", "$0 cut|cuts with $t2, but the hems pull crookedly."),
            (45, "$0 stitch|stitches seams, hems, folds, and fastening points with $t1 and $i2.", "$0 stitch|stitches with $t1, but the joins sit awkwardly and weakly."),
            (30, "$0 shake|shakes out $p1, checking its drape and finished line.", "$0 shake|shakes out only $f1 after the cloth is spoiled.")
        };
        var patternedAssemblyPhases = new (int Seconds, string Echo, string FailEcho)[]
        {
            (35, "$0 lay|lays out $i1 and $i3, matching the coloured cloth lengths and pattern lines.", "$0 lay|lays out $i1 and $i3, but the colours and pattern lines do not align."),
            (35, "$0 cut|cuts the cloth with $t2 and square|squares the visible edges.", "$0 cut|cuts with $t2, but the patterned edges pull crookedly."),
            (45, "$0 stitch|stitches the panels and decorative bands with $t1 and $i2.", "$0 stitch|stitches with $t1, but the bands bunch and weaken."),
            (30, "$0 lift|lifts $p1 to check its patterned fall.", "$0 lift|lifts only $f1 after the patterned cloth is spoiled.")
        };
        var furTrimmedAssemblyPhases = new (int Seconds, string Echo, string FailEcho)[]
        {
            (35, "$0 lay|lays out $i1 and trim|trims the fur stock from $i3.", "$0 lay|lays out $i1 and $i3, but cut|cuts the trim unevenly."),
            (40, "$0 cut|cuts the cloth with $t2 and baste|bastes the fur along the exposed edges.", "$0 cut|cuts with $t2, but the trim and cloth do not sit together cleanly."),
            (50, "$0 stitch|stitches the fur trim to the garment with $t1 and $i2.", "$0 stitch|stitches with $t1, but the fur trim bunches and pulls loose."),
            (30, "$0 lift|lifts $p1, checking its warmth and finished trim.", "$0 lift|lifts only $f1 after the trimming work fails.")
        };
        var sewingTools = new[]
        {
            "TagTool - Held - an item with the Sewing Needle tag",
            "TagTool - Held - an item with the Shears tag"
        };

        foreach (var garment in garments)
        {
            var characteristicRequirements = garment.Fine
                ? "characteristic Colour any; characteristic Fine Colour any"
                : "characteristic Colour any";
            var inputs = new List<string>
            {
                $"Commodity - {garment.Cloth} grams of {garment.Material}; piletag Garment Cloth; {characteristicRequirements}",
                $"Commodity - {garment.Yarn} grams of {garment.Material}; piletag Spun Yarn; characteristic Colour any"
            };

            if (garment.SecondaryCloth > 0)
            {
                inputs.Add($"Commodity - {garment.SecondaryCloth} grams of {garment.Material}; piletag Garment Cloth; {characteristicRequirements}");
            }

            if (garment.Hair > 0)
            {
                inputs.Add($"CommodityTag - {garment.Hair} grams of a material tagged as Hair");
            }

            AddCultureGarmentCraft(
                garment.Name,
                $"assemble {garment.DisplayName} from {garment.Material} garment cloth",
                garment.Name,
                $"{garment.Material} cloth being assembled into {knowledgeSubtype} clothing",
                garment.Minimum,
                garment.Difficulty,
                garment.Hair > 0 ? furTrimmedAssemblyPhases : garment.GeneratedProduct ? patternedAssemblyPhases : assemblyPhases,
                inputs,
                sewingTools,
                [garment.GeneratedProduct ? StableSimpleProduct(garment.StableReference) : StableVariableProduct(garment.StableReference, garment.Fine)]);
        }
    }

    private void SeedAntiquityRomanClothingCrafts()
    {
        if (!ShouldSeedAntiquityCrafts())
        {
            return;
        }

        const string romanKnowledge = "Roman Textilecraft";
        const string romanKnowledgeDescription =
            "Roman garment knowledge for assembling tunicae, togae, pallae, stolae, and practical woollen mantles.";

        var missingGarments = RomanAntiquityClothingStableReferences.Keys
            .Where(x => !TryLookupReworkItem(x, out _))
            .ToList();
        if (missingGarments.Count > 0)
        {
            return;
        }

        void AddRomanGarmentCraft(string name, string blurb, string action, string itemDescription,
            int minimumTraitValue, Difficulty difficulty,
            IEnumerable<(int Seconds, string Echo, string FailEcho)> phases, IEnumerable<string> inputs,
            IEnumerable<string> tools, IEnumerable<string> products)
        {
            AddCraft(
                name,
                "Tailoring",
                blurb,
                action,
                itemDescription,
                romanKnowledge,
                "Tailoring",
                minimumTraitValue,
                difficulty,
                Outcome.MinorFail,
                5,
                3,
                false,
                phases,
                inputs,
                tools,
                products,
                [],
                knowledgeSubtype: "Roman",
                knowledgeDescription: romanKnowledgeDescription,
                knowledgeLongDescription: romanKnowledgeDescription);
        }

        var romanAssemblyPhases = new (int Seconds, string Echo, string FailEcho)[]
        {
            (35, "$0 lay|lays out $i1 and mark|marks the Roman garment dimensions and drape lines.", "$0 lay|lays out $i1, but mark|marks the garment dimensions unevenly."),
            (35, "$0 cut|cuts the cloth with $t2 and shape|shapes the edges, openings, or rounded fall required by the garment.", "$0 cut|cuts with $t2, but the edges and openings sit awkwardly."),
            (40, "$0 thread|threads $t1 and finish|finishes hems, straps, joins, or weighted fold points as needed.", "$0 thread|threads $t1, but the hems and joins pull out of line."),
            (30, "$0 arrange|arranges $p1, checking the weight and drape.", "$0 arrange|arranges only $f1 after the cloth is spoiled.")
        };
        var sewingTools = new[]
        {
            "TagTool - Held - an item with the Sewing Needle tag",
            "TagTool - Held - an item with the Shears tag"
        };

        foreach (var garment in new[]
        {
            (StableReference: "antiquity_knee_length_wool_tunica", Name: "assemble a Roman knee-length wool tunica", DisplayName: "a Roman knee-length wool tunica", Material: "wool", Cloth: 540, Yarn: 25, Fine: false, Minimum: 15, Difficulty: Difficulty.Easy),
            (StableReference: "antiquity_wool_travel_mantle", Name: "assemble a Roman wool travel mantle", DisplayName: "a Roman wool travel mantle", Material: "wool", Cloth: 920, Yarn: 35, Fine: false, Minimum: 15, Difficulty: Difficulty.Normal),
            (StableReference: "antiquity_fine_linen_tunica", Name: "assemble a Roman fine linen tunica", DisplayName: "a Roman fine linen tunica", Material: "linen", Cloth: 360, Yarn: 25, Fine: true, Minimum: 35, Difficulty: Difficulty.Normal),
            (StableReference: "antiquity_wool_toga", Name: "assemble a Roman wool toga", DisplayName: "a Roman wool toga", Material: "wool", Cloth: 2800, Yarn: 90, Fine: true, Minimum: 40, Difficulty: Difficulty.Hard),
            (StableReference: "antiquity_long_wool_tunica", Name: "assemble a Roman long wool tunica", DisplayName: "a Roman long wool tunica", Material: "wool", Cloth: 650, Yarn: 30, Fine: false, Minimum: 20, Difficulty: Difficulty.Normal),
            (StableReference: "antiquity_wool_palla", Name: "assemble a Roman wool palla", DisplayName: "a Roman wool palla", Material: "wool", Cloth: 870, Yarn: 35, Fine: false, Minimum: 20, Difficulty: Difficulty.Normal),
            (StableReference: "antiquity_fine_long_linen_tunica", Name: "assemble a Roman fine long linen tunica", DisplayName: "a Roman fine long linen tunica", Material: "linen", Cloth: 400, Yarn: 30, Fine: true, Minimum: 40, Difficulty: Difficulty.Hard),
            (StableReference: "antiquity_wool_stola", Name: "assemble a Roman wool stola", DisplayName: "a Roman wool stola", Material: "wool", Cloth: 860, Yarn: 40, Fine: true, Minimum: 40, Difficulty: Difficulty.Hard),
            (StableReference: "antiquity_fine_wool_palla", Name: "assemble a Roman fine wool palla", DisplayName: "a Roman fine wool palla", Material: "wool", Cloth: 820, Yarn: 35, Fine: true, Minimum: 40, Difficulty: Difficulty.Hard)
        })
        {
            var characteristicRequirements = garment.Fine
                ? "characteristic Colour any; characteristic Fine Colour any"
                : "characteristic Colour any";
            AddRomanGarmentCraft(
                garment.Name,
                $"assemble {garment.DisplayName} from {garment.Material} garment cloth",
                garment.Name,
                $"{garment.Material} cloth being assembled into Roman clothing",
                garment.Minimum,
                garment.Difficulty,
                romanAssemblyPhases,
                [
                    $"Commodity - {garment.Cloth} grams of {garment.Material}; piletag Garment Cloth; {characteristicRequirements}",
                    $"Commodity - {garment.Yarn} grams of {garment.Material}; piletag Spun Yarn; characteristic Colour any"
                ],
                sewingTools,
                [StableVariableProduct(garment.StableReference, garment.Fine)]);
        }
    }

    private void SeedAntiquityPunicClothingCrafts()
    {
        if (!ShouldSeedAntiquityCrafts())
        {
            return;
        }

        const string punicKnowledge = "Punic Textilecraft";
        const string punicKnowledgeDescription =
            "Punic and Phoenician garment knowledge for assembling fitted linen tunics, waistcloths, overblouses, folded robes, hoods, mantles, gowns, overdrapes, and star-bordered linen robes.";

        SeedAntiquityCultureGarmentCrafts(
            PunicAntiquityClothingStableReferences,
            punicKnowledge,
            "Punic",
            punicKnowledgeDescription,
            [
                new("antiquity_short_fitted_linen_tunic", "assemble a Punic short fitted linen tunic", "a Punic short fitted linen tunic", "linen", 340, 20, false, 15, Difficulty.Normal),
                new("antiquity_patterned_linen_waistcloth", "assemble a Punic patterned linen waistcloth", "a Punic patterned linen waistcloth", "linen", 220, 15, true, 30, Difficulty.Normal),
                new("antiquity_short_sleeved_linen_overblouse", "assemble a Punic short sleeved linen overblouse", "a Punic short sleeved linen overblouse", "linen", 300, 20, true, 35, Difficulty.Normal),
                new("antiquity_long_linen_inner_robe", "assemble a Punic long linen inner robe", "a Punic long linen inner robe", "linen", 670, 30, false, 20, Difficulty.Normal),
                new("antiquity_one_shoulder_wool_mantle", "assemble a Punic one shoulder wool mantle", "a Punic one shoulder wool mantle", "wool", 940, 35, false, 20, Difficulty.Normal),
                new("antiquity_long_folded_linen_robe", "assemble a Punic long folded linen robe", "a Punic long folded linen robe", "linen", 745, 35, false, 25, Difficulty.Normal),
                new("antiquity_loose_linen_hood", "assemble a Punic loose linen hood", "a Punic loose linen hood", "linen", 125, 10, false, 15, Difficulty.Easy),
                new("antiquity_fine_full_linen_gown", "assemble a Punic fine full linen gown", "a Punic fine full linen gown", "linen", 820, 40, true, 40, Difficulty.Hard),
                new("antiquity_left_shoulder_overdrape", "assemble a Punic left shoulder overdrape", "a Punic left shoulder overdrape", "linen", 280, 20, true, 35, Difficulty.Normal),
                new("antiquity_star_bordered_linen_robe", "assemble a Punic star bordered linen robe", "a Punic star bordered linen robe", "linen", 780, 40, true, 40, Difficulty.Hard)
            ]);
    }

    private void SeedAntiquityPersianClothingCrafts()
    {
        if (!ShouldSeedAntiquityCrafts())
        {
            return;
        }

        const string persianKnowledge = "Persian Textilecraft";
        const string persianKnowledgeDescription =
            "Persian and Median garment knowledge for assembling sarapis tunics, anaxyrides trousers, kandyes, pleated court robes and gowns, cloth belts, and head-and-neck veils.";

        SeedAntiquityCultureGarmentCrafts(
            PersianAntiquityClothingStableReferences,
            persianKnowledge,
            "Persian",
            persianKnowledgeDescription,
            [
                new("antiquity_sarapis_wool_tunic", "assemble a Persian wool sarapis", "a Persian wool sarapis", "wool", 690, 35, false, 20, Difficulty.Normal),
                new("antiquity_fine_sarapis_linen_tunic", "assemble a Persian fine linen sarapis", "a Persian fine linen sarapis", "linen", 495, 30, true, 35, Difficulty.Normal),
                new("antiquity_anaxyrides_wool_trousers", "assemble Persian wool anaxyrides", "Persian wool anaxyrides", "wool", 590, 30, false, 20, Difficulty.Normal),
                new("antiquity_fine_patterned_anaxyrides", "assemble Persian fine patterned anaxyrides", "Persian fine patterned anaxyrides", "wool", 440, 35, true, 40, Difficulty.Hard, true, 120),
                new("antiquity_wool_kandys", "assemble a Persian wool kandys", "a Persian wool kandys", "wool", 1290, 50, false, 25, Difficulty.Hard),
                new("antiquity_fine_wool_kandys", "assemble a Persian fine wool kandys", "a Persian fine wool kandys", "wool", 1190, 50, true, 45, Difficulty.Hard),
                new("antiquity_pleated_court_robe", "assemble a Persian pleated court robe", "a Persian pleated court robe", "linen", 930, 50, true, 45, Difficulty.Hard),
                new("antiquity_fine_pleated_court_gown", "assemble a Persian fine pleated court gown", "a Persian fine pleated court gown", "linen", 835, 45, true, 45, Difficulty.Hard),
                new("antiquity_wide_cloth_belt", "assemble a Persian wide cloth belt", "a Persian wide cloth belt", "linen", 150, 10, false, 15, Difficulty.Easy),
                new("antiquity_fine_wide_cloth_belt", "assemble a Persian fine wide cloth belt", "a Persian fine wide cloth belt", "linen", 135, 10, true, 30, Difficulty.Normal),
                new("antiquity_full_head_and_neck_veil", "assemble a Persian full head and neck veil", "a Persian full head and neck veil", "linen", 175, 15, true, 35, Difficulty.Normal)
            ]);
    }

    private void SeedAntiquityEtruscanClothingCrafts()
    {
        if (!ShouldSeedAntiquityCrafts())
        {
            return;
        }

        const string etruscanKnowledge = "Etruscan Textilecraft";
        const string etruscanKnowledgeDescription =
            "Etruscan garment knowledge for assembling short-sleeved linen tunics, bordered wool tunics, curved tebennas, wrapped skirts, shoulder cloaks, fitted gowns, and linen head mantles.";

        SeedAntiquityCultureGarmentCrafts(
            EtruscanAntiquityClothingStableReferences,
            etruscanKnowledge,
            "Etruscan",
            etruscanKnowledgeDescription,
            [
                new("adjacent_antiquity_short_sleeved_linen_tunic", "assemble an Etruscan short sleeved linen tunic", "an Etruscan short sleeved linen tunic", "linen", 395, 25, false, 15, Difficulty.Normal),
                new("adjacent_antiquity_bordered_wool_tunic", "assemble an Etruscan bordered wool tunic", "an Etruscan bordered wool tunic", "wool", 530, 30, true, 35, Difficulty.Normal),
                new("adjacent_antiquity_curved_tebenna", "assemble an Etruscan curved wool tebenna", "an Etruscan curved wool tebenna", "wool", 860, 35, false, 25, Difficulty.Normal),
                new("adjacent_antiquity_fine_curved_tebenna", "assemble an Etruscan fine curved wool tebenna", "an Etruscan fine curved wool tebenna", "wool", 780, 35, true, 40, Difficulty.Hard),
                new("adjacent_antiquity_wrapped_linen_skirt", "assemble an Etruscan wrapped linen skirt", "an Etruscan wrapped linen skirt", "linen", 340, 20, false, 15, Difficulty.Normal),
                new("adjacent_antiquity_rectangular_shoulder_cloak", "assemble an Etruscan rectangular shoulder cloak", "an Etruscan rectangular shoulder cloak", "wool", 740, 35, false, 20, Difficulty.Normal),
                new("adjacent_antiquity_fitted_linen_gown", "assemble an Etruscan fitted linen gown", "an Etruscan fitted linen gown", "linen", 590, 35, true, 40, Difficulty.Hard),
                new("adjacent_antiquity_linen_head_mantle", "assemble an Etruscan linen head mantle", "an Etruscan linen head mantle", "linen", 165, 15, true, 30, Difficulty.Normal)
            ]);
    }

    private void SeedAntiquityAnatolianClothingCrafts()
    {
        if (!ShouldSeedAntiquityCrafts())
        {
            return;
        }

        const string anatolianKnowledge = "Anatolian Textilecraft";
        const string anatolianKnowledgeDescription =
            "Anatolian garment knowledge for assembling belted wool tunics, banded tunics, leg wraps, hooded cloaks, forward-pointing felt caps, wool capes, patterned robes, fringed mantles, wrapped skirts, and rectangular veils.";

        SeedAntiquityCultureGarmentCrafts(
            AnatolianAntiquityClothingStableReferences,
            anatolianKnowledge,
            "Anatolian",
            anatolianKnowledgeDescription,
            [
                new("adjacent_antiquity_belted_wool_tunic", "assemble an Anatolian belted wool tunic", "an Anatolian belted wool tunic", "wool", 590, 30, false, 20, Difficulty.Normal),
                new("adjacent_antiquity_fine_banded_tunic", "assemble an Anatolian fine banded wool tunic", "an Anatolian fine banded wool tunic", "wool", 450, 35, true, 40, Difficulty.Hard, true, 130),
                new("adjacent_antiquity_banded_leg_wraps", "assemble Anatolian banded wool leg wraps", "Anatolian banded wool leg wraps", "wool", 160, 10, false, 15, Difficulty.Easy),
                new("adjacent_antiquity_hooded_wool_cloak", "assemble an Anatolian hooded wool cloak", "an Anatolian hooded wool cloak", "wool", 1000, 45, false, 25, Difficulty.Hard),
                new("adjacent_antiquity_forward_pointing_felt_cap", "assemble an Anatolian forward pointing felt cap", "an Anatolian forward pointing felt cap", "felt", 105, 8, false, 20, Difficulty.Normal),
                new("adjacent_antiquity_short_wool_cape", "assemble an Anatolian short wool cape", "an Anatolian short wool cape", "wool", 495, 25, false, 20, Difficulty.Normal),
                new("adjacent_antiquity_fine_patterned_wool_robe", "assemble an Anatolian fine patterned wool robe", "an Anatolian fine patterned wool robe", "wool", 855, 45, true, 45, Difficulty.Hard),
                new("adjacent_antiquity_fringed_wool_mantle", "assemble an Anatolian fringed wool mantle", "an Anatolian fringed wool mantle", "wool", 720, 40, true, 40, Difficulty.Hard),
                new("adjacent_antiquity_wool_wrapped_skirt", "assemble an Anatolian wrapped wool skirt", "an Anatolian wrapped wool skirt", "wool", 515, 25, false, 20, Difficulty.Normal),
                new("adjacent_antiquity_fine_rectangular_veil", "assemble an Anatolian fine rectangular linen veil", "an Anatolian fine rectangular linen veil", "linen", 110, 10, true, 30, Difficulty.Normal)
            ]);
    }

    private void SeedAntiquityScythianSarmatianClothingCrafts()
    {
        if (!ShouldSeedAntiquityCrafts())
        {
            return;
        }

        const string scythianKnowledge = "Scythian-Sarmatian Textilecraft";
        const string scythianKnowledgeDescription =
            "Scythian and Sarmatian garment knowledge for assembling felt riding caps, riding tunics and trousers, open caftans, fur-trimmed caftans, split riding skirts, and long felt coats.";

        SeedAntiquityCultureGarmentCrafts(
            ScythianSarmatianAntiquityClothingStableReferences,
            scythianKnowledge,
            "Scythian-Sarmatian",
            scythianKnowledgeDescription,
            [
                new("adjacent_antiquity_felt_riding_cap", "assemble a Scythian felt riding cap", "a Scythian felt riding cap", "felt", 90, 8, false, 20, Difficulty.Normal),
                new("adjacent_antiquity_tall_felt_cap", "assemble a Scythian tall felt cap", "a Scythian tall felt cap", "felt", 130, 10, true, 35, Difficulty.Hard),
                new("adjacent_antiquity_riding_tunic", "assemble a Scythian riding tunic", "a Scythian riding tunic", "wool", 645, 35, false, 20, Difficulty.Normal),
                new("adjacent_antiquity_wool_riding_trousers", "assemble Scythian wool riding trousers", "Scythian wool riding trousers", "wool", 495, 25, false, 20, Difficulty.Normal),
                new("adjacent_antiquity_patterned_riding_trousers", "assemble Scythian patterned riding trousers", "Scythian patterned riding trousers", "wool", 470, 30, true, 35, Difficulty.Hard),
                new("adjacent_antiquity_open_riding_caftan", "assemble a Scythian open riding caftan", "a Scythian open riding caftan", "wool", 820, 40, false, 25, Difficulty.Hard),
                new("adjacent_antiquity_fur_trimmed_caftan", "assemble a Scythian fur trimmed caftan", "a Scythian fur trimmed caftan", "wool", 1060, 45, true, 45, Difficulty.Hard, Hair: 140),
                new("adjacent_antiquity_split_riding_skirt", "assemble a Scythian split riding skirt", "a Scythian split riding skirt", "wool", 590, 30, false, 20, Difficulty.Normal),
                new("adjacent_antiquity_long_felt_coat", "assemble a Scythian long felt coat", "a Scythian long felt coat", "felt", 1270, 50, true, 45, Difficulty.Hard)
            ]);
    }
    
    private void SeedAntiquityHellenicClothingCrafts()
    {
        if (!ShouldSeedAntiquityCrafts())
        {
            return;
        }

        const string ancientKnowledge = "Ancient Textile Production";
        const string hellenicKnowledge = "Hellenic Textilecraft";
        const string ancientKnowledgeDescription =
            "Shared ancient techniques for preparing textile fibres, spinning yarn, weaving cloth, dyeing, and fulling.";
        const string hellenicKnowledgeDescription =
            "Hellenic garment knowledge for assembling rectangular wool and linen dress such as chitons, peploi, chlamydes, himatia, and veils.";

        void AddAncientTextileCraft(string name, string category, string blurb, string action, string itemDescription,
            string traitName, int minimumTraitValue, Difficulty difficulty,
            IEnumerable<(int Seconds, string Echo, string FailEcho)> phases, IEnumerable<string> inputs,
            IEnumerable<string> tools, IEnumerable<string> products, IEnumerable<string>? failProducts = null)
        {
            AddCraft(
                name,
                category,
                blurb,
                action,
                itemDescription,
                ancientKnowledge,
                traitName,
                minimumTraitValue,
                difficulty,
                Outcome.MinorFail,
                5,
                3,
                false,
                phases,
                inputs,
                tools,
                products,
                failProducts ?? [],
                knowledgeSubtype: "Textiles",
                knowledgeDescription: ancientKnowledgeDescription,
                knowledgeLongDescription: ancientKnowledgeDescription);
        }

        void AddHellenicGarmentCraft(string name, string blurb, string action, string itemDescription,
            int minimumTraitValue, Difficulty difficulty,
            IEnumerable<(int Seconds, string Echo, string FailEcho)> phases, IEnumerable<string> inputs,
            IEnumerable<string> tools, IEnumerable<string> products)
        {
            AddCraft(
                name,
                "Tailoring",
                blurb,
                action,
                itemDescription,
                hellenicKnowledge,
                "Tailoring",
                minimumTraitValue,
                difficulty,
                Outcome.MinorFail,
                5,
                3,
                false,
                phases,
                inputs,
                tools,
                products,
                [],
                knowledgeSubtype: "Hellenic",
                knowledgeDescription: hellenicKnowledgeDescription,
                knowledgeLongDescription: hellenicKnowledgeDescription);
        }

        AddAncientTextileCraft(
            "sort flax stalks for retting",
            "Farming",
            "sort flax stalks into rettable fibre bundles",
            "sorting flax stalks",
            "flax stalks being sorted for retting",
            "Farming",
            1,
            Difficulty.Easy,
            [
                (25, "$0 sort|sorts through $i1, shaking away dirt and seed heads.", "$0 sort|sorts through $i1, but leave|leaves the bundles uneven and contaminated."),
                (25, "$0 bind|binds the usable stalks into loose, rettable bundles.", "$0 bind|binds the stalks too tightly, spoiling much of the fibre."),
                (20, "$0 set|sets aside $p1 for soaking.", "$0 salvage|salvages only $f1 from the poorly prepared flax.")
            ],
            ["Commodity - 1 kilogram 800 grams of flax"],
            [],
            ["CommodityProduct - 1 kilogram 600 grams of flax commodity; tag Raw Textile Fibre"],
            ["CommodityProduct - 800 grams of flax commodity; tag Raw Textile Fibre"]);

        AddAncientTextileCraft(
            "ret break and hackle flax fibre",
            "Weaving",
            "ret, break, and hackle raw flax into prepared linen fibre",
            "retting and hackling flax",
            "flax being retted, broken, and hackled",
            "Weaving",
            1,
            Difficulty.Normal,
            [
                (35, "$0 soak|soaks $i1 in $t1, turning the bundles through clean water.", "$0 soak|soaks $i1 unevenly in $t1."),
                (35, "$0 work|works the retted stalks through $t2 to crack away the woody core.", "$0 overwork|overworks the stalks through $t2 and shorten|shortens the fibre."),
                (35, "$0 draw|draws the fibre repeatedly through $t3, combing it into clean stricks.", "$0 draw|draws the fibre poorly through $t3, leaving tangles and shive behind."),
                (20, "$0 gather|gathers $p1 into neat prepared bundles.", "$0 gather|gathers only $f1 from the damaged flax.")
            ],
            [
                "Commodity - 1 kilogram 600 grams of flax; piletag Raw Textile Fibre",
                "LiquidUse - 10 litres of Water"
            ],
            [
                "TagTool - InRoom - an item with the Retting Trough tag",
                "TagTool - Held - an item with the Flax Break tag",
                "TagTool - Held - an item with the Hackle tag"
            ],
            [
                "CommodityProduct - 900 grams of linen commodity; tag Prepared Textile Fibre; characteristic Colour=white; characteristic Fine Colour=bone white"
            ],
            [
                "CommodityProduct - 350 grams of linen commodity; tag Prepared Textile Fibre; characteristic Colour=white; characteristic Fine Colour=bone white"
            ]);

        AddAncientTextileCraft(
            "sort raw wool fleece",
            "Farming",
            "sort raw fleece into spinnable wool bundles",
            "sorting raw wool fleece",
            "raw wool fleece being sorted",
            "Farming",
            1,
            Difficulty.Easy,
            [
                (25, "$0 pick|picks burrs and coarse matter from $i1.", "$0 miss|misses many burrs and coarse patches in $i1."),
                (25, "$0 divide|divides the better fleece from greasy waste and matted locks.", "$0 mix|mixes good fleece with matted waste."),
                (20, "$0 roll|rolls $p1 into loose raw-wool bundles.", "$0 salvage|salvages only $f1 from the badly sorted fleece.")
            ],
            ["Commodity - 1 kilogram 400 grams of wool"],
            ["TagTool - Held - an item with the Shears tag"],
            ["CommodityProduct - 1 kilogram 200 grams of wool commodity; tag Raw Textile Fibre"],
            ["CommodityProduct - 500 grams of wool commodity; tag Raw Textile Fibre"]);

        AddAncientTextileCraft(
            "scour and comb wool fleece",
            "Weaving",
            "scour and comb raw wool into prepared spinning fibre",
            "scouring and combing wool",
            "wool fleece being scoured and combed",
            "Weaving",
            1,
            Difficulty.Normal,
            [
                (30, "$0 wash|washes $i1 carefully, pressing grease and dirt out with clean water.", "$0 wash|washes $i1 too roughly, felting some of the locks."),
                (35, "$0 tease|teases the damp wool open by hand and begin|begins to comb it on $t1.", "$0 leave|leaves many locks tangled while combing on $t1."),
                (35, "$0 draw|draws the wool into aligned, airy bundles ready for spinning.", "$0 draw|draws only short and uneven bundles from the fleece."),
                (20, "$0 set|sets aside $p1.", "$0 set|sets aside only $f1.")
            ],
            [
                "Commodity - 1 kilogram 200 grams of wool; piletag Raw Textile Fibre",
                "LiquidUse - 6 litres of Water"
            ],
            ["TagTool - Held - an item with the Fibre Comb tag"],
            [
                "CommodityProduct - 900 grams of wool commodity; tag Prepared Textile Fibre; characteristic Colour=brown; characteristic Fine Colour=light brown"
            ],
            [
                "CommodityProduct - 350 grams of wool commodity; tag Prepared Textile Fibre; characteristic Colour=brown; characteristic Fine Colour=light brown"
            ]);

        AddAncientTextileCraft(
            "sort cotton bolls for ginning",
            "Farming",
            "sort cotton bolls into usable fibre stock",
            "sorting cotton bolls",
            "cotton bolls being sorted for ginning",
            "Farming",
            1,
            Difficulty.Easy,
            [
                (25, "$0 pick|picks clean cotton bolls from $i1 and shake|shakes away leaves and field grit.", "$0 pick|picks through $i1, but leave|leaves too much brittle stalk and dirt among the bolls."),
                (25, "$0 sort|sorts the better bolls into loose baskets for fibre cleaning.", "$0 sort|sorts the bolls poorly, mixing damaged fibre through the usable stock."),
                (20, "$0 gather|gathers $p1 for ginning and combing.", "$0 salvage|salvages only $f1 from the badly sorted cotton.")
            ],
            ["Commodity - 1 kilogram 200 grams of cotton crop"],
            [],
            ["CommodityProduct - 1 kilogram of cotton crop commodity; tag Raw Textile Fibre"],
            ["CommodityProduct - 400 grams of cotton crop commodity; tag Raw Textile Fibre"]);

        AddAncientTextileCraft(
            "clean and comb cotton fibre",
            "Weaving",
            "clean seed and comb raw cotton into prepared spinning fibre",
            "cleaning cotton fibre",
            "cotton fibre being cleaned and combed",
            "Weaving",
            1,
            Difficulty.Normal,
            [
                (30, "$0 open|opens $i1 by hand, pulling seed and husk from the fibre.", "$0 open|opens $i1 unevenly and leave|leaves seed fragments in the fibre."),
                (35, "$0 comb|combs the cotton with $t1 into light, workable rolls.", "$0 comb|combs the cotton too roughly, tearing much of the fibre into knots."),
                (35, "$0 draw|draws the clean fibre into soft prepared bundles.", "$0 draw|draws only patchy, lumpy bundles from the fibre."),
                (20, "$0 set|sets aside $p1.", "$0 set|sets aside only $f1.")
            ],
            ["Commodity - 1 kilogram of cotton crop; piletag Raw Textile Fibre"],
            ["TagTool - Held - an item with the Fibre Comb tag"],
            [
                "CommodityProduct - 600 grams of cotton commodity; tag Prepared Textile Fibre; characteristic Colour=white; characteristic Fine Colour=bone white"
            ],
            [
                "CommodityProduct - 220 grams of cotton commodity; tag Prepared Textile Fibre; characteristic Colour=white; characteristic Fine Colour=bone white"
            ]);

        AddAncientTextileCraft(
            "spin linen yarn on a drop spindle",
            "Spinning",
            "spin prepared linen fibre into yarn",
            "spinning linen yarn",
            "linen fibre being spun into yarn",
            "Spinning",
            1,
            Difficulty.Normal,
            [
                (30, "$0 dress|dresses $i1 onto $t2 and draw|draws out a fine leader.", "$0 dress|dresses $i1 unevenly onto $t2."),
                (45, "$0 spin|spins $t1, drafting the fibres into a steady thread.", "$0 spin|spins $t1 with an uneven draft, snapping the thread repeatedly."),
                (45, "$0 wind|winds the new yarn back onto $t1 and continue|continues drafting.", "$0 wind|winds lumpy, weak yarn back onto $t1."),
                (20, "$0 finish|finishes $p1.", "$0 salvage|salvages only $f1 from the uneven spinning.")
            ],
            ["Commodity - 500 grams of linen; piletag Prepared Textile Fibre; characteristic Colour any; characteristic Fine Colour any"],
            [
                "TagTool - Held - an item with the Drop Spindle tag",
                "TagTool - Held - an item with the Distaff tag"
            ],
            ["CommodityProduct - 450 grams of linen commodity; tag Spun Yarn; characteristic Colour from $i1; characteristic Fine Colour from $i1"],
            ["CommodityProduct - 150 grams of linen commodity; tag Spun Yarn; characteristic Colour from $i1; characteristic Fine Colour from $i1"]);

        AddAncientTextileCraft(
            "spin wool yarn on a drop spindle",
            "Spinning",
            "spin prepared wool fibre into yarn",
            "spinning wool yarn",
            "wool fibre being spun into yarn",
            "Spinning",
            1,
            Difficulty.Easy,
            [
                (30, "$0 arrange|arranges $i1 on $t2 and set|sets a twist with $t1.", "$0 arrange|arranges $i1 poorly on $t2."),
                (45, "$0 draft|drafts wool steadily as $t1 turns.", "$0 draft|drafts the wool unevenly as $t1 turns."),
                (45, "$0 wind|winds the growing yarn around $t1.", "$0 wind|winds a lumpy, weak thread around $t1."),
                (20, "$0 finish|finishes $p1.", "$0 salvage|salvages only $f1 from the uneven spinning.")
            ],
            ["Commodity - 500 grams of wool; piletag Prepared Textile Fibre; characteristic Colour any; characteristic Fine Colour any"],
            [
                "TagTool - Held - an item with the Drop Spindle tag",
                "TagTool - Held - an item with the Distaff tag"
            ],
            ["CommodityProduct - 460 grams of wool commodity; tag Spun Yarn; characteristic Colour from $i1; characteristic Fine Colour from $i1"],
            ["CommodityProduct - 180 grams of wool commodity; tag Spun Yarn; characteristic Colour from $i1; characteristic Fine Colour from $i1"]);

        AddAncientTextileCraft(
            "spin cotton yarn on a drop spindle",
            "Spinning",
            "spin prepared cotton fibre into yarn",
            "spinning cotton yarn",
            "cotton fibre being spun into yarn",
            "Spinning",
            1,
            Difficulty.Normal,
            [
                (30, "$0 dress|dresses $i1 onto $t2 and twist|twists a soft leader onto $t1.", "$0 dress|dresses $i1 unevenly onto $t2."),
                (45, "$0 spin|spins $t1 steadily, drafting the cotton into a fine thread.", "$0 spin|spins $t1 with a ragged draft, breaking the thread again and again."),
                (45, "$0 wind|winds the cotton yarn back onto $t1 and continue|continues drafting.", "$0 wind|winds weak, slubby yarn back onto $t1."),
                (20, "$0 finish|finishes $p1.", "$0 salvage|salvages only $f1 from the uneven spinning.")
            ],
            ["Commodity - 450 grams of cotton; piletag Prepared Textile Fibre; characteristic Colour any; characteristic Fine Colour any"],
            [
                "TagTool - Held - an item with the Drop Spindle tag",
                "TagTool - Held - an item with the Distaff tag"
            ],
            ["CommodityProduct - 400 grams of cotton commodity; tag Spun Yarn; characteristic Colour from $i1; characteristic Fine Colour from $i1"],
            ["CommodityProduct - 140 grams of cotton commodity; tag Spun Yarn; characteristic Colour from $i1; characteristic Fine Colour from $i1"]);

        AddAncientTextileCraft(
            "weave linen garment cloth on a warp-weighted loom",
            "Weaving",
            "weave linen yarn into garment cloth",
            "weaving linen garment cloth",
            "linen yarn being woven into garment cloth",
            "Weaving",
            10,
            Difficulty.Normal,
            [
                (45, "$0 hang|hangs the warp on $t1 and set|sets $t2 along the lower threads.", "$0 hang|hangs the warp unevenly on $t1."),
                (45, "$0 pass|passes $t3 through the shed and beat|beats the weft into place with $t4.", "$0 pass|passes $t3 poorly and beat|beats the weft out of line with $t4."),
                (60, "$0 continue|continues weaving an even rectangular length of cloth.", "$0 continue|continues weaving, but the edges wander and tighten."),
                (30, "$0 cut|cuts down $p1 from the loom.", "$0 cut|cuts down only $f1 from the flawed weaving.")
            ],
            ["Commodity - 520 grams of linen; piletag Spun Yarn; characteristic Colour any; characteristic Fine Colour any"],
            [
                "TagTool - InRoom - an item with the Warp-Weighted Loom tag",
                "TagTool - InRoom - an item with the Loom Weight tag",
                "TagTool - Held - an item with the Shuttle tag",
                "TagTool - Held - an item with the Weaver's Sword tag"
            ],
            ["CommodityProduct - 430 grams of linen commodity; tag Woven Cloth; characteristic Colour from $i1; characteristic Fine Colour from $i1"],
            ["CommodityProduct - 160 grams of linen commodity; tag Woven Cloth; characteristic Colour from $i1; characteristic Fine Colour from $i1"]);

        AddAncientTextileCraft(
            "weave wool garment cloth on a warp-weighted loom",
            "Weaving",
            "weave wool yarn into garment cloth",
            "weaving wool garment cloth",
            "wool yarn being woven into garment cloth",
            "Weaving",
            10,
            Difficulty.Normal,
            [
                (45, "$0 hang|hangs the wool warp on $t1 and weight|weights it with $t2.", "$0 hang|hangs the wool warp unevenly on $t1."),
                (45, "$0 throw|throws $t3 through the shed and beat|beats the weft with $t4.", "$0 throw|throws $t3 unevenly and beat|beats the weft out of line with $t4."),
                (60, "$0 continue|continues working the broad woollen rectangle.", "$0 continue|continues weaving, but the cloth grows uneven and sleazy."),
                (30, "$0 cut|cuts down $p1 from the loom.", "$0 cut|cuts down only $f1 from the flawed weaving.")
            ],
            ["Commodity - 1 kilogram of wool; piletag Spun Yarn; characteristic Colour any; characteristic Fine Colour any"],
            [
                "TagTool - InRoom - an item with the Warp-Weighted Loom tag",
                "TagTool - InRoom - an item with the Loom Weight tag",
                "TagTool - Held - an item with the Shuttle tag",
                "TagTool - Held - an item with the Weaver's Sword tag"
            ],
            ["CommodityProduct - 870 grams of wool commodity; tag Woven Cloth; characteristic Colour from $i1; characteristic Fine Colour from $i1"],
            ["CommodityProduct - 320 grams of wool commodity; tag Woven Cloth; characteristic Colour from $i1; characteristic Fine Colour from $i1"]);

        AddAncientTextileCraft(
            "weave cotton garment cloth on a warp-weighted loom",
            "Weaving",
            "weave cotton yarn into garment cloth",
            "weaving cotton garment cloth",
            "cotton yarn being woven into garment cloth",
            "Weaving",
            10,
            Difficulty.Normal,
            [
                (45, "$0 hang|hangs the cotton warp on $t1 and weights the threads with $t2.", "$0 hang|hangs the cotton warp unevenly on $t1."),
                (45, "$0 pass|passes $t3 through the shed and beat|beats the weft into place with $t4.", "$0 pass|passes $t3 poorly and beat|beats the weft out of line with $t4."),
                (60, "$0 continue|continues weaving a light rectangular cotton length.", "$0 continue|continues weaving, but the cloth tightens and pulls out of square."),
                (30, "$0 cut|cuts down $p1 from the loom.", "$0 cut|cuts down only $f1 from the flawed weaving.")
            ],
            ["Commodity - 460 grams of cotton; piletag Spun Yarn; characteristic Colour any; characteristic Fine Colour any"],
            [
                "TagTool - InRoom - an item with the Warp-Weighted Loom tag",
                "TagTool - InRoom - an item with the Loom Weight tag",
                "TagTool - Held - an item with the Shuttle tag",
                "TagTool - Held - an item with the Weaver's Sword tag"
            ],
            ["CommodityProduct - 380 grams of cotton commodity; tag Woven Cloth; characteristic Colour from $i1; characteristic Fine Colour from $i1"],
            ["CommodityProduct - 140 grams of cotton commodity; tag Woven Cloth; characteristic Colour from $i1; characteristic Fine Colour from $i1"]);

        void AddTextileDyeStockCraft(string name, string dyeMaterial, string basicColour, string fineColour,
            bool usesMordant = true, IEnumerable<string>? additionalInputs = null, IEnumerable<string>? additionalTools = null)
        {
            var inputs = new List<string>
            {
                $"Commodity - 320 grams of {dyeMaterial}",
                "LiquidUse - 4 litres of Water"
            };

            if (usesMordant)
            {
                inputs.Add("Commodity - 40 grams of alum mordant");
            }

            if (additionalInputs is not null)
            {
                inputs.AddRange(additionalInputs);
            }

            var tools = new List<string>
            {
                "TagTool - InRoom - an item with the Dye Vat tag",
                "TagTool - InRoom - an item with the Mordant Cauldron tag",
                "TagTool - Held - an item with the Dye Strainer tag"
            };

            if (additionalTools is not null)
            {
                tools.AddRange(additionalTools);
            }

            AddAncientTextileCraft(
                name,
                "Dyeing",
                $"prepare {fineColour} textile dye stock",
                $"preparing {fineColour} dye stock",
                $"{fineColour} dye stock being prepared",
                "Dyeing",
                10,
                Difficulty.Normal,
                [
                    (30, "$0 crush|crushes and sort|sorts $i1 for the dye bath.", "$0 crush|crushes $i1 poorly, wasting much of the colour-bearing material."),
                    (45, "$0 steep|steeps $i1 in $t1 and work|works the colour into solution.", "$0 steep|steeps $i1 unevenly, leaving the dye weak and muddy."),
                    (35, "$0 strain|strains the dyebath through $t3 and settle|settles the coloured stock.", "$0 strain|strains the dyebath poorly, leaving grit and spent fibre in it."),
                    (25, "$0 set|sets aside $p1.", "$0 salvage|salvages only $f1 from the failed dye stock.")
                ],
                inputs,
                tools,
                [
                    $"CommodityProduct - 220 grams of {dyeMaterial} commodity; tag Textile Dye Stock; characteristic Colour={basicColour}; characteristic Fine Colour={fineColour}"
                ],
                [
                    $"CommodityProduct - 80 grams of {dyeMaterial} commodity; tag Textile Dye Stock; characteristic Colour={basicColour}; characteristic Fine Colour={fineColour}"
                ]);
        }

        void AddDyeCraft(string name, string material, string dyeMaterial, string basicColour, string fineColour)
        {
            AddAncientTextileCraft(
                name,
                "Dyeing",
                $"dye {material} garment cloth {fineColour}",
                $"dyeing {material} cloth",
                $"{material} cloth being dyed",
                "Dyeing",
                10,
                Difficulty.Normal,
                [
                    (35, "$0 heat|heats water and mordant in $t2, stirring $i2 into the bath.", "$0 heat|heats the mordant bath unevenly in $t2."),
                    (35, "$0 steep|steeps $i2 in $t1, drawing colour into the dye bath.", "$0 steep|steeps $i2 poorly in $t1, leaving weak colour."),
                    (45, "$0 immerse|immerses $i1 in the dye bath, turning the cloth steadily.", "$0 immerse|immerses $i1 unevenly, leaving blotched patches."),
                    (25, "$0 lift|lifts out $p1 and set|sets it to drain.", "$0 lift|lifts out only $f1 from the spoiled dyeing.")
                ],
                [
                    $"Commodity - 420 grams of {material}; piletag Garment Cloth; characteristic Colour any; characteristic Fine Colour any",
                    $"Commodity - 100 grams of {dyeMaterial}; piletag Textile Dye Stock; characteristic Colour any; characteristic Fine Colour={fineColour}",
                    "Commodity - 40 grams of alum mordant",
                    "LiquidUse - 8 litres of Water"
                ],
                [
                    "TagTool - InRoom - an item with the Dye Vat tag",
                    "TagTool - InRoom - an item with the Mordant Cauldron tag"
                ],
                [
                    $"CommodityProduct - 390 grams of {material} commodity; tag Dyed Cloth; characteristic Colour={basicColour}; characteristic Fine Colour={fineColour}"
                ],
                [
                    $"CommodityProduct - 150 grams of {material} commodity; tag Dyed Cloth; characteristic Colour={basicColour}; characteristic Fine Colour={fineColour}"
                ]);
        }

        AddTextileDyeStockCraft("prepare madder red dye stock", "madder root", "red", "madder red");
        AddTextileDyeStockCraft("prepare kermes scarlet dye stock", "kermes grain", "red", "kermes scarlet");
        AddTextileDyeStockCraft("prepare lac crimson dye stock", "lac dye cake", "red", "lac crimson");
        AddTextileDyeStockCraft("prepare alkanet purple dye stock", "alkanet root", "purple", "alkanet purple");
        AddTextileDyeStockCraft("prepare orchil violet dye stock", "orchil lichen", "purple", "orchil violet");
        AddTextileDyeStockCraft("prepare tyrian purple dye stock", "murex purple dye", "purple", "tyrian purple",
            false);
        AddTextileDyeStockCraft("prepare indigo dye stock", "indigo dye cake", "dark blue", "deep indigo", false,
            ["Commodity - 100 grams of wood ash"],
            ["TagTool - Held - an item with the Indigo Beating Paddle tag"]);
        AddTextileDyeStockCraft("prepare woad blue dye stock", "woad leaves", "blue", "woad blue", false,
            ["Commodity - 100 grams of wood ash"],
            ["TagTool - Held - an item with the Indigo Beating Paddle tag"]);
        AddTextileDyeStockCraft("prepare weld golden dye stock", "weld", "yellow", "golden yellow");
        AddTextileDyeStockCraft("prepare saffron yellow dye stock", "saffron", "yellow", "saffron yellow");
        AddTextileDyeStockCraft("prepare pomegranate yellow dye stock", "pomegranate rind", "yellow",
            "pomegranate yellow");
        AddTextileDyeStockCraft("prepare henna orange dye stock", "henna leaf", "orange", "henna orange");
        AddTextileDyeStockCraft("prepare walnut brown dye stock", "walnut hull", "brown", "walnut brown", false);
        AddTextileDyeStockCraft("prepare oak-gall black dye stock", "oak gall", "black", "oak-gall black", false,
            ["Commodity - 50 grams of wrought iron"]);

        AddDyeCraft("dye wool cloth madder red", "wool", "madder root", "red", "madder red");
        AddDyeCraft("dye wool cloth kermes scarlet", "wool", "kermes grain", "red", "kermes scarlet");
        AddDyeCraft("dye wool cloth lac crimson", "wool", "lac dye cake", "red", "lac crimson");
        AddDyeCraft("dye wool cloth indigo blue", "wool", "indigo dye cake", "dark blue", "deep indigo");
        AddDyeCraft("dye wool cloth woad blue", "wool", "woad leaves", "blue", "woad blue");
        AddDyeCraft("dye wool cloth weld yellow", "wool", "weld", "yellow", "golden yellow");
        AddDyeCraft("dye wool cloth alkanet purple", "wool", "alkanet root", "purple", "alkanet purple");
        AddDyeCraft("dye wool cloth orchil violet", "wool", "orchil lichen", "purple", "orchil violet");
        AddDyeCraft("dye wool cloth tyrian purple", "wool", "murex purple dye", "purple", "tyrian purple");
        AddDyeCraft("dye wool cloth walnut brown", "wool", "walnut hull", "brown", "walnut brown");
        AddDyeCraft("dye wool cloth oak-gall black", "wool", "oak gall", "black", "oak-gall black");
        AddDyeCraft("dye linen cloth madder red", "linen", "madder root", "red", "madder red");
        AddDyeCraft("dye linen cloth indigo blue", "linen", "indigo dye cake", "dark blue", "deep indigo");
        AddDyeCraft("dye linen cloth woad blue", "linen", "woad leaves", "blue", "woad blue");
        AddDyeCraft("dye linen cloth weld yellow", "linen", "weld", "yellow", "golden yellow");
        AddDyeCraft("dye linen cloth saffron yellow", "linen", "saffron", "yellow", "saffron yellow");
        AddDyeCraft("dye linen cloth pomegranate yellow", "linen", "pomegranate rind", "yellow", "pomegranate yellow");
        AddDyeCraft("dye linen cloth henna orange", "linen", "henna leaf", "orange", "henna orange");
        AddDyeCraft("dye linen cloth oak-gall black", "linen", "oak gall", "black", "oak-gall black");
        AddDyeCraft("dye linen cloth tyrian purple", "linen", "murex purple dye", "purple", "tyrian purple");
        AddDyeCraft("dye cotton cloth madder red", "cotton", "madder root", "red", "madder red");
        AddDyeCraft("dye cotton cloth indigo blue", "cotton", "indigo dye cake", "dark blue", "deep indigo");
        AddDyeCraft("dye cotton cloth weld yellow", "cotton", "weld", "yellow", "golden yellow");
        AddDyeCraft("dye cotton cloth saffron yellow", "cotton", "saffron", "yellow", "saffron yellow");
        AddDyeCraft("dye cotton cloth pomegranate yellow", "cotton", "pomegranate rind", "yellow", "pomegranate yellow");
        AddDyeCraft("dye cotton cloth henna orange", "cotton", "henna leaf", "orange", "henna orange");
        AddDyeCraft("dye cotton cloth walnut brown", "cotton", "walnut hull", "brown", "walnut brown");
        AddDyeCraft("dye felt cloth madder red", "felt", "madder root", "red", "madder red");
        AddDyeCraft("dye felt cloth indigo blue", "felt", "indigo dye cake", "dark blue", "deep indigo");
        AddDyeCraft("dye felt cloth woad blue", "felt", "woad leaves", "blue", "woad blue");
        AddDyeCraft("dye felt cloth walnut brown", "felt", "walnut hull", "brown", "walnut brown");
        AddDyeCraft("dye felt cloth oak-gall black", "felt", "oak gall", "black", "oak-gall black");

        AddAncientTextileCraft(
            "full wool garment cloth",
            "Weaving",
            "full woven or dyed wool cloth for denser garments",
            "fulling wool cloth",
            "wool cloth being fulled",
            "Weaving",
            15,
            Difficulty.Normal,
            [
                (35, "$0 soak|soaks $i1 in $t1 and turn|turns the cloth through the water.", "$0 soak|soaks $i1 unevenly in $t1."),
                (45, "$0 pound|pounds the wet wool with $t2 until the weave thickens.", "$0 pound|pounds the wool too harshly with $t2."),
                (45, "$0 stretch|stretches the cloth across $t3 to dry at a useful width.", "$0 stretch|stretches the cloth poorly across $t3."),
                (25, "$0 take|takes down $p1.", "$0 take|takes down only $f1 from the misshapen cloth.")
            ],
            [
                "Commodity - 900 grams of wool; piletag Garment Cloth; characteristic Colour any; characteristic Fine Colour any",
                "LiquidUse - 10 litres of Water"
            ],
            [
                "TagTool - InRoom - an item with the Fuller's Trough tag",
                "TagTool - Held - an item with the Fuller's Mallet tag",
                "TagTool - InRoom - an item with the Cloth Tenter Frame tag"
            ],
            ["CommodityProduct - 820 grams of wool commodity; tag Fulled Cloth; characteristic Colour from $i1; characteristic Fine Colour from $i1"],
            ["CommodityProduct - 300 grams of wool commodity; tag Fulled Cloth; characteristic Colour from $i1; characteristic Fine Colour from $i1"]);

        AddAncientTextileCraft(
            "felt prepared wool fibre into garment felt",
            "Weaving",
            "felt prepared wool fibre into dense garment felt",
            "felting wool fibre",
            "prepared wool fibre being felted into garment felt",
            "Weaving",
            15,
            Difficulty.Normal,
            [
                (35, "$0 soak|soaks $i1 in $t1 and spread|spreads the fibre into even overlapping layers.", "$0 soak|soaks $i1 unevenly and leave|leaves thin patches in the fibre."),
                (45, "$0 pound|pounds and roll|rolls the wet wool with $t2 until the fibres mat together.", "$0 pound|pounds the wool too harshly with $t2, tearing weak patches through it."),
                (45, "$0 stretch|stretches the felt over $t3 to dry into usable garment sheets.", "$0 stretch|stretches the felt poorly across $t3."),
                (25, "$0 take|takes down $p1.", "$0 take|takes down only $f1 from the misshapen felt.")
            ],
            [
                "Commodity - 700 grams of wool; piletag Prepared Textile Fibre; characteristic Colour any; characteristic Fine Colour any",
                "LiquidUse - 8 litres of Water"
            ],
            [
                "TagTool - InRoom - an item with the Fuller's Trough tag",
                "TagTool - Held - an item with the Fuller's Mallet tag",
                "TagTool - InRoom - an item with the Cloth Tenter Frame tag"
            ],
            ["CommodityProduct - 520 grams of felt commodity; tag Fulled Cloth; characteristic Colour from $i1; characteristic Fine Colour from $i1"],
            ["CommodityProduct - 180 grams of felt commodity; tag Fulled Cloth; characteristic Colour from $i1; characteristic Fine Colour from $i1"]);

        var missingGarments = HellenicAntiquityClothingStableReferences.Keys
            .Where(x => !TryLookupReworkItem(x, out _))
            .ToList();
        if (missingGarments.Count > 0)
        {
            return;
        }

        var assemblyPhases = new (int Seconds, string Echo, string FailEcho)[]
        {
            (35, "$0 lay|lays out $i1 and measure|measures a rectangular garment length, trimming it with $t2.", "$0 lay|lays out $i1, but cut|cuts the rectangle unevenly with $t2."),
            (35, "$0 thread|threads $t1 and turn|turns the edges into a neat hem.", "$0 thread|threads $t1, but the hems pucker and wander."),
            (40, "$0 join|joins and reinforce|reinforces the folds, openings, or fastening points appropriate to the garment.", "$0 join|joins the cloth poorly, leaving weak folds and awkward openings."),
            (25, "$0 shake|shakes out $p1, checking the drape.", "$0 shake|shakes out only $f1 after the cloth is spoiled.")
        };
        var sewingTools = new[]
        {
            "TagTool - Held - an item with the Sewing Needle tag",
            "TagTool - Held - an item with the Shears tag"
        };

        foreach (var garment in new[]
        {
            (StableReference: "antiquity_short_wool_chiton", Name: "assemble a short wool chiton", Material: "wool", Cloth: 470, Yarn: 30, Fine: false, Minimum: 15, Difficulty: Difficulty.Easy),
            (StableReference: "antiquity_wool_himation", Name: "assemble a wool himation", Material: "wool", Cloth: 1000, Yarn: 40, Fine: false, Minimum: 15, Difficulty: Difficulty.Normal),
            (StableReference: "antiquity_fine_linen_chiton", Name: "assemble a fine linen chiton", Material: "linen", Cloth: 340, Yarn: 25, Fine: true, Minimum: 35, Difficulty: Difficulty.Normal),
            (StableReference: "antiquity_fine_wool_himation", Name: "assemble a fine wool himation", Material: "wool", Cloth: 900, Yarn: 35, Fine: true, Minimum: 35, Difficulty: Difficulty.Hard),
            (StableReference: "antiquity_short_wool_chlamys", Name: "assemble a short wool chlamys", Material: "wool", Cloth: 650, Yarn: 30, Fine: false, Minimum: 20, Difficulty: Difficulty.Normal),
            (StableReference: "antiquity_full_length_wool_peplos", Name: "assemble a full-length wool peplos", Material: "wool", Cloth: 780, Yarn: 35, Fine: false, Minimum: 20, Difficulty: Difficulty.Normal),
            (StableReference: "antiquity_full_wool_himation", Name: "assemble a full wool himation", Material: "wool", Cloth: 960, Yarn: 40, Fine: false, Minimum: 20, Difficulty: Difficulty.Normal),
            (StableReference: "antiquity_fine_long_linen_chiton", Name: "assemble a fine long linen chiton", Material: "linen", Cloth: 410, Yarn: 30, Fine: true, Minimum: 40, Difficulty: Difficulty.Hard),
            (StableReference: "antiquity_fine_full_wool_himation", Name: "assemble a fine full wool himation", Material: "wool", Cloth: 880, Yarn: 35, Fine: true, Minimum: 40, Difficulty: Difficulty.Hard),
            (StableReference: "antiquity_light_linen_head_veil", Name: "assemble a light linen head veil", Material: "linen", Cloth: 130, Yarn: 10, Fine: true, Minimum: 30, Difficulty: Difficulty.Normal)
        })
        {
            var characteristicRequirements = garment.Fine
                ? "characteristic Colour any; characteristic Fine Colour any"
                : "characteristic Colour any";
            AddHellenicGarmentCraft(
                garment.Name,
                $"assemble {garment.Name["assemble a ".Length..]} from rectangular {garment.Material} cloth",
                garment.Name,
                $"{garment.Material} cloth being assembled into Hellenic clothing",
                garment.Minimum,
                garment.Difficulty,
                assemblyPhases,
                [
                    $"Commodity - {garment.Cloth} grams of {garment.Material}; piletag Garment Cloth; {characteristicRequirements}",
                    $"Commodity - {garment.Yarn} grams of {garment.Material}; piletag Spun Yarn; characteristic Colour any"
                ],
                sewingTools,
                [StableVariableProduct(garment.StableReference, garment.Fine)]);
        }
    }
    
    private void SeedAntiquityCelticClothingCrafts()
	{
		if (!ShouldSeedAntiquityCrafts())
		{
			return;
		}

		const string celticKnowledge = "Celtic Textilecraft";
		const string celticKnowledgeDescription =
			"Celtic garment knowledge for assembling sleeved wool tunics, braccae, checked cloaks, mantles, gowns, skirts, and linen veils.";

		var missingGarments = CelticAntiquityClothingStableReferences.Keys
			.Where(x => !TryLookupReworkItem(x, out _))
			.ToList();
		if (missingGarments.Count > 0)
		{
			return;
		}

		void AddCelticGarmentCraft(string name, string blurb, string action, string itemDescription,
			int minimumTraitValue, Difficulty difficulty,
			IEnumerable<(int Seconds, string Echo, string FailEcho)> phases, IEnumerable<string> inputs,
			IEnumerable<string> tools, IEnumerable<string> products)
		{
			AddCraft(
				name,
				"Tailoring",
				blurb,
				action,
				itemDescription,
				celticKnowledge,
				"Tailoring",
				minimumTraitValue,
				difficulty,
				Outcome.MinorFail,
				5,
				3,
				false,
				phases,
				inputs,
				tools,
				products,
				[],
				knowledgeSubtype: "Celtic",
				knowledgeDescription: celticKnowledgeDescription,
				knowledgeLongDescription: celticKnowledgeDescription);
		}

		var celticAssemblyPhases = new (int Seconds, string Echo, string FailEcho)[]
		{
			(35, "$0 lay|lays out $i1 and mark|marks a rectangular Celtic garment pattern.", "$0 lay|lays out $i1, but mark|marks the garment unevenly."),
			(35, "$0 cut|cuts the cloth with $t2 and turn|turns the main edges into hems.", "$0 cut|cuts with $t2, but the edges wander out of line."),
			(45, "$0 stitch|stitches seams, folds, and fastening points with $t1, using $i2 for the strongest joins.", "$0 stitch|stitches with $t1, but the joins sit awkwardly and weakly."),
			(30, "$0 shake|shakes out $p1, checking the drape and fit over a belted layer.", "$0 shake|shakes out only $f1 after spoiling the cloth.")
		};
		var checkedAssemblyPhases = new (int Seconds, string Echo, string FailEcho)[]
		{
			(35, "$0 lay|lays out $i1 and $i3, matching the checked cloth lengths and border direction.", "$0 lay|lays out $i1 and $i3, but the checks do not line up cleanly."),
			(35, "$0 cut|cuts the cloth with $t2 and square|squares the cloak edges.", "$0 cut|cuts with $t2, but the pattern pulls crookedly along the edge."),
			(45, "$0 stitch|stitches the cloak edge and reinforcing yarn with $t1.", "$0 stitch|stitches with $t1, but the border bunches and weakens."),
			(30, "$0 lift|lifts $p1 to check its checked fall across the shoulders.", "$0 lift|lifts only $f1 after the checked cloth is spoiled.")
		};
		var sewingTools = new[]
		{
			"TagTool - Held - an item with the Sewing Needle tag",
			"TagTool - Held - an item with the Shears tag"
		};

		foreach (var garment in new[]
		{
			(StableReference: "antiquity_sleeved_common_wool_tunic", Name: "assemble a Celtic sleeved wool tunic", DisplayName: "a Celtic sleeved wool tunic", Material: "wool", Cloth: 590, SecondaryCloth: 0, Yarn: 30, Fine: false, GeneratedProduct: false, Minimum: 15, Difficulty: Difficulty.Easy),
			(StableReference: "antiquity_wool_braccae", Name: "assemble Celtic wool braccae", DisplayName: "Celtic wool braccae", Material: "wool", Cloth: 500, SecondaryCloth: 0, Yarn: 25, Fine: false, GeneratedProduct: false, Minimum: 15, Difficulty: Difficulty.Normal),
			(StableReference: "antiquity_rectangular_wool_cloak", Name: "assemble a Celtic rectangular wool cloak", DisplayName: "a Celtic rectangular wool cloak", Material: "wool", Cloth: 1160, SecondaryCloth: 0, Yarn: 40, Fine: false, GeneratedProduct: false, Minimum: 15, Difficulty: Difficulty.Normal),
			(StableReference: "antiquity_fine_bordered_wool_tunic", Name: "assemble a Celtic fine bordered wool tunic", DisplayName: "a Celtic fine bordered wool tunic", Material: "wool", Cloth: 530, SecondaryCloth: 0, Yarn: 30, Fine: true, GeneratedProduct: false, Minimum: 35, Difficulty: Difficulty.Normal),
			(StableReference: "antiquity_fine_wool_braccae", Name: "assemble Celtic fine wool braccae", DisplayName: "Celtic fine wool braccae", Material: "wool", Cloth: 470, SecondaryCloth: 0, Yarn: 30, Fine: true, GeneratedProduct: false, Minimum: 35, Difficulty: Difficulty.Hard),
			(StableReference: "antiquity_fine_checked_wool_cloak", Name: "assemble a Celtic fine checked wool cloak", DisplayName: "a Celtic fine checked wool cloak", Material: "wool", Cloth: 900, SecondaryCloth: 250, Yarn: 45, Fine: true, GeneratedProduct: true, Minimum: 40, Difficulty: Difficulty.Hard),
			(StableReference: "antiquity_long_sleeved_wool_tunic", Name: "assemble a Celtic long sleeved wool tunic", DisplayName: "a Celtic long sleeved wool tunic", Material: "wool", Cloth: 650, SecondaryCloth: 0, Yarn: 30, Fine: false, GeneratedProduct: false, Minimum: 20, Difficulty: Difficulty.Normal),
			(StableReference: "antiquity_wool_wrap_skirt", Name: "assemble a Celtic wool wrap skirt", DisplayName: "a Celtic wool wrap skirt", Material: "wool", Cloth: 535, SecondaryCloth: 0, Yarn: 25, Fine: false, GeneratedProduct: false, Minimum: 15, Difficulty: Difficulty.Normal),
			(StableReference: "antiquity_broad_wool_mantle", Name: "assemble a Celtic broad wool mantle", DisplayName: "a Celtic broad wool mantle", Material: "wool", Cloth: 960, SecondaryCloth: 0, Yarn: 35, Fine: false, GeneratedProduct: false, Minimum: 20, Difficulty: Difficulty.Normal),
			(StableReference: "antiquity_fine_sleeved_wool_gown", Name: "assemble a Celtic fine sleeved wool gown", DisplayName: "a Celtic fine sleeved wool gown", Material: "wool", Cloth: 740, SecondaryCloth: 0, Yarn: 40, Fine: true, GeneratedProduct: false, Minimum: 40, Difficulty: Difficulty.Hard),
			(StableReference: "antiquity_fine_bordered_wool_mantle", Name: "assemble a Celtic fine bordered wool mantle", DisplayName: "a Celtic fine bordered wool mantle", Material: "wool", Cloth: 910, SecondaryCloth: 0, Yarn: 40, Fine: true, GeneratedProduct: false, Minimum: 40, Difficulty: Difficulty.Hard),
			(StableReference: "antiquity_linen_shoulder_veil", Name: "assemble a Celtic linen shoulder veil", DisplayName: "a Celtic linen shoulder veil", Material: "linen", Cloth: 110, SecondaryCloth: 0, Yarn: 10, Fine: true, GeneratedProduct: false, Minimum: 30, Difficulty: Difficulty.Normal)
		})
		{
			var characteristicRequirements = garment.Fine
				? "characteristic Colour any; characteristic Fine Colour any"
				: "characteristic Colour any";
			var inputs = new List<string>
			{
				$"Commodity - {garment.Cloth} grams of {garment.Material}; piletag Garment Cloth; {characteristicRequirements}",
				$"Commodity - {garment.Yarn} grams of {garment.Material}; piletag Spun Yarn; characteristic Colour any"
			};
			if (garment.SecondaryCloth > 0)
			{
				inputs.Add($"Commodity - {garment.SecondaryCloth} grams of {garment.Material}; piletag Garment Cloth; {characteristicRequirements}");
			}

			AddCelticGarmentCraft(
				garment.Name,
				$"assemble {garment.DisplayName} from {garment.Material} garment cloth",
				garment.Name,
				$"{garment.Material} cloth being assembled into Celtic clothing",
				garment.Minimum,
				garment.Difficulty,
				garment.GeneratedProduct ? checkedAssemblyPhases : celticAssemblyPhases,
				inputs,
				sewingTools,
				[garment.GeneratedProduct ? StableSimpleProduct(garment.StableReference) : StableVariableProduct(garment.StableReference, garment.Fine)]);
		}
	}

	private void SeedAntiquityGermanicClothingCrafts()
	{
		if (!ShouldSeedAntiquityCrafts())
		{
			return;
		}

		const string germanicKnowledge = "Germanic Textilecraft";
		const string germanicKnowledgeDescription =
			"Germanic garment knowledge for assembling wool tunics, trousers, cloaks, scarves, gowns, mantles, veils, and skin capes for northern antiquity dress.";

		var missingGarments = GermanicAntiquityClothingStableReferences.Keys
			.Where(x => !TryLookupReworkItem(x, out _))
			.ToList();
		if (missingGarments.Count > 0)
		{
			return;
		}

		void AddGermanicGarmentCraft(string name, string blurb, string action, string itemDescription,
			int minimumTraitValue, Difficulty difficulty,
			IEnumerable<(int Seconds, string Echo, string FailEcho)> phases, IEnumerable<string> inputs,
			IEnumerable<string> tools, IEnumerable<string> products)
		{
			AddCraft(
				name,
				"Tailoring",
				blurb,
				action,
				itemDescription,
				germanicKnowledge,
				"Tailoring",
				minimumTraitValue,
				difficulty,
				Outcome.MinorFail,
				5,
				3,
				false,
				phases,
				inputs,
				tools,
				products,
				[],
				knowledgeSubtype: "Germanic",
				knowledgeDescription: germanicKnowledgeDescription,
				knowledgeLongDescription: germanicKnowledgeDescription);
		}

		var germanicAssemblyPhases = new (int Seconds, string Echo, string FailEcho)[]
		{
			(35, "$0 lay|lays out $i1 and mark|marks a straight northern garment cut.", "$0 lay|lays out $i1, but mark|marks the garment unevenly."),
			(35, "$0 cut|cuts the cloth with $t2 and shape|shapes the edges, sleeves, or trouser legs.", "$0 cut|cuts with $t2, but the edges and openings sit awkwardly."),
			(45, "$0 stitch|stitches strong seams and hems with $t1, reinforcing them with $i2.", "$0 stitch|stitches with $t1, but the seams pull out of line."),
			(30, "$0 arrange|arranges $p1, checking its warmth and layered fit.", "$0 arrange|arranges only $f1 after the cloth is spoiled.")
		};
		var patternedAssemblyPhases = new (int Seconds, string Echo, string FailEcho)[]
		{
			(30, "$0 lay|lays out $i1 and $i3, matching the checked lengths before cutting.", "$0 lay|lays out $i1 and $i3, but the checked lengths do not align."),
			(35, "$0 cut|cuts the scarf lengths with $t2 and square|squares the ends.", "$0 cut|cuts with $t2, but the ends pull crooked."),
			(40, "$0 stitch|stitches the edges and fringes the ends with $t1.", "$0 stitch|stitches with $t1, but the fringes loosen unevenly."),
			(25, "$0 lift|lifts $p1 to check its fall around the neck and shoulders.", "$0 lift|lifts only $f1 after the scarf is spoiled.")
		};
		var furLinedAssemblyPhases = new (int Seconds, string Echo, string FailEcho)[]
		{
			(35, "$0 lay|lays out $i1 and trim|trims the fur lining stock from $i3.", "$0 lay|lays out $i1 and $i3, but cut|cuts the lining unevenly."),
			(40, "$0 cut|cuts the wool with $t2 and baste|bastes the lining edge into place.", "$0 cut|cuts with $t2, but the wool and lining do not sit together cleanly."),
			(50, "$0 stitch|stitches the fur lining to the cloak with $t1, reinforcing the front opening and collar.", "$0 stitch|stitches with $t1, but the lining bunches and pulls loose."),
			(30, "$0 heft|hefts $p1 to check its warmth and fall.", "$0 heft|hefts only $f1 after the lining work fails.")
		};
		var skinCapePhases = new (int Seconds, string Echo, string FailEcho)[]
		{
			(35, "$0 sort|sorts $i1 and $i2 into matching hair-out panels.", "$0 sort|sorts $i1 and $i2 poorly, leaving the panels mismatched."),
			(40, "$0 trim|trims the skin panels with $t2 and overlap|overlaps the strongest edges.", "$0 trim|trims with $t2, but the panels gape and twist."),
			(50, "$0 stitch|stitches the cape together with $t1 and $i3, leaving the hair side outward.", "$0 stitch|stitches with $t1, but the cape pulls apart under its own weight."),
			(30, "$0 shake|shakes out $p1, checking the shoulder fall.", "$0 shake|shakes out only $f1 after the skin panels are spoiled.")
		};
		var sewingTools = new[]
		{
			"TagTool - Held - an item with the Sewing Needle tag",
			"TagTool - Held - an item with the Shears tag"
		};

		foreach (var garment in new[]
		{
			(StableReference: "antiquity_straight_wool_tunic", Name: "assemble a Germanic straight wool tunic", DisplayName: "a Germanic straight wool tunic", Material: "wool", Cloth: 620, SecondaryCloth: 0, Yarn: 30, Fine: false, GeneratedProduct: false, Minimum: 15, Difficulty: Difficulty.Easy),
			(StableReference: "antiquity_narrow_wool_trousers", Name: "assemble Germanic narrow wool trousers", DisplayName: "Germanic narrow wool trousers", Material: "wool", Cloth: 515, SecondaryCloth: 0, Yarn: 25, Fine: false, GeneratedProduct: false, Minimum: 15, Difficulty: Difficulty.Normal),
			(StableReference: "antiquity_heavy_wool_cloak", Name: "assemble a Germanic heavy wool cloak", DisplayName: "a Germanic heavy wool cloak", Material: "wool", Cloth: 1210, SecondaryCloth: 0, Yarn: 45, Fine: false, GeneratedProduct: false, Minimum: 20, Difficulty: Difficulty.Normal),
			(StableReference: "antiquity_fine_banded_wool_tunic", Name: "assemble a Germanic fine banded wool tunic", DisplayName: "a Germanic fine banded wool tunic", Material: "wool", Cloth: 570, SecondaryCloth: 0, Yarn: 35, Fine: true, GeneratedProduct: false, Minimum: 35, Difficulty: Difficulty.Normal),
			(StableReference: "antiquity_fine_tapered_wool_trousers", Name: "assemble Germanic fine tapered wool trousers", DisplayName: "Germanic fine tapered wool trousers", Material: "wool", Cloth: 495, SecondaryCloth: 0, Yarn: 30, Fine: true, GeneratedProduct: false, Minimum: 35, Difficulty: Difficulty.Hard),
			(StableReference: "antiquity_long_straight_wool_tunic", Name: "assemble a Germanic long straight wool tunic", DisplayName: "a Germanic long straight wool tunic", Material: "wool", Cloth: 670, SecondaryCloth: 0, Yarn: 30, Fine: false, GeneratedProduct: false, Minimum: 20, Difficulty: Difficulty.Normal),
			(StableReference: "antiquity_overlapping_wool_skirt", Name: "assemble a Germanic overlapping wool skirt", DisplayName: "a Germanic overlapping wool skirt", Material: "wool", Cloth: 570, SecondaryCloth: 0, Yarn: 25, Fine: false, GeneratedProduct: false, Minimum: 15, Difficulty: Difficulty.Normal),
			(StableReference: "antiquity_checked_wool_scarf", Name: "assemble a Germanic checked wool scarf", DisplayName: "a Germanic checked wool scarf", Material: "wool", Cloth: 210, SecondaryCloth: 40, Yarn: 12, Fine: false, GeneratedProduct: true, Minimum: 20, Difficulty: Difficulty.Normal),
			(StableReference: "antiquity_fine_long_wool_gown", Name: "assemble a Germanic fine long wool gown", DisplayName: "a Germanic fine long wool gown", Material: "wool", Cloth: 780, SecondaryCloth: 0, Yarn: 40, Fine: true, GeneratedProduct: false, Minimum: 40, Difficulty: Difficulty.Hard),
			(StableReference: "antiquity_fine_heavy_wool_mantle", Name: "assemble a Germanic fine heavy wool mantle", DisplayName: "a Germanic fine heavy wool mantle", Material: "wool", Cloth: 1010, SecondaryCloth: 0, Yarn: 40, Fine: true, GeneratedProduct: false, Minimum: 40, Difficulty: Difficulty.Hard),
			(StableReference: "antiquity_linen_head_veil", Name: "assemble a Germanic linen head veil", DisplayName: "a Germanic linen head veil", Material: "linen", Cloth: 120, SecondaryCloth: 0, Yarn: 10, Fine: true, GeneratedProduct: false, Minimum: 30, Difficulty: Difficulty.Normal)
		})
		{
			var characteristicRequirements = garment.Fine
				? "characteristic Colour any; characteristic Fine Colour any"
				: "characteristic Colour any";
			var inputs = new List<string>
			{
				$"Commodity - {garment.Cloth} grams of {garment.Material}; piletag Garment Cloth; {characteristicRequirements}",
				$"Commodity - {garment.Yarn} grams of {garment.Material}; piletag Spun Yarn; characteristic Colour any"
			};
			if (garment.SecondaryCloth > 0)
			{
				inputs.Add($"Commodity - {garment.SecondaryCloth} grams of {garment.Material}; piletag Garment Cloth; {characteristicRequirements}");
			}

			AddGermanicGarmentCraft(
				garment.Name,
				$"assemble {garment.DisplayName} from {garment.Material} garment cloth",
				garment.Name,
				$"{garment.Material} cloth being assembled into Germanic clothing",
				garment.Minimum,
				garment.Difficulty,
				garment.GeneratedProduct ? patternedAssemblyPhases : germanicAssemblyPhases,
				inputs,
				sewingTools,
				[garment.GeneratedProduct ? StableSimpleProduct(garment.StableReference) : StableVariableProduct(garment.StableReference, garment.Fine)]);
		}

		AddGermanicGarmentCraft(
			"assemble a Germanic fur-lined wool cloak",
			"assemble a Germanic fur-lined wool cloak from fine wool cloth and fur lining stock",
			"assemble a Germanic fur-lined wool cloak",
			"fine wool and fur being assembled into Germanic winter clothing",
			40,
			Difficulty.Hard,
			furLinedAssemblyPhases,
			[
				"Commodity - 1 kilogram 350 grams of wool; piletag Garment Cloth; characteristic Colour any; characteristic Fine Colour any",
				"Commodity - 45 grams of wool; piletag Spun Yarn; characteristic Colour any",
				"CommodityTag - 300 grams of a material tagged as Hair"
			],
			sewingTools,
			[StableVariableProduct("antiquity_fur_lined_wool_cloak", true)]);

		AddGermanicGarmentCraft(
			"assemble a Germanic woolly skin cape",
			"assemble a Germanic woolly skin cape from processed hair-on skin",
			"assemble a Germanic woolly skin cape",
			"hair-on animal skin being assembled into Germanic winter clothing",
			25,
			Difficulty.Normal,
			skinCapePhases,
			[
				"CommodityTag - 1 kilogram 200 grams of a material tagged as Animal Skin",
				"CommodityTag - 250 grams of a material tagged as Hair",
				"Commodity - 30 grams of wool; piletag Spun Yarn; characteristic Colour any"
			],
			sewingTools,
			[StableSimpleProduct("antiquity_woolly_skin_cape")]);
	}

	private void SeedAntiquityKushiteClothingCrafts()
	{
		if (!ShouldSeedAntiquityCrafts())
		{
			return;
		}

		const string kushiteKnowledge = "Kushite Textilecraft";
		const string kushiteKnowledgeDescription =
			"Kushite garment knowledge for assembling Nile Valley linen and cotton kilts, shoulder cloths, robes, shawls, headdresses, beadwork, skirts, dresses, and headcloths.";

		var missingGarments = KushiteAntiquityClothingStableReferences.Keys
			.Where(x => !TryLookupReworkItem(x, out _))
			.ToList();
		if (missingGarments.Count > 0)
		{
			return;
		}

		void AddKushiteGarmentCraft(string name, string blurb, string action, string itemDescription,
			int minimumTraitValue, Difficulty difficulty,
			IEnumerable<(int Seconds, string Echo, string FailEcho)> phases, IEnumerable<string> inputs,
			IEnumerable<string> tools, IEnumerable<string> products)
		{
			AddCraft(
				name,
				"Tailoring",
				blurb,
				action,
				itemDescription,
				kushiteKnowledge,
				"Tailoring",
				minimumTraitValue,
				difficulty,
				Outcome.MinorFail,
				5,
				3,
				false,
				phases,
				inputs,
				tools,
				products,
				[],
				knowledgeSubtype: "Kushite",
				knowledgeDescription: kushiteKnowledgeDescription,
				knowledgeLongDescription: kushiteKnowledgeDescription);
		}

		var kushiteAssemblyPhases = new (int Seconds, string Echo, string FailEcho)[]
		{
			(30, "$0 smooth|smooths out $i1 and mark|marks the wrap, fold, or opening lines.", "$0 smooth|smooths out $i1, but mark|marks the cloth unevenly."),
			(35, "$0 cut|cuts the cloth with $t2 and fold|folds the edges into narrow hems.", "$0 cut|cuts with $t2, but the hems pull crookedly."),
			(40, "$0 stitch|stitches and reinforce|reinforces the folded or draped garment points with $t1.", "$0 stitch|stitches with $t1, but the joins sit awkwardly."),
			(25, "$0 shake|shakes out $p1, checking the fall of the cloth.", "$0 shake|shakes out only $f1 after the cloth is spoiled.")
		};
		var kushiteBeadedPhases = new (int Seconds, string Echo, string FailEcho)[]
		{
			(30, "$0 smooth|smooths out $i1 and measure|measures the linen for a beaded edge or apron run.", "$0 smooth|smooths out $i1, but measure|measures the beadwork unevenly."),
			(35, "$0 cut|cuts the linen with $t2 and sew|sews the supporting hems.", "$0 cut|cuts with $t2, but leave|leaves the supporting hems weak."),
			(45, "$0 thread|threads $i3 onto $t1 and stitch|stitches the bead rows into place.", "$0 thread|threads $i3 onto $t1, but the bead rows sag and bunch."),
			(25, "$0 lift|lifts $p1 to check the fall of the beaded linen.", "$0 lift|lifts only $f1 after the beadwork pulls loose.")
		};
		var sewingTools = new[]
		{
			"TagTool - Held - an item with the Sewing Needle tag",
			"TagTool - Held - an item with the Shears tag"
		};
		var beadingTools = new[]
		{
			"TagTool - Held - an item with the Beading Needle tag",
			"TagTool - Held - an item with the Shears tag"
		};

		foreach (var garment in new[]
		{
			(StableReference: "adjacent_antiquity_narrow_linen_kilt", Name: "assemble a Kushite narrow linen kilt", DisplayName: "a Kushite narrow linen kilt", Material: "linen", Cloth: 210, Yarn: 15, BeadStock: 0, Fine: false, Minimum: 10, Difficulty: Difficulty.Easy),
			(StableReference: "adjacent_antiquity_linen_shoulder_cloth", Name: "assemble a Kushite linen shoulder cloth", DisplayName: "a Kushite linen shoulder cloth", Material: "linen", Cloth: 170, Yarn: 10, BeadStock: 0, Fine: false, Minimum: 10, Difficulty: Difficulty.Easy),
			(StableReference: "adjacent_antiquity_cotton_wrap_skirt", Name: "assemble a Kushite cotton wrap skirt", DisplayName: "a Kushite cotton wrap skirt", Material: "cotton", Cloth: 310, Yarn: 20, BeadStock: 0, Fine: false, Minimum: 15, Difficulty: Difficulty.Normal),
			(StableReference: "adjacent_antiquity_sleeveless_linen_tunic", Name: "assemble a Kushite sleeveless linen tunic", DisplayName: "a Kushite sleeveless linen tunic", Material: "linen", Cloth: 285, Yarn: 25, BeadStock: 0, Fine: false, Minimum: 15, Difficulty: Difficulty.Normal),
			(StableReference: "adjacent_antiquity_fringed_linen_robe", Name: "assemble a Kushite fringed linen robe", DisplayName: "a Kushite fringed linen robe", Material: "linen", Cloth: 500, Yarn: 40, BeadStock: 0, Fine: true, Minimum: 35, Difficulty: Difficulty.Hard),
			(StableReference: "adjacent_antiquity_tasseled_linen_shawl", Name: "assemble a Kushite tasseled linen shawl", DisplayName: "a Kushite tasseled linen shawl", Material: "linen", Cloth: 190, Yarn: 30, BeadStock: 0, Fine: true, Minimum: 30, Difficulty: Difficulty.Normal),
			(StableReference: "adjacent_antiquity_tall_linen_headdress", Name: "assemble a Kushite tall linen headdress", DisplayName: "a Kushite tall linen headdress", Material: "linen", Cloth: 235, Yarn: 25, BeadStock: 0, Fine: true, Minimum: 35, Difficulty: Difficulty.Hard),
			(StableReference: "adjacent_antiquity_beaded_linen_girdle", Name: "assemble a Kushite beaded linen girdle", DisplayName: "a Kushite beaded linen girdle", Material: "linen", Cloth: 210, Yarn: 25, BeadStock: 40, Fine: true, Minimum: 40, Difficulty: Difficulty.Hard),
			(StableReference: "adjacent_antiquity_cotton_draped_dress", Name: "assemble a Kushite cotton draped dress", DisplayName: "a Kushite cotton draped dress", Material: "cotton", Cloth: 430, Yarn: 30, BeadStock: 0, Fine: true, Minimum: 35, Difficulty: Difficulty.Hard),
			(StableReference: "adjacent_antiquity_linen_bead_apron", Name: "assemble a Kushite linen bead apron", DisplayName: "a Kushite linen bead apron", Material: "linen", Cloth: 150, Yarn: 20, BeadStock: 50, Fine: true, Minimum: 35, Difficulty: Difficulty.Normal),
			(StableReference: "adjacent_antiquity_plain_cotton_headcloth", Name: "assemble a Kushite plain cotton headcloth", DisplayName: "a Kushite plain cotton headcloth", Material: "cotton", Cloth: 80, Yarn: 8, BeadStock: 0, Fine: false, Minimum: 10, Difficulty: Difficulty.Easy)
		})
		{
			var characteristicRequirements = garment.Fine
				? "characteristic Colour any; characteristic Fine Colour any"
				: "characteristic Colour any";
			var inputs = new List<string>
			{
				$"Commodity - {garment.Cloth} grams of {garment.Material}; piletag Garment Cloth; {characteristicRequirements}",
				$"Commodity - {garment.Yarn} grams of {garment.Material}; piletag Spun Yarn; characteristic Colour any"
			};

			if (garment.BeadStock > 0)
			{
				inputs.Add($"CommodityTag - {garment.BeadStock} grams of a material tagged as Glass; piletag Bead Stock");
			}

			AddKushiteGarmentCraft(
				garment.Name,
				$"assemble {garment.DisplayName} from {garment.Material} garment cloth",
				garment.Name,
				$"{garment.Material} cloth being assembled into Kushite clothing",
				garment.Minimum,
				garment.Difficulty,
				garment.BeadStock > 0 ? kushiteBeadedPhases : kushiteAssemblyPhases,
				inputs,
				garment.BeadStock > 0 ? beadingTools : sewingTools,
				[StableVariableProduct(garment.StableReference, garment.Fine)]);
		}
	}

    private void SeedAntiquityEgyptianClothingCrafts()
    {
        if (!ShouldSeedAntiquityCrafts())
        {
            return;
        }

        const string ancientKnowledge = "Ancient Textile Production";
        const string egyptianKnowledge = "Egyptian Textilecraft";
        const string ancientKnowledgeDescription =
            "Shared ancient techniques for preparing textile fibres, spinning yarn, weaving cloth, dyeing, fulling, and textile ornament stock.";
        const string egyptianKnowledgeDescription =
            "Egyptian garment knowledge for assembling linen kilts, shoulder cloths, sleeveless tunics, robes, shawls, headdresses, and beaded linen dress pieces.";

        AddCraft(
            "sort glass beads for textile edging",
            "Tailoring",
            "sort glass beads into bead stock for textile edging",
            "sorting glass beads",
            "glass beads being sorted and strung for textile edging",
            ancientKnowledge,
            "Tailoring",
            10,
            Difficulty.Easy,
            Outcome.MinorFail,
            5,
            3,
            false,
            [
                (25, "$0 sort|sorts $i1 into small, even beads suitable for garment edging.", "$0 sort|sorts $i1 poorly, mixing chipped and uneven pieces into the bead stock."),
                (30, "$0 thread|threads trial runs on $t1, setting aside beads with usable holes and regular shapes.", "$0 thread|threads trial runs on $t1, but crack|cracks too many of the beads."),
                (25, "$0 gather|gathers $p1 into measured bead stock.", "$0 gather|gathers only $f1 from the flawed bead sorting.")
            ],
            ["CommodityTag - 130 grams of a material tagged as Glass"],
            ["TagTool - Held - an item with the Beading Needle tag"],
            ["CommodityProduct - 110 grams of glass commodity; tag Bead Stock"],
            ["CommodityProduct - 40 grams of glass commodity; tag Bead Stock"],
            knowledgeSubtype: "Textiles",
            knowledgeDescription: ancientKnowledgeDescription,
            knowledgeLongDescription: ancientKnowledgeDescription);

        var missingGarments = EgyptianAntiquityClothingStableReferences.Keys
            .Where(x => !TryLookupReworkItem(x, out _))
            .ToList();
        if (missingGarments.Count > 0)
        {
            return;
        }

        void AddEgyptianGarmentCraft(string name, string blurb, string action, string itemDescription,
            int minimumTraitValue, Difficulty difficulty,
            IEnumerable<(int Seconds, string Echo, string FailEcho)> phases, IEnumerable<string> inputs,
            IEnumerable<string> tools, IEnumerable<string> products)
        {
            AddCraft(
                name,
                "Tailoring",
                blurb,
                action,
                itemDescription,
                egyptianKnowledge,
                "Tailoring",
                minimumTraitValue,
                difficulty,
                Outcome.MinorFail,
                5,
                3,
                false,
                phases,
                inputs,
                tools,
                products,
                [],
                knowledgeSubtype: "Egyptian",
                knowledgeDescription: egyptianKnowledgeDescription,
                knowledgeLongDescription: egyptianKnowledgeDescription);
        }

        var linenAssemblyPhases = new (int Seconds, string Echo, string FailEcho)[]
        {
            (30, "$0 smooth|smooths out $i1 and mark|marks the linen for the required wrap, fold, or body opening.", "$0 smooth|smooths out $i1, but mark|marks the linen unevenly."),
            (35, "$0 cut|cuts the linen with $t2 and turn|turns the edges into narrow hems.", "$0 cut|cuts with $t2, but the hems pull crookedly."),
            (40, "$0 stitch|stitches, pleat|pleats, and reinforce|reinforces the garment's folds and fastening points with $t1.", "$0 stitch|stitches with $t1, but the folds sit awkwardly and the fastening points are weak."),
            (25, "$0 shake|shakes out $p1, checking the drape and fall.", "$0 shake|shakes out only $f1 after the linen is spoiled.")
        };
        var beadedAssemblyPhases = new (int Seconds, string Echo, string FailEcho)[]
        {
            (30, "$0 smooth|smooths out $i1 and measure|measures the linen panel for a beaded edge.", "$0 smooth|smooths out $i1, but measure|measures the beaded edge poorly."),
            (35, "$0 cut|cuts the linen with $t2 and sew|sews the supporting hems.", "$0 cut|cuts with $t2, but leave|leaves the supporting hems uneven."),
            (45, "$0 thread|threads $i3 onto $t1 and stitch|stitches the bead rows into the garment edge.", "$0 thread|threads $i3 onto $t1, but the bead rows sag and bunch."),
            (25, "$0 lift|lifts $p1 to check the hang of the beaded linen.", "$0 lift|lifts only $f1 after the beadwork pulls loose.")
        };
        var sewingTools = new[]
        {
            "TagTool - Held - an item with the Sewing Needle tag",
            "TagTool - Held - an item with the Shears tag"
        };
        var beadingTools = new[]
        {
            "TagTool - Held - an item with the Beading Needle tag",
            "TagTool - Held - an item with the Shears tag"
        };

        foreach (var garment in new[]
        {
            (StableReference: "adjacent_antiquity_narrow_linen_kilt", Name: "assemble an Egyptian narrow linen kilt", DisplayName: "an Egyptian narrow linen kilt", Cloth: 210, Yarn: 15, BeadStock: 0, Fine: false, Minimum: 10, Difficulty: Difficulty.Easy),
            (StableReference: "adjacent_antiquity_linen_shoulder_cloth", Name: "assemble an Egyptian linen shoulder cloth", DisplayName: "an Egyptian linen shoulder cloth", Cloth: 170, Yarn: 10, BeadStock: 0, Fine: false, Minimum: 10, Difficulty: Difficulty.Easy),
            (StableReference: "adjacent_antiquity_sleeveless_linen_tunic", Name: "assemble an Egyptian sleeveless linen tunic", DisplayName: "an Egyptian sleeveless linen tunic", Cloth: 285, Yarn: 25, BeadStock: 0, Fine: false, Minimum: 15, Difficulty: Difficulty.Normal),
            (StableReference: "adjacent_antiquity_fringed_linen_robe", Name: "assemble an Egyptian fringed linen robe", DisplayName: "an Egyptian fringed linen robe", Cloth: 500, Yarn: 40, BeadStock: 0, Fine: true, Minimum: 35, Difficulty: Difficulty.Hard),
            (StableReference: "adjacent_antiquity_tasseled_linen_shawl", Name: "assemble an Egyptian tasseled linen shawl", DisplayName: "an Egyptian tasseled linen shawl", Cloth: 190, Yarn: 30, BeadStock: 0, Fine: true, Minimum: 30, Difficulty: Difficulty.Normal),
            (StableReference: "adjacent_antiquity_tall_linen_headdress", Name: "assemble an Egyptian tall linen headdress", DisplayName: "an Egyptian tall linen headdress", Cloth: 235, Yarn: 25, BeadStock: 0, Fine: true, Minimum: 35, Difficulty: Difficulty.Hard),
            (StableReference: "adjacent_antiquity_beaded_linen_girdle", Name: "assemble an Egyptian beaded linen girdle", DisplayName: "an Egyptian beaded linen girdle", Cloth: 210, Yarn: 25, BeadStock: 40, Fine: true, Minimum: 40, Difficulty: Difficulty.Hard),
            (StableReference: "adjacent_antiquity_linen_bead_apron", Name: "assemble an Egyptian linen bead apron", DisplayName: "an Egyptian linen bead apron", Cloth: 150, Yarn: 20, BeadStock: 50, Fine: true, Minimum: 35, Difficulty: Difficulty.Normal)
        })
        {
            var characteristicRequirements = garment.Fine
                ? "characteristic Colour any; characteristic Fine Colour any"
                : "characteristic Colour any";
            var inputs = new List<string>
            {
                $"Commodity - {garment.Cloth} grams of linen; piletag Garment Cloth; {characteristicRequirements}",
                $"Commodity - {garment.Yarn} grams of linen; piletag Spun Yarn; characteristic Colour any"
            };

            if (garment.BeadStock > 0)
            {
                inputs.Add($"CommodityTag - {garment.BeadStock} grams of a material tagged as Glass; piletag Bead Stock");
            }

            AddEgyptianGarmentCraft(
                garment.Name,
                $"assemble {garment.DisplayName} from linen garment cloth",
                garment.Name,
                "linen cloth being assembled into Egyptian clothing",
                garment.Minimum,
                garment.Difficulty,
                garment.BeadStock > 0 ? beadedAssemblyPhases : linenAssemblyPhases,
                inputs,
                garment.BeadStock > 0 ? beadingTools : sewingTools,
                [StableVariableProduct(garment.StableReference, garment.Fine)]);
        }
    }
}
