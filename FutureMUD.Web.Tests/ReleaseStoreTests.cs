#nullable enable

using FutureMUD.Web.Configuration;
using FutureMUD.Web.Publishing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace FutureMUD.Web.Tests;

[TestClass]
public sealed class ReleaseStoreTests
{
	private string _root = null!;
	private ReleaseStore _store = null!;

	[TestInitialize]
	public void Initialise()
	{
		_root = Path.Combine(Path.GetTempPath(), $"futuremud-web-tests-{Guid.NewGuid():N}");
		Directory.CreateDirectory(_root);
		_store = CreateStore(new FutureMudWebOptions { DataRoot = _root, MinimumFreeBytes = 0 });
	}

	[TestCleanup]
	public void Cleanup()
	{
		if (Directory.Exists(_root))
		{
			Directory.Delete(_root, true);
		}
	}

	[TestMethod]
	public async Task DraftIsIdempotentChunkIsResumableAndPromotionIsAtomic()
	{
		var bytes = new byte[] { 1, 2, 3, 4, 5 };
		var sha = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
		var request = new CreateReleaseRequest
		{
			Product = "terrainplanner",
			Version = "1.2.3",
			SourceCommit = new string('a', 40),
			Artifacts =
			[
				new ReleaseArtifactRequest
				{
					ArtifactId = "win-x64",
					Runtime = "win-x64",
					FileName = "terrainplanner-1.2.3-win-x64.zip",
					Size = bytes.Length,
					Sha256 = sha
				}
			]
		};

		var first = await _store.CreateOrResumeAsync(request, CancellationToken.None);
		var resumed = await _store.CreateOrResumeAsync(request, CancellationToken.None);
		Assert.AreEqual(first.UploadId, resumed.UploadId);
		var conflictingRequest = new CreateReleaseRequest
		{
			Product = request.Product,
			Version = request.Version,
			SourceCommit = request.SourceCommit,
			Artifacts = [new ReleaseArtifactRequest
			{
				ArtifactId = "win-x64",
				Runtime = "win-x64",
				FileName = request.Artifacts[0].FileName,
				Size = request.Artifacts[0].Size,
				Sha256 = new string('0', 64)
			}]
		};
		var conflict = await Assert.ThrowsExceptionAsync<ReleaseStoreException>(() => _store.CreateOrResumeAsync(conflictingRequest, CancellationToken.None));
		Assert.AreEqual(StatusCodes.Status409Conflict, conflict.StatusCode);

		await using var body = new MemoryStream(bytes);
		await _store.PutChunkAsync(first.UploadId, "win-x64", 0, 0, 4, 5, sha, body, CancellationToken.None);
		await using var duplicateBody = new MemoryStream(bytes);
		var afterDuplicate = await _store.PutChunkAsync(first.UploadId, "win-x64", 0, 0, 4, 5, sha, duplicateBody, CancellationToken.None);
		CollectionAssert.AreEqual(new[] { 0 }, afterDuplicate.ReceivedChunks["win-x64"]);

		await _store.CompleteAsync(first.UploadId, CancellationToken.None);
		var promoted = await _store.PromoteAsync(first.UploadId, CancellationToken.None);
		Assert.AreEqual("1.2.3", promoted.Version);
		Assert.IsTrue(File.Exists(_store.GetLiveArtifactPath("terrainplanner", request.Artifacts[0].FileName)));
	}

	[TestMethod]
	public async Task InvalidRangeAndHashAreRejected()
	{
		var bytes = new byte[] { 1, 2, 3 };
		var sha = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
		var request = new CreateReleaseRequest
		{
			Product = "terrainplanner",
			Version = "1.2.3",
			SourceCommit = new string('b', 40),
			Artifacts = [new ReleaseArtifactRequest { ArtifactId = "win-x64", Runtime = "win-x64", FileName = "terrainplanner-1.2.3-win-x64.zip", Size = 3, Sha256 = sha }]
		};
		var release = await _store.CreateOrResumeAsync(request, CancellationToken.None);

		await Assert.ThrowsExceptionAsync<ReleaseStoreException>(() => _store.PutChunkAsync(
			release.UploadId, "win-x64", 0, 1, 2, 3, sha, new MemoryStream(bytes), CancellationToken.None));
		await Assert.ThrowsExceptionAsync<ReleaseStoreException>(() => _store.PutChunkAsync(
			release.UploadId, "win-x64", 0, 0, 2, 3, new string('0', 64), new MemoryStream(bytes), CancellationToken.None));
		Assert.IsFalse(Directory.EnumerateFiles(_root, "*.tmp", SearchOption.AllDirectories).Any());
	}

	[TestMethod]
	public async Task InvalidDocumentationCatalogueCannotBeValidatedOrPromoted()
	{
		const string version = "1.2.3";
		var commit = new string('e', 40);
		var artifactBytes = new byte[] { 1, 2, 3 };
		var artifactSha = Convert.ToHexString(SHA256.HashData(artifactBytes)).ToLowerInvariant();
		var documentationBytes = "{}"u8.ToArray();
		var documentationSha = Convert.ToHexString(SHA256.HashData(documentationBytes)).ToLowerInvariant();
		var request = new CreateReleaseRequest
		{
			Product = "engine",
			Version = version,
			SourceCommit = commit,
			Artifacts = new[] { "win-x64", "linux-x64", "linux-arm64" }
				.Select(runtime => new ReleaseArtifactRequest
				{
					ArtifactId = runtime,
					Runtime = runtime,
					FileName = $"engine-{version}-{runtime}.zip",
					Size = artifactBytes.Length,
					Sha256 = artifactSha
				})
				.ToList(),
			DocumentationCatalogue = new ReleaseArtifactRequest
			{
				ArtifactId = "documentation",
				Runtime = "documentation",
				FileName = "catalogue.json",
				Size = documentationBytes.Length,
				Sha256 = documentationSha
			}
		};

		var release = await _store.CreateOrResumeAsync(request, CancellationToken.None);
		foreach (var artifact in request.Artifacts)
		{
			await _store.PutChunkAsync(release.UploadId, artifact.ArtifactId, 0, 0, artifactBytes.Length - 1,
				artifactBytes.Length, artifactSha, new MemoryStream(artifactBytes), CancellationToken.None);
		}
		await _store.PutChunkAsync(release.UploadId, "documentation", 0, 0, documentationBytes.Length - 1,
			documentationBytes.Length, documentationSha, new MemoryStream(documentationBytes), CancellationToken.None);

		var exception = await Assert.ThrowsExceptionAsync<ReleaseStoreException>(() =>
			_store.CompleteAsync(release.UploadId, CancellationToken.None));
		Assert.AreEqual(StatusCodes.Status422UnprocessableEntity, exception.StatusCode);
		Assert.IsFalse(Directory.Exists(Path.Combine(_store.LiveRoot, "engine")));
	}

	[TestMethod]
	public async Task ProductRuntimeFilenameSizeAndDiskAllowlistsAreEnforced()
	{
		var valid = CreateTerrainPlannerRequest("1.2.3", new byte[] { 1, 2, 3 }, 'c');
		var wrongName = new CreateReleaseRequest
		{
			Product = valid.Product,
			Version = valid.Version,
			SourceCommit = valid.SourceCommit,
			Artifacts = [new ReleaseArtifactRequest
			{
				ArtifactId = "win-x64",
				Runtime = "win-x64",
				FileName = "wrong.zip",
				Size = valid.Artifacts[0].Size,
				Sha256 = valid.Artifacts[0].Sha256
			}]
		};
		await Assert.ThrowsExceptionAsync<ReleaseStoreException>(() => _store.CreateOrResumeAsync(wrongName, CancellationToken.None));

		var noDiskStore = CreateStore(new FutureMudWebOptions { DataRoot = _root, MinimumFreeBytes = long.MaxValue });
		var diskException = await Assert.ThrowsExceptionAsync<ReleaseStoreException>(() => noDiskStore.CreateOrResumeAsync(valid, CancellationToken.None));
		Assert.AreEqual(StatusCodes.Status507InsufficientStorage, diskException.StatusCode);
	}

	[TestMethod]
	public async Task MissingChunksPreventAssemblyAndExpiredDraftsAreRemoved()
	{
		var expiringStore = CreateStore(new FutureMudWebOptions
		{
			DataRoot = _root,
			MinimumFreeBytes = 0,
			DraftLifetime = TimeSpan.Zero
		});
		var release = await expiringStore.CreateOrResumeAsync(CreateTerrainPlannerRequest("1.2.3", new byte[] { 4, 5, 6 }, 'd'), CancellationToken.None);
		await Assert.ThrowsExceptionAsync<ReleaseStoreException>(() => expiringStore.CompleteAsync(release.UploadId, CancellationToken.None));

		expiringStore.Cleanup(DateTimeOffset.UtcNow.AddMinutes(1));
		Assert.IsNull(await expiringStore.GetAsync(release.UploadId, CancellationToken.None));
	}

	[TestMethod]
	public void StartupRestoresAnInterruptedLiveDirectoryMove()
	{
		var moving = Path.Combine(_root, "releases", "live", "engine.moving");
		Directory.CreateDirectory(moving);
		File.WriteAllText(Path.Combine(moving, "marker"), "old release");

		var recovered = CreateStore(new FutureMudWebOptions { DataRoot = _root, MinimumFreeBytes = 0 });

		Assert.IsTrue(recovered.IsReady);
		Assert.IsTrue(File.Exists(Path.Combine(_root, "releases", "live", "engine", "marker")));
		Assert.IsFalse(Directory.Exists(moving));
	}

	[TestMethod]
	public void StartupRestoresPreviousReleaseWhenPromotionStoppedBetweenRenames()
	{
		var previous = Path.Combine(_root, "releases", "previous", "engine");
		Directory.CreateDirectory(previous);
		File.WriteAllText(Path.Combine(previous, "marker"), "old release");

		var recovered = CreateStore(new FutureMudWebOptions { DataRoot = _root, MinimumFreeBytes = 0 });

		Assert.IsTrue(recovered.IsReady);
		Assert.IsTrue(File.Exists(Path.Combine(_root, "releases", "live", "engine", "marker")));
		Assert.IsFalse(Directory.Exists(previous));
	}

	[TestMethod]
	public async Task StartupRollsBackPreparedPromotionAfterCandidateActivation()
	{
		var staged = await PrepareValidatedReleaseWithDocumentationAsync();
		var candidate = Path.Combine(_root, "staging", staged.UploadId, "release");
		var live = Path.Combine(_root, "releases", "live", staged.Product);
		var previous = Path.Combine(_root, "releases", "previous", staged.Product);
		Directory.Move(live, previous);
		Directory.Move(candidate, live);
		await WritePromotionJournalAsync(staged, "prepared");

		var recovered = CreateStore(new FutureMudWebOptions { DataRoot = _root, MinimumFreeBytes = 0 });

		Assert.IsTrue(recovered.IsReady);
		Assert.IsTrue(File.Exists(Path.Combine(live, "old-release.marker")));
		Assert.IsTrue(File.Exists(Path.Combine(candidate, staged.Artifacts[0].FileName)));
		Assert.AreEqual("old documentation", await File.ReadAllTextAsync(Path.Combine(recovered.DocumentationRoot, "catalogue.json")));
		Assert.IsFalse(Directory.Exists($"{recovered.DocumentationRoot}.new"));
		Assert.IsFalse(File.Exists(GetPromotionJournalPath()));
		Assert.AreEqual("validated", (await recovered.GetAsync(staged.UploadId, CancellationToken.None))?.Status);
	}

	[TestMethod]
	public async Task StartupRollsBackPreparedPromotionAfterDocumentationActivation()
	{
		var staged = await PrepareValidatedReleaseWithDocumentationAsync();
		var candidate = Path.Combine(_root, "staging", staged.UploadId, "release");
		var live = Path.Combine(_root, "releases", "live", staged.Product);
		var previous = Path.Combine(_root, "releases", "previous", staged.Product);
		Directory.Move(live, previous);
		Directory.Move(candidate, live);
		Directory.Move(_store.DocumentationRoot, $"{_store.DocumentationRoot}.previous");
		Directory.Move($"{_store.DocumentationRoot}.new", _store.DocumentationRoot);
		staged.Status = "promoted";
		await WriteTestJsonAsync(Path.Combine(_root, "staging", staged.UploadId, "release.json"), staged);
		await WritePromotionJournalAsync(staged, "prepared");

		var recovered = CreateStore(new FutureMudWebOptions { DataRoot = _root, MinimumFreeBytes = 0 });

		Assert.IsTrue(recovered.IsReady);
		Assert.IsTrue(File.Exists(Path.Combine(live, "old-release.marker")));
		Assert.IsTrue(File.Exists(Path.Combine(candidate, staged.Artifacts[0].FileName)));
		Assert.AreEqual("old documentation", await File.ReadAllTextAsync(Path.Combine(recovered.DocumentationRoot, "catalogue.json")));
		Assert.IsFalse(Directory.Exists($"{recovered.DocumentationRoot}.previous"));
		Assert.IsFalse(Directory.Exists($"{recovered.DocumentationRoot}.new"));
		Assert.IsFalse(File.Exists(GetPromotionJournalPath()));
		Assert.AreEqual("validated", (await recovered.GetAsync(staged.UploadId, CancellationToken.None))?.Status);
	}

	[TestMethod]
	public async Task StartupKeepsCommittedPromotionAndCompletesDraftStatus()
	{
		var staged = await PrepareValidatedReleaseWithDocumentationAsync();
		var candidate = Path.Combine(_root, "staging", staged.UploadId, "release");
		var live = Path.Combine(_root, "releases", "live", staged.Product);
		var previous = Path.Combine(_root, "releases", "previous", staged.Product);
		Directory.Move(live, previous);
		Directory.Move(candidate, live);
		Directory.Move(_store.DocumentationRoot, $"{_store.DocumentationRoot}.previous");
		Directory.Move($"{_store.DocumentationRoot}.new", _store.DocumentationRoot);
		await WritePromotionJournalAsync(staged, "committed");

		var recovered = CreateStore(new FutureMudWebOptions { DataRoot = _root, MinimumFreeBytes = 0 });

		Assert.IsTrue(recovered.IsReady);
		Assert.IsTrue(File.Exists(Path.Combine(live, staged.Artifacts[0].FileName)));
		Assert.IsTrue(File.Exists(Path.Combine(previous, "old-release.marker")));
		Assert.AreEqual("new documentation", await File.ReadAllTextAsync(Path.Combine(recovered.DocumentationRoot, "catalogue.json")));
		Assert.IsFalse(Directory.Exists(candidate));
		Assert.IsFalse(File.Exists(GetPromotionJournalPath()));
		Assert.AreEqual("promoted", (await recovered.GetAsync(staged.UploadId, CancellationToken.None))?.Status);
	}

	[TestMethod]
	public async Task CompletionDeletesStaleCandidateBeforeCheckingAssemblyCapacity()
	{
		var bytes = new byte[1024 * 1024];
		RandomNumberGenerator.Fill(bytes);
		var request = CreateTerrainPlannerRequest("3.0.0", bytes, 'a');
		var staged = await _store.CreateOrResumeAsync(request, CancellationToken.None);
		await _store.PutChunkAsync(
			staged.UploadId,
			request.Artifacts[0].ArtifactId,
			0,
			0,
			bytes.Length - 1,
			bytes.Length,
			request.Artifacts[0].Sha256,
			new MemoryStream(bytes),
			CancellationToken.None);
		var candidate = Path.Combine(_root, "staging", staged.UploadId, "release");
		Directory.CreateDirectory(candidate);
		await File.WriteAllBytesAsync(Path.Combine(candidate, "stale.bin"), new byte[8 * 1024 * 1024]);
		var available = GetAvailableFreeSpace();
		var constrained = CreateStore(new FutureMudWebOptions
		{
			DataRoot = _root,
			MinimumFreeBytes = available - bytes.Length / 2
		});

		var completed = await constrained.CompleteAsync(staged.UploadId, CancellationToken.None);

		Assert.AreEqual("validated", completed.Status);
		Assert.IsFalse(File.Exists(Path.Combine(candidate, "stale.bin")));
		Assert.IsTrue(File.Exists(Path.Combine(candidate, request.Artifacts[0].FileName)));
	}

	[TestMethod]
	public async Task ConcurrentUploadsCannotBothSpendTheSameFreeSpaceMargin()
	{
		var bytes = new byte[8 * 1024 * 1024];
		RandomNumberGenerator.Fill(bytes);
		var firstRequest = CreateTerrainPlannerRequest("4.0.0", bytes, 'b');
		var secondRequest = CreateTerrainPlannerRequest("4.0.1", bytes, 'c');
		var first = await _store.CreateOrResumeAsync(firstRequest, CancellationToken.None);
		var second = await _store.CreateOrResumeAsync(secondRequest, CancellationToken.None);
		var available = GetAvailableFreeSpace();
		var constrained = CreateStore(new FutureMudWebOptions
		{
			DataRoot = _root,
			MinimumFreeBytes = available - bytes.Length - bytes.Length / 2
		});
		var pausingBody = new PausingMemoryStream(bytes);
		var firstUpload = constrained.PutChunkAsync(
			first.UploadId, "win-x64", 0, 0, bytes.Length - 1, bytes.Length,
			firstRequest.Artifacts[0].Sha256, pausingBody, CancellationToken.None);
		await pausingBody.ReadStarted.Task.WaitAsync(TimeSpan.FromSeconds(5));
		var secondUpload = constrained.PutChunkAsync(
			second.UploadId, "win-x64", 0, 0, bytes.Length - 1, bytes.Length,
			secondRequest.Artifacts[0].Sha256, new MemoryStream(bytes), CancellationToken.None);
		pausingBody.ResumeReading.TrySetResult();

		await firstUpload;
		var exception = await Assert.ThrowsExceptionAsync<ReleaseStoreException>(() => secondUpload);
		Assert.AreEqual(StatusCodes.Status507InsufficientStorage, exception.StatusCode);
	}

	[TestMethod]
	public async Task UploadLocksAreReleasedAfterDraftChurn()
	{
		for (var index = 0; index < 32; index++)
		{
			var request = CreateTerrainPlannerRequest($"6.0.{index}", [1, 2, 3], "abcdef"[index % 6]);
			var staged = await _store.CreateOrResumeAsync(request, CancellationToken.None);
			await _store.AbandonAsync(staged.UploadId, CancellationToken.None);
		}

		var field = typeof(ReleaseStore).GetField("_locks", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
		var locks = field?.GetValue(_store) ?? throw new AssertFailedException("Upload lock registry was not found.");
		var count = (int)(locks.GetType().GetProperty("Count")?.GetValue(locks) ?? -1);
		Assert.AreEqual(0, count);
	}

	[TestMethod]
	public async Task ConcurrentIdenticalDraftCreationReturnsOneUpload()
	{
		var request = CreateTerrainPlannerRequest("1.2.3", [1, 2, 3], 'f');

		var releases = await Task.WhenAll(Enumerable.Range(0, 12)
			.Select(_ => _store.CreateOrResumeAsync(request, CancellationToken.None)));

		Assert.AreEqual(1, releases.Select(release => release.UploadId).Distinct(StringComparer.Ordinal).Count());
	}

	[TestMethod]
	public async Task ExistingDraftCanResumeWhenFreeSpaceFallsBelowNewDraftReserve()
	{
		var request = CreateTerrainPlannerRequest("1.2.3", [4, 5, 6], 'a');
		var original = await _store.CreateOrResumeAsync(request, CancellationToken.None);
		var constrainedStore = CreateStore(new FutureMudWebOptions
		{
			DataRoot = _root,
			MinimumFreeBytes = long.MaxValue
		});

		var resumed = await constrainedStore.CreateOrResumeAsync(request, CancellationToken.None);

		Assert.AreEqual(original.UploadId, resumed.UploadId);
	}

	[TestMethod]
	public async Task PublishedVersionCannotBeReusedOrDowngraded()
	{
		var bytes = new byte[] { 7, 8, 9 };
		var firstRequest = CreateTerrainPlannerRequest("1.2.3", bytes, 'a');
		var first = await _store.CreateOrResumeAsync(firstRequest, CancellationToken.None);
		await UploadCompleteAndPromoteAsync(first, firstRequest, bytes);

		foreach (var rejectedVersion in new[] { "1.2.3", "1.2.2" })
		{
			var exception = await Assert.ThrowsExceptionAsync<ReleaseStoreException>(() =>
				_store.CreateOrResumeAsync(
					CreateTerrainPlannerRequest(rejectedVersion, bytes, 'b'),
					CancellationToken.None));
			Assert.AreEqual(StatusCodes.Status409Conflict, exception.StatusCode);
		}

		var next = await _store.CreateOrResumeAsync(
			CreateTerrainPlannerRequest("1.2.4", bytes, 'c'),
			CancellationToken.None);
		Assert.AreEqual("1.2.4", next.Version);
	}

	[TestMethod]
	public async Task ConcurrentSameVersionPromotionsHaveOneWinner()
	{
		var bytes = new byte[] { 10, 11, 12 };
		var firstRequest = CreateTerrainPlannerRequest("2.0.0", bytes, 'd');
		var secondRequest = CreateTerrainPlannerRequest("2.0.0", bytes, 'e');
		var first = await _store.CreateOrResumeAsync(firstRequest, CancellationToken.None);
		var second = await _store.CreateOrResumeAsync(secondRequest, CancellationToken.None);
		await UploadAndCompleteAsync(first, firstRequest, bytes);
		await UploadAndCompleteAsync(second, secondRequest, bytes);

		async Task<int> PromoteAsync(StagedRelease release)
		{
			try
			{
				await _store.PromoteAsync(release.UploadId, CancellationToken.None);
				return StatusCodes.Status200OK;
			}
			catch (ReleaseStoreException exception)
			{
				return exception.StatusCode;
			}
		}

		var results = await Task.WhenAll(PromoteAsync(first), PromoteAsync(second));
		Assert.AreEqual(1, results.Count(status => status == StatusCodes.Status200OK));
		Assert.AreEqual(1, results.Count(status => status == StatusCodes.Status409Conflict));
		Assert.IsNotNull(await _store.GetLiveReleaseAsync("terrainplanner", CancellationToken.None));
	}

	[TestMethod]
	public async Task NullManifestCollectionsReturnBadRequest()
	{
		var request = new CreateReleaseRequest
		{
			Product = "terrainplanner",
			Version = "1.2.3",
			SourceCommit = new string('a', 40),
			Artifacts = null!
		};

		var exception = await Assert.ThrowsExceptionAsync<ReleaseStoreException>(() =>
			_store.CreateOrResumeAsync(request, CancellationToken.None));

		Assert.AreEqual(StatusCodes.Status400BadRequest, exception.StatusCode);
	}

	private async Task<StagedRelease> PrepareValidatedReleaseWithDocumentationAsync()
	{
		var bytes = new byte[] { 21, 22, 23 };
		var request = CreateTerrainPlannerRequest("5.0.0", bytes, 'd');
		var staged = await _store.CreateOrResumeAsync(request, CancellationToken.None);
		await UploadAndCompleteAsync(staged, request, bytes);
		var documentationBytes = Encoding.UTF8.GetBytes("new documentation");
		var documented = new StagedRelease
		{
			UploadId = staged.UploadId,
			Product = staged.Product,
			Version = staged.Version,
			SourceCommit = staged.SourceCommit,
			CreatedAtUtc = staged.CreatedAtUtc,
			Status = staged.Status,
			Artifacts = staged.Artifacts,
			DocumentationCatalogue = new ReleaseArtifactRequest
			{
				ArtifactId = "documentation",
				Runtime = "documentation",
				FileName = "catalogue.json",
				Size = documentationBytes.Length,
				Sha256 = Convert.ToHexString(SHA256.HashData(documentationBytes)).ToLowerInvariant()
			},
			ReceivedChunks = staged.ReceivedChunks
		};
		var draft = Path.Combine(_root, "staging", documented.UploadId);
		var candidate = Path.Combine(draft, "release");
		await WriteTestJsonAsync(Path.Combine(draft, "release.json"), documented);
		await File.WriteAllBytesAsync(Path.Combine(candidate, "catalogue.json"), documentationBytes);
		await WriteTestJsonAsync(Path.Combine(candidate, "release.json"), new PublicRelease
		{
			Product = documented.Product,
			Version = documented.Version,
			SourceCommit = documented.SourceCommit,
			PublishedAtUtc = DateTimeOffset.UtcNow,
			Artifacts = documented.Artifacts
		});

		var live = Path.Combine(_root, "releases", "live", documented.Product);
		Directory.CreateDirectory(live);
		await File.WriteAllTextAsync(Path.Combine(live, "old-release.marker"), "old release");
		await File.WriteAllTextAsync(Path.Combine(_store.DocumentationRoot, "catalogue.json"), "old documentation");
		Directory.CreateDirectory($"{_store.DocumentationRoot}.new");
		await File.WriteAllBytesAsync(Path.Combine($"{_store.DocumentationRoot}.new", "catalogue.json"), documentationBytes);
		return documented;
	}

	private Task WritePromotionJournalAsync(StagedRelease staged, string state) =>
		WriteTestJsonAsync(GetPromotionJournalPath(), new
		{
			UploadId = staged.UploadId,
			Product = staged.Product,
			State = state,
			HadLiveRelease = true,
			HasDocumentation = true,
			HadDocumentation = true
		});

	private static async Task WriteTestJsonAsync<T>(string path, T value)
	{
		await using var stream = File.Create(path);
		await JsonSerializer.SerializeAsync(stream, value, new JsonSerializerOptions(JsonSerializerDefaults.Web));
	}

	private string GetPromotionJournalPath() => Path.Combine(_root, "releases", "promotion-journal.json");

	private long GetAvailableFreeSpace()
	{
		var root = Path.GetPathRoot(Path.GetFullPath(_root)) ?? throw new DirectoryNotFoundException();
		return new DriveInfo(root).AvailableFreeSpace;
	}

	private async Task UploadCompleteAndPromoteAsync(
		StagedRelease release,
		CreateReleaseRequest request,
		byte[] bytes)
	{
		await UploadAndCompleteAsync(release, request, bytes);
		await _store.PromoteAsync(release.UploadId, CancellationToken.None);
	}

	private async Task UploadAndCompleteAsync(
		StagedRelease release,
		CreateReleaseRequest request,
		byte[] bytes)
	{
		var artifact = request.Artifacts[0];
		await _store.PutChunkAsync(
			release.UploadId,
			artifact.ArtifactId,
			0,
			0,
			bytes.Length - 1,
			bytes.Length,
			artifact.Sha256,
			new MemoryStream(bytes),
			CancellationToken.None);
		await _store.CompleteAsync(release.UploadId, CancellationToken.None);
	}

	private ReleaseStore CreateStore(FutureMudWebOptions options)
	{
		var environment = new TestWebHostEnvironment { ContentRootPath = FindWebProjectRoot() };
		return new ReleaseStore(
			Options.Create(options),
			new ReleaseProductCatalogue(environment),
			NullLogger<ReleaseStore>.Instance);
	}

	private static CreateReleaseRequest CreateTerrainPlannerRequest(string version, byte[] bytes, char commitCharacter)
	{
		return new CreateReleaseRequest
		{
			Product = "terrainplanner",
			Version = version,
			SourceCommit = new string(commitCharacter, 40),
			Artifacts =
			[
				new ReleaseArtifactRequest
				{
					ArtifactId = "win-x64",
					Runtime = "win-x64",
					FileName = $"terrainplanner-{version}-win-x64.zip",
					Size = bytes.Length,
					Sha256 = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant()
				}
			]
		};
	}

	private static string FindWebProjectRoot()
	{
		var directory = new DirectoryInfo(AppContext.BaseDirectory);
		while (directory is not null && !Directory.Exists(Path.Combine(directory.FullName, "FutureMUD.Web")))
		{
			directory = directory.Parent;
		}
		return Path.Combine(directory?.FullName ?? throw new DirectoryNotFoundException(), "FutureMUD.Web");
	}

	private sealed class PausingMemoryStream(byte[] buffer) : MemoryStream(buffer)
	{
		public TaskCompletionSource ReadStarted { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
		public TaskCompletionSource ResumeReading { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

		public override async ValueTask<int> ReadAsync(Memory<byte> destination, CancellationToken cancellationToken = default)
		{
			ReadStarted.TrySetResult();
			await ResumeReading.Task.WaitAsync(cancellationToken);
			return await base.ReadAsync(destination, cancellationToken);
		}
	}

	private sealed class TestWebHostEnvironment : IWebHostEnvironment
	{
		public string ApplicationName { get; set; } = "FutureMUD.Web.Tests";
		public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
		public string WebRootPath { get; set; } = string.Empty;
		public string EnvironmentName { get; set; } = "Testing";
		public string ContentRootPath { get; set; } = string.Empty;
		public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
	}
}
