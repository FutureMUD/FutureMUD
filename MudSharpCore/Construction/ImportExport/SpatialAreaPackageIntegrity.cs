#nullable enable

using System;
using System.Security.Cryptography;
using System.Text;

namespace MudSharp.Construction.ImportExport;

internal static class SpatialAreaPackageIntegrity
{
	internal static string ComputeHash(string canonicalJson)
	{
		return Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(canonicalJson)));
	}

	internal static bool Matches(string? suppliedHash, string canonicalWindowsJson)
	{
		if (suppliedHash is null || suppliedHash.Length != 64) return false;
		var supplied = Encoding.ASCII.GetBytes(suppliedHash.ToLowerInvariant());
		var windowsHash = Encoding.ASCII.GetBytes(ComputeHash(canonicalWindowsJson));
		// Serialized JSON escapes newlines inside string values. Only formatting whitespace changes;
		// the model and its embedded text remain untouched until the original checksum is verified.
		var unixHash = Encoding.ASCII.GetBytes(ComputeHash(canonicalWindowsJson.Replace("\r\n", "\n", StringComparison.Ordinal)));
		return CryptographicOperations.FixedTimeEquals(supplied, windowsHash) |
		       CryptographicOperations.FixedTimeEquals(supplied, unixHash);
	}
}
