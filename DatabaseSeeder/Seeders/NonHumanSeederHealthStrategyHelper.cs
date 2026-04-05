#nullable enable

using System;
using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

internal static class NonHumanSeederHealthStrategyHelper
{
    internal const string HpStrategyName = "Non-Human HP";
    internal const string HpPlusStrategyName = "Non-Human HP Plus";
    internal const string FullStrategyName = "Non-Human Full Model";

    internal static IReadOnlyList<string> AllStrategyNames { get; } =
    [
        HpStrategyName,
        HpPlusStrategyName,
        FullStrategyName
    ];

    internal static string GetStrategyName(string choice)
    {
        return choice.Trim().ToLowerInvariant() switch
        {
            "hp" => HpStrategyName,
            "hpplus" => HpPlusStrategyName,
            "full" => FullStrategyName,
            _ => throw new InvalidOperationException($"Unknown non-human health model choice {choice}.")
        };
    }
}
