#nullable enable

using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace RPI_Engine_Worldfile_Converter;

public sealed class RpiRoomWorldfileParser
{
	private const int WeatherDescriptionCount = 12;
	private const int AlasDescriptionCount = 6;

	private static readonly Regex BlockStartRegex = new(@"^#(?<vnum>\d+)$", RegexOptions.Compiled);
	private static readonly Regex ZoneFileRegex = new(@"rooms\.(?<zone>\d+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

	public RpiParsedRoomCorpus ParseDirectory(string directory)
	{
		List<RpiRoomRecord> rooms = [];
		List<RpiRoomBlockFailure> failures = [];

		foreach (var file in Directory
			         .EnumerateFiles(directory, "rooms.*", SearchOption.TopDirectoryOnly)
			         .OrderBy(GetZoneNumber)
			         .ThenBy(Path.GetFileName, StringComparer.OrdinalIgnoreCase))
		{
			ParseFile(file, rooms, failures);
		}

		return new RpiParsedRoomCorpus(rooms, failures);
	}

	private void ParseFile(string file, List<RpiRoomRecord> rooms, List<RpiRoomBlockFailure> failures)
	{
		var lines = File.ReadAllLines(file, Encoding.ASCII);
		List<string>? currentBlock = null;

		for (var i = 0; i < lines.Length; i++)
		{
			var trimmed = lines[i].Trim();
			if (trimmed == "$")
			{
				if (currentBlock is not null)
				{
					TryParseBlock(file, currentBlock, rooms, failures);
					currentBlock = null;
				}

				break;
			}

			if (BlockStartRegex.IsMatch(trimmed))
			{
				if (currentBlock is null)
				{
					currentBlock = [lines[i]];
				}
				continue;
			}

			if (currentBlock is not null)
			{
				currentBlock.Add(lines[i]);
				if (trimmed == "S")
				{
					TryParseBlock(file, currentBlock, rooms, failures);
					currentBlock = null;
				}
			}
		}

		if (currentBlock is not null)
		{
			TryParseBlock(file, currentBlock, rooms, failures);
		}
	}

	private void TryParseBlock(
		string file,
		IReadOnlyList<string> block,
		List<RpiRoomRecord> rooms,
		List<RpiRoomBlockFailure> failures)
	{
		var zone = GetZoneNumber(file);
		try
		{
			rooms.Add(ParseBlock(file, zone, block));
		}
		catch (Exception ex)
		{
			failures.Add(new RpiRoomBlockFailure(file, zone, block.FirstOrDefault(), ex.Message));
		}
	}

	private RpiRoomRecord ParseBlock(string file, int zone, IReadOnlyList<string> block)
	{
		if (block.Count < 5)
		{
			throw new InvalidOperationException("Room block is truncated.");
		}

		var warnings = new List<RpiRoomParseWarning>();
		var index = 0;
		var vnum = ParseVnum(block[index++]);
		var name = ReadTildeString(block, ref index, warnings, "room name");
		var description = ReadTildeString(block, ref index, warnings, "room description");
		var headerValues = ReadIntLine(block, ref index, "room header values");
		if (headerValues.Count < 3)
		{
			throw new InvalidOperationException($"Room #{vnum} is missing room flags or sector data.");
		}

		var rawFlags = unchecked((uint)headerValues[1]);
		var roomFlags = (RpiRoomFlags)rawFlags;
		var rawSectorType = headerValues[2];
		var sectorType = Enum.IsDefined(typeof(RpiRoomSectorType), rawSectorType)
			? (RpiRoomSectorType)rawSectorType
			: RpiRoomSectorType.Inside;
		var deityValues = ReadIntLine(block, ref index, "deity line");
		var deity = deityValues.Count > 0 ? deityValues[0] : 0;

		List<RpiRoomExitRecord> exits = [];
		List<RpiRoomSecretRecord> secrets = [];
		List<RpiRoomExtraDescriptionRecord> extraDescriptions = [];
		List<RpiRoomWrittenDescriptionRecord> writtenDescriptions = [];
		List<RpiRoomProgRecord> roomProgs = [];
		RpiRoomWeatherRecord? weather = null;
		int? xeroxSourceVnum = null;
		int? capacity = null;
		List<string> legacyTrailerLines = [];

		while (index < block.Count)
		{
			var marker = block[index].Trim();
			if (string.IsNullOrWhiteSpace(marker))
			{
				index++;
				continue;
			}

			var normalisedMarker = marker.ToUpperInvariant();
			if (normalisedMarker == "S")
			{
				index++;
				break;
			}

			if (TryParseExitMarker(normalisedMarker, out var sectionType, out var direction))
			{
				index++;
				exits.Add(ParseExit(block, ref index, warnings, direction, sectionType));
				continue;
			}

			if (TryParseSecretMarker(normalisedMarker, out direction))
			{
				index++;
				secrets.Add(ParseSecret(block, ref index, warnings, direction));
				continue;
			}

			switch (normalisedMarker)
			{
				case "E":
					index++;
					extraDescriptions.Add(new RpiRoomExtraDescriptionRecord(
						ReadTildeString(block, ref index, warnings, "extra description keyword"),
						ReadTildeString(block, ref index, warnings, "extra description text")));
					continue;
				case "W":
					index++;
					var languageValues = ReadIntLine(block, ref index, "written description language");
					writtenDescriptions.Add(new RpiRoomWrittenDescriptionRecord(
						languageValues.FirstOrDefault(),
						ReadTildeString(block, ref index, warnings, "written description")));
					continue;
				case "P":
					index++;
					roomProgs.Add(new RpiRoomProgRecord(
						ReadTildeString(block, ref index, warnings, "room prog command"),
						ReadTildeString(block, ref index, warnings, "room prog keys"),
						ReadTildeString(block, ref index, warnings, "room prog script")));
					continue;
				case "A":
					index++;
					weather = ParseWeather(block, ref index, warnings);
					continue;
				case "C":
					index++;
					xeroxSourceVnum = ReadIntLine(block, ref index, "xerox source").FirstOrDefault();
					continue;
				case "X":
					index++;
					capacity = ReadIntLine(block, ref index, "room capacity").FirstOrDefault();
					continue;
				default:
					legacyTrailerLines.Add(block[index]);
					warnings.Add(new RpiRoomParseWarning(
						"unknown-room-marker",
						$"Encountered unknown room marker '{marker}' while parsing room #{vnum}."));
					index++;
					continue;
			}
		}

		if (index < block.Count)
		{
			foreach (var trailer in block.Skip(index))
			{
				if (string.IsNullOrWhiteSpace(trailer))
				{
					continue;
				}

				legacyTrailerLines.Add(trailer);
			}
		}

		if (legacyTrailerLines.Count > 0)
		{
			warnings.Add(new RpiRoomParseWarning(
				"legacy-trailer",
				$"Room block contained {legacyTrailerLines.Count} unparsed trailer line(s)."));
		}

		return new RpiRoomRecord
		{
			Vnum = vnum,
			SourceFile = file,
			Zone = zone,
			Name = name,
			Description = description,
			RawFlags = rawFlags,
			RoomFlags = roomFlags,
			RawSectorType = rawSectorType,
			SectorType = sectorType,
			Deity = deity,
			XeroxSourceVnum = xeroxSourceVnum,
			Capacity = capacity,
			Exits = exits,
			Secrets = secrets,
			ExtraDescriptions = extraDescriptions,
			WrittenDescriptions = writtenDescriptions,
			RoomProgs = roomProgs,
			Weather = weather,
			LegacyTrailerLines = legacyTrailerLines,
			ParseWarnings = warnings,
		};
	}

	private static int ParseVnum(string header)
	{
		var match = BlockStartRegex.Match(header.Trim());
		if (!match.Success)
		{
			throw new InvalidOperationException($"Invalid room header '{header}'.");
		}

		return int.Parse(match.Groups["vnum"].Value, CultureInfo.InvariantCulture);
	}

	private static RpiRoomExitRecord ParseExit(
		IReadOnlyList<string> block,
		ref int index,
		List<RpiRoomParseWarning> warnings,
		RpiRoomDirection direction,
		RpiRoomExitSectionType sectionType)
	{
		var generalDescription = ReadTildeString(block, ref index, warnings, "exit description");
		var keyword = ReadTildeString(block, ref index, warnings, "exit keyword");
		var numericValues = ReadIntLine(block, ref index, "exit numeric values");
		if (numericValues.Count < 3)
		{
			throw new InvalidOperationException($"Exit {direction} is missing door or destination numeric values.");
		}

		var doorTypeValue = numericValues[0];
		var keyVnum = numericValues[1];
		var pickPenalty = numericValues.Count >= 4 ? numericValues[2] : 0;
		var destinationVnum = numericValues.Count >= 4 ? numericValues[3] : numericValues[2];
		var doorType = Enum.IsDefined(typeof(RpiRoomDoorType), doorTypeValue)
			? (RpiRoomDoorType)doorTypeValue
			: RpiRoomDoorType.None;

		return new RpiRoomExitRecord(
			direction,
			sectionType,
			generalDescription,
			keyword,
			doorType,
			keyVnum,
			pickPenalty,
			destinationVnum);
	}

	private static RpiRoomSecretRecord ParseSecret(
		IReadOnlyList<string> block,
		ref int index,
		List<RpiRoomParseWarning> warnings,
		RpiRoomDirection direction)
	{
		var difficulty = ReadIntLine(block, ref index, "secret difficulty").FirstOrDefault();
		var searchText = ReadTildeString(block, ref index, warnings, "secret search text");
		return new RpiRoomSecretRecord(direction, difficulty, searchText);
	}

	private static RpiRoomWeatherRecord ParseWeather(
		IReadOnlyList<string> block,
		ref int index,
		List<RpiRoomParseWarning> warnings)
	{
		List<string?> weatherDescriptions = [];
		for (var i = 0; i < WeatherDescriptionCount; i++)
		{
			weatherDescriptions.Add(NormaliseNullable(ReadTildeString(block, ref index, warnings, $"weather description {i + 1}")));
		}

		List<string?> alasDescriptions = [];
		for (var i = 0; i < AlasDescriptionCount; i++)
		{
			alasDescriptions.Add(NormaliseNullable(ReadTildeString(block, ref index, warnings, $"alas description {i + 1}")));
		}

		return new RpiRoomWeatherRecord(weatherDescriptions, alasDescriptions);
	}

	private static bool TryParseExitMarker(
		string marker,
		out RpiRoomExitSectionType sectionType,
		out RpiRoomDirection direction)
	{
		sectionType = RpiRoomExitSectionType.Normal;
		direction = RpiRoomDirection.North;
		if (marker.Length != 2)
		{
			return false;
		}

		if (!RpiRoomDirections.TryParse(marker[1].ToString(), out direction))
		{
			return false;
		}

		switch (marker[0])
		{
			case 'D':
				sectionType = RpiRoomExitSectionType.Normal;
				return true;
			case 'H':
				sectionType = RpiRoomExitSectionType.Hidden;
				return true;
			case 'T':
				sectionType = RpiRoomExitSectionType.Trapped;
				return true;
			case 'B':
				sectionType = RpiRoomExitSectionType.HiddenTrapped;
				return true;
			default:
				return false;
		}
	}

	private static bool TryParseSecretMarker(string marker, out RpiRoomDirection direction)
	{
		direction = RpiRoomDirection.North;
		return marker.Length == 2 &&
		       marker[0] == 'Q' &&
		       RpiRoomDirections.TryParse(marker[1].ToString(), out direction);
	}

	private static string ReadTildeString(
		IReadOnlyList<string> block,
		ref int index,
		List<RpiRoomParseWarning> warnings,
		string description)
	{
		if (index >= block.Count)
		{
			warnings.Add(new RpiRoomParseWarning("unexpected-eof", $"Unexpected end of block while reading {description}."));
			return string.Empty;
		}

		StringBuilder sb = new();
		while (index < block.Count)
		{
			var line = block[index++];
			var terminatorIndex = line.IndexOf('~');
			if (terminatorIndex >= 0)
			{
				sb.Append(line[..terminatorIndex]);
				return sb.ToString();
			}

			sb.AppendLine(line);
		}

		warnings.Add(new RpiRoomParseWarning(
			"unterminated-string",
			$"Tilde-terminated string for {description} ran to the end of the block."));
		return sb.ToString();
	}

	private static List<int> ReadIntLine(IReadOnlyList<string> block, ref int index, string description)
	{
		if (index >= block.Count)
		{
			throw new InvalidOperationException($"Unexpected end of block while reading {description}.");
		}

		if (!TryParseIntTokens(block[index], out var values))
		{
			throw new InvalidOperationException($"Expected numeric {description}, got '{block[index]}'.");
		}

		index++;
		return values;
	}

	private static bool TryParseIntTokens(string line, out List<int> values)
	{
		values = [];
		foreach (var token in line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
		{
			if (!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
			{
				values.Clear();
				return false;
			}

			values.Add(parsed);
		}

		return values.Count > 0;
	}

	private static string? NormaliseNullable(string? value)
	{
		return string.IsNullOrWhiteSpace(value) ? null : value;
	}

	private static int GetZoneNumber(string file)
	{
		var match = ZoneFileRegex.Match(file);
		return match.Success ? int.Parse(match.Groups["zone"].Value, CultureInfo.InvariantCulture) : -1;
	}
}
