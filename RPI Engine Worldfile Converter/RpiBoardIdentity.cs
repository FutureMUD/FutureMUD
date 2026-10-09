#nullable enable

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace RPI_Engine_Worldfile_Converter;

public static class RpiBoardIdentity
{
	public static string BoardSuffix(string key) => Hash(JsonSerializer.Serialize(Normalise(key)));

	public static string AccessSuffix(string key, IEnumerable<FutureMudBoardClanRestriction> restrictions)
	{
		var access = restrictions
			.Select(x => new[] { Normalise(x.ClanAlias), Normalise(x.RankName) })
			.OrderBy(x => x[0], StringComparer.Ordinal)
			.ThenBy(x => x[1], StringComparer.Ordinal)
			.ToArray();
		return Hash(JsonSerializer.Serialize(new { Key = Normalise(key), Restrictions = access }));
	}

	private static string Normalise(string value) => value.Trim().ToLowerInvariant();
	private static string Hash(string value) =>
		Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(value)))[..32];
}
