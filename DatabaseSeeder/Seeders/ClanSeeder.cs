using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Community;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.TimeAndDate.Intervals;

namespace DatabaseSeeder.Seeders;

public class ClanSeeder : IDatabaseSeeder
{
	public IEnumerable<(string Id, string Question,
		Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
		Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions =>
		new List<(string Id, string Question, Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool>
			Filter, Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)>
		{
		};

	public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		context.Database.BeginTransaction();

		if (!context.FutureProgs.Any(x => x.FunctionName == "IsMale"))
		{
			var prog = new FutureProg
			{
				FunctionName = "IsMale",
				Category = "Character",
				Subcategory = "Descriptions",
				FunctionComment = "True if the character is male",
				ReturnType = 4,
				StaticType = 0,
				FunctionText = "return @ch.Gender == ToGender(\"male\")"
			};
			prog.FutureProgsParameters.Add(new FutureProgsParameter
			{
				FutureProg = prog,
				ParameterIndex = 0,
				ParameterType = 8200,
				ParameterName = "ch"
			});
			context.FutureProgs.Add(prog);
			context.SaveChanges();
		}

		if (!context.FutureProgs.Any(x => x.FunctionName == "IsFemale"))
		{
			var prog = new FutureProg
			{
				FunctionName = "IsFemale",
				Category = "Character",
				Subcategory = "Descriptions",
				FunctionComment = "True if the character is female",
				ReturnType = 4,
				StaticType = 0,
				FunctionText = "return @ch.Gender == ToGender(\"female\")"
			};
			prog.FutureProgsParameters.Add(new FutureProgsParameter
			{
				FutureProg = prog,
				ParameterIndex = 0,
				ParameterType = 8200,
				ParameterName = "ch"
			});
			context.FutureProgs.Add(prog);
			context.SaveChanges();
		}

		var count = context.Clans.Count();
		SetupRomanCity(context, questionAnswers);
		SetupRomanMilitary(context, questionAnswers);
		SetupMilitaryClan(context, questionAnswers);
		SetupUKPoliceOrganisation(context, questionAnswers);
		SetupGangClan(context, questionAnswers);
		SetupCouncilClan(context, questionAnswers);
		SetupKnightlyOrder(context, questionAnswers);
		SetupMonasticOrder(context, questionAnswers);
		SetupPeerage(context, questionAnswers);
		SetupFeudalism(context, questionAnswers);
		var difference = context.Clans.Count() - count;
		context.Database.CommitTransaction();
		return $"Created {difference} new clans.";
	}

	public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
	{
		if (!context.Accounts.Any() || !context.Clocks.Any() || !context.Currencies.Any())
			return ShouldSeedResult.PrerequisitesNotMet;

		if (context.Clans.Any())
		{
			if (
				context.Clans.All(x => x.Name != "Feudalism Template") ||
				context.Clans.All(x => x.Name != "Peerage Template") || 
				context.Clans.All(x => x.Name != "Monastic Order Template") ||
				context.Clans.All(x => x.Name != "Chivalric Order Template") ||
				context.Clans.All(x => x.Name != "Roman Legion Template") ||
				context.Clans.All(x => x.Name != "Council Template") ||
				context.Clans.All(x => x.Name != "Gang Template") ||
				context.Clans.All(x => x.Name != "UK Police Template") ||
				context.Clans.All(x => x.Name != "Army Template") ||
				context.Clans.All(x => x.Name != "Roman City Template")
			)
			{
				return ShouldSeedResult.ExtraPackagesAvailable;
			}
			return ShouldSeedResult.MayAlreadyBeInstalled;
		}

		return ShouldSeedResult.ReadyToInstall;
	}

	public int SortOrder => 50;
	public string Name => "Clans";
	public string Tagline => "Set up a few common clan templates";

	public string FullDescription =>
		@"This seeder will run you through selecting a few clan templates that you might like to use either directly or as inspiration for how to set up your own clans.

Even if you don't choose to use these clans directly as templates, they can be useful in showing you what is possible and how you can potentially set up your own clans; you can always disable players from choosing them as templates or delete them later.

The core seeder, time seeder and currency seeder are all prerequisites for this package.

This package can be run multiple times as I add more options.";

	private void SetupFeudalism(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> questionAnswers)
	{
		if (context.Clans.Any(x => x.Name == "Feudalism Template"))
		{
			return;
		}

		var clan = new Clan
		{
			Name = "Feudalism Template",
			Alias = "fuedalism",
			FullName = "Feudalism Template",
			Description = "This is a template for a hierarchy of nobility based on a feudal society",
			PayIntervalType = (int)IntervalType.Monthly,
			PayIntervalModifier = 1,
			PayIntervalOther = 0,
			CalendarId = context.Calendars.First().Id,
			PayIntervalReferenceDate = context.Calendars.First().Date,
			PayIntervalReferenceTime = $"{context.Timezones.First(x => x.Clock.PrimaryTimezoneId == x.Id).Name} 0:0:0",
			IsTemplate = true,
			ShowClanMembersInWho = false
		};
		context.Clans.Add(clan);
		context.SaveChanges();
		var isFemaleProg = context.FutureProgs.First(x => x.FunctionName == "IsFemale");
		var rankNumber = 0;
		var memberPrivs = (long)(ClanPrivilegeType.CanViewClanStructure |
								 ClanPrivilegeType.CanViewClanOfficeHolders | ClanPrivilegeType.CanViewMembers);
		var retainerPrivs = (long)(ClanPrivilegeType.CanViewClanStructure |
								   ClanPrivilegeType.CanViewClanOfficeHolders | ClanPrivilegeType.CanViewMembers |
								   ClanPrivilegeType.CanAccessLeasedProperties | ClanPrivilegeType.CanReportDead
			);

		var rank = new Rank
		{
			Clan = clan,
			Name = "Slave",
			RankNumber = rankNumber++,
			Privileges = memberPrivs,
			RankPath = "Commoner"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "Slave" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Slave" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Serf",
			RankNumber = rankNumber++,
			Privileges = memberPrivs,
			RankPath = "Commoner"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "Serf" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Serf" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Free Peasant",
			RankNumber = rankNumber++,
			Privileges = memberPrivs,
			RankPath = "Commoner"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Freewoman", FutureProg = isFemaleProg });
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "Freeman" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Freewoman", FutureProg = isFemaleProg });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Freeman" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Noble",
			RankNumber = rankNumber++,
			Privileges = retainerPrivs,
			RankPath = "Nobility"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "No" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Noble" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		var nobleRank = rank;

		rank = new Rank
		{
			Clan = clan,
			Name = "Page",
			RankNumber = rankNumber++,
			Privileges = retainerPrivs,
			RankPath = "Knightly"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Page" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Page" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Squire",
			RankNumber = rankNumber++,
			Privileges = retainerPrivs,
			RankPath = "Knightly"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Hmd", FutureProg = isFemaleProg });
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "Sq" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Handmaiden", FutureProg = isFemaleProg });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Squire" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Esquire",
			RankNumber = rankNumber++,
			Privileges = retainerPrivs,
			RankPath = "Knightly"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Dms", FutureProg = isFemaleProg });
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "Esq" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Damsel", FutureProg = isFemaleProg });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Esquire" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		var esquireRank = rank;

		var knightPrivs = (long)(ClanPrivilegeType.CanViewClanStructure |
								 ClanPrivilegeType.CanViewClanOfficeHolders | ClanPrivilegeType.CanViewMembers |
								 ClanPrivilegeType.CanAccessLeasedProperties |
								 ClanPrivilegeType.CanInduct | ClanPrivilegeType.CanChangeRankPath |
								 ClanPrivilegeType.CanPromote | ClanPrivilegeType.CanReportDead | 
								 ClanPrivilegeType.CanPromoteToOwnRank
								 );

		rank = new Rank
		{
			Clan = clan,
			Name = "Knight",
			RankNumber = rankNumber++,
			Privileges = knightPrivs,
			RankPath = "Knightly"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Dm", FutureProg = isFemaleProg });
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "Kn" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Dame", FutureProg = isFemaleProg });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Knight" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		var higherknightPrivs = (long)(ClanPrivilegeType.CanViewClanStructure |
								 ClanPrivilegeType.CanViewClanOfficeHolders | ClanPrivilegeType.CanViewMembers |
								 ClanPrivilegeType.CanAccessLeasedProperties |
								 ClanPrivilegeType.CanInduct | ClanPrivilegeType.CanChangeRankPath |
								 ClanPrivilegeType.CanPromote | ClanPrivilegeType.CanReportDead
								 );

		rank = new Rank
		{
			Clan = clan,
			Name = "Vassal Knight",
			RankNumber = rankNumber++,
			Privileges = higherknightPrivs,
			RankPath = "Knightly"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations{ Rank = rank, Order = 0, Abbreviation = "VDm", FutureProg = isFemaleProg });
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "VKn" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Vassal Dame", FutureProg = isFemaleProg });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Vassal Knight" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Knight Banneret",
			RankNumber = rankNumber++,
			Privileges = higherknightPrivs,
			RankPath = "Knightly"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Dm B", FutureProg = isFemaleProg });
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "Kn B" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Dame Banneret", FutureProg = isFemaleProg });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Knight Banneret" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		var nobilityPrivs = (long)(ClanPrivilegeType.CanViewClanStructure |
								 ClanPrivilegeType.CanViewClanOfficeHolders | ClanPrivilegeType.CanViewMembers |
								 ClanPrivilegeType.CanAccessLeasedProperties |
								 ClanPrivilegeType.CanInduct | ClanPrivilegeType.CanChangeRankPath |
								 ClanPrivilegeType.CanPromote | ClanPrivilegeType.CanReportDead |
								 ClanPrivilegeType.CanCreateAppointmentsUnderOwn | ClanPrivilegeType.CanDismiss |
								 ClanPrivilegeType.CanAppoint | ClanPrivilegeType.CanDemote
								 );

		rank = new Rank
		{
			Clan = clan,
			Name = "Baron",
			RankNumber = rankNumber++,
			Privileges = nobilityPrivs,
			RankPath = "Nobility"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Bnss", FutureProg = isFemaleProg });
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "Bn" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Baroness", FutureProg = isFemaleProg });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Baron" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		var baronRank = rank;

		rank = new Rank
		{
			Clan = clan,
			Name = "Duke",
			RankNumber = rankNumber++,
			Privileges = nobilityPrivs,
			RankPath = "Nobility"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations
		{ Rank = rank, Order = 0, Abbreviation = "Duxss", FutureProg = isFemaleProg });
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "Dux" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Duchess", FutureProg = isFemaleProg });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Duke" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Sovereign",
			RankNumber = rankNumber++,
			Privileges = (long)ClanPrivilegeType.All,
			RankPath = "Sovereign",
			FameType = (int)ClanFameType.NameOnly
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations
		{ Rank = rank, Order = 0, Abbreviation = "Qu", FutureProg = isFemaleProg });
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "Ki" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Queen", FutureProg = isFemaleProg });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "King" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		var sovereignRank = rank;

		var appointment = new Appointment
		{
			Clan = clan,
			Name = "Consort",
			MaximumSimultaneousHolders = 1,
			MinimumRank = nobleRank,
			MinimumRankToAppoint = sovereignRank,
			IsAppointedByElection = false,
			FameType = (int)ClanFameType.NameOnly
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
		{ Appointment = appointment, Order = 0, Abbreviation = "Qu C", FutureProg = isFemaleProg });
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
		{ Appointment = appointment, Order = 1, Abbreviation = "Ki C" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
		{ Appointment = appointment, Order = 0, Title = "Queen Consort", FutureProg = isFemaleProg });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
		{ Appointment = appointment, Order = 1, Title = "Prince Consort" });
		context.Appointments.Add(appointment);
		context.SaveChanges();

		appointment = new Appointment
		{
			Clan = clan,
			Name = "Prince",
			MaximumSimultaneousHolders = 0,
			MinimumRank = nobleRank,
			MinimumRankToAppoint = sovereignRank,
			IsAppointedByElection = false,
			FameType = (int)ClanFameType.NameOnly
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
		{ Appointment = appointment, Order = 0, Abbreviation = "Prss", FutureProg = isFemaleProg });
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
		{ Appointment = appointment, Order = 1, Abbreviation = "Pr" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
		{ Appointment = appointment, Order = 0, Title = "Princess", FutureProg = isFemaleProg });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
		{ Appointment = appointment, Order = 1, Title = "Prince" });
		context.Appointments.Add(appointment);
		context.SaveChanges();

		appointment = new Appointment
		{
			Clan = clan,
			Name = "Admiral",
			MaximumSimultaneousHolders = 1,
			MinimumRank = esquireRank,
			MinimumRankToAppoint = sovereignRank,
			IsAppointedByElection = false,
			FameType = (int)ClanFameType.NameOnly
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
		{ Appointment = appointment, Order = 1, Abbreviation = "Adm" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
		{ Appointment = appointment, Order = 1, Title = "Admiral" });
		context.Appointments.Add(appointment);
		context.SaveChanges();

		appointment = new Appointment
		{
			Clan = clan,
			Name = "Chancellor",
			MaximumSimultaneousHolders = 1,
			MinimumRank = esquireRank,
			MinimumRankToAppoint = sovereignRank,
			IsAppointedByElection = false,
			FameType = (int)ClanFameType.NameOnly
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
		{ Appointment = appointment, Order = 1, Abbreviation = "Chn" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
		{ Appointment = appointment, Order = 1, Title = "Chancellor" });
		context.Appointments.Add(appointment);
		context.SaveChanges();

		appointment = new Appointment
		{
			Clan = clan,
			Name = "Sheriff",
			MaximumSimultaneousHolders = 0,
			MinimumRank = esquireRank,
			MinimumRankToAppoint = sovereignRank,
			IsAppointedByElection = false,
			FameType = (int)ClanFameType.NameOnly
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
		{ Appointment = appointment, Order = 1, Abbreviation = "Shf" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
		{ Appointment = appointment, Order = 1, Title = "Sheriff" });
		context.Appointments.Add(appointment);
		context.SaveChanges();

		appointment = new Appointment
		{
			Clan = clan,
			Name = "Castellan",
			MaximumSimultaneousHolders = 0,
			MinimumRank = esquireRank,
			MinimumRankToAppoint = baronRank,
			IsAppointedByElection = false,
			FameType = (int)ClanFameType.NameOnly
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
		{ Appointment = appointment, Order = 1, Abbreviation = "Cst" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
		{ Appointment = appointment, Order = 1, Title = "Castellan" });
		context.Appointments.Add(appointment);
		context.SaveChanges();

		appointment = new Appointment
		{
			Clan = clan,
			Name = "Chamberlain",
			MaximumSimultaneousHolders = 0,
			MinimumRank = esquireRank,
			MinimumRankToAppoint = baronRank,
			IsAppointedByElection = false,
			FameType = (int)ClanFameType.NameOnly
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
		{ Appointment = appointment, Order = 1, Abbreviation = "Chm" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
		{ Appointment = appointment, Order = 1, Title = "Chamberlain" });
		context.Appointments.Add(appointment);
		context.SaveChanges();

		appointment = new Appointment
		{
			Clan = clan,
			Name = "Forester",
			MaximumSimultaneousHolders = 0,
			MinimumRank = esquireRank,
			MinimumRankToAppoint = baronRank,
			IsAppointedByElection = false,
			FameType = (int)ClanFameType.NameOnly
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
		{ Appointment = appointment, Order = 1, Abbreviation = "Frst" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
		{ Appointment = appointment, Order = 1, Title = "Forester" });
		context.Appointments.Add(appointment);
		context.SaveChanges();

		appointment = new Appointment
		{
			Clan = clan,
			Name = "Butler",
			MaximumSimultaneousHolders = 0,
			MinimumRank = esquireRank,
			MinimumRankToAppoint = baronRank,
			IsAppointedByElection = false,
			FameType = (int)ClanFameType.NameOnly
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
		{ Appointment = appointment, Order = 1, Abbreviation = "Btl" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
		{ Appointment = appointment, Order = 1, Title = "Butler" });
		context.Appointments.Add(appointment);
		context.SaveChanges();

		appointment = new Appointment
		{
			Clan = clan,
			Name = "Justiciar",
			MaximumSimultaneousHolders = 0,
			MinimumRank = esquireRank,
			MinimumRankToAppoint = sovereignRank,
			IsAppointedByElection = false,
			FameType = (int)ClanFameType.NameOnly
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
		{ Appointment = appointment, Order = 1, Abbreviation = "Jst" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
		{ Appointment = appointment, Order = 1, Title = "Justiciar" });
		context.Appointments.Add(appointment);
		context.SaveChanges();

		appointment = new Appointment
		{
			Clan = clan,
			Name = "Marshall",
			MaximumSimultaneousHolders = 0,
			MinimumRank = esquireRank,
			MinimumRankToAppoint = baronRank,
			IsAppointedByElection = false,
			FameType = (int)ClanFameType.NameOnly
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
		{ Appointment = appointment, Order = 1, Abbreviation = "Mrsh" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
		{ Appointment = appointment, Order = 1, Title = "Marshall" });
		context.Appointments.Add(appointment);
		context.SaveChanges();

		appointment = new Appointment
		{
			Clan = clan,
			Name = "Seneschal",
			MaximumSimultaneousHolders = 0,
			MinimumRank = esquireRank,
			MinimumRankToAppoint = baronRank,
			IsAppointedByElection = false,
			FameType = (int)ClanFameType.NameOnly
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
		{ Appointment = appointment, Order = 1, Abbreviation = "Sen" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
		{ Appointment = appointment, Order = 1, Title = "Seneschal" });
		context.Appointments.Add(appointment);
		context.SaveChanges();

		appointment = new Appointment
		{
			Clan = clan,
			Name = "Royal Castellan",
			MaximumSimultaneousHolders = 1,
			MinimumRank = esquireRank,
			MinimumRankToAppoint = sovereignRank,
			IsAppointedByElection = false,
			FameType = (int)ClanFameType.NameOnly
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
		{ Appointment = appointment, Order = 1, Abbreviation = "RCst" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
		{ Appointment = appointment, Order = 1, Title = "Royal Castellan" });
		context.Appointments.Add(appointment);
		context.SaveChanges();

		appointment = new Appointment
		{
			Clan = clan,
			Name = "Royal Chamberlain",
			MaximumSimultaneousHolders = 1,
			MinimumRank = esquireRank,
			MinimumRankToAppoint = sovereignRank,
			IsAppointedByElection = false,
			FameType = (int)ClanFameType.NameOnly
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
		{ Appointment = appointment, Order = 1, Abbreviation = "RChm" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
		{ Appointment = appointment, Order = 1, Title = "Royal Chamberlain" });
		context.Appointments.Add(appointment);
		context.SaveChanges();

		appointment = new Appointment
		{
			Clan = clan,
			Name = "Royal Forester",
			MaximumSimultaneousHolders = 1,
			MinimumRank = esquireRank,
			MinimumRankToAppoint = sovereignRank,
			IsAppointedByElection = false,
			FameType = (int)ClanFameType.NameOnly
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
		{ Appointment = appointment, Order = 1, Abbreviation = "RFrst" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
		{ Appointment = appointment, Order = 1, Title = "Royal Forester" });
		context.Appointments.Add(appointment);
		context.SaveChanges();

		appointment = new Appointment
		{
			Clan = clan,
			Name = "Royal Butler",
			MaximumSimultaneousHolders = 0,
			MinimumRank = esquireRank,
			MinimumRankToAppoint = sovereignRank,
			IsAppointedByElection = false,
			FameType = (int)ClanFameType.NameOnly
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
		{ Appointment = appointment, Order = 1, Abbreviation = "RBut" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
		{ Appointment = appointment, Order = 1, Title = "Royal Butler" });
		context.Appointments.Add(appointment);
		context.SaveChanges();

		appointment = new Appointment
		{
			Clan = clan,
			Name = "Royal Marshall",
			MaximumSimultaneousHolders = 1,
			MinimumRank = esquireRank,
			MinimumRankToAppoint = sovereignRank,
			IsAppointedByElection = false,
			FameType = (int)ClanFameType.NameOnly
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
		{ Appointment = appointment, Order = 1, Abbreviation = "RMrsh" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
		{ Appointment = appointment, Order = 1, Title = "Royal Marshall" });
		context.Appointments.Add(appointment);
		context.SaveChanges();

		appointment = new Appointment
		{
			Clan = clan,
			Name = "Royal Seneschal",
			MaximumSimultaneousHolders = 1,
			MinimumRank = esquireRank,
			MinimumRankToAppoint = sovereignRank,
			IsAppointedByElection = false,
			FameType = (int)ClanFameType.NameOnly
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
		{ Appointment = appointment, Order = 1, Abbreviation = "RSen" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
		{ Appointment = appointment, Order = 1, Title = "Royal Seneschal" });
		context.Appointments.Add(appointment);
		context.SaveChanges();
	}

	private void SetupPeerage(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> questionAnswers)
	{

		if (context.Clans.Any(x => x.Name == "Peerage Template"))
		{
			return;
		}
		var clan = new Clan
		{
			Name = "Peerage Template",
			Alias = "peerage",
			FullName = "Peerage Template",
			Description = "This is a template for a hierarchy of nobility based on UK Peerage",
			PayIntervalType = (int)IntervalType.Monthly,
			PayIntervalModifier = 1,
			PayIntervalOther = 0,
			CalendarId = context.Calendars.First().Id,
			PayIntervalReferenceDate = context.Calendars.First().Date,
			PayIntervalReferenceTime = $"{context.Timezones.First(x => x.Clock.PrimaryTimezoneId == x.Id).Name} 0:0:0",
			IsTemplate = true,
			ShowClanMembersInWho = false
		};
		context.Clans.Add(clan);
		context.SaveChanges();
		var isFemaleProg = context.FutureProgs.First(x => x.FunctionName == "IsFemale");
		var rankNumber = 0;
		var memberPrivs = (long)(ClanPrivilegeType.CanViewClanStructure |
		                         ClanPrivilegeType.CanViewClanOfficeHolders | ClanPrivilegeType.CanViewMembers);

		var rank = new Rank
		{
			Clan = clan,
			Name = "Gentleman",
			RankNumber = rankNumber++,
			Privileges = memberPrivs,
			RankPath = "Gentry"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations
			{ Rank = rank, Order = 0, Abbreviation = "Lady", FutureProg = isFemaleProg });
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "Gentleman" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Lady", FutureProg = isFemaleProg });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Gentleman" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Esquire",
			RankNumber = rankNumber++,
			Privileges = memberPrivs,
			RankPath = "Gentry"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Esq" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Esquire" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Knight",
			RankNumber = rankNumber++,
			Privileges = memberPrivs,
			RankPath = "Gentry"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations
			{ Rank = rank, Order = 0, Abbreviation = "Dame", FutureProg = isFemaleProg });
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "Knight" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Dame", FutureProg = isFemaleProg });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Knight" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Baronet",
			RankNumber = rankNumber++,
			Privileges = memberPrivs,
			RankPath = "Gentry"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations
			{ Rank = rank, Order = 0, Abbreviation = "Btss", FutureProg = isFemaleProg });
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "Bt" });
		rank.RanksTitles.Add(new RanksTitle
			{ Rank = rank, Order = 0, Title = "Baronetess", FutureProg = isFemaleProg });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Baronet" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Baron",
			RankNumber = rankNumber++,
			Privileges = memberPrivs,
			RankPath = "Peer"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations
			{ Rank = rank, Order = 0, Abbreviation = "Bnss", FutureProg = isFemaleProg });
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "Bn" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Baroness", FutureProg = isFemaleProg });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Baron" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Viscount",
			RankNumber = rankNumber++,
			Privileges = memberPrivs,
			RankPath = "Peer"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations
			{ Rank = rank, Order = 0, Abbreviation = "Vcss", FutureProg = isFemaleProg });
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "Vc" });
		rank.RanksTitles.Add(
			new RanksTitle { Rank = rank, Order = 0, Title = "Viscountess", FutureProg = isFemaleProg });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Viscount" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Earl",
			RankNumber = rankNumber++,
			Privileges = memberPrivs,
			RankPath = "Peer"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations
			{ Rank = rank, Order = 0, Abbreviation = "Ctss", FutureProg = isFemaleProg });
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "Ea" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Countess", FutureProg = isFemaleProg });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Earl" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Marquess",
			RankNumber = rankNumber++,
			Privileges = memberPrivs,
			RankPath = "Peer"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations
			{ Rank = rank, Order = 0, Abbreviation = "Mqss", FutureProg = isFemaleProg });
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "Mq" });
		rank.RanksTitles.Add(
			new RanksTitle { Rank = rank, Order = 0, Title = "Marchioness", FutureProg = isFemaleProg });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Marquess" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Duke",
			RankNumber = rankNumber++,
			Privileges = memberPrivs,
			RankPath = "Peer"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations
			{ Rank = rank, Order = 0, Abbreviation = "Duxss", FutureProg = isFemaleProg });
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "Dux" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Duchess", FutureProg = isFemaleProg });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Duke" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Sovereign",
			RankNumber = rankNumber++,
			Privileges = (long)ClanPrivilegeType.All,
			RankPath = "Sovereign",
			FameType = (int)ClanFameType.NameOnly
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations
			{ Rank = rank, Order = 0, Abbreviation = "Qu", FutureProg = isFemaleProg });
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "Ki" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Queen", FutureProg = isFemaleProg });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "King" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		var appointment = new Appointment
		{
			Clan = clan,
			Name = "Consort",
			MaximumSimultaneousHolders = 1,
			MinimumRank = null,
			MinimumRankToAppoint = null,
			IsAppointedByElection = false,
			FameType = (int)ClanFameType.NameOnly
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = appointment, Order = 0, Abbreviation = "Qu C", FutureProg = isFemaleProg });
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = appointment, Order = 1, Abbreviation = "Ki C" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = appointment, Order = 0, Title = "Queen Consort", FutureProg = isFemaleProg });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = appointment, Order = 1, Title = "Prince Consort" });
		context.Appointments.Add(appointment);
		context.SaveChanges();

		appointment = new Appointment
		{
			Clan = clan,
			Name = "Prince",
			MaximumSimultaneousHolders = 0,
			MinimumRank = null,
			MinimumRankToAppoint = null,
			IsAppointedByElection = false,
			FameType = (int)ClanFameType.NameOnly
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = appointment, Order = 0, Abbreviation = "Prss", FutureProg = isFemaleProg });
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = appointment, Order = 1, Abbreviation = "Pr" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = appointment, Order = 0, Title = "Princess", FutureProg = isFemaleProg });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = appointment, Order = 1, Title = "Prince" });
		context.Appointments.Add(appointment);
		context.SaveChanges();
	}

	private void SetupMonasticOrder(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> questionAnswers)
	{

		if (context.Clans.Any(x => x.Name == "Monastic Order Template"))
		{
			return;
		}
		var clan = new Clan
		{
			Name = "Monastic Order Template",
			Alias = "monastic",
			FullName = "Monastic Order Template",
			Description = "This is a template for a monastic order (with monks and/or nuns)",
			PayIntervalType = (int)IntervalType.Monthly,
			PayIntervalModifier = 1,
			PayIntervalOther = 0,
			CalendarId = context.Calendars.First().Id,
			PayIntervalReferenceDate = context.Calendars.First().Date,
			PayIntervalReferenceTime = $"{context.Timezones.First(x => x.Clock.PrimaryTimezoneId == x.Id).Name} 0:0:0",
			IsTemplate = true,
			ShowClanMembersInWho = false
		};
		context.Clans.Add(clan);
		context.SaveChanges();

		var isFemaleProg = context.FutureProgs.First(x => x.FunctionName == "IsFemale");
		var rankNumber = 0;
		var retainerPrivs = (long)(ClanPrivilegeType.CanViewClanStructure |
		                           ClanPrivilegeType.CanViewClanOfficeHolders | ClanPrivilegeType.CanViewMembers |
		                           ClanPrivilegeType.CanAccessLeasedProperties | ClanPrivilegeType.CanReportDead
			);

		var ranks = new Dictionary<string, Rank>(StringComparer.OrdinalIgnoreCase);

		var rank = new Rank
		{
			Clan = clan,
			Name = "Oblate",
			RankNumber = rankNumber++,
			Privileges = retainerPrivs,
			RankPath = "Layperson"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Oblate" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Oblate" });
		context.Ranks.Add(rank);
		ranks.Add(rank.Name, rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Postulant",
			RankNumber = rankNumber++,
			Privileges = retainerPrivs,
			RankPath = "Layperson"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Postulant" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Postulant" });
		context.Ranks.Add(rank);
		ranks.Add(rank.Name, rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Novice",
			RankNumber = rankNumber++,
			Privileges = retainerPrivs,
			RankPath = "Layperson"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Novice" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Novice" });
		context.Ranks.Add(rank);
		ranks.Add(rank.Name, rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Monk",
			RankNumber = rankNumber++,
			Privileges = retainerPrivs,
			RankPath = "Sworn"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations
			{ Rank = rank, Order = 0, Abbreviation = "Nun", FutureProg = isFemaleProg });
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "Monk" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Nun", FutureProg = isFemaleProg });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Monk" });
		context.Ranks.Add(rank);
		ranks.Add(rank.Name, rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Sub-Prior",
			RankNumber = rankNumber++,
			Privileges = retainerPrivs,
			RankPath = "Sworn"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations
			{ Rank = rank, Order = 0, Abbreviation = "Sub-Prioress", FutureProg = isFemaleProg });
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "Sub-Prior" });
		rank.RanksTitles.Add(new RanksTitle
			{ Rank = rank, Order = 0, Title = "Sub-Prioress", FutureProg = isFemaleProg });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Sub-Prior" });
		context.Ranks.Add(rank);
		ranks.Add(rank.Name, rank);
		context.SaveChanges();

		var seniorPrivs = (long)(ClanPrivilegeType.CanViewClanStructure |
		                         ClanPrivilegeType.CanViewClanOfficeHolders | ClanPrivilegeType.CanViewMembers |
		                         ClanPrivilegeType.CanAccessLeasedProperties |
		                         ClanPrivilegeType.CanInduct | ClanPrivilegeType.CanChangeRankPath |
		                         ClanPrivilegeType.CanPromote | ClanPrivilegeType.CanReportDead |
		                         ClanPrivilegeType.CanCreateAppointments | ClanPrivilegeType.CanCreateBudgets |
		                         ClanPrivilegeType.CanCreatePaygrades |
		                         ClanPrivilegeType.CanCastout | ClanPrivilegeType.CanManageClanJobs |
		                         ClanPrivilegeType.CanManageClanProperty |
		                         ClanPrivilegeType.CanManageEconomicZones |
		                         ClanPrivilegeType.CanManageBankAccounts | ClanPrivilegeType.CanIncreasePaygrade |
		                         ClanPrivilegeType.CanDecreasePaygrade | ClanPrivilegeType.CanDemote);


		rank = new Rank
		{
			Clan = clan,
			Name = "Prior",
			RankNumber = rankNumber++,
			Privileges = seniorPrivs,
			RankPath = "Sworn"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations
			{ Rank = rank, Order = 0, Abbreviation = "Prioress", FutureProg = isFemaleProg });
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "Prior" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Prioress", FutureProg = isFemaleProg });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Prior" });
		context.Ranks.Add(rank);
		ranks.Add(rank.Name, rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Abbot",
			RankNumber = rankNumber++,
			Privileges = seniorPrivs,
			RankPath = "Sworn"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations
			{ Rank = rank, Order = 0, Abbreviation = "Abbess", FutureProg = isFemaleProg });
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "Abbot" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Abbess", FutureProg = isFemaleProg });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Abbot" });
		context.Ranks.Add(rank);
		ranks.Add(rank.Name, rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Hegumen",
			RankNumber = rankNumber++,
			Privileges = seniorPrivs,
			RankPath = "Sworn"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations
			{ Rank = rank, Order = 0, Abbreviation = "Hegumenia", FutureProg = isFemaleProg });
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "Hegumen" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Hegumenia", FutureProg = isFemaleProg });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Hegumen" });
		context.Ranks.Add(rank);
		ranks.Add(rank.Name, rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Archimandrite",
			RankNumber = rankNumber++,
			Privileges = (long)ClanPrivilegeType.All,
			RankPath = "Sworn"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations
			{ Rank = rank, Order = 0, Abbreviation = "Archimandrate", FutureProg = isFemaleProg });
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "Archimandrite" });
		rank.RanksTitles.Add(new RanksTitle
			{ Rank = rank, Order = 0, Title = "Archimandrate", FutureProg = isFemaleProg });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Archimandrite" });
		context.Ranks.Add(rank);
		ranks.Add(rank.Name, rank);
		context.SaveChanges();

		var appointment = new Appointment
		{
			Clan = clan,
			Name = "Almoner",
			MaximumSimultaneousHolders = 1,
			MinimumRank = ranks["Sub-Prior"],
			MinimumRankToAppoint = ranks["Abbot"],
			IsAppointedByElection = false,
			FameType = 0,
			Privileges = (long)(ClanPrivilegeType.CanManageBankAccounts | ClanPrivilegeType.CanViewTreasury)
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = appointment, Order = 0, Abbreviation = "Almoness", FutureProg = isFemaleProg });
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = appointment, Order = 1, Abbreviation = "Almoner" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = appointment, Order = 0, Title = "Almoness", FutureProg = isFemaleProg });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = appointment, Order = 1, Title = "Almoner" });
		context.Appointments.Add(appointment);
		context.SaveChanges();

		appointment = new Appointment
		{
			Clan = clan,
			Name = "Cantor",
			MaximumSimultaneousHolders = 1,
			MinimumRank = ranks["Monk"],
			MinimumRankToAppoint = ranks["Prior"],
			IsAppointedByElection = false,
			FameType = 0
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = appointment, Order = 0, Abbreviation = "Cantess", FutureProg = isFemaleProg });
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = appointment, Order = 1, Abbreviation = "Cantor" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = appointment, Order = 0, Title = "Cantess", FutureProg = isFemaleProg });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = appointment, Order = 1, Title = "Cantor" });
		context.Appointments.Add(appointment);
		context.SaveChanges();

		appointment = new Appointment
		{
			Clan = clan,
			Name = "Cellarer",
			MaximumSimultaneousHolders = 1,
			MinimumRank = ranks["Oblate"],
			MinimumRankToAppoint = ranks["Prior"],
			IsAppointedByElection = false,
			FameType = 0
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = appointment, Order = 0, Abbreviation = "Cellaress", FutureProg = isFemaleProg });
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = appointment, Order = 1, Abbreviation = "Cellarer" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = appointment, Order = 0, Title = "Cellaress", FutureProg = isFemaleProg });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = appointment, Order = 1, Title = "Cellarer" });
		context.Appointments.Add(appointment);
		context.SaveChanges();

		appointment = new Appointment
		{
			Clan = clan,
			Name = "Chamberlain",
			MaximumSimultaneousHolders = 1,
			MinimumRank = ranks["Monk"],
			MinimumRankToAppoint = ranks["Prior"],
			IsAppointedByElection = false,
			FameType = 0
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = appointment, Order = 1, Abbreviation = "Chamberlain" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = appointment, Order = 1, Title = "Chamberlain" });
		context.Appointments.Add(appointment);
		context.SaveChanges();

		appointment = new Appointment
		{
			Clan = clan,
			Name = "Circuitor",
			MaximumSimultaneousHolders = 1,
			MinimumRank = ranks["Sub-Prior"],
			MinimumRankToAppoint = ranks["Abbot"],
			IsAppointedByElection = false,
			FameType = 0,
			Privileges = (long)(ClanPrivilegeType.CanChangeRankPath | ClanPrivilegeType.CanDemote |
			                    ClanPrivilegeType.CanCastout | ClanPrivilegeType.CanDecreasePaygrade)
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = appointment, Order = 0, Abbreviation = "Circuitress", FutureProg = isFemaleProg });
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = appointment, Order = 1, Abbreviation = "Circuitor" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = appointment, Order = 0, Title = "Circuitress", FutureProg = isFemaleProg });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = appointment, Order = 1, Title = "Circuitor" });
		context.Appointments.Add(appointment);
		context.SaveChanges();

		appointment = new Appointment
		{
			Clan = clan,
			Name = "Guest Master",
			MaximumSimultaneousHolders = 1,
			MinimumRank = ranks["Sub-Prior"],
			MinimumRankToAppoint = ranks["Abbot"],
			IsAppointedByElection = false,
			FameType = 0
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = appointment, Order = 0, Abbreviation = "Guest Mistress", FutureProg = isFemaleProg });
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = appointment, Order = 1, Abbreviation = "Guest Master" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = appointment, Order = 0, Title = "Guest Mistress", FutureProg = isFemaleProg });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = appointment, Order = 1, Title = "Guest Master" });
		context.Appointments.Add(appointment);
		context.SaveChanges();

		appointment = new Appointment
		{
			Clan = clan,
			Name = "Infirmerer",
			MaximumSimultaneousHolders = 0,
			MinimumRank = ranks["Monk"],
			MinimumRankToAppoint = ranks["Prior"],
			IsAppointedByElection = false,
			FameType = 0
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = appointment, Order = 0, Abbreviation = "Infirmeress", FutureProg = isFemaleProg });
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = appointment, Order = 1, Abbreviation = "Infirmerer" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = appointment, Order = 0, Title = "Infirmeress", FutureProg = isFemaleProg });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = appointment, Order = 1, Title = "Infirmerer" });
		context.Appointments.Add(appointment);
		context.SaveChanges();

		appointment = new Appointment
		{
			Clan = clan,
			Name = "Kitchener",
			MaximumSimultaneousHolders = 0,
			MinimumRank = ranks["Monk"],
			MinimumRankToAppoint = ranks["Prior"],
			IsAppointedByElection = false,
			FameType = 0
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = appointment, Order = 0, Abbreviation = "Kitcheness", FutureProg = isFemaleProg });
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = appointment, Order = 1, Abbreviation = "Kitchener" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = appointment, Order = 0, Title = "Kitcheness", FutureProg = isFemaleProg });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = appointment, Order = 1, Title = "Kitchener" });
		context.Appointments.Add(appointment);
		context.SaveChanges();

		appointment = new Appointment
		{
			Clan = clan,
			Name = "Librarian",
			MaximumSimultaneousHolders = 0,
			MinimumRank = ranks["Monk"],
			MinimumRankToAppoint = ranks["Prior"],
			IsAppointedByElection = false,
			FameType = 0
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = appointment, Order = 0, Abbreviation = "Librarian" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = appointment, Order = 0, Title = "Librarian" });
		context.Appointments.Add(appointment);
		context.SaveChanges();

		appointment = new Appointment
		{
			Clan = clan,
			Name = "Novice Master",
			MaximumSimultaneousHolders = 0,
			MinimumRank = ranks["Sub-Prior"],
			MinimumRankToAppoint = ranks["Abbot"],
			IsAppointedByElection = false,
			FameType = 0,
			Privileges = (long)(ClanPrivilegeType.CanInduct | ClanPrivilegeType.CanPromote |
			                    ClanPrivilegeType.CanChangeRankPath | ClanPrivilegeType.CanDemote |
			                    ClanPrivilegeType.CanCastout)
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = appointment, Order = 0, Abbreviation = "Novice Mistress", FutureProg = isFemaleProg });
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = appointment, Order = 1, Abbreviation = "Novice Master" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = appointment, Order = 0, Title = "Novice Mistress", FutureProg = isFemaleProg });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = appointment, Order = 1, Title = "Novice Master" });
		context.Appointments.Add(appointment);
		context.SaveChanges();

		appointment = new Appointment
		{
			Clan = clan,
			Name = "Treasurer",
			MaximumSimultaneousHolders = 1,
			MinimumRank = ranks["Sub-Prior"],
			MinimumRankToAppoint = ranks["Abbot"],
			IsAppointedByElection = false,
			FameType = 0,
			Privileges = (long)(ClanPrivilegeType.CanManageBankAccounts | ClanPrivilegeType.CanCreateBudgets |
			                    ClanPrivilegeType.CanManageEconomicZones | ClanPrivilegeType.CanCreatePaygrades |
			                    ClanPrivilegeType.CanIncreasePaygrade | ClanPrivilegeType.CanDecreasePaygrade)
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = appointment, Order = 0, Abbreviation = "Treasuress", FutureProg = isFemaleProg });
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = appointment, Order = 1, Abbreviation = "Treasurer" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = appointment, Order = 0, Title = "Treasuress", FutureProg = isFemaleProg });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = appointment, Order = 1, Title = "Treasurer" });
		context.Appointments.Add(appointment);
		context.SaveChanges();

		appointment = new Appointment
		{
			Clan = clan,
			Name = "Sacrist",
			MaximumSimultaneousHolders = 1,
			MinimumRank = ranks["Monk"],
			MinimumRankToAppoint = ranks["Abbot"],
			IsAppointedByElection = false,
			FameType = 0
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = appointment, Order = 0, Abbreviation = "Sacrist" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = appointment, Order = 0, Title = "Sacrist" });
		context.Appointments.Add(appointment);
		context.SaveChanges();
	}

	private void SetupKnightlyOrder(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> questionAnswers)
	{

		if (context.Clans.Any(x => x.Name == "Chivalric Order Template"))
		{
			return;
		}
		var clan = new Clan
		{
			Name = "Chivalric Order Template",
			Alias = "chivalric",
			FullName = "Chivalric Order Template",
			Description = "This is a template for a chivalric order (with Knights and their retainers)",
			PayIntervalType = (int)IntervalType.Monthly,
			PayIntervalModifier = 1,
			PayIntervalOther = 0,
			CalendarId = context.Calendars.First().Id,
			PayIntervalReferenceDate = context.Calendars.First().Date,
			PayIntervalReferenceTime = $"{context.Timezones.First(x => x.Clock.PrimaryTimezoneId == x.Id).Name} 0:0:0",
			IsTemplate = true,
			ShowClanMembersInWho = false
		};
		context.Clans.Add(clan);
		context.SaveChanges();

		var isFemaleProg = context.FutureProgs.First(x => x.FunctionName == "IsFemale");
		var rankNumber = 0;
		var retainerPrivs = (long)(ClanPrivilegeType.CanViewClanStructure |
		                           ClanPrivilegeType.CanViewClanOfficeHolders | ClanPrivilegeType.CanViewMembers |
		                           ClanPrivilegeType.CanAccessLeasedProperties | ClanPrivilegeType.CanReportDead
			);

		var rank = new Rank
		{
			Clan = clan,
			Name = "Servant",
			RankNumber = rankNumber++,
			Privileges = retainerPrivs,
			RankPath = "Knave"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Servant" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Servant" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Armsman",
			RankNumber = rankNumber++,
			Privileges = retainerPrivs,
			RankPath = "Knave"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations
			{ Rank = rank, Order = 0, Abbreviation = "Armswoman", FutureProg = isFemaleProg });
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "Armsman" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Armswoman", FutureProg = isFemaleProg });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Armsman" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Serjeant",
			RankNumber = rankNumber++,
			Privileges = retainerPrivs,
			RankPath = "Knave"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Serjeant" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Serjeant" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Valet",
			RankNumber = rankNumber++,
			Privileges = retainerPrivs,
			RankPath = "Knave"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations
			{ Rank = rank, Order = 0, Abbreviation = "Lady Valet", FutureProg = isFemaleProg });
		rank.RanksAbbreviations.Add(new RanksAbbreviations
			{ Rank = rank, Order = 1, Abbreviation = "Gentleman Valet" });
		rank.RanksTitles.Add(new RanksTitle
			{ Rank = rank, Order = 0, Title = "Lady Valet", FutureProg = isFemaleProg });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Gentleman Valet" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Page",
			RankNumber = rankNumber++,
			Privileges = retainerPrivs,
			RankPath = "Attendant"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Page" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Page" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Squire",
			RankNumber = rankNumber++,
			Privileges = retainerPrivs,
			RankPath = "Attendant"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Squire" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Squire" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		var knightPrivs = (long)(ClanPrivilegeType.CanViewClanStructure |
		                         ClanPrivilegeType.CanViewClanOfficeHolders | ClanPrivilegeType.CanViewMembers |
		                         ClanPrivilegeType.CanAccessLeasedProperties |
		                         ClanPrivilegeType.CanInduct | ClanPrivilegeType.CanChangeRankPath |
		                         ClanPrivilegeType.CanPromote | ClanPrivilegeType.CanReportDead
			);


		rank = new Rank
		{
			Clan = clan,
			Name = "Knight Lieutenant",
			RankNumber = rankNumber++,
			Privileges = knightPrivs,
			RankPath = "Knight"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations
			{ Rank = rank, Order = 0, Abbreviation = "DL", FutureProg = isFemaleProg });
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "KL" });
		rank.RanksTitles.Add(new RanksTitle
			{ Rank = rank, Order = 0, Title = "Dame Lieutenant", FutureProg = isFemaleProg });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Knight Lieutenant" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Knight Commander",
			RankNumber = rankNumber++,
			Privileges = knightPrivs,
			RankPath = "Knight"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations
			{ Rank = rank, Order = 0, Abbreviation = "DC", FutureProg = isFemaleProg });
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "KC" });
		rank.RanksTitles.Add(new RanksTitle
			{ Rank = rank, Order = 0, Title = "Dame Commander", FutureProg = isFemaleProg });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Knight Commander" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		var seniorPrivs = (long)(ClanPrivilegeType.CanViewClanStructure |
		                         ClanPrivilegeType.CanViewClanOfficeHolders | ClanPrivilegeType.CanViewMembers |
		                         ClanPrivilegeType.CanAccessLeasedProperties |
		                         ClanPrivilegeType.CanInduct | ClanPrivilegeType.CanChangeRankPath |
		                         ClanPrivilegeType.CanPromote | ClanPrivilegeType.CanReportDead |
		                         ClanPrivilegeType.CanCreateAppointments | ClanPrivilegeType.CanCreateBudgets |
		                         ClanPrivilegeType.CanCreatePaygrades |
		                         ClanPrivilegeType.CanCastout | ClanPrivilegeType.CanManageClanJobs |
		                         ClanPrivilegeType.CanManageClanProperty |
		                         ClanPrivilegeType.CanManageEconomicZones |
		                         ClanPrivilegeType.CanManageBankAccounts | ClanPrivilegeType.CanIncreasePaygrade |
		                         ClanPrivilegeType.CanDecreasePaygrade | ClanPrivilegeType.CanDemote
			);

		rank = new Rank
		{
			Clan = clan,
			Name = "Knight Grand Commander",
			RankNumber = rankNumber++,
			Privileges = seniorPrivs,
			RankPath = "Knight"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations
			{ Rank = rank, Order = 0, Abbreviation = "DGC", FutureProg = isFemaleProg });
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "KGC" });
		rank.RanksTitles.Add(new RanksTitle
			{ Rank = rank, Order = 0, Title = "Dame Grand Commander", FutureProg = isFemaleProg });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Knight Grand Commander" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Knight Grand Cross",
			RankNumber = rankNumber++,
			Privileges = seniorPrivs,
			RankPath = "Knight"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations
			{ Rank = rank, Order = 0, Abbreviation = "DGX", FutureProg = isFemaleProg });
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "KGX" });
		rank.RanksTitles.Add(new RanksTitle
			{ Rank = rank, Order = 0, Title = "Dame Grand Cross", FutureProg = isFemaleProg });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Knight Grand Cross" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Knight Marshal",
			RankNumber = rankNumber++,
			Privileges = (long)ClanPrivilegeType.All,
			RankPath = "Knight"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations
			{ Rank = rank, Order = 0, Abbreviation = "DGM", FutureProg = isFemaleProg });
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "KGM" });
		rank.RanksTitles.Add(new RanksTitle
			{ Rank = rank, Order = 0, Title = "Dame Marshal", FutureProg = isFemaleProg });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Knight Marshal" });
		context.Ranks.Add(rank);
		context.SaveChanges();
	}

	private void SetupRomanMilitary(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> questionAnswers)
	{

		if (context.Clans.Any(x => x.Name == "Roman Legion Template"))
		{
			return;
		}

		var clan = new Clan
		{
			Name = "Roman Legion Template",
			Alias = "legion",
			FullName = "Roman Legion Template",
			Description = "This is a template for a roman legion style military unit, post Marian reforms.",
			PayIntervalType = (int)IntervalType.Monthly,
			PayIntervalModifier = 1,
			PayIntervalOther = 0,
			CalendarId = context.Calendars.First().Id,
			PayIntervalReferenceDate = context.Calendars.First().Date,
			PayIntervalReferenceTime = $"{context.Timezones.First(x => x.Clock.PrimaryTimezoneId == x.Id).Name} 0:0:0",
			IsTemplate = true,
			ShowClanMembersInWho = false
		};
		context.Clans.Add(clan);
		context.SaveChanges();

		var isFemaleProg = context.FutureProgs.First(x => x.FunctionName == "IsFemale");

		var rankNumber = 0;

		var rank = new Rank
		{
			Clan = clan,
			Name = "Servus",
			RankNumber = rankNumber++,
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers),
			RankPath = "Slave"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Srv" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Serva", FutureProg = isFemaleProg });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Servus" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Servus Principis",
			RankNumber = rankNumber++,
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers),
			RankPath = "Slave"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "SrvPr" });
		rank.RanksTitles.Add(new RanksTitle
			{ Rank = rank, Order = 0, Title = "Serva Principis", FutureProg = isFemaleProg });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Servus Principis" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Pedes",
			RankNumber = rankNumber++,
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers),
			RankPath = "Auxiliary Infantry"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Pds" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Pedes" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Gregalis",
			RankNumber = rankNumber++,
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers),
			RankPath = "Auxiliary Cavalry"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Grgls" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Gregalis" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Legio",
			RankNumber = rankNumber++,
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers),
			RankPath = "Legionary"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Lg" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Legio" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Tesserarius Cohortis",
			RankNumber = rankNumber++,
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers),
			RankPath = "Auxiliary Infantry"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Tsr" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Tesserarius" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Sesquiplicarius",
			RankNumber = rankNumber++,
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers),
			RankPath = "Auxiliary Cavalry"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Ssqplcrs" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Sesquiplicarius" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Tesserarius",
			RankNumber = rankNumber++,
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers),
			RankPath = "Legionary"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Tsr" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Tesserarius" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Signifer Cohortis",
			RankNumber = rankNumber++,
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers),
			RankPath = "Auxiliary Infantry"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Sgnfr" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Signifer" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Signifer Alae",
			RankNumber = rankNumber++,
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers),
			RankPath = "Auxiliary Cavalry"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Sgnfr" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Signifer" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Signifer",
			RankNumber = rankNumber++,
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers),
			RankPath = "Legionary"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Sgnfr" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Signifer" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Optio Cohortis",
			RankNumber = rankNumber++,
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers | ClanPrivilegeType.CanViewTreasury),
			RankPath = "Auxiliary Infantry"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Opt" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Optio" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Curator",
			RankNumber = rankNumber++,
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers | ClanPrivilegeType.CanViewTreasury),
			RankPath = "Auxiliary Cavalry"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Crtr" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Curator" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Optio",
			RankNumber = rankNumber++,
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers | ClanPrivilegeType.CanViewTreasury),
			RankPath = "Legionary"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Opt" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Optio" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Vexillarius Cohortis",
			RankNumber = rankNumber++,
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers),
			RankPath = "Auxiliary Infantry"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Vxlrs" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Vexillarius" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Vexillarius Alae",
			RankNumber = rankNumber++,
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers),
			RankPath = "Auxiliary Infantry"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Vxlrs" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Vexillarius" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Vexillarius",
			RankNumber = rankNumber++,
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers),
			RankPath = "Legionary"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Vxlrs" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Vexillarius" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Centurio Cohortis",
			RankNumber = rankNumber++,
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers | ClanPrivilegeType.CanViewTreasury |
			                    ClanPrivilegeType.CanInduct | ClanPrivilegeType.CanPromote |
			                    ClanPrivilegeType.CanDemote | ClanPrivilegeType.CanGiveBackpay |
			                    ClanPrivilegeType.CanAppoint),
			RankPath = "Auxiliary Infantry"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Cntr" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Centurio" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Decurio",
			RankNumber = rankNumber++,
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers | ClanPrivilegeType.CanViewTreasury |
			                    ClanPrivilegeType.CanInduct | ClanPrivilegeType.CanPromote |
			                    ClanPrivilegeType.CanDemote | ClanPrivilegeType.CanGiveBackpay |
			                    ClanPrivilegeType.CanAppoint),
			RankPath = "Auxiliary Cavalry"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Dcr" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Decurio" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Centurio",
			RankNumber = rankNumber++,
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers | ClanPrivilegeType.CanViewTreasury |
			                    ClanPrivilegeType.CanInduct | ClanPrivilegeType.CanPromote |
			                    ClanPrivilegeType.CanDemote | ClanPrivilegeType.CanGiveBackpay |
			                    ClanPrivilegeType.CanAppoint),
			RankPath = "Legionary"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Cntr" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Centurio" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Centurio Primus Ordinus",
			RankNumber = rankNumber++,
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers | ClanPrivilegeType.CanViewTreasury |
			                    ClanPrivilegeType.CanInduct | ClanPrivilegeType.CanPromote |
			                    ClanPrivilegeType.CanDemote | ClanPrivilegeType.CanGiveBackpay |
			                    ClanPrivilegeType.CanAppoint),
			RankPath = "Legionary"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "CntrPrPl" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Centurio Primus Pilus" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Centurio Primus Pilus",
			RankNumber = rankNumber++,
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers | ClanPrivilegeType.CanViewTreasury |
			                    ClanPrivilegeType.CanInduct | ClanPrivilegeType.CanPromote |
			                    ClanPrivilegeType.CanDemote | ClanPrivilegeType.CanGiveBackpay |
			                    ClanPrivilegeType.CanAppoint),
			RankPath = "Legionary"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "CntrPrPl" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Centurio Primus Pilus" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Centurio Princeps Cohortis",
			RankNumber = rankNumber++,
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers | ClanPrivilegeType.CanViewTreasury |
			                    ClanPrivilegeType.CanInduct | ClanPrivilegeType.CanPromote |
			                    ClanPrivilegeType.CanDemote | ClanPrivilegeType.CanGiveBackpay |
			                    ClanPrivilegeType.CanAppoint),
			RankPath = "Auxiliary Infantry"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "CntrPr" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Centurio Princeps" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Decurio Princeps",
			RankNumber = rankNumber++,
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers | ClanPrivilegeType.CanViewTreasury |
			                    ClanPrivilegeType.CanInduct | ClanPrivilegeType.CanPromote |
			                    ClanPrivilegeType.CanDemote | ClanPrivilegeType.CanGiveBackpay |
			                    ClanPrivilegeType.CanAppoint),
			RankPath = "Auxiliary Cavalry"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "DcrPr" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Decurio Princeps" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Beneficarius Cohortis",
			RankNumber = rankNumber++,
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers | ClanPrivilegeType.CanViewTreasury |
			                    ClanPrivilegeType.CanInduct | ClanPrivilegeType.CanPromote |
			                    ClanPrivilegeType.CanDemote | ClanPrivilegeType.CanGiveBackpay |
			                    ClanPrivilegeType.CanAppoint | ClanPrivilegeType.CanDismiss),
			RankPath = "Auxiliary Infantry"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "CntrCh" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Centurio" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Beneficarius Alae",
			RankNumber = rankNumber++,
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers | ClanPrivilegeType.CanViewTreasury |
			                    ClanPrivilegeType.CanInduct | ClanPrivilegeType.CanPromote |
			                    ClanPrivilegeType.CanDemote | ClanPrivilegeType.CanGiveBackpay |
			                    ClanPrivilegeType.CanAppoint | ClanPrivilegeType.CanDismiss),
			RankPath = "Auxiliary Cavalry"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Cntr" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Centurio" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Praefectus Cohortis",
			RankNumber = rankNumber++,
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers | ClanPrivilegeType.CanViewTreasury |
			                    ClanPrivilegeType.CanInduct | ClanPrivilegeType.CanPromote |
			                    ClanPrivilegeType.CanDemote | ClanPrivilegeType.CanGiveBackpay |
			                    ClanPrivilegeType.CanAppoint | ClanPrivilegeType.CanDismiss),
			RankPath = "Equestrian"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Prfct" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Praefectus" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Tribunus Cohortis",
			RankNumber = rankNumber++,
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers | ClanPrivilegeType.CanViewTreasury |
			                    ClanPrivilegeType.CanInduct | ClanPrivilegeType.CanPromote |
			                    ClanPrivilegeType.CanDemote | ClanPrivilegeType.CanGiveBackpay |
			                    ClanPrivilegeType.CanAppoint | ClanPrivilegeType.CanDismiss),
			RankPath = "Equestrian"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Trbn" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Tribunus" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Praefectus Alae",
			RankNumber = rankNumber++,
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers | ClanPrivilegeType.CanViewTreasury |
			                    ClanPrivilegeType.CanInduct | ClanPrivilegeType.CanPromote |
			                    ClanPrivilegeType.CanDemote | ClanPrivilegeType.CanGiveBackpay |
			                    ClanPrivilegeType.CanAppoint | ClanPrivilegeType.CanDismiss),
			RankPath = "Equestrian"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Prfct" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Praefectus" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Praefectus Castrorum",
			RankNumber = rankNumber++,
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers | ClanPrivilegeType.CanViewTreasury |
			                    ClanPrivilegeType.CanInduct | ClanPrivilegeType.CanPromote |
			                    ClanPrivilegeType.CanDemote | ClanPrivilegeType.CanGiveBackpay |
			                    ClanPrivilegeType.CanAppoint | ClanPrivilegeType.CanDismiss),
			RankPath = "Equestrian"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "PrfctCst" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Praefectus Castrorum" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Tribunus Angusticlavii",
			RankNumber = rankNumber++,
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers | ClanPrivilegeType.CanViewTreasury |
			                    ClanPrivilegeType.CanInduct | ClanPrivilegeType.CanPromote |
			                    ClanPrivilegeType.CanDemote | ClanPrivilegeType.CanGiveBackpay |
			                    ClanPrivilegeType.CanAppoint | ClanPrivilegeType.CanDismiss |
			                    ClanPrivilegeType.CanChangeRankPath | ClanPrivilegeType.CanCreateAppointments),
			RankPath = "Equestrian"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "TrbnsAngst" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Tribunus Angusticlavii" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Tribunus Laticlavius",
			RankNumber = rankNumber++,
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers | ClanPrivilegeType.CanViewTreasury |
			                    ClanPrivilegeType.CanInduct | ClanPrivilegeType.CanPromote |
			                    ClanPrivilegeType.CanDemote | ClanPrivilegeType.CanGiveBackpay |
			                    ClanPrivilegeType.CanAppoint | ClanPrivilegeType.CanDismiss |
			                    ClanPrivilegeType.CanChangeRankPath | ClanPrivilegeType.CanCreateAppointments),
			RankPath = "Equestrian"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "TrbnsLtclvm" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Tribunus Laticlavius" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Legate",
			RankNumber = rankNumber++,
			Privileges = (long)ClanPrivilegeType.All,
			RankPath = "Equestrian"
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Lgt" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Legate" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		var appointment = new Appointment
		{
			Clan = clan,
			Name = "Immunes",
			Privileges = (long)ClanPrivilegeType.None,
			IsAppointedByElection = false,
			IsSecretBallot = false,
			MinimumRankToAppoint = clan.Ranks.First(x => x.Name == "Centurio Cohortis")
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = appointment, Order = 0, Abbreviation = "Imm" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = appointment, Order = 0, Title = "Immunes" });
		context.Appointments.Add(appointment);
		context.SaveChanges();

		var paygrade = new Paygrade
		{
			Name = "Caligatius Cohortis",
			Abbreviation = "ClgCh",
			Currency = context.Currencies.First(),
			PayAmount = 750M,
			Clan = clan
		};
		context.Paygrades.Add(paygrade);
		foreach (var paidrank in clan.Ranks.Where(x =>
			         x.RankPath == "Auxiliary Infantry" &&
			         !x.Name.Contains("Centurio", StringComparison.InvariantCultureIgnoreCase)))
		{
			if (paidrank.Name != "Pedes") continue;
			paidrank.RanksPaygrades.Add(new RanksPaygrade { Rank = paidrank, Paygrade = paygrade, Order = 0 });
		}

		context.SaveChanges();

		paygrade = new Paygrade
		{
			Name = "Sesquiplicarius Cohortis",
			Abbreviation = "SqpCh",
			Currency = context.Currencies.First(),
			PayAmount = 1125M,
			Clan = clan
		};
		context.Paygrades.Add(paygrade);
		foreach (var paidrank in clan.Ranks.Where(x =>
			         x.RankPath == "Auxiliary Infantry" &&
			         !x.Name.Contains("Centurio", StringComparison.InvariantCultureIgnoreCase)))
		{
			if (!paidrank.Name.In("Pedes", "Tessarius Cohortis")) continue;
			paidrank.RanksPaygrades.Add(new RanksPaygrade { Rank = paidrank, Paygrade = paygrade, Order = 1 });
		}

		context.SaveChanges();

		paygrade = new Paygrade
		{
			Name = "Duplicarius Cohortis",
			Abbreviation = "DplcCh",
			Currency = context.Currencies.First(),
			PayAmount = 1500M,
			Clan = clan
		};
		context.Paygrades.Add(paygrade);
		foreach (var paidrank in clan.Ranks.Where(x =>
			         x.RankPath == "Auxiliary Infantry" &&
			         !x.Name.Contains("Centurio", StringComparison.InvariantCultureIgnoreCase)))
			paidrank.RanksPaygrades.Add(new RanksPaygrade { Rank = paidrank, Paygrade = paygrade, Order = 2 });
		context.SaveChanges();

		paygrade = new Paygrade
		{
			Name = "Caligatius Legionis",
			Abbreviation = "ClgLg",
			Currency = context.Currencies.First(),
			PayAmount = 900M,
			Clan = clan
		};
		context.Paygrades.Add(paygrade);
		foreach (var paidrank in clan.Ranks.Where(x =>
			         x.RankPath == "Legionary" &&
			         !x.Name.Contains("Centurio", StringComparison.InvariantCultureIgnoreCase)))
		{
			if (paidrank.Name != "Legio") continue;
			paidrank.RanksPaygrades.Add(new RanksPaygrade { Rank = paidrank, Paygrade = paygrade, Order = 0 });
		}

		context.SaveChanges();

		paygrade = new Paygrade
		{
			Name = "Sesquiplicarius Legionis",
			Abbreviation = "SqpLg",
			Currency = context.Currencies.First(),
			PayAmount = 1350M,
			Clan = clan
		};
		context.Paygrades.Add(paygrade);
		foreach (var paidrank in clan.Ranks.Where(x =>
			         x.RankPath == "Legionary" &&
			         !x.Name.Contains("Centurio", StringComparison.InvariantCultureIgnoreCase)))
		{
			if (!paidrank.Name.In("Legio", "Tessarius")) continue;
			paidrank.RanksPaygrades.Add(new RanksPaygrade { Rank = paidrank, Paygrade = paygrade, Order = 1 });
		}

		context.SaveChanges();

		paygrade = new Paygrade
		{
			Name = "Duplicarius Legionis",
			Abbreviation = "DplcLg",
			Currency = context.Currencies.First(),
			PayAmount = 1800M,
			Clan = clan
		};
		context.Paygrades.Add(paygrade);
		foreach (var paidrank in clan.Ranks.Where(x =>
			         x.RankPath == "Legionary" &&
			         !x.Name.Contains("Centurio", StringComparison.InvariantCultureIgnoreCase)))
			paidrank.RanksPaygrades.Add(new RanksPaygrade { Rank = paidrank, Paygrade = paygrade, Order = 2 });
		context.SaveChanges();

		paygrade = new Paygrade
		{
			Name = "Caligatius Eques",
			Abbreviation = "ClgEq",
			Currency = context.Currencies.First(),
			PayAmount = 1050M,
			Clan = clan
		};
		context.Paygrades.Add(paygrade);
		foreach (var paidrank in clan.Ranks.Where(x =>
			         x.RankPath == "Auxiliary Cavalry" &&
			         !x.Name.Contains("Centurio", StringComparison.InvariantCultureIgnoreCase)))
		{
			if (paidrank.Name != "Gregalis") continue;
			paidrank.RanksPaygrades.Add(new RanksPaygrade { Rank = paidrank, Paygrade = paygrade, Order = 0 });
		}

		context.SaveChanges();

		paygrade = new Paygrade
		{
			Name = "Sesquiplicarius Eques",
			Abbreviation = "SqpEq",
			Currency = context.Currencies.First(),
			PayAmount = 1575M,
			Clan = clan
		};
		context.Paygrades.Add(paygrade);
		foreach (var paidrank in clan.Ranks.Where(x =>
			         x.RankPath == "Auxiliary Cavalry" &&
			         !x.Name.Contains("Centurio", StringComparison.InvariantCultureIgnoreCase)))
		{
			if (!paidrank.Name.In("Gregalis", "Sesquiplicarius ")) continue;
			paidrank.RanksPaygrades.Add(new RanksPaygrade { Rank = paidrank, Paygrade = paygrade, Order = 1 });
		}

		context.SaveChanges();

		paygrade = new Paygrade
		{
			Name = "Duplicarius Eques",
			Abbreviation = "DplcEq",
			Currency = context.Currencies.First(),
			PayAmount = 2100M,
			Clan = clan
		};
		context.Paygrades.Add(paygrade);
		foreach (var paidrank in clan.Ranks.Where(x =>
			         x.RankPath == "Auxiliary Cavalry" &&
			         !x.Name.Contains("Centurio", StringComparison.InvariantCultureIgnoreCase)))
			paidrank.RanksPaygrades.Add(new RanksPaygrade { Rank = paidrank, Paygrade = paygrade, Order = 2 });
		context.SaveChanges();

		paygrade = new Paygrade
		{
			Name = "Caligatius Centurio",
			Abbreviation = "ClgCt",
			Currency = context.Currencies.First(),
			PayAmount = 5000M,
			Clan = clan
		};
		context.Paygrades.Add(paygrade);
		foreach (var paidrank in clan.Ranks.Where(x =>
			         x.Name.Contains("Centurio", StringComparison.InvariantCultureIgnoreCase)))
			paidrank.RanksPaygrades.Add(new RanksPaygrade { Rank = paidrank, Paygrade = paygrade, Order = 0 });
		context.SaveChanges();

		paygrade = new Paygrade
		{
			Name = "Sesquiplicarius Centurio",
			Abbreviation = "SqpCt",
			Currency = context.Currencies.First(),
			PayAmount = 7500M,
			Clan = clan
		};
		context.Paygrades.Add(paygrade);
		foreach (var paidrank in clan.Ranks.Where(x =>
			         x.Name.Contains("Centurio", StringComparison.InvariantCultureIgnoreCase)))
			paidrank.RanksPaygrades.Add(new RanksPaygrade { Rank = paidrank, Paygrade = paygrade, Order = 1 });
		context.SaveChanges();

		paygrade = new Paygrade
		{
			Name = "Duplicarius Centurio",
			Abbreviation = "DplcCt",
			Currency = context.Currencies.First(),
			PayAmount = 10000M,
			Clan = clan
		};
		context.Paygrades.Add(paygrade);
		foreach (var paidrank in clan.Ranks.Where(x =>
			         x.Name.Contains("Centurio", StringComparison.InvariantCultureIgnoreCase)))
			paidrank.RanksPaygrades.Add(new RanksPaygrade { Rank = paidrank, Paygrade = paygrade, Order = 2 });
		context.SaveChanges();

		paygrade = new Paygrade
		{
			Name = "Triplicarius Centurio",
			Abbreviation = "TrpCt",
			Currency = context.Currencies.First(),
			PayAmount = 15000M,
			Clan = clan
		};
		context.Paygrades.Add(paygrade);
		foreach (var paidrank in clan.Ranks.Where(x =>
			         x.Name.Contains("Centurio", StringComparison.InvariantCultureIgnoreCase)))
			paidrank.RanksPaygrades.Add(new RanksPaygrade { Rank = paidrank, Paygrade = paygrade, Order = 3 });
		context.SaveChanges();

		paygrade = new Paygrade
		{
			Name = "Caligatius Beneficiarius",
			Abbreviation = "ClgBe",
			Currency = context.Currencies.First(),
			PayAmount = 13000M,
			Clan = clan
		};
		context.Paygrades.Add(paygrade);
		foreach (var paidrank in clan.Ranks.Where(x =>
			         x.Name.Contains("Beneficiarius", StringComparison.InvariantCultureIgnoreCase)))
			paidrank.RanksPaygrades.Add(new RanksPaygrade { Rank = paidrank, Paygrade = paygrade, Order = 0 });
		context.SaveChanges();

		paygrade = new Paygrade
		{
			Name = "Caligatius Praefectus",
			Abbreviation = "ClgPr",
			Currency = context.Currencies.First(),
			PayAmount = 50000M,
			Clan = clan
		};
		context.Paygrades.Add(paygrade);
		foreach (var paidrank in clan.Ranks.Where(x =>
			         x.Name.Contains("Praefectus", StringComparison.InvariantCultureIgnoreCase)))
			paidrank.RanksPaygrades.Add(new RanksPaygrade { Rank = paidrank, Paygrade = paygrade, Order = 0 });
		context.SaveChanges();

		paygrade = new Paygrade
		{
			Name = "Sesquiplicarius Praefectus",
			Abbreviation = "SqpPr",
			Currency = context.Currencies.First(),
			PayAmount = 60000M,
			Clan = clan
		};
		context.Paygrades.Add(paygrade);
		foreach (var paidrank in clan.Ranks.Where(x =>
			         x.Name.Contains("Praefectus", StringComparison.InvariantCultureIgnoreCase)))
			paidrank.RanksPaygrades.Add(new RanksPaygrade { Rank = paidrank, Paygrade = paygrade, Order = 1 });
		context.SaveChanges();

		paygrade = new Paygrade
		{
			Name = "Duplicarius Praefectus",
			Abbreviation = "DplcPr",
			Currency = context.Currencies.First(),
			PayAmount = 70000M,
			Clan = clan
		};
		context.Paygrades.Add(paygrade);
		foreach (var paidrank in clan.Ranks.Where(x =>
			         x.Name.Contains("Praefectus", StringComparison.InvariantCultureIgnoreCase)))
			paidrank.RanksPaygrades.Add(new RanksPaygrade { Rank = paidrank, Paygrade = paygrade, Order = 2 });
		context.SaveChanges();

		paygrade = new Paygrade
		{
			Name = "Triplicarius Praefectus",
			Abbreviation = "TrpPr",
			Currency = context.Currencies.First(),
			PayAmount = 80000M,
			Clan = clan
		};
		context.Paygrades.Add(paygrade);
		foreach (var paidrank in clan.Ranks.Where(x =>
			         x.Name.Contains("Praefectus", StringComparison.InvariantCultureIgnoreCase)))
			paidrank.RanksPaygrades.Add(new RanksPaygrade { Rank = paidrank, Paygrade = paygrade, Order = 3 });
		context.SaveChanges();

		paygrade = new Paygrade
		{
			Name = "Caligatius Legatus",
			Abbreviation = "ClgLs",
			Currency = context.Currencies.First(),
			PayAmount = 75000M,
			Clan = clan
		};
		context.Paygrades.Add(paygrade);
		foreach (var paidrank in clan.Ranks.Where(x =>
			         x.Name.Contains("Legatus", StringComparison.InvariantCultureIgnoreCase)))
			paidrank.RanksPaygrades.Add(new RanksPaygrade { Rank = paidrank, Paygrade = paygrade, Order = 0 });
		context.SaveChanges();

		paygrade = new Paygrade
		{
			Name = "Sesquiplicarius Legatus",
			Abbreviation = "SqpLs",
			Currency = context.Currencies.First(),
			PayAmount = 100000M,
			Clan = clan
		};
		context.Paygrades.Add(paygrade);
		foreach (var paidrank in clan.Ranks.Where(x =>
			         x.Name.Contains("Legatus", StringComparison.InvariantCultureIgnoreCase)))
			paidrank.RanksPaygrades.Add(new RanksPaygrade { Rank = paidrank, Paygrade = paygrade, Order = 1 });
		context.SaveChanges();

		paygrade = new Paygrade
		{
			Name = "Duplicarius Legatus",
			Abbreviation = "DplcLs",
			Currency = context.Currencies.First(),
			PayAmount = 125000M,
			Clan = clan
		};
		context.Paygrades.Add(paygrade);
		foreach (var paidrank in clan.Ranks.Where(x =>
			         x.Name.Contains("Legatus", StringComparison.InvariantCultureIgnoreCase)))
			paidrank.RanksPaygrades.Add(new RanksPaygrade { Rank = paidrank, Paygrade = paygrade, Order = 2 });
		context.SaveChanges();

		paygrade = new Paygrade
		{
			Name = "Triplicarius Legatus",
			Abbreviation = "TrpLs",
			Currency = context.Currencies.First(),
			PayAmount = 150000M,
			Clan = clan
		};
		context.Paygrades.Add(paygrade);
		foreach (var paidrank in clan.Ranks.Where(x =>
			         x.Name.Contains("Legatus", StringComparison.InvariantCultureIgnoreCase)))
			paidrank.RanksPaygrades.Add(new RanksPaygrade { Rank = paidrank, Paygrade = paygrade, Order = 3 });
		context.SaveChanges();
	}

	private void SetupCouncilClan(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{

		if (context.Clans.Any(x => x.Name == "Council Template"))
		{
			return;
		}

		var clan = new Clan
		{
			Name = "Council Template",
			Alias = "council",
			FullName = "Council Template",
			Description =
				"This is a template for councils, committees and the like with a flat membership structure but several key appointments.",
			PayIntervalType = (int)IntervalType.Monthly,
			PayIntervalModifier = 1,
			PayIntervalOther = 0,
			CalendarId = context.Calendars.First().Id,
			PayIntervalReferenceDate = context.Calendars.First().Date,
			PayIntervalReferenceTime = $"{context.Timezones.First(x => x.Clock.PrimaryTimezoneId == x.Id).Name} 0:0:0",
			IsTemplate = true,
			ShowClanMembersInWho = false
		};
		context.Clans.Add(clan);
		context.SaveChanges();

		var rank = new Rank
		{
			Clan = clan,
			Name = "Member",
			RankNumber = 0,
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers)
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Mbr" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Member" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		var appointment = new Appointment
		{
			Clan = clan,
			Name = "Chairperson",
			Privileges = (long)(ClanPrivilegeType.CanCastout | ClanPrivilegeType.CanInduct |
			                    ClanPrivilegeType.CanCreateAppointments | ClanPrivilegeType.CanAppoint),
			IsAppointedByElection = true,
			IsSecretBallot = false,
			ElectionTermMinutes = 60 * 24 * 365,
			ElectionLeadTimeMinutes = 0,
			NominationPeriodMinutes = 60 * 24 * 7,
			VotingPeriodMinutes = 60 * 24 * 7,
			NumberOfVotesProg = context.FutureProgs.First(x => x.FunctionName == "AlwaysOne"),
			CanNominateProg = context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue"),
			MaximumConsecutiveTerms = 0,
			MaximumTotalTerms = 0,
			MaximumSimultaneousHolders = 1
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = appointment, Order = 0, Abbreviation = "Chr" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = appointment, Order = 0, Title = "Chairperson" });
		context.Appointments.Add(appointment);
		context.SaveChanges();
	}

	private void SetupGangClan(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{

		if (context.Clans.Any(x => x.Name == "Gang Template"))
		{
			return;
		}

		var clan = new Clan
		{
			Name = "Gang Template",
			Alias = "gang",
			FullName = "Gang Template",
			Description = "This is a template for street gangs",
			PayIntervalType = (int)IntervalType.Monthly,
			PayIntervalModifier = 1,
			PayIntervalOther = 0,
			CalendarId = context.Calendars.First().Id,
			PayIntervalReferenceDate = context.Calendars.First().Date,
			PayIntervalReferenceTime = $"{context.Timezones.First(x => x.Clock.PrimaryTimezoneId == x.Id).Name} 0:0:0",
			IsTemplate = true,
			ShowClanMembersInWho = false
		};
		context.Clans.Add(clan);
		context.SaveChanges();

		var rank = new Rank
		{
			Clan = clan,
			Name = "Uninitiated",
			RankNumber = 0,
			Privileges = (long)ClanPrivilegeType.None
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Uninit" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Uninitiated" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Initiated",
			RankNumber = 1,
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers)
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Init" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Initiated" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Lieutenant",
			RankNumber = 2,
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers | ClanPrivilegeType.CanCastout |
			                    ClanPrivilegeType.CanInduct | ClanPrivilegeType.CanPromote |
			                    ClanPrivilegeType.CanDemote)
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Lt" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Lieutenant" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Clan = clan,
			Name = "Boss",
			RankNumber = 3,
			Privileges = (long)ClanPrivilegeType.All
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Boss" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Boss" });
		context.Ranks.Add(rank);
		context.SaveChanges();
	}

	private void SetupUKPoliceOrganisation(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> questionAnswers)
	{

		if (context.Clans.Any(x => x.Name == "UK Police Template"))
		{
			return;
		}

		var clan = new Clan
		{
			Name = "UK Police Template",
			Alias = "police",
			FullName = "UK Police Template",
			Description =
				"This is designed to be used as a template clan for a modern police organisation based on UK ranks",
			PayIntervalType = (int)IntervalType.Monthly,
			PayIntervalModifier = 1,
			PayIntervalOther = 0,
			CalendarId = context.Calendars.First().Id,
			PayIntervalReferenceDate = context.Calendars.First().Date,
			PayIntervalReferenceTime = $"{context.Timezones.First(x => x.Clock.PrimaryTimezoneId == x.Id).Name} 0:0:0",
			IsTemplate = true,
			ShowClanMembersInWho = false
		};
		context.Clans.Add(clan);
		context.SaveChanges();

		var juniorPrivs = (long)(ClanPrivilegeType.CanViewClanStructure |
		                         ClanPrivilegeType.CanViewClanOfficeHolders |
		                         ClanPrivilegeType.CanViewMembers |
		                         ClanPrivilegeType.CanAccessLeasedProperties);


		var rank = new Rank
		{
			Name = "Probationary Constable",
			Clan = clan,
			RankNumber = 1,
			RankPath = "Constabulary",
			Privileges = juniorPrivs
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "PPC" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Probationary Constable" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		var ranks = new Dictionary<string, Rank>(StringComparer.OrdinalIgnoreCase);

		rank = new Rank
		{
			Name = "Constable",
			Clan = clan,
			RankNumber = 2,
			RankPath = "Constabulary",
			Privileges = juniorPrivs
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "PC" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Constable" });
		context.Ranks.Add(rank);
		ranks.Add("Constable", rank);
		context.SaveChanges();

		rank = new Rank
		{
			Name = "Senior Constable",
			Clan = clan,
			RankNumber = 3,
			RankPath = "Constabulary",
			Privileges = juniorPrivs
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "SPC" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Senior Constable" });
		context.Ranks.Add(rank);
		ranks.Add("Senior Constable", rank);
		context.SaveChanges();

		rank = new Rank
		{
			Name = "Sergeant",
			Clan = clan,
			RankNumber = 4,
			RankPath = "Constabulary",
			Privileges = juniorPrivs
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "Sgt" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Sergeant" });
		context.Ranks.Add(rank);
		ranks.Add("Sergeant", rank);
		context.SaveChanges();

		rank = new Rank
		{
			Name = "Inspector",
			Clan = clan,
			RankNumber = 5,
			RankPath = "Constabulary",
			Privileges = juniorPrivs
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "Insp" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Inspector" });
		context.Ranks.Add(rank);
		ranks.Add("Inspector", rank);
		context.SaveChanges();

		rank = new Rank
		{
			Name = "Chief Inspector",
			Clan = clan,
			RankNumber = 6,
			RankPath = "Constabulary",
			Privileges = juniorPrivs
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "Ch Insp" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Chief Inspector" });
		context.Ranks.Add(rank);
		ranks.Add("Chief Inspector", rank);
		context.SaveChanges();

		var seniorPrivs = (long)(ClanPrivilegeType.CanViewClanStructure |
		                         ClanPrivilegeType.CanViewClanOfficeHolders |
		                         ClanPrivilegeType.CanViewMembers |
		                         ClanPrivilegeType.CanAccessLeasedProperties |
		                         ClanPrivilegeType.CanCastout | ClanPrivilegeType.CanAppoint |
		                         ClanPrivilegeType.CanDecreasePaygrade | ClanPrivilegeType.CanIncreasePaygrade |
		                         ClanPrivilegeType.CanInduct | ClanPrivilegeType.CanManageClanJobs |
		                         ClanPrivilegeType.CanManageBankAccounts | ClanPrivilegeType.CanManageClanProperty |
		                         ClanPrivilegeType.CanManageEconomicZones | ClanPrivilegeType.CanDemote |
		                         ClanPrivilegeType.CanGiveBackpay);

		rank = new Rank
		{
			Name = "Superintendent",
			Clan = clan,
			RankNumber = 7,
			RankPath = "Constabulary",
			Privileges = seniorPrivs
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "Supt" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Superintendent" });
		context.Ranks.Add(rank);
		ranks.Add("Superintendent", rank);
		context.SaveChanges();

		rank = new Rank
		{
			Name = "Chief Superintendent",
			Clan = clan,
			RankNumber = 8,
			RankPath = "Constabulary",
			Privileges = seniorPrivs
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "Ch Supt" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Chief Superintendent" });
		context.Ranks.Add(rank);
		ranks.Add("Chief Superintendent", rank);
		context.SaveChanges();

		rank = new Rank
		{
			Name = "Assistant Chief Constable",
			Clan = clan,
			RankNumber = 9,
			RankPath = "Commander",
			Privileges = seniorPrivs
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "ACC" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Assistant Chief Constable" });
		context.Ranks.Add(rank);
		ranks.Add("Assistant Chief Constable", rank);
		context.SaveChanges();

		rank = new Rank
		{
			Name = "Deputy Chief Constable",
			Clan = clan,
			RankNumber = 10,
			RankPath = "Commander",
			Privileges = seniorPrivs
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "DCC" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Deputy Chief Constable" });
		context.Ranks.Add(rank);
		ranks.Add("Deputy Chief Constable", rank);
		context.SaveChanges();

		var topPrivs = (long)(ClanPrivilegeType.CanViewClanStructure |
		                      ClanPrivilegeType.CanViewClanOfficeHolders |
		                      ClanPrivilegeType.CanViewMembers |
		                      ClanPrivilegeType.CanAccessLeasedProperties |
		                      ClanPrivilegeType.CanCastout | ClanPrivilegeType.CanAppoint |
		                      ClanPrivilegeType.CanDecreasePaygrade | ClanPrivilegeType.CanIncreasePaygrade |
		                      ClanPrivilegeType.CanInduct | ClanPrivilegeType.CanManageClanJobs |
		                      ClanPrivilegeType.CanManageBankAccounts | ClanPrivilegeType.CanManageClanProperty |
		                      ClanPrivilegeType.CanManageEconomicZones | ClanPrivilegeType.CanDemote |
		                      ClanPrivilegeType.CanGiveBackpay | ClanPrivilegeType.CanChangeRankPath |
		                      ClanPrivilegeType.CanCreateAppointments | ClanPrivilegeType.CanCreateBudgets |
		                      ClanPrivilegeType.CanCreatePaygrades | ClanPrivilegeType.CanCreateRanks |
		                      ClanPrivilegeType.CanPromoteToOwnRank);

		rank = new Rank
		{
			Name = "Chief Constable",
			Clan = clan,
			RankNumber = 11,
			RankPath = "Commander",
			Privileges = topPrivs
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "CC" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Chief Constable" });
		context.Ranks.Add(rank);
		ranks.Add("Chief Constable", rank);
		context.SaveChanges();

		var detective = new Appointment
		{
			Clan = clan,
			Name = "Detective",
			MaximumSimultaneousHolders = 0,
			MinimumRank = ranks["Constable"],
			IsAppointedByElection = false,
			FameType = 0
		};
		context.Appointments.Add(detective);
		detective.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = detective, Order = 0, Abbreviation = "Det" });
		detective.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = detective, Order = 0, Title = "Detective" });
		context.SaveChanges();

		var prog = new FutureProg
		{
			FunctionName = "IsDetectiveTemplate",
			Category = "Clan",
			Subcategory = "Constabulary",
			FunctionComment =
				"True if the character holds the appointment of Detective. You should clone and update this for any clan using this template",
			ReturnType = (long)ProgVariableTypes.Boolean,
			StaticType = 0,
			FunctionText = $"return IsClanMember(@ch, ToClan({clan.Id}), ToAppointment({detective.Id}))"
		};
		prog.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = prog,
			ParameterIndex = 0,
			ParameterType = 8200,
			ParameterName = "ch"
		});
		context.FutureProgs.Add(prog);
		context.SaveChanges();

		ranks["Constable"].RanksAbbreviations.Add(new RanksAbbreviations
			{ Rank = ranks["Constable"], Order = 0, FutureProg = prog, Abbreviation = "DPC" });
		ranks["Constable"].RanksTitles.Add(new RanksTitle
			{ Rank = ranks["Constable"], Order = 0, FutureProg = prog, Title = "Detective Constable" });
		ranks["Senior Constable"].RanksAbbreviations.Add(new RanksAbbreviations
			{ Rank = ranks["Senior Constable"], Order = 0, FutureProg = prog, Abbreviation = "DSPC" });
		ranks["Senior Constable"].RanksTitles.Add(new RanksTitle
			{ Rank = ranks["Senior Constable"], Order = 0, FutureProg = prog, Title = "Detective Senior Constable" });
		ranks["Sergeant"].RanksAbbreviations.Add(new RanksAbbreviations
			{ Rank = ranks["Sergeant"], Order = 0, FutureProg = prog, Abbreviation = "DSgt" });
		ranks["Sergeant"].RanksTitles.Add(new RanksTitle
			{ Rank = ranks["Sergeant"], Order = 0, FutureProg = prog, Title = "Detective Sergeant" });
		ranks["Inspector"].RanksAbbreviations.Add(new RanksAbbreviations
			{ Rank = ranks["Inspector"], Order = 0, FutureProg = prog, Abbreviation = "DInsp" });
		ranks["Inspector"].RanksTitles.Add(new RanksTitle
			{ Rank = ranks["Inspector"], Order = 0, FutureProg = prog, Title = "Detective Inspector" });
		ranks["Chief Inspector"].RanksAbbreviations.Add(new RanksAbbreviations
			{ Rank = ranks["Chief Inspector"], Order = 0, FutureProg = prog, Abbreviation = "DCh Insp" });
		ranks["Chief Inspector"].RanksTitles.Add(new RanksTitle
			{ Rank = ranks["Chief Inspector"], Order = 0, FutureProg = prog, Title = "Detective Chief Inspector" });
		ranks["Superintendent"].RanksAbbreviations.Add(new RanksAbbreviations
			{ Rank = ranks["Superintendent"], Order = 0, FutureProg = prog, Abbreviation = "DSupt" });
		ranks["Superintendent"].RanksTitles.Add(new RanksTitle
			{ Rank = ranks["Superintendent"], Order = 0, FutureProg = prog, Title = "Detective Superintendent" });
		ranks["Chief Superintendent"].RanksAbbreviations.Add(new RanksAbbreviations
			{ Rank = ranks["Chief Superintendent"], Order = 0, FutureProg = prog, Abbreviation = "DCh Supt" });
		ranks["Chief Superintendent"].RanksTitles.Add(new RanksTitle
		{
			Rank = ranks["Chief Superintendent"], Order = 0, FutureProg = prog, Title = "Detective Chief Superintendent"
		});

		context.SaveChanges();
	}

	private void SetupMilitaryClan(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> questionAnswers)
	{

		if (context.Clans.Any(x => x.Name == "Army Template"))
		{
			return;
		}

		var clan = new Clan
		{
			Name = "Army Template",
			Alias = "army",
			FullName = "Army Template",
			Description = "This is designed to be used as a template clan for a military clan based on the US Army",
			PayIntervalType = (int)IntervalType.Monthly,
			PayIntervalModifier = 1,
			PayIntervalOther = 0,
			CalendarId = context.Calendars.First().Id,
			PayIntervalReferenceDate = context.Calendars.First().Date,
			PayIntervalReferenceTime = $"{context.Timezones.First(x => x.Clock.PrimaryTimezoneId == x.Id).Name} 0:0:0",
			IsTemplate = true,
			ShowClanMembersInWho = false
		};
		context.Clans.Add(clan);
		context.SaveChanges();

		var rank = new Rank
		{
			Name = "Recruit",
			Clan = clan,
			RankNumber = 1,
			RankPath = "Enlisted",
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers)
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "E-1" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Recruit" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Name = "Private",
			Clan = clan,
			RankNumber = 2,
			RankPath = "Enlisted",
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers)
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "E-2" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Private" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Name = "Private First Class",
			Clan = clan,
			RankNumber = 3,
			RankPath = "Enlisted",
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers)
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "E-3" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Private First Class" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Name = "Corporal",
			Clan = clan,
			RankNumber = 4,
			RankPath = "Enlisted",
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers)
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "E-4" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Corporal" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Name = "Sergeant",
			Clan = clan,
			RankNumber = 5,
			RankPath = "Enlisted",
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers)
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "E-5" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Sergeant" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Name = "Staff Sergeant",
			Clan = clan,
			RankNumber = 6,
			RankPath = "Enlisted",
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers)
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "E-6" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Staff Sergeant" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Name = "Sergeant First Class",
			Clan = clan,
			RankNumber = 7,
			RankPath = "Enlisted",
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers)
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "E-7" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Sergeant First Class" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Name = "Master Sergeant",
			Clan = clan,
			RankNumber = 8,
			RankPath = "Enlisted",
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers)
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "E-8" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Master Sergeant" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Name = "Sergeant Major",
			Clan = clan,
			RankNumber = 9,
			RankPath = "Enlisted",
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers)
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "E-9" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Sergeant Major" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Name = "Warrant Officer Second Class",
			Clan = clan,
			RankNumber = 10,
			RankPath = "Warrant",
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers)
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "W-1" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Warrant Officer Second Class" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Name = "Warrant Officer First Class",
			Clan = clan,
			RankNumber = 11,
			RankPath = "Warrant",
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers)
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "W-2" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Warrant Officer First Class" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Name = "Second Lieutenant",
			Clan = clan,
			RankNumber = 12,
			RankPath = "Officer",
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers)
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "O-1" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Second Lieutenant" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Name = "First Lieutenant",
			Clan = clan,
			RankNumber = 13,
			RankPath = "Officer",
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers)
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "O-2" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "First Lieutenant" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Name = "Captain",
			Clan = clan,
			RankNumber = 14,
			RankPath = "Officer",
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers | ClanPrivilegeType.CanIncreasePaygrade |
			                    ClanPrivilegeType.CanDecreasePaygrade)
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "O-3" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Captain" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Name = "Major",
			Clan = clan,
			RankNumber = 15,
			RankPath = "Officer",
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers | ClanPrivilegeType.CanPromote |
			                    ClanPrivilegeType.CanIncreasePaygrade | ClanPrivilegeType.CanDecreasePaygrade)
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "O-4" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Major" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Name = "Lieutenant Colonel",
			Clan = clan,
			RankNumber = 16,
			RankPath = "Officer",
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers | ClanPrivilegeType.CanPromote |
			                    ClanPrivilegeType.CanIncreasePaygrade | ClanPrivilegeType.CanDecreasePaygrade)
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "O-5" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Lieutenant Colonel" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Name = "Colonel",
			Clan = clan,
			RankNumber = 17,
			RankPath = "Officer",
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers | ClanPrivilegeType.CanPromote |
			                    ClanPrivilegeType.CanIncreasePaygrade | ClanPrivilegeType.CanDecreasePaygrade)
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "O-6" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Colonel" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Name = "Brigadier General",
			Clan = clan,
			RankNumber = 18,
			RankPath = "Officer",
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers | ClanPrivilegeType.CanPromote |
			                    ClanPrivilegeType.CanIncreasePaygrade | ClanPrivilegeType.CanDecreasePaygrade)
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "O-7" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Brigadier General" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Name = "Major General",
			Clan = clan,
			RankNumber = 19,
			RankPath = "Officer",
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers | ClanPrivilegeType.CanPromote |
			                    ClanPrivilegeType.CanIncreasePaygrade | ClanPrivilegeType.CanDecreasePaygrade)
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "O-8" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Major General" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Name = "Lieutenant General",
			Clan = clan,
			RankNumber = 20,
			RankPath = "Officer",
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers | ClanPrivilegeType.CanPromote |
			                    ClanPrivilegeType.CanIncreasePaygrade | ClanPrivilegeType.CanDecreasePaygrade)
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "O-9" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Lieutenant General" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Name = "General",
			Clan = clan,
			RankNumber = 21,
			RankPath = "Officer",
			Privileges = (long)(ClanPrivilegeType.CanViewClanStructure | ClanPrivilegeType.CanViewClanOfficeHolders |
			                    ClanPrivilegeType.CanViewMembers | ClanPrivilegeType.CanPromote |
			                    ClanPrivilegeType.CanPromoteToOwnRank | ClanPrivilegeType.CanChangeRankPath |
			                    ClanPrivilegeType.CanIncreasePaygrade | ClanPrivilegeType.CanDecreasePaygrade)
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "O-10" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "General" });
		var generalRank = rank;
		context.Ranks.Add(rank);
		context.SaveChanges();

		var appointment = new Appointment
		{
			Name = "General of the Army",
			Clan = clan,
			MinimumRank = generalRank,
			Privileges = (long)ClanPrivilegeType.All,
			IsAppointedByElection = false
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = appointment, Order = 0, Abbreviation = "GoA" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = appointment, Order = 0, Title = "General of the Army" });
		context.Appointments.Add(appointment);
		context.SaveChanges();
	}

	private void SetupRomanCity(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{

		if (context.Clans.Any(x => x.Name == "Roman City Template"))
		{
			return;
		}

		var isMaleProg = context.FutureProgs.First(x => x.FunctionName == "IsMale");
		var isFemaleProg = context.FutureProgs.First(x => x.FunctionName == "IsFemale");

		var cityClan = new Clan
		{
			Name = "Roman City Template",
			Alias = "rct",
			FullName = "Roman City Template",
			Description =
				"This clan is designed to be used as a template for a city based on the roman model, with roman named civilian ranks and civic appointments.",
			PayIntervalType = (int)IntervalType.Monthly,
			PayIntervalModifier = 1,
			PayIntervalOther = 0,
			CalendarId = context.Calendars.First().Id,
			PayIntervalReferenceDate = context.Calendars.First().Date,
			PayIntervalReferenceTime = $"{context.Timezones.First(x => x.Clock.PrimaryTimezoneId == x.Id).Name} 0:0:0",
			IsTemplate = true,
			ShowClanMembersInWho = false
		};
		context.Clans.Add(cityClan);
		context.SaveChanges();

		var rank = new Rank
		{
			Name = "Servus",
			Clan = cityClan,
			RankNumber = -1,
			RankPath = "Slave",
			Privileges = (long)ClanPrivilegeType.None
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Srv" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Serva", FutureProg = isFemaleProg });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Servus" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		var civesPrivs = (long)(ClanPrivilegeType.CanViewClanStructure |
		                        ClanPrivilegeType.CanViewClanOfficeHolders | ClanPrivilegeType.CanViewMembers |
		                        ClanPrivilegeType.CanAccessLeasedProperties | ClanPrivilegeType.CanReportDead);

		rank = new Rank
		{
			Name = "Libertinus",
			Clan = cityClan,
			RankNumber = 0,
			RankPath = "Common",
			Privileges = (long)(ClanPrivilegeType.CanViewClanOfficeHolders | ClanPrivilegeType.CanViewClanStructure |
			                    ClanPrivilegeType.CanViewMembers)
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Lbtns" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Libertina", FutureProg = isFemaleProg });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Libertinus" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Name = "Provinciales",
			Clan = cityClan,
			RankNumber = 1,
			RankPath = "Common",
			Privileges = (long)(ClanPrivilegeType.CanViewClanOfficeHolders | ClanPrivilegeType.CanViewClanStructure |
			                    ClanPrivilegeType.CanViewMembers)
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Prcls" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Provinciales" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Name = "Socius",
			Clan = cityClan,
			RankNumber = 2,
			RankPath = "Common",
			Privileges = (long)(ClanPrivilegeType.CanViewClanOfficeHolders | ClanPrivilegeType.CanViewClanStructure |
			                    ClanPrivilegeType.CanViewMembers)
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Scs" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Socia", FutureProg = isFemaleProg });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Socius" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Name = "Latinus",
			Clan = cityClan,
			RankNumber = 3,
			RankPath = "Common",
			Privileges = (long)(ClanPrivilegeType.CanViewClanOfficeHolders | ClanPrivilegeType.CanViewClanStructure |
			                    ClanPrivilegeType.CanViewMembers)
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Ltns" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Latina", FutureProg = isFemaleProg });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Latinus" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		var isPatricianProg = new FutureProg
		{
			FunctionName = "IsPatricianRomanCityTemplate",
			FunctionComment = "True if the individual is one of the patrician class in the Roman City Template clan",
			ReturnType = (long)ProgVariableTypes.Boolean,
			AcceptsAnyParameters = false,
			StaticType = (int)FutureProgStaticType.NotStatic,
			Category = "Character",
			Subcategory = "Clan",
			FunctionText =
				$"return IsClanMember(@ch, ToClan({cityClan.Id}), ToAppointment(ToClan({cityClan.Id}), \"Patrician\"))"
		};
		isPatricianProg.FutureProgsParameters.Add(new FutureProgsParameter
			{ FutureProg = isPatricianProg, ParameterIndex = 0, ParameterName = "ch", ParameterType = 8 });
		context.FutureProgs.Add(isPatricianProg);
		context.SaveChanges();
		var isPatricianMaleProg = new FutureProg
		{
			FunctionName = "IsPatricianMaleRomanCityTemplate",
			FunctionComment =
				"True if the individual is a male and one of the patrician class in the Roman City Template clan",
			ReturnType = (long)ProgVariableTypes.Boolean,
			AcceptsAnyParameters = false,
			StaticType = (int)FutureProgStaticType.NotStatic,
			Category = "Character",
			Subcategory = "Clan",
			FunctionText =
				$"return @ch.Gender == ToGender(\"Male\") and IsClanMember(@ch, ToClan({cityClan.Id}), ToAppointment(ToClan({cityClan.Id}), \"Patrician\"))"
		};
		context.FutureProgs.Add(isPatricianMaleProg);
		isPatricianMaleProg.FutureProgsParameters.Add(new FutureProgsParameter
			{ FutureProg = isPatricianMaleProg, ParameterIndex = 0, ParameterName = "ch", ParameterType = 8 });
		context.SaveChanges();
		var isNotPatricianProg = new FutureProg
		{
			FunctionName = "IsNotPatricianRomanCityTemplate",
			FunctionComment =
				"True if the individual is not one of the patrician class in the Roman City Template clan",
			ReturnType = (long)ProgVariableTypes.Boolean,
			AcceptsAnyParameters = false,
			StaticType = (int)FutureProgStaticType.NotStatic,
			Category = "Character",
			Subcategory = "Clan",
			FunctionText =
				$"return Not(IsClanMember(@ch, ToClan({cityClan.Id}), ToAppointment(ToClan({cityClan.Id}), \"Patrician\")))"
		};
		context.FutureProgs.Add(isNotPatricianProg);
		isNotPatricianProg.FutureProgsParameters.Add(new FutureProgsParameter
			{ FutureProg = isNotPatricianProg, ParameterIndex = 0, ParameterName = "ch", ParameterType = 8 });
		context.SaveChanges();

		rank = new Rank
		{
			Name = "Civis Romanus",
			Clan = cityClan,
			RankNumber = 4,
			RankPath = "Common",
			Privileges = civesPrivs
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations
			{ Rank = rank, Order = 0, Abbreviation = "Ptrcs", FutureProg = isPatricianMaleProg });
		rank.RanksAbbreviations.Add(new RanksAbbreviations
			{ Rank = rank, Order = 1, Abbreviation = "Ptrc", FutureProg = isPatricianProg });
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 2, Abbreviation = "Plbs" });
		rank.RanksTitles.Add(new RanksTitle
			{ Rank = rank, Order = 0, Title = "Patricius", FutureProg = isPatricianMaleProg });
		rank.RanksTitles.Add(
			new RanksTitle { Rank = rank, Order = 1, Title = "Patricia", FutureProg = isPatricianProg });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Plebian" });
		context.Ranks.Add(rank);
		var vulgusRank = rank;
		context.SaveChanges();

		rank = new Rank
		{
			Name = "Eques",
			Clan = cityClan,
			RankNumber = 5,
			RankPath = "Elite",
			Privileges = civesPrivs
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Eqs" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Eques" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		var senatorialPrivs = (long)(ClanPrivilegeType.CanViewClanStructure |
		                             ClanPrivilegeType.CanViewClanOfficeHolders | ClanPrivilegeType.CanViewMembers |
		                             ClanPrivilegeType.CanAccessLeasedProperties | ClanPrivilegeType.CanReportDead |
		                             ClanPrivilegeType.CanInduct |
		                             ClanPrivilegeType.CanPromote
			);

		rank = new Rank
		{
			Name = "Senator",
			Clan = cityClan,
			RankNumber = 6,
			RankPath = "Elite",
			Privileges = senatorialPrivs
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations
			{ Rank = rank, Order = 0, Abbreviation = "Sntr", FutureProg = isMaleProg });
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "Sntrx" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Senator", FutureProg = isMaleProg });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Senatrix" });
		context.Ranks.Add(rank);
		var senatorRank = rank;
		context.SaveChanges();

		rank = new Rank
		{
			Name = "Proquaestor",
			Clan = cityClan,
			RankNumber = 7,
			RankPath = "Elite",
			Privileges = senatorialPrivs
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations
			{ Rank = rank, Order = 0, Abbreviation = "Pqstr", FutureProg = isMaleProg });
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "Pqstrx" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Proquaestor", FutureProg = isMaleProg });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Proquaestrix" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Name = "Propraetor",
			Clan = cityClan,
			RankNumber = 8,
			RankPath = "Elite",
			Privileges = senatorialPrivs
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations
			{ Rank = rank, Order = 0, Abbreviation = "Pptr", FutureProg = isMaleProg });
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 1, Abbreviation = "Pptrx" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Propraetor", FutureProg = isMaleProg });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 1, Title = "Propraetrix" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		rank = new Rank
		{
			Name = "Proconsul",
			Clan = cityClan,
			RankNumber = 9,
			RankPath = "Elite",
			Privileges = senatorialPrivs
		};
		rank.RanksAbbreviations.Add(new RanksAbbreviations { Rank = rank, Order = 0, Abbreviation = "Pcnsl" });
		rank.RanksTitles.Add(new RanksTitle { Rank = rank, Order = 0, Title = "Proconsul" });
		context.Ranks.Add(rank);
		context.SaveChanges();

		var appointment = new Appointment
		{
			Name = "Patricius",
			Clan = cityClan,
			MaximumSimultaneousHolders = 0,
			MinimumRank = vulgusRank,
			Privileges = (long)ClanPrivilegeType.None
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = appointment, Order = 0, Abbreviation = "Ptrcs", FutureProg = isMaleProg });
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = appointment, Order = 1, Abbreviation = "Ptrc" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = appointment, Order = 0, Title = "Patricius", FutureProg = isMaleProg });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = appointment, Order = 1, Title = "Patricia" });
		context.Appointments.Add(appointment);
		context.SaveChanges();

		appointment = new Appointment
		{
			Name = "Quaestor",
			Clan = cityClan,
			MaximumSimultaneousHolders = 0,
			MinimumRank = senatorRank,
			Privileges = (long)ClanPrivilegeType.CanViewTreasury
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = appointment, Order = 0, Abbreviation = "Qstr", FutureProg = isMaleProg });
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = appointment, Order = 1, Abbreviation = "Qstrx" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = appointment, Order = 0, Title = "Quaestor", FutureProg = isMaleProg });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = appointment, Order = 1, Title = "Quaestrix" });
		context.Appointments.Add(appointment);
		context.SaveChanges();

		appointment = new Appointment
		{
			Name = "Aedilis",
			Clan = cityClan,
			MaximumSimultaneousHolders = 0,
			MinimumRank = senatorRank,
			Privileges = (long)(ClanPrivilegeType.CanViewTreasury | ClanPrivilegeType.CanManageClanJobs |
			                    ClanPrivilegeType.CanManageClanProperty |
			                    ClanPrivilegeType.CanManageEconomicZones |
			                    ClanPrivilegeType.CanManageBankAccounts)
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = appointment, Order = 0, Abbreviation = "Adls" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = appointment, Order = 0, Title = "Aedilis" });
		context.Appointments.Add(appointment);
		context.SaveChanges();

		appointment = new Appointment
		{
			Name = "Praetor",
			Clan = cityClan,
			MaximumSimultaneousHolders = 0,
			MinimumRank = senatorRank,
			Privileges = (long)(ClanPrivilegeType.CanViewTreasury | ClanPrivilegeType.CanManageClanJobs |
			                    ClanPrivilegeType.CanManageClanProperty |
			                    ClanPrivilegeType.CanManageEconomicZones |
			                    ClanPrivilegeType.CanManageBankAccounts |
			                    ClanPrivilegeType.CanCreateAppointmentsUnderOwn)
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = appointment, Order = 0, Abbreviation = "Prtr", FutureProg = isMaleProg });
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = appointment, Order = 1, Abbreviation = "Prtrx" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = appointment, Order = 0, Title = "Praetor", FutureProg = isMaleProg });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = appointment, Order = 1, Title = "Praetrix" });
		context.Appointments.Add(appointment);
		context.SaveChanges();

		appointment = new Appointment
		{
			Name = "Consul",
			Clan = cityClan,
			MaximumSimultaneousHolders = 0,
			MinimumRank = senatorRank,
			Privileges = (long)(ClanPrivilegeType.CanViewTreasury | ClanPrivilegeType.CanManageClanJobs |
			                    ClanPrivilegeType.CanManageClanProperty |
			                    ClanPrivilegeType.CanManageEconomicZones |
			                    ClanPrivilegeType.CanManageBankAccounts | ClanPrivilegeType.CanCreateAppointments |
			                    ClanPrivilegeType.CanDemote | ClanPrivilegeType.CanCastout |
			                    ClanPrivilegeType.CanChangeRankPath |
			                    ClanPrivilegeType.CanSubmitClan)
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = appointment, Order = 0, Abbreviation = "Cnsl" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = appointment, Order = 0, Title = "Consul" });
		context.Appointments.Add(appointment);
		context.SaveChanges();

		appointment = new Appointment
		{
			Name = "Censor",
			Clan = cityClan,
			MaximumSimultaneousHolders = 0,
			MinimumRank = senatorRank,
			Privileges = (long)ClanPrivilegeType.None
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = appointment, Order = 0, Abbreviation = "Cnsr" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = appointment, Order = 0, Title = "Censor" });
		context.Appointments.Add(appointment);
		context.SaveChanges();

		appointment = new Appointment
		{
			Name = "Tribunus Plebis",
			Clan = cityClan,
			MaximumSimultaneousHolders = 0,
			MinimumRank = senatorRank,
			Privileges = (long)ClanPrivilegeType.None
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = appointment, Order = 0, Abbreviation = "Trbn Plb" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = appointment, Order = 0, Title = "Tribunus Plebis" });
		context.Appointments.Add(appointment);
		context.SaveChanges();

		appointment = new Appointment
		{
			Name = "Princeps Senatus",
			Clan = cityClan,
			MaximumSimultaneousHolders = 1,
			MinimumRank = senatorRank,
			Privileges = (long)(ClanPrivilegeType.CanDemote | ClanPrivilegeType.CanCastout |
			                    ClanPrivilegeType.CanChangeRankPath)
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = appointment, Order = 0, Abbreviation = "Prncps Snts" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = appointment, Order = 0, Title = "Princeps Senatus" });
		context.Appointments.Add(appointment);

		appointment = new Appointment
		{
			Name = "Dictator",
			Clan = cityClan,
			MaximumSimultaneousHolders = 1,
			MinimumRank = senatorRank,
			Privileges = (long)(ClanPrivilegeType.CanViewTreasury | ClanPrivilegeType.CanManageClanJobs |
			                    ClanPrivilegeType.CanManageClanProperty |
			                    ClanPrivilegeType.CanManageEconomicZones |
			                    ClanPrivilegeType.CanManageBankAccounts | ClanPrivilegeType.CanCreateAppointments |
			                    ClanPrivilegeType.CanDemote | ClanPrivilegeType.CanCastout |
			                    ClanPrivilegeType.CanChangeRankPath |
			                    ClanPrivilegeType.CanSubmitClan)
		};
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = appointment, Order = 0, Abbreviation = "Dctr", FutureProg = isMaleProg });
		appointment.AppointmentsAbbreviations.Add(new AppointmentsAbbreviations
			{ Appointment = appointment, Order = 1, Abbreviation = "Dctrx" });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = appointment, Order = 0, Title = "Dictator", FutureProg = isMaleProg });
		appointment.AppointmentsTitles.Add(new AppointmentsTitles
			{ Appointment = appointment, Order = 0, Title = "Dictatrix" });
		context.Appointments.Add(appointment);

		context.SaveChanges();
	}
}