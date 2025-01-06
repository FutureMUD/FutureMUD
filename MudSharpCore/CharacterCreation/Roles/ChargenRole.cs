using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using MudSharp.Accounts;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Commands.Modules;
using MudSharp.Community;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Editor;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Merits;
using Org.BouncyCastle.Cms;

namespace MudSharp.CharacterCreation.Roles;

internal class ChargenRole : SaveableItem, IChargenRole
{
	public ChargenRole(IAccount originator, string name, ChargenRoleType type, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		using (new FMDB())
		{
			var dbitem = new Models.ChargenRole();
			FMDB.Context.ChargenRoles.Add(dbitem);
			dbitem.Name = name;
			dbitem.ChargenBlurb =
				"This role has not been described. You almost certainly should not be selecting it.";
			dbitem.Expired = false;
			dbitem.AvailabilityProgId = Gameworld.AlwaysFalseProg?.Id ?? 0L;
			dbitem.PosterId = originator.Id;
			dbitem.MaximumNumberAlive = 0;
			dbitem.MaximumNumberTotal = 0;
			dbitem.Type = (int)type;
			FMDB.Context.SaveChanges();
			LoadFromDatabase(dbitem);
		}
	}

	public ChargenRole(MudSharp.Models.ChargenRole role, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		LoadFromDatabase(role);
	}

	public IChargenRole Clone(string newName)
	{
		return new ChargenRole(this, newName);
	}

	private ChargenRole(ChargenRole rhs, string name)
	{
		Gameworld = rhs.Gameworld;
		using (new FMDB())
		{
			var dbitem = new Models.ChargenRole();
			FMDB.Context.ChargenRoles.Add(dbitem);
			dbitem.Name = name;
			dbitem.ChargenBlurb = rhs.ChargenBlurb;
			dbitem.Expired = false;
			dbitem.AvailabilityProgId = rhs.AvailabilityProg?.Id;
			dbitem.PosterId = rhs.PosterId;
			dbitem.MaximumNumberAlive = rhs.MaximumNumberAlive;
			dbitem.MaximumNumberTotal = rhs.MaximumNumberTotal;
			dbitem.Type = (int)rhs.RoleType;
			foreach (var item in rhs._chargenAdvices)
			{
				dbitem.ChargenAdvicesChargenRoles.Add(new ChargenAdvicesChargenRoles
				{ ChargenAdviceId = item.Id, ChargenRole = dbitem });
			}

			foreach (var account in rhs.RequiredApprovers)
			{
				dbitem.ChargenRolesApprovers.Add(
					new ChargenRolesApprovers
					{
						Approver = FMDB.Context.Accounts.FirstOrDefault(x => x.Name == account),
						ChargenRole = dbitem
					});
			}

			foreach (var item in rhs.Costs)
			{
				var newItem = new ChargenRolesCost();
				newItem.ChargenRole = dbitem;
				newItem.ChargenResourceId = item.Key.Id;
				newItem.Amount = item.Value;
				FMDB.Context.ChargenRolesCosts.Add(newItem);
			}

			foreach (var item in rhs.StartingCurrency)
			{
				var newItem = new ChargenRolesCurrency();
				newItem.ChargenRole = dbitem;
				newItem.CurrencyId = item.Key.Id;
				newItem.Amount = item.Value;
				FMDB.Context.ChargenRolesCurrencies.Add(newItem);
			}

			foreach (var item in rhs.TraitAdjustments)
			{
				var newItem = new ChargenRolesTrait();
				newItem.ChargenRole = dbitem;
				newItem.TraitId = item.Key.Id;
				newItem.Amount = item.Value.amount;
				newItem.GiveIfDoesntHave = item.Value.giveIfMissing;
				FMDB.Context.ChargenRolesTraits.Add(newItem);
			}
			
			foreach (var item in rhs.ClanMemberships)
			{
				var newItem = new ChargenRolesClanMemberships();
				newItem.ChargenRole = dbitem;
				newItem.ClanId = item.Clan.Id;
				newItem.RankId = item.Rank.Id;
				newItem.PaygradeId = item.Paygrade?.Id;
				FMDB.Context.ChargenRolesClanMemberships.Add(newItem);
				foreach (var appointment in item.Appointments)
				{
					var newAppointment = new ChargenRolesClanMembershipsAppointments();
					newAppointment.ChargenRolesClanMembership = newItem;
					newAppointment.AppointmentId = appointment.Id;
					FMDB.Context.ChargenRolesClanMembershipsAppointments.Add(newAppointment);
				}

			}

			FMDB.Context.SaveChanges();
			LoadFromDatabase(dbitem);
		}
	}

	public override void Save()
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.ChargenRoles.Find(Id);
			dbitem.Name = Name;
			dbitem.ChargenBlurb = ChargenBlurb;
			dbitem.Type = (int)RoleType;
			dbitem.PosterId = PosterId;
			dbitem.AvailabilityProgId = AvailabilityProg?.Id;
			dbitem.MaximumNumberAlive = MaximumNumberAlive;
			dbitem.MaximumNumberTotal = MaximumNumberTotal;
			dbitem.MinimumAuthorityToApprove = (int)MinimumPermissionToApprove;
			dbitem.MinimumAuthorityToView = (int)MinimumPermissionToView;

			FMDB.Context.ChargenAdvicesChargenRoles.RemoveRange(dbitem.ChargenAdvicesChargenRoles);
			foreach (var item in _chargenAdvices)
			{
				dbitem.ChargenAdvicesChargenRoles.Add(new ChargenAdvicesChargenRoles
					{ ChargenAdviceId = item.Id, ChargenRole = dbitem });
			}

			FMDB.Context.ChargenRolesApprovers.RemoveRange(dbitem.ChargenRolesApprovers);
			foreach (var account in RequiredApprovers)
			{
				dbitem.ChargenRolesApprovers.Add(
					new ChargenRolesApprovers
					{
						Approver = FMDB.Context.Accounts.FirstOrDefault(x => x.Name == account),
						ChargenRole = dbitem
					});
			}

			FMDB.Context.ChargenRolesCosts.RemoveRange(dbitem.ChargenRolesCosts);
			foreach (var item in Costs)
			{
				var newItem = new ChargenRolesCost();
				newItem.ChargenRole = dbitem;
				newItem.ChargenResourceId = item.Key.Id;
				newItem.Amount = item.Value;
				FMDB.Context.ChargenRolesCosts.Add(newItem);
			}

			FMDB.Context.ChargenRolesCurrencies.RemoveRange(dbitem.ChargenRolesCurrencies);
			foreach (var item in StartingCurrency)
			{
				var newItem = new ChargenRolesCurrency();
				newItem.ChargenRole = dbitem;
				newItem.CurrencyId = item.Key.Id;
				newItem.Amount = item.Value;
				FMDB.Context.ChargenRolesCurrencies.Add(newItem);
			}

			FMDB.Context.ChargenRolesTraits.RemoveRange(dbitem.ChargenRolesTraits);
			foreach (var item in TraitAdjustments)
			{
				var newItem = new ChargenRolesTrait();
				newItem.ChargenRole = dbitem;
				newItem.TraitId = item.Key.Id;
				newItem.Amount = item.Value.amount;
				newItem.GiveIfDoesntHave = item.Value.giveIfMissing;
				FMDB.Context.ChargenRolesTraits.Add(newItem);
			}

			foreach (var membership in dbitem.ChargenRolesClanMemberships)
			{
				FMDB.Context.ChargenRolesClanMembershipsAppointments.RemoveRange(membership.ChargenRolesClanMembershipsAppointments);
			}
			FMDB.Context.ChargenRolesClanMemberships.RemoveRange(dbitem.ChargenRolesClanMemberships);
			foreach (var item in ClanMemberships)
			{
				var newItem = new ChargenRolesClanMemberships();
				newItem.ChargenRole = dbitem;
				newItem.ClanId = item.Clan.Id;
				newItem.RankId = item.Rank.Id;
				newItem.PaygradeId = item.Paygrade?.Id;
				FMDB.Context.ChargenRolesClanMemberships.Add(newItem);
				foreach (var appointment in item.Appointments)
				{
					var newAppointment = new ChargenRolesClanMembershipsAppointments();
					newAppointment.ChargenRolesClanMembership = newItem;
					newAppointment.AppointmentId = appointment.Id;
					FMDB.Context.ChargenRolesClanMembershipsAppointments.Add(newAppointment);
				}

			}

			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}

	public override string FrameworkItemType => "ChargenRole";

	private void LoadFromDatabase(MudSharp.Models.ChargenRole role)
	{
		_id = role.Id;
		_name = role.Name;
		RoleType = (ChargenRoleType)role.Type;
		ChargenBlurb = role.ChargenBlurb;
		PosterId = role.PosterId;
		RequiredApprovers = role.ChargenRolesApprovers.Select(x => x.Approver.Name).ToList();
		MaximumNumberAlive = role.MaximumNumberAlive;
		MaximumNumberTotal = role.MaximumNumberTotal;
		MinimumPermissionToApprove = (PermissionLevel)role.MinimumAuthorityToApprove;
		MinimumPermissionToView = (PermissionLevel)role.MinimumAuthorityToView;
		AvailabilityProg = Gameworld.FutureProgs.Get(role.AvailabilityProgId ?? 0);
		_costs.AddRange(role.ChargenRolesCosts.Select(x =>
			new ChargenResourceCost
			{
				Resource = Gameworld.ChargenResources.Get(x.ChargenResourceId) ?? throw new InvalidOperationException(),
				RequirementOnly = x.RequirementOnly,
				Amount = x.Amount
			}));
		TraitAdjustments = role.ChargenRolesTraits.ToDictionary(x => Gameworld.Traits.Get(x.TraitId),
			x => (x.Amount, x.GiveIfDoesntHave));
		StartingCurrency = role.ChargenRolesCurrencies.ToDictionary(x => Gameworld.Currencies.Get(x.CurrencyId),
			x => x.Amount);
		_additionalMerits = role.ChargenRolesMerits.Select(x => Gameworld.Merits.Get(x.MeritId)).ToList();
		ClanMemberships =
			role.ChargenRolesClanMemberships.Select(x => (IRoleClanMembership)new RoleClanMembership(x, Gameworld))
				.ToList();
		Expired = role.Expired;
		foreach (var item in role.ChargenAdvicesChargenRoles)
		{
			_chargenAdvices.Add(Gameworld.ChargenAdvices.Get(item.ChargenAdviceId));
		}
	}

	#region IChargenRole Members

	public ChargenRoleType RoleType { get; set; }

	public string Poster
	{
		get
		{
			using (new FMDB())
			{
				var dbaccount = FMDB.Context.Accounts.Find(PosterId);
				return dbaccount?.Name ?? "System";
			}
		}
	}

	public long PosterId { get; set; }

	public List<string> RequiredApprovers { get; set; }

	public int MaximumNumberAlive { get; set; }

	public int MaximumNumberTotal { get; set; }

	public string ChargenBlurb { get; set; }

	public IFutureProg AvailabilityProg { get; set; }

	private List<ChargenResourceCost> _costs = new();

	public Dictionary<IChargenResource, int> Costs =>
		_costs.Where(x => !x.RequirementOnly).ToDictionary(x => x.Resource, x => x.Amount);

	public Dictionary<IChargenResource, int> Requirements =>
		_costs.Where(x => x.RequirementOnly).ToDictionary(x => x.Resource, x => x.Amount);

	public int ResourceCost(IChargenResource resource)
	{
		return Costs.ValueOrDefault(resource, 0);
	}

	public bool ChargenAvailable(ICharacterTemplate template)
	{
		return _costs.Where(x => x.RequirementOnly)
					 .All(x => template.Account.AccountResources[x.Resource] >= x.Amount) &&
			   (AvailabilityProg?.ExecuteBool(template) ?? true);
	}

	public Dictionary<ITraitDefinition, (double amount, bool giveIfMissing)> TraitAdjustments { get; set; }

	public Dictionary<ICurrency, decimal> StartingCurrency { get; set; }

	private List<IMerit> _additionalMerits;
	public IEnumerable<IMerit> AdditionalMerits => _additionalMerits;

	public List<IRoleClanMembership> ClanMemberships { get; set; }

	public bool Expired { get; set; }

	public void SetName(string name)
	{
		_name = name;
	}

	public PermissionLevel MinimumPermissionToApprove { get; set; }

	public PermissionLevel MinimumPermissionToView { get; set; }

	private readonly List<IChargenAdvice> _chargenAdvices = new();

	public IEnumerable<IChargenAdvice> ChargenAdvices => _chargenAdvices;

	public bool ToggleAdvice(IChargenAdvice advice)
	{
		Changed = true;
		if (_chargenAdvices.Contains(advice))
		{
			_chargenAdvices.Remove(advice);
			return false;
		}

		_chargenAdvices.Add(advice);
		return true;
	}
	#endregion

	#region Building Commands
	public const string BuildingHelpText = @"You can use the following options with this command:

	#3name <name>#0 - renames this role
	#3blurb#0 - drops you into an editor to enter the blurb in chargen
	#3type <type>#0 - changes the role type
	#3cost <resource> <amount>#0 - sets this role to cost a certain amount of resource
	#3cost <resource> 0#0 - removes a cost from this role
	#3prog <which>#0 - sets the prog that controls whether this appears as an option in chargen
	#3prog clear#0 - removes a prog; role will always be available
	#3alive <##>#0 - sets the maximum number of living characters who can have this role
	#3alive 0#0 - removes any maximum number of living characters restriction
	#3total <##>#0 - sets the maximum number of total characters who may ever take this role
	#3total 0#0 - removes any maximum number of total characters restriction
	#3approvers <account1> [<account2>] ...#0 - sets mandatory approvers for this role
	#3approvers clear#0 - removes mandatory approvers from this role
	#3permission <level>#0 - sets a permission level required to approve this role
	#3view <level>#0 - sets a permission level required to view this role as a builder
	#3expired#0 - toggles this role being expired and no longer available
	#3clan <clan> rank <rank>#0 - sets a rank in a clan that is given (also adds a clan)
	#3clan <clan> remove#0 - removes a clan membership given by this role
	#3clan <clan> paygrade <paygrade>#0 - sets a paygrade in a clan membership given by this role
	#3clan <clan> appointment <which>#0 - toggles an appointment in a clan membership given by this role
	#3currency <which> <amount>#0 - sets currency to be given to the character at commencement
	#3currency <which> 0#0 - removes currency given by this role
	#3trait <which> <boost>#0 - sets a boost or penalty to an attribute or trait at commencement
	#3trait <which> 0#0 - removes a boost or penalty  to an attribute or trait at commencement
	#3trait <which> <boost> boost#0 - gives the boost only if the attribute or skill is already possessed
	#3merit <which>#0 - toggles this role giving a merit or flaw";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "type":
				return BuildingCommandType(actor, command);
			case "description":
			case "desc":
			case "blurb":
				return BuildingCommandDescription(actor, command);
			case "cost":
			case "costs":
				return BuildingCommandCost(actor, command);
			case "prog":
				return BuildingCommandProg(actor, command);
			case "alive":
			case "maxalive":
			case "max alive":
			case "maximum alive":
				return BuildingCommandAlive(actor, command);
			case "total":
			case "maxtotal":
			case "max total":
			case "maximum total":
				return BuildingCommandTotal(actor, command);
			case "approver":
			case "approvers":
				return BuildingCommandApprovers(actor, command);
			case "approval permission":
			case "approve permission":
			case "permission":
				return BuildingCommandApprovePermission(actor, command);
			case "view permission":
			case "view":
				return BuildingCommandViewPermission(actor, command);
			case "expired":
			case "expire":
				return BuildingCommandExpired(actor, command);
			case "clans":
			case "clan":
				return BuildingCommandClans(actor, command);
			case "currency":
				return BuildingCommandCurrency(actor, command);
			case "traits":
			case "trait":
				return BuildingCommandTraits(actor, command);
			case "merit":
			case "merits":
			case "quirk":
			case "quirks":
			case "flaw":
			case "flaws":
				return BuildingCommandMerit(actor, command);
			default:
				actor.OutputHandler.Send(BuildingHelpText.SubstituteANSIColour());
				return false;
		}
	}

	#region Building Subcommands
	private bool BuildingCommandName(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("What name do you want to give to this role?");
			return false;
		}

		actor.Send("The {0} role #{1} is now known as {2}.", RoleType.ToString(),
			Id.ToString("N0", actor).Colour(Telnet.Green),
			ss.SafeRemainingArgument.TitleCase().Colour(Telnet.Green));
		SetName(ss.SafeRemainingArgument.TitleCase());
		Changed = true;
		return true;
	}

	private bool BuildingCommandType(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send($"Which type of role do you set this one too? The valid types are {Enum.GetValues<ChargenRoleType>().Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
			return false;
		}

		if (!ss.SafeRemainingArgument.TryParseEnum<ChargenRoleType>(out var type))
		{
			actor.OutputHandler.Send($"That is not a valid role type. The valid types are {Enum.GetValues<ChargenRoleType>().Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
			return false;
		}

		RoleType = type;
		Changed = true;
		actor.OutputHandler.Send($"This role is now a {type.DescribeEnum().ColourName()} role.");
		return true;
	}

	private void BuildingCommandDescriptionPost(string message, IOutputHandler handler, object[] parameters)
	{
		var actor = (ICharacter)parameters[0];
		ChargenBlurb = message;
		Changed = true;
		handler.Send(
			$"You change the description of the {RoleType} role {Name.TitleCase().Colour(Telnet.Green)} (#{Id.ToString("N0", actor).Colour(Telnet.Green)}) to:\n\n{message.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength, "\t")}");
	}

	private void BuildingCommandDescriptionCancel(IOutputHandler handler, object[] parameters)
	{
		handler.Send("You decline to edit the description of the ");

	}

	private bool BuildingCommandDescription(ICharacter actor, StringStack ss)
	{
		if (!string.IsNullOrEmpty(ChargenBlurb))
		{
			actor.OutputHandler.Send($"Replacing:\n\n{ChargenBlurb.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength, "\t")}\n");
		}

		actor.OutputHandler.Send("Enter the description in the editor below.");
		actor.EditorMode(BuildingCommandDescriptionPost, BuildingCommandDescriptionCancel, 1.0, null, EditorOptions.None,
			new object[] { actor });
		return true;
	}

	private bool BuildingCommandCost(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send(
				"How much and which resources should this role cost?\nn.b.: You can remove an existing cost by setting it to 0.");
			return false;
		}

		if (!int.TryParse(ss.Pop(), out var value))
		{
			actor.Send("You must enter a valid amount for this resource to cost.");
			return false;
		}

		if (ss.IsFinished)
		{
			actor.Send("Which resource would you like to set the costs of this role for?");
			return false;
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
			return false;
		}

		if (value == 0)
		{
			if (!Costs.ContainsKey(resource) || Costs[resource] == 0)
			{
				actor.Send("That role does not currently cost any {0}.", resource.PluralName);
				return false;
			}

			Costs.Remove(resource);
			Changed = true;
			actor.Send("The {0} role will no longer cost any {1}.",
				Name.TitleCase().Colour(Telnet.Green),
				resource.PluralName.TitleCase().Colour(Telnet.Green)
			);
			return false;
		}

		Costs[resource] = value;
		Changed = true;
		actor.Send("The {0} role will now cost {1} {2}.",
			Name.TitleCase().Colour(Telnet.Green),
			value.ToString("N0", actor).Colour(Telnet.Green),
			(value == 1 ? resource.Name : resource.PluralName).TitleCase().Colour(Telnet.Green)
		);
		return true;
	}

	private bool BuildingCommandProg(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Do you want to this role to have a particular eligibility prog, or clear the existing one?");
			return false;
		}

		var argText = ss.Pop();
		if (argText.Equals("clear", StringComparison.InvariantCultureIgnoreCase))
		{
			if (AvailabilityProg == null)
			{
				actor.Send("That role does not have an availability prog to clear.");
				return false;
			}

			AvailabilityProg = null;
			Changed = true;
			actor.Send(
				"You clear the availability prog from the {0} role. It will now always be available for selection.",
				Name.TitleCase().Colour(Telnet.Green));
			return false;
		}

		var prog = long.TryParse(argText, out var value)
			? actor.Gameworld.FutureProgs.Get(value)
			: actor.Gameworld.FutureProgs.FirstOrDefault(
				x => x.FunctionName.Equals(argText, StringComparison.InvariantCultureIgnoreCase));
		if (prog == null)
		{
			actor.Send("There is no such prog.");
			return false;
		}

		if (prog.ReturnType != ProgVariableTypes.Boolean ||
			!prog.MatchesParameters(new[] { ProgVariableTypes.Chargen }))
		{
			actor.Send(
				"The prog must return a boolean and accept a single chargen parameter, whereas {1} (#{2:N0}) does not.",
				prog.FunctionName,
				prog.Id
			);
			return false;
		}

		AvailabilityProg = prog;
		Changed = true;
		actor.Send("The {0} role will now use the {1} prog to determine availability in chargen.",
			Name.TitleCase().Colour(Telnet.Green),
			prog.MXPClickableFunctionName()
		);
		return true;
	}

	private bool BuildingCommandAlive(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("How many simultaneous alive characters may take this role?");
			return false;
		}

		if (!int.TryParse(ss.Pop(), out var value))
		{
			actor.Send("You must enter a number.");
			return false;
		}

		if (value < 1)
		{
			MaximumNumberAlive = 0;
			Changed = true;
			actor.Send(
				"The {0} role no longer has any restrictions on how many living characters can hold it at one time.",
				Name.TitleCase().Colour(Telnet.Green)
			);
			return false;
		}

		MaximumNumberAlive = value;
		Changed = true;
		actor.Send(
			"The {0} role will now become ineligible for selection when there are {1:N0} living characters with the ",
			Name.TitleCase().Colour(Telnet.Green),
			value
		);
		return true;
	}

	private bool BuildingCommandTotal(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("How many total characters may take this role?");
			return false;
		}

		if (!int.TryParse(ss.Pop(), out var value))
		{
			actor.Send("You must enter a number.");
			return false;
		}

		if (value < 1)
		{
			MaximumNumberTotal = 0;
			Changed = true;
			actor.Send("The {0} role no longer has any restrictions on how many characters can hold it in total.",
				Name.TitleCase().Colour(Telnet.Green)
			);
			return false;
		}

		MaximumNumberTotal = value;
		Changed = true;
		actor.Send(
			"The {0} role will now become ineligible for selection when there are {1:N0} total characters with the ",
			Name.TitleCase().Colour(Telnet.Green),
			value
		);
		return true;
	}

	private bool BuildingCommandApprovers(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send(
				"You must either specify approvers for this role, or use clear to clear all current approvers.");
			return false;
		}

		if (ss.SafeRemainingArgument.Equals("clear", StringComparison.InvariantCultureIgnoreCase))
		{
			if (!RequiredApprovers.Any())
			{
				actor.OutputHandler.Send($"The {Name.TitleCase().Colour(Telnet.Green)} role does not require any specific approvers.");
				return false;
			}

			RequiredApprovers.Clear();
			Changed = true;
			actor.OutputHandler.Send($"The {Name.TitleCase().Colour(Telnet.Green)} role no longer requires any specific approver.");
			return false;
		}

		var names = ss.SafeRemainingArgument.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
		using (new FMDB())
		{
			var accnames = FMDB.Context.Accounts.Select(x => x.Name).AsNoTracking().ToHashSet();
			foreach (var name in names)
			{
				if (!accnames.Contains(name))
				{
					actor.OutputHandler.Send($"There is no account called {name.ColourName()}.");
					return false;
				}
			}
		}

		RequiredApprovers.Clear();
		RequiredApprovers.AddRange(names);
		Changed = true;
		actor.Send("Applications selecting the {0} role will now be required to be approved by {1}.",
			Name.TitleCase().Colour(Telnet.Green),
			names.Select(x => x.Proper()).ListToString(conjunction: "or ")
		);
		return true;
	}

	private bool BuildingCommandApprovePermission(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send(
				"What permission level do you want to set for the approval of this role?\nn.b.: use \"any\" for no permissions required.");
			return false;
		}

		if (!ss.PopSpeech().TryParseEnum(out PermissionLevel level))
		{
			actor.Send("That is not a valid permission level. See {0}.",
				"show permissions".Colour(Telnet.Yellow).FluentTagMXP("send", "href='show permissions'"));
			return false;
		}

		MinimumPermissionToApprove = level;
		Changed = true;
		actor.Send("The {0} role will now require a permission level of {1} to approve.",
			Name.TitleCase().Colour(Telnet.Green), level.Describe().Colour(Telnet.Green));
		return true;
	}

	private bool BuildingCommandViewPermission(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send(
				"What permission level do you want to set for the viewing of this role?\nn.b.: use \"any\" for no permissions required.");
			return false;
		}

		if (!ss.PopSpeech().TryParseEnum<PermissionLevel>(out var level))
		{
			actor.Send("That is not a valid permission level. See {0}.",
				"show permissions".Colour(Telnet.Yellow).FluentTagMXP("send", "href='show permissions'"));
			return false;
		}

		if (level > actor.PermissionLevel)
		{
			actor.Send("You cannot make a role with view permissions higher than your own.");
			return false;
		}

		MinimumPermissionToView = level;
		Changed = true;
		actor.Send("The {0} role will now require a permission level of {1} to view.",
			Name.TitleCase().Colour(Telnet.Green), level.Describe().Colour(Telnet.Green));
		return true;
	}

	private bool BuildingCommandExpired(ICharacter actor, StringStack ss)
	{
		actor.Send(
			Expired
				? "You change the {0} role to be active once again."
				: "You change the {0} role to be expired, and it will no longer appear in character creation.",
			Name.TitleCase().Colour(Telnet.Green));

		Expired = !Expired;
		Changed = true;
		return true;
	}

	private bool BuildingCommandClans(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a clan.");
			return false;
		}

		var clan = ClanModule.GetTargetClan(actor, ss.PopSpeech(), false);
		if (clan is null)
		{
			actor.OutputHandler.Send("That is not a valid clan.");
			return false;
		}

		switch (ss.PopSpeech())
		{
			case "rank":
				return BuildingCommandClanRank(actor, ss, clan);
			case "appointment":
				return BuildingCommandClanAppointment(actor, ss, clan);
			case "pay":
			case "paygrade":
				return BuildingCommandClanPaygrade(actor, ss, clan);
			case "remove":
				return BuildingCommandClanRemove(actor, clan);
		}

		actor.OutputHandler.Send(@"You can use the following options with this subcommand:

	#3rank <rank>#0 sets the rank in this clan, or adds a clan membership
	#3remove#0 - removes this clan membership
	#3paygrade <which>#0 - sets the paygrade level associated with the rank
	#3appointment <which>#0 - adds or removes an appointment with this clan".SubstituteANSIColour());
		return true;
	}

	private bool BuildingCommandClanRemove(ICharacter actor, IClan clan)
	{
		if (ClanMemberships.RemoveAll(x => x.Clan == clan) > 0)
		{
			Changed = true;
			actor.OutputHandler.Send($"This role will no longer give membership in the {clan.FullName.ColourName()} clan.");
			return true;
		}

		actor.OutputHandler.Send($"This role wasn't giving membership in the {clan.FullName.ColourName()} clan.");
		return true;
	}

	private bool BuildingCommandClanPaygrade(ICharacter actor, StringStack ss, IClan clan)
	{
		if (ClanMemberships.All(x => x.Clan != clan))
		{
			actor.OutputHandler.Send($"This role doesn't currently give any rank in the {clan.FullName.ColourName()} clan. You must give it a rank first.");
			return false;
		}

		var text = ss.SafeRemainingArgument;
		var paygrade = long.TryParse(text, out var id) ?
			clan.Paygrades.FirstOrDefault(x => x.Id == id)
			:
			clan.Paygrades.FirstOrDefault(x => x.Name.Equals(text, StringComparison.InvariantCultureIgnoreCase)) ??
			clan.Paygrades.FirstOrDefault(x => x.Abbreviation.Equals(text, StringComparison.InvariantCultureIgnoreCase)) ??
			clan.Paygrades.FirstOrDefault(x => x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase)) ??
			clan.Paygrades.FirstOrDefault(x => x.Abbreviation.StartsWith(text, StringComparison.InvariantCultureIgnoreCase))
			;
		if (paygrade is null)
		{
			actor.OutputHandler.Send($"The {clan.FullName.ColourName()} clan does not have any such paygrade.");
			return false;
		}

		var membership = ClanMemberships.First(x => x.Clan == clan);
		if (!membership.Rank.Paygrades.Contains(paygrade))
		{
			actor.OutputHandler.Send($"The {membership.Rank.Name.ColourName()} rank in {clan.FullName.ColourName()} currently granted by this role does not include the paygrade {paygrade.Name.ColourName()}.");
			return false;
		}

		membership.Paygrade = paygrade;
		Changed = true;
		actor.OutputHandler.Send($"This role now grants the paygrade {paygrade.Name.ColourName()} in {clan.FullName.ColourName()}.");
		return true;
	}

	private bool BuildingCommandClanAppointment(ICharacter actor, StringStack ss, IClan clan)
	{
		var text = ss.SafeRemainingArgument;
		var appointment = long.TryParse(text, out var id) ?
			clan.Appointments.FirstOrDefault(x => x.Id == id)
			:
			clan.Appointments.FirstOrDefault(x => x.Name.Equals(text, StringComparison.InvariantCultureIgnoreCase)) ??
			clan.Appointments.FirstOrDefault(x => x.Abbreviations.Any(y => y.Equals(text, StringComparison.InvariantCultureIgnoreCase))) ??
			clan.Appointments.FirstOrDefault(x => x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase)) ??
			clan.Appointments.FirstOrDefault(x => x.Abbreviations.Any(y => y.StartsWith(text, StringComparison.InvariantCultureIgnoreCase)))
			;
		if (appointment is null)
		{
			actor.OutputHandler.Send($"The {clan.FullName.ColourName()} clan does not have any such appointment.");
			return false;
		}

		var membership = ClanMemberships.FirstOrDefault(x => x.Clan == clan);
		if (membership is null)
		{
			membership = new RoleClanMembership(clan, appointment.MinimumRankToHold ?? clan.Ranks.MinBy(x => x.RankNumber));
			ClanMemberships.Add(membership);			
		}

		if (membership.Appointments.Any(x => x == appointment))
		{
			membership.Appointments.Remove(appointment);
			actor.OutputHandler.Send($"This role will no longer grant an appointment to the {appointment.Name.ColourName()} position in the {clan.FullName.ColourName()} clan.");
			Changed = true;
			return true;
		}

		membership.Appointments.Add(appointment);
		if ((appointment.MinimumRankToHold?.RankNumber ?? 0) > membership.Rank.RankNumber)
		{
			membership.Rank = appointment.MinimumRankToHold;
			membership.Paygrade = null;
		}
		Changed = true;
		actor.OutputHandler.Send($"This role will now grants an appointment to the {appointment.Name.ColourName()} position in the {clan.FullName.ColourName()} clan.");
		return true;
	}

	private bool BuildingCommandClanRank(ICharacter actor, StringStack ss, IClan clan)
	{
		var text = ss.SafeRemainingArgument;
		var rank = long.TryParse(text, out var id) ?
			clan.Ranks.FirstOrDefault(x => x.Id == id)
			:
			clan.Ranks.FirstOrDefault(x => x.Name.Equals(text, StringComparison.InvariantCultureIgnoreCase)) ??
			clan.Ranks.FirstOrDefault(x => x.Abbreviations.Any(y => y.Equals(text, StringComparison.InvariantCultureIgnoreCase))) ??
			clan.Ranks.FirstOrDefault(x => x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase)) ??
			clan.Ranks.FirstOrDefault(x => x.Abbreviations.Any(y => y.StartsWith(text, StringComparison.InvariantCultureIgnoreCase)))
			;
		if (rank is null)
		{
			actor.OutputHandler.Send($"The {clan.FullName.ColourName()} clan does not have any such rank.");
			return false;
		}

		var membership = ClanMemberships.FirstOrDefault(x => x.Clan == clan);
		if (membership is null)
		{
			membership = new RoleClanMembership(clan, rank);
			ClanMemberships.Add(membership);
		}

		var minimumRank = membership.Appointments.Select(x => x.MinimumRankToHold).DefaultIfEmpty(clan.Ranks.MinBy(x => x.RankNumber)).MaxBy(x => x.RankNumber);
		if (minimumRank.RankNumber > rank.RankNumber)
		{
			actor.OutputHandler.Send($"Due to appointments, the minimum rank would be {rank.Name.ColourName()} in the {clan.FullName.ColourName()} clan.");
			return false;
		}

		membership.Rank = rank;
		if (membership.Paygrade is not null && !rank.Paygrades.Contains(membership.Paygrade))
		{
			membership.Paygrade = null;
		}
		Changed = true;
		actor.OutputHandler.Send($"This role will now grant the rank of {rank.Name.ColourName()} in the {clan.FullName.ColourName()} clan.");
		return true;
	}

	private bool BuildingCommandTraits(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a skill or attribute to boost.");
			return false;
		}

		var trait = Gameworld.Traits.GetByName(ss.PopSpeech());
		if (trait is null)
		{
			actor.OutputHandler.Send("There is no such trait.");
			return false;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("How much should that trait be modified? Use 0 to remove an existing modification.");
			return false;
		}

		if (!double.TryParse(ss.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send("That is not a valid number.");
			return false;
		}

		if (value == 0.0)
		{
			TraitAdjustments.Remove(trait);
			Changed = true;
			actor.OutputHandler.Send($"There will be no modification to the {trait.Name.ColourName()} trait from this role.");
			return true;
		}

		if (!ss.IsFinished)
		{
			if (ss.SafeRemainingArgument.EqualToAny("boost"))
			{
				TraitAdjustments[trait] = (value, false);
				Changed = true;
				actor.OutputHandler.Send($"This role will now give people a {value.ToBonusString(actor)} bonus to the {trait.Name.ColourName()} trait, but only if they already have it.");
				return true;
			}
		}

		TraitAdjustments[trait] = (value, true);
		Changed = true;
		actor.OutputHandler.Send($"This role will now give people a {value.ToBonusString(actor)} bonus to the {trait.Name.ColourName()} trait.");
		return true;
	}

	private bool BuildingCommandCurrency(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which currency would you like to change?");
			return false;
		}

		var currency = Gameworld.Currencies.GetByName(ss.PopSpeech());
		if (currency is null)
		{
			actor.OutputHandler.Send("There is no such currency.");
			return false;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send($"How much {currency.Name.ColourName()} should be given to people with this role at character creation? Use 0 to remove an existing grant.");
			return false;
		}

		if (!currency.TryGetBaseCurrency(ss.SafeRemainingArgument, out var amount))
		{
			actor.OutputHandler.Send($"That is not a valid amount of {currency.Name.ColourName()}.");
			return false;
		}

		if (amount <= 0)
		{
			StartingCurrency.Remove(currency);
			Changed = true;
			actor.OutputHandler.Send($"No {currency.Name.ColourName()} will be given to characters with this role.");
			return true;
		}

		StartingCurrency[currency] = amount;
		Changed = true;
		actor.OutputHandler.Send($"Characters with this role will now receive {currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} when starting.");
		return true;
	}

	private bool BuildingCommandMerit(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which merit or flaw would you like to toggle being given by this role?");
			return false;
		}

		var merit = Gameworld.Merits.GetByIdOrName(command.SafeRemainingArgument);
		if (merit is null)
		{
			actor.OutputHandler.Send("There is no such merit or flaw.");
			return false;
		}

		if (AdditionalMerits.Contains(merit))
		{
			_additionalMerits.Remove(merit);
			actor.OutputHandler.Send($"This role will no longer give the {merit.Name.ColourName()} merit.");
		}
		else
		{
			_additionalMerits.Add(merit);
			actor.OutputHandler.Send($"This role will now give the {merit.Name.ColourName()} merit.");
		}
		Changed = true;
		return true;
	}
	#endregion

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine(string.Format(actor, "Role #{0:N0}".Colour(Telnet.Cyan), Id));
		sb.AppendLine();
		sb.AppendLine(string.Format(actor, "Name: {0}", Name.ColourValue()));
		sb.Append(new[]
		{
			$"Type: {RoleType.ToString().Colour(Telnet.Green)}",
			$"Current: {(Expired ? "no".Colour(Telnet.Red) : "yes".Colour(Telnet.Green))}"
		}.ArrangeStringsOntoLines(2, (uint)actor.Account.LineFormatLength));
		sb.Append(new[]
		{
			$"Creator: {Poster.Proper().Colour(Telnet.Green)}",
			$"Approvers: {RequiredApprovers.Select(x => x.Colour(Telnet.Green)).ListToString()}"
		}.ArrangeStringsOntoLines(2, (uint)actor.Account.LineFormatLength));
		sb.Append(new[]
		{
			$"Approval Permission: {MinimumPermissionToApprove.Describe().Colour(Telnet.Green)}",
			$"View Permission: {MinimumPermissionToView.Describe().Colour(Telnet.Green)}"
		}.ArrangeStringsOntoLines(2, (uint)actor.Account.LineFormatLength));
		sb.Append(new[]
		{
			$"Max Alive: {(MaximumNumberAlive == 0 ? "unlimited" : MaximumNumberAlive.ToString("N0", actor)).Colour(Telnet.Green)}",
			$"Max Total: {(MaximumNumberTotal == 0 ? "unlimited" : MaximumNumberTotal.ToString("N0", actor)).Colour(Telnet.Green)}"
		}.ArrangeStringsOntoLines(2, (uint)actor.Account.LineFormatLength));
		sb.AppendLine(
			$"Availability Prog: {(AvailabilityProg != null ? string.Format("{0} (#{1:N0})".FluentTagMXP("send", $"href='show futureprog {AvailabilityProg.Id}'"), AvailabilityProg.FunctionName, AvailabilityProg.Id) : "None".Colour(Telnet.Red))}");
		sb.AppendLine();
		sb.AppendLine("Blurb:");
		sb.AppendLine();
		sb.AppendLine(ChargenBlurb.SubstituteANSIColour().Wrap(80, "\t"));

		if (Costs.Any())
		{
			sb.AppendLine();
			sb.AppendLine(
				$"Costs: {Costs.Select(x => string.Format(actor, "{0:N0} {1}", x.Value, (x.Value == 1 ? x.Key.Name : x.Key.PluralName).TitleCase())).ListToString()}");
		}

		if (ClanMemberships.Any())
		{
			sb.AppendLine();
			sb.AppendLine("Clan Memberships:");
			sb.AppendLine();
			foreach (var clan in ClanMemberships)
			{
				sb.AppendLine(
					$"\tClan: {clan.Clan.FullName.TitleCase().Colour(Telnet.Green)} Rank: {clan.Rank.Name.TitleCase().Colour(Telnet.Green)} Paygrade: {(clan.Paygrade != null ? clan.Paygrade.Name.TitleCase().Colour(Telnet.Green) : "None".Colour(Telnet.Red))} Appointments: {(clan.Appointments.Any() ? clan.Appointments.Select(x => x.Name.TitleCase().Colour(Telnet.Green)).ListToString() : "None".Colour(Telnet.Red))}");
			}
		}

		if (TraitAdjustments.Any())
		{
			sb.AppendLine();
			sb.AppendLine("Trait Adjustments:");
			sb.AppendLine();
			foreach (var adjust in TraitAdjustments)
			{
				sb.AppendLine(
					$"\tTrait: {adjust.Key.Name.TitleCase().Colour(Telnet.Green)} Adjustment: {adjust.Value.amount.ToString("N2", actor).Colour(Telnet.Green)}{(adjust.Value.giveIfMissing ? "" : " (boost only)")}");
			}
		}

		if (StartingCurrency.Any())
		{
			sb.AppendLine();
			sb.AppendLine("Starting Currency:");
			sb.AppendLine();
			foreach (var item in StartingCurrency)
			{
				sb.AppendLine(
					$"\tCurrency: {item.Key.Name.TitleCase().Colour(Telnet.Green)} Amount: {item.Key.Describe(item.Value, CurrencyDescriptionPatternType.Short).Colour(Telnet.Green)}");
			}
		}

		if (AdditionalMerits.Any())
		{
			sb.AppendLine();
			sb.AppendLine("Additional Merits:");
			sb.AppendLine();
			foreach (var merit in AdditionalMerits)
			{
				sb.AppendLine($"\t#{merit.Id.ToString("N0", actor)} ({merit.Name.ColourName()})");
			}
		}

		return sb.ToString();
	}
	#endregion

	#region IFutureProgVariableType Implementation

	ProgVariableTypes IProgVariable.Type => ProgVariableTypes.Role;

	public object GetObject => this;

	private static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", ProgVariableTypes.Number },
			{ "name", ProgVariableTypes.Text },
			{ "roletype", ProgVariableTypes.Number }
		};
	}

	private new static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "" },
			{ "name", "" },
			{ "roletype", "" }
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Role, DotReferenceHandler(),
			DotReferenceHelp());
	}

	public IProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "id":
				return new NumberVariable(Id);
			case "name":
				return new TextVariable(Name);
			case "roletype":
				return new NumberVariable((int)RoleType);
		}

		throw new NotImplementedException();
	}

	#endregion
}