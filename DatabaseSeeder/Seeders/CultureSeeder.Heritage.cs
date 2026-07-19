#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Database;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System;

namespace DatabaseSeeder.Seeders;

public partial class CultureSeeder
{
	private static readonly string[] OptionalHumanCharacteristicDefinitions = ["Distinctive Feature"];

	internal static IReadOnlyCollection<string> OptionalHumanCharacteristicDefinitionsForTesting =>
		OptionalHumanCharacteristicDefinitions;

	private static readonly IReadOnlyDictionary<string, (double MaximumFoodSatiatedHours, double MaximumDrinkSatiatedHours)> CultureRaceSatiationLimits =
		new Dictionary<string, (double MaximumFoodSatiatedHours, double MaximumDrinkSatiatedHours)>(StringComparer.OrdinalIgnoreCase)
		{
			["Elf"] = SatiationLimitSeederHelper.MaximumLimitsForCadence(24.0, 12.0),
			["Hobbit"] = SatiationLimitSeederHelper.MaximumLimitsForCadence(8.0, 5.0),
			["Dwarf"] = SatiationLimitSeederHelper.MaximumLimitsForCadence(18.0, 9.0),
			["Orc"] = SatiationLimitSeederHelper.MaximumLimitsForCadence(8.0, 5.0),
			["Troll"] = SatiationLimitSeederHelper.MaximumLimitsForCadence(6.0, 4.0)
		};

	internal static IReadOnlyDictionary<string, (double MaximumFoodSatiatedHours, double MaximumDrinkSatiatedHours)> CultureRaceSatiationLimitsForTesting =>
		CultureRaceSatiationLimits;

    private readonly Dictionary<string, FutureProg> _cultureProgs =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly Dictionary<string, Culture> _cultures = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Ethnicity> _ethnicities = new(StringComparer.OrdinalIgnoreCase);

    private readonly Dictionary<string, FutureProg> _ethnicProgs =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly Dictionary<string, FutureProg> _raceProgs =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly Dictionary<string, Race> _races = new(StringComparer.OrdinalIgnoreCase);

	private static bool HasCultureRaceSatiationLimitUpdates(FuturemudDatabaseContext context)
	{
		return CultureRaceSatiationLimits
			.Select(x => (Race: context.Races.FirstOrDefault(race => race.Name == x.Key), Limits: x.Value))
			.Any(x => x.Race is not null &&
			          !SatiationLimitSeederHelper.MatchesLimits(
				          x.Race,
				          x.Limits.MaximumFoodSatiatedHours,
				          x.Limits.MaximumDrinkSatiatedHours));
	}

	private void RefreshExistingCultureRaceSatiationLimits()
	{
		bool dirty = false;
		foreach ((string raceName, (double MaximumFoodSatiatedHours, double MaximumDrinkSatiatedHours) limits) in CultureRaceSatiationLimits)
		{
			Race? race = _context.Races.FirstOrDefault(x => x.Name == raceName);
			if (race is null)
			{
				continue;
			}

			dirty |= SatiationLimitSeederHelper.ApplyLimits(
				race,
				limits.MaximumFoodSatiatedHours,
				limits.MaximumDrinkSatiatedHours);
		}

		if (dirty)
		{
			_context.SaveChanges();
		}
	}

    internal static IReadOnlyDictionary<string, NonHumanAttributeProfile> CultureRaceAttributeProfilesForTesting =>
        CultureRaceAttributeProfiles;

    private static readonly IReadOnlyDictionary<string, NonHumanAttributeProfile> CultureRaceAttributeProfiles =
        new Dictionary<string, NonHumanAttributeProfile>(StringComparer.OrdinalIgnoreCase)
        {
            ["Elf"] = new(-1, 0, 2, 3, PerceptionBonus: 2, AuraBonus: 1),
            ["Hobbit"] = new(-3, 2, 1, 2, WillpowerBonus: 1, AuraBonus: 1),
            ["Dwarf"] = new(2, 4, -1, 0, WillpowerBonus: 3),
            ["Orc"] = new(3, 2, 0, -1, WillpowerBonus: 2, PerceptionBonus: 1, AuraBonus: -1),
            ["Troll"] = new(9, 8, -3, -4, WillpowerBonus: 4, PerceptionBonus: -1, AuraBonus: -2)
        };

    public void AddEthnicity(Race race, string name, string group, string bloodGroup,
        double tempFloor = 0.0, double tempCeiling = 0.0, string subgroup = "", FutureProg? available = null,
        string description = "")
    {
        EnsureEthnicity(race, name, group, bloodGroup, tempFloor, tempCeiling, subgroup, available, description);
    }

    private void AddRaceAttributeAlterations(Race race, NonHumanAttributeProfile profile)
    {
        foreach (TraitDefinition attribute in _context.TraitDefinitions
                     .Where(x => x.Type == (int)TraitType.Attribute || x.Type == (int)TraitType.DerivedAttribute)
                     .ToList())
        {
            RacesAttributes? alteration = _context.RacesAttributes
                .FirstOrDefault(x => x.RaceId == race.Id && x.AttributeId == attribute.Id);
            if (alteration is null)
            {
                _context.RacesAttributes.Add(new RacesAttributes
                {
                    Race = race,
                    Attribute = attribute,
                    IsHealthAttribute = attribute.TraitGroup == "Physical",
                    AttributeBonus = NonHumanAttributeScalingHelper.GetAttributeBonus(attribute, profile),
                    DiceExpression = NonHumanAttributeScalingHelper.GetAttributeDiceExpression(attribute, profile)
                });
                continue;
            }

            alteration.IsHealthAttribute = attribute.TraitGroup == "Physical";
            alteration.AttributeBonus = NonHumanAttributeScalingHelper.GetAttributeBonus(attribute, profile);
            alteration.DiceExpression = NonHumanAttributeScalingHelper.GetAttributeDiceExpression(attribute, profile);
        }
    }

    public void AddEthnicityVariable(string ethnicity, string feature, string profile)
    {
        if (feature.Equals("Humanoid Frame", StringComparison.InvariantCultureIgnoreCase) &&
            !_definitions.ContainsKey("Humanoid Frame"))
        {
            feature = "Frame";
        }

        if (!_definitions.Any(x => x.Key.Equals(feature, StringComparison.OrdinalIgnoreCase)))
        {
			if (OptionalHumanCharacteristicDefinitions.Contains(feature, StringComparer.OrdinalIgnoreCase))
			{
				return;
			}

#if DEBUG
            throw new ApplicationException($"Unknown definition {feature}");
#else
            return;
#endif
        }


        if (!_profiles.ContainsKey(profile))
        {
#if DEBUG
            throw new ApplicationException($"Unknown definition {feature}");
#else
            return;
#endif
        }


        EthnicitiesCharacteristics? existing = _ethnicities[ethnicity].EthnicitiesCharacteristics
            .FirstOrDefault(x => x.CharacteristicDefinition == _definitions[feature]);
        if (existing is not null)
        {
            if (existing.CharacteristicProfile == _profiles[profile])
            {
                return;
            }

            _context.EthnicitiesCharacteristics.Remove(existing);
        }

        _context.EthnicitiesCharacteristics.Add(new EthnicitiesCharacteristics
        {
            Ethnicity = _ethnicities[ethnicity],
            CharacteristicDefinition = _definitions[feature],
            CharacteristicProfile = _profiles[profile]
        });
    }

    public void AddCulture(string name, string nameCulture, string description, FutureProg? available = null,
        FutureProg? skillProg = null, Calendar? calendar = null)
    {
        Culture culture = EnsureCulture(name, description, available, skillProg, calendar);
        ReplaceCultureNameLinks(
            culture,
            (Gender.Male, nameCulture),
            (Gender.Female, nameCulture),
            (Gender.Neuter, nameCulture),
            (Gender.NonBinary, nameCulture),
            (Gender.Indeterminate, nameCulture));
    }

    public void AddCulture(string name, string nameCultureMale, string nameCultureFemale, string description,
        FutureProg? available = null, FutureProg? skillProg = null,
        Calendar? calendar = null)
    {
        Culture culture = EnsureCulture(name, description, available, skillProg, calendar);
        ReplaceCultureNameLinks(
            culture,
            (Gender.Male, nameCultureMale),
            (Gender.Female, nameCultureFemale),
            (Gender.Neuter, nameCultureMale),
            (Gender.NonBinary, nameCultureFemale),
            (Gender.Indeterminate, nameCultureMale));
    }
}
