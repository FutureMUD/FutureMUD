using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;

namespace MudSharp.CharacterCreation;

public class ChargenAdvice : SaveableItem, IChargenAdvice
{
	public ChargenAdvice(MudSharp.Models.ChargenAdvice advice, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = advice.Id;
		_name = advice.AdviceTitle;
		AdviceTitle = advice.AdviceTitle;
		AdviceText = advice.AdviceText;
		TargetStage = (ChargenStage)advice.ChargenStage;
		ShouldShowAdviceProg = gameworld.FutureProgs.Get(advice.ShouldShowAdviceProgId ?? 0);
	}

	public ChargenAdvice(IFuturemud gameworld, string title, string text, ChargenStage stage)
	{
		Gameworld = gameworld;
		_name = title;
		AdviceTitle = title;
		AdviceText = text;
		TargetStage = stage;
		using (new FMDB())
		{
			var dbitem = new MudSharp.Models.ChargenAdvice
			{
				AdviceText = AdviceText,
				AdviceTitle = AdviceTitle,
				ChargenStage = (int)stage
			};
			FMDB.Context.ChargenAdvices.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public ChargenAdvice(IChargenAdvice rhs)
	{
		Gameworld = rhs.Gameworld;
		_name = rhs.AdviceTitle;
		AdviceTitle = rhs.AdviceTitle;
		AdviceText = rhs.AdviceText;
		TargetStage = rhs.TargetStage;
		ShouldShowAdviceProg = rhs.ShouldShowAdviceProg;
		using (new FMDB())
		{
			var dbitem = new MudSharp.Models.ChargenAdvice
			{
				AdviceText = AdviceText,
				AdviceTitle = AdviceTitle,
				ChargenStage = (int)TargetStage,
				ShouldShowAdviceProgId = ShouldShowAdviceProg?.Id
			};
			FMDB.Context.ChargenAdvices.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public string AdviceTitle { get; set; }
	public string AdviceText { get; set; }
	public ChargenStage TargetStage { get; set; }
	public IFutureProg ShouldShowAdviceProg { get; set; }

	#region Overrides of Item

	public override string FrameworkItemType => "ChargenAdvice";

	#endregion

	#region Overrides of SaveableItem

	public override void Save()
	{
		var dbitem = FMDB.Context.ChargenAdvices.Find(Id);
		dbitem.AdviceText = AdviceText;
		dbitem.AdviceTitle = AdviceTitle;
		dbitem.ChargenStage = (int)TargetStage;
		dbitem.ShouldShowAdviceProgId = ShouldShowAdviceProg?.Id;
		Changed = false;
	}

	#endregion

	public const string HelpText = @"You can use the following options with this command:

	#3title <title>#0 - changes the title of this piece of advice
	#3prog <prog>#0 - changes which prog is used to control the application of this advice
	#3stage <stage>#0 - sets which stage of chargen the advice is to appear
	#3text#0 - drops you into an editor to write the text
	#3race <race>#0 - toggles the application of this advice to a particular race
	#3ethnicity <ethnicity>#0 - toggles the application of this advice to a particular ethnicity
	#3culture <culture>#0 - toggles the application of this advice to a particular culture
	#3role <role>#0 - toggles the application of this advice to a particular role";

	#region Implementation of IEditableItem

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "title":
				return BuildingCommandTitle(actor, command);
			case "prog":
				return BuildingCommandProg(actor, command);
			case "stage":
				return BuildingCommandStage(actor, command);
			case "text":
				return BuildingCommandText(actor);
			case "race":
				return BuildingCommandRace(actor, command);
			case "ethnicity":
				return BuildingCommandEthnicity(actor, command);
			case "culture":
				return BuildingCommandCulture(actor, command);
			case "role":
				return BuildingCommandRole(actor, command);
			default:
				actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandRole(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which role do you want to toggle this advice applying to?");
			return false;
		}

		var role = Gameworld.Roles.GetByIdOrName(command.SafeRemainingArgument);
		if (role is null)
		{
			actor.OutputHandler.Send("There is no such role.");
			return false;
		}

		if (role.ToggleAdvice(this))
		{
			actor.OutputHandler.Send(
				$"This chargen advice will now be shown when someone has selected the {role.Name.ColourName()} role.");
			return true;
		}

		actor.OutputHandler.Send(
			$"This chargen advice will no longer be shown when someone has selected the {role.Name.ColourName()} role.");
		return true;
	}

	private bool BuildingCommandCulture(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which culture do you want to toggle this advice applying to?");
			return false;
		}

		var culture = Gameworld.Cultures.GetByIdOrName(command.SafeRemainingArgument);
		if (culture is null)
		{
			actor.OutputHandler.Send("There is no such culture.");
			return false;
		}

		if (culture.ToggleAdvice(this))
		{
			actor.OutputHandler.Send(
				$"This chargen advice will now be shown when someone has selected the {culture.Name.ColourName()} culture.");
			return true;
		}

		actor.OutputHandler.Send(
			$"This chargen advice will no longer be shown when someone has selected the {culture.Name.ColourName()} culture.");
		return true;
	}

	private bool BuildingCommandEthnicity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which ethnicity do you want to toggle this advice applying to?");
			return false;
		}

		var ethnicity = Gameworld.Ethnicities.GetByIdOrName(command.SafeRemainingArgument);
		if (ethnicity is null)
		{
			actor.OutputHandler.Send("There is no such ethnicity.");
			return false;
		}

		if (ethnicity.ToggleAdvice(this))
		{
			actor.OutputHandler.Send(
				$"This chargen advice will now be shown when someone has selected the {ethnicity.Name.ColourName()} ethnicity.");
			return true;
		}

		actor.OutputHandler.Send(
			$"This chargen advice will no longer be shown when someone has selected the {ethnicity.Name.ColourName()} ethnicity.");
		return true;
	}

	private bool BuildingCommandRace(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which race do you want to toggle this advice applying to?");
			return false;
		}

		var race = Gameworld.Races.GetByIdOrName(command.SafeRemainingArgument);
		if (race is null)
		{
			actor.OutputHandler.Send("There is no such race.");
			return false;
		}

		if (race.ToggleAdvice(this))
		{
			actor.OutputHandler.Send(
				$"This chargen advice will now be shown when someone has selected the {race.Name.ColourName()} race.");
			return true;
		}

		actor.OutputHandler.Send(
			$"This chargen advice will no longer be shown when someone has selected the {race.Name.ColourName()} race.");
		return true;
	}

	private bool BuildingCommandText(ICharacter actor)
	{
		var sb = new StringBuilder();
		if (!string.IsNullOrEmpty(AdviceText))
		{
			sb.AppendLine("Replacing:\n");
			sb.AppendLine(AdviceText.ProperSentences().Wrap(actor.InnerLineFormatLength, "\t"));
			sb.AppendLine();
		}

		sb.AppendLine("Enter the advice text in the editor below.");
		sb.AppendLine();
		actor.OutputHandler.Send(sb.ToString());
		actor.EditorMode(PostAction, CancelAction, 1.0, AdviceText);
		return true;
	}

	private void CancelAction(IOutputHandler handler, object[] arg2)
	{
		handler.Send("You decide not to edit the advice text.");
	}

	private void PostAction(string text, IOutputHandler handler, object[] arg3)
	{
		AdviceText = text.ProperSentences();
		Changed = true;
		handler.Send($"You set the advice text to:\n\n{text.Wrap(80, "\t")}");
	}

	private bool BuildingCommandStage(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which chargen stage do you want this advice to apply to?\nThe options are: {Enum.GetValues<ChargenStage>().Select(x => x.DescribeEnum(false, Telnet.Cyan))}");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<ChargenStage>(out var stage))
		{
			actor.OutputHandler.Send(
				$"That is not a valid chargen stage.\nThe options are: {Enum.GetValues<ChargenStage>().Select(x => x.DescribeEnum(false, Telnet.Cyan))}");
			return false;
		}

		TargetStage = stage;
		Changed = true;
		actor.OutputHandler.Send(
			$"This advice will now be shown when the player is at the {stage.DescribeEnum().ColourName()} screen.");
		return true;
	}

	private bool BuildingCommandProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog do you want to use to control whether this advice is shown?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean, new ProgVariableTypes[]
			{
				ProgVariableTypes.Chargen
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		ShouldShowAdviceProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This advice will now use the {prog.MXPClickableFunctionName()} prog to filter whether it shows up for an application.");
		return true;
	}

	private bool BuildingCommandTitle(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What title do you want to give to this piece of chargen advice?");
			return false;
		}

		AdviceTitle = command.SafeRemainingArgument.TitleCase();
		_name = AdviceTitle;
		Changed = true;
		actor.OutputHandler.Send($"This chargen advice now has the title {AdviceTitle.ColourName()}.");
		return true;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Chargen Advice #{Id.ToString("N0", actor)}".ColourName());
		sb.AppendLine($"Stage: {TargetStage.DescribeEnum(true, Telnet.Green)}");
		sb.AppendLine($"Prog: {ShouldShowAdviceProg?.MXPClickableFunctionName() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine($"Title: {AdviceTitle.ColourCommand()}");
		sb.AppendLine("Text:");
		sb.AppendLine();
		sb.AppendLine(AdviceText.Wrap(actor.InnerLineFormatLength, "\t"));
		sb.AppendLine();
		sb.AppendLine("Attached To:");
		foreach (var item in Gameworld.Races.Where(x => x.ChargenAdvices.Contains(this)))
		{
			sb.AppendLine($"\tRace #{item.Id.ToString("N0", actor)} - {item.Name}".ColourValue());
		}

		foreach (var item in Gameworld.Ethnicities.Where(x => x.ChargenAdvices.Contains(this)))
		{
			sb.AppendLine($"\tEthnicity #{item.Id.ToString("N0", actor)} - {item.Name}".ColourValue());
		}

		foreach (var item in Gameworld.Cultures.Where(x => x.ChargenAdvices.Contains(this)))
		{
			sb.AppendLine($"\tCulture #{item.Id.ToString("N0", actor)} - {item.Name}".ColourValue());
		}

		foreach (var item in Gameworld.Roles.Where(x => x.ChargenAdvices.Contains(this)))
		{
			sb.AppendLine($"\tRole #{item.Id.ToString("N0", actor)} - {item.Name}".ColourValue());
		}

		return sb.ToString();
	}

	#endregion
}