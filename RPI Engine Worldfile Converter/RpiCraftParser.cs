#nullable enable

using System.Text;
using System.Text.RegularExpressions;

namespace RPI_Engine_Worldfile_Converter;

public sealed class RpiCraftParser
{
	private static readonly Regex CraftHeaderRegex = new(
		@"^craft\s+(?<craft>\S+)\s+subcraft\s+(?<subcraft>\S+)\s+command\s+(?<command>\S+)\s*$",
		RegexOptions.Compiled | RegexOptions.IgnoreCase);

	private static readonly Regex SlotRegex = new(@"^(?<slot>\d+):\s*(?<value>.+)$", RegexOptions.Compiled);
	private static readonly Regex FailSlotRegex = new(@"^Fail\s+(?<slot>\d+):\s*(?<value>.+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
	private static readonly Regex SkillRegex = new(
		@"^(?<name>.+?)\s+vs\.?\s+(?<dice>\d+)[dD](?<sides>\d+)\s*$",
		RegexOptions.Compiled | RegexOptions.IgnoreCase);

	public RpiCraftCorpus ParseFile(string sourceFile)
	{
		var lines = File.ReadAllLines(sourceFile);
		var relevantLines = ExtractCraftSection(lines);
		var blocks = SplitBlocks(sourceFile, relevantLines);
		List<RpiCraftDefinition> crafts = [];
		List<RpiCraftBlockFailure> failures = [];

		foreach (var block in blocks)
		{
			try
			{
				crafts.Add(ParseBlock(sourceFile, block));
			}
			catch (Exception ex)
			{
				failures.Add(new RpiCraftBlockFailure(
					sourceFile,
					block.Header,
					ex.Message,
					block.StartLineNumber));
			}
		}

		return new RpiCraftCorpus(crafts, failures);
	}

	private static IReadOnlyList<(int LineNumber, string Text)> ExtractCraftSection(IReadOnlyList<string> lines)
	{
		var output = new List<(int LineNumber, string Text)>();
		var inCrafts = false;

		for (var i = 0; i < lines.Count; i++)
		{
			var trimmed = lines[i].Trim();
			if (trimmed.Equals("[CRAFTS]", StringComparison.OrdinalIgnoreCase))
			{
				inCrafts = true;
				continue;
			}

			if (!inCrafts)
			{
				continue;
			}

			if (trimmed.Equals("[END]", StringComparison.OrdinalIgnoreCase) ||
			    trimmed.Equals("[END-CRAFTS]", StringComparison.OrdinalIgnoreCase))
			{
				break;
			}

			if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith('#'))
			{
				continue;
			}

			output.Add((i + 1, lines[i]));
		}

		return output;
	}

	private static IReadOnlyList<CraftBlock> SplitBlocks(string sourceFile, IReadOnlyList<(int LineNumber, string Text)> lines)
	{
		List<CraftBlock> blocks = [];
		CraftBlockBuilder? current = null;
		var craftNumber = 0;

		foreach (var (lineNumber, text) in lines)
		{
			var trimmed = text.Trim();
			if (trimmed.StartsWith("craft ", StringComparison.OrdinalIgnoreCase))
			{
				if (current is not null)
				{
					blocks.Add(current.Build(sourceFile));
				}

				current = new CraftBlockBuilder(trimmed, lineNumber, ++craftNumber);
				current.AddLine(lineNumber, text);
				continue;
			}

			if (current is null)
			{
				continue;
			}

			current.AddLine(lineNumber, text);
			if (trimmed.Equals("end", StringComparison.OrdinalIgnoreCase))
			{
				blocks.Add(current.Build(sourceFile));
				current = null;
			}
		}

		if (current is not null)
		{
			blocks.Add(current.Build(sourceFile));
		}

		return blocks;
	}

	private RpiCraftDefinition ParseBlock(string sourceFile, CraftBlock block)
	{
		var headerMatch = CraftHeaderRegex.Match(block.Header);
		if (!headerMatch.Success)
		{
			throw new InvalidOperationException($"Unrecognised craft header '{block.Header}'.");
		}

		var warnings = new List<RpiCraftParseWarning>();
		var craftNumber = block.CraftNumber;
		var phases = new List<RpiCraftPhaseRecord>();
		var topLevelFailObjects = new List<int>();
		var topLevelFailMobs = new List<int>();
		List<string> flags = [];
		List<string> sectors = [];
		List<string> seasons = [];
		List<string> weather = [];
		List<int> openingIds = [];
		List<int> raceIds = [];
		List<RpiCraftClanRequirement> clans = [];
		var followers = 0;
		var icDelay = 0;
		var failDelay = 0;
		var startKey = default(int?);
		var endKey = default(int?);
		string? failureMessage = null;
		var currentPhase = new PhaseBuilder(1);

		foreach (var (lineNumber, lineText) in block.Lines.Skip(1))
		{
			var trimmed = lineText.Trim();
			if (trimmed.Equals("end", StringComparison.OrdinalIgnoreCase))
			{
				continue;
			}

			if (trimmed.Equals("phase", StringComparison.OrdinalIgnoreCase) ||
			    trimmed.Equals("phase:", StringComparison.OrdinalIgnoreCase))
			{
				if (currentPhase.HasContent)
				{
					phases.Add(currentPhase.Build());
				}

				currentPhase = new PhaseBuilder(phases.Count + 1);
				currentPhase.AddRawLine(lineText);
				continue;
			}

			if (!currentPhase.HasContent)
			{
				ParseTopLevelLine(
					trimmed,
					lineNumber,
					warnings,
					ref failureMessage,
					topLevelFailObjects,
					topLevelFailMobs,
					flags,
					sectors,
					seasons,
					weather,
					openingIds,
					raceIds,
					clans,
					ref followers,
					ref icDelay,
					ref failDelay,
					ref startKey,
					ref endKey);
				continue;
			}

			ParsePhaseLine(trimmed, lineText, lineNumber, currentPhase, warnings);
		}

		if (currentPhase.HasContent)
		{
			phases.Add(currentPhase.Build());
		}

		return new RpiCraftDefinition
		{
			SourceFile = sourceFile,
			SourceLineNumber = block.StartLineNumber,
			CraftNumber = craftNumber,
			CraftName = headerMatch.Groups["craft"].Value,
			SubcraftName = headerMatch.Groups["subcraft"].Value,
			Command = headerMatch.Groups["command"].Value,
			FailureMessage = failureMessage,
			FailObjectVnums = topLevelFailObjects,
			FailMobVnums = topLevelFailMobs,
			Constraints = new RpiCraftConstraintSet(
				flags,
				sectors,
				seasons,
				weather,
				openingIds,
				raceIds,
				clans,
				followers,
				icDelay,
				failDelay),
			VariantLink = new RpiCraftVariantLink(startKey, endKey),
			Phases = phases,
			RawLines = block.Lines.Select(x => x.Text).ToList(),
			ParseWarnings = warnings
		};
	}

	private static void ParseTopLevelLine(
		string trimmed,
		int lineNumber,
		List<RpiCraftParseWarning> warnings,
		ref string? failureMessage,
		List<int> topLevelFailObjects,
		List<int> topLevelFailMobs,
		List<string> flags,
		List<string> sectors,
		List<string> seasons,
		List<string> weather,
		List<int> openingIds,
		List<int> raceIds,
		List<RpiCraftClanRequirement> clans,
		ref int followers,
		ref int icDelay,
		ref int failDelay,
		ref int? startKey,
		ref int? endKey)
	{
		var (keyword, value) = SplitKeyword(trimmed);
		switch (keyword.ToLowerInvariant())
		{
			case "fail":
			case "failure":
				failureMessage = ReadExtendedText(value);
				return;
			case "failobj":
			case "failobjs":
				topLevelFailObjects.AddRange(ParseIntegerList(value));
				return;
			case "failmob":
			case "failmobs":
				topLevelFailMobs.AddRange(ParseIntegerList(value));
				return;
			case "flags":
				flags.AddRange(TokeniseWords(value));
				return;
			case "sectors":
				sectors.AddRange(TokeniseWords(value));
				return;
			case "seasons":
				seasons.AddRange(TokeniseWords(value));
				return;
			case "weather":
				weather.AddRange(TokeniseWords(value));
				return;
			case "opening":
				openingIds.AddRange(ParseIntegerList(value));
				return;
			case "race":
				raceIds.AddRange(ParseIntegerList(value));
				return;
			case "clans":
				clans.AddRange(ParseClanRequirements(value));
				return;
			case "followers":
				followers = ParseSingleInteger(value, 0);
				return;
			case "ic_delay":
				icDelay = ParseSingleInteger(value, 0);
				return;
			case "fail_delay":
				failDelay = ParseSingleInteger(value, 0);
				return;
			case "start_key":
				startKey = ParseNullableInteger(value);
				return;
			case "end_key":
				endKey = ParseNullableInteger(value);
				return;
			default:
				warnings.Add(new RpiCraftParseWarning("unknown-top-level", $"Unrecognised craft directive '{keyword}'.", lineNumber));
				return;
		}
	}

	private void ParsePhaseLine(
		string trimmed,
		string rawLine,
		int lineNumber,
		PhaseBuilder phase,
		List<RpiCraftParseWarning> warnings)
	{
		phase.AddRawLine(rawLine);
		var failSlotMatch = FailSlotRegex.Match(trimmed);
		if (failSlotMatch.Success)
		{
			phase.FailItemLists.Add(ParseItemList(
				int.Parse(failSlotMatch.Groups["slot"].Value),
				failSlotMatch.Groups["value"].Value));
			return;
		}

		var slotMatch = SlotRegex.Match(trimmed);
		if (slotMatch.Success)
		{
			phase.ItemLists.Add(ParseItemList(
				int.Parse(slotMatch.Groups["slot"].Value),
				slotMatch.Groups["value"].Value));
			return;
		}

		var (keyword, value) = SplitKeyword(trimmed);
		switch (keyword.ToLowerInvariant())
		{
			case "1st":
				phase.First = ReadExtendedText(value);
				return;
			case "2nd":
				phase.Second = ReadExtendedText(value);
				return;
			case "3rd":
				phase.Third = ReadExtendedText(value);
				return;
			case "self":
				phase.Self = ReadExtendedText(value);
				return;
			case "group":
				phase.Group = ReadExtendedText(value);
				return;
			case "1stfail":
			case "failure":
				phase.FirstFail = ReadExtendedText(value);
				return;
			case "2ndfail":
				phase.SecondFail = ReadExtendedText(value);
				return;
			case "3rdfail":
				phase.ThirdFail = ReadExtendedText(value);
				return;
			case "groupfail":
				phase.GroupFail = ReadExtendedText(value);
				return;
			case "skill":
			case "s":
				phase.SkillCheck = ParseCheck(value, RpiCraftCheckKind.Skill, phase.PhaseNumber, lineNumber);
				return;
			case "attr":
			case "a":
				phase.AttributeCheck = ParseCheck(value, RpiCraftCheckKind.Attribute, phase.PhaseNumber, lineNumber);
				return;
			case "t":
				phase.Seconds = ParseSingleInteger(value, 0);
				return;
			case "cost":
				phase.Cost = ParseCost(value);
				return;
			case "mob":
				phase.LoadMobVnum = ParseNullableInteger(value);
				phase.MobileFlags.AddRange(ParseMobFlags(value));
				return;
			case "tool":
				phase.ToolList = ParseItemList(0, value);
				return;
			case "spell":
				phase.SpellDefinition = value;
				return;
			case "open":
				phase.OpenDefinition = value;
				return;
			case "req":
			case "require":
				phase.RequirementDefinition = value;
				return;
			default:
				warnings.Add(new RpiCraftParseWarning("unknown-phase", $"Unrecognised phase directive '{keyword}'.", lineNumber));
				return;
		}
	}

	private static RpiCraftCheckRecord ParseCheck(string value, RpiCraftCheckKind kind, int phaseNumber, int lineNumber)
	{
		var match = SkillRegex.Match(value);
		if (!match.Success)
		{
			throw new InvalidOperationException($"Could not parse craft check '{value}' on line {lineNumber}.");
		}

		return new RpiCraftCheckRecord(
			kind,
			match.Groups["name"].Value.Trim(),
			int.Parse(match.Groups["dice"].Value),
			int.Parse(match.Groups["sides"].Value),
			phaseNumber,
			value);
	}

	private static RpiCraftCostRecord ParseCost(string value)
	{
		var tokens = TokeniseWords(value);
		var moves = 0;
		var hits = 0;

		for (var i = 0; i < tokens.Count - 1; i++)
		{
			if (tokens[i].Equals("moves", StringComparison.OrdinalIgnoreCase))
			{
				int.TryParse(tokens[i + 1], out moves);
			}

			if (tokens[i].Equals("hits", StringComparison.OrdinalIgnoreCase))
			{
				int.TryParse(tokens[i + 1], out hits);
			}
		}

		return new RpiCraftCostRecord(moves, hits);
	}

	private static IReadOnlyList<string> ParseMobFlags(string value)
	{
		return TokeniseWords(value)
			.Skip(1)
			.Where(x => !int.TryParse(x, out _))
			.ToList();
	}

	private static RpiCraftItemList ParseItemList(int slot, string value)
	{
		var trimmed = value.Trim();
		if (trimmed.StartsWith('(') && trimmed.EndsWith(')'))
		{
			trimmed = trimmed[1..^1].Trim();
		}

		var tokens = TokeniseWords(trimmed);
		var role = RpiCraftItemRole.Unknown;
		var quantity = 1;
		List<int> vnums = [];

		foreach (var token in tokens)
		{
			if (TryParseRole(token, out var parsedRole))
			{
				role = parsedRole;
				continue;
			}

			if (token.Length > 1 && (token[0] is 'x' or 'X') && int.TryParse(token[1..], out var parsedQuantity))
			{
				quantity = parsedQuantity;
				continue;
			}

			if (int.TryParse(token, out var vnum))
			{
				vnums.Add(vnum);
			}
		}

		return new RpiCraftItemList(slot, role, Math.Max(quantity, 1), vnums, value.Trim());
	}

	private static bool TryParseRole(string token, out RpiCraftItemRole role)
	{
		switch (token.ToLowerInvariant())
		{
			case "in-room":
				role = RpiCraftItemRole.InRoom;
				return true;
			case "give":
				role = RpiCraftItemRole.Give;
				return true;
			case "held":
				role = RpiCraftItemRole.Held;
				return true;
			case "wielded":
				role = RpiCraftItemRole.Wielded;
				return true;
			case "used":
				role = RpiCraftItemRole.Used;
				return true;
			case "produced":
				role = RpiCraftItemRole.Produced;
				return true;
			case "worn":
				role = RpiCraftItemRole.Worn;
				return true;
			default:
				role = RpiCraftItemRole.Unknown;
				return false;
		}
	}

	private static IReadOnlyList<RpiCraftClanRequirement> ParseClanRequirements(string value)
	{
		var tokens = TokeniseWords(value);
		List<RpiCraftClanRequirement> requirements = [];
		for (var i = 0; i + 1 < tokens.Count; i += 2)
		{
			requirements.Add(new RpiCraftClanRequirement(tokens[i], tokens[i + 1]));
		}

		return requirements;
	}

	private static List<string> TokeniseWords(string value)
	{
		List<string> output = [];
		StringBuilder builder = new();
		char? quote = null;

		foreach (var character in value)
		{
			if (quote is not null)
			{
				if (character == quote)
				{
					output.Add(builder.ToString());
					builder.Clear();
					quote = null;
				}
				else
				{
					builder.Append(character);
				}

				continue;
			}

			if (character is '\'' or '"')
			{
				if (builder.Length > 0)
				{
					output.Add(builder.ToString());
					builder.Clear();
				}

				quote = character;
				continue;
			}

			if (char.IsWhiteSpace(character))
			{
				if (builder.Length > 0)
				{
					output.Add(builder.ToString());
					builder.Clear();
				}

				continue;
			}

			builder.Append(character);
		}

		if (builder.Length > 0)
		{
			output.Add(builder.ToString());
		}

		return output;
	}

	private static string ReadExtendedText(string value)
	{
		var builder = new StringBuilder();
		var current = value.Trim();
		while (current.EndsWith('\\'))
		{
			builder.Append(current[..^1].TrimEnd());
			builder.Append(' ');
			current = current[..^1];
		}

		if (builder.Length == 0)
		{
			return value.Trim();
		}

		builder.Append(current.Trim());
		return builder.ToString();
	}

	private static List<int> ParseIntegerList(string value)
	{
		return TokeniseWords(value)
			.Where(x => int.TryParse(x, out _))
			.Select(int.Parse)
			.ToList();
	}

	private static int ParseSingleInteger(string value, int fallback)
	{
		return ParseNullableInteger(value) ?? fallback;
	}

	private static int? ParseNullableInteger(string value)
	{
		return TokeniseWords(value)
			.Select(token => int.TryParse(token, out var parsed) ? (int?)parsed : null)
			.FirstOrDefault(x => x.HasValue);
	}

	private static (string Keyword, string Value) SplitKeyword(string trimmed)
	{
		var index = trimmed.IndexOf(':');
		if (index == -1)
		{
			return (trimmed, string.Empty);
		}

		return (trimmed[..index].Trim(), trimmed[(index + 1)..].Trim());
	}

	private sealed record CraftBlock(
		string SourceFile,
		string Header,
		int StartLineNumber,
		int CraftNumber,
		IReadOnlyList<(int LineNumber, string Text)> Lines);

	private sealed class CraftBlockBuilder
	{
		private readonly List<(int LineNumber, string Text)> _lines = [];

		public CraftBlockBuilder(string header, int startLineNumber, int craftNumber)
		{
			Header = header;
			StartLineNumber = startLineNumber;
			CraftNumber = craftNumber;
		}

		public string Header { get; }
		public int StartLineNumber { get; }
		public int CraftNumber { get; }

		public void AddLine(int lineNumber, string text)
		{
			_lines.Add((lineNumber, text));
		}

		public CraftBlock Build(string sourceFile)
		{
			return new CraftBlock(sourceFile, Header, StartLineNumber, CraftNumber, _lines.ToList());
		}
	}

	private sealed class PhaseBuilder
	{
		public PhaseBuilder(int phaseNumber)
		{
			PhaseNumber = phaseNumber;
		}

		public int PhaseNumber { get; }
		public int Seconds { get; set; }
		public string? First { get; set; }
		public string? Second { get; set; }
		public string? Third { get; set; }
		public string? Self { get; set; }
		public string? Group { get; set; }
		public string? FirstFail { get; set; }
		public string? SecondFail { get; set; }
		public string? ThirdFail { get; set; }
		public string? GroupFail { get; set; }
		public RpiCraftCheckRecord? SkillCheck { get; set; }
		public RpiCraftCheckRecord? AttributeCheck { get; set; }
		public RpiCraftCostRecord? Cost { get; set; }
		public List<RpiCraftItemList> ItemLists { get; } = [];
		public List<RpiCraftItemList> FailItemLists { get; } = [];
		public RpiCraftItemList? ToolList { get; set; }
		public int? LoadMobVnum { get; set; }
		public List<string> MobileFlags { get; } = [];
		public string? SpellDefinition { get; set; }
		public string? OpenDefinition { get; set; }
		public string? RequirementDefinition { get; set; }
		private List<string> RawLines { get; } = [];

		public bool HasContent =>
			RawLines.Count > 0 ||
			First is not null ||
			Second is not null ||
			Third is not null ||
			Self is not null ||
			Group is not null ||
			FirstFail is not null ||
			SecondFail is not null ||
			ThirdFail is not null ||
			GroupFail is not null ||
			ItemLists.Count > 0 ||
			FailItemLists.Count > 0 ||
			ToolList is not null ||
			LoadMobVnum is not null ||
			SkillCheck is not null ||
			AttributeCheck is not null ||
			Cost is not null ||
			Seconds > 0;

		public void AddRawLine(string rawLine)
		{
			RawLines.Add(rawLine);
		}

		public RpiCraftPhaseRecord Build()
		{
			return new RpiCraftPhaseRecord
			{
				PhaseNumber = PhaseNumber,
				Seconds = Seconds,
				Echoes = new RpiCraftEchoSet(
					First,
					Second,
					Third,
					Self,
					Group,
					FirstFail,
					SecondFail,
					ThirdFail,
					GroupFail),
				SkillCheck = SkillCheck,
				AttributeCheck = AttributeCheck,
				Cost = Cost,
				ItemLists = ItemLists.OrderBy(x => x.Slot).ToList(),
				FailItemLists = FailItemLists.OrderBy(x => x.Slot).ToList(),
				ToolList = ToolList,
				LoadMobVnum = LoadMobVnum,
				MobileFlags = MobileFlags.ToList(),
				SpellDefinition = SpellDefinition,
				OpenDefinition = OpenDefinition,
				RequirementDefinition = RequirementDefinition,
				RawLines = RawLines.ToList(),
			};
		}
	}
}
