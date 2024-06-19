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
using System.Xml.Linq;

namespace MudSharp.Commands.Modules;

internal class GuideModule : Module<ICharacter>
{
	private GuideModule()
		: base("Guide")
	{
		IsNecessary = true;
	}

	public static GuideModule Instance { get; } = new();

	private const string RoleHelp = @"The role command is used to edit chargen roles, which are selected by player characters during character creation. They can also be added to NPCs.

Roles are used to control class and subclass (if you use them), starting locations, and plot-specific backgrounds. They are separated into role types, and each character may have zero or one of each role type. For example, a character may have only one #2family#0 role but may choose none, if your settings permit them to do so. That setting is controlled by the chargen storyboards.

The syntax to edit roles is as follows:

	#3role list#0 - lists all of the roles 
	#3role show <which>#0 - shows information about a role
	#3role edit <which>#0 - opens a role for editing
	#3role edit#0 - an alias for #3role show#0 on your currently edited role
	#3role close#0 - no longer edit the role that you are editing
	#3role new <type> <name>#0 - creates a new role
	#3role clone <which> <newName>#0 - clones an existing role to a new one
	#3role set name <name>#0 - renames this role
	#3role set type <type>#0 - changes the type of this role
	#3role set blurb#0 - drops you into an editor to enter the blurb in chargen
	#3role set cost <resource> <amount>#0 - sets this role to cost a certain amount of resource
	#3role set cost <resource> 0#0 - removes a cost from this role
	#3role set prog <which>#0 - sets the prog that controls whether this appears as an option in chargen
	#3role set prog clear#0 - removes a prog; role will always be available
	#3role set alive <##>#0 - sets the maximum number of living characters who can have this role
	#3role set alive 0#0 - removes any maximum number of living characters restriction
	#3role set total <##>#0 - sets the maximum number of total characters who may ever take this role
	#3role set total 0#0 - removes any maximum number of total characters restriction
	#3role set approvers <account1> [<account2>] ...#0 - sets mandatory approvers for this role
	#3role set approvers clear#0 - removes mandatory approvers from this role
	#3role set permission <level>#0 - sets a permission level required to approve this role
	#3role set view <level>#0 - sets a permission level required to view this role as a builder
	#3role set expired#0 - toggles this role being expired and no longer available
	#3role set clan <clan> rank <rank>#0 - sets a rank in a clan that is given (also adds a clan)
	#3role set clan <clan> remove#0 - removes a clan membership given by this role
	#3role set clan <clan> paygrade <paygrade>#0 - sets a paygrade in a clan membership given by this role
	#3role set clan <clan> appointment <which>#0 - toggles an appointment in a clan membership given by this role
	#3role set currency <which> <amount>#0 - sets currency to be given to the character at commencement
	#3role set currency <which> 0#0 - removes currency given by this role
	#3role set trait <which> <boost>#0 - sets a boost or penalty to an attribute or trait at commencement
	#3role set trait <which> 0#0 - removes a boost or penalty  to an attribute or trait at commencement
	#3role set trait <which> <boost> boost#0 - gives the boost only if the attribute or skill is already possessed
	#3role set merit <which>#0 - toggles this role giving a merit or flaw";

	[PlayerCommand("Role", "role")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[HelpInfo("role", RoleHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void Role(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "list":
				RoleList(actor, ss);
				break;
			case "show":
			case "view":
				RoleShow(actor, ss);
				break;
			case "edit":
				RoleEdit(actor, ss);
				break;
			case "close":
				RoleClose(actor, ss);
				break;
			case "set":
				RoleSet(actor, ss);
				break;
			case "create":
			case "new":
				RoleCreate(actor, ss);
				break;
			case "clone":
				RoleClone(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(RoleHelp.SubstituteANSIColour());
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
					x => loadedChargen.Account.AccountResources[x.Key] < x.Value))
			{
				character.Send("Account {0} no longer has sufficient {1} to pay for that application.",
					loadedChargen.Account.Name.Proper(),
					loadedChargen.ApplicationCosts.Where(
									 x => loadedChargen.Account.AccountResources[x.Key] < x.Value)
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
			var text = ss.PopSpeech();
			if (text.TryParseEnum<ChargenRoleType>(out var type))
			{
				roles = roles.Where(x => x.RoleType == type);
				continue;
			}

			if (text[0] == '!')
			{
				text = text.Substring(1);
				if (!Enum.TryParse(text, true, out type))
				{
					actor.OutputHandler.Send($"There is no such type as {text.ColourCommand()}.");
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

				text = ss.PopSpeech();
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

		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from role in roles
			select new[]
			{
				role.Id.ToString("N0", actor),
				role.Name.TitleCase(),
				role.RoleType.DescribeEnum(),
				role.Poster.Proper(),
				role.AvailabilityProg?.MXPClickableFunctionName() ?? "None",
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

		actor.OutputHandler.Send(role.Show(actor));
	}

	private static void RoleEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IChargenRole>>().FirstOrDefault();
			if (editing is null)
			{
				actor.OutputHandler.Send("Which role do you want to edit?");
				return;
			}

			actor.OutputHandler.Send(editing.EditingItem.Show(actor));
			return;
		}

		var role = actor.Gameworld.Roles.GetByIdOrName(ss.SafeRemainingArgument);
		if (role is null)
		{
			actor.OutputHandler.Send("There is no such role.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<IChargenRole>>();
		actor.AddEffect(new BuilderEditingEffect<IChargenRole>(actor) { EditingItem = role });
		actor.OutputHandler.Send($"You are now editing the role {role.Name.TitleCase().ColourName()}.");
	}

	private static void RoleClose(ICharacter actor, StringStack ss)
	{
		if (!actor.CombinedEffectsOfType<BuilderEditingEffect<IChargenRole>>().Any())
		{
			actor.OutputHandler.Send("You are not editing any roles.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<IChargenRole>>();
		actor.OutputHandler.Send("You are no longer editing any roles.");
	}

	private static void RoleSet(ICharacter actor, StringStack ss)
	{
		var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IChargenRole>>().FirstOrDefault();
		if (editing is null)
		{
			actor.OutputHandler.Send("Which role do you want to set the properties of?");
			return;
		}

		var role = editing.EditingItem;
		role.BuildingCommand(actor, ss);
	}

	private static void RoleClone(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which role do you want to clone?");
			return;
		}

		var role = actor.Gameworld.Roles.GetByIdOrName(ss.PopSpeech());
		if (role is null)
		{
			actor.OutputHandler.Send("There is no such role.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to the new role?");
			return;
		}

		var newRole = role.Clone(ss.SafeRemainingArgument.TitleCase());
		actor.Gameworld.Add(newRole);
		actor.RemoveAllEffects<BuilderEditingEffect<IChargenRole>>();
		actor.AddEffect(new BuilderEditingEffect<IChargenRole>(actor) { EditingItem = newRole });
		actor.OutputHandler.Send($"You clone the role called {role.Name.ColourName()} into a new role called {newRole.Name.ColourName()}, which are now editing.");
	}

	private static void RoleCreate(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send($"Which type of role do you want to create? The valid types are {Enum.GetValues<ChargenRoleType>().Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
			return;
		}

		if (!ss.PopSpeech().TryParseEnum<ChargenRoleType>(out var type))
		{
			actor.OutputHandler.Send($"That is not a valid role type. The valid types are {Enum.GetValues<ChargenRoleType>().Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to name the role?");
			return;
		}

		var name = ss.SafeRemainingArgument.TitleCase();
		var role = new ChargenRole(actor.Account, name, type, actor.Gameworld);
		actor.Gameworld.Add(role);
		actor.RemoveAllEffects<BuilderEditingEffect<IChargenRole>>();
		actor.AddEffect(new BuilderEditingEffect<IChargenRole>(actor) { EditingItem = role });
		actor.OutputHandler.Send($"You create a new {type.DescribeEnum().ColourName()} role called {name.ColourName()}, which are now editing.");
	}

	#endregion Role SubCommands
}