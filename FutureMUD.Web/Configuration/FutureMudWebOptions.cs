#nullable enable

namespace FutureMUD.Web.Configuration;

public sealed class FutureMudWebOptions
{
	public const string SectionName = "FutureMUD";
	public string DataRoot { get; set; } = "/var/lib/futuremud-web";
	public string ContentRoot { get; set; } = "Content";
	public string? PublishingTokenSha256 { get; set; }
	public long MinimumFreeBytes { get; set; } = 2L * 1024 * 1024 * 1024;
	public TimeSpan DraftLifetime { get; set; } = TimeSpan.FromHours(24);
	public TimeSpan PreviousReleaseLifetime { get; set; } = TimeSpan.FromHours(24);
}
