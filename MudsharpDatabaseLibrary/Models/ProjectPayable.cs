using System;

namespace MudSharp.Models;

public class ProjectPayable
{
	public long Id { get; set; }
	public long? ActiveProjectId { get; set; }
	public long ProjectDefinitionId { get; set; }
	public int ProjectRevisionNumber { get; set; }
	public string ProjectName { get; set; }
	public long? ProjectOwnerCharacterId { get; set; }
	public long CharacterId { get; set; }
	public long CurrencyId { get; set; }
	public decimal Amount { get; set; }
	public int PayableType { get; set; }
	public long? ProjectLabourRequirementId { get; set; }
	public string RequirementName { get; set; }
	public string Reason { get; set; }
	public DateTime EarnedAt { get; set; }
	public DateTime? ClaimedAt { get; set; }
	public long? ClaimedBankAccountId { get; set; }

	public virtual Currency Currency { get; set; }
	public virtual BankAccount ClaimedBankAccount { get; set; }
}
