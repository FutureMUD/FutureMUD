#nullable enable

using System.Collections.Generic;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class AnimalSeeder
{
	private void SeedReptilianBodies(BodyProto toedQuadruped, BodyProto reptilianProto, BodyProto anuranProto)
	{
		var reptileExclusions = new[]
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

		var anuranExclusions = new HashSet<string>(reptileExclusions, System.StringComparer.OrdinalIgnoreCase)
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
