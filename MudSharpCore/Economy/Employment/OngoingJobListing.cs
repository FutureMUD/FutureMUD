using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using MoreLinq.Extensions;
using MudSharp.Character;
using MudSharp.Community;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Models;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Intervals;
using MudSharp.TimeAndDate.Listeners;
using MudSharp.TimeAndDate.Time;
using MudSharp.Work.Projects.ConcreteTypes;
using NCalc;

namespace MudSharp.Economy.Employment;
#nullable enable
public class OngoingJobListing : JobListingBase, IOngoingJobListing
{
	public RecurringInterval PayInterval { get; protected set; }
	public MudDateTime PayReference { get; protected set; }
	public ICurrency PayCurrency { get; protected set; }
	public ExpressionEngine.Expression PayExpression { get; protected set; }

	public OngoingJobListing(JobListing dbitem, IFuturemud gameworld) : base(dbitem, gameworld)
	{
		var definition = XElement.Parse(dbitem.Definition);
		PayInterval = RecurringInterval.Parse(definition.Element("PayInterval")!.Value);
		PayCurrency = Gameworld.Currencies.Get(long.Parse(definition.Element("PayCurrency")!.Value))!;
		PayExpression = new ExpressionEngine.Expression(definition.Element("PayExpression")!.Value,
			EvaluateOptions.IgnoreCase);
		PayReference = new MudDateTime(definition.Element("PayReference")!.Value, Gameworld);

		foreach (var dbjob in dbitem.ActiveJobs)
		{
			var job = new ActiveOngoingJob(dbjob, this, Gameworld);
			Gameworld.Add(job);
			_activeJobs.Add(job);
		}

		RegisterEvents();
	}

	public OngoingJobListing(IFuturemud gameworld, string name, IEconomicZone economicZone, IFrameworkItem employer,
		ICurrency currency)
		: base(gameworld, name, economicZone, employer,
			$"<Definition><PayInterval>every 1 week</PayInterval><PayCurrency>{currency.Id}</PayCurrency><PayReference>{economicZone.FinancialPeriodReferenceCalendar.CurrentDateTime}</PayReference><PayExpression>{gameworld.GetStaticConfiguration("OngoingJobPerformanceDefaultPayFormula")}</PayExpression></Definition>",
			"ongoing")
	{
		PayCurrency = currency;
		PayInterval = new RecurringInterval { IntervalAmount = 1, Modifier = 0, Type = IntervalType.Weekly };
		PayReference = economicZone.FinancialPeriodReferenceCalendar.CurrentDateTime;
		PayExpression = new ExpressionEngine.Expression(
			Gameworld.GetStaticConfiguration("OngoingJobPerformanceDefaultPayFormula"), EvaluateOptions.IgnoreCase);
		RegisterEvents();
	}

	public void RegisterEvents()
	{
		Gameworld.HeartbeatManager.FuzzyThirtySecondHeartbeat -= DoJobHeartbeat;
		Gameworld.HeartbeatManager.FuzzyThirtySecondHeartbeat += DoJobHeartbeat;
		var referenceTime = PayInterval.GetNextAdjacentToCurrent(PayReference);
		if (!PayReference.Equals(referenceTime))
		{
			PayReference = referenceTime;
			Changed = true;
		}
	}

	public void CheckPayroll()
	{
		if (PayReference.Calendar.CurrentDateTime >= PayReference)
		{
			foreach (var job in ActiveJobs)
			{
				PartialPayrollForJob(job, 1.0);
			}

			PayReference = PayInterval.GetNextDateTime(PayReference);
			Changed = true;
		}
	}

	private void PartialPayrollForJob(IActiveJob job, double portion)
	{
		if (job.IsJobComplete)
		{
			return;
		}

		var pay = PayExpression.EvaluateDecimalWith(("effort", job.CurrentPerformance)) * (decimal)portion;
		job.CurrentPerformance = 0.0;
		if (pay <= 0.0M)
		{
			return;
		}

		if (MoneyPaidIn[PayCurrency] >= pay)
		{
			MoneyPaidIn[PayCurrency] -= pay;
			job.RevenueEarned[PayCurrency] += pay;
			pay = 0.0M;
		}
		else if (MoneyPaidIn[PayCurrency] > 0.0M)
		{
			job.RevenueEarned[PayCurrency] += MoneyPaidIn[PayCurrency];
			pay -= MoneyPaidIn[PayCurrency];
			MoneyPaidIn[PayCurrency] = 0.0M;
		}

		if (pay > 0)
		{
			if (BankAccount is not null)
			{
				var max = BankAccount.MaximumWithdrawal();
				if (max >= pay)
				{
					job.RevenueEarned[PayCurrency] += pay;
					BankAccount.WithdrawFromTransaction(pay, $"Payroll for job '{Name}'");
					pay = 0.0M;
				}
				else if (max > 0.0M)
				{
					job.RevenueEarned[PayCurrency] += max;
					BankAccount.WithdrawFromTransaction(max, $"Payroll for job '{Name}'");
					pay -= max;
				}
			}

			if (pay > 0)
			{
				job.BackpayOwed[PayCurrency] += pay;
			}
		}
	}

	private void DoJobLifecycleCheck()
	{
		var now = PayReference.Calendar.CurrentDateTime;
		foreach (var job in ActiveJobs.ToList())
		{
			if (job.IsJobComplete)
			{
				if (job.BackpayOwed.All(x => x.Value <= 0.0M) && job.RevenueEarned.All(x => x.Value <= 0.0M))
				{
					job.Character.RemoveJob(job);
					_activeJobs.Remove(job);
					job.Delete();
				}

				if ((now - job.JobEnded).TotalDays >= Gameworld.GetStaticDouble("JobFinishedAutoDeleteDays") &&
				    job.BackpayOwed.All(x => x.Value <= 0.0M))
				{
					MoneyPaidIn.Add(job.RevenueEarned);
					job.Character.RemoveJob(job);
					_activeJobs.Remove(job);
					job.Delete();
				}

				continue;
			}

			if (job.JobDueToEnd is not null && job.JobDueToEnd <= now)
			{
				job.FinishFixedTerm();
				continue;
			}

			foreach (var item in job.BackpayOwed.ToList())
			{
				var owed = item.Value;
				if (owed <= 0.0M)
				{
					continue;
				}

				if (MoneyPaidIn[item.Key] >= 0.0M)
				{
					if (MoneyPaidIn[item.Key] >= owed)
					{
						job.BackpayOwed[item.Key] = 0.0M;
						job.RevenueEarned[item.Key] += owed;
						MoneyPaidIn[item.Key] -= owed;
						continue;
					}
					else
					{
						var amount = MoneyPaidIn[item.Key];
						job.BackpayOwed[item.Key] -= amount;
						job.RevenueEarned[item.Key] += amount;
						MoneyPaidIn[item.Key] -= amount;
						owed -= amount;
					}

					job.Changed = true;
					Changed = true;
				}

				if (BankAccount is not null && BankAccount.Currency == item.Key)
				{
					var max = BankAccount.MaximumWithdrawal();
					if (max >= owed)
					{
						job.RevenueEarned[item.Key] += owed;
						job.BackpayOwed[item.Key] -= owed;
						BankAccount.WithdrawFromTransaction(owed, $"Payroll for job '{Name}'");
					}
					else if (max > 0.0M)
					{
						job.RevenueEarned[item.Key] += max;
						job.BackpayOwed[item.Key] -= max;
						BankAccount.WithdrawFromTransaction(max, $"Payroll for job '{Name}'");
					}
				}
			}
		}

		if (!ActiveJobs.Any() && IsArchived)
		{
			Delete();
		}
	}

	private void DoJobHeartbeat()
	{
		DoJobLifecycleCheck();
		var decay = Gameworld.GetStaticDouble("OngoingJobPerformanceDecayPerHour") / 120.0;
		foreach (var job in ActiveJobs.ToList())
		{
			if (job.IsJobComplete)
			{
				continue;
			}

			job.CurrentPerformance -= decay;

			if (job.CurrentPerformance < 0.0)
			{
				job.CurrentPerformance = 0.0;
			}
		}

		CheckPayroll();
	}

	public void RemoveEvents()
	{
		Gameworld.HeartbeatManager.FuzzyThirtySecondHeartbeat -= DoJobHeartbeat;
	}

	protected override void OnDelete()
	{
		RemoveEvents();
	}

	#region Overrides of JobListingBase

	public override void FinishJob()
	{
		IsArchived = true;
		var portion =
			(PayInterval.GetNextDateTime(PayReference) - PayReference.Calendar.CurrentDateTime).TotalDays /
			PayInterval.DaysPerInterval(PayReference.Calendar).IfZero(1.0);
		if (portion > 0.05)
		{
			foreach (var job in ActiveJobs)
			{
				PartialPayrollForJob(job, portion);
			}
		}

		foreach (var job in ActiveJobs)
		{
			job.FinishFixedTerm(); // TODO - redundancy?
		}
	}

	protected override string ShowAddendum(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine();
		sb.AppendLine("Salary Details".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldYellow));
		sb.AppendLine();
		sb.AppendLine($"Pay Currency: {PayCurrency.Name.ColourValue()}");
		sb.AppendLine($"Pay Interval: {PayInterval.Describe(PayReference.Calendar).ColourValue()}");
		sb.AppendLine(
			$"Pay Date: {PayReference.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Immortal).ColourValue()}");
		sb.AppendLine($"Pay Expression: {PayExpression.OriginalExpression.ColourValue()}");
		return sb.ToString();
	}

	protected override string ShowToPlayerAddendum(ICharacter actor)
	{
		return $@"This is an offer of ongoing employment.
You will be paid {PayDescriptionForJobListing()}.{(PersonalProject is not null ? $"\nYou will be required to perform the {PersonalProject.Name.ColourName()} project for job performance." : "")}";
	}

	protected override string HelpInfo => $@"{base.HelpInfo}
	#3projects#0 - lists which projects you can attach to this job
	#3project <which>#0 - sets a personal project associated with this job
	#3project none#0 - clears a project requirement
	#3currency <currency>#0 - sets the currency that this job pays in
	#3interval <interval>#0 - sets the interval that pay is paid out in
	#3payday <date>#0 - sets a reference date for the next payday
	#3pay <expression>#0 - sets an expression for pay
	#3easypay ""<normal amount>"" ""<potential bonus>""#0 - sets up pay without having to know the base currency amount

#1Note: Use the variable #6effort#1 in the pay expression for job project effort.#0";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "currency":
				return BuildingCommandCurrency(actor, command);
			case "interval":
				return BuildingCommandInterval(actor, command);
			case "pay":
			case "amount":
			case "expression":
			case "expr":
				return BuildingCommandExpression(actor, command);
			case "date":
			case "payday":
				return BuildingCommandDate(actor, command);
			case "easypay":
				return BuildingCommandEasyPay(actor, command);
			case "project":
				return BuildingCommandProject(actor, command);
			case "projects":
			case "projectlist":
			case "listprojects":
				return BuildingCommandProjects(actor, command);
		}

		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandProjects(ICharacter actor, StringStack command)
	{
		var sb = new StringBuilder();
		sb.AppendLine("The following projects can be added to your job:");
		sb.AppendLine();
		sb.AppendLine(StringUtilities.GetTextTable(
			from project in Gameworld.Projects.Where(x => x.AppearInJobsList && x.Status == RevisionStatus.Current)
			select new List<string>
			{
				project.Id.ToString("N0", actor),
				project.Name,
				project.Tagline
			},
			new List<string>
			{
				"Id",
				"Name",
				"Tagline"
			},
			actor,
			Telnet.Green
		));
		actor.OutputHandler.Send(sb.ToString());
		return true;
	}

	private bool BuildingCommandProject(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must either specify a project template that employees must perform, or use {"none".ColourCommand()} to not require any.");
			return false;
		}

		if (ActiveJobs.Any(x => !x.IsJobComplete))
		{
			actor.OutputHandler.Send(
				"This setting cannot be changed while you have active workers. You should either make a new job listing or terminate all existing employment with this one.");
			return false;
		}

		if (command.PeekSpeech().EqualToAny("none", "delete", "remove", "clear"))
		{
			PersonalProject = null;
			Changed = true;
			actor.OutputHandler.Send(
				$"This job will no longer have a personal project generated for job performance.");
			return true;
		}

		var projects = Gameworld.Projects.Where(x => x.AppearInJobsList && x.Status == RevisionStatus.Current)
		                        .ToList();
		var project = projects.GetByIdOrName(command.SafeRemainingArgument);
		if (project is null)
		{
			actor.OutputHandler.Send(
				$"There is no such project for you to use. See {"job set projects".MXPSend("job set projects")} for a list of projects.");
			return false;
		}

		PersonalProject = project;
		Changed = true;
		actor.OutputHandler.Send(
			$"Employees working in this job will now get a personal project based on {project.Name.ColourName()} to work on.");
		return true;
	}

	private bool BuildingCommandEasyPay(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How much should the base pay for this job be, without any extra effort?");
			return false;
		}

		if (!PayCurrency.TryGetBaseCurrency(command.PopSpeech(), out var baseAmount) || baseAmount < 0.0M)
		{
			actor.OutputHandler.Send(
				$"The text {command.Last.ColourCommand()} is not a valid amount of {PayCurrency.Name.ColourValue()}.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How much should the bonus pay for this job be, if they put in extra effort?");
			return false;
		}

		if (!PayCurrency.TryGetBaseCurrency(command.SafeRemainingArgument, out var bonusAmount) ||
		    bonusAmount < 0.0M)
		{
			actor.OutputHandler.Send(
				$"The text {command.Last.ColourCommand()} is not a valid amount of {PayCurrency.Name.ColourValue()}.");
			return false;
		}

		if (ActiveJobs.Any(x => !x.IsJobComplete))
		{
			actor.OutputHandler.Send(
				"This setting cannot be changed while you have active workers. You should either make a new job listing or terminate all existing employment with this one.");
			return false;
		}

		if (bonusAmount == 0.0M)
		{
			PayExpression = new ExpressionEngine.Expression($"{baseAmount:F}");
		}
		else
		{
			PayExpression =
				new ExpressionEngine.Expression($"{baseAmount:F} + (min(1.0, effort / 1000.0) * {bonusAmount:F})");
		}

		actor.OutputHandler.Send(
			$"This job will now pay {PayCurrency.Describe(baseAmount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} with a potential bonus of {PayCurrency.Describe(bonusAmount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}.\nThis translates to the following formula: {PayExpression.OriginalExpression.ColourName()}");
		Changed = true;
		return true;
	}

	private bool BuildingCommandDate(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What date and time do you want to use as a reference for the payday?");
			return false;
		}

		if (!MudDateTime.TryParse(command.SafeRemainingArgument, EconomicZone.FinancialPeriodReferenceCalendar,
			    EconomicZone.FinancialPeriodReferenceClock, out var dt))
		{
			var date = PayReference.Date;
			var time = PayReference.Time;
			var tz = PayReference.TimeZone;
			actor.OutputHandler.Send(
				$"That is not a valid date and time for the {EconomicZone.FinancialPeriodReferenceCalendar.FullName.ColourName()} calendar and {EconomicZone.FinancialPeriodReferenceClock.Name.ColourName()} clock.\nValid input is in this format: {"<day>/<month name>/<year> <timezone> <hours>:<minutes>:<seconds>".ColourCommand()}\nFor example, this is how you would enter the current reference date: {$"{date.Day.ToString("N0", actor)}/{date.Month.Alias}/{date.Year} {tz.Name} {time.Hours.ToString("N0", actor)}:{time.Minutes.ToString("N0", actor)}:{time.Seconds.ToString("N0", actor)}".ColourCommand()}");
			return false;
		}

		if (ActiveJobs.Any(x => !x.IsJobComplete))
		{
			actor.OutputHandler.Send(
				"This setting cannot be changed while you have active workers. You should either make a new job listing or terminate all existing employment with this one.");
			return false;
		}

		var referenceTime = PayInterval.GetNextAdjacentToCurrent(dt);
		PayReference = referenceTime;
		Changed = true;
		if (referenceTime != dt)
		{
			actor.OutputHandler.Send(
				$"The next payroll will now occur on {referenceTime.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()}. This is the next actual date based on your supplied reference date and interval.");
		}
		else
		{
			actor.OutputHandler.Send(
				$"The next payroll will now occur on {referenceTime.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()}.");
		}

		return true;
	}

	private bool BuildingCommandCurrency(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which currency do you want to change this job to pay in?");
			return false;
		}

		var currency = Gameworld.Currencies.GetByIdOrName(command.SafeRemainingArgument);
		if (currency is null)
		{
			actor.OutputHandler.Send("There is no such currency.");
			return false;
		}

		if (ActiveJobs.Any(x => !x.IsJobComplete))
		{
			actor.OutputHandler.Send(
				"This setting cannot be changed while you have active workers. You should either make a new job listing or terminate all existing employment with this one.");
			return false;
		}

		PayCurrency = currency;
		actor.OutputHandler.Send($"This job will now pay in the {currency.Name.ColourValue()} currency.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandInterval(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What interval do you want to set for payroll? Use an expression like {"every <x> hours|days|weekdays|weeks|months|years <offset>".ColourCommand()}.");
			return false;
		}

		if (!RecurringInterval.TryParse(command.SafeRemainingArgument, out var interval))
		{
			actor.OutputHandler.Send(
				$"That is not a valid interval. Use an expression like {"every <x> hours|days|weekdays|weeks|months|years <offset>".ColourCommand()}.");
			return false;
		}

		if (ActiveJobs.Any(x => !x.IsJobComplete))
		{
			actor.OutputHandler.Send(
				"This setting cannot be changed while you have active workers. You should either make a new job listing or terminate all existing employment with this one.");
			return false;
		}

		PayInterval = interval;
		var referenceTime = PayInterval.GetNextAdjacentToCurrent(PayReference);
		PayReference = referenceTime;
		Changed = true;
		actor.OutputHandler.Send(
			$"The payroll now occurs on an interval of {PayInterval.Describe(PayReference.Calendar).ColourValue()}. The next payroll will now occur on {referenceTime.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandExpression(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What do you want the expression to be for pay? Hint: You can use the 'effort' variable for the contribution of the job's effort prog.");
			return false;
		}

		var expr = new ExpressionEngine.Expression(command.SafeRemainingArgument, EvaluateOptions.IgnoreCase);
		if (expr.HasErrors())
		{
			actor.OutputHandler.Send(expr.Error);
			return false;
		}

		foreach (var parameter in expr.Parameters)
		{
			if (parameter.Key.EqualTo("effort"))
			{
				continue;
			}

			actor.OutputHandler.Send(
				$"The variable {parameter.Key.ColourName()} from your expression is not valid.");
			return false;
		}

		if (ActiveJobs.Any(x => !x.IsJobComplete))
		{
			actor.OutputHandler.Send(
				"This setting cannot be changed while you have active workers. You should either make a new job listing or terminate all existing employment with this one.");
			return false;
		}

		PayExpression = expr;
		actor.OutputHandler.Send(
			$"The pay expression for this job will now be {expr.OriginalExpression.ColourName()}.");
		Changed = true;
		return true;
	}

	public override string PayDescriptionForJobListing()
	{
		var minimum = Convert.ToDecimal(PayExpression.EvaluateWith(("effort", 0.0M)));
		var maximum = Convert.ToDecimal(PayExpression.EvaluateWith(("effort", 1000000.0M)));
		if (minimum != maximum)
		{
			return
				$"between {PayCurrency.Describe(minimum, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} and {PayCurrency.Describe(maximum, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} {PayInterval.Describe(PayReference.Calendar)}";
		}

		return
			$"{PayCurrency.Describe(minimum, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} {PayInterval.Describe(PayReference.Calendar)}";
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("PayInterval", PayInterval.ToString()),
			new XElement("PayCurrency", PayCurrency.Id),
			new XElement("PayExpression", new XCData(PayExpression.OriginalExpression)),
			new XElement("PayReference", PayReference.GetDateTimeString())
		);
	}

	public override IActiveJob ApplyForJob(ICharacter actor)
	{
		var endingDate = MaximumDuration is not null
			? EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime + MaximumDuration
			: default;
		var project = PersonalProject is not null ? new ActivePersonalProject(PersonalProject, actor) : default;
		if (project is not null)
		{
			actor.AddPersonalProject(project);
			Gameworld.Add(project);
		}

		var hasClan = actor.ClanMemberships.Any(x => !x.IsArchivedMembership && x.Clan == ClanMembership);
		if (ClanMembership is not null)
		{
			IClanMembership? membership = null;
			membership = ClanMembership.Memberships.FirstOrDefault(x => x.MemberId == actor.Id);
			if (membership is null)
			{
				var rank = ClanRank ?? ClanMembership.Ranks.OrderBy(x => x.RankNumber).First();
				if (ClanAppointment is not null &&
				    ClanAppointment.MinimumRankToHold.RankNumber > rank.RankNumber)
				{
					rank = ClanAppointment.MinimumRankToHold;
				}

				var paygrade = ClanPaygrade ?? rank.Paygrades.OrderBy(x => x.PayAmount).FirstOrDefault();
				using (new FMDB())
				{
					var dbitem = new Models.ClanMembership
					{
						CharacterId = actor.Id,
						ClanId = ClanMembership.Id,
						RankId = rank.Id,
						PaygradeId = paygrade?.Id,
						PersonalName = actor.CurrentName.SaveToXml().ToString(),
						JoinDate = ClanMembership.Calendar.CurrentDate.GetDateString(),
						ManagerId = null
					};
					FMDB.Context.ClanMemberships.Add(dbitem);
					if (ClanAppointment is not null)
					{
						dbitem.ClanMembershipsAppointments.Add(new ClanMembershipsAppointments
						{
							ClanMembership = dbitem,
							CharacterId = actor.Id,
							AppointmentId = ClanAppointment.Id
						});
					}

					FMDB.Context.SaveChanges();
					var newMembership = new Community.ClanMembership(dbitem, ClanMembership, Gameworld);
					actor.AddMembership(newMembership);
					ClanMembership.Memberships.Add(newMembership);
				}
			}
			else if (membership.IsArchivedMembership)
			{
				membership.IsArchivedMembership = false;
				if (!actor.ClanMemberships.Contains(membership))
				{
					actor.AddMembership(membership);
				}

				membership.SetRank(ClanRank ?? ClanMembership.Ranks.OrderBy(x => x.RankNumber).First());
				if (membership.Paygrade is not null)
				{
					membership.Paygrade = ClanPaygrade;
				}

				if (ClanAppointment is not null)
				{
					membership.AppointToPosition(ClanAppointment);
				}

				membership.JoinDate = ClanMembership.Calendar.CurrentDate;
				membership.Changed = true;
			}
			else
			{
				if (ClanAppointment is not null)
				{
					if (!membership.Appointments.Contains(ClanAppointment))
					{
						membership.AppointToPosition(ClanAppointment);
					}
				}

				if (ClanRank is not null)
				{
					if (ClanRank.RankNumber > membership.Rank.RankNumber)
					{
						membership.SetRank(ClanRank);
						if (ClanPaygrade is not null)
						{
							membership.Paygrade = ClanPaygrade;
						}
					}
					else
					{
						if (ClanPaygrade is not null && ClanRank == membership.Rank &&
						    ClanPaygrade.PayAmount > (membership.Paygrade?.PayAmount ?? 0.0M))
						{
							membership.Paygrade = ClanPaygrade;
						}
					}
				}

				membership.JoinDate = ClanMembership.Calendar.CurrentDate;
				membership.Changed = true;
			}
		}

		var job = new ActiveOngoingJob(this, actor,
			EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime,
			endingDate, hasClan, project);
		_activeJobs.Add(job);
		Gameworld.Add(job);
		actor.AddJob(job);
		job.BeginJob();
		return job;
	}

	/// <inheritdoc />
	protected override void BaseClassNetFinancialPositionAdditions(DecimalCounter<ICurrency> counter)
	{
		if (counter.ContainsKey(PayCurrency))
		{
			return;
		}

		counter[PayCurrency] = 0.0M;
	}

	#endregion
}