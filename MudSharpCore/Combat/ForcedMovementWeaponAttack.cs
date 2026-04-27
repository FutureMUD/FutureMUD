using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using System;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.Combat;

public class ForcedMovementWeaponAttack : WeaponAttack, IForcedMovementAttack
{
	public Difficulty SecondaryDifficulty { get; set; }
	public ForcedMovementTypes ForcedMovementTypes { get; set; }
	public ForcedMovementVerbs ForcedMovementVerbs { get; set; }
	public ForcedMovementRange RequiredRange { get; set; }

	protected override void LoadFromDatabase(MudSharp.Models.WeaponAttack attack)
	{
		base.LoadFromDatabase(attack);
		LoadAdditionalInfo(attack.AdditionalInfo);
	}

	private void LoadAdditionalInfo(string info)
	{
		SecondaryDifficulty = Difficulty.Normal;
		ForcedMovementTypes = ForcedMovementTypes.All;
		ForcedMovementVerbs = ForcedMovementVerbs.All;
		RequiredRange = ForcedMovementRange.Melee;

		if (string.IsNullOrWhiteSpace(info))
		{
			return;
		}

		if (int.TryParse(info, out var legacyDifficulty))
		{
			SecondaryDifficulty = (Difficulty)legacyDifficulty;
			return;
		}

		try
		{
			var root = XElement.Parse(info);
			if (int.TryParse(root.Element("Resist")?.Value, out var resist))
			{
				SecondaryDifficulty = (Difficulty)resist;
			}
			else if (CheckExtensions.GetDifficulty(root.Element("Resist")?.Value ?? string.Empty, out var difficulty))
			{
				SecondaryDifficulty = difficulty;
			}

			if (Enum.TryParse(root.Element("Types")?.Value, true, out ForcedMovementTypes types))
			{
				ForcedMovementTypes = types;
			}

			if (Enum.TryParse(root.Element("Verbs")?.Value, true, out ForcedMovementVerbs verbs))
			{
				ForcedMovementVerbs = verbs;
			}

			if (Enum.TryParse(root.Element("Range")?.Value, true, out ForcedMovementRange range))
			{
				RequiredRange = range;
			}
		}
		catch
		{
			// Older or hand-authored rows can have malformed metadata. Keep a safe, useful default.
		}
	}

	protected override void SeedInitialData(MudSharp.Models.WeaponAttack attack)
	{
		attack.AdditionalInfo = new XElement("Data",
			new XElement("Resist", (int)Difficulty.Normal),
			new XElement("Types", ForcedMovementTypes.All.ToString()),
			new XElement("Verbs", ForcedMovementVerbs.All.ToString()),
			new XElement("Range", ForcedMovementRange.Melee.ToString())
		).ToString(SaveOptions.DisableFormatting);
	}

	public ForcedMovementWeaponAttack(MudSharp.Models.WeaponAttack attack, IFuturemud gameworld) : base(attack, gameworld)
	{
	}

	public ForcedMovementWeaponAttack(IFuturemud gameworld, BuiltInCombatMoveType type) : base(gameworld, type)
	{
		SecondaryDifficulty = Difficulty.Normal;
		ForcedMovementTypes = ForcedMovementTypes.All;
		ForcedMovementVerbs = ForcedMovementVerbs.All;
		RequiredRange = ForcedMovementRange.Melee;
	}

	protected override void AddAttackSpecificCloneData(MudSharp.Models.WeaponAttack attack)
	{
		attack.AdditionalInfo = SaveAdditionalInfo().ToString(SaveOptions.DisableFormatting);
	}

	protected override void SaveAttackSpecificData(MudSharp.Models.WeaponAttack attack)
	{
		attack.AdditionalInfo = SaveAdditionalInfo().ToString(SaveOptions.DisableFormatting);
	}

	private XElement SaveAdditionalInfo()
	{
		return new XElement("Data",
			new XElement("Resist", (int)SecondaryDifficulty),
			new XElement("Types", ForcedMovementTypes.ToString()),
			new XElement("Verbs", ForcedMovementVerbs.ToString()),
			new XElement("Range", RequiredRange.ToString())
		);
	}

	protected override string ShowBuilderInternal(ICharacter actor)
	{
		return
			$"Resist Difficulty: {SecondaryDifficulty.Describe().Colour(Telnet.Green)}\n" +
			$"Movement Types: {ForcedMovementTypes.DescribeEnum().Colour(Telnet.Green)}\n" +
			$"Verbs: {ForcedMovementVerbs.DescribeEnum().Colour(Telnet.Green)}\n" +
			$"Required Range: {RequiredRange.DescribeEnum().Colour(Telnet.Green)}";
	}

	public override string SpecialListText =>
		$"Forced {ForcedMovementTypes.DescribeEnum()} {ForcedMovementVerbs.DescribeEnum()} @ {RequiredRange.DescribeEnum()}, Resist {SecondaryDifficulty.DescribeColoured()}";

	protected override void DescribeForAttacksCommandInternal(StringBuilder sb, ICharacter actor)
	{
		sb.Append(" - Forced ");
		sb.Append(ForcedMovementTypes.DescribeEnum());
		sb.Append(' ');
		sb.Append(ForcedMovementVerbs.DescribeEnum());
		sb.Append(" @ ");
		sb.Append(RequiredRange.DescribeEnum());
		sb.Append(" - Resist ");
		sb.Append(SecondaryDifficulty.DescribeBrief(true));
	}

	public override string HelpText => $@"{base.HelpText}
	#3resist <difficulty>#0 - sets the difficulty to resist this attack's movement
	#3range <melee|clinch|grapple>#0 - sets the range required to use this forced movement
	#3type <exit|layer|both>#0 - sets what kind of movement this attack can force
	#3verb <shove|pull|both>#0 - sets whether this attack shoves, pulls, or both";

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
			case "range":
				return BuildingCommandRange(actor, command);
			case "type":
			case "types":
			case "destination":
			case "destinations":
				return BuildingCommandType(actor, command);
			case "verb":
			case "verbs":
			case "mode":
			case "modes":
				return BuildingCommandVerb(actor, command);
		}

		return base.BuildingCommand(actor, command);
	}

	private bool BuildingCommandResistDifficulty(ICharacter actor, StringStack command)
	{
		command.PopSpeech();
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What difficulty should the check for the defender to resist this forced movement be?");
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
			$"The difficulty for the defender to resist this forced movement is now {difficulty.Describe().Colour(Telnet.Cyan)}.");
		return true;
	}

	private bool BuildingCommandRange(ICharacter actor, StringStack command)
	{
		command.PopSpeech();
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Should this attack require melee, clinch, or grapple range?");
			return false;
		}

		if (!Enum.TryParse(command.PopSpeech(), true, out ForcedMovementRange range))
		{
			actor.OutputHandler.Send("That is not a valid range. The valid ranges are melee, clinch and grapple.");
			return false;
		}

		RequiredRange = range;
		Changed = true;
		actor.OutputHandler.Send($"This forced movement now requires {range.DescribeEnum().ColourValue()} range.");
		return true;
	}

	private bool BuildingCommandType(ICharacter actor, StringStack command)
	{
		command.PopSpeech();
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Should this attack force exit movement, layer movement, or both?");
			return false;
		}

		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "exit":
			case "exits":
			case "room":
			case "rooms":
				ForcedMovementTypes = ForcedMovementTypes.Exit;
				break;
			case "layer":
			case "layers":
				ForcedMovementTypes = ForcedMovementTypes.Layer;
				break;
			case "both":
			case "all":
				ForcedMovementTypes = ForcedMovementTypes.All;
				break;
			default:
				actor.OutputHandler.Send("That is not a valid forced movement type. Use exit, layer, or both.");
				return false;
		}

		Changed = true;
		actor.OutputHandler.Send($"This attack can now force {ForcedMovementTypes.DescribeEnum().ColourValue()} movement.");
		return true;
	}

	private bool BuildingCommandVerb(ICharacter actor, StringStack command)
	{
		command.PopSpeech();
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Should this attack shove, pull, or both?");
			return false;
		}

		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "shove":
			case "push":
				ForcedMovementVerbs = ForcedMovementVerbs.Shove;
				break;
			case "pull":
			case "drag":
				ForcedMovementVerbs = ForcedMovementVerbs.Pull;
				break;
			case "both":
			case "all":
				ForcedMovementVerbs = ForcedMovementVerbs.All;
				break;
			default:
				actor.OutputHandler.Send("That is not a valid forced movement verb. Use shove, pull, or both.");
				return false;
		}

		Changed = true;
		actor.OutputHandler.Send($"This attack can now {ForcedMovementVerbs.DescribeEnum().ColourValue()} targets.");
		return true;
	}
}
