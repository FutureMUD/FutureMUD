using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

#nullable enable
namespace MudSharp.CharacterCreation;

/// <summary>Authoritative acquisition claims. Boost intent remains in SelectedSkillBoosts.</summary>
public sealed class ChargenSkillClaims
{
	public bool Initialised { get; set; }
	public bool SuggestionsApplied { get; set; }
	public HashSet<long> Mandatory { get; } = [];
	public HashSet<long> Independent { get; } = [];
	public Dictionary<long, double> IndependentValues { get; } = [];
	public HashSet<long> Ordinary { get; } = [];
	public Dictionary<string, ChargenSkillGroupClaim> Groups { get; } = [];
	public IEnumerable<long> GroupSkills => Groups.Values.SelectMany(x => x.Skills).Distinct();
	public IEnumerable<long> Selected => Independent.Concat(Mandatory).Concat(Ordinary).Concat(GroupSkills).Distinct();
	public IEnumerable<long> ChargeableOrdinary => Ordinary.Except(Independent).Except(Mandatory).Except(GroupSkills);

	public XElement Save() => new("SkillClaims", new XAttribute("initialised", Initialised),
		new XAttribute("suggestions", SuggestionsApplied),
		new XElement("Independent", Independent.Select(x => new XElement("Skill", x))),
		new XElement("IndependentValues", IndependentValues.Select(x => new XElement("Skill", new XAttribute("id", x.Key), x.Value))),
		new XElement("Mandatory", Mandatory.Select(x => new XElement("Skill", x))),
		new XElement("Ordinary", Ordinary.Select(x => new XElement("Skill", x))),
		Groups.Select(x => new XElement("Group", new XAttribute("key", x.Key),
			new XAttribute("id", x.Value.GroupId), new XAttribute("revision", x.Value.Revision),
			new XAttribute("complete", x.Value.Complete),
			x.Value.Skills.Select(y => new XElement("Skill", new XAttribute("credit", x.Value.Credits.Contains(y)), y)))));

	public static ChargenSkillClaims Load(XElement? root)
	{
		var result = new ChargenSkillClaims();
		if (root is null) return result;
		result.Initialised = (bool?)root.Attribute("initialised") ?? false;
		result.SuggestionsApplied = (bool?)root.Attribute("suggestions") ?? false;
		result.Independent.UnionWith(root.Element("Independent")?.Elements("Skill").Select(x => (long)x) ?? []);
		foreach (var skill in root.Element("IndependentValues")?.Elements("Skill") ?? []) result.IndependentValues[(long)skill.Attribute("id")!] = (double)skill;
		result.Mandatory.UnionWith(root.Element("Mandatory")?.Elements("Skill").Select(x => (long)x) ?? []);
		result.Ordinary.UnionWith(root.Element("Ordinary")?.Elements("Skill").Select(x => (long)x) ?? []);
		foreach (var group in root.Elements("Group"))
		{
			var claim = new ChargenSkillGroupClaim { GroupId = (long)group.Attribute("id")!,
				Revision = (int)group.Attribute("revision")!, Complete = (bool)group.Attribute("complete")! };
			claim.Skills.UnionWith(group.Elements("Skill").Select(x => (long)x));
			claim.Credits.UnionWith(group.Elements("Skill").Where(x => (bool?)x.Attribute("credit") == true).Select(x => (long)x));
			result.Groups.Add((string)group.Attribute("key")!, claim);
		}
		return result;
	}
}

public sealed class ChargenSkillGroupClaim
{
	public long GroupId { get; set; }
	public int Revision { get; set; }
	public bool Complete { get; set; }
	public HashSet<long> Skills { get; } = [];
	public HashSet<long> Credits { get; } = [];
}
