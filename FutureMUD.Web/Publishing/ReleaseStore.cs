#nullable enable

using FutureMUD.Web.Configuration;
using Microsoft.Extensions.Options;
using MudSharp.Documentation;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace FutureMUD.Web.Publishing;

public sealed partial class ReleaseStore
{
	public const int ChunkSize = 32 * 1024 * 1024;
	private const long MaximumArtifactSize = 4L * 1024 * 1024 * 1024;
	private const long MaximumDocumentationCatalogueSize = 32L * 1024 * 1024;
	private const int MaximumDocumentationEntries = 50_000;
	private const int MaximumDocumentationNestedEntries = 250_000;
	private const int MaximumDocumentationStringLength = 256 * 1024;
	private const long MaximumDocumentationCharacters = 16L * 1024 * 1024;
	private const string PromotionPrepared = "prepared";
	private const string PromotionRollingBack = "rolling-back";
	private const string PromotionRolledBack = "rolled-back";
	private const string PromotionCommitted = "committed";
	private readonly FutureMudWebOptions _options;
	private readonly ReleaseProductCatalogue _products;
	private readonly ILogger<ReleaseStore> _logger;
	private readonly bool _initialised;
	private readonly ConcurrentDictionary<string, UploadLock> _locks = new(StringComparer.Ordinal);
	private readonly SemaphoreSlim _releaseMutationLock = new(1, 1);
	private readonly SemaphoreSlim _storageAllocationLock = new(1, 1);
	private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
	{
		MaxDepth = 32,
		WriteIndented = true
	};

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
			VerifyStorageWriteAccess();
			_initialised = true;
		}
		catch (Exception exception)
		{
			_logger.LogError(exception, "Release storage initialisation failed.");
			_initialised = false;
		}
	}

	public bool IsReady
	{
		get
		{
			if (!_initialised)
			{
				return false;
			}
			try
			{
				EnsureDiskSpace(0);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}

	public string StagingRoot => Path.Combine(_options.DataRoot, "staging");
	public string LiveRoot => Path.Combine(_options.DataRoot, "releases", "live");
	public string PreviousRoot => Path.Combine(_options.DataRoot, "releases", "previous");
	public string DocumentationRoot => Path.Combine(_options.DataRoot, "documentation", "live");
	private string PromotionJournalPath => Path.Combine(_options.DataRoot, "releases", "promotion-journal.json");

	public async Task<StagedRelease> CreateOrResumeAsync(CreateReleaseRequest request, CancellationToken cancellationToken)
	{
		ValidateManifest(request);
		await _releaseMutationLock.WaitAsync(cancellationToken);
		try
		{
			var liveRelease = await GetLiveReleaseAsync(request.Product, cancellationToken);
			if (liveRelease is not null && CompareVersions(request.Version, liveRelease.Version) <= 0)
			{
				throw new ReleaseStoreException(
					"Release versions must increase and a published version can never be replaced.",
					StatusCodes.Status409Conflict);
			}

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

			var uploadedBytes = request.Artifacts.Sum(artifact => artifact.Size) + (request.DocumentationCatalogue?.Size ?? 0);
			EnsureDiskSpace(checked(uploadedBytes * 2));
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
		finally
		{
			_releaseMutationLock.Release();
		}
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
		EnsureDraftExists(uploadId);
		var gate = await AcquireUploadLockAsync(uploadId, cancellationToken);
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
			await _storageAllocationLock.WaitAsync(cancellationToken);
			try
			{
				EnsureDiskSpace(expectedLength);
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
			}
			finally
			{
				_storageAllocationLock.Release();
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
			gate.Dispose();
		}
	}

	public async Task<StagedRelease> CompleteAsync(string uploadId, CancellationToken cancellationToken)
	{
		uploadId = ValidateUploadId(uploadId);
		EnsureDraftExists(uploadId);
		var gate = await AcquireUploadLockAsync(uploadId, cancellationToken);
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
			await _storageAllocationLock.WaitAsync(cancellationToken);
			try
			{
				if (Directory.Exists(outputDirectory))
				{
					Directory.Delete(outputDirectory, true);
				}
				EnsureDiskSpace(checked(AllArtifacts(release).Sum(artifact => artifact.Size)));
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
			}
			finally
			{
				_storageAllocationLock.Release();
			}
			release.Status = "validated";
			await WriteStagedReleaseAsync(release, cancellationToken);
			_logger.LogInformation("Publishing draft {UploadId} validated.", uploadId);
			return release;
		}
		finally
		{
			gate.Dispose();
		}
	}

	public async Task<PublicRelease> PromoteAsync(string uploadId, CancellationToken cancellationToken)
	{
		uploadId = ValidateUploadId(uploadId);
		EnsureDraftExists(uploadId);
		var gate = await AcquireUploadLockAsync(uploadId, cancellationToken);
		var releaseMutationLockAcquired = false;
		try
		{
			await _releaseMutationLock.WaitAsync(cancellationToken);
			releaseMutationLockAcquired = true;
			var staged = await RequireDraftAsync(uploadId, cancellationToken);
			if (staged.Status != "validated")
			{
				throw new ReleaseStoreException("Only a validated release can be promoted.", StatusCodes.Status409Conflict);
			}
			var liveRelease = await GetLiveReleaseAsync(staged.Product, cancellationToken);
			if (liveRelease is not null && CompareVersions(staged.Version, liveRelease.Version) <= 0)
			{
				throw new ReleaseStoreException(
					"Release versions must increase and a published version can never be replaced.",
					StatusCodes.Status409Conflict);
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

			string? documentationTemporary = null;
			string? documentationPrevious = null;
			if (staged.DocumentationCatalogue is not null)
			{
				documentationTemporary = $"{DocumentationRoot}.new";
				documentationPrevious = $"{DocumentationRoot}.previous";
				await _storageAllocationLock.WaitAsync(cancellationToken);
				try
				{
					if (Directory.Exists(documentationTemporary))
					{
						Directory.Delete(documentationTemporary, true);
					}
					EnsureDiskSpace(staged.DocumentationCatalogue.Size);
					Directory.CreateDirectory(documentationTemporary);
					File.Copy(
						Path.Combine(candidate, staged.DocumentationCatalogue.FileName),
						Path.Combine(documentationTemporary, "catalogue.json"));
				}
				catch
				{
					if (Directory.Exists(documentationTemporary))
					{
						Directory.Delete(documentationTemporary, true);
					}
					throw;
				}
				finally
				{
					_storageAllocationLock.Release();
				}
			}
			var live = Path.Combine(LiveRoot, staged.Product);
			var previous = Path.Combine(PreviousRoot, staged.Product);
			var journal = new PromotionJournal
			{
				UploadId = uploadId,
				Product = staged.Product,
				State = PromotionPrepared,
				HadLiveRelease = Directory.Exists(live),
				HasDocumentation = staged.DocumentationCatalogue is not null,
				HadDocumentation = Directory.Exists(DocumentationRoot)
			};
			try
			{
				if (Directory.Exists(previous))
				{
					Directory.Delete(previous, true);
				}
				if (documentationPrevious is not null && Directory.Exists(documentationPrevious))
				{
					Directory.Delete(documentationPrevious, true);
				}
				WritePromotionJournal(journal);

				if (Directory.Exists(live))
				{
					Directory.Move(live, previous);
					Directory.SetLastWriteTimeUtc(previous, DateTime.UtcNow);
				}
				Directory.Move(candidate, live);

				if (documentationTemporary is not null && documentationPrevious is not null)
				{
					if (Directory.Exists(DocumentationRoot))
					{
						Directory.Move(DocumentationRoot, documentationPrevious);
					}
					Directory.Move(documentationTemporary, DocumentationRoot);
				}

				staged.Status = "promoted";
				await WriteStagedReleaseAsync(staged, CancellationToken.None);
				journal.State = PromotionCommitted;
				WritePromotionJournal(journal);
				File.Delete(PromotionJournalPath);
			}
			catch (Exception exception)
			{
				try
				{
					if (File.Exists(PromotionJournalPath))
					{
						if (journal.State == PromotionCommitted)
						{
							CompleteCommittedPromotion(journal);
						}
						else
						{
							RollbackPromotion(journal);
						}
					}
					else if (documentationTemporary is not null && Directory.Exists(documentationTemporary))
					{
						Directory.Delete(documentationTemporary, true);
					}
				}
				catch (Exception rollbackException)
				{
					_logger.LogCritical(
						rollbackException,
						"Promotion rollback failed for draft {UploadId} after {Error}.",
						uploadId,
						exception.Message);
				}
				throw;
			}

			_logger.LogInformation("Publishing draft {UploadId} promoted as {Product} {Version}.", uploadId, staged.Product, staged.Version);
			return publicRelease;
		}
		finally
		{
			if (releaseMutationLockAcquired)
			{
				_releaseMutationLock.Release();
			}
			gate.Dispose();
		}
	}

	public async Task AbandonAsync(string uploadId, CancellationToken cancellationToken)
	{
		uploadId = ValidateUploadId(uploadId);
		EnsureDraftExists(uploadId);
		var gate = await AcquireUploadLockAsync(uploadId, cancellationToken);
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
			gate.Dispose();
		}
	}

	public async Task<PublicRelease?> GetLiveReleaseAsync(string product, CancellationToken cancellationToken)
	{
		if (!IdentifierRegex().IsMatch(product))
		{
			return null;
		}
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
		if (!_releaseMutationLock.Wait(0))
		{
			return;
		}
		try
		{
			foreach (var directory in Directory.EnumerateDirectories(StagingRoot))
			{
				var uploadId = Path.GetFileName(directory);
				var gate = TryAcquireUploadLock(uploadId);
				if (gate is null)
				{
					continue;
				}
				try
				{
					if (!Directory.Exists(directory))
					{
						continue;
					}
					var metadata = Path.Combine(directory, "release.json");
					var lastActivity = File.Exists(metadata)
						? File.GetLastWriteTimeUtc(metadata)
						: Directory.GetLastWriteTimeUtc(directory);
					if (now - lastActivity > _options.DraftLifetime)
					{
						Directory.Delete(directory, true);
					}
				}
				finally
				{
					gate.Dispose();
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
		finally
		{
			_releaseMutationLock.Release();
		}
	}

	private void RecoverInterruptedPromotions()
	{
		if (File.Exists(PromotionJournalPath))
		{
			var journal = ReadPromotionJournal();
			switch (journal.State)
			{
				case PromotionPrepared:
				case PromotionRollingBack:
					RollbackPromotion(journal);
					break;
				case PromotionRolledBack:
					CompleteRollbackCleanup(journal);
					break;
				case PromotionCommitted:
					CompleteCommittedPromotion(journal);
					break;
				default:
					throw new InvalidDataException("The promotion journal state is invalid.");
			}
		}

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

	private PromotionJournal ReadPromotionJournal()
	{
		using var stream = File.OpenRead(PromotionJournalPath);
		var journal = JsonSerializer.Deserialize<PromotionJournal>(stream, _jsonOptions);
		if (journal is null || !UploadIdRegex().IsMatch(journal.UploadId) || !IdentifierRegex().IsMatch(journal.Product) ||
			journal.State is not (PromotionPrepared or PromotionRollingBack or PromotionRolledBack or PromotionCommitted))
		{
			throw new InvalidDataException("The promotion journal is invalid.");
		}
		return journal;
	}

	private void RollbackPromotion(PromotionJournal journal)
	{
		journal.State = PromotionRollingBack;
		WritePromotionJournal(journal);
		var staged = RequirePromotionDraft(journal);
		var candidate = Path.Combine(GetDraftPath(journal.UploadId), "release");
		var live = Path.Combine(LiveRoot, journal.Product);
		var previous = Path.Combine(PreviousRoot, journal.Product);

		if (!Directory.Exists(candidate))
		{
			if (!Directory.Exists(live) || journal.HadLiveRelease && !Directory.Exists(previous))
			{
				throw new InvalidDataException("The interrupted promotion does not have enough state for rollback.");
			}
			Directory.Move(live, candidate);
		}
		if (journal.HadLiveRelease)
		{
			if (!Directory.Exists(live))
			{
				if (!Directory.Exists(previous))
				{
					throw new InvalidDataException("The prior live release required for rollback is missing.");
				}
				Directory.Move(previous, live);
			}
		}
		else if (Directory.Exists(live))
		{
			throw new InvalidDataException("An unexpected live release prevents safe promotion rollback.");
		}

		if (journal.HasDocumentation)
		{
			RollbackDocumentation(journal);
		}

		staged.Status = "validated";
		WriteStagedReleaseSynchronously(staged);
		journal.State = PromotionRolledBack;
		WritePromotionJournal(journal);
		CompleteRollbackCleanup(journal);
	}

	private void RollbackDocumentation(PromotionJournal journal)
	{
		var documentationTemporary = $"{DocumentationRoot}.new";
		var documentationPrevious = $"{DocumentationRoot}.previous";
		if (Directory.Exists(documentationTemporary))
		{
			if (!Directory.Exists(DocumentationRoot) && journal.HadDocumentation)
			{
				if (!Directory.Exists(documentationPrevious))
				{
					throw new InvalidDataException("The prior documentation required for rollback is missing.");
				}
				Directory.Move(documentationPrevious, DocumentationRoot);
			}
			return;
		}

		if (Directory.Exists(documentationPrevious))
		{
			if (!Directory.Exists(DocumentationRoot))
			{
				throw new InvalidDataException("The promoted documentation required for rollback is missing.");
			}
			Directory.Move(DocumentationRoot, documentationTemporary);
			Directory.Move(documentationPrevious, DocumentationRoot);
			return;
		}

		if (!journal.HadDocumentation && Directory.Exists(DocumentationRoot))
		{
			Directory.Move(DocumentationRoot, documentationTemporary);
			return;
		}

		throw new InvalidDataException("The interrupted documentation promotion cannot be rolled back safely.");
	}

	private void CompleteRollbackCleanup(PromotionJournal journal)
	{
		var documentationTemporary = $"{DocumentationRoot}.new";
		if (journal.HasDocumentation && Directory.Exists(documentationTemporary))
		{
			Directory.Delete(documentationTemporary, true);
		}
		File.Delete(PromotionJournalPath);
	}

	private void CompleteCommittedPromotion(PromotionJournal journal)
	{
		var staged = RequirePromotionDraft(journal);
		var candidate = Path.Combine(GetDraftPath(journal.UploadId), "release");
		var live = Path.Combine(LiveRoot, journal.Product);
		var liveMetadataPath = Path.Combine(live, "release.json");
		if (Directory.Exists(candidate) || !File.Exists(liveMetadataPath))
		{
			throw new InvalidDataException("The committed promotion does not have a complete live release.");
		}
		using (var stream = File.OpenRead(liveMetadataPath))
		{
			var publicRelease = JsonSerializer.Deserialize<PublicRelease>(stream, _jsonOptions);
			if (publicRelease is null || publicRelease.Product != staged.Product ||
				publicRelease.Version != staged.Version || publicRelease.SourceCommit != staged.SourceCommit)
			{
				throw new InvalidDataException("The committed live release does not match its draft.");
			}
		}
		if (journal.HasDocumentation &&
			(!File.Exists(Path.Combine(DocumentationRoot, "catalogue.json")) || Directory.Exists($"{DocumentationRoot}.new")))
		{
			throw new InvalidDataException("The committed promotion does not have complete live documentation.");
		}

		if (staged.Status != "promoted")
		{
			staged.Status = "promoted";
			WriteStagedReleaseSynchronously(staged);
		}
		File.Delete(PromotionJournalPath);
	}

	private StagedRelease RequirePromotionDraft(PromotionJournal journal)
	{
		var path = Path.Combine(GetDraftPath(journal.UploadId), "release.json");
		if (!File.Exists(path))
		{
			throw new InvalidDataException("The promotion journal draft is missing.");
		}
		using var stream = File.OpenRead(path);
		var staged = JsonSerializer.Deserialize<StagedRelease>(stream, _jsonOptions);
		if (staged is null || staged.UploadId != journal.UploadId || staged.Product != journal.Product ||
			(staged.DocumentationCatalogue is not null) != journal.HasDocumentation)
		{
			throw new InvalidDataException("The promotion journal does not match its draft.");
		}
		return staged;
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
			!string.Equals(catalogue.SourceRevision, release.SourceCommit, StringComparison.OrdinalIgnoreCase) ||
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

		ValidateDocumentationCatalogueBounds(catalogue);
	}

	private static void ValidateDocumentationCatalogueBounds(DocumentationCatalogue catalogue)
	{
		long characterCount = 0;
		long nestedEntryCount = 0;

		[DoesNotReturn]
		static void Reject() => throw new ReleaseStoreException(
			"The documentation catalogue exceeds its structural safety limits or contains null values.",
			StatusCodes.Status422UnprocessableEntity);

		void AddString(string? value, bool required = false)
		{
			if (value is null || value.Length > MaximumDocumentationStringLength ||
				(required && string.IsNullOrWhiteSpace(value)))
			{
				Reject();
			}
			characterCount += value.Length;
			if (characterCount > MaximumDocumentationCharacters)
			{
				Reject();
			}
		}

		void AddNestedEntries(int count)
		{
			nestedEntryCount += count;
			if (nestedEntryCount > MaximumDocumentationNestedEntries)
			{
				Reject();
			}
		}

		void AddStrings(IReadOnlyList<string>? values, int maximumCount)
		{
			if (values is null || values.Count > maximumCount)
			{
				Reject();
			}
			AddNestedEntries(values.Count);
			foreach (var value in values)
			{
				AddString(value);
			}
		}

		var entryCount = (long)catalogue.Commands.Count + catalogue.ProgFunctions.Count +
			catalogue.ProgTypes.Count + catalogue.CollectionExtensions.Count + catalogue.ItemComponents.Count;
		if (entryCount > MaximumDocumentationEntries)
		{
			Reject();
		}

		AddString(catalogue.EngineVersion, true);
		AddString(catalogue.SourceRevision, true);
		foreach (var command in catalogue.Commands)
		{
			if (command is null || command.ConditionalHelp is null || command.ConditionalHelp.Count > 256)
			{
				Reject();
			}
			AddString(command.Slug, true);
			AddString(command.Name, true);
			AddString(command.Module);
			AddString(command.PermissionLevel);
			AddString(command.Audience);
			AddString(command.DefaultHelp);
			AddString(command.AdminHelp);
			AddStrings(command.CommandWords, 64);
			AddNestedEntries(command.ConditionalHelp.Count);
			if (nestedEntryCount > MaximumDocumentationNestedEntries)
			{
				Reject();
			}
			foreach (var conditional in command.ConditionalHelp)
			{
				if (conditional is null)
				{
					Reject();
				}
				AddString(conditional.Condition);
				AddString(conditional.Help);
			}
		}

		foreach (var function in catalogue.ProgFunctions)
		{
			if (function is null || function.Overloads is null || function.Overloads.Count > 256)
			{
				Reject();
			}
			AddString(function.Slug, true);
			AddString(function.Name, true);
			AddString(function.Category);
			AddNestedEntries(function.Overloads.Count);
			foreach (var overload in function.Overloads)
			{
				if (overload is null || overload.Parameters is null || overload.Parameters.Count > 128)
				{
					Reject();
				}
				AddString(overload.ReturnType);
				AddString(overload.Help);
				AddStrings(overload.Contexts, 128);
				AddNestedEntries(overload.Parameters.Count);
				foreach (var parameter in overload.Parameters)
				{
					if (parameter is null)
					{
						Reject();
					}
					AddString(parameter.Name);
					AddString(parameter.Type);
				}
			}
		}

		foreach (var type in catalogue.ProgTypes)
		{
			if (type is null || type.Properties is null || type.Properties.Count > 4_096)
			{
				Reject();
			}
			AddString(type.Slug, true);
			AddString(type.Name, true);
			AddNestedEntries(type.Properties.Count);
			foreach (var property in type.Properties)
			{
				if (property is null)
				{
					Reject();
				}
				AddString(property.Name);
				AddString(property.Type);
				AddString(property.Help);
			}
		}

		foreach (var extension in catalogue.CollectionExtensions)
		{
			if (extension is null)
			{
				Reject();
			}
			AddString(extension.Slug, true);
			AddString(extension.Name, true);
			AddString(extension.ReturnType);
			AddString(extension.Help);
			AddStrings(extension.Contexts, 128);
		}

		foreach (var component in catalogue.ItemComponents)
		{
			if (component is null)
			{
				Reject();
			}
			AddString(component.Slug, true);
			AddString(component.Name, true);
			AddString(component.Blurb);
			AddString(component.BuilderHelp);
		}

		if (nestedEntryCount > MaximumDocumentationNestedEntries)
		{
			Reject();
		}
	}

	private void ValidateManifest(CreateReleaseRequest request)
	{
		if (request.Artifacts is null || request.Artifacts.Any(artifact => artifact is null))
		{
			throw new ReleaseStoreException("The artifact manifest is required.", StatusCodes.Status400BadRequest);
		}
		if (string.IsNullOrWhiteSpace(request.Product) || !_products.TryGet(request.Product, out var product))
		{
			throw new ReleaseStoreException("Unknown product.", StatusCodes.Status400BadRequest);
		}
		if (string.IsNullOrWhiteSpace(request.Version) || string.IsNullOrWhiteSpace(request.SourceCommit) ||
			request.Version.Length > 32 || !VersionRegex().IsMatch(request.Version) ||
			!Version.TryParse(request.Version, out _) || !CommitRegex().IsMatch(request.SourceCommit))
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
			if (string.IsNullOrWhiteSpace(artifact.ArtifactId) || string.IsNullOrWhiteSpace(artifact.Runtime) ||
				string.IsNullOrWhiteSpace(artifact.FileName) || string.IsNullOrWhiteSpace(artifact.Sha256))
			{
				throw new ReleaseStoreException("An artifact manifest entry is invalid.", StatusCodes.Status400BadRequest);
			}
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
			 request.DocumentationCatalogue.FileName != "catalogue.json" ||
			 request.DocumentationCatalogue.Size > MaximumDocumentationCatalogueSize))
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

	private static int CompareVersions(string left, string right) => Version.Parse(left).CompareTo(Version.Parse(right));

	private void VerifyStorageWriteAccess()
	{
		var probe = Path.Combine(_options.DataRoot, $".write-probe-{Guid.NewGuid():N}.tmp");
		try
		{
			using var stream = new FileStream(probe, FileMode.CreateNew, FileAccess.Write, FileShare.None, 1, FileOptions.WriteThrough);
			stream.WriteByte(0);
			stream.Flush(true);
		}
		finally
		{
			if (File.Exists(probe))
			{
				File.Delete(probe);
			}
		}
	}

	private void EnsureDiskSpace(long requestedBytes)
	{
		var dataRoot = Path.GetFullPath(_options.DataRoot);
		var drive = DriveInfo.GetDrives()
			.Where(candidate => IsPathWithin(dataRoot, candidate.RootDirectory.FullName))
			.OrderByDescending(candidate => candidate.RootDirectory.FullName.Length)
			.FirstOrDefault();
		if (drive is null || !drive.IsReady)
		{
			throw new ReleaseStoreException("Release storage capacity could not be determined.", StatusCodes.Status507InsufficientStorage);
		}
		var available = drive.AvailableFreeSpace;
		if (available - requestedBytes < _options.MinimumFreeBytes)
		{
			throw new ReleaseStoreException("There is insufficient disk space for this release.", StatusCodes.Status507InsufficientStorage);
		}
	}

	private static bool IsPathWithin(string path, string root)
	{
		var relative = Path.GetRelativePath(root, path);
		return !Path.IsPathRooted(relative) &&
			relative != ".." &&
			!relative.StartsWith($"..{Path.DirectorySeparatorChar}", StringComparison.Ordinal) &&
			!relative.StartsWith($"..{Path.AltDirectorySeparatorChar}", StringComparison.Ordinal);
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

	private async Task<UploadLockLease> AcquireUploadLockAsync(string uploadId, CancellationToken cancellationToken)
	{
		while (true)
		{
			var uploadLock = _locks.GetOrAdd(uploadId, _ => new UploadLock());
			lock (uploadLock)
			{
				if (uploadLock.Retired)
				{
					continue;
				}
				uploadLock.References++;
			}
			try
			{
				await uploadLock.Semaphore.WaitAsync(cancellationToken);
				return new UploadLockLease(this, uploadId, uploadLock);
			}
			catch
			{
				ReleaseUploadLockReference(uploadId, uploadLock);
				throw;
			}
		}
	}

	private UploadLockLease? TryAcquireUploadLock(string uploadId)
	{
		while (true)
		{
			var uploadLock = _locks.GetOrAdd(uploadId, _ => new UploadLock());
			lock (uploadLock)
			{
				if (uploadLock.Retired)
				{
					continue;
				}
				uploadLock.References++;
			}
			if (uploadLock.Semaphore.Wait(0))
			{
				return new UploadLockLease(this, uploadId, uploadLock);
			}
			ReleaseUploadLockReference(uploadId, uploadLock);
			return null;
		}
	}

	private void ReleaseUploadLock(string uploadId, UploadLock uploadLock)
	{
		uploadLock.Semaphore.Release();
		ReleaseUploadLockReference(uploadId, uploadLock);
	}

	private void ReleaseUploadLockReference(string uploadId, UploadLock uploadLock)
	{
		lock (uploadLock)
		{
			uploadLock.References--;
			if (uploadLock.References != 0)
			{
				return;
			}
			uploadLock.Retired = true;
			if (_locks.TryRemove(new KeyValuePair<string, UploadLock>(uploadId, uploadLock)))
			{
				uploadLock.Semaphore.Dispose();
			}
		}
	}

	private void EnsureDraftExists(string uploadId)
	{
		if (!File.Exists(Path.Combine(GetDraftPath(uploadId), "release.json")))
		{
			throw new ReleaseStoreException("Release draft not found.", StatusCodes.Status404NotFound);
		}
	}

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

	private void WritePromotionJournal(PromotionJournal journal) =>
		WriteJsonSynchronously(PromotionJournalPath, journal);

	private void WriteStagedReleaseSynchronously(StagedRelease release) =>
		WriteJsonSynchronously(Path.Combine(GetDraftPath(release.UploadId), "release.json"), release);

	private void WriteJsonSynchronously<T>(string path, T value)
	{
		var temporaryPath = $"{path}.{Guid.NewGuid():N}.tmp";
		try
		{
			using (var stream = new FileStream(temporaryPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, FileOptions.WriteThrough))
			{
				JsonSerializer.Serialize(stream, value, _jsonOptions);
				stream.Flush(true);
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

	private sealed class UploadLock
	{
		public SemaphoreSlim Semaphore { get; } = new(1, 1);
		public int References { get; set; }
		public bool Retired { get; set; }
	}

	private sealed class UploadLockLease : IDisposable
	{
		private ReleaseStore? _owner;
		private readonly string _uploadId;
		private readonly UploadLock _uploadLock;

		public UploadLockLease(ReleaseStore owner, string uploadId, UploadLock uploadLock)
		{
			_owner = owner;
			_uploadId = uploadId;
			_uploadLock = uploadLock;
		}

		public void Dispose()
		{
			var owner = Interlocked.Exchange(ref _owner, null);
			owner?.ReleaseUploadLock(_uploadId, _uploadLock);
		}
	}

	private sealed class PromotionJournal
	{
		public string UploadId { get; init; } = string.Empty;
		public string Product { get; init; } = string.Empty;
		public string State { get; set; } = PromotionPrepared;
		public bool HadLiveRelease { get; init; }
		public bool HasDocumentation { get; init; }
		public bool HadDocumentation { get; init; }
	}

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
