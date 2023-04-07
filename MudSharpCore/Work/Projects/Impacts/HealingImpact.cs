using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.Work.Projects.Impacts;

public class HealingImpact : BaseImpact, ILabourImpactHealing
{
	public HealingImpact(Models.ProjectLabourImpact impact, IFuturemud gameworld) : base(impact, gameworld)
	{
		var root = XElement.Parse(impact.Definition);
		HealingCheckBonus = double.Parse(root.Element("HealingCheckBonus").Value);
		HealingRateMultiplier = double.Parse(root.Element("HealingRateMultiplier").Value);
		InfectionChanceMultiplier = double.Parse(root.Element("InfectionChanceMultiplier").Value);
	}

	public HealingImpact(HealingImpact rhs, IProjectLabourRequirement newLabour) : base(rhs, newLabour, "healing")
	{
		HealingCheckBonus = rhs.HealingCheckBonus;
		HealingRateMultiplier = rhs.HealingRateMultiplier;
		InfectionChanceMultiplier = rhs.InfectionChanceMultiplier;
		Changed = true;
	}

	public HealingImpact(IProjectLabourRequirement requirement, string name) : base(requirement, "healing", name)
	{
		HealingCheckBonus = Gameworld.GetStaticDouble("DefaultHealingImpactCheckBonus");
		HealingRateMultiplier = Gameworld.GetStaticDouble("DefaultHealingImpactRateMultiplier");
		InfectionChanceMultiplier = Gameworld.GetStaticDouble("DefaultHealingImpactInfectionMultiplier");
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Impact",
			new XElement("HealingCheckBonus", HealingCheckBonus),
			new XElement("HealingRateMultiplier", HealingRateMultiplier),
			new XElement("InfectionChanceMultiplier", InfectionChanceMultiplier)
		);
	}

	public override ILabourImpact Duplicate(IProjectLabourRequirement requirement)
	{
		return new HealingImpact(this, requirement);
	}

	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.Append(base.Show(actor));
		sb.AppendLine($"Healing Bonus: {HealingCheckBonus.ToBonusString(actor)}");
		sb.AppendLine($"Healing Rate: {HealingRateMultiplier.ToString("P2", actor).ColourValue()}");
		sb.AppendLine($"Infection Rate: {InfectionChanceMultiplier.ToString("P2", actor).ColourValue()}");
		return sb.ToString();
	}

	public override string ShowFull(ICharacter actor)
	{
		return
			$"{"[HealImpact]".Colour(Telnet.Magenta)} {HealingCheckBonus.ToBonusString(actor)} x{HealingRateMultiplier.ToString("P2", actor).ColourValue()} Infection {InfectionChanceMultiplier.ToString("P2", actor).ColourValue()}";
	}

	public override string ShowToPlayer(ICharacter actor)
	{
		return
			$"A {(HealingCheckBonus >= 0.0 ? "bonus" : "penalty")} to healing checks, a {(HealingRateMultiplier >= 1.0 ? "bonus" : "penalty")} to healing rate and a {(HealingRateMultiplier >= 1.0 ? "bonus" : "penalty")} to infection chance";
	}

	protected override string HelpText => $@"{base.HelpText}
	#3bonus <number>#0 - sets the healing check bonus
	#3multiplier <percent>#0 - sets the healing rate multiplier
	#3infection <percent>#0 - sets the infection chance multiplier";

	public override bool BuildingCommand(ICharacter actor, StringStack command, IProjectLabourRequirement requirement)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "bonus":
				return BuildingCommandBonus(actor, command);
			case "infection":
				return BuildingCommandInfection(actor, command);
			case "multiplier":
				return BuildingCommandMultiplier(actor, command);
		}

		return base.BuildingCommand(actor, command.GetUndo(), requirement);
	}

	private bool BuildingCommandMultiplier(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What multiplier to healing rates should this impact give?");
			return false;
		}

		if (!NumberUtilities.TryParsePercentage(command.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send("That is not a valid multiplier.");
			return false;
		}

		HealingRateMultiplier = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This impact will now multiply healing rates by {value.ToString("P2", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandInfection(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What multiplier to infection checks should this impact give?");
			return false;
		}

		if (!NumberUtilities.TryParsePercentage(command.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send("That is not a valid percentage.");
			return false;
		}

		HealingCheckBonus = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This impact will now give a multiplier to infection chance of {value.ToString("P2", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandBonus(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What bonus to healing checks should this impact give?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send("That is not a valid bonus.");
			return false;
		}

		HealingCheckBonus = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This impact will now give a bonus to healing checks of {value.ToBonusString(actor)}.");
		return true;
	}

	public double HealingRateMultiplier { get; protected set; }
	public double HealingCheckBonus { get; protected set; }
	public double InfectionChanceMultiplier { get; protected set; }
}