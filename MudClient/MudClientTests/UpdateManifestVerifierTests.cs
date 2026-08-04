using MudClientDeployment;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Security;
using System.Text.Json;

namespace MudClientTests;

public class UpdateManifestVerifierTests
{
	[Fact]
	public void Verify_AcceptsASignedManifestForTheExpectedRuntime()
	{
		var privateKey = new Ed25519PrivateKeyParameters(new SecureRandom());
		var bytes = JsonSerializer.SerializeToUtf8Bytes(new UpdateManifest
		{
			SchemaVersion = 1,
			Product = "mudclient",
			Version = "1.2.0",
			SourceCommit = new string('a', 40),
			KeyId = UpdateManifestVerifier.ProductionKeyId,
			Artifacts = [new UpdateArtifact { Runtime = "win-x64", FileName = "mudclient-1.2.0-win-x64.zip", Size = 1, Sha256 = new string('b', 64) }]
		}, new JsonSerializerOptions(JsonSerializerDefaults.Web));
		var signature = Sign(bytes, privateKey);

		var manifest = UpdateManifestVerifier.Verify(bytes, signature, "win-x64", privateKey.GeneratePublicKey().GetEncoded());

		Assert.Equal("1.2.0", manifest.Version);
	}

	[Fact]
	public void Verify_RejectsATamperedManifest()
	{
		var privateKey = new Ed25519PrivateKeyParameters(new SecureRandom());
		var bytes = JsonSerializer.SerializeToUtf8Bytes(new UpdateManifest
		{
			SchemaVersion = 1,
			Product = "mudclient",
			Version = "1.2.0",
			SourceCommit = new string('a', 40),
			KeyId = UpdateManifestVerifier.ProductionKeyId,
			Artifacts = [new UpdateArtifact { Runtime = "linux-x64", FileName = "mudclient-1.2.0-linux-x64.zip", Size = 1, Sha256 = new string('b', 64) }]
		}, new JsonSerializerOptions(JsonSerializerDefaults.Web));
		var signature = Sign(bytes, privateKey);
		bytes[^1] ^= 1;

		Assert.Throws<InvalidDataException>(() => UpdateManifestVerifier.Verify(bytes, signature, "linux-x64", privateKey.GeneratePublicKey().GetEncoded()));
	}

	[Fact]
	public void ValidateExpectedVersion_RejectsAMismatchedPackageVersion()
	{
		var manifest = new UpdateManifest { Version = "1.2.0" };

		Assert.Throws<InvalidDataException>(() => UpdateManifestVerifier.ValidateExpectedVersion(manifest, "1.2.1"));
	}

	private static byte[] Sign(byte[] bytes, Ed25519PrivateKeyParameters privateKey)
	{
		var signer = new Ed25519Signer();
		signer.Init(true, privateKey);
		signer.BlockUpdate(bytes, 0, bytes.Length);
		return signer.GenerateSignature();
	}
}
