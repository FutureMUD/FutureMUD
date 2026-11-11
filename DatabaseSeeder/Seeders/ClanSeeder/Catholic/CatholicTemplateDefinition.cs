#nullable enable

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DatabaseSeeder.Seeders;


public sealed class CatholicTemplateDefinition
{
	[JsonPropertyName("key")]
	public string Key { get; set; } = string.Empty;
	public string Name { get; set; } = string.Empty;
	public string Alias { get; set; } = string.Empty;
	public string FullName { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public string Sphere { get; set; } = string.Empty;
	[JsonPropertyName("rank_model")]
	public string RankModel { get; set; } = string.Empty;
	[JsonPropertyName("ranks")]
	public List<CatholicRankDefinition> Ranks { get; set; } = [];
	[JsonPropertyName("appointments")]
	public List<CatholicAppointmentDefinition> Appointments { get; set; } = [];
	[JsonPropertyName("live_bindings")]
	public List<CatholicBindingDefinition> LiveBindings { get; set; } = [];
	[JsonPropertyName("external_controls")]
	public List<CatholicControlDefinition> ExternalControls { get; set; } = [];
}

public sealed class CatholicRankDefinition
{
	[JsonPropertyName("key")]
	public string Key { get; set; } = string.Empty;
	public string Name { get; set; } = string.Empty;
	public string Abbreviation { get; set; } = string.Empty;
	public string Title { get; set; } = string.Empty;
	public int RankNumber { get; set; }
	public string RankPath { get; set; } = string.Empty;
	public int FameType { get; set; }
	public List<string> PrivilegeNames { get; set; } = [];
}

public sealed class CatholicAppointmentDefinition
{
	[JsonPropertyName("key")]
	public string Key { get; set; } = string.Empty;
	public string Name { get; set; } = string.Empty;
	public string Abbreviation { get; set; } = string.Empty;
	public string Title { get; set; } = string.Empty;
	public string MinimumRankKey { get; set; } = string.Empty;
	public string AppointmentRoute { get; set; } = string.Empty;
	public string? MinimumRankToAppointKey { get; set; }
	public string? ParentAppointmentKey { get; set; }
	public string? QualificationRule { get; set; }
	public string? CandidateProfile { get; set; }
	public string? ElectorateProfile { get; set; }
	public string? CanNominateProgRef { get; set; }
	public string? WhyCantNominateProgRef { get; set; }
	public string? NumberOfVotesProgRef { get; set; }
	public string? CanInitiateRecallProgRef { get; set; }
	public string? RecallVotesProgRef { get; set; }
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
	public CatholicOfficeReference? ElectionConfirmationAppointmentRef { get; set; }
}

public sealed class CatholicOfficeReference
{
	[JsonPropertyName("clan")]
	public string Clan { get; set; } = string.Empty;
	[JsonPropertyName("appointment_key")]
	public string AppointmentKey { get; set; } = string.Empty;
}

public sealed class CatholicBindingDefinition
{
	[JsonPropertyName("role")]
	public string Role { get; set; } = string.Empty;
	[JsonPropertyName("required")]
	public bool Required { get; set; }
	[JsonPropertyName("allowed_template_keys")]
	public List<string> AllowedTemplateKeys { get; set; } = [];
}

public sealed class CatholicControlDefinition
{
	[JsonPropertyName("controlled_appointment_key")]
	public string ControlledAppointmentKey { get; set; } = string.Empty;
	[JsonPropertyName("liege_clan_role")]
	public string LiegeClanRole { get; set; } = string.Empty;
	[JsonPropertyName("controlling_appointment_key")]
	public string ControllingAppointmentKey { get; set; } = string.Empty;
	public int NumberOfAppointments { get; set; }
}
