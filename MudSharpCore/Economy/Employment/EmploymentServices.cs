using MudSharp.Economy;
using MudSharp.Economy.Employment;

#nullable enable

namespace MudSharp.Economy.Employment;

public static class EmploymentCandidateMatcher
{
	public static IReadOnlySet<EmploymentAICapability> ImplicitCapabilitiesForRole(EmploymentRole role)
	{
		return role == EmploymentRole.Manager
			? new HashSet<EmploymentAICapability> { EmploymentAICapability.CanManageEmploymentHost }
			: new HashSet<EmploymentAICapability>();
	}

	public static bool IsMatch(IJobOpening opening, EmploymentCandidateProfile candidate, out string reason)
	{
		foreach (var capability in ImplicitCapabilitiesForRole(opening.Role))
		{
			if (candidate.Capabilities.Contains(capability))
			{
				continue;
			}

			reason = $"Candidate lacks required AI capability {capability} for the {opening.Role} role.";
			return false;
		}

		foreach (var requirement in opening.Requirements.Skills)
		{
			if (!TryGetSkill(candidate, requirement.SkillName, out var value) || value < requirement.MinimumValue)
			{
				reason = $"Candidate lacks required skill {requirement.SkillName}.";
				return false;
			}
		}

		foreach (var requirement in opening.Requirements.Knowledges)
		{
			if (!ContainsCaseInsensitive(candidate.Knowledges, requirement.KnowledgeName))
			{
				reason = $"Candidate lacks required knowledge {requirement.KnowledgeName}.";
				return false;
			}
		}

		foreach (var requirement in opening.Requirements.Capabilities)
		{
			if (!candidate.Capabilities.Contains(requirement.Capability))
			{
				reason = $"Candidate lacks required AI capability {requirement.Capability}.";
				return false;
			}
		}

		foreach (var requirement in opening.Requirements.Tags)
		{
			if (!ContainsCaseInsensitive(candidate.Tags, requirement.TagName))
			{
				reason = $"Candidate lacks required tag {requirement.TagName}.";
				return false;
			}
		}

		if (!candidate.AcceptedPaymentMethods.Contains(opening.PaymentMethod.MethodKind))
		{
			reason = $"Candidate does not accept payment by {opening.PaymentMethod.MethodKind}.";
			return false;
		}

		if (opening.Compensation.Cadence != PayCadence.Unpaid &&
		    IsBelowReservationWage(opening, candidate))
		{
			reason = "Offer is below the candidate's reservation wage.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	private static bool TryGetSkill(EmploymentCandidateProfile candidate, string skillName, out double value)
	{
		if (candidate.Skills.TryGetValue(skillName, out value))
		{
			return true;
		}

		foreach (var skill in candidate.Skills)
		{
			if (!skill.Key.Equals(skillName, StringComparison.InvariantCultureIgnoreCase))
			{
				continue;
			}

			value = skill.Value;
			return true;
		}

		value = 0.0;
		return false;
	}

	private static bool ContainsCaseInsensitive(IEnumerable<string> values, string value)
	{
		return values.Any(x => x.Equals(value, StringComparison.InvariantCultureIgnoreCase));
	}

	private static bool IsBelowReservationWage(IJobOpening opening, EmploymentCandidateProfile candidate)
	{
		if (candidate.ReservationWage <= 0.0M)
		{
			return false;
		}

		var offered = EmploymentCompensationEvaluator.EffectiveHourlyRate(opening.Compensation);
		if (offered is not null && candidate.ReservationWageCurrency is not null)
		{
			var offeredGlobal = offered.Amount * offered.Currency.BaseCurrencyToGlobalBaseCurrencyConversion;
			var reservationGlobal = candidate.ReservationWage *
			                        candidate.ReservationWageCurrency.BaseCurrencyToGlobalBaseCurrencyConversion;
			return offeredGlobal < reservationGlobal;
		}

		return (offered?.Amount ?? EmploymentCompensationEvaluator.EffectiveHourlyAmount(opening.Compensation)) <
		       candidate.ReservationWage;
	}
}

public static class EmploymentCompensationEvaluator
{
	private const decimal DefaultDailyHours = 8.0M;
	private const decimal DefaultWeeklyHours = 40.0M;
	private const decimal DefaultSalaryHours = DefaultDailyHours * 30.0M;

	public static MoneyAmount? EffectiveHourlyRate(CompensationTerms compensation)
	{
		var amount = compensation.FixedRate ?? compensation.MinimumEffectivePay;
		return amount is null
			? null
			: amount with { Amount = EffectiveHourlyAmount(amount.Amount, compensation.Cadence) };
	}

	public static decimal EffectiveHourlyAmount(CompensationTerms compensation)
	{
		return EffectiveHourlyRate(compensation)?.Amount ?? 0.0M;
	}

	public static decimal EffectiveHourlyGlobalAmount(CompensationTerms compensation)
	{
		var rate = EffectiveHourlyRate(compensation);
		return rate is null
			? 0.0M
			: rate.Amount * rate.Currency.BaseCurrencyToGlobalBaseCurrencyConversion;
	}

	public static decimal EffectiveHourlyAmount(decimal amount, PayCadence cadence)
	{
		return cadence switch
		{
			PayCadence.Unpaid => 0.0M,
			PayCadence.Hourly => amount,
			PayCadence.Daily => amount / DefaultDailyHours,
			PayCadence.Weekly => amount / DefaultWeeklyHours,
			PayCadence.Salary => amount / DefaultSalaryHours,
			PayCadence.PerTask or PayCadence.Commission or PayCadence.Mixed => amount,
			_ => amount
		};
	}
}

public sealed record EmploymentPaymentSelection(bool Success, PaymentMethod? PaymentMethod, string Reason);

public static class EmploymentPaymentSelector
{
	public static EmploymentPaymentSelection SelectPaymentMethod(IJobOpening opening,
		EmploymentCandidateProfile candidate, IBankAccount? preferredEmployeeAccount = null)
	{
		var advertised = opening.PaymentMethod;
		if (advertised.MethodKind == PaymentMethodKind.EmployeeBankAccount && preferredEmployeeAccount is not null &&
		    candidate.AcceptedPaymentMethods.Contains(PaymentMethodKind.EmployeeBankAccount))
		{
			return new EmploymentPaymentSelection(true, advertised with { BankAccount = preferredEmployeeAccount },
				"Selected the candidate's preferred bank account.");
		}

		if (candidate.AcceptedPaymentMethods.Contains(advertised.MethodKind))
		{
			return new EmploymentPaymentSelection(true, advertised, $"Selected advertised payment method {advertised.MethodKind}.");
		}

		if (candidate.AcceptedPaymentMethods.Contains(PaymentMethodKind.Cash))
		{
			return new EmploymentPaymentSelection(true, new PaymentMethod(PaymentMethodKind.Cash),
				"Fell back to cash payment.");
		}

		return new EmploymentPaymentSelection(false, null, "No acceptable payment method is available.");
	}
}
