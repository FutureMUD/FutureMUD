#nullable enable

using FutureMUD.Web.Configuration;
using Microsoft.Extensions.Options;
using MudSharp.Documentation;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace FutureMUD.Web.Publishing;

public sealed partial class ReleaseStore
{
	public const int ChunkSize = 32 * 1024 * 1024;
	private const long MaximumArtifactSize = 4L * 1024 * 1024 * 1024;
	private readonly FutureMudWebOptions _options;
	private readonly ReleaseProductCatalogue _products;
	private readonly ILogger<ReleaseStore> _logger;
	private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new(StringComparer.Ordinal);
	private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web) { WriteIndented = true };

	public ReleaseStore(
		IOptions<FutureMudWebOptions> options,
		ReleaseProductCatalogue products,
		ILogger<ReleaseStore> logger)
	{
		_options = options.Value;
		_products = products;
		_logger = logger;
		try
		{
			Directory.CreateDirectory(StagingRoot);
			Directory.CreateDirectory(LiveRoot);
			Directory.CreateDirectory(PreviousRoot);
			Directory.CreateDirectory(Path.GetDirectoryName(DocumentationRoot)!);
			RecoverInterruptedPromotions();
			Directory.CreateDirectory(DocumentationRoot);
			IsReady = true;
		}
		catch (Exception exception)
		{
			_logger.LogError(exception, "Release storage initialisation failed.");
			IsReady = false;
		}
	}

	public bool IsReady { get; }
	public string StagingRoot => Path.Combine(_options.DataRoot, "staging");
	public string LiveRoot => Path.Combine(_options.DataRoot, "releases", "live");
	public string PreviousRoot => Path.Combine(_options.DataRoot, "releases", "previous");
	public string DocumentationRoot => Path.Combine(_options.DataRoot, "documentation", "live");

	public async Task<StagedRelease> CreateOrResumeAsync(CreateReleaseRequest request, CancellationToken cancellationToken)
	{
		ValidateManifest(request);
		var uploadedBytes = request.Artifacts.Sum(artifact => artifact.Size) + (request.DocumentationCatalogue?.Size ?? 0);
		EnsureDiskSpace(checked(uploadedBytes * 2));

		foreach (var directory in Directory.EnumerateDirectories(StagingRoot))
		{
			var existing = await ReadStagedReleaseAsync(Path.GetFileName(directory), cancellationToken);
			if (existing is not null &&
				existing.Product == request.Product &&
				existing.Version == request.Version &&
				existing.SourceCommit.Equals(request.SourceCommit, StringComparison.OrdinalIgnoreCase) &&
				existing.Status is not "promoted")
			{
				if (!ManifestMatches(existing, request))
				{
					throw new ReleaseStoreException("An upload with this product, version, and commit has a different manifest.", StatusCodes.Status409Conflict);
				}
				return existing;
			}
		}

		var release = new StagedRelease
		{
			UploadId = Guid.NewGuid().ToString("N"),
			Product = request.Product,
			Version = request.Version,
			SourceCommit = request.SourceCommit.ToLowerInvariant(),
			CreatedAtUtc = DateTimeOffset.UtcNow,
			Artifacts = request.Artifacts,
			DocumentationCatalogue = request.DocumentationCatalogue
		};
		Directory.CreateDirectory(GetDraftPath(release.UploadId));
		Directory.CreateDirectory(GetChunksPath(release.UploadId));
		await WriteStagedReleaseAsync(release, cancellationToken);
		_logger.LogInformation("Publishing draft {UploadId} created for {Product} {Version} at {Commit}.", release.UploadId, release.Product, release.Version, release.SourceCommit);
		return release;
	}

	public Task<StagedRelease?> GetAsync(string uploadId, CancellationToken cancellationToken) =>
		ReadStagedReleaseAsync(ValidateUploadId(uploadId), cancellationToken);

	public async Task<StagedRelease> PutChunkAsync(
		string uploadId,
		string artifactId,
		int index,
		long start,
		long end,
		long total,
		string digest,
		Stream body,
		CancellationToken cancellationToken)
	{
		uploadId = ValidateUploadId(uploadId);
		artifactId = ValidateIdentifier(artifactId, nameof(artifactId));
		var gate = _locks.GetOrAdd(uploadId, _ => new SemaphoreSlim(1, 1));
		await gate.WaitAsync(cancellationToken);
		try
		{
			var release = await RequireDraftAsync(uploadId, cancellationToken);
			if (release.Status != "staged")
			{
				throw new ReleaseStoreException("Only staged releases accept chunks.", StatusCodes.Status409Conflict);
			}
			var artifact = FindArtifact(release, artifactId);
			var expectedStart = (long)index * ChunkSize;
			var expectedLength = Math.Min(ChunkSize, artifact.Size - expectedStart);
			if (index < 0 || start != expectedStart || end != start + expectedLength - 1 || total != artifact.Size || expectedLength <= 0)
			{
				throw new ReleaseStoreException("The chunk range does not match the artifact manifest.", StatusCodes.Status416RangeNotSatisfiable);
			}

			var chunkPath = GetChunkPath(uploadId, artifactId, index);
			var temporaryPath = $"{chunkPath}.{Guid.NewGuid():N}.tmp";
			try
			{
				await using (var target = new FileStream(temporaryPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, FileOptions.Asynchronous | FileOptions.WriteThrough))
				{
					await CopyExactlyAsync(body, target, expectedLength, cancellationToken);
				}
				var actualDigest = await ComputeSha256Async(temporaryPath, cancellationToken);
				if (!actualDigest.Equals(NormaliseDigest(digest), StringComparison.OrdinalIgnoreCase))
				{
					throw new ReleaseStoreException("The chunk digest does not match.", StatusCodes.Status422UnprocessableEntity);
				}

				if (File.Exists(chunkPath))
				{
					var existingDigest = await ComputeSha256Async(chunkPath, cancellationToken);
					if (!existingDigest.Equals(actualDigest, StringComparison.OrdinalIgnoreCase))
					{
						throw new ReleaseStoreException("A different chunk already exists at this index.", StatusCodes.Status409Conflict);
					}
				}
				else
				{
					File.Move(temporaryPath, chunkPath);
				}
			}
			finally
			{
				if (File.Exists(temporaryPath))
				{
					File.Delete(temporaryPath);
				}
			}

			if (!release.ReceivedChunks.TryGetValue(artifactId, out var chunks))
			{
				chunks = [];
				release.ReceivedChunks[artifactId] = chunks;
			}
			if (!chunks.Contains(index))
			{
				chunks.Add(index);
				chunks.Sort();
				await WriteStagedReleaseAsync(release, cancellationToken);
			}
			return release;
		}
		finally
		{
			gate.Release();
		}
	}

	public async Task<StagedRelease> CompleteAsync(string uploadId, CancellationToken cancellationToken)
	{
		uploadId = ValidateUploadId(uploadId);
		var gate = _locks.GetOrAdd(uploadId, _ => new SemaphoreSlim(1, 1));
		await gate.WaitAsync(cancellationToken);
		try
		{
			var release = await RequireDraftAsync(uploadId, cancellationToken);
			if (release.Status == "validated")
			{
				return release;
			}
			if (release.Status != "staged")
			{
				throw new ReleaseStoreException("The release cannot be completed in its current state.", StatusCodes.Status409Conflict);
			}

			var outputDirectory = Path.Combine(GetDraftPath(uploadId), "release");
			if (Directory.Exists(outputDirectory))
			{
				Directory.Delete(outputDirectory, true);
			}
			Directory.CreateDirectory(outputDirectory);
			foreach (var artifact in AllArtifacts(release))
			{
				var outputPath = Path.Combine(outputDirectory, artifact.FileName);
				await using (var output = new FileStream(outputPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, FileOptions.Asynchronous | FileOptions.WriteThrough))
				{
					var chunks = checked((int)Math.Ceiling((double)artifact.Size / ChunkSize));
					for (var index = 0; index < chunks; index++)
					{
						var chunkPath = GetChunkPath(uploadId, artifact.ArtifactId, index);
						if (!File.Exists(chunkPath))
						{
							throw new ReleaseStoreException($"Chunk {index} for {artifact.ArtifactId} is missing.", StatusCodes.Status409Conflict);
						}
						await using var chunk = File.OpenRead(chunkPath);
						await chunk.CopyToAsync(output, cancellationToken);
					}
				}
				if (new FileInfo(outputPath).Length != artifact.Size ||
					!string.Equals(await ComputeSha256Async(outputPath, cancellationToken), artifact.Sha256, StringComparison.OrdinalIgnoreCase))
				{
					throw new ReleaseStoreException($"Artifact {artifact.ArtifactId} failed final verification.", StatusCodes.Status422UnprocessableEntity);
				}
			}
			if (release.DocumentationCatalogue is not null)
			{
				await ValidateDocumentationCatalogueAsync(
					Path.Combine(outputDirectory, release.DocumentationCatalogue.FileName),
					release,
					cancellationToken);
			}

			release.Status = "validated";
			await WriteStagedReleaseAsync(release, cancellationToken);
			_logger.LogInformation("Publishing draft {UploadId} validated.", uploadId);
			return release;
		}
		finally
		{
			gate.Release();
		}
	}

	public async Task<PublicRelease> PromoteAsync(string uploadId, CancellationToken cancellationToken)
	{
		uploadId = ValidateUploadId(uploadId);
		var gate = _locks.GetOrAdd(uploadId, _ => new SemaphoreSlim(1, 1));
		await gate.WaitAsync(cancellationToken);
		try
		{
			var staged = await RequireDraftAsync(uploadId, cancellationToken);
			if (staged.Status != "validated")
			{
				throw new ReleaseStoreException("Only a validated release can be promoted.", StatusCodes.Status409Conflict);
			}

			var publicRelease = new PublicRelease
			{
				Product = staged.Product,
				Version = staged.Version,
				SourceCommit = staged.SourceCommit,
				PublishedAtUtc = DateTimeOffset.UtcNow,
				Artifacts = staged.Artifacts
			};
			var candidate = Path.Combine(GetDraftPath(uploadId), "release");
			await WriteJsonAsync(Path.Combine(candidate, "release.json"), publicRelease, cancellationToken);
			var live = Path.Combine(LiveRoot, staged.Product);
			var previous = Path.Combine(PreviousRoot, staged.Product);
			if (Directory.Exists(previous))
			{
				Directory.Delete(previous, true);
			}
			if (Directory.Exists(live))
			{
				Directory.Move(live, previous);
			}
			try
			{
				Directory.Move(candidate, live);
			}
			catch
			{
				if (!Directory.Exists(live) && Directory.Exists(previous))
				{
					Directory.Move(previous, live);
				}
				throw;
			}

			if (staged.DocumentationCatalogue is not null)
			{
				var documentationCandidate = Path.Combine(live, staged.DocumentationCatalogue.FileName);
				var documentationTemporary = $"{DocumentationRoot}.new";
				if (Directory.Exists(documentationTemporary))
				{
					Directory.Delete(documentationTemporary, true);
				}
				Directory.CreateDirectory(documentationTemporary);
				File.Copy(documentationCandidate, Path.Combine(documentationTemporary, "catalogue.json"));
				var documentationPrevious = $"{DocumentationRoot}.previous";
				if (Directory.Exists(documentationPrevious))
				{
					Directory.Delete(documentationPrevious, true);
				}
				if (Directory.Exists(DocumentationRoot))
				{
					Directory.Move(DocumentationRoot, documentationPrevious);
				}
				try
				{
					Directory.Move(documentationTemporary, DocumentationRoot);
				}
				catch
				{
					if (!Directory.Exists(DocumentationRoot) && Directory.Exists(documentationPrevious))
					{
						Directory.Move(documentationPrevious, DocumentationRoot);
					}
					if (Directory.Exists(live) && !Directory.Exists(candidate))
					{
						Directory.Move(live, candidate);
					}
					if (!Directory.Exists(live) && Directory.Exists(previous))
					{
						Directory.Move(previous, live);
					}
					throw;
				}
			}

			staged.Status = "promoted";
			await WriteStagedReleaseAsync(staged, cancellationToken);
			_logger.LogInformation("Publishing draft {UploadId} promoted as {Product} {Version}.", uploadId, staged.Product, staged.Version);
			return publicRelease;
		}
		finally
		{
			gate.Release();
		}
	}

	public async Task AbandonAsync(string uploadId, CancellationToken cancellationToken)
	{
		uploadId = ValidateUploadId(uploadId);
		var gate = _locks.GetOrAdd(uploadId, _ => new SemaphoreSlim(1, 1));
		await gate.WaitAsync(cancellationToken);
		try
		{
			var release = await RequireDraftAsync(uploadId, cancellationToken);
			if (release.Status == "promoted")
			{
				throw new ReleaseStoreException("A promoted release cannot be abandoned.", StatusCodes.Status409Conflict);
			}
			Directory.Delete(GetDraftPath(uploadId), true);
		}
		finally
		{
			gate.Release();
		}
	}

	public async Task<PublicRelease?> GetLiveReleaseAsync(string product, CancellationToken cancellationToken)
	{
		ValidateIdentifier(product, nameof(product));
		var path = Path.Combine(LiveRoot, product, "release.json");
		if (!File.Exists(path))
		{
			return null;
		}
		await using var stream = File.OpenRead(path);
		return await JsonSerializer.DeserializeAsync<PublicRelease>(stream, _jsonOptions, cancellationToken);
	}

	public string GetLiveArtifactPath(string product, string fileName) => Path.Combine(LiveRoot, ValidateIdentifier(product, nameof(product)), Path.GetFileName(fileName));

	public void Cleanup(DateTimeOffset now)
	{
		foreach (var directory in Directory.EnumerateDirectories(StagingRoot))
		{
			var metadata = Path.Combine(directory, "release.json");
			if (File.Exists(metadata) && now - File.GetLastWriteTimeUtc(metadata) > _options.DraftLifetime)
			{
				Directory.Delete(directory, true);
			}
		}
		foreach (var directory in Directory.EnumerateDirectories(PreviousRoot))
		{
			if (now - Directory.GetLastWriteTimeUtc(directory) > _options.PreviousReleaseLifetime)
			{
				Directory.Delete(directory, true);
			}
		}
	}

	private void RecoverInterruptedPromotions()
	{
		foreach (var moving in Directory.EnumerateDirectories(LiveRoot, "*.moving"))
		{
			var live = moving[..^".moving".Length];
			if (!Directory.Exists(live))
			{
				Directory.Move(moving, live);
			}
			else
			{
				var previous = Path.Combine(PreviousRoot, Path.GetFileName(live));
				if (!Directory.Exists(previous))
				{
					Directory.Move(moving, previous);
				}
			}
		}
		foreach (var previous in Directory.EnumerateDirectories(PreviousRoot))
		{
			var live = Path.Combine(LiveRoot, Path.GetFileName(previous));
			if (!Directory.Exists(live))
			{
				Directory.Move(previous, live);
			}
		}

		var documentationPrevious = $"{DocumentationRoot}.previous";
		if (!Directory.Exists(DocumentationRoot) && Directory.Exists(documentationPrevious))
		{
			Directory.Move(documentationPrevious, DocumentationRoot);
		}
		var documentationTemporary = $"{DocumentationRoot}.new";
		if (Directory.Exists(documentationTemporary))
		{
			Directory.Delete(documentationTemporary, true);
		}
	}

	private async Task ValidateDocumentationCatalogueAsync(
		string path,
		StagedRelease release,
		CancellationToken cancellationToken)
	{
		DocumentationCatalogue? catalogue;
		try
		{
			await using var stream = File.OpenRead(path);
			catalogue = await JsonSerializer.DeserializeAsync<DocumentationCatalogue>(stream, _jsonOptions, cancellationToken);
		}
		catch (JsonException)
		{
			throw new ReleaseStoreException("The documentation catalogue is not valid JSON.", StatusCodes.Status422UnprocessableEntity);
		}

		if (catalogue is null ||
			catalogue.SchemaVersion != DocumentationCatalogue.CurrentSchemaVersion ||
			catalogue.EngineVersion != release.Version ||
			!catalogue.SourceRevision.Equals(release.SourceCommit, StringComparison.OrdinalIgnoreCase) ||
			catalogue.GeneratedAtUtc == default ||
			catalogue.Commands is null || catalogue.Commands.Count == 0 ||
			catalogue.ProgFunctions is null || catalogue.ProgFunctions.Count == 0 ||
			catalogue.ProgTypes is null || catalogue.ProgTypes.Count == 0 ||
			catalogue.CollectionExtensions is null || catalogue.CollectionExtensions.Count == 0 ||
			catalogue.ItemComponents is null || catalogue.ItemComponents.Count == 0)
		{
			throw new ReleaseStoreException(
				"The documentation catalogue schema, release metadata, or required metadata families are invalid.",
				StatusCodes.Status422UnprocessableEntity);
		}
	}

	private void ValidateManifest(CreateReleaseRequest request)
	{
		if (!_products.TryGet(request.Product, out var product))
		{
			throw new ReleaseStoreException("Unknown product.", StatusCodes.Status400BadRequest);
		}
		if (!VersionRegex().IsMatch(request.Version) || !CommitRegex().IsMatch(request.SourceCommit))
		{
			throw new ReleaseStoreException("Version or source commit is invalid.", StatusCodes.Status400BadRequest);
		}
		if (request.Artifacts.Count != product.Runtimes.Count ||
			!request.Artifacts.Select(artifact => artifact.Runtime).Order().SequenceEqual(product.Runtimes.Order()))
		{
			throw new ReleaseStoreException("The release does not contain the required runtime matrix.", StatusCodes.Status400BadRequest);
		}
		if (product.DocumentationCatalogue != (request.DocumentationCatalogue is not null))
		{
			throw new ReleaseStoreException("The documentation catalogue requirement is not satisfied.", StatusCodes.Status400BadRequest);
		}
		var ids = new HashSet<string>(StringComparer.Ordinal);
		foreach (var artifact in request.Artifacts.Append(request.DocumentationCatalogue).Where(artifact => artifact is not null).Cast<ReleaseArtifactRequest>())
		{
			ValidateIdentifier(artifact.ArtifactId, nameof(artifact.ArtifactId));
			if (!ids.Add(artifact.ArtifactId) || artifact.Size <= 0 || artifact.Size > MaximumArtifactSize ||
				!ShaRegex().IsMatch(artifact.Sha256) || Path.GetFileName(artifact.FileName) != artifact.FileName ||
				!FileNameRegex().IsMatch(artifact.FileName))
			{
				throw new ReleaseStoreException("An artifact manifest entry is invalid.", StatusCodes.Status400BadRequest);
			}
		}
		foreach (var artifact in request.Artifacts)
		{
			var expectedName = product.ArchiveName
				.Replace("{version}", request.Version, StringComparison.Ordinal)
				.Replace("{runtime}", artifact.Runtime, StringComparison.Ordinal);
			if (!product.Runtimes.Contains(artifact.Runtime, StringComparer.Ordinal) || artifact.FileName != expectedName || artifact.ArtifactId != artifact.Runtime)
			{
				throw new ReleaseStoreException("An artifact does not match the product filename and runtime allowlist.", StatusCodes.Status400BadRequest);
			}
		}
		if (request.DocumentationCatalogue is not null &&
			(request.DocumentationCatalogue.ArtifactId != "documentation" ||
			 request.DocumentationCatalogue.Runtime != "documentation" ||
			 request.DocumentationCatalogue.FileName != "catalogue.json"))
		{
			throw new ReleaseStoreException("The documentation catalogue manifest entry is invalid.", StatusCodes.Status400BadRequest);
		}
	}

	private static bool ManifestMatches(StagedRelease existing, CreateReleaseRequest request)
	{
		static string Key(ReleaseArtifactRequest artifact) =>
			$"{artifact.ArtifactId}|{artifact.Runtime}|{artifact.FileName}|{artifact.Size}|{artifact.Sha256.ToLowerInvariant()}";

		return existing.Artifacts.Select(Key).Order(StringComparer.Ordinal)
			.SequenceEqual(request.Artifacts.Select(Key).Order(StringComparer.Ordinal), StringComparer.Ordinal) &&
			(existing.DocumentationCatalogue, request.DocumentationCatalogue) switch
			{
				(null, null) => true,
				(not null, not null) => Key(existing.DocumentationCatalogue) == Key(request.DocumentationCatalogue),
				_ => false
			};
	}

	private void EnsureDiskSpace(long requestedBytes)
	{
		var root = Path.GetPathRoot(Path.GetFullPath(_options.DataRoot));
		var available = root is null ? long.MaxValue : new DriveInfo(root).AvailableFreeSpace;
		if (available - requestedBytes < _options.MinimumFreeBytes)
		{
			throw new ReleaseStoreException("There is insufficient disk space for this release.", StatusCodes.Status507InsufficientStorage);
		}
	}

	private static async Task CopyExactlyAsync(Stream source, Stream target, long length, CancellationToken cancellationToken)
	{
		var buffer = new byte[81920];
		var remaining = length;
		while (remaining > 0)
		{
			var read = await source.ReadAsync(buffer.AsMemory(0, (int)Math.Min(buffer.Length, remaining)), cancellationToken);
			if (read == 0)
			{
				throw new ReleaseStoreException("The chunk body ended before its declared range.", StatusCodes.Status400BadRequest);
			}
			await target.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
			remaining -= read;
		}
		if (await source.ReadAsync(buffer.AsMemory(0, 1), cancellationToken) != 0)
		{
			throw new ReleaseStoreException("The chunk body exceeds its declared range.", StatusCodes.Status400BadRequest);
		}
	}

	private static async Task<string> ComputeSha256Async(string path, CancellationToken cancellationToken)
	{
		await using var stream = File.OpenRead(path);
		return Convert.ToHexString(await SHA256.HashDataAsync(stream, cancellationToken)).ToLowerInvariant();
	}

	private static string NormaliseDigest(string digest)
	{
		var value = digest.Trim();
		if (value.StartsWith("sha-256=", StringComparison.OrdinalIgnoreCase))
		{
			value = value[8..].Trim(':', '"');
			try
			{
				return Convert.ToHexString(Convert.FromBase64String(value)).ToLowerInvariant();
			}
			catch (FormatException)
			{
				return value.ToLowerInvariant();
			}
		}
		return value.ToLowerInvariant();
	}

	private IEnumerable<ReleaseArtifactRequest> AllArtifacts(StagedRelease release) => release.Artifacts
		.Append(release.DocumentationCatalogue)
		.Where(artifact => artifact is not null)
		.Cast<ReleaseArtifactRequest>();

	private ReleaseArtifactRequest FindArtifact(StagedRelease release, string artifactId) => AllArtifacts(release)
		.FirstOrDefault(artifact => artifact.ArtifactId == artifactId)
		?? throw new ReleaseStoreException("Artifact not found.", StatusCodes.Status404NotFound);

	private async Task<StagedRelease> RequireDraftAsync(string uploadId, CancellationToken cancellationToken) =>
		await ReadStagedReleaseAsync(uploadId, cancellationToken)
		?? throw new ReleaseStoreException("Release draft not found.", StatusCodes.Status404NotFound);

	private async Task<StagedRelease?> ReadStagedReleaseAsync(string uploadId, CancellationToken cancellationToken)
	{
		var path = Path.Combine(GetDraftPath(uploadId), "release.json");
		if (!File.Exists(path))
		{
			return null;
		}
		await using var stream = File.OpenRead(path);
		return await JsonSerializer.DeserializeAsync<StagedRelease>(stream, _jsonOptions, cancellationToken);
	}

	private Task WriteStagedReleaseAsync(StagedRelease release, CancellationToken cancellationToken) =>
		WriteJsonAsync(Path.Combine(GetDraftPath(release.UploadId), "release.json"), release, cancellationToken);

	private async Task WriteJsonAsync<T>(string path, T value, CancellationToken cancellationToken)
	{
		var temporaryPath = $"{path}.{Guid.NewGuid():N}.tmp";
		try
		{
			await using (var stream = new FileStream(temporaryPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, FileOptions.Asynchronous | FileOptions.WriteThrough))
			{
				await JsonSerializer.SerializeAsync(stream, value, _jsonOptions, cancellationToken);
				await stream.FlushAsync(cancellationToken);
			}
			File.Move(temporaryPath, path, true);
		}
		finally
		{
			if (File.Exists(temporaryPath))
			{
				File.Delete(temporaryPath);
			}
		}
	}

	private string GetDraftPath(string uploadId) => Path.Combine(StagingRoot, uploadId);
	private string GetChunksPath(string uploadId) => Path.Combine(GetDraftPath(uploadId), "chunks");
	private string GetChunkPath(string uploadId, string artifactId, int index) => Path.Combine(GetChunksPath(uploadId), $"{artifactId}.{index:D6}.chunk");

	private static string ValidateUploadId(string uploadId) => UploadIdRegex().IsMatch(uploadId)
		? uploadId
		: throw new ReleaseStoreException("Invalid upload identifier.", StatusCodes.Status400BadRequest);

	private static string ValidateIdentifier(string value, string name) => IdentifierRegex().IsMatch(value)
		? value
		: throw new ReleaseStoreException($"Invalid {name}.", StatusCodes.Status400BadRequest);

	[GeneratedRegex("^[a-f0-9]{32}$", RegexOptions.CultureInvariant)]
	private static partial Regex UploadIdRegex();
	[GeneratedRegex("^[a-z0-9][a-z0-9._-]{0,63}$", RegexOptions.CultureInvariant)]
	private static partial Regex IdentifierRegex();
	[GeneratedRegex("^\\d+\\.\\d+\\.\\d+$", RegexOptions.CultureInvariant)]
	private static partial Regex VersionRegex();
	[GeneratedRegex("^[a-fA-F0-9]{40}$", RegexOptions.CultureInvariant)]
	private static partial Regex CommitRegex();
	[GeneratedRegex("^[a-fA-F0-9]{64}$", RegexOptions.CultureInvariant)]
	private static partial Regex ShaRegex();
	[GeneratedRegex("^[A-Za-z0-9][A-Za-z0-9._-]{0,199}$", RegexOptions.CultureInvariant)]
	private static partial Regex FileNameRegex();
}

public sealed class ReleaseStoreException : Exception
{
	public ReleaseStoreException(string message, int statusCode) : base(message)
	{
		StatusCode = statusCode;
	}

	public int StatusCode { get; }
}
