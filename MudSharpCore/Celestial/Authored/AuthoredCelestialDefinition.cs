#nullable enable

using System.Text.Json;
using System.Text.Json.Serialization;

namespace MudSharp.Celestial.Authored;

public enum AuthoredCelestialKind { RailSun, RailMoon, ScriptedSun, ScriptedMoon }
public enum TrackTransition { Travel, Hold, Jump }
public enum AuthoredPathMode { SimpleRail, ThreePointRail, Sparse, Dense }
public enum AuthoredLightMode { Absolute, PhaseScaled }
public enum AuthoredPhaseMode { Fixed, Regular, Authored }
public enum CelestialEchoAudience { BodyVisible, SkyVisible }

public sealed record PositionKey(long Minute, double Azimuth, double Elevation, TrackTransition Transition = TrackTransition.Travel);
public sealed record ScalarKey(long Minute, double Value, TrackTransition Transition = TrackTransition.Travel);
public sealed record LightKnot(double Elevation, double Lux);
public sealed record CelestialMilestone(string Key, long Period, long Minute);
public sealed record AuthoredEcho(string Id, string Text, CelestialEchoAudience Audience = CelestialEchoAudience.BodyVisible,
	int Order = 0, string? Milestone = null, long Period = 0, long Minute = 0,
	double? Threshold = null, CelestialMoveDirection Direction = CelestialMoveDirection.Ascending);

public sealed record AuthoredPathDefinition
{
	public AuthoredPathMode Mode { get; set; }
	public long Period { get; set; }
	public long Rise { get; set; }
	public long Set { get; set; }
	public double RiseBearing { get; set; } = 90;
	public double SetBearing { get; set; } = 270;
	public double MaximumElevation { get; set; } = 90;
	public double TiltBearing { get; set; } = 180;
	public double ViaBearing { get; set; }
	public double ViaElevation { get; set; } = 30;
	/// <summary>Sparse paths include an explicit closure at Period. Dense paths have exactly Period entries.</summary>
	public List<PositionKey> Keys { get; set; } = [];
}

public sealed record AuthoredScalarTrack
{
	public long Period { get; set; }
	public List<ScalarKey> Keys { get; set; } = [];
}

public sealed record AuthoredPhaseDefinition
{
	public AuthoredPhaseMode Mode { get; set; }
	public double FixedTurns { get; set; }
	public long FullMoonMinute { get; set; }
	public AuthoredScalarTrack Track { get; set; } = new();
	public long? CrescentSunId { get; set; }
	public List<string> CrescentMilestones { get; set; } = [];
}

/// <summary>Versioned source data only. Compilation takes a private copy before activation.</summary>
public sealed record AuthoredCelestialDefinition
{
	public int Version { get; set; } = 1;
	public AuthoredCelestialKind Kind { get; set; }
	public string Name { get; set; } = "an authored celestial";
	public string Description { get; set; } = "An unearthly light follows its appointed course across the sky.";
	public long CalendarId { get; set; }
	public long AnchorTicks { get; set; }
	public long AnnualPeriod { get; set; }
	private bool? _timeOfDay;
	public bool TimeOfDay
	{
		get => _timeOfDay ?? Kind is AuthoredCelestialKind.RailSun or AuthoredCelestialKind.ScriptedSun;
		set => _timeOfDay = value;
	}
	public bool SyntheticLongitude { get; set; }
	public double LongitudeAtEpoch { get; set; }
	public AuthoredPathDefinition Path { get; set; } = new();
	public AuthoredLightMode LightMode { get; set; }
	public List<LightKnot> LightProfile { get; set; } = [];
	public AuthoredScalarTrack? LightTrack { get; set; }
	public AuthoredPhaseDefinition? Phase { get; set; }
	public List<CelestialMilestone> Milestones { get; set; } = [];
	public List<AuthoredEcho> Echoes { get; set; } = [];
	public string? SeederPreset { get; set; }
}

/// <summary>Limits are configurable at the builder/load boundary, never applied to occurrence rank or period length.</summary>
public sealed record AuthoredCelestialLimits(int SourceBytes = 16 * 1024 * 1024, int Entries = 100_000,
	int PreviewSamples = 10_000, int PreviewEvents = 1_000);

public static class AuthoredCelestialFormat
{
	private static readonly JsonSerializerOptions Options = new()
	{
		WriteIndented = true,
		UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
		Converters = { new JsonStringEnumConverter(allowIntegerValues: false) }
	};

	public static string Serialize(AuthoredCelestialDefinition definition) => JsonSerializer.Serialize(definition, Options);
	public static AuthoredCelestialDefinition Parse(string source, AuthoredCelestialLimits? limits = null)
	{
		limits ??= new();
		if (System.Text.Encoding.UTF8.GetByteCount(source) > limits.SourceBytes)
		{
			throw new ArgumentException($"Authored celestial source exceeds the {limits.SourceBytes:N0} byte limit.");
		}
		// JSON has no external entities, DTDs, filesystem includes or executable expressions.
		var definition = JsonSerializer.Deserialize<AuthoredCelestialDefinition>(source, Options)
		                 ?? throw new ArgumentException("An authored celestial definition is required.");
		AuthoredMath.Require(definition.Path?.Keys is not null && definition.LightProfile is not null &&
			definition.Milestones is not null && definition.Echoes is not null &&
			(definition.LightTrack is null || definition.LightTrack.Keys is not null) &&
			(definition.Phase is null || definition.Phase.Track?.Keys is not null && definition.Phase.CrescentMilestones is not null),
			"Definition channels and their entry lists cannot be null.");
		AuthoredMath.Require(definition.Path!.Keys.All(x => x is not null) && definition.LightProfile!.All(x => x is not null) &&
			definition.Milestones!.All(x => x is not null) && definition.Echoes!.All(x => x is not null) &&
			(definition.LightTrack?.Keys.All(x => x is not null) ?? true) &&
			(definition.Phase is null || definition.Phase.Track.Keys.All(x => x is not null) && definition.Phase.CrescentMilestones.All(x => x is not null)),
			"Authored channel entries cannot be null.");
		return definition;
	}
}
