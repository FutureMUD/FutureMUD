#nullable enable

using MudSharp.Character.Name;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class RobotSeeder
{
    internal static string RobotCultureDescriptionForTesting =>
        SeederDescriptionHelpers.JoinParagraphs(
            "The stock Robot culture represents machine societies or service populations that identify through designation, chassis role, serial nickname or storied moniker rather than through inherited family structures.",
            "It assumes a culture shaped by manufacture, maintenance, duty cycles and networked memory. Even when individual robots develop personality, the baseline remains one of designed purpose, serial continuity and practical naming conventions that suit records as much as conversation.",
            "For builders and players, this culture is the bridge between anonymous machine stock and recognisable robotic identity. It supports sentient constructs that need social texture without forcing them into an organic model of kinship, lineage or personal presentation."
        );

    private static readonly IReadOnlyDictionary<Gender, string[]> RobotNameProfiles =
        new Dictionary<Gender, string[]>
        {
            [Gender.Male] = ["Daneel", "Giskard", "Andrew", "Marvin", "Tik-Tok", "Talos", "Sonny", "Speedy", "Robbie", "Cutie", "Herbie", "Roy"],
            [Gender.Female] = ["Daneel", "Giskard", "Andrew", "Marvin", "Tik-Tok", "Talos", "Sonny", "Speedy", "Robbie", "Cutie", "Herbie", "Roy"],
            [Gender.Neuter] = ["Daneel", "Giskard", "Andrew", "Marvin", "Tik-Tok", "Dorfl", "Talos", "Sonny", "Speedy", "Robbie", "Cutie", "Herbie", "Roy"],
            [Gender.NonBinary] = ["Daneel", "Giskard", "Andrew", "Marvin", "Tik-Tok", "Dorfl", "Sonny", "Speedy", "Robbie", "Cutie", "Herbie", "Roy"],
            [Gender.Indeterminate] = ["Daneel", "Giskard", "Andrew", "Marvin", "Tik-Tok", "Dorfl", "Sonny", "Speedy", "Robbie", "Cutie", "Herbie", "Roy"]
        };

    private void SeedRobotCulture()
    {
        NameCulture simpleNameCulture = _context.NameCultures.First(x => x.Name == "Simple");
        _robotCulture = _context.Cultures.FirstOrDefault(x => x.Name == "Robot") ?? new Culture
        {
            Name = "Robot",
            Description = RobotCultureDescriptionForTesting,
            PersonWordMale = "Robot",
            PersonWordFemale = "Robot",
            PersonWordNeuter = "Robot",
            PersonWordIndeterminate = "Robot",
            SkillStartingValueProg = _context.FutureProgs.First(x => x.FunctionName == "AlwaysZero"),
            AvailabilityProg = _alwaysTrue,
            TolerableTemperatureCeilingEffect = 0,
            TolerableTemperatureFloorEffect = 0,
            PrimaryCalendarId = _context.Calendars.First().Id
        };
        if (_robotCulture.Id == 0)
        {
            _context.Cultures.Add(_robotCulture);
            _context.SaveChanges();
        }

        foreach (Gender gender in Enum.GetValues<Gender>())
        {
            if (_context.CulturesNameCultures.Any(x =>
                    x.CultureId == _robotCulture.Id &&
                    x.NameCultureId == simpleNameCulture.Id &&
                    x.Gender == (short)gender))
            {
                continue;
            }

            _context.CulturesNameCultures.Add(new CulturesNameCultures
            {
                Culture = _robotCulture,
                NameCulture = simpleNameCulture,
                Gender = (short)gender
            });
        }

        foreach ((Gender gender, string[]? names) in RobotNameProfiles)
        {
            EnsureRobotRandomNameProfile(simpleNameCulture, gender, names);
        }

        _context.SaveChanges();
    }

    private void ApplyRobotNameCultures(Ethnicity ethnicity)
    {
        NameCulture simpleNameCulture = _context.NameCultures.First(x => x.Name == "Simple");
        foreach (Gender gender in Enum.GetValues<Gender>())
        {
            if (_context.EthnicitiesNameCultures.Any(x =>
                    x.EthnicityId == ethnicity.Id &&
                    x.NameCultureId == simpleNameCulture.Id &&
                    x.Gender == (short)gender))
            {
                continue;
            }

            _context.EthnicitiesNameCultures.Add(new EthnicitiesNameCultures
            {
                Ethnicity = ethnicity,
                NameCulture = simpleNameCulture,
                Gender = (short)gender
            });
        }

        _context.SaveChanges();
    }

    private void EnsureRobotRandomNameProfile(NameCulture nameCulture, Gender gender, IEnumerable<string> names)
    {
        string profileName = $"Robot {gender.DescribeEnum()}";
        RandomNameProfile? profile = _context.RandomNameProfiles.FirstOrDefault(x => x.Name == profileName);
        if (profile is null)
        {
            profile = new RandomNameProfile
            {
                Name = profileName,
                Gender = (int)gender,
                NameCulture = nameCulture,
                UseForChargenSuggestionsProg = _alwaysTrue
            };
            _context.RandomNameProfiles.Add(profile);
            _context.SaveChanges();
        }

        if (!_context.RandomNameProfilesDiceExpressions.Any(x =>
                x.RandomNameProfileId == profile.Id &&
                x.NameUsage == (int)NameUsage.BirthName))
        {
            _context.RandomNameProfilesDiceExpressions.Add(new RandomNameProfilesDiceExpressions
            {
                RandomNameProfile = profile,
                NameUsage = (int)NameUsage.BirthName,
                DiceExpression = "1d1"
            });
        }

        foreach (string name in names)
        {
            if (_context.RandomNameProfilesElements.Any(x =>
                    x.RandomNameProfileId == profile.Id &&
                    x.NameUsage == (int)NameUsage.BirthName &&
                    x.Name == name))
            {
                continue;
            }

            _context.RandomNameProfilesElements.Add(new RandomNameProfilesElements
            {
                RandomNameProfile = profile,
                NameUsage = (int)NameUsage.BirthName,
                Name = name,
                Weighting = 1
            });
        }
    }
}
