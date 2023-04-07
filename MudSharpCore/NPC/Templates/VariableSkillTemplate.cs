using MudSharp.Body.Traits;

namespace MudSharp.NPC.Templates;

public class VariableSkillTemplate
{
	public ITraitDefinition Trait { get; set; }
	public double Chance { get; set; }
	public double SkillMean { get; set; }
	public double SkillStddev { get; set; }
}