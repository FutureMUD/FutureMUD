#nullable enable

namespace FutureMUD.Web.Publishing;

public sealed class ReleaseProductManifest
{
	public IReadOnlyList<ReleaseProductDefinition> Products { get; init; } = [];
}

public sealed class ReleaseProductDefinition
{
	public string Id { get; init; } = string.Empty;
	public string PublicName { get; init; } = string.Empty;
	public string TagPrefix { get; init; } = string.Empty;
	public string ProjectPath { get; init; } = string.Empty;
	public string VersionSource { get; init; } = "Version";
	public bool FrameworkDependent { get; init; } = true;
	public bool SingleFile { get; init; }
	public bool IncludeNativeLibrariesForSelfExtract { get; init; }
	public string ArchiveName { get; init; } = string.Empty;
	public IReadOnlyList<string> TestProjects { get; init; } = [];
	public IReadOnlyList<string> Runtimes { get; init; } = [];
	public bool DocumentationCatalogue { get; init; }
}

public sealed class CreateReleaseRequest
{
	public string Product { get; init; } = string.Empty;
	public string Version { get; init; } = string.Empty;
	public string SourceCommit { get; init; } = string.Empty;
	public IReadOnlyList<ReleaseArtifactRequest> Artifacts { get; init; } = [];
	public ReleaseArtifactRequest? DocumentationCatalogue { get; init; }
}

public sealed class ReleaseArtifactRequest
{
	public string ArtifactId { get; init; } = string.Empty;
	public string Runtime { get; init; } = string.Empty;
	public string FileName { get; init; } = string.Empty;
	public long Size { get; init; }
	public string Sha256 { get; init; } = string.Empty;
}

public sealed class StagedRelease
{
	public string UploadId { get; init; } = string.Empty;
	public string Product { get; init; } = string.Empty;
	public string Version { get; init; } = string.Empty;
	public string SourceCommit { get; init; } = string.Empty;
	public DateTimeOffset CreatedAtUtc { get; init; }
	public string Status { get; set; } = "staged";
	public IReadOnlyList<ReleaseArtifactRequest> Artifacts { get; init; } = [];
	public ReleaseArtifactRequest? DocumentationCatalogue { get; init; }
	public Dictionary<string, List<int>> ReceivedChunks { get; init; } = new(StringComparer.Ordinal);
}

public sealed class PublicRelease
{
	public string Product { get; init; } = string.Empty;
	public string Version { get; init; } = string.Empty;
	public string SourceCommit { get; init; } = string.Empty;
	public DateTimeOffset PublishedAtUtc { get; init; }
	public IReadOnlyList<ReleaseArtifactRequest> Artifacts { get; init; } = [];
}
