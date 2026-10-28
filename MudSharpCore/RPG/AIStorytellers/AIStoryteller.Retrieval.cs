#nullable enable
using System.Text.Json;
using System.Text.Json.Nodes;

namespace MudSharp.RPG.AIStorytellers;

public partial class AIStoryteller
{
	private const int MaximumToolResultCharacters = 8000;
	private static readonly IReadOnlyDictionary<string, string> PagedTools = new Dictionary<string, string>
	{
		["OnlinePlayers"] = "Players", ["ListMemoriesForPlayer"] = "Memories", ["PlayerInformation"] = "Memories",
		["Landmarks"] = "Landmarks", ["SearchReferenceDocuments"] = "Documents", ["RecentCharacterPlans"] = "Plans"
	};

	private static string AddRetrievalSchema(string name, string? schema)
	{
		var paged = PagedTools.ContainsKey(name);
		var chunked = name is "ShowMemory" or "ShowReferenceDocument";
		if (!paged && !chunked) return schema ?? EmptyStrictSchema;
		var node = JsonNode.Parse(schema ?? EmptyStrictSchema)!.AsObject();
		var properties = node["properties"]!.AsObject();
		properties["Offset"] = new JsonObject { ["type"] = "integer", ["minimum"] = 0,
			["description"] = chunked ? "Character offset; default 0." : "Result offset; default 0." };
		properties["Limit"] = new JsonObject { ["type"] = "integer", ["minimum"] = 1,
			["maximum"] = chunked ? 6000 : 100, ["description"] = chunked ? "Maximum characters, default 6000." : "Maximum entries, default 20." };
		return node.ToJsonString();
	}

	internal static ToolExecutionResult BoundToolResult(string name, JsonElement arguments, ToolExecutionResult result)
	{
		if (result.MalformedJson) return result;
		var envelope = JsonNode.Parse(result.OutputJson)!.AsObject();
		if (envelope["result"] is not JsonObject payload) return LimitResult(result);
		if (!PagedTools.ContainsKey(name) && name is not ("ShowMemory" or "ShowReferenceDocument")) return LimitResult(result);
		var offset = TryGetOptionalLong(arguments, "Offset", out var o) ? o : 0;
		var chunked = name is "ShowMemory" or "ShowReferenceDocument";
		var limit = TryGetOptionalLong(arguments, "Limit", out var l) ? l : chunked ? 6000 : 20;
		if (offset < 0 || offset > int.MaxValue || limit < 1 || limit > (chunked ? 6000 : 100))
			return ErrorResult("Invalid Offset or Limit.");
		if (PagedTools.TryGetValue(name, out var key) && payload[key] is JsonArray list)
		{
			if (key == "Memories") foreach (var entry in list.OfType<JsonObject>()) entry.Remove("Details");
			var all = list.Select(x => x?.DeepClone()).ToList();
			var page = new JsonArray();
			payload[key] = page;
			payload["Total"] = all.Count;
			payload["Offset"] = offset;
			payload["NextOffset"] = null;
			for (var i = (int)Math.Min(offset, all.Count); i < all.Count && page.Count < limit; i++)
			{
				page.Add(all[i]);
				payload["NextOffset"] = i + 1 < all.Count ? JsonValue.Create(i + 1) : null;
				if (envelope.ToJsonString().Length <= MaximumToolResultCharacters) continue;
				page.RemoveAt(page.Count - 1);
				payload["NextOffset"] = i;
				if (page.Count == 0) return ErrorResult("A result entry exceeds the 8,000-character limit. Narrow the query or shorten its metadata.");
				break;
			}
		}
		else if (chunked)
		{
			var keyName = name == "ShowMemory" ? "Description" : "Contents";
			var text = (string?)payload[keyName] ?? "";
			var start = (int)Math.Min(offset, text.Length);
			var length = (int)Math.Min(limit, text.Length - start);
			payload["TotalCharacters"] = text.Length;
			payload["Offset"] = start;
			do
			{
				// Preserve UTF-16 surrogate pairs at chunk boundaries.
				if (length > 0 && start + length < text.Length && char.IsHighSurrogate(text[start + length - 1])) length--;
				payload[keyName] = text.Substring(start, length);
				payload["NextOffset"] = start + length < text.Length ? JsonValue.Create(start + length) : null;
				if (envelope.ToJsonString().Length <= MaximumToolResultCharacters && (length > 0 || start == text.Length)) break;
				length /= 2;
			} while (length > 0);
			if (length == 0 && start < text.Length) return ErrorResult("Document metadata exceeds the result limit or Limit is too small; narrow the result or increase Limit.");
		}
		return LimitResult(new ToolExecutionResult(envelope.ToJsonString(), false));
	}

	private static ToolExecutionResult LimitResult(ToolExecutionResult result) => result.OutputJson.Length <= MaximumToolResultCharacters
		? result : ErrorResult("Tool result exceeds the 8,000-character safeguard. Narrow the request or reduce the custom tool output; the tool has already executed and must not be repeated merely to retrieve its result.");
}
