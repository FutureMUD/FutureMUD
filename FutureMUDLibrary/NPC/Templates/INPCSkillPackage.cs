#nullable enable

using System.Collections.Generic;
using MudSharp.Body.Traits;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;

namespace MudSharp.NPC.Templates;

public sealed record NPCSkillPackageEntry(
	ITraitDefinition Skill,
	double Chance,
	double Mean,
	double StandardDeviation,
	double Skewness)
{
	public double WeightedExpectedValue => Chance * Mean;
}

public sealed record NPCSkillPackageApplicationResult(
	int Added,
	int Raised,
	int Replaced,
	int Skipped,
	int FailedChance)
{
	public int Changed => Added + Raised + Replaced;
	public static NPCSkillPackageApplicationResult Empty { get; } = new(0, 0, 0, 0, 0);
}

public interface INPCSkillPackage : IEditableItem, IProgVariable
{
	IReadOnlyCollection<NPCSkillPackageEntry> Skills { get; }
	INPCSkillPackage Clone(string name);
}
