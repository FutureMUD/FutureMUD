#nullable enable
using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;

namespace MudSharp.Accounts;

/// <summary>Argon2id PHC strings with bounded parsing and independently random 128-bit salts.</summary>
public sealed class Argon2AccountCredentialHasher : IAccountCredentialHasher
{
	internal const int MemoryKiB = 19456;
	internal const int Iterations = 2;
	internal const string Prefix = "$argon2id$";
	// A valid-shaped, deliberately nonmatching verifier gives unknown/legacy failures real KDF work.
	internal const string DummyHash = "$argon2id$v=19$m=19456,t=2,p=1$AAAAAAAAAAAAAAAAAAAAAA==$AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=";

	public string Hash(string password)
	{
		var salt = RandomNumberGenerator.GetBytes(16);
		var digest = Derive(password, salt, MemoryKiB, Iterations);
		return $"$argon2id$v=19$m={MemoryKiB},t={Iterations},p=1${Convert.ToBase64String(salt).TrimEnd('=')}${Convert.ToBase64String(digest).TrimEnd('=')}";
	}

	public bool Verify(string password, string encodedHash)
	{
		if (!TryParse(encodedHash, out var salt, out var expected, out var memory, out var iterations)) return false;
		var digest = Derive(password, salt, memory, iterations);
		return CryptographicOperations.FixedTimeEquals(digest, expected);
	}

	internal static bool TryParse(string? encoded, out byte[] salt, out byte[] digest, out int memory, out int iterations)
	{
		salt = digest = [];
		memory = iterations = 0;
		if (encoded is null || encoded.Length > 256) return false;
		var parts = encoded.Split('$');
		if (parts.Length != 6 || parts[0] != "" || parts[1] != "argon2id" || parts[2] != "v=19") return false;
		var parameters = parts[3].Split(',');
		if (parameters.Length != 3 || !parameters[0].StartsWith("m=", StringComparison.Ordinal) ||
		    !parameters[1].StartsWith("t=", StringComparison.Ordinal) || parameters[2] != "p=1" ||
		    !int.TryParse(parameters[0].AsSpan(2), NumberStyles.None, CultureInfo.InvariantCulture, out memory) ||
		    !int.TryParse(parameters[1].AsSpan(2), NumberStyles.None, CultureInfo.InvariantCulture, out iterations) ||
		    memory < MemoryKiB || memory > 65536 || iterations < Iterations || iterations > 6) return false;
		try
		{
			salt = Decode(parts[4]);
			digest = Decode(parts[5]);
			return salt.Length == 16 && digest.Length == 32;
		}
		catch (FormatException) { return false; }
	}
	private static byte[] Decode(string value) => Convert.FromBase64String(value.PadRight((value.Length + 3) / 4 * 4, '='));

	private static byte[] Derive(string password, byte[] salt, int memory, int iterations)
	{
		var bytes = Encoding.UTF8.GetBytes(password);
		try
		{
			using var argon = new Argon2id(bytes) { Salt = salt, MemorySize = memory, Iterations = iterations, DegreeOfParallelism = 1 };
			return argon.GetBytes(32);
		}
		finally { CryptographicOperations.ZeroMemory(bytes); }
	}
}
