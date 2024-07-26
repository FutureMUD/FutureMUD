using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Body;
using MudSharp.Body.PartProtos;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class WillToLiveMerit : CharacterMeritBase, IHypoxiaReducingMerit, IOrganFunctionBonusMerit
{
	protected WillToLiveMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		BrainBonus = double.Parse(definition.Attribute("brainbonus")?.Value ?? "0.0");
		HeartBonus = double.Parse(definition.Attribute("heartbonus")?.Value ?? "0.0");
		HypoxiaReductionFactor = double.Parse(definition.Attribute("hypoxia")?.Value ?? "1.0");
	}

	protected WillToLiveMerit(){}

	protected WillToLiveMerit(IFuturemud gameworld, string name) : base(gameworld, name, "Will To Live", "@ have|has an unusually strong will to live")
	{
		BrainBonus = 0.0;
		HeartBonus = 0.0;
		HypoxiaReductionFactor = 1.0;
		DoDatabaseInsert();
	}

	/// <inheritdoc />
	protected override XElement SaveSubtypeDefinition(XElement root)
	{
		root.Add(new XAttribute("brainbonus", BrainBonus));
		root.Add(new XAttribute("heartbonus", HeartBonus));
		root.Add(new XAttribute("hypoxia", HypoxiaReductionFactor));
		return root;
	}

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Will To Live",
			(merit, gameworld) => new WillToLiveMerit(merit, gameworld));
		MeritFactory.RegisterBuilderMeritInitialiser("Will To Live", (gameworld, name) => new WillToLiveMerit(gameworld, name));
		MeritFactory.RegisterMeritHelp("Will To Live", "Makes a character resistant to mortal peril", new WillToLiveMerit().HelpText);
	}

	/// <inheritdoc />
	protected override void SubtypeShow(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Bonus Brain Function: {BrainBonus.ToBonusPercentageString(actor)}");
		sb.AppendLine($"Bonus Heart Function: {HeartBonus.ToBonusPercentageString(actor)}");
		sb.AppendLine($"Hypoxia Damage Rate: {HypoxiaReductionFactor.ToStringP2Colour(actor)}");
	}

	/// <inheritdoc />
	protected override string SubtypeHelp => $@"{base.SubtypeHelp}
	#3heart <%>#0 - sets the bonus heart organ function
	#3brain <%>#0 - sets the bonus brain organ function
	#3hypoxia <%>#0 - sets the modifier to hypoxia damage";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "heart":
				return BuildingCommandHeart(actor, command);
			case "brain":
				return BuildingCommandBrain(actor, command);
			case "hypoxia":
				return BuildingCommandHypoxia(actor, command);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandHypoxia(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What percentage modifier should be applied to hypoxia damage relative to base?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid percentage.");
			return false;
		}

		HypoxiaReductionFactor = value;
		Changed = true;
		actor.OutputHandler.Send($"Characters with this merit now suffer {value.ToStringP2Colour(actor)} hypoxia damage compared to base.");
		return true;
	}

	private bool BuildingCommandBrain(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What percentage bonus function should the brain organ have?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid percentage.");
			return false;
		}

		BrainBonus = value;
		Changed = true;
		actor.OutputHandler.Send($"Characters with this merit now get {value.ToBonusPercentageString(actor)} brain organ function.");
		return true;
	}

	private bool BuildingCommandHeart(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What percentage bonus function should the heart organ have?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid percentage.");
			return false;
		}

		HeartBonus = value;
		Changed = true;
		actor.OutputHandler.Send($"Characters with this merit now get {value.ToBonusPercentageString(actor)} heart organ function.");
		return true;
	}

	public double BrainBonus { get; set; }
	public double HeartBonus { get; set; }

	public IEnumerable<(IOrganProto Organ, double Bonus)> OrganFunctionBonuses(IBody body)
	{
		foreach (var organ in body.Organs)
		{
			if (organ is BrainProto)
			{
				if (organ.OrganFunctionFactor(body) > 0.0)
				{
					yield return (organ, BrainBonus);
				}

				continue;
			}

			if (organ is HeartProto)
			{
				if (organ.OrganFunctionFactor(body) > 0.0)
				{
					yield return (organ, HeartBonus);
				}

				continue;
			}
		}
	}

	public double HypoxiaReductionFactor { get; set; }
}