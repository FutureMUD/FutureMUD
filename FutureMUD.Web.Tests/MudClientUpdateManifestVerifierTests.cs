#nullable enable

using FutureMUD.Web.Configuration;
using FutureMUD.Web.Publishing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Security;
using System.Text.Json;

namespace FutureMUD.Web.Tests;

[TestClass]
public sealed class MudClientUpdateManifestVerifierTests
{
	[TestMethod]
	[DataRow("mudclient")]
	[DataRow("terrainplanner")]
	public void SignedManifestMustMatchTheReleaseAndItsSignature(string product)
	{
		var key = new Ed25519PrivateKeyParameters(new SecureRandom());
		var artifact = new ReleaseArtifactRequest
		{
			ArtifactId = "win-x64",
			Runtime = "win-x64",
			FileName = $"{product}-1.2.0-win-x64.zip",
			Size = 1,
			Sha256 = new string('a', 64)
		};
		var options = new FutureMudWebOptions
		{
			MudClientUpdateSigningKeyId = "test-key",
			MudClientUpdateSigningPublicKey = Convert.ToBase64String(key.GeneratePublicKey().GetEncoded())
		};
		var bytes = JsonSerializer.SerializeToUtf8Bytes(new MudClientUpdateManifest
		{
			SchemaVersion = 1,
			Product = product,
			Version = "1.2.0",
			SourceCommit = new string('b', 40),
			KeyId = "test-key",
			Artifacts = [artifact]
		}, new JsonSerializerOptions(JsonSerializerDefaults.Web));
		var request = new SignedUpdateManifestRequest
		{
			KeyId = "test-key",
			ContentBase64 = Convert.ToBase64String(bytes),
			SignatureBase64 = Convert.ToBase64String(Sign(bytes, key))
		};

		MudClientUpdateManifestVerifier.Verify(request, options, product, "1.2.0", new string('b', 40), [artifact]);

		request = new SignedUpdateManifestRequest
		{
			KeyId = request.KeyId,
			ContentBase64 = request.ContentBase64,
			SignatureBase64 = Convert.ToBase64String(new byte[64])
		};
		Assert.ThrowsException<ReleaseStoreException>(() =>
			MudClientUpdateManifestVerifier.Verify(request, options, product, "1.2.0", new string('b', 40), [artifact]));
	}

	private static byte[] Sign(byte[] bytes, Ed25519PrivateKeyParameters privateKey)
	{
		var signer = new Ed25519Signer();
		signer.Init(true, privateKey);
		signer.BlockUpdate(bytes, 0, bytes.Length);
		return signer.GenerateSignature();
	}
}
