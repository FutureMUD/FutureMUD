using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.CharacterCreation;
using MudSharp.Database;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using System.IO;
using MudSharp.Commands.Modules;
using MudSharp.Network;
using MudSharp.FutureProg;
using System.Numerics;
using MudSharp.FutureProg.Functions;

namespace MudSharp.Discord;

public sealed class DiscordConnection : IDiscordConnection
{
	private static readonly byte[] ByteSeparators = { (byte)'\n' };
	private TcpClient _client;
	private NetworkStream _stream;
	private string _serverAuth;
	private DateTime _lastConnectionAttempt;

	public IFuturemud Gameworld { get; private set; }

	public DiscordConnection(IFuturemud gameworld)
	{
		Gameworld = gameworld;
	}

	public void SendMessageFromProg(ulong channel, string title, string message)
	{
		try
		{
			SendClientMessage(
				$"sendmessage {channel.ToString("F", CultureInfo.InvariantCulture.NumberFormat)} \"{title}\" {message}");
		}
		catch (SocketException)
		{
			CloseTcpConnection();
			OpenTcpConnection();
		}
	}

	public void NotifyPetition(string account, string message, string location)
	{
		try
		{
			SendClientMessage($"petition {account} \"{location}\" {message}");
		}
		catch (SocketException)
		{
			CloseTcpConnection();
			OpenTcpConnection();
		}
	}

	public void NotifyCharacterSubmission(string account, string name, long id)
	{
		try
		{
			SendClientMessage($"chargen {account} {id} {name}");
		}
		catch (SocketException)
		{
			CloseTcpConnection();
			OpenTcpConnection();
		}
	}

	public void NotifyShutdown(string shutdownAccount)
	{
		try
		{
			SendClientMessage($"shutdown {shutdownAccount}");
		}
		catch (SocketException)
		{
			CloseTcpConnection();
			OpenTcpConnection();
		}
	}

	public void NotifyCharacterApproval(string account, string name, string approver)
	{
		try
		{
			SendClientMessage($"chargen_approved {account} {approver} {name}");
		}
		catch (SocketException)
		{
			CloseTcpConnection();
			OpenTcpConnection();
		}
	}

	public void NotifyCharacterRejection(string account, string name, string reviewer)
	{
		try
		{
			SendClientMessage($"chargen_rejected {account} {reviewer} {name}");
		}
		catch (SocketException)
		{
			CloseTcpConnection();
			OpenTcpConnection();
		}
	}

	public void NotifyCrash(string crashMessage)
	{
		try
		{
			SendClientMessage($"crash {crashMessage}");
		}
		catch (SocketException)
		{
			CloseTcpConnection();
		}
	}

	public void NotifyBadEcho(string badEcho)
	{
		try
		{
			SendClientMessage($"badecho {badEcho}");
		}
		catch (SocketException)
		{
			CloseTcpConnection();
		}
	}

	public void NotifyAdmins(string echo)
	{
		try
		{
			SendClientMessage($"notifyadmins {echo}");
		}
		catch (SocketException)
		{
			CloseTcpConnection();
		}
	}

	public void NotifyCustomChannel(ulong channel, string header, string echo)
	{
		try
		{
			SendClientMessage($"custom {channel} \"{header}\" {echo}");
		}
		catch (SocketException)
		{
			CloseTcpConnection();
		}
	}

	public void NotifyDeath(ICharacter who)
	{
		try
		{
			SendClientMessage(
				$"notifydeath {who.Id} {who.Account.Name} {who.Location.Id} {who.RoomLayer.DescribeEnum()} \"{who.Location.HowSeen(who, colour: false, flags: PerceiveIgnoreFlags.IgnoreCanSee)}\" \"{who.HowSeen(who, colour: false, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreSelf)}\" \"{who.PersonalName.GetName(NameStyle.FullName)}\"");
		}
		catch (SocketException)
		{
			CloseTcpConnection();
		}
	}

	public void NotifyEnforcement(string subtype, ulong discordChannel, string otherText)
	{
		try
		{
			SendClientMessage($"enforcement {subtype} {discordChannel} {otherText}");
		}
		catch (SocketException)
		{
			CloseTcpConnection();
		}
	}

	private void SendClientMessage(string message)
	{
		var bytes = Encoding.Unicode.GetBytes(message).Concat(new byte[] { 1 });
		_client?.Client.Send(bytes.ToArray());
	}

	public bool OpenTcpConnection()
	{
		try
		{
			_lastConnectionAttempt = DateTime.UtcNow;
			_client = new TcpClient(Gameworld.GetStaticConfiguration("DiscordBotIpAddress"),
				Gameworld.GetStaticInt("DiscordBotPort"));
			_serverAuth = Gameworld.GetStaticConfiguration("DiscordAuthToken");
			_stream = _client.GetStream();
			var version = Assembly.GetCallingAssembly().GetName().Version;
			var versionString = $"v{version.Major}.{version.Minor}.{version.Build}.{version.Revision.ToString("0000")}";
			SendClientMessage($"login {_serverAuth} {versionString}");
			return true;
		}
		catch (SocketException)
		{
			CloseTcpConnection();
			return false;
		}
	}

	public void CloseTcpConnection()
	{
		try
		{
			_client?.Close();
			_stream?.Close();
		}
		catch (Exception)
		{
		}

		_client = null;
		_stream = null;
	}

	public void HandleMessages()
	{
		if (_client == null)
		{
			if (DateTime.UtcNow - _lastConnectionAttempt < TimeSpan.FromSeconds(15))
			{
				return;
			}

			if (!OpenTcpConnection())
			{
				return;
			}
		}

		try
		{
			if (!_stream.DataAvailable)
			{
				return;
			}

			var inputBuffer = new byte[4096];
			var bytes = _stream.Read(inputBuffer, 0, 4096);

			var splitBytes =
				inputBuffer.Take(bytes).ToArray().SplitDelimiter(ByteSeparators).ToList();
			foreach (var command in splitBytes)
			{
				HandleTcpCommand(Encoding.Unicode.GetString(command));
			}
		}
		catch (SocketException)
		{
			CloseTcpConnection();
			OpenTcpConnection();
		}
	}

	private void HandleTcpCommand(string getString)
	{
		var ss = new StringStack(getString);
		switch (ss.Pop())
		{
			case "help":
				HandleHelpTcpCommand(ss);
				return;
			case "proghelp":
				HandleProgHelpTcpCommand(ss);
				return;
			case "adminhelp":
				HandleHelpTcpCommand(ss, true);
				return;
			case "authsuccess":
				Console.WriteLine("DiscordConnection successfully authenticated.");
				return;
			case "authfailure":
				Console.WriteLine("DiscordConnection did not successfully authenticate.");
				return;
			case "who":
				Console.WriteLine("Discord asked for WHO.");
				HandleWhoTcpCommand(ss);
				return;
			case "where":
				Console.WriteLine("Discord asked for WHERE.");
				HandleWhereTcpCommand(ss);
				return;
			case "stats":
				Console.WriteLine("Discord asked for STATS.");
				HandleStatsTcpCommand(ss);
				return;
			case "showchargen":
				HandleShowChargenTcpCommand(ss);
				return;
			case "approvechargen":
				HandleApproveChargenTcpCommand(ss);
				return;
			case "rejectchargen":
				HandleRejectChargenTcpCommand(ss);
				return;
			case "broadcast":
				HandleBroadcastTcpCommand(ss);
				return;
			case "send":
				HandleSendTcpCommand(ss);
				return;
			case "register":
				HandleRegisterTcpCommand(ss);
				return;
			case "shutdown":
				HandleShutdownTcpCommand(ss);
				return;
		}
	}

	private void HandleShutdownTcpCommand(StringStack ss)
	{
		var requesterid = long.Parse(ss.Pop(), CultureInfo.InvariantCulture.NumberFormat);
		var reboot = bool.Parse(ss.Pop());
		IAccount account;
		using (new FMDB())
		{
			account = Gameworld.TryAccount(FMDB.Context.Accounts.Find(requesterid));
			if (account.Authority.Level < PermissionLevel.SeniorAdmin)
			{
				return;
			}

			var time = DateTime.UtcNow;
			foreach (var dbchar in Gameworld.Characters.SelectNotNull(ch => FMDB.Context.Characters.Find(ch.Id)))
			{
				dbchar.LastLogoutTime = time;
			}

			FMDB.Context.SaveChanges();
		}

		Gameworld.Characters.ForEach(x => x.EffectsChanged = true);
		Gameworld.SaveManager.Flush();
		Gameworld.DiscordConnection.NotifyShutdown(account.Name.Proper());

		if (reboot)
		{
			Console.WriteLine($"{account.Name.Proper()} excecuted a shutdown [reboot] command via discord.");
			Gameworld.SystemMessage(string.Format(Gameworld.GetStaticString("GameShutdownMessageReboot"),
				Gameworld.Name.Proper().ColourName()));
		}
		else
		{
			Console.WriteLine($"{account.Name.Proper()} excecuted a shutdown command via discord.");
			Gameworld.SystemMessage(string.Format(Gameworld.GetStaticString("GameShutdownMessage"),
				Gameworld.Name.Proper().ColourName()));
			using (var fs = File.Create("STOP-REBOOTING"))
			{
			}
		}

		Gameworld.HaltGameLoop();
	}

	private void HandleRejectChargenTcpCommand(StringStack ss)
	{
		var request = ulong.Parse(ss.Pop());
		var which = long.Parse(ss.Pop(), CultureInfo.InvariantCulture.NumberFormat);
		var requesterid = long.Parse(ss.Pop(), CultureInfo.InvariantCulture.NumberFormat);
		var message = ss.RemainingArgument;

		Chargen chargen;
		IAccount account;
		using (new FMDB())
		{
			var dbitem = FMDB.Context.Chargens.Find(which);
			if (dbitem == null)
			{
				SendClientMessage($"request {request} nosuchchargen {which}");
				return;
			}

			chargen = new Chargen(dbitem, Gameworld, dbitem.Account);
			account = Gameworld.TryAccount(FMDB.Context.Accounts.Find(requesterid));
		}

		if (chargen.State != ChargenState.Submitted)
		{
			SendClientMessage(
				$"request {request} chargenapprovalerror {which} That chargen is not currently in the submitted state.");
			return;
		}

		chargen.RejectApplication(null, account, message, null);
	}

	private void HandleApproveChargenTcpCommand(StringStack ss)
	{
		var request = ulong.Parse(ss.Pop());
		var which = long.Parse(ss.Pop(), CultureInfo.InvariantCulture.NumberFormat);
		var requesterid = long.Parse(ss.Pop(), CultureInfo.InvariantCulture.NumberFormat);
		var message = ss.RemainingArgument;

		Chargen chargen;
		IAccount account;
		using (new FMDB())
		{
			var dbitem = FMDB.Context.Chargens.Find(which);
			if (dbitem == null)
			{
				SendClientMessage($"request {request} nosuchchargen {which}");
				return;
			}

			chargen = new Chargen(dbitem, Gameworld, dbitem.Account);
			account = Gameworld.TryAccount(FMDB.Context.Accounts.Find(requesterid));
		}

		if (chargen.State != ChargenState.Submitted)
		{
			SendClientMessage(
				$"request {request} chargenapprovalerror {which} That chargen is not currently in the submitted state.");
			return;
		}

		if (chargen.MinimumApprovalAuthority > account.Authority.Level)
		{
			SendClientMessage(
				$"request {request} chargenapprovalerror {which} That application requires a minimum authority level of {chargen.MinimumApprovalAuthority.Describe()}.");
			return;
		}

		if (account.Authority.Level < PermissionLevel.HighAdmin &&
		    chargen.SelectedRoles.Any(
			    x =>
				    x.RequiredApprovers.Any() &&
				    x.RequiredApprovers.All(
					    y => !y.Equals(account.Name, StringComparison.InvariantCultureIgnoreCase))))
		{
			var blockingRole =
				chargen.SelectedRoles.First(
					x =>
						x.RequiredApprovers.Any() &&
						x.RequiredApprovers.All(
							y => !y.Equals(account.Name, StringComparison.InvariantCultureIgnoreCase)));
			SendClientMessage(
				$"request {request} chargenapprovalerror {which} The **{blockingRole.Name.TitleCase()}** role requires specific people to approve it, and you are not amongst them.");
			return;
		}

		if (
			chargen.ApplicationCosts.Any(x => chargen.Account.AccountResources[x.Key] < x.Value))
		{
			SendClientMessage(
				$"request {request} chargenapprovalerror {which} Account {chargen.Account.Name.Proper()} no longer has sufficient {chargen.ApplicationCosts.Where(x => chargen.Account.AccountResources[x.Key] < x.Value).Select(x => x.Key.PluralName).ListToString()} to pay for that application.");
			return;
		}

		SendClientMessage($"request {request} success");
		chargen.ApproveApplication(null, account, message, null);
	}

	private void HandleShowChargenTcpCommand(StringStack ss)
	{
		var request = ulong.Parse(ss.Pop());
		var requesterid = long.Parse(ss.Pop(), CultureInfo.InvariantCulture.NumberFormat);
		var id = long.Parse(ss.Pop(), CultureInfo.InvariantCulture.NumberFormat);

		Chargen chargen;
		IAccount account;
		using (new FMDB())
		{
			var dbitem = FMDB.Context.Chargens.Find(id);
			if (dbitem == null)
			{
				SendClientMessage($"request {request} nosuchchargen {id}");
				return;
			}

			chargen = new Chargen(dbitem, Gameworld, dbitem.Account);
			account = Gameworld.TryAccount(FMDB.Context.Accounts.Find(requesterid));
		}

		SendClientMessage(
			$"request {request} chargeninfo {id} {chargen.DisplayForReviewForDiscord(account, account.Authority.Level).RawText()}");
	}

	private void HandleRegisterTcpCommand(StringStack ss)
	{
		var request = ulong.Parse(ss.Pop());
		var discorduserid = ulong.Parse(ss.Pop());
		var discordusername = ss.PopSpeech();
		var accountname = ss.Pop();

		var character = Gameworld.Characters.FirstOrDefault(x => x.Account.Name.EqualTo(accountname));
		if (character == null)
		{
			SendClientMessage($"request {request} notfound");
			return;
		}

		character.OutputHandler.Send(
			$"You have received a request to link your MUD account to discord user {discordusername.Colour(Telnet.Cyan)}.\n{Accept.StandardAcceptPhrasing}");
		var account = character.Account;
		character.AddEffect(new Accept(character, new GenericProposal
		{
			AcceptAction = text =>
			{
				character.OutputHandler.Send(
					$"You accept the proposal to link your MUD account to discord user {discordusername.Colour(Telnet.Cyan)}");
				try
				{
					SendClientMessage($"request {request} registeracknowledge {account.Name} {account.Id}");
				}
				catch (SocketException)
				{
					CloseTcpConnection();
				}
			},
			RejectAction = text =>
			{
				character.OutputHandler.Send(
					$"You decide not to link your MUD account to discord user {discordusername.Colour(Telnet.Cyan)}");
				SendClientMessage($"request {request} rejected");
			},
			ExpireAction = () =>
			{
				character.OutputHandler.Send(
					$"You decide not to link your MUD account to discord user {discordusername.Colour(Telnet.Cyan)}");
				SendClientMessage($"request {request} timeout");
			},
			DescriptionString = "Linking your account to a discord user"
		}), TimeSpan.FromSeconds(120));
	}

	private void HandleSendTcpCommand(StringStack ss)
	{
		var request = ulong.Parse(ss.Pop());
		var from = ss.Pop();
		var to = ss.Pop();
		var message = ss.RemainingArgument;
		var user = Gameworld.Connections.FirstOrDefault(x =>
			x.State == ConnectionState.Open && x.ControlPuppet?.Account?.Name.EqualTo(to) == true);
		if (user == null)
		{
			SendClientMessage($"request {request} sendfailed {to}");
			return;
		}

		user.ControlPuppet.OutputHandler?.Send(
			$"{$"[From {from}]".Colour(Telnet.Green)} {message.ParseSpecialCharacters().SubstituteANSIColour().ProperSentences()}");
		SendClientMessage($"request {request} sendacknowledge {from} {to} {message}");
	}

	private void HandleBroadcastTcpCommand(StringStack ss)
	{
		if (ss.IsFinished)
		{
			return;
		}

		Gameworld.SystemMessage(ss.RemainingArgument.ParseSpecialCharacters().SubstituteANSIColour().ProperSentences());
		HandleBroadcast(ss.RemainingArgument.ParseSpecialCharacters().SubstituteANSIColour().ProperSentences());
	}

	private void HandleProgHelpTcpCommand(StringStack ss)
	{
		var response = ulong.Parse(ss.Pop());
		var which = ss.PopForSwitch();
		switch (which)
		{
			case "collections":
			case "collection":
			case "statements":
			case "statement":
			case "functioncategories":
			case "functions":
			case "function":
			case "types":
			case "type":
				break;
			default:
				SendClientMessage(
					$@"request {response} You can use the following prog help options:

	proghelp types - shows all variable types
	proghelp type <which> - shows help for a specific variable type
	proghelp collections - show all collection functions
	proghelp collection <which> - show help for a specific collection function
	proghelp statements - show all statements
	proghelp statement <which> - show help for a specific statement
	proghelp functioncategories - show all function categories
	proghelp functions <category> - show all prog functions for a specific category
	proghelp function <which> - show help for a specific prog function");
				return;
		}

		switch (which)
		{
			case "collections":
				SendClientMessage($"request {response} {ProgModule.GetTextProgHelpCollections(100, true, false)}");
				return;
			case "collection":
				if (ss.IsFinished)
				{
					SendClientMessage($"request {response} Which collection extension function would you like to see help for? See PROGHELP COLLECTIONS for a list.");
					return;
				}

				var info = CollectionExtensionFunction.FunctionCompilerInformations.FirstOrDefault(x =>
					x.FunctionName.EqualTo(ss.SafeRemainingArgument));
				if (info is null)
				{
					SendClientMessage($"request {response} There is no such collection extension type. See PROGHELP COLLECTIONS for a list.");
					return;
				}
				SendClientMessage($"request {response} {ProgModule.GetTextProgHelpCollection(info, 100, false)}");
				return;
			case "statements":
				SendClientMessage($"request {response} {ProgModule.GetTextProgHelpStatements(100, true, false)}");
				return;
			case "statement":
				if (ss.IsFinished)
				{
					SendClientMessage($"request {response} Which statement would you like to see help for? See PROGHELP STATEMENTS for a list.");
					return;
				}

				if (!FutureProg.FutureProg.StatementHelpTexts.TryGetValue(ss.SafeRemainingArgument, out var value))
				{
					SendClientMessage($"request {response} There is no such statement. See PROGHELP STATEMENTS for a list.");
					return;
				}
				SendClientMessage($"request {response} {ProgModule.GetTextProgHelpStatement(value, ss.SafeRemainingArgument, 100, false)}");
				return;
			case "functioncategories":
				SendClientMessage($"request {response} {FutureProg.FutureProg.GetFunctionCompilerInformations().Select(x => x.Category).Distinct().ListToCommaSeparatedValues(",")}");
				return;
			case "functions":
				var category = ss.SafeRemainingArgument;
				if (string.IsNullOrEmpty(category))
				{
					SendClientMessage($"request {response} You must specify a function category.");
					return;
				}

				var infos = FutureProg.FutureProg.GetFunctionCompilerInformations().Where(x =>
					x.Category.EqualTo(category) ||
					x.Category.StartsWith(category, StringComparison.InvariantCultureIgnoreCase)).ToList();
				if (!infos.Any())
				{
					SendClientMessage($"request {response} There are no prog functions matching that category.");
					return;
				}
				SendClientMessage($"request {response} {ProgModule.GetTextProgHelpFunctions(infos, 100, true, false)}");
				return;
			case "function":
				if (ss.IsFinished)
				{
					SendClientMessage($"request {response} Which function do you want to see help for?");
					return;
				}

				var whichFunction = ss.SafeRemainingArgument;
				var functions = FutureProg.FutureProg.GetFunctionCompilerInformations()
				                          .Where(x => x.FunctionName.EqualTo(whichFunction))
				                          .ToList();

				if (!functions.Any())
				{
					SendClientMessage($"request {response} There are no such functions. Please see PROGHELP FUNCTIONS <CATEGORY> for a list.");
					return;
				}
				SendClientMessage($"request {response} {ProgModule.GetTextProgHelpFunction(functions, 100, 80, true, false)}");
				return;
			case "types":
				SendClientMessage($"request {response} {ProgModule.GetTextProgHelpTypes(false)}");
				return;
			case "type":
				if (ss.IsFinished)
				{
					SendClientMessage($"request {response} What type do you want to see property help for?");
					return;
				}

				var whichType = ss.SafeRemainingArgument;
				var type = FutureProg.FutureProg.GetTypeByName(whichType);
				if (type == FutureProgVariableTypes.Error)
				{
					SendClientMessage($"request {response} There is no such type.");
					return;
				}

				var text = ProgModule.GetProgTypeHelpText(type, 100, true, false);
				if (text is null)
				{
					SendClientMessage($"request {response} The type {type.Describe()} does not have any help.");
					return;
				}
				SendClientMessage($"request {response} {text}");
				return;
		}
	}

	private void HandleHelpTcpCommand(StringStack ss, bool admin = false)
	{
		var response = ulong.Parse(ss.Pop());
		if (ss.IsFinished)
		{
			SendClientMessage(
				$"request {response} You can either use 'help on <category>' to see all helpfiles in a category or 'help <helpfile>' to see a specific helpfile");
			return;
		}

		var commandHelps = Gameworld.RetrieveAppropriateCommandTree(null).Commands.CommandHelpInfos.ToList();
		var sb = new StringBuilder();
		if (ss.Peek().EqualTo("on"))
		{
			if (ss.IsFinished)
			{
				SendClientMessage($"request {response} A category must be specified if the ON keyword is used.");
				return;
			}

			ss.Pop();
			var category = ss.RemainingArgument;
			var helpFileInfos = new List<(string Name, string Subcategory, string Tagline, string Keywords)>();
			var helpfiles = Gameworld.Helpfiles.Where(x =>
				                         x.Category.StartsWith(category, StringComparison.InvariantCultureIgnoreCase) &&
				                         x.CanView(null))
			                         .ToList();
			helpFileInfos.AddRange(helpfiles.Select(x => (x.Name.TitleCase(), x.Subcategory.TitleCase(),
				x.TagLine.Proper(), x.Keywords.ListToString(separator: " ", conjunction: ""))));
			if ("commands".StartsWith(category, StringComparison.InvariantCultureIgnoreCase))
			{
				helpFileInfos.AddRange(commandHelps.Select(x => (x.HelpName.TitleCase(), "Built-in",
					$"Built in help for the {x.HelpName.ToUpperInvariant()} command", x.HelpName.ToLowerInvariant())));
			}

			if (!helpFileInfos.Any())
			{
				SendClientMessage($"request {response} There are no helpfiles in that category.");
				return;
			}

			sb.AppendLine(
				$"There are the following help files in the {helpfiles.FirstOrDefault()?.Category.TitleCase() ?? "Commands"} category:");
			sb.Append(StringUtilities.GetTextTable(
				helpFileInfos.Select(
					x =>
						new[]
						{
							x.Name,
							x.Subcategory,
							x.Tagline,
							x.Keywords
						}),
				new[] { "Help File", "Subcategory", "Synopsis", "Keywords" }, 120, false, null, 2, true));
			SendClientMessage($"request {response} {sb.ToString()}");
			return;
		}

		var desiredFile = ss.PopSpeech();
		var allhelp = Gameworld.Helpfiles.Where(x => x.CanView(null)).ToList();
		var help = allhelp.FirstOrDefault(x => x.Name.EqualTo(desiredFile)) ??
		           allhelp.FirstOrDefault(x =>
			           x.Name.StartsWith(desiredFile, StringComparison.InvariantCultureIgnoreCase));
		if (help != null)
		{
			sb.AppendLine(
				$"Helpfile: {help.Name.TitleCase()} Category: {help.Category.TitleCase()} Subcategory: {help.Subcategory.TitleCase()}");
			sb.AppendLine($"Keywords: {help.Keywords.ListToString(separator: " ", conjunction: "")}");
			sb.AppendLine($"Tagline: {help.TagLine.ProperSentences()}");
			sb.AppendLine();
			sb.AppendLine(help.PublicText.SubstituteANSIColour().Wrap(80).RawText());
			sb.AppendLine();
			sb.AppendLine($"Last edited by {help.LastEditedBy.Proper()} on {help.LastEditedDate.ToLongDateString()}");
			_client.Client.Send(Encoding.Unicode.GetBytes($"request {response} {sb.ToString()}"));
			return;
		}

		var commandhelp = commandHelps.FirstOrDefault(x => x.HelpName.EqualTo(desiredFile)) ??
		                  commandHelps.FirstOrDefault(x =>
			                  x.HelpName.StartsWith(desiredFile, StringComparison.InvariantCultureIgnoreCase));
		if (commandhelp == null)
		{
			SendClientMessage(
				$"request {response} There is no helpfile that could be located by the name {desiredFile.ToLowerInvariant()}.");
			return;
		}

		sb.AppendLine($"Helpfile: {commandhelp.HelpName.TitleCase()} Category: Commands Subcategory: Built-in");
		sb.AppendLine($"Keywords: {commandhelp.HelpName}");
		sb.AppendLine($"Tagline: Built in help for the {commandhelp.HelpName.ToUpperInvariant()} command");
		sb.AppendLine();
		sb.AppendLine((admin ? commandhelp.AdminHelp ?? commandhelp.DefaultHelp : commandhelp.DefaultHelp)
		              .SubstituteANSIColour().Wrap(80).RawText());
		sb.AppendLine();
		sb.AppendLine($"Automatically generated by the MUD");
		SendClientMessage($"request {response} {sb.ToString()}");
	}

	private void HandleStatsTcpCommand(StringStack ss)
	{
		var request = ulong.Parse(ss.Pop());
		var sb = new StringBuilder();
		sb.AppendLine($"The following statistics are available regarding {Gameworld.Name.Proper()}:");
		sb.AppendLine();
		var version = Assembly.GetCallingAssembly().GetName().Version;
		var versionString = $"v{version.Major}.{version.Minor}.{version.Build}.{version.Revision.ToString("0000")}";
		sb.AppendLine($"{Gameworld.Name.Proper()} is running on {versionString} of the FutureMUD engine.");
		sb.AppendLine(
			$"The record number of players online at one time was {Gameworld.GameStatistics.RecordOnlinePlayers:N0}, which was achieved on {Gameworld.GameStatistics.RecordOnlinePlayersDateTime}.");
		sb.AppendLine(
			$"The MUD was last booted on {Gameworld.GameStatistics.LastBootTime}, and took {Gameworld.GameStatistics.LastStartupSpan.Describe()}, with a current uptime of {(DateTime.UtcNow - Gameworld.GameStatistics.LastBootTime).Describe()}.");
		using (new FMDB())
		{
			var sinceTime = DateTime.UtcNow.AddDays(-60);
			sb.AppendLine(
				$"There are {FMDB.Context.Accounts.Count():N0} registered accounts, of which {FMDB.Context.Accounts.Count(x => x.LastLoginTime != null && x.LastLoginTime >= sinceTime):N0} have logged on in the last 60 days.");

			var now = DateTime.UtcNow;
			var totalTime =
				FMDB.Context.Characters.Where(x => x.Account != null).Sum(x => (long)x.TotalMinutesPlayed) +
				(long)Gameworld.Characters.Sum(x => (now - x.LoginDateTime).TotalMinutes);
			sb.AppendLine(
				$"Players have spent a total of {TimeSpan.FromMinutes(totalTime).Describe()} playing {Gameworld.Name.Proper()}.");
		}

		sb.AppendLine(
			$"There are a total of {Gameworld.Cells.Count():N0} rooms, {Gameworld.ItemProtos.Select(x => x.Id).Distinct().Count():N0} items and {Gameworld.NpcTemplates.Select(x => x.Id).Distinct().Count():N0} NPCs built.");
		sb.AppendLine(
			$"There are {Gameworld.Items.Count():N0} items and {Gameworld.NPCs.Count():N0} NPCs in the game world."
		);
		sb.AppendLine(
			$"There are {Gameworld.Crafts.Select(x => x.Id).Distinct().Count().ToString("N0")} distinct crafts.");
		SendClientMessage($"request {request} {sb}");
	}

	private void HandleWhereTcpCommand(StringStack ss)
	{
		var request = ulong.Parse(ss.Pop());
		SendClientMessage($"request {request} " + StringUtilities.GetTextTable(
			from character in Gameworld.Characters.Where(x => !x.State.HasFlag(CharacterState.Dead))
			                           .OrderBy(x => x.Location.Id).ToList()
			select new[]
			{
				character.Id.ToString("N0"),
				character.PersonalName.GetName(NameStyle.FullWithNickname).TitleCase(),
				character.Location.Id.ToString("N0"),
				character.Location.HowSeen(character, colour: false),
				character.Account.Name.TitleCase()
			},
			new[] { "ID", "Name", "Room", "Room Desc", "Account" },
			150,
			false,
			truncatableColumnIndex: 3,
			unicodeTable: true));
	}

	private void HandleWhoTcpCommand(StringStack ss)
	{
		var request = ulong.Parse(ss.Pop());
		var count =
			Gameworld.Characters.Count(
				x => !x.IsAdministrator() && !x.IsGuest) +
			Gameworld.NPCs.Count(x => x.EffectsOfType<ICountForWho>().Any());
		var guestCount = Gameworld.Characters.Count(x => x.IsGuest);
		var availableAdmins = Gameworld.Characters.Where(x => x.AffectedBy<IAdminAvailableEffect>()).ToList();
		var extraText = new StringBuilder();
		foreach (var clan in Gameworld.Clans.Where(x => x.ShowClanMembersInWho))
		{
			var clanMembers =
				Gameworld.Characters.Where(x => x.ClanMemberships.Any(y => y.Clan == clan))
				         .Concat(Gameworld.NPCs.Where(x => x.EffectsOfType<ICountForWho>().Any())).ToList();
			extraText.AppendFormat("\nThere are {0:N0} {1} of {2} online.", clanMembers.Count,
				clanMembers.Count == 1 ? "member" : "members", clan.FullName);
		}

		if (availableAdmins.Any())
		{
			extraText.Append(
				$"\n\nThe following members of staff are available:\n{availableAdmins.Select(x => "\t" + x.Account.Name.TitleCase()).ListToString(separator: "\n", twoItemJoiner: "\n", conjunction: "")}");
		}

		var text = $"request {request} " + string.Format(Gameworld.GetStaticString(count == 0
				? "WhoTextNoneOnline"
				: count == 1
					? "WhoTextOneOnline"
					: "WhoText"),
			count,
			Gameworld.GameStatistics.RecordOnlinePlayers,
			Gameworld.GameStatistics.RecordOnlinePlayersDateTime,
			extraText.Length > 0
				? extraText.ToString()
				: "",
			guestCount > 0
				? $"\nThere are {guestCount} guest{(guestCount == 1 ? "" : "s")} in the guest lounge."
				: "") + (char)1;
		SendClientMessage(text);
	}

	public void HandleBroadcast(string message)
	{
		try
		{
			SendClientMessage($"broadcast {message.RawText()}");
		}
		catch (SocketException)
		{
			CloseTcpConnection();
		}
	}

	public void Dispose()
	{
		_client?.Dispose();
	}
}