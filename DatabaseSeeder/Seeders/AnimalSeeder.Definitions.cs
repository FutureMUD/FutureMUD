#nullable enable

using MudSharp.Body;
using MudSharp.Character.Heritage;
using MudSharp.Combat;
using MudSharp.Form.Material;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class AnimalSeeder
{
    internal enum AnimalBreathingMode
    {
        Simple,
        Insect,
        Partless,
        Freshwater,
        Saltwater,
        Blowhole
    }

    internal enum AnimalBoneExpectation
    {
        Required,
        Optional,
        Forbidden
    }

    internal sealed record AnimalAgeProfileTemplate(
        int ChildAge,
        int YouthAge,
        int YoungAdultAge,
        int AdultAge,
        int ElderAge,
        int VenerableAge
    );

    internal sealed record AnimalHeightWeightTemplate(
        string Name,
        double MeanHeight,
        double StandardDeviationHeight,
        double MeanBmi,
        double StandardDeviationBmi
    );

    internal sealed record AnimalDescriptionVariant(
        string ShortDescription,
        string FullDescription
    );

    internal sealed record AnimalDescriptionPack(
        IReadOnlyList<AnimalDescriptionVariant> BabyMale,
        IReadOnlyList<AnimalDescriptionVariant> BabyFemale,
        IReadOnlyList<AnimalDescriptionVariant> JuvenileMale,
        IReadOnlyList<AnimalDescriptionVariant> JuvenileFemale,
        IReadOnlyList<AnimalDescriptionVariant> AdultMale,
        IReadOnlyList<AnimalDescriptionVariant> AdultFemale,
        bool AddDogBreedDescriptions = false,
        bool UseCatCoatDescriptions = false
    );

    internal sealed record AnimalBodypartUsageTemplate(
        string BodypartAlias,
        string Usage
    );

    internal sealed record AnimalAttackGrant(
        string AttackKey,
        ItemQuality Quality
    );

    internal sealed record AnimalAliasAttackGrant(
        string AttackKey,
        ItemQuality Quality,
        IReadOnlyList<string> BodypartAliases
    );

    internal sealed record AnimalVenomEffectTemplate(
        DrugType DrugType,
        double RelativeIntensity,
        string AdditionalEffects
    );

    internal sealed record AnimalVenomProfileTemplate(
        string Key,
        double IntensityPerGram,
        double RelativeMetabolisationRate,
        DrugVector DrugVectors,
        LiquidInjectionConsequence InjectionConsequence,
        string Description,
        string LongDescription,
        string TasteText,
        string VagueTasteText,
        string SmellText,
        string VagueSmellText,
        string DisplayColour,
        IReadOnlyList<AnimalVenomEffectTemplate> Effects
    );

    internal sealed record AnimalVenomAttackTemplate(
        string AttackNameSuffix,
        string AttackShapeName,
        string[] TargetBodypartAliases,
        string CombatMessage,
        DamageType DamageType,
        string VenomProfileKey,
        double MaximumQuantity,
        int MinimumWoundSeverity,
        BuiltInCombatMoveType MoveType = BuiltInCombatMoveType.EnvenomingAttackClinch,
        MeleeWeaponVerb Verb = MeleeWeaponVerb.Bite,
        Alignment Alignment = Alignment.Front,
        Orientation Orientation = Orientation.Low,
        double StaminaCost = 4.0,
        double BaseDelay = 1.0
    );

    internal sealed record AnimalAttackLoadoutTemplate(
        IReadOnlyList<AnimalAttackGrant> ShapeMatchedAttacks,
        IReadOnlyList<AnimalAliasAttackGrant>? AliasAttacks = null,
        IReadOnlyList<AnimalVenomAttackTemplate>? VenomAttacks = null
    );

    internal sealed record AnimalBodyAuditProfile(
        string Key,
        AnimalBoneExpectation BoneExpectation,
        IReadOnlyList<string> RequiredBodyparts,
        IReadOnlyList<string> RequiredOrgans,
        IReadOnlyList<string> RequiredBones,
        IReadOnlyList<string> RequiredLimbs
    );

    internal sealed record AnimalRaceTemplate(
        string Name,
        string Adjective,
        string? Description,
        string BodyKey,
        SizeCategory Size,
        bool CanClimb,
        double BodypartHealthMultiplier,
        string MaleHeightWeightModel,
        string FemaleHeightWeightModel,
        string AgeProfileKey,
        string AttackLoadoutKey,
        AnimalDescriptionPack DescriptionPack,
        bool Sweats = true,
        AnimalBreathingMode BreathingMode = AnimalBreathingMode.Simple,
        string? BloodProfileKey = null,
        string? BodyAuditKey = null,
        IReadOnlyList<AnimalBodypartUsageTemplate>? AdditionalBodypartUsages = null,
        string CombatStrategyKey = "Beast Brawler",
        IReadOnlyList<SeederTattooTemplateDefinition>? TattooTemplates = null,
		double MaximumFoodSatiatedHours = RacialSatiationDefaults.MaximumFoodSatiatedHours,
		double MaximumDrinkSatiatedHours = RacialSatiationDefaults.MaximumDrinkSatiatedHours
    );

    internal static IReadOnlyDictionary<string, AnimalAgeProfileTemplate> AgeProfilesForTesting => AgeProfiles;
    internal static IReadOnlyDictionary<string, AnimalRaceTemplate> RaceTemplatesForTesting => RaceTemplates;
    internal static IReadOnlyDictionary<string, AnimalAttackLoadoutTemplate> AttackLoadoutsForTesting => AttackLoadouts;
    internal static IReadOnlyDictionary<string, AnimalHeightWeightTemplate> HeightWeightTemplatesForTesting => HeightWeightTemplates;
    internal static IReadOnlyDictionary<string, AnimalVenomProfileTemplate> VenomProfilesForTesting => VenomProfiles;
    internal static IReadOnlyDictionary<string, AnimalBodyAuditProfile> BodyAuditProfilesForTesting => BodyAuditProfiles;

    private static readonly IReadOnlyDictionary<string, AnimalAgeProfileTemplate> AgeProfiles =
        new ReadOnlyDictionary<string, AnimalAgeProfileTemplate>(
            new Dictionary<string, AnimalAgeProfileTemplate>(StringComparer.OrdinalIgnoreCase)
            {
                ["tiny-fast"] = new(1, 2, 3, 4, 5, 6),
                ["standard-mammal"] = new(1, 2, 3, 7, 12, 15),
                ["stock-mammal"] = new(1, 3, 5, 9, 14, 20),
                ["large-hooved"] = new(1, 3, 5, 9, 16, 25),
                ["elephant"] = new(2, 5, 8, 12, 35, 50),
                ["aquatic-bird-fish"] = new(1, 3, 5, 9, 20, 35),
                ["cetacean"] = new(2, 5, 8, 15, 45, 75),
                ["reptile"] = new(1, 2, 4, 8, 16, 24),
                ["amphibian"] = new(1, 2, 3, 5, 8, 12),
                ["arthropod"] = new(1, 2, 3, 6, 10, 14)
            }
        );

    private static readonly IReadOnlyDictionary<string, AnimalHeightWeightTemplate> HeightWeightTemplates =
        new ReadOnlyDictionary<string, AnimalHeightWeightTemplate>(
            BuildHeightWeightTemplates()
        );

    private static readonly IReadOnlyDictionary<string, AnimalAttackLoadoutTemplate> AttackLoadouts =
        new ReadOnlyDictionary<string, AnimalAttackLoadoutTemplate>(
            BuildAttackLoadouts()
        );

    private static readonly IReadOnlyDictionary<string, AnimalVenomProfileTemplate> VenomProfiles =
        new ReadOnlyDictionary<string, AnimalVenomProfileTemplate>(
            BuildVenomProfiles()
        );

    private static readonly IReadOnlyDictionary<string, AnimalBodyAuditProfile> BodyAuditProfiles =
        new ReadOnlyDictionary<string, AnimalBodyAuditProfile>(
            BuildBodyAuditProfiles()
        );

    private static readonly IReadOnlyDictionary<string, AnimalRaceTemplate> RaceTemplates =
        new ReadOnlyDictionary<string, AnimalRaceTemplate>(
            BuildRaceTemplates()
        );

	internal static (double MaximumFoodSatiatedHours, double MaximumDrinkSatiatedHours) GetAnimalSatiationLimitsForTesting(
		AnimalRaceTemplate template)
	{
		return (template.MaximumFoodSatiatedHours, template.MaximumDrinkSatiatedHours);
	}
}
