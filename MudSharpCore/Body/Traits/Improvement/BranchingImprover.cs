using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Body.Traits.Improvement;

public class BranchingImprover : ClassicImprovement
{
	public BranchingImprover(Models.Improver improver, IFuturemud gameworld) : base(gameworld, improver)
	{
	}

	protected BranchingImprover(BranchingImprover rhs, string name)
	{
		Gameworld = rhs.Gameworld;
		_name = name;
		using (new FMDB())
		{
			var definition = rhs.SaveDefinition();
			var dbitem = new Models.Improver
			{
				Name = name,
				Type = "branching",
				Definition = definition.ToString()
			};
			FMDB.Context.Improvers.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
			LoadFromXml(definition);
		}
	}

	public BranchingImprover(IFuturemud gameworld, string name)
	{
		Gameworld = gameworld;
		_name = name;
		ImprovementChance = 0.1;
		ImprovementExpression = new TraitExpression("10 - (variable/10)", gameworld);
		ImproveOnSuccess = true;
		ImproveOnFail = true;
		NoGainSecondsDiceExpression = "1d500+4000";
		DifficultyThresholdInterval = 10;
		using (new FMDB())
		{
			var dbitem = new Models.Improver
			{
				Name = name,
				Type = "branching",
				Definition = SaveDefinition().ToString()
			};
			FMDB.Context.Improvers.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	/// <inheritdoc />
	public override IImprovementModel Clone(string name)
	{
		return new BranchingImprover(this, name);
	}

	/// <inheritdoc />
	public override string ImproverType => "branching";

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

	/// <inheritdoc />
	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine(base.Show(actor));
		sb.AppendLine("Branches:");
		sb.AppendLine();
		foreach (var item in BranchMap)
		{
			sb.AppendLine($"\t{item.BranchTrait.Name.ColourName()} => {item.BaseTrait.Name.ColourName()} @ {item.TraitValue.ToString("N2", actor).ColourValue()} (Opens at {item.OpenValue.ToString("N2", actor).ColourValue()})");
		}
		return sb.ToString();
	}

	/// <inheritdoc />
	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		foreach (var element in root.Element("Branches").Elements())
		{
			var trait = Gameworld.Traits.Get(long.Parse(element.Attribute("base").Value));
			var branch = Gameworld.Traits.Get(long.Parse(element.Attribute("branch").Value));
			if (trait != null && branch != null)
			{
				BranchMap.Add((trait, double.Parse(element.Attribute("on").Value),
					double.Parse(element.Attribute("at").Value), branch));
			}
		}
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Definition",
			new XAttribute("Chance", ImprovementChance),
			new XAttribute("Expression", ImprovementExpression.OriginalFormulaText),
			new XAttribute("ImproveOnFail", ImproveOnFail),
			new XAttribute("ImproveOnSuccess", ImproveOnSuccess),
			new XAttribute("DifficultyThresholdInterval", DifficultyThresholdInterval),
			new XAttribute("NoGainSecondsDiceExpression", NoGainSecondsDiceExpression),
			new XElement("Branches",
				from item in BranchMap
				select new XElement("Branch",
					new XAttribute("base", item.BaseTrait.Id),
					new XAttribute("branch", item.BranchTrait.Id),
					new XAttribute("on", item.TraitValue),
					new XAttribute("at", item.OpenValue)
				)
			)
		);
	}

	/// <inheritdoc />
	protected override string SubtypeHelpText => $@"
	#3branch <trait> <base> <at> <open>#0 - adds a trait that branches when the base trait reaches the #6at#0 value
	#3branch <trait> remove#0 - removes a trait from branching{base.SubtypeHelpText}";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "branch":
				return BuildingCommandBranch(actor, command);
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandBranch(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which trait do you want to edit the branching settings for?");
			return false;
		}

		var trait = Gameworld.Traits.GetByIdOrName(command.PopSpeech());
		if (trait is null)
		{
			actor.OutputHandler.Send($"There is no such trait identified by the text {command.Last.ColourCommand()}.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"You must either specify a trait to branch, or use {"remove".ColourCommand()} to remove the branching of the {trait.Name.ColourName()} trait.");
			return false;
		}

		if (command.PeekSpeech().EqualTo("remove"))
		{
			if (BranchMap.All(x => x.BranchTrait != trait))
			{
				actor.OutputHandler.Send($"There is no existing branch entry for the {trait.Name.ColourName()} trait.");
				return false;
			}

			BranchMap.RemoveAll(x => x.BranchTrait == trait);
			Changed = true;
			actor.OutputHandler.Send($"You remove the branching entry for the {trait.Name.ColourName()} trait.");
			return true;
		}

		var baseTrait = Gameworld.Traits.GetByIdOrName(command.PopSpeech());
		if (baseTrait is null)
		{
			actor.OutputHandler.Send($"There is no such trait identified by the text {command.Last.ColourCommand()}.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What value of {baseTrait.Name.ColourName()} should cause the {trait.Name.ColourName()} trait to branch?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var branch))
		{
			actor.OutputHandler.Send($"The text {command.Last.ColourCommand()} is not a valid number.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What should be the opening value of the {trait.Name.ColourName()} trait when it branches?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var open))
		{
			actor.OutputHandler.Send($"The text {command.Last.ColourCommand()} is not a valid number.");
			return false;
		}

		BranchMap.RemoveAll(x => x.BranchTrait == trait);
		BranchMap.Add((baseTrait, branch, open, trait));
		Changed = true;
		actor.OutputHandler.Send($"The trait {trait.Name.ColourName()} will now branch at a value of {open.ToString("N2", actor).ColourValue()} when the {baseTrait.Name.ColourName()} trait reaches a value of {branch.ToString("N2", actor).ColourValue()}.");
		return true;
	}
}