using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Economy;
using MudSharp.Economy.Employment;

#nullable enable

namespace MudSharp.Economy.Employment;

public static class EmploymentCandidateMatcher
{
	public static bool IsMatch(IJobOpening opening, EmploymentCandidateProfile candidate, out string reason)
	{
		foreach (var requirement in opening.Requirements.Skills)
		{
			if (!candidate.Skills.TryGetValue(requirement.SkillName, out var value) || value < requirement.MinimumValue)
			{
				reason = $"Candidate lacks required skill {requirement.SkillName}.";
				return false;
			}
		}

		foreach (var requirement in opening.Requirements.Knowledges)
		{
			if (!candidate.Knowledges.Contains(requirement.KnowledgeName))
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
			if (!candidate.Tags.Contains(requirement.TagName))
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
		    opening.Compensation.NominalAmount < candidate.ReservationWage)
		{
			reason = "Offer is below the candidate's reservation wage.";
			return false;
		}

		reason = string.Empty;
		return true;
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
