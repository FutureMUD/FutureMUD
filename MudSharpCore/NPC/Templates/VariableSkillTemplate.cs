using MudSharp.Body.Traits;

namespace MudSharp.NPC.Templates;

public class VariableSkillTemplate
{
    public ITraitDefinition Trait { get; init; }
    public double Chance { get; init; }
    public double SkillMean { get; init; }
    public double SkillStddev { get; init; }
	public double SkillSkewness { get; init; }

	public static VariableSkillTemplate LoadFromXml(System.Xml.Linq.XElement element,
		System.Func<long, ITraitDefinition> traitResolver)
	{
		return new VariableSkillTemplate
		{
			Chance = double.Parse(element.Attribute("Chance")!.Value),
			SkillMean = double.Parse(element.Attribute("Mean")!.Value),
			SkillStddev = double.Parse(element.Attribute("Stddev")!.Value),
			SkillSkewness = double.Parse(element.Attribute("Skewness")?.Value ?? "0"),
			Trait = traitResolver(long.Parse(element.Attribute("Trait")!.Value))
		};
	}

	public System.Xml.Linq.XElement SaveToXml()
	{
		return new System.Xml.Linq.XElement("Skill",
			new System.Xml.Linq.XAttribute("Chance", Chance),
			new System.Xml.Linq.XAttribute("Mean", SkillMean),
			new System.Xml.Linq.XAttribute("Stddev", SkillStddev),
			new System.Xml.Linq.XAttribute("Skewness", SkillSkewness),
			new System.Xml.Linq.XAttribute("Trait", Trait.Id));
	}
}
