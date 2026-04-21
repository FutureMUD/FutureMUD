#nullable enable

using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace RPI_Engine_Worldfile_Converter;

public sealed class RpiNpcWorldfileParser
{
	private const int SkillCount = 150;
	private const int SkillsPerLine = 10;
	private const int SkillLineCount = SkillCount / SkillsPerLine;

	private static readonly Regex HeaderRegex = new(@"^#(?<vnum>\d+)$", RegexOptions.Compiled);
	private static readonly Regex ZoneFileRegex = new(@"mobs\.(?<zone>\d+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
	private static readonly Regex ClanMembershipRegex = new(@"'(?<rank>[^']+)'\s+(?<alias>[A-Za-z0-9_\-]+)", RegexOptions.Compiled);

	public RpiParsedNpcCorpus ParseDirectory(string directory)
	{
		List<RpiNpcRecord> npcs = [];
		List<RpiNpcBlockFailure> failures = [];

		foreach (var file in Directory
			         .EnumerateFiles(directory, "mobs.*", SearchOption.TopDirectoryOnly)
			         .OrderBy(GetZoneNumber)
			         .ThenBy(Path.GetFileName, StringComparer.OrdinalIgnoreCase))
		{
			ParseFile(file, npcs, failures);
		}

		return new RpiParsedNpcCorpus(npcs, failures);
	}

	private void ParseFile(string file, List<RpiNpcRecord> npcs, List<RpiNpcBlockFailure> failures)
	{
		var lines = File.ReadAllLines(file, Encoding.ASCII);
		var zone = GetZoneNumber(file);
		var index = 0;

		while (index < lines.Length)
		{
			var trimmed = lines[index].Trim();
			if (string.IsNullOrWhiteSpace(trimmed))
			{
				index++;
				continue;
			}

			if (trimmed == "$")
			{
				break;
			}

			if (!HeaderRegex.IsMatch(trimmed))
			{
				index++;
				continue;
			}

			var header = trimmed;
			try
			{
				npcs.Add(ParseMob(file, zone, lines, ref index));
			}
			catch (Exception ex)
			{
				failures.Add(new RpiNpcBlockFailure(file, zone, header, ex.Message));
				index = SkipToNextHeader(lines, index + 1);
			}
		}
	}

	private static RpiNpcRecord ParseMob(string file, int zone, IReadOnlyList<string> lines, ref int index)
	{
		var warnings = new List<RpiNpcParseWarning>();
		var vnum = ParseHeader(lines[index++]);
		var keywords = ReadTildeString(lines, ref index, warnings, "keywords");
		var shortDescription = ReadTildeString(lines, ref index, warnings, "short description");
		var longDescription = ReadTildeString(lines, ref index, warnings, "long description");
		var fullDescription = ReadTildeString(lines, ref index, warnings, "full description");

		var primaryTokens = ReadRequiredTokens(lines, ref index, "primary numeric line");
		if (primaryTokens.Count < 15)
		{
			throw new InvalidOperationException($"Mob #{vnum} primary numeric line was truncated.");
		}

		var rawActFlags = ParseLong(primaryTokens[0], "act flags");
		var actFlags = (RpiNpcActFlags)rawActFlags;
		var rawAffectedBy = ParseLong(primaryTokens[1], "affected_by");
		var offense = ParseInt(primaryTokens[2], "offense");
		var legacyRaceId = ParseInt(primaryTokens[3], "legacy race");
		var armour = ParseInt(primaryTokens[4], "armour");
		var hitDiceExpression = primaryTokens[5];
		var damageDiceExpression = primaryTokens[6];
		var birthTimestamp = ParseLong(primaryTokens[7], "birth timestamp");
		var position = ParseInt(primaryTokens[8], "position");
		var defaultPosition = ParseInt(primaryTokens[9], "default position");
		var sexValue = ParseInt(primaryTokens[10], "sex");
		var sex = Enum.IsDefined(typeof(RpiNpcSex), sexValue) ? (RpiNpcSex)sexValue : RpiNpcSex.Neutral;
		var merchSeven = ParseInt(primaryTokens[11], "merch seven");
		var materialsMask = ParseLong(primaryTokens[12], "materials");
		var vehicleType = ParseLong(primaryTokens[13], "vehicle type");
		var buyFlags = ParseLong(primaryTokens[14], "buy flags");

		var secondaryTokens = ReadRequiredTokens(lines, ref index, "secondary numeric line");
		if (secondaryTokens.Count < 16)
		{
			throw new InvalidOperationException($"Mob #{vnum} secondary numeric line was truncated.");
		}

		var skinnedVnum = ParseInt(secondaryTokens[0], "skinned vnum");
		var circle = ParseInt(secondaryTokens[1], "circle");
		var cell1 = ParseInt(secondaryTokens[2], "cell1");
		var carcassVnum = ParseInt(secondaryTokens[3], "carcass vnum");
		var cell2 = ParseInt(secondaryTokens[4], "cell2");
		var ppoints = ParseInt(secondaryTokens[5], "ppoints");
		var naturalDelay = ParseInt(secondaryTokens[6], "natural delay");
		var helmRoom = ParseInt(secondaryTokens[7], "helm room");
		var bodyType = ParseInt(secondaryTokens[8], "body type");
		var poisonType = ParseInt(secondaryTokens[9], "poison type");
		var naturalAttackType = ParseInt(secondaryTokens[10], "natural attack type");
		var accessFlags = ParseInt(secondaryTokens[11], "access flags");
		var heightInches = ParseInt(secondaryTokens[12], "height");
		var frame = ParseInt(secondaryTokens[13], "frame");
		var noAccessFlags = ParseInt(secondaryTokens[14], "no access flags");
		var cell3 = ParseInt(secondaryTokens[15], "cell3");
		var roomPos = secondaryTokens.Count > 16 ? ParseInt(secondaryTokens[16], "room position") : 0;
		var fallback = secondaryTokens.Count > 17 ? ParseInt(secondaryTokens[17], "fallback") : 0;

		var attributeTokens = ReadRequiredTokens(lines, ref index, "attribute line");
		if (attributeTokens.Count < 8)
		{
			throw new InvalidOperationException($"Mob #{vnum} attribute line was truncated.");
		}

		var strength = ParseInt(attributeTokens[0], "strength");
		var intelligence = ParseInt(attributeTokens[1], "intelligence");
		var will = ParseInt(attributeTokens[2], "will");
		var aura = ParseInt(attributeTokens[3], "aura");
		var dexterity = ParseInt(attributeTokens[4], "dexterity");
		var constitution = ParseInt(attributeTokens[5], "constitution");
		var speaksSkillId = ParseInt(attributeTokens[6], "speaks");
		var agility = ParseInt(attributeTokens[7], "agility");

		var flagTokens = ReadRequiredTokens(lines, ref index, "flags line");
		if (flagTokens.Count < 2)
		{
			throw new InvalidOperationException($"Mob #{vnum} flags line was truncated.");
		}

		var rawFlags = ParseLong(flagTokens[0], "mob flags");
		var flags = (RpiNpcFlags)rawFlags;
		var currencyType = ParseInt(flagTokens[1], "currency type");

		RpiNpcShopRecord? shop = null;
		if (flags.HasFlag(RpiNpcFlags.Keeper))
		{
			shop = ParseShop(lines, ref index, merchSeven, vnum);
		}

		var skills = ParseSkills(lines, ref index, speaksSkillId, warnings, vnum);
		var clans = ParseClans(ReadTildeString(lines, ref index, warnings, "clans"));

		RpiNpcVenomRecord? venom = null;
		if (actFlags.HasFlag(RpiNpcActFlags.Venom))
		{
			venom = ParseVenom(lines, ref index, vnum);
		}

		RpiNpcMorphRecord? morph = null;
		if (index < lines.Count && !HeaderRegex.IsMatch(lines[index].Trim()) && lines[index].Trim() != "$")
		{
			var morphTokens = SplitTokens(lines[index]);
			if (morphTokens.Count >= 3 &&
			    morphTokens.Take(3).All(x => int.TryParse(x, NumberStyles.Integer, CultureInfo.InvariantCulture, out _)))
			{
				var clock = ParseInt(morphTokens[0], "morph clock");
				var morphTo = ParseInt(morphTokens[1], "morph target");
				var morphType = ParseInt(morphTokens[2], "morph type");
				if (clock != 0 || morphTo != 0 || morphType != 0)
				{
					morph = new RpiNpcMorphRecord(clock, morphTo, morphType);
				}
				index++;
			}
		}

		return new RpiNpcRecord
		{
			Vnum = vnum,
			SourceFile = file,
			Zone = zone,
			Keywords = keywords,
			ShortDescription = shortDescription,
			LongDescription = longDescription,
			FullDescription = fullDescription,
			RawActFlags = rawActFlags,
			ActFlags = actFlags,
			RawAffectedBy = rawAffectedBy,
			Offense = offense,
			LegacyRaceId = legacyRaceId,
			Armour = armour,
			HitDiceExpression = hitDiceExpression,
			DamageDiceExpression = damageDiceExpression,
			BirthTimestamp = birthTimestamp,
			Position = position,
			DefaultPosition = defaultPosition,
			Sex = sex,
			MerchSeven = merchSeven,
			MaterialsMask = materialsMask,
			VehicleType = vehicleType,
			BuyFlags = buyFlags,
			SkinnedVnum = skinnedVnum,
			Circle = circle,
			Cell1 = cell1,
			CarcassVnum = carcassVnum,
			Cell2 = cell2,
			PPoints = ppoints,
			NaturalDelay = naturalDelay,
			HelmRoom = helmRoom,
			BodyType = bodyType,
			PoisonType = poisonType,
			NaturalAttackType = naturalAttackType,
			AccessFlags = accessFlags,
			HeightInches = heightInches,
			Frame = frame,
			NoAccessFlags = noAccessFlags,
			Cell3 = cell3,
			RoomPos = roomPos,
			Fallback = fallback,
			Strength = strength,
			Intelligence = intelligence,
			Will = will,
			Aura = aura,
			Dexterity = dexterity,
			Constitution = constitution,
			SpeaksSkillId = speaksSkillId,
			Agility = agility,
			RawFlags = rawFlags,
			Flags = flags,
			CurrencyType = currencyType,
			Skills = skills,
			ClanMemberships = clans,
			Shop = shop,
			Venom = venom,
			Morph = morph,
			ParseWarnings = warnings,
		};
	}

	private static RpiNpcShopRecord ParseShop(IReadOnlyList<string> lines, ref int index, int merchSeven, int vnum)
	{
		var topTokens = ReadRequiredTokens(lines, ref index, $"shop header for mob #{vnum}");
		if (topTokens.Count < 7)
		{
			throw new InvalidOperationException($"Shop header for mob #{vnum} was truncated.");
		}

		var secondEconomyLine = ReadRequiredTokens(lines, ref index, $"shop economy for mob #{vnum}");
		List<int> deliveries = [];
		while (index < lines.Count)
		{
			var tokens = SplitTokens(lines[index]);
			if (tokens.Count == 0)
			{
				index++;
				continue;
			}

			index++;
			foreach (var token in tokens)
			{
				var value = ParseInt(token, "delivery vnum");
				if (value == -1)
				{
					goto deliveriesComplete;
				}

				deliveries.Add(value);
			}
		}

	deliveriesComplete:
		var tradeTokens = ReadRequiredTokens(lines, ref index, $"shop trades-in for mob #{vnum}");
		return new RpiNpcShopRecord(
			ParseInt(topTokens[0], "shop vnum"),
			ParseInt(topTokens[1], "store vnum"),
			ParseDouble(topTokens[2], "markup"),
			ParseDouble(topTokens[3], "discount"),
			ParseDouble(topTokens[4], "econ markup1"),
			ParseDouble(topTokens[5], "econ discount1"),
			ParseInt(topTokens[6], "econ flags1"),
			secondEconomyLine.Select(x => x.Trim()).ToList(),
			deliveries,
			tradeTokens.Select(x => ParseInt(x, "trades-in value")).ToList());
	}

	private static IReadOnlyList<RpiNpcSkillRecord> ParseSkills(
		IReadOnlyList<string> lines,
		ref int index,
		int speaksSkillId,
		ICollection<RpiNpcParseWarning> warnings,
		int vnum)
	{
		List<RpiNpcSkillRecord> results = [];
		var skillId = 0;
		for (var line = 0; line < SkillLineCount; line++)
		{
			var tokens = ReadRequiredTokens(lines, ref index, $"skills line {line + 1} for mob #{vnum}");
			if (tokens.Count < SkillsPerLine)
			{
				throw new InvalidOperationException($"Mob #{vnum} skills line {line + 1} was truncated.");
			}

			foreach (var token in tokens.Take(SkillsPerLine))
			{
				var value = ParseInt(token, "skill value");
				if (value > 0)
				{
					results.Add(new RpiNpcSkillRecord(
						skillId,
						RpiNpcSkillCatalog.GetName(skillId),
						value,
						skillId == speaksSkillId));
				}

				skillId++;
			}
		}

		if (speaksSkillId > 0 && results.All(x => x.SkillId != speaksSkillId))
		{
			warnings.Add(new RpiNpcParseWarning(
				"missing-spoken-skill",
				$"Mob #{vnum} references spoken language skill {speaksSkillId}, but it was not present in the skill block."));
		}

		return results;
	}

	private static RpiNpcVenomRecord ParseVenom(IReadOnlyList<string> lines, ref int index, int vnum)
	{
		var tokens = ReadRequiredTokens(lines, ref index, $"venom line for mob #{vnum}");
		if (tokens.Count < 12)
		{
			throw new InvalidOperationException($"Venom line for mob #{vnum} was truncated.");
		}

		return new RpiNpcVenomRecord(
			ParseInt(tokens[0], "venom poison type"),
			ParseInt(tokens[1], "venom duration"),
			ParseInt(tokens[2], "venom latency"),
			ParseInt(tokens[3], "venom minute"),
			ParseInt(tokens[4], "venom max power"),
			ParseInt(tokens[5], "venom level power"),
			ParseInt(tokens[6], "venom atm power"),
			ParseInt(tokens[7], "venom attack"),
			ParseInt(tokens[8], "venom decay"),
			ParseInt(tokens[9], "venom sustain"),
			ParseInt(tokens[10], "venom release"),
			ParseInt(tokens[11], "venom uses"));
	}

	private static IReadOnlyList<RpiNpcClanMembershipRecord> ParseClans(string clanString)
	{
		if (string.IsNullOrWhiteSpace(clanString))
		{
			return Array.Empty<RpiNpcClanMembershipRecord>();
		}

		return ClanMembershipRegex.Matches(clanString)
			.Select(x => new RpiNpcClanMembershipRecord(
				x.Groups["rank"].Value.Trim(),
				x.Groups["alias"].Value.Trim()))
			.ToList();
	}

	private static int ParseHeader(string line)
	{
		var match = HeaderRegex.Match(line.Trim());
		if (!match.Success)
		{
			throw new InvalidOperationException($"Invalid mob header '{line}'.");
		}

		return int.Parse(match.Groups["vnum"].Value, CultureInfo.InvariantCulture);
	}

	private static string ReadTildeString(
		IReadOnlyList<string> lines,
		ref int index,
		ICollection<RpiNpcParseWarning> warnings,
		string fieldName)
	{
		if (index >= lines.Count)
		{
			throw new InvalidOperationException($"Unexpected end of file while reading {fieldName}.");
		}

		var line = lines[index++];
		if (line.EndsWith('~'))
		{
			return line[..^1];
		}

		var builder = new StringBuilder();
		builder.Append(line);
		while (index < lines.Count)
		{
			builder.AppendLine();
			var next = lines[index++];
			if (next.EndsWith('~'))
			{
				builder.Append(next[..^1]);
				return builder.ToString();
			}

			builder.Append(next);
		}

		warnings.Add(new RpiNpcParseWarning("unterminated-tilde-string", $"Reached end of file while reading {fieldName}."));
		return builder.ToString();
	}

	private static IReadOnlyList<string> ReadRequiredTokens(IReadOnlyList<string> lines, ref int index, string fieldName)
	{
		while (index < lines.Count)
		{
			var tokens = SplitTokens(lines[index]);
			index++;
			if (tokens.Count == 0)
			{
				continue;
			}

			return tokens;
		}

		throw new InvalidOperationException($"Unexpected end of file while reading {fieldName}.");
	}

	private static List<string> SplitTokens(string line)
	{
		return line
			.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)
			.ToList();
	}

	private static int SkipToNextHeader(IReadOnlyList<string> lines, int index)
	{
		for (var i = index; i < lines.Count; i++)
		{
			var trimmed = lines[i].Trim();
			if (trimmed == "$" || HeaderRegex.IsMatch(trimmed))
			{
				return i;
			}
		}

		return lines.Count;
	}

	private static int GetZoneNumber(string file)
	{
		var match = ZoneFileRegex.Match(Path.GetFileName(file) ?? string.Empty);
		return match.Success
			? int.Parse(match.Groups["zone"].Value, CultureInfo.InvariantCulture)
			: 0;
	}

	private static int ParseInt(string value, string fieldName)
	{
		if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
		{
			return result;
		}

		throw new InvalidOperationException($"Could not parse {fieldName} value '{value}'.");
	}

	private static long ParseLong(string value, string fieldName)
	{
		if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
		{
			return result;
		}

		throw new InvalidOperationException($"Could not parse {fieldName} value '{value}'.");
	}

	private static double ParseDouble(string value, string fieldName)
	{
		if (double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var result))
		{
			return result;
		}

		throw new InvalidOperationException($"Could not parse {fieldName} value '{value}'.");
	}
}

internal static class RpiNpcSkillCatalog
{
	private static readonly IReadOnlyDictionary<int, string> SkillNames = new Dictionary<int, string>
	{
		[1] = "Brawling",
		[2] = "Small-Blade",
		[3] = "Sword",
		[4] = "Axe",
		[5] = "Polearm",
		[6] = "Club",
		[7] = "Flail",
		[8] = "Double-Handed",
		[9] = "Sole-Wield",
		[10] = "Shield-Use",
		[11] = "Dual-Wield",
		[12] = "Throwing",
		[13] = "Blowgun",
		[14] = "Sling",
		[15] = "Hunting-bow",
		[16] = "Warbow",
		[17] = "Avert",
		[18] = "Defunct",
		[19] = "Sneak",
		[20] = "Hide",
		[21] = "Steal",
		[22] = "Picklock",
		[23] = "Forage",
		[24] = "Barter",
		[25] = "Ride",
		[26] = "Butchery",
		[27] = "Poisoning",
		[28] = "Herbalism",
		[29] = "Clairvoyance",
		[30] = "Danger-Sense",
		[31] = "Empathy",
		[32] = "Hex",
		[33] = "Psychic-Bolt",
		[34] = "Prescience",
		[35] = "Aura-Sight",
		[36] = "Telepathy",
		[37] = "Dodge",
		[38] = "Metalcraft",
		[39] = "Woodcraft",
		[40] = "Lumberjack",
		[41] = "Cookery",
		[42] = "Hideworking",
		[43] = "Brewing",
		[44] = "Literacy",
		[45] = "Apothecary",
		[46] = "Mining",
		[47] = "Tracking",
		[48] = "Healing",
		[49] = "Taliska",
		[50] = "Haladin",
		[51] = "Thrunon",
		[52] = "Beast-Tongue",
		[53] = "Valarin",
		[54] = "Nandorin",
		[55] = "Druag",
		[56] = "Sindarin",
		[57] = "Quenya",
		[58] = "Avarin",
		[59] = "Khuzdul",
		[60] = "Orkish",
		[61] = "Trollish",
		[62] = "Sarati",
		[63] = "Tengwar",
		[64] = "Cirth",
		[65] = "Valarin-Script",
		[66] = "Black-Wise",
		[67] = "Grey-Wise",
		[68] = "White-Wise",
		[69] = "Runecasting",
		[70] = "Astronomy",
		[71] = "Eavesdrop",
	};

	public static string GetName(int skillId)
	{
		return SkillNames.TryGetValue(skillId, out var name) ? name : $"Skill {skillId}";
	}
}
