using System;
using System.Text;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat;

public class SecondaryDifficultyWeaponAttack : WeaponAttack, ISecondaryDifficultyAttack
{
	public Difficulty SecondaryDifficulty { get; set; }

	#region Overrides of WeaponAttack

	protected override void LoadFromDatabase(MudSharp.Models.WeaponAttack attack)
	{
		base.LoadFromDatabase(attack);
		SecondaryDifficulty = (Difficulty)int.Parse(attack.AdditionalInfo);
	}

	protected override void SeedInitialData(MudSharp.Models.WeaponAttack attack)
	{
		attack.AdditionalInfo = ((int)Difficulty.Easy).ToString();
	}

	#endregion

	public SecondaryDifficultyWeaponAttack(MudSharp.Models.WeaponAttack attack, IFuturemud gameworld) : base(attack,
		gameworld)
	{
	}

	public SecondaryDifficultyWeaponAttack(IFuturemud gameworld, BuiltInCombatMoveType type) : base(gameworld, type)
	{
		SecondaryDifficulty = Difficulty.Normal;
	}

	protected override void AddAttackSpecificCloneData(MudSharp.Models.WeaponAttack attack)
	{
		attack.AdditionalInfo = ((int)SecondaryDifficulty).ToString();
	}

	protected override void SaveAttackSpecificData(MudSharp.Models.WeaponAttack attack)
	{
		attack.AdditionalInfo = ((int)SecondaryDifficulty).ToString();
	}

	protected override string ShowBuilderInternal(ICharacter actor)
	{
		return $"Resist Difficulty: {SecondaryDifficulty.Describe().Colour(Telnet.Green)}";
	}

	#region Overrides of WeaponAttack

	protected override void DescribeForAttacksCommandInternal(StringBuilder sb, ICharacter actor)
	{
		sb.Append(" - Resist ");
		sb.Append(SecondaryDifficulty.DescribeBrief(true));
	}

	#endregion

	public override string HelpText => $@"{base.HelpText}
	#3resist <difficulty>#0 - sets the difficulty to resist this attack's secondary effects";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PeekSpeech().ToLowerInvariant())
		{
			case "secondary":
			case "secondarydifficulty":
			case "second":
			case "resist":
			case "resistdifficulty":
			case "secondary difficulty":
			case "resist difficulty":
				return BuildingCommandResistDifficulty(actor, command);
		}

		return base.BuildingCommand(actor, command);
	}

	private bool BuildingCommandResistDifficulty(ICharacter actor, StringStack command)
	{
		command.PopSpeech();
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What difficulty should the check for the defender to resist this attack's special effect be?");
			return false;
		}

		if (!CheckExtensions.GetDifficulty(command.PopSpeech(), out var difficulty))
		{
			actor.OutputHandler.Send(
				$"That is not a valid difficulty. See {"show difficulties".FluentTagMXP("send", "href='show difficulty' hint='Click this command to see a list of possible difficulties.'")} for a list of valid entries.");
			return false;
		}

		SecondaryDifficulty = difficulty;
		Changed = true;
		actor.OutputHandler.Send(
			$"The difficulty for the defender to resist this attack's special effect is now {difficulty.Describe().Colour(Telnet.Cyan)}.");
		return true;
	}
}