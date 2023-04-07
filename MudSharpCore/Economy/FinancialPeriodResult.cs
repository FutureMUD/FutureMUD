using System;
using MudSharp.TimeAndDate;

namespace MudSharp.Economy;

public class FinancialPeriodResult
{
	public FinancialPeriodResult(FinancialPeriod period, decimal grossRevenue, decimal netRevenue, decimal salesTax,
		decimal profitsTax)
	{
		Period = period;
		GrossRevenue = grossRevenue;
		NetRevenue = netRevenue;
		SalesTax = salesTax;
		ProfitsTax = profitsTax;
	}

	public FinancialPeriod Period { get; }
	public decimal GrossRevenue { get; }
	public decimal NetRevenue { get; }
	public decimal SalesTax { get; }
	public decimal ProfitsTax { get; }
}