#nullable enable

using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MudClientDeployment;

public static partial class UpdateManifestVerifier
{
	public const string ProductionKeyId = "futuremud-mudclient-2026-08";
	// The public half of the production Ed25519 update-signing key. It is intentionally embedded
	// in every self-contained deployment tool and is not a secret.
	public const string ProductionPublicKeyBase64 = "48R7fiHwdW6CTBW1DP4aK2qcgSvNsb59hSiNK72lyu0=";

	public static UpdateManifest Verify(
		ReadOnlySpan<byte> manifestBytes,
		ReadOnlySpan<byte> signatureBytes,
		string expectedRuntime,
		ReadOnlySpan<byte> publicKey)
	{
		if (publicKey.Length != Ed25519PublicKeyParameters.KeySize)
		{
			throw new InvalidDataException("The configured update signing public key is invalid.");
		}

		var signer = new Ed25519Signer();
		signer.Init(false, new Ed25519PublicKeyParameters(publicKey.ToArray(), 0));
		signer.BlockUpdate(manifestBytes.ToArray(), 0, manifestBytes.Length);
		if (!signer.VerifySignature(signatureBytes.ToArray()))
		{
			throw new InvalidDataException("The update manifest signature is invalid.");
		}

		var manifest = JsonSerializer.Deserialize<UpdateManifest>(manifestBytes, new JsonSerializerOptions(JsonSerializerDefaults.Web))
			?? throw new InvalidDataException("The update manifest is empty.");
		Validate(manifest, expectedRuntime);
		return manifest;
	}

	public static void Validate(UpdateManifest manifest, string expectedRuntime)
	{
		if (manifest.SchemaVersion != UpdateManifest.CurrentSchemaVersion ||
			!string.Equals(manifest.Product, "mudclient", StringComparison.Ordinal) ||
			!VersionRegex().IsMatch(manifest.Version) ||
			!CommitRegex().IsMatch(manifest.SourceCommit) ||
			!string.Equals(manifest.KeyId, ProductionKeyId, StringComparison.Ordinal) ||
			manifest.Artifacts is null)
		{
			throw new InvalidDataException("The update manifest has invalid identity metadata.");
		}

		var matchingArtifacts = manifest.Artifacts
			.Where(artifact => string.Equals(artifact.Runtime, expectedRuntime, StringComparison.Ordinal))
			.ToList();
		if (matchingArtifacts.Count != 1)
		{
			throw new InvalidDataException("The update manifest does not contain exactly one artifact for this runtime.");
		}

		var artifact = matchingArtifacts[0];
		if (artifact.Size <= 0 ||
			!ShaRegex().IsMatch(artifact.Sha256) ||
			!string.Equals(artifact.FileName, $"mudclient-{manifest.Version}-{expectedRuntime}.zip", StringComparison.Ordinal))
		{
			throw new InvalidDataException("The update manifest artifact metadata is invalid.");
		}
	}

	public static void ValidateExpectedVersion(UpdateManifest manifest, string expectedVersion)
	{
		if (!string.Equals(manifest.Version, expectedVersion, StringComparison.Ordinal))
		{
			throw new InvalidDataException("The package version does not match the signed update manifest.");
		}
	}

	public static async Task VerifyArchiveAsync(UpdateManifest manifest, string runtime, string archivePath, CancellationToken cancellationToken)
	{
		var artifact = manifest.Artifacts.Single(item => item.Runtime == runtime);
		var file = new FileInfo(archivePath);
		if (!file.Exists || file.Length != artifact.Size)
		{
			throw new InvalidDataException("The downloaded archive size does not match the signed manifest.");
		}

		await using var stream = File.OpenRead(archivePath);
		var actualHash = Convert.ToHexString(await SHA256.HashDataAsync(stream, cancellationToken)).ToLowerInvariant();
		if (!string.Equals(actualHash, artifact.Sha256, StringComparison.Ordinal))
		{
			throw new InvalidDataException("The downloaded archive SHA-256 does not match the signed manifest.");
		}
	}

	[GeneratedRegex("\\A\\d+\\.\\d+\\.\\d+\\z", RegexOptions.CultureInvariant)]
	private static partial Regex VersionRegex();

	[GeneratedRegex("\\A[0-9a-f]{40}\\z", RegexOptions.CultureInvariant)]
	private static partial Regex CommitRegex();

	[GeneratedRegex("\\A[0-9a-f]{64}\\z", RegexOptions.CultureInvariant)]
	private static partial Regex ShaRegex();
}
