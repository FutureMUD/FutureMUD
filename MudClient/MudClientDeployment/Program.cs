#nullable enable

using MudClientDeployment;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Security;

return await RunAsync(args);

static async Task<int> RunAsync(string[] args)
{
	try
	{
		if (args.Length == 0 || args[0] is "--help" or "-h")
		{
			Console.WriteLine("Usage: MudClientDeployment verify --manifest <path> --signature <path> --archive <path> --runtime <rid>");
			return args.Length == 0 ? 2 : 0;
		}

		if (args[0] == "generate-key")
		{
			var privateKey = new Ed25519PrivateKeyParameters(new SecureRandom());
			Console.WriteLine($"privateKeyBase64={Convert.ToBase64String(privateKey.GetEncoded())}");
			Console.WriteLine($"publicKeyBase64={Convert.ToBase64String(privateKey.GeneratePublicKey().GetEncoded())}");
			return 0;
		}
		if (args[0] == "sign")
		{
			var signingOptions = ParseOptions(args[1..]);
			var signingManifestPath = GetRequired(signingOptions, "manifest");
			var signingSignaturePath = GetRequired(signingOptions, "signature");
			var privateKeyBase64 = Environment.GetEnvironmentVariable("FUTUREMUD_MUDCLIENT_UPDATE_SIGNING_PRIVATE_KEY")
				?? throw new InvalidDataException("The protected MudClient update signing key is unavailable.");
			var privateKey = new Ed25519PrivateKeyParameters(Convert.FromBase64String(privateKeyBase64), 0);
			var signingManifestBytes = await File.ReadAllBytesAsync(signingManifestPath);
			var signer = new Ed25519Signer();
			signer.Init(true, privateKey);
			signer.BlockUpdate(signingManifestBytes, 0, signingManifestBytes.Length);
			await File.WriteAllBytesAsync(signingSignaturePath, signer.GenerateSignature());
			return 0;
		}

		if (args[0] != "verify")
		{
			throw new InvalidDataException("The deployment command is not recognised.");
		}

		var options = ParseOptions(args[1..]);
		var manifestPath = GetRequired(options, "manifest");
		var signaturePath = GetRequired(options, "signature");
		var archivePath = GetRequired(options, "archive");
		var runtime = GetRequired(options, "runtime");
		var publicKey = Convert.FromBase64String(UpdateManifestVerifier.ProductionPublicKeyBase64);
		var manifestBytes = await File.ReadAllBytesAsync(manifestPath);
		var signatureBytes = await File.ReadAllBytesAsync(signaturePath);
		var manifest = UpdateManifestVerifier.Verify(
			manifestBytes,
			signatureBytes,
			runtime,
			publicKey);
		await UpdateManifestVerifier.VerifyArchiveAsync(manifest, runtime, archivePath, CancellationToken.None);
		Console.WriteLine($"Verified MudClient {manifest.Version} for {runtime}.");
		return 0;
	}
	catch (Exception exception) when (exception is ArgumentException or FormatException or InvalidDataException or IOException)
	{
		Console.Error.WriteLine($"MudClient deployment verification failed: {exception.Message}");
		return 1;
	}
}

static Dictionary<string, string> ParseOptions(string[] args)
{
	var options = new Dictionary<string, string>(StringComparer.Ordinal);
	for (var index = 0; index < args.Length; index += 2)
	{
		if (index + 1 >= args.Length || !args[index].StartsWith("--", StringComparison.Ordinal) || !options.TryAdd(args[index][2..], args[index + 1]))
		{
			throw new ArgumentException("Deployment options must be distinct --name value pairs.");
		}
	}
	return options;
}

static string GetRequired(IReadOnlyDictionary<string, string> options, string name) =>
	options.TryGetValue(name, out var value) && !string.IsNullOrWhiteSpace(value)
		? value
		: throw new ArgumentException($"--{name} is required.");
