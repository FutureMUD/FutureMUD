#nullable enable
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Construction.Autobuilder;

namespace MudSharp_Unit_Tests;

[TestClass]
public class TerrainStackCodecTests
{
	private static TerrainStackPayload Example => new(2, 1, [-1, 0],
		[new(0, 0, -1, 2, [5, 9]), new(1, 0, -1, 3, []), new(0, 0, 0, 4, [9])],
		[new(0, 0, -1, Climb: true, Difficulty: 7, Fall: true)]);

	[TestMethod]
	public void Codec_WrappedExport_RoundTripsAllFields()
	{
		var token = TerrainStackCodec.Encode(Example);
		Assert.IsTrue(token.StartsWith("FMS1:"));
		Assert.IsFalse(token.Contains('='));
		var decoded = TerrainStackCodec.Decode(" \r\n" + TerrainStackCodec.WrapForEditor(token) + "\t");
		Assert.AreEqual(JsonSerializer.Serialize(Example), JsonSerializer.Serialize(decoded));
	}

	[TestMethod]
	public void Codec_InvalidInput_RejectsMalformedTruncatedAndDamagedTokens()
	{
		var token = TerrainStackCodec.Encode(Example);
		foreach (var text in new[] { "FMS2:" + token[5..], token[..30], "FMS1:@@", token + "=", "FMS1:" + (token[5] == 'A' ? 'B' : 'A') + token[6..] })
			Assert.ThrowsExactly<InvalidDataException>(() => TerrainStackCodec.Decode(text));
	}

	[TestMethod]
	public void Validate_InvalidTopology_RejectsBeforeImport()
	{
		var example = Example;
		foreach (var payload in new[]
		{
			example with { Levels = [-1, -1] }, example with { Width = 201 }, example with { Version = 2 },
			example with { Rooms = [example.Rooms[0], example.Rooms[0]] },
			example with { Rooms = [example.Rooms[0] with { Tags = [0] }] },
			example with { Links = [new(1, 0, -1)] }, example with { Links = [new(0, 0, -1, Difficulty: 11)] },
			example with { Links = [example.Links[0], example.Links[0]] }, example with { Rooms = [] }
		}) Assert.ThrowsExactly<InvalidDataException>(() => TerrainStackCodec.Validate(payload));
	}

	[TestMethod]
	public void Decode_EncodedAndDecompressionLimits_RejectsBoundedly()
	{
		Assert.ThrowsExactly<InvalidDataException>(() => TerrainStackCodec.Decode(new string(' ', TerrainStackCodec.MaximumEncodedCharacters + 1)));
		using var output = new MemoryStream();
		output.Write(new byte[32]);
		using (var compressor = new BrotliStream(output, CompressionLevel.Fastest, true))
		{
			var block = new byte[8192];
			for (var count = 0; count <= TerrainStackCodec.MaximumDecodedBytes / block.Length; count++) compressor.Write(block);
		}
		var token = "FMS1:" + Convert.ToBase64String(output.ToArray()).TrimEnd('=').Replace('+', '-').Replace('/', '_');
		Assert.ThrowsExactly<InvalidDataException>(() => TerrainStackCodec.Decode(token));
	}

	[TestMethod]
	public void Codec_MapPatterns_ReportsSizesAndWrapsWithinBudget()
	{
		foreach (var pattern in new[] { "uniform", "mixed", "tag-heavy" })
		{
			var rooms = Enumerable.Range(0, 3000).Select(i => new TerrainStackRoom(i % 50, i / 50 % 20, i / 1000 - 1,
				pattern == "uniform" ? 1 : i * 7919L % 97 + 1,
				pattern == "tag-heavy" ? new[] { i * 13L + 1, i * 13L + 2, i * 13L + 3 } : [])).ToArray();
			var payload = new TerrainStackPayload(50, 20, [-1, 0, 1], rooms, []);
			var token = TerrainStackCodec.Encode(payload);
			Console.WriteLine($"{pattern}: {JsonSerializer.SerializeToUtf8Bytes(payload).Length} JSON bytes -> {token.Length} token characters");
			Assert.IsTrue(TerrainStackCodec.WrapForEditor(token).Split('\n').All(line => line.Length <= TerrainStackCodec.EditorLineLength));
			Assert.AreEqual(3000, TerrainStackCodec.Decode(TerrainStackCodec.WrapForEditor(token)).Rooms.Length);
		}
	}
}
