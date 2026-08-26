#nullable enable

using FutureMUD.Web.Configuration;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using System.Text.Json;

namespace FutureMUD.Web.Publishing;

public static class MudClientUpdateManifestVerifier
{
	private const int MaximumManifestBytes = 64 * 1024;

	public static void Verify(
		SignedUpdateManifestRequest request,
		FutureMudWebOptions options,
		string product,
		string version,
		string sourceCommit,
		IReadOnlyList<ReleaseArtifactRequest> artifacts)
	{
		if (product is not ("mudclient" or "terrainplanner") ||
			!string.Equals(request.KeyId, options.MudClientUpdateSigningKeyId, StringComparison.Ordinal))
		{
			throw new ReleaseStoreException("The update manifest signing key is not recognised.", StatusCodes.Status400BadRequest);
		}

		byte[] content;
		byte[] signature;
		byte[] publicKey;
		try
		{
			content = Convert.FromBase64String(request.ContentBase64);
			signature = Convert.FromBase64String(request.SignatureBase64);
			publicKey = Convert.FromBase64String(options.MudClientUpdateSigningPublicKey);
		}
		catch (FormatException)
		{
			throw new ReleaseStoreException("The update manifest encoding is invalid.", StatusCodes.Status400BadRequest);
		}

		if (content.Length == 0 || content.Length > MaximumManifestBytes || signature.Length != Ed25519PrivateKeyParameters.SignatureSize || publicKey.Length != Ed25519PublicKeyParameters.KeySize)
		{
			throw new ReleaseStoreException("The update manifest is invalid.", StatusCodes.Status400BadRequest);
		}

		var signer = new Ed25519Signer();
		signer.Init(false, new Ed25519PublicKeyParameters(publicKey, 0));
		signer.BlockUpdate(content, 0, content.Length);
		if (!signer.VerifySignature(signature))
		{
			throw new ReleaseStoreException("The update manifest signature is invalid.", StatusCodes.Status422UnprocessableEntity);
		}

		var manifest = JsonSerializer.Deserialize<MudClientUpdateManifest>(content, new JsonSerializerOptions(JsonSerializerDefaults.Web));
		if (manifest is null ||
			manifest.SchemaVersion != MudClientUpdateManifest.CurrentSchemaVersion ||
			manifest.Product != product || manifest.Version != version ||
			!string.Equals(manifest.SourceCommit, sourceCommit, StringComparison.OrdinalIgnoreCase) ||
			manifest.KeyId != request.KeyId || manifest.Artifacts is null || manifest.Artifacts.Count != artifacts.Count ||
			!manifest.Artifacts.OrderBy(item => item.Runtime).Select(ArtifactKey)
				.SequenceEqual(artifacts.OrderBy(item => item.Runtime).Select(ArtifactKey), StringComparer.Ordinal))
		{
			throw new ReleaseStoreException("The signed update manifest does not match the release artifacts.", StatusCodes.Status422UnprocessableEntity);
		}
	}

	private static string ArtifactKey(ReleaseArtifactRequest artifact) =>
		$"{artifact.Runtime}|{artifact.FileName}|{artifact.Size}|{artifact.Sha256.ToLowerInvariant()}";
}
