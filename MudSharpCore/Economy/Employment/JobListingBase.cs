using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MoreLinq.Extensions;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Commands.Modules;
using MudSharp.Commands.Trees;
using MudSharp.Community;
using MudSharp.Database;
using MudSharp.Economy.Banking;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Time;
using MudSharp.Work.Projects;
using Org.BouncyCastle.Utilities.IO.Pem;
using Pomelo.EntityFrameworkCore.MySql.Query.ExpressionVisitors.Internal;

namespace MudSharp.Economy.Employment;
#nullable enable
public abstract class JobListingBase : SaveableItem, IJobListing
{
	protected JobListingBase(Models.JobListing dbitem, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = dbitem.Id;
		_name = dbitem.Name;
		Description = dbitem.Description;
		EconomicZone = Gameworld.EconomicZones.Get(dbitem.EconomicZoneId)!;
		EligibilityProg = Gameworld.FutureProgs.Get(dbitem.EligibilityProgId) ??
		                  Gameworld.FutureProgs.Get(Gameworld.GetStaticLong("AlwaysTrueProg"))!;
		;
		_employerReference = new FrameworkItemReference(dbitem.PosterId, dbitem.PosterType, gameworld);
		MaximumDuration = string.IsNullOrEmpty(dbitem.MaximumDuration)
			? default
			: MudTimeSpan.Parse(dbitem.MaximumDuration);
		MaximumNumberOfSimultaneousEmployees = dbitem.MaximumNumberOfSimultaneousEmployees;
		ClanMembership = Gameworld.Clans.Get(dbitem.ClanId ?? 0L);
		ClanRank = ClanMembership?.Ranks.Get(dbitem.RankId ?? 0L);
		ClanAppointment = ClanMembership?.Appointments.Get(dbitem.AppointmentId ?? 0L);
		ClanPaygrade = ClanMembership?.Paygrades.Get(dbitem.PaygradeId ?? 0L);
		PersonalProject = Gameworld.Projects.Get(dbitem.PersonalProjectId ?? 0L);
		RequiredProject = Gameworld.ActiveProjects.Get(dbitem.RequiredProjectId ?? 0L) as ILocalProject;
		RequiredProjectLabour =
			RequiredProject?.CurrentPhase?.LabourRequirements.FirstOrDefault(x =>
				x.Id == (dbitem.RequiredProjectLabourId ?? 0L));
		BankAccount = Gameworld.BankAccounts.Get(dbitem.BankAccountId ?? 0L);
		IsArchived = dbitem.IsArchived;
		IsReadyToBePosted = dbitem.IsReadyToBePosted;
		FullTimeEquivalentRatio = dbitem.FullTimeEquivalentRatio;
		foreach (var money in XElement.Parse(dbitem.MoneyPaidIn).Elements("Money"))
		{
			MoneyPaidIn[Gameworld.Currencies.Get(long.Parse(money.Attribute("currency").Value))] =
				decimal.Parse(money.Attribute("amount").Value);
		}
	}

	protected JobListingBase(IFuturemud gameworld, string name, IEconomicZone economicZone, IFrameworkItem employer,
		string definition, string listingType)
	{
		Gameworld = gameworld;
		IsReadyToBePosted = false;
		_name = name;
		EconomicZone = economicZone;
		_employerReference = new FrameworkItemReference(employer.Id, employer.FrameworkItemType, gameworld);
		_employer = employer;
		Description = Gameworld.GetStaticString("DefaultJobDescription");
		EligibilityProg = Gameworld.FutureProgs.Get(Gameworld.GetStaticLong("AlwaysTrueProg"))!;
		FullTimeEquivalentRatio = 1.0;
		using (new FMDB())
		{
			var dbitem = new Models.JobListing
			{
				Name = Name,
				JobListingType = listingType,
				Description = Description,
				IsReadyToBePosted = false,
				EligibilityProgId = EligibilityProg.Id,
				MaximumNumberOfSimultaneousEmployees = MaximumNumberOfSimultaneousEmployees,
				Definition = definition,
				IsArchived = false,
				EconomicZoneId = economicZone.Id,
				PosterId = employer.Id,
				PosterType = employer.FrameworkItemType,
				MoneyPaidIn = "<Monies/>",
				FullTimeEquivalentRatio = FullTimeEquivalentRatio
			};
			FMDB.Context.JobListings.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public void Delete()
	{
		OnDelete();
		Gameworld.Destroy(this);
		foreach (var job in ActiveJobs)
		{
			job.Delete();
		}

		Gameworld.SaveManager.Abort(this);
		if (_id != 0)
		{
			using (new FMDB())
			{
				Gameworld.SaveManager.Flush();
				var dbitem = FMDB.Context.JobListings.Find(Id);
				if (dbitem != null)
				{
					FMDB.Context.JobListings.Remove(dbitem);
					FMDB.Context.SaveChanges();
				}
			}
		}
	}

	protected virtual void OnDelete()
	{
		// Do nothing unless overriden
	}

	#region Overrides of FrameworkItem

	public sealed override string FrameworkItemType => "JobListing";

	#endregion

	#region Overrides of SaveableItem

	public sealed override void Save()
	{
		var dbitem = FMDB.Context.JobListings.Find(Id);
		dbitem.Name = Name;
		dbitem.IsArchived = IsArchived;
		dbitem.IsReadyToBePosted = IsReadyToBePosted;
		dbitem.Description = Description;
		dbitem.EconomicZoneId = EconomicZone.Id;
		dbitem.EligibilityProgId = EligibilityProg.Id;
		dbitem.RequiredProjectLabourId = RequiredProjectLabour?.Id;
		dbitem.RequiredProjectId = RequiredProject?.Id;
		dbitem.PersonalProjectId = PersonalProject?.Id;
		dbitem.PersonalProjectRevisionNumber = PersonalProject?.RevisionNumber;
		dbitem.ClanId = ClanMembership?.Id;
		dbitem.RankId = ClanRank?.Id;
		dbitem.PaygradeId = ClanPaygrade?.Id;
		dbitem.AppointmentId = ClanAppointment?.Id;
		dbitem.MaximumNumberOfSimultaneousEmployees = MaximumNumberOfSimultaneousEmployees;
		dbitem.MaximumDuration = MaximumDuration?.GetRoundTripParseText;
		dbitem.BankAccountId = BankAccount?.Id;
		dbitem.FullTimeEquivalentRatio = FullTimeEquivalentRatio;
		dbitem.MoneyPaidIn = new XElement("Moneys", from item in MoneyPaidIn
		                                            select new XElement("Money",
			                                            new XAttribute("currency", item.Key.Id),
			                                            new XAttribute("amount", item.Value))).ToString();
		dbitem.Definition = SaveDefinition().ToString();
		Changed = false;
	}

	protected abstract XElement SaveDefinition();

	#endregion

	#region Implementation of IEditableItem

	protected virtual string HelpInfo => @"You can use the following options to edit this job listing:

	#3name <name>#0 - renames this job listing
	#3desc#0 - drops you into an editor to write a description for this job
	#3bank <account>#0 - sets the bank account that backs payroll
	#3bank none#0 - clears the bank account (using cash reserves only)
	#3ratio <verycasual|parttime|fulltime|overtime|punishing>#0 - sets the job effort ratio
	#3employees#0 - permits an unlimited number of simultaneous employees
	#3employees <##>#0 - sets the maximum number of simultaneous employees
	#3clan <name>#0 - sets a clan that employees get membership in
	#3clan none#0 - clears the clan from this job
	#3rank <name>#0 - sets a clan rank for employees
	#3rank none#0 - clears the rank from this job
	#3paygrade <name>#0 - sets a clan paygrade for employees
	#3paygrade none#0 - clears the paygrade from this job
	#3appointment <name>#0 - sets a clan appointment for employees
	#3appointment none#0 - clears the appointment from this job
	#3term <time>#0 - sets the maximum term employees can hold this job
	#3term#0 - clears the term limit from this job
	#3prog <prog>#0 - sets a prog that controls who can hold this job";

	public virtual bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "description":
			case "desc":
				return BuildingCommandDescription(actor, command);
			case "bank":
			case "account":
			case "bankaccount":
				return BuildingCommandBankAccount(actor, command);
			case "effort":
			case "fte":
			case "ratio":
				return BuildingCommandEffort(actor, command);
			case "employees":
			case "count":
			case "maxemployees":
			case "maxcount":
			case "number":
			case "maxnumber":
				return BuildingCommandMaxEmployees(actor, command);
			case "clan":
				return BuildingCommandClan(actor, command);
			case "rank":
				return BuildingCommandRank(actor, command);
			case "appointment":
				return BuildingCommandAppointment(actor, command);
			case "paygrade":
				return BuildingCommandPaygrade(actor, command);
			case "duration":
			case "maxduration":
			case "term":
			case "fixedterm":
			case "maximumterm":
			case "fixedduration":
				return BuildingCommandMaximumDuration(actor, command);
			case "eligibilityprog":
			case "eligibility":
			case "prog":
			case "filter":
			case "filterprog":
				return BuildingCommandEligibilityProg(actor, command);
			default:
				actor.OutputHandler.Send(HelpInfo.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandEffort(ICharacter actor, StringStack command)
	{
		if (ActiveJobs.Any(x => !x.IsJobComplete))
		{
			actor.OutputHandler.Send(
				$"This setting cannot be changed while you have active workers. You should either make a new job listing or terminate all existing employment with this one.");
			return false;
		}

		switch (command.SafeRemainingArgument.ToLowerInvariant().CollapseString())
		{
			case "fulltime":
				FullTimeEquivalentRatio = 1.0;
				Changed = true;
				actor.OutputHandler.Send(
					$"This job is now equivalent to a full time job ({1.0.ToString("P", actor).ColourValue()} of a full time job's time commitment).");
				return true;
			case "parttime":
				FullTimeEquivalentRatio = 0.5;
				Changed = true;
				actor.OutputHandler.Send(
					$"This job is now equivalent to a part time job ({0.5.ToString("P", actor).ColourValue()} of a full time job's time commitment).");
				return true;
			case "overtime":
				FullTimeEquivalentRatio = 1.25;
				Changed = true;
				actor.OutputHandler.Send(
					$"This job is now equivalent to a full time job with overtime ({1.25.ToString("P", actor).ColourValue()} of a full time job's time commitment).");
				return true;
			case "punishing":
				FullTimeEquivalentRatio = 1.5;
				Changed = true;
				actor.OutputHandler.Send(
					$"This job is now equivalent to a full time job with a punishing amount of overtime ({1.5.ToString("P", actor).ColourValue()} of a full time job's time commitment).");
				return true;
			case "verycasual":
				FullTimeEquivalentRatio = 0.25;
				Changed = true;
				actor.OutputHandler.Send(
					$"This job is now equivalent to a very casual part time job ({0.25.ToString("P", actor).ColourValue()} of a full time job's time commitment).");
				return true;
			default:
				actor.OutputHandler.Send(
					$"The valid options are {new List<string> { "fulltime", "parttime", "overtime", "punishing", "verycasual" }.Select(x => x.ColourValue()).ListToString()}.");
				return false;
		}
	}

	private bool BuildingCommandDescription(ICharacter actor, StringStack command)
	{
		if (!string.IsNullOrEmpty(Description))
		{
			actor.OutputHandler.Send("Replacing:\n" +
			                         Description.ProperSentences().Wrap(actor.InnerLineFormatLength, "\t"));
		}

		actor.OutputHandler.Send("Enter the description in the editor below.");
		actor.EditorMode(PostAction, CancelAction, 1.0,
			suppliedArguments: new object[] { actor.InnerLineFormatLength });
		return true;
	}

	private void CancelAction(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to update the description of the job.");
	}

	private void PostAction(string text, IOutputHandler handler, object[] args)
	{
		Description = text.ProperSentences().Trim();
		Changed = true;
		handler.Send(
			$"You update the description of this job to:\n\n{Description.SubstituteANSIColour().Wrap((int)args[0])}");
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to this job listing?");
			return false;
		}

		_name = command.SafeRemainingArgument.TitleCase();
		Changed = true;
		actor.OutputHandler.Send($"This job listing is now called {_name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandEligibilityProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog do you want to set to control who can apply for this job?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Boolean, new FutureProgVariableTypes[]
			{
				FutureProgVariableTypes.Character
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		Changed = true;
		EligibilityProg = prog;
		actor.OutputHandler.Send(
			$"This job listing will now use the {prog.MXPClickableFunctionName()} prog to control who can apply for it.");
		return true;
	}

	private bool BuildingCommandMaximumDuration(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must either specify a time span or {"none".ColourCommand()} to remove the maximum duration.");
			return false;
		}

		if (command.PeekSpeech().EqualToAny("none", "reset", "clear", "delete", "remove"))
		{
			MaximumDuration = null;
			Changed = true;
			actor.OutputHandler.Send("This job listing will no longer have a maximum duration or tenure.");
			return true;
		}

		if (!MudTimeSpan.TryParse(command.SafeRemainingArgument, actor, out var timespan))
		{
			actor.OutputHandler.Send(
				$"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid timespan.");
			return false;
		}

		MaximumDuration = timespan;
		Changed = true;
		actor.OutputHandler.Send(
			$"The maximum amount of time people can hold this job will now be {timespan.Describe(actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandPaygrade(ICharacter actor, StringStack command)
	{
		if (ClanMembership is null || ClanRank is null)
		{
			actor.OutputHandler.Send("You must first set a clan rank for this listing before you can set paygrades.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must either specify a paygrade for the {ClanRank.Name.ColourValue()} rank in the {ClanMembership.FullName.ColourName()} clan or {"none".ColourCommand()} to clear one.");
			return false;
		}

		var membership = actor.ClanMemberships.FirstOrDefault(x => x.Clan == ClanMembership);
		if (!actor.IsAdministrator())
		{
			if (membership is null || !membership.NetPrivileges.HasFlag(ClanPrivilegeType.CanManageClanJobs))
			{
				actor.OutputHandler.Send(
					$"You are not permitted to manage jobs in the {ClanMembership.FullName.ColourName()} clan.");
				return false;
			}
		}

		if (command.PeekSpeech().EqualToAny("none", "clear", "reset", "delete", "remove"))
		{
			ClanPaygrade = null;
			Changed = true;
			actor.OutputHandler.Send($"This job will no longer confer a specific paygrade to employees.");
			return false;
		}

		var paygrade = ClanRank.Paygrades.GetByIdOrName(command.SafeRemainingArgument);
		if (paygrade is null)
		{
			actor.OutputHandler.Send(
				$"There is no such paygrade for the {ClanRank.Name.ColourValue()} rank in the {ClanMembership.FullName.ColourName()} clan.");
			return false;
		}

		if (!actor.IsAdministrator())
		{
			if (ClanRank.RankNumber > membership.Rank.RankNumber &&
			    !membership.NetPrivileges.HasFlag(ClanPrivilegeType.CanViewClanStructure))
			{
				actor.OutputHandler.Send(
					$"There is no rank identified by the text {command.SafeRemainingArgument.ColourCommand()} in the {ClanMembership.FullName.ColourName()} clan.");
				return false;
			}

			if (!membership.NetPrivileges.HasFlag(ClanPrivilegeType.CanIncreasePaygrade))
			{
				actor.OutputHandler.Send(
					$"You are not permitted to specify paygrades for jobs in the {ClanMembership.FullName.ColourName()} clan as you lack the {"CanIncreasePaygrade".ColourName()} privilege.");
				return false;
			}
		}

		ClanPaygrade = paygrade;
		Changed = true;
		actor.OutputHandler.Send(
			$"This job will now confer the {ClanPaygrade.Name.ColourValue()} paygrade to its employees.");
		return true;
	}

	private bool BuildingCommandAppointment(ICharacter actor, StringStack command)
	{
		if (ActiveJobs.Any(x => !x.IsJobComplete))
		{
			actor.OutputHandler.Send(
				$"This setting cannot be changed while you have active workers. You should either make a new job listing or terminate all existing employment with this one.");
			return false;
		}

		if (ClanMembership is null)
		{
			actor.OutputHandler.Send("You must first set a clan membership for this listing before you can set ranks.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must either specify an appointment in the {ClanMembership.FullName.ColourName()} clan or {"none".ColourCommand()} to clear one.");
			return false;
		}

		var membership = actor.ClanMemberships.FirstOrDefault(x => x.Clan == ClanMembership);
		if (!actor.IsAdministrator())
		{
			if (membership is null || !membership.NetPrivileges.HasFlag(ClanPrivilegeType.CanManageClanJobs))
			{
				actor.OutputHandler.Send(
					$"You are not permitted to manage jobs in the {ClanMembership.FullName.ColourName()} clan.");
				return false;
			}
		}

		if (command.PeekSpeech().EqualToAny("none", "clear", "reset", "delete", "remove"))
		{
			ClanAppointment = null;
			Changed = true;
			actor.OutputHandler.Send($"This job will no longer confer a clan appointment to employees.");
			return false;
		}

		var appointment = ClanMembership.Appointments.GetByIdOrName(command.SafeRemainingArgument);
		if (appointment is null)
		{
			actor.OutputHandler.Send(
				$"There is no such appointment in the {ClanMembership.FullName.ColourName()} clan.");
			return false;
		}

		if (!actor.IsAdministrator())
		{
			if (appointment.MinimumRankToHold.RankNumber > membership.Rank.RankNumber &&
			    !membership.NetPrivileges.HasFlag(ClanPrivilegeType.CanViewClanStructure))
			{
				actor.OutputHandler.Send(
					$"There is no such appointment in the {ClanMembership.FullName.ColourName()} clan.");
				return false;
			}

			if (!membership.NetPrivileges.HasFlag(ClanPrivilegeType.CanAppoint))
			{
				actor.OutputHandler.Send(
					$"You are not permitted to specify appointments for jobs in the {ClanMembership.FullName.ColourName()} clan as you lack the {"CanAppoint".ColourName()} privilege.");
				return false;
			}

			if (appointment.MinimumRankToAppoint.RankNumber > membership.Rank.RankNumber)
			{
				actor.OutputHandler.Send(
					$"You are not permitted to specify that appointment as it requires a minimum rank of {appointment.MinimumRankToAppoint.Name.ColourValue()}.");
				return false;
			}
		}

		ClanAppointment = appointment;
		Changed = true;
		actor.OutputHandler.Send(
			$"This job will now confer the {appointment.Name.ColourValue()} appointment in the {ClanMembership.FullName.ColourName()} clan to employees.");
		return true;
	}

	private bool BuildingCommandRank(ICharacter actor, StringStack command)
	{
		if (ClanMembership is null)
		{
			actor.OutputHandler.Send("You must first set a clan membership for this listing before you can set ranks.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must either specify a rank in the {ClanMembership.FullName.ColourName()} clan or {"none".ColourCommand()} to clear one.");
			return false;
		}

		var membership = actor.ClanMemberships.FirstOrDefault(x => x.Clan == ClanMembership);
		if (!actor.IsAdministrator())
		{
			if (membership is null || !membership.NetPrivileges.HasFlag(ClanPrivilegeType.CanManageClanJobs))
			{
				actor.OutputHandler.Send(
					$"You are not permitted to manage jobs in the {ClanMembership.FullName.ColourName()} clan.");
				return false;
			}
		}

		if (command.PeekSpeech().EqualToAny("none", "clear", "reset", "delete", "remove"))
		{
			ClanRank = null;
			ClanPaygrade = null;
			Changed = true;
			actor.OutputHandler.Send($"This job will now confer only the lowest possible clan rank to employees.");
			return false;
		}

		var rank = ClanMembership.Ranks.GetByIdOrName(command.SafeRemainingArgument);
		if (rank is null)
		{
			actor.OutputHandler.Send(
				$"There is no rank identified by the text {command.SafeRemainingArgument.ColourCommand()} in the {ClanMembership.FullName.ColourName()} clan.");
			return false;
		}

		if (!actor.IsAdministrator())
		{
			if (rank.RankNumber > membership.Rank.RankNumber &&
			    !membership.NetPrivileges.HasFlag(ClanPrivilegeType.CanViewClanStructure))
			{
				actor.OutputHandler.Send(
					$"There is no rank identified by the text {command.SafeRemainingArgument.ColourCommand()} in the {ClanMembership.FullName.ColourName()} clan.");
				return false;
			}

			if (!membership.NetPrivileges.HasFlag(ClanPrivilegeType.CanPromote) &&
			    !membership.NetPrivileges.HasFlag(ClanPrivilegeType.CanPromoteToOwnRank))
			{
				actor.OutputHandler.Send(
					$"You are not permitted to specify ranks for jobs in the {ClanMembership.FullName.ColourName()} clan as you lack the {"CanPromote".ColourName()} privilege.");
				return false;
			}

			if (rank.RankNumber > membership.Rank.RankNumber)
			{
				actor.OutputHandler.Send($"You are not permitted to specify ranks that are higher than your own rank.");
				return false;
			}

			if (rank.RankNumber == membership.Rank.RankNumber &&
			    !membership.NetPrivileges.HasFlag(ClanPrivilegeType.CanPromoteToOwnRank))
			{
				actor.OutputHandler.Send(
					$"You are not permitted to specify ranks that are the same rank as your own rank as you lack the {"CanPromoteToOwnRank".ColourName()} privilege.");
				return false;
			}
		}

		ClanRank = rank;
		ClanPaygrade = null;
		Changed = true;
		actor.OutputHandler.Send(
			$"This job will now confer the {rank.Name.ColourValue()} rank in the {ClanMembership.FullName.ColourName()} clan to employees.");
		return true;
	}

	private bool BuildingCommandClan(ICharacter actor, StringStack command)
	{
		if (ActiveJobs.Any(x => !x.IsJobComplete))
		{
			actor.OutputHandler.Send(
				$"This setting cannot be changed while you have active workers. You should either make a new job listing or terminate all existing employment with this one.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must either specify a clan that this job listing should give membership in, or use {"none".ColourCommand()} to clear a clan membership.");
			return false;
		}

		if (command.PeekSpeech().EqualToAny("clear", "reset", "none", "remove", "delete"))
		{
			actor.OutputHandler.Send($"This job listing will no longer confer membership in any clan.");
			ClanMembership = null;
			ClanRank = null;
			ClanPaygrade = null;
			ClanAppointment = null;
			return false;
		}

		var clan = ClanModule.GetTargetClan(actor, command.SafeRemainingArgument);
		if (clan is null)
		{
			actor.OutputHandler.Send(actor.IsAdministrator()
				? "There is no such clan."
				: "You are not a member of any such clan.");
			return false;
		}

		var membership = actor.ClanMemberships.FirstOrDefault(x => x.Clan == clan);
		if (!actor.IsAdministrator())
		{
			if (membership is null || !membership.NetPrivileges.HasFlag(ClanPrivilegeType.CanManageClanJobs))
			{
				actor.OutputHandler.Send(
					$"You are not permitted to manage jobs in the {clan.FullName.ColourName()} clan.");
				return false;
			}
		}

		ClanMembership = clan;
		ClanRank = null;
		ClanPaygrade = null;
		ClanAppointment = null;
		Changed = true;
		actor.OutputHandler.Send(
			$"This job listing will now confer membership in the {clan.FullName.ColourName()} clan.");
		return true;
	}

	private bool BuildingCommandMaxEmployees(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			MaximumNumberOfSimultaneousEmployees = 0;
			Changed = true;
			actor.OutputHandler.Send(
				"There will no longer be any limit to the number of employees who can work in this job.");
			return true;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var number) || number < 1)
		{
			actor.OutputHandler.Send(
				$"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid amount of employees.");
			return false;
		}

		MaximumNumberOfSimultaneousEmployees = number;
		Changed = true;
		actor.OutputHandler.Send(
			$"This job will now have a maximum of {MaximumNumberOfSimultaneousEmployees.ToString("N0", actor).ColourValue()} employee{(MaximumNumberOfSimultaneousEmployees == 1 ? "" : "s")} at a time.");
		return true;
	}

	private bool BuildingCommandBankAccount(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must either specify a bank account or use {"none".ColourCommand()} to remove a bank account.");
			return false;
		}

		if (command.PeekSpeech().EqualToAny("none", "remove", "delete", "clear"))
		{
			BankAccount = null;
			Changed = true;
			actor.OutputHandler.Send("This job will no longer use any bank account to back its payroll.");
			return true;
		}

		var (account, error) = Bank.FindBankAccount(command.SafeRemainingArgument, null, actor);
		if (account is null)
		{
			actor.OutputHandler.Send(error);
			return false;
		}

		if (!account.IsAuthorisedAccountUser(actor))
		{
			actor.OutputHandler.Send(
				$"You are not an authorised user of the {account.AccountReference.ColourName()} account.");
			return false;
		}

		BankAccount = account;
		Changed = true;
		actor.OutputHandler.Send(
			$"This job will now use the bank account {account.AccountReference.ColourName()} to back its payroll.");
		return true;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Job Posting #{Id.ToString("N0", actor)} - {Name}".ColourName());
		sb.AppendLine();
		sb.AppendLine("Description".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldYellow));
		sb.AppendLine();
		sb.AppendLine(Description.Wrap(actor.InnerLineFormatLength, "\t"));
		sb.AppendLine();
		sb.AppendLine("Core Details".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldYellow));
		sb.AppendLine();
		var employer = Employer;
		if (employer is ICharacter ch)
		{
			sb.AppendLine(
				$"Employer: {ch.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreSelf)}");
		}
		else if (employer is IClan clan)
		{
			sb.AppendLine($"Employer: {clan.FullName.ColourName()}");
		}

		sb.AppendLine($"Is Ready: {IsReadyToBePosted.ToColouredString()}");
		sb.AppendLine($"Is Archived: {IsArchived.ToColouredString()}");
		sb.AppendLine($"Full-Time Equivalent: {FullTimeEquivalentRatio.ToString("P", actor).ColourValue()}");

		sb.AppendLine($"Maximum Employees: {MaximumNumberOfSimultaneousEmployees.ToString("N0", actor).ColourValue()}");
		if (ClanMembership is null)
		{
			sb.AppendLine($"Clan Membership: {"None".ColourError()}");
		}
		else
		{
			sb.AppendLine(
				$"Clan Membership: {ClanMembership.FullName.ColourName()} @ {ClanRank?.Name.ColourValue() ?? ClanMembership.Ranks.FirstMin(x => x.RankNumber)?.Name.ColourValue() ?? "None".ColourError()}");
			if (ClanAppointment is not null)
			{
				sb.AppendLine($"Special Appointment: {ClanAppointment.Name.ColourValue()}");
			}

			if (ClanPaygrade is not null)
			{
				sb.AppendLine($"Clan Paygrade: {ClanPaygrade.Name.ColourValue()}");
			}
		}

		if (RequiredProject is null)
		{
			sb.AppendLine($"Required Project: {"None".ColourError()}");
		}
		else
		{
			sb.AppendLine(
				$"Required Project: {RequiredProjectLabour!.Description.ColourValue()} on {RequiredProject.Name.ColourName()}");
		}

		sb.AppendLine($"Personal Project: {PersonalProject?.Name.ColourName() ?? "None".ColourError()}");
		sb.AppendLine();
		sb.AppendLine("Project Financials".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldYellow));
		sb.AppendLine();
		var reserves = new DecimalCounter<ICurrency>();
		sb.AppendLine($"Bank Account: {BankAccount?.AccountReference.ColourName() ?? "None".ColourError()}");
		if (BankAccount is not null)
		{
			sb.AppendLine(
				$"Bank Balance: {BankAccount.Currency.Describe(BankAccount.CurrentBalance, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
			sb.AppendLine(
				$"Total Available: {BankAccount.Currency.Describe(BankAccount.MaximumWithdrawal(), CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
			reserves[BankAccount.Currency] += BankAccount.MaximumWithdrawal();
		}

		sb.AppendLine(
			$"Money Paid In: {MoneyPaidIn.Keys.Select(x => x.Describe(MoneyPaidIn[x], CurrencyDescriptionPatternType.ShortDecimal).ColourValue()).ListToString()}");

		foreach (var money in MoneyPaidIn)
		{
			reserves[money.Key] += money.Value;
		}

		sb.AppendLine(
			$"Total Funds Available: {reserves.Keys.Select(x => x.Describe(reserves[x], CurrencyDescriptionPatternType.ShortDecimal).ColourValue()).ListToString()}");
		var owed = ActiveJobs.SelectMany(x => x.BackpayOwed).ToList();
		var finalOwed = new DecimalCounter<ICurrency>();
		foreach (var debt in owed)
		{
			finalOwed[debt.Key] += debt.Value;
		}

		sb.AppendLine(
			$"Backpay Owed: {finalOwed.Keys.Select(x => x.Describe(finalOwed[x], CurrencyDescriptionPatternType.ShortDecimal).ColourValue()).ListToString()}");
		var dailyWages = new DecimalCounter<ICurrency>();
		foreach (var job in ActiveJobs)
		{
			dailyWages += job.DailyPay();
		}

		sb.AppendLine(
			$"Daily Pay Commitment: {dailyWages.Keys.Select(x => x.Describe(dailyWages[x], CurrencyDescriptionPatternType.ShortDecimal).ColourValue()).ListToString()}");
		if (ClanMembership is not null)
		{
			var clanPays = new DecimalCounter<ICurrency>();
			var rank = ClanRank ?? ClanMembership.Ranks.FirstMin(x => x.RankNumber);
			var paygrade = ClanPaygrade ?? rank.Paygrades.FirstMin(x => x.PayAmount);
			if (paygrade is not null)
			{
				clanPays[paygrade.PayCurrency] += paygrade.PayAmount;
			}

			if (ClanAppointment?.Paygrade is not null)
			{
				clanPays[ClanAppointment.Paygrade.PayCurrency] += ClanAppointment.Paygrade.PayAmount;
			}

			if (clanPays.Any(x => x.Value > 0.0M))
			{
				sb.AppendLine(
					$"Clan Pay Commitment: {dailyWages.Keys.Select(x => x.Describe(dailyWages[x], CurrencyDescriptionPatternType.ShortDecimal).ColourValue()).ListToString()} {ClanMembership.PayInterval.Describe(ClanMembership.Calendar).ColourValue()}");
			}
		}

		sb.Append(ShowAddendum(actor));
		return sb.ToString();
	}

	protected abstract string ShowAddendum(ICharacter actor);

	#endregion

	#region Implementation of IJobListing

	public IEconomicZone EconomicZone { get; protected set; }
	public IFutureProg EligibilityProg { get; protected set; }

	private readonly FrameworkItemReference _employerReference;
	private IFrameworkItem? _employer;
	public IFrameworkItem Employer => _employer ??= _employerReference.GetItem;
	public abstract IActiveJob ApplyForJob(ICharacter actor);
	public abstract void FinishJob();

	public (bool Truth, string Error) IsEligibleForJob(ICharacter actor)
	{
		if (actor.ActiveJobs.Any(x => !x.IsJobComplete && x.Listing == this))
		{
			return (false, "You already hold this job, and so cannot apply for it again.");
		}

		if (EligibilityProg.Execute<bool?>(actor) == false)
		{
			return (false, "You are not permitted to apply for this job.");
		}

		if (RequiredProject is not null && !RequiredProjectLabour!.CharacterIsQualified(actor))
		{
			return (false, "You are not qualified to perform the task that this job requires.");
		}

		if (PersonalProject is not null &&
		    PersonalProject.Phases.Any(x => x.LabourRequirements.All(y => !y.CharacterIsQualified(actor))))
		{
			return (false, "You are not qualified to perform the task that this job requires.");
		}

		if (MaximumNumberOfSimultaneousEmployees > 0 &&
		    ActiveJobs.Count(x => !x.IsJobComplete) >= MaximumNumberOfSimultaneousEmployees)
		{
			return (false, "All of the available positions associated with this job are already taken.");
		}

		var totalEffort = actor.ActiveJobs.Where(x => !x.IsJobComplete).Sum(x => x.FullTimeEquivalentRatio) +
		                  FullTimeEquivalentRatio;
		if (totalEffort > 2.0)
		{
			return (false,
				$"Taking that job would put your job commitment to {totalEffort.ToString("P", actor).ColourValue()}, which is above the maximum of {2.0.ToString("P", actor).ColourValue()}.");
		}

		return (true, string.Empty);
	}

	public bool IsAuthorisedToEdit(ICharacter actor)
	{
		if (actor.IsAdministrator())
		{
			return true;
		}

		if (Employer is ICharacter ch && actor == ch)
		{
			return true;
		}

		if (Employer is IClan clan && actor.ClanMemberships.FirstOrDefault(x => x.Clan == clan)?.NetPrivileges
		                                   .HasFlag(ClanPrivilegeType.CanManageClanJobs) == true)
		{
			return true;
		}

		return false;
	}

	public string ShowToPlayer(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Job Posting #{Id.ToString("N0", actor)} - {Name.ColourName()}");
		sb.AppendLine();
		sb.AppendLine(Description.Wrap(actor.InnerLineFormatLength, "\t"));
		sb.AppendLine();
		var employer = Employer;
		if (employer is ICharacter ch)
		{
			sb.AppendLine($"Employer is {ch.PersonalName.GetName(NameStyle.FullName)}");
		}
		else if (employer is IClan clan)
		{
			sb.AppendLine($"Employer is {clan.FullName.ColourName()}");
		}

		if (MaximumNumberOfSimultaneousEmployees > 0)
		{
			var positions = MaximumNumberOfSimultaneousEmployees - ActiveJobs.Count(x => !x.IsJobComplete);
			sb.AppendLine($"There are {positions.ToString("N0", actor).ColourValue()} positions remaining.");
		}

		sb.AppendLine(
			$"This job is equivalent to {FullTimeEquivalentRatio.ToString("P", actor).ColourValue()} of a full time role.");

		var (truth, error) = IsEligibleForJob(actor);
		if (truth)
		{
			sb.AppendLine($"You are eligible to apply for this job.".Colour(Telnet.BoldGreen));
		}
		else
		{
			sb.AppendLine(
				$"You are not eligible to apply for this job because {error.ToLowerInvariant()}.".Colour(Telnet.Red));
		}

		if (ClanMembership is not null)
		{
			var rank = ClanRank ?? ClanMembership.Ranks.FirstMin(x => x.RankNumber);
			sb.AppendLine(
				$"It grants membership in the {ClanMembership.FullName.ColourName()} clan at rank {rank.Name.ColourValue()}.");
			if (ClanAppointment is not null)
			{
				sb.AppendLine($"\t* Also grants the {ClanAppointment.Name.ColourValue()} appointment.");
			}

			if (ClanPaygrade is not null)
			{
				sb.AppendLine($"\t* The starting paygrade will be {ClanPaygrade.Name.ColourValue()}.");
			}
		}

		sb.Append(ShowToPlayerAddendum(actor));
		return sb.ToString();
	}

	protected DecimalCounter<ICurrency> TotalPayFromClans()
	{
		var counter = new DecimalCounter<ICurrency>();
		if (ClanMembership is null)
		{
			return counter;
		}

		var rank = ClanRank ?? ClanMembership.Ranks.FirstMin(x => x.RankNumber);
		var paygrade = ClanPaygrade ?? rank.Paygrades.FirstMin(x => x.PayAmount);

		if (paygrade is not null)
		{
			counter[paygrade.PayCurrency] += paygrade.PayAmount;
		}

		if (ClanAppointment?.Paygrade is not null)
		{
			counter[ClanAppointment.Paygrade.PayCurrency] += ClanAppointment.Paygrade.PayAmount;
		}

		return counter;
	}

	public abstract string PayDescriptionForJobListing();

	protected abstract string ShowToPlayerAddendum(ICharacter actor);
	public string Description { get; protected set; }
	public MudTimeSpan? MaximumDuration { get; protected set; }
	public int MaximumNumberOfSimultaneousEmployees { get; protected set; }
	public IClan? ClanMembership { get; protected set; }
	public IRank? ClanRank { get; protected set; }
	public IAppointment? ClanAppointment { get; protected set; }
	public IPaygrade? ClanPaygrade { get; protected set; }
	public IProject? PersonalProject { get; protected set; }
	public ILocalProject? RequiredProject { get; protected set; }
	public IProjectLabourRequirement? RequiredProjectLabour { get; protected set; }
	public IBankAccount? BankAccount { get; protected set; }
	public DecimalCounter<ICurrency> MoneyPaidIn { get; } = new();
	protected readonly List<IActiveJob> _activeJobs = new();
	public IEnumerable<IActiveJob> ActiveJobs => _activeJobs;

	public void RemoveJob(IActiveJob job)
	{
		_activeJobs.Remove(job);
	}

	public double FullTimeEquivalentRatio { get; protected set; }
	private bool _isArchived;

	public bool IsArchived
	{
		get => _isArchived;
		set
		{
			_isArchived = value;
			Changed = true;
		}
	}

	private bool _isReadyToBePosted;

	public bool IsReadyToBePosted
	{
		get => _isReadyToBePosted;
		set
		{
			_isReadyToBePosted = value;
			Changed = true;
		}
	}

	public DecimalCounter<ICurrency> NetFinancialPosition
	{
		get
		{
			var counter = new DecimalCounter<ICurrency>();
			counter += MoneyPaidIn;
			foreach (var job in _activeJobs)
			{
				counter -= job.BackpayOwed;
			}

			BaseClassNetFinancialPositionAdditions(counter);
			return counter;
		}
	}

	protected virtual void BaseClassNetFinancialPositionAdditions(DecimalCounter<ICurrency> counter)
	{
		// Do nothing
	}

	public double DaysOfSolvency
	{
		get
		{
			var days = double.PositiveInfinity;
			foreach (var item in NetFinancialPosition)
			{
				var totalDaily = _activeJobs.Sum(x => x.DailyPay()[item.Key]);
				if (totalDaily == 0.0M)
				{
					continue;
				}

				var currencyDays = (double)(item.Value / totalDaily);
				if (currencyDays < days)
				{
					days = currencyDays;
				}
			}

			if (days < 0.0)
			{
				return 0.0;
			}

			return days;
		}
	}

	public virtual void ProjectTick()
	{
		// Do nothing unless behaviour overriden
	}

	#endregion
}