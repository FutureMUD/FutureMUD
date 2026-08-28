using System.Collections.Generic;

namespace MudSharp.Models;

public partial class NpcSkillPackage
{
	public NpcSkillPackage()
	{
		Skills = new HashSet<NpcSkillPackageSkill>();
		Races = new HashSet<Race>();
	}

	public long Id { get; set; }
	public string Name { get; set; }

	public virtual ICollection<NpcSkillPackageSkill> Skills { get; set; }
	public virtual ICollection<Race> Races { get; set; }
}
