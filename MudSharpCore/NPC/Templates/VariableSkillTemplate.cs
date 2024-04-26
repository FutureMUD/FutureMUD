using MudSharp.Body.Traits;

namespace MudSharp.NPC.Templates;

public class VariableSkillTemplate
{
	public ITraitDefinition Trait { get; init; }
	public double Chance { get; init; }
	public double SkillMean { get; init; }
	public double SkillStddev { get; init; }
}