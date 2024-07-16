using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace MudSharp.Body.Traits.Improvement;

public abstract class ImprovementModel : SaveableItem, IImprovementModel
{
	public sealed override string FrameworkItemType => "ImprovementModel";

	public abstract double GetImprovement(IHaveTraits person, ITrait trait, Difficulty difficulty, Outcome outcome,
		TraitUseType usetype);

	public abstract bool CanImprove(IHaveTraits person, ITrait trait, Difficulty difficulty, TraitUseType useType,
		bool ignoreTemporaryBlockers);

	public static ImprovementModel LoadModel(Improver improver, IFuturemud gameworld)
	{
		switch (improver.Type)
		{
			case "classic":
				return new ClassicImprovement(gameworld, improver);
			case "non-improving":
				return new NonImproving(gameworld, improver.Id);
			case "theoretical":
				return new TheoreticalImprovementModel(gameworld, improver);
			case "branching":
				return new BranchingImprover(improver, gameworld);
			default:
				throw new NotSupportedException();
		}
	}

	public abstract IImprovementModel Clone(string name);

	public string HelpText => $@"You can use the following options with this command:

	#3name <name>#0 - renames the improvement model{SubtypeHelpText}";
	protected abstract string SubtypeHelpText { get; }

	protected abstract XElement SaveDefinition();

	/// <inheritdoc />
	public override void Save()
	{
		var dbitem = FMDB.Context.Improvers.Find(Id);
		dbitem.Name = Name;
		dbitem.Definition = SaveDefinition().ToString();
		Changed = false;
	}

	/// <inheritdoc />
	public virtual bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to this improver?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.ImprovementModels.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already an improvement model called {name.ColourName()}. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the improvement model from {Name.ColourName()} to {name.ColourName()}.");
		Changed = true;
		_name = name;
		return true;
	}

	/// <inheritdoc />
	public abstract string Show(ICharacter actor);

	public abstract string ImproverType { get; }
}