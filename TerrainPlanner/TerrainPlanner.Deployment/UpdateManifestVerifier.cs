#nullable enable

using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace TerrainPlanner.Deployment;

public static partial class UpdateManifestVerifier
{
	public const string ProductionKeyId = "futuremud-mudclient-2026-08";
	public const string ProductionPublicKeyBase64 = "48R7fiHwdW6CTBW1DP4aK2qcgSvNsb59hSiNK72lyu0=";

	public static UpdateManifest Verify(ReadOnlySpan<byte> manifestBytes, ReadOnlySpan<byte> signatureBytes,
		string expectedRuntime, ReadOnlySpan<byte> publicKey)
	{
		if (publicKey.Length != Ed25519PublicKeyParameters.KeySize)
		{
			throw new InvalidDataException("The configured update-signing public key is invalid.");
		}

		var signer = new Ed25519Signer();
		signer.Init(false, new Ed25519PublicKeyParameters(publicKey.ToArray(), 0));
		signer.BlockUpdate(manifestBytes.ToArray(), 0, manifestBytes.Length);
		if (!signer.VerifySignature(signatureBytes.ToArray()))
		{
			throw new InvalidDataException("The update manifest signature is invalid.");
		}

		var manifest = JsonSerializer.Deserialize<UpdateManifest>(manifestBytes,
			new JsonSerializerOptions(JsonSerializerDefaults.Web)) ??
			throw new InvalidDataException("The update manifest is empty.");
		Validate(manifest, expectedRuntime);
		return manifest;
	}

	public static void Validate(UpdateManifest manifest, string expectedRuntime)
	{
		if (manifest.SchemaVersion != UpdateManifest.CurrentSchemaVersion ||
			manifest.Product != "terrainplanner" || !VersionRegex().IsMatch(manifest.Version) ||
			!CommitRegex().IsMatch(manifest.SourceCommit) || manifest.KeyId != ProductionKeyId ||
			manifest.Artifacts is null)
		{
			throw new InvalidDataException("The update manifest has invalid identity metadata.");
		}

		var artifacts = manifest.Artifacts.Where(item => item.Runtime == expectedRuntime).ToList();
		if (artifacts.Count != 1)
		{
			throw new InvalidDataException("The manifest does not contain exactly one artifact for this runtime.");
		}

		var artifact = artifacts[0];
		if (artifact.Size <= 0 || !ShaRegex().IsMatch(artifact.Sha256) ||
			artifact.FileName != $"terrainplanner-{manifest.Version}-{expectedRuntime}.zip")
		{
			throw new InvalidDataException("The update manifest artifact metadata is invalid.");
		}
	}

	public static async Task VerifyArchiveAsync(UpdateManifest manifest, string runtime, string archivePath,
		CancellationToken cancellationToken)
	{
		var artifact = manifest.Artifacts.Single(item => item.Runtime == runtime);
		var file = new FileInfo(archivePath);
		if (!file.Exists || file.Length != artifact.Size)
		{
			throw new InvalidDataException("The downloaded archive size does not match the signed manifest.");
		}

		await using var stream = File.OpenRead(archivePath);
		var actual = Convert.ToHexString(await SHA256.HashDataAsync(stream, cancellationToken)).ToLowerInvariant();
		if (actual != artifact.Sha256)
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
