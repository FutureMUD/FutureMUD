using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Dapper;
using Microsoft.EntityFrameworkCore;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Commands;
using MudSharp.Database;
using MudSharp.Email;
using MudSharp.Framework;
using MudSharp.Network;
using TimeZoneConverter;

namespace MudSharp.Menus;

public class MainMenu : Menu, IController
{
	private static string _passwordRecoveryString => Futuremud.Games.First().GetStaticString("PasswordRecoveryMenuText").SubstituteANSIColour().Wrap(80);

	private static string _passwordRecoveryEnterCodeString => Futuremud.Games.First().GetStaticString("PasswordRecoveryEnterCodeText").SubstituteANSIColour().Wrap(80);

	private static string _passwordRecoverySendEmailString => Futuremud.Games.First().GetStaticString("PasswordRecoverySendEmailText").SubstituteANSIColour().Wrap(80);

	private readonly Dictionary<MainMenuState, IExecutable> _commands = new();

	private string _accountName;
	private string _accountPassword;
	private readonly long _accountSalt = SecurityUtilities.GetSalt64();
	private string _culture;
	private string _email;
	private uint _linewidth;
	private CultureInfo _selectedCulture;

	private MainMenuState _state;
	private string _timezone;
	private decimal? _timezoneGMTOffset;
	private bool _unicode;
	private string _unitPreference;

	public MainMenu(IFuturemud gameworld, IPlayerController accountController)
		: base(string.Empty, gameworld)
	{
		_state = MainMenuState.FrontMenu;

		Gameworld = gameworld;
		var version = Assembly.GetCallingAssembly().GetName().Version;
		var versionString = $"v{version.Major}.{version.Minor}.{version.Build}.{version.Revision.ToString("0000")}";
		_menuText = string.Format(gameworld.GetStaticString("MainMenu"), versionString,
			DateTime.UtcNow.Year.ToString("0000"),
			gameworld.Name.TitleCase());

		var frontPageManager = new CommandManager("Invalid command. Please enter an option from the menu.\n");
		frontPageManager.Add("", command => AccountController.OutputHandler.Send(_menuText.SubstituteANSIColour()));
		frontPageManager.Add("c", CreateAccount);
		frontPageManager.Add("l", ConnectAccount);
		frontPageManager.Add("r", RecoverPassword);
		_commands.Add(MainMenuState.FrontMenu, frontPageManager);

		_commands.Add(MainMenuState.CreateAccountLogin, new Command(CreateAccountLogin));
		_commands.Add(MainMenuState.CreateAccountPassword, new Command(CreateAccountPassword));
		_commands.Add(MainMenuState.CreateAccountPasswordConfirm, new Command(CreateAccountPasswordConfirm));
		_commands.Add(MainMenuState.CreateAccountEmail, new Command(CreateAccountEmail));
		_commands.Add(MainMenuState.CreateAccountLineWidth, new Command(CreateAccountLineWidth));
		//_commands.Add(MainMenuState.CreateAccountUnicodeSupport, new Command(CreateAccountUnicode));
		_commands.Add(MainMenuState.CreateAccountCulture, new Command(CreateAccountCulture));
		_commands.Add(MainMenuState.CreateAccountTimeZoneGMTOffset, new Command(CreateAccountTimezoneGMTOffset));
		_commands.Add(MainMenuState.CreateAccountTimeZone, new Command(CreateAccountTimezone));

		_commands.Add(MainMenuState.ConnectAccountLogin, new Command(ConnectAccountLogin));
		_commands.Add(MainMenuState.ConnectAccountPassword, new Command(ConnectAccountPassword));

		_commands.Add(MainMenuState.RecoverPasswordSelectOption, new Command(RecoverPasswordSelectOption));
		_commands.Add(MainMenuState.RecoverPasswordEnterAccountName, new Command(RecoverPasswordEnterAccountName));
		_commands.Add(MainMenuState.RecoverPasswordEnterCode, new Command(RecoverPasswordEnterCode));
		_commands.Add(MainMenuState.RecoverPasswordSendEmail, new Command(RecoverPasswordSendEmail));

		_commands.Add(MainMenuState.SelectUnitPreference, new Command(SelectUnitPreference));

		AccountController = accountController;
	}

	public override int Timeout => 60000 * 15;

	public IPlayerController AccountController { get; protected set; }

	void IController.Close()
	{
		_nextContext = new Quit();
	}

	private enum MainMenuState
	{
		FrontMenu,
		CreateAccountLogin,
		CreateAccountPassword,
		CreateAccountPasswordConfirm,
		CreateAccountEmail,
		CreateAccountUnicodeSupport,
		CreateAccountLineWidth,
		CreateAccountTimeZoneGMTOffset,
		CreateAccountTimeZone,
		CreateAccountCulture,
		ConnectAccountLogin,
		ConnectAccountPassword,
		RecoverPasswordSelectOption,
		RecoverPasswordSendEmail,
		RecoverPasswordEnterAccountName,
		RecoverPasswordEnterCode,
		SelectUnitPreference
	}

	#region IControllable Members

	private bool _isBanned;

	public bool CheckBanned()
	{
		try
		{
			using (new FMDB())
			{
				if (AccountController == null)
				{
					return false;
				}

				var count = FMDB.Connection.Query<int>(
					$"select count(0) from Bans where '{AccountController?.IPAddress ?? "0.0.0.0"}' like IpMask and (Expiry is null or Expiry >= utc_timestamp())");
				if (count.First() > 0)
				{
					OutputHandler.Send(Gameworld.GetStaticString("MainMenuSiteBanned"));
					return true;
				}
			}

			return false;
		}
		catch
		{
			return false;
		}
	}

	public bool CheckAlreadyRegisteredIP()
	{
		using (new FMDB())
		{
			return
				FMDB.Context.LoginIps.Any(
					x => x.IpAddress == AccountController.IPAddress && x.AccountRegisteredOnThisIp);
		}
	}

	public override void AssumeControl(IController controller)
	{
		Controller = controller;
		OutputHandler = controller.OutputHandler;
		if (CheckBanned())
		{
			_nextContext = new Quit();
			_isBanned = true;
		}
		else
		{
			OutputHandler.Send(_menuText, false);
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
		if (_isBanned)
		{
			return true;
		}

		switch (command.ToLowerInvariant())
		{
			case "q":
			case "quit":
			case "exit":
			case "stop":
				_nextContext = new MainMenu(Gameworld, AccountController);
				return true;
		}

		return _commands[_state].Execute(command, CharacterState.Any, PermissionLevel.Any, OutputHandler);
	}

	public override void LoseControl(IController controller)
	{
		OutputHandler = null;
	}

	#endregion

	#region MainMenu Methods

	private void RecoverPassword(string command)
	{
		OutputHandler.Send(_passwordRecoveryString, false);
		_state = MainMenuState.RecoverPasswordSelectOption;
	}

	private void RecoverPasswordSelectOption(string command)
	{
		if (!uint.TryParse(command, out var value) || value > 2)
		{
			OutputHandler.Send(
				"\nYou must enter either 1 to send a code, or 2 to enter one. Please select an option, or 0 to return to main menu: ",
				false);
			return;
		}

		switch (value)
		{
			case 0:
				_state = MainMenuState.FrontMenu;
				OutputHandler.Send(_menuText, false);
				break;
			case 1:
				_state = MainMenuState.RecoverPasswordSendEmail;
				OutputHandler.Send(_passwordRecoverySendEmailString, false);
				break;
			case 2:
				_state = MainMenuState.RecoverPasswordEnterAccountName;
				OutputHandler.Send("Account Recovery\n\rPlease enter your account name: ", false);
				break;
		}
	}

	private void RecoverPasswordSendEmail(string command)
	{
		if (string.IsNullOrEmpty(command))
		{
			OutputHandler.Send(
				"\nPlease enter the name of your account, your account email, or 0 to return to the main menu: ",
				false);
			return;
		}

		using (new FMDB())
		{
			var match = EmailRegex.Match(command);
			var account = match.Success
				? FMDB.Context.Accounts.FirstOrDefault(x => x.Email == command)
				: FMDB.Context.Accounts.FirstOrDefault(x => x.Name == command);

			if (account == null)
			{
				if (match.Success)
				{
					// Don't leak whether or not an email address is used by saying it cannot be found. - Case
					OutputHandler.Send(
						"An Account Recovery Code for the account registered to the specified email address has been sent. Please allow a few minutes for the email to arrive.");
					_state = MainMenuState.FrontMenu;
					OutputHandler.Send(_menuText, false);
				}
				else
				{
					OutputHandler.Send("There are no accounts registered with that name.");
					_state = MainMenuState.FrontMenu;
					OutputHandler.Send(_menuText, false);
				}

				return;
			}

			var code = SecurityUtilities.GetRandomString(20, Constants.ValidRandomCharacters.ToCharArray());
			account.RecoveryCode = SecurityUtilities.GetPasswordHash(code, account.Salt);

			EmailHelper.Instance.SendEmail(EmailTemplateTypes.AccountRecoveryCode, account.Email,
				account.Name.TitleCase(), AccountController.IPAddress, code);
			OutputHandler.Send(match.Success
				? "An Account Recovery Code for the account registered to the specified email address has been sent. Please allow a few minutes for the email to arrive."
				: "An Account Recovery Code for the account has been sent to its registered email address. Please allow a few minutes for the email to arrive.");

			FMDB.Context.SaveChanges();
			_state = MainMenuState.FrontMenu;
			OutputHandler.Send(_menuText, false);
		}
	}

	private void RecoverPasswordEnterAccountName(string command)
	{
		var ss = new StringStack(command);
		_accountName = ss.Pop();
		_state = MainMenuState.RecoverPasswordEnterCode;
		OutputHandler.Send(_passwordRecoveryEnterCodeString);
	}

	private void RecoverPasswordEnterCode(string command)
	{
		using (new FMDB())
		{
			var accountLookup = (from dbAccount in FMDB.Context.Accounts
			                     where dbAccount.Name.Equals(_accountName)
			                     select dbAccount).FirstOrDefault();

			if (accountLookup == null ||
			    !SecurityUtilities.VerifyPassword(command, accountLookup.RecoveryCode, accountLookup.Salt))
			{
				OutputHandler.Send(
					"That is not a valid Account Recovery Code. Please double check the code that you were sent. Codes are not case sensitive.");
				_state = MainMenuState.FrontMenu;
				OutputHandler.Send(_menuText, false);
				return;
			}

			accountLookup.RecoveryCode = null;
			OutputHandler.Send($"Recovering Account {accountLookup.Name.TitleCase()}...");
			OutputHandler.Send("\nPlease enter a new password: ", false);
			Gameworld.SystemMessage(
				$"Account {accountLookup.Name.Proper()} has successfully used the Account Recovery Tool from IP Address {AccountController.IPAddress}.",
				true);
			_nextContext = new LoggedInMenu(Gameworld.TryAccount(accountLookup), Gameworld)
			{
				State = LoggedInMenu.LoggedInMenuState.CheckedOldPassword
			};
		}
	}

	private void CreateAccount(string command)
	{
		if (Gameworld.MaintenanceMode.HasFlag(MaintenanceModeSetting.NoAccountLogin))
		{
			OutputHandler.Send(Futuremud.Games.First().GetStaticString("AccountCreationDisabledMessage").SubstituteANSIColour());
			_state = MainMenuState.FrontMenu;
			OutputHandler.Send(_menuText);
			return;
		}

		OutputHandler.Send(
			string.Format(Futuremud.Games.First().GetStaticString("AccountCreationMessage"), Futuremud.Games.First().Name).SubstituteANSIColour()
				.Wrap(80));

		if (CheckAlreadyRegisteredIP())
		{
			OutputHandler.Send(
				"\nThere is already an account registered from this IP. Please speak to the game administrators about allowing a new one to be created from this IP.");
		}

		OutputHandler.Send(Gameworld.GetStaticString("CreateAccountName").SubstituteANSIColour().Wrap(80), false);
		_state = MainMenuState.CreateAccountLogin;
	}

	private void CreateAccountLogin(string command)
	{
		var ss = new StringStack(command);
		_accountName = ss.Pop();
		if (_accountName.Length < 2)
		{
			OutputHandler.Send("\nAccount names must be at least 2 letters long. Please select another: ", false);
			return;
		}

		if (_accountName.Any(x => !char.IsLetterOrDigit(x) && x != '_'))
		{
			OutputHandler.Send(
				"\nAccount names must contain only letters, numbers and underscores. Please select another: ",
				false);
			return;
		}

		if (_accountName.Any(x => x > 255))
		{
			OutputHandler.Send("\nAccount names may not contain unicode characters. Please select another: ",
				false);
			return;
		}

		if (!char.IsLetter(_accountName[0]))
		{
			OutputHandler.Send("\nAccount names must begin with a letter. Please select another: ", false);
			return;
		}

		using (new FMDB())
		{
			if (
				FMDB.Context.Accounts.Any(
					acc => acc.Name == _accountName))
			{
				// Cannot enforce uniqueness of non primary keys in Entity Model
				OutputHandler.Send("There is already an account with that name. Please use another: ", false);
				return;
			}
		}

		OutputHandler.Send(
			Futuremud.Games.First().GetStaticString("PasswordSelectionMessage")
				.SubstituteANSIColour().Wrap(80), false);
		_state = MainMenuState.CreateAccountPassword;
	}

	private void CreateAccountPassword(string command)
	{
		if (command.Length < 8)
		{
			OutputHandler.Send("\nPasswords must be at least 8 characters long. Please use another: ", false);
			return;
		}

		_accountPassword = SecurityUtilities.GetPasswordHash(command, _accountSalt);

		OutputHandler.Send("\nPlease reenter your password: ", false);
		_state = MainMenuState.CreateAccountPasswordConfirm;
	}

	private void CreateAccountPasswordConfirm(string command)
	{
		if (!SecurityUtilities.VerifyPassword(command, _accountPassword, _accountSalt))
		{
			OutputHandler.Send("\nThe passwords do not match - please try again.");
			OutputHandler.Send(
				Futuremud.Games.First().GetStaticString("PasswordSelectionMessage")
					.SubstituteANSIColour().Wrap(80), false);
			_accountPassword = null;
			_state = MainMenuState.CreateAccountPassword;
		}
		else
		{
			OutputHandler.Send(Gameworld.GetStaticString("CreateAccountEmail").SubstituteANSIColour().Wrap(80),
				false);
			_state = MainMenuState.CreateAccountEmail;
		}
	}

	private static readonly Regex EmailRegex =
		new(
			@"^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+(?:[A-Z]{2}|com|org|net|edu|gov|us|mil|biz|info|mobi|name|aero|asia|jobs|museum)$",
			RegexOptions.IgnoreCase);

	private void CreateAccountEmail(string command)
	{
		if (!EmailRegex.IsMatch(command))
		{
			OutputHandler.Send("\nThat is not a valid email address. Please enter a valid email address: ", false);
			return;
		}

		using (new FMDB())
		{
			if (FMDB.Context.Accounts.Any(x => x.Email == command))
			{
				OutputHandler.Send(
					"\nThere is already an account registered with that email. If you have lost access to your account, please try password recovery. ",
					false);
				return;
			}
		}

		_email = command;

		if (Gameworld.GetStaticBool("UseSimpleAccountCreation"))
		{
			_culture = Gameworld.GetStaticConfiguration("DefaultAccountCulture");
			_timezone = Gameworld.GetStaticConfiguration("DefaultAccountTimezone");
			_unitPreference = Gameworld.GetStaticConfiguration("DefaultAccountUnitPreference");
			_unicode = false;
			_linewidth = (uint)Gameworld.GetStaticInt("DefaultAccountLineWidth");
			CreateAccount();
			return;
		}

		OutputHandler.Send(Gameworld.GetStaticString("CreateAccountLineWrap").SubstituteANSIColour().Wrap(80), false);
		_state = MainMenuState.CreateAccountLineWidth;
	}

	private bool UnitPreferenceSelectionRequired()
	{
		if (Gameworld.UnitManager.Units.Select(x => x.System).Distinct().Count() > 1)
		{
			return true;
		}

		_unitPreference = Gameworld.UnitManager.Units.Select(x => x.System).Distinct().Single();
		return false;
	}

	private void SelectUnitPreference(string command)
	{
		if (string.IsNullOrEmpty(command))
		{
			OutputHandler.Send("Please enter your preference for unit display: ", false);
			return;
		}

		var preference = int.TryParse(command, out var value)
			? Gameworld.UnitManager.Units.Select(x => x.System)
			           .Distinct()
			           .OrderBy(x => x)
			           .ElementAtOrDefault(value - 1)
			: Gameworld.UnitManager.Units.Select(x => x.System)
			           .Distinct()
			           .FirstOrDefault(x => x.Equals(command, StringComparison.InvariantCultureIgnoreCase));
		if (string.IsNullOrEmpty(preference))
		{
			OutputHandler.Send("That is not a valid unit display preference. Please enter a valid one: ", false);
			return;
		}

		_unitPreference = preference;
		OutputHandler.Send($"You will now see all units in the {_unitPreference.ColourValue()} system.");
		_state = MainMenuState.CreateAccountCulture;
		using (new FMDB())
		{
			var number = 1;
			OutputHandler.Send(string.Format(Gameworld.GetStaticString("CreateAccountCulture"),
				                         (from culture in FMDB.Context.CultureInfos
				                          orderby culture.Order, culture.Id
				                          select culture)
				                         .ToList()
				                         .Select(culture => $"{number++}) #6{culture.DisplayName}#0 #2({culture.Id})#0")
				                         .ArrangeStringsOntoLines(_linewidth / 40, _linewidth))
			                         .SubstituteANSIColour().Wrap((int)_linewidth), false);
		}
	}

	private void CreateAccountLineWidth(string command)
	{
		if (!uint.TryParse(command, out var value))
		{
			OutputHandler.Send(
				"\nYou must enter a valid number for your line width. Please enter a line width: ", false);
			return;
		}

		if (value < 40)
		{
			OutputHandler.Send(
				"\nValues less than 40 are not supported and thus not permitted. Please enter a line width: ",
				false);
			return;
		}

		_linewidth = value;
		//_state = MainMenuState.CreateAccountUnicodeSupport;
		//OutputHandler.Send(Gameworld.GetStaticString("CreateAccountUnicode").SubstituteANSIColour().Wrap((int)_linewidth), false);

		// Note - disabled Unicode preferences for now. People will have to opt-in in game.

		_state = MainMenuState.SelectUnitPreference;
		var index = 1;
		OutputHandler.Send(string.Format(Gameworld.GetStaticString("CreateAccountUnitPreference"),
			Gameworld.UnitManager.Units.Select(x => x.System).Distinct().OrderBy(x => x).Select(x =>
				$"{index++}) #2{x}#0").ArrangeStringsOntoLines(1, 80)
		).SubstituteANSIColour().Wrap((int)80), false);
	}

	private void CreateAccountUnicode(string command)
	{
		_unicode = "yes".StartsWith(command, StringComparison.InvariantCultureIgnoreCase);
		_state = MainMenuState.SelectUnitPreference;
		var index = 1;
		OutputHandler.Send(string.Format(Gameworld.GetStaticString("CreateAccountUnitPreference"),
			Gameworld.UnitManager.Units.Select(x => x.System).Distinct().OrderBy(x => x).Select(x =>
				$"{index++}) {x}").ArrangeStringsOntoLines(1, 80)
		).SubstituteANSIColour().Wrap((int)_linewidth), false);
	}

	private void CreateAccountCulture(string command)
	{
		if (string.IsNullOrEmpty(command))
		{
			OutputHandler.Send("\nWhich culture do you wish to select? ", false);
			return;
		}

		CultureInfo selectedCulture = null;
		if (int.TryParse(command, out var value))
		{
			using (new FMDB())
			{
				var whichCulture = (from culture in FMDB.Context.CultureInfos
				                    orderby culture.Order, culture.Id
				                    select culture).ToList().ElementAtOrDefault(value - 1);
				if (whichCulture != null)
				{
					selectedCulture = CultureInfo.GetCultureInfo(whichCulture.Id);
				}
			}
		}
		else
		{
			try
			{
				selectedCulture = CultureInfo.GetCultureInfo(command);
			}
			catch (CultureNotFoundException)
			{
			}
		}

		if (selectedCulture == null)
		{
			OutputHandler.Send("\nThat is not a valid selection. Please select a culture: ", false);
			return;
		}

		_culture = selectedCulture.Name;
		_selectedCulture = selectedCulture;
		_state = MainMenuState.CreateAccountTimeZoneGMTOffset;
		OutputHandler.Send(
			Gameworld.GetStaticString("CreateAccountGMTOffset").SubstituteANSIColour().Wrap((int)_linewidth), false);
	}

	private static readonly Regex _gmtOffsetRegex =
		new(@"^\s*(?:GMT\s*){0,1}(?<number>[-+]{0,1}[0-9]{1,2}(?:\.[0-9]{1,2}){0,1})\s*$",
			RegexOptions.IgnoreCase);

	private void CreateAccountTimezoneGMTOffset(string command)
	{
		if (string.IsNullOrEmpty(command))
		{
			OutputHandler.Send(
				"\nPlease enter your GMT Offset (for example GMT-5 for US East Coast) or \"Unknown\" to be shown a list of all Timezones.",
				false);
			return;
		}

		if (command.Equals("unknown", StringComparison.InvariantCultureIgnoreCase))
		{
			_state = MainMenuState.CreateAccountTimeZone;
			SendTimeZoneString();
			return;
		}

		var match = _gmtOffsetRegex.Match(command);
		if (!match.Success)
		{
			OutputHandler.Send("\nThat is not a valid format, please enter in the form e.g. GMT-5 or GMT+10.",
				false);
			return;
		}

		var number = decimal.Parse(match.Groups["number"].Value);
		using (new FMDB())
		{
			if (!FMDB.Context.TimeZoneInfos.AsNoTracking().Any(x => x.Order == number))
			{
				var closestHigher =
					FMDB.Context.TimeZoneInfos.AsNoTracking()
					    .Where(x => x.Order > number)
					    .OrderBy(x => x.Order)
					    .ToList()
					    .FirstOrDefault();
				var closestLower =
					FMDB.Context.TimeZoneInfos.AsNoTracking()
					    .Where(x => x.Order < number)
					    .OrderBy(x => x.Order)
					    .ToList()
					    .LastOrDefault();
				if (closestHigher != null && closestLower != null)
				{
					OutputHandler.Send(
						string.Format(_selectedCulture,
							"There are no Timezones defined for that offset. The two closest are at GMT{0:+#0.00;-#0.00} and GMT{1:+#0.00;-#0.00}. Please enter your desired GMT Offset: ",
							closestLower.Order, closestHigher.Order).Wrap((int)_linewidth), false);
					return;
				}

				var closest = closestHigher ?? closestLower;
				OutputHandler.Send(
					string.Format(_selectedCulture,
						"There are no Timezones defined for that offset. The closest is at GMT{0:+#0.00;-#0.00}. Please enter your desired GMT Offset: ",
						closest.Order).Wrap((int)_linewidth), false);
				return;
			}
		}

		_timezoneGMTOffset = number;
		_state = MainMenuState.CreateAccountTimeZone;
		SendTimeZoneString();
	}

	private void SendTimeZoneString()
	{
		using (new FMDB())
		{
			var index = 1;
			OutputHandler.Send(string.Format(Gameworld.GetStaticString("CreateAccountTimezone"),
				_timezoneGMTOffset.HasValue
					? $"Time Zones for GMT{_timezoneGMTOffset.Value:+#0.00;-#0.00}\n\n"
					: "All Time Zones:\n\n",
				(from tz in FMDB.Context.TimeZoneInfos
				 where !_timezoneGMTOffset.HasValue || _timezoneGMTOffset.Value == tz.Order
				 orderby tz.Order, tz.Display
				 select tz).ToList()
				           .Select(tz => $"\t{index++}) #6{tz.Display}#0 #2[{tz.Id}]#0")
				           .ArrangeStringsOntoLines(_linewidth / 60, _linewidth)
			).SubstituteANSIColour().Wrap((int)_linewidth), false);
		}
	}

	private void CreateAccountTimezone(string command)
	{
		if (string.IsNullOrEmpty(command))
		{
			OutputHandler.Send("\nWhich timezone do you wish to select? ", false);
			return;
		}

		TimeZoneInfo timezone = null;
		if (int.TryParse(command, out var value))
		{
			using (new FMDB())
			{
				var whichTz = (from tz in FMDB.Context.TimeZoneInfos
				               where !_timezoneGMTOffset.HasValue || _timezoneGMTOffset.Value == tz.Order
				               orderby tz.Order, tz.Display
				               select tz).ToList().ElementAtOrDefault(value - 1);
				if (whichTz != null)
				{
					timezone = TZConvert.GetTimeZoneInfo(whichTz.Id);
				}
			}
		}
		else
		{
			try
			{
				timezone = TZConvert.GetTimeZoneInfo(command);
			}
			catch (TimeZoneNotFoundException)
			{
			}
		}

		if (timezone == null)
		{
			OutputHandler.Send("\nThat is not a valid selection. Please select a timezone: ", false);
			return;
		}

		_timezone = timezone.Id;

		CreateAccount();
	}

	private void CreateAccount()
	{
		var newAccount = Account.Create(_accountName, _accountPassword, _accountSalt, _culture, _timezone, _unicode,
			(int)_linewidth, _email, _unitPreference, AccountController.IPAddress);

		if (newAccount)
		{
			OutputHandler.Send("\nAccount successfully created! You can login from the menu now.");
			_state = MainMenuState.FrontMenu;
			OutputHandler.Send(_menuText);
			Gameworld.GameStatistics.UpdateNewAccount();
		}
		else
		{
			OutputHandler.Send(
				"\nThat account could not be created. The account name may already be in use.\n This should not happen. Please contact the Futuremud team about this.");
			_state = MainMenuState.FrontMenu;
			OutputHandler.Send(_menuText);
		}
	}

	private void ConnectAccount(string command)
	{
		OutputHandler.Send("\nPlease enter your account name: ", false);
		_state = MainMenuState.ConnectAccountLogin;
	}

	private void ConnectAccountLogin(string command)
	{
		var ss = new StringStack(command);
		_accountName = ss.Pop();
		OutputHandler.Send("\nPlease enter your password: ", false);
		_state = MainMenuState.ConnectAccountPassword;
	}

	private void ConnectAccountPassword(string command)
	{
		var loginAttempt = Gameworld.LogIn(_accountName, command, AccountController);

		if (loginAttempt != null)
		{
			lock (Gameworld.Connections)
			{
				var existingConnection =
					Gameworld.Connections.FirstOrDefault(
						x =>
							x.ControlPuppet != null && x.ControlPuppet.Account == loginAttempt &&
							x.ControlPuppet != AccountController);
				existingConnection?.ControlPuppet.OutputHandler.Send("You have logged in from " +
				                                                     loginAttempt.LastIP +
				                                                     " and so been disconnected.");
				existingConnection?.ControlPuppet.Close();
			}

			AccountController.BindAccount(loginAttempt);
			_nextContext = new LoggedInMenu(loginAttempt, Gameworld);
		}
		else
		{
			OutputHandler.Send("Login name does not exist or password was incorrect.\n");
			_state = MainMenuState.FrontMenu;
			OutputHandler.Send(_menuText);
		}
	}

	#endregion
}