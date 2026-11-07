using TerrainPlanner.Contracts.StackTransport;

namespace TerrainPlanner.Contracts;

/// <summary>Browser-compatible Brotli encoding; the shared wire codec has no third-party dependencies.</summary>
public static class PlannerStackExport
{
	public static string Encode(TerrainStackPayload payload) => TerrainStackCodec.Encode(payload,
		json => BrotliSharpLib.Brotli.CompressBuffer(json, 0, json.Length, 4, 22));
}
