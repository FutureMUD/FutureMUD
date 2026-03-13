#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class AnimalSeeder
{
	private void CloneBodyDefinition(BodyProto source, BodyProto target, IEnumerable<string>? excludedAliases = null,
		bool cloneAdditionalUsages = true)
	{
		SeederBodyUtilities.CloneBodyDefinition(_context, source, target, excludedAliases, cloneAdditionalUsages);
	}

	private void CloneBodyPositionsAndSpeeds(BodyProto source, BodyProto target)
	{
		SeederBodyUtilities.CloneBodyPositionsAndSpeeds(_context, source, target);
	}
}
