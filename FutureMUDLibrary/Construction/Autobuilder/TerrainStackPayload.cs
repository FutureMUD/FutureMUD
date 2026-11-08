#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

#if TERRAIN_PLANNER
namespace TerrainPlanner.Contracts.StackTransport;
#else
namespace MudSharp.Construction.Autobuilder;
#endif

/// <summary>A portable stack of XY terrain grids. Coordinates are topology, not world coordinates.</summary>
public sealed record TerrainStackPayload(int Width, int Height, int[] Levels, TerrainStackRoom[] Rooms,
	TerrainStackLink[] Links, int Version = 1);

public sealed record TerrainStackRoom(int X, int Y, int Z, long Terrain, long[] Tags);

/// <summary>An override for the connection from (X,Y,LowerZ) to (X,Y,LowerZ+1).</summary>
public sealed record TerrainStackLink(int X, int Y, int LowerZ, bool Suppressed = false,
	bool Climb = false, int Difficulty = 5, bool Fall = false);

/// <summary>Shared, bounded wire codec used by the browser planner and engine.</summary>
public static class TerrainStackCodec
{
	public const int MaximumDimension = 200;
	public const int MaximumLevels = 32;
	public const int MaximumPositions = 200_000;
	public const int MaximumRooms = 40_000;
	public const int MaximumDecodedBytes = 16 * 1024 * 1024;
	public const int MaximumEncodedCharacters = 4 * 1024 * 1024;
	public const int SingleTokenLimit = 8_000;
	public const int EditorLineLength = 1_000;
	private const string Prefix = "FMS1:";
	private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

	public static void Validate(TerrainStackPayload payload, bool requireRooms = true)
	{
		if (payload is null || payload.Version != 1)
			throw new InvalidDataException("Unsupported terrain stack version.");
		if (payload.Width is < 1 or > MaximumDimension || payload.Height is < 1 or > MaximumDimension ||
			payload.Levels is null || payload.Levels.Length is < 1 or > MaximumLevels ||
			(long)payload.Width * payload.Height * payload.Levels.Length > MaximumPositions ||
			payload.Levels.Distinct().Count() != payload.Levels.Length)
			throw new InvalidDataException("Invalid terrain stack dimensions or duplicate Z levels.");
		if (payload.Rooms is null || payload.Rooms.Length > MaximumRooms || (requireRooms && payload.Rooms.Length == 0) ||
			payload.Links is null || payload.Links.Length > MaximumRooms)
			throw new InvalidDataException("A terrain stack must contain 1–40,000 rooms and at most 40,000 link overrides.");
		var levels = payload.Levels.ToHashSet();
		var occupied = new HashSet<(int X, int Y, int Z)>();
		foreach (var room in payload.Rooms)
		{
			if (room is null || room.X < 0 || room.X >= payload.Width || room.Y < 0 || room.Y >= payload.Height ||
				!levels.Contains(room.Z) || room.Terrain <= 0 || room.Tags is null || room.Tags.Any(id => id <= 0) ||
				room.Tags.Distinct().Count() != room.Tags.Length || !occupied.Add((room.X, room.Y, room.Z)))
				throw new InvalidDataException("Invalid or duplicate terrain stack room or tag ID.");
		}
		var links = new HashSet<(int, int, int)>();
		foreach (var link in payload.Links)
		{
			if (link is null || link.LowerZ == int.MaxValue || link.Difficulty is < 0 or > 10 ||
				!occupied.Contains((link.X, link.Y, link.LowerZ)) ||
				!occupied.Contains((link.X, link.Y, link.LowerZ + 1)) || !links.Add((link.X, link.Y, link.LowerZ)))
				throw new InvalidDataException("Invalid, dangling or duplicate terrain stack vertical connection.");
		}
	}

	/// <param name="compress">Optional portable Brotli encoder for platforms without the native .NET codec.</param>
	public static string Encode(TerrainStackPayload payload, Func<byte[], byte[]>? compress = null)
	{
		Validate(payload);
		var json = JsonSerializer.SerializeToUtf8Bytes(payload, JsonOptions);
		if (json.Length > MaximumDecodedBytes) throw new InvalidDataException("Terrain stack exceeds the decoded size limit.");
		using var output = new MemoryStream();
		output.Write(SHA256.HashData(json));
		if (compress is not null) output.Write(compress(json));
		else
		{
			using var compressor = new BrotliStream(output, CompressionLevel.SmallestSize, true);
			compressor.Write(json);
		}
		var token = Prefix + Convert.ToBase64String(output.ToArray()).TrimEnd('=').Replace('+', '-').Replace('/', '_');
		if (token.Length > MaximumEncodedCharacters) throw new InvalidDataException("Terrain stack exceeds the encoded size limit.");
		return token;
	}

	public static TerrainStackPayload Decode(string text)
	{
		if (text is null || text.Length > MaximumEncodedCharacters)
			throw new InvalidDataException("Terrain stack exceeds the encoded size limit.");
		var token = string.Concat(text.Where(c => !char.IsWhiteSpace(c)));
		if (!token.StartsWith(Prefix, StringComparison.Ordinal)) throw new InvalidDataException("Expected an FMS1: terrain stack token.");
		try
		{
			var encoded = token[Prefix.Length..];
			if (encoded.Any(c => !char.IsAsciiLetterOrDigit(c) && c != '-' && c != '_'))
				throw new InvalidDataException("Invalid terrain stack encoding.");
			var bytes = Convert.FromBase64String(encoded.Replace('-', '+').Replace('_', '/') + new string('=', (4 - encoded.Length % 4) % 4));
			if (bytes.Length <= 32) throw new InvalidDataException("Truncated terrain stack.");
			using var input = new MemoryStream(bytes, 32, bytes.Length - 32);
			using var decompressor = new BrotliStream(input, CompressionMode.Decompress);
			using var output = new MemoryStream();
			var buffer = new byte[8192];
			int count;
			while ((count = decompressor.Read(buffer)) != 0)
			{
				if (output.Length + count > MaximumDecodedBytes) throw new InvalidDataException("Terrain stack exceeds the decoded size limit.");
				output.Write(buffer, 0, count);
			}
			var json = output.ToArray();
			if (!CryptographicOperations.FixedTimeEquals(bytes.AsSpan(0, 32), SHA256.HashData(json)))
				throw new InvalidDataException("Terrain stack checksum failed; copy the complete export again.");
			var payload = JsonSerializer.Deserialize<TerrainStackPayload>(json, JsonOptions)
				?? throw new InvalidDataException("Empty terrain stack.");
			Validate(payload);
			return payload;
		}
		catch (Exception exception) when (exception is FormatException or JsonException or IOException)
		{
			throw new InvalidDataException($"Invalid terrain stack: {exception.Message}", exception);
		}
	}

	public static string WrapForEditor(string token)
	{
		// Reserve space for the inserted newlines within the same encoded-input budget.
		var wrapped = string.Join("\n", Enumerable.Range(0, (token.Length + EditorLineLength - 1) / EditorLineLength)
			.Select(i => token.Substring(i * EditorLineLength, Math.Min(EditorLineLength, token.Length - i * EditorLineLength))));
		if (wrapped.Length > MaximumEncodedCharacters) throw new InvalidDataException("Wrapped terrain stack exceeds the encoded size limit.");
		return wrapped;
	}
}
