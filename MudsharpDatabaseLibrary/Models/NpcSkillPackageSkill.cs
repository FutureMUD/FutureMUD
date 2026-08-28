namespace MudSharp.Models;

public partial class NpcSkillPackageSkill
{
	public long NpcSkillPackageId { get; set; }
	public long TraitDefinitionId { get; set; }
	public double Chance { get; set; }
	public double Mean { get; set; }
	public double StandardDeviation { get; set; }
	public double Skewness { get; set; }

	public virtual NpcSkillPackage NpcSkillPackage { get; set; }
	public virtual TraitDefinition TraitDefinition { get; set; }
}
