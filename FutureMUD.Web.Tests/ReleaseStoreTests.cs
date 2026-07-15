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
