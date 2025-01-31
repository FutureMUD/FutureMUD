using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Models;
using System.Xml.Linq;
using MudSharp.RPG.Merits.Interfaces;
using MudSharp.RPG.Checks;

namespace MudSharp.RPG.Merits.CharacterMerits;
internal class DarksightMerit : CharacterMeritBase, IDarksightMerit
{
	protected DarksightMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		LoadFromXml(XElement.Parse(merit.Definition));
	}

	protected DarksightMerit() { }

	protected DarksightMerit(IFuturemud gameworld, string name) : base(gameworld, name, "Darksight", "@ have|has darksight")
	{
		MinimumEffectiveDifficulty = Difficulty.Automatic;
		DoDatabaseInsert();
	}

	/// <inheritdoc />
	protected override XElement SaveSubtypeDefinition(XElement root)
	{
		root.Add(new XElement("MinimumEffectiveDifficulty", (int)MinimumEffectiveDifficulty));
		return root;
	}

	private void LoadFromXml(XElement root)
	{
		MinimumEffectiveDifficulty = (Difficulty)int.Parse(root.Element("MinimumEffectiveDifficulty").Value);
	}

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Darksight",
			(merit, gameworld) => new DarksightMerit(merit, gameworld));
		MeritFactory.RegisterBuilderMeritInitialiser("Darksight", (gameworld, name) => new DarksightMerit(gameworld, name));
		MeritFactory.RegisterMeritHelp("Darksight", "Enables seeing in the dark", new DarksightMerit().HelpText);
	}

	/// <inheritdoc />
	protected override string SubtypeHelp => $@"{base.SubtypeHelp}
	#3difficulty <diff>#0 - sets the minimum effective difficulty";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "difficulty":
				return BuildingCommandDifficulty(actor, command);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandDifficulty(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What difficulty should be the minimum effective difficulty for this merit?\nValid values are {Enum.GetValues<Difficulty>().ListToColouredString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<Difficulty>(out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid difficulty.\nValid values are {Enum.GetValues<Difficulty>().ListToColouredString()}.");
			return false;
		}

		MinimumEffectiveDifficulty = value;
		Changed = true;
		actor.OutputHandler.Send($"The minimum effective difficulty in darkness is now {MinimumEffectiveDifficulty.DescribeColoured()}.");
		return true;
	}

	/// <inheritdoc />
	protected override void SubtypeShow(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Minimum Effective Difficulty: {MinimumEffectiveDifficulty.DescribeColoured()}");
	}

	public Difficulty MinimumEffectiveDifficulty { get; private set; }
}
