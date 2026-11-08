#nullable enable

using MudSharp.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder;

public static class SeederPackagePlan
{
    public static SeederAssessment Assess(IDatabaseSeeder seeder, FuturemudDatabaseContext context)
    {
        SeederMetadata metadata = seeder.Metadata;
        List<string> missingPrerequisites = metadata.Prerequisites
            .Where(x => !x.IsSatisfied(context))
            .Select(x => x.Description)
            .ToList();
        List<string> warnings = new();
        List<string> notes = new();
        ShouldSeedResult legacyResult = seeder.ShouldSeedData(context);

        if (!string.IsNullOrWhiteSpace(metadata.OwnershipSummary))
        {
            notes.Add(metadata.OwnershipSummary);
        }

        if (missingPrerequisites.Any() || legacyResult == ShouldSeedResult.PrerequisitesNotMet)
        {
            string explanation = missingPrerequisites.Any()
                ? $"Missing prerequisites: {string.Join("; ", missingPrerequisites)}"
                : "This package reports that its prerequisites are not currently met.";

            return new SeederAssessment(
                SeederAssessmentStatus.Blocked,
                explanation,
                missingPrerequisites,
                warnings,
                notes
            );
        }

        switch (legacyResult)
        {
            case ShouldSeedResult.ReadyToInstall:
                if (!string.IsNullOrWhiteSpace(metadata.RerunSummary))
                {
                    notes.Add(metadata.RerunSummary);
                }

                return new SeederAssessment(
                    SeederAssessmentStatus.ReadyToInstall,
                    "This package is ready to install.",
                    missingPrerequisites,
                    warnings,
                    notes
                );

            case ShouldSeedResult.ExtraPackagesAvailable:
                if (!string.IsNullOrWhiteSpace(metadata.RerunSummary))
                {
                    notes.Add(metadata.RerunSummary);
                }

                if (!string.IsNullOrWhiteSpace(metadata.UpdateSummary))
                {
                    notes.Add(metadata.UpdateSummary);
                }

                return new SeederAssessment(
                    metadata.RepeatabilityMode == SeederRepeatabilityMode.Additive
                        ? SeederAssessmentStatus.AdditiveInstallAvailable
                        : SeederAssessmentStatus.UpdateAvailable,
                    metadata.RepeatabilityMode == SeederRepeatabilityMode.Additive
                        ? "This package can add more stock content on a rerun."
                        : "This package can be rerun to install or refresh missing stock content.",
                    missingPrerequisites,
                    warnings,
                    notes
                );

            case ShouldSeedResult.MayAlreadyBeInstalled:
                if (!string.IsNullOrWhiteSpace(metadata.RerunSummary))
                {
                    notes.Add(metadata.RerunSummary);
                }

                if (!string.IsNullOrWhiteSpace(metadata.UpdateSummary))
                {
                    notes.Add(metadata.UpdateSummary);
                }

                if (metadata.RepeatabilityMode == SeederRepeatabilityMode.OneShot &&
                    metadata.UpdateCapability == SeederUpdateCapability.None)
                {
                    warnings.Add("This package appears to already be installed and rerunning it is not currently recommended.");
                }

                return new SeederAssessment(
                    SeederAssessmentStatus.InstalledCurrent,
                    metadata.RepeatabilityMode == SeederRepeatabilityMode.OneShot &&
                    metadata.UpdateCapability == SeederUpdateCapability.None
                        ? "This package appears to already be installed."
                        : "All currently detectable stock records for this package appear to be present.",
                    missingPrerequisites,
                    warnings,
                    notes
                );

            default:
                return new SeederAssessment(
                    SeederAssessmentStatus.Blocked,
                    "This package reported an unknown assessment state.",
                    missingPrerequisites,
                    warnings,
                    notes
                );
        }
    }

    /// <summary>
    /// Produces a stable installation order from the declared seeder dependencies. A dependency is only
    /// an ordering hint; the database-backed prerequisite predicates remain the source of truth for
    /// whether a package can run in a particular world.
    /// </summary>
    public static SeederDependencyPlan GetDependencyPlan(IEnumerable<IDatabaseSeeder> seeders)
    {
        Dictionary<Type, IDatabaseSeeder> seedersByType = seeders
            .GroupBy(x => x.GetType())
            .ToDictionary(x => x.Key, x => x.First());
        Dictionary<Type, HashSet<Type>> prerequisitesBySeeder = seedersByType.Keys
            .ToDictionary(x => x, _ => new HashSet<Type>());
        List<string> errors = new();

        foreach ((Type seederType, IDatabaseSeeder seeder) in seedersByType)
        {
            foreach (Type prerequisiteType in seeder.Metadata.RequiredSeederTypes
                         .Concat((seeder.Metadata.OrderAfterSeederTypes ?? Array.Empty<Type>()).Where(seedersByType.ContainsKey)).Distinct())
            {
                if (prerequisiteType == seederType)
                {
                    errors.Add($"{seeder.Name} cannot depend on itself.");
                    continue;
                }

                if (!seedersByType.ContainsKey(prerequisiteType))
                {
                    errors.Add(
                        $"{seeder.Name} declares unavailable prerequisite seeder {prerequisiteType.Name}.");
                    continue;
                }

                prerequisitesBySeeder[seederType].Add(prerequisiteType);
            }
        }

        List<Type> orderedTypes = new();
        HashSet<Type> remainingTypes = prerequisitesBySeeder.Keys.ToHashSet();
        while (remainingTypes.Any())
        {
            List<Type> readyTypes = remainingTypes
                .Where(x => prerequisitesBySeeder[x].All(prerequisite => !remainingTypes.Contains(prerequisite)))
                .OrderBy(x => seedersByType[x].SortOrder)
                .ThenBy(x => seedersByType[x].Name, StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (!readyTypes.Any())
            {
                string cycle = string.Join(", ", remainingTypes
                    .Select(x => seedersByType[x].Name)
                    .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                    .ToList());
                errors.Add($"Seeder dependency cycle detected between: {cycle}.");
                orderedTypes.AddRange(remainingTypes
                    .OrderBy(x => seedersByType[x].SortOrder)
                    .ThenBy(x => seedersByType[x].Name, StringComparer.OrdinalIgnoreCase));
                break;
            }

            orderedTypes.AddRange(readyTypes);
            remainingTypes.ExceptWith(readyTypes);
        }

        return new SeederDependencyPlan(
            orderedTypes.Select(x => seedersByType[x]).ToList(),
            errors);
    }

}
