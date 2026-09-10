using MudSharp.Community;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.TimeAndDate.Intervals;
using System;
using System.Collections.Generic;
using System.Linq;
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
                      ClanPrivilegeType.CanManageClanProperty |
                      ClanPrivilegeType.UseClanProperty);
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
        Clan clan = new()
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
        Rank rank = new()
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
        Paygrade paygrade = new()
        {
            Name = name,
            Abbreviation = abbreviation,
            Currency = context.Currencies.First(),
            PayAmount = amount,
            Clan = clan
        };
        context.Paygrades.Add(paygrade);
        context.SaveChanges();

        foreach (Rank linkedRank in linkedRanks)
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
        Appointment appointment = new()
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
        Appointment appointment = new()
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
        long memberPrivileges = MemberPrivileges();
        long ncoPrivileges = memberPrivileges | (long)ClanPrivilegeType.CanReportDead;
        long seniorNcoPrivileges = BattalionStaffPrivileges() |
                                  (long)(ClanPrivilegeType.CanIncreasePaygrade |
                                         ClanPrivilegeType.CanDecreasePaygrade);
        long officerPrivileges = BattalionCommandPrivileges();

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
        Dictionary<string, Rank> ranks = context.Ranks
            .Where(x => x.ClanId == clan.Id)
            .ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
        Dictionary<string, Appointment> existingAppointments = context.Appointments
            .Where(x => x.ClanId == clan.Id)
            .ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
        long memberPrivileges = MemberPrivileges();
        long staffPrivileges = BattalionStaffPrivileges() |
                             (long)(ClanPrivilegeType.CanIncreasePaygrade | ClanPrivilegeType.CanDecreasePaygrade);
        long commandPrivileges = BattalionCommandPrivileges() |
                               (long)(ClanPrivilegeType.CanChangeRankPath | ClanPrivilegeType.CanPromoteToOwnRank);

        Rank general = ranks["General"];
        Rank colonel = ranks["Colonel"];
        Rank lieutenantColonel = ranks["Lieutenant Colonel"];
        Rank major = ranks["Major"];
        Rank sergeantMajor = ranks["Sergeant Major"];
        Rank captain = ranks["Captain"];

        Appointment EnsureArmyAppointment(string name, string abbreviation, long privileges, Rank minimumRank,
            Rank minimumRankToAppoint, Appointment? parent = null)
        {
            if (existingAppointments.TryGetValue(name, out Appointment? existingAppointment))
            {
                return existingAppointment;
            }

            Appointment appointment = AddAppointment(context, clan, name, abbreviation, privileges, minimumRank,
                minimumRankToAppoint, parent);
            existingAppointments[name] = appointment;
            return appointment;
        }

        Appointment commandingOfficer = EnsureArmyAppointment("Commanding Officer", "CO", commandPrivileges, colonel, colonel);
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

        Clan clan = CreateTemplateClan(context, "Naval Organisation Template", "navy",
            "This is designed to be used as a template clan for a modern naval organisation using US Navy style NATO-coded ranks and staff appointments.");
        long memberPrivileges = MemberPrivileges();
        long seniorPrivileges = BattalionStaffPrivileges();
        long commandPrivileges = BattalionCommandPrivileges();
        Dictionary<string, Rank> ranks = new(StringComparer.OrdinalIgnoreCase)
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

        Appointment commandingOfficer = AddAppointment(context, clan, "Commanding Officer", "CO", commandPrivileges,
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

        Clan clan = CreateTemplateClan(context, "Air Force Template", "airforce",
            "This is designed to be used as a template clan for a modern air force using US Air Force style NATO-coded ranks and staff appointments.");
        long memberPrivileges = MemberPrivileges();
        long seniorPrivileges = BattalionStaffPrivileges();
        long commandPrivileges = BattalionCommandPrivileges();
        Dictionary<string, Rank> ranks = new(StringComparer.OrdinalIgnoreCase)
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

        Appointment commandingOfficer = AddAppointment(context, clan, "Commanding Officer", "CC", commandPrivileges,
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

        Clan clan = CreateTemplateClan(context, name, alias, description, "Civic");
        Rank resident = AddRank(context, clan, "Resident", "Res", 1, MemberPrivileges(), "Citizen");
        Rank councillor = AddRank(context, clan, "Councillor", "Cllr", 2,
            MemberPrivileges() | (long)(ClanPrivilegeType.CanViewTreasury | ClanPrivilegeType.CanManageClanJobs),
            "Civic Office");
        long mayorPrivileges = CompanyCommandPrivileges() |
                              (long)(ClanPrivilegeType.CanManageEconomicZones | ClanPrivilegeType.CanCreateAppointments);
        Rank mayorMinimumRank = mayorRequiresCouncillorRank ? councillor : resident;

        Appointment mayor = AddElectionAppointment(context, clan, "Mayor", "Mayor", mayorPrivileges,
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

        Clan clan = CreateTemplateClan(context, "Company Template", "company",
            "This template represents a generic company with employees, managers, executives, and a board, including a board-elected chief executive.");
        Rank employee = AddRank(context, clan, "Employee", "Emp", 1, MemberPrivileges(), "Staff");
        Rank supervisor = AddRank(context, clan, "Supervisor", "Sup", 2, CompanyStaffPrivileges(), "Staff");
        Rank middleManager = AddRank(context, clan, "Middle Manager", "Mgr", 3, CompanyStaffPrivileges(), "Management");
        Rank seniorManager = AddRank(context, clan, "Senior Manager", "SrMgr", 4, CompanyCommandPrivileges(),
            "Management");
        Rank executive = AddRank(context, clan, "Executive", "Exec", 5, CompanyCommandPrivileges(), "Executive");
        Rank board = AddRank(context, clan, "Board", "Board", 6,
            CompanyCommandPrivileges() | (long)ClanPrivilegeType.CanPromoteToOwnRank, "Board");

        AddPaygrade(context, clan, "Employee Grade 1", "EMP-1", 1000.0M, employee);
        AddPaygrade(context, clan, "Employee Grade 2", "EMP-2", 1250.0M, employee);
        AddPaygrade(context, clan, "Supervisor Grade 1", "SUP-1", 1500.0M, supervisor);
        AddPaygrade(context, clan, "Supervisor Grade 2", "SUP-2", 1800.0M, supervisor);
        AddPaygrade(context, clan, "Middle Manager Grade 1", "MM-1", 2400.0M, middleManager);
        AddPaygrade(context, clan, "Middle Manager Grade 2", "MM-2", 2800.0M, middleManager);
        AddPaygrade(context, clan, "Senior Manager Grade 1", "SM-1", 3600.0M, seniorManager);
        AddPaygrade(context, clan, "Senior Manager Grade 2", "SM-2", 4200.0M, seniorManager);
        Paygrade executiveGrade1 = AddPaygrade(context, clan, "Executive Grade 1", "EX-1", 5500.0M, executive);
        AddPaygrade(context, clan, "Executive Grade 2", "EX-2", 6500.0M, executive);
        AddPaygrade(context, clan, "Board Grade 1", "BRD-1", 8000.0M, board);
        Paygrade boardGrade2 = AddPaygrade(context, clan, "Board Grade 2", "BRD-2", 10000.0M, board);

        long boardPrivileges = CompanyCommandPrivileges() |
                              (long)(ClanPrivilegeType.CanCreatePaygrades | ClanPrivilegeType.CanCreateRanks |
                                     ClanPrivilegeType.CanCreateAppointments);
        Appointment ceo = AddElectionAppointment(context, clan, "Chief Executive Officer", "CEO", (long)ClanPrivilegeType.All,
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

        Clan clan = CreateTemplateClan(context, "Mercenary Company Template", "mercs",
            "This template represents a medieval mercenary company built around a captain, trusted officers, veteran fighters, and support retainers.");
        long memberPrivileges = MemberPrivileges();
        long sergeantPrivileges = BattalionStaffPrivileges() |
                                 (long)(ClanPrivilegeType.CanInduct | ClanPrivilegeType.CanReportDead);
        long officerPrivileges = BattalionCommandPrivileges();
        Rank recruit = AddRank(context, clan, "Recruit", "Rec", 1, memberPrivileges, "Mercenary");
        Rank mercenary = AddRank(context, clan, "Mercenary", "Merc", 2, memberPrivileges, "Mercenary");
        Rank veteran = AddRank(context, clan, "Veteran Mercenary", "Vet", 3, memberPrivileges, "Mercenary");
        Rank sergeant = AddRank(context, clan, "Sergeant", "Sgt", 4, sergeantPrivileges, "Officer");
        Rank lieutenant = AddRank(context, clan, "Lieutenant", "Lt", 5, officerPrivileges, "Officer");
        Rank captain = AddRank(context, clan, "Captain", "Cpt", 6,
            officerPrivileges | (long)ClanPrivilegeType.CanPromoteToOwnRank, "Officer");

        AddPaygrade(context, clan, "Recruit Share", "R-Share", 150.0M, recruit);
        AddPaygrade(context, clan, "Company Share", "C-Share", 250.0M, mercenary);
        AddPaygrade(context, clan, "Veteran Share", "V-Share", 400.0M, veteran);
        AddPaygrade(context, clan, "Sergeant Share", "S-Share", 550.0M, sergeant);
        Paygrade lieutenantShare = AddPaygrade(context, clan, "Lieutenant Share", "L-Share", 750.0M, lieutenant);
        Paygrade captainShare = AddPaygrade(context, clan, "Captain Share", "Cap-Share", 1000.0M, captain);

        Appointment mercenaryCaptain = AddAppointment(context, clan, "Mercenary Captain", "MC", (long)ClanPrivilegeType.All,
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

        Clan clan = CreateTemplateClan(context, "Infantry Company Template", "infcompany",
            "This template represents a NATO-style infantry company using US Army ranks, company command billets, platoon leadership appointments, and practical functional staff roles.");
        Dictionary<string, Rank> ranks = BuildOperationalArmyRanks(context, clan);
        long memberPrivileges = MemberPrivileges();
        long staffPrivileges = BattalionStaffPrivileges();
        long commandPrivileges = BattalionCommandPrivileges();

        Appointment companyCommander = AddAppointment(context, clan, "Company Commander", "CO", commandPrivileges,
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

        Clan clan = CreateTemplateClan(context, "Battalion Template", "battalion",
            "This template represents a NATO-style battalion using US Army ranks, command billets, and the usual battalion staff system functions.");
        Dictionary<string, Rank> ranks = BuildOperationalArmyRanks(context, clan);
        long staffPrivileges = BattalionStaffPrivileges() |
                             (long)(ClanPrivilegeType.CanIncreasePaygrade | ClanPrivilegeType.CanDecreasePaygrade);
        long commandPrivileges = BattalionCommandPrivileges() |
                               (long)(ClanPrivilegeType.CanChangeRankPath | ClanPrivilegeType.CanPromoteToOwnRank);
        Appointment commander = AddAppointment(context, clan, "Battalion Commander", "BC", commandPrivileges,
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

        Clan clan = CreateTemplateClan(context, "Capital Ship Template", "carrier",
            "This template represents the command structure of a single capital ship with an aircraft-carrier style focus on command, engineering, logistics, intelligence, and flight operations.");
        long memberPrivileges = MemberPrivileges();
        long staffPrivileges = BattalionStaffPrivileges();
        long commandPrivileges = BattalionCommandPrivileges();
        Dictionary<string, Rank> ranks = new(StringComparer.OrdinalIgnoreCase)
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

        Appointment captain = AddAppointment(context, clan, "Captain", "CAPT", (long)ClanPrivilegeType.All, ranks["Captain"],
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

        Clan clan = CreateTemplateClan(context, "Age of Sail Ship Template", "ageofsail",
            "This template represents the clan structure of a classic age-of-sail warship with command billets, deck officers, warrant officers, and specialist roles.");
        long memberPrivileges = MemberPrivileges();
        long pettyPrivileges = BattalionStaffPrivileges();
        long commandPrivileges = BattalionCommandPrivileges();
        Rank landsman = AddRank(context, clan, "Landsman", "Land", 1, memberPrivileges, "Crew");
        Rank ordinarySeaman = AddRank(context, clan, "Ordinary Seaman", "OS", 2, memberPrivileges, "Crew");
        Rank ableSeaman = AddRank(context, clan, "Able Seaman", "AB", 3, pettyPrivileges, "Crew");
        Rank midshipman = AddRank(context, clan, "Midshipman", "Mid", 4, pettyPrivileges, "Officer");
        Rank lieutenant = AddRank(context, clan, "Lieutenant", "Lt", 5, commandPrivileges, "Officer");
        Rank commander = AddRank(context, clan, "Commander", "Cdr", 6,
            commandPrivileges | (long)ClanPrivilegeType.CanPromoteToOwnRank, "Officer");

        Appointment captain = AddAppointment(context, clan, "Captain", "Capt", (long)ClanPrivilegeType.All, commander,
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

	private enum TemplatePrivilegeTier
	{
		Member,
		Staff,
		Command,
		All
	}

	private sealed record ClanTemplateSpecification(
		string Name,
		string Alias,
		string Description,
		string Sphere,
		IReadOnlyList<TemplateRankSpecification> Ranks,
		IReadOnlyList<TemplateAppointmentSpecification> Appointments);

	private sealed record TemplateRankSpecification(
		string Name,
		string Abbreviation,
		int RankNumber,
		string RankPath,
		TemplatePrivilegeTier PrivilegeTier);

	private sealed record TemplateAppointmentSpecification(
		string Name,
		string Abbreviation,
		TemplatePrivilegeTier PrivilegeTier,
		string MinimumRank,
		string? ParentAppointment = null,
		int MaximumSimultaneousHolders = 1);

	private static TemplateRankSpecification TemplateRank(string name, string abbreviation, int rankNumber,
		string rankPath, TemplatePrivilegeTier privilegeTier = TemplatePrivilegeTier.Member)
	{
		return new TemplateRankSpecification(name, abbreviation, rankNumber, rankPath, privilegeTier);
	}

	private static TemplateAppointmentSpecification TemplateAppointment(string name, string abbreviation,
		TemplatePrivilegeTier privilegeTier, string minimumRank, string? parentAppointment = null,
		int maximumSimultaneousHolders = 1)
	{
		return new TemplateAppointmentSpecification(name, abbreviation, privilegeTier, minimumRank,
			parentAppointment, maximumSimultaneousHolders);
	}

	private static readonly IReadOnlyList<ClanTemplateSpecification> AdditionalClanTemplateSpecifications =
	[
		new(
			"Extended Family Template", "family",
			"A multi-generational household template with dependants, adult members, elders, succession, care, and shared-property responsibilities.",
			"Kinship",
			[
				TemplateRank("Dependant", "Dep", 1, "Household"),
				TemplateRank("Adult Member", "Adult", 2, "Household"),
				TemplateRank("Senior Member", "Senior", 3, "Household", TemplatePrivilegeTier.Staff),
				TemplateRank("Family Head", "Head", 4, "Household", TemplatePrivilegeTier.Command)
			],
			[
				TemplateAppointment("Household Head", "Head", TemplatePrivilegeTier.All, "Family Head"),
				TemplateAppointment("Family Elder", "Elder", TemplatePrivilegeTier.Command, "Senior Member", "Household Head", 0),
				TemplateAppointment("Household Steward", "Steward", TemplatePrivilegeTier.Staff, "Adult Member", "Household Head"),
				TemplateAppointment("Designated Heir", "Heir", TemplatePrivilegeTier.Staff, "Adult Member", "Household Head"),
				TemplateAppointment("Caregiver", "Carer", TemplatePrivilegeTier.Staff, "Adult Member", "Household Head", 0)
			]),
		new(
			"Lineage Clan Template", "lineage",
			"A descent-based clan or great house template with lineage branches, an elder council, genealogy, and collective assets.",
			"Kinship",
			[
				TemplateRank("Affiliate", "Aff", 1, "Affiliated"),
				TemplateRank("Lineage Member", "Member", 2, "Lineage"),
				TemplateRank("Branch Representative", "Branch", 3, "Lineage", TemplatePrivilegeTier.Staff),
				TemplateRank("Clan Elder", "Elder", 4, "Lineage", TemplatePrivilegeTier.Command)
			],
			[
				TemplateAppointment("Clan Head", "Head", TemplatePrivilegeTier.All, "Clan Elder"),
				TemplateAppointment("Council Elder", "Council", TemplatePrivilegeTier.Command, "Clan Elder", "Clan Head", 0),
				TemplateAppointment("Branch Head", "Branch", TemplatePrivilegeTier.Command, "Branch Representative", "Clan Head", 0),
				TemplateAppointment("Genealogist", "Geneal", TemplatePrivilegeTier.Staff, "Lineage Member", "Clan Head"),
				TemplateAppointment("Clan Treasurer", "Treas", TemplatePrivilegeTier.Command, "Branch Representative", "Clan Head")
			]),
		new(
			"Tribal Council Template", "tribalcouncil",
			"A broad kin-and-community governance template with respected elders, customary offices, collective decisions, and war leadership.",
			"Traditional Government",
			[
				TemplateRank("Community Member", "Member", 1, "Community"),
				TemplateRank("Proven Adult", "Proven", 2, "Community"),
				TemplateRank("Elder", "Elder", 3, "Council", TemplatePrivilegeTier.Staff),
				TemplateRank("Paramount Leader", "Leader", 4, "Council", TemplatePrivilegeTier.Command)
			],
			[
				TemplateAppointment("Chief", "Chief", TemplatePrivilegeTier.All, "Paramount Leader"),
				TemplateAppointment("Council Elder", "Council", TemplatePrivilegeTier.Command, "Elder", "Chief", 0),
				TemplateAppointment("War Leader", "War", TemplatePrivilegeTier.Command, "Proven Adult", "Chief"),
				TemplateAppointment("Speaker", "Speaker", TemplatePrivilegeTier.Staff, "Proven Adult", "Chief"),
				TemplateAppointment("Keeper of Traditions", "Keeper", TemplatePrivilegeTier.Staff, "Elder", "Chief")
			]),
		new(
			"Norse Warband Template", "warband",
			"A household warband template suitable for Viking-age or similar retainer cultures, with warriors, household guards, law, and reputation roles.",
			"Historical Military",
			[
				TemplateRank("Camp Follower", "Follower", 1, "Household"),
				TemplateRank("Free Warrior", "Warrior", 2, "Warrior"),
				TemplateRank("Retainer", "Ret", 3, "Warrior", TemplatePrivilegeTier.Staff),
				TemplateRank("Household Guard", "Guard", 4, "Warrior", TemplatePrivilegeTier.Command),
				TemplateRank("Jarl", "Jarl", 5, "Lord", TemplatePrivilegeTier.Command)
			],
			[
				TemplateAppointment("Jarl", "Jarl", TemplatePrivilegeTier.All, "Jarl"),
				TemplateAppointment("Hersir", "Hersir", TemplatePrivilegeTier.Command, "Household Guard", "Jarl"),
				TemplateAppointment("Standard Bearer", "Banner", TemplatePrivilegeTier.Staff, "Retainer", "Jarl"),
				TemplateAppointment("Lawspeaker", "Law", TemplatePrivilegeTier.Staff, "Retainer", "Jarl"),
				TemplateAppointment("Skald", "Skald", TemplatePrivilegeTier.Member, "Free Warrior", "Jarl")
			]),
		new(
			"Steppe Horde Template", "horde",
			"A mobile confederation and army template for steppe societies, combining household membership, mounted commands, logistics, diplomacy, and ritual authority.",
			"Historical Military",
			[
				TemplateRank("Camp Member", "Camp", 1, "Household"),
				TemplateRank("Rider", "Rider", 2, "Warrior"),
				TemplateRank("Veteran Rider", "Veteran", 3, "Warrior", TemplatePrivilegeTier.Staff),
				TemplateRank("Noyan", "Noyan", 4, "Commander", TemplatePrivilegeTier.Command),
				TemplateRank("Khan", "Khan", 5, "Ruler", TemplatePrivilegeTier.Command)
			],
			[
				TemplateAppointment("Khan", "Khan", TemplatePrivilegeTier.All, "Khan"),
				TemplateAppointment("Orlok", "Orlok", TemplatePrivilegeTier.Command, "Noyan", "Khan"),
				TemplateAppointment("Tumen Commander", "Tumen", TemplatePrivilegeTier.Command, "Noyan", "Khan", 0),
				TemplateAppointment("Chief Quartermaster", "Quarter", TemplatePrivilegeTier.Staff, "Veteran Rider", "Khan"),
				TemplateAppointment("Envoy", "Envoy", TemplatePrivilegeTier.Staff, "Veteran Rider", "Khan", 0),
				TemplateAppointment("Ritual Specialist", "Ritual", TemplatePrivilegeTier.Member, "Veteran Rider", "Khan")
			]),
		new(
			"Japanese Feudal Domain Template", "han",
			"A Japanese-inspired feudal domain template with ashigaru, samurai retainers, senior councillors, inspectors, castles, and civil administration.",
			"Traditional Government",
			[
				TemplateRank("Household Retainer", "Ret", 1, "Retainer"),
				TemplateRank("Ashigaru", "Ashi", 2, "Military"),
				TemplateRank("Samurai", "Sam", 3, "Military", TemplatePrivilegeTier.Staff),
				TemplateRank("Hatamoto", "Hata", 4, "Senior Retainer", TemplatePrivilegeTier.Command),
				TemplateRank("Daimyo", "Daimyo", 5, "Lord", TemplatePrivilegeTier.Command)
			],
			[
				TemplateAppointment("Daimyo", "Daimyo", TemplatePrivilegeTier.All, "Daimyo"),
				TemplateAppointment("Senior Councillor", "Karo", TemplatePrivilegeTier.Command, "Hatamoto", "Daimyo", 0),
				TemplateAppointment("Magistrate", "Bugyo", TemplatePrivilegeTier.Command, "Samurai", "Daimyo", 0),
				TemplateAppointment("Castellan", "Castle", TemplatePrivilegeTier.Command, "Hatamoto", "Daimyo", 0),
				TemplateAppointment("Inspector", "Metsuke", TemplatePrivilegeTier.Staff, "Samurai", "Daimyo", 0),
				TemplateAppointment("Military Commissioner", "Gun", TemplatePrivilegeTier.Command, "Hatamoto", "Daimyo")
			]),
		new(
			"East Asian Imperial Bureaucracy Template", "imperialcourt",
			"An adaptable examination-and-ministry bureaucracy for Chinese, Korean, Vietnamese, or similarly organised imperial settings.",
			"Traditional Government",
			[
				TemplateRank("Examination Candidate", "Cand", 1, "Civil Service"),
				TemplateRank("Clerk", "Clerk", 2, "Civil Service"),
				TemplateRank("Magistrate", "Mag", 3, "Civil Service", TemplatePrivilegeTier.Staff),
				TemplateRank("Provincial Official", "Prov", 4, "Civil Service", TemplatePrivilegeTier.Command),
				TemplateRank("Minister", "Min", 5, "Court", TemplatePrivilegeTier.Command)
			],
			[
				TemplateAppointment("Imperial Sovereign", "Sovereign", TemplatePrivilegeTier.All, "Minister"),
				TemplateAppointment("Grand Chancellor", "Chancellor", TemplatePrivilegeTier.Command, "Minister", "Imperial Sovereign"),
				TemplateAppointment("Minister of Personnel", "Personnel", TemplatePrivilegeTier.Command, "Minister", "Grand Chancellor"),
				TemplateAppointment("Minister of Revenue", "Revenue", TemplatePrivilegeTier.Command, "Minister", "Grand Chancellor"),
				TemplateAppointment("Minister of Rites", "Rites", TemplatePrivilegeTier.Command, "Minister", "Grand Chancellor"),
				TemplateAppointment("Minister of War", "War", TemplatePrivilegeTier.Command, "Minister", "Grand Chancellor"),
				TemplateAppointment("Minister of Justice", "Justice", TemplatePrivilegeTier.Command, "Minister", "Grand Chancellor"),
				TemplateAppointment("Minister of Works", "Works", TemplatePrivilegeTier.Command, "Minister", "Grand Chancellor"),
				TemplateAppointment("Chief Censor", "Censor", TemplatePrivilegeTier.Staff, "Provincial Official", "Imperial Sovereign")
			]),
		new(
			"East Asian Imperial Army Template", "imperialarmy",
			"A traditional East Asian imperial army template with soldiers, unit officers, generals, inspectors, logistics, engineering, and palace command.",
			"Historical Military",
			[
				TemplateRank("Conscript", "Con", 1, "Soldier"),
				TemplateRank("Soldier", "Sold", 2, "Soldier"),
				TemplateRank("Unit Leader", "Leader", 3, "Soldier", TemplatePrivilegeTier.Staff),
				TemplateRank("Military Officer", "Officer", 4, "Officer", TemplatePrivilegeTier.Command),
				TemplateRank("General", "General", 5, "Officer", TemplatePrivilegeTier.Command)
			],
			[
				TemplateAppointment("Commander-in-Chief", "C-in-C", TemplatePrivilegeTier.All, "General"),
				TemplateAppointment("Field General", "Field", TemplatePrivilegeTier.Command, "General", "Commander-in-Chief", 0),
				TemplateAppointment("Army Inspector", "Inspect", TemplatePrivilegeTier.Command, "Military Officer", "Commander-in-Chief"),
				TemplateAppointment("Quartermaster General", "Quarter", TemplatePrivilegeTier.Staff, "Military Officer", "Commander-in-Chief"),
				TemplateAppointment("Chief Engineer", "Engineer", TemplatePrivilegeTier.Staff, "Military Officer", "Commander-in-Chief"),
				TemplateAppointment("Palace Guard Commander", "Palace", TemplatePrivilegeTier.Command, "Military Officer", "Commander-in-Chief")
			]),
		new(
			"Islamic Sultanate Court Template", "sultanate",
			"A broad medieval or early-modern sultanate court template with the sovereign, vizierate, judiciary, fiscal offices, household, and military command.",
			"Traditional Government",
			[
				TemplateRank("Court Servant", "Serv", 1, "Household"),
				TemplateRank("Courtier", "Court", 2, "Court"),
				TemplateRank("Amir", "Amir", 3, "Military", TemplatePrivilegeTier.Staff),
				TemplateRank("Vizier", "Viz", 4, "Administration", TemplatePrivilegeTier.Command),
				TemplateRank("Sultan", "Sultan", 5, "Sovereign", TemplatePrivilegeTier.Command)
			],
			[
				TemplateAppointment("Sultan", "Sultan", TemplatePrivilegeTier.All, "Sultan"),
				TemplateAppointment("Grand Vizier", "Vizier", TemplatePrivilegeTier.Command, "Vizier", "Sultan"),
				TemplateAppointment("Chief Judge", "Qadi", TemplatePrivilegeTier.Command, "Vizier", "Sultan"),
				TemplateAppointment("Treasury Minister", "Diwan", TemplatePrivilegeTier.Command, "Vizier", "Grand Vizier"),
				TemplateAppointment("Lord Chamberlain", "Hajib", TemplatePrivilegeTier.Staff, "Courtier", "Sultan"),
				TemplateAppointment("Commander of Commanders", "Amir", TemplatePrivilegeTier.Command, "Amir", "Sultan"),
				TemplateAppointment("Intelligence Chief", "Intel", TemplatePrivilegeTier.Staff, "Courtier", "Grand Vizier")
			]),
		new(
			"South Asian Royal Court Template", "southasiancourt",
			"An adaptable South Asian royal court with household retainers, ranked service, civil administration, military command, ritual advice, and urban security.",
			"Traditional Government",
			[
				TemplateRank("Household Retainer", "Ret", 1, "Household"),
				TemplateRank("Courtier", "Court", 2, "Court"),
				TemplateRank("Ranked Officer", "Officer", 3, "Service", TemplatePrivilegeTier.Staff),
				TemplateRank("Minister", "Min", 4, "Administration", TemplatePrivilegeTier.Command),
				TemplateRank("Sovereign", "Sov", 5, "Sovereign", TemplatePrivilegeTier.Command)
			],
			[
				TemplateAppointment("Sovereign", "Sov", TemplatePrivilegeTier.All, "Sovereign"),
				TemplateAppointment("Chief Minister", "Diwan", TemplatePrivilegeTier.Command, "Minister", "Sovereign"),
				TemplateAppointment("Commander of the Army", "Senapati", TemplatePrivilegeTier.Command, "Ranked Officer", "Sovereign"),
				TemplateAppointment("Royal Preceptor", "Preceptor", TemplatePrivilegeTier.Staff, "Courtier", "Sovereign"),
				TemplateAppointment("City Constable", "Kotwal", TemplatePrivilegeTier.Command, "Ranked Officer", "Chief Minister"),
				TemplateAppointment("Royal Treasurer", "Treas", TemplatePrivilegeTier.Command, "Minister", "Chief Minister")
			]),
		new(
			"West African Royal Court Template", "westafricancourt",
			"A flexible royal-court template inspired by West and Central African kingdoms, with lineage authority, provincial chiefs, oral historians, and military offices.",
			"Traditional Government",
			[
				TemplateRank("Community Member", "Member", 1, "Community"),
				TemplateRank("Court Retainer", "Ret", 2, "Court"),
				TemplateRank("Provincial Chief", "Chief", 3, "Provincial", TemplatePrivilegeTier.Staff),
				TemplateRank("Court Elder", "Elder", 4, "Court", TemplatePrivilegeTier.Command),
				TemplateRank("Sovereign", "Sov", 5, "Royal", TemplatePrivilegeTier.Command)
			],
			[
				TemplateAppointment("Sovereign", "Sov", TemplatePrivilegeTier.All, "Sovereign"),
				TemplateAppointment("Queen Mother", "QMother", TemplatePrivilegeTier.Command, "Court Elder", "Sovereign"),
				TemplateAppointment("Council Elder", "Council", TemplatePrivilegeTier.Command, "Court Elder", "Sovereign", 0),
				TemplateAppointment("Provincial Chief", "Province", TemplatePrivilegeTier.Command, "Provincial Chief", "Sovereign", 0),
				TemplateAppointment("War Captain", "War", TemplatePrivilegeTier.Command, "Court Retainer", "Sovereign"),
				TemplateAppointment("Oral Historian", "Griot", TemplatePrivilegeTier.Staff, "Court Retainer", "Sovereign", 0),
				TemplateAppointment("Royal Treasurer", "Treas", TemplatePrivilegeTier.Command, "Court Elder", "Sovereign")
			]),
		new(
			"Roman Religious Cult Template", "romancult",
			"A Roman civic or temple cult template with initiates, ritual personnel, priests, divination offices, and a chief priesthood.",
			"Religion",
			[
				TemplateRank("Initiate", "Init", 1, "Congregant"),
				TemplateRank("Cult Member", "Member", 2, "Congregant"),
				TemplateRank("Ritual Attendant", "Attend", 3, "Temple", TemplatePrivilegeTier.Staff),
				TemplateRank("Priest", "Priest", 4, "Priesthood", TemplatePrivilegeTier.Command),
				TemplateRank("Senior Priest", "Senior", 5, "Priesthood", TemplatePrivilegeTier.Command)
			],
			[
				TemplateAppointment("Chief Pontiff", "Pontifex", TemplatePrivilegeTier.All, "Senior Priest"),
				TemplateAppointment("Temple Priest", "Flamen", TemplatePrivilegeTier.Command, "Priest", "Chief Pontiff", 0),
				TemplateAppointment("Augur", "Augur", TemplatePrivilegeTier.Staff, "Priest", "Chief Pontiff", 0),
				TemplateAppointment("Ritual Steward", "Steward", TemplatePrivilegeTier.Staff, "Ritual Attendant", "Chief Pontiff"),
				TemplateAppointment("Temple Treasurer", "Treas", TemplatePrivilegeTier.Command, "Priest", "Chief Pontiff")
			]),
		new(
			"Buddhist Temple Template", "buddhisttemple",
			"A broad Buddhist monastic community template with lay supporters, novices, ordained monastics, teaching, discipline, hospitality, and alms administration.",
			"Religion",
			[
				TemplateRank("Lay Supporter", "Lay", 1, "Lay"),
				TemplateRank("Novice", "Novice", 2, "Monastic"),
				TemplateRank("Ordained Monastic", "Ord", 3, "Monastic", TemplatePrivilegeTier.Staff),
				TemplateRank("Senior Monastic", "Senior", 4, "Monastic", TemplatePrivilegeTier.Command),
				TemplateRank("Abbot", "Abbot", 5, "Leadership", TemplatePrivilegeTier.Command)
			],
			[
				TemplateAppointment("Abbot", "Abbot", TemplatePrivilegeTier.All, "Abbot"),
				TemplateAppointment("Preceptor", "Precept", TemplatePrivilegeTier.Command, "Senior Monastic", "Abbot"),
				TemplateAppointment("Disciplinarian", "Discipline", TemplatePrivilegeTier.Command, "Senior Monastic", "Abbot"),
				TemplateAppointment("Chant Leader", "Chant", TemplatePrivilegeTier.Staff, "Ordained Monastic", "Abbot"),
				TemplateAppointment("Guest Master", "Guest", TemplatePrivilegeTier.Staff, "Ordained Monastic", "Abbot"),
				TemplateAppointment("Almoner", "Alms", TemplatePrivilegeTier.Staff, "Ordained Monastic", "Abbot")
			]),
		new(
			"Daoist Temple Template", "daoisttemple",
			"A Daoist-inspired temple community with lay affiliates, disciples, ordained priests, ritual masters, scripture custody, charity, and discipline.",
			"Religion",
			[
				TemplateRank("Lay Affiliate", "Lay", 1, "Lay"),
				TemplateRank("Disciple", "Disc", 2, "Temple"),
				TemplateRank("Ordained Priest", "Priest", 3, "Priesthood", TemplatePrivilegeTier.Staff),
				TemplateRank("Senior Priest", "Senior", 4, "Priesthood", TemplatePrivilegeTier.Command),
				TemplateRank("Temple Master", "Master", 5, "Leadership", TemplatePrivilegeTier.Command)
			],
			[
				TemplateAppointment("Temple Master", "Master", TemplatePrivilegeTier.All, "Temple Master"),
				TemplateAppointment("Ritual Master", "Ritual", TemplatePrivilegeTier.Command, "Senior Priest", "Temple Master"),
				TemplateAppointment("Scripture Keeper", "Scripture", TemplatePrivilegeTier.Staff, "Ordained Priest", "Temple Master"),
				TemplateAppointment("Temple Steward", "Steward", TemplatePrivilegeTier.Command, "Senior Priest", "Temple Master"),
				TemplateAppointment("Almoner", "Alms", TemplatePrivilegeTier.Staff, "Ordained Priest", "Temple Master"),
				TemplateAppointment("Disciplinarian", "Discipline", TemplatePrivilegeTier.Staff, "Ordained Priest", "Temple Master")
			]),
		new(
			"Sufi Order Template", "sufiorder",
			"A Sufi tariqa or lodge template with visitors, aspirants, initiated members, teaching guides, deputies, hospitality, and charitable work.",
			"Religion",
			[
				TemplateRank("Visitor", "Visitor", 1, "Affiliate"),
				TemplateRank("Aspirant", "Aspirant", 2, "Path"),
				TemplateRank("Initiated Member", "Initiate", 3, "Path", TemplatePrivilegeTier.Staff),
				TemplateRank("Guide", "Guide", 4, "Teaching", TemplatePrivilegeTier.Command),
				TemplateRank("Senior Guide", "Senior", 5, "Teaching", TemplatePrivilegeTier.Command)
			],
			[
				TemplateAppointment("Shaykh", "Shaykh", TemplatePrivilegeTier.All, "Senior Guide"),
				TemplateAppointment("Deputy", "Deputy", TemplatePrivilegeTier.Command, "Guide", "Shaykh"),
				TemplateAppointment("Teaching Guide", "Teacher", TemplatePrivilegeTier.Command, "Guide", "Shaykh", 0),
				TemplateAppointment("Lodge Steward", "Steward", TemplatePrivilegeTier.Staff, "Initiated Member", "Shaykh"),
				TemplateAppointment("Almoner", "Alms", TemplatePrivilegeTier.Staff, "Initiated Member", "Shaykh")
			]),
		new(
			"Hindu Temple Template", "hindutemple",
			"A broad Hindu temple institution with devotees, temple service, ritual priesthood, administration, music, festivals, and treasury responsibilities.",
			"Religion",
			[
				TemplateRank("Devotee", "Dev", 1, "Congregant"),
				TemplateRank("Temple Servant", "Serv", 2, "Temple"),
				TemplateRank("Junior Priest", "JrPriest", 3, "Priesthood", TemplatePrivilegeTier.Staff),
				TemplateRank("Priest", "Priest", 4, "Priesthood", TemplatePrivilegeTier.Command),
				TemplateRank("Chief Priest", "Chief", 5, "Leadership", TemplatePrivilegeTier.Command)
			],
			[
				TemplateAppointment("Chief Priest", "Chief", TemplatePrivilegeTier.All, "Chief Priest"),
				TemplateAppointment("Ritual Priest", "Ritual", TemplatePrivilegeTier.Command, "Priest", "Chief Priest", 0),
				TemplateAppointment("Temple Manager", "Manager", TemplatePrivilegeTier.Command, "Priest", "Chief Priest"),
				TemplateAppointment("Temple Musician", "Music", TemplatePrivilegeTier.Member, "Temple Servant", "Chief Priest", 0),
				TemplateAppointment("Treasurer", "Treas", TemplatePrivilegeTier.Command, "Priest", "Temple Manager"),
				TemplateAppointment("Festival Coordinator", "Festival", TemplatePrivilegeTier.Staff, "Junior Priest", "Temple Manager")
			]),
		new(
			"Merchant Guild Template", "merchantguild",
			"A merchant association with apprenticeship, recognised trading status, guild regulation, finance, quality control, and agents in distant markets.",
			"Commerce",
			[
				TemplateRank("Apprentice", "Appr", 1, "Trade"),
				TemplateRank("Journeyman", "Journey", 2, "Trade"),
				TemplateRank("Freeman Merchant", "Merchant", 3, "Trade", TemplatePrivilegeTier.Staff),
				TemplateRank("Master Merchant", "Master", 4, "Trade", TemplatePrivilegeTier.Command),
				TemplateRank("Guild Elder", "Elder", 5, "Council", TemplatePrivilegeTier.Command)
			],
			[
				TemplateAppointment("Guildmaster", "GM", TemplatePrivilegeTier.All, "Guild Elder"),
				TemplateAppointment("Guild Warden", "Warden", TemplatePrivilegeTier.Command, "Master Merchant", "Guildmaster", 0),
				TemplateAppointment("Treasurer", "Treas", TemplatePrivilegeTier.Command, "Master Merchant", "Guildmaster"),
				TemplateAppointment("Assayer", "Assay", TemplatePrivilegeTier.Staff, "Freeman Merchant", "Guildmaster", 0),
				TemplateAppointment("Factor", "Factor", TemplatePrivilegeTier.Staff, "Freeman Merchant", "Guildmaster", 0),
				TemplateAppointment("Guild Clerk", "Clerk", TemplatePrivilegeTier.Staff, "Journeyman", "Guildmaster")
			]),
		new(
			"Craft Guild Template", "craftguild",
			"A craft fraternity with apprentices, journeymen, masters, examinations, workshop oversight, standards enforcement, and shared funds.",
			"Commerce",
			[
				TemplateRank("Apprentice", "Appr", 1, "Craft"),
				TemplateRank("Journeyman", "Journey", 2, "Craft"),
				TemplateRank("Master", "Master", 3, "Craft", TemplatePrivilegeTier.Staff),
				TemplateRank("Guild Elder", "Elder", 4, "Council", TemplatePrivilegeTier.Command)
			],
			[
				TemplateAppointment("Guildmaster", "GM", TemplatePrivilegeTier.All, "Guild Elder"),
				TemplateAppointment("Guild Warden", "Warden", TemplatePrivilegeTier.Command, "Master", "Guildmaster", 0),
				TemplateAppointment("Masterwork Examiner", "Examiner", TemplatePrivilegeTier.Staff, "Master", "Guildmaster", 0),
				TemplateAppointment("Treasurer", "Treas", TemplatePrivilegeTier.Command, "Master", "Guildmaster"),
				TemplateAppointment("Workshop Steward", "Workshop", TemplatePrivilegeTier.Staff, "Journeyman", "Guildmaster", 0)
			]),
		new(
			"Pirate Crew Template", "piratecrew",
			"A pirate or privateer crew template with ordinary hands, officers, an overall captain, and the historically important independent quartermaster role.",
			"Maritime",
			[
				TemplateRank("New Hand", "New", 1, "Crew"),
				TemplateRank("Able Sailor", "Able", 2, "Crew"),
				TemplateRank("Petty Officer", "PO", 3, "Crew", TemplatePrivilegeTier.Staff),
				TemplateRank("Ship Officer", "Officer", 4, "Officer", TemplatePrivilegeTier.Command),
				TemplateRank("Captain", "Captain", 5, "Officer", TemplatePrivilegeTier.Command)
			],
			[
				TemplateAppointment("Captain", "Capt", TemplatePrivilegeTier.All, "Captain"),
				TemplateAppointment("Quartermaster", "QM", TemplatePrivilegeTier.Command, "Ship Officer", "Captain"),
				TemplateAppointment("Sailing Master", "Master", TemplatePrivilegeTier.Command, "Ship Officer", "Captain"),
				TemplateAppointment("Boatswain", "Bosun", TemplatePrivilegeTier.Staff, "Petty Officer", "Captain"),
				TemplateAppointment("Gunner", "Gunner", TemplatePrivilegeTier.Staff, "Petty Officer", "Captain"),
				TemplateAppointment("Surgeon", "Surgeon", TemplatePrivilegeTier.Staff, "Able Sailor", "Captain")
			]),
		new(
			"University Template", "university",
			"A modern or historical university template with students, academics, faculties, departments, registry, and institutional finance.",
			"Education",
			[
				TemplateRank("Student", "Student", 1, "Student"),
				TemplateRank("Graduate", "Grad", 2, "Academic"),
				TemplateRank("Lecturer", "Lect", 3, "Academic", TemplatePrivilegeTier.Staff),
				TemplateRank("Professor", "Prof", 4, "Academic", TemplatePrivilegeTier.Command),
				TemplateRank("Distinguished Professor", "DistProf", 5, "Academic", TemplatePrivilegeTier.Command)
			],
			[
				TemplateAppointment("Chancellor", "Chancellor", TemplatePrivilegeTier.All, "Distinguished Professor"),
				TemplateAppointment("Vice-Chancellor", "VC", TemplatePrivilegeTier.Command, "Professor", "Chancellor"),
				TemplateAppointment("Dean", "Dean", TemplatePrivilegeTier.Command, "Professor", "Vice-Chancellor", 0),
				TemplateAppointment("Department Head", "Head", TemplatePrivilegeTier.Command, "Professor", "Dean", 0),
				TemplateAppointment("Registrar", "Registrar", TemplatePrivilegeTier.Staff, "Lecturer", "Vice-Chancellor"),
				TemplateAppointment("Bursar", "Bursar", TemplatePrivilegeTier.Command, "Lecturer", "Vice-Chancellor")
			]),
		new(
			"Hospital Template", "hospital",
			"A hospital organisation template separating trainees, support staff, clinical seniority, departments, nursing, operations, pharmacy, and executive leadership.",
			"Health",
			[
				TemplateRank("Trainee", "Trainee", 1, "Clinical"),
				TemplateRank("Support Staff", "Support", 2, "Operations"),
				TemplateRank("Clinician", "Clinician", 3, "Clinical", TemplatePrivilegeTier.Staff),
				TemplateRank("Senior Clinician", "Senior", 4, "Clinical", TemplatePrivilegeTier.Command),
				TemplateRank("Hospital Executive", "Exec", 5, "Executive", TemplatePrivilegeTier.Command)
			],
			[
				TemplateAppointment("Hospital Director", "Director", TemplatePrivilegeTier.All, "Hospital Executive"),
				TemplateAppointment("Medical Director", "Medical", TemplatePrivilegeTier.Command, "Senior Clinician", "Hospital Director"),
				TemplateAppointment("Nursing Director", "Nursing", TemplatePrivilegeTier.Command, "Senior Clinician", "Hospital Director"),
				TemplateAppointment("Operations Manager", "Operations", TemplatePrivilegeTier.Command, "Support Staff", "Hospital Director"),
				TemplateAppointment("Department Head", "Dept", TemplatePrivilegeTier.Command, "Senior Clinician", "Medical Director", 0),
				TemplateAppointment("Chief Pharmacist", "Pharmacy", TemplatePrivilegeTier.Staff, "Clinician", "Medical Director")
			]),
		new(
			"Fire and Rescue Service Template", "firerescue",
			"A modern fire and rescue organisation with operational ranks, stations, training, prevention, logistics, and service-wide command.",
			"Emergency Services",
			[
				TemplateRank("Recruit", "Rec", 1, "Operations"),
				TemplateRank("Firefighter", "FF", 2, "Operations"),
				TemplateRank("Crew Leader", "Crew", 3, "Operations", TemplatePrivilegeTier.Staff),
				TemplateRank("Station Officer", "Station", 4, "Officer", TemplatePrivilegeTier.Command),
				TemplateRank("Chief Officer", "Chief", 5, "Officer", TemplatePrivilegeTier.Command)
			],
			[
				TemplateAppointment("Fire Chief", "Chief", TemplatePrivilegeTier.All, "Chief Officer"),
				TemplateAppointment("Deputy Chief", "Deputy", TemplatePrivilegeTier.Command, "Chief Officer", "Fire Chief"),
				TemplateAppointment("Station Commander", "Station", TemplatePrivilegeTier.Command, "Station Officer", "Deputy Chief", 0),
				TemplateAppointment("Training Officer", "Training", TemplatePrivilegeTier.Staff, "Station Officer", "Deputy Chief"),
				TemplateAppointment("Fire Marshal", "Marshal", TemplatePrivilegeTier.Command, "Station Officer", "Fire Chief"),
				TemplateAppointment("Logistics Officer", "Logistics", TemplatePrivilegeTier.Staff, "Crew Leader", "Deputy Chief")
			]),
		new(
			"Intelligence Agency Template", "intelagency",
			"A modern intelligence service template with analysts and field personnel under operations, assessment, counter-intelligence, technical, and independent oversight offices.",
			"Intelligence",
			[
				TemplateRank("Trainee", "Trainee", 1, "Service"),
				TemplateRank("Officer", "Officer", 2, "Operations"),
				TemplateRank("Senior Officer", "Senior", 3, "Operations", TemplatePrivilegeTier.Staff),
				TemplateRank("Section Chief", "Chief", 4, "Management", TemplatePrivilegeTier.Command),
				TemplateRank("Director", "Director", 5, "Executive", TemplatePrivilegeTier.Command)
			],
			[
				TemplateAppointment("Director-General", "DG", TemplatePrivilegeTier.All, "Director"),
				TemplateAppointment("Deputy Director", "DD", TemplatePrivilegeTier.Command, "Director", "Director-General"),
				TemplateAppointment("Operations Director", "Ops", TemplatePrivilegeTier.Command, "Section Chief", "Deputy Director"),
				TemplateAppointment("Analysis Director", "Analysis", TemplatePrivilegeTier.Command, "Section Chief", "Deputy Director"),
				TemplateAppointment("Counter-Intelligence Director", "CI", TemplatePrivilegeTier.Command, "Section Chief", "Deputy Director"),
				TemplateAppointment("Technical Director", "Tech", TemplatePrivilegeTier.Command, "Section Chief", "Deputy Director"),
				TemplateAppointment("Inspector-General", "IG", TemplatePrivilegeTier.Staff, "Section Chief", "Director-General")
			]),
		new(
			"Political Party Template", "politicalparty",
			"A political party template with supporters, members, organisers, regional branches, policy work, treasury, and parliamentary or internal leadership.",
			"Politics",
			[
				TemplateRank("Supporter", "Support", 1, "Affiliate"),
				TemplateRank("Party Member", "Member", 2, "Membership"),
				TemplateRank("Organiser", "Org", 3, "Organisation", TemplatePrivilegeTier.Staff),
				TemplateRank("Executive Member", "Exec", 4, "Executive", TemplatePrivilegeTier.Command),
				TemplateRank("Party Leader", "Leader", 5, "Leadership", TemplatePrivilegeTier.Command)
			],
			[
				TemplateAppointment("Party Leader", "Leader", TemplatePrivilegeTier.All, "Party Leader"),
				TemplateAppointment("Deputy Leader", "Deputy", TemplatePrivilegeTier.Command, "Executive Member", "Party Leader"),
				TemplateAppointment("General Secretary", "Secretary", TemplatePrivilegeTier.Command, "Executive Member", "Party Leader"),
				TemplateAppointment("Treasurer", "Treas", TemplatePrivilegeTier.Command, "Executive Member", "General Secretary"),
				TemplateAppointment("Party Whip", "Whip", TemplatePrivilegeTier.Staff, "Organiser", "Party Leader"),
				TemplateAppointment("Policy Chair", "Policy", TemplatePrivilegeTier.Staff, "Organiser", "Party Leader"),
				TemplateAppointment("Regional Chair", "Regional", TemplatePrivilegeTier.Command, "Organiser", "General Secretary", 0)
			]),
		new(
			"Labour Union Template", "labourunion",
			"A labour union template with ordinary members, workplace stewards, organisers, elected-style offices, bargaining, safety, and union funds.",
			"Labour",
			[
				TemplateRank("Union Member", "Member", 1, "Membership"),
				TemplateRank("Shop Steward", "Steward", 2, "Workplace", TemplatePrivilegeTier.Staff),
				TemplateRank("Organiser", "Organiser", 3, "Organisation", TemplatePrivilegeTier.Staff),
				TemplateRank("Union Officer", "Officer", 4, "Executive", TemplatePrivilegeTier.Command),
				TemplateRank("Union Executive", "Exec", 5, "Executive", TemplatePrivilegeTier.Command)
			],
			[
				TemplateAppointment("President", "President", TemplatePrivilegeTier.All, "Union Executive"),
				TemplateAppointment("General Secretary", "Secretary", TemplatePrivilegeTier.Command, "Union Officer", "President"),
				TemplateAppointment("Treasurer", "Treas", TemplatePrivilegeTier.Command, "Union Officer", "President"),
				TemplateAppointment("Lead Organiser", "LeadOrg", TemplatePrivilegeTier.Command, "Organiser", "General Secretary"),
				TemplateAppointment("Lead Negotiator", "Negotiate", TemplatePrivilegeTier.Command, "Union Officer", "President"),
				TemplateAppointment("Safety Representative", "Safety", TemplatePrivilegeTier.Staff, "Shop Steward", "General Secretary", 0)
			]),
		new(
			"Non-Governmental Organisation Template", "ngo",
			"A humanitarian, advocacy, or development organisation template with volunteers, programmes, field teams, finance, safeguarding, and logistics.",
			"Civil Society",
			[
				TemplateRank("Volunteer", "Volunteer", 1, "Volunteer"),
				TemplateRank("Member", "Member", 2, "Organisation"),
				TemplateRank("Coordinator", "Coord", 3, "Programme", TemplatePrivilegeTier.Staff),
				TemplateRank("Manager", "Manager", 4, "Management", TemplatePrivilegeTier.Command),
				TemplateRank("Director", "Director", 5, "Executive", TemplatePrivilegeTier.Command)
			],
			[
				TemplateAppointment("Executive Director", "ED", TemplatePrivilegeTier.All, "Director"),
				TemplateAppointment("Programmes Director", "Programmes", TemplatePrivilegeTier.Command, "Director", "Executive Director"),
				TemplateAppointment("Field Coordinator", "Field", TemplatePrivilegeTier.Staff, "Coordinator", "Programmes Director", 0),
				TemplateAppointment("Finance Director", "Finance", TemplatePrivilegeTier.Command, "Manager", "Executive Director"),
				TemplateAppointment("Safeguarding Officer", "Safeguard", TemplatePrivilegeTier.Staff, "Coordinator", "Executive Director"),
				TemplateAppointment("Logistics Manager", "Logistics", TemplatePrivilegeTier.Command, "Manager", "Programmes Director")
			]),
		new(
			"Space Colony Administration Template", "spacecolony",
			"A near- or far-future colony administration balancing civic authority with life support, science, security, logistics, and settlement operations.",
			"Science Fiction",
			[
				TemplateRank("Colonist", "Colonist", 1, "Resident"),
				TemplateRank("Specialist", "Specialist", 2, "Technical"),
				TemplateRank("Section Lead", "Lead", 3, "Management", TemplatePrivilegeTier.Staff),
				TemplateRank("Administrator", "Admin", 4, "Administration", TemplatePrivilegeTier.Command),
				TemplateRank("Governor", "Governor", 5, "Executive", TemplatePrivilegeTier.Command)
			],
			[
				TemplateAppointment("Governor", "Governor", TemplatePrivilegeTier.All, "Governor"),
				TemplateAppointment("Deputy Governor", "Deputy", TemplatePrivilegeTier.Command, "Administrator", "Governor"),
				TemplateAppointment("Life Support Director", "LifeSup", TemplatePrivilegeTier.Command, "Section Lead", "Governor"),
				TemplateAppointment("Security Director", "Security", TemplatePrivilegeTier.Command, "Section Lead", "Governor"),
				TemplateAppointment("Science Director", "Science", TemplatePrivilegeTier.Command, "Section Lead", "Governor"),
				TemplateAppointment("Logistics Director", "Logistics", TemplatePrivilegeTier.Command, "Section Lead", "Deputy Governor"),
				TemplateAppointment("Civic Coordinator", "Civic", TemplatePrivilegeTier.Staff, "Specialist", "Deputy Governor")
			]),
		new(
			"Civilian Starship Crew Template", "civilianstarship",
			"A civilian freighter, liner, research ship, or independent starship crew with watches, technical departments, cargo, passengers, and shipboard services.",
			"Science Fiction",
			[
				TemplateRank("Crew Trainee", "Trainee", 1, "Crew"),
				TemplateRank("Rated Crew", "Rated", 2, "Crew"),
				TemplateRank("Watch Officer", "Watch", 3, "Officer", TemplatePrivilegeTier.Staff),
				TemplateRank("Department Chief", "Chief", 4, "Officer", TemplatePrivilegeTier.Command),
				TemplateRank("Ship Master", "Master", 5, "Command", TemplatePrivilegeTier.Command)
			],
			[
				TemplateAppointment("Ship Master", "Master", TemplatePrivilegeTier.All, "Ship Master"),
				TemplateAppointment("First Officer", "XO", TemplatePrivilegeTier.Command, "Department Chief", "Ship Master"),
				TemplateAppointment("Chief Engineer", "Engineer", TemplatePrivilegeTier.Command, "Department Chief", "Ship Master"),
				TemplateAppointment("Navigator", "Navigator", TemplatePrivilegeTier.Staff, "Watch Officer", "First Officer"),
				TemplateAppointment("Purser", "Purser", TemplatePrivilegeTier.Command, "Watch Officer", "First Officer"),
				TemplateAppointment("Medical Officer", "Medical", TemplatePrivilegeTier.Staff, "Watch Officer", "Ship Master"),
				TemplateAppointment("Cargo Master", "Cargo", TemplatePrivilegeTier.Staff, "Watch Officer", "First Officer"),
				TemplateAppointment("Security Chief", "Security", TemplatePrivilegeTier.Command, "Department Chief", "Ship Master")
			]),
		new(
			"Exploration Corps Template", "explorationcorps",
			"A scientific, planetary, archaeological, or deep-space exploration service with expedition teams and central science, operations, logistics, medical, and liaison offices.",
			"Science Fiction",
			[
				TemplateRank("Candidate", "Candidate", 1, "Corps"),
				TemplateRank("Explorer", "Explorer", 2, "Corps"),
				TemplateRank("Specialist", "Specialist", 3, "Technical", TemplatePrivilegeTier.Staff),
				TemplateRank("Team Leader", "Leader", 4, "Command", TemplatePrivilegeTier.Command),
				TemplateRank("Expedition Commander", "Commander", 5, "Command", TemplatePrivilegeTier.Command)
			],
			[
				TemplateAppointment("Director-General", "DG", TemplatePrivilegeTier.All, "Expedition Commander"),
				TemplateAppointment("Expedition Commander", "ExpCmd", TemplatePrivilegeTier.Command, "Team Leader", "Director-General", 0),
				TemplateAppointment("Chief Scientist", "Science", TemplatePrivilegeTier.Command, "Specialist", "Director-General"),
				TemplateAppointment("Operations Director", "Operations", TemplatePrivilegeTier.Command, "Team Leader", "Director-General"),
				TemplateAppointment("Logistics Director", "Logistics", TemplatePrivilegeTier.Command, "Team Leader", "Operations Director"),
				TemplateAppointment("Medical Director", "Medical", TemplatePrivilegeTier.Staff, "Specialist", "Director-General"),
				TemplateAppointment("Liaison Officer", "Liaison", TemplatePrivilegeTier.Staff, "Explorer", "Operations Director", 0)
			]),
		new(
			"Resistance Network Template", "resistance",
			"A clandestine resistance or insurgent network template with compartmentalised cells, couriers, logistics, intelligence, and political coordination.",
			"Clandestine",
			[
				TemplateRank("Sympathiser", "Symp", 1, "Affiliate"),
				TemplateRank("Cell Member", "Member", 2, "Cell"),
				TemplateRank("Operative", "Operative", 3, "Operations", TemplatePrivilegeTier.Staff),
				TemplateRank("Cell Leader", "CellLead", 4, "Command", TemplatePrivilegeTier.Command),
				TemplateRank("Network Coordinator", "Coord", 5, "Command", TemplatePrivilegeTier.Command)
			],
			[
				TemplateAppointment("Network Coordinator", "Coordinator", TemplatePrivilegeTier.All, "Network Coordinator"),
				TemplateAppointment("Cell Leader", "Cell", TemplatePrivilegeTier.Command, "Cell Leader", "Network Coordinator", 0),
				TemplateAppointment("Quartermaster", "Quarter", TemplatePrivilegeTier.Command, "Operative", "Network Coordinator"),
				TemplateAppointment("Intelligence Handler", "Intel", TemplatePrivilegeTier.Staff, "Operative", "Network Coordinator", 0),
				TemplateAppointment("Courier Master", "Courier", TemplatePrivilegeTier.Staff, "Operative", "Network Coordinator"),
				TemplateAppointment("Political Liaison", "Liaison", TemplatePrivilegeTier.Staff, "Operative", "Network Coordinator", 0)
			])
	];

	private void SetupAdditionalTemplateClans(FuturemudDatabaseContext context)
	{
		ValidateAdditionalClanTemplateSpecifications();

		foreach (ClanTemplateSpecification specification in AdditionalClanTemplateSpecifications)
		{
			if (context.Clans.Any(x => x.Name == specification.Name))
			{
				continue;
			}

			Clan clan = CreateTemplateClan(context, specification.Name, specification.Alias,
				specification.Description, specification.Sphere);
			Dictionary<string, Rank> ranks = specification.Ranks.ToDictionary(
				x => x.Name,
				x => AddRank(context, clan, x.Name, x.Abbreviation, x.RankNumber,
					PrivilegesForTier(x.PrivilegeTier), x.RankPath),
				StringComparer.OrdinalIgnoreCase);
			Dictionary<string, Appointment> appointments = new(StringComparer.OrdinalIgnoreCase);

			foreach (TemplateAppointmentSpecification appointmentSpecification in specification.Appointments)
			{
				Rank minimumRank = ranks[appointmentSpecification.MinimumRank];
				Appointment? parent = appointmentSpecification.ParentAppointment is null
					? null
					: appointments[appointmentSpecification.ParentAppointment];
				appointments[appointmentSpecification.Name] = AddAppointment(
					context,
					clan,
					appointmentSpecification.Name,
					appointmentSpecification.Abbreviation,
					PrivilegesForTier(appointmentSpecification.PrivilegeTier),
					minimumRank,
					minimumRank,
					parent,
					maximumSimultaneousHolders: appointmentSpecification.MaximumSimultaneousHolders);
			}
		}
	}

	private static long PrivilegesForTier(TemplatePrivilegeTier tier)
	{
		long staffPrivileges = MemberPrivileges() |
			(long)(ClanPrivilegeType.CanInduct |
			       ClanPrivilegeType.CanReportDead |
			       ClanPrivilegeType.CanManageClanJobs |
			       ClanPrivilegeType.CanViewTreasury |
			       ClanPrivilegeType.CanAccessLeasedProperties |
			       ClanPrivilegeType.UseClanProperty);
		long commandPrivileges = staffPrivileges |
			(long)(ClanPrivilegeType.CanPromote |
			       ClanPrivilegeType.CanPromoteToOwnRank |
			       ClanPrivilegeType.CanDemote |
			       ClanPrivilegeType.CanCastout |
			       ClanPrivilegeType.CanAppoint |
			       ClanPrivilegeType.CanDismiss |
			       ClanPrivilegeType.CanIncreasePaygrade |
			       ClanPrivilegeType.CanDecreasePaygrade |
			       ClanPrivilegeType.CanGiveBackpay |
			       ClanPrivilegeType.CanManageClanProperty |
			       ClanPrivilegeType.CanManageBankAccounts |
			       ClanPrivilegeType.CanChangeRankPath);

		return tier switch
		{
			TemplatePrivilegeTier.Member => MemberPrivileges(),
			TemplatePrivilegeTier.Staff => staffPrivileges,
			TemplatePrivilegeTier.Command => commandPrivileges,
			TemplatePrivilegeTier.All => (long)ClanPrivilegeType.All,
			_ => throw new ArgumentOutOfRangeException(nameof(tier), tier, null)
		};
	}

	private static void ValidateAdditionalClanTemplateSpecifications()
	{
		if (AdditionalClanTemplateSpecifications.Select(x => x.Name).Distinct(StringComparer.OrdinalIgnoreCase).Count() !=
		    AdditionalClanTemplateSpecifications.Count)
		{
			throw new InvalidOperationException("Additional clan template names must be unique.");
		}

		if (AdditionalClanTemplateSpecifications.Select(x => x.Alias).Distinct(StringComparer.OrdinalIgnoreCase).Count() !=
		    AdditionalClanTemplateSpecifications.Count)
		{
			throw new InvalidOperationException("Additional clan template aliases must be unique.");
		}

		foreach (ClanTemplateSpecification specification in AdditionalClanTemplateSpecifications)
		{
			HashSet<string> rankNames = specification.Ranks
				.Select(x => x.Name)
				.ToHashSet(StringComparer.OrdinalIgnoreCase);
			if (rankNames.Count != specification.Ranks.Count ||
			    specification.Ranks.Select(x => x.RankNumber).Distinct().Count() != specification.Ranks.Count)
			{
				throw new InvalidOperationException($"Clan template {specification.Name} has duplicate ranks.");
			}

			HashSet<string> appointmentNames = new(StringComparer.OrdinalIgnoreCase);
			foreach (TemplateAppointmentSpecification appointment in specification.Appointments)
			{
				if (!rankNames.Contains(appointment.MinimumRank))
				{
					throw new InvalidOperationException(
						$"Clan template {specification.Name} appointment {appointment.Name} references an unknown rank.");
				}

				if (appointment.ParentAppointment is not null &&
				    !appointmentNames.Contains(appointment.ParentAppointment))
				{
					throw new InvalidOperationException(
						$"Clan template {specification.Name} appointment {appointment.Name} references a parent that has not been declared yet.");
				}

				if (!appointmentNames.Add(appointment.Name))
				{
					throw new InvalidOperationException(
						$"Clan template {specification.Name} has duplicate appointments.");
				}
			}
		}
	}
}
