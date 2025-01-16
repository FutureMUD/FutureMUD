using System.Text;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Models;

namespace MudSharp.Work.Projects.Impacts;

public class TraitCapImpact : BaseImpact, ILabourImpactTraitCaps
{
	public TraitCapImpact(ProjectLabourImpact impact, IFuturemud gameworld) : base(impact, gameworld)
	{
		var root = XElement.Parse(impact.Definition);
		Trait = Gameworld.Traits.Get(long.Parse(root.Element("Trait").Value));
		TraitCapChange = double.Parse(root.Element("TraitChange").Value);
	}

	public TraitCapImpact(TraitCapImpact rhs, IProjectLabourRequirement newLabour) : base(rhs, newLabour, "trait cap")
	{
		Trait = rhs.Trait;
		TraitCapChange = rhs.TraitCapChange;
		Changed = true;
	}

	public TraitCapImpact(IProjectLabourRequirement requirement, string name) : base(requirement, "trait cap", name)
	{
	}

	public ITraitDefinition Trait { get; protected set; }

	public double TraitCapChange { get; protected set; }

	public double EffectOnTrait(ITraitDefinition trait)
	{
		if (Trait != trait)
		{
			return 0.0;
		}

		return TraitCapChange;
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Impact",
			new XElement("Trait", Trait?.Id ?? 0),
			new XElement("TraitChange", TraitCapChange)
		);
	}

	public override ILabourImpact Duplicate(IProjectLabourRequirement requirement)
	{
		return new TraitCapImpact(this, requirement);
	}

	#region Overrides of BaseImpact

	protected override string HelpText => $@"{base.HelpText}
	#3trait <trait>#0 - sets the skill or attribute to be affected.
	#3bonus <bonus>#0 - sets the bonus to be applied. Negative for penalties.";

	#endregion

	public override bool BuildingCommand(ICharacter actor, StringStack command, IProjectLabourRequirement requirement)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "trait":
			case "skill":
				return BuildingCommandTrait(actor, command);
			case "bonus":
			case "change":
			case "trait bonus":
			case "trait change":
			case "trait_bonus":
			case "trait_change":
				return BuildingCommandBonus(actor, command);
		}

		return base.BuildingCommand(actor, command.GetUndo(), requirement);
	}

	private bool BuildingCommandBonus(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What bonus should be applied to the trait cap? Use negative numbers for penalties.");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send("You must enter a valid number for the trait cap bonus.");
			return false;
		}

		TraitCapChange = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This impact will now impose a change of {TraitCapChange.ToString("N2", actor).ColourValue()} to the impacted trait's cap.");
		return true;
	}

	private bool BuildingCommandTrait(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which trait do you want to give a cap bonus or cap penalty to with this impact?");
			return false;
		}

		var trait = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.Traits.Get(value)
			: Gameworld.Traits.GetByName(command.Last);
		if (trait == null)
		{
			actor.OutputHandler.Send("There is no such trait.");
			return false;
		}

		Trait = trait;
		Changed = true;
		actor.OutputHandler.Send($"This impact will now affect the {trait.Name.ColourValue()} trait.");
		return true;
	}

	public override (bool Truth, string Error) CanSubmit()
	{
		if (Trait == null)
		{
			return (false, "You must specify a trait for all trait impacts.");
		}

		return base.CanSubmit();
	}

	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.Append(base.Show(actor));
		sb.AppendLine($"Trait: {Trait?.Name.ColourValue() ?? "None".ColourError()}");
		sb.AppendLine($"Bonus: {TraitCapChange.ToBonusString(actor)}");
		return sb.ToString();
	}

	public override string ShowFull(ICharacter actor)
	{
		return
			$"{"[TraitImpact]".Colour(Telnet.Magenta)} {TraitCapChange.ToBonusString(actor, 2)} to {Trait?.Name.ColourValue() ?? "None".Colour(Telnet.Red)} cap";
	}

	public override string ShowToPlayer(ICharacter actor)
	{
		return $"A {(TraitCapChange >= 0.0 ? "bonus" : "penalty")} to the cap for {Trait.Name.ColourValue()}";
	}
}