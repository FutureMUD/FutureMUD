#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace MudSharp.Database;

/// <summary>Only the removed parent identity is translated. Cell and Room:v2 already name the retained child.</summary>
internal sealed class RoomReferenceMap
{
	private readonly Dictionary<long, long?> _children = new();
	private readonly HashSet<long> _retainedIds;

	public RoomReferenceMap(IEnumerable<(long Parent, long? Child)> rows, IEnumerable<long> retainedIds)
	{
		_retainedIds = retainedIds.ToHashSet();
		var children = new HashSet<long>();
		foreach (var (parent, child) in rows)
		{
			if (parent <= 0 || !_children.TryAdd(parent, child) || (child is not null && (child <= 0 || !children.Add(child.Value))))
			{
				throw new InvalidOperationException($"Room reference correction: ambiguous parent mapping #{parent}.");
			}
		}
	}

	public long Resolve(long parent, string context)
	{
		if (!_children.TryGetValue(parent, out var child) || child is null || !_retainedIds.Contains(child.Value))
		{
			throw new InvalidOperationException($"Room reference correction: {context}: legacy Room #{parent} has no surviving Cell; inspect the live relationship or final contraction ledger.");
		}
		return child.Value;
	}

	public long? Convert(string? type, long? id, string context, bool zeroIsAbsent = false)
	{
		if (id is null || (zeroIsAbsent && id == 0)) return null;
		if (string.IsNullOrWhiteSpace(type))
			throw new InvalidOperationException($"Room reference correction: {context}: non-null target ID has no type discriminator; its identity namespace is ambiguous.");
		ValidateType(type, context);
		if (type != "Room") return null;
		if (id <= 0) throw new InvalidOperationException($"Room reference correction: {context}: legacy Room ID must be positive.");
		return Resolve(id.Value, context);
	}

	internal static void ValidateType(string? type, string context)
	{
		if (type is null or "Room" or "Room:v2" or "Cell") return;
		var trimmed = type.Trim();
		if (trimmed.Equals("Room", StringComparison.OrdinalIgnoreCase) || trimmed.StartsWith("Room:", StringComparison.OrdinalIgnoreCase))
		{
			throw new InvalidOperationException($"Room reference correction: {context}: malformed or unsupported Room type discriminator; exact Room, Cell or Room:v2 required.");
		}
	}
}

internal static class RoomReferenceXml
{
	public static string? Rewrite(string? source, bool emote, RoomReferenceMap map, string context)
	{
		if (string.IsNullOrWhiteSpace(source)) return source;
		XDocument document;
		try
		{
			using var reader = XmlReader.Create(new StringReader(source), new XmlReaderSettings
			{
				DtdProcessing = DtdProcessing.Prohibit,
				XmlResolver = null
			});
			document = XDocument.Load(reader, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
		}
		catch (XmlException ex)
		{
			throw new InvalidOperationException($"Room reference correction: {context}: malformed supported XML at line {ex.LineNumber}, column {ex.LinePosition}.");
		}
		if (document.Root?.Name != (emote ? "Emote" : "Effects"))
		{
			throw new InvalidOperationException($"Room reference correction: {context}: expected {(emote ? "Emote" : "Effects")} root.");
		}

		var patches = new List<(int Start, int Length, string Value)>();
		var lineStarts = new List<int> { 0 };
		for (var i = 0; i < source.Length; i++)
		{
			if (source[i] == '\r')
			{
				if (i + 1 < source.Length && source[i + 1] == '\n') i++;
				lineStarts.Add(i + 1);
			}
			else if (source[i] == '\n') lineStarts.Add(i + 1);
		}

		void Patch(XObject node, string replacement)
		{
			var line = (IXmlLineInfo)node;
			var start = lineStarts[line.LineNumber - 1] + line.LinePosition - 1;
			if (node is XAttribute)
			{
				start = source.IndexOf('=', start) + 1;
				while (char.IsWhiteSpace(source[start])) start++;
				var quote = source[start++];
				var end = source.IndexOf(quote, start);
				patches.Add((start, end - start, replacement));
				return;
			}
			// Element line information points at its name, immediately after '<'.
			var delimiter = '\0';
			while (start < source.Length)
			{
				var ch = source[start++];
				if (delimiter != '\0') { if (ch == delimiter) delimiter = '\0'; continue; }
				if (ch is '\'' or '"') { delimiter = ch; continue; }
				if (ch == '>') break;
			}
			var close = source.IndexOf("</", start, StringComparison.Ordinal);
			patches.Add((start, close - start, replacement));
		}

		void Pair(XObject? typeNode, XObject? idNode, string path, bool zeroIsAbsent = false)
		{
			var type = typeNode switch { XAttribute a => a.Value, XElement e => e.Value, _ => null };
			var idText = idNode switch { XAttribute a => a.Value, XElement e => e.Value, _ => null };
			if (zeroIsAbsent && idText == "0") return;
			if (string.IsNullOrWhiteSpace(type) && !string.IsNullOrEmpty(idText))
				throw new InvalidOperationException($"Room reference correction: {context}{path}: target ID has no type discriminator; its identity namespace is ambiguous.");
			RoomReferenceMap.ValidateType(type, $"{context}{path}");
			if (type != "Room") return;
			if (!long.TryParse(idText, NumberStyles.None, CultureInfo.InvariantCulture, out var id) || id <= 0)
			{
				throw new InvalidOperationException($"Room reference correction: {context}{path}: missing or malformed legacy Room ID.");
			}
			foreach (var node in new[] { typeNode, idNode })
			{
				if (node is XElement element && element.Nodes().Any(x => x is not XText))
					throw new InvalidOperationException($"Room reference correction: {context}{path}: ambiguous scalar XML.");
			}
			var child = map.Resolve(id, $"{context}{path}");
			Patch(typeNode!, "Cell");
			Patch(idNode!, child.ToString(CultureInfo.InvariantCulture));
		}

		XElement? Single(XElement parent, string name, string path)
		{
			var elements = parent.Elements(name).ToList();
			if (elements.Count > 1)
				throw new InvalidOperationException($"Room reference correction: {context}{path}: duplicate {name} elements.");
			return elements.SingleOrDefault();
		}

		if (emote)
		{
			var ordinal = 0;
			foreach (var token in document.Root.Elements("Token"))
			{
				var path = $"/Emote/Token[{++ordinal}]";
				Pair(token.Attribute("TargetType"), token.Attribute("TargetId"), path + "/@Target");
				Pair(token.Attribute("OtherType"), token.Attribute("OtherId"), path + "/@Other");
			}
		}
		else
		{
			var pending = new Queue<(XElement Effect, string Path)>();
			var ordinal = 0;
			foreach (var effect in document.Root.Elements("Effect")) pending.Enqueue((effect, $"/Effects/Effect[{++ordinal}]"));
			while (pending.TryDequeue(out var next))
			{
				var (effect, path) = next;
				var kind = Single(effect, "Type", path)?.Value;
				if (!RoomReferenceEffectTypes.IsKnown(kind))
					throw new InvalidOperationException($"Room reference correction: {context}{path}: missing/custom/unclassified effect factory; explicitly audit its serializer before cutover.");
				if (kind is not ("ZeroGravityTether" or "SpellZeroGravityTether" or "OverrideDescFromProg" or "OverrideSDescFromProg" or "CheckResult" or "MagicSpellParent")) continue;
				var payload = Single(effect, "Effect", path) ?? throw new InvalidOperationException($"Room reference correction: {context}{path}: missing {kind} payload.");
				if (kind == "MagicSpellParent")
				{
					var children = Single(payload, "Children", path);
					if (children is null) continue;
					var childOrdinal = 0;
					foreach (var child in children.Elements("Effect")) pending.Enqueue((child, $"{path}/Effect/Children/Effect[{++childOrdinal}]"));
				}
				else if (kind is "ZeroGravityTether" or "SpellZeroGravityTether")
				{
					Pair(Single(payload, "AnchorType", path), Single(payload, "AnchorId", path), path + "/Effect/Anchor");
				}
				else if (kind is "OverrideDescFromProg" or "OverrideSDescFromProg")
				{
					var fixedPerceiver = Single(payload, "FixedPerceiver", path);
					Pair(fixedPerceiver?.Attribute("type"), fixedPerceiver?.Attribute("id"), path + "/Effect/FixedPerceiver", true);
				}
				else if (kind == "CheckResult")
				{
					// Older writers copied TargetType into ToolType. Two legacy Room types with a tool
					// cannot establish that the recorded tool ID really belongs to the parent namespace.
					if (payload.Attribute("TargetType")?.Value == "Room" && payload.Attribute("ToolType")?.Value == "Room" && payload.Attribute("ToolId")?.Value is not (null or "0"))
						throw new InvalidOperationException($"Room reference correction: {context}{path}: ambiguous historical CheckResult ToolType (older writer copied TargetType); explicit disposition required.");
					Pair(payload.Attribute("TargetType"), payload.Attribute("TargetId"), path + "/Effect/@Target", true);
					Pair(payload.Attribute("ToolType"), payload.Attribute("ToolId"), path + "/Effect/@Tool", true);
				}
			}
		}

		if (patches.Count == 0) return source;
		var result = new StringBuilder(source);
		var previousStart = source.Length;
		foreach (var patch in patches.OrderByDescending(x => x.Start))
		{
			if (patch.Start < 0 || patch.Length < 0 || patch.Start + patch.Length > previousStart)
				throw new InvalidOperationException($"Room reference correction: {context}: overlapping or invalid XML edits.");
			result.Remove(patch.Start, patch.Length).Insert(patch.Start, patch.Value);
			previousStart = patch.Start;
		}
		return result.ToString();
	}
}
