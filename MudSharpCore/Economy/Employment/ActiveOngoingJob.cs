using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Community;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Models;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using MudSharp.Work.Projects;

namespace MudSharp.Economy.Employment;
#nullable enable
public class ActiveOngoingJob : ActiveJobBase
{
	public ActiveOngoingJob(ActiveJob dbitem, IJobListing listing, IFuturemud gameworld) : base(dbitem, listing,
		gameworld)
	{
		CurrentPerformance = dbitem.CurrentPerformance;
	}

	public ActiveOngoingJob(IJobListing listing, ICharacter character, MudDateTime commenced, MudDateTime? ending,
		bool alreadyHadClanPosition, IActiveProject? activeProject) : base(listing, character, commenced, ending,
		alreadyHadClanPosition, activeProject)
	{
	}

	#region Overrides of ActiveJobBase

	protected override void Save(ActiveJob dbitem)
	{
		dbitem.CurrentPerformance = CurrentPerformance;
	}

	public override void BeginJob()
	{
		if (!Character.IsPlayerCharacter || Character.IsGuest)
		{
			return;
		}

		var sb = new StringBuilder();
		sb.Append($"Today I began my job as {Name.A_An()}.");
		if (Listing.Employer is ICharacter ch)
		{
			sb.Append($" I worked for {ch.PersonalName.GetName(NameStyle.FullName)}.");
		}
		else if (Listing.Employer is IClan clan)
		{
			sb.Append($" I worked for the clan {clan.FullName}.");
		}

		if (JobDueToEnd is not null)
		{
			sb.Append(
				$" This is a fixed term engagement which is due to end on {JobDueToEnd.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short)}.");
		}

		using (new FMDB())
		{
			var note = new AccountNote
			{
				Text = sb.ToString(),
				AccountId = Character.Account.Id,
				AuthorId = Character.Account.Id,
				Subject = $"Began The Job '{Name}'",
				TimeStamp = DateTime.UtcNow,
				IsJournalEntry = true,
				InGameTimeStamp = Listing.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime
				                         .GetDateTimeString(),
				CharacterId = Character.Id
			};
			FMDB.Context.AccountNotes.Add(note);
			FMDB.Context.SaveChanges();
		}
	}

	public override void QuitJob()
	{
		EndEmployment();
		if (!Character.IsPlayerCharacter || Character.IsGuest)
		{
			return;
		}

		var sb = new StringBuilder();
		var duration = Listing.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime - JobCommenced;
		sb.Append($"Today I left my job as {Name.A_An()}.");
		if (Listing.Employer is ICharacter ch)
		{
			sb.Append($" I worked for {ch.PersonalName.GetName(NameStyle.FullName)}.");
		}
		else if (Listing.Employer is IClan clan)
		{
			sb.Append($" I worked for the clan {clan.FullName}.");
		}

		sb.AppendLine($" I had held the job for {duration.Describe(Character)}.");

		using (new FMDB())
		{
			var note = new AccountNote
			{
				Text = sb.ToString(),
				AccountId = Character.Account.Id,
				AuthorId = Character.Account.Id,
				Subject = $"Quit The Job '{Name}'",
				TimeStamp = DateTime.UtcNow,
				IsJournalEntry = true,
				InGameTimeStamp = Listing.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime
				                         .GetDateTimeString(),
				CharacterId = Character.Id
			};
			FMDB.Context.AccountNotes.Add(note);
			FMDB.Context.SaveChanges();
		}
	}

	public override void FireFromJob()
	{
		EndEmployment();
		if (!Character.IsPlayerCharacter || Character.IsGuest)
		{
			return;
		}

		var sb = new StringBuilder();
		var duration = Listing.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime - JobCommenced;
		sb.Append($"Today I was fired from my job as {Name.A_An()}.");
		if (Listing.Employer is ICharacter ch)
		{
			sb.Append($" I worked for {ch.PersonalName.GetName(NameStyle.FullName)}.");
		}
		else if (Listing.Employer is IClan clan)
		{
			sb.Append($" I worked for the clan {clan.FullName}.");
		}

		sb.AppendLine($" I had held the job for {duration.Describe(Character)}.");

		using (new FMDB())
		{
			var note = new AccountNote
			{
				Text = sb.ToString(),
				AccountId = Character.Account.Id,
				AuthorId = Character.Account.Id,
				Subject = $"Fired From The Job '{Name}'",
				TimeStamp = DateTime.UtcNow,
				IsJournalEntry = true,
				InGameTimeStamp = Listing.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime
				                         .GetDateTimeString(),
				CharacterId = Character.Id
			};
			FMDB.Context.AccountNotes.Add(note);
			FMDB.Context.SaveChanges();
		}
	}

	public override void FinishFixedTerm()
	{
		EndEmployment();
		if (!Character.IsPlayerCharacter || Character.IsGuest)
		{
			return;
		}

		var sb = new StringBuilder();
		var duration = Listing.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime - JobCommenced;
		sb.Append($"Today I finished my tenure as {Name.A_An()}.");
		if (Listing.Employer is ICharacter ch)
		{
			sb.Append($" I worked for {ch.PersonalName.GetName(NameStyle.FullName)}.");
		}
		else if (Listing.Employer is IClan clan)
		{
			sb.Append($" I worked for the clan {clan.FullName}.");
		}

		sb.AppendLine($" I had held the job for {duration.Describe(Character)}.");

		using (new FMDB())
		{
			var note = new AccountNote
			{
				Text = sb.ToString(),
				AccountId = Character.Account.Id,
				AuthorId = Character.Account.Id,
				Subject = $"Finished The Job '{Name}'",
				TimeStamp = DateTime.UtcNow,
				IsJournalEntry = true,
				InGameTimeStamp = Listing.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime
				                         .GetDateTimeString(),
				CharacterId = Character.Id
			};
			FMDB.Context.AccountNotes.Add(note);
			FMDB.Context.SaveChanges();
		}
	}

	protected override void EndEmployment()
	{
		base.EndEmployment();
		if (Listing.PersonalProject is not null)
		{
			var active =
				Character.PersonalProjects.FirstOrDefault(x => x.ProjectDefinition == Listing.PersonalProject);
			if (active is not null)
			{
				active.Cancel(Character);
			}
		}
	}

	public override DecimalCounter<ICurrency> DailyPay()
	{
		var job = (IOngoingJobListing)Listing;
		var pay = new DecimalCounter<ICurrency>();
		pay[job.PayCurrency] += job.PayExpression.EvaluateDecimalWith(("effort", CurrentPerformance)) /
		                        (decimal)job.PayInterval.DaysPerInterval(job.PayReference.Calendar);
		return pay;
	}

	#endregion
}