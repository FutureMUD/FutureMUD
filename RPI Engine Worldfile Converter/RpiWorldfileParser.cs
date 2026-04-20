#nullable enable

using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace RPI_Engine_Worldfile_Converter;

public sealed class RpiWorldfileParser
{
	private static readonly Regex BlockStartRegex = new(@"^#(?<vnum>\d+)$", RegexOptions.Compiled);
	private static readonly Regex ZoneFileRegex = new(@"objs\.(?<zone>\d+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
	private static readonly HashSet<string> KnownTailMarkers = ["A", "C", "E", "P"];

	public RpiParsedCorpus ParseDirectory(string directory)
	{
		List<RpiItemRecord> items = [];
		List<RpiItemBlockFailure> failures = [];

		foreach (var file in Directory
			         .EnumerateFiles(directory, "objs.*", SearchOption.TopDirectoryOnly)
			         .OrderBy(GetZoneNumber)
			         .ThenBy(Path.GetFileName, StringComparer.OrdinalIgnoreCase))
		{
			ParseFile(file, items, failures);
		}

		return new RpiParsedCorpus(items, failures);
	}

	private void ParseFile(string file, List<RpiItemRecord> items, List<RpiItemBlockFailure> failures)
	{
		var lines = File.ReadAllLines(file, Encoding.ASCII);
		List<string>? currentBlock = null;

		for (var i = 0; i < lines.Length; i++)
		{
			var line = lines[i];
			if (line.Trim() == "$")
			{
				break;
			}

			if (LooksLikeObjectStart(lines, i))
			{
				if (currentBlock is not null)
				{
					TryParseBlock(file, currentBlock, items, failures);
				}

				currentBlock = [line];
				continue;
			}

			if (currentBlock is not null)
			{
				currentBlock.Add(line);
			}
		}

		if (currentBlock is not null)
		{
			TryParseBlock(file, currentBlock, items, failures);
		}
	}

	private void TryParseBlock(string file, IReadOnlyList<string> block, List<RpiItemRecord> items, List<RpiItemBlockFailure> failures)
	{
		var zone = GetZoneNumber(file);
		try
		{
			items.Add(ParseBlock(file, zone, block));
		}
		catch (Exception ex)
		{
			failures.Add(new RpiItemBlockFailure(file, zone, block.FirstOrDefault(), ex.Message));
		}
	}

	private RpiItemRecord ParseBlock(string file, int zone, IReadOnlyList<string> block)
	{
		if (block.Count < 7)
		{
			throw new InvalidOperationException("Object block is truncated.");
		}

		var warnings = new List<RpiItemParseWarning>();
		var index = 0;
		var header = block[index++];
		var vnum = int.Parse(header[1..], CultureInfo.InvariantCulture);

		var rawName = ReadTildeString(block, ref index, warnings, "name");
		var shortDescription = ReadTildeString(block, ref index, warnings, "short description");
		var longDescription = ReadTildeString(block, ref index, warnings, "long description");
		var fullDescription = ShouldTreatNextLineAsHeader(block, index)
			? longDescription
			: ReadTildeString(block, ref index, warnings, "full description");

		var nameKeywords = rawName
			.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			.ToList();

		var headerValues = ReadIntLine(block, ref index, "header values");
		if (headerValues.Count < 3)
		{
			throw new InvalidOperationException($"Object #{vnum} is missing core numeric values.");
		}

		var valueLine = ReadIntLine(block, ref index, "oval values");
		if (valueLine.Count < 4)
		{
			throw new InvalidOperationException($"Object #{vnum} is missing oval 0-3 values.");
		}

		var weightLine = ReadDoubleLine(block, ref index, "weight line");
		if (weightLine.Count < 2)
		{
			throw new InvalidOperationException($"Object #{vnum} is missing weight and silver values.");
		}

		var stateLine = ReadIntLine(block, ref index, "state line");
		if (stateLine.Count < 1)
		{
			throw new InvalidOperationException($"Object #{vnum} is missing oval 4 / state values.");
		}

		var itemType = Enum.IsDefined(typeof(RPIItemType), headerValues[0]) ? (RPIItemType)headerValues[0] : RPIItemType.Undefined;
		var extraBits = (RPIExtraBits)headerValues[1];
		var wearBits = (RPIWearBits)headerValues[2];

		var oval0 = valueLine[0];
		var oval1 = valueLine[1];
		var oval2 = valueLine[2];
		var oval3 = valueLine[3];
		var oval4 = stateLine[0];
		var roomPosition = stateLine.Count > 1 ? stateLine[1] : 0;

		string? descKeys = null;
		string? inkColour = null;

		var remainingStateValues = new Queue<int>(stateLine.Skip(Math.Min(2, stateLine.Count)));
		if (itemType == RPIItemType.Ink)
		{
			if (ShouldReadOptionalText(block, index))
			{
				inkColour = ReadTildeString(block, ref index, warnings, "ink colour");
			}
			else
			{
				warnings.Add(new RpiItemParseWarning("missing-ink-colour", "Ink item did not include a colour string."));
			}
		}
		else if (MayHaveDescKeys(itemType, extraBits))
		{
			if (ShouldReadOptionalText(block, index))
			{
				descKeys = ReadTildeString(block, ref index, warnings, "desc_keys");
			}
		}

		while (remainingStateValues.Count < 5 && index < block.Count && TryParseIntTokens(block[index], out var moreInts))
		{
			index++;
			foreach (var value in moreInts)
			{
				remainingStateValues.Enqueue(value);
			}
		}

		var oval5 = DequeueOrDefault(remainingStateValues, 0);
		var activation = DequeueOrDefault(remainingStateValues, 0);
		var quality = DequeueOrDefault(remainingStateValues, 30);
		var rawEconFlags = DequeueOrDefault(remainingStateValues, 0);
		var size = DequeueOrDefault(remainingStateValues, 0);
		var count = DequeueOrDefault(remainingStateValues, 1);

		var tailValues = ReadDoubleLine(block, ref index, "farthings tail");
		while (tailValues.Count < 7)
		{
			tailValues.Add(0.0);
		}

		var farthings = tailValues[0];
		var clock = (int)Math.Round(tailValues[1]);
		var morphTo = (int)Math.Round(tailValues[2]);
		var itemWear = (int)Math.Round(tailValues[3]);
		var rawMaterialValue = (int)Math.Round(tailValues[4]);
		var tailReserved0 = (int)Math.Round(tailValues[5]);
		var tailReserved1 = (int)Math.Round(tailValues[6]);

		List<RpiExtraDescriptionRecord> extraDescriptions = [];
		List<RpiAffectRecord> affects = [];
		List<RpiClanRecord> clans = [];
		List<RpiPoisonRecord> poisons = [];

		while (index < block.Count)
		{
			var marker = block[index].Trim();
			if (marker.Length == 0)
			{
				index++;
				continue;
			}

			switch (marker)
			{
				case "E":
					index++;
					extraDescriptions.Add(new RpiExtraDescriptionRecord(
						ReadTildeString(block, ref index, warnings, "extra description keyword"),
						ReadTildeString(block, ref index, warnings, "extra description text")));
					continue;
				case "A":
					index++;
					var affectValues = ReadIntLine(block, ref index, "affect line");
					if (affectValues.Count >= 2)
					{
						affects.Add(new RpiAffectRecord(affectValues[0], affectValues[1]));
					}
					else
					{
						warnings.Add(new RpiItemParseWarning("invalid-affect", "Affect row did not contain location and modifier."));
					}
					continue;
				case "C":
					index++;
					clans.Add(new RpiClanRecord(
						ReadTildeString(block, ref index, warnings, "clan name"),
						ReadTildeString(block, ref index, warnings, "clan rank")));
					continue;
				case "P":
					index++;
					var poisonValues = ReadIntLine(block, ref index, "poison line");
					if (poisonValues.Count >= 12)
					{
						poisons.Add(new RpiPoisonRecord(
							poisonValues[0],
							poisonValues[1],
							poisonValues[2],
							poisonValues[3],
							poisonValues[4],
							poisonValues[5],
							poisonValues[6],
							poisonValues[7],
							poisonValues[8],
							poisonValues[9],
							poisonValues[10],
							poisonValues[11]));
					}
					else
					{
						warnings.Add(new RpiItemParseWarning("invalid-poison", "Poison row did not contain 12 numeric values."));
					}
					continue;
			}

			break;
		}

		var legacyTrailer = block.Skip(index).ToList();
		if (legacyTrailer.Count > 0)
		{
			warnings.Add(new RpiItemParseWarning(
				"legacy-trailer",
				$"Object block contained {legacyTrailer.Count} unparsed trailer line(s)."));
		}

		var rawOvals = new RpiRawOvalValues(oval0, oval1, oval2, oval3, oval4, oval5);
		var inferredMaterial = InferMaterial(nameKeywords, rawMaterialValue);

		return new RpiItemRecord
		{
			Vnum = vnum,
			SourceFile = file,
			Zone = zone,
			RawName = rawName,
			NameKeywords = nameKeywords,
			ShortDescription = shortDescription,
			LongDescription = longDescription,
			FullDescription = fullDescription,
			ItemType = itemType,
			ExtraBits = extraBits,
			WearBits = wearBits,
			RawOvals = rawOvals,
			RawStateValues = stateLine,
			RawTailValues = tailValues,
			Weight = (int)Math.Round(weightLine[0]),
			SilverValue = weightLine[1],
			RoomPosition = roomPosition,
			Activation = activation,
			Quality = quality,
			RawEconFlags = rawEconFlags,
			Size = size,
			Count = Math.Max(1, count),
			NumericTail = new RpiNumericTail(farthings, clock, morphTo, itemWear, rawMaterialValue, tailReserved0, tailReserved1),
			InferredMaterial = inferredMaterial,
			QualityKeyword = nameKeywords.FirstOrDefault(x => x.EndsWith("_QUALITY", StringComparison.OrdinalIgnoreCase)),
			DescKeys = descKeys,
			InkColour = inkColour,
			ExtraDescriptions = extraDescriptions,
			Affects = affects,
			Clans = clans,
			Poisons = poisons,
			LegacyTrailerLines = legacyTrailer,
			ParseWarnings = warnings,
			WeaponData = CreateWeaponData(itemType, rawOvals),
			ArmourData = CreateArmourData(itemType, rawOvals),
			ContainerData = CreateContainerData(itemType, extraBits, rawOvals),
			LightData = CreateLightData(itemType, rawOvals),
			DrinkContainerData = CreateDrinkContainerData(itemType, rawOvals),
			FoodData = CreateFoodData(itemType, rawOvals),
			RepairKitData = CreateRepairKitData(itemType, rawOvals),
			KeyData = CreateKeyData(itemType, rawOvals),
			WritingData = CreateWritingData(itemType, rawOvals),
			AmmoData = CreateAmmoData(itemType, rawOvals),
		};
	}

	private static RpiWeaponData? CreateWeaponData(RPIItemType itemType, RpiRawOvalValues rawOvals)
	{
		if (itemType != RPIItemType.Weapon)
		{
			return null;
		}

		var useSkill = Enum.IsDefined(typeof(RPISkill), rawOvals.Oval3) ? (RPISkill)rawOvals.Oval3 : RPISkill.None;
		var bowType = rawOvals.Oval1 switch
		{
			0 => "War Bow",
			1 => "Hunting Bow",
			2 => "Sling",
			_ => null,
		};
		var isRangedWeapon = useSkill is RPISkill.Shortbow or RPISkill.Longbow or RPISkill.Sling or RPISkill.Crossbow or RPISkill.Blowgun;

		return new RpiWeaponData(
			rawOvals.Oval0,
			rawOvals.Oval1,
			rawOvals.Oval2,
			useSkill,
			rawOvals.Oval4,
			rawOvals.Oval5,
			isRangedWeapon,
			bowType,
			rawOvals.Oval2,
			rawOvals.Oval4,
			rawOvals.Oval5);
	}

	private static RpiArmourData? CreateArmourData(RPIItemType itemType, RpiRawOvalValues rawOvals)
	{
		if (itemType != RPIItemType.Armor)
		{
			return null;
		}

		var family = rawOvals.Oval1 switch
		{
			0 => "Fur",
			1 => "Quilted",
			2 => "Leather",
			3 => "Scale",
			4 => "Mail",
			5 => "Plate",
			_ => null,
		};

		return new RpiArmourData(rawOvals.Oval0, rawOvals.Oval1, family);
	}

	private static RpiContainerData? CreateContainerData(RPIItemType itemType, RPIExtraBits extraBits, RpiRawOvalValues rawOvals)
	{
		return itemType switch
		{
			RPIItemType.Container or RPIItemType.Quiver or RPIItemType.Sheath or RPIItemType.Keyring => new RpiContainerData(
				rawOvals.Oval0,
				rawOvals.Oval1,
				rawOvals.Oval2,
				rawOvals.Oval3,
				rawOvals.Oval4,
				rawOvals.Oval5,
				extraBits.HasFlag(RPIExtraBits.Table)),
			_ => null,
		};
	}

	private static RpiLightData? CreateLightData(RPIItemType itemType, RpiRawOvalValues rawOvals)
	{
		return itemType == RPIItemType.Light
			? new RpiLightData(rawOvals.Oval0, rawOvals.Oval1, rawOvals.Oval2, rawOvals.Oval3 != 0)
			: null;
	}

	private static RpiDrinkContainerData? CreateDrinkContainerData(RPIItemType itemType, RpiRawOvalValues rawOvals)
	{
		return itemType switch
		{
			RPIItemType.Liquid_container => new RpiDrinkContainerData(rawOvals.Oval0, rawOvals.Oval1, rawOvals.Oval2, false),
			RPIItemType.Fountain => new RpiDrinkContainerData(rawOvals.Oval0, rawOvals.Oval1, rawOvals.Oval2, rawOvals.Oval0 < 0 || rawOvals.Oval1 < 0),
			_ => null,
		};
	}

	private static RpiFoodData? CreateFoodData(RPIItemType itemType, RpiRawOvalValues rawOvals)
	{
		return itemType == RPIItemType.Food
			? new RpiFoodData(rawOvals.Oval0, rawOvals.Oval1, rawOvals.Oval2, rawOvals.Oval3, rawOvals.Oval4, rawOvals.Oval5)
			: null;
	}

	private static RpiRepairKitData? CreateRepairKitData(RPIItemType itemType, RpiRawOvalValues rawOvals)
	{
		return itemType == RPIItemType.Repair || itemType == RPIItemType.Lockpick
			? new RpiRepairKitData(rawOvals.Oval0, rawOvals.Oval1, rawOvals.Oval2, rawOvals.Oval3, rawOvals.Oval5)
			: null;
	}

	private static RpiKeyData? CreateKeyData(RPIItemType itemType, RpiRawOvalValues rawOvals)
	{
		return itemType == RPIItemType.Key
			? new RpiKeyData(rawOvals.Oval0, rawOvals.Oval1)
			: null;
	}

	private static RpiWritingData? CreateWritingData(RPIItemType itemType, RpiRawOvalValues rawOvals)
	{
		return itemType switch
		{
			RPIItemType.Parchment => new RpiWritingData(Math.Max(1, rawOvals.Oval0), rawOvals.Oval1, false),
			RPIItemType.Book => new RpiWritingData(Math.Max(1, rawOvals.Oval0), rawOvals.Oval1, true),
			_ => null,
		};
	}

	private static RpiAmmoData? CreateAmmoData(RPIItemType itemType, RpiRawOvalValues rawOvals)
	{
		return itemType switch
		{
			RPIItemType.Missile => new RpiAmmoData(rawOvals.Oval0, rawOvals.Oval1, false),
			RPIItemType.Bullet => new RpiAmmoData(rawOvals.Oval0, rawOvals.Oval1, true),
			_ => null,
		};
	}

	private static bool MayHaveDescKeys(RPIItemType itemType, RPIExtraBits extraBits)
	{
		if (itemType == RPIItemType.Tossable)
		{
			return true;
		}

		return extraBits.HasFlag(RPIExtraBits.Mask) &&
		       (itemType == RPIItemType.Worn || itemType == RPIItemType.Armor || itemType == RPIItemType.Container);
	}

	private static bool ShouldReadOptionalText(IReadOnlyList<string> block, int index)
	{
		if (index >= block.Count)
		{
			return false;
		}

		var candidate = block[index].Trim();
		if (BlockStartRegex.IsMatch(candidate) || candidate == "$")
		{
			return false;
		}

		if (KnownTailMarkers.Contains(candidate))
		{
			return false;
		}

		if (TryParseIntTokens(block[index], out _))
		{
			return false;
		}

		return !TryParseDoubleTokens(block[index], out _);
	}

	private static string ReadTildeString(IReadOnlyList<string> block, ref int index, List<RpiItemParseWarning> warnings, string description)
	{
		if (index >= block.Count)
		{
			warnings.Add(new RpiItemParseWarning("unexpected-eof", $"Unexpected end of block while reading {description}."));
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

		warnings.Add(new RpiItemParseWarning("unterminated-string", $"Tilde-terminated string for {description} ran to the end of the block."));
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

	private static List<double> ReadDoubleLine(IReadOnlyList<string> block, ref int index, string description)
	{
		if (index >= block.Count)
		{
			throw new InvalidOperationException($"Unexpected end of block while reading {description}.");
		}

		var line = block[index++];
		var values = new List<double>();
		foreach (var token in line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
		{
			if (!double.TryParse(token, NumberStyles.Float | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var parsed))
			{
				throw new InvalidOperationException($"Expected numeric {description}, got '{line}'.");
			}

			values.Add(parsed);
		}

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

	private static bool TryParseDoubleTokens(string line, out List<double> values)
	{
		values = [];
		foreach (var token in line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
		{
			if (!double.TryParse(token, NumberStyles.Float | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var parsed))
			{
				values.Clear();
				return false;
			}

			values.Add(parsed);
		}

		return values.Count > 0;
	}

	private static bool ShouldTreatNextLineAsHeader(IReadOnlyList<string> block, int index)
	{
		if (index >= block.Count)
		{
			return false;
		}

		return TryParseIntTokens(block[index], out var values) && values.Count >= 3;
	}

	private static bool LooksLikeObjectStart(IReadOnlyList<string> lines, int startIndex)
	{
		if (!BlockStartRegex.IsMatch(lines[startIndex]))
		{
			return false;
		}

		var tildeCount = 0;
		for (var i = startIndex + 1; i < lines.Count && i <= startIndex + 40; i++)
		{
			var trimmed = lines[i].Trim();
			if (trimmed == "$")
			{
				return false;
			}

			if (BlockStartRegex.IsMatch(trimmed) && tildeCount < 3)
			{
				return false;
			}

			if (lines[i].Contains('~'))
			{
				tildeCount++;
			}

			if (tildeCount < 3)
			{
				continue;
			}

			for (var j = i + 1; j < lines.Count && j <= i + 4; j++)
			{
				if (string.IsNullOrWhiteSpace(lines[j]))
				{
					continue;
				}

				if (TryParseIntTokens(lines[j], out var values) && values.Count >= 3)
				{
					return true;
				}

				break;
			}
		}

		return false;
	}

	private static int DequeueOrDefault(Queue<int> values, int fallback)
	{
		return values.Count > 0 ? values.Dequeue() : fallback;
	}

	private static int GetZoneNumber(string file)
	{
		var match = ZoneFileRegex.Match(file);
		return match.Success ? int.Parse(match.Groups["zone"].Value, CultureInfo.InvariantCulture) : -1;
	}

	private static RPIMaterial InferMaterial(IEnumerable<string> nameKeywords, int rawMaterialValue)
	{
		foreach (var keyword in nameKeywords)
		{
			var cleaned = keyword.Trim().TrimEnd('~');
			if (Enum.TryParse<RPIMaterial>(cleaned, true, out var parsed))
			{
				return parsed;
			}
		}

		if (rawMaterialValue > 0 && (rawMaterialValue & (rawMaterialValue - 1)) == 0)
		{
			var ordinal = (int)Math.Log(rawMaterialValue, 2.0);
			if (Enum.IsDefined(typeof(RPIMaterial), ordinal))
			{
				return (RPIMaterial)ordinal;
			}
		}

		return RPIMaterial.Other;
	}
}
