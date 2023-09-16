using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Screens;
using MudSharp.Database;
using MudSharp.Framework;

namespace MudSharp.Menus;
#nullable enable
public abstract class CharacterLoginMenu : Menu
{
	public ICharacter Character { get; }
	public IAccount Account => Character.Account;

	protected CharacterLoginMenu(ICharacter character)
	{
		Gameworld = character.Gameworld;
		Character = character;
	}

	protected CharacterLoginMenu(string text, IFuturemud gameworld, ICharacter character) : base(text, gameworld)
	{
		Character = character;
	}

	public void DoLogin(bool firstTime)
	{
		Gameworld.Add(Character, false);
		Character.Register(OutputHandler);
		if (firstTime)
		{
			var startingLocation =
				(Gameworld.ChargenStoryboard.StageScreenMap[ChargenStage.SelectStartingLocation] as
					StartingLocationPickerScreenStoryboard)?.Locations.FirstOrDefault(
					x => Character.Roles.Contains(x.Role));
			startingLocation?.OnCommenceProg?.Execute(Character);
		}

		if (InvalidCharacteristicsMenu.IsRequired(Character))
		{
			_nextContext = new InvalidCharacteristicsMenu(Character, firstTime);
			return;
		}

		var scriptedEvents = Gameworld.ScriptedEvents.Where(x => x.IsReady && !x.IsFinished && x.Character == Character).ToList();
		if (scriptedEvents.Any())
		{
			_nextContext = new ScriptedEventMenu(Character, scriptedEvents.First(), firstTime);
			return;
		}

		if (Character.Location != null)
		{
			Character.Location.Login(Character);
		}
		else
		{
			Gameworld.Cells.First().Login(Character);
		}

		_nextContext = Character;
	}

	public override int Timeout => 3600000;
	public override bool HasPrompt => true;

	public override void AssumeControl(IController controller)
	{
		Controller = controller;
		OutputHandler = controller.OutputHandler;
	}

	public override void SilentAssumeControl(IController controller)
	{
		Controller = controller;
		OutputHandler = controller.OutputHandler;
	}

	public override void LoseControl(IController controller)
	{
		OutputHandler = null;
		Controller = null;
	}
}