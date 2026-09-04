#nullable enable

using System;

namespace MudSharp.Models;

public class EconomicActivityRecord
{
	public long Id { get; set; }
	public DateTime RealDateTime { get; set; }
	public long? EconomicZoneId { get; set; }
	public long CurrencyId { get; set; }
	public long? FinancialPeriodId { get; set; }
	public long? MudCalendarId { get; set; }
	public int? MudYear { get; set; }
	public int? MudMonth { get; set; }
	public int? MudWeek { get; set; }
	public int? MudDay { get; set; }
	public string? MudDateTime { get; set; }
	public int ActivityType { get; set; }
	public int VolumeClassification { get; set; }
	public decimal Amount { get; set; }
	public decimal GlobalBaseValue { get; set; }
	public long? SourceId { get; set; }
	public string? SourceType { get; set; }
	public int SourceControlBucket { get; set; }
	public long? DestinationId { get; set; }
	public string? DestinationType { get; set; }
	public int DestinationControlBucket { get; set; }
	public long? ReferenceId { get; set; }
	public string? ReferenceType { get; set; }
	public string? ReferenceText { get; set; }

	public virtual EconomicZone? EconomicZone { get; set; }
	public virtual Currency Currency { get; set; } = null!;
	public virtual FinancialPeriod? FinancialPeriod { get; set; }
}
