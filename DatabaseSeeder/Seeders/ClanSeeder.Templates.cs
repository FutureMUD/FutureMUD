using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Community;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.TimeAndDate.Intervals;
using Appointment = MudSharp.Models.Appointment;
using Clan = MudSharp.Models.Clan;
using FutureProgModel = MudSharp.Models.FutureProg;
using Paygrade = MudSharp.Models.Paygrade;
using Rank = MudSharp.Models.Rank;

namespace DatabaseSeeder.Seeders;

public partial class ClanSeeder
{
	private static long MemberPrivileges()
	{
		return (long)(ClanPrivilegeType.CanViewClanStructure |
		              ClanPrivilegeType.CanViewClanOfficeHolders |
		              ClanPrivilegeType.CanViewMembers);
	}

	private static long CompanyStaffPrivileges()
	{
		return MemberPrivileges() |
		       (long)(ClanPrivilegeType.CanManageClanJobs |
		              ClanPrivilegeType.CanIncreasePaygrade |
		              ClanPrivilegeType.CanDecreasePaygrade);
	}

	private static long CompanyCommandPrivileges()
	{
		return CompanyStaffPrivileges() |
		       (long)(ClanPrivilegeType.CanInduct |
		              ClanPrivilegeType.CanPromote |
		              ClanPrivilegeType.CanDemote |
		              ClanPrivilegeType.CanCastout |
		              ClanPrivilegeType.CanAppoint |
		              ClanPrivilegeType.CanDismiss |
		              ClanPrivilegeType.CanGiveBackpay |
		              ClanPrivilegeType.CanManageBankAccounts |
		              ClanPrivilegeType.CanManageClanProperty);
	}

	private static long BattalionStaffPrivileges()
	{
		return MemberPrivileges() |
		       (long)(ClanPrivilegeType.CanReportDead |
		              ClanPrivilegeType.CanManageClanJobs);
	}

	private static long BattalionCommandPrivileges()
	{
		return BattalionStaffPrivileges() |
		       (long)(ClanPrivilegeType.CanInduct |
		              ClanPrivilegeType.CanPromote |
		              ClanPrivilegeType.CanDemote |
		              ClanPrivilegeType.CanCastout |
		              ClanPrivilegeType.CanAppoint |
		              ClanPrivilegeType.CanDismiss |
		              ClanPrivilegeType.CanIncreasePaygrade |
		              ClanPrivilegeType.CanDecreasePaygrade |
		              ClanPrivilegeType.CanGiveBackpay);
	}

	private Clan CreateTemplateClan(FuturemudDatabaseContext context, string name, string alias, string description,
		string? sphere = null)
	{
		var clan = new Clan
		{
			Name = name,
			Alias = alias,
			FullName = name,
			Description = description,
			PayIntervalType = (int)IntervalType.Monthly,
			PayIntervalModifier = 1,
			PayIntervalOther = 0,
			CalendarId = context.Calendars.First().Id,
			PayIntervalReferenceDate = context.Calendars.First().Date,
			PayIntervalReferenceTime = $"{context.Timezones.First(x => x.Clock.PrimaryTimezoneId == x.Id).Name} 0:0:0",
			IsTemplate = true,
			ShowClanMembersInWho = false,
			ShowFamousMembersInNotables = false,
			Sphere = sphere
		};
		context.Clans.Add(clan);
		context.SaveChanges();
		return clan;
	}

	private Rank AddRank(FuturemudDatabaseContext context, Clan clan, string name, string abbreviation, int rankNumber,
		long privileges, string rankPath, string? title = null, int fameType = 0)
	{
		var rank = new Rank
		{
			Name = name,
			Clan = clan,
			RankNumber = rankNumber,
			RankPath = rankPath,
			Privileges = privileges,
			FameType = fameType
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = abbreviation });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = title ?? name });
		context.Ranks.Add(rank);
		context.SaveChanges();
		return rank;
	}

	private Paygrade AddPaygrade(FuturemudDatabaseContext context, Clan clan, string name, string abbreviation,
		decimal amount, params Rank[] linkedRanks)
	{
		var paygrade = new Paygrade
		{
			Name = name,
			Abbreviation = abbreviation,
			Currency = context.Currencies.First(),
			PayAmount = amount,
			Clan = clan
		};
		context.Paygrades.Add(paygrade);
		context.SaveChanges();

		foreach (var linkedRank in linkedRanks)
		{
			linkedRank.RanksPaygrades.Add(new RanksPaygrade
			{
				Rank = linkedRank,
				Paygrade = paygrade,
				Order = linkedRank.RanksPaygrades.Count
			});
		}

		context.SaveChanges();
		return paygrade;
	}

	private Appointment AddAppointment(FuturemudDatabaseContext context, Clan clan, string name, string abbreviation,
		long privileges, Rank? minimumRank = null, Rank? minimumRankToAppoint = null, Appointment? parent = null,
		Paygrade? paygrade = null, int maximumSimultaneousHolders = 1, string? title = null, int fameType = 0)
	{
		var appointment = new Appointment
		{
			Name = name,
			Clan = clan,
			MinimumRank = minimumRank,
			MinimumRankToAppoint = minimumRankToAppoint,
			ParentAppointment = parent,
			Paygrade = paygrade,
			Privileges = privileges,
			FameType = fameType,
			MaximumSimultaneousHolders = maximumSimultaneousHolders,
			IsAppointedByElection = false
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = appointment, Order = 0, Abbreviation = abbreviation });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = appointment, Order = 0, Title = title ?? name });
		context.Appointments.Add(appointment);
		context.SaveChanges();
		return appointment;
	}

	private Appointment AddElectionAppointment(FuturemudDatabaseContext context, Clan clan, string name,
		string abbreviation, long privileges, FutureProgModel canNominateProg, FutureProgModel whyCantNominateProg,
		FutureProgModel numberOfVotesProg, Rank? minimumRank = null, Rank? minimumRankToAppoint = null,
		Appointment? parent = null, Paygrade? paygrade = null, double electionTermDays = 365.0,
		double nominationPeriodDays = 7.0, double votingPeriodDays = 7.0, int maximumSimultaneousHolders = 1,
		bool isSecretBallot = false, string? title = null)
	{
		var appointment = new Appointment
		{
			Name = name,
			Clan = clan,
			MinimumRank = minimumRank,
			MinimumRankToAppoint = minimumRankToAppoint,
			ParentAppointment = parent,
			Paygrade = paygrade,
			Privileges = privileges,
			MaximumSimultaneousHolders = maximumSimultaneousHolders,
			IsAppointedByElection = true,
			IsSecretBallot = isSecretBallot,
			ElectionTermMinutes = electionTermDays * 24.0 * 60.0,
			ElectionLeadTimeMinutes = 0,
			NominationPeriodMinutes = nominationPeriodDays * 24.0 * 60.0,
			VotingPeriodMinutes = votingPeriodDays * 24.0 * 60.0,
			MaximumConsecutiveTerms = 0,
			MaximumTotalTerms = 0,
			CanNominateProg = canNominateProg,
			WhyCantNominateProg = whyCantNominateProg,
			NumberOfVotesProg = numberOfVotesProg
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = appointment, Order = 0, Abbreviation = abbreviation });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = appointment, Order = 0, Title = title ?? name });
		context.Appointments.Add(appointment);
		context.SaveChanges();
		return appointment;
	}

	private FutureProgModel GetProg(FuturemudDatabaseContext context, string functionName)
	{
		return context.FutureProgs.First(x => x.FunctionName == functionName);
	}

	private Dictionary<string, Rank> BuildOperationalArmyRanks(FuturemudDatabaseContext context, Clan clan)
	{
		var memberPrivileges = MemberPrivileges();
		var ncoPrivileges = memberPrivileges | (long)ClanPrivilegeType.CanReportDead;
		var seniorNcoPrivileges = BattalionStaffPrivileges() |
		                          (long)(ClanPrivilegeType.CanIncreasePaygrade |
		                                 ClanPrivilegeType.CanDecreasePaygrade);
		var officerPrivileges = BattalionCommandPrivileges();

		return new Dictionary<string, Rank>(StringComparer.OrdinalIgnoreCase)
		{
			["Private"] = AddRank(context, clan, "Private", "E-1", 1, memberPrivileges, "Enlisted"),
			["Private First Class"] = AddRank(context, clan, "Private First Class", "E-3", 2, memberPrivileges,
				"Enlisted"),
			["Specialist"] = AddRank(context, clan, "Specialist", "E-4", 3, memberPrivileges, "Enlisted"),
			["Corporal"] = AddRank(context, clan, "Corporal", "E-4", 4, ncoPrivileges, "Enlisted"),
			["Sergeant"] = AddRank(context, clan, "Sergeant", "E-5", 5, ncoPrivileges, "Enlisted"),
			["Staff Sergeant"] = AddRank(context, clan, "Staff Sergeant", "E-6", 6, ncoPrivileges, "Enlisted"),
			["Sergeant First Class"] = AddRank(context, clan, "Sergeant First Class", "E-7", 7, ncoPrivileges,
				"Enlisted"),
			["First Sergeant"] = AddRank(context, clan, "First Sergeant", "E-8", 8, seniorNcoPrivileges,
				"Enlisted"),
			["Master Sergeant"] = AddRank(context, clan, "Master Sergeant", "E-8", 9, seniorNcoPrivileges,
				"Enlisted"),
			["Sergeant Major"] = AddRank(context, clan, "Sergeant Major", "E-9", 10, seniorNcoPrivileges,
				"Enlisted"),
			["Command Sergeant Major"] = AddRank(context, clan, "Command Sergeant Major", "E-9", 11,
				seniorNcoPrivileges, "Enlisted"),
			["Warrant Officer 1"] = AddRank(context, clan, "Warrant Officer 1", "W-1", 12, ncoPrivileges,
				"Warrant"),
			["Chief Warrant Officer 2"] = AddRank(context, clan, "Chief Warrant Officer 2", "CW2", 13,
				seniorNcoPrivileges, "Warrant"),
			["Second Lieutenant"] = AddRank(context, clan, "Second Lieutenant", "O-1", 14, officerPrivileges,
				"Officer"),
			["First Lieutenant"] = AddRank(context, clan, "First Lieutenant", "O-2", 15, officerPrivileges,
				"Officer"),
			["Captain"] = AddRank(context, clan, "Captain", "O-3", 16, officerPrivileges, "Officer"),
			["Major"] = AddRank(context, clan, "Major", "O-4", 17, officerPrivileges, "Officer"),
			["Lieutenant Colonel"] = AddRank(context, clan, "Lieutenant Colonel", "O-5", 18, officerPrivileges,
				"Officer"),
			["Colonel"] = AddRank(context, clan, "Colonel", "O-6", 19,
				officerPrivileges | (long)ClanPrivilegeType.CanPromoteToOwnRank, "Officer")
		};
	}

	private void AddArmyGeneralStaffAppointments(FuturemudDatabaseContext context, Clan clan)
	{
		var ranks = context.Ranks
			.Where(x => x.ClanId == clan.Id)
			.ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
		var existingAppointments = context.Appointments
			.Where(x => x.ClanId == clan.Id)
			.ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
		var memberPrivileges = MemberPrivileges();
		var staffPrivileges = BattalionStaffPrivileges() |
		                     (long)(ClanPrivilegeType.CanIncreasePaygrade | ClanPrivilegeType.CanDecreasePaygrade);
		var commandPrivileges = BattalionCommandPrivileges() |
		                       (long)(ClanPrivilegeType.CanChangeRankPath | ClanPrivilegeType.CanPromoteToOwnRank);

		var general = ranks["General"];
		var colonel = ranks["Colonel"];
		var lieutenantColonel = ranks["Lieutenant Colonel"];
		var major = ranks["Major"];
		var sergeantMajor = ranks["Sergeant Major"];
		var captain = ranks["Captain"];

		Appointment EnsureArmyAppointment(string name, string abbreviation, long privileges, Rank minimumRank,
			Rank minimumRankToAppoint, Appointment? parent = null)
		{
			if (existingAppointments.TryGetValue(name, out var existingAppointment))
			{
				return existingAppointment;
			}

			var appointment = AddAppointment(context, clan, name, abbreviation, privileges, minimumRank,
				minimumRankToAppoint, parent);
			existingAppointments[name] = appointment;
			return appointment;
		}

		var commandingOfficer = EnsureArmyAppointment("Commanding Officer", "CO", commandPrivileges, colonel, colonel);
		EnsureArmyAppointment("Executive Officer", "XO", commandPrivileges, lieutenantColonel, colonel,
			commandingOfficer);
		EnsureArmyAppointment("Command Sergeant Major", "CSM",
			staffPrivileges | (long)(ClanPrivilegeType.CanAppoint | ClanPrivilegeType.CanDismiss), sergeantMajor,
			lieutenantColonel, commandingOfficer);
		EnsureArmyAppointment("S1 Personnel Officer", "S1", staffPrivileges, major, lieutenantColonel,
			commandingOfficer);
		EnsureArmyAppointment("S2 Intelligence Officer", "S2", staffPrivileges, major, lieutenantColonel,
			commandingOfficer);
		EnsureArmyAppointment("S3 Operations Officer", "S3", staffPrivileges, major, lieutenantColonel,
			commandingOfficer);
		EnsureArmyAppointment("S4 Logistics Officer", "S4", staffPrivileges, major, lieutenantColonel,
			commandingOfficer);
		EnsureArmyAppointment("S5 Plans Officer", "S5", staffPrivileges, major, lieutenantColonel,
			commandingOfficer);
		EnsureArmyAppointment("S6 Communications Officer", "S6", staffPrivileges, major, lieutenantColonel,
			commandingOfficer);
		EnsureArmyAppointment("Chaplain", "Chap", memberPrivileges, captain, major, commandingOfficer);
		EnsureArmyAppointment("Judge Advocate", "JAG", staffPrivileges, captain, major, commandingOfficer);
		EnsureArmyAppointment("Theatre Commander", "THCOM", (long)ClanPrivilegeType.All, general, general);
	}

	private void SetupNavalOrganisationClan(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> questionAnswers)
	{
		if (context.Clans.Any(x => x.Name == "Naval Organisation Template"))
		{
			return;
		}

		var clan = CreateTemplateClan(context, "Naval Organisation Template", "navy",
			"This is designed to be used as a template clan for a modern naval organisation using US Navy style NATO-coded ranks and staff appointments.");
		var memberPrivileges = MemberPrivileges();
		var seniorPrivileges = BattalionStaffPrivileges();
		var commandPrivileges = BattalionCommandPrivileges();
		var ranks = new Dictionary<string, Rank>(StringComparer.OrdinalIgnoreCase)
		{
			["Seaman Recruit"] = AddRank(context, clan, "Seaman Recruit", "OR-1", 1, memberPrivileges, "Enlisted"),
			["Seaman Apprentice"] = AddRank(context, clan, "Seaman Apprentice", "OR-2", 2, memberPrivileges,
				"Enlisted"),
			["Seaman"] = AddRank(context, clan, "Seaman", "OR-3", 3, memberPrivileges, "Enlisted"),
			["Petty Officer Third Class"] = AddRank(context, clan, "Petty Officer Third Class", "OR-4", 4,
				memberPrivileges, "Petty Officer"),
			["Petty Officer Second Class"] = AddRank(context, clan, "Petty Officer Second Class", "OR-5", 5,
				seniorPrivileges, "Petty Officer"),
			["Petty Officer First Class"] = AddRank(context, clan, "Petty Officer First Class", "OR-6", 6,
				seniorPrivileges, "Petty Officer"),
			["Chief Petty Officer"] = AddRank(context, clan, "Chief Petty Officer", "OR-7", 7, seniorPrivileges,
				"Chief"),
			["Senior Chief Petty Officer"] = AddRank(context, clan, "Senior Chief Petty Officer", "OR-8", 8,
				seniorPrivileges, "Chief"),
			["Master Chief Petty Officer"] = AddRank(context, clan, "Master Chief Petty Officer", "OR-9", 9,
				commandPrivileges, "Chief"),
			["Ensign"] = AddRank(context, clan, "Ensign", "OF-1", 10, commandPrivileges, "Officer"),
			["Lieutenant Junior Grade"] = AddRank(context, clan, "Lieutenant Junior Grade", "OF-1", 11,
				commandPrivileges, "Officer"),
			["Lieutenant"] = AddRank(context, clan, "Lieutenant", "OF-2", 12, commandPrivileges, "Officer"),
			["Lieutenant Commander"] = AddRank(context, clan, "Lieutenant Commander", "OF-3", 13,
				commandPrivileges, "Officer"),
			["Commander"] = AddRank(context, clan, "Commander", "OF-4", 14, commandPrivileges, "Officer"),
			["Captain"] = AddRank(context, clan, "Captain", "OF-5", 15,
				commandPrivileges | (long)ClanPrivilegeType.CanPromoteToOwnRank, "Officer"),
			["Rear Admiral (Lower Half)"] = AddRank(context, clan, "Rear Admiral (Lower Half)", "OF-6", 16,
				commandPrivileges, "Flag Officer"),
			["Rear Admiral"] = AddRank(context, clan, "Rear Admiral", "OF-7", 17, commandPrivileges, "Flag Officer"),
			["Vice Admiral"] = AddRank(context, clan, "Vice Admiral", "OF-8", 18, commandPrivileges, "Flag Officer"),
			["Admiral"] = AddRank(context, clan, "Admiral", "OF-9", 19, (long)ClanPrivilegeType.All, "Flag Officer")
		};

		var commandingOfficer = AddAppointment(context, clan, "Commanding Officer", "CO", commandPrivileges,
			ranks["Captain"], ranks["Captain"]);
		AddAppointment(context, clan, "Executive Officer", "XO", commandPrivileges, ranks["Commander"],
			ranks["Captain"], commandingOfficer);
		AddAppointment(context, clan, "Command Master Chief", "CMC",
			seniorPrivileges | (long)(ClanPrivilegeType.CanAppoint | ClanPrivilegeType.CanDismiss),
			ranks["Master Chief Petty Officer"], ranks["Commander"], commandingOfficer);
		AddAppointment(context, clan, "N1 Personnel Officer", "N1", seniorPrivileges, ranks["Lieutenant Commander"],
			ranks["Commander"], commandingOfficer);
		AddAppointment(context, clan, "N2 Intelligence Officer", "N2", seniorPrivileges,
			ranks["Lieutenant Commander"], ranks["Commander"], commandingOfficer);
		AddAppointment(context, clan, "N3 Operations Officer", "N3", seniorPrivileges,
			ranks["Lieutenant Commander"], ranks["Commander"], commandingOfficer);
		AddAppointment(context, clan, "N4 Logistics Officer", "N4", seniorPrivileges,
			ranks["Lieutenant Commander"], ranks["Commander"], commandingOfficer);
		AddAppointment(context, clan, "N5 Plans Officer", "N5", seniorPrivileges,
			ranks["Lieutenant Commander"], ranks["Commander"], commandingOfficer);
		AddAppointment(context, clan, "N6 Communications Officer", "N6", seniorPrivileges,
			ranks["Lieutenant Commander"], ranks["Commander"], commandingOfficer);
		AddAppointment(context, clan, "Chief Engineer", "CHENG", seniorPrivileges, ranks["Lieutenant"],
			ranks["Lieutenant Commander"], commandingOfficer);
		AddAppointment(context, clan, "Chief Medical Officer", "CMO", seniorPrivileges, ranks["Lieutenant"],
			ranks["Lieutenant Commander"], commandingOfficer);
		AddAppointment(context, clan, "Fleet Admiral", "FADM", (long)ClanPrivilegeType.All, ranks["Admiral"],
			ranks["Admiral"]);
	}

	private void SetupAirForceClan(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		if (context.Clans.Any(x => x.Name == "Air Force Template"))
		{
			return;
		}

		var clan = CreateTemplateClan(context, "Air Force Template", "airforce",
			"This is designed to be used as a template clan for a modern air force using US Air Force style NATO-coded ranks and staff appointments.");
		var memberPrivileges = MemberPrivileges();
		var seniorPrivileges = BattalionStaffPrivileges();
		var commandPrivileges = BattalionCommandPrivileges();
		var ranks = new Dictionary<string, Rank>(StringComparer.OrdinalIgnoreCase)
		{
			["Airman Basic"] = AddRank(context, clan, "Airman Basic", "OR-1", 1, memberPrivileges, "Enlisted"),
			["Airman"] = AddRank(context, clan, "Airman", "OR-2", 2, memberPrivileges, "Enlisted"),
			["Airman First Class"] = AddRank(context, clan, "Airman First Class", "OR-3", 3, memberPrivileges,
				"Enlisted"),
			["Senior Airman"] = AddRank(context, clan, "Senior Airman", "OR-4", 4, memberPrivileges, "Enlisted"),
			["Staff Sergeant"] = AddRank(context, clan, "Staff Sergeant", "OR-5", 5, seniorPrivileges, "NCO"),
			["Technical Sergeant"] = AddRank(context, clan, "Technical Sergeant", "OR-6", 6, seniorPrivileges,
				"NCO"),
			["Master Sergeant"] = AddRank(context, clan, "Master Sergeant", "OR-7", 7, seniorPrivileges, "Senior NCO"),
			["Senior Master Sergeant"] = AddRank(context, clan, "Senior Master Sergeant", "OR-8", 8,
				seniorPrivileges, "Senior NCO"),
			["Chief Master Sergeant"] = AddRank(context, clan, "Chief Master Sergeant", "OR-9", 9,
				commandPrivileges, "Senior NCO"),
			["Second Lieutenant"] = AddRank(context, clan, "Second Lieutenant", "OF-1", 10, commandPrivileges,
				"Officer"),
			["First Lieutenant"] = AddRank(context, clan, "First Lieutenant", "OF-1", 11, commandPrivileges,
				"Officer"),
			["Captain"] = AddRank(context, clan, "Captain", "OF-2", 12, commandPrivileges, "Officer"),
			["Major"] = AddRank(context, clan, "Major", "OF-3", 13, commandPrivileges, "Officer"),
			["Lieutenant Colonel"] = AddRank(context, clan, "Lieutenant Colonel", "OF-4", 14, commandPrivileges,
				"Officer"),
			["Colonel"] = AddRank(context, clan, "Colonel", "OF-5", 15,
				commandPrivileges | (long)ClanPrivilegeType.CanPromoteToOwnRank, "Officer"),
			["Brigadier General"] = AddRank(context, clan, "Brigadier General", "OF-6", 16, commandPrivileges,
				"General Officer"),
			["Major General"] = AddRank(context, clan, "Major General", "OF-7", 17, commandPrivileges,
				"General Officer"),
			["Lieutenant General"] = AddRank(context, clan, "Lieutenant General", "OF-8", 18, commandPrivileges,
				"General Officer"),
			["General"] = AddRank(context, clan, "General", "OF-9", 19, (long)ClanPrivilegeType.All, "General Officer")
		};

		var commandingOfficer = AddAppointment(context, clan, "Commanding Officer", "CC", commandPrivileges,
			ranks["Colonel"], ranks["Colonel"]);
		AddAppointment(context, clan, "Deputy Commander", "DC", commandPrivileges, ranks["Lieutenant Colonel"],
			ranks["Colonel"], commandingOfficer);
		AddAppointment(context, clan, "Command Chief Master Sergeant", "CCM",
			seniorPrivileges | (long)(ClanPrivilegeType.CanAppoint | ClanPrivilegeType.CanDismiss),
			ranks["Chief Master Sergeant"], ranks["Lieutenant Colonel"], commandingOfficer);
		AddAppointment(context, clan, "A1 Personnel Officer", "A1", seniorPrivileges, ranks["Major"],
			ranks["Lieutenant Colonel"], commandingOfficer);
		AddAppointment(context, clan, "A2 Intelligence Officer", "A2", seniorPrivileges, ranks["Major"],
			ranks["Lieutenant Colonel"], commandingOfficer);
		AddAppointment(context, clan, "A3 Operations Officer", "A3", seniorPrivileges, ranks["Major"],
			ranks["Lieutenant Colonel"], commandingOfficer);
		AddAppointment(context, clan, "A4 Logistics Officer", "A4", seniorPrivileges, ranks["Major"],
			ranks["Lieutenant Colonel"], commandingOfficer);
		AddAppointment(context, clan, "A5 Plans Officer", "A5", seniorPrivileges, ranks["Major"],
			ranks["Lieutenant Colonel"], commandingOfficer);
		AddAppointment(context, clan, "A6 Communications Officer", "A6", seniorPrivileges, ranks["Major"],
			ranks["Lieutenant Colonel"], commandingOfficer);
		AddAppointment(context, clan, "Operations Group Commander", "OGC", commandPrivileges, ranks["Lieutenant Colonel"],
			ranks["Colonel"], commandingOfficer);
		AddAppointment(context, clan, "Maintenance Group Commander", "MGC", commandPrivileges,
			ranks["Lieutenant Colonel"], ranks["Colonel"], commandingOfficer);
		AddAppointment(context, clan, "General of the Air Force", "GAF", (long)ClanPrivilegeType.All, ranks["General"],
			ranks["General"]);
	}

	private void SetupLocalGovernmentTemplates(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> questionAnswers)
	{
		SetupLocalGovernmentTemplate(context, "Local Government Template - Popular Mayor", "lgpopular",
			"This template represents a local government with residents, councillors, and a mayor elected directly by the residents. Builders can clone this and then flesh out wards, councils, and civic offices as needed.",
			TemplateCloneElectorateAllMembersCanNominateProgName,
			TemplateCloneElectorateAllMembersWhyCantNominateProgName,
			TemplateCloneElectorateAllMembersVotesProgName,
			false);
		SetupLocalGovernmentTemplate(context, "Local Government Template - Councillor Mayor", "lgcouncil",
			"This template represents a local government with residents, councillors, and a mayor elected by councillors. Staggered councillor elections are intentionally left for builders to configure after cloning.",
			TemplateCloneElectorateCouncillorCanNominateProgName,
			TemplateCloneElectorateCouncillorWhyCantNominateProgName,
			TemplateCloneElectorateCouncillorVotesProgName,
			true);
	}

	private void SetupLocalGovernmentTemplate(FuturemudDatabaseContext context, string name, string alias,
		string description, string canNominateProgName, string whyCantNominateProgName, string numberOfVotesProgName,
		bool mayorRequiresCouncillorRank)
	{
		if (context.Clans.Any(x => x.Name == name))
		{
			return;
		}

		var clan = CreateTemplateClan(context, name, alias, description, "Civic");
		var resident = AddRank(context, clan, "Resident", "Res", 1, MemberPrivileges(), "Citizen");
		var councillor = AddRank(context, clan, "Councillor", "Cllr", 2,
			MemberPrivileges() | (long)(ClanPrivilegeType.CanViewTreasury | ClanPrivilegeType.CanManageClanJobs),
			"Civic Office");
		var mayorPrivileges = CompanyCommandPrivileges() |
		                      (long)(ClanPrivilegeType.CanManageEconomicZones | ClanPrivilegeType.CanCreateAppointments);
		var mayorMinimumRank = mayorRequiresCouncillorRank ? councillor : resident;

		var mayor = AddElectionAppointment(context, clan, "Mayor", "Mayor", mayorPrivileges,
			GetProg(context, canNominateProgName),
			GetProg(context, whyCantNominateProgName),
			GetProg(context, numberOfVotesProgName),
			mayorMinimumRank,
			councillor,
			electionTermDays: 365.0,
			nominationPeriodDays: 14.0,
			votingPeriodDays: 7.0);
		AddAppointment(context, clan, "Deputy Mayor", "Dep Mayor", mayorPrivileges, councillor, councillor, mayor);
		AddAppointment(context, clan, "Town Clerk", "Clerk",
			MemberPrivileges() | (long)(ClanPrivilegeType.CanManageBankAccounts | ClanPrivilegeType.CanManageClanJobs),
			councillor, councillor, mayor);
	}

	private void SetupCompanyClan(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		if (context.Clans.Any(x => x.Name == "Company Template"))
		{
			return;
		}

		var clan = CreateTemplateClan(context, "Company Template", "company",
			"This template represents a generic company with employees, managers, executives, and a board, including a board-elected chief executive.");
		var employee = AddRank(context, clan, "Employee", "Emp", 1, MemberPrivileges(), "Staff");
		var supervisor = AddRank(context, clan, "Supervisor", "Sup", 2, CompanyStaffPrivileges(), "Staff");
		var middleManager = AddRank(context, clan, "Middle Manager", "Mgr", 3, CompanyStaffPrivileges(), "Management");
		var seniorManager = AddRank(context, clan, "Senior Manager", "SrMgr", 4, CompanyCommandPrivileges(),
			"Management");
		var executive = AddRank(context, clan, "Executive", "Exec", 5, CompanyCommandPrivileges(), "Executive");
		var board = AddRank(context, clan, "Board", "Board", 6,
			CompanyCommandPrivileges() | (long)ClanPrivilegeType.CanPromoteToOwnRank, "Board");

		AddPaygrade(context, clan, "Employee Grade 1", "EMP-1", 1000.0M, employee);
		AddPaygrade(context, clan, "Employee Grade 2", "EMP-2", 1250.0M, employee);
		AddPaygrade(context, clan, "Supervisor Grade 1", "SUP-1", 1500.0M, supervisor);
		AddPaygrade(context, clan, "Supervisor Grade 2", "SUP-2", 1800.0M, supervisor);
		AddPaygrade(context, clan, "Middle Manager Grade 1", "MM-1", 2400.0M, middleManager);
		AddPaygrade(context, clan, "Middle Manager Grade 2", "MM-2", 2800.0M, middleManager);
		AddPaygrade(context, clan, "Senior Manager Grade 1", "SM-1", 3600.0M, seniorManager);
		AddPaygrade(context, clan, "Senior Manager Grade 2", "SM-2", 4200.0M, seniorManager);
		var executiveGrade1 = AddPaygrade(context, clan, "Executive Grade 1", "EX-1", 5500.0M, executive);
		AddPaygrade(context, clan, "Executive Grade 2", "EX-2", 6500.0M, executive);
		AddPaygrade(context, clan, "Board Grade 1", "BRD-1", 8000.0M, board);
		var boardGrade2 = AddPaygrade(context, clan, "Board Grade 2", "BRD-2", 10000.0M, board);

		var boardPrivileges = CompanyCommandPrivileges() |
		                      (long)(ClanPrivilegeType.CanCreatePaygrades | ClanPrivilegeType.CanCreateRanks |
		                             ClanPrivilegeType.CanCreateAppointments);
		var ceo = AddElectionAppointment(context, clan, "Chief Executive Officer", "CEO", (long)ClanPrivilegeType.All,
			GetProg(context, TemplateCloneElectorateBoardCanNominateProgName),
			GetProg(context, TemplateCloneElectorateBoardWhyCantNominateProgName),
			GetProg(context, TemplateCloneElectorateBoardVotesProgName),
			board,
			board,
			paygrade: boardGrade2,
			electionTermDays: 730.0,
			nominationPeriodDays: 14.0,
			votingPeriodDays: 7.0,
			isSecretBallot: true);
		AddAppointment(context, clan, "Board Chair", "Chair", boardPrivileges, board, board, ceo, boardGrade2);
		AddAppointment(context, clan, "Chief Operating Officer", "COO", CompanyCommandPrivileges(), executive, board,
			ceo, executiveGrade1);
		AddAppointment(context, clan, "Chief Financial Officer", "CFO", CompanyCommandPrivileges(), executive, board,
			ceo, executiveGrade1);
		AddAppointment(context, clan, "Chief Technology Officer", "CTO", CompanyCommandPrivileges(), executive, board,
			ceo, executiveGrade1);
		AddAppointment(context, clan, "Chief People Officer", "CPO", CompanyCommandPrivileges(), executive, board,
			ceo, executiveGrade1);
		AddAppointment(context, clan, "Corporate Secretary", "CorpSec", CompanyStaffPrivileges(), seniorManager, board,
			ceo);
	}

	private void SetupMercenaryCompanyClan(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> questionAnswers)
	{
		if (context.Clans.Any(x => x.Name == "Mercenary Company Template"))
		{
			return;
		}

		var clan = CreateTemplateClan(context, "Mercenary Company Template", "mercs",
			"This template represents a medieval mercenary company built around a captain, trusted officers, veteran fighters, and support retainers.");
		var memberPrivileges = MemberPrivileges();
		var sergeantPrivileges = BattalionStaffPrivileges() |
		                         (long)(ClanPrivilegeType.CanInduct | ClanPrivilegeType.CanReportDead);
		var officerPrivileges = BattalionCommandPrivileges();
		var recruit = AddRank(context, clan, "Recruit", "Rec", 1, memberPrivileges, "Mercenary");
		var mercenary = AddRank(context, clan, "Mercenary", "Merc", 2, memberPrivileges, "Mercenary");
		var veteran = AddRank(context, clan, "Veteran Mercenary", "Vet", 3, memberPrivileges, "Mercenary");
		var sergeant = AddRank(context, clan, "Sergeant", "Sgt", 4, sergeantPrivileges, "Officer");
		var lieutenant = AddRank(context, clan, "Lieutenant", "Lt", 5, officerPrivileges, "Officer");
		var captain = AddRank(context, clan, "Captain", "Cpt", 6,
			officerPrivileges | (long)ClanPrivilegeType.CanPromoteToOwnRank, "Officer");

		AddPaygrade(context, clan, "Recruit Share", "R-Share", 150.0M, recruit);
		AddPaygrade(context, clan, "Company Share", "C-Share", 250.0M, mercenary);
		AddPaygrade(context, clan, "Veteran Share", "V-Share", 400.0M, veteran);
		AddPaygrade(context, clan, "Sergeant Share", "S-Share", 550.0M, sergeant);
		var lieutenantShare = AddPaygrade(context, clan, "Lieutenant Share", "L-Share", 750.0M, lieutenant);
		var captainShare = AddPaygrade(context, clan, "Captain Share", "Cap-Share", 1000.0M, captain);

		var mercenaryCaptain = AddAppointment(context, clan, "Mercenary Captain", "MC", (long)ClanPrivilegeType.All,
			captain, captain, paygrade: captainShare);
		AddAppointment(context, clan, "Company Lieutenant", "CL", officerPrivileges, lieutenant, captain,
			mercenaryCaptain, lieutenantShare);
		AddAppointment(context, clan, "Quartermaster", "QM",
			sergeantPrivileges | (long)(ClanPrivilegeType.CanManageBankAccounts | ClanPrivilegeType.CanGiveBackpay),
			sergeant, lieutenant, mercenaryCaptain);
		AddAppointment(context, clan, "Standard Bearer", "Std", memberPrivileges, veteran, sergeant, mercenaryCaptain);
		AddAppointment(context, clan, "Company Chirurgeon", "Chir", memberPrivileges, veteran, lieutenant,
			mercenaryCaptain);
		AddAppointment(context, clan, "Paymaster", "Pay", sergeantPrivileges, sergeant, lieutenant, mercenaryCaptain);
	}

	private void SetupInfantryCompanyClan(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> questionAnswers)
	{
		if (context.Clans.Any(x => x.Name == "Infantry Company Template"))
		{
			return;
		}

		var clan = CreateTemplateClan(context, "Infantry Company Template", "infcompany",
			"This template represents a NATO-style infantry company using US Army ranks, company command billets, platoon leadership appointments, and practical functional staff roles.");
		var ranks = BuildOperationalArmyRanks(context, clan);
		var memberPrivileges = MemberPrivileges();
		var staffPrivileges = BattalionStaffPrivileges();
		var commandPrivileges = BattalionCommandPrivileges();

		var companyCommander = AddAppointment(context, clan, "Company Commander", "CO", commandPrivileges,
			ranks["Captain"], ranks["Captain"]);
		AddAppointment(context, clan, "Executive Officer", "XO", commandPrivileges, ranks["First Lieutenant"],
			ranks["Captain"], companyCommander);
		AddAppointment(context, clan, "First Sergeant", "1SG",
			staffPrivileges | (long)(ClanPrivilegeType.CanAppoint | ClanPrivilegeType.CanDismiss),
			ranks["First Sergeant"], ranks["Captain"], companyCommander);
		AddAppointment(context, clan, "First Platoon Leader", "1PL", commandPrivileges, ranks["First Lieutenant"],
			ranks["Captain"], companyCommander);
		AddAppointment(context, clan, "Second Platoon Leader", "2PL", commandPrivileges, ranks["First Lieutenant"],
			ranks["Captain"], companyCommander);
		AddAppointment(context, clan, "Third Platoon Leader", "3PL", commandPrivileges, ranks["First Lieutenant"],
			ranks["Captain"], companyCommander);
		AddAppointment(context, clan, "First Platoon Sergeant", "1PSG", staffPrivileges, ranks["Sergeant First Class"],
			ranks["First Sergeant"], companyCommander);
		AddAppointment(context, clan, "Second Platoon Sergeant", "2PSG", staffPrivileges,
			ranks["Sergeant First Class"], ranks["First Sergeant"], companyCommander);
		AddAppointment(context, clan, "Third Platoon Sergeant", "3PSG", staffPrivileges,
			ranks["Sergeant First Class"], ranks["First Sergeant"], companyCommander);
		AddAppointment(context, clan, "Supply Sergeant", "SUP", staffPrivileges, ranks["Staff Sergeant"],
			ranks["First Sergeant"], companyCommander);
		AddAppointment(context, clan, "Training NCO", "TNG", staffPrivileges, ranks["Staff Sergeant"],
			ranks["First Sergeant"], companyCommander);
		AddAppointment(context, clan, "Armorer", "ARM", memberPrivileges, ranks["Sergeant"], ranks["Staff Sergeant"],
			companyCommander);
		AddAppointment(context, clan, "Company Clerk", "CLK", memberPrivileges, ranks["Specialist"],
			ranks["Staff Sergeant"], companyCommander);
		AddAppointment(context, clan, "Intelligence NCO", "S2 NCO", staffPrivileges, ranks["Staff Sergeant"],
			ranks["First Sergeant"], companyCommander);
		AddAppointment(context, clan, "Communications NCO", "S6 NCO", staffPrivileges, ranks["Staff Sergeant"],
			ranks["First Sergeant"], companyCommander);
	}

	private void SetupBattalionClan(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		if (context.Clans.Any(x => x.Name == "Battalion Template"))
		{
			return;
		}

		var clan = CreateTemplateClan(context, "Battalion Template", "battalion",
			"This template represents a NATO-style battalion using US Army ranks, command billets, and the usual battalion staff system functions.");
		var ranks = BuildOperationalArmyRanks(context, clan);
		var staffPrivileges = BattalionStaffPrivileges() |
		                     (long)(ClanPrivilegeType.CanIncreasePaygrade | ClanPrivilegeType.CanDecreasePaygrade);
		var commandPrivileges = BattalionCommandPrivileges() |
		                       (long)(ClanPrivilegeType.CanChangeRankPath | ClanPrivilegeType.CanPromoteToOwnRank);
		var commander = AddAppointment(context, clan, "Battalion Commander", "BC", commandPrivileges,
			ranks["Lieutenant Colonel"], ranks["Lieutenant Colonel"]);
		AddAppointment(context, clan, "Executive Officer", "XO", commandPrivileges, ranks["Major"],
			ranks["Lieutenant Colonel"], commander);
		AddAppointment(context, clan, "Command Sergeant Major", "CSM",
			staffPrivileges | (long)(ClanPrivilegeType.CanAppoint | ClanPrivilegeType.CanDismiss),
			ranks["Command Sergeant Major"], ranks["Major"], commander);
		AddAppointment(context, clan, "S1 Personnel Officer", "S1", staffPrivileges, ranks["Major"], ranks["Major"],
			commander);
		AddAppointment(context, clan, "S2 Intelligence Officer", "S2", staffPrivileges, ranks["Major"],
			ranks["Major"], commander);
		AddAppointment(context, clan, "S3 Operations Officer", "S3", staffPrivileges, ranks["Major"], ranks["Major"],
			commander);
		AddAppointment(context, clan, "S4 Logistics Officer", "S4", staffPrivileges, ranks["Major"], ranks["Major"],
			commander);
		AddAppointment(context, clan, "S5 Plans Officer", "S5", staffPrivileges, ranks["Major"], ranks["Major"],
			commander);
		AddAppointment(context, clan, "S6 Communications Officer", "S6", staffPrivileges, ranks["Major"],
			ranks["Major"], commander);
		AddAppointment(context, clan, "Battalion Chaplain", "Chap", MemberPrivileges(), ranks["Captain"],
			ranks["Major"], commander);
		AddAppointment(context, clan, "Battalion Surgeon", "Surg", staffPrivileges, ranks["Captain"], ranks["Major"],
			commander);
	}

	private void SetupCapitalShipClan(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> questionAnswers)
	{
		if (context.Clans.Any(x => x.Name == "Capital Ship Template"))
		{
			return;
		}

		var clan = CreateTemplateClan(context, "Capital Ship Template", "carrier",
			"This template represents the command structure of a single capital ship with an aircraft-carrier style focus on command, engineering, logistics, intelligence, and flight operations.");
		var memberPrivileges = MemberPrivileges();
		var staffPrivileges = BattalionStaffPrivileges();
		var commandPrivileges = BattalionCommandPrivileges();
		var ranks = new Dictionary<string, Rank>(StringComparer.OrdinalIgnoreCase)
		{
			["Seaman Recruit"] = AddRank(context, clan, "Seaman Recruit", "SR", 1, memberPrivileges, "Enlisted"),
			["Seaman"] = AddRank(context, clan, "Seaman", "SN", 2, memberPrivileges, "Enlisted"),
			["Petty Officer"] = AddRank(context, clan, "Petty Officer", "PO", 3, staffPrivileges, "Petty Officer"),
			["Chief Petty Officer"] = AddRank(context, clan, "Chief Petty Officer", "CPO", 4, staffPrivileges,
				"Chief"),
			["Master Chief Petty Officer"] = AddRank(context, clan, "Master Chief Petty Officer", "MCPO", 5,
				commandPrivileges, "Chief"),
			["Ensign"] = AddRank(context, clan, "Ensign", "ENS", 6, commandPrivileges, "Officer"),
			["Lieutenant"] = AddRank(context, clan, "Lieutenant", "LT", 7, commandPrivileges, "Officer"),
			["Lieutenant Commander"] = AddRank(context, clan, "Lieutenant Commander", "LCDR", 8, commandPrivileges,
				"Officer"),
			["Commander"] = AddRank(context, clan, "Commander", "CDR", 9, commandPrivileges, "Officer"),
			["Captain"] = AddRank(context, clan, "Captain", "CAPT", 10, (long)ClanPrivilegeType.All, "Officer")
		};

		var captain = AddAppointment(context, clan, "Captain", "CAPT", (long)ClanPrivilegeType.All, ranks["Captain"],
			ranks["Captain"]);
		AddAppointment(context, clan, "Executive Officer", "XO", commandPrivileges, ranks["Commander"],
			ranks["Captain"], captain);
		AddAppointment(context, clan, "Command Master Chief", "CMC",
			staffPrivileges | (long)(ClanPrivilegeType.CanAppoint | ClanPrivilegeType.CanDismiss),
			ranks["Master Chief Petty Officer"], ranks["Commander"], captain);
		AddAppointment(context, clan, "Navigator", "NAV", staffPrivileges, ranks["Lieutenant Commander"],
			ranks["Commander"], captain);
		AddAppointment(context, clan, "Air Boss", "AIR", staffPrivileges, ranks["Commander"], ranks["Commander"],
			captain);
		AddAppointment(context, clan, "Mini Boss", "MINI", staffPrivileges, ranks["Lieutenant Commander"],
			ranks["Commander"], captain);
		AddAppointment(context, clan, "Chief Engineer", "CHENG", staffPrivileges, ranks["Lieutenant Commander"],
			ranks["Commander"], captain);
		AddAppointment(context, clan, "Reactor Officer", "RO", staffPrivileges, ranks["Lieutenant Commander"],
			ranks["Commander"], captain);
		AddAppointment(context, clan, "Operations Officer", "OPS", staffPrivileges, ranks["Lieutenant Commander"],
			ranks["Commander"], captain);
		AddAppointment(context, clan, "Intelligence Officer", "N2", staffPrivileges, ranks["Lieutenant"],
			ranks["Lieutenant Commander"], captain);
		AddAppointment(context, clan, "Communications Officer", "N6", staffPrivileges, ranks["Lieutenant"],
			ranks["Lieutenant Commander"], captain);
		AddAppointment(context, clan, "Supply Officer", "SUPPO", staffPrivileges, ranks["Lieutenant"],
			ranks["Lieutenant Commander"], captain);
		AddAppointment(context, clan, "Medical Officer", "MED", staffPrivileges, ranks["Lieutenant"],
			ranks["Lieutenant Commander"], captain);
		AddAppointment(context, clan, "Weapons Officer", "WEPS", staffPrivileges, ranks["Lieutenant"],
			ranks["Lieutenant Commander"], captain);
		AddAppointment(context, clan, "Security Officer", "SEC", staffPrivileges, ranks["Lieutenant"],
			ranks["Lieutenant Commander"], captain);
	}

	private void SetupAgeOfSailShipClan(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> questionAnswers)
	{
		if (context.Clans.Any(x => x.Name == "Age of Sail Ship Template"))
		{
			return;
		}

		var clan = CreateTemplateClan(context, "Age of Sail Ship Template", "ageofsail",
			"This template represents the clan structure of a classic age-of-sail warship with command billets, deck officers, warrant officers, and specialist roles.");
		var memberPrivileges = MemberPrivileges();
		var pettyPrivileges = BattalionStaffPrivileges();
		var commandPrivileges = BattalionCommandPrivileges();
		var landsman = AddRank(context, clan, "Landsman", "Land", 1, memberPrivileges, "Crew");
		var ordinarySeaman = AddRank(context, clan, "Ordinary Seaman", "OS", 2, memberPrivileges, "Crew");
		var ableSeaman = AddRank(context, clan, "Able Seaman", "AB", 3, pettyPrivileges, "Crew");
		var midshipman = AddRank(context, clan, "Midshipman", "Mid", 4, pettyPrivileges, "Officer");
		var lieutenant = AddRank(context, clan, "Lieutenant", "Lt", 5, commandPrivileges, "Officer");
		var commander = AddRank(context, clan, "Commander", "Cdr", 6,
			commandPrivileges | (long)ClanPrivilegeType.CanPromoteToOwnRank, "Officer");

		var captain = AddAppointment(context, clan, "Captain", "Capt", (long)ClanPrivilegeType.All, commander,
			commander);
		AddAppointment(context, clan, "Sailing Master", "Master", pettyPrivileges, lieutenant, commander, captain);
		AddAppointment(context, clan, "First Lieutenant", "1st Lt", commandPrivileges, lieutenant, commander, captain);
		AddAppointment(context, clan, "Second Lieutenant", "2nd Lt", commandPrivileges, lieutenant, commander, captain);
		AddAppointment(context, clan, "Boatswain", "Bosun", pettyPrivileges, ableSeaman, lieutenant, captain);
		AddAppointment(context, clan, "Gunner", "Gnr", pettyPrivileges, ableSeaman, lieutenant, captain);
		AddAppointment(context, clan, "Carpenter", "Carp", pettyPrivileges, ableSeaman, lieutenant, captain);
		AddAppointment(context, clan, "Sailmaker", "Sail", pettyPrivileges, ableSeaman, lieutenant, captain);
		AddAppointment(context, clan, "Purser", "Purs", pettyPrivileges, midshipman, lieutenant, captain);
		AddAppointment(context, clan, "Surgeon", "Surg", pettyPrivileges, midshipman, lieutenant, captain);
		AddAppointment(context, clan, "Marine Lieutenant", "MLt", commandPrivileges, lieutenant, commander, captain);
		AddAppointment(context, clan, "Coxswain", "Cox", pettyPrivileges, ableSeaman, lieutenant, captain);
		AddAppointment(context, clan, "Quartermaster", "QM", pettyPrivileges, ordinarySeaman, lieutenant, captain);
	}
}
