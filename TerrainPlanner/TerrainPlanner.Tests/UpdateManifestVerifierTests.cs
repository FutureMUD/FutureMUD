using System.Security.Cryptography;
using System.Text.Json;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using TerrainPlanner.Deployment;

namespace TerrainPlanner.Tests;

[TestClass]
public class UpdateManifestVerifierTests
{
	[TestMethod]
	public async Task SignedManifestAndArchiveAreVerifiedTogether()
	{
		var archivePath = Path.GetTempFileName();
		try
		{
			var archiveBytes = RandomNumberGenerator.GetBytes(1024);
			await File.WriteAllBytesAsync(archivePath, archiveBytes);
			var manifest = CreateManifest(archiveBytes);
			var manifestBytes = JsonSerializer.SerializeToUtf8Bytes(manifest, new JsonSerializerOptions(JsonSerializerDefaults.Web));
			var privateKey = new Ed25519PrivateKeyParameters(RandomNumberGenerator.GetBytes(Ed25519PrivateKeyParameters.KeySize), 0);
			var signer = new Ed25519Signer();
			signer.Init(true, privateKey);
			signer.BlockUpdate(manifestBytes, 0, manifestBytes.Length);

			var verified = UpdateManifestVerifier.Verify(manifestBytes, signer.GenerateSignature(), "linux-x64",
				privateKey.GeneratePublicKey().GetEncoded());
			await UpdateManifestVerifier.VerifyArchiveAsync(verified, "linux-x64", archivePath, CancellationToken.None);
		}
		finally
		{
			File.Delete(archivePath);
		}
	}

	[TestMethod]
	public void InvalidSignatureAndProductAreRejected()
	{
		var privateKey = new Ed25519PrivateKeyParameters(RandomNumberGenerator.GetBytes(Ed25519PrivateKeyParameters.KeySize), 0);
		var manifest = CreateManifest([1, 2, 3], "mudclient");
		var bytes = JsonSerializer.SerializeToUtf8Bytes(manifest, new JsonSerializerOptions(JsonSerializerDefaults.Web));
		Assert.ThrowsException<InvalidDataException>(() =>
			UpdateManifestVerifier.Verify(bytes, new byte[Ed25519PrivateKeyParameters.KeySize * 2], "linux-x64",
				privateKey.GeneratePublicKey().GetEncoded()));

		Assert.ThrowsException<InvalidDataException>(() => UpdateManifestVerifier.Validate(manifest, "linux-x64"));
	}

	private static UpdateManifest CreateManifest(byte[] archiveBytes, string product = "terrainplanner") => new()
	{
		SchemaVersion = 1,
		Product = product,
		Version = "2.0.0",
		SourceCommit = new string('a', 40),
		KeyId = UpdateManifestVerifier.ProductionKeyId,
		Artifacts =
		[
			new UpdateArtifact
			{
				Runtime = "linux-x64",
				FileName = "terrainplanner-2.0.0-linux-x64.zip",
				Size = archiveBytes.Length,
				Sha256 = Convert.ToHexString(SHA256.HashData(archiveBytes)).ToLowerInvariant()
			}
		]
	};
}
