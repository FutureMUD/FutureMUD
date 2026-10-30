#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

namespace DatabaseSeeder.Seeders;

internal sealed class ArmageddonWorldfilePackage
{
	internal const string DataDirectoryEnvironmentVariable = "FUTUREMUD_ARMAGEDDON_DATA_DIRECTORY";
	internal const string PackageFileName = "ArmageddonWorldfiles.json.gz";
	internal const string ManifestFileName = "ArmageddonWorldfiles.manifest.json";
	internal const string SpecialTextFileName = "ArmageddonWorldfileSpecialText.json";
	internal static string MissingDataMessage => $"Armageddon Worldfile requires private source-derived data files. Set {DataDirectoryEnvironmentVariable} to a directory containing {PackageFileName}, {ManifestFileName}, and {SpecialTextFileName}. These files are intentionally not distributed with public builds.";
	private static string? DataDirectory => Environment.GetEnvironmentVariable(DataDirectoryEnvironmentVariable)?.Trim() is { Length: > 0 } path
		? Path.GetFullPath(path) : null;
	internal static bool FileAvailable => DataDirectory is { } directory &&
		new[] { PackageFileName, ManifestFileName, SpecialTextFileName }.All(name => File.Exists(Path.Combine(directory, name)));
	private static readonly Lazy<ArmageddonWorldfilePackage?> Cached = new(TryRead);
	public static ArmageddonWorldfilePackage Load() => Cached.Value ?? TryRead() ?? throw new InvalidOperationException(MissingDataMessage);
	internal static JsonDocument LoadSpecialText()
	{
		if (!FileAvailable) throw new InvalidOperationException(MissingDataMessage);
		return JsonDocument.Parse(File.ReadAllText(Path.Combine(DataDirectory!, SpecialTextFileName)));
	}
	public int Version { get; init; }
	public string Encoding { get; init; } = "";
	public ArmWorldZone[] Zones { get; init; } = [];
	public ArmWorldRoom[] Rooms { get; init; } = [];
	public ArmWorldItem[] Items { get; init; } = [];
	public ArmWorldNpc[] Npcs { get; init; } = [];
	public ArmWorldReset[] Resets { get; init; } = [];
	public ArmWorldRace[] Races { get; init; } = [];
	public ArmWorldGuild[] Guilds { get; init; } = [];
	public ArmWorldGuild[] Subguilds { get; init; } = [];
	public ArmWorldClan[] Clans { get; init; } = [];
	public ArmWorldShop[] Shops { get; init; } = [];
	public ArmWorldForage[] Forage { get; init; } = [];
	public JsonElement[] Crafts { get; init; } = [];
	public string[] Skills { get; init; } = [];
	public ArmWorldPhysical[] PhysicalGenerators { get; init; } = [];
	public JsonElement[] SupportFiles { get; init; } = [];
	public string Fingerprint { get; private set; } = "";
	public JsonElement Manifest { get; private set; }

	internal static readonly HashSet<int> ExcludedEnterables =
	[
		6023, 6028, 6043, 6602, 6605, 20800, 28503, 29800, 50063, 50214, 76030, 76032, 76033, 76034
	];
	internal static readonly HashSet<int> ExcludedPlaygroundZones = [57, 76];
	// Public z76 definitions reused by retained resets, plus three referenced key dependencies.
	internal static readonly HashSet<int> RetainedPlaygroundItems =
	[
		76013, 76018, 76019, 76021, 76025, 76031, 76037, 76044, 76046, 76047, 76048,
		76056, 76057, 76059, 76061, 76065, 76067, 76069, 76070, 76071, 76072, 76081,
		76087, 76088, 76089, 76090, 76092, 76096, 76100, 76101, 76103, 76104, 76105,
		76106, 76107, 76109, 76110, 76112, 76113, 76115, 76116, 76117, 76119, 76120, 76125
	];

	// Keep the qualified archive intact; this is the setting's reviewed import selection.
	internal ArmageddonWorldfilePackage ForInstallation() => new()
	{
		Version = Version, Encoding = Encoding, Fingerprint = Fingerprint, Manifest = Manifest,
		Zones = Zones.Where(x => !ExcludedPlaygroundZones.Contains(x.Index)).ToArray(),
		Rooms = Rooms.Where(x => !ExcludedPlaygroundZones.Contains(x.Zone)).ToArray(),
		Items = Items.Where(x => !ExcludedEnterables.Contains(x.Vnum) &&
			x.Source.Path != "objects/z57.obj" &&
			(x.Source.Path != "objects/z76.obj" || RetainedPlaygroundItems.Contains(x.Vnum))).ToArray(),
		Npcs = Npcs.Where(x => x.Source.Path != "npcs/z57.npc").ToArray(),
		Resets = Resets.Where(x => !ExcludedPlaygroundZones.Contains(x.Zone)).ToArray(),
		Races = Races, Guilds = Guilds, Subguilds = Subguilds, Clans = Clans, Shops = Shops,
		Forage = Forage, Crafts = Crafts, Skills = Skills, PhysicalGenerators = PhysicalGenerators,
		SupportFiles = SupportFiles
	};

	private static ArmageddonWorldfilePackage? TryRead()
	{
		if (!FileAvailable) return null;
		var directory = DataDirectory!;
		var packageBytes = File.ReadAllBytes(Path.Combine(directory, PackageFileName));
		var fingerprint = Convert.ToHexString(SHA256.HashData(packageBytes)).ToLowerInvariant();
		using var bytes = new MemoryStream(packageBytes);
		using var manifest = JsonDocument.Parse(File.ReadAllText(Path.Combine(directory, ManifestFileName)));
		if (manifest.RootElement.GetProperty("PackageSha256").GetString() != fingerprint)
			throw new InvalidOperationException("Armageddon worldfile package and manifest hashes disagree.");
		bytes.Position = 0;
		using var gzip = new GZipStream(bytes, CompressionMode.Decompress);
		var package = JsonSerializer.Deserialize<ArmageddonWorldfilePackage>(gzip)
			?? throw new InvalidOperationException("Armageddon worldfile package is empty.");
		if (package.Version != 1 || package.Encoding != "latin-1" || package.Zones.Length != 81 ||
			package.Rooms.Length != 26137 || package.Items.Length != 7415 || package.Npcs.Length != 2460 ||
			package.Resets.Length != 27682 || package.Forage.Length != 5 || package.Crafts.Length != 1)
			throw new InvalidOperationException("Armageddon package has an unqualified version or corpus inventory.");
		if (package.Rooms.Select(x => x.Vnum).Distinct().Count() != package.Rooms.Length ||
			package.Items.Select(x => x.Vnum).Distinct().Count() != package.Items.Length ||
			package.Npcs.Select(x => x.Vnum).Distinct().Count() != package.Npcs.Length ||
			package.Items.Any(x => x.Flags.Length != 5 || x.Values.Length != 6 || x.Physical.Length != 3 || x.Damage.Length != 4) ||
			package.Npcs.Any(x => x.Flags.Length != 4 || x.Identity.Length != 7 || x.Combat.Length != 6 || x.Money.Length != 2))
			throw new InvalidOperationException("Armageddon package contains duplicate identities or malformed records.");
		package.Fingerprint = fingerprint;
		package.Manifest = manifest.RootElement.Clone();
		return package;
	}
}

internal sealed record ArmWorldSource(string Path, int Line);
internal sealed record ArmWorldExtra(string Keyword, string Description);
internal sealed record ArmWorldExit(int Direction, int Destination, string Keyword, int Flags, int Key, string Description);
internal sealed record ArmWorldZone(ArmWorldSource Source, int Index, int Header, string Name, int[] Fields, string[] Privileges);
internal sealed record ArmWorldRoom(ArmWorldSource Source, int Zone, string Name, int Vnum, int Sector, int Flags,
	int Background, ArmWorldExit[] Exits, ArmWorldExtra[] ExtraDescriptions, JsonElement[] ObservationLinks, int City,
	string[] Programs, string Description);
internal sealed record ArmWorldItem(ArmWorldSource Source, int Vnum, int Limit, string Name, string ShortDescription,
	string LongDescription, string FullDescription, int[] Flags, int[] Values, int[] Physical, int[] Damage,
	int[][] Affects, string[] Programs, ArmWorldExtra[] ExtraDescriptions)
{
	public int ItemType => Flags[0];
	public int Material => Flags[4];
}
internal sealed record ArmWorldNpc(ArmWorldSource Source, int Vnum, int Limit, string Name, string ShortDescription,
	string LongDescription, string FullDescription, string Background, int[] Flags, int[] Combat, int[] Money,
	int[] Identity, int Age, int[][] Skills, JsonElement[] Clans, string[] Programs, ArmWorldExtra[] ExtraDescriptions);
internal sealed record ArmWorldReset(ArmWorldSource Source, int Zone, string Opcode, int[] Arguments, string[] Strings,
	string[] ReferenceIssues);
internal sealed record ArmWorldRace(ArmWorldSource Source, int Index, string Name, string Abbreviation, string[] Stats,
	int[] Combat, string Attack, int[] Identity, JsonElement[] Skins);
internal sealed record ArmWorldGuild(ArmWorldSource Source, int Index, string Name, string Abbreviation, int[] Stats,
	int[] Combat, int Player, JsonElement[] Skills);
internal sealed record ArmWorldClan(ArmWorldSource Source, int Index, string Name, int[] Fields, int[][] Relations, string[] Ranks);
internal sealed record ArmWorldShop(ArmWorldSource Source, int Index, int[] Producing, int[] NewItems,
	double[] BuyMultipliers, double[] SellMultipliers, int[] BuyTypes, int[] SellTypes, string[] Messages,
	int Keeper, int OpenHour, int CloseHour, JsonElement[] Repairs);
internal sealed record ArmWorldForage(ArmWorldSource Source, string Category, string Sector, int Vnum, int Shares, int Rarity);
internal sealed record ArmWorldDistribution(string Expression, int Minimum, int Maximum, double Mean);
internal sealed record ArmWorldPhysical(int Race, ArmWorldDistribution Height, ArmWorldDistribution Weight);
