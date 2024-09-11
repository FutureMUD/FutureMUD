using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.TimeAndDate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.RPG.Law.PunishmentStrategies;

public class PunishmentStrategyGoodBehaviourBond : PunishmentStrategyBase
{
	public PunishmentStrategyGoodBehaviourBond(IFuturemud gameworld) : base(gameworld)
	{
		GoodBehaviourBondLength = MudTimeSpan.FromDays(30);
	}

	public PunishmentStrategyGoodBehaviourBond(IFuturemud gameworld, XElement root) : base(gameworld, root)
	{
		GoodBehaviourBondLength = MudTimeSpan.Parse(root.Element("Length").Value);
	}

	public MudTimeSpan GoodBehaviourBondLength { get; set; }

	public override string TypeSpecificHelpText => @"
	length <time span> - the length of the good behaviour bond";

	public override string Describe(IPerceiver voyeur)
	{
		return $"a {GoodBehaviourBondLength.Describe(voyeur)} good behaviour bond";
	}

	public override bool BuildingCommand(ICharacter actor, ILegalAuthority authority, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "length":
				return BuildingCommandLength(actor, command);
			default:
				return base.BuildingCommand(actor, authority, command.GetUndo());
		}
	}

	private bool BuildingCommandLength(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How long should the good behaviour bond for this punishment be?");
			return false;
		}

		if (!MudTimeSpan.TryParse(command.SafeRemainingArgument, out var mts))
		{
			actor.OutputHandler.Send(
				$"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid time span.");
			return false;
		}

		GoodBehaviourBondLength = mts;
		actor.OutputHandler.Send(
			$"This punishment will now impose a {mts.Describe(actor).ColourValue()} good behaviour bond.");
		return true;
	}

	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Good Behaviour Bond".ColourName());
		sb.AppendLine($"Length: {GoodBehaviourBondLength.Describe(actor).ColourValue()}");
		BaseShowText(actor, sb);
		return sb.ToString();
	}

	public override PunishmentResult GetResult(ICharacter actor, ICrime crime, double severity = 0)
	{
		return new PunishmentResult { GoodBehaviourBondLength = GoodBehaviourBondLength };
	}

	protected override void SaveSpecificType(XElement root)
	{
		root.Add(new XAttribute("type", "bond"));
		root.Add(new XElement("Length", GoodBehaviourBondLength.GetRoundTripParseText));
	}
}