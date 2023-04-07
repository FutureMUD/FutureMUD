using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Screens;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;

namespace MudSharp.Menus;

public class ShowCharacterIntroMenu : CharacterLoginMenu
{
	public static bool IsRequired(Models.Character dbcharacter, ICharacter character, IFuturemud gameworld)
	{
		return !dbcharacter.ShownIntroductionMessage &&
		       gameworld.CharacterIntroTemplates.Any(x => x.AppliesToCharacter(character));
	}

	public ICharacterIntro CharacterIntro { get; set; }

	public ShowCharacterIntroMenu(string text, IFuturemud gameworld, ICharacter actor, ICharacterIntro intro) : base(
		text, gameworld, actor)
	{
		CharacterIntro = intro;
		Gameworld.Scheduler.AddSchedule(new Schedule<ICharacter>(Character, HandleNextIntroTick,
			ScheduleType.CharacterIntro, CharacterIntro.Delays.Dequeue(), "Character Intro Menu"));
	}

	public static Regex EchoVariableRegex = new(@"$(?<variable>\w+)$");

	public void HandleNextIntroTick(ICharacter actor)
	{
		var echo = CharacterIntro.Echoes.Dequeue();
		echo = EchoVariableRegex.Replace(echo, m =>
		{
			switch (m.Groups["variable"].Value.ToLowerInvariant())
			{
				case "name":
					return actor.PersonalName.GetName(NameStyle.FullName);
				case "firstname":
				case "givenname":
					return actor.PersonalName.GetName(NameStyle.GivenOnly);
				case "simplename":
					return actor.PersonalName.GetName(NameStyle.SimpleFull);
				case "surname":
				case "familyname":
				case "family":
					return actor.PersonalName.GetName(NameStyle.SurnameOnly);
				case "fullname":
					return actor.PersonalName.GetName(NameStyle.FullWithNickname);
				case "sdesc":
					return actor.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreSelf);
				case "he":
					return actor.Gender.Subjective();
				case "his":
					return actor.Gender.Possessive();
				case "him":
					return actor.Gender.Objective();
				case "himself":
					return actor.Gender.Reflexive();
				case "male":
					return actor.Gender.GenderClass();
				case "cultureperson":
					return actor.Culture.PersonWord(actor.Gender.Enum);
			}

			return m.Groups[0].Value;
		});

		OutputHandler.Send(echo.ProperSentences().NormaliseSpacing().SubstituteANSIColour()
		                       .Wrap(Character.InnerLineFormatLength));
		if (CharacterIntro.Echoes.Any())
		{
			Gameworld.Scheduler.AddSchedule(new Schedule<ICharacter>(Character, HandleNextIntroTick,
				ScheduleType.CharacterIntro, CharacterIntro.Delays.Dequeue(), "Character Intro Menu"));
			return;
		}

		OutputHandler.Send($"Type {"BEGIN".ColourCommand()} to enter the gameworld with your new character.");
		_introHasFinished = true;
	}

	private bool _introHasFinished;

	public override int Timeout => 3600000;
	public override string Prompt => "\n";


	public override void AssumeControl(IController controller)
	{
		base.AssumeControl(controller);
		OutputHandler.Send(
			$"\nLogging in to {Character.HowSeen(Character, flags: PerceiveIgnoreFlags.IgnoreSelf)} for the first time.");
		OutputHandler.Send("Beginning intro sequence...");
		OutputHandler.Send(
			$"You may type {"quit".ColourCommand()} to abort and go back to the main menu, or {"skip".ColourCommand()} to skip the intro sequence.");
	}

	public override void SilentAssumeControl(IController controller)
	{
		base.SilentAssumeControl(controller);
		OutputHandler.Send(
			$"Logging in to {Character.HowSeen(Character, flags: PerceiveIgnoreFlags.IgnoreSelf)} for the first time.");
		OutputHandler.Send("Beginning intro sequence...");
	}

	public override bool ExecuteCommand(string command)
	{
		if (command.EqualToAny("begin", "skip", "commence", "start", "login", "continue", "go"))
		{
			using (new FMDB())
			{
				var character = FMDB.Context.Characters.Find(Character.Id);
				character.ShownIntroductionMessage = true;
				FMDB.Context.SaveChanges();
			}

			DoLogin(true);
			return true;
		}

		if (command.EqualToAny("quit", "close", "stop", "back"))
		{
			_nextContext = new LoggedInMenu(Character.Account, Gameworld);
		}

		return true;
	}

	public override void LoseControl(IController controller)
	{
		Gameworld.Scheduler.Destroy(Character, ScheduleType.CharacterIntro);
		OutputHandler = null;
		Controller = null;
	}
}