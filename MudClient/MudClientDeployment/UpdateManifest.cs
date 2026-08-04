#nullable enable

namespace MudClientDeployment;

public sealed class UpdateManifest
{
	public const int CurrentSchemaVersion = 1;
	public int SchemaVersion { get; init; }
	public string Product { get; init; } = string.Empty;
	public string Version { get; init; } = string.Empty;
	public string SourceCommit { get; init; } = string.Empty;
	public string KeyId { get; init; } = string.Empty;
	public IReadOnlyList<UpdateArtifact> Artifacts { get; init; } = [];
}

public sealed class UpdateArtifact
{
	public string Runtime { get; init; } = string.Empty;
	public string FileName { get; init; } = string.Empty;
	public long Size { get; init; }
	public string Sha256 { get; init; } = string.Empty;
}
