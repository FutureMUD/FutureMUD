#nullable enable

using System.Text.Json;
using MudSharp.Community.Templates;

namespace MudSharp.Community;

/// <summary>Provisioning input/result retained in a private prog comment, not a new clan schema.</summary>
public sealed class ClanTemplateProfile
{
	public const string CommentPrefix = "ClanTemplateProfile_v1\n";
	public string TemplateKey { get; set; } = string.Empty;
	public long ClanId { get; set; }
	public bool Activated { get; set; }
	public bool StockTemplate { get; set; }
	public int AddressContentVersion { get; set; }
	public Dictionary<string, long> Ranks { get; set; } = [];
	public Dictionary<string, long> Appointments { get; set; } = [];
	public Dictionary<string, ClanTemplateBinding> Bindings { get; set; } = [];
	public List<string> Variants { get; set; } = [];
	public Dictionary<string, long> GeneratedProgs { get; set; } = [];
	public static string ProgName(long clanId) => $"ClanTemplate_{clanId}_Profile";
	public string ToComment() => CommentPrefix + JsonSerializer.Serialize(this);
	public ClanTemplateDefinition Definition { get; set; } = new();

	public static ClanTemplateProfile? Read(IFuturemud world, long clanId)
	{
		var prog = world.FutureProgs.FirstOrDefault(x => x.FunctionName == ProgName(clanId));
		if (prog?.FunctionComment?.StartsWith(CommentPrefix, StringComparison.Ordinal) != true) return null;
		try
		{
			var profile = JsonSerializer.Deserialize<ClanTemplateProfile>(prog.FunctionComment[CommentPrefix.Length..]);
			return profile?.ClanId == clanId ? profile : null;
		}
		catch (JsonException) { return null; }
	}

	public static ClanTemplateDefinition? Source(IFuturemud world, IClan clan) => Read(world, clan.Id)?.Definition;

	public static string? SourceError(IFuturemud world, IClan source, ClanTemplateDefinition definition)
	{
		try { ClanTemplateDefinitions.Validate(definition); }
		catch (InvalidOperationException ex) { return ex.Message; }
		var profile = Read(world, source.Id);
		if (definition.Ranks.Any(x => profile is null ? source.Ranks.All(y => y.Name != x.Name) :
		    !profile.Ranks.TryGetValue(x.Key, out var id) || source.Ranks.All(y => y.Id != id)) ||
		    definition.Appointments.Any(x => profile is null ? source.Appointments.All(y => y.Name != x.Name) :
		    !profile.Appointments.TryGetValue(x.Key, out var id) || source.Appointments.All(y => y.Id != id)))
			return "This template profile source has incomplete or ambiguous rank/office mappings. Review it before copying; no clone was created.";
		return null;
	}
}

public sealed class ClanTemplateBinding
{
	public long ClanId { get; set; }
	public long? AppointmentId { get; set; }
	/// <summary>Explicit affiliation input for a custom authority without its own template profile.</summary>
	public long? AffiliationId { get; set; }
}
