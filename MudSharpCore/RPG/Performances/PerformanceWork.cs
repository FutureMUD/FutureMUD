#nullable enable

using System;
using System.Collections.Generic;

namespace MudSharp.RPG.Performances;

public enum PerformanceMode
{
	Npc,
	Pc,
	Either
}

public enum PerformanceVerb
{
	Speak,
	Sing,
	Emote,
	Travel,
	Give,
	Get,
	Play,
	Stop,
	Guard,
	Stage,
	Unstage,
	Wait
}

public sealed class PerformanceRole
{
	public string Key { get; set; } = string.Empty;
	public string Mark { get; set; } = string.Empty;
	public PerformanceMode Mode { get; set; }
	public bool Required { get; set; }
	public long EligibilityProgId { get; set; }
	public bool ReturnAfterPerformance { get; set; } = true;
}

public sealed class PerformanceExit
{
	public string Key { get; set; } = string.Empty;
	public string From { get; set; } = string.Empty;
	public string To { get; set; } = string.Empty;
}

public sealed class PerformanceCue
{
	public string Role { get; set; } = string.Empty;
	public string Place { get; set; } = string.Empty;
	public PerformanceVerb Verb { get; set; }
	public string Target { get; set; } = string.Empty;
	public string Text { get; set; } = string.Empty;
	public int Seconds { get; set; }
}

public sealed class PerformanceWork
{
	// Seeder provenance belongs to the installed revision. Authored copies and revisions start without it.
	public string? SeedKey { get; set; }
	public long Id { get; set; }
	public long OwnerId { get; set; }
	public string Name { get; set; } = string.Empty;
	public int Revision { get; set; } = 1;
	public bool Published { get; set; }
	public bool StaffAuthored { get; set; }
	public List<PerformanceRole> Roles { get; set; } = new();
	public List<string> Places { get; set; } = new();
	public List<PerformanceExit> Exits { get; set; } = new();
	public List<string> Props { get; set; } = new();
	public List<string> LeaveProps { get; set; } = [];
	public List<PerformanceCue> Cues { get; set; } = new();
	public string FailureRole { get; set; } = string.Empty;
	public string FailureText { get; set; } = string.Empty;
}
