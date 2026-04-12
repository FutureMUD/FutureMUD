#nullable enable

using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace MudSharp.FutureProg;

public static class ProgVariableTypeRegistry
{
    private sealed record TypeMetadata(
        ProgVariableTypes Type,
        string DisplayName,
        ProgTypeKind ExactKind,
        bool SingleBit,
        IReadOnlyCollection<string> ParseTokens);

    private static readonly Regex ModifierRegex =
        new(@"^(?<base>.+?)\s+(?<modifier>collection dictionary|collectiondictionary|collection|dictionary)$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Dictionary<ProgVariableTypes, TypeMetadata> MetadataByType = new();
    private static readonly Dictionary<ProgTypeKind, ProgVariableTypes> TypeByKind = new();
    private static readonly Dictionary<string, ProgVariableTypes> TypeByToken =
        new(StringComparer.InvariantCultureIgnoreCase);
    private static readonly List<ProgVariableTypes> AllNamedTypes = new();
    private static readonly List<ProgVariableTypes> SingleBitTypes = new();

    internal static readonly BigInteger KnownMask;

    static ProgVariableTypeRegistry()
    {
        RegisterExact(ProgTypeKind.Void, ProgVariableTypes.Void, "Void", "void");
        RegisterExact(ProgTypeKind.Text, ProgVariableTypes.Text, "Text", "text");
        RegisterExact(ProgTypeKind.Number, ProgVariableTypes.Number, "Number", "number");
        RegisterExact(ProgTypeKind.Boolean, ProgVariableTypes.Boolean, "Boolean", "boolean", "bool");
        RegisterExact(ProgTypeKind.Character, ProgVariableTypes.Character, "Character", "character");
        RegisterExact(ProgTypeKind.Location, ProgVariableTypes.Location, "Location", "location", "cell");
        RegisterExact(ProgTypeKind.Item, ProgVariableTypes.Item, "Item", "item");
        RegisterExact(ProgTypeKind.Shard, ProgVariableTypes.Shard, "Shard", "shard");
        RegisterExact(ProgTypeKind.Error, ProgVariableTypes.Error, "Error", "error");
        RegisterExact(ProgTypeKind.Gender, ProgVariableTypes.Gender, "Gender", "gender");
        RegisterExact(ProgTypeKind.Zone, ProgVariableTypes.Zone, "Zone", "zone");
        RegisterNamed(ProgVariableTypes.Collection, "Collection", true, "collection");
        RegisterExact(ProgTypeKind.Race, ProgVariableTypes.Race, "Race", "race");
        RegisterExact(ProgTypeKind.Culture, ProgVariableTypes.Culture, "Culture", "culture");
        RegisterExact(ProgTypeKind.Chargen, ProgVariableTypes.Chargen, "Chargen", "chargen");
        RegisterExact(ProgTypeKind.Trait, ProgVariableTypes.Trait, "Trait", "trait");
        RegisterExact(ProgTypeKind.Clan, ProgVariableTypes.Clan, "Clan", "clan");
        RegisterExact(ProgTypeKind.ClanRank, ProgVariableTypes.ClanRank, "Rank", "rank", "clanrank");
        RegisterExact(ProgTypeKind.ClanAppointment, ProgVariableTypes.ClanAppointment, "Appointment", "appointment", "clanappointment");
        RegisterExact(ProgTypeKind.ClanPaygrade, ProgVariableTypes.ClanPaygrade, "Paygrade", "paygrade", "clanpaygrade");
        RegisterExact(ProgTypeKind.Currency, ProgVariableTypes.Currency, "Currency", "currency");
        RegisterExact(ProgTypeKind.Exit, ProgVariableTypes.Exit, "Exit", "exit");
        RegisterNamed(ProgVariableTypes.Literal, "Literal", true, "literal");
        RegisterExact(ProgTypeKind.DateTime, ProgVariableTypes.DateTime, "DateTime", "datetime");
        RegisterExact(ProgTypeKind.TimeSpan, ProgVariableTypes.TimeSpan, "TimeSpan", "timespan");
        RegisterExact(ProgTypeKind.Language, ProgVariableTypes.Language, "Language", "language");
        RegisterExact(ProgTypeKind.Accent, ProgVariableTypes.Accent, "Accent", "accent");
        RegisterExact(ProgTypeKind.Merit, ProgVariableTypes.Merit, "Merit", "merit");
        RegisterExact(ProgTypeKind.MudDateTime, ProgVariableTypes.MudDateTime, "MudDateTime", "muddatetime", "muddate", "mudtime");
        RegisterExact(ProgTypeKind.Calendar, ProgVariableTypes.Calendar, "Calendar", "calendar");
        RegisterExact(ProgTypeKind.Clock, ProgVariableTypes.Clock, "Clock", "clock");
        RegisterExact(ProgTypeKind.Effect, ProgVariableTypes.Effect, "Effect", "effect");
        RegisterExact(ProgTypeKind.Knowledge, ProgVariableTypes.Knowledge, "Knowledge", "knowledge");
        RegisterExact(ProgTypeKind.Role, ProgVariableTypes.Role, "Role", "role");
        RegisterExact(ProgTypeKind.Ethnicity, ProgVariableTypes.Ethnicity, "Ethnicity", "ethnicity");
        RegisterExact(ProgTypeKind.Drug, ProgVariableTypes.Drug, "Drug", "drug");
        RegisterExact(ProgTypeKind.WeatherEvent, ProgVariableTypes.WeatherEvent, "WeatherEvent", "weatherevent");
        RegisterExact(ProgTypeKind.Shop, ProgVariableTypes.Shop, "Shop", "shop");
        RegisterExact(ProgTypeKind.Merchandise, ProgVariableTypes.Merchandise, "Merchandise", "merchandise", "merch");
        RegisterExact(ProgTypeKind.Outfit, ProgVariableTypes.Outfit, "Outfit", "outfit");
        RegisterExact(ProgTypeKind.OutfitItem, ProgVariableTypes.OutfitItem, "OutfitItem", "outfititem");
        RegisterExact(ProgTypeKind.Project, ProgVariableTypes.Project, "Project", "project");
        RegisterExact(ProgTypeKind.OverlayPackage, ProgVariableTypes.OverlayPackage, "OverlayPackage", "overlaypackage");
        RegisterExact(ProgTypeKind.Terrain, ProgVariableTypes.Terrain, "Terrain", "terrain");
        RegisterExact(ProgTypeKind.Solid, ProgVariableTypes.Solid, "Solid", "solid");
        RegisterExact(ProgTypeKind.Liquid, ProgVariableTypes.Liquid, "Liquid", "liquid");
        RegisterExact(ProgTypeKind.Gas, ProgVariableTypes.Gas, "Gas", "gas");
        RegisterNamed(ProgVariableTypes.Dictionary, "Dictionary", true, "dictionary");
        RegisterNamed(ProgVariableTypes.CollectionDictionary, "CollectionDictionary", true, "collectiondictionary", "collection dictionary");
        RegisterExact(ProgTypeKind.MagicSpell, ProgVariableTypes.MagicSpell, "MagicSpell", "magicspell", "spell");
        RegisterExact(ProgTypeKind.MagicSchool, ProgVariableTypes.MagicSchool, "MagicSchool", "magicschool", "school");
        RegisterExact(ProgTypeKind.MagicCapability, ProgVariableTypes.MagicCapability, "MagicCapability", "magiccapability", "capability");
        RegisterExact(ProgTypeKind.Bank, ProgVariableTypes.Bank, "Bank", "bank");
        RegisterExact(ProgTypeKind.BankAccount, ProgVariableTypes.BankAccount, "BankAccount", "bankaccount");
        RegisterExact(ProgTypeKind.BankAccountType, ProgVariableTypes.BankAccountType, "BankAccountType", "bankaccounttype");
        RegisterExact(ProgTypeKind.LegalAuthority, ProgVariableTypes.LegalAuthority, "LegalAuthority", "legalauthority");
        RegisterExact(ProgTypeKind.Law, ProgVariableTypes.Law, "Law", "law");
        RegisterExact(ProgTypeKind.Crime, ProgVariableTypes.Crime, "Crime", "crime");
        RegisterExact(ProgTypeKind.Market, ProgVariableTypes.Market, "Market", "market");
        RegisterExact(ProgTypeKind.MarketCategory, ProgVariableTypes.MarketCategory, "MarketCategory", "marketcategory");
        RegisterExact(ProgTypeKind.LiquidMixture, ProgVariableTypes.LiquidMixture, "LiquidMixture", "liquidmixture");
		RegisterExact(ProgTypeKind.Script, ProgVariableTypes.Script, "Script", "script");
		RegisterExact(ProgTypeKind.Writing, ProgVariableTypes.Writing, "Writing", "writing");
		RegisterExact(ProgTypeKind.Area, ProgVariableTypes.Area, "Area", "area");
		RegisterExact(ProgTypeKind.LegalClass, ProgVariableTypes.LegalClass, "LegalClass", "legalclass");

        RegisterNamed(ProgVariableTypes.CollectionItem, "CollectionItem", false, "collectionitem");
        RegisterNamed(ProgVariableTypes.Perceivable, "Perceivable", false, "perceivable");
        RegisterNamed(ProgVariableTypes.Perceiver, "Perceiver", false, "perceiver");
        RegisterNamed(ProgVariableTypes.MagicResourceHaver, "MagicResourceHaver", false, "magicresourcehaver", "mrh");
        RegisterNamed(ProgVariableTypes.ReferenceType, "ReferenceType", false, "referencetype");
        RegisterNamed(ProgVariableTypes.ValueType, "ValueType", false, "valuetype");
        RegisterNamed(ProgVariableTypes.Anything, "Anything", false, "anything");
        RegisterNamed(ProgVariableTypes.Toon, "Toon", false, "toon");
        RegisterNamed(ProgVariableTypes.Tagged, "Tagged", false, "tagged");
        RegisterNamed(ProgVariableTypes.Material, "Material", false, "material");

        KnownMask = SingleBitTypes.Aggregate(BigInteger.Zero, (current, item) => current | item.Mask);
    }

    public static string Describe(ProgVariableTypes type)
    {
        if (type == ProgVariableTypes.Anything)
        {
            return "Anything";
        }

        if (type == ProgVariableTypes.Literal)
        {
            return "Literal";
        }

        string containerSuffix = string.Empty;
        ProgVariableTypes underlying = type.WithoutLiteral();
        if (underlying.IsCollection)
        {
            containerSuffix = " Collection";
            underlying ^= ProgVariableTypes.Collection;
        }
        else if (underlying.IsDictionary)
        {
            containerSuffix = " Dictionary";
            underlying ^= ProgVariableTypes.Dictionary;
        }
        else if (underlying.IsCollectionDictionary)
        {
            containerSuffix = " CollectionDictionary";
            underlying ^= ProgVariableTypes.CollectionDictionary;
        }

        if (MetadataByType.TryGetValue(underlying, out TypeMetadata? metadata))
        {
            return $"{metadata.DisplayName}{containerSuffix}";
        }

        List<ProgVariableTypes> flags = underlying.GetFlags().ToList();
        if (!flags.Any())
        {
            return "Unknown Type";
        }

        return $"{string.Join(" ", flags.Select(x => MetadataByType.TryGetValue(x, out TypeMetadata? item) ? item.DisplayName : x.ToStorageString()))}{containerSuffix}";
    }

    public static ProgTypeKind GetExactKind(ProgVariableTypes type)
    {
        return MetadataByType.TryGetValue(type, out TypeMetadata? metadata) ? metadata.ExactKind : ProgTypeKind.Unknown;
    }

    public static IEnumerable<ProgVariableTypes> GetAllFlags(ProgVariableTypes type)
    {
        return AllNamedTypes.Where(type.HasFlag);
    }

    public static IEnumerable<ProgVariableTypes> GetFlags(ProgVariableTypes type)
    {
        return SingleBitTypes.Where(type.HasFlag);
    }

    public static bool TryParse(string? value, out ProgVariableTypes type)
    {
        type = ProgVariableTypes.Error;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        value = value.Trim();
        if (value.StartsWith("v1:", StringComparison.InvariantCultureIgnoreCase))
        {
            return TryParseStorageValue(value[3..], out type);
        }

        if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out long legacy))
        {
            if (legacy < 0)
            {
                return false;
            }

            type = ProgVariableTypes.FromLegacyLong(legacy);
            return true;
        }

        if (ModifierRegex.IsMatch(value))
        {
            Match match = ModifierRegex.Match(value);
            if (!TryParse(match.Groups["base"].Value, out type))
            {
                return false;
            }

            type |= match.Groups["modifier"].Value.ToLowerInvariant() switch
            {
                "collection" => ProgVariableTypes.Collection,
                "dictionary" => ProgVariableTypes.Dictionary,
                "collectiondictionary" => ProgVariableTypes.CollectionDictionary,
                "collection dictionary" => ProgVariableTypes.CollectionDictionary,
                _ => ProgVariableTypes.Void
            };
            return type != ProgVariableTypes.Error;
        }

        if (TypeByToken.TryGetValue(value, out type))
        {
            return true;
        }

        string condensed = value.Replace(" ", string.Empty);
        return TypeByToken.TryGetValue(condensed, out type);
    }

    private static bool TryParseStorageValue(string hex, out ProgVariableTypes type)
    {
        type = ProgVariableTypes.Void;
        hex = hex.Trim();
        if (hex.Length == 0)
        {
            return false;
        }

        if (hex.Length % 2 != 0)
        {
            hex = $"0{hex}";
        }

        try
        {
            byte[] bytes = Convert.FromHexString(hex);
            type = new ProgVariableTypes(new BigInteger(bytes, isUnsigned: true, isBigEndian: true));
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private static void RegisterExact(ProgTypeKind kind, ProgVariableTypes type, string displayName, params string[] parseTokens)
    {
        RegisterNamed(type, displayName, true, parseTokens);
        TypeByKind[kind] = type;
        MetadataByType[type] = MetadataByType[type] with { ExactKind = kind };
    }

    private static void RegisterNamed(ProgVariableTypes type, string displayName, bool singleBit, params string[] parseTokens)
    {
        HashSet<string> tokens = new(StringComparer.InvariantCultureIgnoreCase)
        {
            displayName,
            displayName.Replace(" ", string.Empty),
            displayName.SplitCamelCase()
        };

        foreach (string token in parseTokens)
        {
            tokens.Add(token);
        }

        TypeMetadata metadata = new(type, displayName, ProgTypeKind.Unknown, singleBit, tokens.ToList());
        MetadataByType[type] = metadata;
        AllNamedTypes.Add(type);
        if (singleBit && type != ProgVariableTypes.Void)
        {
            SingleBitTypes.Add(type);
        }

        foreach (string token in tokens)
        {
            TypeByToken[token] = type;
        }
    }
}
