using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class SkillLearningMerit : CharacterMeritBase, ITraitLearningMerit
{
	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Skill Learning",
			(merit, gameworld) => new SkillLearningMerit(merit, gameworld));
	}

	protected SkillLearningMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		var element = definition.Element("Groups");
		if (element != null)
		{
			foreach (var item in element.Elements("Group"))
			{
				Groups.Add(item.Value);
			}
		}

		BranchingModifier = double.Parse(definition.Attribute("branching")?.Value ?? "1.0");
		LearningModifier = double.Parse(definition.Attribute("improving")?.Value ?? "1.0");
		MinimumDifficultyForModifier = (Difficulty)int.Parse(definition.Attribute("min_difficulty")?.Value ??
		                                                     ((int)Difficulty.Automatic).ToString());
		MaximumDifficultyForModifier = (Difficulty)int.Parse(definition.Attribute("max_difficulty")?.Value ??
		                                                     ((int)Difficulty.Impossible).ToString());
	}

	public List<string> Groups { get; } = new();
	public double BranchingModifier { get; set; }
	public double LearningModifier { get; set; }
	public Difficulty MinimumDifficultyForModifier { get; set; }
	public Difficulty MaximumDifficultyForModifier { get; set; }

	public double BranchingChanceModifier(IPerceivableHaveTraits ch, ITraitDefinition trait)
	{
		return !Groups.Any() || Groups.Any(x => x.EqualTo(trait.Group)) ? BranchingModifier : 1.0;
	}

	public double SkillLearningChanceModifier(IPerceivableHaveTraits ch, ITraitDefinition trait, Outcome outcome,
		Difficulty difficulty, TraitUseType useType)
	{
		return (!Groups.Any() || Groups.Any(x => x.EqualTo(trait.Group))) &&
		       difficulty >= MinimumDifficultyForModifier &&
		       difficulty <= MaximumDifficultyForModifier
			? LearningModifier
			: 1.0;
	}
}