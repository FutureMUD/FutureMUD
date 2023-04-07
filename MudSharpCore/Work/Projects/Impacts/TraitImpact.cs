using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Models;

namespace MudSharp.Work.Projects.Impacts;

public class TraitImpact : BaseImpact, ILabourImpactTraits
{
	public TraitImpact(ProjectLabourImpact impact, IFuturemud gameworld) : base(impact, gameworld)
	{
		var root = XElement.Parse(impact.Definition);
		Trait = Gameworld.Traits.Get(long.Parse(root.Element("Trait").Value));
		TraitChange = double.Parse(root.Element("TraitChange").Value);
		TraitBonusContext = (TraitBonusContext)int.Parse(root.Element("TraitBonusContext").Value);
	}

	public TraitImpact(TraitImpact rhs, IProjectLabourRequirement newLabour) : base(rhs, newLabour, "trait")
	{
		Trait = rhs.Trait;
		TraitChange = rhs.TraitChange;
		TraitBonusContext = rhs.TraitBonusContext;
		Changed = true;
	}

	public TraitImpact(IProjectLabourRequirement requirement, string name) : base(requirement, "trait", name)
	{
	}

	public ITraitDefinition Trait { get; protected set; }

	public double TraitChange { get; protected set; }

	public TraitBonusContext TraitBonusContext { get; protected set; }

	public double EffectOnTrait(ITrait trait, TraitBonusContext context)
	{
		if (Trait != trait?.Definition)
		{
			return 0.0;
		}

		if (TraitBonusContext != TraitBonusContext.None && TraitBonusContext != context)
		{
			return 0.0;
		}

		return TraitChange;
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Impact",
			new XElement("Trait", Trait?.Id ?? 0),
			new XElement("TraitChange", TraitChange),
			new XElement("TraitBonusContext", (int)TraitBonusContext)
		);
	}

	public override ILabourImpact Duplicate(IProjectLabourRequirement requirement)
	{
		return new TraitImpact(this, requirement);
	}

	#region Overrides of BaseImpact

	protected override string HelpText => $@"{base.HelpText}
	#3trait <trait>#0 - sets the skill or attribute affected
	#3bonus <bonus>#0 - sets the bonus to the trait. Negative for penalties
	#3context#0 - shows the valid values for <context>
	#3context <context>#0 - sets the context that this bonus applies in
	#3context none#0 - clears the bonus context";

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
			case "context":
				return BuildingCommandContext(actor, command);
		}

		return base.BuildingCommand(actor, command.GetUndo(), requirement);
	}

	private bool BuildingCommandContext(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What context should this bonus apply in? Use 'none' to apply always.\nValid values are {Enum.GetNames(typeof(TraitBonusContext)).Select(x => StringColourExtensions.ColourCommand(x)).ListToString()}.");
			return false;
		}

		if (!Enum.TryParse<TraitBonusContext>(command.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send(
				$"That is not a valid context.\nValid values are {Enum.GetNames(typeof(TraitBonusContext)).Select(x => x.ColourCommand()).ListToString()}.");
			return false;
		}

		TraitBonusContext = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This bonus will now only apply in the {TraitBonusContext.DescribeEnum().ColourCommand()} context.");
		return true;
	}

	private bool BuildingCommandBonus(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What bonus should be applied to the trait? Use negative numbers for penalties.");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send("You must enter a valid number for the trait bonus.");
			return false;
		}

		TraitChange = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This impact will now impose a change of {TraitChange.ToString("N2", actor).ColourValue()} to the impacted trait.");
		return true;
	}

	private bool BuildingCommandTrait(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which trait do you want to give a bonus or penalty to with this impact?");
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
		sb.AppendLine($"Bonus: {TraitChange.ToBonusString(actor)}");
		sb.AppendLine($"Context: {TraitBonusContext.DescribeEnum(false, Telnet.Cyan)}");
		return sb.ToString();
	}

	public override string ShowFull(ICharacter actor)
	{
		if (TraitBonusContext == TraitBonusContext.None)
		{
			return
				$"{"[TraitImpact]".Colour(Telnet.Magenta)} {TraitChange.ToBonusString(actor)} to {Trait?.Name.ColourValue() ?? "None".Colour(Telnet.Red)}";
		}

		return
			$"{"[TraitImpact]".Colour(Telnet.Magenta)} {TraitChange.ToBonusString(actor)} to {Trait?.Name.ColourValue() ?? "None".Colour(Telnet.Red)} [{TraitBonusContext.DescribeEnum().Colour(Telnet.Cyan)} only]";
	}

	public override string ShowToPlayer(ICharacter actor)
	{
		if (TraitBonusContext == TraitBonusContext.None)
		{
			return $"A {(TraitChange >= 0.0 ? "bonus" : "penalty")} to {Trait.Name.ColourValue()}";
		}

		return
			$"A {(TraitChange >= 0.0 ? "bonus" : "penalty")} to {Trait.Name.ColourValue()} (in {TraitBonusContext.DescribeEnum().ColourName()} context)";
	}
}