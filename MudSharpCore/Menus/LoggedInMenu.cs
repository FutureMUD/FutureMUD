using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Dapper;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Screens;
using MudSharp.Commands;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Email;
using MudSharp.Framework;
using MudSharp.Network;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using Chargen = MudSharp.Models.Chargen;

namespace MudSharp.Menus;

internal class LoggedInMenu : Menu
{
	public enum LoggedInMenuState
	{
		LoggedInMenu,
		LoginCharacterMenu,
		LoginWithdrawApplication,
		ConfirmCreateNewCharacter,
		ChangeEmail,
		ChangePassword,
		CheckedOldPassword,
		EnteredNewPassword,
		RegisterAccount,
		ResendAccountEmail
	}

	private Dictionary<LoggedInMenuState, IExecutable> _commands;

	private CommandManager _loggedInMenuManager;
	private string _newPasswordHash;
	private long _newPasswordSalt;

	private int _passwordEntryCount;

	public LoggedInMenuState State { get; set; }

	public override int Timeout => 60000 * 30;

	public IAccount Account { get; }

	#region Guests

	public void LoginGuest(string playerinput)
	{
		if (!Gameworld.Guests.Any())
		{
			OutputHandler.Send(
				"Unfortunately, there are no free guest avatars for you to log in to! Try again some other time!");
			return;
		}

		var guestAvatar = GuestCharacter.GetRandomGuestCharacter(Gameworld);

		if (GuestCharacter.GuestLoungeCell == null)
		{
			OutputHandler.Send(
				"Unfortunately we do not have a guest lounge for you to log in to! Try again some other time!");
			return;
		}

		guestAvatar.Account = Account;
		guestAvatar.Register(OutputHandler);
		Gameworld.Add(guestAvatar, false);
		guestAvatar.RoomLayer = GuestCharacter.GuestLoungeCell.Terrain(guestAvatar).TerrainLayers
		                                      .FirstMin(x => Math.Abs(x.LayerHeight()));
		GuestCharacter.GuestLoungeCell.Login(guestAvatar);
		_nextContext = guestAvatar;
	}

	#endregion

	#region Create Character

	private void CreateCharacter(string command)
	{
		using (new FMDB())
		{
			//var activeCount =
			//(from character in FMDB.Context.Characters
			//    where character.AccountId == Account.ID
			//    where character.Status == (int) Accounts.CharacterStatus.Active
			//    select character).Count();

			//var submittedCount = (from chargen in FMDB.Context.Chargens
			//    where chargen.AccountId == Account.ID
			//    where chargen.Status == (int) ChargenState.Submitted
			//    select chargen).Count();

			//if (Account.ActiveCharactersAllowed <= activeCount + submittedCount) {
			//    OutputHandler.Send(
			//        "Your account is only allowed " + Account.ActiveCharactersAllowed + " active " +
			//        (Account.ActiveCharactersAllowed == 1 ? "character" : "characters") + ", and you already have " +
			//        (activeCount + submittedCount) + ".", false);
			//    OutputHandler.Send(ToString());
			//    return;
			//}

			var inprogressCount = (from chargen in FMDB.Context.Chargens
			                       where chargen.AccountId == Account.Id
			                       where chargen.Status == (int)ChargenState.InProgress
			                       select chargen).Count();

			if (inprogressCount >= 10)
			{
				OutputHandler.Send(
					"Don't you think you have enough characters in progress? 10 is enough. Go outside or something.");
				OutputHandler.Send(ToString());
				return;
			}

			if (inprogressCount > 0)
			{
				OutputHandler.Send(
					"Warning: You already have characters in the process of being created. Do you want to load these characters instead? Y/N?");
				State = LoggedInMenuState.ConfirmCreateNewCharacter;
				return;
			}
		}

		// TODO: Check for limitations on character generation here
		OutputHandler.Send("Creating a new character.");
		var menu = new ChargenMenu(Account, Gameworld);
		_nextContext = menu;
	}

	#endregion

	#region Quit

	private void Quit(string command)
	{
		_nextContext = new Quit();
	}

	#endregion

	#region Setup

	public LoggedInMenu(IAccount account, IFuturemud gameworld)
	{
		Gameworld = gameworld;

		State = LoggedInMenuState.LoggedInMenu;
		Account = account;
		SetupCommands();
	}

	private void SetupCommands()
	{
		_loggedInMenuManager = new CommandManager("Invalid command. Please enter an option from the menu.\n");
		_loggedInMenuManager.Add("", text =>
		{
			SetupCommands();
			OutputHandler.Send(_menuText);
		});
		_commands = new Dictionary<LoggedInMenuState, IExecutable>();
		int activeChargens, submittedChargens;
		using (new FMDB())
		{
			activeChargens = FMDB.Connection.Query<int>(
				                     $"SELECT COUNT(1) FROM Chargens WHERE AccountId = {Account.Id} and Status = {(int)ChargenState.InProgress}")
			                     .First();
			submittedChargens = FMDB.Connection.Query<int>(
				                        $"SELECT COUNT(1) FROM Chargens WHERE AccountId = {Account.Id} and Status = {(int)ChargenState.Submitted}")
			                        .First();
		}

		if (Account.AccountStatus == AccountStatus.Suspended)
		{
			_menuText = Gameworld.GetStaticString("LoggedInMenuBanned");
		}
		else if (!Account.IsRegistered)
		{
			_menuText = string.Format(Gameworld.GetStaticString("LoggedInMenuUnregistered"),
				Account.Name.Proper().ColourCharacter());
			_loggedInMenuManager.Add("1", RegisterAccount);
			_loggedInMenuManager.Add("R", RegisterAccount);
			_loggedInMenuManager.Add("2", ResendRegistrationEmail);
			_loggedInMenuManager.Add("S", ResendRegistrationEmail);
			_loggedInMenuManager.Add("3", ChangeEmail);
			_loggedInMenuManager.Add("E", ChangeEmail);
			_loggedInMenuManager.Add("4", ChangePassword);
			_loggedInMenuManager.Add("P", ChangePassword);
		}
		else if (Gameworld.MaintenanceMode != MaintenanceModeSetting.None &&
		         Account.Authority.Level < PermissionLevel.JuniorAdmin)
		{
			if (Gameworld.MaintenanceMode.HasFlag(MaintenanceModeSetting.NoChargen) &&
			    Gameworld.MaintenanceMode.HasFlag(MaintenanceModeSetting.NoLogin))
			{
				_menuText = string.Format(Gameworld.GetStaticString("LoggedInMenuMaintenance"),
					Account.Name.Proper().ColourCharacter());
				_loggedInMenuManager.Add("1", ChangeEmail);
				_loggedInMenuManager.Add("2", ChangePassword);
				_loggedInMenuManager.Add("E", ChangeEmail);
				_loggedInMenuManager.Add("P", ChangePassword);
			}
			else if (Gameworld.MaintenanceMode.HasFlag(MaintenanceModeSetting.NoChargen))
			{
				_menuText = string.Format(Gameworld.GetStaticString("LoggedInMenuMaintenanceNoChargen"),
					Account.Name.Proper().ColourCharacter());
				_loggedInMenuManager.Add("1", ConnectCharacter);
				_loggedInMenuManager.Add("C", ConnectCharacter);
				_loggedInMenuManager.Add("G", LoginGuest);
				_loggedInMenuManager.Add("2", LoginGuest);
				_loggedInMenuManager.Add("3", ChangeEmail);
				_loggedInMenuManager.Add("4", ChangePassword);
				_loggedInMenuManager.Add("E", ChangeEmail);
				_loggedInMenuManager.Add("P", ChangePassword);
			}
			else if (Gameworld.MaintenanceMode.HasFlag(MaintenanceModeSetting.NoLogin))
			{
				if (activeChargens > 0 && submittedChargens > 0)
				{
					_menuText =
						string.Format(Gameworld.GetStaticString("LoggedInMenuMaintenanceNoLoginBothChargens"),
							Account.Name.Proper().ColourCharacter());
					_loggedInMenuManager.Add("1", ConnectCharacter);
					_loggedInMenuManager.Add("2", LoginGuest);
					_loggedInMenuManager.Add("C", ConnectCharacter);
					_loggedInMenuManager.Add("G", LoginGuest);
					_loggedInMenuManager.Add("3", WithdrawApplication);
					_loggedInMenuManager.Add("4", CreateCharacter);
					_loggedInMenuManager.Add("W", WithdrawApplication);
					_loggedInMenuManager.Add("N", CreateCharacter);
					_loggedInMenuManager.Add("5", ChangeEmail);
					_loggedInMenuManager.Add("6", ChangePassword);
					_loggedInMenuManager.Add("E", ChangeEmail);
					_loggedInMenuManager.Add("P", ChangePassword);
				}
				else if (submittedChargens > 0)
				{
					_menuText = string.Format(Gameworld.GetStaticString("LoggedInMenuMaintenanceNoLoginSubmitted"),
						Account.Name.Proper().ColourCharacter());
					_loggedInMenuManager.Add("G", LoginGuest);
					_loggedInMenuManager.Add("1", LoginGuest);
					_loggedInMenuManager.Add("2", WithdrawApplication);
					_loggedInMenuManager.Add("3", CreateCharacter);
					_loggedInMenuManager.Add("W", WithdrawApplication);
					_loggedInMenuManager.Add("N", CreateCharacter);
					_loggedInMenuManager.Add("4", ChangeEmail);
					_loggedInMenuManager.Add("5", ChangePassword);
					_loggedInMenuManager.Add("E", ChangeEmail);
					_loggedInMenuManager.Add("P", ChangePassword);
				}
				else if (activeChargens > 0)
				{
					_menuText = string.Format(Gameworld.GetStaticString("LoggedInMenuMaintenanceNoLoginResume"),
						Account.Name.Proper().ColourCharacter());
					_loggedInMenuManager.Add("1", ConnectCharacter);
					_loggedInMenuManager.Add("2", LoginGuest);
					_loggedInMenuManager.Add("C", ConnectCharacter);
					_loggedInMenuManager.Add("G", LoginGuest);
					_loggedInMenuManager.Add("3", CreateCharacter);
					_loggedInMenuManager.Add("N", CreateCharacter);
					_loggedInMenuManager.Add("4", ChangeEmail);
					_loggedInMenuManager.Add("5", ChangePassword);
					_loggedInMenuManager.Add("E", ChangeEmail);
					_loggedInMenuManager.Add("P", ChangePassword);
				}
				else
				{
					_menuText = string.Format(Gameworld.GetStaticString("LoggedInMenuMaintenanceNoLogin"),
						Account.Name.Proper().ColourCharacter());
					_loggedInMenuManager.Add("G", LoginGuest);
					_loggedInMenuManager.Add("1", LoginGuest);
					_loggedInMenuManager.Add("2", CreateCharacter);
					_loggedInMenuManager.Add("N", CreateCharacter);
					_loggedInMenuManager.Add("3", ChangeEmail);
					_loggedInMenuManager.Add("4", ChangePassword);
					_loggedInMenuManager.Add("E", ChangeEmail);
					_loggedInMenuManager.Add("P", ChangePassword);
				}
			}
			else
			{
				_menuText = string.Format(Gameworld.GetStaticString("LoggedInMenuMaintenance"),
					Account.Name.Proper().ColourCharacter());
				_loggedInMenuManager.Add("1", ChangeEmail);
				_loggedInMenuManager.Add("2", ChangePassword);
				_loggedInMenuManager.Add("E", ChangeEmail);
				_loggedInMenuManager.Add("P", ChangePassword);
			}
		}
		else
		{
			if (activeChargens > 0 && submittedChargens > 0)
			{
				_menuText = string.Format(Gameworld.GetStaticString("LoggedInMenuBothChargens"),
					Account.Name.Proper().ColourCharacter());
				_loggedInMenuManager.Add("1", ConnectCharacter);
				_loggedInMenuManager.Add("2", LoginGuest);
				_loggedInMenuManager.Add("C", ConnectCharacter);
				_loggedInMenuManager.Add("G", LoginGuest);
				_loggedInMenuManager.Add("3", WithdrawApplication);
				_loggedInMenuManager.Add("4", CreateCharacter);
				_loggedInMenuManager.Add("W", WithdrawApplication);
				_loggedInMenuManager.Add("N", CreateCharacter);
				_loggedInMenuManager.Add("5", ChangeEmail);
				_loggedInMenuManager.Add("6", ChangePassword);
				_loggedInMenuManager.Add("E", ChangeEmail);
				_loggedInMenuManager.Add("P", ChangePassword);
			}
			else if (submittedChargens > 0)
			{
				_menuText = string.Format(Gameworld.GetStaticString("LoggedInMenuSubmitted"),
					Account.Name.Proper().ColourCharacter());
				_loggedInMenuManager.Add("1", ConnectCharacter);
				_loggedInMenuManager.Add("2", LoginGuest);
				_loggedInMenuManager.Add("C", ConnectCharacter);
				_loggedInMenuManager.Add("G", LoginGuest);
				_loggedInMenuManager.Add("3", WithdrawApplication);
				_loggedInMenuManager.Add("4", CreateCharacter);
				_loggedInMenuManager.Add("W", WithdrawApplication);
				_loggedInMenuManager.Add("N", CreateCharacter);
				_loggedInMenuManager.Add("5", ChangeEmail);
				_loggedInMenuManager.Add("6", ChangePassword);
				_loggedInMenuManager.Add("E", ChangeEmail);
				_loggedInMenuManager.Add("P", ChangePassword);
			}
			else if (activeChargens > 0)
			{
				_menuText = string.Format(Gameworld.GetStaticString("LoggedInMenuResume"),
					Account.Name.Proper().ColourCharacter());
				_loggedInMenuManager.Add("1", ConnectCharacter);
				_loggedInMenuManager.Add("2", LoginGuest);
				_loggedInMenuManager.Add("C", ConnectCharacter);
				_loggedInMenuManager.Add("G", LoginGuest);
				_loggedInMenuManager.Add("3", CreateCharacter);
				_loggedInMenuManager.Add("N", CreateCharacter);
				_loggedInMenuManager.Add("4", ChangeEmail);
				_loggedInMenuManager.Add("5", ChangePassword);
				_loggedInMenuManager.Add("E", ChangeEmail);
				_loggedInMenuManager.Add("P", ChangePassword);
			}
			else
			{
				_menuText = string.Format(Gameworld.GetStaticString("LoggedInMenu"),
					Account.Name.Proper().ColourCharacter());
				_loggedInMenuManager.Add("1", ConnectCharacter);
				_loggedInMenuManager.Add("2", LoginGuest);
				_loggedInMenuManager.Add("C", ConnectCharacter);
				_loggedInMenuManager.Add("G", LoginGuest);
				_loggedInMenuManager.Add("3", CreateCharacter);
				_loggedInMenuManager.Add("N", CreateCharacter);
				_loggedInMenuManager.Add("4", ChangeEmail);
				_loggedInMenuManager.Add("5", ChangePassword);
				_loggedInMenuManager.Add("E", ChangeEmail);
				_loggedInMenuManager.Add("P", ChangePassword);
			}
		}

		_loggedInMenuManager.Add("x", Quit);
		_commands.Add(LoggedInMenuState.LoggedInMenu, _loggedInMenuManager);
		_commands.Add(LoggedInMenuState.ConfirmCreateNewCharacter, new Command(ConfirmCreateCharacter));
		_commands.Add(LoggedInMenuState.ChangeEmail, new Command(CommitEmail));
		_commands.Add(LoggedInMenuState.ChangePassword, new Command(CheckOldPassword));
		_commands.Add(LoggedInMenuState.CheckedOldPassword, new Command(EnterNewPassword));
		_commands.Add(LoggedInMenuState.EnteredNewPassword, new Command(CommitPassword));
		_commands.Add(LoggedInMenuState.RegisterAccount, new Command(RegisterAccountSubmit));
		_commands.Add(LoggedInMenuState.ResendAccountEmail, new Command(ResendRegistrationEmailConfirm));
	}

	private void ConfirmCreateCharacter(string playerInput)
	{
		if ("yes".StartsWith(playerInput, StringComparison.InvariantCultureIgnoreCase))
		{
			State = LoggedInMenuState.LoginCharacterMenu;
			ConnectCharacter("");
			return;
		}

		OutputHandler.Send("Creating a new character.");
		var menu = new ChargenMenu(Account, Gameworld);
		_nextContext = menu;
	}

	private void WithdrawApplication(string playerInput)
	{
		using (new FMDB())
		{
			var chargensQuery = (from chargen in FMDB.Context.Chargens
			                     where
				                     chargen.AccountId == Account.Id &&
				                     chargen.Status == (int)CharacterStatus.Submitted
			                     select chargen).ToList();

			if (chargensQuery.Count == 0)
			{
				OutputHandler.Send("You have no submitted applications that you can withdraw.");
				State = LoggedInMenuState.LoggedInMenu;
				OutputHandler.Send(_menuText);
				return;
			}

			var characterSelectionManager = new CommandManager
			{
				FailedToFindCommand = "Invalid character. Please select one from the list."
			};
			characterSelectionManager.Add("0", ConnectCharacterBack);

			var sb = new StringBuilder();
			sb.AppendLine("\nYou have the following applications that you can withdraw: ");
			var nextChoice = 1;
			foreach (var character in chargensQuery)
			{
				sb.AppendLine($"\t{nextChoice}: {character.Name}");
				characterSelectionManager.Add(nextChoice.ToString(CultureInfo.InvariantCulture),
					new TaggedCommand<Chargen>(character, SelectWithdrawApplication));
				nextChoice++;
			}

			sb.Append("\n\rPlease select an application to withdraw, or type 0 to return to the main menu: ");
			OutputHandler.Send(sb.ToString(), false);
			State = LoggedInMenuState.LoginWithdrawApplication;
			_commands[LoggedInMenuState.LoginWithdrawApplication] = characterSelectionManager;
		}
	}

	private void SelectWithdrawApplication(Chargen chargen, string command)
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.Chargens.Find(chargen.Id);
			if (dbitem == null)
			{
				OutputHandler.Send("Could not find your application. Perhaps it has been deleted.");
				State = LoggedInMenuState.LoggedInMenu;
				SetupCommands();
				OutputHandler.Send(_menuText);
				return;
			}

			dbitem.Status = (int)ChargenState.InProgress;
			FMDB.Context.SaveChanges();
			OutputHandler.Send($"You withdraw your application {chargen.Name} from consideration.");
		}

		State = LoggedInMenuState.LoggedInMenu;
		SetupCommands();
		OutputHandler.Send(_menuText);
	}

	#endregion

	#region IControllable Members

	public override void AssumeControl(IController controller)
	{
		Controller = controller;
		OutputHandler = controller.OutputHandler;
		// When assuming control of a menu in CheckedOldPassword State, don't echo the main menu text
		if (State != LoggedInMenuState.CheckedOldPassword)
		{
			var sb = new StringBuilder();
			sb.AppendLine(_menuText);
			OutputHandler.Send(sb.ToString(), false);
		}
	}

	#region Overrides of Menu

	public override void SilentAssumeControl(IController controller)
	{
		Controller = controller;
		OutputHandler = controller.OutputHandler;
	}

	#endregion

	public override bool ExecuteCommand(string command)
	{
		return _commands[State].Execute(command, CharacterState.Any, PermissionLevel.Any, OutputHandler);
	}

	public override void LoseControl(IController controller)
	{
		OutputHandler = null;
	}

	#endregion

	#region Character Login

	private void ConnectCharacterBack(string command)
	{
		State = LoggedInMenuState.LoggedInMenu;
		OutputHandler.Send(_menuText);
	}

	private void ConnectCharacter(string command)
	{
		using (new FMDB())
		{
			var characterQuery = (from character in FMDB.Context.Characters
			                      where character.AccountId == Account.Id
			                            && character.Status == (int)CharacterStatus.Active
			                      orderby character.CreationTime
			                      select character).ToList();

			var chargensQuery = (from chargen in FMDB.Context.Chargens
			                     where
				                     chargen.AccountId == Account.Id &&
				                     chargen.Status <= (int)CharacterStatus.Submitted
			                     orderby chargen.Status == (int)CharacterStatus.Submitted
			                     select chargen).ToList();

			if (characterQuery.Count < 1 && chargensQuery.Count < 1)
			{
				OutputHandler.Send("\nYou have no characters!");
				State = LoggedInMenuState.LoggedInMenu;
				OutputHandler.Send(_menuText);
			}
			else
			{
				var sb = new StringBuilder();
				var characterSelectionManager = new CommandManager
				{
					FailedToFindCommand = "Invalid character. Please select one from the list."
				};

				characterSelectionManager.Add("0", ConnectCharacterBack);

				sb.AppendLine("\nYou have the following characters: ");
				var nextChoice = 1;

				// Alive characters appear first in the list
				foreach (
					var character in
					characterQuery
						.ToList())
				{
					sb.AppendLine(
						$"\t{nextChoice}: {character.Name} ({MUDConstants.CharacterStatusStrings[character.Status]})");
					characterSelectionManager.Add(nextChoice.ToString(CultureInfo.InvariantCulture),
						new TaggedCommand<MudSharp.Models.Character>(character, SelectCharacter));
					nextChoice++;
				}

				foreach (var character in chargensQuery)
				{
					sb.AppendLine(
						$"\t{nextChoice}: {character.Name} ({MUDConstants.CharacterStatusStrings[character.Status]})");
					characterSelectionManager.Add(nextChoice.ToString(CultureInfo.InvariantCulture),
						new TaggedCommand<Chargen>(character, SelectChargen));
					nextChoice++;
				}

				sb.AppendLine("\n\rPlease select a character to connect to, or type 0 to return to the main menu: ");
				OutputHandler.Send(sb.ToString(), false);
				State = LoggedInMenuState.LoginCharacterMenu;
				_commands[LoggedInMenuState.LoginCharacterMenu] = characterSelectionManager;
			}
		}
	}

	private void SelectChargen(Chargen character, string command)
	{
		if (character.Status == (int)CharacterStatus.Submitted)
		{
			OutputHandler.Send(
				$"{character.Name} is awaiting approval from staff, and so you cannot yet log in to them.");
			return;
		}

		_nextContext = new ChargenMenu(character, Account, Gameworld);
	}

	private void SelectCharacter(MudSharp.Models.Character character, string command)
	{
		if (command == "")
		{
			State = LoggedInMenuState.LoggedInMenu;
			OutputHandler.Send(_menuText);
			return;
		}

		if (Gameworld.MaintenanceMode.HasFlag(MaintenanceModeSetting.NoLogin) &&
		    Account.Authority.Level < PermissionLevel.JuniorAdmin)
		{
			OutputHandler.Send("The game is in maintenance mode at the moment and you cannot log in to characters.");
			State = LoggedInMenuState.LoggedInMenu;
			OutputHandler.Send(_menuText);
			return;
		}

		if (character.Status != (int)CharacterStatus.Active)
		{
			switch (character.Status)
			{
				case (int)CharacterStatus.Deceased:
					OutputHandler.Send($"{character.Name.Proper()} is dead, and so you cannot log in to them.");
					return;
				case (int)CharacterStatus.Retired:
					OutputHandler.Send(
						$"{character.Name.Proper()} has been retired, and so you cannot log in to them.");
					return;
				case (int)CharacterStatus.Suspended:
					OutputHandler.Send($"{character.Name.Proper()} is suspended, and so you cannot log in to them.");
					return;
				default:
					OutputHandler.Send("That is not a valid character to log in to.");
					return;
			}
		}

		var loginCharacter = Gameworld.Characters.Get(character.Id);
		if (loginCharacter == null)
		{
			using (new FMDB())
			{
				character = FMDB.Context.Characters.Find(character.Id);
				loginCharacter = Gameworld.TryGetCharacter(character.Id, true);

				if (ShowCharacterIntroMenu.IsRequired(character, loginCharacter, Gameworld))
				{
					var template = Gameworld.CharacterIntroTemplates.OrderByDescending(x => x.ResolutionPriority)
					                        .FirstOrDefault(x => x.AppliesToCharacter(loginCharacter));
					if (template != null)
					{
						_nextContext = new ShowCharacterIntroMenu("Character Introduction...", Gameworld,
							loginCharacter, template.GetCharacterIntro());
						return;
					}

					Gameworld.Add(loginCharacter, false);
					loginCharacter.Register(OutputHandler);
					var startingLocation =
						(Gameworld.ChargenStoryboard.StageScreenMap[ChargenStage.SelectStartingLocation] as
							StartingLocationPickerScreenStoryboard)?.Locations.FirstOrDefault(
							x => loginCharacter.Roles.Contains(x.Role));
					startingLocation?.OnCommenceProg?.Execute(loginCharacter);
				}
				else
				{
					loginCharacter.Register(OutputHandler);
				}

				character.ShownIntroductionMessage = true;
				FMDB.Context.SaveChanges();
			}

			if (InvalidCharacteristicsMenu.IsRequired(loginCharacter))
			{
				_nextContext = new InvalidCharacteristicsMenu(loginCharacter, false);
				return;
			}

			var scripted = Gameworld.ScriptedEvents.FirstOrDefault(x => x.IsReady && !x.IsFinished && x.Character == loginCharacter && x.EarliestDate <= DateTime.UtcNow);
			if (scripted is not null)
			{
				_nextContext = new ScriptedEventMenu(loginCharacter, scripted, false);
				return;
			}

			if (loginCharacter.Location != null)
			{
				loginCharacter.Location.Login(loginCharacter);
			}
			else
			{
				Gameworld.Cells.First().Login(loginCharacter);
			}
			Gameworld.Add(loginCharacter, false);
		}
		else
		{
			if (InvalidCharacteristicsMenu.IsRequired(loginCharacter))
			{
				_nextContext = new InvalidCharacteristicsMenu(loginCharacter, false);
				return;
			}

			var scripted = Gameworld.ScriptedEvents.FirstOrDefault(x => x.IsReady && !x.IsFinished && x.Character == loginCharacter && x.EarliestDate <= DateTime.UtcNow);
			if (scripted is not null)
			{
				_nextContext = new ScriptedEventMenu(loginCharacter, scripted, false);
				return;
			}

			loginCharacter.Register(OutputHandler);
			loginCharacter.OutputHandler.Handle(new EmoteOutput(new Emote("@ has reconnected.", loginCharacter),
				flags: OutputFlags.SuppressSource | OutputFlags.SuppressObscured));
		}

		loginCharacter.RemoveAllEffects(x => x.IsEffectType<LinkdeadLogout>());
		_nextContext = loginCharacter;
	}

	#endregion

	#region Account Registration

	private void RegisterAccount(string command)
	{
		State = LoggedInMenuState.RegisterAccount;
		OutputHandler.Send(
			$"\nPlease enter the confirmation text that was emailed to {Account.EmailAddress} in order to register your account: ",
			false);
	}

	private void RegisterAccountSubmit(string command)
	{
		if (Account.TryAccountRegistration(command))
		{
			OutputHandler.Send(
				"\nCongratulations! Your account is now registered and can access full account functionality.");
			SetupCommands();
			State = LoggedInMenuState.LoggedInMenu;
			OutputHandler.Send(_menuText, false);
		}
		else
		{
			OutputHandler.Send(
				"\nThat code was not correct! Please refer to the email that was sent to you. The code is not case sensitive and only contains letters.");
			State = LoggedInMenuState.LoggedInMenu;
			OutputHandler.Send(_menuText, false);
		}
	}

	private void ResendRegistrationEmail(string command)
	{
		State = LoggedInMenuState.ResendAccountEmail;
		OutputHandler.Send(
			$"\nThe email address that we have on file for you is {Account.EmailAddress}. Is this email the correct email to send the registration code to? (y/n): ",
			false);
	}

	private void ResendRegistrationEmailConfirm(string command)
	{
		// TODO - consider placing some kind of a timer on this so people can't force FutureMUD to spam an email address
		if ("yes".StartsWith(command, StringComparison.InvariantCultureIgnoreCase))
		{
			using (new FMDB())
			{
				var dbAccount = FMDB.Context.Accounts.Find(Account.Id);
				dbAccount.RegistrationCode = SecurityUtilities.GetRandomString(8,
					Constants.ValidRandomCharacters.ToCharArray());
				FMDB.Context.SaveChanges();

				EmailHelper.Instance.SendEmail(EmailTemplateTypes.NewAccountVerification, Account.EmailAddress,
					Account.Name.Proper(), dbAccount.RegistrationCode);
			}

			OutputHandler.Send(
				$"\nAn email has been sent to you at {Account.EmailAddress} containing your registration code. If you have not received it, please be sure to check your junk mail folder.");
			State = LoggedInMenuState.LoggedInMenu;
			OutputHandler.Send(_menuText, false);
		}
		else
		{
			State = LoggedInMenuState.ChangeEmail;
			OutputHandler.Send(
				"\nYour current email address is " + Account.EmailAddress + ". Please enter a new one: ", false);
		}
	}

	#endregion

	#region Email Options

	private void ChangeEmail(string command)
	{
		State = LoggedInMenuState.ChangeEmail;
		OutputHandler.Send(
			"\nYour current email address is " + Account.EmailAddress + ". Please enter a new one: ", false);
	}

	private void CommitEmail(string command)
	{
		if (Regex.Match(command, Constants.EmailRegex, RegexOptions.IgnoreCase).Success)
		{
			using (new FMDB())
			{
				if (FMDB.Context.Accounts.Any(acc => acc.Email == command))
				{
					OutputHandler.Send(
						"\nThat email address is already in use by another account. Please enter another or type 'q' to leave your email address as " +
						Account.EmailAddress + ".", false);
				}
				else
				{
					Account.EmailAddress = command;
					var dbAccount = FMDB.Context.Accounts.FirstOrDefault(acc => acc.Id == Account.Id);
					if (dbAccount != null)
					{
						dbAccount.Email = command;
						FMDB.Context.SaveChanges();
						OutputHandler.Send("\nYour email address has been set to " + command + ".", false);
					}
					else
					{
						OutputHandler.Send("\nAn error was encountered. Could not update email address.", false);
						throw new ApplicationException("Account: " + Account.Name + ", ID: " + Account.Id +
						                               " does not exist in the database. Cannot change email address.");
					}

					EmailHelper.Instance.SendEmail(EmailTemplateTypes.AccountEmailChanged, Account.EmailAddress,
						Account.Name.Proper(), Account.LastIP, command);
					State = LoggedInMenuState.LoggedInMenu;
					OutputHandler.Send(_menuText, false);
				}
			}
		}
		else if (command == "q")
		{
			OutputHandler.Send("\nEmail address not changed.", false);
			State = LoggedInMenuState.LoggedInMenu;
			OutputHandler.Send(_menuText);
		}
		else
		{
			OutputHandler.Send(
				"\nYou have entered an invalid email address. Please enter a valid one or type 'q' to leave your email address as " +
				Account.EmailAddress + ".", false);
		}
	}

	#endregion

	#region Password Options

	private void ChangePassword(string command)
	{
		State = LoggedInMenuState.ChangePassword;
		OutputHandler.Send("\nPlease enter your previous password:", false);
	}

	private void CheckOldPassword(string command)
	{
		using (new FMDB())
		{
			var currentAccount = (from account in FMDB.Context.Accounts
			                      where account.Id == Account.Id
			                      select account).FirstOrDefault();
			if (currentAccount == null)
			{
				OutputHandler.Send("\nAn error was encountered. Could not find account to update password for.");
				throw new ApplicationException("Account: " + Account.Name + ", ID: " + Account.Id +
				                               " does not exist in the database. Cannot change password.");
			}

			if (SecurityUtilities.VerifyPassword(command, currentAccount.Password, currentAccount.Salt))
			{
				State = LoggedInMenuState.CheckedOldPassword;
				_passwordEntryCount = 0;
				OutputHandler.Send("\nPlease enter a new password:", false);
			}
			else
			{
				if (++_passwordEntryCount > 2)
				{
					// TODO: Stop bruteforcing passwords, yo. Do a ban here!
					OutputHandler.Send("\nToo many incorrect password entries.");
					OutputHandler.Send("\nYou have been logged out!");
					_nextContext = new Quit();
				}
				else
				{
					OutputHandler.Send("\nPassword incorrect.\nPlease enter your previous password:", false);
				}
			}
		}
	}

	private void EnterNewPassword(string command)
	{
		State = LoggedInMenuState.EnteredNewPassword;
		_newPasswordSalt = SecurityUtilities.GetSalt64();
		_newPasswordHash = SecurityUtilities.GetPasswordHash(command, _newPasswordSalt);
		OutputHandler.Send("\nPlease enter the new password again:", false);
	}

	private void CommitPassword(string command)
	{
		if (SecurityUtilities.VerifyPassword(command, _newPasswordHash, _newPasswordSalt))
		{
			using (new FMDB())
			{
				var currentAccount = (from account in FMDB.Context.Accounts
				                      where account.Id == Account.Id
				                      select account).FirstOrDefault();
				if (currentAccount == null)
				{
					OutputHandler.Send(
						"\nAn error was encountered. Could not find account to update password for.", false);
					throw new ApplicationException("Account: " + Account.Name + ", ID: " + Account.Id +
					                               " does not exist in the database. Cannot change password.");
				}

				currentAccount.Password = _newPasswordHash;
				currentAccount.Salt = _newPasswordSalt;
				FMDB.Context.SaveChanges();
				OutputHandler.Send("\nPassword updated!", false);
				EmailHelper.Instance.SendEmail(EmailTemplateTypes.AccountPasswordChanged, Account.EmailAddress,
					Account.Name.Proper(), Account.LastIP, command);
				State = LoggedInMenuState.LoggedInMenu;
				OutputHandler.Send(_menuText);
				_passwordEntryCount = 0;
			}
		}
		else
		{
			var hasRecoveryCode = false;
			using (new FMDB())
			{
				var currentAccount = (from account in FMDB.Context.Accounts
				                      where account.Id == Account.Id
				                      select account).FirstOrDefault();
				hasRecoveryCode = currentAccount.RecoveryCode != null;
			}

			if (hasRecoveryCode)
			{
				OutputHandler.Send("\nCould not match those passwords.", false);
				State = LoggedInMenuState.CheckedOldPassword;
				OutputHandler.Send("\nPlease enter a new password:", false);
			}
			else
			{
				OutputHandler.Send("\nCould not match new password.", false);
				State = LoggedInMenuState.LoggedInMenu;
				OutputHandler.Send(_menuText);
			}
		}
	}

	#endregion
}