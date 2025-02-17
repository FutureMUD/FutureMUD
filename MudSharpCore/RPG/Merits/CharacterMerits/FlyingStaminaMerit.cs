using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Models;
using MudSharp.Movement;
using System.Xml.Linq;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class FlyingStaminaMerit : CharacterMeritBase
{
	protected FlyingStaminaMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		Multiplier = double.Parse(definition.Attribute("multiplier")?.Value ?? "1.0");
	}

	protected FlyingStaminaMerit() { }

	protected FlyingStaminaMerit(IFuturemud gameworld, string name) : base(gameworld, name, "Flying Stamina Multiplier", "@ have|has a stamina modifier to flying")
	{
		Multiplier = 1.0;
		DoDatabaseInsert();
	}

	/// <inheritdoc />
	protected override XElement SaveSubtypeDefinition(XElement root)
	{
		root.Add(new XAttribute("multiplier", Multiplier));
		return root;
	}

	public double Multiplier { get; set; }

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Flying Stamina Multiplier",
			(merit, gameworld) => new FlyingStaminaMerit(merit, gameworld));
		MeritFactory.RegisterBuilderMeritInitialiser("Flying Stamina Multiplier", (gameworld, name) => new FlyingStaminaMerit(gameworld, name));
		MeritFactory.RegisterMeritHelp("Flying Stamina Multiplier", "Multiplies flying stamina usage", new FlyingStaminaMerit().HelpText);
	}

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "multiplier":
			case "mult":
				return BuildingCommandMultiplier(actor, command);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandMultiplier(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What multiplier should be applied to stamina usage?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid percentage.");
			return false;
		}

		Multiplier = value;
		Changed = true;
		actor.OutputHandler.Send($"This merit will now multiply stamina usage by {value.ToString("P2", actor).ColourValue()} when it applies.");
		return true;
	}

	/// <inheritdoc />
	protected override string SubtypeHelp => $@"{base.SubtypeHelp}
	#3multiplier <%>#0 - sets the percentage multiplier for stamina usage";

	/// <inheritdoc />
	protected override void SubtypeShow(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Stamina Usage Multiplier: {Multiplier.ToString("P2", actor).ColourValue()}");
	}
}