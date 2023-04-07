using System;
using System.Linq;
using MudSharp.Character;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Roles;
using MudSharp.Database;
using MudSharp.Editor;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using Account = MudSharp.Accounts.Account;
using Chargen = MudSharp.Models.Chargen;
using Microsoft.EntityFrameworkCore;
using MudSharp.Accounts;
using MudSharp.Character.Name;

namespace MudSharp.Commands.Modules;

internal class GuideModule : Module<ICharacter>
{
	private GuideModule()
		: base("Guide")
	{
		IsNecessary = true;
	}

	public static GuideModule Instance { get; } = new();

	[PlayerCommand("Role", "role")]
	[CommandPermission(PermissionLevel.Guide)]
	protected static void Role(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.Send("What is it that you want to do with the role command?");
			return;
		}

		switch (ss.Pop().ToLowerInvariant())
		{
			case "list":
				RoleList(actor, ss);
				break;
			case "show":
				RoleShow(actor, ss);
				break;
			case "edit":
			case "set":
				RoleSet(actor, ss);
				break;
			case "create":
			case "new":
				RoleCreate(actor, ss);
				break;
			default:
				actor.Send("That is not a valid option with the role command.");
				return;
		}
	}

	[PlayerCommand("Available", "available")]
	[CommandPermission(PermissionLevel.Guide)]
	protected static void Available(ICharacter actor, string input)
	{
		if (actor.AffectedBy<IAdminAvailableEffect>())
		{
			actor.RemoveAllEffects(x => x.IsEffectType<IAdminAvailableEffect>());
			actor.OutputHandler.Send("You are no longer available for player enquiries.");
		}
		else
		{
			actor.AddEffect(new AdminAvailable(actor));
			actor.OutputHandler.Send("You are now available for player enquiries.");
		}
	}

	[PlayerCommand("Send", "send")]
	[CommandPermission(PermissionLevel.Guide)]
	protected static void Send(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var starget = ss.PopSpeech();
		var characters = actor.PermissionLevel > PermissionLevel.Guide
			? actor.Gameworld.Characters
			: actor.Gameworld.Characters.Where(x => x.AffectedBy<INewPlayerEffect>());
		var target = (actor.PermissionLevel > PermissionLevel.Guide ? characters.GetByPersonalName(starget) : null) ??
		             characters.FirstOrDefault(
			             x => x.Account.Name.Equals(starget, StringComparison.InvariantCultureIgnoreCase)) ??
		             characters.FirstOrDefault(
			             x => x.Account.Name.StartsWith(starget, StringComparison.InvariantCultureIgnoreCase));
		if (target == null)
		{
			actor.OutputHandler.Send("You do not see them to send to.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to send to them?");
			return;
		}

		target.OutputHandler.Send(
			$"{(actor.AffectedBy<IAdminAvailableEffect>() || target.IsAdministrator() ? string.Format("[From {1}{0}]", actor.Account.Name.TitleCase(), actor.PermissionLevel > PermissionLevel.Guide ? "" : "Player Guide ").Colour(Telnet.Green) : (actor.PermissionLevel > PermissionLevel.Guide ? "[Staff Member]" : "[Player Guide]").Colour(Telnet.Green))} {ss.RemainingArgument.ProperSentences().Fullstop()}");
		actor.OutputHandler.Send(
			$"{$"[Sent to {(actor.PermissionLevel > PermissionLevel.Guide ? target.PersonalName.GetName(NameStyle.SimpleFull) : target.Account.Name.TitleCase())}]".Colour(Telnet.Magenta)} {ss.RemainingArgument.ProperSentences().Fullstop()}");
	}

	[PlayerCommand("Applications", "applications", "apps")]
	[CommandPermission(PermissionLevel.Guide)]
	protected static void Applications(ICharacter character, string command)
	{
		using (new FMDB())
		{
			var applications =
				FMDB.Context.Chargens.Where(x => x.Status == (int)CharacterStatus.Submitted).ToList();
			if (!applications.Any())
			{
				character.OutputHandler.Send("There are not currently any applications for new characters.");
				return;
			}

			character.OutputHandler.Send(
				StringUtilities.GetTextTable(
					from app in applications
					select
						new[]
						{
							app.Id.ToString(), app.Account.Name, app.Name,
							((PermissionLevel)(app.MinimumApprovalAuthority ?? 0)).Describe(),
							(app.SubmitTime != null ? DateTime.UtcNow - app.SubmitTime.Value : TimeSpan.Zero).Describe()
						},
					new[] { "ID#", "Account", "Name", "Authority Required", "Time in Queue" },
					character.Account.LineFormatLength, colour: Telnet.Green
				) +
				$"\n\nThere are a total of {applications.Count.ToString("N0", character).Colour(Telnet.Green)} applications awaiting review."
			);
		}
	}

	[PlayerCommand("Application", "application")]
	[CommandPermission(PermissionLevel.Guide)]
	protected static void Application(ICharacter character, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.Pop().ToLowerInvariant())
		{
			case "view":
			case "show":
				ApplicationView(character, ss);
				break;
			case "review":
				ApplicationReview(character, ss);
				break;
			case "list":
				Applications(character, ss.RemainingArgument);
				break;
			default:
				character.OutputHandler.Send("You must specify either view or review to the application command.");
				return;
		}
	}

	private static void ApplicationView(ICharacter character, StringStack command)
	{
		using (new FMDB())
		{
			var cmd = command.Pop();
			var applications =
				FMDB.Context.Chargens.Where(x => x.Status == (int)CharacterStatus.Submitted).ToList();
			Chargen chargen = null;
			chargen = long.TryParse(cmd, out var value)
				? applications.FirstOrDefault(x => x.Id == value)
				: applications.FirstOrDefault(
					x => x.Name.StartsWith(cmd, StringComparison.InvariantCultureIgnoreCase));

			if (chargen == null)
			{
				character.OutputHandler.Send("There is no such application for you to view.");
				return;
			}

			var loadedChargen = new CharacterCreation.Chargen(chargen, character.Gameworld, chargen.Account);
			character.OutputHandler.Send(loadedChargen.DisplayForReview(character.Account, character.PermissionLevel));
		}
	}

	private static void ApplicationReview(ICharacter character, StringStack command)
	{
		using (new FMDB())
		{
			var cmd = command.Pop();
			var applications =
				FMDB.Context.Chargens.Where(x => x.Status == (int)CharacterStatus.Submitted).ToList();
			Chargen chargen = null;
			chargen = long.TryParse(cmd, out var value)
				? applications.FirstOrDefault(x => x.Id == value)
				: applications.FirstOrDefault(
					x => x.Name.StartsWith(cmd, StringComparison.InvariantCultureIgnoreCase));

			if (chargen == null)
			{
				character.OutputHandler.Send("There is no such application for you to view.");
				return;
			}

			if (chargen.Status == (int)CharacterStatus.Active)
			{
				character.OutputHandler.Send("That application has already been approved.");
				return;
			}

			var loadedChargen = new CharacterCreation.Chargen(chargen, character.Gameworld, chargen.Account);

			if (loadedChargen.MinimumApprovalAuthority > character.PermissionLevel)
			{
				character.Send("That application requires a minimum authority level of {0}.",
					loadedChargen.MinimumApprovalAuthority.Describe());
				return;
			}

			if (!character.IsAdministrator(PermissionLevel.HighAdmin) &&
			    loadedChargen.SelectedRoles.Any(
				    x =>
					    x.RequiredApprovers.Any() &&
					    x.RequiredApprovers.All(
						    y => !y.Equals(character.Account.Name, StringComparison.InvariantCultureIgnoreCase))))
			{
				var blockingRole =
					loadedChargen.SelectedRoles.First(
						x =>
							x.RequiredApprovers.Any() &&
							x.RequiredApprovers.All(
								y => !y.Equals(character.Account.Name, StringComparison.InvariantCultureIgnoreCase)));
				character.Send("The role {0} requires specific people to approve it, and you are not among them.",
					blockingRole.Name.TitleCase().Colour(Telnet.Green));
				return;
			}

			if (
				loadedChargen.ApplicationCosts.Any(
					x => loadedChargen.Account.AccountResources.ValueOrDefault(x.Key, 0) < x.Value))
			{
				character.Send("Account {0} no longer has sufficient {1} to pay for that application.",
					loadedChargen.Account.Name.Proper(),
					loadedChargen.ApplicationCosts.Where(
						             x => loadedChargen.Account.AccountResources.ValueOrDefault(x.Key, 0) < x.Value)
					             .Select(x => x.Key.PluralName)
					             .ListToString()
				);
				return;
			}

			character.OutputHandler.Send(loadedChargen.DisplayForReview(character.Account, character.PermissionLevel));
			loadedChargen.LockApplication();
			character.Send("\nUse the {0} and {1} commands to review this application.",
				"accept".Colour(Telnet.Yellow), "decline".Colour(Telnet.Yellow));
			character.AddEffect(new Accept(character, new ChargenApprovalProposal(character, loadedChargen)),
				TimeSpan.FromSeconds(300));
		}
	}

	#region Role SubCommands

	private static void RoleList(ICharacter actor, StringStack ss)
	{
		var roles = actor.Gameworld.Roles.Where(x => actor.PermissionLevel >= x.MinimumPermissionToView);
		while (!ss.IsFinished)
		{
			var text = ss.Pop();
			if (
				Enum.GetNames(typeof(ChargenRoleType))
				    .Any(x => x.Equals(text, StringComparison.InvariantCultureIgnoreCase)))
			{
				var type = (ChargenRoleType)Enum.Parse(typeof(ChargenRoleType), text, true);
				roles = roles.Where(x => x.RoleType == type);
				continue;
			}

			if (text[0] == '!')
			{
				text = text.Substring(1);
				if (!Enum.TryParse(text, true, out ChargenRoleType type))
				{
					actor.Send("There is no such type as {0}.", type);
					return;
				}

				roles = roles.Where(x => x.RoleType != type);
				continue;
			}

			if (text.Equals("by", StringComparison.InvariantCultureIgnoreCase))
			{
				if (ss.IsFinished)
				{
					actor.Send("View roles by whom?");
					return;
				}

				text = ss.Pop();
				roles = roles.Where(x => x.Poster.Equals(text, StringComparison.InvariantCultureIgnoreCase));
				continue;
			}

			if (text.Equals("mine", StringComparison.InvariantCultureIgnoreCase))
			{
				roles =
					roles.Where(
						x => x.Poster.Equals(actor.Account.Name, StringComparison.InvariantCultureIgnoreCase));
				continue;
			}

			if (text.Equals("current", StringComparison.InvariantCultureIgnoreCase))
			{
				roles = roles.Where(x => !x.Expired);
				continue;
			}

			if (text.Equals("expired", StringComparison.InvariantCultureIgnoreCase))
			{
				roles = roles.Where(x => x.Expired);
			}
		}

		actor.Send(StringUtilities.GetTextTable(
			from role in roles
			select new[]
			{
				role.Id.ToString("N0", actor),
				role.Name.TitleCase(),
				role.RoleType.ToString(),
				role.Poster.Proper(),
				role.AvailabilityProg != null
					? $"{role.AvailabilityProg.FunctionName} (#{role.AvailabilityProg.Id:N0})".FluentTagMXP("send",
						$"href='show futureprog {role.AvailabilityProg.Id}'")
					: "None",
				role.Costs.Select(x => string.Format(actor, "{0:N0} {1}", x.Value, x.Key.Alias))
				    .ListToString(separator: " ", conjunction: ""),
				role.Expired ? "No" : "Yes"
			},
			new[] { "ID#", "Name", "Type", "Poster", "Availability", "Cost", "Current" },
			actor.Account.LineFormatLength,
			colour: Telnet.Green
		));
	}

	private static void RoleShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Which role do you want to show?");
			return;
		}

		var roles = actor.Gameworld.Roles.Where(x => x.MinimumPermissionToView <= actor.PermissionLevel);
		var role = long.TryParse(ss.PopSpeech(), out var value)
			? roles.FirstOrDefault(x => x.Id == value)
			: roles.FirstOrDefault(x => x.Name.Equals(ss.Last, StringComparison.InvariantCultureIgnoreCase));
		if (role == null)
		{
			actor.Send("There is no such role to show you.");
			return;
		}

		actor.Send(role.Show(actor));
	}

	#region Role Set Subcommands

	private static void RoleSetName(ICharacter actor, StringStack ss, IChargenRole role)
	{
		if (ss.IsFinished)
		{
			actor.Send("What name do you want to give to this role?");
			return;
		}

		actor.Send("The {0} role #{1} is now known as {2}.", role.RoleType.ToString(),
			role.Id.ToString("N0", actor).Colour(Telnet.Green),
			ss.SafeRemainingArgument.TitleCase().Colour(Telnet.Green));
		role.SetName(ss.SafeRemainingArgument);
		role.Changed = true;
	}

	private static void RoleSetDescriptionPost(string message, IOutputHandler handler, object[] parameters)
	{
		var role = (IChargenRole)parameters[0];
		var actor = (ICharacter)parameters[1];
		role.ChargenBlurb = message;
		role.Changed = true;
		handler.Send(
			$"You change the description of the {role.RoleType} role {role.Name.TitleCase().Colour(Telnet.Green)} (#{role.Id.ToString("N0", actor).Colour(Telnet.Green)}) to:\n\n{message.SubstituteANSIColour().Wrap(80, "\t")}");
	}

	private static void RoleSetDescriptionCancel(IOutputHandler handler, object[] parameters)
	{
		handler.Send("You decline to edit the description of the role.");
	}

	private static void RoleSetDescription(ICharacter actor, StringStack ss, IChargenRole role)
	{
		if (!string.IsNullOrEmpty(role.ChargenBlurb))
		{
			actor.OutputHandler.Send("Replacing:\n" + role.ChargenBlurb.Wrap(80, "\t"));
		}

		actor.OutputHandler.Send("Enter the description in the editor below.");
		actor.EditorMode(RoleSetDescriptionPost, RoleSetDescriptionCancel, 1.0, null, EditorOptions.None,
			new object[] { role, actor });
	}

	private static void RoleSetCost(ICharacter actor, StringStack ss, IChargenRole role)
	{
		if (ss.IsFinished)
		{
			actor.Send(
				"How much and which resources should this role cost?\nn.b.: You can remove an existing cost by setting it to 0.");
			return;
		}

		if (!int.TryParse(ss.Pop(), out var value) || value < 0)
		{
			actor.Send("You must enter a valid amount for this resource to cost.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("Which resource would you like to set the costs of this role for?");
			return;
		}

		var resourceText = ss.PopSpeech();
		var resource =
			actor.Gameworld.ChargenResources.FirstOrDefault(
				x => x.Name.Equals(resourceText, StringComparison.InvariantCultureIgnoreCase)) ??
			actor.Gameworld.ChargenResources.FirstOrDefault(
				x => x.PluralName.Equals(resourceText, StringComparison.InvariantCultureIgnoreCase)) ??
			actor.Gameworld.ChargenResources.FirstOrDefault(
				x => x.Alias.Equals(resourceText, StringComparison.InvariantCultureIgnoreCase));

		if (resource == null)
		{
			actor.Send("There is no such resource.");
			return;
		}

		if (value == 0)
		{
			if (!role.Costs.ContainsKey(resource) || role.Costs[resource] == 0)
			{
				actor.Send("That role does not currently cost any {0}.", resource.PluralName);
				return;
			}

			role.Costs.Remove(resource);
			role.Changed = true;
			actor.Send("The {0} role will no longer cost any {1}.",
				role.Name.TitleCase().Colour(Telnet.Green),
				resource.PluralName.TitleCase().Colour(Telnet.Green)
			);
			return;
		}

		role.Costs[resource] = value;
		role.Changed = true;
		actor.Send("The {0} role will now cost {1} {2}.",
			role.Name.TitleCase().Colour(Telnet.Green),
			value.ToString("N0", actor).Colour(Telnet.Green),
			(value == 1 ? resource.Name : resource.PluralName).TitleCase().Colour(Telnet.Green)
		);
	}

	private static void RoleSetProg(ICharacter actor, StringStack ss, IChargenRole role)
	{
		if (ss.IsFinished)
		{
			actor.Send("Do you want to this role to have a particular eligability prog, or clear the existing one?");
			return;
		}

		var argText = ss.Pop();
		if (argText.Equals("clear", StringComparison.InvariantCultureIgnoreCase))
		{
			if (role.AvailabilityProg == null)
			{
				actor.Send("That role does not have an availability prog to clear.");
				return;
			}

			role.AvailabilityProg = null;
			role.Changed = true;
			actor.Send(
				"You clear the availability prog from the {0} role. It will now always be available for selection.",
				role.Name.TitleCase().Colour(Telnet.Green));
			return;
		}

		var prog = long.TryParse(argText, out var value)
			? actor.Gameworld.FutureProgs.Get(value)
			: actor.Gameworld.FutureProgs.FirstOrDefault(
				x => x.FunctionName.Equals(argText, StringComparison.InvariantCultureIgnoreCase));
		if (prog == null)
		{
			actor.Send("There is no such prog.");
			return;
		}

		if (prog.ReturnType != FutureProgVariableTypes.Boolean ||
		    !prog.MatchesParameters(new[] { FutureProgVariableTypes.Chargen }))
		{
			actor.Send(
				"The prog must return a boolean and accept a single chargen parameter, whereas {1} (#{2:N0}) does not.",
				prog.FunctionName,
				prog.Id
			);
			return;
		}

		role.AvailabilityProg = prog;
		role.Changed = true;
		actor.Send("The {0} role will now use the {1} prog to determine availability in chargen.",
			role.Name.TitleCase().Colour(Telnet.Green),
			prog.FunctionName.Colour(Telnet.Green)
		);
	}

	private static void RoleSetAlive(ICharacter actor, StringStack ss, IChargenRole role)
	{
		if (ss.IsFinished)
		{
			actor.Send("How many simultaneous alive characters may take this role?");
			return;
		}

		if (!int.TryParse(ss.Pop(), out var value))
		{
			actor.Send("You must enter a number.");
			return;
		}

		if (value < 1)
		{
			role.MaximumNumberAlive = 0;
			role.Changed = true;
			actor.Send(
				"The {0} role no longer has any restrictions on how many living characters can hold it at one time.",
				role.Name.TitleCase().Colour(Telnet.Green)
			);
			return;
		}

		role.MaximumNumberAlive = value;
		role.Changed = true;
		actor.Send(
			"The {0} role will now become ineligable for selection when there are {1:N0} living characters with the role.",
			role.Name.TitleCase().Colour(Telnet.Green),
			value
		);
	}

	private static void RoleSetTotal(ICharacter actor, StringStack ss, IChargenRole role)
	{
		if (ss.IsFinished)
		{
			actor.Send("How many total characters may take this role?");
			return;
		}

		if (!int.TryParse(ss.Pop(), out var value))
		{
			actor.Send("You must enter a number.");
			return;
		}

		if (value < 1)
		{
			role.MaximumNumberTotal = 0;
			role.Changed = true;
			actor.Send("The {0} role no longer has any restrictions on how many characters can hold it in total.",
				role.Name.TitleCase().Colour(Telnet.Green)
			);
			return;
		}

		role.MaximumNumberTotal = value;
		role.Changed = true;
		actor.Send(
			"The {0} role will now become ineligable for selection when there are {1:N0} total characters with the role.",
			role.Name.TitleCase().Colour(Telnet.Green),
			value
		);
	}

	private static void RoleSetApprovers(ICharacter actor, StringStack ss, IChargenRole role)
	{
		if (ss.IsFinished)
		{
			actor.Send(
				"You must either specify approvers for this role, or use clear to clear all current approvers.");
			return;
		}

		if (ss.SafeRemainingArgument.Equals("clear", StringComparison.InvariantCultureIgnoreCase))
		{
			if (!role.RequiredApprovers.Any())
			{
				actor.Send("The {0} role does not require any specific approvers.",
					role.Name.TitleCase().Colour(Telnet.Green));
				return;
			}

			role.RequiredApprovers.Clear();
			role.Changed = true;
			actor.Send("The {0} role no longer requires any specific approver.",
				role.Name.TitleCase().Colour(Telnet.Green));
			return;
		}

		var names = ss.SafeRemainingArgument.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
		using (new FMDB())
		{
			if (
				names.Any(
					x =>
						!FMDB.Context.Accounts.AsNoTracking()
						     .Any(y => y.Name == x)))
			{
				actor.Send("You must enter valid account names.");
				return;
			}
		}

		role.RequiredApprovers.Clear();
		role.RequiredApprovers.AddRange(names);
		role.Changed = true;
		actor.Send("Applications selecting the {0} role will now be required to be approved by {1}.",
			role.Name.TitleCase().Colour(Telnet.Green),
			names.Select(x => x.Proper()).ListToString(conjunction: "or ")
		);
	}

	private static void RoleSetApprovePermission(ICharacter actor, StringStack ss, IChargenRole role)
	{
		if (ss.IsFinished)
		{
			actor.Send(
				"What permission level do you want to set for the approval of this role?\nn.b.: use \"any\" for no permissions required.");
			return;
		}

		if (!Enum.TryParse(ss.Pop(), out PermissionLevel level))
		{
			actor.Send("That is not a valid permission level. See {0}.",
				"show permissions".Colour(Telnet.Yellow).FluentTagMXP("send", "href='show permissions'"));
			return;
		}

		role.MinimumPermissionToApprove = level;
		role.Changed = true;
		actor.Send("The {0} role will now require a permission level of {1} to approve.",
			role.Name.TitleCase().Colour(Telnet.Green), level.Describe().Colour(Telnet.Green));
	}

	private static void RoleSetViewPermission(ICharacter actor, StringStack ss, IChargenRole role)
	{
		if (ss.IsFinished)
		{
			actor.Send(
				"What permission level do you want to set for the viewing of this role?\nn.b.: use \"any\" for no permissions required.");
			return;
		}

		if (!Enum.TryParse(ss.Pop(), out PermissionLevel level))
		{
			actor.Send("That is not a valid permission level. See {0}.",
				"show permissions".Colour(Telnet.Yellow).FluentTagMXP("send", "href='show permissions'"));
			return;
		}

		if (level > actor.PermissionLevel)
		{
			actor.Send("You cannot make a role with view permissions higher than your own.");
			return;
		}

		role.MinimumPermissionToView = level;
		role.Changed = true;
		actor.Send("The {0} role will now require a permission level of {1} to view.",
			role.Name.TitleCase().Colour(Telnet.Green), level.Describe().Colour(Telnet.Green));
	}

	private static void RoleSetExpired(ICharacter actor, StringStack ss, IChargenRole role)
	{
		actor.Send(
			role.Expired
				? "You change the {0} role to be active once again."
				: "You change the {0} role to be expired, and it will no longer appear in character creation.",
			role.Name.TitleCase().Colour(Telnet.Green));

		role.Expired = !role.Expired;
		role.Changed = true;
	}

	private static void RoleSetClans(ICharacter actor, StringStack ss, IChargenRole role)
	{
		actor.Send("Coming soon.");
	}

	private static void RoleSetTraits(ICharacter actor, StringStack ss, IChargenRole role)
	{
		actor.Send("Coming soon.");
	}

	private static void RoleSetCurrency(ICharacter actor, StringStack ss, IChargenRole role)
	{
		actor.Send("Coming soon.");
	}

	#endregion

	private static void RoleSet(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Which role do you wish to set the properties of?");
			return;
		}

		var roles = actor.Gameworld.Roles.Where(x => x.MinimumPermissionToView <= actor.PermissionLevel);
		var role = long.TryParse(ss.PopSpeech(), out var value)
			? roles.FirstOrDefault(x => x.Id == value)
			: roles.FirstOrDefault(x => x.Name.Equals(ss.Last, StringComparison.InvariantCultureIgnoreCase));
		if (role == null)
		{
			actor.Send("There is no such role to set.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("What do you want to set for that role?");
			return;
		}

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "name":
				RoleSetName(actor, ss, role);
				break;
			case "description":
			case "desc":
			case "blurb":
				RoleSetDescription(actor, ss, role);
				break;
			case "cost":
			case "costs":
				RoleSetCost(actor, ss, role);
				break;
			case "prog":
				RoleSetProg(actor, ss, role);
				break;
			case "alive":
			case "maxalive":
			case "max alive":
			case "maximum alive":
				RoleSetAlive(actor, ss, role);
				break;
			case "total":
			case "maxtotal":
			case "max total":
			case "maximum total":
				RoleSetTotal(actor, ss, role);
				break;
			case "approver":
			case "approvers":
				RoleSetApprovers(actor, ss, role);
				break;
			case "approval permission":
			case "approve permission":
			case "permission":
				RoleSetApprovePermission(actor, ss, role);
				break;
			case "view permission":
			case "view":
				RoleSetViewPermission(actor, ss, role);
				break;
			case "expired":
				RoleSetExpired(actor, ss, role);
				break;
			case "clans":
			case "clan":
				RoleSetClans(actor, ss, role);
				break;
			case "currency":
				RoleSetCurrency(actor, ss, role);
				break;
			case "traits":
				RoleSetTraits(actor, ss, role);
				break;
			default:
				actor.Send("That is not a valid option for the role set command.");
				return;
		}
	}

	private static void RoleCreate(ICharacter actor, StringStack ss)
	{
	}

	#endregion Role SubCommands
}