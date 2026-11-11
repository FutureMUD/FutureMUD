#nullable enable

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json.Nodes;

namespace MudSharp.Community.Templates;


public sealed class ClanTemplateDefinition
{
	[JsonPropertyName("key")]
	public string Key { get; set; } = string.Empty;
	public bool IsAffiliationRoot { get; set; }
	public string? AffiliationRole { get; set; }
	public List<ClanTemplateCondition> BaseConditions { get; set; } = [];
	public List<ClanTemplateVariant> AvailableVariants { get; set; } = [];
	public string Name { get; set; } = string.Empty;
	public string Alias { get; set; } = string.Empty;
	public string FullName { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public string Sphere { get; set; } = string.Empty;
	[JsonPropertyName("ranks")]
	public List<ClanTemplateRankDefinition> Ranks { get; set; } = [];
	[JsonPropertyName("appointments")]
	public List<ClanTemplateAppointmentDefinition> Appointments { get; set; } = [];
	[JsonPropertyName("live_bindings")]
	public List<ClanTemplateBindingDefinition> LiveBindings { get; set; } = [];
	[JsonPropertyName("external_controls")]
	public List<ClanTemplateControlDefinition> ExternalControls { get; set; } = [];
}

public sealed class ClanTemplateRankDefinition
{
	[JsonPropertyName("key")]
	public string Key { get; set; } = string.Empty;
	public string Name { get; set; } = string.Empty;
	public string Abbreviation { get; set; } = string.Empty;
	public string Title { get; set; } = string.Empty;
	public List<ClanTemplateAddressRule> FormsOfAddress { get; set; } = [];
	public int RankNumber { get; set; }
	public string RankPath { get; set; } = string.Empty;
	public int FameType { get; set; }
	public List<string> PrivilegeNames { get; set; } = [];
}

public sealed class ClanTemplateAppointmentDefinition
{
	public int AddressPriority { get; set; }
	[JsonPropertyName("key")]
	public string Key { get; set; } = string.Empty;
	public string Name { get; set; } = string.Empty;
	public string Abbreviation { get; set; } = string.Empty;
	public string Title { get; set; } = string.Empty;
	public List<ClanTemplateAddressRule> FormsOfAddress { get; set; } = [];
	public string MinimumRankKey { get; set; } = string.Empty;
	public string? MinimumRankToAppointKey { get; set; }
	public string? ParentAppointmentKey { get; set; }
	public int MaximumSimultaneousHolders { get; set; }
	public int FameType { get; set; }
	public int? MaximumConsecutiveTerms { get; set; }
	public int? MaximumTotalTerms { get; set; }
	public double? ElectionTermMinutes { get; set; }
	public double? ElectionLeadTimeMinutes { get; set; }
	public double? NominationPeriodMinutes { get; set; }
	public double? VotingPeriodMinutes { get; set; }
	public double RecallThreshold { get; set; }
	public bool IsAppointedByElection { get; set; }
	public bool CanDirectlyAppoint { get; set; }
	public bool CanParentDismiss { get; set; }
	public bool CanResign { get; set; }
	public bool CanBeRecalled { get; set; }
	public bool? IsSecretBallot { get; set; }
	public List<string> PrivilegeNames { get; set; } = [];
	public List<ClanTemplateCondition> CandidateConditions { get; set; } = [];
	public List<ClanTemplateCondition> QualificationConditions { get; set; } = [];
	public List<ClanTemplateCondition> VoterConditions { get; set; } = [];
	public List<ClanTemplateCondition> RecallConditions { get; set; } = [];
	public ClanTemplateOfficeReference? ElectionConfirmationAppointmentRef { get; set; }
}

public sealed class ClanTemplateOfficeReference
{
	[JsonPropertyName("clan")]
	public string Clan { get; set; } = string.Empty;
	[JsonPropertyName("appointment_key")]
	public string AppointmentKey { get; set; } = string.Empty;
}

public sealed class ClanTemplateBindingDefinition
{
	[JsonPropertyName("role")]
	public string Role { get; set; } = string.Empty;
	[JsonPropertyName("required")]
	public bool Required { get; set; }
	public bool RequiresSelectedOffice { get; set; }
	[JsonPropertyName("allowed_template_keys")]
	public List<string> AllowedTemplateKeys { get; set; } = [];
}

public sealed class ClanTemplateControlDefinition
{
	[JsonPropertyName("controlled_appointment_key")]
	public string ControlledAppointmentKey { get; set; } = string.Empty;
	[JsonPropertyName("liege_clan_role")]
	public string LiegeClanRole { get; set; } = string.Empty;
	[JsonPropertyName("controlling_appointment_key")]
	public string ControllingAppointmentKey { get; set; } = string.Empty;
	public int NumberOfAppointments { get; set; }
}

/// <summary>A conjunction of data-authored membership, minimum-rank, exact-rank or office requirements.</summary>
public sealed class ClanTemplateCondition
{
	public string Role { get; set; } = "$self";
	public string Kind { get; set; } = "membership";
	public string? Key { get; set; }
	public string Error { get; set; } = "The required membership or standing is missing.";
}

/// <summary>Explicit keyed field overrides; applying a variant never deletes history or offices.</summary>
public sealed class ClanTemplateVariant
{
	public string Key { get; set; } = string.Empty;
	public List<ClanTemplateRankDefinition> AddedRanks { get; set; } = [];
	public Dictionary<string, JsonObject> RankOverrides { get; set; } = [];
	public Dictionary<string, JsonObject> AppointmentOverrides { get; set; } = [];
	public List<string> RemovedExternalControls { get; set; } = [];
}

/// <summary>Literal address plus optional explicit, clone-bound subject conditions or a shared global prog.</summary>
public sealed class ClanTemplateAddressRule
{
	public string Key { get; set; } = string.Empty;
	public string? Text { get; set; }
	public ClanAddressRelation Relations { get; set; } = ClanAddressRelation.Any;
	public bool IsPublic { get; set; }
	public List<ClanTemplateCondition> Conditions { get; set; } = [];
	public string? ProgName { get; set; }
}
