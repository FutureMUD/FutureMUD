#nullable enable

using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using TerrainPlanner.Deployment;

try
{
	if (args.Length == 0 || args[0] is "--help" or "-h")
	{
		Console.WriteLine("TerrainPlanner.Deployment verify --manifest <path> --signature <path> --archive <path> --runtime <rid>");
		Console.WriteLine("TerrainPlanner.Deployment sign --manifest <path> --signature <path>");
		return args.Length == 0 ? 2 : 0;
	}

	var options = ParseOptions(args[1..]);
	if (args[0] == "sign")
	{
		var privateKeyValue = Environment.GetEnvironmentVariable("FUTUREMUD_MUDCLIENT_UPDATE_SIGNING_PRIVATE_KEY") ??
			throw new InvalidDataException("The protected FutureMUD update-signing key is unavailable.");
		var privateKey = new Ed25519PrivateKeyParameters(Convert.FromBase64String(privateKeyValue), 0);
		var bytes = await File.ReadAllBytesAsync(Required(options, "manifest"));
		var signer = new Ed25519Signer();
		signer.Init(true, privateKey);
		signer.BlockUpdate(bytes, 0, bytes.Length);
		await File.WriteAllBytesAsync(Required(options, "signature"), signer.GenerateSignature());
		return 0;
	}

	if (args[0] != "verify")
	{
		throw new InvalidDataException("The deployment command is not recognised.");
	}

	var manifestBytes = await File.ReadAllBytesAsync(Required(options, "manifest"));
	var signatureBytes = await File.ReadAllBytesAsync(Required(options, "signature"));
	var runtime = Required(options, "runtime");
	var manifest = UpdateManifestVerifier.Verify(manifestBytes, signatureBytes, runtime,
		Convert.FromBase64String(UpdateManifestVerifier.ProductionPublicKeyBase64));
	await UpdateManifestVerifier.VerifyArchiveAsync(manifest, runtime, Required(options, "archive"), CancellationToken.None);
	Console.WriteLine($"Verified Terrain Planner {manifest.Version} for {runtime}.");
	return 0;
}
catch (Exception exception) when (exception is ArgumentException or FormatException or InvalidDataException or IOException)
{
	Console.Error.WriteLine($"Terrain Planner deployment verification failed: {exception.Message}");
	return 1;
}

static Dictionary<string, string> ParseOptions(string[] values)
{
	var result = new Dictionary<string, string>(StringComparer.Ordinal);
	for (var index = 0; index < values.Length; index += 2)
	{
		if (index + 1 >= values.Length || !values[index].StartsWith("--", StringComparison.Ordinal) ||
			!result.TryAdd(values[index][2..], values[index + 1]))
		{
			throw new ArgumentException("Options must be distinct --name value pairs.");
		}
	}
	return result;
}

static string Required(IReadOnlyDictionary<string, string> options, string name) =>
	options.TryGetValue(name, out var value) && !string.IsNullOrWhiteSpace(value)
		? value
		: throw new ArgumentException($"--{name} is required.");
