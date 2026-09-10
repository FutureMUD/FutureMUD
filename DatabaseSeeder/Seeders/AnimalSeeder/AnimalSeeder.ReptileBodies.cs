#nullable enable

using MudSharp.Models;
using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

public partial class AnimalSeeder
{
    private void SeedReptilianBodies(BodyProto toedQuadruped, BodyProto reptilianProto, BodyProto anuranProto)
    {
        string[] reptileExclusions = new[]
        {
            "rwingbase",
            "lwingbase",
            "rwing",
            "lwing",
            "udder",
            "rhorn",
            "lhorn",
            "horn",
            "rantler",
            "lantler",
            "rtusk",
            "ltusk",
            "rrdewclaw",
            "lrdewclaw"
        };
        CloneBodyDefinition(toedQuadruped, reptilianProto, reptileExclusions, cloneAdditionalUsages: false);

        HashSet<string> anuranExclusions = new(reptileExclusions, System.StringComparer.OrdinalIgnoreCase)
        {
            "utail",
            "mtail",
            "ltail"
        };
        CloneBodyDefinition(toedQuadruped, anuranProto, anuranExclusions, cloneAdditionalUsages: false);

        AuditBody(reptilianProto, "reptilian");
        AuditBody(anuranProto, "anuran");
    }
}
