#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using MudSharp.Community;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.TimeAndDate;
using MudSharp.Models;
using MudSharp.Character;

namespace MudSharp.Economy.Lending;

internal sealed class LendingProductPolicy
{
	public bool AutomaticApproval { get; set; }
	public decimal MaximumUnsecuredAmount { get; set; } = 1000.0M;
	public decimal MaximumUnsecuredExposure { get; set; } = 2000.0M;
	public decimal MaximumSecuredAmount { get; set; } = 5000.0M;
	public decimal MaximumSecuredExposure { get; set; } = 10000.0M;
	public decimal MinimumCollateralValue { get; set; } = 1.0M;
	public decimal MinimumCollateralCoverage { get; set; } = 1.25M;
	public long? UnsecuredEligibilityProgId { get; set; }
	public long? UnsecuredMaximumAmountProgId { get; set; }
	public long? SecuredEligibilityProgId { get; set; }
	public long? SecuredMaximumAmountProgId { get; set; }
}

internal sealed class LendingProductDefinition
{
	public LoanTerms Terms { get; set; } = new();
	public LendingProductPolicy Policy { get; set; } = new();
}

internal static partial class LoanService
{
	private static readonly JsonSerializerOptions ProductJsonOptions = new()
	{
		PropertyNameCaseInsensitive = true
	};

	internal static bool Enabled(ICharacter actor) => actor.Gameworld.GetStaticBool("LoansEnabled");

	internal static LendingProductDefinition ProductDefinition(LendingProduct product)
	{
		return TryProductDefinition(product, out var definition)
			? definition
			: new LendingProductDefinition();
	}

	private static bool TryProductDefinition(LendingProduct product, out LendingProductDefinition definition)
	{
		try
		{
			definition = JsonSerializer.Deserialize<LendingProductDefinition>(product.Definition, ProductJsonOptions)
			              ?? throw new JsonException("Product definition is empty.");
			return definition.Terms is not null && definition.Policy is not null;
		}
		catch (JsonException)
		{
			definition = new LendingProductDefinition();
			return false;
		}
	}

	internal static LoanTerms ProductTerms(LendingProduct product) => ProductDefinition(product).Terms;

	internal static bool Automatic(LendingProduct product) =>
		product.Enabled && ProductDefinition(product).Policy.AutomaticApproval;

	internal static IEnumerable<IFrameworkItem> LocalLenders(ICharacter actor)
	{
		if (actor.Location is null)
		{
			return Enumerable.Empty<IFrameworkItem>();
		}

		return actor.Gameworld.Banks.Where(x => x.BranchLocations.Contains(actor.Location))
			.Cast<IFrameworkItem>()
			.Concat(actor.Gameworld.Shops.Where(x => x.CurrentLocations.Contains(actor.Location)));
	}

	internal static bool IsManager(ICharacter actor, IFrameworkItem lender) => lender switch
	{
		IBank bank => bank.IsManager(actor),
		IShop shop => shop.IsManager(actor) || shop.IsProprietor(actor),
		_ => false
	};

	internal static IEnumerable<LendingProduct> Products(IFrameworkItem lender, bool includeDisabled = false)
	{
		using (new FMDB())
		{
			return FMDB.Context.LendingProducts
				.Where(x => x.LenderId == lender.Id && x.LenderType == lender.FrameworkItemType && (includeDisabled || x.Enabled))
				.OrderBy(x => x.Name)
				.ToList();
		}
	}

	internal static bool Underwrite(ICharacter borrower, IFrameworkItem lender, LendingProduct product,
		decimal amount, decimal appraisal, decimal exposure, out string error, bool servicing = false)
	{
		error = string.Empty;
		if (!TryProductDefinition(product, out var definition))
		{
			error = "This lending product has invalid saved terms. A manager must repair the product before it can be offered.";
			return false;
		}

		if (amount <= 0.0M)
		{
			error = "The requested advance must be positive.";
			return false;
		}

		var secured = definition.Terms.Kind is "Pawn" or "Property" or "Mortgage";
		if (!secured && definition.Terms.Kind != "Unsecured")
		{
			error = "This product has an unsupported security type.";
			return false;
		}

		if (secured && appraisal < definition.Policy.MinimumCollateralValue)
		{
			error = "The collateral does not meet this lender's minimum value.";
			return false;
		}

		var maximum = MaximumAdvance(borrower, lender, product, amount, appraisal, exposure, servicing);
		if (maximum <= 0.0M)
		{
			error = secured
				? "The appraisal and this lender's underwriting rules do not support an advance."
				: "This lender's underwriting rules do not support an advance.";
			return false;
		}

		if (amount > maximum)
		{
			error = $"The requested amount exceeds this lender's maximum advance of {maximum:N2}.";
			return false;
		}

		return true;
	}

	internal static decimal MaximumAdvance(ICharacter borrower, IFrameworkItem lender, LendingProduct product,
		decimal appraisal, decimal exposure) => MaximumAdvance(borrower, lender, product, 0.0M, appraisal, exposure);

	internal static decimal MaximumAdvance(ICharacter borrower, IFrameworkItem lender, LendingProduct product,
		decimal requestedAmount, decimal appraisal, decimal exposure, bool servicing = false)
	{
		if (!TryProductDefinition(product, out var definition))
		{
			return 0.0M;
		}

		var policy = definition.Policy;
		var secured = definition.Terms.Kind is "Pawn" or "Property" or "Mortgage";
		if ((!secured && definition.Terms.Kind != "Unsecured") || !product.Enabled && !servicing)
		{
			return 0.0M;
		}

		var maximum = HardMaximum(policy, secured, appraisal, exposure);
		if (maximum <= 0.0M)
		{
			return 0.0M;
		}

		var eligibilityId = secured ? policy.SecuredEligibilityProgId : policy.UnsecuredEligibilityProgId;
		var maximumAmountId = secured ? policy.SecuredMaximumAmountProgId : policy.UnsecuredMaximumAmountProgId;
		var eligibility = eligibilityId.HasValue ? borrower.Gameworld.FutureProgs.Get(eligibilityId.Value) : null;
		var maximumAmount = maximumAmountId.HasValue ? borrower.Gameworld.FutureProgs.Get(maximumAmountId.Value) : null;
		if ((eligibilityId.HasValue && eligibility is null) || (maximumAmountId.HasValue && maximumAmount is null))
		{
			return 0.0M;
		}

		var progRequestAmount = requestedAmount > 0.0M ? requestedAmount : maximum;
		var arguments = new object[] { borrower, progRequestAmount, appraisal, exposure };
		// Automatic pawn quotes first find the amount, then run eligibility on that actual offer.
		if (requestedAmount > 0 && eligibility is not null &&
		    (!HasSignature(eligibility, ProgVariableTypes.Boolean) || !ExecuteEligibility(eligibility, arguments)))
		{
			return 0.0M;
		}

		if (maximumAmount is not null)
		{
			if (!HasSignature(maximumAmount, ProgVariableTypes.Number) ||
			    !ExecuteMaximumAmount(maximumAmount, arguments, out var progMaximum))
			{
				return 0.0M;
			}

			maximum = Math.Min(maximum, progMaximum);
		}

		return Math.Max(0.0M, maximum);
	}

	internal static decimal HardMaximum(LendingProductPolicy policy, bool secured, decimal appraisal, decimal exposure)
	{
		if (secured && (appraisal < policy.MinimumCollateralValue || policy.MinimumCollateralCoverage < 1.0M))
		{
			return 0.0M;
		}

		var perLoanLimit = secured ? policy.MaximumSecuredAmount : policy.MaximumUnsecuredAmount;
		var portfolioLimit = secured ? policy.MaximumSecuredExposure : policy.MaximumUnsecuredExposure;
		var remainingExposure = portfolioLimit - exposure;
		if (perLoanLimit <= 0.0M || remainingExposure <= 0.0M)
		{
			return 0.0M;
		}

		var maximum = Math.Min(perLoanLimit, remainingExposure);
		return secured ? Math.Min(maximum, appraisal / policy.MinimumCollateralCoverage) : maximum;
	}

	internal static bool HasSignature(IFutureProg prog, ProgVariableTypes returnType)
	{
		return prog.ReturnType.CompatibleWith(returnType) &&
		       prog.MatchesParameters(new[]
		       {
			       ProgVariableTypes.Character,
			       ProgVariableTypes.Number,
			       ProgVariableTypes.Number,
			       ProgVariableTypes.Number
		       });
	}

	private static bool ExecuteEligibility(IFutureProg prog, object[] arguments)
	{
		try
		{
			return prog.ExecuteBool(false, arguments);
		}
		catch
		{
			return false;
		}
	}

	private static bool ExecuteMaximumAmount(IFutureProg prog, object[] arguments, out decimal maximum)
	{
		maximum = 0.0M;
		try
		{
			var value = prog.ExecuteDecimal(arguments);
			if (value < 0.0M)
			{
				return false;
			}

			maximum = value;
			return true;
		}
		catch
		{
			return false;
		}
	}
}
