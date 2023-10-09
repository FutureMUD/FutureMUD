using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Dapper;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Editor;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.NPC;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Intervals;
using MudSharp.RPG.Merits;
using MudSharp.CharacterCreation.Roles;
using MudSharp.Construction.Boundary;
using MudSharp.PerceptionEngine.Handlers;
using MudSharp.Form.Material;
using MudSharp.Form.Colour;
using MudSharp.Body.Disfigurements;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character.Name;
using MudSharp.Economy.Currency;
using MudSharp.Models;
using TraitExpression = MudSharp.Body.Traits.TraitExpression;
using System.Xml.Linq;
using MudSharp.Form.Characteristics;
using MudSharp.Framework.Units;
using MudSharp.RPG.ScriptedEvents;
using MudSharp.Communication.Language;

namespace MudSharp.Commands.Modules;

internal class StorytellerModule : Module<ICharacter>
{
	private static readonly Regex _echoOnFailureRegex = new("^[.+]\\s*\\([.+]\\)$");

	private StorytellerModule()
		: base("Storyteller")
	{
		IsNecessary = true;
	}

	public static StorytellerModule Instance { get; } = new();

	[PlayerCommand("Spy", "spy")]
	[HelpInfo("spy",
		"This command allows you to toggle spying on a location, which means you'll see all output as if you were there.\r\n\r\nSyntax:\r\n\tSPY LIST - shows you where you're spying\r\n\tSPY HERE - toggles the current location's spy status\r\n\tSPY <ID> - toggles the specified cell ID's spy status",
		AutoHelp.HelpArgOrNoArg)]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void Spy(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.Peek().EqualTo("list"))
		{
			var effect = actor.EffectsOfType<AdminSpyMaster>().FirstOrDefault();
			if (effect == null)
			{
				actor.Send("You aren't currently spying on any locations.");
				return;
			}

			var sb = new StringBuilder();
			sb.AppendLine("You are currently spying on the following locations:");
			foreach (var location in effect.SpiedCells)
			{
				sb.AppendLine(
					$"\t{location.HowSeen(actor)} ({location.Id}) in {location.Room.Zone.Name.Colour(Telnet.BoldWhite)}");
			}

			actor.Send(sb.ToString());
			return;
		}

		if (ss.Peek().EqualToAny("clear", "none", "off"))
		{
			var effect = actor.EffectsOfType<AdminSpyMaster>().FirstOrDefault();
			if (effect == null)
			{
				actor.Send("You aren't currently spying upon any locations.");
				return;
			}

			foreach (var cell in effect.SpiedCells.ToList())
			{
				effect.RemoveSpiedCell(cell);
			}

			actor.Send("All your spied upon locations have been cleared.");
			return;
		}

		var targetCell = ss.Peek().Equals("here")
			? actor.Location
			: RoomBuilderModule.LookupCell(actor.Gameworld, ss.PopSpeech());

		if (targetCell == null)
		{
			actor.Send("There is no such cell to spy on.");
			return;
		}

		var smeffect = actor.EffectsOfType<AdminSpyMaster>().FirstOrDefault();
		if (smeffect == null)
		{
			smeffect = new AdminSpyMaster(actor);
			actor.AddEffect(smeffect);
		}

		if (smeffect.SpiedCells.Contains(targetCell))
		{
			smeffect.RemoveSpiedCell(targetCell);
			actor.Send($"You will no longer spy on {targetCell.HowSeen(actor)} ({targetCell.Id})");
			return;
		}

		smeffect.AddSpiedCell(targetCell);
		actor.Send($"You are now spying on {targetCell.HowSeen(actor)} ({targetCell.Id})");
	}

	[PlayerCommand("NewPlayer", "newplayer")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[HelpInfo("newplayer",
		@"This command allows you to add the ""New Player"" tag that accompanies the in-room description of new players for the first 48 hours of their play time. You can use this to add it to someone who accidentally removes it and requests that it be added back, and you can also use it on NPCs to 'fake' being a new player.

The syntax to use this command is #3newplayer <target>#0", AutoHelp.HelpArgOrNoArg)]
	protected static void NewPlayer(ICharacter actor, string input)
	{
		var target = actor.TargetActor(input.RemoveFirstWord());
		if (target == null)
		{
			actor.Send("You don't see anyone like that.");
			return;
		}

		if (target.EffectsOfType<NewPlayer>().Any())
		{
			actor.OutputHandler.Send($"{target.HowSeen(actor, true)} is already tagged as a new player.");
			return;
		}

		target.AddEffect(new NewPlayer(target), Effects.Concrete.NewPlayer.NewPlayerEffectLength);
		actor.OutputHandler.Send($"{target.HowSeen(actor, true)} is now tagged as a new player.");
	}

	[PlayerCommand("FullAudit", "fullaudit")]
	[CommandPermission(PermissionLevel.Admin)]
	protected static void FullAudit(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.Send("Which resource do you want to audit?");
			return;
		}

		var name = ss.SafeRemainingArgument;
		var targetResource = actor.Gameworld.ChargenResources.GetByName(name) ??
		                     actor.Gameworld.ChargenResources.FirstOrDefault(x => x.Alias.EqualTo(name));
		if (targetResource == null)
		{
			actor.Send("There is no such resource to audit.");
			return;
		}

		using (new FMDB())
		{
			var accounts = FMDB.Connection.Query<(string Account, int Amount, DateTime LastAward)>(
				$"select a.Name, r.Amount, r.LastAwardDate from Accounts a left outer join Accounts_ChargenResources r on r.accountid = a.id and chargenresourceid = {targetResource.Id} order by Amount desc, LastAwardDate desc;");
			actor.Send(StringUtilities.GetTextTable(
				from account in accounts
				select new[]
				{
					account.Account,
					account.Amount.ToString("N0", actor),
					account.LastAward.GetLocalDateString(actor)
				},
				new[] { "Account", "Amount", "Last Award" },
				actor.LineFormatLength,
				colour: Telnet.Green,
				unicodeTable: actor.Account.UseUnicode
			));
		}
	}

	[PlayerCommand("Audit", "audit")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void Audit(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.Send("Which resource do you want to audit?");
			return;
		}

		var name = ss.SafeRemainingArgument;
		var targetResource = actor.Gameworld.ChargenResources.GetByName(name) ??
		                     actor.Gameworld.ChargenResources.FirstOrDefault(x => x.Alias.EqualTo(name));
		if (targetResource == null)
		{
			actor.Send("There is no such resource to audit.");
			return;
		}

		actor.Send(StringUtilities.GetTextTable(
			from ch in actor.Gameworld.Characters
			select new[]
			{
				ch.Account.Name,
				ch.PersonalName.GetName(NameStyle.FullName),
				ch.HowSeen(ch, flags: PerceiveIgnoreFlags.IgnoreSelf),
				ch.Account.AccountResources.ValueOrDefault(targetResource, 0).ToString(),
				ch.Account.AccountResourcesLastAwarded.ValueOrDefault(targetResource, null)
				  ?.GetLocalDateString(actor, true)
			},
			new[] { "Account", "Character", "Desc", "Resource", "Last Award" },
			actor.LineFormatLength,
			colour: Telnet.Green,
			unicodeTable: actor.Account.UseUnicode
		));
	}

	[PlayerCommand("ImmCommands", "immcommands")]
	protected static void ImmCommands(ICharacter actor, string input)
	{
		actor.OutputHandler.Send(
			actor.CommandTree.Commands.ReportCommands(PermissionLevel.JuniorAdmin, actor.PermissionLevel, actor)
			     .ArrangeStringsOntoLines());
	}

	private static void AwardEditorPost(string text, IOutputHandler handler, params object[] arguments)
	{
		var award = (bool)arguments[0];
		using (new FMDB())
		{
			var dbaccount = FMDB.Context.Accounts.Find((long)arguments[3]);
			if (dbaccount == null)
			{
				throw new ApplicationException("Account was not found in the database in AwardEditorPost.");
			}

			var type = (IChargenResource)arguments[5];
			var dbaccountresource =
				dbaccount.AccountsChargenResources.FirstOrDefault(
					x => x.ChargenResourceId == type.Id);
			if (dbaccountresource == null)
			{
				dbaccountresource = new AccountsChargenResources
				{
					Amount = 0,
					ChargenResourceId = type.Id
				};
				dbaccount.AccountsChargenResources.Add(dbaccountresource);
			}

			dbaccountresource.LastAwardDate = DateTime.UtcNow;
			var amount = (int)arguments[2];
			dbaccountresource.Amount += ((bool)arguments[0] ? 1 : -1) * amount;

			var character = (ICharacter)arguments[1];
			var account = character.Gameworld.Accounts.FirstOrDefault(x => x.Id == dbaccount.Id);
			if (account != null)
			{
				account.AccountResources[type] = dbaccountresource.Amount;
				account.AccountResourcesLastAwarded[type] = dbaccountresource.LastAwardDate;
				if (account.ControllingContext != null &&
				    !string.IsNullOrEmpty(award
					    ? type.TextDisplayedToPlayerOnAward
					    : type.TextDisplayedToPlayerOnDeduct))
				{
					account.ControllingContext.Send("{0} {1}", "[System Message]".Colour(Telnet.Green),
						award ? type.TextDisplayedToPlayerOnAward : type.TextDisplayedToPlayerOnDeduct);
				}
			}

			var newNote = new AccountNote
			{
				AuthorId = character.Account.Id,
				Subject =
					$"{(award ? "Award" : "Deduction")} of {amount} {(amount == 1 ? type.Name : type.PluralName).Proper()} by {character.Account.Name.Proper()}.",
				Text = text,
				TimeStamp = dbaccountresource.LastAwardDate
			};
			dbaccount.AccountNotesAccount.Add(newNote);
			character.Gameworld.SystemMessage(
				new EmoteOutput(
					new Emote(
						$"You|{character.Account.Name.Proper()} have|has {(award ? "awarded" : "deducted")} {(amount == 1 ? type.Name : type.PluralName)} {amount} {(award ? "to" : "from")} account {dbaccount.Name.Proper()}.",
						character)), true);

			FMDB.Context.SaveChanges();
		}
	}

	private static void AwardEditorCancel(IOutputHandler handler, params object[] arguments)
	{
		var award = (bool)arguments[0];
		handler.Send(
			$"You decide not to {(award ? "award" : "deduct")} {((IChargenResource)arguments[5]).PluralName} {(award ? "to" : "from")} {(string)arguments[4]}.");
	}

	[PlayerCommand("Award", "award", "deduct")]
	[DisplayOptions(CommandDisplayOptions.DisplayCommandWords)]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void Award(ICharacter character, string input)
	{
		var ss = new StringStack(input);
		var award = ss.Pop().Equals("award", StringComparison.InvariantCultureIgnoreCase);

		if (ss.IsFinished)
		{
			character.OutputHandler.Send(
				$"{(award ? "To" : "From")} whose account do you wish to {(award ? "award" : "deduct")}?");
			return;
		}

		var accountText = ss.Pop();
		var typeText = ss.PopSpeech();
		var amountText = ss.Pop();

		if (string.IsNullOrEmpty(typeText))
		{
			character.OutputHandler.Send(
				$"What type of resource do you wish to {(award ? "award to" : "deduct from")} them?");
			return;
		}

		var type =
			character.Gameworld.ChargenResources.FirstOrDefault(
				x =>
					x.Alias.Equals(typeText, StringComparison.InvariantCultureIgnoreCase) ||
					x.Name.Equals(typeText, StringComparison.InvariantCultureIgnoreCase));

		if (type == null)
		{
			character.OutputHandler.Send(
				$"That is not a valid type of resource to {(award ? "award to" : "deduct from")} an account.");
			return;
		}

		if (!character.IsAdministrator(type.PermissionLevelRequiredToAward))
		{
			character.OutputHandler.Send($"You are not allowed to award {type.PluralName.TitleCase()}.");
			return;
		}

		int amount;
		if (string.IsNullOrEmpty(amountText))
		{
			amount = 1;
		}
		else
		{
			if (!int.TryParse(amountText, out amount))
			{
				character.OutputHandler.Send(
					$"You must enter a valid number of {type.PluralName.TitleCase()} to {(award ? "award to" : "deduct from")} them, or do not include any additional text to use the default amount.");
				return;
			}
		}

		if (award && amount > type.MaximumNumberAwardedPerAward)
		{
			character.Send("It is only possible to award {0} {1} per award.", type.MaximumNumberAwardedPerAward,
				type.MaximumNumberAwardedPerAward == 1 ? type.Name : type.PluralName);
			return;
		}

		using (new FMDB())
		{
			var dbaccount =
				FMDB.Context.Accounts.FirstOrDefault(x => x.Name == accountText);
			if (dbaccount == null)
			{
				character.OutputHandler.Send("That is not a valid account.");
				return;
			}

			var dbaccountresource =
				dbaccount.AccountsChargenResources.FirstOrDefault(x => x.ChargenResourceId == type.Id);
			if (award && dbaccountresource != null &&
			    DateTime.UtcNow - dbaccountresource.LastAwardDate <= type.MinimumTimeBetweenAwards &&
			    !character.IsAdministrator(type.PermissionLevelRequiredToCircumventMinimumTime))
			{
				character.OutputHandler.Send(
					string.Format(character,
						"{0} has been awarded {1} too recently. They will become eligable for further awards on {2:f}.",
						dbaccount.Name.Proper(),
						type.PluralName.TitleCase(),
						(dbaccountresource.LastAwardDate + type.MinimumTimeBetweenAwards).ToUniversalTime()
						.GetLocalDateString(character)
					));
				return;
			}

			character.OutputHandler.Send(
				$"You will now be dropped into an editor where you must enter a comment about your {(award ? "award" : "deduction")} of {(amount == 1 ? type.Name : type.PluralName)} {(award ? "to" : "from")} account {dbaccount.Name.Proper()}.");
			character.EditorMode(AwardEditorPost, AwardEditorCancel, 1.0, null, EditorOptions.None,
				new object[] { award, character, amount, dbaccount.Id, dbaccount.Name, type });
		}
	}

	[PlayerCommand("Force", "force")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[HelpInfo("force",
		"This command allows you to force someone or a group of someones to do a specified command. All admins will see that you used this command. There are a few different versions:\n\tforce <target> <command> - forces an individual target to do a command\n\tforce here <command> - forces all characters (PC and NPC, excluding yourself and other admins)\n\tforce npchere <command> - same as here, but excludes all PCs\n\tforce all <command> - can only be used by senior admins and up - forces ALL PCs and NPCs in the game\n\tforce players <command> - can only be used by senior admins and up - forces all PCs in the game\n\tforce npcs <command> - can only be used by senior admins and up - forces all NPCs in the game ",
		AutoHelp.HelpArgOrNoArg)]
	protected static void Force(ICharacter character, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var targetText = ss.Pop();
		if (string.IsNullOrEmpty(targetText))
		{
			character.OutputHandler.Send("Force who to do what?");
			return;
		}

		if (targetText.Equals("all", StringComparison.InvariantCultureIgnoreCase) &&
		    character.IsAdministrator(PermissionLevel.SeniorAdmin))
		{
			character.OutputHandler.Send(
				$"Are you sure that you want to force EVERYONE (PC and NPC, including yourself) to execute the following command:\n\n\t{ss.RemainingArgument.ColourCommand()}\n\nThis should typically only be done for a VERY important reason. If you wish to proceed, type ACCEPT. Otherwise, DECLINE.");
			character.AddEffect(new Accept(character, new GenericProposal
			{
				AcceptAction = text =>
				{
					character.Gameworld.SystemMessage(new EmoteOutput(new Emote(
							$"@ force|forces everyone in the game to do the command '{ss.RemainingArgument}'",
							character),
						flags: OutputFlags.WizOnly), true);
					foreach (var person in character.Gameworld.Characters
					                                .Where(x => !x.AffectedBy<IIgnoreForceEffect>())
					                                .ToList())
					{
						person.ExecuteCommand(ss.RemainingArgument);
					}
				},
				DescriptionString = "forcing everyone in the game to do something",
				ExpireAction = () =>
				{
					character.OutputHandler.Send(
						"You decide against forcing everyone in the game to do something.");
				},
				Keywords = new List<string> { "force", "game", "all" },
				RejectAction = text =>
				{
					character.OutputHandler.Send(
						"You decide against forcing everyone in the game to do something.");
				}
			}), TimeSpan.FromSeconds(30));

			return;
		}

		if (targetText.Equals("players", StringComparison.InvariantCultureIgnoreCase) &&
		    character.IsAdministrator(PermissionLevel.SeniorAdmin))
		{
			character.OutputHandler.Send(
				$"Are you sure that you want to force EVERY PC IN THE GAME (including yourself) to execute the following command:\n\n\t{ss.RemainingArgument.ColourCommand()}\n\nThis should typically only be done for a VERY important reason. If you wish to proceed, type ACCEPT. Otherwise, DECLINE.");
			character.AddEffect(new Accept(character, new GenericProposal
			{
				AcceptAction = text =>
				{
					character.Gameworld.SystemMessage(new EmoteOutput(new Emote(
							$"@ force|forces all players in the game to do the command '{ss.RemainingArgument}'",
							character),
						flags: OutputFlags.WizOnly), true);
					foreach (var person in character.Gameworld.Actors.Where(x => !x.AffectedBy<IIgnoreForceEffect>())
					                                .ToList())
					{
						person.ExecuteCommand(ss.RemainingArgument);
					}
				},
				DescriptionString = "forcing every PC in the game to do something",
				ExpireAction = () =>
				{
					character.OutputHandler.Send(
						"You decide against forcing every PC in the game to do something.");
				},
				Keywords = new List<string> { "force", "game", "all" },
				RejectAction = text =>
				{
					character.OutputHandler.Send(
						"You decide against forcing every PC in the game to do something.");
				}
			}), TimeSpan.FromSeconds(30));

			return;
		}

		if (targetText.Equals("npcs", StringComparison.InvariantCultureIgnoreCase) &&
		    character.IsAdministrator(PermissionLevel.SeniorAdmin))
		{
			character.OutputHandler.Send(
				$"Are you sure that you want to force EVERY NPC IN THE GAME to execute the following command:\n\n\t{ss.RemainingArgument.ColourCommand()}\n\nThis should typically only be done for a VERY important reason. If you wish to proceed, type ACCEPT. Otherwise, DECLINE.");
			character.AddEffect(new Accept(character, new GenericProposal
			{
				AcceptAction = text =>
				{
					character.Gameworld.SystemMessage(new EmoteOutput(new Emote(
							$"@ force|forces all NPCs in the game to do the command '{ss.RemainingArgument}'",
							character),
						flags: OutputFlags.WizOnly), true);
					foreach (var person in character.Gameworld.NPCs.Where(x => !x.AffectedBy<IIgnoreForceEffect>())
					                                .ToList())
					{
						person.ExecuteCommand(ss.RemainingArgument);
					}
				},
				DescriptionString = "forcing every NPC in the game to do something",
				ExpireAction = () =>
				{
					character.OutputHandler.Send(
						"You decide against forcing every NPC in the game to do something.");
				},
				Keywords = new List<string> { "force", "game", "all" },
				RejectAction = text =>
				{
					character.OutputHandler.Send(
						"You decide against forcing every NPC in the game to do something.");
				}
			}), TimeSpan.FromSeconds(30));

			return;
		}

		if (targetText.Equals("here", StringComparison.InvariantCultureIgnoreCase))
		{
			character.OutputHandler.Handle(new EmoteOutput(new Emote(
					$"@ force|forces everyone in the room to do the command '{ss.RemainingArgument}'", character),
				flags: OutputFlags.WizOnly));
			foreach (var person in character.Location.Characters
			                                .Where(x => !x.AffectedBy<IIgnoreForceEffect>() &&
			                                            !x.AffectedBy<IAdminInvisEffect>())
			                                .ToList())
			{
				person.ExecuteCommand(ss.RemainingArgument);
			}

			return;
		}

		if (targetText.Equals("npcshere", StringComparison.InvariantCultureIgnoreCase))
		{
			character.OutputHandler.Handle(new EmoteOutput(new Emote(
					$"@ force|forces all NPCs in the room to do the command '{ss.RemainingArgument}'", character),
				flags: OutputFlags.WizOnly));
			foreach (var person in character.Location.Characters.Where(x => !x.AffectedBy<IIgnoreForceEffect>())
			                                .Where(x => x is INPC).ToList())
			{
				person.ExecuteCommand(ss.RemainingArgument);
			}

			return;
		}

		var target = character.TargetActor(targetText);
		if (target == null)
		{
			character.OutputHandler.Send("You do not see them here to force.");
			return;
		}

		if (ss.IsFinished)
		{
			character.OutputHandler.Send("What do you want to force " + target.HowSeen(character) + " to do?");
			return;
		}

		if (target.AffectedBy<IIgnoreForceEffect>())
		{
			character.OutputHandler.Send(
				$"{target.HowSeen(character, true)} is affected by a NOFORCE effect, so they will not respond to FORCE.");
			return;
		}

		character.OutputHandler.Handle(new EmoteOutput(new Emote(
				$"@ force|forces $0 to do the command '{ss.RemainingArgument}'", character, target),
			flags: OutputFlags.WizOnly));
		target.ExecuteCommand(ss.RemainingArgument);
	}

	[PlayerCommand("As", "as")]
	[CommandPermission(PermissionLevel.Admin)]
	protected static void As(ICharacter character, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var targetText = ss.Pop();
		if (string.IsNullOrEmpty(targetText))
		{
			character.OutputHandler.Send("Force who to do what?");
			return;
		}

		var target = character.TargetActor(targetText);
		if (target == null)
		{
			character.OutputHandler.Send("You do not see them here to force.");
			return;
		}

		if (ss.IsFinished)
		{
			character.OutputHandler.Send("What do you want to force " + target.HowSeen(character) + " to do?");
			return;
		}

		var oldController = target.Controller;
		character.OutputHandler.Handle(new EmoteOutput(new Emote(
				$"@ force|forces $0 to do the command '{ss.RemainingArgument}'", character, target),
			flags: OutputFlags.WizOnly));
		target.SilentAssumeControl(character.Controller);
		target.ExecuteCommand(ss.RemainingArgument);
		target.SilentAssumeControl(oldController);
		character.SilentAssumeControl(character.Controller);
	}

	[PlayerCommand("LoadCurrency", "loadcurrency")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void LoadCurrency(ICharacter character, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var currency = character.Currency;
		var coins = new Dictionary<ICoin, int>();
		IGameItem newItem = null;
		if (ss.Peek().ToLowerInvariant() == "coins")
		{
			ss.Pop();
			while (true)
			{
				var samount = ss.Pop();
				if (string.IsNullOrEmpty(samount))
				{
					character.OutputHandler.Send("You must enter the specific coins which you want to load.");
					return;
				}

				if (!int.TryParse(samount, out var amount))
				{
					character.OutputHandler.Send("You must enter a whole number of coins of each type to load.");
					return;
				}

				var scoin = ss.PopSpeech();
				if (string.IsNullOrEmpty(scoin))
				{
					character.OutputHandler.Send("Which coin do you want to load " + amount + " of?");
					return;
				}

				var coin =
					currency.Coins.FirstOrDefault(
						x =>
							x.Name.StartsWith(scoin, StringComparison.InvariantCultureIgnoreCase) ||
							x.Name.Replace(x.PluralWord, x.PluralWord.Pluralise())
							 .StartsWith(scoin, StringComparison.InvariantCultureIgnoreCase));
				if (coin == null)
				{
					character.OutputHandler.Send("There is no such coin as \"" + scoin + "\" for this currency.");
					return;
				}

				if (coins.ContainsKey(coin))
				{
					character.OutputHandler.Send("You cannot specify the same coin twice.");
					return;
				}

				coins.Add(coin, amount);
				if (ss.IsFinished)
				{
					break;
				}
			}
		}
		else
		{
			var strAmount = ss.SafeRemainingArgument;
			if (string.IsNullOrEmpty(strAmount))
			{
				character.OutputHandler.Send("What amount of currency do you wish to load?");
				return;
			}

			var decimalAmount = currency.GetBaseCurrency(strAmount, out var success);
			if (!success || decimalAmount <= 0.0M)
			{
				character.OutputHandler.Send("That is not a valid amount of currency to load.");
				return;
			}

			coins = currency.FindCoinsForAmount(decimalAmount, out var exact);
			if (!exact)
			{
				character.OutputHandler.Send(
					"Warning: Could not find an exact match for the total specified.".Colour(Telnet.Red));
			}
		}

		newItem = CurrencyGameItemComponentProto.CreateNewCurrencyPile(currency,
			coins.Select(x => Tuple.Create(x.Key, x.Value)));
		character.Gameworld.Add(newItem);
		character.OutputHandler.Handle(new EmoteOutput(new Emote("@ load|loads $0.", character, newItem),
			flags: OutputFlags.SuppressObscured));
		if (character.Body.CanGet(newItem, 0))
		{
			character.Body.Get(newItem, silent: true);
		}
		else
		{
			newItem.RoomLayer = character.RoomLayer;
			character.OutputHandler.Send("Your hands are full, so you loaded the item to the ground.");
			character.Location.Insert(newItem);
		}
	}

	[PlayerCommand("LoadCommodity", "loadcommodity")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[HelpInfo("loadcommodity",
		"This command allows you to load commodities, which are raw quantities of bulk materials. The syntax is #3loadcommodity <weight> <material> [<tag>]#0.",
		AutoHelp.HelpArgOrNoArg)]
	protected static void LoadCommodity(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var amount =
			actor.Gameworld.UnitManager.GetBaseUnits(ss.PopSpeech(), Framework.Units.UnitType.Mass, out var success);
		if (!success)
		{
			actor.Send("That is not a valid weight of commodity to load.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("Which material do you want to load as a commodity?");
			return;
		}

		var material = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.Materials.Get(value)
			: actor.Gameworld.Materials.GetByName(ss.Last);

		if (material == null)
		{
			actor.Send("There is no such material.");
			return;
		}

		ITag tag = null;
		if (!ss.IsFinished)
		{
			tag = actor.Gameworld.Tags.GetByIdOrName(ss.SafeRemainingArgument);
			if (tag is null)
			{
				actor.OutputHandler.Send(
					$"There is no such tag identified by {ss.SafeRemainingArgument.ColourCommand()}.");
				return;
			}
		}

		var commodity = CommodityGameItemComponentProto.CreateNewCommodity(material, amount, tag);
		commodity.RoomLayer = actor.RoomLayer;
		actor.Gameworld.Add(commodity);
		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ load|loads $0.", actor, commodity),
			flags: OutputFlags.SuppressObscured));
		if (actor.Body.CanGet(commodity, 0))
		{
			actor.Body.Get(commodity, silent: true);
		}
		else
		{
			actor.OutputHandler.Send("Your hands are full, so you loaded the item to the ground.");
			commodity.RoomLayer = actor.RoomLayer;
			actor.Location.Insert(commodity);
		}
	}

	[PlayerCommand("LoadColourLiquid", "loadcolourliquid", "loadcolorliquid")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[HelpInfo("LoadColourLiquid",
		@"The LoadColourLiquid command is used to load coloured liquids, such as tattoo ink. The syntax is #3loadcolourliquid <type> <colour> <target>#0.",
		AutoHelp.HelpArgOrNoArg)]
	protected static void LoadColourLiquid(ICharacter character, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			character.Send("Which type of colour liquid do you want to load? Valid types are tattoo_ink.");
			return;
		}

		var typeText = ss.PopSpeech().ToLowerInvariant();

		if (ss.IsFinished)
		{
			character.OutputHandler.Send("What colour do you want to load for that liquid?");
			return;
		}

		var colour = long.TryParse(ss.PopSpeech(), out var value)
			? character.Gameworld.Colours.Get(value)
			: character.Gameworld.Colours.GetByName(ss.Last);
		if (colour == null)
		{
			character.OutputHandler.Send("There is no such colour.");
			return;
		}

		if (ss.IsFinished)
		{
			character.Send("What do you want to load that liquid into?");
			return;
		}

		string targettext = ss.Pop(), chartext = null;
		if (!ss.IsFinished) // They can load something into a liquid container someone holds
		{
			chartext = targettext;
			targettext = ss.Pop();
		}

		IGameItem target;
		if (!string.IsNullOrEmpty(chartext))
		{
			var charTarget = character.TargetActor(chartext);
			if (charTarget == null)
			{
				character.Send("You do not see that person here.");
				return;
			}

			target = charTarget.Inventory.GetFromItemListByKeyword(targettext, character);
		}
		else
		{
			target = character.TargetItem(targettext);
		}

		if (target == null)
		{
			character.Send("You do not see anything like that to load the liquid into.");
			return;
		}

		if (!target.IsItemType<ILiquidContainer>())
		{
			character.Send("{0} is not a liquid container, and so cannot contain liquid.", target.HowSeen(character));
			return;
		}

		var lqtarget = target.GetItemType<ILiquidContainer>();
		if (lqtarget.LiquidVolume >= lqtarget.LiquidCapacity)
		{
			character.OutputHandler.Send($"{target.HowSeen(character, true)} is already full of liquid.");
			return;
		}

		LiquidMixture liquid;
		string liquidDescription;
		switch (typeText)
		{
			case "tattoo":
			case "tattooink":
			case "tattoo ink":
			case "tattoo_ink":
				liquid = new LiquidMixture(
					new List<LiquidInstance>
					{
						new ColourLiquidInstance(TattooTemplate.InkLiquid, colour,
							lqtarget.LiquidCapacity - lqtarget.LiquidVolume)
					}, character.Gameworld);
				liquidDescription = $"{colour.Name} tattoo ink";
				break;
			default:
				character.OutputHandler.Send("That is not a valid colour liquid type. Valid types are tattoo_ink.");
				return;
		}

		character.OutputHandler.Handle(
			new EmoteOutput(new Emote($"@ load|loads {liquidDescription} into $0.", character, target),
				flags: OutputFlags.SuppressObscured));
		lqtarget.MergeLiquid(liquid, character, "loadliquid");
	}

	[PlayerCommand("LoadBlood", "loadblood")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void LoadBlood(ICharacter character, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			character.Send("Which character's blood do you want to load?");
			return;
		}

		var targetCharacter = long.TryParse(ss.PopSpeech(), out var value)
			? character.Gameworld.TryGetCharacter(value, true)
			: character.TargetActor(ss.Last);
		if (targetCharacter == null)
		{
			character.OutputHandler.Send("There is no such character.");
			return;
		}

		if (ss.IsFinished)
		{
			character.Send("What do you want to load that blood into?");
			return;
		}

		string targettext = ss.Pop(), chartext = null;
		if (!ss.IsFinished) // They can load something into a liquid container someone holds
		{
			chartext = targettext;
			targettext = ss.Pop();
		}

		IGameItem target;
		if (!string.IsNullOrEmpty(chartext))
		{
			var charTarget = character.TargetActor(chartext);
			if (charTarget == null)
			{
				character.Send("You do not see that person here.");
				return;
			}

			target = charTarget.Inventory.GetFromItemListByKeyword(targettext, character);
		}
		else
		{
			target = character.TargetItem(targettext);
		}

		if (target == null)
		{
			character.Send("You do not see anything like that to load blood into.");
			return;
		}

		if (!target.IsItemType<ILiquidContainer>())
		{
			character.Send("{0} is not a liquid container, and so cannot contain liquid.", target.HowSeen(character));
			return;
		}

		var lqtarget = target.GetItemType<ILiquidContainer>();
		if (lqtarget.LiquidVolume >= lqtarget.LiquidCapacity)
		{
			character.OutputHandler.Send($"{target.HowSeen(character, true)} is already full of liquid.");
			return;
		}

		var liquid =
			new LiquidMixture(
				new List<LiquidInstance>
					{ new BloodLiquidInstance(character, lqtarget.LiquidCapacity - lqtarget.LiquidVolume) },
				character.Gameworld);
		character.OutputHandler.Handle(
			new EmoteOutput(new Emote($"@ load|loads $1's blood into $0.", character, target, targetCharacter),
				flags: OutputFlags.SuppressObscured));

		lqtarget.MergeLiquid(liquid, character, "loadliquid");
	}

	[PlayerCommand("LoadLiquid", "loadliquid")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void LoadLiquid(ICharacter character, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			character.Send("Which liquid do you want to load?");
			return;
		}

		var liquid = long.TryParse(ss.PopSpeech(), out var value)
			? character.Gameworld.Liquids.Get(value)
			: character.Gameworld.Liquids.FirstOrDefault(
				x => x.Name.Equals(ss.Last, StringComparison.InvariantCultureIgnoreCase));
		if (liquid == null)
		{
			character.Send("There is no such liquid for you to load.");
			return;
		}

		if (ss.IsFinished)
		{
			character.Send("What do you want to load that liquid into?");
			return;
		}

		string targettext = ss.Pop(), chartext = null;
		if (!ss.IsFinished) // They can load something into a liquid container someone holds
		{
			chartext = targettext;
			targettext = ss.Pop();
		}

		IGameItem target;
		if (!string.IsNullOrEmpty(chartext))
		{
			var charTarget = character.TargetActor(chartext);
			if (charTarget == null)
			{
				character.Send("You do not see that person here.");
				return;
			}

			target = charTarget.Inventory.GetFromItemListByKeyword(targettext, character);
		}
		else
		{
			target = character.TargetItem(targettext);
		}

		if (target == null)
		{
			character.Send("You do not see anything like that to load liquid into.");
			return;
		}

		if (!target.IsItemType<ILiquidContainer>())
		{
			character.Send("{0} is not a liquid container, and so cannot contain liquid.", target.HowSeen(character));
			return;
		}

		var lqtarget = target.GetItemType<ILiquidContainer>();

		if (lqtarget.LiquidVolume >= lqtarget.LiquidCapacity)
		{
			character.OutputHandler.Send($"{target.HowSeen(character, true)} is already full of liquid.");
			return;
		}

		character.OutputHandler.Handle(
			new EmoteOutput(new Emote($"@ load|loads {liquid.MaterialDescription} into $0.", character, target),
				flags: OutputFlags.SuppressObscured));

		lqtarget.MergeLiquid(
			new LiquidMixture(liquid, lqtarget.LiquidCapacity - lqtarget.LiquidVolume, character.Gameworld), character,
			"loadliquid");
	}

	[PlayerCommand("LoadGas", "loadgas")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void LoadGas(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which gas did you want to load?");
			return;
		}

		var gas = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.Gases.Get(value)
			: actor.Gameworld.Gases.FirstOrDefault(
				x => x.Name.Equals(ss.Last, StringComparison.InvariantCultureIgnoreCase));
		if (gas == null)
		{
			actor.Send("There is no such gas for you to load.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("What do you want to load that gas into?");
			return;
		}

		string targettext = ss.Pop(), chartext = null;
		if (!ss.IsFinished) // They can load something into a gas container someone holds
		{
			chartext = targettext;
			targettext = ss.Pop();
		}

		IGameItem target;
		if (!string.IsNullOrEmpty(chartext))
		{
			var charTarget = actor.TargetActor(chartext);
			if (charTarget == null)
			{
				actor.Send("You do not see that person here.");
				return;
			}

			target = charTarget.Inventory.GetFromItemListByKeyword(targettext, actor);
		}
		else
		{
			target = actor.TargetItem(targettext);
		}

		if (target == null)
		{
			actor.Send("You do not see anything like that to load liquid into.");
			return;
		}

		if (!target.IsItemType<IGasContainer>())
		{
			actor.Send("{0} is not a gas container, and so cannot contain gases.", target.HowSeen(actor));
			return;
		}

		var gcTarget = target.GetItemType<IGasContainer>();
		gcTarget.Gas = gas;
		gcTarget.GasVolumeAtOneAtmosphere = gcTarget.GasCapacityAtOneAtmosphere;
		actor.OutputHandler.Handle(
			new EmoteOutput(new Emote($"@ load|loads {gas.MaterialDescription} into $0.", actor, target),
				flags: OutputFlags.SuppressObscured));
	}

	[PlayerCommand("Notes", "notes")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void Notes(ICharacter character, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var cmd = ss.Pop();

		if (string.IsNullOrEmpty(cmd))
		{
			character.OutputHandler.Send("Which account do you want to list the notes for?");
			return;
		}

		using (new FMDB())
		{
			var account =
				FMDB.Context.Accounts.FirstOrDefault(x => x.Name == cmd);
			if (account == null)
			{
				character.OutputHandler.Send("That is not a valid account.");
				return;
			}

			var notes =
				FMDB.Context.AccountNotes.Where(x => x.AccountId == account.Id)
				    .OrderByDescending(x => x.TimeStamp)
				    .ThenBy(x => x.Id)
				    .ToList();
			character.OutputHandler.Send(
				StringUtilities.GetTextTable(
					from note in notes
					select
						new[]
						{
							note.Id.ToString(), note.TimeStamp.GetLocalDateString(character),
							note.Author != null ? note.Author.Name.Proper() : "System",
							note.IsJournalEntry
								? $"[{note.Character.Name}'s Journal]: \"{note.Subject}\""
								: note.Subject.Proper(),
							note.IsJournalEntry.ToString(character)
						},
					new[] { "Id", "Time", "Author", "Subject", "Journal?" },
					character.Account.LineFormatLength,
					colour: Telnet.Green,
					truncatableColumnIndex: 3,
					unicodeTable: character.Account.UseUnicode
				)
			);
		}
	}

	private static void NoteRead(ICharacter character, StringStack input)
	{
		if (!long.TryParse(input.Pop(), out var value))
		{
			character.OutputHandler.Send("That is not a valid note ID.");
			return;
		}

		using (new FMDB())
		{
			var note = FMDB.Context.AccountNotes.Find(value);
			if (note == null)
			{
				character.OutputHandler.Send("There is no note with that ID number.");
				return;
			}

			var sb = new StringBuilder();
			sb.AppendLine(
				$"{$"Account Note {note.Id.ToString().Colour(Telnet.Green)}".RawTextPadRight(25)}{$"Account: {note.Account.Name.Proper().Colour(Telnet.Green)}".RawTextPadRight(25)}{string.Format($"Author: {(note.Author != null ? note.Author.Name.Proper().Colour(Telnet.Green) : "System".Colour(Telnet.Green))}", note.Author != null ? note.Author.Name.Proper().Colour(Telnet.Green) : "System".Colour(Telnet.Green))}");
			sb.AppendLine("Subject: " + note.Subject.Colour(Telnet.Green));
			if (note.CharacterId != null)
			{
				sb.AppendLine();
				sb.AppendLine(
					$"From the journal of {new PersonalName(XElement.Parse(note.Character.NameInfo).Element("PersonalName"), character.Gameworld).GetName(NameStyle.FullName).ColourName()}.");
				sb.AppendLine();
			}

			sb.AppendLine("Text:");
			sb.AppendLine();
			sb.AppendLine(note.Text.Wrap(80, "\t").NoWrap());

			character.OutputHandler.Send(sb.ToString());
		}
	}

	private static void NoteWritePost(string message, IOutputHandler handler, object[] arguments)
	{
		using (new FMDB())
		{
			var note = new AccountNote
			{
				Text = message,
				AccountId = (long)arguments[0],
				AuthorId = (long)arguments[2],
				Subject = (string)arguments[1],
				TimeStamp = DateTime.UtcNow
			};
			FMDB.Context.AccountNotes.Add(note);
			FMDB.Context.SaveChanges();
			handler.Send("You finish writing account note #" + note.Id + ".");
		}
	}

	private static void NoteWriteCancel(IOutputHandler handler, object[] arguments)
	{
		handler.Send("You decide not to post any account note.");
	}

	private static void NoteWrite(ICharacter character, StringStack input)
	{
		var accountName = input.Pop().ToLowerInvariant();
		if (string.IsNullOrEmpty(accountName))
		{
			character.OutputHandler.Send("To which account will your note pertain?");
			return;
		}

		var subject = input.SafeRemainingArgument.Trim();
		if (string.IsNullOrEmpty(subject))
		{
			character.OutputHandler.Send("You must supply a subject for your account note.");
			return;
		}

		using (new FMDB())
		{
			var account = FMDB.Context.Accounts.FirstOrDefault(x => x.Name == accountName);
			if (account == null)
			{
				character.OutputHandler.Send("There is no such account.");
				return;
			}

			character.OutputHandler.Send("Write your note in the text editor below.");
			character.EditorMode(NoteWritePost, NoteWriteCancel, 1.0,
				suppliedArguments: new object[] { account.Id, subject, character.Account.Id });
		}
	}

	[PlayerCommand("Note", "note")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void Note(ICharacter character, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		switch (ss.Pop().ToLowerInvariant())
		{
			case "read":
				NoteRead(character, ss);
				break;
			case "write":
				NoteWrite(character, ss);
				break;
			default:
				character.OutputHandler.Send("Do you want to read or write a note?");
				return;
		}
	}

	[PlayerCommand("Echo", "echo")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void Echo(ICharacter actor, string input)
	{
		bool auditory = false, visual = false;
		var difficulty = Difficulty.Automatic;
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.Peek().StartsWith("-", StringComparison.Ordinal))
		{
			switch (ss.Pop().RemoveFirstCharacter().ToLowerInvariant())
			{
				case "a":
					auditory = true;
					break;
				case "v":
					visual = true;
					break;
				case "av":
					auditory = true;
					visual = true;
					break;
				default:
					actor.OutputHandler.Send("Valid options are -a, -v or -av.");
					return;
			}

			if (!Enum.TryParse(ss.PopSpeech(), true, out difficulty))
			{
				actor.OutputHandler.Send("That is not a valid difficulty. Valid difficulties are " +
				                         (from differ in Enum.GetNames(typeof(Difficulty))
				                          select differ.Colour(Telnet.Cyan)).ListToString() + ".");
				return;
			}
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What did you want to echo?");
			return;
		}

		string primaryText, secondaryText = "";
		var match = _echoOnFailureRegex.Match(ss.RemainingArgument);
		if (match.Success)
		{
			primaryText = match.Groups[1].Value.SubstituteANSIColour();
			secondaryText = match.Groups[2].Value.SubstituteANSIColour();
		}
		else
		{
			primaryText = ss.RemainingArgument.SubstituteANSIColour();
		}

		var primaryEmote = new PlayerEmote(primaryText, actor, false);
		if (!primaryEmote.Valid)
		{
			actor.OutputHandler.Send(primaryEmote.ErrorMessage);
			return;
		}

		var secondaryEmote = new PlayerEmote(secondaryText, actor, false);
		if (!string.IsNullOrEmpty(secondaryText) && !secondaryEmote.Valid)
		{
			actor.OutputHandler.Send(secondaryEmote.ErrorMessage);
			return;
		}

		var primaryOutput = new EmoteOutput(primaryEmote);
		var secondaryOutput = new EmoteOutput(secondaryEmote);

		actor.OutputHandler.Send("You send out an echo:");
		actor.OutputHandler.Send(primaryOutput);
		var audioCheck = actor.Gameworld.GetCheck(CheckType.GenericListenCheck);
		var visualCheck = actor.Gameworld.GetCheck(CheckType.GenericSpotCheck);
		foreach (var person in actor.Location.Characters.Except(actor))
		{
			if (!person.IsAdministrator() &&
			    ((auditory && audioCheck.Check(person, difficulty).IsFail()) ||
			     (visual && visualCheck.Check(person, difficulty).IsFail())))
			{
				if (secondaryText.Length != 0)
				{
					person.OutputHandler.Send(secondaryOutput);
				}

				actor.Send("{0} failed {1} check.", person.HowSeen(actor, true), person.Gender.Possessive());
				continue;
			}

			person.OutputHandler.Send(primaryOutput);
		}
	}

	[PlayerCommand("ZEcho", "zecho")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void ZEcho(ICharacter actor, string input)
	{
		var echo = input.RemoveFirstWord();
		if (string.IsNullOrEmpty(echo))
		{
			actor.Send("What do you want to zecho?");
			return;
		}

		actor.Send("Echoing to zone...");
		var message = echo.SubstituteANSIColour().ProperSentences();
		foreach (var perceiver in actor.Location.Room.Zone.Characters)
		{
			perceiver.Send(message);
		}
	}

	[PlayerCommand("PEcho", "pecho")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void PEcho(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var targets = new List<ICharacter>();
		foreach (var starget in ss.Pop().Split(','))
		{
			var target = actor.TargetActor(starget);
			if (target == null)
			{
				actor.Send("You do not see anyone like that to pecho to.");
				return;
			}

			if (targets.Contains(target))
			{
				continue;
			}

			targets.Add(target);
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to echo?");
			return;
		}

		var emoteData = new PlayerEmote(ss.RemainingArgument.SubstituteANSIColour(), actor.Body);
		if (emoteData.Valid)
		{
			foreach (var target in targets)
			{
				target.OutputHandler.Handle(new EmoteOutput(emoteData), OutputRange.Personal);
			}

			actor.OutputHandler.Send(
				$"You echo to {targets.Select(x => x.HowSeen(actor)).ListToString()}: \n{emoteData.ParseFor(actor)}");
		}
		else
		{
			actor.OutputHandler.Send(emoteData.ErrorMessage);
		}
	}

	[PlayerCommand("Mortal", "mortal")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void Mortal(ICharacter actor, string input)
	{
		if (!actor.AffectedBy<IAdminSightEffect>())
		{
			actor.OutputHandler.Send("You are already mortal.");
			return;
		}

		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ are|is no longer immortal.", actor.Body),
			flags: OutputFlags.WizOnly));
		actor.RemoveAllEffects(x => x.IsEffectType<IAdminSightEffect>());
		actor.Gameworld.GameStatistics.UpdateOnlinePlayers();
	}

	[PlayerCommand("ImmWalk", "immwalk")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void ImmWalk(ICharacter actor, string input)
	{
		if (actor.AffectedBy<IImmwalkEffect>())
		{
			actor.RemoveAllEffects(x => x.IsEffectType<IImmwalkEffect>());
			actor.OutputHandler.Send("You will no longer Imm Walk.");
		}
		else
		{
			actor.AddEffect(new Immwalk(actor));
			actor.OutputHandler.Send("You will now Imm Walk.");
		}
	}

	[PlayerCommand("Immortal", "immortal", "imm")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void Immortal(ICharacter actor, string input)
	{
		if (actor.AffectedBy<IAdminSightEffect>())
		{
			actor.OutputHandler.Send("You are already immortal.");
			return;
		}

		actor.AddEffect(new AdminSight(actor));
		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ become|becomes immortal.", actor),
			flags: OutputFlags.WizOnly));
	}

	[PlayerCommand("Vis", "vis")]
	protected static void Vis(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var cmd = ss.Pop().ToLowerInvariant();

		if (cmd == "silent" || cmd == "discrete")
		{
			if (!actor.AffectedBy<IAdminInvisEffect>())
			{
				actor.OutputHandler.Send("You are already visible.");
				return;
			}

			actor.RemoveAllEffects(x => x.IsEffectType<IAdminInvisEffect>());
			actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ fade|fades into view discretely.", actor),
				flags: OutputFlags.WizOnly));
		}

		if (!string.IsNullOrEmpty(cmd))
		{
			var target = actor.Target(cmd);
			if (target == null)
			{
				actor.Send("You don't see anything or anyone like that to turn visible.");
				return;
			}

			if (!target.AffectedBy<IAdminInvisEffect>())
			{
				actor.Send($"{target.HowSeen(actor, true)} is already visible.");
				return;
			}

			target.RemoveAllEffects(x => x.IsEffectType<IAdminInvisEffect>());
			target.OutputHandler.Handle(new EmoteOutput(new Emote("@ fade|fades into view.", (IPerceiver)target)));
			return;
		}

		if (!actor.AffectedBy<IAdminInvisEffect>())
		{
			actor.OutputHandler.Send("You are already visible.");
			return;
		}

		actor.RemoveAllEffects(x => x.IsEffectType<IAdminInvisEffect>());
		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ fade|fades into view.", actor)));
	}

	[PlayerCommand("Invis", "invis")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void Invis(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var cmd = ss.Pop().ToLowerInvariant();
		if (cmd == "silent" || cmd == "discrete")
		{
			if (actor.AffectedBy<IAdminInvisEffect>())
			{
				actor.OutputHandler.Send("You are already invisible.");
				return;
			}

			actor.OutputHandler.Handle(
				new EmoteOutput(new Emote("@ disappear|disappears from view discretely.", actor.Body),
					flags: OutputFlags.WizOnly));
			actor.AddEffect(new AdminInvis(actor));
			return;
		}

		if (!string.IsNullOrEmpty(cmd))
		{
			var target = actor.Target(cmd);
			if (target == null)
			{
				actor.Send("You don't see anything or anyone like that to turn invisible.");
				return;
			}

			if (target.AffectedBy<IAdminInvisEffect>())
			{
				actor.Send($"{target.HowSeen(actor, true)} is already invisible.");
				return;
			}

			target.AddEffect(new AdminInvis(target));
			target.OutputHandler.Handle(
				new EmoteOutput(new Emote("@ disappear|disappears from view.", (IPerceiver)target)));
			return;
		}

		if (actor.AffectedBy<IAdminInvisEffect>())
		{
			actor.OutputHandler.Send("You are already invisible.");
			return;
		}

		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ disappear|disappears from view.", actor)));
		actor.AddEffect(new AdminInvis(actor));
	}

	[PlayerCommand("Transfer", "transfer")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void Transfer(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var targetText = ss.SafeRemainingArgument;
		if (string.IsNullOrEmpty(targetText))
		{
			actor.OutputHandler.Send("Who do you want to transfer?");
			return;
		}

		var target = actor.Gameworld.Actors.GetByName(targetText) ??
		             actor.Gameworld.Actors.GetFromItemListByKeywordIncludingNames(targetText, actor);
		if (target == null)
		{
			actor.OutputHandler.Send("There is no such player for you to transfer.");
			return;
		}

		if (target.ColocatedWith(actor))
		{
			actor.OutputHandler.Send("They are already in the same location as you.");
			return;
		}

		target.Movement?.CancelForMoverOnly(target);
		target.RemoveAllEffects(x => x.IsEffectType<IActionEffect>());

		target.OutputHandler.Handle(new EmoteOutput(new Emote("@ leaves the area.", target),
			flags: OutputFlags.SuppressObscured | OutputFlags.SuppressSource));
		target.OutputHandler.Send(new EmoteOutput(new Emote("$0 transfers you to &0's location.", target, actor)));
		actor.OutputHandler.Send(new EmoteOutput(new Emote("You transfer $0 to your location.", actor, target)));
		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ enters the area.", target),
			flags: OutputFlags.SuppressObscured));
		// TODO - hook events here?
		target.Location.Leave(target);
		target.RoomLayer = actor.RoomLayer;
		actor.Location.Enter(target);
		target.Body.Look(true);
	}

	[PlayerCommand("Goto", "goto")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[RequiredCharacterState(CharacterState.Conscious)]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[HelpInfo("goto",
		@"The GOTO command allows you to move yourself to a particular location in the gameworld. If you are not invisible, this will echo to the room - otherwise, if you have your admin invisibility up only other admins in the origin or destination will see you go.

There are several different ways you can use this command, as per below:

	#3goto <roomnumber>#0 - Go to a particular room identified by number
	#3goto <character name>#0 - Go to the location of a particular named character
	#3goto <character keywords>#0 - Go to the location of a character by keywords
	#3goto @<number>#0 - Go to a recently created room. See below for explanation:

#6For example, @1 is the most recently created new room, @2 is the 2nd most recently created room etc.
This only works for rooms created since the last reboot.
This command is useful when you write-up a bunch of room creation commands in a text file to paste into the MUD at once, so you can refer to the rooms that you create rather than having to presuppose what the room ID will be.#0",
		AutoHelp.HelpArgOrNoArg)]
	protected static void Goto(ICharacter actor, string input)
	{
		var destinationLayer = actor.RoomLayer;
		var destination = RoomBuilderModule.LookupCell(actor.Gameworld, input.RemoveFirstWord());
		if (destination == null)
		{
			var ss = new StringStack(input.RemoveFirstWord());
			var cmd = ss.SafeRemainingArgument;
			var target = actor.Gameworld.Actors.Where(x => !x.State.HasFlag(CharacterState.Dead))
			                  .GetFromItemListByKeywordIncludingNames(cmd, actor);

			if (target == null)
			{
				actor.OutputHandler.Send("There are no locations and no-one with that name or keyword to go to.");
				return;
			}

			destination = target.Location;
			destinationLayer = target.RoomLayer;
		}

		if (destination == actor.Location && destinationLayer == actor.RoomLayer)
		{
			actor.OutputHandler.Send("You are already there!");
			return;
		}

		actor.TransferTo(destination, destinationLayer);
	}

	public const string SkillCommandHelp =
		@"This skill allows you to create and edit skills, as well as adding, removing, setting the level, pausing and unpausing skills for players.

This is the syntax for the storyteller portion of the command:

    skill add <who> <skill> [<level>] - adds a skill to a character
    skill remove <who> <skill> - removes a skill from a character
    skill level <who> <skill> <level> - sets a character's skill to the specified amount

This is the syntax for editing skills:

    skill edit <skill> - begins editing a particular skill
    skill edit - synonymous with SKILL VIEW on your currently edited skill
    skill edit new <name> - creates a new skill
    skill clone <cloned> <name> - clones an existing skill
    skill edit close - stops editing a skill
    skill view <skill> - shows details of a skill
    skill set name <name> - edits the name of a skill
    skill set expression <expression> - changes the cap expression for a skill(*)
    skill set improver <which> - sets the skill improver for a skill
    skill set describer <which> - sets the skill describer for a skill
    skill set group <group> - sets the skill group for a skill
    skill set branch <multiplier%> - sets the branch multiplier for a skill
    skill set chargen <prog> - sets a prog to determine chargen availability
    skill set teachable <prog> - sets a prog to determine teachability
    skill set learnable <prog> - sets a prog to determine learnability
    skill set teach <difficulty> - sets the difficulty of teaching the skill
    skill set learn <difficulty> - sets the difficulty of learning the skill
    skill set hidden - toggles this being a hidden skill

    * - most often you will want to use the TRAITEXPRESSION command to edit the existing trait expression rather than changing to a new one";

	[PlayerCommand("Skill", "skill")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("Skill", SkillCommandHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void Skill(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var actionText = ss.PopSpeech().ToLowerInvariant();

		switch (actionText)
		{
			case "edit":
			case "set":
			case "clone":
				if (!actor.IsAdministrator(PermissionLevel.SeniorAdmin))
				{
					actor.OutputHandler.Send("Skill editing is only available to Senior Administrators or higher.");
					return;
				}

				break;
		}

		switch (actionText)
		{
			case "list":
				ShowModule.Show_Skills(actor, ss);
				return;
			case "edit":
				SkillEdit(actor, ss);
				return;
			case "set":
				SkillSet(actor, ss);
				return;
			case "clone":
				SkillClone(actor, ss);
				return;
			case "view":
				SkillView(actor, ss);
				return;
			case "add":
			case "level":
			case "remove":
			case "pause":
			case "unpause":
				break;
			default:
				actor.OutputHandler.Send(SkillCommandHelp);
				return;
		}

		if (ss.IsFinished)
		{
			actor.Send("Whose skills do you want to change?");
			return;
		}

		var targetText = ss.PopSpeech();
		ICharacter target;
		if (long.TryParse(targetText, out var value))
		{
			target = actor.Gameworld.TryGetCharacter(value, true);
		}
		else
		{
			target = actor.TargetActor(targetText) ?? actor.Gameworld.Characters.GetByPersonalName(targetText);
		}

		if (target == null)
		{
			actor.Send("You do not see anybody like that whose skills you can edit.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("Which skill do you want to change for {0}?", target.HowSeen(actor));
			return;
		}

		var skill = long.TryParse(ss.PopSpeech(), out value)
			? actor.Gameworld.Traits.Get(value)
			: actor.Gameworld.Traits.GetByName(ss.Last);
		if (skill == null || skill.TraitType != TraitType.Skill)
		{
			actor.Send("There is no such skill.");
			return;
		}

		switch (actionText)
		{
			case "add":
				if (target.HasTrait(skill))
				{
					actor.Send("{0} already has the {1} skill.", target.HowSeen(actor, true),
						skill.Name.Proper().Colour(Telnet.Green));
					return;
				}

				var dvalue = 1.0;
				if (!ss.IsFinished && !double.TryParse(ss.Pop(), out dvalue))
				{
					actor.Send("You must either specify a value for the new skill, or leave blank to start at 1.");
					return;
				}

				target.AddTrait(skill, dvalue);
				actor.Send("You add the {0} skill to {1} at a value of {2:N2}.",
					skill.Name.Proper().Colour(Telnet.Green), target.HowSeen(actor), dvalue);
				return;
			case "remove":
			case "rem":
				if (!target.HasTrait(skill))
				{
					actor.Send("{0} does not have the {1} skill.", target.HowSeen(actor, true),
						skill.Name.Proper().Colour(Telnet.Green));
					return;
				}

				target.RemoveTrait(skill);
				actor.Send("You remove the {0} skill from {1}", skill.Name.Proper().Colour(Telnet.Green),
					target.HowSeen(actor));
				return;
			case "level":
				dvalue = 1.0;
				if (!ss.IsFinished && !double.TryParse(ss.Pop(), out dvalue))
				{
					actor.Send("You must either specify a value for the new skill, or leave blank to start at 1.");
					return;
				}

				if (!target.HasTrait(skill))
				{
					target.AddTrait(skill, dvalue);
					actor.Send("You add the {0} skill to {1} at a value of {2:N2}.",
						skill.Name.Proper().Colour(Telnet.Green), target.HowSeen(actor), dvalue);
					return;
				}

				target.SetTraitValue(skill, dvalue);
				actor.Send("You set the value of {0} on {1} to {2:N2}.", skill.Name.Proper().Colour(Telnet.Green),
					target.HowSeen(actor), dvalue);
				return;
			case "pause":
			case "unpause":
				actor.Send("Coming soon.");
				return;
			default:
				actor.Send("Do you want to add, remove, set, pause or unpause a skill?");
				return;
		}
	}

	private static void SkillView(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which skill do you want to view?");
			return;
		}

		if ((long.TryParse(ss.PopSpeech(), out var value)
			    ? actor.Gameworld.Traits.Get(value)
			    : actor.Gameworld.Traits.GetByName(ss.Last)) is not ISkillDefinition skill)
		{
			actor.OutputHandler.Send("There is no such skill.");
			return;
		}

		actor.OutputHandler.Send(skill.Show(actor));
	}

	private static void SkillClone(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which skill do you want to clone?");
			return;
		}

		if ((long.TryParse(ss.PopSpeech(), out var value)
			    ? actor.Gameworld.Traits.Get(value)
			    : actor.Gameworld.Traits.GetByName(ss.Last)) is not ISkillDefinition skill)
		{
			actor.OutputHandler.Send("There is no such skill.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to the new skill?");
			return;
		}

		var name = ss.PopSpeech().ToLowerInvariant().TitleCase();
		if (actor.Gameworld.Traits.Any(x => x.TraitType == TraitType.Skill && x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				$"There is already a skill called {name.ColourName()}. Skill names must be unique.");
			return;
		}

		using (new FMDB())
		{
			var newSkillModel = new Models.TraitDefinition
			{
				Name = name,
				Type = (int)TraitType.Skill,
				DerivedType = 0,
				TraitGroup = skill.Group,
				ChargenBlurb = string.Empty,
				Hidden = skill.Hidden,
				DecoratorId = skill.Decorator.Id,
				ImproverId = skill.Improver.Id,
				AvailabilityProgId = skill.AvailabilityProg.Id,
				TeachDifficulty = (int)skill.TeachDifficulty,
				LearnDifficulty = (int)skill.LearnDifficulty,
				TeachableProgId = skill.TeachableProg.Id,
				LearnableProgId = skill.LearnableProg.Id,
				BranchMultiplier = skill.BranchMultiplier
			};
			var expression = new Models.TraitExpression
			{
				Name = $"{name} Cap",
				Expression = skill.Cap.Formula.OriginalExpression
			};
			foreach (var parameter in skill.Cap.Parameters)
			{
				expression.TraitExpressionParameters.Add(new TraitExpressionParameters
				{
					TraitExpression = expression,
					TraitDefinitionId = parameter.Value.Trait.Id,
					CanBranch = parameter.Value.CanBranch,
					CanImprove = parameter.Value.CanImprove,
					Parameter = parameter.Key
				});
			}

			newSkillModel.Expression = expression;
			FMDB.Context.TraitExpressions.Add(expression);
			FMDB.Context.TraitDefinitions.Add(newSkillModel);
			FMDB.Context.SaveChanges();

			actor.Gameworld.Add(new TraitExpression(expression, actor.Gameworld));
			var newSkill = new SkillDefinition(newSkillModel, actor.Gameworld);
			actor.Gameworld.Add(newSkill);
			newSkill.Initialise(newSkillModel);
			actor.RemoveAllEffects<BuilderEditingEffect<ISkillDefinition>>();
			actor.AddEffect(new BuilderEditingEffect<ISkillDefinition>(actor) { EditingItem = newSkill });
			actor.OutputHandler.Send(
				$"You create the {newSkill.Name.TitleCase().ColourName()} skill, which you are now editing.");
		}
	}

	private static void SkillSet(ICharacter actor, StringStack ss)
	{
		var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<ISkillDefinition>>().FirstOrDefault();
		if (editing == null)
		{
			actor.OutputHandler.Send("You are not editing any skills.");
			return;
		}

		editing.EditingItem.BuildingCommand(actor, ss);
	}

	private static void SkillEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<ISkillDefinition>>().FirstOrDefault();
			if (editing == null)
			{
				actor.OutputHandler.Send("You are not editing any skills.");
				return;
			}

			actor.OutputHandler.Send(editing.EditingItem.Show(actor));
			return;
		}

		var actionText = ss.PopSpeech();
		if (actionText.EqualTo("new"))
		{
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("What name do you want to give to the new skill?");
				return;
			}

			var name = ss.PopSpeech().ToLowerInvariant().TitleCase();
			if (actor.Gameworld.Traits.Any(x => x.TraitType == TraitType.Skill && x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already a skill called {name.ColourName()}. Skill name must be unique.");
				return;
			}

			using (new FMDB())
			{
				var newSkillModel = new Models.TraitDefinition
				{
					Name = name,
					Type = (int)TraitType.Skill,
					DerivedType = 0,
					TraitGroup = "General",
					ChargenBlurb = string.Empty,
					Hidden = false,
					DecoratorId =
						(actor.Gameworld.TraitDecorators.FirstOrDefault(x =>
							 x.Id == actor.Gameworld.GetStaticLong("DefaultSkillDecorator")) ??
						 actor.Gameworld.TraitDecorators.First()).Id,
					ImproverId =
						(actor.Gameworld.ImprovementModels.FirstOrDefault(x =>
							 x.Id == actor.Gameworld.GetStaticLong("DefaultSkillImprover")) ??
						 actor.Gameworld.ImprovementModels.First()).Id,
					AvailabilityProgId = actor.Gameworld.FutureProgs.First(x => x.FunctionName == "AlwaysTrue").Id,
					TeachDifficulty = (int)Difficulty.VeryHard,
					LearnDifficulty = (int)Difficulty.VeryHard,
					TeachableProgId = actor.Gameworld.FutureProgs.First(x => x.FunctionName == "AlwaysFalse").Id,
					LearnableProgId = actor.Gameworld.FutureProgs.First(x => x.FunctionName == "AlwaysTrue").Id,
					BranchMultiplier = 1.0,
					Expression = new Models.TraitExpression { Name = $"{name} Cap", Expression = "70" }
				};
				FMDB.Context.TraitDefinitions.Add(newSkillModel);
				FMDB.Context.SaveChanges();

				actor.Gameworld.Add(new TraitExpression(newSkillModel.Expression, actor.Gameworld));
				var newSkill = new SkillDefinition(newSkillModel, actor.Gameworld);
				actor.Gameworld.Add(newSkill);
				newSkill.Initialise(newSkillModel);
				actor.RemoveAllEffects<BuilderEditingEffect<ISkillDefinition>>();
				actor.AddEffect(new BuilderEditingEffect<ISkillDefinition>(actor) { EditingItem = newSkill });
				actor.OutputHandler.Send(
					$"You create the {newSkill.Name.TitleCase().ColourName()} skill, which you are now editing.");
				return;
			}
		}

		if (actionText.EqualTo("close"))
		{
			var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<ISkillDefinition>>().FirstOrDefault();
			if (editing == null)
			{
				actor.OutputHandler.Send("You are not editing any skills.");
				return;
			}

			actor.RemoveEffect(editing);
			actor.OutputHandler.Send(
				$"You are no longer editing the {editing.EditingItem.Name.TitleCase().ColourName()} skill.");
			return;
		}

		if (actor.Gameworld.Traits.GetByIdOrName(actionText) is not ISkillDefinition skill)
		{
			actor.OutputHandler.Send("There is no such skill for you to edit.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<ISkillDefinition>>();
		actor.AddEffect(new BuilderEditingEffect<ISkillDefinition>(actor) { EditingItem = skill });
		actor.OutputHandler.Send($"You are now editing the {skill.Name.TitleCase().ColourName()} skill.");
	}

	[PlayerCommand("SetAttribute", "setattribute")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("setattribute",
		"This command allows you to set the value of an attribute on a character. The syntax is SETATTRIBUTE <target> <attribute> <value>.",
		AutoHelp.HelpArgOrNoArg)]
	protected static void SetAttribute(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var target = actor.TargetActor(ss.PopSpeech());
		if (target == null)
		{
			actor.OutputHandler.Send("You don't see anyone like that.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(new EmoteOutput(new Emote(
				$"Which attribute did you want to set a value for? $0 have|has the following: {target.TraitsOfType(TraitType.Attribute).Select(x => x.Definition.Name.ColourName()).ListToString()}.",
				target, target)));
			return;
		}

		var text = ss.PopSpeech();
		var attribute = target.TraitsOfType(TraitType.Attribute).Select(x => x.Definition)
		                      .FirstOrDefault(x =>
			                      x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
		if (attribute == null)
		{
			actor.OutputHandler.Send(new EmoteOutput(new Emote(
				$"$0 have|has no such attribute. $0 have|has the following: {target.TraitsOfType(TraitType.Attribute).Select(x => x.Definition.Name.ColourName()).ListToString()}.",
				target, target)));
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What value did you want to give to the attribute?");
			return;
		}

		if (!double.TryParse(ss.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send("You must enter a valid number.");
			return;
		}

		target.SetTraitValue(attribute, value);
		actor.OutputHandler.Send(
			$"You set the value of {target.HowSeen(actor, type: DescriptionType.Possessive)} {attribute.Name.ColourName()} attribute to {value.ToString("N2", actor).ColourValue()} ({attribute.Decorator.Decorate(value)})");
	}

	[PlayerCommand("Decay", "decay")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void Decay(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		double decay = 1000;
		if (!ss.IsFinished)
		{
			if (!double.TryParse(ss.Pop(), out decay))
			{
				actor.Send("How much decay do you want to add to corpses in this room?");
				return;
			}
		}

		foreach (var item in
		         actor.Location.LayerGameItems(actor.RoomLayer).SelectNotNull(x => x.GetItemType<ICorpse>()))
		{
			item.DecayPoints += decay;
		}

		foreach (var item in actor.Location.LayerGameItems(actor.RoomLayer)
		                          .SelectNotNull(x => x.GetItemType<ISeveredBodypart>()))
		{
			item.DecayPoints += decay;
		}

		actor.Send("You add {0:N} decay points to all corpses in the room.", decay);
	}

	[PlayerCommand("Kill", "kill")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void Kill(ICharacter actor, string input)
	{
		var ss = new StringStack(input);
		var cmd = ss.Pop();
		if (!cmd.Equals("kill", StringComparison.InvariantCultureIgnoreCase))
		{
			actor.Send("You must type out kill in its entirety, to avoid accidents.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("Who do you want to kill?");
			return;
		}

		var target = actor.TargetActor(ss.Pop());
		if (target == null)
		{
			actor.Send("There is no such person to kill.");
			return;
		}

		if (target == actor)
		{
			actor.Send("You cannot kill yourself.");
			return;
		}

		if (target.IsAdministrator())
		{
			actor.Send(StringUtilities.HMark + "You can't kill active admin avatars.");
			return;
		}

		target.Die();
	}

	[PlayerCommand("Resurrect", "resurrect")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void Resurrect(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.Send("Who do you want to resurrect?");
			return;
		}

		ICharacter character;
		var targetText = ss.PopSpeech();
		// Allow resurrection of offline PCs
		if (targetText[0] == '*')
		{
			if (!long.TryParse(targetText.Substring(1), out var value))
			{
				actor.Send(
					"You must specify a valid character ID to resurrect if you want to use this version of the command.");
				return;
			}

			character = actor.Gameworld.TryGetCharacter(value, true);
			if (character == null)
			{
				actor.OutputHandler.Send("There is no character with that ID.");
				return;
			}

			if (!character.State.HasFlag(CharacterState.Dead))
			{
				actor.OutputHandler.Send(
					$"{character.HowSeen(actor, true, flags: PerceiveIgnoreFlags.IgnoreSelf)} ({character.PersonalName.GetName(NameStyle.FullName)}) is not dead.");
				return;
			}

			character.Resurrect(actor.Location);

			// First, check to see if there are any corpses of this character already in the world
			foreach (var corpseitem in actor.Gameworld.Items.SelectNotNull(x => x.GetItemType<ICorpse>()).ToList())
			{
				if (corpseitem.OriginalCharacter.Id != value)
				{
					continue;
				}

				corpseitem.Parent.Delete();
			}
		}
		else
		{
			var item = actor.TargetItem(targetText);
			if (item == null)
			{
				actor.Send("There is no such corpse to resurrect.");
				return;
			}

			var corpse = item.GetItemType<ICorpse>();
			if (corpse == null)
			{
				actor.Send("{0} is not a corpse.", item.HowSeen(actor, true));
				return;
			}

			character = corpse.OriginalCharacter.Resurrect(actor.Location);
			item.Delete();
		}

		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ resurrect|resurrects $0.", actor, character),
			flags: OutputFlags.SuppressObscured));
		if (character.Account.Id != 0)
		{
			character.Quit(); //Unload PCs so they can log in w/o a clone.
		}

		var deathBoard = actor.Gameworld.Boards.Get(actor.Gameworld.GetStaticLong("DeathsBoardId"));
		if (deathBoard != null)
		{
			var deathProg = actor.Gameworld.FutureProgs.Get(actor.Gameworld.GetStaticLong("PostToDeathsProg"));
			if ((bool?)deathProg?.Execute(character) != false)
			{
				deathBoard.MakeNewPost(default(IAccount),
					$"{character.Id} - {character.PersonalName.GetName(NameStyle.FullWithNickname)} Resurrected by {actor.Account.Name.Proper()}",
					$"Character #{character.Id} ({character.PersonalName.GetName(NameStyle.FullWithNickname)}) was resurrected by {actor.Account.Name.Proper()}."
				);
			}
		}
	}

	[PlayerCommand("Possess", "possess")]
	[CommandPermission(PermissionLevel.Admin)]
	protected static void Possess(ICharacter actor, string input)
	{
		if (actor.EffectsOfType<Switched>().Any())
		{
			actor.Send("You are already possessing an NPC. You must return before you can possess another.");
			return;
		}

		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.Send("Who do you want to possess?");
			return;
		}

		var target = actor.TargetActor(ss.Pop());
		if (target == null)
		{
			actor.Send("You don't see anyone like that to possess.");
			return;
		}

		if (target == actor)
		{
			actor.Send("You cannot possess yourself.");
			return;
		}

		if (target.IsPlayerCharacter)
		{
			actor.Send("You can only possess NPCs.");
			return;
		}

		if (target.EffectsOfType<Switched>().Any())
		{
			actor.Send(
				$"{target.HowSeen(actor, true)} is already being possessed by someone. Only one possessor at a time.");
			return;
		}

		actor.OutputHandler.Send(
			$"You take control of {target.HowSeen(actor, type: DescriptionType.Possessive)} mind.");
		actor.Controller.SetContext(target);
		target.AddEffect(new Switched(target, actor));
		actor.SetNoControllerTags(" (switched)");
	}

	[PlayerCommand("GiveInvis", "giveinvis")]
	[CommandPermission(PermissionLevel.Admin)]
	protected static void GiveInvis(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var clear = false;
		if (ss.Peek().EqualTo("clear"))
		{
			clear = true;
			ss.Pop();
		}

		if (ss.IsFinished)
		{
			actor.Send(clear
				? "What do you want to clear invisibility from?"
				: "What do you want to give invisibility to?");
			return;
		}

		var target = actor.Target(ss.Pop());
		if (target == null)
		{
			actor.Send("You don't see anything like that.");
			return;
		}

		if (clear)
		{
			target.RemoveAllEffects(x => x.IsEffectType<Invis>());
			actor.Send($"You remove all invisibility effects from {target.HowSeen(actor)}.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send(
				"Which prog do you want to attach to this invisibility? It should return bool and accept two perceivables as parameters.");
			return;
		}

		var name = ss.SafeRemainingArgument;
		var prog = long.TryParse(name, out var value)
			? actor.Gameworld.FutureProgs.Get(value)
			: actor.Gameworld.FutureProgs.GetByName(name);
		if (prog == null)
		{
			actor.Send("There is no prog like that.");
			return;
		}

		if (prog.ReturnType != FutureProgVariableTypes.Boolean)
		{
			actor.Send("The prog you select must return boolean.");
			return;
		}

		if (!prog.MatchesParameters(new[]
		    {
			    FutureProgVariableTypes.Perceivable, FutureProgVariableTypes.Perceivable
		    }))
		{
			actor.Send("The prog must accept two perceivable parameters.");
			return;
		}

		actor.Send(
			$"You add invisibility to {target.HowSeen(actor)} with the prog {prog.FunctionName} (#{prog.Id}) controlling whether it applies.");
		target.AddEffect(new Invis(target, prog));
	}

	[PlayerCommand("GiveEmpathy", "giveempathy")]
	[CommandPermission(PermissionLevel.Admin)]
	protected static void GiveEmpathy(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Who do you want to give the gift of empathy to?");
			return;
		}

		var target = actor.TargetActor(ss.PopSpeech());
		if (target == null)
		{
			actor.OutputHandler.Send("You don't see anyone like that here.");
			return;
		}

		if (target.EffectsOfType<Empathy>().Any())
		{
			actor.OutputHandler.Send(
				$"{target.HowSeen(actor, true, flags: PerceiveIgnoreFlags.IgnoreSelf)} already has the gift of empathy.");
			return;
		}

		target.AddEffect(new Empathy(target));
		actor.OutputHandler.Send(
			$"You give {target.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreSelf)} the gift of empathy.");
	}

	[PlayerCommand("MeritSearch", "meritsearch")]
	[CommandPermission(PermissionLevel.Admin)]
	protected static void MeritSearch(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished || ss.Peek().EqualToAny("help", "?"))
		{
			actor.Send(
				$"The MeritSearch command is used to search for online character with particular merits or flaws. The syntax is {"meritsearch <merit1> <merit2> ...".ColourCommand()}. If you specify multiple merits and flaws, it will search for only those characters who have ALL of them.");
			return;
		}

		var merits = new List<IMerit>();
		while (!ss.IsFinished)
		{
			var merit = long.TryParse(ss.PopSpeech(), out var value)
				? actor.Gameworld.Merits.Get(value)
				: actor.Gameworld.Merits.GetByName(ss.Last);
			if (merit == null)
			{
				actor.Send($"There is no such merit or flaw as '{ss.Last}'.");
				return;
			}

			merits.Add(merit);
		}

		var characters = actor.Gameworld.Characters.Where(x => merits.All(y => x.Merits.Contains(y))).ToList();
		if (!characters.Any())
		{
			actor.Send(
				$"There aren't any online characters with all of the following merits and flaws: {merits.Select(x => x.Name.Colour(Telnet.Cyan)).ListToString()}");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine(
			$"The following online characters have the merits and flaws {merits.Select(x => x.Name.Colour(Telnet.Cyan)).ListToString()}:");
		foreach (var ch in characters)
		{
			sb.AppendLine($"\t{ch.HowSeen(actor)} ({ch.PersonalName.GetName(NameStyle.FullWithNickname)})");
		}

		actor.Send(sb.ToString());
	}

	[PlayerCommand("RoleSearch", "rolesearch")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("rolesearch", @"The Rolesearch command is used to search for online character with particular roles. The syntax is #3rolesearch <role1> <role2> ...#0. 

If you specify multiple roles, it will search for only those characters who have ALL of them.", AutoHelp.HelpArgOrNoArg)]
	protected static void RoleSearch(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished || ss.Peek().EqualToAny("help", "?"))
		{
			actor.Send(
				$"The RoleSearch command is used to search for online character with particular roles. The syntax is {"rolesearch <role1> <role2> ...".ColourCommand()}. If you specify multiple roles, it will search for only those characters who have ALL of them.");
			return;
		}

		var roles = new List<IChargenRole>();
		while (!ss.IsFinished)
		{
			var role = long.TryParse(ss.PopSpeech(), out var value)
				? actor.Gameworld.Roles.Get(value)
				: actor.Gameworld.Roles.GetByName(ss.Last);
			if (role == null)
			{
				actor.Send($"There is no such role as '{ss.Last}'.");
				return;
			}

			roles.Add(role);
		}

		var characters = actor.Gameworld.Characters.Where(x => roles.All(y => x.Roles.Contains(y))).ToList();
		if (!characters.Any())
		{
			actor.Send(
				$"There aren't any online characters with all of the roles {roles.Select(x => x.Name.Colour(Telnet.Cyan)).ListToString()}");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine(
			$"The following online characters have the roles {roles.Select(x => x.Name.Colour(Telnet.Cyan)).ListToString()}:");
		foreach (var ch in characters)
		{
			sb.AppendLine($"\t{ch.HowSeen(actor)} ({ch.PersonalName.GetName(NameStyle.FullWithNickname)})");
		}

		actor.Send(sb.ToString());
	}

	[PlayerCommand("LoadPC", "loadpc")]
	[CommandPermission(PermissionLevel.SeniorAdmin)]
	protected static void LoadPC(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which PC do you want to load?");
			return;
		}

		if (!long.TryParse(ss.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send("You must enter the ID number of a character that you want to load.");
			return;
		}

		var character = actor.Gameworld.TryGetCharacter(value, true);
		if (character == null)
		{
			actor.OutputHandler.Send("There is no such character to load.");
			return;
		}

		if (!character.IsPlayerCharacter || character.IsGuest)
		{
			actor.OutputHandler.Send("You cannot use this command to load guests or NPCs.");
			return;
		}

		if (actor.Gameworld.Characters.Has(character))
		{
			actor.OutputHandler.Send(
				$"{character.HowSeen(actor, true, flags: PerceiveIgnoreFlags.IgnoreSelf)} ({character.PersonalName.GetName(NameStyle.FullName)}) is already in the gameworld at {character.Location.HowSeen(actor)} (#{character.Location.Id.ToString("N0", actor)}), so cannot be loaded.");
			return;
		}

		actor.Gameworld.SystemMessage(
			new EmoteOutput(new Emote(
				$"@ offline load|loads $0 ({character.PersonalName.GetName(NameStyle.FullName)}, account {character.Account.Name.Proper()})",
				actor, character)), true);
		character.Register(new NonPlayerOutputHandler());
		actor.Gameworld.Add(character, false);
		var controller = new NPCController();
		controller.UpdateControlFocus(character);
		character.SilentAssumeControl(controller);
		actor.Location.Login(character);
	}

	[PlayerCommand("OverrideDesc", "overridedesc")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[HelpInfo("overridedesc",
		"This command allows you to set an override effect on a target that sets their sdesc or desc to something else. The syntax is OVERRIDEDESC <target> sdesc|desc|clear \"<description>\"",
		AutoHelp.HelpArgOrNoArg)]
	protected static void OverrideDesc(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var target = actor.Target(ss.PopSpeech());
		if (target == null)
		{
			actor.OutputHandler.Send("You don't see anything like that.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("You must specify SDESC, DESC or CLEAR as the second argument.");
			return;
		}

		DescriptionType type;
		switch (ss.Pop().ToLowerInvariant())
		{
			case "desc":
				type = DescriptionType.Full;
				break;
			case "sdesc":
				type = DescriptionType.Short;
				break;
			case "clear":
				if (target is ICharacter tch)
				{
					tch.Body.RemoveAllEffects(x => x.IsEffectType<StorytellerDescOverride>());
				}
				else
				{
					target.RemoveAllEffects(x => x.IsEffectType<StorytellerDescOverride>());
				}

				actor.OutputHandler.Send(
					$"{target.HowSeen(actor, true, flags: PerceiveIgnoreFlags.IgnoreSelf)} is now back to {target.ApparentGender(actor).Possessive()} regular descriptions.");
				return;
			default:
				actor.OutputHandler.Send("You must specify SDESC, DESC or CLEAR as the second argument.");
				return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What custom override description do you want to set?");
			return;
		}

		var olddesc = target.HowSeen(actor, true, type, flags: PerceiveIgnoreFlags.IgnoreSelf);
		if (target is ICharacter ch)
		{
			ch.Body.RemoveAllEffects(x => x.GetSubtype<StorytellerDescOverride>()?.OverridenType == type);
			ch.Body.AddEffect(new StorytellerDescOverride(target, type, ss.PopSpeech()));
		}
		else
		{
			target.RemoveAllEffects(x => x.GetSubtype<StorytellerDescOverride>()?.OverridenType == type);
			target.AddEffect(new StorytellerDescOverride(target, type, ss.PopSpeech()));
		}

		if (type == DescriptionType.Short)
		{
			actor.OutputHandler.Send(
				$"You override the short description of {olddesc} to {target.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreSelf)}");
		}
		else
		{
			actor.OutputHandler.Send(
				$"You override the full description of {target.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreSelf)} to {ss.Last.Fullstop().ColourCommand()}");
		}
	}

	[PlayerCommand("Rename", "rename")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[HelpInfo("rename",
		"This command allows you to override a character's true personal name. The syntax is RENAME <target>|<ID> <new name>",
		AutoHelp.HelpArgOrNoArg)]
	protected static void Rename(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var targetText = ss.PopSpeech();
		ICharacter target = null;
		if (long.TryParse(targetText, out var value))
		{
			target = actor.Gameworld.TryGetCharacter(value, true);
		}
		else
		{
			target = actor.TargetActor(targetText);
		}

		if (target == null)
		{
			actor.OutputHandler.Send("There is no character like that to rename.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What should their new name be?");
			return;
		}

		var newName = target.Culture.NameCultureForGender(target.Gender.Enum)
		                    .GetPersonalName(ss.SafeRemainingArgument, true);
		if (newName == null)
		{
			actor.OutputHandler.Send("That is not a valid name for their naming culture.");
			return;
		}

		target.PersonalName = newName;
		actor.OutputHandler.Send(
			$"You rename {target.HowSeen(actor)} to {newName.GetName(NameStyle.FullWithNickname)}.");
	}

	[PlayerCommand("Redesc", "redesc")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[HelpInfo("redesc",
		"This command allows you to edit the description of a character. Simply use REDESC <Target>.",
		AutoHelp.HelpArgOrNoArg)]
	protected static void Redesc(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var target = actor.TargetActorOrCorpse(ss.PopSpeech());
		if (target is null)
		{
			actor.OutputHandler.Send("You don't see anyone like that.");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Editing the description for {target.HowSeen(actor)}.");
		sb.AppendLine();
		sb.AppendLine("Replacing:\n");
		sb.AppendLine(target.Body.GetRawDescriptions.FullDescription.Wrap(actor.InnerLineFormatLength, "\t").ColourCommand());
		sb.AppendLine();
		sb.AppendLine("You can use the following variables in the markup:");
		sb.AppendLine();
		sb.AppendLine("\t#6&a_an[#3your content#6]#0 - puts 'a' or 'an' and a space in front of your content as appropriate".SubstituteANSIColour());
		sb.AppendLine("\t#6&?a_an[#3your content#6]#0 - puts 'a' or 'an' and a space or nothing in front of your content based on pluralisation".SubstituteANSIColour());
		sb.AppendLine("\t#6&pronoun|#3plural text#6|#3singular text#6&#0 - alternates text based on the grammatical number of the pronoun (e.g. he/she/it vs you/they/them)".SubstituteANSIColour());
		sb.AppendLine("\t#6&he#0 - he/she/it/they/you as appropriate".SubstituteANSIColour());
		sb.AppendLine("\t#6&him#0 - him/her/it/them/you as appropriate".SubstituteANSIColour());
		sb.AppendLine("\t#6&his#0 - his/her/its/theirs/your as appropriate".SubstituteANSIColour());
		sb.AppendLine("\t#6&himself#0 - himself/herself/itself/themself/yourself as appropriate".SubstituteANSIColour());
		sb.AppendLine("\t#6&male#0 - male/female/neuter/non-binary as appropriate".SubstituteANSIColour());
		sb.AppendLine($"\t#6&race#0 - the name of the character's race ({target.Race.Name.ToLowerInvariant().ColourValue()})".SubstituteANSIColour());
		sb.AppendLine($"\t#6&culture#0 - the name of the character's culture ({target.Culture.Name.ToLowerInvariant().ColourValue()})".SubstituteANSIColour());
		sb.AppendLine($"\t#6&ethnicity#0 - the name of the character's ethnicity ({target.Ethnicity.Name.ToLowerInvariant().ColourValue()})".SubstituteANSIColour());
		sb.AppendLine($"\t#6&ethnicgroup#0 - the character's ethnic group ({target.Ethnicity.EthnicGroup?.ToLowerInvariant().ColourValue() ?? ""})".SubstituteANSIColour());
		sb.AppendLine($"\t#6&personword#0 - the character's culture's person word ({target.Culture.PersonWord(target.Gender.Enum)?.ToLowerInvariant().ColourValue() ?? ""})".SubstituteANSIColour());
		sb.AppendLine($"\t#6&age#0 - the character's age category ({target.Race.AgeCategory(target).DescribeEnum(true).ToLowerInvariant().ColourValue()})".SubstituteANSIColour());
		sb.AppendLine($"\t#6&height#0 - the character's height ({actor.Gameworld.UnitManager.Describe(target.Height, UnitType.Length, actor).ColourValue()})".SubstituteANSIColour());
		sb.AppendLine($"\t#6&tattoos#0 - a description of the character's tattoos, or blank if none ({target.ParseCharacteristics("&tattoos", actor, true).ColourValue()})".SubstituteANSIColour());
		sb.AppendLine($"\t#6&withtattoos#0 - an alternate description of the character's tattoos, or blank if none ({target.ParseCharacteristics("&withtattoos", actor, true).ColourValue()})".SubstituteANSIColour());
		sb.AppendLine($"\t#6&scars#0 - a description of the character's scars, or blank if none ({target.ParseCharacteristics("&scars", actor, true).ColourValue()})".SubstituteANSIColour());
		sb.AppendLine($"\t#6&withscars#0 - an alternate description of the character's scars, or blank if none ({target.ParseCharacteristics("&withscars", actor, true).ColourValue()})".SubstituteANSIColour());
		sb.AppendLine();
		sb.AppendLine("You can also use the following forms with each characteristic:");
		sb.AppendLine();
		sb.AppendLine($@"	#6$varname#0 - Displays the ordinary form of the variable. See below for specifics.
	#6$varnamefancy#0 - Displays the fancy form of the variable. See below for specifics.
	#6$varnamebasic#0 - Displays the basic form of the variable. See below for specifics.
	#6$varname[#3display if not obscured#6][#3display if obscured#6]#0 - See below for specifics.
	#6$?varname[#3display if not null/default#6][#3display if null/default#6]#0 - See below for specifics.

#1Note: Inside the above, @ substitutes the variable description, $ substitutes for the name of the obscuring item (e.g. sunglasses), and * substitutes for the sdesc of the obscuring item (e.g. a pair of gold-rimmed sunglasses).#0".SubstituteANSIColour());
		sb.AppendLine();
		sb.AppendLine($"The following variables can be used with the above syntax: {target.CharacteristicDefinitions.Select(x => x.Pattern.TransformRegexIntoPattern().ColourName()).ListToCommaSeparatedValues(", ")}");
		var partCharacteristics = target.CharacteristicDefinitions.OfType<IBodypartSpecificCharacteristicDefinition>().ToList();
		if (partCharacteristics.Any())
		{
			sb.AppendLine();
			sb.AppendLine($"The {"characteristic".Pluralise(partCharacteristics.Count != 1)} {partCharacteristics.Select(x => x.Pattern.TransformRegexIntoPattern().ColourName()).ListToString()} can be used with an additional form. Inside this form, you can use @ for the variable description, % for the number of parts possessed, * for the same but in wordy form:");
			sb.AppendLine();
			sb.AppendLine($"\t#6%varname[text if normal count][n-n2:text if between n and n2 count][x:text if x count][y:text if y count]#0".SubstituteANSIColour());
			sb.AppendLine($"\tExample: #3%$eyecolour[%eyecolour[$eyecolour eyes][1:one $eyecolour eye][0:no eyes]#0".SubstituteANSIColour());
		}
		sb.AppendLine();
		sb.AppendLine("Enter the description in the editor below.");
		sb.AppendLine();
		actor.OutputHandler.Send(sb.ToString());
		actor.EditorMode(postAction: RedescPostAction, RedescCancelAction, 1.0, null, EditorOptions.None, new object[] { target, actor });
	}

	private static void RedescCancelAction(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to change the description.");
	}

	private static void RedescPostAction(string text, IOutputHandler handler, object[] args)
	{
		var target = (ICharacter)args[0];
		var actor = (ICharacter)args[1];
		target.Body.SetFullDescription(text);
		handler.Send($"You change the description of {target.HowSeen(actor)} to:\n\n{text.ColourCommand()}");
	}

	[PlayerCommand("Resdesc", "resdesc")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[HelpInfo("resdesc",
		"This command allows you to edit the short description of a character. Simply use RESDESC <Target>.",
		AutoHelp.HelpArgOrNoArg)]
	protected static void Resdesc(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var target = actor.TargetActorOrCorpse(ss.PopSpeech());
		if (target is null)
		{
			actor.OutputHandler.Send("You don't see anyone like that.");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Editing the short description for {target.HowSeen(actor)}.");
		sb.AppendLine();
		sb.AppendLine("Replacing:\n");
		sb.AppendLine(target.Body.GetRawDescriptions.ShortDescription.ColourCommand());
		sb.AppendLine();
		sb.AppendLine("You can use the following variables in the markup:");
		sb.AppendLine();
		sb.AppendLine("\t#6&a_an[#3your content#6]#0 - puts 'a' or 'an' and a space in front of your content as appropriate".SubstituteANSIColour());
		sb.AppendLine("\t#6&?a_an[#3your content#6]#0 - puts 'a' or 'an' and a space or nothing in front of your content based on pluralisation".SubstituteANSIColour());
		sb.AppendLine("\t#6&pronoun|#3plural text#6|#3singular text#6&#0 - alternates text based on the grammatical number of the pronoun (e.g. he/she/it vs you/they/them)".SubstituteANSIColour());
		sb.AppendLine("\t#6&he#0 - he/she/it/they/you as appropriate".SubstituteANSIColour());
		sb.AppendLine("\t#6&him#0 - him/her/it/them/you as appropriate".SubstituteANSIColour());
		sb.AppendLine("\t#6&his#0 - his/her/its/theirs/your as appropriate".SubstituteANSIColour());
		sb.AppendLine("\t#6&himself#0 - himself/herself/itself/themself/yourself as appropriate".SubstituteANSIColour());
		sb.AppendLine("\t#6&male#0 - male/female/neuter/non-binary as appropriate".SubstituteANSIColour());
		sb.AppendLine($"\t#6&race#0 - the name of the character's race ({target.Race.Name.ToLowerInvariant().ColourValue()})".SubstituteANSIColour());
		sb.AppendLine($"\t#6&culture#0 - the name of the character's culture ({target.Culture.Name.ToLowerInvariant().ColourValue()})".SubstituteANSIColour());
		sb.AppendLine($"\t#6&ethnicity#0 - the name of the character's ethnicity ({target.Ethnicity.Name.ToLowerInvariant().ColourValue()})".SubstituteANSIColour());
		sb.AppendLine($"\t#6&ethnicgroup#0 - the character's ethnic group ({target.Ethnicity.EthnicGroup?.ToLowerInvariant().ColourValue() ?? ""})".SubstituteANSIColour());
		sb.AppendLine($"\t#6&personword#0 - the character's culture's person word ({target.Culture.PersonWord(target.Gender.Enum)?.ToLowerInvariant().ColourValue() ?? ""})".SubstituteANSIColour());
		sb.AppendLine($"\t#6&age#0 - the character's age category ({target.Race.AgeCategory(target).DescribeEnum(true).ToLowerInvariant().ColourValue()})".SubstituteANSIColour());
		sb.AppendLine($"\t#6&height#0 - the character's height ({actor.Gameworld.UnitManager.Describe(target.Height, UnitType.Length, actor).ColourValue()})".SubstituteANSIColour());
		sb.AppendLine($"\t#6&tattoos#0 - a description of the character's tattoos, or blank if none ({target.ParseCharacteristics("&tattoos", actor, true).ColourValue()})".SubstituteANSIColour());
		sb.AppendLine($"\t#6&withtattoos#0 - an alternate description of the character's tattoos, or blank if none ({target.ParseCharacteristics("&withtattoos", actor, true).ColourValue()})".SubstituteANSIColour());
		sb.AppendLine($"\t#6&scars#0 - a description of the character's scars, or blank if none ({target.ParseCharacteristics("&scars", actor, true).ColourValue()})".SubstituteANSIColour());
		sb.AppendLine($"\t#6&withscars#0 - an alternate description of the character's scars, or blank if none ({target.ParseCharacteristics("&withscars", actor, true).ColourValue()})".SubstituteANSIColour());
		sb.AppendLine();
		sb.AppendLine("You can also use the following forms with each characteristic:");
		sb.AppendLine();
		sb.AppendLine($@"	#6$varname#0 - Displays the ordinary form of the variable. See below for specifics.
	#6$varnamefancy#0 - Displays the fancy form of the variable. See below for specifics.
	#6$varnamebasic#0 - Displays the basic form of the variable. See below for specifics.
	#6$varname[#3display if not obscured#6][#3display if obscured#6]#0 - See below for specifics.
	#6$?varname[#3display if not null/default#6][#3display if null/default#6]#0 - See below for specifics.

#1Note: Inside the above, @ substitutes the variable description, $ substitutes for the name of the obscuring item (e.g. sunglasses), and * substitutes for the sdesc of the obscuring item (e.g. a pair of gold-rimmed sunglasses).#0".SubstituteANSIColour());
		sb.AppendLine();
		sb.AppendLine($"The following variables can be used with the above syntax: {target.CharacteristicDefinitions.Select(x => x.Pattern.TransformRegexIntoPattern().ColourName()).ListToCommaSeparatedValues(", ")}");
		var partCharacteristics = target.CharacteristicDefinitions.OfType<IBodypartSpecificCharacteristicDefinition>().ToList();
		if (partCharacteristics.Any())
		{
			sb.AppendLine();
			sb.AppendLine($"The {"characteristic".Pluralise(partCharacteristics.Count != 1)} {partCharacteristics.Select(x => x.Pattern.TransformRegexIntoPattern().ColourName()).ListToString()} can be used with an additional form. Inside this form, you can use @ for the variable description, % for the number of parts possessed, * for the same but in wordy form:");
			sb.AppendLine();
			sb.AppendLine($"\t#6%varname[text if normal count][n-n2:text if between n and n2 count][x:text if x count][y:text if y count]#0".SubstituteANSIColour());
			sb.AppendLine($"\tExample: #3%$eyecolour[%eyecolour[$eyecolour eyes][1:one $eyecolour eye][0:no eyes]#0".SubstituteANSIColour());
		}
		sb.AppendLine();
		sb.AppendLine("Enter the description in the editor below.");
		sb.AppendLine();
		actor.OutputHandler.Send(sb.ToString());
		actor.EditorMode(postAction: ResdescPostAction, ResdescCancelAction, 1.0, null, EditorOptions.None, new object[] { target, actor, target.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreSelf) });
	}

	private static void ResdescCancelAction(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to change the short description.");
	}

	private static void ResdescPostAction(string text, IOutputHandler handler, object[] args)
	{
		var target = (ICharacter)args[0];
		var actor = (ICharacter)args[1];
		var old = (string)args[2];
		target.Body.SetShortDescription(text);
		handler.Send($"You change the short description of {old} to {target.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreSelf)}.");
	}

	[PlayerCommand("Sniff", "sniff")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("sniff",
		"This command allows you to see various debug info about characters, items, rooms, and directions.\nSyntax: sniff here|*dirn|<item>|<ch> [<subitem>]",
		AutoHelp.HelpArgOrNoArg)]
	protected static void Sniff(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.Peek().EqualTo("here"))
		{
			SniffRoom(actor);
			return;
		}

		if (ss.Peek()[0] == '*')
		{
			var targetExit = actor.Location.GetExitKeyword(ss.Pop().RemoveFirstCharacter(), actor);
			if (targetExit == null)
			{
				actor.Send("There is no such exit for you to sniff.");
				return;
			}

			SniffExit(actor, targetExit, ss);
			return;
		}

		var target = actor.Target(ss.Pop());
		if (target == null)
		{
			actor.Send("There is nothing like that for you to sniff.");
			return;
		}

		if (target is ICharacter ch)
		{
			SniffCharacter(actor, ch, ss);
			return;
		}

		if (target is IGameItem gi)
		{
			SniffGameItem(actor, gi, ss);
			return;
		}

		throw new NotImplementedException("Unknown target type in sniff.");
	}

	private static void SniffGameItem(ICharacter actor, IGameItem gi, StringStack ss)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Sniffing Item {gi.Id.ToString("N0", actor)}...");
		sb.AppendLine();
		sb.AppendLine($"Desc: {gi.HowSeen(actor)}");
		sb.AppendLine(
			$"Proto: {$"{gi.Prototype.Id.ToString("N0", actor)}r{gi.Prototype.RevisionNumber.ToString("N0", actor)}".ColourValue()}");
		sb.AppendLine($"Quality: {gi.Quality.Describe().ColourValue()}");
		sb.AppendLine($"Size: {gi.Size.Describe().ColourValue()}");
		sb.AppendLine($"Material: {gi.Material.Name.ColourValue()}");
		sb.AppendLine(
			$"Weight: {actor.Gameworld.UnitManager.DescribeMostSignificantExact(gi.Weight, Framework.Units.UnitType.Mass, actor).ColourValue()}");
		sb.AppendLine($"Quantity: {gi.Quantity.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Condition: {gi.Condition.ToString("P2", actor).ColourValue()}");
		sb.AppendLine($"Position: {gi.PositionState.GetType().Name.ColourValue()}");
		sb.AppendLine();
		sb.AppendLine($"InInventoryOf: {gi.InInventoryOf?.Actor.HowSeen(actor) ?? "Noone".Colour(Telnet.Red)}");
		sb.AppendLine($"ContainedIn: {gi.ContainedIn?.HowSeen(actor) ?? "Nothing".Colour(Telnet.Red)}");
		var fluid = gi.TrueLocations.FirstOrDefault()?.Terrain(gi).WaterFluid ?? actor.Gameworld.Liquids.First();
		sb.AppendLine($"Buoyancy: {gi.Buoyancy(fluid.Density).ToString("N0", actor).ColourValue()}");
		if (gi.MorphTime != default)
		{
			sb.AppendLine($"Morphs In: {(gi.MorphTime - DateTime.UtcNow).Describe(actor).ColourValue()}");
			sb.AppendLine($"Morph Time: {gi.MorphTime.GetLocalDateString(actor).ColourValue()}");
		}

		sb.AppendLine();
		sb.AppendLine("Components:");
		foreach (var component in gi.Components)
		{
			sb.AppendLine(
				$"\t##{component.Id.ToString("N0", actor)} (proto #2{component.Prototype.Id.ToString("N0", actor)}r{component.Prototype.RevisionNumber.ToString("N0", actor)}#0) - #5{component.Prototype.Name}#0".SubstituteANSIColour());
		}

		sb.AppendLine();
		sb.AppendLine($"Attached Items:");
		foreach (var item in gi.AttachedAndConnectedItems)
		{
			sb.AppendLine($"\tItem #{item.Id.ToString("N0", actor)} - {item.HowSeen(actor)}");
		}

		sb.AppendLine();
		sb.AppendLine("Effects:");
		foreach (var effect in gi.Effects)
		{
			sb.AppendLine($"\t{effect.Describe(actor)}");
		}

		sb.AppendLine();
		sb.AppendLine("Variables:");
		foreach (var variable in actor.Gameworld.VariableRegister.AllVariables(FutureProgVariableTypes.Item))
		{
			var value = actor.Gameworld.VariableRegister.GetValue(gi, variable.Item1);
			sb.AppendLine(
				$"\t{variable.Item2.Describe().Colour(Telnet.Cyan)} {variable.Item1.Colour(Telnet.BoldWhite)}: {FutureProg.FutureProg.VariableValueToText(value, actor)}");
		}

		actor.Send(sb.ToString());
	}

	private static void SniffCharacter(ICharacter actor, ICharacter ch, StringStack ss)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Sniffing Character #{ch.Id.ToString("N0", actor)}...");
		sb.AppendLine($"True Name: {ch.PersonalName.GetName(NameStyle.FullWithNickname).Colour(Telnet.Green)}");
		sb.AppendLine($"Current Name: {ch.CurrentName.GetName(NameStyle.FullWithNickname).Colour(Telnet.Green)}");
		sb.AppendLine(
			$"Aliases: {ch.Aliases.Where(x => x != ch.CurrentName).Select(x => x.GetName(NameStyle.FullWithNickname).Colour(Telnet.Green)).ListToString()}");
		sb.AppendLine($"Description: {ch.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee)}");
		sb.AppendLine($"Account: {ch.Account.Name.Colour(Telnet.Green)}");
		sb.AppendLine($"IsHelpless: {ch.IsHelpless.ToString(actor).Colour(Telnet.Green)}");
		sb.AppendLine($"State: {ch.State.Describe().Colour(Telnet.Cyan)}");
		sb.AppendLine($"Status: {ch.Status.Describe().Colour(Telnet.Cyan)}");
		sb.AppendLine($"Breathing: {ch.BreathingStrategy.GetType().Name.Colour(Telnet.Cyan)}");
		sb.AppendLine(
			$"Burden: Offense[{ch.CombatBurdenOffense.ToString("N2", actor).Colour(Telnet.Green)}]\tDefense[{ch.CombatBurdenDefense.ToString("N2", actor).Colour(Telnet.Green)}]");
		sb.AppendLine(
			$"Advantage: Offense[{ch.OffensiveAdvantage.ToString("N2", actor).Colour(Telnet.Green)}]\tDefense[{ch.DefensiveAdvantage.ToString("N2", actor).Colour(Telnet.Green)}]");
		var fluid = ch.Location.Terrain(ch).WaterFluid ?? actor.Gameworld.Liquids.First();
		sb.AppendLine(
			$"Inventory Buoyancy: {ch.Body.AllItems.Sum(x => x.Buoyancy(fluid.Density)).ToString("N0", actor).ColourValue()}");
		sb.AppendLine();
		sb.AppendLine("Effects:");
		foreach (var effect in ch.Effects)
		{
			sb.AppendLine($"\t{effect.Describe(actor)}");
		}

		sb.AppendLine();
		sb.AppendLine("Variables:");
		foreach (var variable in actor.Gameworld.VariableRegister.AllVariables(FutureProgVariableTypes.Character))
		{
			var value = actor.Gameworld.VariableRegister.GetValue(ch, variable.Item1);
			sb.AppendLine(
				$"\t{variable.Item2.Describe().Colour(Telnet.Cyan)} {variable.Item1.Colour(Telnet.BoldWhite)}: {FutureProg.FutureProg.VariableValueToText(value, actor)}");
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	private static void SniffExit(ICharacter actor, ICellExit exit, StringStack ss)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Sniffing Exit {exit.Exit.Id.ToString("N0", actor).Colour(Telnet.Green)}...");
		sb.AppendLine(
			$"Origin: {exit.Origin.HowSeen(actor)} (#{exit.Origin.Id.ToString("N0", actor).Colour(Telnet.Green)})");
		sb.AppendLine(
			$"Destination: {exit.Destination.HowSeen(actor)} (#{exit.Destination.Id.ToString("N0", actor).Colour(Telnet.Green)})");
		sb.AppendLine($"Outbound Direction: {exit.OutboundDirection.Describe().Colour(Telnet.Yellow)}");
		sb.AppendLine($"Outbound Description: {exit.OutboundDirectionDescription.Colour(Telnet.Yellow)}");
		sb.AppendLine($"Outbound Suffix: {exit.OutboundDirectionSuffix.Colour(Telnet.Yellow)}");
		sb.AppendLine($"Outbound Movement: {exit.OutboundMovementSuffix.Colour(Telnet.Yellow)}");
		sb.AppendLine($"Inbound Direction: {exit.InboundDirection.Describe().Colour(Telnet.Yellow)}");
		sb.AppendLine($"Inbound Suffix: {exit.InboundDirectionSuffix.Colour(Telnet.Yellow)}");
		sb.AppendLine($"Inbound Movement: {exit.InboundMovementSuffix.Colour(Telnet.Yellow)}");
		sb.AppendLine();
		sb.AppendLine($"Accepts Door: {exit.Exit.AcceptsDoor.ToString(actor).Colour(Telnet.Cyan)}");
		sb.AppendLine($"Door Size: {exit.Exit.DoorSize.Describe().Colour(Telnet.Cyan)}");
		sb.AppendLine($"Door: {exit.Exit.Door?.Parent.HowSeen(actor) ?? "None"}");
		sb.AppendLine($"Max Size to Enter: {exit.Exit.MaximumSizeToEnter.Describe().Colour(Telnet.Cyan)}");
		sb.AppendLine(
			$"Max Size to Enter Upright: {exit.Exit.MaximumSizeToEnterUpright.Describe().Colour(Telnet.Cyan)}");
		sb.AppendLine();
		sb.AppendLine("Effects:");
		foreach (var effect in exit.Exit.Effects)
		{
			sb.AppendLine($"\t{effect.Describe(actor)}");
		}

		sb.AppendLine();
		sb.AppendLine("Variables:");
		foreach (var variable in actor.Gameworld.VariableRegister.AllVariables(FutureProgVariableTypes.Exit))
		{
			var value = actor.Gameworld.VariableRegister.GetValue(exit, variable.Item1);
			sb.AppendLine(
				$"\t{variable.Item2.Describe().Colour(Telnet.Cyan)} {variable.Item1.Colour(Telnet.BoldWhite)}: {FutureProg.FutureProg.VariableValueToText(value, actor)}");
		}

		actor.Send(sb.ToString());
	}

	private static void SniffRoom(ICharacter actor)
	{
		var cell = actor.Location;
		var sb = new StringBuilder();
		sb.AppendLine($"Sniffing Cell {cell.Id}...");
		sb.AppendLine(
			$"Room: {cell.Room.Id} - Zone: {cell.Room.Zone.Name} ({cell.Room.Zone.Id}) - Shard: {cell.Room.Zone.Shard.Name} ({cell.Room.Zone.Shard.Id})");
		sb.AppendLine(
			$"Current Overlay: {cell.CurrentOverlay.Id} from package {cell.CurrentOverlay.Package.Name.Colour(Telnet.Green)} ({cell.CurrentOverlay.Package.Id}r{cell.CurrentOverlay.Package.RevisionNumber})");
		sb.AppendLine("All overlays:");
		foreach (var overlay in cell.Overlays)
		{
			sb.AppendLine(
				$"\t{overlay.Id} - {overlay.Name.Colour(Telnet.Cyan)} - package {overlay.Package.Name.Colour(Telnet.Green)} ({overlay.Package.Id}r{overlay.Package.RevisionNumber})");
		}

		sb.AppendLine();
		sb.AppendLine("Exits:");
		foreach (var exit in actor.Gameworld.ExitManager.GetAllExits(cell)
		                          .OrderByDescending(x => cell.CurrentOverlay.ExitIDs.Contains(x.Exit.Id)))
		{
			sb.AppendLine(
				$"\t{exit.Exit.Id} - {exit.OutboundDirectionDescription} to {exit.Destination.CurrentOverlay.Name} ({exit.Destination.Id})");
			if (exit.Exit.AcceptsDoor)
			{
				if (exit.Exit.Door == null)
				{
					sb.AppendLine($"\t\tAccepts {exit.Exit.DoorSize.Describe()} door");
				}
				else
				{
					sb.AppendLine($"\t\tInstalled Door {exit.Exit.Door.Parent.HowSeen(actor)}");
				}
			}
		}

		sb.AppendLine();
		sb.AppendLine("Tags:");
		foreach (var tag in cell.Tags)
		{
			sb.AppendLine($"\t[{tag.Id.ToString("N0", actor)}] {tag.FullName.Colour(Telnet.Cyan)}");
		}

		sb.AppendLine();
		sb.AppendLine("Effects:");
		foreach (var effect in cell.Effects)
		{
			sb.AppendLine($"\t{effect.Describe(actor)}");
		}

		sb.AppendLine();
		sb.AppendLine("Variables:");
		foreach (var variable in actor.Gameworld.VariableRegister.AllVariables(FutureProgVariableTypes.Location))
		{
			var value = actor.Gameworld.VariableRegister.GetValue(cell, variable.Item1);
			sb.AppendLine(
				$"\t{variable.Item2.Describe().Colour(Telnet.Cyan)} {variable.Item1.Colour(Telnet.BoldWhite)}: {FutureProg.FutureProg.VariableValueToText(value, actor)}");
		}

		actor.Send(sb.ToString());
	}

	[PlayerCommand("LocateItem", "locateitem", "li")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void LocateItem(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				"What keyword, item ID (prefixed by !) or item prototype ID (prefixed by *) do you want to search for?");
			return;
		}

		var text = ss.PopSpeech();
		List<IGameItem> items;
		if (text[0] == '*')
		{
			if (!long.TryParse(text.Substring(1), out var value))
			{
				actor.OutputHandler.Send("That is not a valid ID.");
				return;
			}

			items = actor.Gameworld.Items.Where(x => x.Prototype.Id == value).ToList();
		}
		else if (text[0] == '!')
		{
			if (!long.TryParse(text.Substring(1), out var value))
			{
				actor.OutputHandler.Send("That is not a valid ID.");
				return;
			}

			items = actor.Gameworld.Items.Where(x => x.Id == value).ToList();
		}
		else
		{
			var split = text.Split('.');
			// The early ToList() in the next query is necessary because this is one of the few cases in the code where the entire Gameworld.Items is queried as an 
			// IEnumerable and HasKeyword can cause some items to initialise themselves (such as corpses) that modify the list of items loaded into the world
			items = actor.Gameworld.Items.ToList().Where(x => split.All(y => x.HasKeyword(y, actor, true))).ToList();
		}

		if (!items.Any())
		{
			actor.OutputHandler.Send("You don't find any items like that.");
			return;
		}

		items = items.OrderBy(x => x.TrueLocations.FirstOrDefault()?.Id ?? 0).ToList();

		var sb = new StringBuilder();
		foreach (var item in items)
		{
			var location = item.TrueLocations.FirstOrDefault();
			if (item.ContainedIn != null)
			{
				sb.AppendLine(
					$"[#{item.Id.ToString("N0", actor)}] {{{item.Prototype.Id.ToString("N0", actor)}r{item.Prototype.RevisionNumber.ToString("N0", actor)}}} {item.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee)} - {location?.HowSeen(actor) ?? "Nowhere".Colour(Telnet.Red)}{(location != null ? $" ({location.Id})" : "")} [contained: {item.ContainedIn.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee)}]");
			}
			else if (item.InInventoryOf != null)
			{
				sb.AppendLine(
					$"[#{item.Id.ToString("N0", actor)}] {{{item.Prototype.Id.ToString("N0", actor)}r{item.Prototype.RevisionNumber.ToString("N0", actor)}}} {item.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee)} - {location?.HowSeen(actor) ?? "Nowhere".Colour(Telnet.Red)}{(location != null ? $" ({location.Id})" : "")} [inventory: {item.InInventoryOf.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee)}]");
			}
			else
			{
				sb.AppendLine(
					$"[#{item.Id.ToString("N0", actor)}] {{{item.Prototype.Id.ToString("N0", actor)}r{item.Prototype.RevisionNumber.ToString("N0", actor)}}} {item.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee)} - {location?.HowSeen(actor) ?? "Nowhere".Colour(Telnet.Red)}{(location != null ? $" ({location.Id})" : "")}");
			}
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	[PlayerCommand("Drawings", "drawings")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[HelpInfo("drawings",
		"This command allows you to see all drawings in the game and search through them. You can specify multiple search arguments after the DRAWINGS keyword. Keep in mind that anything that touches the author (whether their ID, Name or Account) will force all the drawings to load their characters, and this will make the command very slow the first time you do it. As such, if you can possibly filter by other things first then that would be advisable.\n\nThe options you can use are as follows:\n\tby <id> - shows drawings by a character with specified ID\n\tby *<accountname> - shows all drawings by a character with specified account name\n\tby \"full name\" - searches based on a character's full name\n\t+<keyword> - searches the text for a specified keyword\n\t-<keyword> - excludes drawings whose text includes the specified keyword",
		AutoHelp.HelpArg)]
	protected static void Drawings(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		var drawings = actor.Gameworld.Drawings.AsEnumerable();
		while (!ss.IsFinished)
		{
			var text = ss.PopSpeech();

			if (text.EqualTo("by"))
			{
				text = ss.PopSpeech();
				if (ss.IsFinished)
				{
					actor.OutputHandler.Send("Show writings by whom?");
					return;
				}

				if (long.TryParse(text, out var value))
				{
					drawings = drawings.Where(x => x.Author.Id == value);
				}
				else if (text[0] == '*' && text.Length > 1)
				{
					drawings = drawings.Where(x => x.Author.Account.Name.EqualTo(text.Substring(1)));
				}
				else
				{
					drawings = drawings.Where(x => x.Author.PersonalName.GetName(NameStyle.FullName).EqualTo(text));
				}

				continue;
			}

			if (text[0] == '+' && text.Length > 1)
			{
				var search = text.Substring(1);
				drawings = drawings.Where(x =>
					x.FullDescription.Contains(search, StringComparison.InvariantCultureIgnoreCase) ||
					x.ShortDescription.Contains(search, StringComparison.InvariantCultureIgnoreCase));
			}

			if (text[0] == '-' && text.Length > 1)
			{
				var search = text.Substring(1);
				drawings = drawings.Where(x =>
					!x.FullDescription.Contains(search, StringComparison.InvariantCultureIgnoreCase) &&
					!x.ShortDescription.Contains(search, StringComparison.InvariantCultureIgnoreCase));
			}
		}

		actor.OutputHandler.Send(
			StringUtilities.GetTextTable(
				from drawing in drawings
				select new string[]
				{
					drawing.Id.ToString("N0", actor),
					drawing.Author.PersonalName.GetName(NameStyle.FullName),
					drawing.ImplementType.Describe(),
					drawing.DrawingSize.DescribeEnum(true),
					drawing.ShortDescription
				},
				new string[]
				{
					"ID",
					"Author",
					"Implement",
					"Size",
					"Short Description"
				},
				actor.LineFormatLength,
				colour: Telnet.Cyan,
				unicodeTable: actor.Account.UseUnicode)
		);
	}

	[PlayerCommand("Writings", "writings")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[HelpInfo("writings",
		"This command allows you to see all writings in the game and search through them. You can specify multiple search arguments after the WRITINGS keyword. Keep in mind that anything that touches the author (whether their ID, Name or Account) will force all the writings to load their characters, and this will make the command very slow the first time you do it. As such, if you can possibly filter by other things first then that would be advisable.\n\nThe options you can use are as follows:\n\tby <id> - shows writings by a character with specified ID\n\tby *<accountname> - shows all writings by a character with specified account name\n\tby \"full name\" - searches based on a character's full name\n\t+<keyword> - searches the text for a specified keyword\n\t-<keyword> - excludes writings whose text includes the specified keyword\n\t&<languagename> - shows only writings in the specified language\n\t$<script> - shows only writings in the specified script",
		AutoHelp.HelpArg)]
	protected static void Writings(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		var writings = actor.Gameworld.Writings.AsEnumerable();
		while (!ss.IsFinished)
		{
			var text = ss.PopSpeech();

			if (text.EqualTo("by"))
			{
				text = ss.PopSpeech();
				if (ss.IsFinished)
				{
					actor.OutputHandler.Send("Show writings by whom?");
					return;
				}

				if (long.TryParse(text, out var value))
				{
					writings = writings.Where(x => x.Author.Id == value);
				}
				else if (text[0] == '*' && text.Length > 1)
				{
					writings = writings.Where(x => x.Author.Account.Name.EqualTo(text.Substring(1)));
				}
				else
				{
					writings = writings.Where(x => x.Author.PersonalName.GetName(NameStyle.FullName).EqualTo(text));
				}

				continue;
			}

			if (text[0] == '+' && text.Length > 1)
			{
				var search = text.Substring(1);
				writings = writings.Where(x =>
					x.ParseFor(actor).Contains(search, StringComparison.InvariantCultureIgnoreCase));
			}

			if (text[0] == '-' && text.Length > 1)
			{
				var search = text.Substring(1);
				writings = writings.Where(x =>
					!x.ParseFor(actor).Contains(search, StringComparison.InvariantCultureIgnoreCase));
			}

			if (text[0] == '&' && text.Length > 1)
			{
				var search = text.Substring(1);
				writings = writings.Where(x => !x.Language.Name.EqualTo(search));
			}

			if (text[0] == '$' && text.Length > 1)
			{
				var search = text.Substring(1);
				writings = writings.Where(x => !x.Script.Name.EqualTo(search));
			}
		}

		actor.OutputHandler.Send(
			StringUtilities.GetTextTable(
				from writing in writings
				select new string[]
				{
					writing.Id.ToString("N0", actor),
					writing.Author.PersonalName.GetName(NameStyle.FullName),
					writing.Language.Name,
					writing.Script.Name,
					writing.ImplementType.Describe(),
					writing.Style.DescribeEnum(),
					writing.DocumentLength.ToString("N0")
				},
				new string[]
				{
					"ID",
					"Author",
					"Language",
					"Script",
					"Implement",
					"Style",
					"Length"
				},
				actor.LineFormatLength,
				colour: Telnet.Cyan,
				unicodeTable: actor.Account.UseUnicode)
		);
	}

	[PlayerCommand("Writing", "writing")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[HelpInfo("writing",
		"This command allows you to view and copy writings. You can use it in the following ways:\n\twriting show <id> - shows a particular writing\n\twriting copy <id> <newitem> - copies an existing writing to a new writable",
		AutoHelp.HelpArgOrNoArg)]
	protected static void Writing(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "show":
			case "view":
				WritingShow(actor, ss);
				return;
			case "copy":
				WritingCopy(actor, ss);
				return;
			default:
				actor.OutputHandler.Send("That is not a valid argument. Please see WRITING HELP.");
				return;
		}
	}

	private static void WritingCopy(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which writing do you want to copy?");
			return;
		}

		if (!long.TryParse(ss.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send("That is not a valid ID.");
			return;
		}

		var writing = actor.Gameworld.Writings.Get(value);
		if (writing == null)
		{
			actor.OutputHandler.Send("There is no such writing.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What writable do you want to copy that writing onto?");
			return;
		}

		var target = actor.TargetItem(ss.PopSpeech());
		if (target == null)
		{
			actor.OutputHandler.Send("You don't see anything like that.");
			return;
		}

		var targetAsWritable = target.GetItemType<IWriteable>();
		if (targetAsWritable == null)
		{
			actor.OutputHandler.Send($"{target.HowSeen(actor, true)} is not something that can be written on.");
			return;
		}

		if (!targetAsWritable.CanAddWriting(writing))
		{
			actor.OutputHandler.Send(
				$"That writing won't fit in the current writable section of {target.HowSeen(actor)}.");
			return;
		}

		targetAsWritable.AddWriting(writing.Copy());
		actor.OutputHandler.Send($"You copy writing #{writing.Id.ToString("N0")} to {target.HowSeen(actor)}.");
	}

	private static void WritingShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What is the ID of the writing you want to view?");
			return;
		}

		if (!long.TryParse(ss.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send("That is not a valid ID.");
			return;
		}

		var writing = actor.Gameworld.Writings.Get(value);
		if (writing == null)
		{
			actor.OutputHandler.Send("There is no such writing.");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Writing #{writing.Id.ToString("N0", actor)}");
		sb.AppendLine(
			$"Written in the {writing.Language.Name.ColourValue()} language and the {writing.Script.Name.ColourValue()} script.");
		sb.AppendLine(
			$"Written in {writing.Style.DescribeEnum().A_An().Colour(Telnet.Yellow)} style with {writing.WritingColour.Name.ColourValue()} {writing.ImplementType.Describe(writing.WritingColour).ColourValue()}.");
		sb.AppendLine(
			$"Written by {writing.Author.PersonalName.GetName(NameStyle.FullWithNickname).ColourName()} (ID #{writing.Author.Id.ToString("N0", actor)} - Account {writing.Author.Account.Name.ColourName()}).");
		sb.AppendLine();
		sb.AppendLine(writing.ParseFor(actor));
		actor.OutputHandler.Send(sb.ToString());
	}

	#region Scripted Events
	public const string ScriptedEventHelpText = @"Scripted events are things that you as a storyteller can program to happen to a character when they next log in. 

Essentially, when they try to log in next time they will instead go into a menu that presents them with a speil that you specify and then optionally poses them a series of questions.

The results of their choices are recorded and also saved to their character journal. You can also have progs execute based on their choices.

The following syntax is used with this command:

	#3scriptedevent list#0 - lists all scripted events. See below for filters.
	#3scriptedevent show <which>#0 - shows all information about a scripted event
	#3scriptedevent edit <which>#0 - begins editing a scripted event
	#3scriptedevent edit#0 - an alias for #scriptedevent show <id>#0 on whichever event you're currently editing
	#3scriptedevent close#0 - closes the scripted event you're editing
	#3scriptedevent new <name>#0 - creates a new scripted event
	#3scriptedevent clone <which>#0 - clones a scripted event to a new one
	#3scriptedevent applyall <which> <date>#0 - takes a template and creates a scripted event for all eligible characters
	#3scriptedevent assign <character>#0 - assigns the scripted event you're currently editing to a player
	#3scriptedevent set name <name>#0 - renames a scripted event
	#3scriptedevent set ready#0 - declares an event ready
	#3scriptedevent set earliest <date>#0 - declares that the event can't start until the date
	#3scriptedevent set template#0 - changes an event into an event template
	#3scriptedevent set name <name>#0 - gives a name to this event
	#3scriptedevent set earliest <date>#0 - sets the earliest time this event can fire
	#3scriptedevent set character <name|id>#0 - sets this event as assigned to a character
	#3scriptedevent set ready#0 - toggles this event being ready to be fire
	#3scriptedevent set template#0 - toggles this event being a template for other events
	#3scriptedevent set filter <prog>#0 - sets a prog as a filter for auto assigning
	#3scriptedevent set autoassign#0 - automatically assigned clones of this event to all matching PCs
	#3scriptedevent set addfree#0 - drops you into an editor to enter the text of a new free text question
	#3scriptedevent set addmulti#0 - drops you into an editor to enter the text of a new multiple choice question
	#3scriptedevent set remfree <##>#0 - removes a free text question
	#3scriptedevent set remmulti <##>#0 - removes a multiple choice question
	#3scriptedevent set free <##>#0 - shows detailed information about a free text question
	#3scriptedevent set free <##> question#0 - edits the question text
	#3scriptedevent set multi <##>#0 - shows detailed information about a multi choice question
	#3scriptedevent set multi <##> question#0 - edits the question text
	#3scriptedevent set multi <##> question <##> question#0 - edit the question text
	#3scriptedevent set multi <##> question <##> addanswer#0 - adds a new answer
	#3scriptedevent set multi <##> question <##> removeanswer <##>#0 - removes an answer
	#3scriptedevent set multi <##> question <##> answer <##>#0 - shows detailed information about an answer
	#3scriptedevent set multi <##> question <##> answer <##> before#0 - edits the before text of an answer
	#3scriptedevent set multi <##> question <##> answer <##> after#0 - edits the after text of an answer
	#3scriptedevent set multi <##> question <##> answer <##> filter <prog>#0 - edits the filter prog of an answer
	#3scriptedevent set multi <##> question <##> answer <##> choice <prog>#0 - edits the on choice prog of an answer

Filters for list:

	#Bfinished#0 - only show finished events
	#B!finished#0 - don't show finished events
	#Bready#0 - only show ready events
	#B!ready#0 - don't show ready events
	#Btemplate#0 - only show template events
	#B!template#0 - don't show template events
	#Bassigned#0 - only show assigned events
	#B!assigned#0 - don't show assigned events
	#B+<keyword>#0 - events with name containing the keyword
	#B-<keyword>#0 - events with name NOT containing the keyword
	#B*<id>#0 - events assigned to character with specified id
	#B*<name>#0 - events assigned to character with specified name";

	[PlayerCommand("ScriptedEvent", "scriptedevent", "sevent")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[HelpInfo("ScriptedEvent", ScriptedEventHelpText, AutoHelp.HelpArgOrNoArg)]
	protected static void ScriptedEvent(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "list":
				ScriptedEventList(actor, ss);
				return;
			case "edit":
				ScriptedEventEdit(actor, ss);
				return;
			case "show":
			case "view":
				ScriptedEventShow(actor, ss);
				return;
			case "close":
				ScriptedEventClose(actor, ss);
				return;
			case "new":
				ScriptedEventNew(actor, ss);
				return;
			case "assign":
				ScriptedEventAssign(actor, ss);
				return;
			case "clone":
				ScriptedEventClone(actor, ss);
				return;
			case "set":
				ScriptedEventSet(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(ScriptedEventHelpText.SubstituteANSIColour());
				return;
		}
	}

	private static void ScriptedEventSet(ICharacter actor, StringStack ss)
	{
		var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IScriptedEvent>>().FirstOrDefault();
		if (editing is null)
		{
			actor.OutputHandler.Send("You are not editing any scripted events.");
			return;
		}

		editing.EditingItem.BuildingCommand(actor, ss);
	}

	private static void ScriptedEventClone(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which scripted event do you want to clone?");
			return;
		}

		var item = actor.Gameworld.ScriptedEvents.GetByIdOrName(ss.SafeRemainingArgument);
		if (item is null)
		{
			actor.OutputHandler.Send("There is no such scripted event.");
			return;
		}

		var clone = item.Clone();
		actor.RemoveAllEffects<BuilderEditingEffect<IScriptedEvent>>();
		actor.AddEffect(new BuilderEditingEffect<IScriptedEvent>(actor) { EditingItem = clone });
		actor.OutputHandler.Send($"You clone scripted event #{item.Id.ToString("N0", actor)} ({item.Name.ColourName()}) to a new item with id #{clone.Id.ToString("N0", actor)}, which you are now editing.");
	}

	private static void ScriptedEventAssign(ICharacter actor, StringStack ss)
	{
		var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IScriptedEvent>>().FirstOrDefault();
		if (editing is null)
		{
			actor.OutputHandler.Send("You are not editing any scripted events.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which character do you want to assign that scripted event to?");
			return;
		}

		if (!long.TryParse(ss.SafeRemainingArgument, out var id))
		{
			actor.OutputHandler.Send("That is not a valid character id.");
			return;
		}

		var character = actor.Gameworld.TryGetCharacter(id, true);
		if (character is null)
		{
			actor.OutputHandler.Send("There is no such character to assign a scripted event to.");
			return;
		}

		if (character is INPC)
		{
			actor.OutputHandler.Send("You cannot assign scripted events to NPCs.");
			return;
		}

		if (character.IsGuest)
		{
			actor.OutputHandler.Send("You cannot assigned scripted events to guests.");
			return;
		}

		var se = editing.EditingItem.Assign(character);
		if (se != editing.EditingItem)
		{
			actor.OutputHandler.Send($"You assign a new scripted event with id #{se.Id.ToString("N0", actor)} from template #{editing.EditingItem.Id.ToString("N0", actor)} ({editing.EditingItem.Name.ColourValue()}) to {character.PersonalName.GetName(NameStyle.FullName).ColourName()}.");
			return;
		}

		actor.OutputHandler.Send($"You assign scripted event #{editing.EditingItem.Id.ToString("N0", actor)} ({editing.EditingItem.Name.ColourValue()}) to {character.PersonalName.GetName(NameStyle.FullName).ColourName()}.");
	}

	private static void ScriptedEventNew(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to the scripted event?");
			return;
		}

		var item = new RPG.ScriptedEvents.ScriptedEvent(ss.SafeRemainingArgument, actor.Gameworld);
		actor.Gameworld.Add(item);
		actor.RemoveAllEffects<BuilderEditingEffect<IScriptedEvent>>();
		actor.AddEffect(new BuilderEditingEffect<IScriptedEvent>(actor) { EditingItem = item });
		actor.OutputHandler.Send($"You are now editing newly created scripted event #{item.Id.ToString("N0", actor)} ({item.Name.ColourName()}).");
	}

	private static void ScriptedEventClose(ICharacter actor, StringStack ss)
	{
		var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IScriptedEvent>>().FirstOrDefault();
		if (editing is null)
		{
			actor.OutputHandler.Send("You are not editing any scripted events.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<IScriptedEvent>>();
		actor.OutputHandler.Send($"You are no longer editing any scripted events.");
	}

	private static void ScriptedEventShow(ICharacter actor, StringStack ss)
	{
		var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IScriptedEvent>>().FirstOrDefault();
		if (ss.IsFinished)
		{
			if (editing is not null)
			{
				actor.OutputHandler.Send(editing.EditingItem.Show(actor));
				return;
			}

			actor.OutputHandler.Send("Which scripted event would you like to view?");
			return;
		}

		var item = actor.Gameworld.ScriptedEvents.GetByIdOrName(ss.SafeRemainingArgument);
		if (item is null)
		{
			actor.OutputHandler.Send("There is no such scripted event.");
			return;
		}

		actor.OutputHandler.Send(item.Show(actor));
	}

	private static void ScriptedEventEdit(ICharacter actor, StringStack ss)
	{
		var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IScriptedEvent>>().FirstOrDefault();
		if (ss.IsFinished)
		{
			if (editing is not null)
			{
				actor.OutputHandler.Send(editing.EditingItem.Show(actor));
				return;
			}

			actor.OutputHandler.Send("Which scripted event would you like to edit?");
			return;
		}

		var item = actor.Gameworld.ScriptedEvents.GetByIdOrName(ss.SafeRemainingArgument);
		if (item is null)
		{
			actor.OutputHandler.Send("There is no such scripted event.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<IScriptedEvent>>();
		actor.AddEffect(new BuilderEditingEffect<IScriptedEvent>(actor) { EditingItem = item });
		actor.OutputHandler.Send($"You are now editing scripted event #{item.Id.ToString("N0", actor)} ({item.Name.ColourName()}).");
	}

	private static void ScriptedEventList(ICharacter actor, StringStack ss)
	{
		var list = actor.Gameworld.ScriptedEvents.AsEnumerable();
		while (!ss.IsFinished)
		{
			switch (ss.PopSpeech().ToLowerInvariant().CollapseString())
			{
				case "finished":
					list = list.Where(x => x.IsFinished);
					continue;
				case "!finished":
				case "notfinished":
				case "open":
					list = list.Where(x => !x.IsFinished);
					continue;
				case "ready":
					list = list.Where(x => x.IsReady);
					continue;
				case "!ready":
				case "notready":
				case "unready":
					list = list.Where(x => !x.IsReady);
					continue;
				case "template":
					list = list.Where(x => x.IsTemplate);
					continue;
				case "!template":
				case "nottemplate":
					list = list.Where(x => !x.IsTemplate);
					continue;
				case "assigned":
					list = list.Where(x => x.Character is not null);
					continue;
				case "!assigned":
				case "notassigned":
				case "unassigned":
					list = list.Where(x => x.Character is null);
					continue;
			}

			var substrText = ss.Last.Substring(1);

			if (ss.Last.StartsWith("+") && ss.Last.Length > 1)
			{
				list = list.Where(x => x.Name.Contains(substrText, StringComparison.InvariantCultureIgnoreCase));
				continue;
			}

			if (ss.Last.StartsWith("-") && ss.Last.Length > 1)
			{
				list = list.Where(x => !x.Name.Contains(substrText, StringComparison.InvariantCultureIgnoreCase));
				continue;
			}

			if (ss.Last.StartsWith("*") && ss.Last.Length > 1)
			{
				if (long.TryParse(substrText, out var id))
				{
					list = list.Where(x => x.Character?.Id == id);
					continue;
				}

				list = list.Where(x => x.Character is not null && x.Character.PersonalName.GetName(NameStyle.FullWithNickname).Contains(substrText, StringComparison.InvariantCultureIgnoreCase));
				continue;
			}

			actor.OutputHandler.Send($"The text {ss.Last.ColourCommand()} is not a valid filter.");
			return;
		}

		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from item in list select new List<string>
			{
				item.Id.ToString("N0", actor),
				item.Name,
				item.IsTemplate.ToColouredString(),
				item.IsReady.ToColouredString(),
				item.IsFinished.ToColouredString(),
				item.Character?.PersonalName.GetName(NameStyle.FullWithNickname) ?? ""
			},
		new List<string>
		{
			"Id",
			"Name",
			"Template",
			"Ready",
			"Finished",
			"Character"
		},
		actor,
		Telnet.Green
		));
	}
	#endregion
}