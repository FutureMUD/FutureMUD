using System.Collections.Generic;
using System.Xml.Linq;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Body.Traits.Improvement;

public class BranchingImprover : ClassicImprovement
{
	public BranchingImprover(Models.Improver improver, IFuturemud gameworld) : base(improver)
	{
		var root = XElement.Parse(improver.Definition);
		foreach (var element in root.Element("Branches").Elements())
		{
			var trait = gameworld.Traits.Get(long.Parse(element.Attribute("base").Value));
			var branch = gameworld.Traits.Get(long.Parse(element.Attribute("branch").Value));
			if (trait != null && branch != null)
			{
				BranchMap.Add((trait, double.Parse(element.Attribute("on").Value),
					double.Parse(element.Attribute("at").Value), branch));
			}
		}
	}

	public List<(ITraitDefinition BaseTrait, double TraitValue, double OpenValue, ITraitDefinition BranchTrait)>
		BranchMap { get; } = new();

	public override double GetImprovement(IHaveTraits person, ITrait trait, Difficulty difficulty, Outcome outcome,
		TraitUseType usetype)
	{
		var result = base.GetImprovement(person, trait, difficulty, outcome, usetype);
		foreach (var item in BranchMap)
		{
			if (person.TraitRawValue(item.BaseTrait) >= item.TraitValue && !person.HasTrait(item.BranchTrait))
			{
				person.AddTrait(item.BranchTrait, item.OpenValue);
				trait.Gameworld.LogManager.CustomLogEntry(Logging.LogEntryType.SkillBranch, person, item.BranchTrait,
					Outcome.MajorPass);
			}
		}

		return result;
	}
}